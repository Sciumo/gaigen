#include <time.h> /* used to seed random generator */
#include "e11ga.h"
namespace e11ga {
// Missing dependencies declarations:

/**
Generates a random versor.
The scale is uniformly distributed over the interval [0 1).
The maximum non-zero grade-part is 'grade'.
Only the basis vectors marked in 'basisVectorBitmap' will be used to generate the
versor/blade. Use 'basisVectorBitmap = -1' (the default) to use all basisvectors.

Returns random_versor_dont_mangle_1_returns_mv_ex(arg1, scale, grade, basisVectorBitmap, 0.01, scale * 4.0);
*/
mv random_versor_dont_mangle_1_returns_mv(myDouble scale, int grade, int basisVectorBitmap = -1);

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
mv random_versor_dont_mangle_1_returns_mv_ex(myDouble scale, int grade, int basisVectorBitmap, myDouble minimumNorm, myDouble largestCoordinate);


/**
Generates a random blade.
The scale is uniformly distributed over the interval [0 1).
The maximum non-zero grade-part is 'grade'.
Only the basis vectors marked in 'basisVectorBitmap' will be used to generate the
versor/blade. Use 'basisVectorBitmap = -1' (the default) to use all basisvectors.

Returns random_blade_dont_mangle_3_returns_mv_ex(arg1, scale, grade, basisVectorBitmap, 0.01, scale * 4.0);
*/
mv random_blade_dont_mangle_3_returns_mv(myDouble scale, int grade, int basisVectorBitmap = -1);

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
mv random_blade_dont_mangle_3_returns_mv_ex(myDouble scale, int grade, int basisVectorBitmap, myDouble minimumNorm, myDouble largestCoordinate);

// Missing dependencies inline definitions:
// Missing dependencies definitions:
/// Computes the partial geometric product of two multivectors (group 0  x  group 0 -> group 0)
void gp_default_0_0_0(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 1 -> group 1)
void gp_default_0_1_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 2 -> group 2)
void gp_default_0_2_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 3 -> group 3)
void gp_default_0_3_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 4 -> group 4)
void gp_default_0_4_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 5 -> group 5)
void gp_default_0_5_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 6 -> group 6)
void gp_default_0_6_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 7 -> group 7)
void gp_default_0_7_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 8 -> group 8)
void gp_default_0_8_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 9 -> group 9)
void gp_default_0_9_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 10 -> group 10)
void gp_default_0_10_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 11 -> group 11)
void gp_default_0_11_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 0 -> group 1)
void gp_default_1_0_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 0)
void gp_default_1_1_0(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 2)
void gp_default_1_1_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 1)
void gp_default_1_2_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 3)
void gp_default_1_2_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 3 -> group 2)
void gp_default_1_3_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 3 -> group 4)
void gp_default_1_3_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 4 -> group 3)
void gp_default_1_4_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 4 -> group 5)
void gp_default_1_4_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 5 -> group 4)
void gp_default_1_5_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 5 -> group 6)
void gp_default_1_5_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 6 -> group 5)
void gp_default_1_6_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 6 -> group 7)
void gp_default_1_6_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 7 -> group 6)
void gp_default_1_7_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 7 -> group 8)
void gp_default_1_7_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 8 -> group 7)
void gp_default_1_8_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 8 -> group 9)
void gp_default_1_8_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 9 -> group 8)
void gp_default_1_9_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 9 -> group 10)
void gp_default_1_9_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 10 -> group 9)
void gp_default_1_10_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 10 -> group 11)
void gp_default_1_10_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 11 -> group 10)
void gp_default_1_11_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 0 -> group 2)
void gp_default_2_0_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 1)
void gp_default_2_1_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 3)
void gp_default_2_1_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 0)
void gp_default_2_2_0(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 2)
void gp_default_2_2_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 4)
void gp_default_2_2_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 1)
void gp_default_2_3_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 3)
void gp_default_2_3_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 5)
void gp_default_2_3_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 2)
void gp_default_2_4_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 4)
void gp_default_2_4_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 6)
void gp_default_2_4_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 5 -> group 3)
void gp_default_2_5_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 5 -> group 5)
void gp_default_2_5_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 5 -> group 7)
void gp_default_2_5_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 6 -> group 4)
void gp_default_2_6_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 6 -> group 6)
void gp_default_2_6_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 6 -> group 8)
void gp_default_2_6_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 7 -> group 5)
void gp_default_2_7_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 7 -> group 7)
void gp_default_2_7_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 7 -> group 9)
void gp_default_2_7_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 8 -> group 6)
void gp_default_2_8_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 8 -> group 8)
void gp_default_2_8_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 8 -> group 10)
void gp_default_2_8_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 9 -> group 7)
void gp_default_2_9_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 9 -> group 9)
void gp_default_2_9_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 9 -> group 11)
void gp_default_2_9_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 10 -> group 8)
void gp_default_2_10_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 10 -> group 10)
void gp_default_2_10_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 11 -> group 9)
void gp_default_2_11_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 11 -> group 11)
void gp_default_2_11_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 0 -> group 3)
void gp_default_3_0_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 1 -> group 2)
void gp_default_3_1_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 1 -> group 4)
void gp_default_3_1_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 1)
void gp_default_3_2_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 3)
void gp_default_3_2_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 5)
void gp_default_3_2_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 0)
void gp_default_3_3_0(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 2)
void gp_default_3_3_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 4)
void gp_default_3_3_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 6)
void gp_default_3_3_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 1)
void gp_default_3_4_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 3)
void gp_default_3_4_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 5)
void gp_default_3_4_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 7)
void gp_default_3_4_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 5 -> group 2)
void gp_default_3_5_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 5 -> group 4)
void gp_default_3_5_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 5 -> group 6)
void gp_default_3_5_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 5 -> group 8)
void gp_default_3_5_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 6 -> group 3)
void gp_default_3_6_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 6 -> group 5)
void gp_default_3_6_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 6 -> group 7)
void gp_default_3_6_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 6 -> group 9)
void gp_default_3_6_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 7 -> group 4)
void gp_default_3_7_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 7 -> group 6)
void gp_default_3_7_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 7 -> group 8)
void gp_default_3_7_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 7 -> group 10)
void gp_default_3_7_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 8 -> group 5)
void gp_default_3_8_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 8 -> group 7)
void gp_default_3_8_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 8 -> group 9)
void gp_default_3_8_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 8 -> group 11)
void gp_default_3_8_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 9 -> group 6)
void gp_default_3_9_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 9 -> group 8)
void gp_default_3_9_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 9 -> group 10)
void gp_default_3_9_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 10 -> group 7)
void gp_default_3_10_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 10 -> group 9)
void gp_default_3_10_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 10 -> group 11)
void gp_default_3_10_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 11 -> group 8)
void gp_default_3_11_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 11 -> group 10)
void gp_default_3_11_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 0 -> group 4)
void gp_default_4_0_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 3)
void gp_default_4_1_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 5)
void gp_default_4_1_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 2)
void gp_default_4_2_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 4)
void gp_default_4_2_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 6)
void gp_default_4_2_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 1)
void gp_default_4_3_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 3)
void gp_default_4_3_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 5)
void gp_default_4_3_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 7)
void gp_default_4_3_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 0)
void gp_default_4_4_0(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 2)
void gp_default_4_4_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 4)
void gp_default_4_4_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 6)
void gp_default_4_4_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 8)
void gp_default_4_4_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 5 -> group 1)
void gp_default_4_5_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 5 -> group 3)
void gp_default_4_5_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 5 -> group 5)
void gp_default_4_5_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 5 -> group 7)
void gp_default_4_5_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 5 -> group 9)
void gp_default_4_5_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 6 -> group 2)
void gp_default_4_6_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 6 -> group 4)
void gp_default_4_6_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 6 -> group 6)
void gp_default_4_6_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 6 -> group 8)
void gp_default_4_6_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 6 -> group 10)
void gp_default_4_6_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 7 -> group 3)
void gp_default_4_7_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 7 -> group 5)
void gp_default_4_7_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 7 -> group 7)
void gp_default_4_7_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 7 -> group 9)
void gp_default_4_7_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 7 -> group 11)
void gp_default_4_7_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 8 -> group 4)
void gp_default_4_8_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 8 -> group 6)
void gp_default_4_8_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 8 -> group 8)
void gp_default_4_8_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 8 -> group 10)
void gp_default_4_8_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 9 -> group 5)
void gp_default_4_9_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 9 -> group 7)
void gp_default_4_9_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 9 -> group 9)
void gp_default_4_9_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 9 -> group 11)
void gp_default_4_9_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 10 -> group 6)
void gp_default_4_10_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 10 -> group 8)
void gp_default_4_10_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 10 -> group 10)
void gp_default_4_10_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 11 -> group 7)
void gp_default_4_11_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 11 -> group 9)
void gp_default_4_11_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 11 -> group 11)
void gp_default_4_11_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 0 -> group 5)
void gp_default_5_0_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 1 -> group 4)
void gp_default_5_1_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 1 -> group 6)
void gp_default_5_1_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 2 -> group 3)
void gp_default_5_2_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 2 -> group 5)
void gp_default_5_2_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 2 -> group 7)
void gp_default_5_2_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 3 -> group 2)
void gp_default_5_3_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 3 -> group 4)
void gp_default_5_3_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 3 -> group 6)
void gp_default_5_3_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 3 -> group 8)
void gp_default_5_3_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 4 -> group 1)
void gp_default_5_4_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 4 -> group 3)
void gp_default_5_4_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 4 -> group 5)
void gp_default_5_4_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 4 -> group 7)
void gp_default_5_4_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 4 -> group 9)
void gp_default_5_4_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 5 -> group 0)
void gp_default_5_5_0(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 5 -> group 2)
void gp_default_5_5_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 5 -> group 4)
void gp_default_5_5_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 5 -> group 6)
void gp_default_5_5_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 5 -> group 8)
void gp_default_5_5_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 5 -> group 10)
void gp_default_5_5_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 6 -> group 1)
void gp_default_5_6_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 6 -> group 3)
void gp_default_5_6_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 6 -> group 5)
void gp_default_5_6_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 6 -> group 7)
void gp_default_5_6_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 6 -> group 9)
void gp_default_5_6_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 6 -> group 11)
void gp_default_5_6_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 7 -> group 2)
void gp_default_5_7_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 7 -> group 4)
void gp_default_5_7_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 7 -> group 6)
void gp_default_5_7_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 7 -> group 8)
void gp_default_5_7_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 7 -> group 10)
void gp_default_5_7_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 8 -> group 3)
void gp_default_5_8_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 8 -> group 5)
void gp_default_5_8_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 8 -> group 7)
void gp_default_5_8_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 8 -> group 9)
void gp_default_5_8_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 8 -> group 11)
void gp_default_5_8_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 9 -> group 4)
void gp_default_5_9_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 9 -> group 6)
void gp_default_5_9_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 9 -> group 8)
void gp_default_5_9_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 9 -> group 10)
void gp_default_5_9_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 10 -> group 5)
void gp_default_5_10_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 10 -> group 7)
void gp_default_5_10_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 10 -> group 9)
void gp_default_5_10_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 10 -> group 11)
void gp_default_5_10_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 11 -> group 6)
void gp_default_5_11_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 11 -> group 8)
void gp_default_5_11_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 11 -> group 10)
void gp_default_5_11_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 0 -> group 6)
void gp_default_6_0_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 1 -> group 5)
void gp_default_6_1_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 1 -> group 7)
void gp_default_6_1_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 2 -> group 4)
void gp_default_6_2_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 2 -> group 6)
void gp_default_6_2_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 2 -> group 8)
void gp_default_6_2_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 3 -> group 3)
void gp_default_6_3_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 3 -> group 5)
void gp_default_6_3_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 3 -> group 7)
void gp_default_6_3_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 3 -> group 9)
void gp_default_6_3_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 4 -> group 2)
void gp_default_6_4_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 4 -> group 4)
void gp_default_6_4_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 4 -> group 6)
void gp_default_6_4_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 4 -> group 8)
void gp_default_6_4_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 4 -> group 10)
void gp_default_6_4_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 5 -> group 1)
void gp_default_6_5_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 5 -> group 3)
void gp_default_6_5_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 5 -> group 5)
void gp_default_6_5_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 5 -> group 7)
void gp_default_6_5_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 5 -> group 9)
void gp_default_6_5_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 5 -> group 11)
void gp_default_6_5_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 6 -> group 0)
void gp_default_6_6_0(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 6 -> group 2)
void gp_default_6_6_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 6 -> group 4)
void gp_default_6_6_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 6 -> group 6)
void gp_default_6_6_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 6 -> group 8)
void gp_default_6_6_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 6 -> group 10)
void gp_default_6_6_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 7 -> group 1)
void gp_default_6_7_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 7 -> group 3)
void gp_default_6_7_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 7 -> group 5)
void gp_default_6_7_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 7 -> group 7)
void gp_default_6_7_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 7 -> group 9)
void gp_default_6_7_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 7 -> group 11)
void gp_default_6_7_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 8 -> group 2)
void gp_default_6_8_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 8 -> group 4)
void gp_default_6_8_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 8 -> group 6)
void gp_default_6_8_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 8 -> group 8)
void gp_default_6_8_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 8 -> group 10)
void gp_default_6_8_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 9 -> group 3)
void gp_default_6_9_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 9 -> group 5)
void gp_default_6_9_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 9 -> group 7)
void gp_default_6_9_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 9 -> group 9)
void gp_default_6_9_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 9 -> group 11)
void gp_default_6_9_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 10 -> group 4)
void gp_default_6_10_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 10 -> group 6)
void gp_default_6_10_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 10 -> group 8)
void gp_default_6_10_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 10 -> group 10)
void gp_default_6_10_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 11 -> group 5)
void gp_default_6_11_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 11 -> group 7)
void gp_default_6_11_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 11 -> group 9)
void gp_default_6_11_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 6  x  group 11 -> group 11)
void gp_default_6_11_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 0 -> group 7)
void gp_default_7_0_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 1 -> group 6)
void gp_default_7_1_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 1 -> group 8)
void gp_default_7_1_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 2 -> group 5)
void gp_default_7_2_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 2 -> group 7)
void gp_default_7_2_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 2 -> group 9)
void gp_default_7_2_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 3 -> group 4)
void gp_default_7_3_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 3 -> group 6)
void gp_default_7_3_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 3 -> group 8)
void gp_default_7_3_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 3 -> group 10)
void gp_default_7_3_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 4 -> group 3)
void gp_default_7_4_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 4 -> group 5)
void gp_default_7_4_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 4 -> group 7)
void gp_default_7_4_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 4 -> group 9)
void gp_default_7_4_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 4 -> group 11)
void gp_default_7_4_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 5 -> group 2)
void gp_default_7_5_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 5 -> group 4)
void gp_default_7_5_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 5 -> group 6)
void gp_default_7_5_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 5 -> group 8)
void gp_default_7_5_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 5 -> group 10)
void gp_default_7_5_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 6 -> group 1)
void gp_default_7_6_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 6 -> group 3)
void gp_default_7_6_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 6 -> group 5)
void gp_default_7_6_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 6 -> group 7)
void gp_default_7_6_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 6 -> group 9)
void gp_default_7_6_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 6 -> group 11)
void gp_default_7_6_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 7 -> group 0)
void gp_default_7_7_0(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 7 -> group 2)
void gp_default_7_7_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 7 -> group 4)
void gp_default_7_7_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 7 -> group 6)
void gp_default_7_7_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 7 -> group 8)
void gp_default_7_7_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 7 -> group 10)
void gp_default_7_7_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 8 -> group 1)
void gp_default_7_8_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 8 -> group 3)
void gp_default_7_8_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 8 -> group 5)
void gp_default_7_8_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 8 -> group 7)
void gp_default_7_8_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 8 -> group 9)
void gp_default_7_8_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 8 -> group 11)
void gp_default_7_8_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 9 -> group 2)
void gp_default_7_9_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 9 -> group 4)
void gp_default_7_9_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 9 -> group 6)
void gp_default_7_9_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 9 -> group 8)
void gp_default_7_9_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 9 -> group 10)
void gp_default_7_9_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 10 -> group 3)
void gp_default_7_10_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 10 -> group 5)
void gp_default_7_10_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 10 -> group 7)
void gp_default_7_10_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 10 -> group 9)
void gp_default_7_10_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 10 -> group 11)
void gp_default_7_10_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 11 -> group 4)
void gp_default_7_11_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 11 -> group 6)
void gp_default_7_11_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 11 -> group 8)
void gp_default_7_11_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 7  x  group 11 -> group 10)
void gp_default_7_11_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 0 -> group 8)
void gp_default_8_0_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 1 -> group 7)
void gp_default_8_1_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 1 -> group 9)
void gp_default_8_1_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 2 -> group 6)
void gp_default_8_2_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 2 -> group 8)
void gp_default_8_2_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 2 -> group 10)
void gp_default_8_2_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 3 -> group 5)
void gp_default_8_3_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 3 -> group 7)
void gp_default_8_3_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 3 -> group 9)
void gp_default_8_3_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 3 -> group 11)
void gp_default_8_3_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 4 -> group 4)
void gp_default_8_4_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 4 -> group 6)
void gp_default_8_4_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 4 -> group 8)
void gp_default_8_4_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 4 -> group 10)
void gp_default_8_4_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 5 -> group 3)
void gp_default_8_5_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 5 -> group 5)
void gp_default_8_5_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 5 -> group 7)
void gp_default_8_5_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 5 -> group 9)
void gp_default_8_5_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 5 -> group 11)
void gp_default_8_5_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 6 -> group 2)
void gp_default_8_6_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 6 -> group 4)
void gp_default_8_6_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 6 -> group 6)
void gp_default_8_6_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 6 -> group 8)
void gp_default_8_6_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 6 -> group 10)
void gp_default_8_6_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 7 -> group 1)
void gp_default_8_7_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 7 -> group 3)
void gp_default_8_7_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 7 -> group 5)
void gp_default_8_7_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 7 -> group 7)
void gp_default_8_7_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 7 -> group 9)
void gp_default_8_7_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 7 -> group 11)
void gp_default_8_7_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 8 -> group 0)
void gp_default_8_8_0(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 8 -> group 2)
void gp_default_8_8_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 8 -> group 4)
void gp_default_8_8_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 8 -> group 6)
void gp_default_8_8_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 8 -> group 8)
void gp_default_8_8_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 8 -> group 10)
void gp_default_8_8_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 9 -> group 1)
void gp_default_8_9_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 9 -> group 3)
void gp_default_8_9_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 9 -> group 5)
void gp_default_8_9_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 9 -> group 7)
void gp_default_8_9_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 9 -> group 9)
void gp_default_8_9_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 9 -> group 11)
void gp_default_8_9_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 10 -> group 2)
void gp_default_8_10_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 10 -> group 4)
void gp_default_8_10_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 10 -> group 6)
void gp_default_8_10_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 10 -> group 8)
void gp_default_8_10_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 10 -> group 10)
void gp_default_8_10_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 11 -> group 3)
void gp_default_8_11_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 11 -> group 5)
void gp_default_8_11_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 11 -> group 7)
void gp_default_8_11_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 11 -> group 9)
void gp_default_8_11_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 8  x  group 11 -> group 11)
void gp_default_8_11_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 0 -> group 9)
void gp_default_9_0_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 1 -> group 8)
void gp_default_9_1_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 1 -> group 10)
void gp_default_9_1_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 2 -> group 7)
void gp_default_9_2_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 2 -> group 9)
void gp_default_9_2_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 2 -> group 11)
void gp_default_9_2_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 3 -> group 6)
void gp_default_9_3_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 3 -> group 8)
void gp_default_9_3_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 3 -> group 10)
void gp_default_9_3_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 4 -> group 5)
void gp_default_9_4_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 4 -> group 7)
void gp_default_9_4_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 4 -> group 9)
void gp_default_9_4_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 4 -> group 11)
void gp_default_9_4_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 5 -> group 4)
void gp_default_9_5_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 5 -> group 6)
void gp_default_9_5_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 5 -> group 8)
void gp_default_9_5_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 5 -> group 10)
void gp_default_9_5_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 6 -> group 3)
void gp_default_9_6_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 6 -> group 5)
void gp_default_9_6_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 6 -> group 7)
void gp_default_9_6_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 6 -> group 9)
void gp_default_9_6_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 6 -> group 11)
void gp_default_9_6_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 7 -> group 2)
void gp_default_9_7_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 7 -> group 4)
void gp_default_9_7_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 7 -> group 6)
void gp_default_9_7_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 7 -> group 8)
void gp_default_9_7_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 7 -> group 10)
void gp_default_9_7_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 8 -> group 1)
void gp_default_9_8_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 8 -> group 3)
void gp_default_9_8_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 8 -> group 5)
void gp_default_9_8_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 8 -> group 7)
void gp_default_9_8_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 8 -> group 9)
void gp_default_9_8_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 8 -> group 11)
void gp_default_9_8_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 9 -> group 0)
void gp_default_9_9_0(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 9 -> group 2)
void gp_default_9_9_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 9 -> group 4)
void gp_default_9_9_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 9 -> group 6)
void gp_default_9_9_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 9 -> group 8)
void gp_default_9_9_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 9 -> group 10)
void gp_default_9_9_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 10 -> group 1)
void gp_default_9_10_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 10 -> group 3)
void gp_default_9_10_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 10 -> group 5)
void gp_default_9_10_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 10 -> group 7)
void gp_default_9_10_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 10 -> group 9)
void gp_default_9_10_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 10 -> group 11)
void gp_default_9_10_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 11 -> group 2)
void gp_default_9_11_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 11 -> group 4)
void gp_default_9_11_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 11 -> group 6)
void gp_default_9_11_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 11 -> group 8)
void gp_default_9_11_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 9  x  group 11 -> group 10)
void gp_default_9_11_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 0 -> group 10)
void gp_default_10_0_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 1 -> group 9)
void gp_default_10_1_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 1 -> group 11)
void gp_default_10_1_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 2 -> group 8)
void gp_default_10_2_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 2 -> group 10)
void gp_default_10_2_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 3 -> group 7)
void gp_default_10_3_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 3 -> group 9)
void gp_default_10_3_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 3 -> group 11)
void gp_default_10_3_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 4 -> group 6)
void gp_default_10_4_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 4 -> group 8)
void gp_default_10_4_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 4 -> group 10)
void gp_default_10_4_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 5 -> group 5)
void gp_default_10_5_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 5 -> group 7)
void gp_default_10_5_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 5 -> group 9)
void gp_default_10_5_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 5 -> group 11)
void gp_default_10_5_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 6 -> group 4)
void gp_default_10_6_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 6 -> group 6)
void gp_default_10_6_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 6 -> group 8)
void gp_default_10_6_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 6 -> group 10)
void gp_default_10_6_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 7 -> group 3)
void gp_default_10_7_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 7 -> group 5)
void gp_default_10_7_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 7 -> group 7)
void gp_default_10_7_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 7 -> group 9)
void gp_default_10_7_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 7 -> group 11)
void gp_default_10_7_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 8 -> group 2)
void gp_default_10_8_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 8 -> group 4)
void gp_default_10_8_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 8 -> group 6)
void gp_default_10_8_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 8 -> group 8)
void gp_default_10_8_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 8 -> group 10)
void gp_default_10_8_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 9 -> group 1)
void gp_default_10_9_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 9 -> group 3)
void gp_default_10_9_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 9 -> group 5)
void gp_default_10_9_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 9 -> group 7)
void gp_default_10_9_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 9 -> group 9)
void gp_default_10_9_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 9 -> group 11)
void gp_default_10_9_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 10 -> group 0)
void gp_default_10_10_0(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 10 -> group 2)
void gp_default_10_10_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 10 -> group 4)
void gp_default_10_10_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 10 -> group 6)
void gp_default_10_10_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 10 -> group 8)
void gp_default_10_10_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 10 -> group 10)
void gp_default_10_10_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 11 -> group 1)
void gp_default_10_11_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 11 -> group 3)
void gp_default_10_11_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 11 -> group 5)
void gp_default_10_11_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 11 -> group 7)
void gp_default_10_11_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 11 -> group 9)
void gp_default_10_11_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 10  x  group 11 -> group 11)
void gp_default_10_11_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 0 -> group 11)
void gp_default_11_0_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 1 -> group 10)
void gp_default_11_1_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 2 -> group 9)
void gp_default_11_2_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 2 -> group 11)
void gp_default_11_2_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 3 -> group 8)
void gp_default_11_3_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 3 -> group 10)
void gp_default_11_3_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 4 -> group 7)
void gp_default_11_4_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 4 -> group 9)
void gp_default_11_4_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 4 -> group 11)
void gp_default_11_4_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 5 -> group 6)
void gp_default_11_5_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 5 -> group 8)
void gp_default_11_5_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 5 -> group 10)
void gp_default_11_5_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 6 -> group 5)
void gp_default_11_6_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 6 -> group 7)
void gp_default_11_6_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 6 -> group 9)
void gp_default_11_6_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 6 -> group 11)
void gp_default_11_6_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 7 -> group 4)
void gp_default_11_7_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 7 -> group 6)
void gp_default_11_7_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 7 -> group 8)
void gp_default_11_7_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 7 -> group 10)
void gp_default_11_7_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 8 -> group 3)
void gp_default_11_8_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 8 -> group 5)
void gp_default_11_8_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 8 -> group 7)
void gp_default_11_8_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 8 -> group 9)
void gp_default_11_8_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 8 -> group 11)
void gp_default_11_8_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 9 -> group 2)
void gp_default_11_9_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 9 -> group 4)
void gp_default_11_9_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 9 -> group 6)
void gp_default_11_9_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 9 -> group 8)
void gp_default_11_9_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 9 -> group 10)
void gp_default_11_9_10(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 10 -> group 1)
void gp_default_11_10_1(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 10 -> group 3)
void gp_default_11_10_3(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 10 -> group 5)
void gp_default_11_10_5(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 10 -> group 7)
void gp_default_11_10_7(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 10 -> group 9)
void gp_default_11_10_9(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 10 -> group 11)
void gp_default_11_10_11(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 11 -> group 0)
void gp_default_11_11_0(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 11 -> group 2)
void gp_default_11_11_2(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 11 -> group 4)
void gp_default_11_11_4(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 11 -> group 6)
void gp_default_11_11_6(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 11 -> group 8)
void gp_default_11_11_8(const myDouble *A, const myDouble *B, myDouble *C);
/// Computes the partial geometric product of two multivectors (group 11  x  group 11 -> group 10)
void gp_default_11_11_10(const myDouble *A, const myDouble *B, myDouble *C);
/// copies coordinates of group 0
void copyGroup_0(const myDouble *A, myDouble *C);
/// copies and multiplies (by s) coordinates of group 0
void copyMul_0(const myDouble *A, myDouble *C, myDouble s);
/// copies and divides (by s) coordinates of group 0
void copyDiv_0(const myDouble *A, myDouble *C, myDouble s);
/// adds coordinates of group 0 from variable A to C
void add_0(const myDouble *A, myDouble *C);
/// subtracts coordinates of group 0 in variable A from C
void sub_0(const myDouble *A, myDouble *C);
/// negate coordinates of group 0 of variable A
void neg_0(const myDouble *A, myDouble *C);
/// adds coordinates of group 0 of variables A and B
void add2_0_0(const myDouble *A, const myDouble *B, myDouble *C);
/// subtracts coordinates of group 0 of variables A from B
void sub2_0_0(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise multiplication of coordinates of group 0 of variables A and B
void hp_0_0(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise division of coordinates of group 0 of variables A and B
/// (no checks for divide by zero are made)
void ihp_0_0(const myDouble *A, const myDouble *B, myDouble *C);
/// check for equality up to eps of coordinates of group 0 of variables A and B
bool equals_0_0(const myDouble *A, const myDouble *B, myDouble eps);
/// checks if coordinates of group 0 of variable A are zero up to eps
bool zeroGroup_0(const myDouble *A, myDouble eps);
/// copies coordinates of group 1
void copyGroup_1(const myDouble *A, myDouble *C);
/// copies and multiplies (by s) coordinates of group 1
void copyMul_1(const myDouble *A, myDouble *C, myDouble s);
/// copies and divides (by s) coordinates of group 1
void copyDiv_1(const myDouble *A, myDouble *C, myDouble s);
/// adds coordinates of group 1 from variable A to C
void add_1(const myDouble *A, myDouble *C);
/// subtracts coordinates of group 1 in variable A from C
void sub_1(const myDouble *A, myDouble *C);
/// negate coordinates of group 1 of variable A
void neg_1(const myDouble *A, myDouble *C);
/// adds coordinates of group 1 of variables A and B
void add2_1_1(const myDouble *A, const myDouble *B, myDouble *C);
/// subtracts coordinates of group 1 of variables A from B
void sub2_1_1(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise multiplication of coordinates of group 1 of variables A and B
void hp_1_1(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise division of coordinates of group 1 of variables A and B
/// (no checks for divide by zero are made)
void ihp_1_1(const myDouble *A, const myDouble *B, myDouble *C);
/// check for equality up to eps of coordinates of group 1 of variables A and B
bool equals_1_1(const myDouble *A, const myDouble *B, myDouble eps);
/// checks if coordinates of group 1 of variable A are zero up to eps
bool zeroGroup_1(const myDouble *A, myDouble eps);
/// copies coordinates of group 2
void copyGroup_2(const myDouble *A, myDouble *C);
/// copies and multiplies (by s) coordinates of group 2
void copyMul_2(const myDouble *A, myDouble *C, myDouble s);
/// copies and divides (by s) coordinates of group 2
void copyDiv_2(const myDouble *A, myDouble *C, myDouble s);
/// adds coordinates of group 2 from variable A to C
void add_2(const myDouble *A, myDouble *C);
/// subtracts coordinates of group 2 in variable A from C
void sub_2(const myDouble *A, myDouble *C);
/// negate coordinates of group 2 of variable A
void neg_2(const myDouble *A, myDouble *C);
/// adds coordinates of group 2 of variables A and B
void add2_2_2(const myDouble *A, const myDouble *B, myDouble *C);
/// subtracts coordinates of group 2 of variables A from B
void sub2_2_2(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise multiplication of coordinates of group 2 of variables A and B
void hp_2_2(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise division of coordinates of group 2 of variables A and B
/// (no checks for divide by zero are made)
void ihp_2_2(const myDouble *A, const myDouble *B, myDouble *C);
/// check for equality up to eps of coordinates of group 2 of variables A and B
bool equals_2_2(const myDouble *A, const myDouble *B, myDouble eps);
/// checks if coordinates of group 2 of variable A are zero up to eps
bool zeroGroup_2(const myDouble *A, myDouble eps);
/// copies coordinates of group 3
void copyGroup_3(const myDouble *A, myDouble *C);
/// copies and multiplies (by s) coordinates of group 3
void copyMul_3(const myDouble *A, myDouble *C, myDouble s);
/// copies and divides (by s) coordinates of group 3
void copyDiv_3(const myDouble *A, myDouble *C, myDouble s);
/// adds coordinates of group 3 from variable A to C
void add_3(const myDouble *A, myDouble *C);
/// subtracts coordinates of group 3 in variable A from C
void sub_3(const myDouble *A, myDouble *C);
/// negate coordinates of group 3 of variable A
void neg_3(const myDouble *A, myDouble *C);
/// adds coordinates of group 3 of variables A and B
void add2_3_3(const myDouble *A, const myDouble *B, myDouble *C);
/// subtracts coordinates of group 3 of variables A from B
void sub2_3_3(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise multiplication of coordinates of group 3 of variables A and B
void hp_3_3(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise division of coordinates of group 3 of variables A and B
/// (no checks for divide by zero are made)
void ihp_3_3(const myDouble *A, const myDouble *B, myDouble *C);
/// check for equality up to eps of coordinates of group 3 of variables A and B
bool equals_3_3(const myDouble *A, const myDouble *B, myDouble eps);
/// checks if coordinates of group 3 of variable A are zero up to eps
bool zeroGroup_3(const myDouble *A, myDouble eps);
/// copies coordinates of group 4
void copyGroup_4(const myDouble *A, myDouble *C);
/// copies and multiplies (by s) coordinates of group 4
void copyMul_4(const myDouble *A, myDouble *C, myDouble s);
/// copies and divides (by s) coordinates of group 4
void copyDiv_4(const myDouble *A, myDouble *C, myDouble s);
/// adds coordinates of group 4 from variable A to C
void add_4(const myDouble *A, myDouble *C);
/// subtracts coordinates of group 4 in variable A from C
void sub_4(const myDouble *A, myDouble *C);
/// negate coordinates of group 4 of variable A
void neg_4(const myDouble *A, myDouble *C);
/// adds coordinates of group 4 of variables A and B
void add2_4_4(const myDouble *A, const myDouble *B, myDouble *C);
/// subtracts coordinates of group 4 of variables A from B
void sub2_4_4(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise multiplication of coordinates of group 4 of variables A and B
void hp_4_4(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise division of coordinates of group 4 of variables A and B
/// (no checks for divide by zero are made)
void ihp_4_4(const myDouble *A, const myDouble *B, myDouble *C);
/// check for equality up to eps of coordinates of group 4 of variables A and B
bool equals_4_4(const myDouble *A, const myDouble *B, myDouble eps);
/// checks if coordinates of group 4 of variable A are zero up to eps
bool zeroGroup_4(const myDouble *A, myDouble eps);
/// copies coordinates of group 5
void copyGroup_5(const myDouble *A, myDouble *C);
/// copies and multiplies (by s) coordinates of group 5
void copyMul_5(const myDouble *A, myDouble *C, myDouble s);
/// copies and divides (by s) coordinates of group 5
void copyDiv_5(const myDouble *A, myDouble *C, myDouble s);
/// adds coordinates of group 5 from variable A to C
void add_5(const myDouble *A, myDouble *C);
/// subtracts coordinates of group 5 in variable A from C
void sub_5(const myDouble *A, myDouble *C);
/// negate coordinates of group 5 of variable A
void neg_5(const myDouble *A, myDouble *C);
/// adds coordinates of group 5 of variables A and B
void add2_5_5(const myDouble *A, const myDouble *B, myDouble *C);
/// subtracts coordinates of group 5 of variables A from B
void sub2_5_5(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise multiplication of coordinates of group 5 of variables A and B
void hp_5_5(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise division of coordinates of group 5 of variables A and B
/// (no checks for divide by zero are made)
void ihp_5_5(const myDouble *A, const myDouble *B, myDouble *C);
/// check for equality up to eps of coordinates of group 5 of variables A and B
bool equals_5_5(const myDouble *A, const myDouble *B, myDouble eps);
/// checks if coordinates of group 5 of variable A are zero up to eps
bool zeroGroup_5(const myDouble *A, myDouble eps);
/// copies coordinates of group 6
void copyGroup_6(const myDouble *A, myDouble *C);
/// copies and multiplies (by s) coordinates of group 6
void copyMul_6(const myDouble *A, myDouble *C, myDouble s);
/// copies and divides (by s) coordinates of group 6
void copyDiv_6(const myDouble *A, myDouble *C, myDouble s);
/// adds coordinates of group 6 from variable A to C
void add_6(const myDouble *A, myDouble *C);
/// subtracts coordinates of group 6 in variable A from C
void sub_6(const myDouble *A, myDouble *C);
/// negate coordinates of group 6 of variable A
void neg_6(const myDouble *A, myDouble *C);
/// adds coordinates of group 6 of variables A and B
void add2_6_6(const myDouble *A, const myDouble *B, myDouble *C);
/// subtracts coordinates of group 6 of variables A from B
void sub2_6_6(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise multiplication of coordinates of group 6 of variables A and B
void hp_6_6(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise division of coordinates of group 6 of variables A and B
/// (no checks for divide by zero are made)
void ihp_6_6(const myDouble *A, const myDouble *B, myDouble *C);
/// check for equality up to eps of coordinates of group 6 of variables A and B
bool equals_6_6(const myDouble *A, const myDouble *B, myDouble eps);
/// checks if coordinates of group 6 of variable A are zero up to eps
bool zeroGroup_6(const myDouble *A, myDouble eps);
/// copies coordinates of group 7
void copyGroup_7(const myDouble *A, myDouble *C);
/// copies and multiplies (by s) coordinates of group 7
void copyMul_7(const myDouble *A, myDouble *C, myDouble s);
/// copies and divides (by s) coordinates of group 7
void copyDiv_7(const myDouble *A, myDouble *C, myDouble s);
/// adds coordinates of group 7 from variable A to C
void add_7(const myDouble *A, myDouble *C);
/// subtracts coordinates of group 7 in variable A from C
void sub_7(const myDouble *A, myDouble *C);
/// negate coordinates of group 7 of variable A
void neg_7(const myDouble *A, myDouble *C);
/// adds coordinates of group 7 of variables A and B
void add2_7_7(const myDouble *A, const myDouble *B, myDouble *C);
/// subtracts coordinates of group 7 of variables A from B
void sub2_7_7(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise multiplication of coordinates of group 7 of variables A and B
void hp_7_7(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise division of coordinates of group 7 of variables A and B
/// (no checks for divide by zero are made)
void ihp_7_7(const myDouble *A, const myDouble *B, myDouble *C);
/// check for equality up to eps of coordinates of group 7 of variables A and B
bool equals_7_7(const myDouble *A, const myDouble *B, myDouble eps);
/// checks if coordinates of group 7 of variable A are zero up to eps
bool zeroGroup_7(const myDouble *A, myDouble eps);
/// copies coordinates of group 8
void copyGroup_8(const myDouble *A, myDouble *C);
/// copies and multiplies (by s) coordinates of group 8
void copyMul_8(const myDouble *A, myDouble *C, myDouble s);
/// copies and divides (by s) coordinates of group 8
void copyDiv_8(const myDouble *A, myDouble *C, myDouble s);
/// adds coordinates of group 8 from variable A to C
void add_8(const myDouble *A, myDouble *C);
/// subtracts coordinates of group 8 in variable A from C
void sub_8(const myDouble *A, myDouble *C);
/// negate coordinates of group 8 of variable A
void neg_8(const myDouble *A, myDouble *C);
/// adds coordinates of group 8 of variables A and B
void add2_8_8(const myDouble *A, const myDouble *B, myDouble *C);
/// subtracts coordinates of group 8 of variables A from B
void sub2_8_8(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise multiplication of coordinates of group 8 of variables A and B
void hp_8_8(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise division of coordinates of group 8 of variables A and B
/// (no checks for divide by zero are made)
void ihp_8_8(const myDouble *A, const myDouble *B, myDouble *C);
/// check for equality up to eps of coordinates of group 8 of variables A and B
bool equals_8_8(const myDouble *A, const myDouble *B, myDouble eps);
/// checks if coordinates of group 8 of variable A are zero up to eps
bool zeroGroup_8(const myDouble *A, myDouble eps);
/// copies coordinates of group 9
void copyGroup_9(const myDouble *A, myDouble *C);
/// copies and multiplies (by s) coordinates of group 9
void copyMul_9(const myDouble *A, myDouble *C, myDouble s);
/// copies and divides (by s) coordinates of group 9
void copyDiv_9(const myDouble *A, myDouble *C, myDouble s);
/// adds coordinates of group 9 from variable A to C
void add_9(const myDouble *A, myDouble *C);
/// subtracts coordinates of group 9 in variable A from C
void sub_9(const myDouble *A, myDouble *C);
/// negate coordinates of group 9 of variable A
void neg_9(const myDouble *A, myDouble *C);
/// adds coordinates of group 9 of variables A and B
void add2_9_9(const myDouble *A, const myDouble *B, myDouble *C);
/// subtracts coordinates of group 9 of variables A from B
void sub2_9_9(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise multiplication of coordinates of group 9 of variables A and B
void hp_9_9(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise division of coordinates of group 9 of variables A and B
/// (no checks for divide by zero are made)
void ihp_9_9(const myDouble *A, const myDouble *B, myDouble *C);
/// check for equality up to eps of coordinates of group 9 of variables A and B
bool equals_9_9(const myDouble *A, const myDouble *B, myDouble eps);
/// checks if coordinates of group 9 of variable A are zero up to eps
bool zeroGroup_9(const myDouble *A, myDouble eps);
/// copies coordinates of group 10
void copyGroup_10(const myDouble *A, myDouble *C);
/// copies and multiplies (by s) coordinates of group 10
void copyMul_10(const myDouble *A, myDouble *C, myDouble s);
/// copies and divides (by s) coordinates of group 10
void copyDiv_10(const myDouble *A, myDouble *C, myDouble s);
/// adds coordinates of group 10 from variable A to C
void add_10(const myDouble *A, myDouble *C);
/// subtracts coordinates of group 10 in variable A from C
void sub_10(const myDouble *A, myDouble *C);
/// negate coordinates of group 10 of variable A
void neg_10(const myDouble *A, myDouble *C);
/// adds coordinates of group 10 of variables A and B
void add2_10_10(const myDouble *A, const myDouble *B, myDouble *C);
/// subtracts coordinates of group 10 of variables A from B
void sub2_10_10(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise multiplication of coordinates of group 10 of variables A and B
void hp_10_10(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise division of coordinates of group 10 of variables A and B
/// (no checks for divide by zero are made)
void ihp_10_10(const myDouble *A, const myDouble *B, myDouble *C);
/// check for equality up to eps of coordinates of group 10 of variables A and B
bool equals_10_10(const myDouble *A, const myDouble *B, myDouble eps);
/// checks if coordinates of group 10 of variable A are zero up to eps
bool zeroGroup_10(const myDouble *A, myDouble eps);
/// copies coordinates of group 11
void copyGroup_11(const myDouble *A, myDouble *C);
/// copies and multiplies (by s) coordinates of group 11
void copyMul_11(const myDouble *A, myDouble *C, myDouble s);
/// copies and divides (by s) coordinates of group 11
void copyDiv_11(const myDouble *A, myDouble *C, myDouble s);
/// adds coordinates of group 11 from variable A to C
void add_11(const myDouble *A, myDouble *C);
/// subtracts coordinates of group 11 in variable A from C
void sub_11(const myDouble *A, myDouble *C);
/// negate coordinates of group 11 of variable A
void neg_11(const myDouble *A, myDouble *C);
/// adds coordinates of group 11 of variables A and B
void add2_11_11(const myDouble *A, const myDouble *B, myDouble *C);
/// subtracts coordinates of group 11 of variables A from B
void sub2_11_11(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise multiplication of coordinates of group 11 of variables A and B
void hp_11_11(const myDouble *A, const myDouble *B, myDouble *C);
/// performs coordinate-wise division of coordinates of group 11 of variables A and B
/// (no checks for divide by zero are made)
void ihp_11_11(const myDouble *A, const myDouble *B, myDouble *C);
/// check for equality up to eps of coordinates of group 11 of variables A and B
bool equals_11_11(const myDouble *A, const myDouble *B, myDouble eps);
/// checks if coordinates of group 11 of variable A are zero up to eps
bool zeroGroup_11(const myDouble *A, myDouble eps);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_0_11(const myDouble *A, myDouble *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_0_11(const myDouble *A, myDouble *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_1_10(const myDouble *A, myDouble *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_1_10(const myDouble *A, myDouble *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_2_9(const myDouble *A, myDouble *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_2_9(const myDouble *A, myDouble *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_3_8(const myDouble *A, myDouble *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_3_8(const myDouble *A, myDouble *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_4_7(const myDouble *A, myDouble *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_4_7(const myDouble *A, myDouble *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_5_6(const myDouble *A, myDouble *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_5_6(const myDouble *A, myDouble *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_6_5(const myDouble *A, myDouble *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_6_5(const myDouble *A, myDouble *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_7_4(const myDouble *A, myDouble *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_7_4(const myDouble *A, myDouble *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_8_3(const myDouble *A, myDouble *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_8_3(const myDouble *A, myDouble *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_9_2(const myDouble *A, myDouble *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_9_2(const myDouble *A, myDouble *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_10_1(const myDouble *A, myDouble *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_10_1(const myDouble *A, myDouble *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_11_0(const myDouble *A, myDouble *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_11_0(const myDouble *A, myDouble *C);

		
		// swap IR pointers: 
		std::swap(IR1, IR2);
		
		// lower grade
		grade--;
	}
	
	// compute norm/multiplier, apply it, or recurse if we happened to create a near-null versor
	n2 = norm_returns_scalar(*IR1);
	if ((myDouble)fabs(n2) > minimumNorm * minimumNorm) {
		if (n2 != (myDouble)0.0) {
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


mv random_blade_dont_mangle_3_returns_mv(myDouble scale, int grade, int basisVectorBitmap) {
	myDouble minimumNorm = (myDouble)0.01;
	myDouble largestCoordinate = (myDouble)4.0;
	return random_blade_dont_mangle_3_returns_mv_ex(scale, grade, basisVectorBitmap, minimumNorm, scale * largestCoordinate);
}

mv random_blade_dont_mangle_3_returns_mv_ex(myDouble scale, int _grade, int basisVectorBitmap, 
		myDouble minimumNorm, myDouble largestCoordinate) 
{
	mv randomVector, tmp1, tmp2;
	mv *IR1 = &tmp1, *IR2 = &tmp2; // IR = intermediate result
	myDouble randomValues[11];
	myDouble n2, mul;
	int i;
	int grade = _grade;
	
	// set IR1 (intermediate result) to 1
	IR1->set((myDouble)1.0);

	while (grade > 0) {	// loop until grade == 0
		// fill array with random values
		for (i = 0; i < 11; i++) 
			randomValues[i] = (basisVectorBitmap & (1 << i))
				? ((myDouble)-1.0 + (myDouble)2.0 * genrand())
				: (myDouble)0.0;
		
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
	if ((myDouble)fabs(n2) > minimumNorm * minimumNorm) {
		if (n2 != (myDouble)0.0) {
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
// Testing code definitions:

int test_metric_default_mv(int NB_TESTS_SCALER) 
{
	int i, j;
	myDouble arr[11], dif;
	mv A, bv[11];
	myDouble M[121+1] = {
		(myDouble)1.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)1.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)1.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)1.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)1.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)1.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)1.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)1.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)1.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)1.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)0.0, (myDouble)1.0, (myDouble)0.0	}; // metric matrix

	// get all basis vectors

	e11ga::zero_11(arr);
	arr[0] = (myDouble)1.0;
	bv[0].set(2, arr);

	e11ga::zero_11(arr);
	arr[1] = (myDouble)1.0;
	bv[1].set(2, arr);

	e11ga::zero_11(arr);
	arr[2] = (myDouble)1.0;
	bv[2].set(2, arr);

	e11ga::zero_11(arr);
	arr[3] = (myDouble)1.0;
	bv[3].set(2, arr);

	e11ga::zero_11(arr);
	arr[4] = (myDouble)1.0;
	bv[4].set(2, arr);

	e11ga::zero_11(arr);
	arr[5] = (myDouble)1.0;
	bv[5].set(2, arr);

	e11ga::zero_11(arr);
	arr[6] = (myDouble)1.0;
	bv[6].set(2, arr);

	e11ga::zero_11(arr);
	arr[7] = (myDouble)1.0;
	bv[7].set(2, arr);

	e11ga::zero_11(arr);
	arr[8] = (myDouble)1.0;
	bv[8].set(2, arr);

	e11ga::zero_11(arr);
	arr[9] = (myDouble)1.0;
	bv[9].set(2, arr);

	e11ga::zero_11(arr);
	arr[10] = (myDouble)1.0;
	bv[10].set(2, arr);

	for (i = 0; i < 11; i++) {
		for (j = 0; j < 11; j++) {
			A = gp(bv[i], bv[j]);
			dif = M[i * 11 + j] - A.get_scalar();
			if ((dif < (myDouble)-1E-06) || (dif > (myDouble)1E-06)) {
				printf("test_metric_default_mv() test failed for %s %s\n", e11ga_basisVectorNames[i], e11ga_basisVectorNames[j]);
				return 0;
			}
		}
	}
	
	return 1;
}

int test_parse_mv(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 8192;
	mv A, B, C;
	std::string str;
	
	int i, basisVectorBitmap = -1;

	for (i = 0; i < NB_LOOPS; i++) {
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		str = A.toString("%2.20e");
		
		try {
			B = parse(str);
		} catch (const std::string &exStr) {
			printf("parse() test failed (%s)\n", exStr.c_str());
			return 0; // failure
		}
		
		C = subtract(A, B);

		if (C.largestCoordinate() > (myDouble)1E-06) {
			printf("parse() test failed (%s)\n", str.c_str());
			return 0; // failure
		}
	}
	
	return 1; // success
}

int test_genrand_myDouble(int NB_TESTS_SCALER) 
{
	int NB_BINS = 256;
	const int NB_LOOPS = NB_BINS * 1024;
	int bins[256];
	myDouble avg = (myDouble)0.0;
	myDouble r;
	int idx, i;
	
	// reset count of bins
	memset(bins, 0, NB_BINS * sizeof(int));
	
	// generate a lot of random values, compute average (should be 0.5) and fill bins
	for (i = 0; i < NB_LOOPS; i++) {
		r = genrand();
		avg += r;
		idx = (int)(r * (myDouble)NB_BINS);
		if (idx >= NB_BINS) idx = NB_BINS-1;
		bins[idx]++;
	}
	avg /= (myDouble)NB_LOOPS;
	
	if ((avg < (myDouble)0.49) || (avg > (myDouble)0.51)) {
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

int test_add_dont_mangle_59(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C;
	int i, g;
	myDouble s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		g = (int)(genrand() * (myDouble)11.5);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);
		
		// B = -A
		B = negate(A);
		
		C = add(A, B);
		
		// use mv_largestCoordinate() to verify
		if (C.largestCoordinate() > (myDouble)9.9999999999999991E-06) {
			printf("add() test failed\n");
			return 0; // failure
		}
		
	}
	return 1; // success
}

int test_subtract_dont_mangle_60(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C;
	int i, g;
	myDouble s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		g = (int)(genrand() * (myDouble)11.5);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);
		
		B = A;
		
		C = subtract(A, B);
		
		// use mv_largestCoordinate() to verify
		if (C.largestCoordinate() > (myDouble)9.9999999999999991E-06) {
			printf("subtract() test failed\n");
			return 0; // failure
		}
		
	}
	return 1; // success
}

int test_applyVersor_dont_mangle_99(int NB_TESTS_SCALER) {
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	myDouble baseScale = (myDouble)0.5;
	int g, i;
	myDouble s;
	mv V, IV, X, Y, VX, VY, IVVX, XdY, VXdVY, dif;
	int versorBasisVectorBitmap = 2047; // note: random versors restricted to Euclidean basis vectors.
	int bladeBasisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor and its inverse. Optionally make versor unit.
		s = baseScale + genrand();
		g = (int)(genrand() * (myDouble)11.5);
		V = random_versor_dont_mangle_1_returns_mv(s, g, versorBasisVectorBitmap);
		
		// avoid 'near'-singular versors
		if (V.largestCoordinate() > (myDouble)2.0)
			continue;		
		
		IV = versorInverse(V);

		// get two random blades		
		s = baseScale + genrand();
		g = (int)(genrand() * (myDouble)11.5);
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
		if (dif.largestCoordinate() > ((myDouble)9.9999999999999991E-05 )) {
			printf("applyVersor() test failed (metric test) (largestCoordinate = %e)\n", (double)dif.largestCoordinate() );
			return 0; // failure
		}
		
		// apply inverse transformation to VX
		IVVX = applyVersor(IV, VX);
		
		// see if X equals IVVX
		dif = subtract(X, IVVX);
		if (dif.largestCoordinate() > ((myDouble)9.9999999999999991E-05 )) {
			printf("applyVersor() test failed (inverse test) (largestCoordinate = %e)\n", (double)dif.largestCoordinate() );
			return 0; // failure
		}
	}
	return 1; // success
}

int test_applyUnitVersor_dont_mangle_101(int NB_TESTS_SCALER) {
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	myDouble baseScale = (myDouble)0.5;
	int g, i;
	myDouble s;
	mv V, IV, X, Y, VX, VY, IVVX, XdY, VXdVY, dif;
	mv tmp;
	int versorBasisVectorBitmap = 2047; // note: random versors restricted to Euclidean basis vectors.
	int bladeBasisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor and its inverse. Optionally make versor unit.
		s = baseScale + genrand();
		g = (int)(genrand() * (myDouble)11.5);
		V = random_versor_dont_mangle_1_returns_mv(s, g, versorBasisVectorBitmap);
		
		// avoid 'near'-singular versors
		if (V.largestCoordinate() > (myDouble)2.0)
			continue;		
		
		tmp = unit(V);
		V = tmp;
		IV = versorInverse(V);

		// get two random blades		
		s = baseScale + genrand();
		g = (int)(genrand() * (myDouble)11.5);
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
		if (dif.largestCoordinate() > ((myDouble)9.9999999999999991E-05 )) {
			printf("applyUnitVersor() test failed (metric test) (largestCoordinate = %e)\n", (double)dif.largestCoordinate() );
			return 0; // failure
		}
		
		// apply inverse transformation to VX
		IVVX = applyUnitVersor(IV, VX);
		
		// see if X equals IVVX
		dif = subtract(X, IVVX);
		if (dif.largestCoordinate() > ((myDouble)9.9999999999999991E-05 )) {
			printf("applyUnitVersor() test failed (inverse test) (largestCoordinate = %e)\n", (double)dif.largestCoordinate() );
			return 0; // failure
		}
	}
	return 1; // success
}

int test_applyVersorWI_dont_mangle_100(int NB_TESTS_SCALER) {
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	myDouble baseScale = (myDouble)0.5;
	int g, i;
	myDouble s;
	mv V, IV, X, Y, VX, VY, IVVX, XdY, VXdVY, dif;
	mv tmp;
	int versorBasisVectorBitmap = 2047; // note: random versors restricted to Euclidean basis vectors.
	int bladeBasisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor and its inverse. Optionally make versor unit.
		s = baseScale + genrand();
		g = (int)(genrand() * (myDouble)11.5);
		V = random_versor_dont_mangle_1_returns_mv(s, g, versorBasisVectorBitmap);
		
		// avoid 'near'-singular versors
		if (V.largestCoordinate() > (myDouble)2.0)
			continue;		
		
		tmp = unit(V);
		V = tmp;
		IV = versorInverse(V);

		// get two random blades		
		s = baseScale + genrand();
		g = (int)(genrand() * (myDouble)11.5);
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
		if (dif.largestCoordinate() > ((myDouble)9.9999999999999991E-05 )) {
			printf("applyVersorWI() test failed (metric test) (largestCoordinate = %e)\n", (double)dif.largestCoordinate() );
			return 0; // failure
		}
		
		// apply inverse transformation to VX
		IVVX = applyVersorWI(IV, VX, V);
		
		// see if X equals IVVX
		dif = subtract(X, IVVX);
		if (dif.largestCoordinate() > ((myDouble)9.9999999999999991E-05 )) {
			printf("applyVersorWI() test failed (inverse test) (largestCoordinate = %e)\n", (double)dif.largestCoordinate() );
			return 0; // failure
		}
	}
	return 1; // success
}

int test_div_dont_mangle_57(int NB_TESTS_SCALER) {
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	int i;
	mv A, B, C, dif;
	myDouble divider;
	
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		A = random_blade_dont_mangle_3_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		divider = (myDouble)0.01 + genrand();
		
		B = div(A, divider);
		
		C = gp(B, divider);
		
		// see if C equals A
		dif = subtract(C, A);
		if (dif.largestCoordinate() > ((myDouble)1E-06 )) {
			printf("div() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}

	return 1; // success
}

int test_dual_dont_mangle_55(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, dif;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		A = random_blade_dont_mangle_3_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		B = dual(A);
		
		C = undual(B);
		
		// check if equal to original:
		dif = subtract(A, C);
		if (dif.largestCoordinate() > (myDouble)1E-06) {
			printf("dual() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_undual_dont_mangle_56(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, dif;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		A = random_blade_dont_mangle_3_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		B = undual(A);
		
		C = dual(B);
		
		// check if equal to original:
		dif = subtract(A, C);
		if (dif.largestCoordinate() > (myDouble)1E-06) {
			printf("undual() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_equals_dont_mangle_58(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C;
	myDouble s, eps = (myDouble)0.2;
	int g, i;
	bool e, l;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		g = (int)(genrand() * (myDouble)11.5);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);
		B = random_versor_dont_mangle_1_returns_mv((myDouble)1.1 * eps, g, basisVectorBitmap);
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

int test_extractGrade_dont_mangle_61(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, G0, G1, G2, G3, G4, G5, G6, G7, G8, G9, G10, G11;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		// split it up into groups/grades:
		G0 = extractGrade(A, GROUP_0);
		G1 = extractGrade(A, GROUP_1);
		G2 = extractGrade(A, GROUP_2);
		G3 = extractGrade(A, GROUP_3);
		G4 = extractGrade(A, GROUP_4);
		G5 = extractGrade(A, GROUP_5);
		G6 = extractGrade(A, GROUP_6);
		G7 = extractGrade(A, GROUP_7);
		G8 = extractGrade(A, GROUP_8);
		G9 = extractGrade(A, GROUP_9);
		G10 = extractGrade(A, GROUP_10);
		G11 = extractGrade(A, GROUP_11);
		// sum all into 'B'
		B.set();
		B = add(B, G0);
		B = add(B, G1);
		B = add(B, G2);
		B = add(B, G3);
		B = add(B, G4);
		B = add(B, G5);
		B = add(B, G6);
		B = add(B, G7);
		B = add(B, G8);
		B = add(B, G9);
		B = add(B, G10);
		B = add(B, G11);

		// check if equal to original:
		C = subtract(A, B);
		if (C.largestCoordinate() > (myDouble)1E-06) {
			printf("extractGrade() test failed (largestCoordinate = %e)\n", (double)C.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade0_dont_mangle_67(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		B = extractGrade0(A);
		
		C = extractGrade(A, 4094);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > (myDouble)1E-06) {
			printf("extractGrade0() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade1_dont_mangle_63(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		B = extractGrade1(A);
		
		C = extractGrade(A, 4093);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > (myDouble)1E-06) {
			printf("extractGrade1() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade2_dont_mangle_66(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		B = extractGrade2(A);
		
		C = extractGrade(A, 4091);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > (myDouble)1E-06) {
			printf("extractGrade2() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade3_dont_mangle_62(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		B = extractGrade3(A);
		
		C = extractGrade(A, 4087);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > (myDouble)1E-06) {
			printf("extractGrade3() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade4_dont_mangle_65(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		B = extractGrade4(A);
		
		C = extractGrade(A, 4079);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > (myDouble)1E-06) {
			printf("extractGrade4() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade5_dont_mangle_69(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		B = extractGrade5(A);
		
		C = extractGrade(A, 4063);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > (myDouble)1E-06) {
			printf("extractGrade5() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade6_dont_mangle_71(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		B = extractGrade6(A);
		
		C = extractGrade(A, 4031);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > (myDouble)1E-06) {
			printf("extractGrade6() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade7_dont_mangle_72(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		B = extractGrade7(A);
		
		C = extractGrade(A, 3967);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > (myDouble)1E-06) {
			printf("extractGrade7() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade8_dont_mangle_73(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		B = extractGrade8(A);
		
		C = extractGrade(A, 3839);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > (myDouble)1E-06) {
			printf("extractGrade8() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade9_dont_mangle_74(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		B = extractGrade9(A);
		
		C = extractGrade(A, 3583);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > (myDouble)1E-06) {
			printf("extractGrade9() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade10_dont_mangle_77(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		B = extractGrade10(A);
		
		C = extractGrade(A, 3071);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > (myDouble)1E-06) {
			printf("extractGrade10() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_extractGrade11_dont_mangle_64(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D;
	int i;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		A = random_versor_dont_mangle_1_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		B = extractGrade11(A);
		
		C = extractGrade(A, 2047);
		
		// sum all into 'B'
		D = add(B, C);

		// check if equal to original:
		C = subtract(A, D);
		if (C.largestCoordinate() > (myDouble)1E-06) {
			printf("extractGrade11() test failed\n");
			return 0; // failure
		}
	}
	return 1; // success
}

int test_gp_dont_mangle_78(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 8192;
	mv A, B, C, D, E, V1, V2;
	int i;
	int o;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		o = (genrand() < (myDouble)0.5) ? 0 : 1; // even or odd?
		A = random_versor_dont_mangle_1_returns_mv(genrand(), ((int)(genrand() * (myDouble)11.5) & 0xFFFE) + o, basisVectorBitmap);
		B = random_versor_dont_mangle_1_returns_mv(genrand(), ((int)(genrand() * (myDouble)11.5) & 0xFFFE) + o, basisVectorBitmap);
		C = random_versor_dont_mangle_1_returns_mv(genrand(), ((int)(genrand() * (myDouble)11.5) & 0xFFFE) + o, basisVectorBitmap);
		
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
			if (D.largestCoordinate() > (myDouble)0.001) {
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
			if (D.largestCoordinate() > (myDouble)9.9999999999999991E-05) {
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
			if (D.largestCoordinate() > (myDouble)9.9999999999999991E-05) {
				printf("gp() test failed on 'A (B C) = (A B) C' (dif=%e)\n", (double)D.largestCoordinate());
				return 0; // failure
			}
		}		
	}
	return 1; // success
}

int test_gradeBitmap_dont_mangle_54(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, tmp, randomBlade;
	int i, j;
	int basisVectorBitmap = -1;
	int gradeBitmap1, gradeBitmap2, nbBlades, grade;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get sum of random blades, keep track of grades used
		gradeBitmap1 = 0;
		A.set();
		nbBlades = (int)(genrand() * (myDouble)11.5);
		for (j = 0; j < nbBlades; j++) {
			grade = (int)(genrand() * (myDouble)11.5);
			gradeBitmap1 |= 1 << grade;
			randomBlade = random_blade_dont_mangle_3_returns_mv((myDouble)1.0, grade, basisVectorBitmap);
			tmp = add(A, randomBlade);
			A = tmp;
		}
		
		gradeBitmap2 = gradeBitmap(A, (myDouble)0.0);
		
		// check if grade bitmaps match
		if (gradeBitmap1 != gradeBitmap2) {
			printf("gradeBitmap() test failed (grade bitmap %d vs %d)\n", gradeBitmap1, gradeBitmap2);
			return 0; // failure
		}
	}
	return 1; // success
}

int test_hp_dont_mangle_80(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D, dif;
	int i, j, g;
	int basisVectorBitmap = -1;
	myDouble s;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		s = genrand();
		g = (int)(genrand() * (myDouble)11.5);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);
		
		// copy it to another versor
		B = A;
		
		// set coordinates of B to random values (which may not be zero)
		for (j = 0; j < e11ga_mvSize[B.gu()]; j++) {
			B.m_c[j] = (myDouble)0.5 + genrand();
		}
		
		// do hadamard product
		C = hp(A, B);
		
		// invert coordinates of B manually
		for (j = 0; j < e11ga_mvSize[B.gu()]; j++) {
			B.m_c[j] = (myDouble)1.0 / B.m_c[j];
		}

		// do inverse hadamard product
		D = hp(C, B);
		
		// check if equal to original:
		dif = subtract(A, D);
		if (dif.largestCoordinate() > (myDouble)9.9999999999999991E-05) {
			printf("hp() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_ihp_dont_mangle_81(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D, dif;
	int i, j, g;
	int basisVectorBitmap = -1;
	myDouble s;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		s = genrand();
		g = (int)(genrand() * (myDouble)11.5);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);
		
		// copy it to another versor
		B = A;
		
		// set coordinates of B to random values (which may not be zero)
		for (j = 0; j < e11ga_mvSize[B.gu()]; j++) {
			B.m_c[j] = (myDouble)0.5 + genrand();
		}
		
		// do hadamard product
		C = ihp(A, B);
		
		// invert coordinates of B manually
		for (j = 0; j < e11ga_mvSize[B.gu()]; j++) {
			B.m_c[j] = (myDouble)1.0 / B.m_c[j];
		}

		// do inverse hadamard product
		D = ihp(C, B);
		
		// check if equal to original:
		dif = subtract(A, D);
		if (dif.largestCoordinate() > (myDouble)9.9999999999999991E-05) {
			printf("ihp() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_increment_dont_mangle_75(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D, one;
	int i, g;
	int basisVectorBitmap = -1;

	one = (myDouble)1.0;

	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		g = (int)(genrand() * (myDouble)11.5);
		A = random_versor_dont_mangle_1_returns_mv(genrand() + (myDouble)0.5, g, basisVectorBitmap);
		
		// increment/decrement
		B = increment(A);
		
		// undo the increment/decrement
		C = subtract(B, one);
		
		// see if (A - (B-1)) == 0
		D = subtract(A, C);
		
		if (D.largestCoordinate() > (myDouble)1E-06) {
			printf("increment() test failed (largestCoordinate of D = %e)\n", (double)D.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_decrement_dont_mangle_76(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D, one;
	int i, g;
	int basisVectorBitmap = -1;

	one = (myDouble)-1.0;

	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		g = (int)(genrand() * (myDouble)11.5);
		A = random_versor_dont_mangle_1_returns_mv(genrand() + (myDouble)0.5, g, basisVectorBitmap);
		
		// increment/decrement
		B = decrement(A);
		
		// undo the increment/decrement
		C = subtract(B, one);
		
		// see if (A - (B-1)) == 0
		D = subtract(A, C);
		
		if (D.largestCoordinate() > (myDouble)1E-06) {
			printf("decrement() test failed (largestCoordinate of D = %e)\n", (double)D.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_sp_dont_mangle_93(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D, E, dif;
	int i, ga, gb, gd;
	myDouble s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		ga = (int)(genrand() * (myDouble)11.5);
		A = random_blade_dont_mangle_3_returns_mv(s, ga, basisVectorBitmap);
		
		s = genrand();
		gb = (int)(genrand() * (myDouble)11.5);
		B = random_blade_dont_mangle_3_returns_mv(s, gb, basisVectorBitmap);
		
		// compute product using sp()
		C = sp(A, B);
		
		// simulate product using geometric product & grade part selection
		D = gp(A, B);
		gd = (ga > gb) ? ga - gb : gb - ga;
		E = extractGrade(D, e11ga_grades[0]);

		// check if equal:
		dif = subtract(C, E);
		if (dif.largestCoordinate() > (myDouble)9.9999999999999991E-05) {
			printf("sp() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_lc_dont_mangle_94(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D, E, dif;
	int i, ga, gb, gd;
	myDouble s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		ga = (int)(genrand() * (myDouble)11.5);
		A = random_blade_dont_mangle_3_returns_mv(s, ga, basisVectorBitmap);
		
		s = genrand();
		gb = (int)(genrand() * (myDouble)11.5);
		B = random_blade_dont_mangle_3_returns_mv(s, gb, basisVectorBitmap);
		
		// compute product using lc()
		C = lc(A, B);
		
		// simulate product using geometric product & grade part selection
		D = gp(A, B);
		gd = (ga > gb) ? ga - gb : gb - ga;
		if (ga > gb) E = (myDouble)0.0;
		else E = extractGrade(D, e11ga_grades[gd]);

		// check if equal:
		dif = subtract(C, E);
		if (dif.largestCoordinate() > (myDouble)9.9999999999999991E-05) {
			printf("lc() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_rc_dont_mangle_96(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D, E, dif;
	int i, ga, gb, gd;
	myDouble s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		ga = (int)(genrand() * (myDouble)11.5);
		A = random_blade_dont_mangle_3_returns_mv(s, ga, basisVectorBitmap);
		
		s = genrand();
		gb = (int)(genrand() * (myDouble)11.5);
		B = random_blade_dont_mangle_3_returns_mv(s, gb, basisVectorBitmap);
		
		// compute product using rc()
		C = rc(A, B);
		
		// simulate product using geometric product & grade part selection
		D = gp(A, B);
		gd = (ga > gb) ? ga - gb : gb - ga;
		if (ga < gb) E = (myDouble)0.0;
		else E = extractGrade(D, e11ga_grades[gd]);

		// check if equal:
		dif = subtract(C, E);
		if (dif.largestCoordinate() > (myDouble)9.9999999999999991E-05) {
			printf("rc() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_hip_dont_mangle_95(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D, E, dif;
	int i, ga, gb, gd;
	myDouble s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		ga = (int)(genrand() * (myDouble)11.5);
		A = random_blade_dont_mangle_3_returns_mv(s, ga, basisVectorBitmap);
		
		s = genrand();
		gb = (int)(genrand() * (myDouble)11.5);
		B = random_blade_dont_mangle_3_returns_mv(s, gb, basisVectorBitmap);
		
		// compute product using hip()
		C = hip(A, B);
		
		// simulate product using geometric product & grade part selection
		D = gp(A, B);
		gd = (ga > gb) ? ga - gb : gb - ga;
		if ((ga == 0) || (gb == 0)) E = (myDouble)0.0;
		else E = extractGrade(D, e11ga_grades[gd]);

		// check if equal:
		dif = subtract(C, E);
		if (dif.largestCoordinate() > (myDouble)9.9999999999999991E-05) {
			printf("hip() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_mhip_dont_mangle_97(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D, E, dif;
	int i, ga, gb, gd;
	myDouble s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		ga = (int)(genrand() * (myDouble)11.5);
		A = random_blade_dont_mangle_3_returns_mv(s, ga, basisVectorBitmap);
		
		s = genrand();
		gb = (int)(genrand() * (myDouble)11.5);
		B = random_blade_dont_mangle_3_returns_mv(s, gb, basisVectorBitmap);
		
		// compute product using mhip()
		C = mhip(A, B);
		
		// simulate product using geometric product & grade part selection
		D = gp(A, B);
		gd = (ga > gb) ? ga - gb : gb - ga;
		E = extractGrade(D, e11ga_grades[gd]);

		// check if equal:
		dif = subtract(C, E);
		if (dif.largestCoordinate() > (myDouble)9.9999999999999991E-05) {
			printf("mhip() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_norm_dont_mangle_87(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, reverseA, tmp;
	
	int i, g;
	int basisVectorBitmap = -1;
	myDouble s;
	myDouble n1, n2;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		s = genrand();
		g = (int)(genrand() * (myDouble)11.5);
		A = random_blade_dont_mangle_3_returns_mv(s, g, basisVectorBitmap);
		
		// compute norm
		n1 = norm(A);
		
		// compute norm manually (A . reverse(A))
		reverseA = reverse(A);
		tmp = gp(A, reverseA);
		n2 = tmp.get_scalar();
		n2 = (myDouble)sqrt(fabs(n2));
		
		// check if equal to original:
		if (fabs(n1 - n2) > (myDouble)1E-06) {
			printf("norm() test failed (difference = %e)\n", (double)fabs(n1 - n2));
			return 0; // failure
		}
	}
	return 1; // success
}

int test_norm2_dont_mangle_88(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, reverseA, tmp;
	
	int i, g;
	int basisVectorBitmap = -1;
	myDouble s;
	myDouble n1, n2;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		s = genrand();
		g = (int)(genrand() * (myDouble)11.5);
		A = random_blade_dont_mangle_3_returns_mv(s, g, basisVectorBitmap);
		
		// compute norm
		n1 = norm2(A);
		
		// compute norm manually (A . reverse(A))
		reverseA = reverse(A);
		tmp = gp(A, reverseA);
		n2 = tmp.get_scalar();
		
		// check if equal to original:
		if (fabs(n1 - n2) > (myDouble)1E-06) {
			printf("norm2() test failed (difference = %e)\n", (double)fabs(n1 - n2));
			return 0; // failure
		}
	}
	return 1; // success
}

int test_op_dont_mangle_98_1(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, C, D, E, dif;
	int i, ga, gb, gd;
	myDouble s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		ga = (int)(genrand() * (myDouble)11.5);
		A = random_blade_dont_mangle_3_returns_mv(s, ga, basisVectorBitmap);
		
		s = genrand();
		gb = (int)(genrand() * (myDouble)11.5);
		B = random_blade_dont_mangle_3_returns_mv(s, gb, basisVectorBitmap);
		
		// compute product using op()
		C = op(A, B);
		
		// simulate product using geometric product & grade part selection
		D = gp(A, B);
		gd = (ga > gb) ? ga - gb : gb - ga;
		E = extractGrade(D,  e11ga_grades[ga + gb]);

		// check if equal:
		dif = subtract(C, E);
		if (dif.largestCoordinate() > (myDouble)9.9999999999999991E-05) {
			printf("op() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_op_dont_mangle_98_2(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B;
	int i, ga;
	myDouble s;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		ga = (int)(genrand() * (myDouble)11.5);
		if (ga == 0) continue; // do not perform this test for grade 0
		A = random_blade_dont_mangle_3_returns_mv(s, ga, basisVectorBitmap);
		
		// compute A ^ A (should be zero)
		B = op(A, A);
		
		// check if B is zero:
		if (B.largestCoordinate() > (myDouble)9.9999999999999991E-06) {
			printf("op() test failed (largestCoordinate = %e)\n", (double)B.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_exp_dont_mangle_83(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, expA, sinhA, coshA, S, dif, tmp1;//, tmp2;
	int i, g;
	int basisVectorBitmap = -1;
	myDouble s;
	int order = 12;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade of grade 2
		s = (myDouble)2.0 * genrand();
		g = 2;
		A = random_blade_dont_mangle_3_returns_mv(s, g, basisVectorBitmap);
		
		if (genrand() > (myDouble)0.5) { // make sure that 'A' is not always a blade
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
		if (dif.largestCoordinate() > (myDouble)0.0316227766016838) { // sinh, cosh precision is very low
			printf("exp() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_cosh_dont_mangle_84(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, expA, sinhA, coshA, S, dif, tmp1;//, tmp2;
	int i, g;
	int basisVectorBitmap = -1;
	myDouble s;
	int order = 12;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade of grade 2
		s = (myDouble)2.0 * genrand();
		g = 2;
		A = random_blade_dont_mangle_3_returns_mv(s, g, basisVectorBitmap);
		
		if (genrand() > (myDouble)0.5) { // make sure that 'A' is not always a blade
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
		if (dif.largestCoordinate() > (myDouble)0.0316227766016838) { // sinh, cosh precision is very low
			printf("cosh() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_sinh_dont_mangle_82(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, expA, sinhA, coshA, S, dif, tmp1;//, tmp2;
	int i, g;
	int basisVectorBitmap = -1;
	myDouble s;
	int order = 12;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade of grade 2
		s = (myDouble)2.0 * genrand();
		g = 2;
		A = random_blade_dont_mangle_3_returns_mv(s, g, basisVectorBitmap);
		
		if (genrand() > (myDouble)0.5) { // make sure that 'A' is not always a blade
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
		if (dif.largestCoordinate() > (myDouble)0.0316227766016838) { // sinh, cosh precision is very low
			printf("sinh() test failed (largestCoordinate = %e)\n", (double)dif.largestCoordinate());
			return 0; // failure
		}
	}
	return 1; // success
}

int test_negate_dont_mangle_89(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, GA, GB;
	int i, c, n, g;
	int basisVectorBitmap = -1;
	myDouble s, dif;
	int signTable[12] = {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		s = genrand();
		g = (int)(genrand() * (myDouble)11.5);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);

		// apply function
		B = negate(A);
		
		// check grade
		for (n = 0; n <= 11; n++) {
			GA = extractGrade(A, e11ga_grades[n]);
			GB = extractGrade(B, e11ga_grades[n]);
			
			// check if grade usage matches
			if (GA.gu() != GB.gu()) {
				printf("negate() test failed (grade usage does not match)\n");
				return 0; // failure
			}
			
			// check each coordinate 
			for (c = 0; c < e11ga_mvSize[GA.gu()]; c++) {
				dif = (myDouble)fabs(GA.m_c[c] * (myDouble)signTable[n] - GB.m_c[c]);
				if (dif > (myDouble)1E-06) {
					printf("negate() test failed (dif = %e)\n", (double)dif);
					return 0; // failure
				}
			}
		}
		
	}
	return 1; // success
}

int test_cliffordConjugate_dont_mangle_92(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, GA, GB;
	int i, c, n, g;
	int basisVectorBitmap = -1;
	myDouble s, dif;
	int signTable[12] = {1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1};
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		s = genrand();
		g = (int)(genrand() * (myDouble)11.5);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);

		// apply function
		B = cliffordConjugate(A);
		
		// check grade
		for (n = 0; n <= 11; n++) {
			GA = extractGrade(A, e11ga_grades[n]);
			GB = extractGrade(B, e11ga_grades[n]);
			
			// check if grade usage matches
			if (GA.gu() != GB.gu()) {
				printf("cliffordConjugate() test failed (grade usage does not match)\n");
				return 0; // failure
			}
			
			// check each coordinate 
			for (c = 0; c < e11ga_mvSize[GA.gu()]; c++) {
				dif = (myDouble)fabs(GA.m_c[c] * (myDouble)signTable[n] - GB.m_c[c]);
				if (dif > (myDouble)1E-06) {
					printf("cliffordConjugate() test failed (dif = %e)\n", (double)dif);
					return 0; // failure
				}
			}
		}
		
	}
	return 1; // success
}

int test_gradeInvolution_dont_mangle_91(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, GA, GB;
	int i, c, n, g;
	int basisVectorBitmap = -1;
	myDouble s, dif;
	int signTable[12] = {1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1};
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		s = genrand();
		g = (int)(genrand() * (myDouble)11.5);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);

		// apply function
		B = gradeInvolution(A);
		
		// check grade
		for (n = 0; n <= 11; n++) {
			GA = extractGrade(A, e11ga_grades[n]);
			GB = extractGrade(B, e11ga_grades[n]);
			
			// check if grade usage matches
			if (GA.gu() != GB.gu()) {
				printf("gradeInvolution() test failed (grade usage does not match)\n");
				return 0; // failure
			}
			
			// check each coordinate 
			for (c = 0; c < e11ga_mvSize[GA.gu()]; c++) {
				dif = (myDouble)fabs(GA.m_c[c] * (myDouble)signTable[n] - GB.m_c[c]);
				if (dif > (myDouble)1E-06) {
					printf("gradeInvolution() test failed (dif = %e)\n", (double)dif);
					return 0; // failure
				}
			}
		}
		
	}
	return 1; // success
}

int test_reverse_dont_mangle_90(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, B, GA, GB;
	int i, c, n, g;
	int basisVectorBitmap = -1;
	myDouble s, dif;
	int signTable[12] = {1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1};
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		s = genrand();
		g = (int)(genrand() * (myDouble)11.5);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);

		// apply function
		B = reverse(A);
		
		// check grade
		for (n = 0; n <= 11; n++) {
			GA = extractGrade(A, e11ga_grades[n]);
			GB = extractGrade(B, e11ga_grades[n]);
			
			// check if grade usage matches
			if (GA.gu() != GB.gu()) {
				printf("reverse() test failed (grade usage does not match)\n");
				return 0; // failure
			}
			
			// check each coordinate 
			for (c = 0; c < e11ga_mvSize[GA.gu()]; c++) {
				dif = (myDouble)fabs(GA.m_c[c] * (myDouble)signTable[n] - GB.m_c[c]);
				if (dif > (myDouble)1E-06) {
					printf("reverse() test failed (dif = %e)\n", (double)dif);
					return 0; // failure
				}
			}
		}
		
	}
	return 1; // success
}

int test_unit_dont_mangle_68(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A, UA, RUA;
	int i;
	int basisVectorBitmap = -1;
	myDouble n;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random blade
		A = random_blade_dont_mangle_3_returns_mv(genrand(), (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		UA = unit(A);
		RUA = reverse(UA);
		n = sp(RUA, UA);
		
		if ((myDouble)(fabs(n) - (myDouble)1.0) > (myDouble)9.9999999999999991E-05) {
			printf("unit() test failed (|norm|-1 = %e)\n", (double)(fabs((double)n) - 1.0));
			return 0; // failure
		}

	}
	return 1; // success
}

int test_versorInverse_dont_mangle_70(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv V, VI, VVI, VIV, X, Y;
	int i;
	int basisVectorBitmap = -1;
	myDouble n1, n2;
	
	for (i = 0; i < NB_LOOPS; i++) {
		// get random versor
		V = random_versor_dont_mangle_1_returns_mv(genrand() + (myDouble)0.5, (int)(genrand() * (myDouble)11.5), basisVectorBitmap);
		
		// avoid 'near'-singular versors
		if (V.largestCoordinate() > (myDouble)2.0)
			continue;
		
		// compute inverse
		VI = versorInverse(V);
		
		// compute (inverse(V) V) and (V inverse(V))
		VIV = gp(VI, V);
		VVI = gp(V, VI);
		
		// check that scalar parts are 1
		n1 = VIV.get_scalar();
		n2 = VVI.get_scalar();
		
		if (fabs(n1 - (myDouble)1.0) > (myDouble)0.001) {
			printf("versorInverse() test failed |VI . V - 1|= %e)\n", (double)fabs(n1 - (myDouble)1.0));
			return 0; // failure
		}
		
		if (fabs(n2 - (myDouble)1.0) > (myDouble)0.001) {
			printf("versorInverse() test failed ( |V . VI - 1| = %e)\n", (double)fabs(n2 - (myDouble)1.0));
			return 0; // failure
		}
		
		// check that other grade parts are zero:
		X = extractGrade(VIV, 4094);
		Y = extractGrade(VVI, 4094);
		
		if (X.largestCoordinate() > (myDouble)0.001) {
			printf("versorInverse() test failed (largestCoordinate of VIV = %e)\n", (double)X.largestCoordinate());
			return 0; // failure
		}
		
		if (Y.largestCoordinate() > (myDouble)0.001) {
			printf("versorInverse() test failed (largestCoordinate of VVI = %e)\n", (double)Y.largestCoordinate());
			return 0; // failure
		}
		
	}
	return 1; // success
}

int test_zero_dont_mangle_53(int NB_TESTS_SCALER) 
{
	const int NB_LOOPS = 100 + NB_TESTS_SCALER / 2048;
	mv A;
	myDouble s, eps = (myDouble)0.2;
	int g, i, j;
	bool z, l, c;
	int basisVectorBitmap = -1;
	
	for (i = 0; i < NB_LOOPS; i++) {
		s = genrand();
		g = (int)(genrand() * (myDouble)11.5);
		A = random_versor_dont_mangle_1_returns_mv(s, g, basisVectorBitmap);
		
		// ask if zero thinks A is zero
		z = zero(A, eps);
		
		// check it yourself
		c = true; // assume it is zero
		for (j = 0; j < e11ga_mvSize[A.gu()]; j++) {
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
} // end of namespace e11ga

int main(int argc, char *argv[]) {
	int retVal = 0;
	// The number of tests will be proportional to this value.
	// This should become a command-line option
	const int NB_TESTS_SCALER = 16384;
	
	// seed random number generators with current time
	e11ga::genrand_timeSeed();

	// run all test functions
	if (!e11ga::test_metric_default_mv(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_parse_mv(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_genrand_myDouble(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_add_dont_mangle_59(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_subtract_dont_mangle_60(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_applyVersor_dont_mangle_99(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_applyUnitVersor_dont_mangle_101(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_applyVersorWI_dont_mangle_100(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_div_dont_mangle_57(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_dual_dont_mangle_55(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_undual_dont_mangle_56(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_equals_dont_mangle_58(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_extractGrade_dont_mangle_61(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_extractGrade0_dont_mangle_67(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_extractGrade1_dont_mangle_63(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_extractGrade2_dont_mangle_66(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_extractGrade3_dont_mangle_62(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_extractGrade4_dont_mangle_65(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_extractGrade5_dont_mangle_69(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_extractGrade6_dont_mangle_71(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_extractGrade7_dont_mangle_72(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_extractGrade8_dont_mangle_73(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_extractGrade9_dont_mangle_74(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_extractGrade10_dont_mangle_77(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_extractGrade11_dont_mangle_64(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_gp_dont_mangle_78(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_gradeBitmap_dont_mangle_54(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_hp_dont_mangle_80(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_ihp_dont_mangle_81(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_increment_dont_mangle_75(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_decrement_dont_mangle_76(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_sp_dont_mangle_93(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_lc_dont_mangle_94(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_rc_dont_mangle_96(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_hip_dont_mangle_95(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_mhip_dont_mangle_97(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_norm_dont_mangle_87(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_norm2_dont_mangle_88(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_op_dont_mangle_98_1(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_op_dont_mangle_98_2(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_exp_dont_mangle_83(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_cosh_dont_mangle_84(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_sinh_dont_mangle_82(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_negate_dont_mangle_89(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_cliffordConjugate_dont_mangle_92(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_gradeInvolution_dont_mangle_91(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_reverse_dont_mangle_90(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_unit_dont_mangle_68(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_versorInverse_dont_mangle_70(NB_TESTS_SCALER)) retVal = -1;
	if (!e11ga::test_zero_dont_mangle_53(NB_TESTS_SCALER)) retVal = -1;

	if (retVal != 0) printf("Test failed.\n");
	else printf("Done.\n");	

	return retVal;
}
