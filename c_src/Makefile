# This make file creates a shared library for the gpio pins based on RaspberryPi-GPIO code.
CC=gcc
CCFLAGS=-Wall -Werror -g  -I.

AR=ar
ARFLAGS=-rcs

OUT_DIR=output
LIB_DIR=library
LIB_NAME=librpigpio.a		#name needs to start with lib and end with .a
LIB_NAME_SHARED=librpigpio.so	#name needs to start with lib and end with .so but the part inside should match LIB_NAME's 
LIB_VERSION=0
LIB_SUBVERSION=0.1

VPATH= $(OUT_DIR) $(LIB_DIR) 

all: dirs $(LIB_NAME) $(LIB_NAME_SHARED)

$(LIB_NAME_SHARED): gpio.o i2c.o
	$(CC) $(CCFLAGS) -shared -W1,soname,$(LIB_DIR)/$@.$(LIB_VERSION) -o $(LIB_DIR)/$@.$(LIB_VERSION).$(LIB_SUBVERSION) $(OUT_DIR)/gpio.o $(OUT_DIR)/i2c.o 

$(LIB_NAME): gpio.o i2c.o
	$(AR) $(ARFLAGS) $(LIB_DIR)/$@ $(OUT_DIR)/gpio.o $(OUT_DIR)/i2c.o

%.o: %.c
	$(CC) $(CCFLAGS)  -o $(OUT_DIR)/$@ -c -fPIC $<

dirs:
	test -d $(OUT_DIR) || mkdir $(OUT_DIR);
	test -d $(LIB_DIR) || mkdir $(LIB_DIR);

clean:
	-rm $(OUT_DIR)/*.o;
	-rmdir $(OUT_DIR);
	-rm $(LIB_DIR)/*.a;
	-rm $(LIB_DIR)/*.so*;
	-rmdir $(LIB_DIR);
