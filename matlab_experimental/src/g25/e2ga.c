/*
Copyright (C) 2008 Some Random Person
*/
/*
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

*/
#include <stdio.h>
#include "e2ga.h"

const int e2ga_spaceDim = 2;
const int e2ga_nbGroups = 3;
const int e2ga_metricEuclidean = 1;
const char *e2ga_basisVectorNames[2] = {
	"e1", "e2"
};
const int e2ga_grades[] = {GRADE_0, GRADE_1, GRADE_2, 0, 0, 0};
const int e2ga_groups[] = {GROUP_0, GROUP_1, GROUP_2};
const int e2ga_groupSize[3] = {
	1, 2, 1
};
const int e2ga_mvSize[8] = {
	0, 1, 2, 3, 1, 2, 3, 4};
const int e2ga_basisElements[4][3] = {
	{-1},
	{0, -1},
	{1, -1},
	{0, 1, -1}
};
const double e2ga_basisElementSignByIndex[4] =
	{1, 1, 1, 1};
const double e2ga_basisElementSignByBitmap[4] =
	{1, 1, 1, 1};
const int e2ga_basisElementIndexByBitmap[4] =
	{0, 1, 2, 3};
const int e2ga_basisElementBitmapByIndex[4] =
	{0, 1, 2, 3};
const int e2ga_basisElementGradeByBitmap[4] =
	{0, 1, 1, 2};
const int e2ga_basisElementGroupByBitmap[4] =
	{0, 1, 1, 2};

const char *g_e2gaTypenames[] = 
{
	"vector",
	"rotor",
	"e1_t",
	"e2_t",
	"I2_t"
};
e1_t e1 = {0};
e2_t e2 = {0};
I2_t I2 = {0};


void e2ga_double_zero_1(double *dst) {
	dst[0]=0.0;
}
void e2ga_double_copy_1(double *dst, const double *src) {
	dst[0] = src[0];
}
void e2ga_double_zero_2(double *dst) {
	dst[0]=dst[1]=0.0;
}
void e2ga_double_copy_2(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
}
void e2ga_double_zero_3(double *dst) {
	dst[0]=dst[1]=dst[2]=0.0;
}
void e2ga_double_copy_3(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
}
void e2ga_double_zero_4(double *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=0.0;
}
void e2ga_double_copy_4(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
}
void e2ga_double_zero_5(double *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=0.0;
}
void e2ga_double_copy_5(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
}
void e2ga_double_zero_6(double *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=0.0;
}
void e2ga_double_copy_6(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
	dst[5] = src[5];
}
void e2ga_double_zero_7(double *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=0.0;
}
void e2ga_double_copy_7(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
	dst[5] = src[5];
	dst[6] = src[6];
}
void e2ga_double_zero_8(double *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=0.0;
}
void e2ga_double_copy_8(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
	dst[5] = src[5];
	dst[6] = src[6];
	dst[7] = src[7];
}
void e2ga_double_zero_9(double *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=0.0;
}
void e2ga_double_copy_9(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
	dst[5] = src[5];
	dst[6] = src[6];
	dst[7] = src[7];
	dst[8] = src[8];
}
void e2ga_double_zero_10(double *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=0.0;
}
void e2ga_double_copy_10(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
	dst[5] = src[5];
	dst[6] = src[6];
	dst[7] = src[7];
	dst[8] = src[8];
	dst[9] = src[9];
}
void e2ga_double_zero_11(double *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=0.0;
}
void e2ga_double_copy_11(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
	dst[5] = src[5];
	dst[6] = src[6];
	dst[7] = src[7];
	dst[8] = src[8];
	dst[9] = src[9];
	dst[10] = src[10];
}
void e2ga_double_zero_12(double *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=0.0;
}
void e2ga_double_copy_12(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
	dst[5] = src[5];
	dst[6] = src[6];
	dst[7] = src[7];
	dst[8] = src[8];
	dst[9] = src[9];
	dst[10] = src[10];
	dst[11] = src[11];
}
void e2ga_double_zero_13(double *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=dst[12]=0.0;
}
void e2ga_double_copy_13(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
	dst[5] = src[5];
	dst[6] = src[6];
	dst[7] = src[7];
	dst[8] = src[8];
	dst[9] = src[9];
	dst[10] = src[10];
	dst[11] = src[11];
	dst[12] = src[12];
}
void e2ga_double_zero_14(double *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=dst[12]=dst[13]=0.0;
}
void e2ga_double_copy_14(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
	dst[5] = src[5];
	dst[6] = src[6];
	dst[7] = src[7];
	dst[8] = src[8];
	dst[9] = src[9];
	dst[10] = src[10];
	dst[11] = src[11];
	dst[12] = src[12];
	dst[13] = src[13];
}
void e2ga_double_zero_15(double *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=dst[12]=dst[13]=dst[14]=0.0;
}
void e2ga_double_copy_15(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
	dst[5] = src[5];
	dst[6] = src[6];
	dst[7] = src[7];
	dst[8] = src[8];
	dst[9] = src[9];
	dst[10] = src[10];
	dst[11] = src[11];
	dst[12] = src[12];
	dst[13] = src[13];
	dst[14] = src[14];
}
void e2ga_double_zero_16(double *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=dst[12]=dst[13]=dst[14]=dst[15]=0.0;
}
void e2ga_double_copy_16(double *dst, const double *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
	dst[5] = src[5];
	dst[6] = src[6];
	dst[7] = src[7];
	dst[8] = src[8];
	dst[9] = src[9];
	dst[10] = src[10];
	dst[11] = src[11];
	dst[12] = src[12];
	dst[13] = src[13];
	dst[14] = src[14];
	dst[15] = src[15];
}
/** Sets N doubles to zero */
void e2ga_double_zero_N(double *dst, int N) {
	int i = 0;
	while ((N-i) > 16) {
		e2ga_double_zero_16(dst + i);
		i += 16;
	}
	for (; i < N; i++)
		dst[i] = 0.0;
}
/** Copies N doubles from 'src' to 'dst' */
void e2ga_double_copy_N(double *dst, const double *src, int N) {
	int i = 0;
	while ((N-i) > 16) {
		e2ga_double_copy_16(dst + i, src + i);
		i += 16;
	}
	for (; i < N; i++)
		dst[i] = src[i];
}

/** This function is not for external use. It compresses arrays of floats for storage in multivectors. */
void compress(const double *c, double *cc, int *cgu, double epsilon, int gu) {
	int i, j, ia = 0, ib = 0, f, s;
	*cgu = 0;

	// for all grade parts...
	for (i = 0; i <= 3; i++) {
		// check if grade part has memory use:
		if (!(gu & (1 << i))) continue;

		// check if abs coordinates of grade part are all < epsilon
		s = e2ga_groupSize[i];
		j = ia + s;
		f = 0;
		for (; ia < j; ia++)
			if (fabs(c[ia]) > epsilon) {f = 1; break;}
		ia = j;
		if (f) {
			if ((ib+s) > 2) break; // when this 'break' happens, the input is not parity pure
			e2ga_double_copy_N(cc + ib, c + ia - s, s);
			ib += s;
			*cgu |= (1 << i);
		}
	}
}

/** This function is not for external use. It decompresses the coordinates stored in this */
void expand(const double *ptrs[3], const mv *src) {
	const double *c = src->c;
	
	if (src->gu & 1) {
		ptrs[0] =  c;
		c += 1;
	}
	else ptrs[0] = NULL;	
	if (src->gu & 2) {
		ptrs[1] =  c;
		c += 2;
	}
	else ptrs[1] = NULL;	
	if (src->gu & 4) {
		ptrs[2] =  c;
	}
	else ptrs[2] = NULL;	
}


void swapPointer(void **P1, void **P2)
{
	void *tmp = *P1;
	*P1 = *P2;
	*P2 = tmp;
}

/* 
These strings determine how the output of string() is formatted.
You can alter them at runtime using e2ga_setStringFormat().
*/
 
const char *e2ga_string_fp = "%2.2f"; 
const char *e2ga_string_start = ""; 
const char *e2ga_string_end = ""; 
const char *e2ga_string_mul = "*"; 
const char *e2ga_string_wedge = "^"; 
const char *e2ga_string_plus = " + "; 
const char *e2ga_string_minus = " - "; 

void e2ga_setStringFormat(const char *what, const char *format) {
 
	if (!strcmp(what, "fp")) 
		e2ga_string_fp = (format) ? format : "%2.2f"; 
	else if (!strcmp(what, "start")) 
		e2ga_string_start = (format) ? format : ""; 
	else if (!strcmp(what, "end")) 
		e2ga_string_end = (format) ? format : ""; 
	else if (!strcmp(what, "mul")) 
		e2ga_string_mul = (format) ? format : "*"; 
	else if (!strcmp(what, "wedge")) 
		e2ga_string_wedge = (format) ? format : "^"; 
	else if (!strcmp(what, "plus")) 
		e2ga_string_plus = (format) ? format : " + "; 
	else if (!strcmp(what, "minus")) 
		e2ga_string_minus = (format) ? format : " - ";
}

const char *toString_vector(const vector *V, char *str, int maxLength, const char *fp)
{
	mv tmp;
	vector_to_mv(&tmp,V);
	return toString_mv(&tmp, str, maxLength, fp);
}
const char *toString_rotor(const rotor *V, char *str, int maxLength, const char *fp)
{
	mv tmp;
	rotor_to_mv(&tmp,V);
	return toString_mv(&tmp, str, maxLength, fp);
}
const char *toString_e1_t(const e1_t *V, char *str, int maxLength, const char *fp)
{
	mv tmp;
	e1_t_to_mv(&tmp,V);
	return toString_mv(&tmp, str, maxLength, fp);
}
const char *toString_e2_t(const e2_t *V, char *str, int maxLength, const char *fp)
{
	mv tmp;
	e2_t_to_mv(&tmp,V);
	return toString_mv(&tmp, str, maxLength, fp);
}
const char *toString_I2_t(const I2_t *V, char *str, int maxLength, const char *fp)
{
	mv tmp;
	I2_t_to_mv(&tmp,V);
	return toString_mv(&tmp, str, maxLength, fp);
}

#ifdef WIN32
#define snprintf _snprintf
#pragma warning(disable:4996) /* quit your whining already */
#endif /* WIN32 */
const char *toString_mv(const mv *V, char *str, int maxLength, const char *fp) 
{
	int dummyArg = 0; // prevents compiler warning on some platforms
	int stdIdx = 0, l;
	char tmpBuf[256], tmpFloatBuf[256]; 
	int i, j, k = 0, bei, ia = 0, s = e2ga_mvSize[V->gu], p = 0, cnt = 0;

	// set up the floating point precision
	if (fp == NULL) fp = e2ga_string_fp;

	// start the string
	l = snprintf(tmpBuf, 256, "%s", e2ga_string_start);
	if (stdIdx + l <= maxLength) {
		strcpy(str + stdIdx, tmpBuf);
		stdIdx += l;
	}
	else {
		snprintf(str, maxLength, "toString_mv: buffer too small");
		return str;
	}

	// print all coordinates
	for (i = 0; i <= 3; i++) {
		if (V->gu & (1 << i)) {
			for (j = 0; j < e2ga_groupSize[i]; j++) {
				double coord = (double)e2ga_basisElementSignByIndex[ia] * V->c[k];
				/* goal: print [+|-]V->c[k][* basisVector1 ^ ... ^ basisVectorN] */			
				snprintf(tmpFloatBuf, 256, fp, fabs(coord));
				if (atof(tmpFloatBuf) != 0.0) {
					l = 0;

					// print [+|-]
					l += snprintf(tmpBuf + l, 256-l, "%s", (coord >= 0.0) 
						? (cnt ? e2ga_string_plus : "")
						: e2ga_string_minus);
						
					// print obj.m_c[k]
					l += snprintf(tmpBuf + l, 256-l, tmpFloatBuf, dummyArg);

					if (i) { // if not grade 0, print [* basisVector1 ^ ... ^ basisVectorN]
						l += snprintf(tmpBuf + l, 256-l, "%s", e2ga_string_mul);

						// print all basis vectors
						bei = 0;
						while (e2ga_basisElements[ia][bei] >= 0) {
							l += snprintf(tmpBuf + l, 256-l, "%s%s", (bei) ? e2ga_string_wedge : "", 
							 e2ga_basisVectorNames[e2ga_basisElements[ia][bei]]);
							 bei++;
						}
					}

					// copy all to 'str'
					if (stdIdx + l <= maxLength) {
						strcpy(str + stdIdx, tmpBuf);
						stdIdx += l;
					}
					else {
						snprintf(str, maxLength, "toString_mv: buffer too small");
						return str;
					}
					cnt++;
				}
				k++; ia++;
			}
		}
		else ia += e2ga_groupSize[i];
	}

    // if no coordinates printed: 0
	l = 0;
	if (cnt == 0) {
		l += snprintf(tmpBuf + l, 256-l, "0");
	}

    // end the string
	l += snprintf(tmpBuf + l, 256-l, "%s", e2ga_string_end);
	if (stdIdx + l <= maxLength) {
		strcpy(str + stdIdx, tmpBuf);
		stdIdx += l;
		return str;
	}
	else {
		snprintf(str, maxLength, "toString_mv: buffer too small\n");
		return str;
	}
}

// def SB:
/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 0 -> group 0)
 */
void gp_default_0_0_0(const double *A, const double *B, double *C);
/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 1 -> group 1)
 */
void gp_default_0_1_1(const double *A, const double *B, double *C);
/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 2 -> group 2)
 */
void gp_default_0_2_2(const double *A, const double *B, double *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 0 -> group 1)
 */
void gp_default_1_0_1(const double *A, const double *B, double *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 0)
 */
void gp_default_1_1_0(const double *A, const double *B, double *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 2)
 */
void gp_default_1_1_2(const double *A, const double *B, double *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 1)
 */
void gp_default_1_2_1(const double *A, const double *B, double *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 0 -> group 2)
 */
void gp_default_2_0_2(const double *A, const double *B, double *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 1)
 */
void gp_default_2_1_1(const double *A, const double *B, double *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 0)
 */
void gp_default_2_2_0(const double *A, const double *B, double *C);
/**
 * copies coordinates of group 0
 */
void copyGroup_0(const double *A, double *C);
/**
 * copies and multiplies (by s) coordinates of group 0
 */
void copyMul_0(const double *A, double *C, double s);
/**
 * copies and divides (by s) coordinates of group 0
 */
void copyDiv_0(const double *A, double *C, double s);
/**
 * adds coordinates of group 0 from variable A to C
 */
void add_0(const double *A, double *C);
/**
 * subtracts coordinates of group 0 in variable A from C
 */
void sub_0(const double *A, double *C);
/**
 * negate coordinates of group 0 of variable A
 */
void neg_0(const double *A, double *C);
/**
 * adds coordinates of group 0 of variables A and B
 */
void add2_0_0(const double *A, const double *B, double *C);
/**
 * subtracts coordinates of group 0 of variables A from B
 */
void sub2_0_0(const double *A, const double *B, double *C);
/**
 * performs coordinate-wise multiplication of coordinates of group 0 of variables A and B
 */
void hp_0_0(const double *A, const double *B, double *C);
/**
 * performs coordinate-wise division of coordinates of group 0 of variables A and B
 * (no checks for divide by zero are made)
 */
void ihp_0_0(const double *A, const double *B, double *C);
/**
 * check for equality up to eps of coordinates of group 0 of variables A and B
 */
int equals_0_0(const double *A, const double *B, double eps);
/**
 * checks if coordinates of group 0 of variable A are zero up to eps
 */
int zeroGroup_0(const double *A, double eps);
/**
 * copies coordinates of group 1
 */
void copyGroup_1(const double *A, double *C);
/**
 * copies and multiplies (by s) coordinates of group 1
 */
void copyMul_1(const double *A, double *C, double s);
/**
 * copies and divides (by s) coordinates of group 1
 */
void copyDiv_1(const double *A, double *C, double s);
/**
 * adds coordinates of group 1 from variable A to C
 */
void add_1(const double *A, double *C);
/**
 * subtracts coordinates of group 1 in variable A from C
 */
void sub_1(const double *A, double *C);
/**
 * negate coordinates of group 1 of variable A
 */
void neg_1(const double *A, double *C);
/**
 * adds coordinates of group 1 of variables A and B
 */
void add2_1_1(const double *A, const double *B, double *C);
/**
 * subtracts coordinates of group 1 of variables A from B
 */
void sub2_1_1(const double *A, const double *B, double *C);
/**
 * performs coordinate-wise multiplication of coordinates of group 1 of variables A and B
 */
void hp_1_1(const double *A, const double *B, double *C);
/**
 * performs coordinate-wise division of coordinates of group 1 of variables A and B
 * (no checks for divide by zero are made)
 */
void ihp_1_1(const double *A, const double *B, double *C);
/**
 * check for equality up to eps of coordinates of group 1 of variables A and B
 */
int equals_1_1(const double *A, const double *B, double eps);
/**
 * checks if coordinates of group 1 of variable A are zero up to eps
 */
int zeroGroup_1(const double *A, double eps);
/**
 * copies coordinates of group 2
 */
void copyGroup_2(const double *A, double *C);
/**
 * copies and multiplies (by s) coordinates of group 2
 */
void copyMul_2(const double *A, double *C, double s);
/**
 * copies and divides (by s) coordinates of group 2
 */
void copyDiv_2(const double *A, double *C, double s);
/**
 * adds coordinates of group 2 from variable A to C
 */
void add_2(const double *A, double *C);
/**
 * subtracts coordinates of group 2 in variable A from C
 */
void sub_2(const double *A, double *C);
/**
 * negate coordinates of group 2 of variable A
 */
void neg_2(const double *A, double *C);
/**
 * adds coordinates of group 2 of variables A and B
 */
void add2_2_2(const double *A, const double *B, double *C);
/**
 * subtracts coordinates of group 2 of variables A from B
 */
void sub2_2_2(const double *A, const double *B, double *C);
/**
 * performs coordinate-wise multiplication of coordinates of group 2 of variables A and B
 */
void hp_2_2(const double *A, const double *B, double *C);
/**
 * performs coordinate-wise division of coordinates of group 2 of variables A and B
 * (no checks for divide by zero are made)
 */
void ihp_2_2(const double *A, const double *B, double *C);
/**
 * check for equality up to eps of coordinates of group 2 of variables A and B
 */
int equals_2_2(const double *A, const double *B, double eps);
/**
 * checks if coordinates of group 2 of variable A are zero up to eps
 */
int zeroGroup_2(const double *A, double eps);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual_default_0_2(const double *A, double *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual_default_0_2(const double *A, double *C);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual_default_1_1(const double *A, double *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual_default_1_1(const double *A, double *C);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual_default_2_0(const double *A, double *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual_default_2_0(const double *A, double *C);
void gp_default_0_0_0(const double *A, const double *B, double *C) {
	C[0] += A[0]*B[0];
}
void gp_default_0_1_1(const double *A, const double *B, double *C) {
	C[0] += A[0]*B[0];
	C[1] += A[0]*B[1];
}
void gp_default_0_2_2(const double *A, const double *B, double *C) {
	gp_default_0_0_0(A, B, C);
}
void gp_default_1_0_1(const double *A, const double *B, double *C) {
	C[0] += A[0]*B[0];
	C[1] += A[1]*B[0];
}
void gp_default_1_1_0(const double *A, const double *B, double *C) {
	C[0] += (A[0]*B[0]+A[1]*B[1]);
}
void gp_default_1_1_2(const double *A, const double *B, double *C) {
	C[0] += (A[0]*B[1]-A[1]*B[0]);
}
void gp_default_1_2_1(const double *A, const double *B, double *C) {
	C[0] += -A[1]*B[0];
	C[1] += A[0]*B[0];
}
void gp_default_2_0_2(const double *A, const double *B, double *C) {
	gp_default_0_0_0(A, B, C);
}
void gp_default_2_1_1(const double *A, const double *B, double *C) {
	C[0] += A[0]*B[1];
	C[1] += -A[0]*B[0];
}
void gp_default_2_2_0(const double *A, const double *B, double *C) {
	C[0] += -A[0]*B[0];
}
void copyGroup_0(const double *A, double *C) {
	C[0] = A[0];
}
void copyMul_0(const double *A, double *C, double s) {
	C[0] = A[0]*s;
}
void copyDiv_0(const double *A, double *C, double s) {
	C[0] = A[0]/s;
}
void add_0(const double *A, double *C) {
	C[0] += A[0];
}
void sub_0(const double *A, double *C) {
	C[0] -= A[0];
}
void neg_0(const double *A, double *C) {
	C[0] = -A[0];
}
void add2_0_0(const double *A, const double *B, double *C) {
	C[0] = (A[0]+B[0]);
}
void sub2_0_0(const double *A, const double *B, double *C) {
	C[0] = (A[0]-B[0]);
}
void hp_0_0(const double *A, const double *B, double *C) {
	C[0] = A[0]*B[0];
}
void ihp_0_0(const double *A, const double *B, double *C) {
	C[0] = A[0]/((B[0]));
}
int equals_0_0(const double *A, const double *B, double eps) {
		if (((A[0] - B[0]) < -eps) || ((A[0] - B[0]) > eps)) return 0;
	return 1;
}
int zeroGroup_0(const double *A, double eps) {
		if ((A[0] < -eps) || (A[0] > eps)) return 0;
		return 1;
}
void copyGroup_1(const double *A, double *C) {
	C[0] = A[0];
	C[1] = A[1];
}
void copyMul_1(const double *A, double *C, double s) {
	C[0] = A[0]*s;
	C[1] = A[1]*s;
}
void copyDiv_1(const double *A, double *C, double s) {
	C[0] = A[0]/s;
	C[1] = A[1]/s;
}
void add_1(const double *A, double *C) {
	C[0] += A[0];
	C[1] += A[1];
}
void sub_1(const double *A, double *C) {
	C[0] -= A[0];
	C[1] -= A[1];
}
void neg_1(const double *A, double *C) {
	C[0] = -A[0];
	C[1] = -A[1];
}
void add2_1_1(const double *A, const double *B, double *C) {
	C[0] = (A[0]+B[0]);
	C[1] = (A[1]+B[1]);
}
void sub2_1_1(const double *A, const double *B, double *C) {
	C[0] = (A[0]-B[0]);
	C[1] = (A[1]-B[1]);
}
void hp_1_1(const double *A, const double *B, double *C) {
	C[0] = A[0]*B[0];
	C[1] = A[1]*B[1];
}
void ihp_1_1(const double *A, const double *B, double *C) {
	C[0] = A[0]/((B[0]));
	C[1] = A[1]/((B[1]));
}
int equals_1_1(const double *A, const double *B, double eps) {
		if (((A[0] - B[0]) < -eps) || ((A[0] - B[0]) > eps)) return 0;
		if (((A[1] - B[1]) < -eps) || ((A[1] - B[1]) > eps)) return 0;
	return 1;
}
int zeroGroup_1(const double *A, double eps) {
		if ((A[0] < -eps) || (A[0] > eps)) return 0;
		if ((A[1] < -eps) || (A[1] > eps)) return 0;
		return 1;
}
void copyGroup_2(const double *A, double *C) {
	copyGroup_0(A, C);
}
void copyMul_2(const double *A, double *C, double s) {
	copyMul_0(A, C, s);
}
void copyDiv_2(const double *A, double *C, double s) {
	copyDiv_0(A, C, s);
}
void add_2(const double *A, double *C) {
	add_0(A, C);
}
void sub_2(const double *A, double *C) {
	sub_0(A, C);
}
void neg_2(const double *A, double *C) {
	neg_0(A, C);
}
void add2_2_2(const double *A, const double *B, double *C) {
	add2_0_0(A, B, C);
}
void sub2_2_2(const double *A, const double *B, double *C) {
	sub2_0_0(A, B, C);
}
void hp_2_2(const double *A, const double *B, double *C) {
	hp_0_0(A, B, C);
}
void ihp_2_2(const double *A, const double *B, double *C) {
	ihp_0_0(A, B, C);
}
int equals_2_2(const double *A, const double *B, double eps) {
	return equals_0_0(A, B, eps);
}
int zeroGroup_2(const double *A, double eps) {
	return zeroGroup_0(A, eps);
}
void dual_default_0_2(const double *A, double *C) {
	C[0] = -A[0];
}
void undual_default_0_2(const double *A, double *C) {
	C[0] = A[0];
}
void dual_default_1_1(const double *A, double *C) {
	C[0] = A[1];
	C[1] = -A[0];
}
void undual_default_1_1(const double *A, double *C) {
	C[0] = -A[1];
	C[1] = A[0];
}
void dual_default_2_0(const double *A, double *C) {
	undual_default_0_2(A, C);
}
void undual_default_2_0(const double *A, double *C) {
	dual_default_0_2(A, C);
}
void mv_setZero(mv *M) {
	M->gu = 0;
}
void mv_setScalar(mv *M, double val) {
	M->gu = 1;
	M->c[0] = val;
}
void mv_setArray(mv *M, int gu, const double *arr) {
	M->gu = gu;
	e2ga_double_copy_N(M->c, arr, e2ga_mvSize[gu]);

}
void mv_copy(mv *dst, const mv *src) {
	int i;
	dst->gu = src->gu;
	for (i = 0; i < e2ga_mvSize[src->gu]; i++)
		dst->c[i] = (double)src->c[i];
}
void mv_to_vector(vector *dst, const mv *src) {
	const double *ptr = src->c;

	if (src->gu & 1) {
		ptr += 1;
	}
	if (src->gu & 2) {
		dst->c[0] = ptr[0];
		dst->c[1] = ptr[1];
		ptr += 2;
	}
	else {
		dst->c[0] = 0.0;
		dst->c[1] = 0.0;
	}
}
void mv_to_rotor(rotor *dst, const mv *src) {
	const double *ptr = src->c;

	if (src->gu & 1) {
		dst->c[0] = ptr[0];
		ptr += 1;
	}
	else {
		dst->c[0] = 0.0;
	}
	if (src->gu & 2) {
		ptr += 2;
	}
	if (src->gu & 4) {
		dst->c[1] = ptr[0];
		ptr += 1;
	}
	else {
		dst->c[1] = 0.0;
	}
}
void mv_to_e1_t(e1_t *dst, const mv *src) {
	const double *ptr = src->c;

	if (src->gu & 1) {
		ptr += 1;
	}
	if (src->gu & 2) {
		ptr += 2;
	}
	else {
	}
}
void mv_to_e2_t(e2_t *dst, const mv *src) {
	const double *ptr = src->c;

	if (src->gu & 1) {
		ptr += 1;
	}
	if (src->gu & 2) {
		ptr += 2;
	}
	else {
	}
}
void mv_to_I2_t(I2_t *dst, const mv *src) {
	const double *ptr = src->c;

	if (src->gu & 1) {
		ptr += 1;
	}
	if (src->gu & 2) {
		ptr += 2;
	}
	if (src->gu & 4) {
		ptr += 1;
	}
	else {
	}
}
void vector_to_mv(mv *dst, const vector *src) {
	double *ptr = dst->c;
	dst->gu = 2;
	ptr[0] = src->c[0];
	ptr[1] = src->c[1];
	ptr += 2;
}
void rotor_to_mv(mv *dst, const rotor *src) {
	double *ptr = dst->c;
	dst->gu = 5;
	ptr[0] = src->c[0];
	ptr += 1;
	ptr[0] = src->c[1];
	ptr += 1;
}
void e1_t_to_mv(mv *dst, const e1_t *src) {
	double *ptr = dst->c;
	dst->gu = 2;
	ptr[0] = 1.0;
	ptr[1] = 0.0;
	ptr += 2;
}
void e2_t_to_mv(mv *dst, const e2_t *src) {
	double *ptr = dst->c;
	dst->gu = 2;
	ptr[0] = 0.0;
	ptr[1] = 1.0;
	ptr += 2;
}
void I2_t_to_mv(mv *dst, const I2_t *src) {
	double *ptr = dst->c;
	dst->gu = 4;
	ptr[0] = 1.0;
	ptr += 1;
}

void mv_reserveGroup_0(mv *A) {
	int groupUsageBelow, groupUsageAbove, newGroupUsage, newGroupUsageBelowNextGroup;
	int i;
	double *dst, *src;
	if ((A->gu & 1) == 0) {

		groupUsageBelow = A->gu & 0;
		groupUsageAbove = A->gu ^ groupUsageBelow;
		newGroupUsage = A->gu | 1;
		newGroupUsageBelowNextGroup = newGroupUsage & 1;

		dst = A->c + e2ga_mvSize[newGroupUsageBelowNextGroup];
		src = A->c + e2ga_mvSize[groupUsageBelow];
		for (i = e2ga_mvSize[groupUsageAbove]-1; i >= 0; i--) // work from end to start of array to avoid overwriting (dst is always beyond src)
			dst[i] = src[i];
		e2ga_double_zero_1(A->c);

		A->gu = newGroupUsage;
	}
}

void mv_reserveGroup_1(mv *A) {
	int groupUsageBelow, groupUsageAbove, newGroupUsage, newGroupUsageBelowNextGroup;
	int i;
	double *dst, *src;
	if ((A->gu & 2) == 0) {

		groupUsageBelow = A->gu & 1;
		groupUsageAbove = A->gu ^ groupUsageBelow;
		newGroupUsage = A->gu | 2;
		newGroupUsageBelowNextGroup = newGroupUsage & 3;

		dst = A->c + e2ga_mvSize[newGroupUsageBelowNextGroup];
		src = A->c + e2ga_mvSize[groupUsageBelow];
		for (i = e2ga_mvSize[groupUsageAbove]-1; i >= 0; i--) // work from end to start of array to avoid overwriting (dst is always beyond src)
			dst[i] = src[i];
		e2ga_double_zero_2(A->c);

		A->gu = newGroupUsage;
	}
}

void mv_reserveGroup_2(mv *A) {
	int groupUsageBelow, groupUsageAbove, newGroupUsage, newGroupUsageBelowNextGroup;
	if ((A->gu & 4) == 0) {

		groupUsageBelow = A->gu & 3;
		groupUsageAbove = A->gu ^ groupUsageBelow;
		newGroupUsage = A->gu | 4;
		newGroupUsageBelowNextGroup = newGroupUsage & 7;

		e2ga_double_zero_1(A->c);

		A->gu = newGroupUsage;
	}
}

double mv_scalar(const mv *A) {
	return (A->gu & 1) ? A->c[e2ga_mvSize[A->gu & 0] + 0] : 0.0;
}
double mv_double(const mv *A) {
	return mv_scalar(A);
}

void mv_set_scalar(mv *A, double scalar_coord) {
	mv_reserveGroup_0(A);
	A->c[e2ga_mvSize[A->gu & 0] + 0] = scalar_coord;
}

double mv_e1(const mv *A) {
	return (A->gu & 2) ? A->c[e2ga_mvSize[A->gu & 1] + 0] : 0.0;
}

void mv_set_e1(mv *A, double e1_coord) {
	mv_reserveGroup_1(A);
	A->c[e2ga_mvSize[A->gu & 1] + 0] = e1_coord;
}

double mv_e2(const mv *A) {
	return (A->gu & 2) ? A->c[e2ga_mvSize[A->gu & 1] + 1] : 0.0;
}

void mv_set_e2(mv *A, double e2_coord) {
	mv_reserveGroup_1(A);
	A->c[e2ga_mvSize[A->gu & 1] + 1] = e2_coord;
}

double mv_e1_e2(const mv *A) {
	return (A->gu & 4) ? A->c[e2ga_mvSize[A->gu & 3] + 0] : 0.0;
}

void mv_set_e1_e2(mv *A, double e1_e2_coord) {
	mv_reserveGroup_2(A);
	A->c[e2ga_mvSize[A->gu & 3] + 0] = e1_e2_coord;
}

double mv_largestCoordinate(const mv *x) {
	double maxValue = 0.0;
	int nbC = e2ga_mvSize[x->gu], i;
	for (i = 0; i < nbC; i++)
		if (fabs(x->c[i]) > maxValue) maxValue = fabs(x->c[i]);
	return maxValue;
}

double mv_largestBasisBlade(const mv *x, unsigned int *bm) {
	int nc = e2ga_mvSize[x->gu];
	double maxC = -1.0, C;

	int idx = 0;
	int group = 0;
	int i = 0, j;
	*bm = 0;

	while (i < nc) {
		if (x->gu & (1 << group)) {
			for (j = 0; j < e2ga_groupSize[group]; j++) {
				C = fabs(x->c[i]);
				if (C > maxC) {
					maxC = C;
					*bm = e2ga_basisElementBitmapByIndex[idx];
				}
				idx++;
				i++;
			}
		}
		else idx += e2ga_groupSize[group];
		group++;
	}

	return maxC;
} // end of mv::largestBasisBlade()



void vector_setZero(vector *_dst)
{
	_dst->c[0] = _dst->c[1] = 0.0;

}
void rotor_setZero(rotor *_dst)
{
	_dst->c[0] = _dst->c[1] = 0.0;

}

void rotor_setScalar(rotor *_dst, const double scalarVal)
{
	_dst->c[0] = scalarVal;
	_dst->c[1] = 0.0;

}

void vector_set(vector *_dst, const double _e1, const double _e2)
{
	_dst->c[0] = _e1;
	_dst->c[1] = _e2;

}
void rotor_set(rotor *_dst, const double _scalar, const double _e1_e2)
{
	_dst->c[0] = _scalar;
	_dst->c[1] = _e1_e2;

}

void vector_setArray(vector *_dst, const double *A)
{
	_dst->c[0] = A[0];
	_dst->c[1] = A[1];

}
void rotor_setArray(rotor *_dst, const double *A)
{
	_dst->c[0] = A[0];
	_dst->c[1] = A[1];

}

void vector_copy(vector *_dst, const vector *a)
{
	_dst->c[0] = a->c[0];
	_dst->c[1] = a->c[1];

}
void rotor_copy(rotor *_dst, const rotor *a)
{
	_dst->c[0] = a->c[0];
	_dst->c[1] = a->c[1];

}


double vector_largestCoordinate(const vector *x) {
	double maxValue = fabs(x->c[0]);
	if (fabs(x->c[1]) > maxValue) maxValue = fabs(x->c[1]);
	return maxValue;
}
double rotor_largestCoordinate(const rotor *x) {
	double maxValue = fabs(x->c[0]);
	if (fabs(x->c[1]) > maxValue) maxValue = fabs(x->c[1]);
	return maxValue;
}
double e1_t_largestCoordinate(const e1_t *x) {
	double maxValue = 1.0;
	return maxValue;
}
double e2_t_largestCoordinate(const e2_t *x) {
	double maxValue = 1.0;
	return maxValue;
}
double I2_t_largestCoordinate(const I2_t *x) {
	double maxValue = 1.0;
	return maxValue;
}

double vector_double(const vector *x) {
	return 0.0;
}
double rotor_double(const rotor *x) {
	return x->c[0];
}
double e1_t_double(const e1_t *x) {
	return 0.0;
}
double e2_t_double(const e2_t *x) {
	return 0.0;
}
double I2_t_double(const I2_t *x) {
	return 0.0;
}
void add_mv_mv(mv *_dst, const mv *a, const mv *b)
{
	int aidx = 0, bidx = 0, cidx = 0;
	_dst->gu = a->gu | b->gu;
	
	if (a->gu & 1) {
		if (b->gu & 1) {
			add2_0_0(a->c + aidx, b->c + bidx, _dst->c + cidx);
			bidx += 1;
		}
		else copyGroup_0(a->c + aidx, _dst->c + cidx);
		aidx += 1;
		cidx += 1;
	}
	else if (b->gu & 1) {
		copyGroup_0(b->c + bidx, _dst->c + cidx);
		bidx += 1;
		cidx += 1;
	}
	
	if (a->gu & 2) {
		if (b->gu & 2) {
			add2_1_1(a->c + aidx, b->c + bidx, _dst->c + cidx);
			bidx += 2;
		}
		else copyGroup_1(a->c + aidx, _dst->c + cidx);
		aidx += 2;
		cidx += 2;
	}
	else if (b->gu & 2) {
		copyGroup_1(b->c + bidx, _dst->c + cidx);
		bidx += 2;
		cidx += 2;
	}
	
	if (a->gu & 4) {
		if (b->gu & 4) {
			add2_2_2(a->c + aidx, b->c + bidx, _dst->c + cidx);
		}
		else copyGroup_2(a->c + aidx, _dst->c + cidx);
		cidx += 1;
	}
	else if (b->gu & 4) {
		copyGroup_2(b->c + bidx, _dst->c + cidx);
		cidx += 1;
	}
}
void add_vector_vector(vector *_dst, const vector *a, const vector *b)
{
	_dst->c[0] = (a->c[0]+b->c[0]);
	_dst->c[1] = (a->c[1]+b->c[1]);

}
void gp_mv_mv(mv *_dst, const mv *a, const mv *b)
{
	double c[4];
	const double* _a[3];
	const double* _b[3];
	expand(_a, a);
	expand(_b, b);
	e2ga_double_zero_4(c);
	if (a->gu & 1) {
		if (b->gu & 1) {
			gp_default_0_0_0(_a[0], _b[0], c + 0);
		}
		if (b->gu & 2) {
			gp_default_0_1_1(_a[0], _b[1], c + 1);
		}
		if (b->gu & 4) {
			gp_default_0_2_2(_a[0], _b[2], c + 3);
		}
	}
	if (a->gu & 2) {
		if (b->gu & 1) {
			gp_default_1_0_1(_a[1], _b[0], c + 1);
		}
		if (b->gu & 2) {
			gp_default_1_1_0(_a[1], _b[1], c + 0);
			gp_default_1_1_2(_a[1], _b[1], c + 3);
		}
		if (b->gu & 4) {
			gp_default_1_2_1(_a[1], _b[2], c + 1);
		}
	}
	if (a->gu & 4) {
		if (b->gu & 1) {
			gp_default_2_0_2(_a[2], _b[0], c + 3);
		}
		if (b->gu & 2) {
			gp_default_2_1_1(_a[2], _b[1], c + 1);
		}
		if (b->gu & 4) {
			gp_default_2_2_0(_a[2], _b[2], c + 0);
		}
	}
	compress(c, _dst->c, &(_dst->gu), 0.0, 7);
}
void gp_vector_vector(rotor *_dst, const vector *a, const vector *b)
{
	_dst->c[0] = (a->c[0]*b->c[0]+a->c[1]*b->c[1]);
	_dst->c[1] = (a->c[0]*b->c[1]-a->c[1]*b->c[0]);

}
void applyUnitVersor_rotor_vector(vector *_dst, const rotor *a, const vector *b)
{
	_dst->c[0] = (a->c[0]*a->c[0]*b->c[0]+2.0*a->c[0]*a->c[1]*b->c[1]-a->c[1]*a->c[1]*b->c[0]);
	_dst->c[1] = (a->c[0]*a->c[0]*b->c[1]+-2.0*a->c[0]*a->c[1]*b->c[0]-a->c[1]*a->c[1]*b->c[1]);

}
void extractGrade_mv(mv *_dst, const mv *a, int groupBitmap)
{
	int aidx = 0, cidx = 0;
	_dst->gu = a->gu & groupBitmap;
	
	if (a->gu & 1) {
		if (groupBitmap & 1) {
			copyGroup_0(a->c + aidx, _dst->c + cidx);
			cidx += 1;
		}
		aidx += 1;
	}
	
	if (a->gu & 2) {
		if (groupBitmap & 2) {
			copyGroup_1(a->c + aidx, _dst->c + cidx);
			cidx += 2;
		}
		aidx += 2;
	}
	
	if (a->gu & 4) {
		if (groupBitmap & 4) {
			copyGroup_2(a->c + aidx, _dst->c + cidx);
		}
	}
}
void reverse_mv(mv *_dst, const mv *a)
{
	int idx = 0;
	_dst->gu = a->gu;
	
	if (a->gu & 1) {
		copyGroup_0(a->c + idx, _dst->c + idx);
		idx += 1;
	}
	
	if (a->gu & 2) {
		copyGroup_1(a->c + idx, _dst->c + idx);
		idx += 2;
	}
	
	if (a->gu & 4) {
		neg_2(a->c + idx, _dst->c + idx);
	}
}
