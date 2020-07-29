using myApp.Drivers.Mifare.NFC.NFCIP1;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.LLCP.ServiceManagers
{
    public class IsoIec18092LinkServiceManager : ServiceManager, ILinkManager
    {
        public byte[] NfcId3T_TX { get; private set; } = new byte[] {
                0x10,
                0x11,
                0x12,
                0x13,
                0x14,
                0x15,
                0x16,
                0x17,
                0x18,
                0x19,
            };

        public byte[] NfcId3T_RX { get; private set; } = new byte[0];

        public bool LinkActivation(ILLCP chip, Version llcpVersion, int wellKnownServiceList, int linkTimeOut, LinkServiceClass linkServiceClass)
        {
            int length = 0;
            // All PAX fields should be send in the ATR_REQ payload 
            byte[] magicNumber = LLCP.LLCPMagicNumber;
            length += magicNumber.Length;
            // version must be send 
            byte[] version = LLCPParameters.LLCParameterBlockVersion(llcpVersion);
            length += version.Length;
            // well know service-List should be send
            byte[] wks = LLCPParameters.LLCParameterBlockWellKnownServiceList(wellKnownServiceList);
            length += wks.Length;
            // Link timeout may be send
            byte[] lto = LLCPParameters.LLCParameterBlockLinkTimOut(linkTimeOut);
            length += lto.Length;
            // options may be send
            byte[] opt = LLCPParameters.LLCParameterBlockOption(linkServiceClass);
            length += opt.Length;
            byte[] payload = new byte[length];
            int index = 0;
            magicNumber.CopyTo(payload, index);
            index = magicNumber.Length;
            version.CopyTo(version, index);
            index += version.Length;
            wks.CopyTo(version, index);
            index += wks.Length;
            lto.CopyTo(version, index);
            index += lto.Length;
            opt.CopyTo(version, index);
            index += opt.Length;
            //todo: generate a new nfcId3T_TX
            byte[] atr_req = Nfcip1.AtrReq(NfcId3T_TX, payload);
            //todo: send atr_req and handle reply
            throw new NotImplementedException();
        }
    }
}
