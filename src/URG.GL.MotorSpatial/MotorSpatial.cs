#region Header
//     MotorSpatial.cs created with MonoDevelop
//     User: Patricio Palma S. as ppalma at 5:20 PM 12/29/2010
//     --------------------------------------------------------------------------
//     URG.GL.MotorSpatial
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
//     UNIT                : MotorSpatial.cs
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


	public class MotorSpatial:LaserDataDraw
	{
		protected Servo servo;
		protected Spatial spatial;
		protected double accx, accy, accz;
		protected double servoInitPos = 70;
		
		public MotorSpatial ():base()
		{
			accx = accy = accz = 0;
		}
		
		public override void Show ()
		{
			ServoInitialization();
			SpatialInitialization();
			
			base.Show();
		}
		protected void ServoInitialization()
		{
			servo = new Servo();
			servo.Attach += new AttachEventHandler(delegate (object sender, AttachEventArgs e){
				Console.WriteLine("Servo {0} attached!", e.Device.SerialNumber.ToString());
			} );
			
			servo.Detach += new DetachEventHandler(delegate (object sender, DetachEventArgs e){
				Console.WriteLine("Servo {0} detached!", e.Device.SerialNumber.ToString());
			} );
			servo.Error += new Phidgets.Events.ErrorEventHandler(delegate (object sender, Phidgets.Events.ErrorEventArgs e){
				Console.WriteLine("Servo: {0}",e.Description);
			} );

			servo.open();
			Console.WriteLine("Servo Open!");
			servo.servos[0].Position = servoInitPos;
		}

		private void SpatialInitialization()
		{
			spatial = new Spatial();
			spatial.Attach += new AttachEventHandler(delegate (object sender, AttachEventArgs e){
				Console.WriteLine("Spatial {0} attached!", e.Device.SerialNumber.ToString());
			});
            spatial.Detach += new DetachEventHandler(delegate(object sender, DetachEventArgs e){
				Console.WriteLine("Spatial {0} detached!", e.Device.SerialNumber.ToString());
			});
			spatial.Error += new Phidgets.Events.ErrorEventHandler(delegate 
			                                                       (object sender, Phidgets.Events.ErrorEventArgs e){
				Console.Write(e.Description);
			});
			
			spatial.SpatialData += new SpatialDataEventHandler(spatial_SpatialData);
			spatial.open();
			Console.WriteLine("Waiting for spatial to be attached....");
			spatial.waitForAttachment();
			spatial.DataRate = 400 ;
		}
		private void spatial_SpatialData(object sender, SpatialDataEventArgs e)
		{
				if (spatial.accelerometerAxes.Count > 0)
				{
				//Acc[0] = x, Acc[1] = y, Acc[2]= z
					accx = e.spatialData[0].Acceleration[2]; 
					accy = e.spatialData[0].Acceleration[1]; 
					accz = e.spatialData[0].Acceleration[0]; 
				}
		}
	}
}
