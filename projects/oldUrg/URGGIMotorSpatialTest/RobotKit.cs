using System;
using MBF.Motors;
using MBF.RobotKits;
using MBF.RobotKits.Phidgets;
using MBF.Core;
using Phidgets;

namespace URGGlMotorTest
{
	public class RobotKit
	{
		private Sabertooth2x5 sbt;
		private Servo servos;
		private double yPos = 120;
		private double xPos = 115;
		private double power = 70;
		private double camVStep = 3;
		
		public double Power {
			get {
				return power;
			}
			set {
				power = value;
			}
		}
		
	
		public RobotKit()
		{
			sbt = new Sabertooth2x5();
			sbt.RaiseDesignerCreatedEvent();
			sbt.SerialPort.Open();
			
			//Fixing some errors
			sbt.Motor1.Speed.Maximum = 100;
			sbt.Motor1.Speed.Minimum = 0;
			
			sbt.Motor1 = sbt.Motor1;
			
			sbt.Motor2.Speed.Maximum = 100;
			sbt.Motor2.Speed.Minimum = 0;
			
			sbt.Motor2 = sbt.Motor2;
			
			
			servos = new Servo();
			servos.open();
			System.Threading.Thread.Sleep(200);
			try
			{
				servos.servos[0].Engaged = true;
				servos.servos[0].Position = xPos;
			}
			catch (Exception)
			{
				//Console.WriteLine("position value out of bounds!");
			}
			
			try
			{
				servos.servos[3].Engaged = true;
				servos.servos[3].Position = yPos;
			}
			catch (Exception)
			{
				//Console.WriteLine("position value out of bounds!");
			}
			
			
			//sbt.InitialiseSimulation(sbt);
		}
		
		public void Off()
		{
			sbt.SerialPort.Close();
			servos.close();
		}
		
		public double YPos
		{
			set {yPos =  value;}
			get {return yPos;}
		}
		
		public double XPos
		{
			set {xPos = value;}
			get {return xPos;}
		}
		
		public double CamVStep {
			get {
				return camVStep;
			}
			set {
				camVStep = value;
			}
		}
		
		
		public void aplyServos()
		{
			servos.servos[0].Position = xPos;
			servos.servos[3].Position = yPos;
		}
		
		public void CamUp()
		{
			try
			{
				xPos += CamVStep;
				servos.servos[0].Position = xPos;
			}
			catch (PhidgetException)
			{
				//Console.WriteLine("position value out of bounds!");
			}
		}
		public void CamDown()
		{
			try
			{
				xPos -= CamVStep;
				servos.servos[0].Position = xPos;
			}
			catch (PhidgetException)
			{
				//Console.WriteLine("position value out of bounds!");
            }
		}
		public void CamRight()
		{
			try
			{
				yPos += 3;
				servos.servos[3].Position = yPos;
			}
			catch (PhidgetException)
			{
				//Console.WriteLine("position value out of bounds!");
			}
		}
		public void CamLeft()
		{
			try
			{
				yPos -= 3;
				servos.servos[3].Position = yPos;
			}
			catch (PhidgetException)
			{
                //Console.WriteLine("position value out of bounds!");
            }
		}
		
		public void Fordward()

		{
			sbt.Motor1.Direction.Value = Spin.CCW;
			sbt.Motor2.Direction.Value = Spin.CCW;
			sbt.Motor1.Speed.Value = power;
			sbt.Motor2.Speed.Value = power;
		}
		public void Backward()
		{
			sbt.Motor1.Direction.Value = Spin.CW;
			sbt.Motor2.Direction.Value = Spin.CW;
			sbt.Motor1.Speed.Value = power;
			sbt.Motor2.Speed.Value = power;
		}
		public void TurnLeft()
		{
			sbt.Motor1.Direction.Value = Spin.CW;
			sbt.Motor2.Direction.Value = Spin.CCW;
			sbt.Motor1.Speed.Value = power;
			sbt.Motor2.Speed.Value = power;
		}
		public void TurnRigth()
		{
			sbt.Motor1.Direction.Value = Spin.CCW;
			sbt.Motor2.Direction.Value = Spin.CW;
			sbt.Motor1.Speed.Value = power;
			sbt.Motor2.Speed.Value = power;
		}
		
		public void Stop()
		{
			sbt.Motor1.Speed.Value = 0;
			sbt.Motor2.Speed.Value = 0;
		}
		
	}
}