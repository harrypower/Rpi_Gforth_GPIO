require rpi_GPIO_lib.fs

piosetup 0= if 
25 pisetoutput 0= if s" pin 25 set for output" type cr endif
25 pipinlow 0= if s" pin 25 is low now" type cr endif
1000 ms
25 pipinhigh 0= if s" pin 25 is high now" type cr endif
piocleanup 0= if s" Ok GPIO is now turned off!" endif

else s" oops GPIO did not initalize!" type endif 
