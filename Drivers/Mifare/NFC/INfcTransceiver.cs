using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.NFC
{
    public interface INfcTransceiver
    {
        /// <summary>
        /// TransmitData should send the data and wait for it to be send to the PICC. 
        /// Normally this can be checked by a TX IRQ flag in the IRQ_Status register
        /// This function should reset the TX IRQ flag before sending the data
        /// </summary>
        /// <param name="targetNumber"></param>
        /// <param name="dataToSend"></param>
        /// <returns></returns>
        int TransmitData(byte targetNumber, ReadOnlySpan<byte> dataToSend);

        /// <summary>
        /// DataReceived will check if data is received and available
        /// Normally this can be checked by a RX IRQ flag in the IRQ_Status register
        /// </summary>
        /// <param name="targetNumber"></param>
        /// <param name="dataToSend"></param>
        /// <param name="timeOutInMilliSeconds"></param>
        /// <returns></returns>
        bool DataReceived(byte targetNumber);

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
        int ReceiveData(byte targetNumber, out Span<byte> dataToReceive, int timeOutInMilliSeconds);

        /// <summary>
        /// Tranceive is a combination of the transmit and the receive function.
        /// </summary>
        /// <param name="targetNumber"></param>
        /// <param name="dataToSend"></param>
        /// <param name="dataFromCard"></param>
        /// <param name="timeOutInMilliSeconds"></param>
        /// <returns></returns>
        int Transceive(byte targetNumber, ReadOnlySpan<byte> dataToSend, out Span<byte> dataFromCard, int timeOutInMilliSeconds);
    }
}
