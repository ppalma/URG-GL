#include "urg_ctrl.h"
#include "detect_os.h"
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
  //const char device[] = "COM3"; /* Example for Windows */
  const char device[] = "/dev/ttyACM0"; /* Example for Linux */

  urg_t urg;
  urg_parameter_t parameters;
  int ret;

  /* Connection */
  ret = urg_connect(&urg, device, 115200);
  if (ret < 0) {
    urg_exit(&urg, "urg_connect()");
  }

  /* Get sensor parameter */
  ret = urg_parameters(&urg, &parameters);
  printf("urg_getParameters: %s\n", urg_error(&urg));
  if (ret < 0) {
    urg_disconnect(&urg);
    exit(1);
  }

  /* Connection */
  printf("distance_min: %ld\n", parameters.distance_min_);
  printf("distance_max: %ld\n", parameters.distance_max_);
  printf("area_total: %d\n", parameters.area_total_);
  printf("area_min: %d\n", parameters.area_min_);
  printf("area_max: %d\n", parameters.area_max_);
  printf("area_front: %d\n", parameters.area_front_);
  printf("scan_rpm: %d\n", parameters.scan_rpm_);
  printf("\n");

  /* Display information from URG structure ( same resource as above) */
  printf("urg_getDistanceMax(): %ld\n", urg_maxDistance(&urg));
  printf("urg_getDistanceMin(): %ld\n", urg_minDistance(&urg));
  printf("urg_getScanMsec(): %d\n", urg_scanMsec(&urg));
  printf("urg_getDataMax(): %d\n", urg_dataMax(&urg));

  return 0;
}

