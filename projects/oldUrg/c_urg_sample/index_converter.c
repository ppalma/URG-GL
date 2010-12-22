#include "urg_ctrl.h"
#include "math_utils.h"
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
  //const char device[] = "COM3"; /* Example for Windows  */
  const char device[] = "/dev/ttyACM0"; /* Example for  Linux */

  int ret = 0;
  int n;
  int i;

  /* Connection */
  urg_t urg;
  ret = urg_connect(&urg, device, 115200);
  if (ret < 0) {
    urg_exit(&urg, "urg_connect()");
  }

  /* Outputs the angle of each index */
  n = urg_dataMax(&urg);
  for (i = 0; i < n; i += 8) {
    double radian = urg_index2rad(&urg, i);
    int degree = urg_index2deg(&urg, i);
    printf("%03d, %d [deg], %.1f [rad], %d, %d\n", i, degree, radian,
           urg_deg2index(&urg, degree), urg_rad2index(&urg, radian));
  }

  return 0;
}

