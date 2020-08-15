using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.NFC.LLCP.ServiceManagers.NFCIP1
{
    public class NfcidGenerator
    {
        private byte[] nfcid = new byte[] {
                0x10,
                0x11,
                0x12,
                0x13,
                0x14,
                0x15,
                0x16,
                0x17,
                0x18,
                0x19,
            };
        public byte[] Nfcid { get { return GetNfcid(); } }

        public void Generate()
        {
            lock (this)
            {
                //todo generate new random nfcid
            }
        }

        public byte[] GetNfcid()
        {
            byte[] clone;
            lock (this)
            {
                clone = (byte[])nfcid.Clone();
            }
            return nfcid;
        }

    }
}
