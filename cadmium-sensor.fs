#! /usr/bin/gforth

\ This Gforth code will read a Cadmium sensor on a GPIO pin with just a capacitor and a resistor. 
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
\
\    This code works with version 2 boards but could work with other boards
\    This code is run from command line as follows: sudo ./cadmium-sensor.fs 7
\    This example has the pin number 7 for where the cadmium sensor is located.
\    The sensor is hooked up to the pin as follows: ( note this could damage your Pi if done wrong)
\    The GPIO 3.3 volt output pin # 1 is attached to one end or a 5k resistor.
\    The other end of the resistor is attached to one pin of the CdS sensor.
\    The other pin of CdS sensor is attached to the GPIO pin you want to use and one pin of a 0.1 uf capacitor
\    The other pin of the 0.1 uf capacitor is attached to GPIO pin #6 for digital ground ( you could also use pin #14,20,25,9 as they are also ground )
\
\    This code simply returns to stdout a error value and a time value
\    The error value is 0 for all ok and 777 for time out on reading CdS sensor.
\    You will also see a error value of 2 if you use a pin that is not allowed.
\    The smaller the time value the more light the larger the time value the less light.
\    The time value is just a raw one time reading.
\    If you want to use the code from inside your code just comment out the last line and call the word CdS-raw-read or CdS-average for an average value from 12 readings.
\    Other component values could be used for the resistor and capacitor but you will need to ensure you do not exceed GPIO pin sink current with your setup.
\    Also if you change the component values you can play with the time value for time-exceeded-fail detection ... that is the 1000000 value below.

include rpi_GPIO_lib.fs

777 constant time-exceeded-fail

: CdS-raw-read { ncadpin -- ntime nflag } \ nflag is 0 if ntime is a real value
    try 
	piosetup throw
	ncadpin pipinsetpulldisable throw
	ncadpin pipinoutput throw
	ncadpin pipinlow throw
	2 ms
	ncadpin pipininput throw
	utime
	1000000
	begin
	    1 - dup 
	    if
		ncadpin pad pipinread throw
		pad c@
	    else
		drop time-exceeded-fail throw
	    then
	until
	utime rot drop 2swap d- d>s
	piocleanup throw
	false
    restore dup if 0 swap piocleanup drop then
    endtry ;

: CdS-average { ncadpin -- ntime nflag }
    ncadpin CdS-raw-read dup 0=
    if 
	10 0 ?do 
	    ncadpin CdS-raw-read if 2drop true leave else swap drop + 0 then 
	loop
	if true else 12 / 0 then 
    else 
    then ;

: CdS ( -- )
    next-arg dup
    if s>number? if d>s CdS-raw-read . . else ." Pin value not valid for CdS sensor location !" then
    else ." Pin value needed as argument for this CdS code to read light level!"
    then ;
  
CdS bye 

