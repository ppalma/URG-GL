#region Header
//     URG.cs created with MonoDevelop
//     User: Patricio Palma S. as ppalma at 5:53 PM 12/22/2010
//     --------------------------------------------------------------------------
//     MBF.Sensors
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
//     UNIT                : URG.cs
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
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security;

namespace MBF.Sensors
{
	public class URG //: MBF.Core.Entity //TODO: inherit from MBF.SomeSensor....or something MBFish
	{
		private IntPtr pUrg;
		private URG.Native.urg_parameter parameter;
			
		protected string port;
		protected bool isConnected = false;
		private List<int> ldata;
		private List<int> lintensity;
		
		
		public URG() { }
		
		public URG (string port) 
		{
			this.port = port;
		}
		
		public virtual void Connect()
		{
			try {
				pUrg = URG.Native.Urg_Initialise(port);
		
				//Fill parameters..
				IntPtr pParam = URG.Native.Urg_GetParameters(pUrg);
				parameter = new URG.Native.urg_parameter();
				parameter =  (URG.Native.urg_parameter)Marshal.PtrToStructure(pParam, typeof(URG.Native.urg_parameter));

				if (pUrg != IntPtr.Zero)
					isConnected = true;
			} catch (Exception e) {
				//IConsole.Write("URG Connection Error:", e.Message);					
				Console.WriteLine("URG Connection Error: {0} ", e.Message);
			}
		}

		public virtual void Disconnect()
		{
			try {
				URG.Native.Urg_Finalise(pUrg);
			} catch (Exception e) {
				//IConsole.Write("URG Connection Error:", e.Message);					
				Console.WriteLine("URG Connection Error: {0} ", e.Message);
			}
		}

		public override string ToString()
		{
			return GetVersionLines();
		}
		
		public string GetVersionLines()
		{
			if (!isConnected)
				return "URG Device must be connected to get version lines";
			try {
				IntPtr lines = URG.Native.Urg_GetVersionLines(pUrg);
				string s1 = Marshal.PtrToStringAnsi(lines);
				return s1;
			} catch (Exception e) {
				//IConsole.Write("URG Device must be connected to get version lines:", e.Message);					
				return "URG GetVersionLines Error: " + e.Message;
			}


		}

		
		public List<int> GetFullGDData()
		{
			if (!isConnected)
				return null;

			lock (this) {
				
				try {
					//Using GD request data
					IntPtr pData = URG.Native.Urg_RequestFullGDData(pUrg);
					int n = URG.Native.GetInteger(URG.Native.URGIntegers.DATA_READED);
					int[] data = new int[n];
					Marshal.Copy(pData, data, 0, n);
					
					//ldata = new List<long>(data.Length);
					ldata = new List<int>(data.Length);
					ldata.AddRange(data);
					
					//for (int i = 0; i < n; ++i) {
			    		/*Neglect the distance less than  urg_minDistance()  */
	    				//Console.WriteLine("index: {0} data:{1}", i, data[i].ToString());
					//	ldata.Add((long)data[i]);
		  			//}
				
					return ldata;
				} catch (Exception e) {
					//IConsole.Write("URG Device must be connected to get data:", e.Message);					
					return null;
				}

			}
		}
		
		public List<int> PartialGDScan(int buffersize,int first, int last){
			List<int> listp = new List<int>();
			IntPtr pParam = URG.Native.Urg_GetParameters(this.pUrg);
			this.parameter =  (URG.Native.urg_parameter)Marshal.PtrToStructure(pParam, typeof(URG.Native.urg_parameter));
            IntPtr pData= URG.Native.Urg_PartialGDScan(this.pUrg,first, last, buffersize);
            int[] data = new int[buffersize];
            Marshal.Copy(pData, data, 0, buffersize);
            for(int i =0; i<buffersize;i++)
                listp.Add(data[i]);
			return listp;
		}
		
		public List<List<int>> GetFullMDData(int capturetimes)
		{
			if (!isConnected)
				return null;

			lock (this) {
				
				try {
					int Captimes = capturetimes;
					int previous_timestamp;
					int timestamp;
					int remain_times;
					List<List<int>> tlist = new List<List<int>>();
					//Using MD request data
					int ret = URG.Native.Urg_InitMDRequest(pUrg, Captimes);
					
					if (ret >= 0){
						
						IntPtr pData; 
						int i=0;
						for (i = 0; i < Captimes; ++i) {
							/* Reception */
							pData = URG.Native.Urg_RequestFullMDData(pUrg);
							int n = URG.Native.GetInteger(URG.Native.URGIntegers.DATA_READED);
							int[] data = new int[n];
							Marshal.Copy(pData, data, 0, n);
							//System.Console.WriteLine("n = {0}", n);
						
							if (n < 0) {
								//TODO: Export urg_exit
								URG.Native.Urg_Exit(pUrg, "urg_receiveData()");
							} 
							
							else if (n == 0) {
								//System.Console.WriteLine("n == 0");
								--i;
								continue;
							}
			
							/* Display the front data with timestamp */
							/* Delay in reception of data at PC causes URG to discard the data which
								cannot be transmitted. This may  results in remain_times to become
								discontinuous */
							previous_timestamp = URG.Native.GetInteger(URG.Native.URGIntegers.TIMESTAMP);
							timestamp = URG.Native.Urg_RecentTimestamp(pUrg);
							remain_times = URG.Native.Urg_RemainCaptureTimes(pUrg);
							ldata = new List<int>(data.Length);
							ldata.AddRange(data);
							tlist.Add(ldata);
							}
						}				
					return tlist;
				}
				catch (Exception e) {
				return null;
				}
			}
		}
		
		public List<int> GetFullGDIntensityData()
		{
			if (!isConnected)
				return null;

			lock (this) {
				
				try {
		
						IntPtr pData = URG.Native.Urg_RequestFullGDIntensityData(pUrg);
									
						int n = URG.Native.GetInteger(URG.Native.URGIntegers.DATA_READED);
									
						IntPtr pIntensity = URG.Native.GetIntensity();
									
						int[] data = new int[n];
						int[] intensity = new int[n];
						
						
						Marshal.Copy(pData, data, 0, n);
						Marshal.Copy(pIntensity, intensity, 0, n);
												
						ldata = new List<int>(data.Length);
						lintensity = new List<int>(intensity.Length);
						
						ldata.AddRange(data);
						lintensity.AddRange(intensity);
			
						return ldata;
				} catch (Exception e) {
					//IConsole.Write("URG Device must be connected to get data:", e.Message);					
					return null;
				}
			}
		}
		
		public List<int> GetFullMDIntensityData(int capturetimes)
		{
			
			if (!isConnected)
				return null;

			lock (this) {
				
				try {
					    int Captimes = capturetimes;
						int previous_timestamp;
						int timestamp;
						int remain_times;
						
						int ret = URG.Native.Urg_InitMDIntensityRequest(pUrg,Captimes);
						if (ret >= 0){
						
				        ldata = new List<int>();
					    lintensity = new List<int>();
				        IntPtr pData; 
						IntPtr pIntensity;
						int i;
				 
						for (i = 0; i < Captimes; ++i) {
							/* Reception */
							pData = URG.Native.Urg_RequestFullMDIntensityData(pUrg);
							pIntensity = URG.Native.GetIntensity();
						
							int n = URG.Native.GetInteger(URG.Native.URGIntegers.DATA_READED);
							
					        int[] data = new int[n];
					        int[] intensity = new int[n];
					        
					        
							Marshal.Copy(pData, data, 0, n);
							Marshal.Copy(pIntensity, intensity, 0, n);
										
					        lintensity.Add(intensity[parameter.area_front_]);      
					        ldata.Add(data[parameter.area_front_]);
							
					 		if (n < 0) {
								//TODO: Export urg_exit
								URG.Native.Urg_Exit(pUrg, "urg_receiveData()");
							} 
							
							else if (n == 0) {
								//System.Console.WriteLine("n == 0");
								--i;
								continue;
							}
			
							
							previous_timestamp = URG.Native.GetInteger(URG.Native.URGIntegers.TIMESTAMP);
							timestamp = URG.Native.Urg_RecentTimestamp(pUrg);
							remain_times = URG.Native.Urg_RemainCaptureTimes(pUrg);
						
							}
						}				                    
						return ldata;
				}
				catch (Exception e) {
				
				return null;
				}
			}	
		}
		
		public List<int> PartialMDScan(int buffersize, int first, int last){
			List<int> listp = new List<int>();
			//IntPtr pParam = URG.Native.Urg_GetParameters(this.pUrg);
			//parameter =  (URG.Native.urg_parameter)Marshal.PtrToStructure(pParam, typeof(URG.Native.urg_parameter));
            IntPtr pData=URG.Native.Urg_PartialMDScan(this.pUrg,first, last, buffersize);
            int[] data = new int[buffersize];
            Marshal.Copy(pData, data, 0, buffersize);
            for(int i =0; i<buffersize;i++)
                listp.Add(data[i]);
			return listp;
		}
		
		public double Index2Rad(int index) 
		{
			if (!isConnected)
				return 0;

			try {

  				int index_from_front = index - parameter.area_front_;
  				return index_from_front * (2.0 * System.Math.PI) / parameter.area_total_;

			} catch (Exception e) {
				//IConsole.Write("URG Device must be connected to get data:", e.Message);					
				return 0;
			}

		}

		public int UrgMinDistance()
		{
			if (!isConnected)
				return 0;

			return Native.Urg_MinDistance(pUrg);
		}
		
		public int UrgMaxDistance()
		{
			if (!isConnected)
				return 0;

			return Native.Urg_MaxDistance(pUrg);
		}
		
		public double UrgIndex2Rad(int index)
		{
			if (!isConnected)
				return 0;

			return Native.Urg_Index2Rad(pUrg, index);
		}


		public double UrgIndex2Deg(int index)
		{
			if (!isConnected)
				return 0;

			return Native.Urg_Index2Deg(pUrg,index);
		}

		public double UrgRad2Index(int index)
		{
			if (!isConnected)
				return 0;

			return Native.Urg_Rad2Index(pUrg, index);
		}

		public double Urg_Deg2Index(int index)
		{
			if (!isConnected)
				return 0;

			return Native.Urg_Deg2Index(pUrg, index);
		}

		public int RecentTimestamp()
		{
			if (!isConnected)
				return 0;

			return Native.Urg_RecentTimestamp(pUrg);
		}

		public int FrontArea
		{
			get { return parameter.area_front_; }
		}
		
		public int MaxArea
		{
			get { return parameter.area_max_; }
		}

		public int MinArea
		{
			get { return parameter.area_min_; }
		}

		public int MaxScanLines
		{
			get { return parameter.area_max_ + 1; }
		}
		
		public int TotalArea
		{
			get { return parameter.area_total_; }
		}

		public int MaxDistance
		{
			get { return parameter.distance_max_; }
		}

		public int MinDistance
		{
			get { return parameter.distance_min_; }
		}

		public int ScanRPM
		{
			get { return parameter.scan_rpm_; }
		}

		public string SensorType
		{
			get { return new string(parameter.sensor_type); }
		}
		
		public List<int> Intensity
		{
			get{
				return lintensity;
			}
		}

		public virtual bool IsConnected
		{
			get 
			{ 
				return isConnected;
			}
		}
		
		
		
		
		
		public class Native {
			private const string MBF_URG_NATIVE_LIBRARY = "MBF.URG.dll";

			public const int SerialErrorStringSize = 256;
	  		public const int RingBufferSizeShift = 10;
	  		public const int RingBufferSize = 1 << RingBufferSizeShift;
			public const int UrgParameterLines = 8 + 1 + 1;
	  		public const int SensorTypeLineMax = 80;
			public const int UrgLineWidth = 64 + 1 + 1;    /*!< Maximum length of a line */
	  		public const int UrgInfinityTimes = 0;         /*!< continuous data transmission */
			public const int NCCS = 32;         
			
			private const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;
			
			public const int InfiniteTimes = 0;
			
			public enum URGIntegers {
				DATA_READED 				= 10000,
				TIMESTAMP					= 10001,
				PREVIOUS_TIMESTAMP 			= 10002,
				REMAIN_TIMES				= 10003,
				AREA_FRONT					= 10004,
	    		LINES_MAX					= 10005
			}
	
			public enum LaserState{
	  			Off = 0,
	  			On,
	  			Unknown,
			}
	
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
	        public static extern IntPtr Urg_Initialise([MarshalAs(UnmanagedType.LPStr)]String device);
			
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
	        public static extern void Urg_Exit(IntPtr urg,[MarshalAs(UnmanagedType.LPStr)]String mes);
			
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
	        public static extern void Urg_Finalise(IntPtr urg);
			
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
	        public static extern IntPtr Urg_RequestFullGDData(IntPtr urg);
	
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
	        public static extern int Urg_InitMDRequest(IntPtr urg, int cap_times);
			
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
	        public static extern IntPtr Urg_RequestFullMDData(IntPtr urg);
			
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
	        public static extern int Urg_RecentTimestamp(IntPtr urg);
			
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
	        public static extern int Urg_RemainCaptureTimes(IntPtr urg);
			
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "MBFURG_GetInteger"), SuppressUnmanagedCodeSecurity]        
	        public static extern int GetInteger(URGIntegers item);
			
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
	        public static extern IntPtr Urg_GetVersionLines(IntPtr urg);
			
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
			public static extern IntPtr Urg_GetParameters(IntPtr urg);
	
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
			public static extern int Urg_MinDistance(IntPtr urg);
		
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
			public static extern int Urg_MaxDistance(IntPtr urg);
		
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
			public static extern double Urg_Index2Rad(IntPtr urg,int index);
		
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
			public static extern int Urg_Index2Deg(IntPtr urg,int index);
		
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
			public static extern int Urg_Rad2Index(IntPtr urg,double radian);
		
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
			public static extern int Urg_Deg2Index(IntPtr urg,int degree);
		
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        	public static extern int Urg_InitMDIntensityRequest(IntPtr urg, int cap_times);
			
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
	        public static extern IntPtr Urg_RequestFullGDIntensityData(IntPtr urg);
			
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
	        public static extern IntPtr Urg_RequestFullMDIntensityData(IntPtr urg);
	
			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        	public static extern IntPtr GetIntensity();

			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        	public static extern IntPtr Urg_PartialGDScan(IntPtr urg, int first, int last, int BufferSize);

			[DllImport(MBF_URG_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        	public static extern IntPtr Urg_PartialMDScan(IntPtr urg, int first, int last, int BufferSize);

			[StructLayout(LayoutKind.Sequential)]
			public struct urg_parameter {
				[MarshalAs(UnmanagedType.ByValArray, SizeConst = SensorTypeLineMax)]
	  			public char[] sensor_type;	 				/*!< Sensor type */
	  			public int distance_min_;   	    	    /*!< DMIN Information */
	  			public int distance_max_;                   /*!< DMAX Information */
	  			public int area_total_;                     /*!< ARES Information */
	  			public int area_min_;                       /*!< AMIN Information */
	  			public int area_max_;                       /*!< AMAX Information */
	  			public int area_front_;                     /*!< AFRT Information */
	  			public int scan_rpm_;                       /*!< SCAN Information */
			}
		}
		
	}
}
