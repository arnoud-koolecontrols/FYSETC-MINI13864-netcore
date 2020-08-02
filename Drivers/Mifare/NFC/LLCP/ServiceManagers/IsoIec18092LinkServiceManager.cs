using myApp.Drivers.Mifare.NFC.LLCP.Parameters;
using myApp.Drivers.Mifare.NFC.NFCIP1;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.LLCP.ServiceManagers
{
    public class IsoIec18092LinkServiceManager : ServiceManager, ILinkManager
    {

        private Nfcip1 Nfcip1 { get; } = new Nfcip1();

        public bool LinkActivation(ITranceiver chip, byte targetNumber, LLCPParameters paramsOut, out LLCPParameters paramsIn)
        {
            paramsIn = new LLCPParameters();
            byte[] payload = paramsOut.GetMagicHeader();
            byte[] reply = new byte[0];
            Nfcip1.Reset();
            Console.WriteLine("Sending ATR_REQ");
            if (Nfcip1.Atr_req(chip, targetNumber, payload, out reply))
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

        public bool LinkDeActivation(ITranceiver chip, byte targetNumber)
        {
            if (Nfcip1.Dsl_req(chip, targetNumber))
            {
                return true;
            }
            return false;
        }

        public bool Symm(ITranceiver chip, byte targetNumber)
        {
            byte[] reply = new byte[0];
            if (Nfcip1.Dep_req(chip, targetNumber,Nfcip1.PfbTypes.Information, new byte[] { 0, 0 }, out reply))
            {
                return true;
            }
            return false;
        }

    }
}
