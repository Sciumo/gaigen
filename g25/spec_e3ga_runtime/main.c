#include <math.h>

#include "e3ga_rt.h"

int main(int argc, char *argv[]) {
	mv x, y, z;
	double cx[3] = {1.0, 2.0, 3.0};
	double cy[3] = {-1.0, -0.5, 0.0};
	int g1, g2;
	z.c[0] = z.c[1] = z.c[2] = z.c[3] = 0.0;

	printf("RUNTIME VERSION\n");

	if (0) {
		printf("test gp\n");
		for (g1 = 0; g1 <=3; g1++) {
			for (g2 = 0; g2 <=3; g2++) {
				mv_setArray(&x, 1 << g1, cx);
				mv_setArray(&y, 1 << g2, cy);
				gp_mv_mv(&z, &x, &y);
				printf("%f %f %f %f\n", z.c[0], z.c[1], z.c[2], z.c[3]);
			}
		}
	}

	if (0) {
		printf("test add, sub, neg\n");
		for (g1 = 0; g1 <=3; g1++) {
			mv_setArray(&x, 1 << g1, cx);
			mv_setArray(&y, 1 << g1, cy);
			add_mv_mv(&z, &x, &y);
			printf("%f %f %f %f\n", z.c[0], z.c[1], z.c[2], z.c[3]);
			subtract_mv_mv(&z, &x, &y);
			printf("%f %f %f %f\n", z.c[0], z.c[1], z.c[2], z.c[3]);
			negate_mv(&z, &x);
			printf("%f %f %f %f\n", z.c[0], z.c[1], z.c[2], z.c[3]);
		}
	}

	if (0) {
		printf("test exp\n");
		mv_setArray(&x, 1 << 2, cx);
		mv_setArray(&y, 1 << 2, cy);
		exp_mv(&z, &x, 12);
		printf("%f %f %f %f\n", z.c[0], z.c[1], z.c[2], z.c[3]);
		exp_mv(&z, &y, 12);
		printf("%f %f %f %f\n", z.c[0], z.c[1], z.c[2], z.c[3]);
	}

	if (1) {
		printf("test dual, undual\n");
		for (g1 = 0; g1 <=3; g1++) {
			mv_setArray(&x, 1 << g1, cx);
			mv_setArray(&y, 1 << g1, cy);
			dual_mv(&z, &x);
			printf("%f %f %f %f\n", z.c[0], z.c[1], z.c[2], z.c[3]);
			undual_mv(&z, &x);
			printf("%f %f %f %f\n", z.c[0], z.c[1], z.c[2], z.c[3]);
		}
	}

	return 0;
}