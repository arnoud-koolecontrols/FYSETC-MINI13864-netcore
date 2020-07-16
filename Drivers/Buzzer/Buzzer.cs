using System.Device.Gpio;
using System.Device.Pwm;
using System.Device.Pwm.Drivers;
using System.Drawing;
using System.Threading;

namespace Drivers.Buzzer
{
	public class Buzzer
	{
		public class BuzzerPinning
		{
			public int BuzzerPin { get; set; } = -1;
			public int Chip { get; set; } = -1;
			public int Channel { get; set; } = -1;
		}

		private Buzzer.BuzzerPinning Pinning { get; set; } = new Buzzer.BuzzerPinning();
		private PwmChannel Pwm { get; set; } = null;

		public Buzzer(Buzzer.BuzzerPinning pinning)
		{
			Pinning = pinning;
			InitIO();
		}

		private void InitIO()
		{
			if (Pinning != null)
			{
				if ((Pinning.Chip != -1) && (Pinning.Channel != -1))
                {
					Pwm = PwmChannel.Create(0, 1, 1000, 0.5);
				} else
                {
					Pwm = new SoftwarePwmChannel(Pinning.BuzzerPin, 1000, 0.5, true);
				}	
			}
		}

		public void Buzz(int frequency, double volume, int ms)
        {
			Pwm.Frequency = frequency;
			if (volume>0.5)
            {
				volume = 0.5;
            }
			
			Pwm.DutyCycle = volume;
			Pwm.Start();
			Thread.Sleep(ms);
			Pwm.Stop();
		}
	}
}