using Drivers.Buzzer;
using Drivers.Display;
using Drivers.LED;
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

			int switchTime = 1000;
            while (true) {
				app.RgbLed.SetColor(Color.Green);
				Thread.Sleep (switchTime);
				app.RgbLed.SetColor(Color.Blue);
				Thread.Sleep (switchTime);
				app.RgbLed.SetColor(Color.Red);
				Thread.Sleep(switchTime);
			}
        }

    }
}
