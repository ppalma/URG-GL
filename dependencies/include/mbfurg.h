// Compiler notes:

// to compile on linux
//gcc -fPIC -c -DMBFURG_EXPORTS_LINUX  mbfurg.c -lc_urg -lc_urg_connection -lc_urg_system -g -O0 -Wall -Werror -I/usr/include/c_urg -Iinclude

//gcc -shared -o libmbfurg.so mbfurg.o -lc_urg -lc_urg_connection -lc_urg_system  -g -O0 -Wall -Werror -I/usr/include/c_urg -Iinclude -Iinclude

//then as root
// cp libmbfurg.so /usr/local/lib
//ldconfig

#include "urg_ctrl.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdarg.h>
#include <math.h>

#define MBFURG_EXPORTS

#ifdef MBFURG_EXPORTS_LINUX
	#ifdef MBFURG_EXPORTS
		#define MBFURG_API  __attribute__((stdcall))
	#else
		#define MBFURG_API  __attribute__((stdcall))
	#endif
#else
	#define inline _inline
	#define snprintf _snprintf
	#ifdef MBFURG_EXPORTS
		#define MBFURG_API __declspec(dllexport)
	#else
		#define MBFURG_API __declspec(dllimport)
	#endif
#endif


#ifndef M_PI
#define M_PI 3.14159265358979323846
#endif

enum URGValue {
			DATA_READED 						= 10000,
			TIMESTAMP		 						= 10001,
			PREVIOUS_TIMESTAMP 			= 10002,
			REMAIN_TIMES						= 10003,
			AREA_FRONT							= 10004
};

void mbfprint(const char *Format, ...);

