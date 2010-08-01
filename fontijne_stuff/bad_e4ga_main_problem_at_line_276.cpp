#include <time.h> /* used to seed random generator */
#include "e4ga.h"
namespace e4ga {
// Missing dependencies declarations:

/**
Generates a random versor.
The scale is uniformly distributed over the interval [0 1).
The maximum non-zero grade-part is 'grade'.
Only the basis vectors marked in 'basisVectorBitmap' will be used to generate the
versor/blade. Use 'basisVectorBitmap = -1' (the default) to use all basisvectors.

Returns random_versor_dont_mangle_1_returns_mv_ex(arg1, scale, grade, basisVectorBitmap, 0.01, scale * 4.0);
*/
mv random_versor_dont_mangle_1_returns_mv(float scale, int grade, int basisVectorBitmap = -1);

/**
This version of random_versor_dont_mangle_1_returns_mv() has extra arguments which help to avoid 
near-singular blades / versors.

Near-singular blades / versors are avoid by testing the norm and largest coordinate
of the random blade / versor. If the test does not pass, the function recursively
tries to generate another random blade / versor.

'minimumNorm' is the minimum allowed norm of the blade/versor before scaling. 
'minimumNorm' must be > 0.0 for versors.

'largestCoordinate' is the largest coordinate allowed after scaling.
*/
mv random_versor_dont_mangle_1_returns_mv_ex(float scale, int grade, int basisVectorBitmap, float minimumNorm, float largestCoordinate);


/**
Generates a random blade.
The scale is uniformly distributed over the interval [0 1).
The maximum non-zero grade-part is 'grade'.
Only the basis vectors marked in 'basisVectorBitmap' will be used to generate the
versor/blade. Use 'basisVectorBitmap = -1' (the default) to use all basisvectors.

Returns random_blade_dont_mangle_3_returns_mv_ex(arg1, scale, grade, basisVectorBitmap, 0.01, scale * 4.0);
*/
mv random_blade_dont_mangle_3_returns_mv(float scale, int grade, int basisVectorBitmap = -1);

/**
This version of random_blade_dont_mangle_3_returns_mv() has extra arguments which help to avoid 
near-singular blades / versors.

Near-singular blades / versors are avoid by testing the norm and largest coordinate
of the random blade / versor. If the test does not pass, the function recursively
tries to generate another random blade / versor.

'minimumNorm' is the minimum allowed norm of the blade/versor before scaling. 
'minimumNorm' must be > 0.0 for versors.

'largestCoordinate' is the largest coordinate allowed after scaling.
*/
mv random_blade_dont_mangle_3_returns_mv_ex(float scale, int grade, int basisVectorBitmap, float minimumNorm, float largestCoordinate);

// Missing dependencies inline definitions:
// Missing dependencies definitions:
/// Computes the partial geometric product of two multivectors (group 0  x  group 0 -> group 0)
void gp_default_0_0_0(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 1 -> group 1)
void gp_default_0_1_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 2 -> group 2)
void gp_default_0_2_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 3 -> group 3)
void gp_default_0_3_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 4 -> group 4)
void gp_default_0_4_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 0 -> group 1)
void gp_default_1_0_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 0)
void gp_default_1_1_0(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 2)
void gp_default_1_1_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 1)
void gp_default_1_2_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 3)
void gp_default_1_2_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 3 -> group 2)
void gp_default_1_3_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 3 -> group 4)
void gp_default_1_3_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 4 -> group 3)
void gp_default_1_4_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 0 -> group 2)
void gp_default_2_0_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 1)
void gp_default_2_1_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 3)
void gp_default_2_1_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 0)
void gp_default_2_2_0(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 2)
void gp_default_2_2_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 4)
void gp_default_2_2_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 1)
void gp_default_2_3_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 3)
void gp_default_2_3_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 2)
void gp_default_2_4_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 0 -> group 3)
void gp_default_3_0_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 1 -> group 2)
void gp_default_3_1_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 1 -> group 4)
void gp_default_3_1_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 1)
void gp_default_3_2_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 3)
void gp_default_3_2_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 0)
void gp_default_3_3_0(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 2)
void gp_default_3_3_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 1)
void gp_default_3_4_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 0 -> group 4)
void gp_default_4_0_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 3)
void gp_default_4_1_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 2)
void gp_default_4_2_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 1)
void gp_default_4_3_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 0)
void gp_default_4_4_0(const float *A, const float *B, float *C);
/// copies coordinates of group 0
void copyGroup_0(const float *A, float *C);
/// copies and multiplies (by s) coordinates of group 0
void copyMul_0(const float *A, float *C, float s);
/// copies and divides (by s) coordinates of group 0
void copyDiv_0(const float *A, float *C, float s);
/// adds coordinates of group 0 from variable A to C
void add_0(const float *A, float *C);
/// subtracts coordinates of group 0 in variable A from C
void sub_0(const float *A, float *C);
/// negate coordinates of group 0 of variable A
void neg_0(const float *A, float *C);
/// adds coordinates of group 0 of variables A and B
void add2_0_0(const float *A, const float *B, float *C);
/// subtracts coordinates of group 0 of variables A from B
void sub2_0_0(const float *A, const float *B, float *C);
/// performs coordinate-wise multiplication of coordinates of group 0 of variables A and B
void hp_0_0(const float *A, const float *B, float *C);
/// performs coordinate-wise division of coordinates of group 0 of variables A and B
/// (no checks for divide by zero are made)
void ihp_0_0(const float *A, const float *B, float *C);
/// check for equality up to eps of coordinates of group 0 of variables A and B
bool equals_0_0(const float *A, const float *B, float eps);
/// checks if coordinates of group 0 of variable A are zero up to eps
bool zeroGroup_0(const float *A, float eps);
/// copies coordinates of group 1
void copyGroup_1(const float *A, float *C);
/// copies and multiplies (by s) coordinates of group 1
void copyMul_1(const float *A, float *C, float s);
/// copies and divides (by s) coordinates of group 1
void copyDiv_1(const float *A, float *C, float s);
/// adds coordinates of group 1 from variable A to C
void add_1(const float *A, float *C);
/// subtracts coordinates of group 1 in variable A from C
void sub_1(const float *A, float *C);
/// negate coordinates of group 1 of variable A
void neg_1(const float *A, float *C);
/// adds coordinates of group 1 of variables A and B
void add2_1_1(const float *A, const float *B, float *C);
/// subtracts coordinates of group 1 of variables A from B
void sub2_1_1(const float *A, const float *B, float *C);
/// performs coordinate-wise multiplication of coordinates of group 1 of variables A and B
void hp_1_1(const float *A, const float *B, float *C);
/// performs coordinate-wise division of coordinates of group 1 of variables A and B
/// (no checks for divide by zero are made)
void ihp_1_1(const float *A, const float *B, float *C);
/// check for equality up to eps of coordinates of group 1 of variables A and B
bool equals_1_1(const float *A, const float *B, float eps);
/// checks if coordinates of group 1 of variable A are zero up to eps
bool zeroGroup_1(const float *A, float eps);
/// copies coordinates of group 2
void copyGroup_2(const float *A, float *C);
/// copies and multiplies (by s) coordinates of group 2
void copyMul_2(const float *A, float *C, float s);
/// copies and divides (by s) coordinates of group 2
void copyDiv_2(const float *A, float *C, float s);
/// adds coordinates of group 2 from variable A to C
void add_2(const float *A, float *C);
/// subtracts coordinates of group 2 in variable A from C
void sub_2(const float *A, float *C);
/// negate coordinates of group 2 of variable A
void neg_2(const float *A, float *C);
/// adds coordinates of group 2 of variables A and B
void add2_2_2(const float *A, const float *B, float *C);
/// subtracts coordinates of group 2 of variables A from B
void sub2_2_2(const float *A, const float *B, float *C);
/// performs coordinate-wise multiplication of coordinates of group 2 of variables A and B
void hp_2_2(const float *A, const float *B, float *C);
/// performs coordinate-wise division of coordinates of group 2 of variables A and B
/// (no checks for divide by zero are made)
void ihp_2_2(const float *A, const float *B, float *C);
/// check for equality up to eps of coordinates of group 2 of variables A and B
bool equals_2_2(const float *A, const float *B, float eps);
/// checks if coordinates of group 2 of variable A are zero up to eps
bool zeroGroup_2(const float *A, float eps);
/// copies coordinates of group 3
void copyGroup_3(const float *A, float *C);
/// copies and multiplies (by s) coordinates of group 3
void copyMul_3(const float *A, float *C, float s);
/// copies and divides (by s) coordinates of group 3
void copyDiv_3(const float *A, float *C, float s);
/// adds coordinates of group 3 from variable A to C
void add_3(const float *A, float *C);
/// subtracts coordinates of group 3 in variable A from C
void sub_3(const float *A, float *C);
/// negate coordinates of group 3 of variable A
void neg_3(const float *A, float *C);
/// adds coordinates of group 3 of variables A and B
void add2_3_3(const float *A, const float *B, float *C);
/// subtracts coordinates of group 3 of variables A from B
void sub2_3_3(const float *A, const float *B, float *C);
/// performs coordinate-wise multiplication of coordinates of group 3 of variables A and B
void hp_3_3(const float *A, const float *B, float *C);
/// performs coordinate-wise division of coordinates of group 3 of variables A and B
/// (no checks for divide by zero are made)
void ihp_3_3(const float *A, const float *B, float *C);
/// check for equality up to eps of coordinates of group 3 of variables A and B
bool equals_3_3(const float *A, const float *B, float eps);
/// checks if coordinates of group 3 of variable A are zero up to eps
bool zeroGroup_3(const float *A, float eps);
/// copies coordinates of group 4
void copyGroup_4(const float *A, float *C);
/// copies and multiplies (by s) coordinates of group 4
void copyMul_4(const float *A, float *C, float s);
/// copies and divides (by s) coordinates of group 4
void copyDiv_4(const float *A, float *C, float s);
/// adds coordinates of group 4 from variable A to C
void add_4(const float *A, float *C);
/// subtracts coordinates of group 4 in variable A from C
void sub_4(const float *A, float *C);
/// negate coordinates of group 4 of variable A
void neg_4(const float *A, float *C);
/// adds coordinates of group 4 of variables A and B
void add2_4_4(const float *A, const float *B, float *C);
/// subtracts coordinates of group 4 of variables A from B
void sub2_4_4(const float *A, const float *B, float *C);
/// performs coordinate-wise multiplication of coordinates of group 4 of variables A and B
void hp_4_4(const float *A, const float *B, float *C);
/// performs coordinate-wise division of coordinates of group 4 of variables A and B
/// (no checks for divide by zero are made)
void ihp_4_4(const float *A, const float *B, float *C);
/// check for equality up to eps of coordinates of group 4 of variables A and B
bool equals_4_4(const float *A, const float *B, float eps);
/// checks if coordinates of group 4 of variable A are zero up to eps
bool zeroGroup_4(const float *A, float eps);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_0_4(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_0_4(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_1_3(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_1_3(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_2_2(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_2_2(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_3_1(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_3_1(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_4_0(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_4_0(const float *A, float *C);
;
		
		// make it a multivector:
		randomVector.set(GRADE_1, randomValues);
		
		// multiply 
		(*IR2) = gp(*IR1, randomVector);
		
		// swap IR pointers: 
		std::swap(IR1, IR2);
		
		// lower grade
		grade--;
	}
	
	// compute norm/multiplier, apply it, or recurse if we happened to create a near-null versor
	n2 = norm_returns_scalar(*IR1);
	if ((float)fabs(n2) > minimumNorm * minimumNorm) {
		if (n2 != 0.0f) {
			mul = scale * genrand() / n2;
			if (IR1->largestCoordinate() * mul < largestCoordinate)
				return gp(*IR1, mul);
		}
		else if (IR1->largestCoordinate() < largestCoordinate)
			return *IR1;
	}
	
	// try again:
	return random_versor_dont_mangle_1_returns_mv_ex(scale, _grade, basisVectorBitmap, minimumNorm, largestCoordinate); 
}


mv random_blade_dont_mangle_3_returns_mv(float scale, int grade, int basisVectorBitmap) {
	float minimumNorm = 0.01f;
	float largestCoordinate = 4.0f;
	return random_blade_dont_mangle_3_returns_mv_ex(scale, grade, basisVectorBitmap, minimumNorm, scale * largestCoordinate);
}

mv random_blade_dont_mangle_3_returns_mv_ex(float scale, int _grade, int basisVectorBitmap, 
		float minimumNorm, float largestCoordinate) 
{
	mv randomVector, tmp1, tmp2;
	mv *IR1 = &tmp1, *IR2 = &tmp2; // IR = intermediate result
	float randomValues[4];
	float n2, mul;
	int i;
	int grade = _grade;
	
	// set IR1 (intermediate result) to 1
	IR1->set(1.0f);

	while (grade > 0) {	// loop until grade == 0
		// fill array with random values
		for (i = 0; i < 4; i++) 
			randomValues[i] = (basisVectorBitmap & (1 << i))
				? (-1.0f + 2.0f * genrand())
				: 0.0f;
		
		// make it a multivector:
		randomVector.set(GRADE_1, randomValues);
		
		// multiply 
		(*IR2) = op(*IR1, randomVector);
		
		// swap IR pointers: 
		std::swap(IR1, IR2);
		
		// lower grade
		grade--;
	}
	
	// compute norm/multiplier, apply it, or recurse if we happened to create a near-null versor
	n2 = norm_returns_scalar(*IR1);
	if ((float)fabs(n2) > minimumNorm * minimumNorm) {
		if (n2 != 0.0f) {
			mul = scale * genrand() / n2;
			if (IR1->largestCoordinate() * mul < largestCoordinate)
				return gp(*IR1, mul);
		}
		else if (IR1->largestCoordinate() < largestCoordinate)
			return *IR1;
	}
	
	// try again:
	return random_blade_dont_mangle_3_returns_mv_ex(scale, _grade, basisVectorBitmap, minimumNorm, largestCoordinate); 
}
// Testing code declarations:
// Testing code inline definitions:

int test_genrand_float(int NB_TESTS_SCALER) 
{
	int NB_BINS = 256;
	const int NB_LOOPS = NB_BINS * 1024;
	int bins[256];
	float avg = 0.0f;
	float r;
	int idx, i;
	
	// reset count of bins
	memset(bins, 0, NB_BINS * sizeof(int));
	
	// generate a lot of random values, compute average (should be 0.5) and fill bins
	for (i = 0; i < NB_LOOPS; i++) {
		r = genrand();
		avg += r;
		idx = (int)(r * (float)NB_BINS);
		if (idx >= NB_BINS) idx = NB_BINS-1;
		bins[idx]++;
	}
	avg /= (float)NB_LOOPS;
	
	if ((avg < 0.49f) || (avg > 0.51f)) {
		printf("Random number generator genrand() likely failed: average is %f instead of 0.5.\n", (double)avg);
		return 0; // failure
	}
	
	for (i = 0; i < NB_BINS; i++) {
		if ((bins[i] < (6 * NB_LOOPS / NB_BINS / 8)) ||
			(bins[i] > (10 * NB_LOOPS / NB_BINS / 8))) {
			printf("It is very likely that the random number generator genrand() failed:\n");
			printf("The distribution is not uniform (bin %d of %d ,value=%d, expected value=%d)\n", i, NB_BINS, bins[i], NB_LOOPS / NB_BINS);
			return 0; // failure
		}
	}
	
	return 1; // success
}

int test_add_dont_mangle_52(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C;
	int i, g;
	float s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		g = (int)(genrand() * 4.5f);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);
		
		// B = -A
		B = negate(A);
		
		C = add(A, B);
		
		// use mv_largestCoordinate() to verify
		if (C.largestCoordinate() > 9.9999999999999991E-06f) {
			printf("add() test failed\n");
			return 0; // failure
		}
		
	}
	return 1; // success
}

int test_subtract_dont_mangle_53(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C;
	int i, g;
	float s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		g = (int)(genrand() * 4.5f);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);
		
		B = A;
		
		C = subtract(A, B);
		
		// use mv_largestCoordinate() to verify
		if (C.largestCoordinate() > 9.9999999999999991E-06f) {
			printf("subtract() test failed\n");
			return 0; // failure
		}
		
	}
	return 1; // success
}

int test_applyVersor_dont_mangle_85(int NB_TESTS_SCALER) {
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	float baseScale = 0.5f;
	int g, i;
	float s;
	mv V, IV, X, Y, VX, VY, IVVX, XdY, VXdVY, dif;
	int versorBasisVectorBitmap = 15; // note: random versors restricted to Euclidean basis vectors.
	int bladeBasisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor and its inverse. Optionally make versor unit.
		s = baseScale + genrand();
		g = (int)(genrand() * 4.5f);
		V = random_versor_dont_mangle_1_returns_mv(s, g, versorBasisVectorBitmap);
		
		// avoid 'near'-singular versors
		if (V.largestCoordinate() > 2.0f)
			continue;		
		
		IV = versorInverse(V);

		// get two random blades		
		s = baseScale + genrand();
		g = (int)(genrand() * 4.5f);
		X = random_blade_dont_mangle_3_returns_mv(s, g, bladeBasisVectorBitmap);
		s = baseScale + genrand();
		Y = random_blade_dont_mangle_3_returns_mv(s, g, bladeBasisVectorBitmap);

		// apply versor to blades
		VX = applyVersor(V, X);
		VY = applyVersor(V, Y);
		
		// compute inner product
		XdY = mhip(X, Y);
		VXdVY = mhip(VX, VY);
		
		// see if inner products are equal (versor application should not change metric)
		dif = subtract(XdY, VXdVY);
		if (dif.largestCoordinate() > (9.9999999999999991E-05f )) {
			printf("applyVersor() test failed (metric test) (largestCoordinate = %e)\n", (double)dif.largestCoordinate() );
			return 0; // failure
		}
		
		// apply inverse transformation to VX
		IVVX = applyVersor(IV, VX);
		
		// see if X equals IVVX
		dif = subtract(X, IVVX);
		if (dif.largestCoordinate() > (9.9999999999999991E-05f )) {
			printf("applyVersor() test failed (inverse test) (largestCoordinate = %e)\n", (double)dif.largestCoordinate() );
			return 0; // failure
		}
	}
	return 1; // success
}

int test_applyUnitVersor_dont_mangle_86(int NB_TESTS_SCALER) {
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	float baseScale = 0.5f;
	int g, i;
	float s;
	mv V, IV, X, Y, VX, VY, IVVX, XdY, VXdVY, dif;
	mv tmp;
	int versorBasisVectorBitmap = 15; // note: random versors restricted to Euclidean basis vectors.
	int bladeBasisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor and its inverse. Optionally make versor unit.
		s = baseScale + genrand();
		g = (int)(genrand() * 4.5f);
		V = random_versor_dont_mangle_1_returns_mv(s, g, versorBasisVectorBitmap);
		
		// avoid 'near'-singular versors
		if (V.largestCoordinate() > 2.0f)
			continue;		
		
		tmp = unit(V);
		V = tmp;
		IV = versorInverse(V);

		// get two random blades		
		s = baseScale + genrand();
		g = (int)(genrand() * 4.5f);
		X = random_blade_dont_mangle_3_returns_mv(s, g, bladeBasisVectorBitmap);
		s = baseScale + genrand();
		Y = random_blade_dont_mangle_3_returns_mv(s, g, bladeBasisVectorBitmap);

		// apply versor to blades
		VX = applyUnitVersor(V, X);
		VY = applyUnitVersor(V, Y);
		
		// compute inner product
		XdY = mhip(X, Y);
		VXdVY = mhip(VX, VY);
		
		// see if inner products are equal (versor application should not change metric)
		dif = subtract(XdY, VXdVY);
		if (dif.largestCoordinate() > (9.9999999999999991E-05f )) {
			printf("applyUnitVersor() test failed (metric test) (largestCoordinate = %e)\n", (double)dif.largestCoordinate() );
			return 0; // failure
		}
		
		// apply inverse transformation to VX
		IVVX = applyUnitVersor(IV, VX);
		
		// see if X equals IVVX
		dif = subtract(X, IVVX);
		if (dif.largestCoordinate() > (9.9999999999999991E-05f )) {
			printf("applyUnitVersor() test failed (inverse test) (largestCoordinate = %e)\n", (double)dif.largestCoordinate() );
			return 0; // failure
		}
	}
	return 1; // success
}

int test_applyVersorWI_dont_mangle_87(int NB_TESTS_SCALER) {
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	float baseScale = 0.5f;
	int g, i;
	float s;
	mv V, IV, X, Y, VX, VY, IVVX, XdY, VXdVY, dif;
	mv tmp;
	int versorBasisVectorBitmap = 15; // note: random versors restricted to Euclidean basis vectors.
	int bladeBasisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor and its inverse. Optionally make versor unit.
		s = baseScale + genrand();
		g = (int)(genrand() * 4.5f);
		V = random_versor_dont_mangle_1_returns_mv(s, g, versorBasisVectorBitmap);
		
		// avoid 'near'-singular versors
		if (V.largestCoordinate() > 2.0f)
			continue;		
		
		tmp = unit(V);
		V = tmp;
		IV = versorInverse(V);

		// get two random blades		
		s = baseScale + genrand();
		g = (int)(genrand() * 4.5f);
		X = random_blade_dont_mangle_3_returns_mv(s, g, bladeBasisVectorBitmap);
		s = baseScale + genrand();
		Y = random_blade_dont_mangle_3_returns_mv(s, g, bladeBasisVectorBitmap);

		// apply versor to blades
		VX = applyVersorWI(V, X, IV);
		VY = applyVersorWI(V, Y, IV);
		
		// compute inner product
		XdY = mhip(X, Y);
		VXdVY = mhip(VX, VY);
		
		// see if inner products are equal (versor application should not change metric)
		dif = subtract(XdY, VXdVY);
		if (dif.largestCoordinate() > (9.9999999999999991E-05f )) {
			printf("applyVersorWI() test failed (metric test) (largestCoordinate = %e)\n", (double)dif.largestCoordinate() );
			return 0; // failure
		}
		
		// apply inverse transformation to VX
		IVVX = applyVersorWI(IV, VX, V);
		
		// see if X equals IVVX
		dif = subtract(X, IVVX);
		if (dif.largestCoordinate() > (9.9999999999999991E-05f )) {
			printf("applyVersorWI() test failed (inverse test) (largestCoordinate = %e)\n", (double)dif.largestCoordinate() );
			return 0; // failure
		}
	}
	return 1; // success
}

int test_div_dont_mangle_50(int NB_TESTS_SCALER) {
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	int i;
	mv A, B, C, dif;
	float divider;
	
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		A = random_blade_dont_mangle_3_returns_mv(genrand(), (int)(genrand() * 4.5f), basisVectorBitmap);
		
		divider = 0.01f + genrand();
		
		B = div(A, divider);
		
		C = gp(B, divider);
		
		// see if C equals A
		dif = subtract(C, A);
		if (dif.largestCoordinate() > (1E-06f )) {
			printf("div() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}

	return 1; // success
}

int test_dual_dont_mangle_48(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, dif;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		A = random_blade_dont_mangle_3_returns_mv(genrand(), (int)(genrand() * 4.5f), basisVectorBitmap);
		
		B = dual(A);
		
		C = undual(B);
		
		// check if equal to original:
		dif = subtract(A, C);
		if (dif.largestCoordinate() > 1E-06f) {
			printf("dual() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_undual_dont_mangle_49(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, dif;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		A = random_blade_dont_mangle_3_returns_mv(genrand(), (int)(genrand() * 4.5f), basisVectorBitmap);
		
		B = undual(A);
		
		C = dual(B);
		
		// check if equal to original:
		dif = subtract(A, C);
		if (dif.largestCoordinate() > 1E-06f) {
			printf("undual() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_equals_dont_mangle_51(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C;
	float s, eps = 0.2f;
	int g, i;
	bool e, l;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		g = (int)(genrand() * 4.5f);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);
		B = random_versor_dont_mangle_1_returns_mv(1.1f * eps, g, basisVectorBitmap);
		C = add(B, A);
		
		// check if equals thinks A if is equal to itself
		e = equals(A, A, eps);
		if (!e) {
			printf("equals() failed (variable not equal to itself)\n");
			return 0; // failure
		}
		
		// check if equals thinks A if is equal C
		e = equals(A, C, eps);
		
		// use mv_largestCoordinate() to verify
		l = !(B.largestCoordinate() > eps);
		
		if (e != l) {
			printf("equals() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade_dont_mangle_56(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, G0, G1, G2, G3, G4;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * 4.5f), basisVectorBitmap);
		// split it up into groups/grades:
		G0 = extractGrade(A, GROUP_0);
		G1 = extractGrade(A, GROUP_1);
		G2 = extractGrade(A, GROUP_2);
		G3 = extractGrade(A, GROUP_3);
		G4 = extractGrade(A, GROUP_4);
		// sum all into 'B'
		B.set();
		B = add(B, G0);
		B = add(B, G1);
		B = add(B, G2);
		B = add(B, G3);
		B = add(B, G4);

		// check if equal to original:
		C = subtract(A, B);
		if (C.largestCoordinate() > 1E-06f) {
			printf("extractGrade() test failed (largestCoordinate = %e)\n", (double)C.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade0_dont_mangle_58(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * 4.5f), basisVectorBitmap);
		
		B = extractGrade0(A);
		
		C = extractGrade(A, 30);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > 1E-06f) {
			printf("extractGrade0() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade1_dont_mangle_61(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * 4.5f), basisVectorBitmap);
		
		B = extractGrade1(A);
		
		C = extractGrade(A, 29);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > 1E-06f) {
			printf("extractGrade1() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade2_dont_mangle_59(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * 4.5f), basisVectorBitmap);
		
		B = extractGrade2(A);
		
		C = extractGrade(A, 27);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > 1E-06f) {
			printf("extractGrade2() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade3_dont_mangle_57(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * 4.5f), basisVectorBitmap);
		
		B = extractGrade3(A);
		
		C = extractGrade(A, 23);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > 1E-06f) {
			printf("extractGrade3() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade4_dont_mangle_63(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * 4.5f), basisVectorBitmap);
		
		B = extractGrade4(A);
		
		C = extractGrade(A, 15);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > 1E-06f) {
			printf("extractGrade4() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_gp_dont_mangle_64(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 64;
	mv A, B, C, D, E, V1, V2;
	int i;
	int o;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		o = (genrand() < 0.5f) ? 0 : 1; // even or odd?
		A = random_versor_dont_mangle_1_returns_mv(genrand(), ((int)(genrand() * 4.5f) & 0xFFFE) + o, basisVectorBitmap);
		B = random_versor_dont_mangle_1_returns_mv(genrand(), ((int)(genrand() * 4.5f) & 0xFFFE) + o, basisVectorBitmap);
		C = random_versor_dont_mangle_1_returns_mv(genrand(), ((int)(genrand() * 4.5f) & 0xFFFE) + o, basisVectorBitmap);
		
		{ // test (A+B) C = A C + B C
			// D = A + B
			D = add(A, B);
			// V1 = D C
			V1 = gp(D, C);
			// D = A C
			D = gp(A, C);
			// E = B C
			E = gp(B, C);
			// V2 = D + E
			V2 = add(D, E);
			// test equality
			D = subtract(V1, V2);
			// use mv_largestCoordinate() to verify
			if (D.largestCoordinate() > 0.001f) {
				printf("gp() test failed on '(A+B) C = A C + B C' (dif=%e)\n", (double)D.largestCoordinate());
				return 0; // failure
			}
		}
		
		{ // test A (B+C) = A B + A C
			// D = B + C
			D = add(B, C);
			// V1 = A D
			V1 = gp(A, D);
			// D = A B
			D = gp(A, B);
			// E = A C
			E = gp(A, C);
			// V2 = D + E
			V2 = add(D, E);
			// test equality
			D = subtract(V1, V2);
			// use largestCoordinate() to verify
			if (D.largestCoordinate() > 9.9999999999999991E-05f) {
				printf("gp() test failed on 'A (B+C) = A B + A C' (dif=%e)\n", (double)D.largestCoordinate());
				return 0; // failure
			}
		}
		
		{ // test A (B C) = (A B) C
			// D = B C
			D = gp(B, C);
			// V1 = A D
			V1 = gp(A, D);
			// D = A B
			D = gp(A, B);
			// V2 = D C
			V2 = gp(D, C);
			// test equality
			D = subtract(V1, V2);
			// use largestCoordinate() to verify
			if (D.largestCoordinate() > 9.9999999999999991E-05f) {
				printf("gp() test failed on 'A (B C) = (A B) C' (dif=%e)\n", (double)D.largestCoordinate());
				return 0; // failure
			}
		}		
	}
	return 1; // success
}

int test_gradeBitmap_dont_mangle_47(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, tmp, randomBlade;
	int i, j;
	int basisVectorBitmap = -1;
	int gradeBitmap1, gradeBitmap2, nbBlades, grade;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get sum of random blades, keep track of grades used
		gradeBitmap1 = 0;
		A.set();
		nbBlades = (int)(genrand() * 4.5f);
		for (j = 0; j < nbBlades; j++) {
			grade = (int)(genrand() * 4.5f);
			gradeBitmap1 |= 1 << grade;
			randomBlade = random_blade_dont_mangle_3_returns_mv(1.0f, grade, basisVectorBitmap);
			tmp = add(A, randomBlade);
			A = tmp;
		}
		
		gradeBitmap2 = gradeBitmap(A, 0.0f);
		
		// check if grade bitmaps match
		if (gradeBitmap1 != gradeBitmap2) {
			printf("gradeBitmap() test failed (grade bitmap %d vs %d)\n", gradeBitmap1, gradeBitmap2);
			return 0; // failure
		}
	}
	return 1; // success
}

int test_hp_dont_mangle_66(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D, dif;
	int i, j, g;
	int basisVectorBitmap = -1;
	float s;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		s = genrand();
		g = (int)(genrand() * 4.5f);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);
		
		// copy it to another versor
		B = A;
		
		// set coordinates of B to random values (which may not be zero)
		for (j = 0; j < e4ga_mvSize[B.gu()]; j++) {
			B.m_c[j] = 0.5f + genrand();
		}
		
		// do hadamard product
		C = hp(A, B);
		
		// invert coordinates of B manually
		for (j = 0; j < e4ga_mvSize[B.gu()]; j++) {
			B.m_c[j] = 1.0f / B.m_c[j];
		}

		// do inverse hadamard product
		D = hp(C, B);
		
		// check if equal to original:
		dif = subtract(A, D);
		if (dif.largestCoordinate() > 9.9999999999999991E-05f) {
			printf("hp() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_ihp_dont_mangle_67(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D, dif;
	int i, j, g;
	int basisVectorBitmap = -1;
	float s;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		s = genrand();
		g = (int)(genrand() * 4.5f);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);
		
		// copy it to another versor
		B = A;
		
		// set coordinates of B to random values (which may not be zero)
		for (j = 0; j < e4ga_mvSize[B.gu()]; j++) {
			B.m_c[j] = 0.5f + genrand();
		}
		
		// do hadamard product
		C = ihp(A, B);
		
		// invert coordinates of B manually
		for (j = 0; j < e4ga_mvSize[B.gu()]; j++) {
			B.m_c[j] = 1.0f / B.m_c[j];
		}

		// do inverse hadamard product
		D = ihp(C, B);
		
		// check if equal to original:
		dif = subtract(A, D);
		if (dif.largestCoordinate() > 9.9999999999999991E-05f) {
			printf("ihp() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_increment_dont_mangle_60(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D, one;
	int i, g;
	int basisVectorBitmap = -1;

	one = 1.0f;

	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		g = (int)(genrand() * 4.5f);
		A = random_versor_dont_mangle_1_returns_mv(genrand() + 0.5f, g, basisVectorBitmap);
		
		// increment/decrement
		B = increment(A);
		
		// undo the increment/decrement
		C = subtract(B, one);
		
		// see if (A - (B-1)) == 0
		D = subtract(A, C);
		
		if (D.largestCoordinate() > 1E-06f) {
			printf("increment() test failed (largestCoordinate of D = %e)\n", (double)D.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_decrement_dont_mangle_62(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D, one;
	int i, g;
	int basisVectorBitmap = -1;

	one = -1.0f;

	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		g = (int)(genrand() * 4.5f);
		A = random_versor_dont_mangle_1_returns_mv(genrand() + 0.5f, g, basisVectorBitmap);
		
		// increment/decrement
		B = decrement(A);
		
		// undo the increment/decrement
		C = subtract(B, one);
		
		// see if (A - (B-1)) == 0
		D = subtract(A, C);
		
		if (D.largestCoordinate() > 1E-06f) {
			printf("decrement() test failed (largestCoordinate of D = %e)\n", (double)D.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_sp_dont_mangle_76(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D, E, dif;
	int i, ga, gb, gd;
	float s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		ga = (int)(genrand() * 4.5f);
		A = random_blade_dont_mangle_3_returns_mv(s, ga, basisVectorBitmap);
		
		s = genrand();
		gb = (int)(genrand() * 4.5f);
		B = random_blade_dont_mangle_3_returns_mv(s, gb, basisVectorBitmap);
		
		// compute product using sp()
		C = sp(A, B);
		
		// simulate product using geometric product & grade part selection
		D = gp(A, B);
		gd = (ga > gb) ? ga - gb : gb - ga;
		E = extractGrade(D, e4ga_grades[0]);

		// check if equal:
		dif = subtract(C, E);
		if (dif.largestCoordinate() > 9.9999999999999991E-05f) {
			printf("sp() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_lc_dont_mangle_79(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D, E, dif;
	int i, ga, gb, gd;
	float s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		ga = (int)(genrand() * 4.5f);
		A = random_blade_dont_mangle_3_returns_mv(s, ga, basisVectorBitmap);
		
		s = genrand();
		gb = (int)(genrand() * 4.5f);
		B = random_blade_dont_mangle_3_returns_mv(s, gb, basisVectorBitmap);
		
		// compute product using lc()
		C = lc(A, B);
		
		// simulate product using geometric product & grade part selection
		D = gp(A, B);
		gd = (ga > gb) ? ga - gb : gb - ga;
		if (ga > gb) E = 0.0f;
		else E = extractGrade(D, e4ga_grades[gd]);

		// check if equal:
		dif = subtract(C, E);
		if (dif.largestCoordinate() > 9.9999999999999991E-05f) {
			printf("lc() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_rc_dont_mangle_81(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D, E, dif;
	int i, ga, gb, gd;
	float s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		ga = (int)(genrand() * 4.5f);
		A = random_blade_dont_mangle_3_returns_mv(s, ga, basisVectorBitmap);
		
		s = genrand();
		gb = (int)(genrand() * 4.5f);
		B = random_blade_dont_mangle_3_returns_mv(s, gb, basisVectorBitmap);
		
		// compute product using rc()
		C = rc(A, B);
		
		// simulate product using geometric product & grade part selection
		D = gp(A, B);
		gd = (ga > gb) ? ga - gb : gb - ga;
		if (ga < gb) E = 0.0f;
		else E = extractGrade(D, e4ga_grades[gd]);

		// check if equal:
		dif = subtract(C, E);
		if (dif.largestCoordinate() > 9.9999999999999991E-05f) {
			printf("rc() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_hip_dont_mangle_83(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D, E, dif;
	int i, ga, gb, gd;
	float s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		ga = (int)(genrand() * 4.5f);
		A = random_blade_dont_mangle_3_returns_mv(s, ga, basisVectorBitmap);
		
		s = genrand();
		gb = (int)(genrand() * 4.5f);
		B = random_blade_dont_mangle_3_returns_mv(s, gb, basisVectorBitmap);
		
		// compute product using hip()
		C = hip(A, B);
		
		// simulate product using geometric product & grade part selection
		D = gp(A, B);
		gd = (ga > gb) ? ga - gb : gb - ga;
		if ((ga == 0) || (gb == 0)) E = 0.0f;
		else E = extractGrade(D, e4ga_grades[gd]);

		// check if equal:
		dif = subtract(C, E);
		if (dif.largestCoordinate() > 9.9999999999999991E-05f) {
			printf("hip() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_mhip_dont_mangle_78(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D, E, dif;
	int i, ga, gb, gd;
	float s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		ga = (int)(genrand() * 4.5f);
		A = random_blade_dont_mangle_3_returns_mv(s, ga, basisVectorBitmap);
		
		s = genrand();
		gb = (int)(genrand() * 4.5f);
		B = random_blade_dont_mangle_3_returns_mv(s, gb, basisVectorBitmap);
		
		// compute product using mhip()
		C = mhip(A, B);
		
		// simulate product using geometric product & grade part selection
		D = gp(A, B);
		gd = (ga > gb) ? ga - gb : gb - ga;
		E = extractGrade(D, e4ga_grades[gd]);

		// check if equal:
		dif = subtract(C, E);
		if (dif.largestCoordinate() > 9.9999999999999991E-05f) {
			printf("mhip() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_norm_dont_mangle_73(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, reverseA, tmp;
	
	int i, g;
	int basisVectorBitmap = -1;
	float s;
	float n1, n2;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		s = genrand();
		g = (int)(genrand() * 4.5f);
		A = random_blade_dont_mangle_3_returns_mv(s, g, basisVectorBitmap);
		
		// compute norm
		n1 = norm(A);
		
		// compute norm manually (A . reverse(A))
		reverseA = reverse(A);
		tmp = gp(A, reverseA);
		n2 = tmp.get_scalar();
		n2 = (float)sqrt(fabs(n2));
		
		// check if equal to original:
		if (fabs(n1 - n2) > 1E-06f) {
			printf("norm() test failed (difference = %e)\n", (double)fabs(n1 - n2));
			return 0; // failure
		}
	}
	return 1; // success
}

int test_norm2_dont_mangle_74(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, reverseA, tmp;
	
	int i, g;
	int basisVectorBitmap = -1;
	float s;
	float n1, n2;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		s = genrand();
		g = (int)(genrand() * 4.5f);
		A = random_blade_dont_mangle_3_returns_mv(s, g, basisVectorBitmap);
		
		// compute norm
		n1 = norm2(A);
		
		// compute norm manually (A . reverse(A))
		reverseA = reverse(A);
		tmp = gp(A, reverseA);
		n2 = tmp.get_scalar();
		
		// check if equal to original:
		if (fabs(n1 - n2) > 1E-06f) {
			printf("norm2() test failed (difference = %e)\n", (double)fabs(n1 - n2));
			return 0; // failure
		}
	}
	return 1; // success
}

int test_op_dont_mangle_84_1(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, C, D, E, dif;
	int i, ga, gb, gd;
	float s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		ga = (int)(genrand() * 4.5f);
		A = random_blade_dont_mangle_3_returns_mv(s, ga, basisVectorBitmap);
		
		s = genrand();
		gb = (int)(genrand() * 4.5f);
		B = random_blade_dont_mangle_3_returns_mv(s, gb, basisVectorBitmap);
		
		// compute product using op()
		C = op(A, B);
		
		// simulate product using geometric product & grade part selection
		D = gp(A, B);
		gd = (ga > gb) ? ga - gb : gb - ga;
		E = extractGrade(D,  e4ga_grades[ga + gb]);

		// check if equal:
		dif = subtract(C, E);
		if (dif.largestCoordinate() > 9.9999999999999991E-05f) {
			printf("op() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_op_dont_mangle_84_2(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B;
	int i, ga;
	float s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		ga = (int)(genrand() * 4.5f);
		if (ga == 0) continue; // do not perform this test for grade 0
		A = random_blade_dont_mangle_3_returns_mv(s, ga, basisVectorBitmap);
		
		// compute A ^ A (should be zero)
		B = op(A, A);
		
		// check if B is zero:
		if (B.largestCoordinate() > 9.9999999999999991E-06f) {
			printf("op() test failed (largestCoordinate = %e)\n", (double)B.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_exp_dont_mangle_68(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, expA, sinhA, coshA, S, dif, tmp1;//, tmp2;
	int i, g;
	int basisVectorBitmap = -1;
	float s;
	int order = 12;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade of grade 2
		s = 2.0f * genrand();
		g = 2;
		A = random_blade_dont_mangle_3_returns_mv(s, g, basisVectorBitmap);
		
		if (genrand() > 0.5f) { // make sure that 'A' is not always a blade
			s = genrand();
			tmp1 = random_blade_dont_mangle_3_returns_mv(s, g, basisVectorBitmap);	
			A = add(A, tmp1);
			//A = tmp2;
		}

		// apply sinh, cosh, exp functions
		expA = exp(A, order);
		sinhA = sinh(A, order);
		coshA = cosh(A, order);
		
		// sum sinh and cosh
		S = add(coshA, sinhA);
		
		// test that sinh+cosh == exp
		dif = subtract(expA, S);
		if (dif.largestCoordinate() > 0.0316227766016838f) { // sinh, cosh precision is very low
			printf("exp() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_cosh_dont_mangle_69(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, expA, sinhA, coshA, S, dif, tmp1;//, tmp2;
	int i, g;
	int basisVectorBitmap = -1;
	float s;
	int order = 12;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade of grade 2
		s = 2.0f * genrand();
		g = 2;
		A = random_blade_dont_mangle_3_returns_mv(s, g, basisVectorBitmap);
		
		if (genrand() > 0.5f) { // make sure that 'A' is not always a blade
			s = genrand();
			tmp1 = random_blade_dont_mangle_3_returns_mv(s, g, basisVectorBitmap);	
			A = add(A, tmp1);
			//A = tmp2;
		}

		// apply sinh, cosh, exp functions
		expA = exp(A, order);
		sinhA = sinh(A, order);
		coshA = cosh(A, order);
		
		// sum sinh and cosh
		S = add(coshA, sinhA);
		
		// test that sinh+cosh == exp
		dif = subtract(expA, S);
		if (dif.largestCoordinate() > 0.0316227766016838f) { // sinh, cosh precision is very low
			printf("cosh() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_sinh_dont_mangle_72(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, expA, sinhA, coshA, S, dif, tmp1;//, tmp2;
	int i, g;
	int basisVectorBitmap = -1;
	float s;
	int order = 12;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade of grade 2
		s = 2.0f * genrand();
		g = 2;
		A = random_blade_dont_mangle_3_returns_mv(s, g, basisVectorBitmap);
		
		if (genrand() > 0.5f) { // make sure that 'A' is not always a blade
			s = genrand();
			tmp1 = random_blade_dont_mangle_3_returns_mv(s, g, basisVectorBitmap);	
			A = add(A, tmp1);
			//A = tmp2;
		}

		// apply sinh, cosh, exp functions
		expA = exp(A, order);
		sinhA = sinh(A, order);
		coshA = cosh(A, order);
		
		// sum sinh and cosh
		S = add(coshA, sinhA);
		
		// test that sinh+cosh == exp
		dif = subtract(expA, S);
		if (dif.largestCoordinate() > 0.0316227766016838f) { // sinh, cosh precision is very low
			printf("sinh() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_negate_dont_mangle_75(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, GA, GB;
	int i, c, n, g;
	int basisVectorBitmap = -1;
	float s, dif;
	int signTable[5] = {-1, -1, -1, -1, -1};
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		s = genrand();
		g = (int)(genrand() * 4.5f);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);

		// apply function
		B = negate(A);
		
		// check grade
		for (n = 0; n <= 4; n++) {
			GA = extractGrade(A, e4ga_grades[n]);
			GB = extractGrade(B, e4ga_grades[n]);
			
			// check if grade usage matches
			if (GA.gu() != GB.gu()) {
				printf("negate() test failed (grade usage does not match)\n");
				return 0; // failure
			}
			
			// check each coordinate 
			for (c = 0; c < e4ga_mvSize[GA.gu()]; c++) {
				dif = (float)fabs(GA.m_c[c] * (float)signTable[n] - GB.m_c[c]);
				if (dif > 1E-06f) {
					printf("negate() test failed (dif = %e)\n", (double)dif);
					return 0; // failure
				}
			}
		}
		
	}
	return 1; // success
}

int test_cliffordConjugate_dont_mangle_77(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, GA, GB;
	int i, c, n, g;
	int basisVectorBitmap = -1;
	float s, dif;
	int signTable[5] = {1, -1, -1, 1, 1};
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		s = genrand();
		g = (int)(genrand() * 4.5f);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);

		// apply function
		B = cliffordConjugate(A);
		
		// check grade
		for (n = 0; n <= 4; n++) {
			GA = extractGrade(A, e4ga_grades[n]);
			GB = extractGrade(B, e4ga_grades[n]);
			
			// check if grade usage matches
			if (GA.gu() != GB.gu()) {
				printf("cliffordConjugate() test failed (grade usage does not match)\n");
				return 0; // failure
			}
			
			// check each coordinate 
			for (c = 0; c < e4ga_mvSize[GA.gu()]; c++) {
				dif = (float)fabs(GA.m_c[c] * (float)signTable[n] - GB.m_c[c]);
				if (dif > 1E-06f) {
					printf("cliffordConjugate() test failed (dif = %e)\n", (double)dif);
					return 0; // failure
				}
			}
		}
		
	}
	return 1; // success
}

int test_gradeInvolution_dont_mangle_82(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, GA, GB;
	int i, c, n, g;
	int basisVectorBitmap = -1;
	float s, dif;
	int signTable[5] = {1, -1, 1, -1, 1};
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		s = genrand();
		g = (int)(genrand() * 4.5f);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);

		// apply function
		B = gradeInvolution(A);
		
		// check grade
		for (n = 0; n <= 4; n++) {
			GA = extractGrade(A, e4ga_grades[n]);
			GB = extractGrade(B, e4ga_grades[n]);
			
			// check if grade usage matches
			if (GA.gu() != GB.gu()) {
				printf("gradeInvolution() test failed (grade usage does not match)\n");
				return 0; // failure
			}
			
			// check each coordinate 
			for (c = 0; c < e4ga_mvSize[GA.gu()]; c++) {
				dif = (float)fabs(GA.m_c[c] * (float)signTable[n] - GB.m_c[c]);
				if (dif > 1E-06f) {
					printf("gradeInvolution() test failed (dif = %e)\n", (double)dif);
					return 0; // failure
				}
			}
		}
		
	}
	return 1; // success
}

int test_reverse_dont_mangle_80(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, B, GA, GB;
	int i, c, n, g;
	int basisVectorBitmap = -1;
	float s, dif;
	int signTable[5] = {1, 1, -1, -1, 1};
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		s = genrand();
		g = (int)(genrand() * 4.5f);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);

		// apply function
		B = reverse(A);
		
		// check grade
		for (n = 0; n <= 4; n++) {
			GA = extractGrade(A, e4ga_grades[n]);
			GB = extractGrade(B, e4ga_grades[n]);
			
			// check if grade usage matches
			if (GA.gu() != GB.gu()) {
				printf("reverse() test failed (grade usage does not match)\n");
				return 0; // failure
			}
			
			// check each coordinate 
			for (c = 0; c < e4ga_mvSize[GA.gu()]; c++) {
				dif = (float)fabs(GA.m_c[c] * (float)signTable[n] - GB.m_c[c]);
				if (dif > 1E-06f) {
					printf("reverse() test failed (dif = %e)\n", (double)dif);
					return 0; // failure
				}
			}
		}
		
	}
	return 1; // success
}

int test_unit_dont_mangle_54(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A, UA, RUA;
	int i;
	int basisVectorBitmap = -1;
	float n;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		A = random_blade_dont_mangle_3_returns_mv(genrand(), (int)(genrand() * 4.5f), basisVectorBitmap);
		
		UA = unit(A);
		RUA = reverse(UA);
		n = sp(RUA, UA);
		
		if ((float)(fabs(n) - 1.0f) > 9.9999999999999991E-05f) {
			printf("unit() test failed (|norm|-1 = %e)\n", (double)(fabs((double)n) - 1.0));
			return 0; // failure
		}

	}
	return 1; // success
}

int test_versorInverse_dont_mangle_55(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv V, VI, VVI, VIV, X, Y;
	int i;
	int basisVectorBitmap = -1;
	float n1, n2;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		V = random_versor_dont_mangle_1_returns_mv(genrand() + 0.5f, (int)(genrand() * 4.5f), basisVectorBitmap);
		
		// avoid 'near'-singular versors
		if (V.largestCoordinate() > 2.0f)
			continue;
		
		// compute inverse
		VI = versorInverse(V);
		
		// compute (inverse(V) V) and (V inverse(V))
		VIV = gp(VI, V);
		VVI = gp(V, VI);
		
		// check that scalar parts are 1
		n1 = VIV.get_scalar();
		n2 = VVI.get_scalar();
		
		if (fabs(n1 - 1.0f) > 0.001f) {
			printf("versorInverse() test failed |VI . V - 1|= %e)\n", (double)fabs(n1 - 1.0f));
			return 0; // failure
		}
		
		if (fabs(n2 - 1.0f) > 0.001f) {
			printf("versorInverse() test failed ( |V . VI - 1| = %e)\n", (double)fabs(n2 - 1.0f));
			return 0; // failure
		}
		
		// check that other grade parts are zero:
		X = extractGrade(VIV, 30);
		Y = extractGrade(VVI, 30);
		
		if (X.largestCoordinate() > 0.001f) {
			printf("versorInverse() test failed (largestCoordinate of VIV = %e)\n", (double)X.largestCoordinate());
			return 0; // failure
		}
		
		if (Y.largestCoordinate() > 0.001f) {
			printf("versorInverse() test failed (largestCoordinate of VVI = %e)\n", (double)Y.largestCoordinate());
			return 0; // failure
		}
		
	}
	return 1; // success
}

int test_zero_dont_mangle_46(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 16;
	mv A;
	float s, eps = 0.2f;
	int g, i, j;
	bool z, l, c;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		g = (int)(genrand() * 4.5f);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);
		
		// ask if zero thinks A is zero
		z = zero(A, eps);
		
		// check it yourself
		c = true; // assume it is zero
		for (j = 0; j < e4ga_mvSize[A.gu()]; j++) {
			if (fabs(A.m_c[j]) > eps) c = false;
		}
		
		// also use mv_largestCoordinate() to verify
		l = !(A.largestCoordinate() > eps);
		
		if ((c != z) || (l != z)) {
			printf("zero() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}
// Testing code definitions:

int test_metric_default_mv(int NB_TESTS_SCALER) 
{
	int i, j;
	float arr[4], dif;
	mv A, bv[4];
	float M[16+1] = {
		1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f	}; // metric matrix

	// get all basis vectors

	e4ga::zero_4(arr);
	arr[0] = 1.0f;
	bv[0].set(2, arr);

	e4ga::zero_4(arr);
	arr[1] = 1.0f;
	bv[1].set(2, arr);

	e4ga::zero_4(arr);
	arr[2] = 1.0f;
	bv[2].set(2, arr);

	e4ga::zero_4(arr);
	arr[3] = 1.0f;
	bv[3].set(2, arr);

	for (i = 0; i < 4; i++) {
		for (j = 0; j < 4; j++) {
			A = gp(bv[i], bv[j]);
			dif = M[i * 4 + j] - A.get_scalar();
			if ((dif < -1E-06f) || (dif > 1E-06f)) {
				printf("test_metric_default_mv() test failed for %s %s\n", e4ga_basisVectorNames[i], e4ga_basisVectorNames[j]);
				return 0;
			}
		}
	}
	
	return 1;
}

int test_parse_mv(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 64;
	mv A, B, C;
	std::string str;
	
	int i, basisVectorBitmap = -1;

	for (i = 0; i < NB_LOOPS; i++) {
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * 4.5f), basisVectorBitmap);
		
		str = A.toString("%2.20e");
		
		try {
			B = parse(str);
		} catch (const std::string &exStr) {
			printf("parse() test failed (%s)\n", exStr.c_str());
			return 0; // failure
		}
		
		C = subtract(A, B);

		if (C.largestCoordinate() > 1E-06f) {
			printf("parse() test failed (%s)\n", str.c_str());
			return 0; // failure
		}
	}
	
	return 1; // success
}
} // end of namespace e4ga

int main(int argc, char *argv[]) {
	int retVal = 0;
	// The number of tests will be proportional to this value.
	// This should become a command-line option
	const int NB_TESTS_SCALER = 16384;
	
	// seed random number generators with current time
	e4ga::genrand_timeSeed();

	// run all test functions
	if (!e4ga::test_metric_default_mv(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_parse_mv(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_genrand_float(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_add_dont_mangle_52(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_subtract_dont_mangle_53(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_applyVersor_dont_mangle_85(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_applyUnitVersor_dont_mangle_86(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_applyVersorWI_dont_mangle_87(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_div_dont_mangle_50(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_dual_dont_mangle_48(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_undual_dont_mangle_49(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_equals_dont_mangle_51(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_extractGrade_dont_mangle_56(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_extractGrade0_dont_mangle_58(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_extractGrade1_dont_mangle_61(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_extractGrade2_dont_mangle_59(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_extractGrade3_dont_mangle_57(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_extractGrade4_dont_mangle_63(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_gp_dont_mangle_64(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_gradeBitmap_dont_mangle_47(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_hp_dont_mangle_66(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_ihp_dont_mangle_67(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_increment_dont_mangle_60(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_decrement_dont_mangle_62(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_sp_dont_mangle_76(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_lc_dont_mangle_79(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_rc_dont_mangle_81(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_hip_dont_mangle_83(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_mhip_dont_mangle_78(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_norm_dont_mangle_73(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_norm2_dont_mangle_74(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_op_dont_mangle_84_1(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_op_dont_mangle_84_2(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_exp_dont_mangle_68(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_cosh_dont_mangle_69(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_sinh_dont_mangle_72(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_negate_dont_mangle_75(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_cliffordConjugate_dont_mangle_77(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_gradeInvolution_dont_mangle_82(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_reverse_dont_mangle_80(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_unit_dont_mangle_54(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_versorInverse_dont_mangle_55(NB_TESTS_SCALER)) retVal = -1;
	if (!e4ga::test_zero_dont_mangle_46(NB_TESTS_SCALER)) retVal = -1;

	if (retVal != 0) printf("Test failed.\n");
	else printf("Done.\n");	

	return retVal;
}
