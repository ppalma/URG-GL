// Compiler notes:

// to compile on linux
//gcc -fPIC -c -DMBFURG_EXPORTS_LINUX  mbfurg.c -lc_urg -lc_urg_connection -lc_urg_system -g -O0 -Wall -Werror -I/usr/include/c_urg -Iinclude

//gcc -shared -o libmbfurg.so mbfurg.o -lc_urg -lc_urg_connection -lc_urg_system  -g -O0 -Wall -Werror -I/usr/include/c_urg -Iinclude -Iinclude

//then as root
//cp libmbfurg.so /usr/local/lib
//ldconfig

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdarg.h>
#include <math.h>
#include <assert.h>

#include "mbfurg.h"

#include "urg_ctrl.h"
#include "detect_os.h"

int data_max;
long *data;
long *intensity;
int timestamp;
int ret;
int n;
int i;
int previous_timestamp;
int remain_times;
int scan_msec;
int CaptureTimes = 10;
urg_parameter_t parameter;
urg_t urg;
enum {
    LinesMax = 5,
  };
char* lines[LinesMax];
char aux[66*2]="";


MBFURG_API int MBFURG_GetInteger(int item) 
{
	//TODO: validate all this crap!!!
	switch (item)
  {
	  case DATA_READED :
		  return n; 
	  case TIMESTAMP :
		  return timestamp;
	  case PREVIOUS_TIMESTAMP :
		  return previous_timestamp; 
	  case REMAIN_TIMES :
		  return remain_times; 
	  case AREA_FRONT :
		  return data[parameter.area_front_];
	 // case LINES_MAX :
	//	  return LinesMax;
	  default:
	  	  return -1;
	}
	return 0;
}

void mbfprint(const char *format, ...)
{   
	//va_list args;
	//printf("MBF.URG: ");
	//va_start(args, format);
	//vprintf(format, args);
	//va_end(args);
	//printf("\n");
}

static void urg_exit(urg_t *urg, const char *message)
{
  mbfprint("%s: %s\n", message, urg_error(urg));
  urg_disconnect(urg);

#ifdef MSC
  getchar();
#endif
  exit(1);
}

MBFURG_API urg_t* Urg_Initialise(const char* device)
{
//	if (device == NULL) {
//		#ifdef WINDOWS_OS
//  		const char internal_device[] = "COM3"; /* For Windows */
//		#else
//  		const char internal_device[] = "/dev/ttyACM0"; /* For Linux */
//		#endif
//	} else {
//		const char internal_device = device;
//	}

//	const char internal_device = device;

  /* Connection */

  ret = urg_connect(&urg, device, 115200);
  if (ret < 0) {
    urg_exit(&urg, "urg_connect()");
  }


	mbfprint("Initialised\n");


	return &urg;
}

MBFURG_API long* Urg_RequestFullGDData(urg_t* urg)
{

	//free(data); //TODO: free before using

  /* Reserve for reception data */
  data_max = urg_dataMax(urg);
  data = (long*)malloc(sizeof(long) * data_max);
  if (data == NULL) {
    perror("malloc");
    exit(1);
  }

  /* Request for GD data */
  ret = urg_requestData(urg, URG_GD, URG_FIRST, URG_LAST);
  if (ret < 0) {
    urg_exit(urg, "urg_requestData()");
  }

  /* Reception */
  n = urg_receiveData(urg, data, data_max);
  mbfprint("# n = %d\n", n);
  if (n < 0) {
    urg_exit(urg, "urg_receiveData()");
  }

  /* Display */
  timestamp = urg_recentTimestamp(urg);
  mbfprint("# timestamp: %d\n", timestamp);

	return data;
}

MBFURG_API int Urg_InitMDRequest(urg_t* urg, int cap_times)
{
	urg_parameters(urg, &parameter);
  scan_msec = urg_scanMsec(urg);

  /* Request for MD data */
  /* To get data continuously for more than 100 times, set capture times equal
     to infinity times(UrgInfinityTimes) */
  /* urg_setCaptureTimes(&urg, UrgInfinityTimes); */
  //assert(cap_times < 100);
	if (cap_times > 99)
		cap_times = UrgInfinityTimes;  
	urg_setCaptureTimes(urg, cap_times);

  /* Request for data */
  ret = urg_requestData(urg, URG_MD, URG_FIRST, URG_LAST);
  if (ret < 0) {
    urg_exit(urg, "urg_requestData()");
  }
	return ret;
}

MBFURG_API long* Urg_RequestFullMDData(urg_t* urg)
{
	/* Reserve for receive buffer */
  data_max = urg_dataMax(urg);
  data = (long*)malloc(sizeof(long) * data_max);
  if (data == NULL) {
  	mbfprint("data_max: %d\n", data_max);
		perror("data buffer");
    exit(1);
  }
  

  /* Reception */
  n = urg_receiveData(urg, data, data_max);
  mbfprint("n = %d\n", n);
  if (n < 0) {
    urg_exit(urg, "urg_receiveData()");
  } else if (n == 0) {
    mbfprint("n == 0\n");
    --i;
 	}
  /* Display the front data with timestamp */
  /* Delay in reception of data at PC causes URG to discard the data which
     cannot be transmitted. This may  results in remain_times to become
     discontinuous */
	// previous_timestamp = timestamp;
	// timestamp = urg_recentTimestamp(urg);
	// remain_times = urg_remainCaptureTimes(urg);
	return data;
}

MBFURG_API char* Urg_GetVersionLines(urg_t* urg)
{
	char buffer[LinesMax][UrgLineWidth];	
	for (i = 0; i < LinesMax; ++i) {
    		lines[i] = buffer[i];
	}
  
	ret = urg_versionLines(urg, lines, LinesMax);

	for(i=0; i < LinesMax; ++i) {
		strcat(aux,lines[i]);
		strcat(aux,"\n");
	}

	return aux;

}

MBFURG_API urg_parameter_t* Urg_GetParameters(urg_t* urg)
{
	int ret;
	ret = urg_parameters(urg, &parameter);
	return &parameter;
	
}

MBFURG_API int Urg_RecentTimestamp(urg_t* urg)
{
	return urg_recentTimestamp(urg);
}

MBFURG_API int Urg_RemainCaptureTimes(urg_t* urg)
{
	return urg_remainCaptureTimes(urg);
}

MBFURG_API long Urg_MinDistance(urg_t* urg)
{
	return urg_minDistance(urg);
}

MBFURG_API long Urg_MaxDistance(urg_t* urg)
{
	return urg_maxDistance(urg);
}

MBFURG_API double Urg_Index2Rad(urg_t* urg, int index)
{
    	return urg_index2rad(urg,index);
}

MBFURG_API int Urg_Index2Deg(urg_t* urg, int index)
{
	return urg_index2deg(urg,index);
}

MBFURG_API int Urg_Rad2Index(urg_t* urg, double radian)
{
	return urg_rad2index(urg,radian);
}

MBFURG_API int Urg_Deg2Index(urg_t* urg, int degree)
{
	return urg_deg2index(urg,degree);
}

MBFURG_API long* Urg_RequestFullGDIntensityData(urg_t* urg)
{
  data_max = urg_dataMax(urg);
  data = (long*)malloc(sizeof(long) * data_max);
  if (data == NULL) {
    perror("data buffer");
    exit(1);
  }
  intensity = (long*)malloc(sizeof(long) * data_max);
  if (intensity == NULL) {
    perror("data buffer");
    exit(1);
  }
  
  urg_parameters(urg, &parameter);
  scan_msec = urg_scanMsec(urg);

  ret = urg_requestData(urg, URG_GD_INTENSITY, URG_FIRST, URG_LAST);
  
  if (ret < 0) {
    urg_exit(urg, "urg_requestData()");
  }

  n = urg_receiveDataWithIntensity(urg, data, data_max, intensity);
  
 
  mbfprint("# n = %d\n", n);
  if (n < 0) {
    urg_exit(urg, "urg_receiveDataWithIntensityData()");
  }
  
  /* Display */
  timestamp = urg_recentTimestamp(urg);
  mbfprint("# timestamp: %d\n", timestamp);

  return data;
}


MBFURG_API int Urg_InitMDIntensityRequest(urg_t* urg, int cap_times)
{
   urg_parameters(urg, &parameter);
   scan_msec = urg_scanMsec(urg);

   if (cap_times > 99)
	cap_times = UrgInfinityTimes;  
   
   urg_setCaptureTimes(urg, cap_times);

  /* Request for data */
  ret = urg_requestData(urg, URG_MD_INTENSITY, URG_FIRST, URG_LAST);
  if (ret < 0) {
    urg_exit(urg, "urg_requestData()");
  }
 
   return ret;
}

MBFURG_API long* Urg_RequestFullMDIntensityData(urg_t* urg)
{
  data_max = urg_dataMax(urg);

  data = (long*)malloc(sizeof(long) * data_max);
  if (data == NULL) {
  	mbfprint("data_max: %d\n", data_max);
		perror("data buffer");
    exit(1);
  }

  intensity = (long*)malloc(sizeof(long) * data_max);
  if (intensity == NULL) {
    perror("data buffer");
    exit(1);
  }
 
  n = urg_receiveDataWithIntensity(urg, data, data_max, intensity);

  mbfprint("n = %d\n", n);
  if (n < 0) {
    urg_exit(urg, "urg_receiveData()");
  } else if (n == 0) {
    mbfprint("n == 0\n");
    --i;
 	}
  
  return data;
}

MBFURG_API long* GetIntensity()
{
     return intensity;
}

MBFURG_API long* Urg_PartialGDScan(urg_t* urg, int first, int last, int BufferSize){
	int ret;
	data = (long*)malloc(sizeof(long) * BufferSize);
  	int n;
	ret = urg_requestData(urg, URG_GD, first, last);
	n = urg_receivePartialData(urg, data, BufferSize, first, last);
	return data;

}

MBFURG_API long* Urg_PartialMDScan(urg_t* urg, int first, int last, int BufferSize){
   
    urg_parameters(urg, &parameter);
      scan_msec = urg_scanMsec(urg);
    if (BufferSize > 99)
        BufferSize = UrgInfinityTimes; 
      data = (long*)malloc(sizeof(long) * BufferSize);
      if (data == NULL) {
          mbfprint("BufferSize: %d\n", BufferSize);
            perror("data buffer");
        exit(1);
      }
      /* Reception */
    ret = urg_requestData(urg, URG_MD, first, last);
      n = urg_receivePartialData(urg, data, BufferSize, first, last);
      mbfprint("n = %d\n", n);
      if (n < 0) {
            urg_exit(urg, "urg_receiveData()");
      } else if (n == 0) {
            mbfprint("n == 0\n");
            --i;
     }
    return data;

}

MBFURG_API void Urg_Exit(urg_t* urg,const char* mes)
{
	urg_exit(urg, mes);
}


MBFURG_API void Urg_Finalise(urg_t* urg)
{	
	urg_disconnect(urg);
	free(data);

	mbfprint("Finalised\n");
}


