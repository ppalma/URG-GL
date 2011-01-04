#region Header
//     LaserDataDraw.cs created with MonoDevelop
//     User: Patricio Palma S. as ppalma at 6:08 PM 12/24/2010
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
//     UNIT                : LaserDataDraw.cs
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
using MBF.Sensors;
using System.Collections.Generic;

namespace URG.GL
{


	public class LaserDataDraw:DataDraw
	{
		protected MBF.Sensors.URG  sensor;
		protected bool pre_record_ = false;
		protected Lines saved_lines_data_ = new Lines();
		protected Lines normal_lines_data_ = new Lines();
  		protected Line recent_line_data_ = new Line();
		public double data_max_;
		public int length_min_;
		public int length_max_;
		public bool h_type_ = false;
		public bool front_only_ = false;
		public List<int> data_ = new List<int>();
		public List<int> intensity_data_ = new List<int>();
		
		
		public LaserDataDraw():base()
		{
			data_max_ = 0;
			length_min_ = 0;
			length_max_ = 0;
			
            h_type_ = false;
			front_only_ = true; 
			pre_record_ = false;
			
			
		}
		
		public override void Show ()
		{
			SensorInitialization();
			base.Show ();
		}
		
		protected virtual void SensorInitialization()
		{

//			try {
				sensor = new MBF.Sensors.URG("/dev/ttyACM0");
				sensor.Connect();//TODO:Why connect ends app, unsing unplugged laser   ? ??????
				Console.WriteLine(sensor.GetVersionLines());
//			} 
//			catch (Exception e) {
//				sensor = new MBF.Sensors.URG();
//				Console.WriteLine("URG Error:[{0}]", e.Message);
//			}			
		
		}
		
		protected override void PaintHandler ()
		{
			Tao.OpenGl.Gl.glClearColor(0.0f, 0.0f, 0.0f, 1f);
			Tao.OpenGl.Gl.glClear(Tao.OpenGl.Gl.GL_COLOR_BUFFER_BIT | Tao.OpenGl.Gl.GL_DEPTH_BUFFER_BIT);
			Tao.OpenGl.Gl.glLoadIdentity();
			
			Tao.OpenGl.Gl.glRotated(x_rot_ / 16.0, 1.0, 0.0, 0.0);
			Tao.OpenGl.Gl.glRotated(y_rot_ / 16.0, 0.0, 1.0, 0.0);
			Tao.OpenGl.Gl.glRotated((z_rot_ / 16.0) + 90, 0.0, 0.0, 1.0);
			
			Line l = new Line();
			
			convertScanData(l);
			double ratio = (1.0 / 2.0) + (5.0 * magnify_ / 100.0);
			drawLaser(l,ratio);
			Tao.FreeGlut.Glut.glutSwapBuffers();
		}
		
		public void drawLaser(Line line, double ratio)
		{
			Tao.OpenGl.Gl.glColor3d(0.6, 0.0, 0.0);

			int index = 0;
			
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
		
		protected void convertScanData(Line line)
		{
			data_max_ = sensor.MaxScanLines;
			length_min_ = sensor.MinDistance;
			length_max_ = sensor.MaxDistance;
			
			
			line.timestamp = ticks;
			
			int n;
			data_ = sensor.GetFullGDData();
			if (data_ != null)
				n = data_.Count;
			else
				n = 0;
			
			if (n <= 0) {
				return;
			}
			
			int intensity_n = 0; //sensor.captureWithIntensity(data_, intensity_data_, null); //!!!!!!!!!!!!!1
			if (intensity_n == 0) {
				intensity_mode_ = false;
			}
			
			for (int i = 0; i < n; ++i) {
				int length = data_[i];
				if ((length <= length_min_) || (length >= length_max_)) {
					continue;
				}
				
				int index = i;     
				double radian = sensor.Index2Rad(index); 
				if (front_only_ && (System.Math.Abs(radian) > System.Math.PI / 2.0)) {
					continue;	
				}
				
				Point3d<int> p = new Point3d<int>();
				if (! h_type_) {
					p.x = (int)(length * System.Math.Cos(radian));
					p.y = (int)(length * System.Math.Sin(radian));
					p.z = 0;
				} else {
					p.x = 0;
					p.y = -(int)(length * System.Math.Cos(radian));
					p.z = -(int)(length * System.Math.Sin(radian));
				}
				
				if (h_type_) { 
					adjustTypeH(p, (int)(180.0 * radian / System.Math.PI)); 
				}
				
				rotateX(p, line.rotate.x);
				rotateY(p, line.rotate.y);
				rotateY(p, line.rotate.z);
				
				line.points.Add(p);
				if (intensity_n > index) {
					line.intensity.Add(intensity_data_[index]);
				}	
			}
		}
		private void rotateX(Point3d<int> point, int rotate_degree)
		{
			double radian = rotate_degree * System.Math.PI / 180.0;
			double z2 = (point.z * System.Math.Cos(-radian)) - (point.y * System.Math.Sin(-radian));
			double y2 = (point.z * System.Math.Sin(-radian)) + (point.y * System.Math.Cos(-radian));
			
			point.z = (int)(z2);
    		point.y = (int)(y2);
		}
		
		private void rotateY(Point3d<int> point, int rotate_degree)
		{
			double radian = -rotate_degree * System.Math.PI / 180.0;
			double z2 = (point.z * System.Math.Cos(-radian)) - (point.x * System.Math.Sin(-radian));
			double x2 = (point.z * System.Math.Sin(-radian)) + (point.x * System.Math.Cos(-radian));
			
			point.z = (int)(z2);
			point.x = (int)(x2);
  		}

		private void rotateZ(Point3d<int> point, int rotate_degree)
		{
			double radian = rotate_degree * System.Math.PI / 180.0;
			double x2 = (point.x * System.Math.Cos(-radian)) - (point.y * System.Math.Sin(-radian));
			double y2 = (point.x * System.Math.Sin(-radian)) + (point.y * System.Math.Cos(-radian));
			
			point.x = (int)(x2);
			point.y = (int)(y2);
		}
		
		private  void adjustTypeH(Point3d<int> p, int degree)
		{
			if ((degree > -20) && (degree < 20)) {
				rotateZ(p, -90);
				
				
			} else if (degree < -45) {
				rotateY(p, +(60 + 15 + 0));
				
				
			} else if (degree > +45) {
				rotateY(p, -(60 + 15 + 0));
				
			} else {
				p = new Point3d<int>(0, 0, 0);
			}
		}

	}
}
