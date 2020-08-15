using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.LLCP
{
    public class LLCPLinkActivatedEventArgs : EventArgs
    {
        public bool Connected { get; set; }
    }
}
