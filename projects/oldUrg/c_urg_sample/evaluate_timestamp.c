#include "urg_ctrl.h"
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>


static void urg_exit(urg_t *urg, const char *message) {

  printf("%s: %s\n", message, urg_error(urg));
  urg_disconnect(urg);

#ifdef MSC
  getchar();
#endif
  exit(1);
}


int main(int argc, char *argv[])
{
  enum {
    CaptureTimes = 20,          /* Frequency of data acquisition */
  };


  //const char device[] = "COM3"; /* Example for Windows */
  const char device[] = "/dev/ttyACM0"; /* Example for Linux */


  int data_max;
  long* data;
  int timestamp;
  int pre_timestamp;
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
  }

  /* Reserve for receive buffer */
  data_max = urg_dataMax(&urg);
  data = (long*)malloc(sizeof(long) * data_max);
  if (data == NULL) {
    perror("data buffer");
    exit(1);
  }
  urg_parameters(&urg, &parameter);
  scan_msec = urg_scanMsec(&urg);

  /* Request for MD data。Frequency of data acquisition = captureTimes */
  /* Set capture times equals to infinte times to capture data more than 100 times */
  /* urg_setCaptureTimes(&urg, UrgInfinityTimes); */
  assert(CaptureTimes < 100);
  urg_setCaptureTimes(&urg, CaptureTimes);

  /* Request for data */
  ret = urg_requestData(&urg, URG_MD, URG_FIRST, URG_LAST);
  if (ret < 0) {
    urg_exit(&urg, "urg_requestData()");
  }

  /* Get data */
  pre_timestamp = 0;
  for (i = 0; i < CaptureTimes; ++i) {
    /* Reception */
    n = urg_receiveData(&urg, data, data_max);
    if (n < 0) {
      urg_exit(&urg, "urg_receiveData()");
    }

    /* Distane data of front is displayed with time stamp */
    timestamp = urg_recentTimestamp(&urg);
    printf("%d: %ld [mm], %d [msec], %d\n",
           i, data[parameter.area_front_], timestamp,
           pre_timestamp - timestamp);
    pre_timestamp = timestamp;
  }

  urg_disconnect(&urg);
  free(data);

  return 0;
}

