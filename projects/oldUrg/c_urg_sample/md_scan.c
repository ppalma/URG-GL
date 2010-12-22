/*!
  \example md_scan.c

  \brief Sample of MD scan

  Get the distance data of specified number of times using MD command.

  \author Satofumi KAMIMURA

  $Id: md_scan.c 1718 2010-02-21 22:05:45Z satofumi $
*/

#include "urg_ctrl.h"
#include "delay.h"
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>


static void urg_exit(urg_t *urg, const char *message)
{
  printf("%s: %s\n", message, urg_error(urg));
  urg_disconnect(urg);

#ifdef MSC
  getchar();
#endif
  exit(1);
}


//! main
int main(int argc, char *argv[])
{
  enum {
    CaptureTimes = 10,
  };

#ifdef WINDOWS_OS
  const char device[] = "COM3"; /* For Windows */
#else
  const char device[] = "/dev/ttyACM0"; /* For Linux */
#endif

  int data_max;
  long* data;
  int timestamp = -1;
  int previous_timestamp;
  int remain_times;
  int scan_msec;
  urg_parameter_t parameter;
  int ret;
  int n;
  int i;

  /* Connection */
  urg_t urg;
  ret = urg_connect(&urg, device, 115200);
  if (ret < 0) {
    urg_exit(&urg, "urg_connect()");
    exit(1);
  }

  /* Reserve for receive buffer */
  data_max = urg_dataMax(&urg);
  data = (long*)malloc(sizeof(long) * data_max);
  if (data == NULL) {
    fprintf(stderr, "data_max: %d\n", data_max);
    perror("data buffer");
    exit(1);
  }
  urg_parameters(&urg, &parameter);
  scan_msec = urg_scanMsec(&urg);

  /* Request for MD data */
  /* To get data continuously for more than 100 times, set capture times equal
     to infinity times(UrgInfinityTimes) */
  /* urg_setCaptureTimes(&urg, UrgInfinityTimes); */
  assert(CaptureTimes < 100);
  urg_setCaptureTimes(&urg, CaptureTimes);

  /* Request for data */
  ret = urg_requestData(&urg, URG_MD, URG_FIRST, URG_LAST);
  if (ret < 0) {
    urg_exit(&urg, "urg_requestData()");
  }

  for (i = 0; i < CaptureTimes; ++i) {
    /* Reception */
    n = urg_receiveData(&urg, data, data_max);
    printf("n = %d\n", n);
    if (n < 0) {
      urg_exit(&urg, "urg_receiveData()");
    } else if (n == 0) {
      printf("n == 0\n");
      --i;
      continue;
    }

    /* Display the front data with timestamp */
    /* Delay in reception of data at PC causes URG to discard the data which
       cannot be transmitted. This may  results in remain_times to become
       discontinuous */
    previous_timestamp = timestamp;
    timestamp = urg_recentTimestamp(&urg);
    remain_times = urg_remainCaptureTimes(&urg);

    /* Neglect the distance data if it is less than urg_minDistance() */
    printf("%d/%d: %ld [mm], %d [msec], (%d)\n",
           remain_times, CaptureTimes, data[parameter.area_front_], timestamp,
           timestamp - previous_timestamp);

    printf("%d, %d\n", i, remain_times);

    if (remain_times <= 0) {
      break;
    }
  }

  urg_disconnect(&urg);
  free(data);

#ifdef MSC
  getchar();
#endif

  return 0;
}
