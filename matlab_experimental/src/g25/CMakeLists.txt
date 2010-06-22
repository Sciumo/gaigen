
set (SRCS e2ga.c e2ga_parse_mv.c)
add_library (e2ga_matlab SHARED ${SRCS})

install (TARGETS e2ga_matlab LIBRARY DESTINATION lib)
#install (TARGETS gaigen_${alg} RUNTIME DESTINATION lib)
install (FILES ${CMAKE_CURRENT_SOURCE_DIR}/e2ga.h DESTINATION include/g25 )
