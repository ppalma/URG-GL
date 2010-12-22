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
using Tao.FreeGlut;
using System.Collections.Generic;
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
		
		private int ticks;
		public int magnify_ = 50;
			public bool no_plot_ = false ;
		Lines saved_lines_data_ = new Lines();
		Line recent_line_data_ = new Line();
		public bool intensity_mode_ = false ;
		List<Point3d<double>> color_table_ = new List<Point3d<double>>();
		
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
		
		protected override void ReshapeHandler(int width, int height)
		{
	
			Tao.OpenGl.Gl.glViewport(0, 0, width, height);

  			Tao.OpenGl.Gl.glMatrixMode(Tao.OpenGl.Gl.GL_PROJECTION);
  			Tao.OpenGl.Gl.glLoadIdentity();

  			double aspect = 1.0 * width / height;
  			Tao.OpenGl.Gl.glOrtho(-5000 * aspect, 5000 * aspect, -5000, 5000, -100000, 100000);

  			Tao.OpenGl.Gl.glMatrixMode(Tao.OpenGl.Gl.GL_MODELVIEW);
		}
		
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

	}
}
