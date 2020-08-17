using Iot.Device.Card;
using System;

namespace Iot.Device.Nfc
{
    public abstract class NfcTransceiver: CardTransceiver, INfcTransceiver
    {
        protected NfcTransceiver() : base()
        {
        
        }

        /// <inheritdoc/>
        public abstract int TransmitData(byte targetNumber, ReadOnlySpan<byte> dataToSend);

        /// <inheritdoc/>
        public abstract bool DataReceived(byte targetNumber);

        /// <inheritdoc/>
        public abstract int ReceiveData(byte targetNumber, out Span<byte> dataToReceive, int timeOutInMilliSeconds);

        /// <inheritdoc/>
        public abstract int Transceive(byte targetNumber, ReadOnlySpan<byte> dataToSend, out Span<byte> dataFromCard, int timeOutInMilliSeconds);

    }
}
