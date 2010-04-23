#include <stdio.h>

#include "c3ga.h"

namespace c3ga {

	/*
// +
inline vectorE3GA operator+(const vectorE3GA &v1, const vectorE3GA &v2) {
	return add(v1, v2);
}

// +=
inline vectorE3GA &operator+=(vectorE3GA &v1, const vectorE3GA &v2) {
	return (v1 = add(v1, v2));
}

// -
inline vectorE3GA operator-(const vectorE3GA &v1, const vectorE3GA &v2) {
	return subtract(v1, v2);
}

// -=
inline vectorE3GA &operator-=(vectorE3GA &v1, const vectorE3GA &v2) {
	return (v1 = subtract(v1, v2));
}

// unary -
inline vectorE3GA operator-(vectorE3GA &v) {
	return negate(v);
}

// prefix ++
inline scalar &operator++(scalar &s) {
	return (s = add(s, 1.0));
}

// postfix ++
inline scalar operator++(scalar &s, int dummy) {
	scalar result(s);
	s = add(s, 1.0);
	return result;
}
*/
}

int main(int argc, char *argv[]) {
	{
		int a = 1;
		printf("a++ = %d\n", a++);
		a = 1;
		printf("++a = %d\n", ++a);
	}

	c3ga::vectorE3GA v1, v2;
	c3ga::rotorE3GA r1, r2;

	v1.set(v1.coord_e1_e2_e3, 1, 2, 3);
	v2.set(v2.coord_e1_e2_e3, 3, 4, 5);

	printf("v1 = %s,\n", v1.toString().c_str());
	printf("v2 = %s,\n", v2.toString().c_str());

	v1 += -v2;
	v1 += v2;

	printf("v1 = %s,\n", v1.toString().c_str());

	c3ga::mv X;
	c3ga::mv Y = c3ga::mv(v1) * c3ga::mv(v2);

	Y = c3ga::mv(v1) ^ c3ga::mv(r2);
	Y = c3ga::mv(v1) ^ c3ga::mv(r2);
	Y = c3ga::mv(v1) ^ c3ga::mv(r2);
	Y = c3ga::mv(r1) * c3ga::mv(v2);
	Y = gpem(c3ga::mv(r1), c3ga::mv(r2));

	c3ga::ReportUsage::printReport();
	return 0;
}