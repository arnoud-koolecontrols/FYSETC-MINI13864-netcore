using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.LLCP.Parameters
{
    public class LLCParameterLinkTimeOut : LLCPParameter
    {
        public int LinkTimeOut { get { return GetLTO(); }  }
        public LLCParameterLinkTimeOut(int timeoutInMilliSeconds)
        {
            this.data = LLCParameterBlockLinkTimeOut(timeoutInMilliSeconds);
        }

        public LLCParameterLinkTimeOut(LLCPParameter param) : base(param.Data, 0)
        {

        }
        private int GetLTO()
        {
            if (this.data.Length >= 3)
            {
                int lto = data[2] * 10;
                return lto;
            }
            return 100;
        }
        public static byte[] LLCParameterBlockLinkTimeOut(int valueInMilliSeconds)
        {
            byte[] result = new byte[3];
            valueInMilliSeconds /= 10;
            result[0] = (byte)LLCParameterType.LinkTimeOut;
            result[1] = 1; // length
            result[2] = (byte)(valueInMilliSeconds & 0xFF);
            return result;
        }
    }
}
