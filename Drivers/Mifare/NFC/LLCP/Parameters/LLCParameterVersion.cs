using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.NFC.LLCP.Parameters
{
    public class LLCParameterVersion : LLCPParameter
    {
        public Version Version { get { return GetVersion(); }  }
        public LLCParameterVersion(Version version)
        {
            this.data = LLCParameterBlockVersion(version);
        }
        public LLCParameterVersion(LLCPParameter param) : base(param.Data, 0)
        {

        }

        private Version GetVersion()
        {
            if (this.data.Length >= 3)
            {
                int minor = data[2] & 0xF;
                int major = (data[2] >> 4) & 0xF;
                return new Version(major, minor);
            }
            return new Version(0, 0);
        }

        public static byte[] LLCParameterBlockVersion(Version version)
        {
            byte[] result = new byte[3];
            result[0] = (byte)LLCParameterType.Version;
            result[1] = 1; // length
            result[2] = (byte)(((version.Major & 0xF) << 4) | (version.Minor & 0xF));
            return result;
        }
    }
}
