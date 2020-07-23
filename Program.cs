using Drivers.Buzzer;
using Drivers.Display;
using Drivers.LED;
using Iot.Device.Pn5180;
using myApp.Drivers.Encoder;
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
				app.Mifare.ScanForISO14443TypeADevices();
				app.RgbLed.SetColor(Color.Blue);
				app.Mifare.ScanForISO14443TypeADevices(); 
				app.RgbLed.SetColor(Color.Red);
				app.Mifare.ScanForISO14443TypeADevices();
			}
        }

    }
}
