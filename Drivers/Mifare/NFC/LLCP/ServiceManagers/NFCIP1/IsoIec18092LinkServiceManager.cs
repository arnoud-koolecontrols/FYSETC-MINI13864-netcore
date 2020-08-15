using Iot.Device.NFC.LLCP.Parameters;
using System;

namespace Iot.Device.NFC.LLCP.ServiceManagers.NFCIP1
{
    public class IsoIec18092LinkServiceManager : ServiceManager, ILinkManager
    {
        public int LinkTimeOut { get; set; } = 100;
        public INfcTransceiver Chip { get; set; }
        public byte TargetNumber { get; set; }
        private Nfcip1 Nfcip1 { get; } = new Nfcip1();

        public IsoIec18092LinkServiceManager() : base(null)
        {
            this.SSAP = (byte)WelKnownServiceAccessPoints.LLCLinkManagementService;
        }

        public bool LinkActivation(LLCPParameters paramsOut, out LLCPParameters paramsIn)
        {
            paramsIn = new LLCPParameters();
            byte[] payload = paramsOut.GetMagicHeader();
            byte[] reply = new byte[0];
            Nfcip1.Reset();
            Console.WriteLine("Sending ATR_REQ");
            if (Nfcip1.Atr_req(Chip, TargetNumber, payload, out reply))
            {
                Console.WriteLine("ATR_RES received checking magic header");
                if (reply.Length > 3)
                {
                    if ((reply[0] == 0x46) && (reply[1] == 0x66) && (reply[2] == 0x6D))
                    {
                        byte[] paramA = new byte[reply.Length - 3];
                        Array.Copy(reply, 3, paramA, 0, paramA.Length);
                        paramsIn = new LLCPParameters(paramA);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool LinkDeActivation()
        {
            if (Nfcip1.Dsl_req(Chip, TargetNumber))
            {
                return true;
            }
            return false;
        }

        public bool Tranceive(byte[] dataToSend, out byte[] dataToReceive)
        {
            if (Nfcip1.Dep_req(Chip, TargetNumber, Nfcip1.PfbTypes.Information, dataToSend, out dataToReceive))
            {
                ResetSendSymmDelay();
                return true;
            }
            return false;
        }

        DateTime DataSentAt { get; set; } = DateTime.MinValue;

        private void ResetSendSymmDelay()
        {
            if (LinkTimeOut <= 100)
            {
                DataSentAt = DateTime.Now.AddMilliseconds(LinkTimeOut - 10); //Perhaps not needed but Send the SYMM 1 unit before timing out
            }
            else
            {
                DataSentAt = DateTime.Now.AddMilliseconds(90);
            }
        } 

        public bool Symm()
        {
            bool result = true;
            if (DataSentAt < DateTime.Now)
            {
                byte[] reply = new byte[0];
                if (! Tranceive(new byte[] { 0, 0 }, out reply))
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
