#region Header
//     DataDraw.cs created with MonoDevelop
//     User: Patricio Palma S. as ppalma at 6:22 PM 12/22/2010
//     --------------------------------------------------------------------------
//     URG.Gl
//     Copyright (C) 2006-2008  Patricio Palma S. All Rights Reserved.
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//     --------------------------------------------------------------------------
// 
//     UNIT                : DataDraw.cs
//     SUMMARY            :     
//     PRINCIPLE AUTHOR     : Patricio Palma S. <mail@ppalma.cl>
// 
#endregion Header
#region Revisions
//  --------------------------------------------------------------------------
//     REVISIONS/NOTES
//         dd-mm-yyyy    By          Revision Summary
// 
//  --------------------------------------------------------------------------
#endregion Revisions

using System;
using System.IO;
using Tao.FreeGlut;
using System.Collections.Generic;
using System.Drawing;
namespace URG.Gl
{


	public class DataDraw:Window
	{
		public DataDraw() :base(){}		
		public int x_rot_ = 0;
  		public int y_rot_ = 0;
  		public int z_rot_ = 0;
		
		Lines normal_lines_data_ = new Lines();
		public int AliveMsec = 3; //20;  
		
		public Point last_pos_ = new Point(0 ,0);
		
		private int ticks;
		public int magnify_ = 50;
		public bool no_plot_ = false ;
		Lines saved_lines_data_ = new Lines();
		Line recent_line_data_ = new Line();
		public bool intensity_mode_ = false ;
		private int last_button, last_state;
		private bool record = false, plot=false;
		List<Point3d<double>> color_table_ = new List<Point3d<double>>();
		
		protected MBF.Sensors.URG sensor;
		
		public void drawLaser(Line line, double ratio)
		{
			Tao.OpenGl.Gl.glColor3d(0.6, 0.0, 0.0);
			
			int index = 0;
			//for (Points::iterator it = line.points.begin();
			//	it != line.points.end(); ++it, ++index) {
			foreach (Point3d<int> it in line.points) {
				
				if ((it.x == 0) && (it.y == 0) && (it.z == 0)) {
					continue;
				}
				
				if ((index & 0x3) == 0x00) {
					Tao.OpenGl.Gl.glBegin(Tao.OpenGl.Gl.GL_LINE_STRIP);
					Tao.OpenGl.Gl.glVertex3d(0.0, 0.0, 0.0);
					Tao.OpenGl.Gl.glVertex3d(it.x * ratio, it.y * ratio, it.z * ratio);
					Tao.OpenGl.Gl.glEnd();
				}
			}
		}
		
		public void drawLine(Line line, bool record, double ratio)
		{
			if (! record) {
				double gradation =
					1.0 * (AliveMsec - (ticks - line.timestamp)) / AliveMsec;
				Tao.OpenGl.Gl.glColor3d(1.0 * gradation, 1.0 * gradation, 1.0 * gradation);
				
			} else {
				Tao.OpenGl.Gl.glColor3d(0.0, 1.0, 0.0);
			}
			
			int index = 0;
			foreach (Point3d<int> it in line.points) {
				if (record) {
					if (intensity_mode_) {
						int ratioN = (int)(line.intensity[index] + 1000) % 1000;
						Point3d<double> color = color_table_[ratioN];
						Tao.OpenGl.Gl.glColor3d(color.x, color.y, color.z);
						
					} else {
		
						int mm = ((it.x % 1000) + 1000) % 1000;
						Point3d<double> color = color_table_[mm];
						Tao.OpenGl.Gl.glColor3d(color.x, color.y, color.z);
					}
				}
				Tao.OpenGl.Gl.glVertex3d(it.x * ratio, it.y * ratio, it.z * ratio);
				
				index++;
			}
		}
#region SceneHandlers
		protected override void PaintHandler ()
		{
			Tao.OpenGl.Gl.glClearColor(0.0f, 0.0f, 0.0f, 1f);
			Tao.OpenGl.Gl.glClear(Tao.OpenGl.Gl.GL_COLOR_BUFFER_BIT | Tao.OpenGl.Gl.GL_DEPTH_BUFFER_BIT);
			Tao.OpenGl.Gl.glLoadIdentity();
			
			Tao.OpenGl.Gl.glRotated(x_rot_ / 16.0, 1.0, 0.0, 0.0);
			Tao.OpenGl.Gl.glRotated(y_rot_ / 16.0, 0.0, 1.0, 0.0);
			Tao.OpenGl.Gl.glRotated((z_rot_ / 16.0) + 90, 0.0, 0.0, 1.0);
			
			while ((normal_lines_data_.Count > 0) &&
			       ((normal_lines_data_[0].timestamp + AliveMsec) < ticks)) {
				//normal_lines_data_.pop_front();
				normal_lines_data_.Remove(normal_lines_data_[0]);
				
			}
			
			Tao.OpenGl.Gl.glBegin(Tao.OpenGl.Gl.GL_POINTS);

			double ratio = (1.0 / 2.0) + (5.0 * magnify_ / 100.0);
			
			if (! no_plot_) {
				
				//for (Lines::iterator line_it = normal_lines_data_.begin();
				//     					line_it != normal_lines_data_.end(); ++line_it)
				foreach (Line line_it in normal_lines_data_) {
					// !!! false をマクロに置き換える
					drawLine(line_it, false, ratio); 
				}
			}
			
			foreach (Line line_it in saved_lines_data_) {
				drawLine(line_it, true, ratio);
			}

			
			if (! no_plot_) {
				drawLaser(recent_line_data_, ratio);
			}
			
			Tao.OpenGl.Gl.glEnd();
			
			Glut.glutSwapBuffers();
		}
			
		protected override void KeyboardHandler(byte key, int x, int y)
		{
			switch (key)
			{
			case (byte)'m': 
                        case (byte)'M': 
				this.magnify_ += 10;
				break;
			case (byte)'n': 
			case (byte)'N': 
				this.magnify_ -= 10;
                                break;
			case (byte)'w': 
			case (byte)'W': 
//				                                bigfoot.Fordward();
				break;
			case (byte)'s': 
			case (byte)'S':
//				                                bigfoot.Backward();
				break;
			case (byte)'a': 
			case (byte)'A': 
//				                                bigfoot.TurnLeft();
				break;
			case (byte)'d': 
			case (byte)'D': 
//				                                bigfoot.TurnRigth();
				break;
			case (byte)'+': 
//				                                bigfoot.Power+=5;
//				                                Console.Write("Incressing power to " + bigfoot.Power);
				break;
			case (byte)'-': 
//				                                bigfoot.Power-=5;
//				                                Console.Write("Decressing power to " + bigfoot.Power);
				break;
			case (byte)'i': 
			case (byte)'I': 
//				servoPos+=servoSteep;
				break;
			case (byte)'K': 
			case (byte)'k': 
//				servoPos-=servoSteep;
				break;
			case (byte)'j': 
                        case (byte)'J':  
				//                                bigfoot.CamLeft();
				break;
			case (byte)'l': 
			case (byte)'L': 
				//                                bigfoot.CamRight();
				break;
			case (byte)'{':  
//				servoSteep -=0.5;
//				Console.WriteLine("Decreasing step to {0}",servoSteep );
				break;
			case (byte)'}':  
//				servoSteep +=0.5;
//				Console.WriteLine("Increasing step to {0}",servoSteep );
				break;
			case (byte)'c': 
			case (byte)'C': 
				this.record  = true;
				break;
			case (byte)'v': 
			case (byte)'V': 
				this.record = false;
				break;
			case (byte)'P': 
			case (byte)'p': 
//				this.saveVrml("test.wrl");
				break;
			case (byte)'o':
			case (byte)'O':
				this.LoadVrml();
				break;
				
			case (byte) 'r':
			case (byte) 'R':
				this.record = !this.record;
				break;
			case (byte) 't':
			case (byte) 'T':
				this.plot = !this.plot;
				break;
			case (byte)'.': 
//				                                bigfoot.Off();
//				sensor.Disconnect();
				System.Environment.Exit(0);
				break;
			default:
				Console.WriteLine(key);
//				                                bigfoot.Stop();
				break;
			}
		}
	
		protected override void ReshapeHandler(int width, int height)
		{
			
			Tao.OpenGl.Gl.glViewport(0, 0, width, height);
			
			Tao.OpenGl.Gl.glMatrixMode(Tao.OpenGl.Gl.GL_PROJECTION);
			Tao.OpenGl.Gl.glLoadIdentity();

			double aspect = 1.0 * width / height;
			Tao.OpenGl.Gl.glOrtho(-5000 * aspect, 5000 * aspect, -5000, 5000, -100000, 100000);

			Tao.OpenGl.Gl.glMatrixMode(Tao.OpenGl.Gl.GL_MODELVIEW);
		}
		
		
		override protected void HotKeys()
		{
			Console.WriteLine("Hot Keys");
			Console.WriteLine("".PadRight(30,'*'));
			Console.WriteLine("");
			Console.WriteLine("Zoom");
                        
			Console.WriteLine("\t m : Zoom in  | Scroll down");
			Console.WriteLine("\t n : Zoom Out | Scroll up");
                        
			Console.WriteLine("Navigation");
			Console.WriteLine("\t w  : FF");
			Console.WriteLine("\t s  : FWD");
			Console.WriteLine("\t d  : Right");
			Console.WriteLine("\t a  : Left");
			Console.WriteLine("\t +/-: Increase/Decrease Power ");
			
			Console.WriteLine("Scan");
			Console.WriteLine("\t \t { / } : Increase/Decrease Step");
                        
			Console.WriteLine("\t Manual");
			Console.WriteLine("\t\t i : Up");
			Console.WriteLine("\t\t j : Down");
			Console.WriteLine("\t\t k : Rigth");
			Console.WriteLine("\t\t l : Left");
			Console.WriteLine("\t\t r : Start/Stop record");
			Console.WriteLine("\t\t t : Show/Hide Laser");

			Console.WriteLine("\t Auto");
			Console.WriteLine("\t\t c : Start");
			Console.WriteLine("\t\t v : Cancel");
                        
			Console.WriteLine("Files");
			Console.WriteLine("\t o : Open");
			Console.WriteLine("\t p : Save");
		}
	
		override protected void MouseHandler(int button, int state, int x, int y)
		{
			//Console.WriteLine("Mx = {0}, My = {1}, B = {2}, State = {3}", x, y, button, state);
			last_pos_.X = x;
			last_pos_.Y = y;
			last_button = button; //0 left, 1 middle, 2 right
			last_state = state; //0 pressed, 1 unpressed
			if (state == 1)
			{
				switch (button){
				case 3://Scroll up
				{
					this.magnify_ -= 10;
					break;
				}
				case 4://Scroll down
				{
					this.magnify_ += 10;
					break;
				}
				default:break;
				}
			}
		}
		override protected void MotionHandler(int x, int y)
		{
			//Console.WriteLine("Mx = {0}, My = {1}", x, y);
			
			int dx = x - last_pos_.X;
			int dy = y - last_pos_.Y;
			
			if ((last_button == 0)&&(last_state == 0)) {
				setXRotation(x_rot_ + 8 * dy);
				setZRotation(z_rot_ + 8 * dx);
				
			} else if ((last_button == 2)&&(last_state == 0)) {
				setXRotation(x_rot_ + 8 * dy);
				setYRotation(y_rot_ + 8 * dx);
			}
			
			last_pos_.X = x;
			last_pos_.Y = y;
			
		}
		override protected void TimerHandler(int value)
		{
		
			ticks +=1 ;// value;
			//Console.WriteLine("Ticks {0}", ticks);
			Glut.glutPostRedisplay();
			Glut.glutTimerFunc(1, new Glut.TimerCallback(TimerHandler), value);

		}

#endregion
#region SetAxisRotation	
		private void setXRotation(int angle)
		{
			angle = normalizeAngle(angle);
			if (angle != x_rot_) {
				x_rot_ = angle;
				PaintHandler(); //??
			}
		}

		private void setYRotation(int angle)
		{
			angle = normalizeAngle(angle);
			if (angle != y_rot_) {
				y_rot_ = angle;
				PaintHandler(); //??
			}
		}

		private void setZRotation(int angle)
		{
			angle = normalizeAngle(angle);
			if (angle != z_rot_) {
				z_rot_ = angle;
				PaintHandler(); //??
			}
		}

		private int normalizeAngle(int angle)
		{
			while (angle < 0) {
				angle += 360 * 16;
			}
			
			while (angle > 360 * 16) {
				angle -= 360 * 16;
			}
			return angle;
		}
#endregion
		public void LoadVrml()
		{
			string filename = "/usr/src/git/URG-GL/tmp/vater.wrl"; 
			if (filename != null)
			{
				LoadVrml(filename);
			}
		}
		public void LoadVrml(string filename)
		{
			Console.WriteLine("Loading file {0}",filename);
			using (StreamReader sr = new StreamReader(filename)) 
			{
				string line;
				Line line_data = new Line();
				
				while ((line = sr.ReadLine()) != null) 
				{
					if(line.IndexOf("end") == 0)
						return;
					double x,y,z;
					try{
						char [] separators = new char [] {' '};
						string [] parts = line.Split (separators, StringSplitOptions.RemoveEmptyEntries);
						if (double.TryParse (parts [0] , out x) &&
						    double.TryParse (parts [1], out y) &&
						    double.TryParse (parts [2], out z)) 
						{
							line_data.points.Add(new Point3d<int>((int)(x*1000),(int)(y*1000),(int)(z*1000)));
						}
					}
					catch(Exception e){}
				}
				Console.WriteLine("finished load");
				this.saved_lines_data_.Clear();
				this.saved_lines_data_.Add(line_data);
			}
		}
	}
}
