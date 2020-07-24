using Drivers.Display.Buffers;
using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

namespace Drivers.Display
{
	public class ST7567
	{
		public class ST7567Pinning
		{
			public int RST { get; set; } = -1;
			public int A0 { get; set; } = -1;
			public int CS { get; set; } = -1;
			public int SpiBus { get; set; } = 0;
			public SpiDevice SPI { get; set; } = null;
			public object SpiLock { get; set; } = new object();
			public void InitSPI(SpiConnectionSettings settings)
			{
				this.SPI = SpiDevice.Create(settings);
			}
		}

		public PagingScreenBuffer Screen { get; private set; } = null;
		private ST7567.ST7567Pinning Pinning { get; set; } = new ST7567.ST7567Pinning();
		private GpioController IoController { get; set; } = null;


		public void WriteBuffer()
		{
			byte[] pagesBuffer = Screen.GetPagesBuffer();
			if (Pinning != null)
			{
				lock(Pinning.SpiLock)
                {
					for (int i = 0; i < pagesBuffer.Length; i++)
					{
						if (i % Screen.Canvas.Width == 0)
						{
							int page = (i / Screen.Canvas.Width);
							this.IoController.Write(this.Pinning.A0, PinValue.Low);
							this.Pinning.SPI.WriteByte((byte)(0xB0 + page));
							this.Pinning.SPI.WriteByte(0x10);
							this.Pinning.SPI.WriteByte(0x00);
							this.IoController.Write(this.Pinning.A0, PinValue.High);
						}
						this.Pinning.SPI.WriteByte(pagesBuffer[i]);
					}
				}	
			}
		}

		public ST7567(ST7567.ST7567Pinning pinning, int x, int y)
		{
			Pinning = pinning;
			Screen = new PagingScreenBuffer(x, y);
			InitIO();
			HwReset();
			InitDisplay();
			Screen.NewBufferAvailable += Screen_NewBufferAvailable;
		}

		private void Screen_NewBufferAvailable(object sender, EventArgs e)
		{
			WriteBuffer();
		}

		private void InitIO()
		{
			if (Pinning != null)
			{
				IoController = new GpioController();
				if (this.Pinning.RST > -1)
					this.IoController.OpenPin(this.Pinning.RST, PinMode.Output);
				if (this.Pinning.A0 > -1)
					this.IoController.OpenPin(this.Pinning.A0, PinMode.Output);
				SpiConnectionSettings settings = new SpiConnectionSettings(this.Pinning.SpiBus, this.Pinning.CS);
				settings.ClockFrequency = 32000000;
				settings.Mode = SpiMode.Mode3;
				settings.ChipSelectLineActiveState = PinValue.Low;
				settings.DataFlow = DataFlow.MsbFirst;
				settings.DataBitLength = 8;
				this.Pinning.InitSPI(settings);
			}
		}

		public void HwReset()
		{
			if (Pinning != null)
			{
				if (this.Pinning.RST > -1)
				{
					this.IoController.Write(this.Pinning.RST, PinValue.High);
					Thread.Sleep(5);
					this.IoController.Write(this.Pinning.RST, PinValue.Low);
					Thread.Sleep(5);
					this.IoController.Write(this.Pinning.RST, PinValue.High);
					Thread.Sleep(5);
					//this.IoController.Write(this.Pinning.RST, PinValue.Low);
					//Thread.Sleep(5);
				}
			}
		}

		private void InitDisplay()
		{
			if (Pinning != null)
			{
				lock(Pinning.SpiLock)
                {
					Screen.FlushBitmapToBuffer();
					WriteBuffer();
					this.IoController.Write(this.Pinning.A0, PinValue.Low);
					this.Pinning.SPI.WriteByte(0xA3); // # Bias 1/7
					this.Pinning.SPI.WriteByte(0xA0);
					this.Pinning.SPI.WriteByte(0xC8); // # Normal Orientation
					this.Pinning.SPI.WriteByte(0xA6); // # Normal Display (0xA7 = inverse)
					this.Pinning.SPI.WriteByte(0x40); // set display start line at 0
					this.Pinning.SPI.WriteByte(0x2F); // PowerMode control
					this.Pinning.SPI.WriteByte(0x23); // Regulation Ratio
					this.Pinning.SPI.WriteByte(0xAF);
				}		
			}
		}
	}
}