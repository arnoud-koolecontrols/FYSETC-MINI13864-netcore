using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.NFC.LLCP
{
    public class LLCPLinkActivatedEventArgs : EventArgs
    {
        public bool Connected { get; set; }
    }
}
