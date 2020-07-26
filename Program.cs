using Drivers.Buzzer;
using Drivers.Display;
using Drivers.LED;
using Iot.Device.Pn5180;
using myApp.Drivers.Encoder;
using myApp.Drivers.Mifare.NFC.NFCIP1;
using System;
using System.Drawing;
using System.Threading;

namespace myApp
{
    class Program
    {
		static void Main(string[] args)
        {
			App app = new App();
			
            while (true) {
                app.RgbLed.SetColor(Color.Green);
                for (int i = 0; i < 5; i++)
                {
                    app.Mifare.ScanForISO14443TypeADevices(1); //we only poll for 1ms every 200 ms as the spi bus is shared with the screen
                    Thread.Sleep(199);
                }
                app.RgbLed.SetColor(Color.Blue);
                for (int i = 0; i < 5; i++)
                {
                    app.Mifare.ScanForISO14443TypeADevices(1); //we only poll for 1ms every 200 ms as the spi bus is shared with the screen
                    Thread.Sleep(199);
                }
                app.RgbLed.SetColor(Color.Red);
                for (int i = 0; i < 5; i++)
                {
                    app.Mifare.ScanForISO14443TypeADevices(1); //we only poll for 1ms every 200 ms as the spi bus is shared with the screen
                    Thread.Sleep(199);
                }
            }
        }

    }
}
