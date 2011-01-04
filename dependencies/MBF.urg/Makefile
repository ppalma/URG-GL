# Makefile for mbfurg
CC = gcc 
CFLAGS = -lc_urg -lc_urg_connection -lc_urg_system -g -O0 -Wall -Werror
CINCLUDE = -I/usr/include/c_urg -I/usr/local/include/c_urg -Iinclude
LDFLAGS = `c_urg-config --libs`

all: mbfurg.o
	${CC} -shared -o libmbfurg.so mbfurg.o ${CFLAGS} ${CINCLUDE}

mbfurg.o:
	${CC} -fPIC -c -DMBFURG_EXPORTS_LINUX  mbfurg.c ${CFLAGS} ${CINCLUDE}

install:
	cp libmbfurg.so /usr/local/lib/
	ldconfig
clean: 
	rm *.o  libmbfurg.so
main: all
	${CC} -o prueba prueba.c ${CFLAGS} ${CINCLUDE} libmbfurg.so mbfurg.o 
