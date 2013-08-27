#! /usr/bin/gforth

\ This Gforth code is a Raspberry Pi data logging software
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

include ../gpio/rpi_GPIO_lib.fs

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

