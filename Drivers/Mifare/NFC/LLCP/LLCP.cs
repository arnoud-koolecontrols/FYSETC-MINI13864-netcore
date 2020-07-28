using Iot.Device.Rfid;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;

namespace myApp.Drivers.Mifare.NFC.LLCP
{
    public class LLCPServiceManager
    {
        /// <summary>
        /// Address Field DSAP: Destination Service Access Point
        /// 00h – 0Fh   Identifies the Well-Known Service Access Points
        /// 10h – 1Fh   Identifies Services in the local service environment and are advertised by local SDP
        /// 20h – 3Fh   Identifies Services in the local service environment and are NOT advertised by local SDP
        /// </summary>
        public byte DSAP { get; set; } = 0;
        /// <summary>
        /// Address Field SSAP: Source Service Access Point
        /// 00h – 0Fh   Identifies the Well-Known Service Access Points
        /// 10h – 1Fh   Identifies Services in the local service environment and are advertised by local SDP
        /// 20h – 3Fh   Identifies Services in the local service environment and are NOT advertised by local SDP
        /// </summary>
        public byte SSAP { get; set; } = 0;
        /// <summary>
        /// The maximum information unit (MIU) is the maximum number of octets in the information field of 
        ///   an LLC PDU that the local LLC is able to receive. 
        /// The default MIU is 128.
        /// The MIUX parameter MAY be transmitted in the information field of a CONNECT or CC PDU 
        ///   to announce the local LLC’s larger MIU for that data link connection endpoint
        /// </summary>
        public ushort MIUX { get; set; } = 128;
        /// <summary>
        /// The RW parameter SHALL be encoded as a 4-bit unsigned integer value indicating the receive window size.
        /// The receive window size SHALL be in the inclusive range of values between 0 and 15.
        ///     NOTE A receive window size of zero indicates that the local LLC will not accept I PDUs on that data link connection.
        ///     A receive window size of one indicates that the local LLC will acknowledge every I PDU before accepting additional I PDUs.
        /// </summary>
        public byte RW { get; set; } = 1;
        /// <summary>
        /// A servicename which can be looked up in the ServiceDiscoveryService
        /// </summary>
        public string ServiceName { get; set; } = "";
    }

    public class LLCP
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

        /// <summary>
        /// 
        /// </summary>
        public enum LinkServiceClass
        {
            Unknown = 0,
            /// <summary>
            /// Devices offering only the Connectionless transport service
            /// </summary>
            Class1,
            /// <summary>
            /// Devices offering only the Connection-oriented transport service
            /// </summary>
            Class2,
            /// <summary>
            /// Devices offering both Connectionless and Connection-oriented transport services
            /// </summary>
            Class3
        }

        public enum PTYPES
        {
            SYMM = 0,
            PAX,
            AGF,
            UI,
            CONNECT,
            DISC,
            CC,
            DM,
            FRMR,
            I = 12,
            RR,
            RNR,
        }

        public enum WelKnownServiceAccessPoints
        {
            LLCLinkManagementService = 0,               // LLC-LM
            ServiceDiscoveryProtocolService = 1,        // SDP
            SimpleNdefExchangeProtocolService = 4,      // SNEP
        }

        public Data106kbpsTypeA Iso14443Device { get; private set; } = null;
        public ILLCP Chip { get; private set; } = null;
        /// <summary>
        /// The LTO parameter SHALL specify the maximum time interval between the last received bit of an 
        ///   LLC PDU transmission from the remote to the local LLC and the first bit of the subsequent LLC 
        ///   PDU transmission from the local to the remote LLC
        /// the default link timeout value is 100 milliseconds
        /// </summary>
        public int LinkTimeOut { get; set; } = 100;
        /// <summary>
        /// LLCP protocol version
        /// </summary>
        public Version Version { get; private set; } = new Version(1, 1);

        public LinkServiceClass LSC { get; private set; } = LinkServiceClass.Class3;
        public LLCP(ILLCP chip, Data106kbpsTypeA iso14443Device)
        {
            Chip = chip;
            Iso14443Device = iso14443Device;

        }

        private byte[] LLCParameterBlockVersion(Version version)
        {
            byte[] result = new byte[3];
            result[0] = (byte)LLCParameters.Version;
            result[1] = 1; // length
            result[2] = (byte)(((version.Major & 0xF) << 4) | (version.Minor & 0xF));
            return result;
        }

        private byte[] LLCParameterBlockMIUX(int miux)
        {
            if (miux < 128)
            {
                throw new Exception("miux should be atleast 128");
            } else
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

        private byte[] LLCParameterBlockWellKnownServiceList(int value)
        {
            value |= 1; // LLC Link Management Service 
            byte[] result = new byte[4];
            result[0] = (byte) LLCParameters.WellKnownServiceList;
            result[1] = 2; // length
            result[2] = (byte) ((value >> 8) & 0xFF);
            result[3] = (byte) (value & 0xFF);
            return result;
        }

        private byte[] LLCParameterBlockLinkTimOut(int valueInMilliSeconds)
        {
            byte[] result = new byte[3];
            valueInMilliSeconds /= 10;
            result[0] = (byte)LLCParameters.LinkTimOut;
            result[1] = 1; // length
            result[2] = (byte)(valueInMilliSeconds & 0xFF);
            return result;
        }

        private byte[] LLCParameterBlockReceiveWindowSize(int receiveWindow)
        {
            byte[] result = new byte[3];
            result[0] = (byte)LLCParameters.ReceiveWindowSize;
            result[1] = 1; // length
            result[2] = (byte)(receiveWindow & 0xF);
            return result;
        }

        private byte[] LLCParameterBlockServiceName(string servicename)
        {
            byte[] result = new byte[servicename.Length + 2];
            result[0] = (byte)LLCParameters.ServiceName;
            result[1] = (byte)servicename.Length; // length
            Encoding.UTF8.GetBytes(servicename).CopyTo(result, 2);
            return result;
        }
        private byte[] LLCParameterBlockOption(LinkServiceClass lsc)
        {
            byte[] result = new byte[3];
            result[0] = (byte)LLCParameters.ServiceName;
            result[1] = 1; // length
            result[2] = (byte)lsc;
            return result;
        }

        private byte[] LLCParameterBlockServiceDiscoveryRequest(byte tid, string servicename)
        {
            byte[] result = new byte[servicename.Length + 3];
            result[0] = (byte)LLCParameters.ServiceDiscoveryRequest;
            result[1] = (byte)(servicename.Length + 1); // length
            result[2] = tid;
            Encoding.UTF8.GetBytes(servicename).CopyTo(result, 3);
            return result;
        }

        private byte[] LLCParameterBlockServiceDiscoveryResponse(byte tid, byte serviceAccessPoint)
        {
            byte[] result = new byte[4];
            result[0] = (byte)LLCParameters.ServiceDiscoveryResponse;
            result[1] = 2; // length
            result[2] = tid;
            result[2] = (byte)(serviceAccessPoint & 0x3F);
            return result;
        }

        public static byte[] GetFrame(byte dsap, PTYPES pType, byte ssap, byte seq, byte[] payload)
        {
            int length = payload.Length + 2;
            if ((pType == PTYPES.I) || (pType == PTYPES.RR) || (pType == PTYPES.RNR))
            {
                length++;
            }

            byte[] result = new byte[length];
            byte ptype = (byte)pType;
            int index = 0;
            result[index++] = (byte)(((dsap & 0x3F) << 2) | ((ptype >> 2) & 0x03));
            result[index++] = (byte)((ssap & 0x3F) | ((ptype & 0x03) << 6));
            if ((pType == PTYPES.I) || (pType == PTYPES.RR) || (pType == PTYPES.RNR))
            {
                result[index++] = seq;
            }

            payload.CopyTo(result, index);
            return result;
        }
    }
}
