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
#include "c3ga.h"

const int c3ga_spaceDim = 5;
const int c3ga_nbGroups = 6;
const int c3ga_metricEuclidean = 0;
const char *c3ga_basisVectorNames[5] = {
	"no", "e1", "e2", "e3", "ni"
};
const int c3ga_grades[] = {GRADE_0, GRADE_1, GRADE_2, GRADE_3, GRADE_4, GRADE_5, 0, 0, 0, 0, 0, 0};
const int c3ga_groups[] = {GROUP_0, GROUP_1, GROUP_2, GROUP_3, GROUP_4, GROUP_5};
const int c3ga_groupSize[6] = {
	1, 5, 10, 10, 5, 1
};
const int c3ga_mvSize[64] = {
	0, 1, 5, 6, 10, 11, 15, 16, 10, 11, 15, 16, 20, 21, 25, 26, 5, 6, 10, 11, 
	15, 16, 20, 21, 15, 16, 20, 21, 25, 26, 30, 31, 1, 2, 6, 7, 11, 12, 16, 17, 
	11, 12, 16, 17, 21, 22, 26, 27, 6, 7, 11, 12, 16, 17, 21, 22, 16, 17, 21, 22, 
	26, 27, 31, 32};
const int c3ga_basisElements[32][6] = {
	{-1},
	{0, -1},
	{1, -1},
	{2, -1},
	{3, -1},
	{4, -1},
	{0, 1, -1},
	{0, 2, -1},
	{1, 2, -1},
	{0, 3, -1},
	{1, 3, -1},
	{2, 3, -1},
	{0, 4, -1},
	{1, 4, -1},
	{2, 4, -1},
	{3, 4, -1},
	{0, 1, 2, -1},
	{0, 1, 3, -1},
	{0, 2, 3, -1},
	{1, 2, 3, -1},
	{0, 1, 4, -1},
	{0, 2, 4, -1},
	{1, 2, 4, -1},
	{0, 3, 4, -1},
	{1, 3, 4, -1},
	{2, 3, 4, -1},
	{0, 1, 2, 3, -1},
	{0, 1, 2, 4, -1},
	{0, 1, 3, 4, -1},
	{0, 2, 3, 4, -1},
	{1, 2, 3, 4, -1},
	{0, 1, 2, 3, 4, -1}
};
const double c3ga_basisElementSignByIndex[32] =
	{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
const double c3ga_basisElementSignByBitmap[32] =
	{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
const int c3ga_basisElementIndexByBitmap[32] =
	{0, 1, 2, 6, 3, 7, 8, 16, 4, 9, 10, 17, 11, 18, 19, 26, 5, 12, 13, 20, 14, 21, 22, 27, 15, 23, 24, 28, 25, 29, 30, 31};
const int c3ga_basisElementBitmapByIndex[32] =
	{0, 1, 2, 4, 8, 16, 3, 5, 6, 9, 10, 12, 17, 18, 20, 24, 7, 11, 13, 14, 19, 21, 22, 25, 26, 28, 15, 23, 27, 29, 30, 31};
const int c3ga_basisElementGradeByBitmap[32] =
	{0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4, 1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5};
const int c3ga_basisElementGroupByBitmap[32] =
	{0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4, 1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5};

const char *g_c3gaTypenames[] = 
{
	"normalizedPoint",
	"flatPoint",
	"line",
	"dualLine",
	"plane",
	"no_t",
	"e1_t",
	"e2_t",
	"e3_t",
	"ni_t"
};
no_t no = {0};
e1_t e1 = {0};
e2_t e2 = {0};
e3_t e3 = {0};
ni_t ni = {0};


void c3ga_float_zero_1(float *dst) {
	dst[0]=0.0f;
}
void c3ga_float_copy_1(float *dst, const float *src) {
	dst[0] = src[0];
}
void c3ga_float_zero_2(float *dst) {
	dst[0]=dst[1]=0.0f;
}
void c3ga_float_copy_2(float *dst, const float *src) {
	dst[0] = src[0];
	dst[1] = src[1];
}
void c3ga_float_zero_3(float *dst) {
	dst[0]=dst[1]=dst[2]=0.0f;
}
void c3ga_float_copy_3(float *dst, const float *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
}
void c3ga_float_zero_4(float *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=0.0f;
}
void c3ga_float_copy_4(float *dst, const float *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
}
void c3ga_float_zero_5(float *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=0.0f;
}
void c3ga_float_copy_5(float *dst, const float *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
}
void c3ga_float_zero_6(float *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=0.0f;
}
void c3ga_float_copy_6(float *dst, const float *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
	dst[5] = src[5];
}
void c3ga_float_zero_7(float *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=0.0f;
}
void c3ga_float_copy_7(float *dst, const float *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
	dst[5] = src[5];
	dst[6] = src[6];
}
void c3ga_float_zero_8(float *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=0.0f;
}
void c3ga_float_copy_8(float *dst, const float *src) {
	dst[0] = src[0];
	dst[1] = src[1];
	dst[2] = src[2];
	dst[3] = src[3];
	dst[4] = src[4];
	dst[5] = src[5];
	dst[6] = src[6];
	dst[7] = src[7];
}
void c3ga_float_zero_9(float *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=0.0f;
}
void c3ga_float_copy_9(float *dst, const float *src) {
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
void c3ga_float_zero_10(float *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=0.0f;
}
void c3ga_float_copy_10(float *dst, const float *src) {
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
void c3ga_float_zero_11(float *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=0.0f;
}
void c3ga_float_copy_11(float *dst, const float *src) {
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
void c3ga_float_zero_12(float *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=0.0f;
}
void c3ga_float_copy_12(float *dst, const float *src) {
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
void c3ga_float_zero_13(float *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=dst[12]=0.0f;
}
void c3ga_float_copy_13(float *dst, const float *src) {
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
void c3ga_float_zero_14(float *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=dst[12]=dst[13]=0.0f;
}
void c3ga_float_copy_14(float *dst, const float *src) {
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
void c3ga_float_zero_15(float *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=dst[12]=dst[13]=dst[14]=0.0f;
}
void c3ga_float_copy_15(float *dst, const float *src) {
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
void c3ga_float_zero_16(float *dst) {
	dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=dst[12]=dst[13]=dst[14]=dst[15]=0.0f;
}
void c3ga_float_copy_16(float *dst, const float *src) {
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
/** Sets N floats to zero */
void c3ga_float_zero_N(float *dst, int N) {
	int i = 0;
	while ((N-i) > 16) {
		c3ga_float_zero_16(dst + i);
		i += 16;
	}
	for (; i < N; i++)
		dst[i] = 0.0f;
}
/** Copies N floats from 'src' to 'dst' */
void c3ga_float_copy_N(float *dst, const float *src, int N) {
	int i = 0;
	while ((N-i) > 16) {
		c3ga_float_copy_16(dst + i, src + i);
		i += 16;
	}
	for (; i < N; i++)
		dst[i] = src[i];
}

/** This function is not for external use. It compresses arrays of floats for storage in multivectors. */
void compress(const float *c, float *cc, int *cgu, float epsilon, int gu) {
	int i, j, ia = 0, ib = 0, f, s;
	*cgu = 0;

	/* for all grade parts... */
	for (i = 0; i <= 6; i++) {
		/* check if grade part has memory use: */
		if (!(gu & (1 << i))) continue;

		/* check if abs coordinates of grade part are all < epsilon */
		s = c3ga_groupSize[i];
		j = ia + s;
		f = 0;
		for (; ia < j; ia++)
			if (fabsf(c[ia]) > epsilon) {f = 1; break;}
		ia = j;
		if (f) {
						c3ga_float_copy_N(cc + ib, c + ia - s, s);
			ib += s;
			*cgu |= (1 << i);
		}
	}
}

/** This function is not for external use. It decompresses the coordinates stored in this */
void expand(const float *ptrs[6], const mv *src) {
	const float *c = src->c;
	
	if (src->gu & 1) {
		ptrs[0] =  c;
		c += 1;
	}
	else ptrs[0] = NULL;	
	if (src->gu & 2) {
		ptrs[1] =  c;
		c += 5;
	}
	else ptrs[1] = NULL;	
	if (src->gu & 4) {
		ptrs[2] =  c;
		c += 10;
	}
	else ptrs[2] = NULL;	
	if (src->gu & 8) {
		ptrs[3] =  c;
		c += 10;
	}
	else ptrs[3] = NULL;	
	if (src->gu & 16) {
		ptrs[4] =  c;
		c += 5;
	}
	else ptrs[4] = NULL;	
	if (src->gu & 32) {
		ptrs[5] =  c;
	}
	else ptrs[5] = NULL;	
}


void swapPointer(void **P1, void **P2)
{
	void *tmp = *P1;
	*P1 = *P2;
	*P2 = tmp;
}

/* 
These strings determine how the output of string() is formatted.
You can alter them at runtime using c3ga_setStringFormat().
*/
 
const char *c3ga_string_fp = "%2.2f"; 
const char *c3ga_string_start = ""; 
const char *c3ga_string_end = ""; 
const char *c3ga_string_mul = "*"; 
const char *c3ga_string_wedge = "^"; 
const char *c3ga_string_plus = " + "; 
const char *c3ga_string_minus = " - "; 

void c3ga_setStringFormat(const char *what, const char *format) {
 
	if (!strcmp(what, "fp")) 
		c3ga_string_fp = (format) ? format : "%2.2f"; 
	else if (!strcmp(what, "start")) 
		c3ga_string_start = (format) ? format : ""; 
	else if (!strcmp(what, "end")) 
		c3ga_string_end = (format) ? format : ""; 
	else if (!strcmp(what, "mul")) 
		c3ga_string_mul = (format) ? format : "*"; 
	else if (!strcmp(what, "wedge")) 
		c3ga_string_wedge = (format) ? format : "^"; 
	else if (!strcmp(what, "plus")) 
		c3ga_string_plus = (format) ? format : " + "; 
	else if (!strcmp(what, "minus")) 
		c3ga_string_minus = (format) ? format : " - ";
}

const char *toString_normalizedPoint(const normalizedPoint *V, char *str, int maxLength, const char *fp)
{
	mv tmp;
	normalizedPoint_to_mv(&tmp,V);
	return toString_mv(&tmp, str, maxLength, fp);
}
const char *toString_flatPoint(const flatPoint *V, char *str, int maxLength, const char *fp)
{
	mv tmp;
	flatPoint_to_mv(&tmp,V);
	return toString_mv(&tmp, str, maxLength, fp);
}
const char *toString_line(const line *V, char *str, int maxLength, const char *fp)
{
	mv tmp;
	line_to_mv(&tmp,V);
	return toString_mv(&tmp, str, maxLength, fp);
}
const char *toString_dualLine(const dualLine *V, char *str, int maxLength, const char *fp)
{
	mv tmp;
	dualLine_to_mv(&tmp,V);
	return toString_mv(&tmp, str, maxLength, fp);
}
const char *toString_plane(const plane *V, char *str, int maxLength, const char *fp)
{
	mv tmp;
	plane_to_mv(&tmp,V);
	return toString_mv(&tmp, str, maxLength, fp);
}
const char *toString_no_t(const no_t *V, char *str, int maxLength, const char *fp)
{
	mv tmp;
	no_t_to_mv(&tmp,V);
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
const char *toString_e3_t(const e3_t *V, char *str, int maxLength, const char *fp)
{
	mv tmp;
	e3_t_to_mv(&tmp,V);
	return toString_mv(&tmp, str, maxLength, fp);
}
const char *toString_ni_t(const ni_t *V, char *str, int maxLength, const char *fp)
{
	mv tmp;
	ni_t_to_mv(&tmp,V);
	return toString_mv(&tmp, str, maxLength, fp);
}

#ifdef WIN32
#define snprintf _snprintf
#pragma warning(disable:4996) /* quit your whining already */
#endif /* WIN32 */
const char *toString_mv(const mv *V, char *str, int maxLength, const char *fp) 
{
	int dummyArg = 0; /* prevents compiler warning on some platforms */
	int stdIdx = 0, l;
	char tmpBuf[256], tmpFloatBuf[256]; 
	int i, j, k = 0, bei, ia = 0, s = c3ga_mvSize[V->gu], p = 0, cnt = 0;

	/* set up the floating point precision */
	if (fp == NULL) fp = c3ga_string_fp;

	/* start the string */
	l = snprintf(tmpBuf, 256, "%s", c3ga_string_start);
	if (stdIdx + l <= maxLength) {
		strcpy(str + stdIdx, tmpBuf);
		stdIdx += l;
	}
	else {
		snprintf(str, maxLength, "toString_mv: buffer too small");
		return str;
	}

	/* print all coordinates */
	for (i = 0; i <= 6; i++) {
		if (V->gu & (1 << i)) {
			for (j = 0; j < c3ga_groupSize[i]; j++) {
				float coord = (float)c3ga_basisElementSignByIndex[ia] * V->c[k];
				/* goal: print [+|-]V->c[k][* basisVector1 ^ ... ^ basisVectorN] */			
				snprintf(tmpFloatBuf, 256, fp, fabs(coord));
				if (atof(tmpFloatBuf) != 0.0) {
					l = 0;

					/* print [+|-] */
					l += snprintf(tmpBuf + l, 256-l, "%s", (coord >= 0.0) 
						? (cnt ? c3ga_string_plus : "")
						: c3ga_string_minus);
						
					/* print obj.m_c[k] */
					l += snprintf(tmpBuf + l, 256-l, tmpFloatBuf, dummyArg);

					if (i) { /* if not grade 0, print [* basisVector1 ^ ... ^ basisVectorN] */
						l += snprintf(tmpBuf + l, 256-l, "%s", c3ga_string_mul);

						/* print all basis vectors */
						bei = 0;
						while (c3ga_basisElements[ia][bei] >= 0) {
							l += snprintf(tmpBuf + l, 256-l, "%s%s", (bei) ? c3ga_string_wedge : "", 
							 c3ga_basisVectorNames[c3ga_basisElements[ia][bei]]);
							 bei++;
						}
					}

					/* copy all to 'str' */
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
		else ia += c3ga_groupSize[i];
	}

    /* if no coordinates printed: 0 */
	l = 0;
	if (cnt == 0) {
		l += snprintf(tmpBuf + l, 256-l, "0");
	}

    /* end the string */
	l += snprintf(tmpBuf + l, 256-l, "%s", c3ga_string_end);
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

/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 0 -> group 0)
 */
void gp_default_0_0_0(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 1 -> group 1)
 */
void gp_default_0_1_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 2 -> group 2)
 */
void gp_default_0_2_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 3 -> group 3)
 */
void gp_default_0_3_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 4 -> group 4)
 */
void gp_default_0_4_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 5 -> group 5)
 */
void gp_default_0_5_5(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 0 -> group 1)
 */
void gp_default_1_0_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 0)
 */
void gp_default_1_1_0(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 2)
 */
void gp_default_1_1_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 1)
 */
void gp_default_1_2_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 3)
 */
void gp_default_1_2_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 3 -> group 2)
 */
void gp_default_1_3_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 3 -> group 4)
 */
void gp_default_1_3_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 4 -> group 3)
 */
void gp_default_1_4_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 4 -> group 5)
 */
void gp_default_1_4_5(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 5 -> group 4)
 */
void gp_default_1_5_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 0 -> group 2)
 */
void gp_default_2_0_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 1)
 */
void gp_default_2_1_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 3)
 */
void gp_default_2_1_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 0)
 */
void gp_default_2_2_0(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 2)
 */
void gp_default_2_2_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 4)
 */
void gp_default_2_2_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 1)
 */
void gp_default_2_3_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 3)
 */
void gp_default_2_3_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 5)
 */
void gp_default_2_3_5(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 2)
 */
void gp_default_2_4_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 4)
 */
void gp_default_2_4_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 5 -> group 3)
 */
void gp_default_2_5_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 0 -> group 3)
 */
void gp_default_3_0_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 1 -> group 2)
 */
void gp_default_3_1_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 1 -> group 4)
 */
void gp_default_3_1_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 1)
 */
void gp_default_3_2_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 3)
 */
void gp_default_3_2_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 5)
 */
void gp_default_3_2_5(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 0)
 */
void gp_default_3_3_0(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 2)
 */
void gp_default_3_3_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 4)
 */
void gp_default_3_3_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 1)
 */
void gp_default_3_4_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 3)
 */
void gp_default_3_4_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 5 -> group 2)
 */
void gp_default_3_5_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 0 -> group 4)
 */
void gp_default_4_0_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 3)
 */
void gp_default_4_1_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 5)
 */
void gp_default_4_1_5(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 2)
 */
void gp_default_4_2_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 4)
 */
void gp_default_4_2_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 1)
 */
void gp_default_4_3_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 3)
 */
void gp_default_4_3_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 0)
 */
void gp_default_4_4_0(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 2)
 */
void gp_default_4_4_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 5 -> group 1)
 */
void gp_default_4_5_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 5  x  group 0 -> group 5)
 */
void gp_default_5_0_5(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 5  x  group 1 -> group 4)
 */
void gp_default_5_1_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 5  x  group 2 -> group 3)
 */
void gp_default_5_2_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 5  x  group 3 -> group 2)
 */
void gp_default_5_3_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 5  x  group 4 -> group 1)
 */
void gp_default_5_4_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 5  x  group 5 -> group 0)
 */
void gp_default_5_5_0(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 0 -> group 0)
 */
void gp__internal_euclidean_metric__0_0_0(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 1 -> group 1)
 */
void gp__internal_euclidean_metric__0_1_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 2 -> group 2)
 */
void gp__internal_euclidean_metric__0_2_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 3 -> group 3)
 */
void gp__internal_euclidean_metric__0_3_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 4 -> group 4)
 */
void gp__internal_euclidean_metric__0_4_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 0  x  group 5 -> group 5)
 */
void gp__internal_euclidean_metric__0_5_5(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 0 -> group 1)
 */
void gp__internal_euclidean_metric__1_0_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 0)
 */
void gp__internal_euclidean_metric__1_1_0(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 2)
 */
void gp__internal_euclidean_metric__1_1_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 1)
 */
void gp__internal_euclidean_metric__1_2_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 3)
 */
void gp__internal_euclidean_metric__1_2_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 3 -> group 2)
 */
void gp__internal_euclidean_metric__1_3_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 3 -> group 4)
 */
void gp__internal_euclidean_metric__1_3_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 4 -> group 3)
 */
void gp__internal_euclidean_metric__1_4_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 4 -> group 5)
 */
void gp__internal_euclidean_metric__1_4_5(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 1  x  group 5 -> group 4)
 */
void gp__internal_euclidean_metric__1_5_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 0 -> group 2)
 */
void gp__internal_euclidean_metric__2_0_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 1)
 */
void gp__internal_euclidean_metric__2_1_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 3)
 */
void gp__internal_euclidean_metric__2_1_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 0)
 */
void gp__internal_euclidean_metric__2_2_0(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 2)
 */
void gp__internal_euclidean_metric__2_2_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 4)
 */
void gp__internal_euclidean_metric__2_2_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 1)
 */
void gp__internal_euclidean_metric__2_3_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 3)
 */
void gp__internal_euclidean_metric__2_3_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 5)
 */
void gp__internal_euclidean_metric__2_3_5(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 2)
 */
void gp__internal_euclidean_metric__2_4_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 4)
 */
void gp__internal_euclidean_metric__2_4_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 2  x  group 5 -> group 3)
 */
void gp__internal_euclidean_metric__2_5_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 0 -> group 3)
 */
void gp__internal_euclidean_metric__3_0_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 1 -> group 2)
 */
void gp__internal_euclidean_metric__3_1_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 1 -> group 4)
 */
void gp__internal_euclidean_metric__3_1_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 1)
 */
void gp__internal_euclidean_metric__3_2_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 3)
 */
void gp__internal_euclidean_metric__3_2_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 5)
 */
void gp__internal_euclidean_metric__3_2_5(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 0)
 */
void gp__internal_euclidean_metric__3_3_0(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 2)
 */
void gp__internal_euclidean_metric__3_3_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 4)
 */
void gp__internal_euclidean_metric__3_3_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 1)
 */
void gp__internal_euclidean_metric__3_4_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 3)
 */
void gp__internal_euclidean_metric__3_4_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 3  x  group 5 -> group 2)
 */
void gp__internal_euclidean_metric__3_5_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 0 -> group 4)
 */
void gp__internal_euclidean_metric__4_0_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 3)
 */
void gp__internal_euclidean_metric__4_1_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 5)
 */
void gp__internal_euclidean_metric__4_1_5(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 2)
 */
void gp__internal_euclidean_metric__4_2_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 4)
 */
void gp__internal_euclidean_metric__4_2_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 1)
 */
void gp__internal_euclidean_metric__4_3_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 3)
 */
void gp__internal_euclidean_metric__4_3_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 0)
 */
void gp__internal_euclidean_metric__4_4_0(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 2)
 */
void gp__internal_euclidean_metric__4_4_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 4  x  group 5 -> group 1)
 */
void gp__internal_euclidean_metric__4_5_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 5  x  group 0 -> group 5)
 */
void gp__internal_euclidean_metric__5_0_5(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 5  x  group 1 -> group 4)
 */
void gp__internal_euclidean_metric__5_1_4(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 5  x  group 2 -> group 3)
 */
void gp__internal_euclidean_metric__5_2_3(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 5  x  group 3 -> group 2)
 */
void gp__internal_euclidean_metric__5_3_2(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 5  x  group 4 -> group 1)
 */
void gp__internal_euclidean_metric__5_4_1(const float *A, const float *B, float *C);
/**
 * Computes the partial geometric product of two multivectors (group 5  x  group 5 -> group 0)
 */
void gp__internal_euclidean_metric__5_5_0(const float *A, const float *B, float *C);
/**
 * copies coordinates of group 0
 */
void copyGroup_0(const float *A, float *C);
/**
 * copies and multiplies (by s) coordinates of group 0
 */
void copyMul_0(const float *A, float *C, float s);
/**
 * copies and divides (by s) coordinates of group 0
 */
void copyDiv_0(const float *A, float *C, float s);
/**
 * adds coordinates of group 0 from variable A to C
 */
void add_0(const float *A, float *C);
/**
 * subtracts coordinates of group 0 in variable A from C
 */
void sub_0(const float *A, float *C);
/**
 * negate coordinates of group 0 of variable A
 */
void neg_0(const float *A, float *C);
/**
 * adds coordinates of group 0 of variables A and B
 */
void add2_0_0(const float *A, const float *B, float *C);
/**
 * subtracts coordinates of group 0 of variables A from B
 */
void sub2_0_0(const float *A, const float *B, float *C);
/**
 * performs coordinate-wise multiplication of coordinates of group 0 of variables A and B
 */
void hp_0_0(const float *A, const float *B, float *C);
/**
 * performs coordinate-wise division of coordinates of group 0 of variables A and B
 * (no checks for divide by zero are made)
 */
void ihp_0_0(const float *A, const float *B, float *C);
/**
 * check for equality up to eps of coordinates of group 0 of variables A and B
 */
int equals_0_0(const float *A, const float *B, float eps);
/**
 * checks if coordinates of group 0 of variable A are zero up to eps
 */
int zeroGroup_0(const float *A, float eps);
/**
 * copies coordinates of group 1
 */
void copyGroup_1(const float *A, float *C);
/**
 * copies and multiplies (by s) coordinates of group 1
 */
void copyMul_1(const float *A, float *C, float s);
/**
 * copies and divides (by s) coordinates of group 1
 */
void copyDiv_1(const float *A, float *C, float s);
/**
 * adds coordinates of group 1 from variable A to C
 */
void add_1(const float *A, float *C);
/**
 * subtracts coordinates of group 1 in variable A from C
 */
void sub_1(const float *A, float *C);
/**
 * negate coordinates of group 1 of variable A
 */
void neg_1(const float *A, float *C);
/**
 * adds coordinates of group 1 of variables A and B
 */
void add2_1_1(const float *A, const float *B, float *C);
/**
 * subtracts coordinates of group 1 of variables A from B
 */
void sub2_1_1(const float *A, const float *B, float *C);
/**
 * performs coordinate-wise multiplication of coordinates of group 1 of variables A and B
 */
void hp_1_1(const float *A, const float *B, float *C);
/**
 * performs coordinate-wise division of coordinates of group 1 of variables A and B
 * (no checks for divide by zero are made)
 */
void ihp_1_1(const float *A, const float *B, float *C);
/**
 * check for equality up to eps of coordinates of group 1 of variables A and B
 */
int equals_1_1(const float *A, const float *B, float eps);
/**
 * checks if coordinates of group 1 of variable A are zero up to eps
 */
int zeroGroup_1(const float *A, float eps);
/**
 * copies coordinates of group 2
 */
void copyGroup_2(const float *A, float *C);
/**
 * copies and multiplies (by s) coordinates of group 2
 */
void copyMul_2(const float *A, float *C, float s);
/**
 * copies and divides (by s) coordinates of group 2
 */
void copyDiv_2(const float *A, float *C, float s);
/**
 * adds coordinates of group 2 from variable A to C
 */
void add_2(const float *A, float *C);
/**
 * subtracts coordinates of group 2 in variable A from C
 */
void sub_2(const float *A, float *C);
/**
 * negate coordinates of group 2 of variable A
 */
void neg_2(const float *A, float *C);
/**
 * adds coordinates of group 2 of variables A and B
 */
void add2_2_2(const float *A, const float *B, float *C);
/**
 * subtracts coordinates of group 2 of variables A from B
 */
void sub2_2_2(const float *A, const float *B, float *C);
/**
 * performs coordinate-wise multiplication of coordinates of group 2 of variables A and B
 */
void hp_2_2(const float *A, const float *B, float *C);
/**
 * performs coordinate-wise division of coordinates of group 2 of variables A and B
 * (no checks for divide by zero are made)
 */
void ihp_2_2(const float *A, const float *B, float *C);
/**
 * check for equality up to eps of coordinates of group 2 of variables A and B
 */
int equals_2_2(const float *A, const float *B, float eps);
/**
 * checks if coordinates of group 2 of variable A are zero up to eps
 */
int zeroGroup_2(const float *A, float eps);
/**
 * copies coordinates of group 3
 */
void copyGroup_3(const float *A, float *C);
/**
 * copies and multiplies (by s) coordinates of group 3
 */
void copyMul_3(const float *A, float *C, float s);
/**
 * copies and divides (by s) coordinates of group 3
 */
void copyDiv_3(const float *A, float *C, float s);
/**
 * adds coordinates of group 3 from variable A to C
 */
void add_3(const float *A, float *C);
/**
 * subtracts coordinates of group 3 in variable A from C
 */
void sub_3(const float *A, float *C);
/**
 * negate coordinates of group 3 of variable A
 */
void neg_3(const float *A, float *C);
/**
 * adds coordinates of group 3 of variables A and B
 */
void add2_3_3(const float *A, const float *B, float *C);
/**
 * subtracts coordinates of group 3 of variables A from B
 */
void sub2_3_3(const float *A, const float *B, float *C);
/**
 * performs coordinate-wise multiplication of coordinates of group 3 of variables A and B
 */
void hp_3_3(const float *A, const float *B, float *C);
/**
 * performs coordinate-wise division of coordinates of group 3 of variables A and B
 * (no checks for divide by zero are made)
 */
void ihp_3_3(const float *A, const float *B, float *C);
/**
 * check for equality up to eps of coordinates of group 3 of variables A and B
 */
int equals_3_3(const float *A, const float *B, float eps);
/**
 * checks if coordinates of group 3 of variable A are zero up to eps
 */
int zeroGroup_3(const float *A, float eps);
/**
 * copies coordinates of group 4
 */
void copyGroup_4(const float *A, float *C);
/**
 * copies and multiplies (by s) coordinates of group 4
 */
void copyMul_4(const float *A, float *C, float s);
/**
 * copies and divides (by s) coordinates of group 4
 */
void copyDiv_4(const float *A, float *C, float s);
/**
 * adds coordinates of group 4 from variable A to C
 */
void add_4(const float *A, float *C);
/**
 * subtracts coordinates of group 4 in variable A from C
 */
void sub_4(const float *A, float *C);
/**
 * negate coordinates of group 4 of variable A
 */
void neg_4(const float *A, float *C);
/**
 * adds coordinates of group 4 of variables A and B
 */
void add2_4_4(const float *A, const float *B, float *C);
/**
 * subtracts coordinates of group 4 of variables A from B
 */
void sub2_4_4(const float *A, const float *B, float *C);
/**
 * performs coordinate-wise multiplication of coordinates of group 4 of variables A and B
 */
void hp_4_4(const float *A, const float *B, float *C);
/**
 * performs coordinate-wise division of coordinates of group 4 of variables A and B
 * (no checks for divide by zero are made)
 */
void ihp_4_4(const float *A, const float *B, float *C);
/**
 * check for equality up to eps of coordinates of group 4 of variables A and B
 */
int equals_4_4(const float *A, const float *B, float eps);
/**
 * checks if coordinates of group 4 of variable A are zero up to eps
 */
int zeroGroup_4(const float *A, float eps);
/**
 * copies coordinates of group 5
 */
void copyGroup_5(const float *A, float *C);
/**
 * copies and multiplies (by s) coordinates of group 5
 */
void copyMul_5(const float *A, float *C, float s);
/**
 * copies and divides (by s) coordinates of group 5
 */
void copyDiv_5(const float *A, float *C, float s);
/**
 * adds coordinates of group 5 from variable A to C
 */
void add_5(const float *A, float *C);
/**
 * subtracts coordinates of group 5 in variable A from C
 */
void sub_5(const float *A, float *C);
/**
 * negate coordinates of group 5 of variable A
 */
void neg_5(const float *A, float *C);
/**
 * adds coordinates of group 5 of variables A and B
 */
void add2_5_5(const float *A, const float *B, float *C);
/**
 * subtracts coordinates of group 5 of variables A from B
 */
void sub2_5_5(const float *A, const float *B, float *C);
/**
 * performs coordinate-wise multiplication of coordinates of group 5 of variables A and B
 */
void hp_5_5(const float *A, const float *B, float *C);
/**
 * performs coordinate-wise division of coordinates of group 5 of variables A and B
 * (no checks for divide by zero are made)
 */
void ihp_5_5(const float *A, const float *B, float *C);
/**
 * check for equality up to eps of coordinates of group 5 of variables A and B
 */
int equals_5_5(const float *A, const float *B, float eps);
/**
 * checks if coordinates of group 5 of variable A are zero up to eps
 */
int zeroGroup_5(const float *A, float eps);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual_default_0_5(const float *A, float *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual_default_0_5(const float *A, float *C);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual_default_1_4(const float *A, float *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual_default_1_4(const float *A, float *C);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual_default_2_3(const float *A, float *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual_default_2_3(const float *A, float *C);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual_default_3_2(const float *A, float *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual_default_3_2(const float *A, float *C);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual_default_4_1(const float *A, float *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual_default_4_1(const float *A, float *C);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual_default_5_0(const float *A, float *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual_default_5_0(const float *A, float *C);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual__internal_euclidean_metric__0_5(const float *A, float *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual__internal_euclidean_metric__0_5(const float *A, float *C);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual__internal_euclidean_metric__1_4(const float *A, float *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual__internal_euclidean_metric__1_4(const float *A, float *C);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual__internal_euclidean_metric__2_3(const float *A, float *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual__internal_euclidean_metric__2_3(const float *A, float *C);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual__internal_euclidean_metric__3_2(const float *A, float *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual__internal_euclidean_metric__3_2(const float *A, float *C);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual__internal_euclidean_metric__4_1(const float *A, float *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual__internal_euclidean_metric__4_1(const float *A, float *C);
/**
 * Computes the partial dual (w.r.t. full space) of a multivector.
 */
void dual__internal_euclidean_metric__5_0(const float *A, float *C);
/**
 * Computes the partial undual (w.r.t. full space) of a multivector.
 */
void undual__internal_euclidean_metric__5_0(const float *A, float *C);
void gp_default_0_0_0(const float *A, const float *B, float *C) {
	C[0] += A[0]*B[0];
}
void gp_default_0_1_1(const float *A, const float *B, float *C) {
	C[0] += A[0]*B[0];
	C[1] += A[0]*B[1];
	C[2] += A[0]*B[2];
	C[3] += A[0]*B[3];
	C[4] += A[0]*B[4];
}
void gp_default_0_2_2(const float *A, const float *B, float *C) {
	C[0] += A[0]*B[0];
	C[1] += A[0]*B[1];
	C[2] += A[0]*B[2];
	C[3] += A[0]*B[3];
	C[4] += A[0]*B[4];
	C[5] += A[0]*B[5];
	C[6] += A[0]*B[6];
	C[7] += A[0]*B[7];
	C[8] += A[0]*B[8];
	C[9] += A[0]*B[9];
}
void gp_default_0_3_3(const float *A, const float *B, float *C) {
	gp_default_0_2_2(A, B, C);
}
void gp_default_0_4_4(const float *A, const float *B, float *C) {
	gp_default_0_1_1(A, B, C);
}
void gp_default_0_5_5(const float *A, const float *B, float *C) {
	gp_default_0_0_0(A, B, C);
}
void gp_default_1_0_1(const float *A, const float *B, float *C) {
	C[0] += A[0]*B[0];
	C[1] += A[1]*B[0];
	C[2] += A[2]*B[0];
	C[3] += A[3]*B[0];
	C[4] += A[4]*B[0];
}
void gp_default_1_1_0(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[4]+A[1]*B[1]+A[2]*B[2]+A[3]*B[3]-A[4]*B[0]);
}
void gp_default_1_1_2(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[1]-A[1]*B[0]);
	C[1] += (A[0]*B[2]-A[2]*B[0]);
	C[2] += (A[1]*B[2]-A[2]*B[1]);
	C[3] += (A[0]*B[3]-A[3]*B[0]);
	C[4] += (A[1]*B[3]-A[3]*B[1]);
	C[5] += (A[2]*B[3]-A[3]*B[2]);
	C[6] += (A[0]*B[4]-A[4]*B[0]);
	C[7] += (A[1]*B[4]-A[4]*B[1]);
	C[8] += (A[2]*B[4]-A[4]*B[2]);
	C[9] += (A[3]*B[4]-A[4]*B[3]);
}
void gp_default_1_2_1(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[6]-A[1]*B[0]-A[2]*B[1]-A[3]*B[3]);
	C[1] += (A[0]*B[7]-A[2]*B[2]-A[3]*B[4]-A[4]*B[0]);
	C[2] += (A[0]*B[8]+A[1]*B[2]-A[3]*B[5]-A[4]*B[1]);
	C[3] += (A[0]*B[9]+A[1]*B[4]+A[2]*B[5]-A[4]*B[3]);
	C[4] += (A[1]*B[7]+A[2]*B[8]+A[3]*B[9]-A[4]*B[6]);
}
void gp_default_1_2_3(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[2]-A[1]*B[1]+A[2]*B[0]);
	C[1] += (A[0]*B[4]-A[1]*B[3]+A[3]*B[0]);
	C[2] += (A[0]*B[5]-A[2]*B[3]+A[3]*B[1]);
	C[3] += (A[1]*B[5]-A[2]*B[4]+A[3]*B[2]);
	C[4] += (A[0]*B[7]-A[1]*B[6]+A[4]*B[0]);
	C[5] += (A[0]*B[8]-A[2]*B[6]+A[4]*B[1]);
	C[6] += (A[1]*B[8]-A[2]*B[7]+A[4]*B[2]);
	C[7] += (A[0]*B[9]-A[3]*B[6]+A[4]*B[3]);
	C[8] += (A[1]*B[9]-A[3]*B[7]+A[4]*B[4]);
	C[9] += (A[2]*B[9]-A[3]*B[8]+A[4]*B[5]);
}
void gp_default_1_3_2(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[4]+A[2]*B[0]+A[3]*B[1]);
	C[1] += (-A[0]*B[5]-A[1]*B[0]+A[3]*B[2]);
	C[2] += (-A[0]*B[6]+A[3]*B[3]-A[4]*B[0]);
	C[3] += (-A[0]*B[7]-A[1]*B[1]-A[2]*B[2]);
	C[4] += (-A[0]*B[8]-A[2]*B[3]-A[4]*B[1]);
	C[5] += (-A[0]*B[9]+A[1]*B[3]-A[4]*B[2]);
	C[6] += (-A[1]*B[4]-A[2]*B[5]-A[3]*B[7]);
	C[7] += (-A[2]*B[6]-A[3]*B[8]-A[4]*B[4]);
	C[8] += (A[1]*B[6]-A[3]*B[9]-A[4]*B[5]);
	C[9] += (A[1]*B[8]+A[2]*B[9]-A[4]*B[7]);
}
void gp_default_1_3_4(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[3]-A[1]*B[2]+A[2]*B[1]-A[3]*B[0]);
	C[1] += (A[0]*B[6]-A[1]*B[5]+A[2]*B[4]-A[4]*B[0]);
	C[2] += (A[0]*B[8]-A[1]*B[7]+A[3]*B[4]-A[4]*B[1]);
	C[3] += (A[0]*B[9]-A[2]*B[7]+A[3]*B[5]-A[4]*B[2]);
	C[4] += (A[1]*B[9]-A[2]*B[8]+A[3]*B[6]-A[4]*B[3]);
}
void gp_default_1_4_3(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[1]-A[3]*B[0]);
	C[1] += (A[0]*B[2]+A[2]*B[0]);
	C[2] += (A[0]*B[3]-A[1]*B[0]);
	C[3] += (A[0]*B[4]-A[4]*B[0]);
	C[4] += (A[2]*B[1]+A[3]*B[2]);
	C[5] += (-A[1]*B[1]+A[3]*B[3]);
	C[6] += (A[3]*B[4]-A[4]*B[1]);
	C[7] += (-A[1]*B[2]-A[2]*B[3]);
	C[8] += (-A[2]*B[4]-A[4]*B[2]);
	C[9] += (A[1]*B[4]-A[4]*B[3]);
}
void gp_default_1_4_5(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[4]-A[1]*B[3]+A[2]*B[2]-A[3]*B[1]+A[4]*B[0]);
}
void gp_default_1_5_4(const float *A, const float *B, float *C) {
	C[0] += -A[0]*B[0];
	C[1] += -A[3]*B[0];
	C[2] += A[2]*B[0];
	C[3] += -A[1]*B[0];
	C[4] += -A[4]*B[0];
}
void gp_default_2_0_2(const float *A, const float *B, float *C) {
	C[0] += A[0]*B[0];
	C[1] += A[1]*B[0];
	C[2] += A[2]*B[0];
	C[3] += A[3]*B[0];
	C[4] += A[4]*B[0];
	C[5] += A[5]*B[0];
	C[6] += A[6]*B[0];
	C[7] += A[7]*B[0];
	C[8] += A[8]*B[0];
	C[9] += A[9]*B[0];
}
void gp_default_2_1_1(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[1]+A[1]*B[2]+A[3]*B[3]-A[6]*B[0]);
	C[1] += (A[0]*B[4]+A[2]*B[2]+A[4]*B[3]-A[7]*B[0]);
	C[2] += (A[1]*B[4]-A[2]*B[1]+A[5]*B[3]-A[8]*B[0]);
	C[3] += (A[3]*B[4]-A[4]*B[1]-A[5]*B[2]-A[9]*B[0]);
	C[4] += (A[6]*B[4]-A[7]*B[1]-A[8]*B[2]-A[9]*B[3]);
}
void gp_default_2_1_3(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[2]-A[1]*B[1]+A[2]*B[0]);
	C[1] += (A[0]*B[3]-A[3]*B[1]+A[4]*B[0]);
	C[2] += (A[1]*B[3]-A[3]*B[2]+A[5]*B[0]);
	C[3] += (A[2]*B[3]-A[4]*B[2]+A[5]*B[1]);
	C[4] += (A[0]*B[4]-A[6]*B[1]+A[7]*B[0]);
	C[5] += (A[1]*B[4]-A[6]*B[2]+A[8]*B[0]);
	C[6] += (A[2]*B[4]-A[7]*B[2]+A[8]*B[1]);
	C[7] += (A[3]*B[4]-A[6]*B[3]+A[9]*B[0]);
	C[8] += (A[4]*B[4]-A[7]*B[3]+A[9]*B[1]);
	C[9] += (A[5]*B[4]-A[8]*B[3]+A[9]*B[2]);
}
void gp_default_2_2_0(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[7]-A[1]*B[8]-A[2]*B[2]-A[3]*B[9]-A[4]*B[4]-A[5]*B[5]+A[6]*B[6]-A[7]*B[0]-A[8]*B[1]-A[9]*B[3]);
}
void gp_default_2_2_2(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[6]-A[1]*B[2]+A[2]*B[1]-A[3]*B[4]+A[4]*B[3]-A[6]*B[0]);
	C[1] += (A[0]*B[2]+A[1]*B[6]-A[2]*B[0]-A[3]*B[5]+A[5]*B[3]-A[6]*B[1]);
	C[2] += (-A[0]*B[8]+A[1]*B[7]-A[4]*B[5]+A[5]*B[4]-A[7]*B[1]+A[8]*B[0]);
	C[3] += (A[0]*B[4]+A[1]*B[5]+A[3]*B[6]-A[4]*B[0]-A[5]*B[1]-A[6]*B[3]);
	C[4] += (-A[0]*B[9]+A[2]*B[5]+A[3]*B[7]-A[5]*B[2]-A[7]*B[3]+A[9]*B[0]);
	C[5] += (-A[1]*B[9]-A[2]*B[4]+A[3]*B[8]+A[4]*B[2]-A[8]*B[3]+A[9]*B[1]);
	C[6] += (A[0]*B[7]+A[1]*B[8]+A[3]*B[9]-A[7]*B[0]-A[8]*B[1]-A[9]*B[3]);
	C[7] += (A[2]*B[8]+A[4]*B[9]+A[6]*B[7]-A[7]*B[6]-A[8]*B[2]-A[9]*B[4]);
	C[8] += (-A[2]*B[7]+A[5]*B[9]+A[6]*B[8]+A[7]*B[2]-A[8]*B[6]-A[9]*B[5]);
	C[9] += (-A[4]*B[7]-A[5]*B[8]+A[6]*B[9]+A[7]*B[4]+A[8]*B[5]-A[9]*B[6]);
}
void gp_default_2_2_4(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[5]-A[1]*B[4]+A[2]*B[3]+A[3]*B[2]-A[4]*B[1]+A[5]*B[0]);
	C[1] += (A[0]*B[8]-A[1]*B[7]+A[2]*B[6]+A[6]*B[2]-A[7]*B[1]+A[8]*B[0]);
	C[2] += (A[0]*B[9]-A[3]*B[7]+A[4]*B[6]+A[6]*B[4]-A[7]*B[3]+A[9]*B[0]);
	C[3] += (A[1]*B[9]-A[3]*B[8]+A[5]*B[6]+A[6]*B[5]-A[8]*B[3]+A[9]*B[1]);
	C[4] += (A[2]*B[9]-A[4]*B[8]+A[5]*B[7]+A[7]*B[5]-A[8]*B[4]+A[9]*B[2]);
}
void gp_default_2_3_1(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[4]-A[1]*B[5]-A[2]*B[0]-A[3]*B[7]-A[4]*B[1]-A[5]*B[2]);
	C[1] += (-A[1]*B[6]-A[3]*B[8]-A[5]*B[3]-A[6]*B[4]+A[8]*B[0]+A[9]*B[1]);
	C[2] += (A[0]*B[6]-A[3]*B[9]+A[4]*B[3]-A[6]*B[5]-A[7]*B[0]+A[9]*B[2]);
	C[3] += (A[0]*B[8]+A[1]*B[9]-A[2]*B[3]-A[6]*B[7]-A[7]*B[1]-A[8]*B[2]);
	C[4] += (-A[2]*B[6]-A[4]*B[8]-A[5]*B[9]-A[7]*B[4]-A[8]*B[5]-A[9]*B[7]);
}
void gp_default_2_3_3(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[5]+A[1]*B[4]+A[3]*B[3]-A[4]*B[2]+A[5]*B[1]-A[6]*B[0]);
	C[1] += (-A[0]*B[7]-A[1]*B[3]+A[2]*B[2]+A[3]*B[4]-A[5]*B[0]-A[6]*B[1]);
	C[2] += (A[0]*B[3]-A[1]*B[7]-A[2]*B[1]+A[3]*B[5]+A[4]*B[0]-A[6]*B[2]);
	C[3] += (A[0]*B[9]-A[1]*B[8]+A[3]*B[6]-A[7]*B[2]+A[8]*B[1]-A[9]*B[0]);
	C[4] += (-A[1]*B[6]+A[2]*B[5]-A[3]*B[8]+A[4]*B[7]-A[8]*B[0]-A[9]*B[1]);
	C[5] += (A[0]*B[6]-A[2]*B[4]-A[3]*B[9]+A[5]*B[7]+A[7]*B[0]-A[9]*B[2]);
	C[6] += (-A[4]*B[9]+A[5]*B[8]+A[6]*B[6]-A[7]*B[5]+A[8]*B[4]-A[9]*B[3]);
	C[7] += (A[0]*B[8]+A[1]*B[9]-A[4]*B[4]-A[5]*B[5]+A[7]*B[1]+A[8]*B[2]);
	C[8] += (A[2]*B[9]-A[5]*B[6]+A[6]*B[8]-A[7]*B[7]+A[8]*B[3]+A[9]*B[4]);
	C[9] += (-A[2]*B[8]+A[4]*B[6]+A[6]*B[9]-A[7]*B[3]-A[8]*B[7]+A[9]*B[5]);
}
void gp_default_2_3_5(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[9]-A[1]*B[8]+A[2]*B[7]+A[3]*B[6]-A[4]*B[5]+A[5]*B[4]-A[6]*B[3]+A[7]*B[2]-A[8]*B[1]+A[9]*B[0]);
}
void gp_default_2_4_2(const float *A, const float *B, float *C) {
	C[0] += (-A[1]*B[1]-A[3]*B[2]-A[5]*B[0]);
	C[1] += (A[0]*B[1]-A[3]*B[3]+A[4]*B[0]);
	C[2] += (-A[3]*B[4]+A[6]*B[1]-A[9]*B[0]);
	C[3] += (A[0]*B[2]+A[1]*B[3]-A[2]*B[0]);
	C[4] += (A[1]*B[4]+A[6]*B[2]+A[8]*B[0]);
	C[5] += (-A[0]*B[4]+A[6]*B[3]-A[7]*B[0]);
	C[6] += (-A[2]*B[1]-A[4]*B[2]-A[5]*B[3]);
	C[7] += (-A[5]*B[4]+A[8]*B[1]+A[9]*B[2]);
	C[8] += (A[4]*B[4]-A[7]*B[1]+A[9]*B[3]);
	C[9] += (-A[2]*B[4]-A[7]*B[2]-A[8]*B[3]);
}
void gp_default_2_4_4(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[3]-A[1]*B[2]+A[3]*B[1]-A[6]*B[0]);
	C[1] += (A[3]*B[4]-A[4]*B[3]+A[5]*B[2]-A[9]*B[0]);
	C[2] += (-A[1]*B[4]+A[2]*B[3]-A[5]*B[1]+A[8]*B[0]);
	C[3] += (A[0]*B[4]-A[2]*B[2]+A[4]*B[1]-A[7]*B[0]);
	C[4] += (A[6]*B[4]-A[7]*B[3]+A[8]*B[2]-A[9]*B[1]);
}
void gp_default_2_5_3(const float *A, const float *B, float *C) {
	C[0] += -A[3]*B[0];
	C[1] += A[1]*B[0];
	C[2] += -A[0]*B[0];
	C[3] += -A[6]*B[0];
	C[4] += -A[5]*B[0];
	C[5] += A[4]*B[0];
	C[6] += -A[9]*B[0];
	C[7] += -A[2]*B[0];
	C[8] += A[8]*B[0];
	C[9] += -A[7]*B[0];
}
void gp_default_3_0_3(const float *A, const float *B, float *C) {
	gp_default_2_0_2(A, B, C);
}
void gp_default_3_1_2(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[2]+A[1]*B[3]-A[4]*B[0]);
	C[1] += (-A[0]*B[1]+A[2]*B[3]-A[5]*B[0]);
	C[2] += (-A[0]*B[4]+A[3]*B[3]-A[6]*B[0]);
	C[3] += (-A[1]*B[1]-A[2]*B[2]-A[7]*B[0]);
	C[4] += (-A[1]*B[4]-A[3]*B[2]-A[8]*B[0]);
	C[5] += (-A[2]*B[4]+A[3]*B[1]-A[9]*B[0]);
	C[6] += (-A[4]*B[1]-A[5]*B[2]-A[7]*B[3]);
	C[7] += (-A[4]*B[4]-A[6]*B[2]-A[8]*B[3]);
	C[8] += (-A[5]*B[4]+A[6]*B[1]-A[9]*B[3]);
	C[9] += (-A[7]*B[4]+A[8]*B[1]+A[9]*B[2]);
}
void gp_default_3_1_4(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[3]-A[1]*B[2]+A[2]*B[1]-A[3]*B[0]);
	C[1] += (A[0]*B[4]-A[4]*B[2]+A[5]*B[1]-A[6]*B[0]);
	C[2] += (A[1]*B[4]-A[4]*B[3]+A[7]*B[1]-A[8]*B[0]);
	C[3] += (A[2]*B[4]-A[5]*B[3]+A[7]*B[2]-A[9]*B[0]);
	C[4] += (A[3]*B[4]-A[6]*B[3]+A[8]*B[2]-A[9]*B[1]);
}
void gp_default_3_2_1(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[2]-A[1]*B[4]-A[2]*B[5]-A[4]*B[0]-A[5]*B[1]-A[7]*B[3]);
	C[1] += (A[0]*B[8]+A[1]*B[9]-A[3]*B[5]-A[4]*B[6]-A[6]*B[1]-A[8]*B[3]);
	C[2] += (-A[0]*B[7]+A[2]*B[9]+A[3]*B[4]-A[5]*B[6]+A[6]*B[0]-A[9]*B[3]);
	C[3] += (-A[1]*B[7]-A[2]*B[8]-A[3]*B[2]-A[7]*B[6]+A[8]*B[0]+A[9]*B[1]);
	C[4] += (-A[4]*B[7]-A[5]*B[8]-A[6]*B[2]-A[7]*B[9]-A[8]*B[4]-A[9]*B[5]);
}
void gp_default_3_2_3(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[6]-A[1]*B[5]+A[2]*B[4]-A[3]*B[3]-A[4]*B[1]+A[5]*B[0]);
	C[1] += (A[0]*B[5]+A[1]*B[6]-A[2]*B[2]+A[3]*B[1]-A[4]*B[3]+A[7]*B[0]);
	C[2] += (-A[0]*B[4]+A[1]*B[2]+A[2]*B[6]-A[3]*B[0]-A[5]*B[3]+A[7]*B[1]);
	C[3] += (A[0]*B[9]-A[1]*B[8]+A[2]*B[7]-A[6]*B[3]+A[8]*B[1]-A[9]*B[0]);
	C[4] += (A[0]*B[8]+A[1]*B[9]-A[5]*B[2]+A[6]*B[1]-A[7]*B[4]+A[8]*B[3]);
	C[5] += (-A[0]*B[7]+A[2]*B[9]+A[4]*B[2]-A[6]*B[0]-A[7]*B[5]+A[9]*B[3]);
	C[6] += (A[3]*B[9]-A[4]*B[8]+A[5]*B[7]-A[6]*B[6]-A[8]*B[5]+A[9]*B[4]);
	C[7] += (-A[1]*B[7]-A[2]*B[8]+A[4]*B[4]+A[5]*B[5]-A[8]*B[0]-A[9]*B[1]);
	C[8] += (-A[3]*B[8]-A[4]*B[9]+A[6]*B[5]+A[7]*B[7]-A[8]*B[6]-A[9]*B[2]);
	C[9] += (A[3]*B[7]-A[5]*B[9]-A[6]*B[4]+A[7]*B[8]+A[8]*B[2]-A[9]*B[6]);
}
void gp_default_3_2_5(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[9]-A[1]*B[8]+A[2]*B[7]-A[3]*B[6]+A[4]*B[5]-A[5]*B[4]+A[6]*B[3]+A[7]*B[2]-A[8]*B[1]+A[9]*B[0]);
}
void gp_default_3_3_0(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[6]+A[1]*B[8]+A[2]*B[9]-A[3]*B[3]+A[4]*B[4]+A[5]*B[5]+A[6]*B[0]+A[7]*B[7]+A[8]*B[1]+A[9]*B[2]);
}
void gp_default_3_3_2(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[5]-A[1]*B[7]-A[2]*B[3]+A[3]*B[2]+A[5]*B[0]+A[7]*B[1]);
	C[1] += (A[0]*B[4]+A[1]*B[3]-A[2]*B[7]-A[3]*B[1]-A[4]*B[0]+A[7]*B[2]);
	C[2] += (A[1]*B[9]-A[2]*B[8]+A[4]*B[5]-A[5]*B[4]+A[8]*B[2]-A[9]*B[1]);
	C[3] += (-A[0]*B[3]+A[1]*B[4]+A[2]*B[5]+A[3]*B[0]-A[4]*B[1]-A[5]*B[2]);
	C[4] += (-A[0]*B[9]+A[2]*B[6]+A[4]*B[7]-A[6]*B[2]-A[7]*B[4]+A[9]*B[0]);
	C[5] += (A[0]*B[8]-A[1]*B[6]+A[5]*B[7]+A[6]*B[1]-A[7]*B[5]-A[8]*B[0]);
	C[6] += (-A[0]*B[6]-A[1]*B[8]-A[2]*B[9]+A[6]*B[0]+A[8]*B[1]+A[9]*B[2]);
	C[7] += (-A[3]*B[9]+A[5]*B[6]-A[6]*B[5]+A[7]*B[8]-A[8]*B[7]+A[9]*B[3]);
	C[8] += (A[3]*B[8]-A[4]*B[6]+A[6]*B[4]+A[7]*B[9]-A[8]*B[3]-A[9]*B[7]);
	C[9] += (-A[3]*B[6]-A[4]*B[8]-A[5]*B[9]+A[6]*B[3]+A[8]*B[4]+A[9]*B[5]);
}
void gp_default_3_3_4(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[7]+A[1]*B[5]-A[2]*B[4]-A[4]*B[2]+A[5]*B[1]-A[7]*B[0]);
	C[1] += (-A[1]*B[9]+A[2]*B[8]-A[3]*B[7]-A[7]*B[3]+A[8]*B[2]-A[9]*B[1]);
	C[2] += (A[0]*B[9]-A[2]*B[6]+A[3]*B[5]+A[5]*B[3]-A[6]*B[2]+A[9]*B[0]);
	C[3] += (-A[0]*B[8]+A[1]*B[6]-A[3]*B[4]-A[4]*B[3]+A[6]*B[1]-A[8]*B[0]);
	C[4] += (-A[4]*B[9]+A[5]*B[8]-A[6]*B[7]-A[7]*B[6]+A[8]*B[5]-A[9]*B[4]);
}
void gp_default_3_4_1(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[1]-A[1]*B[2]-A[2]*B[3]+A[3]*B[0]);
	C[1] += (-A[2]*B[4]+A[5]*B[1]+A[7]*B[2]+A[9]*B[0]);
	C[2] += (A[1]*B[4]-A[4]*B[1]+A[7]*B[3]-A[8]*B[0]);
	C[3] += (-A[0]*B[4]-A[4]*B[2]-A[5]*B[3]+A[6]*B[0]);
	C[4] += (-A[3]*B[4]+A[6]*B[1]+A[8]*B[2]+A[9]*B[3]);
}
void gp_default_3_4_3(const float *A, const float *B, float *C) {
	C[0] += (-A[1]*B[3]+A[2]*B[2]-A[7]*B[0]);
	C[1] += (A[0]*B[3]-A[2]*B[1]+A[5]*B[0]);
	C[2] += (-A[0]*B[2]+A[1]*B[1]-A[4]*B[0]);
	C[3] += (-A[4]*B[3]+A[5]*B[2]-A[7]*B[1]);
	C[4] += (-A[2]*B[4]+A[3]*B[3]-A[9]*B[0]);
	C[5] += (A[1]*B[4]-A[3]*B[2]+A[8]*B[0]);
	C[6] += (-A[7]*B[4]+A[8]*B[3]-A[9]*B[2]);
	C[7] += (-A[0]*B[4]+A[3]*B[1]-A[6]*B[0]);
	C[8] += (A[5]*B[4]-A[6]*B[3]+A[9]*B[1]);
	C[9] += (-A[4]*B[4]+A[6]*B[2]-A[8]*B[1]);
}
void gp_default_3_5_2(const float *A, const float *B, float *C) {
	C[0] += A[2]*B[0];
	C[1] += -A[1]*B[0];
	C[2] += A[7]*B[0];
	C[3] += A[0]*B[0];
	C[4] += -A[5]*B[0];
	C[5] += A[4]*B[0];
	C[6] += A[3]*B[0];
	C[7] += A[9]*B[0];
	C[8] += -A[8]*B[0];
	C[9] += A[6]*B[0];
}
void gp_default_4_0_4(const float *A, const float *B, float *C) {
	gp_default_1_0_1(A, B, C);
}
void gp_default_4_1_3(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[3]-A[1]*B[0]);
	C[1] += (-A[0]*B[2]-A[2]*B[0]);
	C[2] += (A[0]*B[1]-A[3]*B[0]);
	C[3] += (A[0]*B[4]-A[4]*B[0]);
	C[4] += (-A[1]*B[2]-A[2]*B[3]);
	C[5] += (A[1]*B[1]-A[3]*B[3]);
	C[6] += (A[1]*B[4]-A[4]*B[3]);
	C[7] += (A[2]*B[1]+A[3]*B[2]);
	C[8] += (A[2]*B[4]+A[4]*B[2]);
	C[9] += (A[3]*B[4]-A[4]*B[1]);
}
void gp_default_4_1_5(const float *A, const float *B, float *C) {
	gp_default_1_4_5(A, B, C);
}
void gp_default_4_2_2(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[5]-A[1]*B[1]-A[2]*B[3]);
	C[1] += (A[0]*B[4]+A[1]*B[0]-A[3]*B[3]);
	C[2] += (-A[0]*B[9]+A[1]*B[6]-A[4]*B[3]);
	C[3] += (-A[0]*B[2]+A[2]*B[0]+A[3]*B[1]);
	C[4] += (A[0]*B[8]+A[2]*B[6]+A[4]*B[1]);
	C[5] += (-A[0]*B[7]+A[3]*B[6]-A[4]*B[0]);
	C[6] += (-A[1]*B[2]-A[2]*B[4]-A[3]*B[5]);
	C[7] += (A[1]*B[8]+A[2]*B[9]-A[4]*B[5]);
	C[8] += (-A[1]*B[7]+A[3]*B[9]+A[4]*B[4]);
	C[9] += (-A[2]*B[7]-A[3]*B[8]-A[4]*B[2]);
}
void gp_default_4_2_4(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[6]-A[1]*B[3]+A[2]*B[1]-A[3]*B[0]);
	C[1] += (A[0]*B[9]-A[2]*B[5]+A[3]*B[4]-A[4]*B[3]);
	C[2] += (-A[0]*B[8]+A[1]*B[5]-A[3]*B[2]+A[4]*B[1]);
	C[3] += (A[0]*B[7]-A[1]*B[4]+A[2]*B[2]-A[4]*B[0]);
	C[4] += (A[1]*B[9]-A[2]*B[8]+A[3]*B[7]-A[4]*B[6]);
}
void gp_default_4_3_1(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[3]+A[1]*B[0]+A[2]*B[1]+A[3]*B[2]);
	C[1] += (-A[0]*B[9]-A[1]*B[5]-A[2]*B[7]+A[4]*B[2]);
	C[2] += (A[0]*B[8]+A[1]*B[4]-A[3]*B[7]-A[4]*B[1]);
	C[3] += (-A[0]*B[6]+A[2]*B[4]+A[3]*B[5]+A[4]*B[0]);
	C[4] += (-A[1]*B[6]-A[2]*B[8]-A[3]*B[9]+A[4]*B[3]);
}
void gp_default_4_3_3(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[7]+A[2]*B[2]-A[3]*B[1]);
	C[1] += (A[0]*B[5]-A[1]*B[2]+A[3]*B[0]);
	C[2] += (-A[0]*B[4]+A[1]*B[1]-A[2]*B[0]);
	C[3] += (-A[1]*B[7]+A[2]*B[5]-A[3]*B[4]);
	C[4] += (-A[0]*B[9]+A[3]*B[3]-A[4]*B[2]);
	C[5] += (A[0]*B[8]-A[2]*B[3]+A[4]*B[1]);
	C[6] += (-A[2]*B[9]+A[3]*B[8]-A[4]*B[7]);
	C[7] += (-A[0]*B[6]+A[1]*B[3]-A[4]*B[0]);
	C[8] += (A[1]*B[9]-A[3]*B[6]+A[4]*B[5]);
	C[9] += (-A[1]*B[8]+A[2]*B[6]-A[4]*B[4]);
}
void gp_default_4_4_0(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[4]-A[1]*B[1]-A[2]*B[2]-A[3]*B[3]+A[4]*B[0]);
}
void gp_default_4_4_2(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[3]+A[3]*B[0]);
	C[1] += (A[0]*B[2]-A[2]*B[0]);
	C[2] += (-A[2]*B[3]+A[3]*B[2]);
	C[3] += (-A[0]*B[1]+A[1]*B[0]);
	C[4] += (A[1]*B[3]-A[3]*B[1]);
	C[5] += (-A[1]*B[2]+A[2]*B[1]);
	C[6] += (-A[0]*B[4]+A[4]*B[0]);
	C[7] += (-A[3]*B[4]+A[4]*B[3]);
	C[8] += (A[2]*B[4]-A[4]*B[2]);
	C[9] += (-A[1]*B[4]+A[4]*B[1]);
}
void gp_default_4_5_1(const float *A, const float *B, float *C) {
	C[0] += A[0]*B[0];
	C[1] += A[3]*B[0];
	C[2] += -A[2]*B[0];
	C[3] += A[1]*B[0];
	C[4] += A[4]*B[0];
}
void gp_default_5_0_5(const float *A, const float *B, float *C) {
	gp_default_0_0_0(A, B, C);
}
void gp_default_5_1_4(const float *A, const float *B, float *C) {
	C[0] += -A[0]*B[0];
	C[1] += -A[0]*B[3];
	C[2] += A[0]*B[2];
	C[3] += -A[0]*B[1];
	C[4] += -A[0]*B[4];
}
void gp_default_5_2_3(const float *A, const float *B, float *C) {
	C[0] += -A[0]*B[3];
	C[1] += A[0]*B[1];
	C[2] += -A[0]*B[0];
	C[3] += -A[0]*B[6];
	C[4] += -A[0]*B[5];
	C[5] += A[0]*B[4];
	C[6] += -A[0]*B[9];
	C[7] += -A[0]*B[2];
	C[8] += A[0]*B[8];
	C[9] += -A[0]*B[7];
}
void gp_default_5_3_2(const float *A, const float *B, float *C) {
	C[0] += A[0]*B[2];
	C[1] += -A[0]*B[1];
	C[2] += A[0]*B[7];
	C[3] += A[0]*B[0];
	C[4] += -A[0]*B[5];
	C[5] += A[0]*B[4];
	C[6] += A[0]*B[3];
	C[7] += A[0]*B[9];
	C[8] += -A[0]*B[8];
	C[9] += A[0]*B[6];
}
void gp_default_5_4_1(const float *A, const float *B, float *C) {
	C[0] += A[0]*B[0];
	C[1] += A[0]*B[3];
	C[2] += -A[0]*B[2];
	C[3] += A[0]*B[1];
	C[4] += A[0]*B[4];
}
void gp_default_5_5_0(const float *A, const float *B, float *C) {
	C[0] += -A[0]*B[0];
}
void gp__internal_euclidean_metric__0_0_0(const float *A, const float *B, float *C) {
	C[0] += A[0]*B[0];
}
void gp__internal_euclidean_metric__0_1_1(const float *A, const float *B, float *C) {
	C[0] += A[0]*B[0];
	C[1] += A[0]*B[1];
	C[2] += A[0]*B[2];
	C[3] += A[0]*B[3];
	C[4] += A[0]*B[4];
}
void gp__internal_euclidean_metric__0_2_2(const float *A, const float *B, float *C) {
	C[0] += A[0]*B[0];
	C[1] += A[0]*B[1];
	C[2] += A[0]*B[2];
	C[3] += A[0]*B[3];
	C[4] += A[0]*B[4];
	C[5] += A[0]*B[5];
	C[6] += A[0]*B[6];
	C[7] += A[0]*B[7];
	C[8] += A[0]*B[8];
	C[9] += A[0]*B[9];
}
void gp__internal_euclidean_metric__0_3_3(const float *A, const float *B, float *C) {
	gp__internal_euclidean_metric__0_2_2(A, B, C);
}
void gp__internal_euclidean_metric__0_4_4(const float *A, const float *B, float *C) {
	gp__internal_euclidean_metric__0_1_1(A, B, C);
}
void gp__internal_euclidean_metric__0_5_5(const float *A, const float *B, float *C) {
	gp__internal_euclidean_metric__0_0_0(A, B, C);
}
void gp__internal_euclidean_metric__1_0_1(const float *A, const float *B, float *C) {
	C[0] += A[0]*B[0];
	C[1] += A[1]*B[0];
	C[2] += A[2]*B[0];
	C[3] += A[3]*B[0];
	C[4] += A[4]*B[0];
}
void gp__internal_euclidean_metric__1_1_0(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[0]+A[1]*B[1]+A[2]*B[2]+A[3]*B[3]+A[4]*B[4]);
}
void gp__internal_euclidean_metric__1_1_2(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[1]-A[1]*B[0]);
	C[1] += (A[0]*B[2]-A[2]*B[0]);
	C[2] += (A[1]*B[2]-A[2]*B[1]);
	C[3] += (A[0]*B[3]-A[3]*B[0]);
	C[4] += (A[1]*B[3]-A[3]*B[1]);
	C[5] += (A[2]*B[3]-A[3]*B[2]);
	C[6] += (A[0]*B[4]-A[4]*B[0]);
	C[7] += (A[1]*B[4]-A[4]*B[1]);
	C[8] += (A[2]*B[4]-A[4]*B[2]);
	C[9] += (A[3]*B[4]-A[4]*B[3]);
}
void gp__internal_euclidean_metric__1_2_1(const float *A, const float *B, float *C) {
	C[0] += (-A[1]*B[0]-A[2]*B[1]-A[3]*B[3]-A[4]*B[6]);
	C[1] += (A[0]*B[0]-A[2]*B[2]-A[3]*B[4]-A[4]*B[7]);
	C[2] += (A[0]*B[1]+A[1]*B[2]-A[3]*B[5]-A[4]*B[8]);
	C[3] += (A[0]*B[3]+A[1]*B[4]+A[2]*B[5]-A[4]*B[9]);
	C[4] += (A[0]*B[6]+A[1]*B[7]+A[2]*B[8]+A[3]*B[9]);
}
void gp__internal_euclidean_metric__1_2_3(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[2]-A[1]*B[1]+A[2]*B[0]);
	C[1] += (A[0]*B[4]-A[1]*B[3]+A[3]*B[0]);
	C[2] += (A[0]*B[5]-A[2]*B[3]+A[3]*B[1]);
	C[3] += (A[1]*B[5]-A[2]*B[4]+A[3]*B[2]);
	C[4] += (A[0]*B[7]-A[1]*B[6]+A[4]*B[0]);
	C[5] += (A[0]*B[8]-A[2]*B[6]+A[4]*B[1]);
	C[6] += (A[1]*B[8]-A[2]*B[7]+A[4]*B[2]);
	C[7] += (A[0]*B[9]-A[3]*B[6]+A[4]*B[3]);
	C[8] += (A[1]*B[9]-A[3]*B[7]+A[4]*B[4]);
	C[9] += (A[2]*B[9]-A[3]*B[8]+A[4]*B[5]);
}
void gp__internal_euclidean_metric__1_3_2(const float *A, const float *B, float *C) {
	C[0] += (A[2]*B[0]+A[3]*B[1]+A[4]*B[4]);
	C[1] += (-A[1]*B[0]+A[3]*B[2]+A[4]*B[5]);
	C[2] += (A[0]*B[0]+A[3]*B[3]+A[4]*B[6]);
	C[3] += (-A[1]*B[1]-A[2]*B[2]+A[4]*B[7]);
	C[4] += (A[0]*B[1]-A[2]*B[3]+A[4]*B[8]);
	C[5] += (A[0]*B[2]+A[1]*B[3]+A[4]*B[9]);
	C[6] += (-A[1]*B[4]-A[2]*B[5]-A[3]*B[7]);
	C[7] += (A[0]*B[4]-A[2]*B[6]-A[3]*B[8]);
	C[8] += (A[0]*B[5]+A[1]*B[6]-A[3]*B[9]);
	C[9] += (A[0]*B[7]+A[1]*B[8]+A[2]*B[9]);
}
void gp__internal_euclidean_metric__1_3_4(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[3]-A[1]*B[2]+A[2]*B[1]-A[3]*B[0]);
	C[1] += (A[0]*B[6]-A[1]*B[5]+A[2]*B[4]-A[4]*B[0]);
	C[2] += (A[0]*B[8]-A[1]*B[7]+A[3]*B[4]-A[4]*B[1]);
	C[3] += (A[0]*B[9]-A[2]*B[7]+A[3]*B[5]-A[4]*B[2]);
	C[4] += (A[1]*B[9]-A[2]*B[8]+A[3]*B[6]-A[4]*B[3]);
}
void gp__internal_euclidean_metric__1_4_3(const float *A, const float *B, float *C) {
	C[0] += (-A[3]*B[0]-A[4]*B[1]);
	C[1] += (A[2]*B[0]-A[4]*B[2]);
	C[2] += (-A[1]*B[0]-A[4]*B[3]);
	C[3] += (A[0]*B[0]-A[4]*B[4]);
	C[4] += (A[2]*B[1]+A[3]*B[2]);
	C[5] += (-A[1]*B[1]+A[3]*B[3]);
	C[6] += (A[0]*B[1]+A[3]*B[4]);
	C[7] += (-A[1]*B[2]-A[2]*B[3]);
	C[8] += (A[0]*B[2]-A[2]*B[4]);
	C[9] += (A[0]*B[3]+A[1]*B[4]);
}
void gp__internal_euclidean_metric__1_4_5(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[4]-A[1]*B[3]+A[2]*B[2]-A[3]*B[1]+A[4]*B[0]);
}
void gp__internal_euclidean_metric__1_5_4(const float *A, const float *B, float *C) {
	C[0] += A[4]*B[0];
	C[1] += -A[3]*B[0];
	C[2] += A[2]*B[0];
	C[3] += -A[1]*B[0];
	C[4] += A[0]*B[0];
}
void gp__internal_euclidean_metric__2_0_2(const float *A, const float *B, float *C) {
	C[0] += A[0]*B[0];
	C[1] += A[1]*B[0];
	C[2] += A[2]*B[0];
	C[3] += A[3]*B[0];
	C[4] += A[4]*B[0];
	C[5] += A[5]*B[0];
	C[6] += A[6]*B[0];
	C[7] += A[7]*B[0];
	C[8] += A[8]*B[0];
	C[9] += A[9]*B[0];
}
void gp__internal_euclidean_metric__2_1_1(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[1]+A[1]*B[2]+A[3]*B[3]+A[6]*B[4]);
	C[1] += (-A[0]*B[0]+A[2]*B[2]+A[4]*B[3]+A[7]*B[4]);
	C[2] += (-A[1]*B[0]-A[2]*B[1]+A[5]*B[3]+A[8]*B[4]);
	C[3] += (-A[3]*B[0]-A[4]*B[1]-A[5]*B[2]+A[9]*B[4]);
	C[4] += (-A[6]*B[0]-A[7]*B[1]-A[8]*B[2]-A[9]*B[3]);
}
void gp__internal_euclidean_metric__2_1_3(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[2]-A[1]*B[1]+A[2]*B[0]);
	C[1] += (A[0]*B[3]-A[3]*B[1]+A[4]*B[0]);
	C[2] += (A[1]*B[3]-A[3]*B[2]+A[5]*B[0]);
	C[3] += (A[2]*B[3]-A[4]*B[2]+A[5]*B[1]);
	C[4] += (A[0]*B[4]-A[6]*B[1]+A[7]*B[0]);
	C[5] += (A[1]*B[4]-A[6]*B[2]+A[8]*B[0]);
	C[6] += (A[2]*B[4]-A[7]*B[2]+A[8]*B[1]);
	C[7] += (A[3]*B[4]-A[6]*B[3]+A[9]*B[0]);
	C[8] += (A[4]*B[4]-A[7]*B[3]+A[9]*B[1]);
	C[9] += (A[5]*B[4]-A[8]*B[3]+A[9]*B[2]);
}
void gp__internal_euclidean_metric__2_2_0(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[0]-A[1]*B[1]-A[2]*B[2]-A[3]*B[3]-A[4]*B[4]-A[5]*B[5]-A[6]*B[6]-A[7]*B[7]-A[8]*B[8]-A[9]*B[9]);
}
void gp__internal_euclidean_metric__2_2_2(const float *A, const float *B, float *C) {
	C[0] += (-A[1]*B[2]+A[2]*B[1]-A[3]*B[4]+A[4]*B[3]-A[6]*B[7]+A[7]*B[6]);
	C[1] += (A[0]*B[2]-A[2]*B[0]-A[3]*B[5]+A[5]*B[3]-A[6]*B[8]+A[8]*B[6]);
	C[2] += (-A[0]*B[1]+A[1]*B[0]-A[4]*B[5]+A[5]*B[4]-A[7]*B[8]+A[8]*B[7]);
	C[3] += (A[0]*B[4]+A[1]*B[5]-A[4]*B[0]-A[5]*B[1]-A[6]*B[9]+A[9]*B[6]);
	C[4] += (-A[0]*B[3]+A[2]*B[5]+A[3]*B[0]-A[5]*B[2]-A[7]*B[9]+A[9]*B[7]);
	C[5] += (-A[1]*B[3]-A[2]*B[4]+A[3]*B[1]+A[4]*B[2]-A[8]*B[9]+A[9]*B[8]);
	C[6] += (A[0]*B[7]+A[1]*B[8]+A[3]*B[9]-A[7]*B[0]-A[8]*B[1]-A[9]*B[3]);
	C[7] += (-A[0]*B[6]+A[2]*B[8]+A[4]*B[9]+A[6]*B[0]-A[8]*B[2]-A[9]*B[4]);
	C[8] += (-A[1]*B[6]-A[2]*B[7]+A[5]*B[9]+A[6]*B[1]+A[7]*B[2]-A[9]*B[5]);
	C[9] += (-A[3]*B[6]-A[4]*B[7]-A[5]*B[8]+A[6]*B[3]+A[7]*B[4]+A[8]*B[5]);
}
void gp__internal_euclidean_metric__2_2_4(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[5]-A[1]*B[4]+A[2]*B[3]+A[3]*B[2]-A[4]*B[1]+A[5]*B[0]);
	C[1] += (A[0]*B[8]-A[1]*B[7]+A[2]*B[6]+A[6]*B[2]-A[7]*B[1]+A[8]*B[0]);
	C[2] += (A[0]*B[9]-A[3]*B[7]+A[4]*B[6]+A[6]*B[4]-A[7]*B[3]+A[9]*B[0]);
	C[3] += (A[1]*B[9]-A[3]*B[8]+A[5]*B[6]+A[6]*B[5]-A[8]*B[3]+A[9]*B[1]);
	C[4] += (A[2]*B[9]-A[4]*B[8]+A[5]*B[7]+A[7]*B[5]-A[8]*B[4]+A[9]*B[2]);
}
void gp__internal_euclidean_metric__2_3_1(const float *A, const float *B, float *C) {
	C[0] += (-A[2]*B[0]-A[4]*B[1]-A[5]*B[2]-A[7]*B[4]-A[8]*B[5]-A[9]*B[7]);
	C[1] += (A[1]*B[0]+A[3]*B[1]-A[5]*B[3]+A[6]*B[4]-A[8]*B[6]-A[9]*B[8]);
	C[2] += (-A[0]*B[0]+A[3]*B[2]+A[4]*B[3]+A[6]*B[5]+A[7]*B[6]-A[9]*B[9]);
	C[3] += (-A[0]*B[1]-A[1]*B[2]-A[2]*B[3]+A[6]*B[7]+A[7]*B[8]+A[8]*B[9]);
	C[4] += (-A[0]*B[4]-A[1]*B[5]-A[2]*B[6]-A[3]*B[7]-A[4]*B[8]-A[5]*B[9]);
}
void gp__internal_euclidean_metric__2_3_3(const float *A, const float *B, float *C) {
	C[0] += (A[3]*B[3]-A[4]*B[2]+A[5]*B[1]+A[6]*B[6]-A[7]*B[5]+A[8]*B[4]);
	C[1] += (-A[1]*B[3]+A[2]*B[2]-A[5]*B[0]+A[6]*B[8]-A[7]*B[7]+A[9]*B[4]);
	C[2] += (A[0]*B[3]-A[2]*B[1]+A[4]*B[0]+A[6]*B[9]-A[8]*B[7]+A[9]*B[5]);
	C[3] += (-A[0]*B[2]+A[1]*B[1]-A[3]*B[0]+A[7]*B[9]-A[8]*B[8]+A[9]*B[6]);
	C[4] += (-A[1]*B[6]+A[2]*B[5]-A[3]*B[8]+A[4]*B[7]-A[8]*B[0]-A[9]*B[1]);
	C[5] += (A[0]*B[6]-A[2]*B[4]-A[3]*B[9]+A[5]*B[7]+A[7]*B[0]-A[9]*B[2]);
	C[6] += (-A[0]*B[5]+A[1]*B[4]-A[4]*B[9]+A[5]*B[8]-A[6]*B[0]-A[9]*B[3]);
	C[7] += (A[0]*B[8]+A[1]*B[9]-A[4]*B[4]-A[5]*B[5]+A[7]*B[1]+A[8]*B[2]);
	C[8] += (-A[0]*B[7]+A[2]*B[9]+A[3]*B[4]-A[5]*B[6]-A[6]*B[1]+A[8]*B[3]);
	C[9] += (-A[1]*B[7]-A[2]*B[8]+A[3]*B[5]+A[4]*B[6]-A[6]*B[2]-A[7]*B[3]);
}
void gp__internal_euclidean_metric__2_3_5(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[9]-A[1]*B[8]+A[2]*B[7]+A[3]*B[6]-A[4]*B[5]+A[5]*B[4]-A[6]*B[3]+A[7]*B[2]-A[8]*B[1]+A[9]*B[0]);
}
void gp__internal_euclidean_metric__2_4_2(const float *A, const float *B, float *C) {
	C[0] += (-A[5]*B[0]-A[8]*B[1]-A[9]*B[2]);
	C[1] += (A[4]*B[0]+A[7]*B[1]-A[9]*B[3]);
	C[2] += (-A[3]*B[0]-A[6]*B[1]-A[9]*B[4]);
	C[3] += (-A[2]*B[0]+A[7]*B[2]+A[8]*B[3]);
	C[4] += (A[1]*B[0]-A[6]*B[2]+A[8]*B[4]);
	C[5] += (-A[0]*B[0]-A[6]*B[3]-A[7]*B[4]);
	C[6] += (-A[2]*B[1]-A[4]*B[2]-A[5]*B[3]);
	C[7] += (A[1]*B[1]+A[3]*B[2]-A[5]*B[4]);
	C[8] += (-A[0]*B[1]+A[3]*B[3]+A[4]*B[4]);
	C[9] += (-A[0]*B[2]-A[1]*B[3]-A[2]*B[4]);
}
void gp__internal_euclidean_metric__2_4_4(const float *A, const float *B, float *C) {
	C[0] += (-A[6]*B[4]+A[7]*B[3]-A[8]*B[2]+A[9]*B[1]);
	C[1] += (A[3]*B[4]-A[4]*B[3]+A[5]*B[2]-A[9]*B[0]);
	C[2] += (-A[1]*B[4]+A[2]*B[3]-A[5]*B[1]+A[8]*B[0]);
	C[3] += (A[0]*B[4]-A[2]*B[2]+A[4]*B[1]-A[7]*B[0]);
	C[4] += (-A[0]*B[3]+A[1]*B[2]-A[3]*B[1]+A[6]*B[0]);
}
void gp__internal_euclidean_metric__2_5_3(const float *A, const float *B, float *C) {
	C[0] += -A[9]*B[0];
	C[1] += A[8]*B[0];
	C[2] += -A[7]*B[0];
	C[3] += A[6]*B[0];
	C[4] += -A[5]*B[0];
	C[5] += A[4]*B[0];
	C[6] += -A[3]*B[0];
	C[7] += -A[2]*B[0];
	C[8] += A[1]*B[0];
	C[9] += -A[0]*B[0];
}
void gp__internal_euclidean_metric__3_0_3(const float *A, const float *B, float *C) {
	gp__internal_euclidean_metric__2_0_2(A, B, C);
}
void gp__internal_euclidean_metric__3_1_2(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[2]+A[1]*B[3]+A[4]*B[4]);
	C[1] += (-A[0]*B[1]+A[2]*B[3]+A[5]*B[4]);
	C[2] += (A[0]*B[0]+A[3]*B[3]+A[6]*B[4]);
	C[3] += (-A[1]*B[1]-A[2]*B[2]+A[7]*B[4]);
	C[4] += (A[1]*B[0]-A[3]*B[2]+A[8]*B[4]);
	C[5] += (A[2]*B[0]+A[3]*B[1]+A[9]*B[4]);
	C[6] += (-A[4]*B[1]-A[5]*B[2]-A[7]*B[3]);
	C[7] += (A[4]*B[0]-A[6]*B[2]-A[8]*B[3]);
	C[8] += (A[5]*B[0]+A[6]*B[1]-A[9]*B[3]);
	C[9] += (A[7]*B[0]+A[8]*B[1]+A[9]*B[2]);
}
void gp__internal_euclidean_metric__3_1_4(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[3]-A[1]*B[2]+A[2]*B[1]-A[3]*B[0]);
	C[1] += (A[0]*B[4]-A[4]*B[2]+A[5]*B[1]-A[6]*B[0]);
	C[2] += (A[1]*B[4]-A[4]*B[3]+A[7]*B[1]-A[8]*B[0]);
	C[3] += (A[2]*B[4]-A[5]*B[3]+A[7]*B[2]-A[9]*B[0]);
	C[4] += (A[3]*B[4]-A[6]*B[3]+A[8]*B[2]-A[9]*B[1]);
}
void gp__internal_euclidean_metric__3_2_1(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[2]-A[1]*B[4]-A[2]*B[5]-A[4]*B[7]-A[5]*B[8]-A[7]*B[9]);
	C[1] += (A[0]*B[1]+A[1]*B[3]-A[3]*B[5]+A[4]*B[6]-A[6]*B[8]-A[8]*B[9]);
	C[2] += (-A[0]*B[0]+A[2]*B[3]+A[3]*B[4]+A[5]*B[6]+A[6]*B[7]-A[9]*B[9]);
	C[3] += (-A[1]*B[0]-A[2]*B[1]-A[3]*B[2]+A[7]*B[6]+A[8]*B[7]+A[9]*B[8]);
	C[4] += (-A[4]*B[0]-A[5]*B[1]-A[6]*B[2]-A[7]*B[3]-A[8]*B[4]-A[9]*B[5]);
}
void gp__internal_euclidean_metric__3_2_3(const float *A, const float *B, float *C) {
	C[0] += (-A[1]*B[5]+A[2]*B[4]-A[3]*B[3]-A[4]*B[8]+A[5]*B[7]-A[6]*B[6]);
	C[1] += (A[0]*B[5]-A[2]*B[2]+A[3]*B[1]-A[4]*B[9]+A[7]*B[7]-A[8]*B[6]);
	C[2] += (-A[0]*B[4]+A[1]*B[2]-A[3]*B[0]-A[5]*B[9]+A[7]*B[8]-A[9]*B[6]);
	C[3] += (A[0]*B[3]-A[1]*B[1]+A[2]*B[0]-A[6]*B[9]+A[8]*B[8]-A[9]*B[7]);
	C[4] += (A[0]*B[8]+A[1]*B[9]-A[5]*B[2]+A[6]*B[1]-A[7]*B[4]+A[8]*B[3]);
	C[5] += (-A[0]*B[7]+A[2]*B[9]+A[4]*B[2]-A[6]*B[0]-A[7]*B[5]+A[9]*B[3]);
	C[6] += (A[0]*B[6]+A[3]*B[9]-A[4]*B[1]+A[5]*B[0]-A[8]*B[5]+A[9]*B[4]);
	C[7] += (-A[1]*B[7]-A[2]*B[8]+A[4]*B[4]+A[5]*B[5]-A[8]*B[0]-A[9]*B[1]);
	C[8] += (A[1]*B[6]-A[3]*B[8]-A[4]*B[3]+A[6]*B[5]+A[7]*B[0]-A[9]*B[2]);
	C[9] += (A[2]*B[6]+A[3]*B[7]-A[5]*B[3]-A[6]*B[4]+A[7]*B[1]+A[8]*B[2]);
}
void gp__internal_euclidean_metric__3_2_5(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[9]-A[1]*B[8]+A[2]*B[7]-A[3]*B[6]+A[4]*B[5]-A[5]*B[4]+A[6]*B[3]+A[7]*B[2]-A[8]*B[1]+A[9]*B[0]);
}
void gp__internal_euclidean_metric__3_3_0(const float *A, const float *B, float *C) {
	gp__internal_euclidean_metric__2_2_0(A, B, C);
}
void gp__internal_euclidean_metric__3_3_2(const float *A, const float *B, float *C) {
	C[0] += (-A[2]*B[3]+A[3]*B[2]-A[5]*B[6]+A[6]*B[5]-A[7]*B[8]+A[8]*B[7]);
	C[1] += (A[1]*B[3]-A[3]*B[1]+A[4]*B[6]-A[6]*B[4]-A[7]*B[9]+A[9]*B[7]);
	C[2] += (-A[1]*B[2]+A[2]*B[1]-A[4]*B[5]+A[5]*B[4]-A[8]*B[9]+A[9]*B[8]);
	C[3] += (-A[0]*B[3]+A[3]*B[0]+A[4]*B[8]+A[5]*B[9]-A[8]*B[4]-A[9]*B[5]);
	C[4] += (A[0]*B[2]-A[2]*B[0]-A[4]*B[7]+A[6]*B[9]+A[7]*B[4]-A[9]*B[6]);
	C[5] += (-A[0]*B[1]+A[1]*B[0]-A[5]*B[7]-A[6]*B[8]+A[7]*B[5]+A[8]*B[6]);
	C[6] += (-A[0]*B[6]-A[1]*B[8]-A[2]*B[9]+A[6]*B[0]+A[8]*B[1]+A[9]*B[2]);
	C[7] += (A[0]*B[5]+A[1]*B[7]-A[3]*B[9]-A[5]*B[0]-A[7]*B[1]+A[9]*B[3]);
	C[8] += (-A[0]*B[4]+A[2]*B[7]+A[3]*B[8]+A[4]*B[0]-A[7]*B[2]-A[8]*B[3]);
	C[9] += (-A[1]*B[4]-A[2]*B[5]-A[3]*B[6]+A[4]*B[1]+A[5]*B[2]+A[6]*B[3]);
}
void gp__internal_euclidean_metric__3_3_4(const float *A, const float *B, float *C) {
	C[0] += (A[4]*B[9]-A[5]*B[8]+A[6]*B[7]+A[7]*B[6]-A[8]*B[5]+A[9]*B[4]);
	C[1] += (-A[1]*B[9]+A[2]*B[8]-A[3]*B[7]-A[7]*B[3]+A[8]*B[2]-A[9]*B[1]);
	C[2] += (A[0]*B[9]-A[2]*B[6]+A[3]*B[5]+A[5]*B[3]-A[6]*B[2]+A[9]*B[0]);
	C[3] += (-A[0]*B[8]+A[1]*B[6]-A[3]*B[4]-A[4]*B[3]+A[6]*B[1]-A[8]*B[0]);
	C[4] += (A[0]*B[7]-A[1]*B[5]+A[2]*B[4]+A[4]*B[2]-A[5]*B[1]+A[7]*B[0]);
}
void gp__internal_euclidean_metric__3_4_1(const float *A, const float *B, float *C) {
	C[0] += (A[3]*B[0]+A[6]*B[1]+A[8]*B[2]+A[9]*B[3]);
	C[1] += (-A[2]*B[0]-A[5]*B[1]-A[7]*B[2]+A[9]*B[4]);
	C[2] += (A[1]*B[0]+A[4]*B[1]-A[7]*B[3]-A[8]*B[4]);
	C[3] += (-A[0]*B[0]+A[4]*B[2]+A[5]*B[3]+A[6]*B[4]);
	C[4] += (-A[0]*B[1]-A[1]*B[2]-A[2]*B[3]-A[3]*B[4]);
}
void gp__internal_euclidean_metric__3_4_3(const float *A, const float *B, float *C) {
	C[0] += (-A[7]*B[4]+A[8]*B[3]-A[9]*B[2]);
	C[1] += (A[5]*B[4]-A[6]*B[3]+A[9]*B[1]);
	C[2] += (-A[4]*B[4]+A[6]*B[2]-A[8]*B[1]);
	C[3] += (A[4]*B[3]-A[5]*B[2]+A[7]*B[1]);
	C[4] += (-A[2]*B[4]+A[3]*B[3]-A[9]*B[0]);
	C[5] += (A[1]*B[4]-A[3]*B[2]+A[8]*B[0]);
	C[6] += (-A[1]*B[3]+A[2]*B[2]-A[7]*B[0]);
	C[7] += (-A[0]*B[4]+A[3]*B[1]-A[6]*B[0]);
	C[8] += (A[0]*B[3]-A[2]*B[1]+A[5]*B[0]);
	C[9] += (-A[0]*B[2]+A[1]*B[1]-A[4]*B[0]);
}
void gp__internal_euclidean_metric__3_5_2(const float *A, const float *B, float *C) {
	C[0] += -A[9]*B[0];
	C[1] += A[8]*B[0];
	C[2] += -A[7]*B[0];
	C[3] += -A[6]*B[0];
	C[4] += A[5]*B[0];
	C[5] += -A[4]*B[0];
	C[6] += A[3]*B[0];
	C[7] += -A[2]*B[0];
	C[8] += A[1]*B[0];
	C[9] += -A[0]*B[0];
}
void gp__internal_euclidean_metric__4_0_4(const float *A, const float *B, float *C) {
	gp__internal_euclidean_metric__1_0_1(A, B, C);
}
void gp__internal_euclidean_metric__4_1_3(const float *A, const float *B, float *C) {
	C[0] += (A[0]*B[3]+A[1]*B[4]);
	C[1] += (-A[0]*B[2]+A[2]*B[4]);
	C[2] += (A[0]*B[1]+A[3]*B[4]);
	C[3] += (-A[0]*B[0]+A[4]*B[4]);
	C[4] += (-A[1]*B[2]-A[2]*B[3]);
	C[5] += (A[1]*B[1]-A[3]*B[3]);
	C[6] += (-A[1]*B[0]-A[4]*B[3]);
	C[7] += (A[2]*B[1]+A[3]*B[2]);
	C[8] += (-A[2]*B[0]+A[4]*B[2]);
	C[9] += (-A[3]*B[0]-A[4]*B[1]);
}
void gp__internal_euclidean_metric__4_1_5(const float *A, const float *B, float *C) {
	gp__internal_euclidean_metric__1_4_5(A, B, C);
}
void gp__internal_euclidean_metric__4_2_2(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[5]-A[1]*B[8]-A[2]*B[9]);
	C[1] += (A[0]*B[4]+A[1]*B[7]-A[3]*B[9]);
	C[2] += (-A[0]*B[3]-A[1]*B[6]-A[4]*B[9]);
	C[3] += (-A[0]*B[2]+A[2]*B[7]+A[3]*B[8]);
	C[4] += (A[0]*B[1]-A[2]*B[6]+A[4]*B[8]);
	C[5] += (-A[0]*B[0]-A[3]*B[6]-A[4]*B[7]);
	C[6] += (-A[1]*B[2]-A[2]*B[4]-A[3]*B[5]);
	C[7] += (A[1]*B[1]+A[2]*B[3]-A[4]*B[5]);
	C[8] += (-A[1]*B[0]+A[3]*B[3]+A[4]*B[4]);
	C[9] += (-A[2]*B[0]-A[3]*B[1]-A[4]*B[2]);
}
void gp__internal_euclidean_metric__4_2_4(const float *A, const float *B, float *C) {
	C[0] += (-A[1]*B[9]+A[2]*B[8]-A[3]*B[7]+A[4]*B[6]);
	C[1] += (A[0]*B[9]-A[2]*B[5]+A[3]*B[4]-A[4]*B[3]);
	C[2] += (-A[0]*B[8]+A[1]*B[5]-A[3]*B[2]+A[4]*B[1]);
	C[3] += (A[0]*B[7]-A[1]*B[4]+A[2]*B[2]-A[4]*B[0]);
	C[4] += (-A[0]*B[6]+A[1]*B[3]-A[2]*B[1]+A[3]*B[0]);
}
void gp__internal_euclidean_metric__4_3_1(const float *A, const float *B, float *C) {
	C[0] += (-A[0]*B[3]-A[1]*B[6]-A[2]*B[8]-A[3]*B[9]);
	C[1] += (A[0]*B[2]+A[1]*B[5]+A[2]*B[7]-A[4]*B[9]);
	C[2] += (-A[0]*B[1]-A[1]*B[4]+A[3]*B[7]+A[4]*B[8]);
	C[3] += (A[0]*B[0]-A[2]*B[4]-A[3]*B[5]-A[4]*B[6]);
	C[4] += (A[1]*B[0]+A[2]*B[1]+A[3]*B[2]+A[4]*B[3]);
}
void gp__internal_euclidean_metric__4_3_3(const float *A, const float *B, float *C) {
	C[0] += (-A[2]*B[9]+A[3]*B[8]-A[4]*B[7]);
	C[1] += (A[1]*B[9]-A[3]*B[6]+A[4]*B[5]);
	C[2] += (-A[1]*B[8]+A[2]*B[6]-A[4]*B[4]);
	C[3] += (A[1]*B[7]-A[2]*B[5]+A[3]*B[4]);
	C[4] += (-A[0]*B[9]+A[3]*B[3]-A[4]*B[2]);
	C[5] += (A[0]*B[8]-A[2]*B[3]+A[4]*B[1]);
	C[6] += (-A[0]*B[7]+A[2]*B[2]-A[3]*B[1]);
	C[7] += (-A[0]*B[6]+A[1]*B[3]-A[4]*B[0]);
	C[8] += (A[0]*B[5]-A[1]*B[2]+A[3]*B[0]);
	C[9] += (-A[0]*B[4]+A[1]*B[1]-A[2]*B[0]);
}
void gp__internal_euclidean_metric__4_4_0(const float *A, const float *B, float *C) {
	gp__internal_euclidean_metric__1_1_0(A, B, C);
}
void gp__internal_euclidean_metric__4_4_2(const float *A, const float *B, float *C) {
	C[0] += (A[3]*B[4]-A[4]*B[3]);
	C[1] += (-A[2]*B[4]+A[4]*B[2]);
	C[2] += (A[2]*B[3]-A[3]*B[2]);
	C[3] += (A[1]*B[4]-A[4]*B[1]);
	C[4] += (-A[1]*B[3]+A[3]*B[1]);
	C[5] += (A[1]*B[2]-A[2]*B[1]);
	C[6] += (-A[0]*B[4]+A[4]*B[0]);
	C[7] += (A[0]*B[3]-A[3]*B[0]);
	C[8] += (-A[0]*B[2]+A[2]*B[0]);
	C[9] += (A[0]*B[1]-A[1]*B[0]);
}
void gp__internal_euclidean_metric__4_5_1(const float *A, const float *B, float *C) {
	gp__internal_euclidean_metric__1_5_4(A, B, C);
}
void gp__internal_euclidean_metric__5_0_5(const float *A, const float *B, float *C) {
	gp__internal_euclidean_metric__0_0_0(A, B, C);
}
void gp__internal_euclidean_metric__5_1_4(const float *A, const float *B, float *C) {
	C[0] += A[0]*B[4];
	C[1] += -A[0]*B[3];
	C[2] += A[0]*B[2];
	C[3] += -A[0]*B[1];
	C[4] += A[0]*B[0];
}
void gp__internal_euclidean_metric__5_2_3(const float *A, const float *B, float *C) {
	C[0] += -A[0]*B[9];
	C[1] += A[0]*B[8];
	C[2] += -A[0]*B[7];
	C[3] += A[0]*B[6];
	C[4] += -A[0]*B[5];
	C[5] += A[0]*B[4];
	C[6] += -A[0]*B[3];
	C[7] += -A[0]*B[2];
	C[8] += A[0]*B[1];
	C[9] += -A[0]*B[0];
}
void gp__internal_euclidean_metric__5_3_2(const float *A, const float *B, float *C) {
	C[0] += -A[0]*B[9];
	C[1] += A[0]*B[8];
	C[2] += -A[0]*B[7];
	C[3] += -A[0]*B[6];
	C[4] += A[0]*B[5];
	C[5] += -A[0]*B[4];
	C[6] += A[0]*B[3];
	C[7] += -A[0]*B[2];
	C[8] += A[0]*B[1];
	C[9] += -A[0]*B[0];
}
void gp__internal_euclidean_metric__5_4_1(const float *A, const float *B, float *C) {
	gp__internal_euclidean_metric__5_1_4(A, B, C);
}
void gp__internal_euclidean_metric__5_5_0(const float *A, const float *B, float *C) {
	gp__internal_euclidean_metric__0_0_0(A, B, C);
}
void copyGroup_0(const float *A, float *C) {
	C[0] = A[0];
}
void copyMul_0(const float *A, float *C, float s) {
	C[0] = A[0]*s;
}
void copyDiv_0(const float *A, float *C, float s) {
	C[0] = A[0]/s;
}
void add_0(const float *A, float *C) {
	C[0] += A[0];
}
void sub_0(const float *A, float *C) {
	C[0] -= A[0];
}
void neg_0(const float *A, float *C) {
	C[0] = -A[0];
}
void add2_0_0(const float *A, const float *B, float *C) {
	C[0] = (A[0]+B[0]);
}
void sub2_0_0(const float *A, const float *B, float *C) {
	C[0] = (A[0]-B[0]);
}
void hp_0_0(const float *A, const float *B, float *C) {
	C[0] = A[0]*B[0];
}
void ihp_0_0(const float *A, const float *B, float *C) {
	C[0] = A[0]/((B[0]));
}
int equals_0_0(const float *A, const float *B, float eps) {
		if (((A[0] - B[0]) < -eps) || ((A[0] - B[0]) > eps)) return 0;
	return 1;
}
int zeroGroup_0(const float *A, float eps) {
		if ((A[0] < -eps) || (A[0] > eps)) return 0;
		return 1;
}
void copyGroup_1(const float *A, float *C) {
	C[0] = A[0];
	C[1] = A[1];
	C[2] = A[2];
	C[3] = A[3];
	C[4] = A[4];
}
void copyMul_1(const float *A, float *C, float s) {
	C[0] = A[0]*s;
	C[1] = A[1]*s;
	C[2] = A[2]*s;
	C[3] = A[3]*s;
	C[4] = A[4]*s;
}
void copyDiv_1(const float *A, float *C, float s) {
	C[0] = A[0]/s;
	C[1] = A[1]/s;
	C[2] = A[2]/s;
	C[3] = A[3]/s;
	C[4] = A[4]/s;
}
void add_1(const float *A, float *C) {
	C[0] += A[0];
	C[1] += A[1];
	C[2] += A[2];
	C[3] += A[3];
	C[4] += A[4];
}
void sub_1(const float *A, float *C) {
	C[0] -= A[0];
	C[1] -= A[1];
	C[2] -= A[2];
	C[3] -= A[3];
	C[4] -= A[4];
}
void neg_1(const float *A, float *C) {
	C[0] = -A[0];
	C[1] = -A[1];
	C[2] = -A[2];
	C[3] = -A[3];
	C[4] = -A[4];
}
void add2_1_1(const float *A, const float *B, float *C) {
	C[0] = (A[0]+B[0]);
	C[1] = (A[1]+B[1]);
	C[2] = (A[2]+B[2]);
	C[3] = (A[3]+B[3]);
	C[4] = (A[4]+B[4]);
}
void sub2_1_1(const float *A, const float *B, float *C) {
	C[0] = (A[0]-B[0]);
	C[1] = (A[1]-B[1]);
	C[2] = (A[2]-B[2]);
	C[3] = (A[3]-B[3]);
	C[4] = (A[4]-B[4]);
}
void hp_1_1(const float *A, const float *B, float *C) {
	C[0] = A[0]*B[0];
	C[1] = A[1]*B[1];
	C[2] = A[2]*B[2];
	C[3] = A[3]*B[3];
	C[4] = A[4]*B[4];
}
void ihp_1_1(const float *A, const float *B, float *C) {
	C[0] = A[0]/((B[0]));
	C[1] = A[1]/((B[1]));
	C[2] = A[2]/((B[2]));
	C[3] = A[3]/((B[3]));
	C[4] = A[4]/((B[4]));
}
int equals_1_1(const float *A, const float *B, float eps) {
		if (((A[0] - B[0]) < -eps) || ((A[0] - B[0]) > eps)) return 0;
		if (((A[1] - B[1]) < -eps) || ((A[1] - B[1]) > eps)) return 0;
		if (((A[2] - B[2]) < -eps) || ((A[2] - B[2]) > eps)) return 0;
		if (((A[3] - B[3]) < -eps) || ((A[3] - B[3]) > eps)) return 0;
		if (((A[4] - B[4]) < -eps) || ((A[4] - B[4]) > eps)) return 0;
	return 1;
}
int zeroGroup_1(const float *A, float eps) {
		if ((A[0] < -eps) || (A[0] > eps)) return 0;
		if ((A[1] < -eps) || (A[1] > eps)) return 0;
		if ((A[2] < -eps) || (A[2] > eps)) return 0;
		if ((A[3] < -eps) || (A[3] > eps)) return 0;
		if ((A[4] < -eps) || (A[4] > eps)) return 0;
		return 1;
}
void copyGroup_2(const float *A, float *C) {
	C[0] = A[0];
	C[1] = A[1];
	C[2] = A[2];
	C[3] = A[3];
	C[4] = A[4];
	C[5] = A[5];
	C[6] = A[6];
	C[7] = A[7];
	C[8] = A[8];
	C[9] = A[9];
}
void copyMul_2(const float *A, float *C, float s) {
	C[0] = A[0]*s;
	C[1] = A[1]*s;
	C[2] = A[2]*s;
	C[3] = A[3]*s;
	C[4] = A[4]*s;
	C[5] = A[5]*s;
	C[6] = A[6]*s;
	C[7] = A[7]*s;
	C[8] = A[8]*s;
	C[9] = A[9]*s;
}
void copyDiv_2(const float *A, float *C, float s) {
	C[0] = A[0]/s;
	C[1] = A[1]/s;
	C[2] = A[2]/s;
	C[3] = A[3]/s;
	C[4] = A[4]/s;
	C[5] = A[5]/s;
	C[6] = A[6]/s;
	C[7] = A[7]/s;
	C[8] = A[8]/s;
	C[9] = A[9]/s;
}
void add_2(const float *A, float *C) {
	C[0] += A[0];
	C[1] += A[1];
	C[2] += A[2];
	C[3] += A[3];
	C[4] += A[4];
	C[5] += A[5];
	C[6] += A[6];
	C[7] += A[7];
	C[8] += A[8];
	C[9] += A[9];
}
void sub_2(const float *A, float *C) {
	C[0] -= A[0];
	C[1] -= A[1];
	C[2] -= A[2];
	C[3] -= A[3];
	C[4] -= A[4];
	C[5] -= A[5];
	C[6] -= A[6];
	C[7] -= A[7];
	C[8] -= A[8];
	C[9] -= A[9];
}
void neg_2(const float *A, float *C) {
	C[0] = -A[0];
	C[1] = -A[1];
	C[2] = -A[2];
	C[3] = -A[3];
	C[4] = -A[4];
	C[5] = -A[5];
	C[6] = -A[6];
	C[7] = -A[7];
	C[8] = -A[8];
	C[9] = -A[9];
}
void add2_2_2(const float *A, const float *B, float *C) {
	C[0] = (A[0]+B[0]);
	C[1] = (A[1]+B[1]);
	C[2] = (A[2]+B[2]);
	C[3] = (A[3]+B[3]);
	C[4] = (A[4]+B[4]);
	C[5] = (A[5]+B[5]);
	C[6] = (A[6]+B[6]);
	C[7] = (A[7]+B[7]);
	C[8] = (A[8]+B[8]);
	C[9] = (A[9]+B[9]);
}
void sub2_2_2(const float *A, const float *B, float *C) {
	C[0] = (A[0]-B[0]);
	C[1] = (A[1]-B[1]);
	C[2] = (A[2]-B[2]);
	C[3] = (A[3]-B[3]);
	C[4] = (A[4]-B[4]);
	C[5] = (A[5]-B[5]);
	C[6] = (A[6]-B[6]);
	C[7] = (A[7]-B[7]);
	C[8] = (A[8]-B[8]);
	C[9] = (A[9]-B[9]);
}
void hp_2_2(const float *A, const float *B, float *C) {
	C[0] = A[0]*B[0];
	C[1] = A[1]*B[1];
	C[2] = A[2]*B[2];
	C[3] = A[3]*B[3];
	C[4] = A[4]*B[4];
	C[5] = A[5]*B[5];
	C[6] = A[6]*B[6];
	C[7] = A[7]*B[7];
	C[8] = A[8]*B[8];
	C[9] = A[9]*B[9];
}
void ihp_2_2(const float *A, const float *B, float *C) {
	C[0] = A[0]/((B[0]));
	C[1] = A[1]/((B[1]));
	C[2] = A[2]/((B[2]));
	C[3] = A[3]/((B[3]));
	C[4] = A[4]/((B[4]));
	C[5] = A[5]/((B[5]));
	C[6] = A[6]/((B[6]));
	C[7] = A[7]/((B[7]));
	C[8] = A[8]/((B[8]));
	C[9] = A[9]/((B[9]));
}
int equals_2_2(const float *A, const float *B, float eps) {
		if (((A[0] - B[0]) < -eps) || ((A[0] - B[0]) > eps)) return 0;
		if (((A[1] - B[1]) < -eps) || ((A[1] - B[1]) > eps)) return 0;
		if (((A[2] - B[2]) < -eps) || ((A[2] - B[2]) > eps)) return 0;
		if (((A[3] - B[3]) < -eps) || ((A[3] - B[3]) > eps)) return 0;
		if (((A[4] - B[4]) < -eps) || ((A[4] - B[4]) > eps)) return 0;
		if (((A[5] - B[5]) < -eps) || ((A[5] - B[5]) > eps)) return 0;
		if (((A[6] - B[6]) < -eps) || ((A[6] - B[6]) > eps)) return 0;
		if (((A[7] - B[7]) < -eps) || ((A[7] - B[7]) > eps)) return 0;
		if (((A[8] - B[8]) < -eps) || ((A[8] - B[8]) > eps)) return 0;
		if (((A[9] - B[9]) < -eps) || ((A[9] - B[9]) > eps)) return 0;
	return 1;
}
int zeroGroup_2(const float *A, float eps) {
		if ((A[0] < -eps) || (A[0] > eps)) return 0;
		if ((A[1] < -eps) || (A[1] > eps)) return 0;
		if ((A[2] < -eps) || (A[2] > eps)) return 0;
		if ((A[3] < -eps) || (A[3] > eps)) return 0;
		if ((A[4] < -eps) || (A[4] > eps)) return 0;
		if ((A[5] < -eps) || (A[5] > eps)) return 0;
		if ((A[6] < -eps) || (A[6] > eps)) return 0;
		if ((A[7] < -eps) || (A[7] > eps)) return 0;
		if ((A[8] < -eps) || (A[8] > eps)) return 0;
		if ((A[9] < -eps) || (A[9] > eps)) return 0;
		return 1;
}
void copyGroup_3(const float *A, float *C) {
	copyGroup_2(A, C);
}
void copyMul_3(const float *A, float *C, float s) {
	copyMul_2(A, C, s);
}
void copyDiv_3(const float *A, float *C, float s) {
	copyDiv_2(A, C, s);
}
void add_3(const float *A, float *C) {
	add_2(A, C);
}
void sub_3(const float *A, float *C) {
	sub_2(A, C);
}
void neg_3(const float *A, float *C) {
	neg_2(A, C);
}
void add2_3_3(const float *A, const float *B, float *C) {
	add2_2_2(A, B, C);
}
void sub2_3_3(const float *A, const float *B, float *C) {
	sub2_2_2(A, B, C);
}
void hp_3_3(const float *A, const float *B, float *C) {
	hp_2_2(A, B, C);
}
void ihp_3_3(const float *A, const float *B, float *C) {
	ihp_2_2(A, B, C);
}
int equals_3_3(const float *A, const float *B, float eps) {
	return equals_2_2(A, B, eps);
}
int zeroGroup_3(const float *A, float eps) {
	return zeroGroup_2(A, eps);
}
void copyGroup_4(const float *A, float *C) {
	copyGroup_1(A, C);
}
void copyMul_4(const float *A, float *C, float s) {
	copyMul_1(A, C, s);
}
void copyDiv_4(const float *A, float *C, float s) {
	copyDiv_1(A, C, s);
}
void add_4(const float *A, float *C) {
	add_1(A, C);
}
void sub_4(const float *A, float *C) {
	sub_1(A, C);
}
void neg_4(const float *A, float *C) {
	neg_1(A, C);
}
void add2_4_4(const float *A, const float *B, float *C) {
	add2_1_1(A, B, C);
}
void sub2_4_4(const float *A, const float *B, float *C) {
	sub2_1_1(A, B, C);
}
void hp_4_4(const float *A, const float *B, float *C) {
	hp_1_1(A, B, C);
}
void ihp_4_4(const float *A, const float *B, float *C) {
	ihp_1_1(A, B, C);
}
int equals_4_4(const float *A, const float *B, float eps) {
	return equals_1_1(A, B, eps);
}
int zeroGroup_4(const float *A, float eps) {
	return zeroGroup_1(A, eps);
}
void copyGroup_5(const float *A, float *C) {
	copyGroup_0(A, C);
}
void copyMul_5(const float *A, float *C, float s) {
	copyMul_0(A, C, s);
}
void copyDiv_5(const float *A, float *C, float s) {
	copyDiv_0(A, C, s);
}
void add_5(const float *A, float *C) {
	add_0(A, C);
}
void sub_5(const float *A, float *C) {
	sub_0(A, C);
}
void neg_5(const float *A, float *C) {
	neg_0(A, C);
}
void add2_5_5(const float *A, const float *B, float *C) {
	add2_0_0(A, B, C);
}
void sub2_5_5(const float *A, const float *B, float *C) {
	sub2_0_0(A, B, C);
}
void hp_5_5(const float *A, const float *B, float *C) {
	hp_0_0(A, B, C);
}
void ihp_5_5(const float *A, const float *B, float *C) {
	ihp_0_0(A, B, C);
}
int equals_5_5(const float *A, const float *B, float eps) {
	return equals_0_0(A, B, eps);
}
int zeroGroup_5(const float *A, float eps) {
	return zeroGroup_0(A, eps);
}
void dual_default_0_5(const float *A, float *C) {
	C[0] = -A[0];
}
void undual_default_0_5(const float *A, float *C) {
	C[0] = A[0];
}
void dual_default_1_4(const float *A, float *C) {
	C[0] = A[0];
	C[1] = A[3];
	C[2] = -A[2];
	C[3] = A[1];
	C[4] = A[4];
}
void undual_default_1_4(const float *A, float *C) {
	C[0] = -A[0];
	C[1] = -A[3];
	C[2] = A[2];
	C[3] = -A[1];
	C[4] = -A[4];
}
void dual_default_2_3(const float *A, float *C) {
	C[0] = A[3];
	C[1] = -A[1];
	C[2] = A[0];
	C[3] = A[6];
	C[4] = A[5];
	C[5] = -A[4];
	C[6] = A[9];
	C[7] = A[2];
	C[8] = -A[8];
	C[9] = A[7];
}
void undual_default_2_3(const float *A, float *C) {
	C[0] = -A[3];
	C[1] = A[1];
	C[2] = -A[0];
	C[3] = -A[6];
	C[4] = -A[5];
	C[5] = A[4];
	C[6] = -A[9];
	C[7] = -A[2];
	C[8] = A[8];
	C[9] = -A[7];
}
void dual_default_3_2(const float *A, float *C) {
	C[0] = -A[2];
	C[1] = A[1];
	C[2] = -A[7];
	C[3] = -A[0];
	C[4] = A[5];
	C[5] = -A[4];
	C[6] = -A[3];
	C[7] = -A[9];
	C[8] = A[8];
	C[9] = -A[6];
}
void undual_default_3_2(const float *A, float *C) {
	C[0] = A[2];
	C[1] = -A[1];
	C[2] = A[7];
	C[3] = A[0];
	C[4] = -A[5];
	C[5] = A[4];
	C[6] = A[3];
	C[7] = A[9];
	C[8] = -A[8];
	C[9] = A[6];
}
void dual_default_4_1(const float *A, float *C) {
	undual_default_1_4(A, C);
}
void undual_default_4_1(const float *A, float *C) {
	dual_default_1_4(A, C);
}
void dual_default_5_0(const float *A, float *C) {
	undual_default_0_5(A, C);
}
void undual_default_5_0(const float *A, float *C) {
	dual_default_0_5(A, C);
}
void dual__internal_euclidean_metric__0_5(const float *A, float *C) {
	undual_default_0_5(A, C);
}
void undual__internal_euclidean_metric__0_5(const float *A, float *C) {
	undual_default_0_5(A, C);
}
void dual__internal_euclidean_metric__1_4(const float *A, float *C) {
	C[0] = A[4];
	C[1] = -A[3];
	C[2] = A[2];
	C[3] = -A[1];
	C[4] = A[0];
}
void undual__internal_euclidean_metric__1_4(const float *A, float *C) {
	dual__internal_euclidean_metric__1_4(A, C);
}
void dual__internal_euclidean_metric__2_3(const float *A, float *C) {
	C[0] = -A[9];
	C[1] = A[8];
	C[2] = -A[7];
	C[3] = A[6];
	C[4] = -A[5];
	C[5] = A[4];
	C[6] = -A[3];
	C[7] = -A[2];
	C[8] = A[1];
	C[9] = -A[0];
}
void undual__internal_euclidean_metric__2_3(const float *A, float *C) {
	dual__internal_euclidean_metric__2_3(A, C);
}
void dual__internal_euclidean_metric__3_2(const float *A, float *C) {
	C[0] = -A[9];
	C[1] = A[8];
	C[2] = -A[7];
	C[3] = -A[6];
	C[4] = A[5];
	C[5] = -A[4];
	C[6] = A[3];
	C[7] = -A[2];
	C[8] = A[1];
	C[9] = -A[0];
}
void undual__internal_euclidean_metric__3_2(const float *A, float *C) {
	dual__internal_euclidean_metric__3_2(A, C);
}
void dual__internal_euclidean_metric__4_1(const float *A, float *C) {
	dual__internal_euclidean_metric__1_4(A, C);
}
void undual__internal_euclidean_metric__4_1(const float *A, float *C) {
	dual__internal_euclidean_metric__1_4(A, C);
}
void dual__internal_euclidean_metric__5_0(const float *A, float *C) {
	undual_default_0_5(A, C);
}
void undual__internal_euclidean_metric__5_0(const float *A, float *C) {
	undual_default_0_5(A, C);
}
void mv_setZero(mv *M) {
	M->gu = 0;
}
void mv_setScalar(mv *M, float val) {
	M->gu = 1;
	M->c[0] = val;
}
void mv_setArray(mv *M, int gu, const float *arr) {
	M->gu = gu;
	c3ga_float_copy_N(M->c, arr, c3ga_mvSize[gu]);

}
mv* mv_copy(mv *dst, const mv *src) {
	int i;
	dst->gu = src->gu;
	for (i = 0; i < c3ga_mvSize[src->gu]; i++)
		dst->c[i] = (float)src->c[i];
	return dst;
}
normalizedPoint *mv_to_normalizedPoint(normalizedPoint *dst, const mv *src) {
	const float *ptr = src->c;

	if (src->gu & 1) {
		ptr += 1;
	}
	if (src->gu & 2) {
		dst->c[0] = ptr[1];
		dst->c[1] = ptr[2];
		dst->c[2] = ptr[3];
		dst->c[3] = ptr[4];
		ptr += 5;
	}
	else {
		dst->c[0] = 0.0f;
		dst->c[1] = 0.0f;
		dst->c[2] = 0.0f;
		dst->c[3] = 0.0f;
	}
	return dst;
}
flatPoint *mv_to_flatPoint(flatPoint *dst, const mv *src) {
	const float *ptr = src->c;

	if (src->gu & 1) {
		ptr += 1;
	}
	if (src->gu & 2) {
		ptr += 5;
	}
	if (src->gu & 4) {
		dst->c[0] = ptr[7];
		dst->c[1] = ptr[8];
		dst->c[2] = ptr[9];
		dst->c[3] = ptr[6];
		ptr += 10;
	}
	else {
		dst->c[0] = 0.0f;
		dst->c[1] = 0.0f;
		dst->c[2] = 0.0f;
		dst->c[3] = 0.0f;
	}
	return dst;
}
line *mv_to_line(line *dst, const mv *src) {
	const float *ptr = src->c;

	if (src->gu & 1) {
		ptr += 1;
	}
	if (src->gu & 2) {
		ptr += 5;
	}
	if (src->gu & 4) {
		ptr += 10;
	}
	if (src->gu & 8) {
		dst->c[0] = ptr[6];
		dst->c[1] = ptr[8];
		dst->c[2] = ptr[9];
		dst->c[3] = -ptr[4];
		dst->c[4] = -ptr[5];
		dst->c[5] = -ptr[7];
		ptr += 10;
	}
	else {
		dst->c[0] = 0.0f;
		dst->c[1] = 0.0f;
		dst->c[2] = 0.0f;
		dst->c[3] = 0.0f;
		dst->c[4] = 0.0f;
		dst->c[5] = 0.0f;
	}
	return dst;
}
dualLine *mv_to_dualLine(dualLine *dst, const mv *src) {
	const float *ptr = src->c;

	if (src->gu & 1) {
		ptr += 1;
	}
	if (src->gu & 2) {
		ptr += 5;
	}
	if (src->gu & 4) {
		dst->c[0] = ptr[2];
		dst->c[1] = ptr[4];
		dst->c[2] = ptr[5];
		dst->c[3] = ptr[7];
		dst->c[4] = ptr[8];
		dst->c[5] = ptr[9];
		ptr += 10;
	}
	else {
		dst->c[0] = 0.0f;
		dst->c[1] = 0.0f;
		dst->c[2] = 0.0f;
		dst->c[3] = 0.0f;
		dst->c[4] = 0.0f;
		dst->c[5] = 0.0f;
	}
	return dst;
}
plane *mv_to_plane(plane *dst, const mv *src) {
	const float *ptr = src->c;

	if (src->gu & 1) {
		ptr += 1;
	}
	if (src->gu & 2) {
		ptr += 5;
	}
	if (src->gu & 4) {
		ptr += 10;
	}
	if (src->gu & 8) {
		ptr += 10;
	}
	if (src->gu & 16) {
		dst->c[0] = ptr[4];
		dst->c[1] = ptr[3];
		dst->c[2] = ptr[2];
		dst->c[3] = ptr[1];
		ptr += 5;
	}
	else {
		dst->c[0] = 0.0f;
		dst->c[1] = 0.0f;
		dst->c[2] = 0.0f;
		dst->c[3] = 0.0f;
	}
	return dst;
}
no_t *mv_to_no_t(no_t *dst, const mv *src) {
	const float *ptr = src->c;

	if (src->gu & 1) {
		ptr += 1;
	}
	if (src->gu & 2) {
		ptr += 5;
	}
	else {
	}
	return dst;
}
e1_t *mv_to_e1_t(e1_t *dst, const mv *src) {
	const float *ptr = src->c;

	if (src->gu & 1) {
		ptr += 1;
	}
	if (src->gu & 2) {
		ptr += 5;
	}
	else {
	}
	return dst;
}
e2_t *mv_to_e2_t(e2_t *dst, const mv *src) {
	const float *ptr = src->c;

	if (src->gu & 1) {
		ptr += 1;
	}
	if (src->gu & 2) {
		ptr += 5;
	}
	else {
	}
	return dst;
}
e3_t *mv_to_e3_t(e3_t *dst, const mv *src) {
	const float *ptr = src->c;

	if (src->gu & 1) {
		ptr += 1;
	}
	if (src->gu & 2) {
		ptr += 5;
	}
	else {
	}
	return dst;
}
ni_t *mv_to_ni_t(ni_t *dst, const mv *src) {
	const float *ptr = src->c;

	if (src->gu & 1) {
		ptr += 1;
	}
	if (src->gu & 2) {
		ptr += 5;
	}
	else {
	}
	return dst;
}
mv *normalizedPoint_to_mv(mv *dst, const normalizedPoint *src) {
	float *ptr = dst->c;
	dst->gu = 2;
	ptr[0] = 1.0f;
	ptr[1] = src->c[0];
	ptr[2] = src->c[1];
	ptr[3] = src->c[2];
	ptr[4] = src->c[3];
	ptr += 5;
	return dst;
}
mv *flatPoint_to_mv(mv *dst, const flatPoint *src) {
	float *ptr = dst->c;
	dst->gu = 4;
	ptr[0] = ptr[1] = ptr[2] = ptr[3] = ptr[4] = ptr[5] = 0.0f;
	ptr[6] = src->c[3];
	ptr[7] = src->c[0];
	ptr[8] = src->c[1];
	ptr[9] = src->c[2];
	ptr += 10;
	return dst;
}
mv *line_to_mv(mv *dst, const line *src) {
	float *ptr = dst->c;
	dst->gu = 8;
	ptr[0] = ptr[1] = ptr[2] = ptr[3] = 0.0f;
	ptr[4] = -src->c[3];
	ptr[5] = -src->c[4];
	ptr[6] = src->c[0];
	ptr[7] = -src->c[5];
	ptr[8] = src->c[1];
	ptr[9] = src->c[2];
	ptr += 10;
	return dst;
}
mv *dualLine_to_mv(mv *dst, const dualLine *src) {
	float *ptr = dst->c;
	dst->gu = 4;
	ptr[0] = ptr[1] = ptr[3] = ptr[6] = 0.0f;
	ptr[2] = src->c[0];
	ptr[4] = src->c[1];
	ptr[5] = src->c[2];
	ptr[7] = src->c[3];
	ptr[8] = src->c[4];
	ptr[9] = src->c[5];
	ptr += 10;
	return dst;
}
mv *plane_to_mv(mv *dst, const plane *src) {
	float *ptr = dst->c;
	dst->gu = 16;
	ptr[0] = 0.0f;
	ptr[1] = src->c[3];
	ptr[2] = src->c[2];
	ptr[3] = src->c[1];
	ptr[4] = src->c[0];
	ptr += 5;
	return dst;
}
mv *no_t_to_mv(mv *dst, const no_t *src) {
	float *ptr = dst->c;
	dst->gu = 2;
	ptr[0] = 1.0f;
	ptr[1] = ptr[2] = ptr[3] = ptr[4] = 0.0f;
	ptr += 5;
	return dst;
}
mv *e1_t_to_mv(mv *dst, const e1_t *src) {
	float *ptr = dst->c;
	dst->gu = 2;
	ptr[0] = ptr[2] = ptr[3] = ptr[4] = 0.0f;
	ptr[1] = 1.0f;
	ptr += 5;
	return dst;
}
mv *e2_t_to_mv(mv *dst, const e2_t *src) {
	float *ptr = dst->c;
	dst->gu = 2;
	ptr[0] = ptr[1] = ptr[3] = ptr[4] = 0.0f;
	ptr[2] = 1.0f;
	ptr += 5;
	return dst;
}
mv *e3_t_to_mv(mv *dst, const e3_t *src) {
	float *ptr = dst->c;
	dst->gu = 2;
	ptr[0] = ptr[1] = ptr[2] = ptr[4] = 0.0f;
	ptr[3] = 1.0f;
	ptr += 5;
	return dst;
}
mv *ni_t_to_mv(mv *dst, const ni_t *src) {
	float *ptr = dst->c;
	dst->gu = 2;
	ptr[0] = ptr[1] = ptr[2] = ptr[3] = 0.0f;
	ptr[4] = 1.0f;
	ptr += 5;
	return dst;
}

void mv_reserveGroup_0(mv *A) {
	int groupUsageBelow, groupUsageAbove, newGroupUsage, newGroupUsageBelowNextGroup;
	int i;
	float *dst, *src;
	if ((A->gu & 1) == 0) {

		groupUsageBelow = A->gu & 0;
		groupUsageAbove = A->gu ^ groupUsageBelow;
		newGroupUsage = A->gu | 1;
		newGroupUsageBelowNextGroup = newGroupUsage & 1;

		dst = A->c + c3ga_mvSize[newGroupUsageBelowNextGroup];
		src = A->c + c3ga_mvSize[groupUsageBelow];
		for (i = c3ga_mvSize[groupUsageAbove]-1; i >= 0; i--) /* work from end to start of array to avoid overwriting (dst is always beyond src) */
			dst[i] = src[i];
		c3ga_float_zero_1(A->c);

		A->gu = newGroupUsage;
	}
}

void mv_reserveGroup_1(mv *A) {
	int groupUsageBelow, groupUsageAbove, newGroupUsage, newGroupUsageBelowNextGroup;
	int i;
	float *dst, *src;
	if ((A->gu & 2) == 0) {

		groupUsageBelow = A->gu & 1;
		groupUsageAbove = A->gu ^ groupUsageBelow;
		newGroupUsage = A->gu | 2;
		newGroupUsageBelowNextGroup = newGroupUsage & 3;

		dst = A->c + c3ga_mvSize[newGroupUsageBelowNextGroup];
		src = A->c + c3ga_mvSize[groupUsageBelow];
		for (i = c3ga_mvSize[groupUsageAbove]-1; i >= 0; i--) /* work from end to start of array to avoid overwriting (dst is always beyond src) */
			dst[i] = src[i];
		c3ga_float_zero_5(A->c);

		A->gu = newGroupUsage;
	}
}

void mv_reserveGroup_2(mv *A) {
	int groupUsageBelow, groupUsageAbove, newGroupUsage, newGroupUsageBelowNextGroup;
	int i;
	float *dst, *src;
	if ((A->gu & 4) == 0) {

		groupUsageBelow = A->gu & 3;
		groupUsageAbove = A->gu ^ groupUsageBelow;
		newGroupUsage = A->gu | 4;
		newGroupUsageBelowNextGroup = newGroupUsage & 7;

		dst = A->c + c3ga_mvSize[newGroupUsageBelowNextGroup];
		src = A->c + c3ga_mvSize[groupUsageBelow];
		for (i = c3ga_mvSize[groupUsageAbove]-1; i >= 0; i--) /* work from end to start of array to avoid overwriting (dst is always beyond src) */
			dst[i] = src[i];
		c3ga_float_zero_10(A->c);

		A->gu = newGroupUsage;
	}
}

void mv_reserveGroup_3(mv *A) {
	int groupUsageBelow, groupUsageAbove, newGroupUsage, newGroupUsageBelowNextGroup;
	int i;
	float *dst, *src;
	if ((A->gu & 8) == 0) {

		groupUsageBelow = A->gu & 7;
		groupUsageAbove = A->gu ^ groupUsageBelow;
		newGroupUsage = A->gu | 8;
		newGroupUsageBelowNextGroup = newGroupUsage & 15;

		dst = A->c + c3ga_mvSize[newGroupUsageBelowNextGroup];
		src = A->c + c3ga_mvSize[groupUsageBelow];
		for (i = c3ga_mvSize[groupUsageAbove]-1; i >= 0; i--) /* work from end to start of array to avoid overwriting (dst is always beyond src) */
			dst[i] = src[i];
		c3ga_float_zero_10(A->c);

		A->gu = newGroupUsage;
	}
}

void mv_reserveGroup_4(mv *A) {
	int groupUsageBelow, groupUsageAbove, newGroupUsage, newGroupUsageBelowNextGroup;
	int i;
	float *dst, *src;
	if ((A->gu & 16) == 0) {

		groupUsageBelow = A->gu & 15;
		groupUsageAbove = A->gu ^ groupUsageBelow;
		newGroupUsage = A->gu | 16;
		newGroupUsageBelowNextGroup = newGroupUsage & 31;

		dst = A->c + c3ga_mvSize[newGroupUsageBelowNextGroup];
		src = A->c + c3ga_mvSize[groupUsageBelow];
		for (i = c3ga_mvSize[groupUsageAbove]-1; i >= 0; i--) /* work from end to start of array to avoid overwriting (dst is always beyond src) */
			dst[i] = src[i];
		c3ga_float_zero_5(A->c);

		A->gu = newGroupUsage;
	}
}

void mv_reserveGroup_5(mv *A) {
	int groupUsageBelow, groupUsageAbove, newGroupUsage, newGroupUsageBelowNextGroup;
	if ((A->gu & 32) == 0) {

		groupUsageBelow = A->gu & 31;
		groupUsageAbove = A->gu ^ groupUsageBelow;
		newGroupUsage = A->gu | 32;
		newGroupUsageBelowNextGroup = newGroupUsage & 63;

		c3ga_float_zero_1(A->c);

		A->gu = newGroupUsage;
	}
}

float mv_scalar(const mv *A) {
	return (A->gu & 1) ? A->c[c3ga_mvSize[A->gu & 0] + 0] : 0.0f;
}
float mv_float(const mv *A) {
	return mv_scalar(A);
}

void mv_set_scalar(mv *A, float scalar_coord) {
	mv_reserveGroup_0(A);
	A->c[c3ga_mvSize[A->gu & 0] + 0] = scalar_coord;
}

float mv_no(const mv *A) {
	return (A->gu & 2) ? A->c[c3ga_mvSize[A->gu & 1] + 0] : 0.0f;
}

void mv_set_no(mv *A, float no_coord) {
	mv_reserveGroup_1(A);
	A->c[c3ga_mvSize[A->gu & 1] + 0] = no_coord;
}

float mv_e1(const mv *A) {
	return (A->gu & 2) ? A->c[c3ga_mvSize[A->gu & 1] + 1] : 0.0f;
}

void mv_set_e1(mv *A, float e1_coord) {
	mv_reserveGroup_1(A);
	A->c[c3ga_mvSize[A->gu & 1] + 1] = e1_coord;
}

float mv_e2(const mv *A) {
	return (A->gu & 2) ? A->c[c3ga_mvSize[A->gu & 1] + 2] : 0.0f;
}

void mv_set_e2(mv *A, float e2_coord) {
	mv_reserveGroup_1(A);
	A->c[c3ga_mvSize[A->gu & 1] + 2] = e2_coord;
}

float mv_e3(const mv *A) {
	return (A->gu & 2) ? A->c[c3ga_mvSize[A->gu & 1] + 3] : 0.0f;
}

void mv_set_e3(mv *A, float e3_coord) {
	mv_reserveGroup_1(A);
	A->c[c3ga_mvSize[A->gu & 1] + 3] = e3_coord;
}

float mv_ni(const mv *A) {
	return (A->gu & 2) ? A->c[c3ga_mvSize[A->gu & 1] + 4] : 0.0f;
}

void mv_set_ni(mv *A, float ni_coord) {
	mv_reserveGroup_1(A);
	A->c[c3ga_mvSize[A->gu & 1] + 4] = ni_coord;
}

float mv_no_e1(const mv *A) {
	return (A->gu & 4) ? A->c[c3ga_mvSize[A->gu & 3] + 0] : 0.0f;
}

void mv_set_no_e1(mv *A, float no_e1_coord) {
	mv_reserveGroup_2(A);
	A->c[c3ga_mvSize[A->gu & 3] + 0] = no_e1_coord;
}

float mv_no_e2(const mv *A) {
	return (A->gu & 4) ? A->c[c3ga_mvSize[A->gu & 3] + 1] : 0.0f;
}

void mv_set_no_e2(mv *A, float no_e2_coord) {
	mv_reserveGroup_2(A);
	A->c[c3ga_mvSize[A->gu & 3] + 1] = no_e2_coord;
}

float mv_e1_e2(const mv *A) {
	return (A->gu & 4) ? A->c[c3ga_mvSize[A->gu & 3] + 2] : 0.0f;
}

void mv_set_e1_e2(mv *A, float e1_e2_coord) {
	mv_reserveGroup_2(A);
	A->c[c3ga_mvSize[A->gu & 3] + 2] = e1_e2_coord;
}

float mv_no_e3(const mv *A) {
	return (A->gu & 4) ? A->c[c3ga_mvSize[A->gu & 3] + 3] : 0.0f;
}

void mv_set_no_e3(mv *A, float no_e3_coord) {
	mv_reserveGroup_2(A);
	A->c[c3ga_mvSize[A->gu & 3] + 3] = no_e3_coord;
}

float mv_e1_e3(const mv *A) {
	return (A->gu & 4) ? A->c[c3ga_mvSize[A->gu & 3] + 4] : 0.0f;
}

void mv_set_e1_e3(mv *A, float e1_e3_coord) {
	mv_reserveGroup_2(A);
	A->c[c3ga_mvSize[A->gu & 3] + 4] = e1_e3_coord;
}

float mv_e2_e3(const mv *A) {
	return (A->gu & 4) ? A->c[c3ga_mvSize[A->gu & 3] + 5] : 0.0f;
}

void mv_set_e2_e3(mv *A, float e2_e3_coord) {
	mv_reserveGroup_2(A);
	A->c[c3ga_mvSize[A->gu & 3] + 5] = e2_e3_coord;
}

float mv_no_ni(const mv *A) {
	return (A->gu & 4) ? A->c[c3ga_mvSize[A->gu & 3] + 6] : 0.0f;
}

void mv_set_no_ni(mv *A, float no_ni_coord) {
	mv_reserveGroup_2(A);
	A->c[c3ga_mvSize[A->gu & 3] + 6] = no_ni_coord;
}

float mv_e1_ni(const mv *A) {
	return (A->gu & 4) ? A->c[c3ga_mvSize[A->gu & 3] + 7] : 0.0f;
}

void mv_set_e1_ni(mv *A, float e1_ni_coord) {
	mv_reserveGroup_2(A);
	A->c[c3ga_mvSize[A->gu & 3] + 7] = e1_ni_coord;
}

float mv_e2_ni(const mv *A) {
	return (A->gu & 4) ? A->c[c3ga_mvSize[A->gu & 3] + 8] : 0.0f;
}

void mv_set_e2_ni(mv *A, float e2_ni_coord) {
	mv_reserveGroup_2(A);
	A->c[c3ga_mvSize[A->gu & 3] + 8] = e2_ni_coord;
}

float mv_e3_ni(const mv *A) {
	return (A->gu & 4) ? A->c[c3ga_mvSize[A->gu & 3] + 9] : 0.0f;
}

void mv_set_e3_ni(mv *A, float e3_ni_coord) {
	mv_reserveGroup_2(A);
	A->c[c3ga_mvSize[A->gu & 3] + 9] = e3_ni_coord;
}

float mv_no_e1_e2(const mv *A) {
	return (A->gu & 8) ? A->c[c3ga_mvSize[A->gu & 7] + 0] : 0.0f;
}

void mv_set_no_e1_e2(mv *A, float no_e1_e2_coord) {
	mv_reserveGroup_3(A);
	A->c[c3ga_mvSize[A->gu & 7] + 0] = no_e1_e2_coord;
}

float mv_no_e1_e3(const mv *A) {
	return (A->gu & 8) ? A->c[c3ga_mvSize[A->gu & 7] + 1] : 0.0f;
}

void mv_set_no_e1_e3(mv *A, float no_e1_e3_coord) {
	mv_reserveGroup_3(A);
	A->c[c3ga_mvSize[A->gu & 7] + 1] = no_e1_e3_coord;
}

float mv_no_e2_e3(const mv *A) {
	return (A->gu & 8) ? A->c[c3ga_mvSize[A->gu & 7] + 2] : 0.0f;
}

void mv_set_no_e2_e3(mv *A, float no_e2_e3_coord) {
	mv_reserveGroup_3(A);
	A->c[c3ga_mvSize[A->gu & 7] + 2] = no_e2_e3_coord;
}

float mv_e1_e2_e3(const mv *A) {
	return (A->gu & 8) ? A->c[c3ga_mvSize[A->gu & 7] + 3] : 0.0f;
}

void mv_set_e1_e2_e3(mv *A, float e1_e2_e3_coord) {
	mv_reserveGroup_3(A);
	A->c[c3ga_mvSize[A->gu & 7] + 3] = e1_e2_e3_coord;
}

float mv_no_e1_ni(const mv *A) {
	return (A->gu & 8) ? A->c[c3ga_mvSize[A->gu & 7] + 4] : 0.0f;
}

void mv_set_no_e1_ni(mv *A, float no_e1_ni_coord) {
	mv_reserveGroup_3(A);
	A->c[c3ga_mvSize[A->gu & 7] + 4] = no_e1_ni_coord;
}

float mv_no_e2_ni(const mv *A) {
	return (A->gu & 8) ? A->c[c3ga_mvSize[A->gu & 7] + 5] : 0.0f;
}

void mv_set_no_e2_ni(mv *A, float no_e2_ni_coord) {
	mv_reserveGroup_3(A);
	A->c[c3ga_mvSize[A->gu & 7] + 5] = no_e2_ni_coord;
}

float mv_e1_e2_ni(const mv *A) {
	return (A->gu & 8) ? A->c[c3ga_mvSize[A->gu & 7] + 6] : 0.0f;
}

void mv_set_e1_e2_ni(mv *A, float e1_e2_ni_coord) {
	mv_reserveGroup_3(A);
	A->c[c3ga_mvSize[A->gu & 7] + 6] = e1_e2_ni_coord;
}

float mv_no_e3_ni(const mv *A) {
	return (A->gu & 8) ? A->c[c3ga_mvSize[A->gu & 7] + 7] : 0.0f;
}

void mv_set_no_e3_ni(mv *A, float no_e3_ni_coord) {
	mv_reserveGroup_3(A);
	A->c[c3ga_mvSize[A->gu & 7] + 7] = no_e3_ni_coord;
}

float mv_e1_e3_ni(const mv *A) {
	return (A->gu & 8) ? A->c[c3ga_mvSize[A->gu & 7] + 8] : 0.0f;
}

void mv_set_e1_e3_ni(mv *A, float e1_e3_ni_coord) {
	mv_reserveGroup_3(A);
	A->c[c3ga_mvSize[A->gu & 7] + 8] = e1_e3_ni_coord;
}

float mv_e2_e3_ni(const mv *A) {
	return (A->gu & 8) ? A->c[c3ga_mvSize[A->gu & 7] + 9] : 0.0f;
}

void mv_set_e2_e3_ni(mv *A, float e2_e3_ni_coord) {
	mv_reserveGroup_3(A);
	A->c[c3ga_mvSize[A->gu & 7] + 9] = e2_e3_ni_coord;
}

float mv_no_e1_e2_e3(const mv *A) {
	return (A->gu & 16) ? A->c[c3ga_mvSize[A->gu & 15] + 0] : 0.0f;
}

void mv_set_no_e1_e2_e3(mv *A, float no_e1_e2_e3_coord) {
	mv_reserveGroup_4(A);
	A->c[c3ga_mvSize[A->gu & 15] + 0] = no_e1_e2_e3_coord;
}

float mv_no_e1_e2_ni(const mv *A) {
	return (A->gu & 16) ? A->c[c3ga_mvSize[A->gu & 15] + 1] : 0.0f;
}

void mv_set_no_e1_e2_ni(mv *A, float no_e1_e2_ni_coord) {
	mv_reserveGroup_4(A);
	A->c[c3ga_mvSize[A->gu & 15] + 1] = no_e1_e2_ni_coord;
}

float mv_no_e1_e3_ni(const mv *A) {
	return (A->gu & 16) ? A->c[c3ga_mvSize[A->gu & 15] + 2] : 0.0f;
}

void mv_set_no_e1_e3_ni(mv *A, float no_e1_e3_ni_coord) {
	mv_reserveGroup_4(A);
	A->c[c3ga_mvSize[A->gu & 15] + 2] = no_e1_e3_ni_coord;
}

float mv_no_e2_e3_ni(const mv *A) {
	return (A->gu & 16) ? A->c[c3ga_mvSize[A->gu & 15] + 3] : 0.0f;
}

void mv_set_no_e2_e3_ni(mv *A, float no_e2_e3_ni_coord) {
	mv_reserveGroup_4(A);
	A->c[c3ga_mvSize[A->gu & 15] + 3] = no_e2_e3_ni_coord;
}

float mv_e1_e2_e3_ni(const mv *A) {
	return (A->gu & 16) ? A->c[c3ga_mvSize[A->gu & 15] + 4] : 0.0f;
}

void mv_set_e1_e2_e3_ni(mv *A, float e1_e2_e3_ni_coord) {
	mv_reserveGroup_4(A);
	A->c[c3ga_mvSize[A->gu & 15] + 4] = e1_e2_e3_ni_coord;
}

float mv_no_e1_e2_e3_ni(const mv *A) {
	return (A->gu & 32) ? A->c[c3ga_mvSize[A->gu & 31] + 0] : 0.0f;
}

void mv_set_no_e1_e2_e3_ni(mv *A, float no_e1_e2_e3_ni_coord) {
	mv_reserveGroup_5(A);
	A->c[c3ga_mvSize[A->gu & 31] + 0] = no_e1_e2_e3_ni_coord;
}

float mv_largestCoordinate(const mv *x) {
	float maxValue = 0.0f;
	int nbC = c3ga_mvSize[x->gu], i;
	for (i = 0; i < nbC; i++)
		if (fabsf(x->c[i]) > maxValue) maxValue = fabsf(x->c[i]);
	return maxValue;
}

float mv_largestBasisBlade(const mv *x, unsigned int *bm) {
	int nc = c3ga_mvSize[x->gu];
	float maxC = -1.0f, C;

	int idx = 0;
	int group = 0;
	int i = 0, j;
	*bm = 0;

	while (i < nc) {
		if (x->gu & (1 << group)) {
			for (j = 0; j < c3ga_groupSize[group]; j++) {
				C = fabsf(x->c[i]);
				if (C > maxC) {
					maxC = C;
					*bm = c3ga_basisElementBitmapByIndex[idx];
				}
				idx++;
				i++;
			}
		}
		else idx += c3ga_groupSize[group];
		group++;
	}

	return maxC;
} /* end of mv::largestBasisBlade() */



normalizedPoint* normalizedPoint_setZero(normalizedPoint *_dst)
{
	_dst->c[0] = _dst->c[1] = _dst->c[2] = _dst->c[3] = 0.0f;

	return _dst;
}
flatPoint* flatPoint_setZero(flatPoint *_dst)
{
	_dst->c[0] = _dst->c[1] = _dst->c[2] = _dst->c[3] = 0.0f;

	return _dst;
}
line* line_setZero(line *_dst)
{
	_dst->c[0] = _dst->c[1] = _dst->c[2] = _dst->c[3] = _dst->c[4] = _dst->c[5] = 0.0f;

	return _dst;
}
dualLine* dualLine_setZero(dualLine *_dst)
{
	_dst->c[0] = _dst->c[1] = _dst->c[2] = _dst->c[3] = _dst->c[4] = _dst->c[5] = 0.0f;

	return _dst;
}
plane* plane_setZero(plane *_dst)
{
	_dst->c[0] = _dst->c[1] = _dst->c[2] = _dst->c[3] = 0.0f;

	return _dst;
}


normalizedPoint* normalizedPoint_set(normalizedPoint *_dst, const float _e1, const float _e2, const float _e3, const float _ni)
{
	_dst->c[0] = _e1;
	_dst->c[1] = _e2;
	_dst->c[2] = _e3;
	_dst->c[3] = _ni;

	return _dst;
}
flatPoint* flatPoint_set(flatPoint *_dst, const float _e1_ni, const float _e2_ni, const float _e3_ni, const float _no_ni)
{
	_dst->c[0] = _e1_ni;
	_dst->c[1] = _e2_ni;
	_dst->c[2] = _e3_ni;
	_dst->c[3] = _no_ni;

	return _dst;
}
line* line_set(line *_dst, const float _e1_e2_ni, const float _e1_e3_ni, const float _e2_e3_ni, const float _e1_no_ni, const float _e2_no_ni, const float _e3_no_ni)
{
	_dst->c[0] = _e1_e2_ni;
	_dst->c[1] = _e1_e3_ni;
	_dst->c[2] = _e2_e3_ni;
	_dst->c[3] = _e1_no_ni;
	_dst->c[4] = _e2_no_ni;
	_dst->c[5] = _e3_no_ni;

	return _dst;
}
dualLine* dualLine_set(dualLine *_dst, const float _e1_e2, const float _e1_e3, const float _e2_e3, const float _e1_ni, const float _e2_ni, const float _e3_ni)
{
	_dst->c[0] = _e1_e2;
	_dst->c[1] = _e1_e3;
	_dst->c[2] = _e2_e3;
	_dst->c[3] = _e1_ni;
	_dst->c[4] = _e2_ni;
	_dst->c[5] = _e3_ni;

	return _dst;
}
plane* plane_set(plane *_dst, const float _e1_e2_e3_ni, const float _no_e2_e3_ni, const float _no_e1_e3_ni, const float _no_e1_e2_ni)
{
	_dst->c[0] = _e1_e2_e3_ni;
	_dst->c[1] = _no_e2_e3_ni;
	_dst->c[2] = _no_e1_e3_ni;
	_dst->c[3] = _no_e1_e2_ni;

	return _dst;
}

normalizedPoint* normalizedPoint_setArray(normalizedPoint *_dst, const float *A)
{
	_dst->c[0] = A[0];
	_dst->c[1] = A[1];
	_dst->c[2] = A[2];
	_dst->c[3] = A[3];

	return _dst;
}
flatPoint* flatPoint_setArray(flatPoint *_dst, const float *A)
{
	_dst->c[0] = A[0];
	_dst->c[1] = A[1];
	_dst->c[2] = A[2];
	_dst->c[3] = A[3];

	return _dst;
}
line* line_setArray(line *_dst, const float *A)
{
	_dst->c[0] = A[0];
	_dst->c[1] = A[1];
	_dst->c[2] = A[2];
	_dst->c[3] = A[3];
	_dst->c[4] = A[4];
	_dst->c[5] = A[5];

	return _dst;
}
dualLine* dualLine_setArray(dualLine *_dst, const float *A)
{
	_dst->c[0] = A[0];
	_dst->c[1] = A[1];
	_dst->c[2] = A[2];
	_dst->c[3] = A[3];
	_dst->c[4] = A[4];
	_dst->c[5] = A[5];

	return _dst;
}
plane* plane_setArray(plane *_dst, const float *A)
{
	_dst->c[0] = A[0];
	_dst->c[1] = A[1];
	_dst->c[2] = A[2];
	_dst->c[3] = A[3];

	return _dst;
}

normalizedPoint* normalizedPoint_copy(normalizedPoint *_dst, const normalizedPoint *a)
{
	_dst->c[0] = a->c[0];
	_dst->c[1] = a->c[1];
	_dst->c[2] = a->c[2];
	_dst->c[3] = a->c[3];

	return _dst;
}
flatPoint* flatPoint_copy(flatPoint *_dst, const flatPoint *a)
{
	_dst->c[0] = a->c[0];
	_dst->c[1] = a->c[1];
	_dst->c[2] = a->c[2];
	_dst->c[3] = a->c[3];

	return _dst;
}
line* line_copy(line *_dst, const line *a)
{
	_dst->c[0] = a->c[0];
	_dst->c[1] = a->c[1];
	_dst->c[2] = a->c[2];
	_dst->c[3] = a->c[3];
	_dst->c[4] = a->c[4];
	_dst->c[5] = a->c[5];

	return _dst;
}
dualLine* dualLine_copy(dualLine *_dst, const dualLine *a)
{
	_dst->c[0] = a->c[0];
	_dst->c[1] = a->c[1];
	_dst->c[2] = a->c[2];
	_dst->c[3] = a->c[3];
	_dst->c[4] = a->c[4];
	_dst->c[5] = a->c[5];

	return _dst;
}
plane* plane_copy(plane *_dst, const plane *a)
{
	_dst->c[0] = a->c[0];
	_dst->c[1] = a->c[1];
	_dst->c[2] = a->c[2];
	_dst->c[3] = a->c[3];

	return _dst;
}


float normalizedPoint_largestCoordinate(const normalizedPoint *x) {
	float maxValue = 1.0f;
	if (fabsf(x->c[0]) > maxValue) maxValue = fabsf(x->c[0]);
	if (fabsf(x->c[1]) > maxValue) maxValue = fabsf(x->c[1]);
	if (fabsf(x->c[2]) > maxValue) maxValue = fabsf(x->c[2]);
	if (fabsf(x->c[3]) > maxValue) maxValue = fabsf(x->c[3]);
	return maxValue;
}
float flatPoint_largestCoordinate(const flatPoint *x) {
	float maxValue = fabsf(x->c[0]);
	if (fabsf(x->c[1]) > maxValue) maxValue = fabsf(x->c[1]);
	if (fabsf(x->c[2]) > maxValue) maxValue = fabsf(x->c[2]);
	if (fabsf(x->c[3]) > maxValue) maxValue = fabsf(x->c[3]);
	return maxValue;
}
float line_largestCoordinate(const line *x) {
	float maxValue = fabsf(x->c[0]);
	if (fabsf(x->c[1]) > maxValue) maxValue = fabsf(x->c[1]);
	if (fabsf(x->c[2]) > maxValue) maxValue = fabsf(x->c[2]);
	if (fabsf(x->c[3]) > maxValue) maxValue = fabsf(x->c[3]);
	if (fabsf(x->c[4]) > maxValue) maxValue = fabsf(x->c[4]);
	if (fabsf(x->c[5]) > maxValue) maxValue = fabsf(x->c[5]);
	return maxValue;
}
float dualLine_largestCoordinate(const dualLine *x) {
	float maxValue = fabsf(x->c[0]);
	if (fabsf(x->c[1]) > maxValue) maxValue = fabsf(x->c[1]);
	if (fabsf(x->c[2]) > maxValue) maxValue = fabsf(x->c[2]);
	if (fabsf(x->c[3]) > maxValue) maxValue = fabsf(x->c[3]);
	if (fabsf(x->c[4]) > maxValue) maxValue = fabsf(x->c[4]);
	if (fabsf(x->c[5]) > maxValue) maxValue = fabsf(x->c[5]);
	return maxValue;
}
float plane_largestCoordinate(const plane *x) {
	float maxValue = fabsf(x->c[0]);
	if (fabsf(x->c[1]) > maxValue) maxValue = fabsf(x->c[1]);
	if (fabsf(x->c[2]) > maxValue) maxValue = fabsf(x->c[2]);
	if (fabsf(x->c[3]) > maxValue) maxValue = fabsf(x->c[3]);
	return maxValue;
}
float no_t_largestCoordinate(const no_t *x) {
	float maxValue = 1.0f;
	return maxValue;
}
float e1_t_largestCoordinate(const e1_t *x) {
	float maxValue = 1.0f;
	return maxValue;
}
float e2_t_largestCoordinate(const e2_t *x) {
	float maxValue = 1.0f;
	return maxValue;
}
float e3_t_largestCoordinate(const e3_t *x) {
	float maxValue = 1.0f;
	return maxValue;
}
float ni_t_largestCoordinate(const ni_t *x) {
	float maxValue = 1.0f;
	return maxValue;
}

float normalizedPoint_float(const normalizedPoint *x) {
	return 0.0f;
}
float flatPoint_float(const flatPoint *x) {
	return 0.0f;
}
float line_float(const line *x) {
	return 0.0f;
}
float dualLine_float(const dualLine *x) {
	return 0.0f;
}
float plane_float(const plane *x) {
	return 0.0f;
}
float no_t_float(const no_t *x) {
	return 0.0f;
}
float e1_t_float(const e1_t *x) {
	return 0.0f;
}
float e2_t_float(const e2_t *x) {
	return 0.0f;
}
float e3_t_float(const e3_t *x) {
	return 0.0f;
}
float ni_t_float(const ni_t *x) {
	return 0.0f;
}
mv* add_mv_mv(mv *_dst, const mv *a, const mv *b)
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
			bidx += 5;
		}
		else copyGroup_1(a->c + aidx, _dst->c + cidx);
		aidx += 5;
		cidx += 5;
	}
	else if (b->gu & 2) {
		copyGroup_1(b->c + bidx, _dst->c + cidx);
		bidx += 5;
		cidx += 5;
	}
	
	if (a->gu & 4) {
		if (b->gu & 4) {
			add2_2_2(a->c + aidx, b->c + bidx, _dst->c + cidx);
			bidx += 10;
		}
		else copyGroup_2(a->c + aidx, _dst->c + cidx);
		aidx += 10;
		cidx += 10;
	}
	else if (b->gu & 4) {
		copyGroup_2(b->c + bidx, _dst->c + cidx);
		bidx += 10;
		cidx += 10;
	}
	
	if (a->gu & 8) {
		if (b->gu & 8) {
			add2_3_3(a->c + aidx, b->c + bidx, _dst->c + cidx);
			bidx += 10;
		}
		else copyGroup_3(a->c + aidx, _dst->c + cidx);
		aidx += 10;
		cidx += 10;
	}
	else if (b->gu & 8) {
		copyGroup_3(b->c + bidx, _dst->c + cidx);
		bidx += 10;
		cidx += 10;
	}
	
	if (a->gu & 16) {
		if (b->gu & 16) {
			add2_4_4(a->c + aidx, b->c + bidx, _dst->c + cidx);
			bidx += 5;
		}
		else copyGroup_4(a->c + aidx, _dst->c + cidx);
		aidx += 5;
		cidx += 5;
	}
	else if (b->gu & 16) {
		copyGroup_4(b->c + bidx, _dst->c + cidx);
		bidx += 5;
		cidx += 5;
	}
	
	if (a->gu & 32) {
		if (b->gu & 32) {
			add2_5_5(a->c + aidx, b->c + bidx, _dst->c + cidx);
		}
		else copyGroup_5(a->c + aidx, _dst->c + cidx);
		cidx += 1;
	}
	else if (b->gu & 32) {
		copyGroup_5(b->c + bidx, _dst->c + cidx);
		cidx += 1;
	}
	return _dst;
}
mv* subtract_mv_mv(mv *_dst, const mv *a, const mv *b)
{
	int aidx = 0, bidx = 0, cidx = 0;
	_dst->gu = a->gu | b->gu;
	
	if (a->gu & 1) {
		if (b->gu & 1) {
			sub2_0_0(a->c + aidx, b->c + bidx, _dst->c + cidx);
			bidx += 1;
		}
		else copyGroup_0(a->c + aidx, _dst->c + cidx);
		aidx += 1;
		cidx += 1;
	}
	else if (b->gu & 1) {
		neg_0(b->c + bidx, _dst->c + cidx);
		bidx += 1;
		cidx += 1;
	}
	
	if (a->gu & 2) {
		if (b->gu & 2) {
			sub2_1_1(a->c + aidx, b->c + bidx, _dst->c + cidx);
			bidx += 5;
		}
		else copyGroup_1(a->c + aidx, _dst->c + cidx);
		aidx += 5;
		cidx += 5;
	}
	else if (b->gu & 2) {
		neg_1(b->c + bidx, _dst->c + cidx);
		bidx += 5;
		cidx += 5;
	}
	
	if (a->gu & 4) {
		if (b->gu & 4) {
			sub2_2_2(a->c + aidx, b->c + bidx, _dst->c + cidx);
			bidx += 10;
		}
		else copyGroup_2(a->c + aidx, _dst->c + cidx);
		aidx += 10;
		cidx += 10;
	}
	else if (b->gu & 4) {
		neg_2(b->c + bidx, _dst->c + cidx);
		bidx += 10;
		cidx += 10;
	}
	
	if (a->gu & 8) {
		if (b->gu & 8) {
			sub2_3_3(a->c + aidx, b->c + bidx, _dst->c + cidx);
			bidx += 10;
		}
		else copyGroup_3(a->c + aidx, _dst->c + cidx);
		aidx += 10;
		cidx += 10;
	}
	else if (b->gu & 8) {
		neg_3(b->c + bidx, _dst->c + cidx);
		bidx += 10;
		cidx += 10;
	}
	
	if (a->gu & 16) {
		if (b->gu & 16) {
			sub2_4_4(a->c + aidx, b->c + bidx, _dst->c + cidx);
			bidx += 5;
		}
		else copyGroup_4(a->c + aidx, _dst->c + cidx);
		aidx += 5;
		cidx += 5;
	}
	else if (b->gu & 16) {
		neg_4(b->c + bidx, _dst->c + cidx);
		bidx += 5;
		cidx += 5;
	}
	
	if (a->gu & 32) {
		if (b->gu & 32) {
			sub2_5_5(a->c + aidx, b->c + bidx, _dst->c + cidx);
		}
		else copyGroup_5(a->c + aidx, _dst->c + cidx);
		cidx += 1;
	}
	else if (b->gu & 32) {
		neg_5(b->c + bidx, _dst->c + cidx);
		cidx += 1;
	}
	return _dst;
}
normalizedPoint* cgaPoint_float_float_float(normalizedPoint *_dst, const float a, const float b, const float c)
{
	_dst->c[0] = a;
	_dst->c[1] = b;
	_dst->c[2] = c;
	_dst->c[3] = (0.5f*a*a+0.5f*b*b+0.5f*c*c);

	return _dst;
}
mv* extractGrade_mv(mv *_dst, const mv *a, const int groupBitmap)
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
			cidx += 5;
		}
		aidx += 5;
	}
	
	if (a->gu & 4) {
		if (groupBitmap & 4) {
			copyGroup_2(a->c + aidx, _dst->c + cidx);
			cidx += 10;
		}
		aidx += 10;
	}
	
	if (a->gu & 8) {
		if (groupBitmap & 8) {
			copyGroup_3(a->c + aidx, _dst->c + cidx);
			cidx += 10;
		}
		aidx += 10;
	}
	
	if (a->gu & 16) {
		if (groupBitmap & 16) {
			copyGroup_4(a->c + aidx, _dst->c + cidx);
			cidx += 5;
		}
		aidx += 5;
	}
	
	if (a->gu & 32) {
		if (groupBitmap & 32) {
			copyGroup_5(a->c + aidx, _dst->c + cidx);
		}
	}
	return _dst;
}
mv* negate_mv(mv *_dst, const mv *a)
{
	int idx = 0;
	_dst->gu = a->gu;
	
	if (a->gu & 1) {
		neg_0(a->c + idx, _dst->c + idx);
		idx += 1;
	}
	
	if (a->gu & 2) {
		neg_1(a->c + idx, _dst->c + idx);
		idx += 5;
	}
	
	if (a->gu & 4) {
		neg_2(a->c + idx, _dst->c + idx);
		idx += 10;
	}
	
	if (a->gu & 8) {
		neg_3(a->c + idx, _dst->c + idx);
		idx += 10;
	}
	
	if (a->gu & 16) {
		neg_4(a->c + idx, _dst->c + idx);
		idx += 5;
	}
	
	if (a->gu & 32) {
		neg_5(a->c + idx, _dst->c + idx);
	}
	return _dst;
}
mv* reverse_mv(mv *_dst, const mv *a)
{
	int idx = 0;
	_dst->gu = a->gu;
	
	if (a->gu & 1) {
		copyGroup_0(a->c + idx, _dst->c + idx);
		idx += 1;
	}
	
	if (a->gu & 2) {
		copyGroup_1(a->c + idx, _dst->c + idx);
		idx += 5;
	}
	
	if (a->gu & 4) {
		neg_2(a->c + idx, _dst->c + idx);
		idx += 10;
	}
	
	if (a->gu & 8) {
		neg_3(a->c + idx, _dst->c + idx);
		idx += 10;
	}
	
	if (a->gu & 16) {
		copyGroup_4(a->c + idx, _dst->c + idx);
		idx += 5;
	}
	
	if (a->gu & 32) {
		copyGroup_5(a->c + idx, _dst->c + idx);
	}
	return _dst;
}
mv* gradeInvolution_mv(mv *_dst, const mv *a)
{
	int idx = 0;
	_dst->gu = a->gu;
	
	if (a->gu & 1) {
		copyGroup_0(a->c + idx, _dst->c + idx);
		idx += 1;
	}
	
	if (a->gu & 2) {
		neg_1(a->c + idx, _dst->c + idx);
		idx += 5;
	}
	
	if (a->gu & 4) {
		copyGroup_2(a->c + idx, _dst->c + idx);
		idx += 10;
	}
	
	if (a->gu & 8) {
		neg_3(a->c + idx, _dst->c + idx);
		idx += 10;
	}
	
	if (a->gu & 16) {
		copyGroup_4(a->c + idx, _dst->c + idx);
		idx += 5;
	}
	
	if (a->gu & 32) {
		neg_5(a->c + idx, _dst->c + idx);
	}
	return _dst;
}
mv* cliffordConjugate_mv(mv *_dst, const mv *a)
{
	int idx = 0;
	_dst->gu = a->gu;
	
	if (a->gu & 1) {
		copyGroup_0(a->c + idx, _dst->c + idx);
		idx += 1;
	}
	
	if (a->gu & 2) {
		neg_1(a->c + idx, _dst->c + idx);
		idx += 5;
	}
	
	if (a->gu & 4) {
		neg_2(a->c + idx, _dst->c + idx);
		idx += 10;
	}
	
	if (a->gu & 8) {
		copyGroup_3(a->c + idx, _dst->c + idx);
		idx += 10;
	}
	
	if (a->gu & 16) {
		copyGroup_4(a->c + idx, _dst->c + idx);
		idx += 5;
	}
	
	if (a->gu & 32) {
		neg_5(a->c + idx, _dst->c + idx);
	}
	return _dst;
}
mv* gp_mv_mv(mv *_dst, const mv *a, const mv *b)
{
	float c[32];
	const float* _a[6];
	const float* _b[6];
	expand(_a, a);
	expand(_b, b);
	c3ga_float_zero_N(c, 32);
	if (a->gu & 1) {
		if (b->gu & 1) {
			gp_default_0_0_0(_a[0], _b[0], c + 0);
		}
		if (b->gu & 2) {
			gp_default_0_1_1(_a[0], _b[1], c + 1);
		}
		if (b->gu & 4) {
			gp_default_0_2_2(_a[0], _b[2], c + 6);
		}
		if (b->gu & 8) {
			gp_default_0_3_3(_a[0], _b[3], c + 16);
		}
		if (b->gu & 16) {
			gp_default_0_4_4(_a[0], _b[4], c + 26);
		}
		if (b->gu & 32) {
			gp_default_0_5_5(_a[0], _b[5], c + 31);
		}
	}
	if (a->gu & 2) {
		if (b->gu & 1) {
			gp_default_1_0_1(_a[1], _b[0], c + 1);
		}
		if (b->gu & 2) {
			gp_default_1_1_0(_a[1], _b[1], c + 0);
			gp_default_1_1_2(_a[1], _b[1], c + 6);
		}
		if (b->gu & 4) {
			gp_default_1_2_1(_a[1], _b[2], c + 1);
			gp_default_1_2_3(_a[1], _b[2], c + 16);
		}
		if (b->gu & 8) {
			gp_default_1_3_2(_a[1], _b[3], c + 6);
			gp_default_1_3_4(_a[1], _b[3], c + 26);
		}
		if (b->gu & 16) {
			gp_default_1_4_3(_a[1], _b[4], c + 16);
			gp_default_1_4_5(_a[1], _b[4], c + 31);
		}
		if (b->gu & 32) {
			gp_default_1_5_4(_a[1], _b[5], c + 26);
		}
	}
	if (a->gu & 4) {
		if (b->gu & 1) {
			gp_default_2_0_2(_a[2], _b[0], c + 6);
		}
		if (b->gu & 2) {
			gp_default_2_1_1(_a[2], _b[1], c + 1);
			gp_default_2_1_3(_a[2], _b[1], c + 16);
		}
		if (b->gu & 4) {
			gp_default_2_2_0(_a[2], _b[2], c + 0);
			gp_default_2_2_2(_a[2], _b[2], c + 6);
			gp_default_2_2_4(_a[2], _b[2], c + 26);
		}
		if (b->gu & 8) {
			gp_default_2_3_1(_a[2], _b[3], c + 1);
			gp_default_2_3_3(_a[2], _b[3], c + 16);
			gp_default_2_3_5(_a[2], _b[3], c + 31);
		}
		if (b->gu & 16) {
			gp_default_2_4_2(_a[2], _b[4], c + 6);
			gp_default_2_4_4(_a[2], _b[4], c + 26);
		}
		if (b->gu & 32) {
			gp_default_2_5_3(_a[2], _b[5], c + 16);
		}
	}
	if (a->gu & 8) {
		if (b->gu & 1) {
			gp_default_3_0_3(_a[3], _b[0], c + 16);
		}
		if (b->gu & 2) {
			gp_default_3_1_2(_a[3], _b[1], c + 6);
			gp_default_3_1_4(_a[3], _b[1], c + 26);
		}
		if (b->gu & 4) {
			gp_default_3_2_1(_a[3], _b[2], c + 1);
			gp_default_3_2_3(_a[3], _b[2], c + 16);
			gp_default_3_2_5(_a[3], _b[2], c + 31);
		}
		if (b->gu & 8) {
			gp_default_3_3_0(_a[3], _b[3], c + 0);
			gp_default_3_3_2(_a[3], _b[3], c + 6);
			gp_default_3_3_4(_a[3], _b[3], c + 26);
		}
		if (b->gu & 16) {
			gp_default_3_4_1(_a[3], _b[4], c + 1);
			gp_default_3_4_3(_a[3], _b[4], c + 16);
		}
		if (b->gu & 32) {
			gp_default_3_5_2(_a[3], _b[5], c + 6);
		}
	}
	if (a->gu & 16) {
		if (b->gu & 1) {
			gp_default_4_0_4(_a[4], _b[0], c + 26);
		}
		if (b->gu & 2) {
			gp_default_4_1_3(_a[4], _b[1], c + 16);
			gp_default_4_1_5(_a[4], _b[1], c + 31);
		}
		if (b->gu & 4) {
			gp_default_4_2_2(_a[4], _b[2], c + 6);
			gp_default_4_2_4(_a[4], _b[2], c + 26);
		}
		if (b->gu & 8) {
			gp_default_4_3_1(_a[4], _b[3], c + 1);
			gp_default_4_3_3(_a[4], _b[3], c + 16);
		}
		if (b->gu & 16) {
			gp_default_4_4_0(_a[4], _b[4], c + 0);
			gp_default_4_4_2(_a[4], _b[4], c + 6);
		}
		if (b->gu & 32) {
			gp_default_4_5_1(_a[4], _b[5], c + 1);
		}
	}
	if (a->gu & 32) {
		if (b->gu & 1) {
			gp_default_5_0_5(_a[5], _b[0], c + 31);
		}
		if (b->gu & 2) {
			gp_default_5_1_4(_a[5], _b[1], c + 26);
		}
		if (b->gu & 4) {
			gp_default_5_2_3(_a[5], _b[2], c + 16);
		}
		if (b->gu & 8) {
			gp_default_5_3_2(_a[5], _b[3], c + 6);
		}
		if (b->gu & 16) {
			gp_default_5_4_1(_a[5], _b[4], c + 1);
		}
		if (b->gu & 32) {
			gp_default_5_5_0(_a[5], _b[5], c + 0);
		}
	}
	compress(c, _dst->c, &(_dst->gu), 0.0f, 63);
	return _dst;
}
mv* op_mv_mv(mv *_dst, const mv *a, const mv *b)
{
	float c[32];
	const float* _a[6];
	const float* _b[6];
	expand(_a, a);
	expand(_b, b);
	c3ga_float_zero_N(c, 32);
	if (a->gu & 1) {
		if (b->gu & 1) {
			gp_default_0_0_0(_a[0], _b[0], c + 0);
		}
		if (b->gu & 2) {
			gp_default_0_1_1(_a[0], _b[1], c + 1);
		}
		if (b->gu & 4) {
			gp_default_0_2_2(_a[0], _b[2], c + 6);
		}
		if (b->gu & 8) {
			gp_default_0_3_3(_a[0], _b[3], c + 16);
		}
		if (b->gu & 16) {
			gp_default_0_4_4(_a[0], _b[4], c + 26);
		}
		if (b->gu & 32) {
			gp_default_0_5_5(_a[0], _b[5], c + 31);
		}
	}
	if (a->gu & 2) {
		if (b->gu & 1) {
			gp_default_1_0_1(_a[1], _b[0], c + 1);
		}
		if (b->gu & 2) {
			gp_default_1_1_2(_a[1], _b[1], c + 6);
		}
		if (b->gu & 4) {
			gp_default_1_2_3(_a[1], _b[2], c + 16);
		}
		if (b->gu & 8) {
			gp_default_1_3_4(_a[1], _b[3], c + 26);
		}
		if (b->gu & 16) {
			gp_default_1_4_5(_a[1], _b[4], c + 31);
		}
	}
	if (a->gu & 4) {
		if (b->gu & 1) {
			gp_default_2_0_2(_a[2], _b[0], c + 6);
		}
		if (b->gu & 2) {
			gp_default_2_1_3(_a[2], _b[1], c + 16);
		}
		if (b->gu & 4) {
			gp_default_2_2_4(_a[2], _b[2], c + 26);
		}
		if (b->gu & 8) {
			gp_default_2_3_5(_a[2], _b[3], c + 31);
		}
	}
	if (a->gu & 8) {
		if (b->gu & 1) {
			gp_default_3_0_3(_a[3], _b[0], c + 16);
		}
		if (b->gu & 2) {
			gp_default_3_1_4(_a[3], _b[1], c + 26);
		}
		if (b->gu & 4) {
			gp_default_3_2_5(_a[3], _b[2], c + 31);
		}
	}
	if (a->gu & 16) {
		if (b->gu & 1) {
			gp_default_4_0_4(_a[4], _b[0], c + 26);
		}
		if (b->gu & 2) {
			gp_default_4_1_5(_a[4], _b[1], c + 31);
		}
	}
	if (a->gu & 32) {
		if (b->gu & 1) {
			gp_default_5_0_5(_a[5], _b[0], c + 31);
		}
	}
	compress(c, _dst->c, &(_dst->gu), 0.0f, 63);
	return _dst;
}
float sp_mv_mv(const mv *a, const mv *b)
{
	float c[1];
	const float* _a[6];
	const float* _b[6];
	expand(_a, a);
	expand(_b, b);
	c3ga_float_zero_1(c);
	if (a->gu & 1) {
		if (b->gu & 1) {
			gp_default_0_0_0(_a[0], _b[0], c + 0);
		}
	}
	if (a->gu & 2) {
		if (b->gu & 2) {
			gp_default_1_1_0(_a[1], _b[1], c + 0);
		}
	}
	if (a->gu & 4) {
		if (b->gu & 4) {
			gp_default_2_2_0(_a[2], _b[2], c + 0);
		}
	}
	if (a->gu & 8) {
		if (b->gu & 8) {
			gp_default_3_3_0(_a[3], _b[3], c + 0);
		}
	}
	if (a->gu & 16) {
		if (b->gu & 16) {
			gp_default_4_4_0(_a[4], _b[4], c + 0);
		}
	}
	if (a->gu & 32) {
		if (b->gu & 32) {
			gp_default_5_5_0(_a[5], _b[5], c + 0);
		}
	}
	return c[0];
}
mv* mhip_mv_mv(mv *_dst, const mv *a, const mv *b)
{
	float c[32];
	const float* _a[6];
	const float* _b[6];
	expand(_a, a);
	expand(_b, b);
	c3ga_float_zero_N(c, 32);
	if (a->gu & 1) {
		if (b->gu & 1) {
			gp_default_0_0_0(_a[0], _b[0], c + 0);
		}
		if (b->gu & 2) {
			gp_default_0_1_1(_a[0], _b[1], c + 1);
		}
		if (b->gu & 4) {
			gp_default_0_2_2(_a[0], _b[2], c + 6);
		}
		if (b->gu & 8) {
			gp_default_0_3_3(_a[0], _b[3], c + 16);
		}
		if (b->gu & 16) {
			gp_default_0_4_4(_a[0], _b[4], c + 26);
		}
		if (b->gu & 32) {
			gp_default_0_5_5(_a[0], _b[5], c + 31);
		}
	}
	if (a->gu & 2) {
		if (b->gu & 1) {
			gp_default_1_0_1(_a[1], _b[0], c + 1);
		}
		if (b->gu & 2) {
			gp_default_1_1_0(_a[1], _b[1], c + 0);
		}
		if (b->gu & 4) {
			gp_default_1_2_1(_a[1], _b[2], c + 1);
		}
		if (b->gu & 8) {
			gp_default_1_3_2(_a[1], _b[3], c + 6);
		}
		if (b->gu & 16) {
			gp_default_1_4_3(_a[1], _b[4], c + 16);
		}
		if (b->gu & 32) {
			gp_default_1_5_4(_a[1], _b[5], c + 26);
		}
	}
	if (a->gu & 4) {
		if (b->gu & 1) {
			gp_default_2_0_2(_a[2], _b[0], c + 6);
		}
		if (b->gu & 2) {
			gp_default_2_1_1(_a[2], _b[1], c + 1);
		}
		if (b->gu & 4) {
			gp_default_2_2_0(_a[2], _b[2], c + 0);
		}
		if (b->gu & 8) {
			gp_default_2_3_1(_a[2], _b[3], c + 1);
		}
		if (b->gu & 16) {
			gp_default_2_4_2(_a[2], _b[4], c + 6);
		}
		if (b->gu & 32) {
			gp_default_2_5_3(_a[2], _b[5], c + 16);
		}
	}
	if (a->gu & 8) {
		if (b->gu & 1) {
			gp_default_3_0_3(_a[3], _b[0], c + 16);
		}
		if (b->gu & 2) {
			gp_default_3_1_2(_a[3], _b[1], c + 6);
		}
		if (b->gu & 4) {
			gp_default_3_2_1(_a[3], _b[2], c + 1);
		}
		if (b->gu & 8) {
			gp_default_3_3_0(_a[3], _b[3], c + 0);
		}
		if (b->gu & 16) {
			gp_default_3_4_1(_a[3], _b[4], c + 1);
		}
		if (b->gu & 32) {
			gp_default_3_5_2(_a[3], _b[5], c + 6);
		}
	}
	if (a->gu & 16) {
		if (b->gu & 1) {
			gp_default_4_0_4(_a[4], _b[0], c + 26);
		}
		if (b->gu & 2) {
			gp_default_4_1_3(_a[4], _b[1], c + 16);
		}
		if (b->gu & 4) {
			gp_default_4_2_2(_a[4], _b[2], c + 6);
		}
		if (b->gu & 8) {
			gp_default_4_3_1(_a[4], _b[3], c + 1);
		}
		if (b->gu & 16) {
			gp_default_4_4_0(_a[4], _b[4], c + 0);
		}
		if (b->gu & 32) {
			gp_default_4_5_1(_a[4], _b[5], c + 1);
		}
	}
	if (a->gu & 32) {
		if (b->gu & 1) {
			gp_default_5_0_5(_a[5], _b[0], c + 31);
		}
		if (b->gu & 2) {
			gp_default_5_1_4(_a[5], _b[1], c + 26);
		}
		if (b->gu & 4) {
			gp_default_5_2_3(_a[5], _b[2], c + 16);
		}
		if (b->gu & 8) {
			gp_default_5_3_2(_a[5], _b[3], c + 6);
		}
		if (b->gu & 16) {
			gp_default_5_4_1(_a[5], _b[4], c + 1);
		}
		if (b->gu & 32) {
			gp_default_5_5_0(_a[5], _b[5], c + 0);
		}
	}
	compress(c, _dst->c, &(_dst->gu), 0.0f, 63);
	return _dst;
}
mv* lc_mv_mv(mv *_dst, const mv *a, const mv *b)
{
	float c[32];
	const float* _a[6];
	const float* _b[6];
	expand(_a, a);
	expand(_b, b);
	c3ga_float_zero_N(c, 32);
	if (a->gu & 1) {
		if (b->gu & 1) {
			gp_default_0_0_0(_a[0], _b[0], c + 0);
		}
		if (b->gu & 2) {
			gp_default_0_1_1(_a[0], _b[1], c + 1);
		}
		if (b->gu & 4) {
			gp_default_0_2_2(_a[0], _b[2], c + 6);
		}
		if (b->gu & 8) {
			gp_default_0_3_3(_a[0], _b[3], c + 16);
		}
		if (b->gu & 16) {
			gp_default_0_4_4(_a[0], _b[4], c + 26);
		}
		if (b->gu & 32) {
			gp_default_0_5_5(_a[0], _b[5], c + 31);
		}
	}
	if (a->gu & 2) {
		if (b->gu & 2) {
			gp_default_1_1_0(_a[1], _b[1], c + 0);
		}
		if (b->gu & 4) {
			gp_default_1_2_1(_a[1], _b[2], c + 1);
		}
		if (b->gu & 8) {
			gp_default_1_3_2(_a[1], _b[3], c + 6);
		}
		if (b->gu & 16) {
			gp_default_1_4_3(_a[1], _b[4], c + 16);
		}
		if (b->gu & 32) {
			gp_default_1_5_4(_a[1], _b[5], c + 26);
		}
	}
	if (a->gu & 4) {
		if (b->gu & 4) {
			gp_default_2_2_0(_a[2], _b[2], c + 0);
		}
		if (b->gu & 8) {
			gp_default_2_3_1(_a[2], _b[3], c + 1);
		}
		if (b->gu & 16) {
			gp_default_2_4_2(_a[2], _b[4], c + 6);
		}
		if (b->gu & 32) {
			gp_default_2_5_3(_a[2], _b[5], c + 16);
		}
	}
	if (a->gu & 8) {
		if (b->gu & 8) {
			gp_default_3_3_0(_a[3], _b[3], c + 0);
		}
		if (b->gu & 16) {
			gp_default_3_4_1(_a[3], _b[4], c + 1);
		}
		if (b->gu & 32) {
			gp_default_3_5_2(_a[3], _b[5], c + 6);
		}
	}
	if (a->gu & 16) {
		if (b->gu & 16) {
			gp_default_4_4_0(_a[4], _b[4], c + 0);
		}
		if (b->gu & 32) {
			gp_default_4_5_1(_a[4], _b[5], c + 1);
		}
	}
	if (a->gu & 32) {
		if (b->gu & 32) {
			gp_default_5_5_0(_a[5], _b[5], c + 0);
		}
	}
	compress(c, _dst->c, &(_dst->gu), 0.0f, 63);
	return _dst;
}
flatPoint* lc_dualLine_plane(flatPoint *_dst, const dualLine *a, const plane *b)
{
	_dst->c[0] = (-a->c[2]*b->c[0]+a->c[4]*b->c[3]+a->c[5]*b->c[2]);
	_dst->c[1] = (a->c[1]*b->c[0]-a->c[3]*b->c[3]+a->c[5]*b->c[1]);
	_dst->c[2] = (-a->c[0]*b->c[0]-a->c[3]*b->c[2]-a->c[4]*b->c[1]);
	_dst->c[3] = (-a->c[0]*b->c[3]-a->c[1]*b->c[2]-a->c[2]*b->c[1]);

	return _dst;
}
flatPoint* op_normalizedPoint_ni_t(flatPoint *_dst, const normalizedPoint *a, const ni_t *b)
{
	_dst->c[0] = a->c[0];
	_dst->c[1] = a->c[1];
	_dst->c[2] = a->c[2];
	_dst->c[3] = 1.0f;

	return _dst;
}
line* op_normalizedPoint_flatPoint(line *_dst, const normalizedPoint *a, const flatPoint *b)
{
	_dst->c[0] = (a->c[0]*b->c[1]-a->c[1]*b->c[0]);
	_dst->c[1] = (a->c[0]*b->c[2]-a->c[2]*b->c[0]);
	_dst->c[2] = (a->c[1]*b->c[2]-a->c[2]*b->c[1]);
	_dst->c[3] = -(-a->c[0]*b->c[3]+b->c[0]);
	_dst->c[4] = -(-a->c[1]*b->c[3]+b->c[1]);
	_dst->c[5] = -(-a->c[2]*b->c[3]+b->c[2]);

	return _dst;
}
plane* op_normalizedPoint_line(plane *_dst, const normalizedPoint *a, const line *b)
{
	_dst->c[0] = (a->c[0]*b->c[2]-a->c[1]*b->c[1]+a->c[2]*b->c[0]);
	_dst->c[1] = (a->c[1]*b->c[5]-a->c[2]*b->c[4]+b->c[2]);
	_dst->c[2] = (a->c[0]*b->c[5]-a->c[2]*b->c[3]+b->c[1]);
	_dst->c[3] = (a->c[0]*b->c[4]-a->c[1]*b->c[3]+b->c[0]);

	return _dst;
}
float norm_mv(const mv *a)
{
	float n2 = 0.0f;
	float c[1];
	const float* _a[6];
	expand(_a, a);
	c3ga_float_zero_1(c);
	if (_a[0] != NULL) {  /* group 0 (grade 0) */
		c[0] = 0.0f;
			gp_default_0_0_0(_a[0], _a[0], c);
		n2 += c[0];
	}
	if (_a[1] != NULL) {  /* group 1 (grade 1) */
		c[0] = 0.0f;
			gp_default_1_1_0(_a[1], _a[1], c);
		n2 += c[0];
	}
	if (_a[2] != NULL) {  /* group 2 (grade 2) */
		c[0] = 0.0f;
			gp_default_2_2_0(_a[2], _a[2], c);
		n2 -= c[0];
	}
	if (_a[3] != NULL) {  /* group 3 (grade 3) */
		c[0] = 0.0f;
			gp_default_3_3_0(_a[3], _a[3], c);
		n2 -= c[0];
	}
	if (_a[4] != NULL) {  /* group 4 (grade 4) */
		c[0] = 0.0f;
			gp_default_4_4_0(_a[4], _a[4], c);
		n2 += c[0];
	}
	if (_a[5] != NULL) {  /* group 5 (grade 5) */
		c[0] = 0.0f;
			gp_default_5_5_0(_a[5], _a[5], c);
		n2 += c[0];
	}
	return ((n2 < 0.0f) ? (float)sqrt(-n2) : (float)sqrt(n2));
}
float norm_mv_returns_scalar(const mv *a) {
	return norm_mv(a);
}
mv* unit_mv(mv *_dst, const mv *a)
{
	int idx = 0;
	float n = norm_mv_returns_scalar(a);
	_dst->gu = a->gu;
	
	if (a->gu & 1) {
		copyDiv_0(a->c + idx, _dst->c + idx, n);
		idx += 1;
	}
	
	if (a->gu & 2) {
		copyDiv_1(a->c + idx, _dst->c + idx, n);
		idx += 5;
	}
	
	if (a->gu & 4) {
		copyDiv_2(a->c + idx, _dst->c + idx, n);
		idx += 10;
	}
	
	if (a->gu & 8) {
		copyDiv_3(a->c + idx, _dst->c + idx, n);
		idx += 10;
	}
	
	if (a->gu & 16) {
		copyDiv_4(a->c + idx, _dst->c + idx, n);
		idx += 5;
	}
	
	if (a->gu & 32) {
		copyDiv_5(a->c + idx, _dst->c + idx, n);
	}
	return _dst;
}
mv* applyVersor_mv_mv(mv *_dst, const mv *a, const mv *b)
{
	mv inv; /* temp space for reverse */
	mv tmp, tmp2; /* temp variables */
	versorInverse_mv(&inv, a); /* compute inverse or reverse */
	gp_mv_mv(&tmp, a, b); /* compute geometric product a b */
	gp_mv_mv(&tmp2, &tmp, &inv); /* compute geometric product (a b) &inv */
	
	extractGrade_mv(_dst, &tmp2, b->gu); /* ditch grade parts which were not in b */
	return _dst;
}
mv* applyUnitVersor_mv_mv(mv *_dst, const mv *a, const mv *b)
{
	mv rev; /* temp space for reverse */
	mv tmp, tmp2; /* temp variables */
	reverse_mv(&rev, a); /* compute inverse or reverse */
	gp_mv_mv(&tmp, a, b); /* compute geometric product a b */
	gp_mv_mv(&tmp2, &tmp, &rev); /* compute geometric product (a b) &rev */
	
	extractGrade_mv(_dst, &tmp2, b->gu); /* ditch grade parts which were not in b */
	return _dst;
}
mv* versorInverse_mv(mv *_dst, const mv *a)
{
	int idx = 0;
	float n2 = norm2_mv_returns_scalar(a);
	_dst->gu = a->gu;
	
	if (a->gu & 1) {
		copyDiv_0(a->c + idx, _dst->c + idx, n2);
		idx += 1;
	}
	
	if (a->gu & 2) {
		copyDiv_1(a->c + idx, _dst->c + idx, n2);
		idx += 5;
	}
	
	if (a->gu & 4) {
		copyDiv_2(a->c + idx, _dst->c + idx, -n2);
		idx += 10;
	}
	
	if (a->gu & 8) {
		copyDiv_3(a->c + idx, _dst->c + idx, -n2);
		idx += 10;
	}
	
	if (a->gu & 16) {
		copyDiv_4(a->c + idx, _dst->c + idx, n2);
		idx += 5;
	}
	
	if (a->gu & 32) {
		copyDiv_5(a->c + idx, _dst->c + idx, n2);
	}
	return _dst;
}
int equals_mv_mv_float(const mv *a, const mv *b, const float c)
{
	int aidx = 0, bidx = 0;
	
	if (a->gu & 1) {
		if (b->gu & 1) {
			if (!equals_0_0(a->c + aidx, b->c + bidx, c)) return 0;
			bidx += 1;
		}
		else if (!zeroGroup_0(a->c + bidx, c)) return 0;
		aidx += 1;
	}
	else if (b->gu & 1) {
		if (!zeroGroup_0(b->c + bidx, c)) return 0;
		bidx += 1;
	}
	
	if (a->gu & 2) {
		if (b->gu & 2) {
			if (!equals_1_1(a->c + aidx, b->c + bidx, c)) return 0;
			bidx += 5;
		}
		else if (!zeroGroup_1(a->c + bidx, c)) return 0;
		aidx += 5;
	}
	else if (b->gu & 2) {
		if (!zeroGroup_1(b->c + bidx, c)) return 0;
		bidx += 5;
	}
	
	if (a->gu & 4) {
		if (b->gu & 4) {
			if (!equals_2_2(a->c + aidx, b->c + bidx, c)) return 0;
			bidx += 10;
		}
		else if (!zeroGroup_2(a->c + bidx, c)) return 0;
		aidx += 10;
	}
	else if (b->gu & 4) {
		if (!zeroGroup_2(b->c + bidx, c)) return 0;
		bidx += 10;
	}
	
	if (a->gu & 8) {
		if (b->gu & 8) {
			if (!equals_3_3(a->c + aidx, b->c + bidx, c)) return 0;
			bidx += 10;
		}
		else if (!zeroGroup_3(a->c + bidx, c)) return 0;
		aidx += 10;
	}
	else if (b->gu & 8) {
		if (!zeroGroup_3(b->c + bidx, c)) return 0;
		bidx += 10;
	}
	
	if (a->gu & 16) {
		if (b->gu & 16) {
			if (!equals_4_4(a->c + aidx, b->c + bidx, c)) return 0;
			bidx += 5;
		}
		else if (!zeroGroup_4(a->c + bidx, c)) return 0;
		aidx += 5;
	}
	else if (b->gu & 16) {
		if (!zeroGroup_4(b->c + bidx, c)) return 0;
		bidx += 5;
	}
	
	if (a->gu & 32) {
		if (b->gu & 32) {
			if (!equals_5_5(a->c + aidx, b->c + bidx, c)) return 0;
		}
		else if (!zeroGroup_5(a->c + bidx, c)) return 0;
	}
	else if (b->gu & 32) {
		if (!zeroGroup_5(b->c + bidx, c)) return 0;
	}
	return 1;
}
int zero_mv_float(const mv *a, const float b)
{
	int idx = 0;
	
	if (a->gu & 1) {
		if (!zeroGroup_0(a->c + idx, b)) return 0;
		idx += 1;
	}
	
	if (a->gu & 2) {
		if (!zeroGroup_1(a->c + idx, b)) return 0;
		idx += 5;
	}
	
	if (a->gu & 4) {
		if (!zeroGroup_2(a->c + idx, b)) return 0;
		idx += 10;
	}
	
	if (a->gu & 8) {
		if (!zeroGroup_3(a->c + idx, b)) return 0;
		idx += 10;
	}
	
	if (a->gu & 16) {
		if (!zeroGroup_4(a->c + idx, b)) return 0;
		idx += 5;
	}
	
	if (a->gu & 32) {
		if (!zeroGroup_5(a->c + idx, b)) return 0;
	}
	return 1;
}
mv* dual_mv(mv *_dst, const mv *a)
{
	int idx = 0;
	float c[32];
	c3ga_float_zero_N(c, 32);
	if (a->gu & 1) {
		dual_default_0_5(a->c + idx, c + 31);
		idx += 1;
	}
	
	if (a->gu & 2) {
		dual_default_1_4(a->c + idx, c + 26);
		idx += 5;
	}
	
	if (a->gu & 4) {
		dual_default_2_3(a->c + idx, c + 16);
		idx += 10;
	}
	
	if (a->gu & 8) {
		dual_default_3_2(a->c + idx, c + 6);
		idx += 10;
	}
	
	if (a->gu & 16) {
		dual_default_4_1(a->c + idx, c + 1);
		idx += 5;
	}
	
	if (a->gu & 32) {
		dual_default_5_0(a->c + idx, c + 0);
	}
	
	compress(c, _dst->c, &(_dst->gu), 0.0f, 63);
	return _dst;
}
mv* undual_mv(mv *_dst, const mv *a)
{
	int idx = 0;
	float c[32];
	c3ga_float_zero_N(c, 32);
	if (a->gu & 1) {
		undual_default_0_5(a->c + idx, c + 31);
		idx += 1;
	}
	
	if (a->gu & 2) {
		undual_default_1_4(a->c + idx, c + 26);
		idx += 5;
	}
	
	if (a->gu & 4) {
		undual_default_2_3(a->c + idx, c + 16);
		idx += 10;
	}
	
	if (a->gu & 8) {
		undual_default_3_2(a->c + idx, c + 6);
		idx += 10;
	}
	
	if (a->gu & 16) {
		undual_default_4_1(a->c + idx, c + 1);
		idx += 5;
	}
	
	if (a->gu & 32) {
		undual_default_5_0(a->c + idx, c + 0);
	}
	
	compress(c, _dst->c, &(_dst->gu), 0.0f, 63);
	return _dst;
}
dualLine* dual_line(dualLine *_dst, const line *a)
{
	_dst->c[0] = a->c[5];
	_dst->c[1] = -a->c[4];
	_dst->c[2] = a->c[3];
	_dst->c[3] = -a->c[2];
	_dst->c[4] = a->c[1];
	_dst->c[5] = -a->c[0];

	return _dst;
}


void exp_mv(mv *R, const mv *x, int order) {
	unsigned long maxC;
	int scale = 1;
	mv xScaled;
	mv tmp1, tmp2, tmp3; /* temp mv used for various purposes */
	mv *xPow1 = &tmp1, *xPow2 = &tmp2; /* xScaled^... goes here */
	mv *result1 = R, *result2 = &tmp3; /* accumulated result goes here (note: 'result1' = 'R') */
	float s_x2, a;
	int i;
   
	/* First try special cases: check if (x * x) is scalar */
	gp_mv_mv(&tmp1, x, x);
	s_x2 = mv_scalar(&tmp1);
	if ((norm2_mv_returns_scalar(&tmp1) - s_x2 * s_x2) < 1E-06f) {
		/* OK (x * x == ~scalar), so use special cases: */
		if (s_x2 < 0.0f) {
			a = (float)sqrt(-s_x2);
			sas_mv_float_float(R, x, (float)sin(a) / a, (float)cos(a));
			return;
		}
		else if (s_x2 > 0.0f) {
			a = (float)sqrt(s_x2);
			sas_mv_float_float(R, x, (float)sinh(a) / a, (float)cosh(a));
			return;
		}
		else {
			sas_mv_float_float(R, x, 1.0f, 1.0f);
			return;
		}
	}

	/* else do general series eval . . . */

	/* result = 1 + ....	 */
	mv_setScalar(result1, 1.0f);
	if (order == 0) return;

	/* find scale (power of 2) such that its norm is < 1 */
	maxC = (unsigned long)mv_largestCoordinate(x);
	scale = 1;
	if (maxC > 1) scale <<= 1;
	while (maxC)
	{
		maxC >>= 1;
		scale <<= 1;
	}

	/* scale */
	gp_mv_float(&xScaled, x, 1.0f / (float)scale); /* xScaled = x / scale */

	/* taylor series approximation */
	mv_setScalar(xPow1, 1.0f); /* xPow1 = 1.0 */
	for (i = 1; i <= order; i++) {
		gp_mv_mv(xPow2, xPow1, &xScaled); /* xPow2 = xPow1 * xScaled */
		gp_mv_float(xPow1, xPow2, 1.0f / (float)i); /* xPow1 = xScaled^i / i! */
		
		add_mv_mv(result2, result1, xPow1); /* result2 = result1 + xPow1 */
		swapPointer((void**)&result1, (void**)&result2); /* result is always in 'result1' at end of loop */
    }

	/* undo scaling */
	while (scale > 1)
	{
		gp_mv_mv(result2, result1, result1); /* result2 = result1 * result1 */
		swapPointer((void**)&result1, (void**)&result2); /* result is always in 'result1' at end of loop */
		scale >>= 1;
	}
    
	if (R != result1) { /* if result does not reside in 'R' in the end, do an explicit copy */
		mv_copy(R, result1);
	}
} /* end of exp_mv() */

mv* gp_mv_float(mv *_dst, const mv *a, const float b)
{
	float c[32];
	const float* _a[6];
	const float* _b[1] = {&b};
	expand(_a, a);
	c3ga_float_zero_N(c, 32);
	if (a->gu & 1) {
			gp_default_0_0_0(_a[0], _b[0], c + 0);
	}
	if (a->gu & 2) {
			gp_default_1_0_1(_a[1], _b[0], c + 1);
	}
	if (a->gu & 4) {
			gp_default_2_0_2(_a[2], _b[0], c + 6);
	}
	if (a->gu & 8) {
			gp_default_3_0_3(_a[3], _b[0], c + 16);
	}
	if (a->gu & 16) {
			gp_default_4_0_4(_a[4], _b[0], c + 26);
	}
	if (a->gu & 32) {
			gp_default_5_0_5(_a[5], _b[0], c + 31);
	}
	compress(c, _dst->c, &(_dst->gu), 0.0f, 63);
	return _dst;
}
float norm2_mv(const mv *a)
{
	float n2 = 0.0f;
	float c[1];
	const float* _a[6];
	expand(_a, a);
	c3ga_float_zero_1(c);
	if (_a[0] != NULL) {  /* group 0 (grade 0) */
		c[0] = 0.0f;
			gp_default_0_0_0(_a[0], _a[0], c);
		n2 += c[0];
	}
	if (_a[1] != NULL) {  /* group 1 (grade 1) */
		c[0] = 0.0f;
			gp_default_1_1_0(_a[1], _a[1], c);
		n2 += c[0];
	}
	if (_a[2] != NULL) {  /* group 2 (grade 2) */
		c[0] = 0.0f;
			gp_default_2_2_0(_a[2], _a[2], c);
		n2 -= c[0];
	}
	if (_a[3] != NULL) {  /* group 3 (grade 3) */
		c[0] = 0.0f;
			gp_default_3_3_0(_a[3], _a[3], c);
		n2 -= c[0];
	}
	if (_a[4] != NULL) {  /* group 4 (grade 4) */
		c[0] = 0.0f;
			gp_default_4_4_0(_a[4], _a[4], c);
		n2 += c[0];
	}
	if (_a[5] != NULL) {  /* group 5 (grade 5) */
		c[0] = 0.0f;
			gp_default_5_5_0(_a[5], _a[5], c);
		n2 += c[0];
	}
	return n2;
}
float norm2_mv_returns_scalar(const mv *a) {
	return norm2_mv(a);
}
mv* sas_mv_float_float(mv *_dst, const mv *a, const float b, const float c)
{
	int idxa = 0, idxc = 0;
	_dst->gu = a->gu | ((c != 0.0) ? GRADE_0 : 0); /* '| GRADE_0' to make sure the scalar part is included */
	
	if (a->gu & 1) {
		copyMul_0(a->c + idxa, _dst->c + idxc, b);
		if (c != 0.0) _dst->c[idxc] += c;
		idxa += 1;
		idxc += 1;
	}
	else if (c != 0.0) {
		_dst->c[idxc] = c;
		idxc += 1;
	}
	
	if (a->gu & 2) {
		copyMul_1(a->c + idxa, _dst->c + idxc, b);
		idxa += 5;
		idxc += 5;
	}
	
	if (a->gu & 4) {
		copyMul_2(a->c + idxa, _dst->c + idxc, b);
		idxa += 10;
		idxc += 10;
	}
	
	if (a->gu & 8) {
		copyMul_3(a->c + idxa, _dst->c + idxc, b);
		idxa += 10;
		idxc += 10;
	}
	
	if (a->gu & 16) {
		copyMul_4(a->c + idxa, _dst->c + idxc, b);
		idxa += 5;
		idxc += 5;
	}
	
	if (a->gu & 32) {
		copyMul_5(a->c + idxa, _dst->c + idxc, b);
	}
	return _dst;
}
