using System;
using MBF.Sensors;
namespace URGTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			URG urg = new URG();
			urg.Connect();
		}
	}
}
