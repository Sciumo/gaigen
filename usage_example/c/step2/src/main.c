#include <stdio.h>
#include "c3ga.h"

#define BUF_LEN 1024
/*
This small example creates a line and a plane from points.
It then computes the intersection point of the line and the plane.

In this step, all GA variables are stored in 'mv' (the multivector type).
*/
int main(int argc, char *argv[]) {

	normalizedPoint linePt1, linePt2, planePt1, planePt2, planePt3;
	line L, tmpL;
	plane P;
	flatPoint fp, intersection;
	dualLine dl;
	mv tmp1;
	char buf[BUF_LEN];

	// get five points
	cgaPoint_float_float_float(&linePt1, 1.0f, 0.0f, 0.0f);
	cgaPoint_float_float_float(&linePt2, 1.0f, 1.0f, 0.0f);
	cgaPoint_float_float_float(&planePt1, 1.0f, 2.0f, 0.0f);
	cgaPoint_float_float_float(&planePt2, 1.0f, 2.0f, 1.0f);
	cgaPoint_float_float_float(&planePt3, 0.0f, 2.0f, 1.0f);

	// output text the can be copy-pasted into GAViewer
	// warning: c_str() is not multi-threading safe (for that, use toString() instead).
	printf("linePt1 = %s,\n", toString_mv(normalizedPoint_to_mv(&tmp1, &linePt1), buf, BUF_LEN, "%2.2f")); 
	printf("linePt2 = %s,\n", toString_mv(normalizedPoint_to_mv(&tmp1, &linePt2), buf, BUF_LEN, "%2.2f"));
	printf("planePt1 = %s,\n", toString_mv(normalizedPoint_to_mv(&tmp1, &planePt1), buf, BUF_LEN, "%2.2f"));
	printf("planePt2 = %s,\n", toString_mv(normalizedPoint_to_mv(&tmp1, &planePt2), buf, BUF_LEN, "%2.2f"));
	printf("planePt3 = %s,\n", toString_mv(normalizedPoint_to_mv(&tmp1, &planePt3), buf, BUF_LEN, "%2.2f"));

	// create line and plane out of points
	op_normalizedPoint_flatPoint(&L, &linePt1, op_normalizedPoint_ni_t(&fp, &linePt2, &ni));
	op_normalizedPoint_line(&P, &planePt1, op_normalizedPoint_flatPoint(&tmpL, &planePt2, op_normalizedPoint_ni_t(&fp, &planePt3, &ni)));

	// output text the can be copy-pasted into GAViewer
	printf("L = %s,\n", toString_mv(line_to_mv(&tmp1, &L), buf, BUF_LEN, "%2.2f")); 
	printf("P = %s,\n", toString_mv(plane_to_mv(&tmp1, &P), buf, BUF_LEN, "%2.2f")); 

	// compute intersection of line and plane
	lc_dualLine_plane(&intersection, dual_line(&dl, &L), &P);

	printf("intersection = %s,\n", toString_mv(flatPoint_to_mv(&tmp1, &intersection), buf, BUF_LEN, "%2.2f"));

	return 0;
}