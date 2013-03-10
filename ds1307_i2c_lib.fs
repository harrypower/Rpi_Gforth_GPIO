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

\ This is a basic library to access a DS1307 Real Time Clock chip via I2C1 on Raspberry Pi Rev2.
\ The connections to the raspberry pi to use this code are as follows:
\ 	SDA is P1-03 
\	SCL is P1-05
\ Note this code uses I2C1 and will only work with the above pins on Raspberry Pi Rev. 2 board.  
\ ***** Note pi internal pull up resistors are disabled so resistors are needed for chip hookup! *****

include rpi_GPIO_lib.fs

: valid_addr ( reg_addr -- reg_addr flag )	\ 0 to 63 are only valid addresses.  Flag returns zero for correct addresses.
    dup 0< if 11 else dup 63 > if 11 else 0 then then ;	

: !ds1307 ( val reg_addr -- flag )		\ Write val to ds1307 reg_addr.  Flag is zero for success. 
    TRY
	valid_addr throw
	piosetup throw pii2csetup throw 104 pii2caddress throw 100000 pii2clock throw
	pad c! pad 1 + c! pad 2 pii2cwrite throw pii2cleanup throw piocleanup throw 0
    RESTORE dup if swap drop swap drop  then	\ ds1307 writing failed error # is on stack at exit of this code
    ENDTRY ;

: @ds1307 ( -- val flag )		\ Read next ds1307 register value.  Flag is zero for success.
    TRY
	piosetup throw pii2csetup throw 104 pii2caddress throw 100000 pii2clock throw
	pad 1 pii2cread throw pii2cleanup throw piocleanup throw pad c@ 0 
    RESTORE	dup if 11 swap  then	\ read error # is on stack at exit of this code
    ENDTRY ;				 
	

: @ds1307_reg	( reg_addr -- val flag )	\ Read reg_addr on ds1307.  Flag is zero for success.
	TRY	valid_addr throw
		0 63 !ds1307 throw		\ This clobers reg 63 or the last ram location in ds1307
		0 ?DO @ds1307 throw drop LOOP 	\ Now reg_addr is the next address to be read.
		@ds1307 			
	RESTORE 
	ENDTRY ;
	
: ds1307_clock_off ( -- flag ) \ Turn on clock
    TRY
	0 @ds1307_reg throw 128 or 0 !ds1307 
    RESTORE 
    ENDTRY ;

: ds1307_clock_on ( -- flag )  \ Clock off
    TRY
	0 @ds1307_reg throw 127 and 0 !ds1307
    RESTORE
    ENDTRY ;

: dectobcd ( n -- n ) \ 8 bit dec to binary coded decimal 
    dup 10 / dup 16 * swap 10 * rot swap - + ;

: bcdtodec ( n -- n ) \ 8 bit binary coded decimal to dec
    dup 16 / dup 10 * swap 16 * rot swap - + ;

: @ds1307_time ( -- sec min hour flag )  \ Read seconds, minutes, hours (24 hour format).  Flag zero for correct reading
    TRY
	0 @ds1307_reg throw 127 and bcdtodec @ds1307 throw 127 and bcdtodec @ds1307 throw  \ reading done
	dup 64 and if dup 32 and if 12 else 0 then swap 31 and bcdtodec + else 63 and bcdtodec then 0
    RESTORE dup if 11 swap 11 swap 11 swap then    \ some read error
    ENDTRY ;

: !ds1307_time ( sec min hour -- flag ) \ Set the time in 24 hour format.  Flag zero for no errors
    TRY
	31 and dectobcd 2 !ds1307 throw 63 and dectobcd 1 !ds1307 throw 63 and 0 !ds1307 throw ds1307_clock_on throw 0 
    RESTORE dup if swap drop swap drop swap drop then
    ENDTRY ;

: @ds1307_date ( -- day date month year flag ) \ day is 1-7, date is 1-31, month 1-12, year 2000-2099.  Flag zero if read was successfull
    TRY
	3 @ds1307_reg throw 7 and bcdtodec @ds1307 throw 63 and bcdtodec @ds1307 throw 31 and bcdtodec @ds1307 throw bcdtodec 2000 + 0
    RESTORE dup if 11 swap 11 swap 11 swap 11 swap then  
    ENDTRY ;

: !ds1307_date ( day date month year -- flag ) \ day is 1-7, date is 1-31, month 1-12, year 2000-2099. Flag zero indicates successfull write
    TRY
	dup 2000 >= if 2000 - dup 99 <= if dectobcd 6 !ds1307 throw else 11 throw then else 11 throw then	\ set year 
	dup 1 >= if dup 12 <= if dectobcd 5 !ds1307 throw else 11 throw then  else 11 throw then		\ set month
	31 and dectobcd 4 !ds1307 throw		\ set date
	7 and dectobcd 3 !ds1307 throw 0	\ set day
    RESTORE dup if swap drop swap drop swap drop swap drop then
    ENDTRY ;
