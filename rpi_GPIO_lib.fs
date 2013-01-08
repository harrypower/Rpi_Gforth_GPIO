\ This Gforth code is a Raspberry Pi GPIO library
\    Copyright (C) 2013  Philip K. Smith

\    This program is free software: you can redistribute it and/or modify
\    it under the terms of the GNU General Public License as published by
\    the Free Software Foundation, either version 3 of the License, or
\    (at your option) any later version.

\    This program is distributed in the hope that it will be useful,
\    but WITHOUT ANY WARRANTY; without even the implied warranty of
\    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\    GNU General Public License for more details.

\    You should have received a copy of the GNU General Public License
\    along with this program.  If not, see <http://www.gnu.org/licenses/>.

\ This gforth code is used as follows:
\ include forth_lib_example.fs	\ this will load this file into gforth and compile and run it.
\ Note remember to ajust the path of the #include in below code for your repository needed!

clear-libs
c-library mygpio
s" rpigpio" add-lib
\c #include "/home/pi/git/Rpi_Gforth_GPIO/c_src/rpiGpio.h"	\\ this path may need to be changed for your system
\c int pipinsetpullup(int pin) { return ( gpioSetPullResistor( pin, pullup ));}
\c int pipinsetpulldown(int pin) { return ( gpioSetPullResistor( pin, pulldown));}
\c int pipinsetpulldisable(int pin) { return ( gpioSetPullResistor( pin, pullDisable));} 
\c int pipininput(int pin) { return ( gpioSetFunction ( pin, input )) ; }
\c int pipinoutput(int pin) { return ( gpioSetFunction ( pin, output )) ; }
\c int pipinlow(int pin) { return ( gpioSetPin ( pin, low )) ; }
\c int pipinhigh(int pin) { return ( gpioSetPin ( pin, high )) ; }
\c int pipinread(int pin) { 
\c	eState state;
\c	int rtn = 0;
\c	gpioReadPin(pin, &state);
\c	if ( state == high )  rtn = 1;
\c	if ( state == low )  rtn = 0; 
\c	return rtn ; }
\c int pipinread2(int pin, eState *state) { return ( gpioReadPin(pin, state)); }
c-function pipinread2 pipinread2 n a -- n
c-function pipinread pipinread n -- n
c-function pipinsetpullup pipinsetpullup n -- n
c-function pipinsetpulldown pipinsetpulldown n -- n
c-function pipinsetpulldisable pipinsetpulldisable n -- n
c-function pipininput pipininput n -- n
c-function pipinoutput pipinoutput n -- n 
c-function piosetup gpioSetup -- n
c-function piocleanup gpioCleanup -- n
c-function pipinlow pipinlow n -- n
c-function pipinhigh pipinhigh n -- n
end-c-library

\ basic output on gpio pin 25 example
\ piosetup .	\ this should do basic setup of the gpio function and show errStatus value (0 for all ok)
\ 25 pipinoutput .
\ 25 pipinlow .
\ 1000 ms
\ 25 pipinhigh .
\ piocleanup .	\ this should stop the gpio function and show errStatus value ( 0 for all ok)
\ *******************************************************

\ basic input on gpio pin 25 example
\ piosetup .
\ 25 pipininput .
\ 25 pipinread .
\ here cell allot dup 25 swap pipinread2 . 1 dump
\ piocleanup .
\ ******************************************************
