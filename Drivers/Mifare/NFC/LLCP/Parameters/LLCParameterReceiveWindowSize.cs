using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.LLCP.Parameters
{
    public class LLCParameterReceiveWindowSize : LLCPParameter
    {
        public int ReceiveWindowSize { get { return GetReceiveWindowSize(); }  }
        public LLCParameterReceiveWindowSize(int receiveWindow)
        {
            this.data = LLCParameterBlockReceiveWindowSize(receiveWindow);
        }
        public LLCParameterReceiveWindowSize(LLCPParameter param) : base(param.Data, 0)
        {

        }
        private int GetReceiveWindowSize()
        {
            if (this.data.Length >= 3)
            {
                int rws = data[2] & 0xF;
                return rws;
            }
            return 1;
        }
        public static byte[] LLCParameterBlockReceiveWindowSize(int receiveWindow)
        {
            byte[] result = new byte[3];
            result[0] = (byte)LLCParameterType.ReceiveWindowSize;
            result[1] = 1; // length
            result[2] = (byte)(receiveWindow & 0xF);
            return result;
        }
    }
}
