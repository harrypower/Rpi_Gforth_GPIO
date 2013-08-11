#! /usr/bin/gforth

\ This Gforth code is a Raspberry Pi DTH 11/22 sensor reader
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
\ sudo ./dth_11_22.fs -11_24 to read the dth11 sensor on pin 24 with a 5 k resistor pullup to 3.3 volts.
\ The code needs gforth installed on your rasberry pi and version 0.7.0 up should work properly.
\ Use ./dth_11_22.fs -help to see other options for sensor use and pin use.
\ The code will simply put out with stdout to give you values of error, temperature, humidity.
\ The values are as follows:
\ Error should be 0 meaning the temperatue and humidity values are good.
\ Temperature value needs to be divided by 10 to get the celsius value with correct decimal place.
\ Humidity value needs to be divided by 10 to get the % relavite humidity value with correct decimal place.
\ This code will shutdown if another version of dt1_11_22.fs is running.
\ This is needed because the sensor or pi could possibly be damaged if the same pin is accessed by by two running codes.

\ error 1000 -- to many transitions have happened to be a proper message from dth device
\ error 1001 -- failed to find bit # 40 in data sample so data is bad
\ error 1002 -- checksum did not add up so data is bad
\ error 1003 -- another copy of this dth code is currently busy with some sensors so it is busy!
\ error 1004 -- after 10 reading attemps dth sensor failed to be read
\ error 1005 -- incorrect header detected data is bad

warnings off

include rpi_GPIO_lib.fs
include string.fs

1000 constant many_transitions_fail
1001 constant bit_40_not_found_fail
1002 constant checksum_fail
1003 constant dth_busy_fail
1004 constant 10_trys_fail
1005 constant no_header_fail

1500 constant max_data_quantity
100 constant max_transitions
max_transitions 1 - constant max_timings
18 constant dth_start_time

25 value gpio_dth_pin
0 value dth_data_location
0 value dth_data_transitions
0 value dth_data_timings
0 value dth_data_bits
0 value dth_transitions_size
0 value dth_timings_index
0 value dth_data_bits_index
11 value dth-11-22?
20 value retry-times
0 value start_bit_value
0 value bit_40_location

variable junk$
variable dth_self$

junk$ $init
s" dth_11_22.fs" dth_self$ $!

: dth_var_reset ( -- )
    0 to dth_timings_index
    0 to dth_transitions_size
    0 to start_bit_value
    0 to bit_40_location
    0 to dth_data_bits_index
    dth_data_location 0<>
    if
	dth_data_transitions max_transitions cell * 0 fill
	dth_data_timings max_timings cell * 0 fill
	dth_data_bits 40 0 fill
    then ;

: dth_data_storage_setup ( -- ) \ gets the memory for storage of dth reading procedure
    dth_data_location 0=
    if
	max_data_quantity allocate throw to dth_data_location
	max_transitions cell * allocate throw to dth_data_transitions
	dth_data_transitions max_transitions cell * 0 fill 
	max_timings cell * allocate throw to dth_data_timings
	dth_data_timings max_timings cell * 0 fill
	40 allocate throw to dth_data_bits
	dth_data_bits 40 0 fill
    then  ;

: dth_start_signal ( -- ) \ will put dth sensor in sampling mode
    piosetup throw gpio_dth_pin pipinsetpulldisable throw gpio_dth_pin pipinhigh throw
    gpio_dth_pin pipinoutput throw gpio_dth_pin pipinhigh throw gpio_dth_pin pipinlow throw
    dth_start_time ms gpio_dth_pin pipinhigh throw gpio_dth_pin pipininput throw  ;

: dth_shutdown ( -- ) \ clean up gpio mode
    piocleanup throw ;  

: dth_read ( -- nvalue )
    gpio_dth_pin pad pipinread throw pad c@ ;

: dth_get_data ( -- )
    dth_var_reset dth_data_storage_setup
    max_data_quantity 0 dth_start_signal ?do dth_read dth_data_location i + c! loop dth_shutdown ;

: dth_data@ ( nindex -- nvalue ) \ retreaves unprocesed raw dth stored readings
    dth_data_location + c@ ;

: seedata ( nvalue -- ) \ testing word to see nvalue amount of the data starting at location 0
    cr 0 ?do i dth_data@  . s"  " type loop ; 

: transitions! ( nvalue -- ) \ store list of transitions 
    dth_transitions_size cell * dth_data_transitions + !
    dth_transitions_size 1 + to dth_transitions_size
    dth_transitions_size max_transitions > if many_transitions_fail throw then ;

: transitions ( -- ) \ make list of transitions to work with
    0 dth_data@ { nvalue } max_data_quantity 1 ?do i dth_data@ nvalue <> if i transitions! i dth_data@ to nvalue then loop ;

: transitions@ ( nindex -- nvalue ) \ get the transition from list
    cell * dth_data_transitions + @ ;

: seetransitions ( nvalue -- ) \ testing word to see transition list
    cr 0 ?do i transitions@ . s"  " type loop ;

: timings! ( nvalue -- )
    dth_timings_index cell * dth_data_timings + !
    dth_timings_index 1 + to dth_timings_index ;

: timings ( -- )
    0 transitions@ { nvalue } max_transitions 1 ?do i transitions@ nvalue - timings! i transitions@ to nvalue loop ;

: timings@ ( nindex -- nvalue )
    cell * dth_data_timings + @ ;

: seetimings ( nvalue -- ) \ testing word for timings list
    cr 0 ?do i timings@ . s"  " type loop ;

: find_start_bit ( -- )
    max_timings 0 ?do i timings@ 0 < if i 1 - timings@ to start_bit_value i 2 - to bit_40_location leave then loop
    bit_40_location 0 = if bit_40_not_found_fail throw then ;

: dth_bits! ( cvalue nindex -- ) dth_data_bits + c! ;

: dth_bits@ ( nindex -- cvalue ) dth_data_bits + c@ ;

: dth_bits ( -- )
    bit_40_location 2 + bit_40_location 78 -
    ?do
	i timings@ start_bit_value >
	if 1 else 0 then dth_data_bits_index dup 1 + to dth_data_bits_index dth_bits!
    2 +loop ;

: seebits ( -- ) \ displays the dth bits recieved as interpreted 
    cr 40 0 ?do i dth_bits@ . s"  " type loop ;

: 8bits>byte ( n7 n6 n5 n4 n3 n2 n1 n0 - nbyte )
    swap 2 * +
    swap 4 * +
    swap 8 * +
    swap 16 * +
    swap 32 * +
    swap 64 * +
    swap 128 * + ;

: get_dth_data ( -- nrh nt  )
    8 0 ?do i dth_bits@ loop 
    8bits>byte
    16 8 ?do i dth_bits@ loop
    8bits>byte
    24 16 ?do i dth_bits@ loop
    8bits>byte
    32 24 ?do i dth_bits@ loop
    8bits>byte
    40 32 ?do i dth_bits@ loop
    8bits>byte
    { nrh nrhd nt ntd nack }
    nrh nrhd + nt + ntd + 255 and nack <> if checksum_fail throw then
    dth-11-22? 11 =
    if
	nrh 10 *  nt 10 * 
    else
	nrh 256 * nrhd +
	nt 128 >= if nt 127 and 256 * ntd + -1 * else nt 256 * ntd + then 
    then ;

: check_header ( -- )
    0 transitions@ 100 > if no_header_fail throw then
    1 transitions@ 100 > if no_header_fail throw then
    2 transitions@ 100 > if no_header_fail throw then ;

: testit ( -- ) \ testing word to show data from dth device
    dth_get_data max_data_quantity seedata
    transitions  max_transitions seetransitions
    timings max_timings seetimings
    find_start_bit
    dth_bits
    seebits
    cr get_dth_data . . ;

: dth_parse ( -- ntemp nhumd nflag )
    try
	dth_get_data transitions check_header timings find_start_bit dth_bits get_dth_data false
    restore dup if 0 swap 0 swap then
    endtry ;

: dth_busy? ( -- nflag ) \ false means dth_11_22.fs is running only once this is the only process
                         \ true means dth_11_22.fs is running more then once 
    try                  \ or there is some system error not allowing detection of processes running!
	s" pgrep -c " junk$ $! dth_self$ $@ junk$ $+! junk$ $@ r/o open-pipe throw { mypipe }
	pad 80 mypipe read-file throw pad swap s>number?
        if 2drop true else d>s 1 <= if false else true then then
    restore 
    endtry ; 
    
: get_temp_humd ( -- )
    dth_busy? false =
    if
	10 { ntimes } 1 2 3
	begin
	    drop drop drop 
	    ntimes 0 >
	    if
		ntimes 1 - to ntimes
		dth_parse dup 0 = if true else  false 2000 ms then
	    else 0 0 10_trys_fail true
	    then
	until
    else
	0 0 dth_busy_fail
    then
    . . . ;

: pin_check ( npin -- nflag ) \ false returned means pin not allowed true returned means pin is allowed for dth sensor
    case
	4 of true endof
	7 of true endof
	8 of true endof
	9 of true endof
	10 of true endof
	11 of true endof
	17 of true endof
	18 of true endof
	22 of true endof
	23 of true endof
	24 of true endof
	25 of true endof
	27 of true endof
	false swap
    endcase ;

: get_pin ( caddr u -- nflag ) \ nflag is false for pin not allowed or present in command line switch
    -trailing 4 /string s>number? if d>s dup pin_check if to gpio_dth_pin true else drop false then else 2drop false then ;
   
: config-dth-type
    next-arg dup 0=
    if
	." Argument needed!" cr 2drop s" -help"
    then

    s" -help" search
    if
	." -22_xx  use for dth22 sensor raspberry pi gpio pin xx" cr
	." -11_xx  use for dth11 sensor raspberry pi gpio pin xx" cr
	." Replace xx with the raspberry pi gpio pin number. Not all pins can be used with the sensors.  The pins need to be pulled up with a 5 k resistor to 3.3 volts of gpio header only!" cr
	2drop bye
    then
    s" -22_" search
    if
	22 to dth-11-22? get_pin if get_temp_humd bye else ." GPIO pin not allowed with DTH22 sensor!" cr bye then 
    then
    s" -11_" search
    if
	11 to dth-11-22? get_pin if get_temp_humd bye else ." GPIO pin not allowed with DTH11 sensor!" cr bye then
    then
    ." Switch not supported!" cr
    ." -22_xx  use for dth22 sensor raspberry pi gpio pin xx" cr
    ." -11_xx  use for dth11 sensor raspberry pi gpio pin xx" cr
    ." Replace xx with the raspberry pi gpio pin number. Not all pins can be used with the sensors.  The pins need to be pulled up with a 5 k resistor to 3.3 volts of gpio header only!" cr
    2drop bye ;

  config-dth-type