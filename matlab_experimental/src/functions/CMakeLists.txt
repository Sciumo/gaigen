
message( "===== generating files for ${alg} =====" )	    

# for libgaigen_${alg}
link_directories( ../g25 ../mex )

# add libs 
set(MEX_LIBS_LIST e2ga_matlab mex_if_e2ga)


# form mex command line args
get_directory_property(INCDIRS_LIST  INCLUDE_DIRECTORIES )
foreach( dir ${INCDIRS_LIST} )
  list(APPEND MEX_ARGS -I${dir})
endforeach(dir)
get_directory_property( LINKDIRS_LIST LINK_DIRECTORIES )
foreach( dir ${LINKDIRS_LIST} )
  list(APPEND MEX_ARGS -L${dir})
endforeach(dir)
foreach( lib ${MEX_LIBS_LIST} )
  list(APPEND MEX_ARGS -l${lib})
endforeach(lib)


# explicitly list all the mex functions:
set( MEXSRCS ${E2GA_SOURCE_DIR}/functions/e1.c )

foreach(srcfile ${MEXSRCS})
  get_filename_component(basename ${srcfile} NAME_WE) 
  set(filename   "${basename}.${MEXEXT}" )
  set(targetname "${basename}" ) 
 
  message ( "build ${filename}" ) 

	message("${MEXCOMMAND} ${MEX_ARGS} -output ${filename} ${srcfile}")

  add_custom_command(OUTPUT ${filename} 
    COMMAND ${MEXCOMMAND} -O CFLAGS="\$CFLAGS -std=C99" ${MEX_ARGS} -output ${filename} ${srcfile}  
    DEPENDS ${srcfile}) 
  add_custom_target(${targetname} ALL DEPENDS ${filename})
  
  
  #install(FILES ${CMAKE_CURRENT_BINARY_DIR}/${filename} DESTINATION matlab/algebras/${alg} )
  
endforeach(srcfile)


#add_subdirectory ( "@GA" )
#add_subdirectory ( "@Outermorphism" )
