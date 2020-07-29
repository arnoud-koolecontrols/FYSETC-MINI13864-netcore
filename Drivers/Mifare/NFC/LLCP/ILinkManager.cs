using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.LLCP
{
    public interface ILinkManager
    {
        byte[] NfcId3T_TX { get; }
        byte[] NfcId3T_RX { get; }
        bool LinkActivation(ILLCP chip, Version llcpVersion, int wellKnownServiceList, int linkTimeOut, LinkServiceClass linkServiceClass);
    }
}
