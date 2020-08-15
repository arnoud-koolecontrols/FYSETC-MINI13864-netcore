using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.Nfc.LLCP
{
    public class SequenceGenerator
    {
        private int generatedSequenceNumber = 0;

        public int SequenceNumber { get { return GenerateSequenceNumber(); } }

        public void Reset()
        {
            lock (this)
            {
                generatedSequenceNumber = 0;
            }
        }

        int GenerateSequenceNumber()
        {
            int result = 0;
            lock (this)
            {
                result = generatedSequenceNumber;
                generatedSequenceNumber++;
                if (generatedSequenceNumber > 255)
                {
                    generatedSequenceNumber = 0;
                }
            }
            return result;
        }
    }

}
