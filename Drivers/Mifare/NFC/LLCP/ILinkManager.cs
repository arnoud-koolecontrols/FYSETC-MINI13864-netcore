using Iot.Device.NFC.LLCP.Parameters;

namespace Iot.Device.NFC.LLCP
{
    public interface ILinkManager
    {

        /// <summary>
        /// The LTO parameter SHALL specify the maximum time interval between the last received bit of an 
        ///   LLC PDU transmission from the remote to the local LLC and the first bit of the subsequent LLC 
        ///   PDU transmission from the local to the remote LLC
        /// the default link timeout value is 100 milliseconds
        /// </summary>
        int LinkTimeOut { get; set; }
        INfcTransceiver Chip { get; set; }
        byte TargetNumber { get; set; }

        bool LinkActivation(LLCPParameters paramsOut, out LLCPParameters paramsIn);
        bool LinkDeActivation();
        bool Tranceive(byte[] dataToSend, out byte[] dataToReceive);
        bool Symm();

    }
}
