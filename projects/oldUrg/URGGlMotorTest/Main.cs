using System;
using MBF.Sensors;
namespace URGGlMotorTest
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello URGGlMotorTest");
			
			URGDataDraw win = new URGDataDraw();
			win.Show();
		}
	}
}