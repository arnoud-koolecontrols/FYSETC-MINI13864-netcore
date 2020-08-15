using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.Nfc.LLCP.ServiceManagers
{
    public enum WelKnownServiceAccessPoints
    {
        LLCLinkManagementService = 0,               // LLC-LM
        ServiceDiscoveryProtocolService = 1,        // SDP
        SimpleNdefExchangeProtocolService = 4,      // SNEP
    }
}
