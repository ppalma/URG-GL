#region Header
//     LaserSpatialServoDataDraw.cs created with MonoDevelop
//     User: Patricio Palma S. as ppalma at 4:39 PM 1/1/2011
//     --------------------------------------------------------------------------
//     URG.GL.LaserSpatialServoDataDraw
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
//     UNIT                : LaserSpatialServoDataDraw.cs
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


	public class LaserSpatialServoDataDraw:LaserSpatialDataDraw
	{
		
		protected  Servo servo;// = new Servo();
		protected double servoSteep;
		protected double servoPos;
		protected double servoInitPos;
		protected double servoFinalPos;

		
		public LaserSpatialServoDataDraw ():base()
		{
			servoInitPos = 98;
			servoSteep = 0.5;
			servoPos = 90;
			servoFinalPos = 150;
		}
		override public void Show()
		{
			ServoInitialization();
		
			base.Show();
		}
		override protected void HotKeys()
		{
			base.HotKeys();
			Console.WriteLine("Servos");

			Console.WriteLine("\t\t i : servo up");
			Console.WriteLine("\t\t k : servo down");
			Console.WriteLine("\t \t { / } : Increase/Decrease Step");
			
		}
		protected override void KeyboardHandler(byte key, int x, int y)
		{
			switch (key)
			{
			case (byte)'i': 
			case (byte)'I': 
				servoPos+=servoSteep;
				break;
			case (byte) 'k':
			case (byte) 'K':
				servoPos-=servoSteep;
				break;
			case (byte)'{':  
				servoSteep -=0.5;
				Console.WriteLine("Decreasing step to {0}",servoSteep );
				break;
			case (byte)'}':  
				servoSteep +=0.5;
				Console.WriteLine("Increasing step to {0}",servoSteep );
				break;
			default:
				base.KeyboardHandler(key,x,y);
				break;
			}
		}
		private void ServoInitialization()
		{
			servo = new Servo();
			servo.Attach += new AttachEventHandler(delegate(object sender, AttachEventArgs e){
				Console.WriteLine("Servo {0} attached!", e.Device.SerialNumber.ToString());
			});
			servo.Detach += new DetachEventHandler(delegate (object sender, DetachEventArgs e){
				Console.WriteLine("Servo {0} detached!", e.Device.SerialNumber.ToString());
			});
			servo.Error += new Phidgets.Events.ErrorEventHandler(delegate(object sender, ErrorEventArgs e){
				Console.WriteLine(e.Description);
			});

			servo.open();
			  Console.WriteLine("Waiting for Servo to be attached...");
                servo.waitForAttachment();
			if(servo.Attached)
				servo.servos[0].Position = servoInitPos;


		}
		override protected void redrawCap()
		{
			Point3d<int> spatial_rotate = new Point3d<int>();
			spatial_rotate = getSpatialRotate();
			
			UpdateServoPos();

			if(record && (!save_ok_)) {
    			save_ok_ = true;
				servoPos = servoInitPos;
			}
			
  			redraw(spatial_rotate, record, plot);
		}
		private void  UpdateServoPos()
		{
			if (record && servo.Attached)
			{
				if(servoPos > servoFinalPos)
				{
					record = false;
					plot = false;
					servoPos = servoInitPos;
				}
				servoPos = servoPos + servoSteep;
			}
					System.Threading.Thread.Sleep(10);
					servo.servos[0].Position = servoPos;
		}
	}
}
