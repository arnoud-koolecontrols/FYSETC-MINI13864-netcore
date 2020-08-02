using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.LLCP.Parameters
{
    public class LLCParameterOption : LLCPParameter
    {
        public LinkServiceClass LinkServiceClass { get { return GetLinkServiceClass(); }  }
        public LLCParameterOption(LinkServiceClass lsc)
        {
            this.data = LLCParameterBlockOption(lsc);
        }

        public LLCParameterOption(LLCPParameter param) : base(param.Data, 0)
        {

        }

        private LinkServiceClass GetLinkServiceClass()
        {
            if (this.data.Length >= 3)
            {
                LinkServiceClass result = (LinkServiceClass)(data[2] & 0x3);
                return result;
            }
            return LinkServiceClass.Class3;
        }
        public static byte[] LLCParameterBlockOption(LinkServiceClass lsc)
        {
            byte[] result = new byte[3];
            result[0] = (byte)LLCParameterType.Option;
            result[1] = 1; // length
            result[2] = (byte)lsc;
            return result;
        }
    }
}
