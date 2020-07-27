using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Security;
using System.Text;

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
        public static byte[] GetFrame(byte dsap, PTYPES pType, byte ssap, byte seq, byte[] payload)
        {
            byte[] result = new byte[payload.Length + 3];
            byte ptype = (byte)pType;
            result[0] = (byte)(((dsap & 0x3F) << 2) | ((ptype >> 2) & 0x03));
            result[1] = (byte)((ssap & 0x3F) | ((ptype & 0x03) << 6));
            result[2] = seq;
            payload.CopyTo(result, 3);
            return result;
        }
    }
}
