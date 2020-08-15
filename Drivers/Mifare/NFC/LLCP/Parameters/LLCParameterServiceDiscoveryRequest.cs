using System.Text;

namespace Iot.Device.Nfc.LLCP.Parameters
{
    public class LLCParameterServiceDiscoveryRequest : LLCPParameter
    {
        public string ServiceName { get { return GetServiceName(); }  }
        public byte TID { get { return GetTid(); } }
        public LLCParameterServiceDiscoveryRequest(byte tid, string serviceName)
        {
            this.data = LLCParameterBlockServiceDiscoveryRequest(tid, serviceName);
        }
        public LLCParameterServiceDiscoveryRequest(LLCPParameter param) : base(param.Data, 0)
        {

        }
        private byte GetTid()
        {
            if (this.data.Length >= 3)
            {
                return data[2];
            }
            return 0;
        }

        private string GetServiceName()
        {
            if (this.data.Length >= 3)
            {
                int length = data[1] - 1;
                string result = Encoding.UTF8.GetString(data, 3, length);
                return result;
            }
            return "";
        }
        public static byte[] LLCParameterBlockServiceDiscoveryRequest(byte tid, string servicename)
        {
            byte[] result = new byte[servicename.Length + 3];
            result[0] = (byte)LLCParameterType.ServiceDiscoveryRequest;
            result[1] = (byte)(servicename.Length + 1); // length
            result[2] = tid;
            Encoding.UTF8.GetBytes(servicename).CopyTo(result, 3);
            return result;
        }

    }
}
