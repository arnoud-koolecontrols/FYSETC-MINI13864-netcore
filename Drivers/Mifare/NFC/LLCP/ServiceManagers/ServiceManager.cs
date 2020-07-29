using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.LLCP.ServiceManagers
{
    public class ServiceManager
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

        public bool IsConnected { get; protected set; } = false;

    }

}
