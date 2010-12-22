/*!
  \example gd_scan.c

  \brief Sample of GD scan

  Get distance data continuously using GD command

  \author Satofumi KAMIMURA

  $Id: gd_scan.c 1718 2010-02-21 22:05:45Z satofumi $
*/

#include "urg_ctrl.h"
#include <stdio.h>
#include <stdlib.h>


static void urg_exit(urg_t *urg, const char *message)
{
  printf("%s: %s\n", message, urg_error(urg));
  urg_disconnect(urg);

#ifdef MSC
  getchar();
#endif
  exit(1);
}


int main(int argc, char *argv[])
{
#ifdef WINDOWS_OS
  const char device[] = "COM3"; /* For Windows */
#else
  const char device[] = "/dev/ttyACM0"; /* For Linux */
#endif

  int data_max;
  long *data;
  int timestamp;
  int ret;
  int n;
  int i;

  /* Connection */
  urg_t urg;
  ret = urg_connect(&urg, device, 115200);
  if (ret < 0) {
    urg_exit(&urg, "urg_connect()");
  }

  /* Reserve for reception data */
  data_max = urg_dataMax(&urg);
  data = (long*)malloc(sizeof(long) * data_max);
  if (data == NULL) {
    perror("malloc");
    exit(1);
  }

  /* Request for GD data */
  ret = urg_requestData(&urg, URG_GD, URG_FIRST, URG_LAST);
  if (ret < 0) {
    urg_exit(&urg, "urg_requestData()");
  }

  /* Reception */
  n = urg_receiveData(&urg, data, data_max);
  printf("# n = %d\n", n);
  if (n < 0) {
    urg_exit(&urg, "urg_receiveData()");
  }

  /* Display */
  timestamp = urg_recentTimestamp(&urg);
  printf("# timestamp: %d\n", timestamp);
  for (i = 0; i < n; ++i) {
    /*Neglect the distance less than  urg_minDistance()  */
    printf("%d %ld, ", i, data[i]);
  }
  printf("\n");

  urg_disconnect(&urg);
  free(data);

#ifdef MSC
  getchar();
#endif

  return 0;
}
