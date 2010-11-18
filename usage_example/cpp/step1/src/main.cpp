#include <stdio.h>
#include "c3ga.h"

using namespace c3ga;

/*
This small example creates a line and a plane from points.
It then computes the intersection point of the line and the plane.

In this step, all GA variables are stored in 'mv' (the multivector type).
*/
int main(int argc, char *argv[]) {

	// get five points
	mv linePt1 = cgaPoint(1.0f, 0.0f, 0.0f);
	mv linePt2 = cgaPoint(1.0f, 1.0f, 0.0f);
	mv planePt1 = cgaPoint(1.0f, 2.0f, 0.0f);
	mv planePt2 = cgaPoint(1.0f, 2.0f, 1.0f);
	mv planePt3 = cgaPoint(0.0f, 2.0f, 1.0f);

	// output text the can be copy-pasted into GAViewer
	// warning: c_str() is not multi-threading safe (for that, use toString() instead).
	printf("linePt1 = %s,\n", linePt1.c_str()); 
	printf("linePt2 = %s,\n", linePt2.c_str());
	printf("planePt1 = %s,\n", planePt1.c_str());
	printf("planePt2 = %s,\n", planePt2.c_str());
	printf("planePt3 = %s,\n", planePt3.c_str());

	// create line and plane out of points
	mv L = linePt1 ^ linePt2 ^ ni;
	mv P = planePt1 ^ planePt2 ^ planePt3 ^ ni;

	// output text the can be copy-pasted into GAViewer
	printf("L = %s,\n", L.c_str());
	printf("P = %s,\n", P.c_str());

	// compute intersection of line and plane
	mv intersection = dual(L) << P;

	printf("intersection = %s,\n", intersection.c_str());

	return 0;
}