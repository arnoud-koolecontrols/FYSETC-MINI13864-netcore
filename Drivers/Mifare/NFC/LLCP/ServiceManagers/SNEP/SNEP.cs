using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.NFC.LLCP.ServiceManagers.SNEP
{
    public class SNEP
    {
        //byte Version { get; set; } = 0x10; //version 1.0
        
        public static byte[] Put(byte[] payload)
        {
            byte[] result = new byte[payload.Length +6];
            int length = payload.Length;
            result[0] = 0x10;
            result[1] = 0x02; //Put - section 3.1.2, SNEP)
            result[2] = (byte)((length >> 24) & 0xFF);
            result[3] = (byte)((length >> 16) & 0xFF);
            result[4] = (byte)((length >> 8) & 0xFF);
            result[5] = (byte)((length >> 0) & 0xFF);
            payload.CopyTo(result, 6);
            return result;
        }

    }
}
