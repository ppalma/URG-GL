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
  enum {
    BufferSize = 3,
  };

#ifdef WINDOWS_OS
  const char device[] = "COM3"; /* Example for Windows  */
#else
  const char device[] = "/dev/ttyACM0"; /* Example for Linux */
#endif

  long data[BufferSize];
  urg_parameter_t parameter;
  int first, last;
  int ret;
  int n;
  int i;

  /* Connection */
  urg_t urg;
  ret = urg_connect(&urg, device, 115200);
  if (ret < 0) {
    urg_exit(&urg, "urg_connect()");
  }
  urg_parameters(&urg, &parameter);

  first = parameter.area_front_;
  last = parameter.area_front_ + BufferSize;

  /* Get only front data */
  ret = urg_requestData(&urg, URG_GD, first, last);
  if (ret < 0) {
    urg_exit(&urg, "urg_requestData()");
  }

  /* Reception */
  n = urg_receivePartialData(&urg, data, BufferSize, first, last);
  printf("# n = %d\n", n);
  if (n < 0) {
    urg_exit(&urg, "urg_receivePartialData()");
  }

  /* Display */
  for (i = 0; i < n; ++i) {
    /* Neglect the distance data if it is less than urg_minDistance()  */
    printf("%d %ld\n", i, data[i]);
  }

  urg_disconnect(&urg);
  return 0;
}

