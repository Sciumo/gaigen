

set(srcs "mex_if.c")

# algebra specific gaigen header - gaigen_if.hpp
include_directories( ${GAMatlab_SOURCE_DIR}/src/mex_if/${alg} )

add_library( mex_if_${alg} SHARED ${srcs} )

install( FILES  ${CMAKE_CURRENT_SOURCE_DIR}/../mex_if.hpp DESTINATION include/gamatlab/${alg} )
install( FILES  ${CMAKE_CURRENT_SOURCE_DIR}/gaigen_if.hpp DESTINATION include/gamatlab/${alg} )
install( TARGETS mex_if_${alg} LIBRARY DESTINATION lib )
#install( TARGETS mex_if_${alg} RUNTIME DESTINATION lib )
