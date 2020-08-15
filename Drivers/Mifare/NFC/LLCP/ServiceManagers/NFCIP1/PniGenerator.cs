using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.NFC.LLCP.ServiceManagers.NFCIP1
{
    public class PniGenerator
    {
        private int pni = 0;

        public byte PNI { get { return GetPni(); } }

        public void Reset()
        {
            lock (this)
            {
                pni = 0;
            }
        }

        public void Increase()
        {
            lock (this)
            {
                pni++;
                if (pni > 3)
                {
                    pni = 0;
                }
            }
        }

        byte GetPni()
        {
            int result = 0;
            lock (this)
            {
                result = pni;
            }
            return (byte)result;
        }
    }
}
