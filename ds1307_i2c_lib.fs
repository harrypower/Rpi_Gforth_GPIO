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

\ This is a basic library to access a DS1307 Real Time Clock chip via I2C
\ The connection to the raspberry pi to use this code are as follows:
\ 	SDA is P1-03 
\	SCL is P1-05
\ Note this code uses I2C1 and will only work with the above pins on Raspberry Pi Rev. 2 board.  
\ ***** Note pi internal pull up resistors are disabled so resistors are needed for chip hookup! *****

include rpi_GPIO_lib.fs

: !ds1307 ( val reg_addr -- flag )	\ Write val to ds1307 reg_addr.  Flag is zero for success. 
	TRY 	dup 0< if 11 throw then	\ can't have a reg_addr less then 0
		dup 63 > if 11 throw then \ can't have a reg_addr greater then 63 
		piosetup throw pii2csetup throw 104 pii2caddress throw 100000 pii2clock throw
		pad c! pad 1 + c! pad 2 pii2cwrite throw pii2cleanup throw piocleanup throw 0
 	ENDTRY-IFERROR	swap drop swap drop	\ ds1307 writing failed error # is on stack at exit of this code
	THEN					\ all ok no errors
	;

: @ds1307 ( -- val flag )	\ Read next ds1307 register value.  Flag is zero for success.
	TRY	piosetup throw pii2csetup throw 104 pii2caddress throw 100000 pii2clock throw
		pad 1 pii2cread throw pii2cleanup throw piocleanup throw pad c@ 0 
	ENDTRY-IFERROR	0 swap	\ read error # is on stack at exit of this code
	THEN			\ read ok no errors 
	;

