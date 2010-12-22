using System;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security;
using MBF.Sensors;

namespace Laser
{
	class URGTest
	{
		public static void Main (string[] args)
		{
			Console.WriteLine("Hello URG!");
			
			//OOP tests...
			URG urg = new URG("/dev/ttyACM0");
			
			//Connect
			urg.Connect();

			//Version Lines
			//string version = urg.GetVersionLines();
			//Console.WriteLine(version);
			
			//URG parameters from properties...
			//Console.WriteLine("Area Front: {0}", urg.FrontArea);
			//Console.WriteLine("Area Max: {0}", urg.MaxArea);
			//Console.WriteLine("Area Min: {0}", urg.MinArea);
			//Console.WriteLine("Area Total: {0}", urg.TotalArea);
			//Console.WriteLine("Distance Max: {0}", urg.MaxDistance);
			//Console.WriteLine("Distance Min: {0}", urg.MinDistance);
			//Console.WriteLine("Scan RPM: {0}", urg.ScanRPM);
			//Console.WriteLine("Sensor Type: {0}", urg.SensorType);

			//URG get parameter data from functions..
			//Console.WriteLine("Distance Max: {0}", urg.UrgMaxDistance());
			//Console.WriteLine("Distance Min: {0}", urg.UrgMinDistance());
			
			//Full GD Data
			//List<int> data = urg.GetFullGDData();
			//int i = 0;
			//foreach (int point in data){
			//	Console.WriteLine("Point {1}: \t{0}",point,i++);	
			//}
				
			//MD Data
			//List<List<int>> data = urg.GetFullMDData(1);
			//int i = 0;
			//foreach( List<int> list in data)
			//	foreach(int point in list)
			//		Console.WriteLine("Point {0}: \t{1}",i++,point);
			
			//Partial GD Scan
			//List<int> data = urg.PartialGDScan(6,urg.FrontArea,urg.FrontArea+10);
			//int i =0;
			//foreach(int point in data){
			//	Console.WriteLine("Point {1}: \t{0}",point,i++);
			//}

			//Partial GD Scan
			//List<int> data2 = urg.PartialMDScan(6,urg.FrontArea,urg.FrontArea+10);
			//i=0;
			//foreach(int point in data2){
			//	Console.WriteLine("Point {1}: \t{0}",point,i++);
			//}
			
			//Index to Rad
		//	double val=urg.UrgIndex2Rad(6);
		//	Console.WriteLine("index to rad (val 6): \t{0}",val);
			
			//Rad to index
		//	double val2=urg.UrgRad2Index((int)val);
		//	Console.WriteLine("rad to index (val {0}): \t{1}",((int)val),val2);
			
			// Index to Deg
//			double val=urg.UrgIndex2Deg(6);
//			Console.WriteLine("index2Deg (val 6): \t{0}",val);
			
			// Deg to index
		//	double val2=urg.Urg_Deg2Index(6);
		//	Console.WriteLine("Deg to index (val 6): \t{0}",val2);
			
			
			//Close Connection
//			urg.Disconnect();

			
			
			
			//Native tests...
			int ret;
			
	//		IntPtr urg = URG.Native.Urg_Initialise("/dev/ttyACM0");

			//Requesting URG parameters
//			IntPtr pParam = URG.Native.Urg_GetParameters(urg);
//			URG.Native.urg_parameter parameter = new URG.Native.urg_parameter();
//			parameter =  (URG.Native.urg_parameter)Marshal.PtrToStructure(pParam, typeof(URG.Native.urg_parameter));
//			Console.WriteLine("Area Front: {0}", parameter.area_front_);
//			Console.WriteLine("Area Max: {0}", parameter.area_max_);
//			Console.WriteLine("Area Min: {0}", parameter.area_min_);
//			Console.WriteLine("Area Total: {0}", parameter.area_total_);
//			Console.WriteLine("Distance Max: {0}", parameter.distance_max_);
//			Console.WriteLine("Distance Min: {0}", parameter.distance_min_);
//			Console.WriteLine("Scan RPM: {0}", parameter.scan_rpm_);
//			Console.WriteLine("Sensor Type: {0}", new string(parameter.sensor_type));


			//Requesting Version Lines
//			IntPtr lines = URG.Native.Urg_GetVersionLines(urg);
//			string s1 = Marshal.PtrToStringAnsi(lines);
//			Console.WriteLine(s1);
			
			//Using GD request data
//			IntPtr pData = URG.Native.Urg_RequestFullGDData(urg);
//			int n = URG.Native.GetInteger(URG.Native.URGIntegers.DATA_READED);
//			int[] data = new int[n];
//			Marshal.Copy(pData, data, 0, n);
//			for (int i = 0; i < n; ++i) {
//		    /*Neglect the distance less than  urg_minDistance()  */
//    			Console.WriteLine("index: {0} data:{1}", i, data[i].ToString());
//  		}
			
			//Using MD request data
//			int Captimes = 10;
//			int previous_timestamp;
// 			int timestamp;
//	    	int remain_times;
//
//			ret = URG.Native.Urg_InitMDRequest(urg, Captimes);
//			//ret = URG.Native.Urg_InitMDRequest(urg, URG.Native.InfiniteTimes);
//			if (ret >= 0)
//			{				
//				IntPtr pData; 
//				int i=0;
//				for (i = 0; i < Captimes; ++i) {
//				//while(true){	
//    				/* Reception */
//    				pData = URG.Native.Urg_RequestFullMDData(urg);
//					int n = URG.Native.GetInteger(URG.Native.URGIntegers.DATA_READED);
//					int[] data = new int[n];
//					Marshal.Copy(pData, data, 0, n);
//					System.Console.WriteLine("n = {0}", n);
//    				if (n < 0) {
//						//TODO: Export urg_exit
//    		  			URG.Native.Urg_Exit(urg, "urg_receiveData()");
//    				} else if (n == 0) {
//      					System.Console.WriteLine("n == 0");
//      					--i;
//      					continue;
//    				}
//
//	    			/* Display the front data with timestamp */
//    				/* Delay in reception of data at PC causes URG to discard the data which
//       				cannot be transmitted. This may  results in remain_times to become
//       				discontinuous */
//    				previous_timestamp = URG.Native.GetInteger(URG.Native.URGIntegers.TIMESTAMP);
//    				timestamp = URG.Native.Urg_RecentTimestamp(urg);
//	    			remain_times = URG.Native.Urg_RemainCaptureTimes(urg);
//
//    				/* Neglect the distance data if it is less than urg_minDistance() */
//    				System.Console.WriteLine("{0}/{1}: {2} [mm], {3} [msec], ({4})",
//        	   			remain_times, Captimes, URG.Native.GetInteger(URG.Native.URGIntegers.AREA_FRONT), timestamp,
//    	       			timestamp - previous_timestamp);
//
//	    			System.Console.WriteLine("{0}, {1}", i, remain_times);
//
//    				if (remain_times <= 0) {
//      					break;
//    				}
// 				}
//			}
			

			//Request partial GD data
//			IntPtr pParam = URG.Native.Urg_GetParameters(urg);
//			URG.Native.urg_parameter parameter = new URG.Native.urg_parameter();
//			parameter =  (URG.Native.urg_parameter)Marshal.PtrToStructure(pParam, typeof(URG.Native.urg_parameter));
//
//			int buffersize = 6;
//            IntPtr pData= URG.Native.Urg_PartialGDScan(urg,parameter.area_front_, parameter.area_front_ + 20, buffersize);
//           
//            int[] data = new int[buffersize];
//            Marshal.Copy(pData, data, 0, buffersize);
//            for(int i =0; i<buffersize;i++)
//                Console.WriteLine(data[i]);

			//Request partial MD data
//			IntPtr pParam = URG.Native.Urg_GetParameters(urg);
//			URG.Native.urg_parameter parameter = new URG.Native.urg_parameter();
//			parameter =  (URG.Native.urg_parameter)Marshal.PtrToStructure(pParam, typeof(URG.Native.urg_parameter));
//
//			int buffersize = 6;
//            IntPtr pData= URG.Native.Urg_PartialMDScan(urg,parameter.area_front_, parameter.area_front_ + 20, buffersize);
//           
//            int[] data = new int[buffersize];
//            Marshal.Copy(pData, data, 0, buffersize);
//            for(int i =0; i<buffersize;i++)
//                Console.WriteLine(data[i]);
			
			//index to rad
//			double val= URG.Native.Urg_Index2Rad(urg,6);
//			Console.WriteLine("index to rad(val 6):\t {0}",val);
			
			//rad to index
//			double val=URG.Native.Urg_Rad2Index(-2);
//			Console.WriteLine("Rad to index(val 6):\t {0}",val);
			
			//index to deg
//			double val= URG.Native.Urg_Index2Deg(urg,6);
//			Console.WriteLine("index to Deg(val 6):\t {0}",val);
			
			//deg to index
//			double val= URG.Native.Urg_Deg2Index(urg,6);
//			Console.WriteLine("Deg to index(val 6):\t {0}",val);
			
	//		URG.Native.Urg_Finalise(urg);
	
			
		}
	}
}
