using Drivers.Buzzer;
using Drivers.Display;
using Drivers.LED;
using myApp.Drivers.Encoder;
using myApp.Drivers.Mifare;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace myApp
{
    public class App
    {
		public PN5180 Mifare { get; private set; }
		public ST7567 Display { get; private set; }
		public RgbLed RgbLed { get; private set; }
		public DigitalEncoder DigitalEncoder { get; private set; }
		public Buzzer Buzzer { get; private set; }
		/// <summary>
		/// Drwastring alternative
		///		There seems to be a bug where running under Linuc multiple chars are a problem. 
		///		Probably a encoding thingy..
		/// </summary>
		/// <param name="g"></param>
		/// <param name="text"></param>
		/// <param name="font"></param>
		/// <param name="brush"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void DrawString(Graphics g, string text, Font font, Brush brush, float x, float y)
		{
			foreach (char ch in text)
			{
				SizeF size = g.MeasureString("" + ch, font);
				g.DrawString("" + ch, font, brush, x, y);
				x += size.Width;
				if (font.Size < 12)
					x += 1;
			}
		}

		public void CreateMainScreen(Graphics g)
		{
			Font font = new Font("Calibri", 8, FontStyle.Regular, GraphicsUnit.Pixel);
			Pen pen = new Pen(Color.Black);
			g.Clear(Color.White);
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
			DrawString(g, "Welkom", font, Brushes.Black, 45, 0);
		}

		public void UpdateScreen()
        {
			if (Display != null)
            {
				using (Graphics g = Graphics.FromImage(Display.Screen.Canvas))
				{
					Font font = new Font("Calibri", 8, FontStyle.Regular, GraphicsUnit.Pixel);
					Pen pen = new Pen(Color.Black);
					CreateMainScreen(g);

					g.DrawLine(pen, 0, 0, 128, 64);
					g.DrawLine(pen, 0, 64, 128, 0);
					if (this.pushed)
					{
						g.FillEllipse(Brushes.Black, 49, 17, 30, 30);
					}
					else
					{
						g.DrawEllipse(pen, 49, 17, 30, 30);
					}
					switch (this.memPos)
					{
						case 0:
							g.FillPie(pen.Brush, 30, 44, 24, 24, -30, 60);
							break;

						case 1:
							g.FillRectangle(pen.Brush, 58, 50, 12, 12);
							break;

						case 2:
							g.FillPie(pen.Brush, 74, 44, 24, 24, 150, 60);
							break;
					}
				}
				Display.Screen.FlushBitmapToBuffer();   //after editing we flush the picture to a display buffer	
			}
		}

		public App()
        {
			object spiLock = new object(); //two devices on one spi bus, needs a lockObject

            ST7567.ST7567Pinning displayPinning = new ST7567.ST7567Pinning()
            {
                RST = 2,
                A0 = 7,
                CS = 0, //note this is not the pin but the /dev/spi1.x
                SpiBus = 1,
                SpiLock = spiLock,
            };
            Display = new ST7567(displayPinning, 128, 64);

            PN5180.PN5180Pinning pn5180Pinning = new PN5180.PN5180Pinning()
            {
                RST = 19,
                BUSY = 18,
                NSS = 10,
                //IRQ = 18,
                CS = 1, //note this is not the pin but the /dev/spi1.x
                SpiBus = 1,
                SpiLock = spiLock,
            };
            Mifare = new PN5180(pn5180Pinning);

            RgbLed.RgbLedPinning rgbLedPinning = new RgbLed.RgbLedPinning()
			{
				Red = 0,
				Green = 1,
				Blue = 3
			};
			RgbLed = new RgbLed(rgbLedPinning);

            DigitalEncoder.DigitalEncoderPinning digitalEncoderPinning = new DigitalEncoder.DigitalEncoderPinning()
            {
                Enc0 = 12,
                Enc1 = 198,
                Enc2 = 199
            };
            DigitalEncoder = new DigitalEncoder(digitalEncoderPinning);
            DigitalEncoder.Pushed += DigitalEncoder_Pushed;
            DigitalEncoder.Releashed += DigitalEncoder_Releashed;
            DigitalEncoder.Right += DigitalEncoder_Right;
            DigitalEncoder.Left += DigitalEncoder_Left;

            Buzzer.BuzzerPinning buzzerPinning = new Buzzer.BuzzerPinning()
            {
                BuzzerPin = 6, //PA6
            };
            Buzzer = new Buzzer(buzzerPinning);

            UpdateScreen();
		}

        private void DigitalEncoder_Releashed(object sender, EventArgs e)
        {
			pushed = false;
			UpdateScreen();
			this.Mifare.Hold = false;
		}

		int memPos = 1;			//do we need locking? 
		bool pushed = false;    //do we need locking? 

		private void DigitalEncoder_Left(object sender, EventArgs e)
		{
			if (memPos > 0)
            {
				memPos--;
				UpdateScreen();
				Buzzer.Buzz(8000, 5);
			}
			//Console.WriteLine("Left");
		}

		private void DigitalEncoder_Right(object sender, EventArgs e)
		{
			if (memPos < 2)
			{
				memPos++;
				UpdateScreen();
				Buzzer.Buzz(8000, 5);
			}
			//Console.WriteLine("Right");
		}

		private void DigitalEncoder_Pushed(object sender, EventArgs e)
		{
			pushed = true;
			UpdateScreen();
		}
	}
}
