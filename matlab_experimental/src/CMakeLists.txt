cmake_minimum_required(VERSION 2.6)

project(E2GA) 

set (E2GA_VERSION_MAJOR 1)
set (E2GA_VERSION_MINOR 0)

message( "E2GA_SOURCE_DIR  = " ${E2GA_SOURCE_DIR} )

######################################################################
# Set up Matlab stuff for building mex files
# get mex file extention
execute_process(COMMAND  mexext OUTPUT_VARIABLE  MEXEXT)
##### EDIT HERE #####
# - if mexext cant be found you can try manually setting it here e.g.
# set (MEXEXT mexa64)   # unix 64 bit
# set (MEXEXT mexglx)   # unix 32 bit
#set (MEXEXT mexw64)    # windows 64 bit
string(STRIP ${MEXEXT} MEXEXT)    # mexext seems to add a CR
message( "MEXEXT               = " ${MEXEXT} )

set (MEXCOMMAND mex)
	


include_directories( /matlab/extern/include mex )

add_subdirectory(g25)
add_subdirectory(mex)
add_subdirectory(functions)