using Iot.Device.Rfid;
using myApp.Drivers.Mifare.NFC.LLCP.ServiceManagers;
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
    public class LLCP
    {
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
        public static Version Version { get; } = new Version(1, 1);
        public static byte[] LLCPMagicNumber { get; } = new byte[] { 0x46, 0x66, 0x6D };
        public LinkServiceClass LSC { get; private set; } = LinkServiceClass.Class3;
        private Dictionary<int, ServiceManager> ServiceManagers { get; set; } = new Dictionary<int, ServiceManager>();
        
        public LLCP(ILLCP chip, Data106kbpsTypeA iso14443Device)
        {
            Chip = chip;
            Iso14443Device = iso14443Device;
            IsoIec18092LinkServiceManager linkManager = new IsoIec18092LinkServiceManager();
            ServiceManagers.Add(linkManager.SSAP, linkManager);
        }

        private int GetSupportedWelKnownServiceList()
        {
            int wks = 0;
            foreach (KeyValuePair<int, ServiceManager> pair in ServiceManagers)
            {
                if (pair.Key < 0x10)
                {
                    wks |= 1 << pair.Key;
                }
            }
            return wks;
        }

        public void Start()
        {
            if (ServiceManagers.ContainsKey(0))
            {
                if (ServiceManagers[0] is ILinkManager)
                {
                    ILinkManager manager = (ILinkManager)ServiceManagers[0];
                    if (manager.LinkActivation( this.Chip,Version, GetSupportedWelKnownServiceList(), LinkTimeOut, LSC ))
                    {

                    }
                    return;
                } 
            }
            throw new Exception("No linkmanager configured");
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
