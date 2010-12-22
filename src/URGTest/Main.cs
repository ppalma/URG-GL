using System;
using MBF.Sensors;
namespace URGTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World from URG console test");
			URG urg = new URG();
			urg.Connect();
		}
	}
}
