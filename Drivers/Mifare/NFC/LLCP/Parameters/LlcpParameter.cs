using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.NFC.LLCP.Parameters
{
    public class LLCPParameter
    {
        public LLCParameterType Type { get { return GetParameterType(); } }
        public int Length { get { return GetParameterLength(); } }
        protected byte[] data = new byte[0];
        public byte[] Data { get { return (byte[])data.Clone(); } }

        public LLCPParameter(byte[] data, int index)
        {
            if ((data.Length - index) >= 2) // We need atleast two bytes
            {
                int amountOfBytes = (data[index + 1] + 2);
                if (amountOfBytes + index <= data.Length) // Are there enough bytes
                {
                    this.data = new byte[amountOfBytes];
                    Array.Copy(data, index, this.data, 0, amountOfBytes);
                }
            }
        }

        private int GetParameterLength()
        {
            int result = 0;
            if (data.Length >= 2)
            {
                result = data[1];
            }
            return result;
        }

        private LLCParameterType GetParameterType()
        {
            LLCParameterType result = LLCParameterType.Unknown;
            if (data.Length >= 2)
            {
                result = (LLCParameterType)data[0];
            }
            return result;
        }

        public LLCPParameter()
        {

        }
    }
}
