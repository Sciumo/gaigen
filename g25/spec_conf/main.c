#include <math.h>

#include "c3ga.h"

int main(int argc, char *argv[]) {
	mv_f A,B;
	char buf[1024];
	random_blade_f(&A, 1.0, 4, -1);
	random_versor_f(&B, 1.0, 4, -1);

	printf("%s\n", toString_mv_f(&A, buf, 1024, "%f"));
	printf("%s\n", toString_mv_f(&B, buf, 1024, "%f"));
	return 0;
}