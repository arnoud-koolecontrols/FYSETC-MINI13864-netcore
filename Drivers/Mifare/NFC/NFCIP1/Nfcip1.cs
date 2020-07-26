using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using UnitsNet;

namespace myApp.Drivers.Mifare.NFC.NFCIP1
{
   public class Nfcip1
   {
        public enum Types
		{
            REQ = 0xD4,
            RES = 0xD5
        }

        public enum Commands
        {
            ATR_REQ = 0,
            ATR_RES = 1,
            WUP_REQ = 2,
            WUP_RES = 3,
            PSL_REQ = 4,
            PSL_RES = 5,
            DEP_REQ = 6,
            DEP_RES = 7,
            DSL_REQ = 8,
            DSL_RES = 9,
            RLS_REQ = 10,
            RLS_RES = 11,
        }

		public static bool IsNfcipCompliant(byte Sak, byte[] UID)
        {
			if ((Sak & 0x44) == 0x40)
            {
				if (UID.Length >= 4)
                {
					if (UID.First() == 0x08)
                    {
						return true;
                    }
                }
            }
			return false;
        }

		public static byte[] ISO14443aCRC(byte[] data)
		{
			ushort crc = 0x6363;
			foreach (byte value in data)
			{
				byte bt = value;
				bt ^= (byte)(crc & 0xFF);
				bt ^= (byte)(bt << 4);
				crc = (ushort)((crc >> 8) ^ (bt << 8) ^ (bt << 3) ^ (bt >> 4));
			}
			return new byte[] { (byte)((crc >> 8) & 0xFF), (byte)(crc & 0xFF) };
		}

		public static byte[] Fc128AddStartAndCRC(byte[] data)
        {



            //byte[] result = new byte[data.Length + 1];
            //int index = 0;
            //result[index++] = 0xF0;
            //data.CopyTo(result, index);
            //return result;

            //byte[] result = new byte[data.Length + 3];
            //int index = 0;
            //result[index++] = 0xF0;
            //data.CopyTo(result, index);
            //index += data.Length;
            //byte[] crc = ISO14443aCRC(data);
            //crc.CopyTo(result, index);
            //return result;





            byte[] result = new byte[data.Length + 4];
            int index = 0;
            result[index++] = 0xF0;
			result[index++] = (byte)(data.Length + 1);
			data.CopyTo(result, index);
            index += data.Length;
            byte[] crc = ISO14443aCRC(data);
            crc.CopyTo(result, index);
            return result;

            //return data;
        }

		public static byte[] AtrReq(byte[] nfcid, byte[] genBytes)
		{ 
            int length = 16 + genBytes.Length;
            byte[] result = new byte[length];
            result[0] = (byte)Types.REQ;
            result[1] = (byte)Commands.ATR_REQ;
            nfcid.CopyTo(result, 2);
            result[12] = 0; //DID
            result[13] = 0; //BSI
            result[14] = 0; //BRI
            result[15] = 0; //PPI
            if (genBytes.Length > 0)
                result[15] |= 0x02;
            genBytes.CopyTo(result, 16);

            result = Fc128AddStartAndCRC(result);
            return result;
		}

		public static byte[] WupReq(byte[] nfcid)
		{
			int length = 14;
			byte[] result = new byte[length];
			result[0] = (byte)length;
			result[1] = (byte)Types.REQ;
			result[2] = (byte)Commands.WUP_REQ;
			nfcid.CopyTo(result, 3);
			result[13] = 0; //DID

			result = Fc128AddStartAndCRC(result);
			return result;
		}

		public static byte[] PslReq(byte brs, byte fsl)
		{
			int length = 6;
			byte[] result = new byte[length];
			result[0] = (byte)length;
			result[1] = (byte)Types.REQ;
			result[2] = (byte)Commands.PSL_REQ;
			result[3] = 0; //DID
			result[4] = brs; //BRS
			result[5] = fsl; //FSL

			result = Fc128AddStartAndCRC(result);
			return result;
		}

		public static byte[] DepReq(byte[] nfcid, byte pfl, byte[] data)
		{
			int length = 4 + data.Length;
			byte[] result = new byte[length];
			result[0] = (byte)length;
			result[1] = (byte)Types.REQ;
			result[2] = (byte)Commands.DEP_REQ;
			result[3] = pfl; //PFL
			data.CopyTo(result, 4);

			result = Fc128AddStartAndCRC(result);
			return result;
		}

		public static byte[] DslReq()
		{
			int length = 4;
			byte[] result = new byte[length];
			result[0] = (byte)length;
			result[1] = (byte)Types.REQ;
			result[2] = (byte)Commands.DSL_REQ;
			result[3] = 0; //DID

			result = Fc128AddStartAndCRC(result);
			return result;
		}

		public static byte[] RlsReq()
		{
			int length = 4;
			byte[] result = new byte[length];
			result[0] = (byte)length;
			result[1] = (byte)Types.REQ;
			result[2] = (byte)Commands.RLS_REQ;
			result[3] = 0; //DID

			result = Fc128AddStartAndCRC(result);
			return result;
		}
		public static byte[] PolReq()
		{
			int length = 6;
			byte[] result = new byte[length];
			result[0] = (byte)length;
			result[1] = 0;
			result[2] = 0xFF;
			result[3] = 0xFF;
			result[4] = 0;
			result[5] = 0;

			result = Fc128AddStartAndCRC(result);
			return result;
		}
	}
}
