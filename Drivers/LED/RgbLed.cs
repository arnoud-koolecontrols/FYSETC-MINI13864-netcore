using System.Device.Gpio;
using System.Drawing;

namespace Drivers.LED
{
	public class RgbLed
	{
		public class RgbLedPinning
		{
			public int Red { get; set; } = -1;
			public int Blue { get; set; } = -1;
			public int Green { get; set; } = -1;
		}

		private RgbLed.RgbLedPinning Pinning { get; set; } = new RgbLed.RgbLedPinning();
		private GpioController IoController { get; set; } = null;

		public RgbLed(RgbLed.RgbLedPinning pinning)
		{
			Pinning = pinning;
			InitIO();
		}

		private void InitIO()
		{
			if (Pinning != null)
			{
				IoController = new GpioController();
				if (this.Pinning.Red > -1)
					this.IoController.OpenPin(this.Pinning.Red, PinMode.Output);
				if (this.Pinning.Green > -1)
					this.IoController.OpenPin(this.Pinning.Green, PinMode.Output);
				if (this.Pinning.Blue > -1)
					this.IoController.OpenPin(this.Pinning.Blue, PinMode.Output);
			}
		}

		public void SetColor(Color color)
        {
			if (color.R > 0)
            {
				this.IoController.Write(this.Pinning.Red, PinValue.High);
			} else
            {
				this.IoController.Write(this.Pinning.Red, PinValue.Low);
			}
			if (color.G > 0)
			{
				this.IoController.Write(this.Pinning.Green, PinValue.High);
			}
			else
			{
				this.IoController.Write(this.Pinning.Green, PinValue.Low);
			}
			if (color.B > 0)
			{
				this.IoController.Write(this.Pinning.Blue, PinValue.High);
			}
			else
			{
				this.IoController.Write(this.Pinning.Blue, PinValue.Low);
			}
		}
	}
}