#include <stdio.h>
#include "c3ga.h"

#define BUF_LEN 1024

/*
This small example creates a line and a plane from points.
It then computes the intersection point of the line and the plane.

In this step, all GA variables are stored in 'mv' (the multivector type).
*/
int main(int argc, char *argv[]) {

	normalizedPoint tmpPt;
	mv linePt1, linePt2, planePt1, planePt2, planePt3;
	mv L, P, tmp1, tmp2, mv_ni;
	mv intersection;
	char buf[BUF_LEN];

	// get 'ni' as mv
	ni_t_to_mv(&mv_ni, &ni);

	// get five points (note: need to convert from 'normalizedPoint' to 'mv')
	normalizedPoint_to_mv(&linePt1, cgaPoint_float_float_float(&tmpPt, 1.0f, 0.0f, 0.0f));
	normalizedPoint_to_mv(&linePt2, cgaPoint_float_float_float(&tmpPt, 1.0f, 1.0f, 0.0f));
	normalizedPoint_to_mv(&planePt1, cgaPoint_float_float_float(&tmpPt, 1.0f, 2.0f, 0.0f));
	normalizedPoint_to_mv(&planePt2, cgaPoint_float_float_float(&tmpPt, 1.0f, 2.0f, 1.0f));
	normalizedPoint_to_mv(&planePt3, cgaPoint_float_float_float(&tmpPt, 0.0f, 2.0f, 1.0f));

	// output text the can be copy-pasted into GAViewer
	printf("linePt1 = %s,\n", toString_mv(&linePt1, buf, BUF_LEN, "%2.2f")); 
	printf("linePt2 = %s,\n", toString_mv(&linePt2, buf, BUF_LEN, "%2.2f"));
	printf("planePt1 = %s,\n", toString_mv(&planePt1, buf, BUF_LEN, "%2.2f"));
	printf("planePt2 = %s,\n", toString_mv(&planePt2, buf, BUF_LEN, "%2.2f"));
	printf("planePt3 = %s,\n", toString_mv(&planePt3, buf, BUF_LEN, "%2.2f"));

	// create line and plane out of points
	op_mv_mv(&L, op_mv_mv(&tmp1, &linePt1, &linePt2), &mv_ni);
	op_mv_mv(&P, op_mv_mv(&tmp2, op_mv_mv(&tmp1, &planePt1, &planePt2), &planePt3), &mv_ni);

	// output text the can be copy-pasted into GAViewer
	printf("L = %s,\n", toString_mv(&L, buf, BUF_LEN, "%2.2f")); 
	printf("P = %s,\n", toString_mv(&P, buf, BUF_LEN, "%2.2f")); 

	// compute intersection of line and plane
	dual_mv(&tmp1, &L);
	lc_mv_mv(&intersection, &tmp1, &P);

	printf("intersection = %s,\n", toString_mv(&intersection, buf, BUF_LEN, "%2.2f"));

	return 0;
}