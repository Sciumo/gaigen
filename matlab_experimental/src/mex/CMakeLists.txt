

set(srcs "mex_if.c")

add_library( mex_e2ga SHARED ${srcs} )

#install( FILES  ${CMAKE_CURRENT_SOURCE_DIR}/../mex_if.hpp DESTINATION include/gamatlab/${alg} )
#install( FILES  ${CMAKE_CURRENT_SOURCE_DIR}/gaigen_if.hpp DESTINATION include/gamatlab/${alg} )

#install( TARGETS mex_if_e2ga LIBRARY DESTINATION lib )
#install( TARGETS mex_if_e2ga RUNTIME DESTINATION lib )
