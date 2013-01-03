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
\ Note remember to ajust the path to the #include in below code if needed!

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

