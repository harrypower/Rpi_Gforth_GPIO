# This make file creates a shared library for the gpio pins based on RaspberryPi-GPIO code.
# This make file and code will only work on Raspberry Pi hardware.
# This make file needs to be in the directory of the source and header files.
# To use:	sudo make	# this will make the directorys output and library and compile and make the librarys
# 		sudo make clean	# this will remove the object and library files from the directorys 
#		sudo make install	# this will install the shared library in /usr/lib/ and run ln commands
#		sudo make uninstall	# this will remove the shared library and clean up systems indexing of it

CC=gcc
CCFLAGS=-Wall -Werror -g  -I.

AR=ar
ARFLAGS=-rcs

OUT_DIR=output
LIB_DIR=library
LIB_BASE_NAME=rpigpio
LIB_NAME=lib$(LIB_BASE_NAME).a
LIB_NAME_SHARED=lib$(LIB_BASE_NAME).so
LIB_VERSION=.0
LIB_SUBVERSION=.0.2
LIB_INSTALL=/usr/lib


VPATH= $(OUT_DIR) $(LIB_DIR) 

all: dirs $(LIB_NAME) $(LIB_NAME_SHARED)

$(LIB_NAME_SHARED): gpio.o i2c.o
	$(CC) $(CCFLAGS) -shared -W1,soname,$(LIB_DIR)/$@$(LIB_VERSION) -o $(LIB_DIR)/$@$(LIB_VERSION)$(LIB_SUBVERSION) $(OUT_DIR)/gpio.o $(OUT_DIR)/i2c.o 

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

install:
	-cp rpiGpio.h /usr/include/rpiGpio.h
	-cp bcm2835_gpio.h /usr/include/bcm2835_gpio.h
	-cp $(LIB_DIR)/$(LIB_NAME_SHARED)$(LIB_VERSION)$(LIB_SUBVERSION) $(LIB_INSTALL)/$(LIB_NAME_SHARED)$(LIB_VERSION)$(LIB_SUBVERSION)
	-ln -sf $(LIB_INSTALL)/$(LIB_NAME_SHARED)$(LIB_VERSION)$(LIB_SUBVERSION) $(LIB_INSTALL)/$(LIB_NAME_SHARED)
	-ln -sf $(LIB_INSTALL)/$(LIB_NAME_SHARED)$(LIB_VERSION)$(LIB_SUBVERSION) $(LIB_INSTALL)/$(LIB_NAME_SHARED)$(LIB_VERSION)
	-ldconfig -n $(LIB_INSTALL)/

uninstall:
	-rm /usr/include/rpiGpio.h
	-rm /usr/include/bcm2835_gpio.h
	-rm $(LIB_INSTALL)/$(LIB_NAME_SHARED)$(LIB_VERSION)$(LIB_SUBVERSION)
	-rm $(LIB_INSTALL)/$(LIB_NAME_SHARED)
	-rm $(LIB_INSTALL)/$(LIB_NAME_SHARED)$(LIB_VERSION)
	-ldconfig -n $(LIB_INSTALL)/
