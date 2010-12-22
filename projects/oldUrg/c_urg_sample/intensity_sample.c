#include "urg_ctrl.h"
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


static void printData(urg_t *urg, urg_parameter_t *parameter,
                      long data[], int index, long intensity[])
{
  /* Displays the front distance data with time stamp */
  int timestamp = urg_recentTimestamp(urg);

  /* Neglect those data which are less than urg_minDistance()  */
  printf("%d: %ld [mm] (%ld) %d [msec]\n", index,
         data[parameter->area_front_],
         intensity[parameter->area_front_],
         timestamp);
}


int main(int argc, char *argv[])
{
  enum {
    CaptureTimes = 10,           /* Frequency of data acquisition */
  };

  //const char device[] = "COM3"; /* Example for Windows */
  const char device[] = "/dev/ttyACM0"; /* Example for Linux */

  int data_max;
  long* data;
  long* intensity;
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

  /* Reserve for Receive buffer */
  data_max = urg_dataMax(&urg);
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

  urg_parameters(&urg, &parameter);
  scan_msec = urg_scanMsec(&urg);

  /* Request for GD data. */
  printf("GD capture\n");
  for (i = 0; i < CaptureTimes; ++i) {
    /* Requrest for data */
    ret = urg_requestData(&urg, URG_GD_INTENSITY, URG_FIRST, URG_LAST);
    if (ret < 0) {
      urg_exit(&urg, "urg_requestData()");
    }

    /* Reception */
    n = urg_receiveDataWithIntensity(&urg, data, data_max, intensity);
    if (n < 0) {
      urg_exit(&urg, "urg_receiveData()");
    }
    printData(&urg, &parameter, data, i, intensity);
  }
  printf("\n");

  /* Request for MD data. */
  printf("MD capture\n");

  /* Set capture time value equal to infinity time(UrgInfinityTimes) to get data continuously for more than 99 times */
  /* urg_setCaptureTimes(&urg, UrgInfinityTimes); */
  assert(CaptureTimes < 100);
  urg_setCaptureTimes(&urg, CaptureTimes);

  /* Request for data */
  ret = urg_requestData(&urg, URG_MD_INTENSITY, URG_FIRST, URG_LAST);
  if (ret < 0) {
    urg_exit(&urg, "urg_requestData()");
  }

  /* Get data */
  for (i = 0; i < CaptureTimes; ++i) {
    /* Reception */
    n = urg_receiveDataWithIntensity(&urg, data, data_max, intensity);
    if (n < 0) {
      urg_exit(&urg, "urg_receiveData()");
    }
    printData(&urg, &parameter, data, i, intensity);
  }

  if (CaptureTimes > 99) {
    // It is necessary to explicitly stop the data acquisition to get data more than 99 times.
    urg_laserOff(&urg);
  }

  urg_disconnect(&urg);
  free(data);
  free(intensity);

  return 0;
}

