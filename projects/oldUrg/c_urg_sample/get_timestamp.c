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
    CaptureTimes = 10,
  };

  //const char device[] = "COM3"; /* Example for Windows */
  const char device[] = "/dev/ttyACM0"; /* Example for Linux */

  long timestamp = 0;
  int ret = 0;
  int i;

  /* Connection */
  urg_t urg;
  ret = urg_connect(&urg, device, 115200);
  if (ret < 0) {
    urg_exit(&urg, "urg_connect()");
  }

  /* Transit to timestamp mode and displays time stamp */
  urg_enableTimestampMode(&urg);
  for (i = 0; i < CaptureTimes; ++i) {
    timestamp = urg_currentTimestamp(&urg);
    printf("%02d: timestamp: %ld [msec]\n", i, timestamp);
  }
  urg_disableTimestampMode(&urg);

  return 0;
}

