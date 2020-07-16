using myApp.Drivers.Helper;
using System;
using System.Device.Gpio;

namespace myApp.Drivers.Encoder
{
    public class DigitalEncoder
    {
		public event EventHandler<EventArgs> Pushed;
		public event EventHandler<EventArgs> Releashed;
		public event EventHandler<EventArgs> Left;
		public event EventHandler<EventArgs> Right;


		public class DigitalEncoderPinning
        {
            public int Enc0 { get; set; } = -1;
            public int Enc1 { get; set; } = -1;
            public int Enc2 { get; set; } = -1;
        }

		private DigitalEncoder.DigitalEncoderPinning Pinning { get; set; } = new DigitalEncoder.DigitalEncoderPinning();
		private GpioController IoController { get; set; } = null;

		private Debouncer Enc0Deboucer = new Debouncer(50); // one second

		public void DetectDirection(bool isEnc1)
        {
			if (isEnc1)
            {

            }

        }

		public DigitalEncoder(DigitalEncoder.DigitalEncoderPinning pinning)
		{
			Pinning = pinning;
			InitIO();
		}

		private void Enc0Falling(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
			Enc0Deboucer.Debouce(() => {
				Pushed?.Invoke(this, new EventArgs());
				//Console.WriteLine("ENC0 push");
			});
		}
		private void Enc0Rising(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
		{
			Enc0Deboucer.Debouce(() => {
				Releashed?.Invoke(this, new EventArgs());
				//Console.WriteLine("ENC0 release");
			});
		}

		private void Enc2Changed(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
		{
            bool enc1 = IoController.Read(Pinning.Enc1) == PinValue.High;
            bool enc2 = IoController.Read(Pinning.Enc2) == PinValue.High;
			if (!enc2)
            {
				if (enc1)
                {
					Right?.Invoke(this, new EventArgs());
					//Console.WriteLine("Rechts");
				} else
                {
					Left?.Invoke(this, new EventArgs());
					//Console.WriteLine("Left");
				}
            } 
        }

		private void InitIO()
		{
			if (Pinning != null)
			{
				IoController = new GpioController();
				if (this.Pinning.Enc0 > -1)
                {
					this.IoController.OpenPin(this.Pinning.Enc0, PinMode.Input);
					this.IoController.RegisterCallbackForPinValueChangedEvent(this.Pinning.Enc0, PinEventTypes.Falling, Enc0Falling);
					this.IoController.RegisterCallbackForPinValueChangedEvent(this.Pinning.Enc0, PinEventTypes.Rising, Enc0Rising);
				}
					
				if (this.Pinning.Enc1 > -1)
                {
					this.IoController.OpenPin(this.Pinning.Enc1, PinMode.Input);
				}
					
				if (this.Pinning.Enc2 > -1)
                {
					this.IoController.OpenPin(this.Pinning.Enc2, PinMode.Input);
					this.IoController.RegisterCallbackForPinValueChangedEvent(this.Pinning.Enc2, PinEventTypes.Falling, Enc2Changed);
				}
					
			}
		}
	}
}
