using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.NFCIP1
{
	public class CRC16
	{
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

	}

}
