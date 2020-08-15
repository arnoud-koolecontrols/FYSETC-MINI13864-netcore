using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.Nfc.LLCP
{
    public class LLCPLinkActivatedEventArgs : EventArgs
    {
        public bool Connected { get; set; }
    }
}
