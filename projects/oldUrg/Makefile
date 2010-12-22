# Makefile for mbfurg
CC = gcc 
CFLAGS = -g -O0 -Wall -Werror `c_urg-config --cflags` 
LDFLAGS = `c_urg-config --libs`
LDLIBS =

all: mbfurg.o
	${CC} ${CFLAGS} ${LDFLAGS} -shared -o libmbfurg.so mbfurg.o -Iinclude

mbfurg.o:
	${CC} ${CFLAGS} ${LDFLAGS} -fPIC -c mbfurg.c 

install:
	cp libmbfurg.so /usr/local/lib/
clean: 
	$(RM) *.o  libmbfurg.so

