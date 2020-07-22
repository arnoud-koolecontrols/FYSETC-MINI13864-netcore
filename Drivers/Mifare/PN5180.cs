﻿using Iot.Device.Pn5180;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Spi;
using System.Text;
using System.Threading;

namespace myApp.Drivers.Mifare
{
    public class PN5180
    {
		public class PN5180Pinning
		{
			public int BUSY { get; set; } = -1;
			public int RST { get; set; } = -1;
			public int IRQ { get; set; } = -1;
			public int NSS { get; set; } = -1;
			public int CS { get; set; } = -1;		//note this is not the pin but the chipselect line indicator typical 0 or 1
			public int SpiBus { get; set; } = 0;
			public SpiDevice SPI { get; set; } = null;

			public void InitSPI(SpiConnectionSettings settings)
			{
				this.SPI = SpiDevice.Create(settings);
			}
		}

		private Iot.Device.Pn5180.Pn5180 Chip { get; set; } = null;
		private PN5180.PN5180Pinning Pinning { get; set; } = new PN5180.PN5180Pinning();
		private GpioController IoController { get; set; } = null;

		public PN5180(PN5180.PN5180Pinning pinning)
		{
			Pinning = pinning;
			InitIO();
			//HwReset();
			InitChip();
		}

		private void InitIO()
		{
            if (Pinning != null)
            {
                IoController = new GpioController();
                if (this.Pinning.RST > -1)
                    this.IoController.OpenPin(this.Pinning.RST, PinMode.Output);
                if (this.Pinning.IRQ > -1)
                {
                    this.IoController.OpenPin(this.Pinning.IRQ, PinMode.Input);
                }
                SpiConnectionSettings settings = new SpiConnectionSettings(this.Pinning.SpiBus, this.Pinning.CS);
                settings.ClockFrequency = Pn5180.MaximumSpiClockFrequency;
                settings.Mode = Pn5180.DefaultSpiMode;
                settings.ChipSelectLineActiveState = PinValue.Low;
                settings.DataFlow = DataFlow.MsbFirst;
                settings.DataBitLength = 8;
                this.Pinning.InitSPI(settings);
				HwReset();
				Chip = new Iot.Device.Pn5180.Pn5180(Pinning.SPI, Pinning.BUSY, Pinning.NSS, IoController, true, Iot.Device.Card.LogLevel.Debug);
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
					Thread.Sleep(10);
					this.IoController.Write(this.Pinning.RST, PinValue.High);
					Thread.Sleep(10);
					//this.IoController.Write(this.Pinning.RST, PinValue.Low);
					//Thread.Sleep(5);
				}
			}
		}

		private void InitChip()
		{
			if (Pinning != null)
			{
				(Version product, Version firmware, Version eeprom) = Chip.GetVersions();
				Console.WriteLine("product: " + product.ToString());
				Console.WriteLine("firmware: " + firmware.ToString());
				Console.WriteLine("eeprom: " + eeprom.ToString());
			}
		}
	}
}