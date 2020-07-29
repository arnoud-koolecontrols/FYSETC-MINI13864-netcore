using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.LLCP
{
    public class LLCPParameters
    {
        public enum LLCParameters
        {
            Version = 1,
            MIUX = 2,
            WellKnownServiceList = 3,
            LinkTimOut = 4,
            ReceiveWindowSize = 5,
            ServiceName = 6,
            Option = 7,
            ServiceDiscoveryRequest = 8,
            ServiceDiscoveryResponse = 9,
        }

        public static byte[] LLCParameterBlockVersion(Version version)
        {
            byte[] result = new byte[3];
            result[0] = (byte)LLCParameters.Version;
            result[1] = 1; // length
            result[2] = (byte)(((version.Major & 0xF) << 4) | (version.Minor & 0xF));
            return result;
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
            result[0] = (byte)LLCParameters.MIUX;
            result[1] = 2; // length
            result[2] = (byte)((miux >> 8) & 0x3);
            result[3] = (byte)(miux & 0xFF);
            return result;
        }

        public static byte[] LLCParameterBlockWellKnownServiceList(int value)
        {
            value |= 1; // LLC Link Management Service 
            byte[] result = new byte[4];
            result[0] = (byte)LLCParameters.WellKnownServiceList;
            result[1] = 2; // length
            result[2] = (byte)((value >> 8) & 0xFF);
            result[3] = (byte)(value & 0xFF);
            return result;
        }

        public static byte[] LLCParameterBlockLinkTimOut(int valueInMilliSeconds)
        {
            byte[] result = new byte[3];
            valueInMilliSeconds /= 10;
            result[0] = (byte)LLCParameters.LinkTimOut;
            result[1] = 1; // length
            result[2] = (byte)(valueInMilliSeconds & 0xFF);
            return result;
        }

        public static byte[] LLCParameterBlockReceiveWindowSize(int receiveWindow)
        {
            byte[] result = new byte[3];
            result[0] = (byte)LLCParameters.ReceiveWindowSize;
            result[1] = 1; // length
            result[2] = (byte)(receiveWindow & 0xF);
            return result;
        }

        public static byte[] LLCParameterBlockServiceName(string servicename)
        {
            byte[] result = new byte[servicename.Length + 2];
            result[0] = (byte)LLCParameters.ServiceName;
            result[1] = (byte)servicename.Length; // length
            Encoding.UTF8.GetBytes(servicename).CopyTo(result, 2);
            return result;
        }
        public static byte[] LLCParameterBlockOption(LinkServiceClass lsc)
        {
            byte[] result = new byte[3];
            result[0] = (byte)LLCParameters.ServiceName;
            result[1] = 1; // length
            result[2] = (byte)lsc;
            return result;
        }

        public static byte[] LLCParameterBlockServiceDiscoveryRequest(byte tid, string servicename)
        {
            byte[] result = new byte[servicename.Length + 3];
            result[0] = (byte)LLCParameters.ServiceDiscoveryRequest;
            result[1] = (byte)(servicename.Length + 1); // length
            result[2] = tid;
            Encoding.UTF8.GetBytes(servicename).CopyTo(result, 3);
            return result;
        }

        public static byte[] LLCParameterBlockServiceDiscoveryResponse(byte tid, byte serviceAccessPoint)
        {
            byte[] result = new byte[4];
            result[0] = (byte)LLCParameters.ServiceDiscoveryResponse;
            result[1] = 2; // length
            result[2] = tid;
            result[2] = (byte)(serviceAccessPoint & 0x3F);
            return result;
        }

    }
}
