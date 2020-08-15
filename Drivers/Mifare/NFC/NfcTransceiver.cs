using Iot.Device.Card;
using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.Nfc
{
    //
    // Summary:
    //     Abstract class implementing a specific Write and Read function This class allow
    //     to transceive information with the card This class has to be implemented in all
    //     RFID/NFC/Card readers So Mifare cards can be used the same way independent of
    //     any reader
    public abstract class NfcTransceiver: CardTransceiver, INfcTransceiver
    {
        protected NfcTransceiver() : base()
        {
        
        }

        /// <summary>
        /// TransmitData should send the data and wait for it to be send to the PICC. 
        /// Normally this can be checked by a TX IRQ flag in the IRQ_Status register
        /// This function should reset the TX IRQ flag before sending the data
        /// </summary>
        /// <param name="targetNumber"></param>
        /// <param name="dataToSend"></param>
        /// <returns></returns>
        public abstract int TransmitData(byte targetNumber, ReadOnlySpan<byte> dataToSend);

        /// <summary>
        /// DataReceived will check if data is received and available
        /// Normally this can be checked by a RX IRQ flag in the IRQ_Status register
        /// </summary>
        /// <param name="targetNumber"></param>
        /// <param name="dataToSend"></param>
        /// <param name="timeOutInMilliSeconds"></param>
        /// <returns></returns>
        public abstract bool DataReceived(byte targetNumber);

        /// <summary>
        /// ReceiveData will wait for a given time for data to receive. If data is received it will stop waiting and fill the
        ///  dataToReceive out parameter.
        /// Normally a RX IRQ is available if data is received.
        /// If the RX IRQ flag is not set it will wait till it is set or the timeouttime is passed
        /// If the RX IRQ flag is set the data is red from the chip. After this the RX IRQ flag will be reset. (If not done automaticly by the chip)
        /// </summary>
        /// <param name="targetNumber"></param>
        /// <param name="dataToReceive"></param>
        /// <param name="timeOutInMilliSeconds"></param>
        /// <returns></returns>
        public abstract int ReceiveData(byte targetNumber, out Span<byte> dataToReceive, int timeOutInMilliSeconds);

        /// <summary>
        /// Tranceive should be the same as in the CardTranceive but with a timeout paramater for rceiving data.
        /// </summary>
        /// <param name="targetNumber"></param>
        /// <param name="dataToSend"></param>
        /// <param name="dataFromCard"></param>
        /// <param name="timeOutInMilliSeconds"></param>
        /// <returns></returns>
        public abstract int Transceive(byte targetNumber, ReadOnlySpan<byte> dataToSend, out Span<byte> dataFromCard, int timeOutInMilliSeconds);

    }
}
