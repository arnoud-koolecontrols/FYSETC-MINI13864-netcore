using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.NFC.LLCP.Parameters
{
    public class LLCParameterServiceName : LLCPParameter
    {
        public string ServiceName { get { return GetServiceName(); }  }
        public LLCParameterServiceName(string serviceName)
        {
            this.data = LLCParameterBlockServiceName(serviceName);
        }
        public LLCParameterServiceName(LLCPParameter param) : base(param.Data, 0)
        {

        }
        private string GetServiceName()
        {
            if (this.data.Length >= 3)
            {
                int length = data[1];
                string result = Encoding.UTF8.GetString(data, 2, length);
                return result;
            }
            return "";
        }
        public static byte[] LLCParameterBlockServiceName(string servicename)
        {
            byte[] result = new byte[servicename.Length + 2];
            result[0] = (byte)LLCParameterType.ServiceName;
            result[1] = (byte)servicename.Length; // length
            Encoding.UTF8.GetBytes(servicename).CopyTo(result, 2);
            return result;
        }
    }
}
