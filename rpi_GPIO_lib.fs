\ This gforth code is used as follows:
\ include forth_lib_example.fs	\ this will load this file into gforth and compile and run it
\ *** NOTE the file named librpigpio.a needs to be placed into /usr/lib directory 
\ *** This library is refered to in the below add-lib command.  I have not yet figured out how to 
\ *** give the add-lib a path to the library but the standard library path is used and works fine.
\ The library does not need to be a shared library.  I have only tested with dynamic librarys but it should work with static ones also

c-library mypigpio
s" rpigpio" add-lib
\c #include <sys/types.h>
\c #include <unistd.h>
\c #include "/home/pi/git/testgit/gforth_stuff/rpiGpio.h"
\c #include "/home/pi/git/testgit/gforth_stuff/bcm2835_gpio.h"
\c int pipinoutput(int);
\c int pipinlow(int);
\c int pipinhigh(int);
\c int pipinoutput(int pin) { return ( gpioSetFunction ( pin, output )) ; }
\c int pipinlow(int pin) { return ( gpioSetPin ( pin, low )) ; }
\c int pipinhigh(int pin) { return ( gpioSetPin ( pin, high )) ; }
c-function pisetoutput pipinoutput n -- n 
c-function piosetup gpioSetup -- n
c-function piocleanup gpioCleanup -- n
c-function pipinlow pipinlow n -- n
c-function pipinhigh pipinhigh n -- n
end-c-library

piosetup .	\ this should do basic setup of the gpio function and show errStatus value (0 for all ok)
25 pisetoutput .
25 pipinlow .
1000 ms
25 pipinhigh .
piocleanup .	\ this should stop the gpio function and show errStatus value ( 0 for all ok)


