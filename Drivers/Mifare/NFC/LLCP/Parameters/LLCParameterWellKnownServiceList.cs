using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.LLCP.Parameters
{
    public class LLCParameterWellKnownServiceList : LLCPParameter
    {
        public int WellKnownServiceList { get { return GetWellKnownServiceList(); }  }
        public LLCParameterWellKnownServiceList(int wellKnownServiceList)
        {
            this.data = LLCParameterBlockWellKnownServiceList(wellKnownServiceList);
        }
        public LLCParameterWellKnownServiceList(LLCPParameter param) : base(param.Data, 0)
        {

        }
        private int GetWellKnownServiceList()
        {
            if (this.data.Length >= 4)
            {
                int wellKnownServiceList = data[3];
                wellKnownServiceList += data[2] << 8;
                return wellKnownServiceList;
            }
            return 1;
        }

        public static byte[] LLCParameterBlockWellKnownServiceList(int wellKnownServiceList)
        {
            wellKnownServiceList |= 1; // LLC Link Management Service 
            byte[] result = new byte[4];
            result[0] = (byte)LLCParameterType.WellKnownServiceList;
            result[1] = 2; // length
            result[2] = (byte)((wellKnownServiceList >> 8) & 0xFF);
            result[3] = (byte)(wellKnownServiceList & 0xFF);
            return result;
        }
    }
}
