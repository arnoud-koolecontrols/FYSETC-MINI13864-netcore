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
		public enum PfbTypes
		{
			Information = 0,
			Protected,
			AckNak,
			Supervisory
		}
		private enum Types
		{
			REQ = 0xD4,
			RES = 0xD5
		}
		private enum Commands
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

		public byte[] Nfcid_TX { get { return NfcidGenerator.Nfcid; } }
		public byte[] Nfcid_RX { get; private set; } = new byte[10];

		public double TimeOutTime { get; private set; } = 100;
		public int MaxBufferSize { get; private set; } = 128;
		private NfcidGenerator NfcidGenerator { get; } = new NfcidGenerator();
		private PniGenerator PniGenerator { get; } = new PniGenerator();
		public Nfcip1()
        {

        }

		public void Reset()
        {
			PniGenerator.Reset();
			Nfcid_RX = new byte[10];
		}

		public bool Atr_req(INfcTranceiver chip, byte targetNumber, byte[] genBytes, out byte[] response)
        {
			response = new byte[0];
			bool result = false;
			byte[] req = AtrReq(NfcidGenerator.Nfcid, genBytes);
			Span<byte> res;
			if (chip.TransmitData(targetNumber, req) >= 0)
			{
				if (chip.ReceiveData(targetNumber, out res, 10) >= 0)
				{
					byte[] reply = res.ToArray();
					if ((reply[2] == (byte)Nfcip1.Types.RES) && (reply[3] == (byte)Nfcip1.Commands.ATR_RES)) //hebben we een reply ontvangen
					{
						Array.Copy(reply, 4, Nfcid_RX, 0, 10);
						TimeOutTime = (reply[17] * 309.8);
						byte ppt = reply[18];
						MaxBufferSize = ((ppt >> 4) + 1) * 64;
						if ((ppt & 0x02) > 0)
						{
							response = new byte[reply.Length - 19];
							Array.Copy(reply, 19, response, 0, response.Length);
						}
						result = true;
					}
				}
			}
			return result;
        }

		public bool Dsl_req(INfcTranceiver chip, byte targetNumber)
        {
			byte[] req = DslReq();
			Span<byte> res;
			if (chip.TransmitData(targetNumber, req) >= 0)
			{
				if (chip.ReceiveData(targetNumber, out res, 10) >= 0)
				{
					byte[] reply = res.ToArray();
					if ((reply[2] == (byte)Nfcip1.Types.RES) && (reply[3] == (byte)Nfcip1.Commands.DSL_RES)) //hebben we een reply ontvangen
					{
						//todo data analyseren
						return true;
					}
				}
			}
			return false;
		}

		public bool Dep_req(INfcTranceiver chip, byte targetNumber, PfbTypes type, byte[] payload, out byte[] response)
		{
			byte pni = PniGenerator.PNI;
			byte pfb = (byte)(((int)type << 5 ) | pni);
			// todo:
			//		- MI NAD DID support
			//		- ACk/Nack support
			//		- Attention/Timeout support
			response = new byte[0];
			byte[] req = DepReq(pfb, payload);
			Span<byte> res;
			if (chip.TransmitData(targetNumber, req) >= 0)
			{
				if (chip.ReceiveData(targetNumber, out res, 100) >= 0)
				{
					byte[] reply = res.ToArray();
					if ((reply[2] == (byte)Nfcip1.Types.RES) && (reply[3] == (byte)Nfcip1.Commands.DEP_RES)) //hebben we een reply ontvangen
					{
						if ((reply[4] & 0x03) == pni)
                        {
							PniGenerator.Increase();
							int startIndex = 4;
							if ((reply[4] & 0x4) > 0)   //NAD byte available
								startIndex++;
							if ((reply[4] & 0x2) > 0)   //DID byte available
								startIndex++;

							// todo:
							//		- MI NAD DID support
							//		- ACk/Nack support
							//		- Attention/Timeout support
							response = new byte[reply.Length - startIndex];
							Array.Copy(reply, startIndex, response, 0, response.Length);
							return true;
						}
					}
				}
			}
			return false;
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

		public static byte[] Fc128Framing(byte[] data)
        {
			//full frame without crc
			byte[] result = new byte[data.Length + 2];
			int index = 0;
			result[index++] = 0xF0;
			result[index++] = (byte)(data.Length + 1);
			data.CopyTo(result, index);
			return result;
		}

		private byte[] AtrReq(byte[] nfcid, byte[] genBytes)
		{ 
            int length = 16 + genBytes.Length;
            byte[] result = new byte[length];
            result[0] = (byte)Types.REQ;
            result[1] = (byte)Commands.ATR_REQ;
            nfcid.CopyTo(result, 2);
            result[12] = 0; //DID
            result[13] = 0; //BSI
            result[14] = 0; //BRI
            result[15] = 32; //PPI
            if (genBytes.Length > 0)
                result[15] |= 0x02;
            genBytes.CopyTo(result, 16);

            result = Fc128Framing(result);
            return result;
		}

		private byte[] DslReq()
		{
			int length = 3;
			byte[] result = new byte[length];
			result[0] = (byte)Types.REQ;
			result[1] = (byte)Commands.DSL_REQ;
			result[2] = 0; //DID

			result = Fc128Framing(result);
			return result;
		}

		private byte[] DepReq(byte pfb, byte[] data)
		{
			int length = 3 + data.Length;
			byte[] result = new byte[length];
			result[0] = (byte)Types.REQ;
			result[1] = (byte)Commands.DEP_REQ;
			result[2] = pfb; //PFL
			data.CopyTo(result, 3);

			result = Fc128Framing(result);
			return result;
		}

		//public static byte[] WupReq(byte[] nfcid)
		//{
		//	int length = 13;
		//	byte[] result = new byte[length];
		//	result[0] = (byte)Types.REQ;
		//	result[1] = (byte)Commands.WUP_REQ;
		//	nfcid.CopyTo(result, 2);
		//	result[12] = 0; //DID

		//	result = Fc128Framing(result);
		//	return result;
		//}

		//public static byte[] PslReq(byte brs, byte fsl)
		//{
		//	int length = 5;
		//	byte[] result = new byte[length];
		//	result[0] = (byte)Types.REQ;
		//	result[1] = (byte)Commands.PSL_REQ;
		//	result[2] = 0; //DID
		//	result[3] = brs; //BRS
		//	result[4] = fsl; //FSL

		//	result = Fc128Framing(result);
		//	return result;
		//}

		//public static byte[] RlsReq()
		//{
		//	int length = 3;
		//	byte[] result = new byte[length];
		//	result[0] = (byte)Types.REQ;
		//	result[1] = (byte)Commands.RLS_REQ;
		//	result[2] = 0; //DID

		//	result = Fc128Framing(result);
		//	return result;
		//}
		//public static byte[] PolReq()
		//{
		//	int length = 5;
		//	byte[] result = new byte[length];
		//	result[0] = 0;
		//	result[1] = 0xFF;
		//	result[2] = 0xFF;
		//	result[3] = 0;
		//	result[4] = 0;

		//	result = Fc128Framing(result);
		//	return result;
		//}
	}
}
