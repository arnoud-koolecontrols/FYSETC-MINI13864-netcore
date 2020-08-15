
namespace Iot.Device.Nfc.LLCP.Parameters
{
    public class LLCParameterServiceDiscoveryResponse : LLCPParameter
    {
        public byte TID { get { return GetTid(); }  }
        public byte serviceAccessPoint { get { return GetserviceAccessPoint();  } }

        public LLCParameterServiceDiscoveryResponse(byte tid, byte serviceAccessPoint)
        {
            this.data = LLCParameterBlockServiceDiscoveryResponse(tid, serviceAccessPoint);
        }
        public LLCParameterServiceDiscoveryResponse(LLCPParameter param) : base(param.Data, 0)
        {

        }
        private byte GetserviceAccessPoint()
        {
            if (this.data.Length >= 4)
            {
                return (byte)(data[3] & 0x3F);
            }
            return 0;
        }

        private byte GetTid()
        {
            if (this.data.Length >= 3)
            {
                return data[2];
            }
            return 0;
        }
        public static byte[] LLCParameterBlockServiceDiscoveryResponse(byte tid, byte serviceAccessPoint)
        {
            byte[] result = new byte[4];
            result[0] = (byte)LLCParameterType.ServiceDiscoveryResponse;
            result[1] = 2; // length
            result[2] = tid;
            result[3] = (byte)(serviceAccessPoint & 0x3F);
            return result;
        }
    }
}
