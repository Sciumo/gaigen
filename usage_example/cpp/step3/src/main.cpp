#include <stdio.h>
#include "c3ga.h"

using namespace c3ga;

/*
This small example creates a line and a plane from points.
It then computes the intersection point of the line and the plane.

In this step, the 'reportUsage' funtionality is used to extract what
specialized functions and types are missing.
*/
int main(int argc, char *argv[]) {

	// get five points
	normalizedPoint linePt1 = cgaPoint(1.0f, 0.0f, 0.0f);
	normalizedPoint linePt2 = cgaPoint(1.0f, 1.0f, 0.0f);
	normalizedPoint planePt1 = cgaPoint(1.0f, 2.0f, 0.0f);
	normalizedPoint planePt2 = cgaPoint(1.0f, 2.0f, 1.0f);
	normalizedPoint planePt3 = cgaPoint(0.0f, 2.0f, 1.0f);

	// output text the can be copy-pasted into GAViewer
	// warning: c_str() is not multi-threading safe (for that, use toString() instead).
	printf("linePt1 = %s,\n", linePt1.c_str()); 
	printf("linePt2 = %s,\n", linePt2.c_str());
	printf("planePt1 = %s,\n", planePt1.c_str());
	printf("planePt2 = %s,\n", planePt2.c_str());
	printf("planePt3 = %s,\n", planePt3.c_str());

	// create line and plane out of points
	line L = _line(linePt1 ^ linePt2 ^ ni);
	plane P = _plane(planePt1 ^ planePt2 ^ planePt3 ^ ni);

	// output text the can be copy-pasted into GAViewer
	printf("L = %s,\n", L.c_str());
	printf("P = %s,\n", P.c_str());

	// compute intersection of line and plane
	flatPoint intersection = _flatPoint(dual(L) << P);

	printf("intersection = %s,\n", intersection.c_str());

	// output what functions could be optimized
	c3ga::ReportUsage::printReport();

	return 0;
}