\ This gforth code is used as follows:
\ include forth_lib_example.fs	\ this will load this file into gforth and compile and run it


c-library mypigpio
s" rpigpio" add-lib
\c #include <sys/types.h>
\c #include <unistd.h>
\c #include "/home/pi/git/Rpi_Gforth_GPIO/c_src/rpiGpio.h"
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


