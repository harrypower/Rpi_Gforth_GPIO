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

require rpi_GPIO_lib.fs

: simpleoutput ( -- )
piosetup 0= if 
	25 pipinoutput 0= if s" pin 25 set for output" type cr endif
	25 pipinlow 0= if s" pin 25 is low now" type cr endif
	1000 ms
	25 pipinhigh 0= if s" pin 25 is high now" type cr endif
	piocleanup 0= if s" Ok GPIO is now turned off!" type cr endif
else s" oops GPIO did not initalize!" type endif 
;

: simpleinput ( -- )
piosetup 0= if
	25 pipininput 0= if s" pin 25 now set for input" type cr endif
	25 pad pipinread 0= if s" pin 25 was read ok" type cr else s" pin 25 failed to read for some reason!" type cr endif
	pad c@ 0= if s" pin 25 current input is low" type cr else s" pin 25 current input is high" type cr endif
	piocleanup 0= if s" GPIO is now turned off!" type cr endif
else s" oops GPIO did not initalize!" type endif
;

: output_timed { N_noop -- }
N_noop 1000 > if 1000 to N_noop endif   \ N_noop is limited to 1000 arbitrarily
piosetup 0= if
	25 pipinoutput 0= if s" pin 25 set for output" type cr endif
	BEGIN
		25 pipinlow drop
		N_noop 0 ?DO noop LOOP
		25 pipinhigh drop
	key? UNTIL	
	piocleanup 0= if s" GPIO is now turned off!" type cr endif
else s" oops GPIO did not initalize!" type cr endif
;
 
cr .( To use a simple output on pin 25 with an adacobler use command:  simpleoutput)
cr .( To use a simple input on pin 25 with an adacobler use command: simpleinput)
cr .( To use a variable time for low to high transition use command: 500 output_timed)
