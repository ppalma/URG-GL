# Makefile for c_urg_sample

CFLAGS = -g -O0 -Wall -Werror `c_urg-config --cflags`
LDFLAGS = `c_urg-config --libs`
LDLIBS =

TARGET = gd_scan md_scan expand_2d capture_sample evaluate_timestamp get_parameters get_timestamp get_version_lines index_converter intensity_sample partial_scan

all : $(TARGET)

clean :
	$(RM) *.o $(TARGET)

.PHONY : all clean
