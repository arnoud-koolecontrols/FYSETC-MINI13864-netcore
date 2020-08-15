using Iot.Device.Card.Mifare;
using Iot.Device.Pn5180V2;
using Iot.Device.Rfid;
using myApp.Drivers.Mifare.NFC.LLCP;
using myApp.Drivers.Mifare.NFC.LLCP.ServiceManagers;
using myApp.Drivers.Mifare.NFC.NFCIP1;
using System;
using System.Device.Gpio;
using System.Device.Spi;
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
			public object SpiLock { get; set; } = new object();
			public void InitSPI(SpiConnectionSettings settings)
			{
				this.SPI = SpiDevice.Create(settings);
			}
		}

		private Pn5180 Chip { get; set; } = null;
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
				settings.ClockFrequency = 2000000;//Pn5180.MaximumSpiClockFrequency;
                settings.Mode = Pn5180.DefaultSpiMode;
                settings.ChipSelectLineActiveState = PinValue.Low;
                settings.DataFlow = DataFlow.MsbFirst;
                settings.DataBitLength = 8;
                this.Pinning.InitSPI(settings);
				HwReset();
				Chip = new Iot.Device.Pn5180V2.Pn5180(Pinning.SPI, Pinning.BUSY, Pinning.NSS,null,true,Iot.Device.Card.LogLevel.None);
				Chip.LogTo = Iot.Device.Card.LogTo.Console;
			}
		}

		public void HwReset()
		{
			if (Pinning != null)
			{
				if (this.Pinning.RST > -1)
				{
					this.IoController.Write(this.Pinning.RST, PinValue.High);
					Thread.Sleep(50);
					this.IoController.Write(this.Pinning.RST, PinValue.Low);
					Thread.Sleep(50);
					this.IoController.Write(this.Pinning.RST, PinValue.High);
					Thread.Sleep(100);
					//this.IoController.Write(this.Pinning.RST, PinValue.Low);
					//Thread.Sleep(5);
				}
			}
		}

		private void InitChip()
		{
			if (Pinning != null)
			{
				Version product = new Version();
				Version firmware = new Version();
				Version eeprom = new Version();
				lock (Pinning.SpiLock)
                {
					(product, firmware, eeprom) = Chip.GetVersions();
				}
				Console.WriteLine("product: " + product.ToString());
				Console.WriteLine("firmware: " + firmware.ToString());
				Console.WriteLine("eeprom: " + eeprom.ToString());
			}
		}

		public bool Hold { get; set; } = false;
		private LLCP llcp;
		public void ScanForISO14443TypeADevices(int scanTimeInMilliseconds)
		{
			Data106kbpsTypeA cardTypeA;
			// This will try to select the card for 1 second and will wait 300 milliseconds before trying again if none is found
			bool retok = false;
			lock (Pinning.SpiLock)
			{

				retok = Chip.ListenToCardIso14443TypeA(TransmitterRadioFrequencyConfiguration.Iso14443A_Nfc_PI_106_106, ReceiverRadioFrequencyConfiguration.Iso14443A_Nfc_PI_106_106, out cardTypeA, scanTimeInMilliseconds);

				if (retok)
				{
					Hold = true;
					Console.WriteLine($"ISO 14443 Type A found:");
					Console.WriteLine($"  ATQA: {cardTypeA.Atqa}");
					Console.WriteLine($"  SAK: {cardTypeA.Sak}");
					Console.WriteLine($"  UID: {BitConverter.ToString(cardTypeA.NfcId)}");

					if (Nfcip1.IsNfcipCompliant(cardTypeA.Sak, cardTypeA.NfcId))
					{
						Console.WriteLine($"NFCIP1 compliant!!");
						if ((cardTypeA.Sak & 0x20) == 0x20)
						{
							Chip.OutputAllRegisters();
							if (llcp != null)
							{
								llcp.Stop();
							}
							llcp = new LLCP(Chip, cardTypeA);
                            llcp.ConnectionChanged += Llcp_ConnectionChanged;
							llcp.Start();
						}
					}
					else
					{
						// This is where you do something with the card
						MifareCard mifareCard = new MifareCard(Chip, cardTypeA.TargetNumber);
						mifareCard.SetCapacity(cardTypeA.Atqa, cardTypeA.Sak);
						mifareCard.SerialNumber = cardTypeA.NfcId;
						mifareCard.KeyA = new byte[6] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
						mifareCard.KeyB = new byte[6] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
						for (byte block = 0; block < 64; block++)
						{
							mifareCard.BlockNumber = block;
							mifareCard.Command = MifareCardCommand.AuthenticationA;
							int ret = mifareCard.RunMifiCardCommand();
							if (ret >= 0)
							{
								mifareCard.BlockNumber = block;
								mifareCard.Command = MifareCardCommand.Read16Bytes;
								ret = mifareCard.RunMifiCardCommand();
								if (ret >= 0)
								{
									Console.WriteLine($"Block: {block}, Data: {BitConverter.ToString(mifareCard.Data)}");
									if (block % 4 == 3)
									{
										// Check what are the permissions
										for (byte j = 3; j > 0; j--)
										{
											var access = mifareCard.BlockAccess((byte)(block - j), mifareCard.Data);
											Console.WriteLine($"Block: {block - j}, Access: {access}");
										}
										var sector = mifareCard.SectorTailerAccess(block, mifareCard.Data);
										Console.WriteLine($"Block: {block}, Access: {sector}");
									}
								}
								else
								{
									Console.WriteLine($"Error reading block: {block}, Data: {BitConverter.ToString(mifareCard.Data)}");
								}
							}
							else
							{
								break;
							}
						}
					}
				}
			}
			while (Hold)
				Thread.Sleep(100);
		}

        private void Llcp_ConnectionChanged(object sender, LLCPLinkActivatedEventArgs e)
        {
            if (e.Connected)
            {
				lock (Pinning.SpiLock)
				{
					Console.WriteLine("LLCP open");
					LLCP llcp = (LLCP)sender;
					SnepServiceManager snepServiceManager = llcp.SnepServiceManager;
					if (snepServiceManager != null)
					{
						Console.WriteLine("Snep service found");
						NdefLibrary.Ndef.NdefMessage message = new NdefLibrary.Ndef.NdefMessage();
						NdefLibrary.Ndef.NdefUriRecord uriRecord = new NdefLibrary.Ndef.NdefUriRecord();
						uriRecord.Uri = "http://www.koolecontrols.nl";
						//NdefLibrary.Ndef.NdefTextRecord textRecord = new NdefLibrary.Ndef.NdefTextRecord();
						//textRecord.LanguageCode = "en";
						//textRecord.Text = "Wat een geweldige test is dit";
						message.Add(uriRecord);
						byte[] ndef = message.ToByteArray();
						snepServiceManager.SendNdefMessage(ndef);
					}
				}
			} else
            {
				Console.WriteLine("LLCP closed");
            }
        }
    }
}
