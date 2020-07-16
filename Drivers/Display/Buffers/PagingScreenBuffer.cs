using System;
using System.Drawing;

namespace Drivers.Display.Buffers
{

	/*
	* depends on:
	* linux:	sudo apt-get install libgdiplus
	*			sudo apt-get install ttf-mscorefonts-installer
	*	
	*	
	*	Installeren nieuwe versie libgdiplus (duurt lang):
	*	
	*		apt install -y libtool libjpeg-dev libtiff-dev libcairo2-dev libglib2.0-dev git autoconf
	*	
	*		git clone https://github.com/mono/libgdiplus --single-branch --branch master --depth 1
	*		cd libgdiplus
	*		./autogen.sh
	*		make install
	*				
	*/
	public class PagingScreenBuffer
	{
		public Bitmap Canvas { get; private set; } = null;
		public byte[] Buffer = new byte[1024];
		private object BufferLock { get; set; } = new object();

		int[] pos = new int[8]; //page = 8 pixel heigh

		public event EventHandler<EventArgs> NewBufferAvailable;

		public PagingScreenBuffer(int x, int y)
		{
			Canvas = new Bitmap(x, y);
			Buffer = new byte[(x * y) / 8]; //first screen is black screen by defining an empty array
			for (int i = 0; i < pos.Length; i++)
			{
				pos[i] = i * (x / 8);
			}
		}

		public byte GetPixelValue(Color pixel)
		{
			if ((pixel.R == 0xFF) || (pixel.G == 0xFF) || (pixel.B == 0xFF))
				return 1;
			else
				return 0;
		}

		public void FlushBitmapToBuffer()
		{
			lock (BufferLock)
			{
				Buffer = new byte[Buffer.Length];
				for (int y = 0; y < Canvas.Height; y++)
				{
					for (int x = 0; x < Canvas.Width; x++)
					{
						byte value = GetPixelValue(Canvas.GetPixel(x, y));
						int bufPos = x + ((y / 8) * Canvas.Width);
						int bitPos = y % 8;
						value <<= bitPos;
						Buffer[bufPos] |= value;
					}
				}
				//Console.WriteLine(BitConverter.ToString(Buffer));
			}
			NewBufferAvailable?.Invoke(this, new EventArgs());
		}

		public byte[] GetPagesBuffer()
		{
			byte[] pagesBuffer = null;
			lock (BufferLock)
			{
				pagesBuffer = (byte[])Buffer.Clone();
			}
			return pagesBuffer;
		}

	}
}