using System;

namespace URGGlTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello URG Gl Test!");
			
//			URGDataDraw win = new URGDataDraw();
			URG.Gl.DataDraw win = new URG.Gl.DataDraw();
			win.Show();
			
		}
	}
}
