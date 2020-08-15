using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.NFC.LLCP.ServiceManagers
{
    public enum WelKnownServiceAccessPoints
    {
        LLCLinkManagementService = 0,               // LLC-LM
        ServiceDiscoveryProtocolService = 1,        // SDP
        SimpleNdefExchangeProtocolService = 4,      // SNEP
    }
}
