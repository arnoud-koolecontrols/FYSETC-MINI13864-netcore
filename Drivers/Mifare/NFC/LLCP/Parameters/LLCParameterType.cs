using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.NFC.LLCP.Parameters
{
    public enum LLCParameterType
    {
        Unknown = 0,
        Version = 1,
        MIUX = 2,
        WellKnownServiceList = 3,
        LinkTimeOut = 4,
        ReceiveWindowSize = 5,
        ServiceName = 6,
        Option = 7,
        ServiceDiscoveryRequest = 8,
        ServiceDiscoveryResponse = 9,
    }
}
