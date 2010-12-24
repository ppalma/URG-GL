using System;

namespace URGGlTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello URG Gl Test!");
//			URG.Gl.DataDraw win = new URG.Gl.DataDraw();
			URG.Gl.LaserDataDraw win = new URG.Gl.LaserDataDraw();
			win.Show();
		}
	}
}
