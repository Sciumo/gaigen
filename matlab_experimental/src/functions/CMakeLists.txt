
message( "===== generating files for ${alg} =====" )	    

# for libgaigen_${alg}
link_directories( ../g25 ../mex )

# add libs 
set(MEX_LIBS_LIST e2ga_matlab mex_if_e2ga)


# form mex args
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



file( GLOB MEXSRCS *.cpp  ${GAMATLAB_ALGEBRA_TEMPLATE_DIR}/*.cpp)

foreach(srcfile ${MEXSRCS})
  get_filename_component(basename ${srcfile} NAME_WE) 
  set(filename   "${basename}.${MEXEXT}" )
  set(targetname "${basename}_${alg}" ) 
 
  message ( "build ${filename}" ) 

  add_custom_command(OUTPUT ${filename} 
    COMMAND ${MEXCOMMAND} ${MEX_ARGS} -output ${filename} ${srcfile}  
    DEPENDS ${srcfile} ${GAMatlab_SOURCE_DIR}/src/internal/gamatlab_config.hpp )
  add_custom_target(${targetname} ALL DEPENDS ${filename})
  install(FILES ${CMAKE_CURRENT_BINARY_DIR}/${filename} DESTINATION matlab/algebras/${alg} )
  
endforeach(srcfile)


add_subdirectory ( "@GA" )
add_subdirectory ( "@Outermorphism" )
