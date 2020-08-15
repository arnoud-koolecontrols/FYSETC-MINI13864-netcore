using System;

namespace Iot.Device.Nfc.LLCP.Parameters
{
    public class LLCParameterMiux : LLCPParameter
    {
        public int MIUX { get { return GetMIUX(); }  }
        public LLCParameterMiux(int miux)
        {
            this.data = LLCParameterBlockMIUX(miux);
        }
        public LLCParameterMiux(LLCPParameter param) : base(param.Data, 0)
        {

        }
        private int GetMIUX()
        {
            if (this.data.Length >= 4)
            {
                int miux = data[3];
                miux += (data[2] & 0x7) << 8;
                return miux;
            }
            return 128;
        }

        public static byte[] LLCParameterBlockMIUX(int miux)
        {
            if (miux < 128)
            {
                throw new Exception("miux should be atleast 128");
            }
            else
            {
                miux -= 128;
            }
            byte[] result = new byte[4];
            result[0] = (byte)LLCParameterType.MIUX;
            result[1] = 2; // length
            result[2] = (byte)((miux >> 8) & 0x7);
            result[3] = (byte)(miux & 0xFF);
            return result;
        }
    }
}
