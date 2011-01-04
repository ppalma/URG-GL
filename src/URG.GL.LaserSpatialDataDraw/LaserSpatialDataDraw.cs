#region Header
//     LaserSpatialDataDraw.cs created with MonoDevelop
//     User: Patricio Palma S. as ppalma at 12:47 PM 1/1/2011
//     --------------------------------------------------------------------------
//     URG.GL.LaserSpatialDataDraw
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
//     UNIT                : LaserSpatialDataDraw.cs
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
using Phidgets; 
using Phidgets.Events;
namespace URG.GL
{


	public class LaserSpatialDataDraw:LaserDataDraw
	{
		protected Spatial spatial;
		protected Point3d<double> acc;
		protected bool record = false, plot=false;
		protected bool save_ok_;

		public LaserSpatialDataDraw ():base()
		{
			 acc = new Point3d<double>(0,0,0);
		}
		public override void Show ()
		{
			SpatialInitialization();
			base.Show ();
		}
		override protected void HotKeys()
		{
			base.HotKeys();
			Console.WriteLine("Laser");

			Console.WriteLine("\t\t r : Start/Stop record");
			Console.WriteLine("\t\t t : Show/Hide Laser");
			
		}
		private void SpatialInitialization()
		{
			spatial = new Spatial();
			spatial.Attach += new AttachEventHandler(delegate(object sender, AttachEventArgs e){
				Console.WriteLine("Spatial {0} attached!", 
				                  e.Device.SerialNumber.ToString());
			});
			spatial.Detach += new DetachEventHandler(delegate(object sender, DetachEventArgs e){
				Console.WriteLine("Spatial {0} detached!", 
				                  e.Device.SerialNumber.ToString());
			});
			spatial.Error += new Phidgets.Events.ErrorEventHandler(delegate(object sender, Phidgets.Events.ErrorEventArgs e){
				Console.Write(e.Description);
			});
			
			spatial.SpatialData += new SpatialDataEventHandler(SpatialData);
			spatial.open();
			Console.WriteLine("Waiting for spatial to be attached....");
			spatial.waitForAttachment();
			spatial.DataRate = 80 ;
		}
		
		protected virtual void SpatialData(object sender, SpatialDataEventArgs e)
		{
				if (spatial.accelerometerAxes.Count > 0)
				{
				//Acc[0] = x, Acc[1] = y, Acc[2]= z
					acc.x = e.spatialData[0].Acceleration[2]; 
					acc.y = e.spatialData[0].Acceleration[1]; 
					acc.z = e.spatialData[0].Acceleration[0]; 
				}
//			Console.WriteLine(acc);
		}
		protected virtual void redrawCap()
		{
			Point3d<int> spatial_rotate = new Point3d<int>();
			spatial_rotate = getSpatialRotate();
			
			if(record && (!save_ok_)) {
    			save_ok_ = true;
			}
			redraw(spatial_rotate, record, plot);
		}
		protected virtual Point3d<int> getSpatialRotate()
		{
			Point3d<int> spatial_rotate = new Point3d<int>();
			
			if (spatial.Attached) {

				double length = Math.Sqrt((this.acc.x * this.acc.x) + (this.acc.z * this.acc.z));
				
				double x_rad = Math.Atan2(-this.acc.y, length);
				double z_rad = Math.Atan2(this.acc.z, this.acc.x);
				
				spatial_rotate.x = -(int)(180 * z_rad / Math.PI) + 90;
				spatial_rotate.y = -(int)(180 * x_rad / Math.PI);
				spatial_rotate.z = 0;
    		} else {
				spatial_rotate.x = 0;
			}
			
			return spatial_rotate;
		}
		protected virtual void  redraw(Point3d<int> spatial_rotate, bool record, bool no_plot)
		{
			no_plot_ = no_plot;

		  	Line line = new Line();
		  	line.rotate = spatial_rotate;
		  	convertScanData(line);
		
			if (record) {
				if (pre_record_ == false) {
					saved_lines_data_.Clear();
				}
				addSaveLine(line);
		
			} else {
				addTemporaryLine(line);
			}
		  
			if (line.points.Count > 0) {
				
				Line tmpl = new Line();
				tmpl.intensity = recent_line_data_.intensity;
				tmpl.points = recent_line_data_.points;
				tmpl.rotate = recent_line_data_.rotate;
				tmpl.timestamp = recent_line_data_.timestamp;
				
				recent_line_data_.intensity = line.intensity;
				recent_line_data_.points = line.points;
				recent_line_data_.rotate = line.rotate;
				recent_line_data_.timestamp = line.timestamp;
				
				line.intensity = tmpl.intensity;
				line.points = tmpl.points;
				line.rotate = tmpl.rotate;
				line.timestamp = tmpl.timestamp;

		  	}
		
			pre_record_ = record;
			PaintHandler(); //??
		}
		override protected void IdleHandler()
		{
			redrawCap();
		}
		override protected void PaintHandler()
		{
			Tao.OpenGl.Gl.glClearColor(0.0f, 0.0f, 0.0f, 1f);
			Tao.OpenGl.Gl.glClear(Tao.OpenGl.Gl.GL_COLOR_BUFFER_BIT | Tao.OpenGl.Gl.GL_DEPTH_BUFFER_BIT);
			Tao.OpenGl.Gl.glLoadIdentity();
			
			Tao.OpenGl.Gl.glRotated(x_rot_ / 16.0, 1.0, 0.0, 0.0);
			Tao.OpenGl.Gl.glRotated(y_rot_ / 16.0, 0.0, 1.0, 0.0);
			Tao.OpenGl.Gl.glRotated((z_rot_ / 16.0) + 90, 0.0, 0.0, 1.0);
			
			while ((normal_lines_data_.Count > 0) &&
			       ((normal_lines_data_[0].timestamp + AliveMsec) < ticks)) {
				normal_lines_data_.Remove(normal_lines_data_[0]);
				
			}
			
			Tao.OpenGl.Gl.glBegin(Tao.OpenGl.Gl.GL_POINTS);
			
			double ratio = (1.0 / 2.0) + (5.0 * magnify_ / 100.0);
			
			if (! no_plot_) {
				
				foreach (Line line_it in normal_lines_data_) {
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
			Tao.FreeGlut.Glut.glutSwapBuffers();
		}

		private void addSaveLine(Line line)
		{
			saved_lines_data_.Add(line);
		}

		public void addTemporaryLine(Line line)
		{
			normal_lines_data_.Add(line);
		}
		protected override void KeyboardHandler(byte key, int x, int y)
		{
			switch (key)
			{
			case (byte)'r': 
			case (byte)'R': 
				this.record = !this.record;
				break;
			case (byte) 't':
			case (byte) 'T':
				this.plot = !this.plot;
				break;
			default:
				base.KeyboardHandler(key,x,y);
				break;
			}
		}
	}
}
