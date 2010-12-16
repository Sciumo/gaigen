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
#include <utility> // for std::swap
#include "c3ga.h"
namespace c3ga {

const int c3ga_spaceDim = 5;
const int c3ga_nbGroups = 6;
const bool c3ga_metricEuclidean = false;
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
	"mv",
	"float",
	"normalizedPoint",
	"flatPoint",
	"line",
	"dualLine",
	"plane",
	"pointPair",
	"circle",
	"no_t",
	"e1_t",
	"e2_t",
	"e3_t",
	"ni_t"
};
no_t no;
e1_t e1;
e2_t e2;
e3_t e3;
ni_t ni;


void ReportUsage::printReport(FILE *F /*= stdout*/, bool includeCount /* = true */) {
	fprintf(F, "Report usage is disabled");
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



#ifdef WIN32
#define snprintf _snprintf
#pragma warning(disable:4996) /* quit your whining already */
#endif /* WIN32 */
const char *c_str(const mv &V, char *str, int maxLength, const char *fp) 
{
	int stdIdx = 0, l;
	char tmpBuf[256], tmpFloatBuf[256]; 
	int k = 0, bei, ia = 0, s = c3ga_mvSize[V.gu()], p = 0, cnt = 0;
	const float *c = V.getC();

	// set up the floating point precision
	if (fp == NULL) fp = c3ga_string_fp;

	// start the string
	l = snprintf(tmpBuf, 256, "%s", c3ga_string_start);
	if (stdIdx + l <= maxLength) {
		strcpy(str + stdIdx, tmpBuf);
		stdIdx += l;
	}
	else {
		snprintf(str, maxLength, "toString_mv: buffer too small");
		return str;
	}

	// print all coordinates
	for (int i = 0; i <= 6; i++) {
		if (V.gu() & (1 << i)) {
			for (int j = 0; j < c3ga_groupSize[i]; j++) {
				float coord = (float)c3ga_basisElementSignByIndex[ia] *c[k];
				/* goal: print [+|-]V.m_c[k][* basisVector1 ^ ... ^ basisVectorN] */			
				snprintf(tmpFloatBuf, 256, fp, (double)fabs(coord)); // cast to double to force custom float types to Plain Old Data
				if (atof(tmpFloatBuf) != 0.0) {
					l = 0;

					// print [+|-]
					l += snprintf(tmpBuf + l, 256-l, "%s", (coord >= 0.0) 
						? (cnt ? c3ga_string_plus : "")
						: c3ga_string_minus);
						
					// print obj.m_c[k]
					int dummyArg = 0; // prevents compiler warning on some platforms
					l += snprintf(tmpBuf + l, 256-l, tmpFloatBuf, dummyArg);

					if (i) { // if not grade 0, print [* basisVector1 ^ ... ^ basisVectorN]
						l += snprintf(tmpBuf + l, 256-l, "%s", c3ga_string_mul);

						// print all basis vectors
						bei = 0;
						while (c3ga_basisElements[ia][bei] >= 0) {
							l += snprintf(tmpBuf + l, 256-l, "%s%s", (bei) ? c3ga_string_wedge : "", 
							 c3ga_basisVectorNames[c3ga_basisElements[ia][bei]]);
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
		else ia += c3ga_groupSize[i];
	}

    // if no coordinates printed: 0
	l = 0;
	if (cnt == 0) {
		l += snprintf(tmpBuf + l, 256-l, "0");
	}

    // end the string
	l += snprintf(tmpBuf + l, 256-l, "%s", c3ga_string_end);
	if (stdIdx + l <= maxLength) {
		strcpy(str + stdIdx, tmpBuf);
		stdIdx += l;
		return str;
	}
	else {
		snprintf(str, maxLength, "toString(): buffer too small\n");
		return str;
	}
}

std::string toString(const mv & obj, const char *fp /* = NULL */) {
	std::string str;
	int strSize = 2048;
	while (strSize <= 1024 * 1024) {
		str.resize(strSize);
		c_str(obj, &(str[0]), strSize-1, fp);
		if (str.find("buffer too small") != std::string::npos) {
			strSize *= 2; // need larger buffer
			continue;
		}
		else break; // done
	}
	str.resize(strlen(&(str[0])));
	return str;
}

// def SB:
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
/// Computes the partial geometric product of two multivectors (group 0  x  group 5 -> group 5)
void gp_default_0_5_5(const float *A, const float *B, float *C);
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
/// Computes the partial geometric product of two multivectors (group 1  x  group 4 -> group 5)
void gp_default_1_4_5(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 5 -> group 4)
void gp_default_1_5_4(const float *A, const float *B, float *C);
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
/// Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 5)
void gp_default_2_3_5(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 2)
void gp_default_2_4_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 4)
void gp_default_2_4_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 5 -> group 3)
void gp_default_2_5_3(const float *A, const float *B, float *C);
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
/// Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 5)
void gp_default_3_2_5(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 0)
void gp_default_3_3_0(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 2)
void gp_default_3_3_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 4)
void gp_default_3_3_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 1)
void gp_default_3_4_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 3)
void gp_default_3_4_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 5 -> group 2)
void gp_default_3_5_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 0 -> group 4)
void gp_default_4_0_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 3)
void gp_default_4_1_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 5)
void gp_default_4_1_5(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 2)
void gp_default_4_2_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 4)
void gp_default_4_2_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 1)
void gp_default_4_3_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 3)
void gp_default_4_3_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 0)
void gp_default_4_4_0(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 2)
void gp_default_4_4_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 5 -> group 1)
void gp_default_4_5_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 0 -> group 5)
void gp_default_5_0_5(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 1 -> group 4)
void gp_default_5_1_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 2 -> group 3)
void gp_default_5_2_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 3 -> group 2)
void gp_default_5_3_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 4 -> group 1)
void gp_default_5_4_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 5 -> group 0)
void gp_default_5_5_0(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 0 -> group 0)
void gp__internal_euclidean_metric__0_0_0(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 1 -> group 1)
void gp__internal_euclidean_metric__0_1_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 2 -> group 2)
void gp__internal_euclidean_metric__0_2_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 3 -> group 3)
void gp__internal_euclidean_metric__0_3_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 4 -> group 4)
void gp__internal_euclidean_metric__0_4_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 0  x  group 5 -> group 5)
void gp__internal_euclidean_metric__0_5_5(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 0 -> group 1)
void gp__internal_euclidean_metric__1_0_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 0)
void gp__internal_euclidean_metric__1_1_0(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 2)
void gp__internal_euclidean_metric__1_1_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 1)
void gp__internal_euclidean_metric__1_2_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 3)
void gp__internal_euclidean_metric__1_2_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 3 -> group 2)
void gp__internal_euclidean_metric__1_3_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 3 -> group 4)
void gp__internal_euclidean_metric__1_3_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 4 -> group 3)
void gp__internal_euclidean_metric__1_4_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 4 -> group 5)
void gp__internal_euclidean_metric__1_4_5(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 1  x  group 5 -> group 4)
void gp__internal_euclidean_metric__1_5_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 0 -> group 2)
void gp__internal_euclidean_metric__2_0_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 1)
void gp__internal_euclidean_metric__2_1_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 3)
void gp__internal_euclidean_metric__2_1_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 0)
void gp__internal_euclidean_metric__2_2_0(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 2)
void gp__internal_euclidean_metric__2_2_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 4)
void gp__internal_euclidean_metric__2_2_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 1)
void gp__internal_euclidean_metric__2_3_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 3)
void gp__internal_euclidean_metric__2_3_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 5)
void gp__internal_euclidean_metric__2_3_5(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 2)
void gp__internal_euclidean_metric__2_4_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 4)
void gp__internal_euclidean_metric__2_4_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 2  x  group 5 -> group 3)
void gp__internal_euclidean_metric__2_5_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 0 -> group 3)
void gp__internal_euclidean_metric__3_0_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 1 -> group 2)
void gp__internal_euclidean_metric__3_1_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 1 -> group 4)
void gp__internal_euclidean_metric__3_1_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 1)
void gp__internal_euclidean_metric__3_2_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 3)
void gp__internal_euclidean_metric__3_2_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 5)
void gp__internal_euclidean_metric__3_2_5(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 0)
void gp__internal_euclidean_metric__3_3_0(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 2)
void gp__internal_euclidean_metric__3_3_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 4)
void gp__internal_euclidean_metric__3_3_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 1)
void gp__internal_euclidean_metric__3_4_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 3)
void gp__internal_euclidean_metric__3_4_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 3  x  group 5 -> group 2)
void gp__internal_euclidean_metric__3_5_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 0 -> group 4)
void gp__internal_euclidean_metric__4_0_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 3)
void gp__internal_euclidean_metric__4_1_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 5)
void gp__internal_euclidean_metric__4_1_5(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 2)
void gp__internal_euclidean_metric__4_2_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 4)
void gp__internal_euclidean_metric__4_2_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 1)
void gp__internal_euclidean_metric__4_3_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 3)
void gp__internal_euclidean_metric__4_3_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 0)
void gp__internal_euclidean_metric__4_4_0(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 2)
void gp__internal_euclidean_metric__4_4_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 4  x  group 5 -> group 1)
void gp__internal_euclidean_metric__4_5_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 0 -> group 5)
void gp__internal_euclidean_metric__5_0_5(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 1 -> group 4)
void gp__internal_euclidean_metric__5_1_4(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 2 -> group 3)
void gp__internal_euclidean_metric__5_2_3(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 3 -> group 2)
void gp__internal_euclidean_metric__5_3_2(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 4 -> group 1)
void gp__internal_euclidean_metric__5_4_1(const float *A, const float *B, float *C);
/// Computes the partial geometric product of two multivectors (group 5  x  group 5 -> group 0)
void gp__internal_euclidean_metric__5_5_0(const float *A, const float *B, float *C);
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
/// copies coordinates of group 5
void copyGroup_5(const float *A, float *C);
/// copies and multiplies (by s) coordinates of group 5
void copyMul_5(const float *A, float *C, float s);
/// copies and divides (by s) coordinates of group 5
void copyDiv_5(const float *A, float *C, float s);
/// adds coordinates of group 5 from variable A to C
void add_5(const float *A, float *C);
/// subtracts coordinates of group 5 in variable A from C
void sub_5(const float *A, float *C);
/// negate coordinates of group 5 of variable A
void neg_5(const float *A, float *C);
/// adds coordinates of group 5 of variables A and B
void add2_5_5(const float *A, const float *B, float *C);
/// subtracts coordinates of group 5 of variables A from B
void sub2_5_5(const float *A, const float *B, float *C);
/// performs coordinate-wise multiplication of coordinates of group 5 of variables A and B
void hp_5_5(const float *A, const float *B, float *C);
/// performs coordinate-wise division of coordinates of group 5 of variables A and B
/// (no checks for divide by zero are made)
void ihp_5_5(const float *A, const float *B, float *C);
/// check for equality up to eps of coordinates of group 5 of variables A and B
bool equals_5_5(const float *A, const float *B, float eps);
/// checks if coordinates of group 5 of variable A are zero up to eps
bool zeroGroup_5(const float *A, float eps);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_0_5(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_0_5(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_1_4(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_1_4(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_2_3(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_2_3(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_3_2(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_3_2(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_4_1(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_4_1(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual_default_5_0(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual_default_5_0(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual__internal_euclidean_metric__0_5(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual__internal_euclidean_metric__0_5(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual__internal_euclidean_metric__1_4(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual__internal_euclidean_metric__1_4(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual__internal_euclidean_metric__2_3(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual__internal_euclidean_metric__2_3(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual__internal_euclidean_metric__3_2(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual__internal_euclidean_metric__3_2(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual__internal_euclidean_metric__4_1(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
void undual__internal_euclidean_metric__4_1(const float *A, float *C);
/// Computes the partial dual (w.r.t. full space) of a multivector.
void dual__internal_euclidean_metric__5_0(const float *A, float *C);
/// Computes the partial undual (w.r.t. full space) of a multivector.
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
bool equals_0_0(const float *A, const float *B, float eps) {
		if (((A[0] - B[0]) < -eps) || ((A[0] - B[0]) > eps)) return false;
	return true;
}
bool zeroGroup_0(const float *A, float eps) {
		if ((A[0] < -eps) || (A[0] > eps)) return false;
		return true;
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
bool equals_1_1(const float *A, const float *B, float eps) {
		if (((A[0] - B[0]) < -eps) || ((A[0] - B[0]) > eps)) return false;
		if (((A[1] - B[1]) < -eps) || ((A[1] - B[1]) > eps)) return false;
		if (((A[2] - B[2]) < -eps) || ((A[2] - B[2]) > eps)) return false;
		if (((A[3] - B[3]) < -eps) || ((A[3] - B[3]) > eps)) return false;
		if (((A[4] - B[4]) < -eps) || ((A[4] - B[4]) > eps)) return false;
	return true;
}
bool zeroGroup_1(const float *A, float eps) {
		if ((A[0] < -eps) || (A[0] > eps)) return false;
		if ((A[1] < -eps) || (A[1] > eps)) return false;
		if ((A[2] < -eps) || (A[2] > eps)) return false;
		if ((A[3] < -eps) || (A[3] > eps)) return false;
		if ((A[4] < -eps) || (A[4] > eps)) return false;
		return true;
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
bool equals_2_2(const float *A, const float *B, float eps) {
		if (((A[0] - B[0]) < -eps) || ((A[0] - B[0]) > eps)) return false;
		if (((A[1] - B[1]) < -eps) || ((A[1] - B[1]) > eps)) return false;
		if (((A[2] - B[2]) < -eps) || ((A[2] - B[2]) > eps)) return false;
		if (((A[3] - B[3]) < -eps) || ((A[3] - B[3]) > eps)) return false;
		if (((A[4] - B[4]) < -eps) || ((A[4] - B[4]) > eps)) return false;
		if (((A[5] - B[5]) < -eps) || ((A[5] - B[5]) > eps)) return false;
		if (((A[6] - B[6]) < -eps) || ((A[6] - B[6]) > eps)) return false;
		if (((A[7] - B[7]) < -eps) || ((A[7] - B[7]) > eps)) return false;
		if (((A[8] - B[8]) < -eps) || ((A[8] - B[8]) > eps)) return false;
		if (((A[9] - B[9]) < -eps) || ((A[9] - B[9]) > eps)) return false;
	return true;
}
bool zeroGroup_2(const float *A, float eps) {
		if ((A[0] < -eps) || (A[0] > eps)) return false;
		if ((A[1] < -eps) || (A[1] > eps)) return false;
		if ((A[2] < -eps) || (A[2] > eps)) return false;
		if ((A[3] < -eps) || (A[3] > eps)) return false;
		if ((A[4] < -eps) || (A[4] > eps)) return false;
		if ((A[5] < -eps) || (A[5] > eps)) return false;
		if ((A[6] < -eps) || (A[6] > eps)) return false;
		if ((A[7] < -eps) || (A[7] > eps)) return false;
		if ((A[8] < -eps) || (A[8] > eps)) return false;
		if ((A[9] < -eps) || (A[9] > eps)) return false;
		return true;
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
bool equals_3_3(const float *A, const float *B, float eps) {
	return equals_2_2(A, B, eps);
}
bool zeroGroup_3(const float *A, float eps) {
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
bool equals_4_4(const float *A, const float *B, float eps) {
	return equals_1_1(A, B, eps);
}
bool zeroGroup_4(const float *A, float eps) {
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
bool equals_5_5(const float *A, const float *B, float eps) {
	return equals_0_0(A, B, eps);
}
bool zeroGroup_5(const float *A, float eps) {
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

float mv::largestCoordinate() const {
	float maxValue = 0.0f;
	int nbC = c3ga_mvSize[m_gu], i;
	for (i = 0; i < nbC; i++)
		if (fabsf(m_c[i]) > maxValue) maxValue = fabsf(m_c[i]);
	return maxValue;
}

float mv::largestBasisBlade(unsigned int &bm) const {
	int nc = c3ga_mvSize[gu()];
	float maxC = -1.0f, C;

	int idx = 0;
	int group = 0;
	int i = 0;
	
	bm = 0;

	while (i < nc) {
		if (gu() & (1 << group)) {
			for (int j = 0; j < c3ga_groupSize[group]; j++) {
				C = fabsf(m_c[i]);
				if (C > maxC) {
					maxC = C;
					bm = c3ga_basisElementBitmapByIndex[idx];
				}
				idx++;
				i++;
			}
		}
		else idx += c3ga_groupSize[group];
		group++;
	}

	return maxC;
} // end of mv::largestBasisBlade()




void mv::compress(float epsilon /*= 0.0f*/) {
	set(mv_compress(m_c, epsilon, m_gu));
}

mv mv_compress(const float *c, float epsilon /*= 0.0f*/, int gu /*= 63*/) {
	int i, j, ia = 0, ib = 0, f, s;
	int cgu = 0;
	float cc[32];

	// for all grade parts...
	for (i = 0; i < 6; i++) {
		// check if grade part has memory use:
		if (!(gu & (1 << i))) continue;

		// check if abs coordinates of grade part are all < epsilon
		s = c3ga_groupSize[i];
		j = ia + s;
		f = 0;
		for (; ia < j; ia++)
			if (fabsf(c[ia]) > epsilon) {f = 1; break;}
		ia = j;
		if (f) {
			c3ga::copy_N(cc + ib, c + ia - s, s);
			ib += s;
			cgu |= (1 << i);
		}
	}
	
	return mv(cgu, cc);
}

mv mv_compress(int nbBlades, const unsigned int *bitmaps, const float *coords) {
	// convert basis blade compression to regular coordinate array:
	float A[32];
	for (int i = 0; i < 32; i++) A[i] = 0.0f;

	for (int i = 0; i < nbBlades; i++) {
		A[c3ga_basisElementIndexByBitmap[bitmaps[i]]] = coords[i] * (float)c3ga_basisElementSignByBitmap[bitmaps[i]];		
	}

	return mv_compress(A);
}


void mv::expand(const float *ptrs[6], bool nulls /* = true */) const {
	const float *c = m_c; // this pointer gets incremented below
	
	if (m_gu & 1) {
		ptrs[0] =  c;
		c += 1;
	}
	else ptrs[0] = (nulls) ? NULL : nullFloats();
	if (m_gu & 2) {
		ptrs[1] =  c;
		c += 5;
	}
	else ptrs[1] = (nulls) ? NULL : nullFloats();
	if (m_gu & 4) {
		ptrs[2] =  c;
		c += 10;
	}
	else ptrs[2] = (nulls) ? NULL : nullFloats();
	if (m_gu & 8) {
		ptrs[3] =  c;
		c += 10;
	}
	else ptrs[3] = (nulls) ? NULL : nullFloats();
	if (m_gu & 16) {
		ptrs[4] =  c;
		c += 5;
	}
	else ptrs[4] = (nulls) ? NULL : nullFloats();
	if (m_gu & 32) {
		ptrs[5] =  c;
	}
	else ptrs[5] = (nulls) ? NULL : nullFloats();
}

mv add(const mv &a, const mv &b)
{
	int aidx = 0, bidx = 0, cidx = 0;
	int gu = a.gu() | b.gu();
	float c[32];
	
	if (a.gu() & 1) {
		if (b.gu() & 1) {
			add2_0_0(a.getC() + aidx, b.getC() + bidx, c + cidx);
			bidx += 1;
		}
		else copyGroup_0(a.getC() + aidx, c + cidx);
		aidx += 1;
		cidx += 1;
	}
	else if (b.gu() & 1) {
		copyGroup_0(b.getC() + bidx, c + cidx);
		bidx += 1;
		cidx += 1;
	}
	
	if (a.gu() & 2) {
		if (b.gu() & 2) {
			add2_1_1(a.getC() + aidx, b.getC() + bidx, c + cidx);
			bidx += 5;
		}
		else copyGroup_1(a.getC() + aidx, c + cidx);
		aidx += 5;
		cidx += 5;
	}
	else if (b.gu() & 2) {
		copyGroup_1(b.getC() + bidx, c + cidx);
		bidx += 5;
		cidx += 5;
	}
	
	if (a.gu() & 4) {
		if (b.gu() & 4) {
			add2_2_2(a.getC() + aidx, b.getC() + bidx, c + cidx);
			bidx += 10;
		}
		else copyGroup_2(a.getC() + aidx, c + cidx);
		aidx += 10;
		cidx += 10;
	}
	else if (b.gu() & 4) {
		copyGroup_2(b.getC() + bidx, c + cidx);
		bidx += 10;
		cidx += 10;
	}
	
	if (a.gu() & 8) {
		if (b.gu() & 8) {
			add2_3_3(a.getC() + aidx, b.getC() + bidx, c + cidx);
			bidx += 10;
		}
		else copyGroup_3(a.getC() + aidx, c + cidx);
		aidx += 10;
		cidx += 10;
	}
	else if (b.gu() & 8) {
		copyGroup_3(b.getC() + bidx, c + cidx);
		bidx += 10;
		cidx += 10;
	}
	
	if (a.gu() & 16) {
		if (b.gu() & 16) {
			add2_4_4(a.getC() + aidx, b.getC() + bidx, c + cidx);
			bidx += 5;
		}
		else copyGroup_4(a.getC() + aidx, c + cidx);
		aidx += 5;
		cidx += 5;
	}
	else if (b.gu() & 16) {
		copyGroup_4(b.getC() + bidx, c + cidx);
		bidx += 5;
		cidx += 5;
	}
	
	if (a.gu() & 32) {
		if (b.gu() & 32) {
			add2_5_5(a.getC() + aidx, b.getC() + bidx, c + cidx);
		}
		else copyGroup_5(a.getC() + aidx, c + cidx);
		cidx += 1;
	}
	else if (b.gu() & 32) {
		copyGroup_5(b.getC() + bidx, c + cidx);
		cidx += 1;
	}
	return mv(gu, c);
}
mv subtract(const mv &a, const mv &b)
{
	int aidx = 0, bidx = 0, cidx = 0;
	int gu = a.gu() | b.gu();
	float c[32];
	
	if (a.gu() & 1) {
		if (b.gu() & 1) {
			sub2_0_0(a.getC() + aidx, b.getC() + bidx, c + cidx);
			bidx += 1;
		}
		else copyGroup_0(a.getC() + aidx, c + cidx);
		aidx += 1;
		cidx += 1;
	}
	else if (b.gu() & 1) {
		neg_0(b.getC() + bidx, c + cidx);
		bidx += 1;
		cidx += 1;
	}
	
	if (a.gu() & 2) {
		if (b.gu() & 2) {
			sub2_1_1(a.getC() + aidx, b.getC() + bidx, c + cidx);
			bidx += 5;
		}
		else copyGroup_1(a.getC() + aidx, c + cidx);
		aidx += 5;
		cidx += 5;
	}
	else if (b.gu() & 2) {
		neg_1(b.getC() + bidx, c + cidx);
		bidx += 5;
		cidx += 5;
	}
	
	if (a.gu() & 4) {
		if (b.gu() & 4) {
			sub2_2_2(a.getC() + aidx, b.getC() + bidx, c + cidx);
			bidx += 10;
		}
		else copyGroup_2(a.getC() + aidx, c + cidx);
		aidx += 10;
		cidx += 10;
	}
	else if (b.gu() & 4) {
		neg_2(b.getC() + bidx, c + cidx);
		bidx += 10;
		cidx += 10;
	}
	
	if (a.gu() & 8) {
		if (b.gu() & 8) {
			sub2_3_3(a.getC() + aidx, b.getC() + bidx, c + cidx);
			bidx += 10;
		}
		else copyGroup_3(a.getC() + aidx, c + cidx);
		aidx += 10;
		cidx += 10;
	}
	else if (b.gu() & 8) {
		neg_3(b.getC() + bidx, c + cidx);
		bidx += 10;
		cidx += 10;
	}
	
	if (a.gu() & 16) {
		if (b.gu() & 16) {
			sub2_4_4(a.getC() + aidx, b.getC() + bidx, c + cidx);
			bidx += 5;
		}
		else copyGroup_4(a.getC() + aidx, c + cidx);
		aidx += 5;
		cidx += 5;
	}
	else if (b.gu() & 16) {
		neg_4(b.getC() + bidx, c + cidx);
		bidx += 5;
		cidx += 5;
	}
	
	if (a.gu() & 32) {
		if (b.gu() & 32) {
			sub2_5_5(a.getC() + aidx, b.getC() + bidx, c + cidx);
		}
		else copyGroup_5(a.getC() + aidx, c + cidx);
		cidx += 1;
	}
	else if (b.gu() & 32) {
		neg_5(b.getC() + bidx, c + cidx);
		cidx += 1;
	}
	return mv(gu, c);
}
mv extractGrade(const mv &a, const int groupBitmap)
{
	int aidx = 0, cidx = 0;
	int gu =  a.gu() & groupBitmap;
	float c[32];
	
	if (a.gu() & 1) {
		if (groupBitmap & 1) {
			copyGroup_0(a.getC() + aidx, c + cidx);
			cidx += 1;
		}
		aidx += 1;
	}
	
	if (a.gu() & 2) {
		if (groupBitmap & 2) {
			copyGroup_1(a.getC() + aidx, c + cidx);
			cidx += 5;
		}
		aidx += 5;
	}
	
	if (a.gu() & 4) {
		if (groupBitmap & 4) {
			copyGroup_2(a.getC() + aidx, c + cidx);
			cidx += 10;
		}
		aidx += 10;
	}
	
	if (a.gu() & 8) {
		if (groupBitmap & 8) {
			copyGroup_3(a.getC() + aidx, c + cidx);
			cidx += 10;
		}
		aidx += 10;
	}
	
	if (a.gu() & 16) {
		if (groupBitmap & 16) {
			copyGroup_4(a.getC() + aidx, c + cidx);
			cidx += 5;
		}
		aidx += 5;
	}
	
	if (a.gu() & 32) {
		if (groupBitmap & 32) {
			copyGroup_5(a.getC() + aidx, c + cidx);
		}
	}
	return mv(gu, c);
}
mv negate(const mv &a)
{
	int idx = 0;
	int gu = a.gu();
	float c[32];
	
	if (a.gu() & 1) {
		neg_0(a.getC() + idx, c + idx);
		idx += 1;
	}
	
	if (a.gu() & 2) {
		neg_1(a.getC() + idx, c + idx);
		idx += 5;
	}
	
	if (a.gu() & 4) {
		neg_2(a.getC() + idx, c + idx);
		idx += 10;
	}
	
	if (a.gu() & 8) {
		neg_3(a.getC() + idx, c + idx);
		idx += 10;
	}
	
	if (a.gu() & 16) {
		neg_4(a.getC() + idx, c + idx);
		idx += 5;
	}
	
	if (a.gu() & 32) {
		neg_5(a.getC() + idx, c + idx);
	}
	return mv(gu, c);
}
mv reverse(const mv &a)
{
	int idx = 0;
	int gu = a.gu();
	float c[32];
	
	if (a.gu() & 1) {
		copyGroup_0(a.getC() + idx, c + idx);
		idx += 1;
	}
	
	if (a.gu() & 2) {
		copyGroup_1(a.getC() + idx, c + idx);
		idx += 5;
	}
	
	if (a.gu() & 4) {
		neg_2(a.getC() + idx, c + idx);
		idx += 10;
	}
	
	if (a.gu() & 8) {
		neg_3(a.getC() + idx, c + idx);
		idx += 10;
	}
	
	if (a.gu() & 16) {
		copyGroup_4(a.getC() + idx, c + idx);
		idx += 5;
	}
	
	if (a.gu() & 32) {
		copyGroup_5(a.getC() + idx, c + idx);
	}
	return mv(gu, c);
}
mv gradeInvolution(const mv &a)
{
	int idx = 0;
	int gu = a.gu();
	float c[32];
	
	if (a.gu() & 1) {
		copyGroup_0(a.getC() + idx, c + idx);
		idx += 1;
	}
	
	if (a.gu() & 2) {
		neg_1(a.getC() + idx, c + idx);
		idx += 5;
	}
	
	if (a.gu() & 4) {
		copyGroup_2(a.getC() + idx, c + idx);
		idx += 10;
	}
	
	if (a.gu() & 8) {
		neg_3(a.getC() + idx, c + idx);
		idx += 10;
	}
	
	if (a.gu() & 16) {
		copyGroup_4(a.getC() + idx, c + idx);
		idx += 5;
	}
	
	if (a.gu() & 32) {
		neg_5(a.getC() + idx, c + idx);
	}
	return mv(gu, c);
}
mv cliffordConjugate(const mv &a)
{
	int idx = 0;
	int gu = a.gu();
	float c[32];
	
	if (a.gu() & 1) {
		copyGroup_0(a.getC() + idx, c + idx);
		idx += 1;
	}
	
	if (a.gu() & 2) {
		neg_1(a.getC() + idx, c + idx);
		idx += 5;
	}
	
	if (a.gu() & 4) {
		neg_2(a.getC() + idx, c + idx);
		idx += 10;
	}
	
	if (a.gu() & 8) {
		copyGroup_3(a.getC() + idx, c + idx);
		idx += 10;
	}
	
	if (a.gu() & 16) {
		copyGroup_4(a.getC() + idx, c + idx);
		idx += 5;
	}
	
	if (a.gu() & 32) {
		neg_5(a.getC() + idx, c + idx);
	}
	return mv(gu, c);
}
mv gp(const mv &a, const mv &b)
{
	float c[32];
	const float* _a[6];
	const float* _b[6];
	a.expand(_a);
	b.expand(_b);
	c3ga::zero_N(c, 32);
	if (a.gu() & 1) {
		if (b.gu() & 1) {
			gp_default_0_0_0(_a[0], _b[0], c + 0);
		}
		if (b.gu() & 2) {
			gp_default_0_1_1(_a[0], _b[1], c + 1);
		}
		if (b.gu() & 4) {
			gp_default_0_2_2(_a[0], _b[2], c + 6);
		}
		if (b.gu() & 8) {
			gp_default_0_3_3(_a[0], _b[3], c + 16);
		}
		if (b.gu() & 16) {
			gp_default_0_4_4(_a[0], _b[4], c + 26);
		}
		if (b.gu() & 32) {
			gp_default_0_5_5(_a[0], _b[5], c + 31);
		}
	}
	if (a.gu() & 2) {
		if (b.gu() & 1) {
			gp_default_1_0_1(_a[1], _b[0], c + 1);
		}
		if (b.gu() & 2) {
			gp_default_1_1_0(_a[1], _b[1], c + 0);
			gp_default_1_1_2(_a[1], _b[1], c + 6);
		}
		if (b.gu() & 4) {
			gp_default_1_2_1(_a[1], _b[2], c + 1);
			gp_default_1_2_3(_a[1], _b[2], c + 16);
		}
		if (b.gu() & 8) {
			gp_default_1_3_2(_a[1], _b[3], c + 6);
			gp_default_1_3_4(_a[1], _b[3], c + 26);
		}
		if (b.gu() & 16) {
			gp_default_1_4_3(_a[1], _b[4], c + 16);
			gp_default_1_4_5(_a[1], _b[4], c + 31);
		}
		if (b.gu() & 32) {
			gp_default_1_5_4(_a[1], _b[5], c + 26);
		}
	}
	if (a.gu() & 4) {
		if (b.gu() & 1) {
			gp_default_2_0_2(_a[2], _b[0], c + 6);
		}
		if (b.gu() & 2) {
			gp_default_2_1_1(_a[2], _b[1], c + 1);
			gp_default_2_1_3(_a[2], _b[1], c + 16);
		}
		if (b.gu() & 4) {
			gp_default_2_2_0(_a[2], _b[2], c + 0);
			gp_default_2_2_2(_a[2], _b[2], c + 6);
			gp_default_2_2_4(_a[2], _b[2], c + 26);
		}
		if (b.gu() & 8) {
			gp_default_2_3_1(_a[2], _b[3], c + 1);
			gp_default_2_3_3(_a[2], _b[3], c + 16);
			gp_default_2_3_5(_a[2], _b[3], c + 31);
		}
		if (b.gu() & 16) {
			gp_default_2_4_2(_a[2], _b[4], c + 6);
			gp_default_2_4_4(_a[2], _b[4], c + 26);
		}
		if (b.gu() & 32) {
			gp_default_2_5_3(_a[2], _b[5], c + 16);
		}
	}
	if (a.gu() & 8) {
		if (b.gu() & 1) {
			gp_default_3_0_3(_a[3], _b[0], c + 16);
		}
		if (b.gu() & 2) {
			gp_default_3_1_2(_a[3], _b[1], c + 6);
			gp_default_3_1_4(_a[3], _b[1], c + 26);
		}
		if (b.gu() & 4) {
			gp_default_3_2_1(_a[3], _b[2], c + 1);
			gp_default_3_2_3(_a[3], _b[2], c + 16);
			gp_default_3_2_5(_a[3], _b[2], c + 31);
		}
		if (b.gu() & 8) {
			gp_default_3_3_0(_a[3], _b[3], c + 0);
			gp_default_3_3_2(_a[3], _b[3], c + 6);
			gp_default_3_3_4(_a[3], _b[3], c + 26);
		}
		if (b.gu() & 16) {
			gp_default_3_4_1(_a[3], _b[4], c + 1);
			gp_default_3_4_3(_a[3], _b[4], c + 16);
		}
		if (b.gu() & 32) {
			gp_default_3_5_2(_a[3], _b[5], c + 6);
		}
	}
	if (a.gu() & 16) {
		if (b.gu() & 1) {
			gp_default_4_0_4(_a[4], _b[0], c + 26);
		}
		if (b.gu() & 2) {
			gp_default_4_1_3(_a[4], _b[1], c + 16);
			gp_default_4_1_5(_a[4], _b[1], c + 31);
		}
		if (b.gu() & 4) {
			gp_default_4_2_2(_a[4], _b[2], c + 6);
			gp_default_4_2_4(_a[4], _b[2], c + 26);
		}
		if (b.gu() & 8) {
			gp_default_4_3_1(_a[4], _b[3], c + 1);
			gp_default_4_3_3(_a[4], _b[3], c + 16);
		}
		if (b.gu() & 16) {
			gp_default_4_4_0(_a[4], _b[4], c + 0);
			gp_default_4_4_2(_a[4], _b[4], c + 6);
		}
		if (b.gu() & 32) {
			gp_default_4_5_1(_a[4], _b[5], c + 1);
		}
	}
	if (a.gu() & 32) {
		if (b.gu() & 1) {
			gp_default_5_0_5(_a[5], _b[0], c + 31);
		}
		if (b.gu() & 2) {
			gp_default_5_1_4(_a[5], _b[1], c + 26);
		}
		if (b.gu() & 4) {
			gp_default_5_2_3(_a[5], _b[2], c + 16);
		}
		if (b.gu() & 8) {
			gp_default_5_3_2(_a[5], _b[3], c + 6);
		}
		if (b.gu() & 16) {
			gp_default_5_4_1(_a[5], _b[4], c + 1);
		}
		if (b.gu() & 32) {
			gp_default_5_5_0(_a[5], _b[5], c + 0);
		}
	}
	return mv_compress(c, 0.0f, 63);
}
mv op(const mv &a, const mv &b)
{
	float c[32];
	const float* _a[6];
	const float* _b[6];
	a.expand(_a);
	b.expand(_b);
	c3ga::zero_N(c, 32);
	if (a.gu() & 1) {
		if (b.gu() & 1) {
			gp_default_0_0_0(_a[0], _b[0], c + 0);
		}
		if (b.gu() & 2) {
			gp_default_0_1_1(_a[0], _b[1], c + 1);
		}
		if (b.gu() & 4) {
			gp_default_0_2_2(_a[0], _b[2], c + 6);
		}
		if (b.gu() & 8) {
			gp_default_0_3_3(_a[0], _b[3], c + 16);
		}
		if (b.gu() & 16) {
			gp_default_0_4_4(_a[0], _b[4], c + 26);
		}
		if (b.gu() & 32) {
			gp_default_0_5_5(_a[0], _b[5], c + 31);
		}
	}
	if (a.gu() & 2) {
		if (b.gu() & 1) {
			gp_default_1_0_1(_a[1], _b[0], c + 1);
		}
		if (b.gu() & 2) {
			gp_default_1_1_2(_a[1], _b[1], c + 6);
		}
		if (b.gu() & 4) {
			gp_default_1_2_3(_a[1], _b[2], c + 16);
		}
		if (b.gu() & 8) {
			gp_default_1_3_4(_a[1], _b[3], c + 26);
		}
		if (b.gu() & 16) {
			gp_default_1_4_5(_a[1], _b[4], c + 31);
		}
	}
	if (a.gu() & 4) {
		if (b.gu() & 1) {
			gp_default_2_0_2(_a[2], _b[0], c + 6);
		}
		if (b.gu() & 2) {
			gp_default_2_1_3(_a[2], _b[1], c + 16);
		}
		if (b.gu() & 4) {
			gp_default_2_2_4(_a[2], _b[2], c + 26);
		}
		if (b.gu() & 8) {
			gp_default_2_3_5(_a[2], _b[3], c + 31);
		}
	}
	if (a.gu() & 8) {
		if (b.gu() & 1) {
			gp_default_3_0_3(_a[3], _b[0], c + 16);
		}
		if (b.gu() & 2) {
			gp_default_3_1_4(_a[3], _b[1], c + 26);
		}
		if (b.gu() & 4) {
			gp_default_3_2_5(_a[3], _b[2], c + 31);
		}
	}
	if (a.gu() & 16) {
		if (b.gu() & 1) {
			gp_default_4_0_4(_a[4], _b[0], c + 26);
		}
		if (b.gu() & 2) {
			gp_default_4_1_5(_a[4], _b[1], c + 31);
		}
	}
	if (a.gu() & 32) {
		if (b.gu() & 1) {
			gp_default_5_0_5(_a[5], _b[0], c + 31);
		}
	}
	return mv_compress(c, 0.0f, 63);
}
float sp(const mv &a, const mv &b)
{
	float c[1];
	const float* _a[6];
	const float* _b[6];
	a.expand(_a);
	b.expand(_b);
	c3ga::zero_1(c);
	if (a.gu() & 1) {
		if (b.gu() & 1) {
			gp_default_0_0_0(_a[0], _b[0], c + 0);
		}
	}
	if (a.gu() & 2) {
		if (b.gu() & 2) {
			gp_default_1_1_0(_a[1], _b[1], c + 0);
		}
	}
	if (a.gu() & 4) {
		if (b.gu() & 4) {
			gp_default_2_2_0(_a[2], _b[2], c + 0);
		}
	}
	if (a.gu() & 8) {
		if (b.gu() & 8) {
			gp_default_3_3_0(_a[3], _b[3], c + 0);
		}
	}
	if (a.gu() & 16) {
		if (b.gu() & 16) {
			gp_default_4_4_0(_a[4], _b[4], c + 0);
		}
	}
	if (a.gu() & 32) {
		if (b.gu() & 32) {
			gp_default_5_5_0(_a[5], _b[5], c + 0);
		}
	}
	return c[0];
}
mv mhip(const mv &a, const mv &b)
{
	float c[32];
	const float* _a[6];
	const float* _b[6];
	a.expand(_a);
	b.expand(_b);
	c3ga::zero_N(c, 32);
	if (a.gu() & 1) {
		if (b.gu() & 1) {
			gp_default_0_0_0(_a[0], _b[0], c + 0);
		}
		if (b.gu() & 2) {
			gp_default_0_1_1(_a[0], _b[1], c + 1);
		}
		if (b.gu() & 4) {
			gp_default_0_2_2(_a[0], _b[2], c + 6);
		}
		if (b.gu() & 8) {
			gp_default_0_3_3(_a[0], _b[3], c + 16);
		}
		if (b.gu() & 16) {
			gp_default_0_4_4(_a[0], _b[4], c + 26);
		}
		if (b.gu() & 32) {
			gp_default_0_5_5(_a[0], _b[5], c + 31);
		}
	}
	if (a.gu() & 2) {
		if (b.gu() & 1) {
			gp_default_1_0_1(_a[1], _b[0], c + 1);
		}
		if (b.gu() & 2) {
			gp_default_1_1_0(_a[1], _b[1], c + 0);
		}
		if (b.gu() & 4) {
			gp_default_1_2_1(_a[1], _b[2], c + 1);
		}
		if (b.gu() & 8) {
			gp_default_1_3_2(_a[1], _b[3], c + 6);
		}
		if (b.gu() & 16) {
			gp_default_1_4_3(_a[1], _b[4], c + 16);
		}
		if (b.gu() & 32) {
			gp_default_1_5_4(_a[1], _b[5], c + 26);
		}
	}
	if (a.gu() & 4) {
		if (b.gu() & 1) {
			gp_default_2_0_2(_a[2], _b[0], c + 6);
		}
		if (b.gu() & 2) {
			gp_default_2_1_1(_a[2], _b[1], c + 1);
		}
		if (b.gu() & 4) {
			gp_default_2_2_0(_a[2], _b[2], c + 0);
		}
		if (b.gu() & 8) {
			gp_default_2_3_1(_a[2], _b[3], c + 1);
		}
		if (b.gu() & 16) {
			gp_default_2_4_2(_a[2], _b[4], c + 6);
		}
		if (b.gu() & 32) {
			gp_default_2_5_3(_a[2], _b[5], c + 16);
		}
	}
	if (a.gu() & 8) {
		if (b.gu() & 1) {
			gp_default_3_0_3(_a[3], _b[0], c + 16);
		}
		if (b.gu() & 2) {
			gp_default_3_1_2(_a[3], _b[1], c + 6);
		}
		if (b.gu() & 4) {
			gp_default_3_2_1(_a[3], _b[2], c + 1);
		}
		if (b.gu() & 8) {
			gp_default_3_3_0(_a[3], _b[3], c + 0);
		}
		if (b.gu() & 16) {
			gp_default_3_4_1(_a[3], _b[4], c + 1);
		}
		if (b.gu() & 32) {
			gp_default_3_5_2(_a[3], _b[5], c + 6);
		}
	}
	if (a.gu() & 16) {
		if (b.gu() & 1) {
			gp_default_4_0_4(_a[4], _b[0], c + 26);
		}
		if (b.gu() & 2) {
			gp_default_4_1_3(_a[4], _b[1], c + 16);
		}
		if (b.gu() & 4) {
			gp_default_4_2_2(_a[4], _b[2], c + 6);
		}
		if (b.gu() & 8) {
			gp_default_4_3_1(_a[4], _b[3], c + 1);
		}
		if (b.gu() & 16) {
			gp_default_4_4_0(_a[4], _b[4], c + 0);
		}
		if (b.gu() & 32) {
			gp_default_4_5_1(_a[4], _b[5], c + 1);
		}
	}
	if (a.gu() & 32) {
		if (b.gu() & 1) {
			gp_default_5_0_5(_a[5], _b[0], c + 31);
		}
		if (b.gu() & 2) {
			gp_default_5_1_4(_a[5], _b[1], c + 26);
		}
		if (b.gu() & 4) {
			gp_default_5_2_3(_a[5], _b[2], c + 16);
		}
		if (b.gu() & 8) {
			gp_default_5_3_2(_a[5], _b[3], c + 6);
		}
		if (b.gu() & 16) {
			gp_default_5_4_1(_a[5], _b[4], c + 1);
		}
		if (b.gu() & 32) {
			gp_default_5_5_0(_a[5], _b[5], c + 0);
		}
	}
	return mv_compress(c, 0.0f, 63);
}
mv lc(const mv &a, const mv &b)
{
	float c[32];
	const float* _a[6];
	const float* _b[6];
	a.expand(_a);
	b.expand(_b);
	c3ga::zero_N(c, 32);
	if (a.gu() & 1) {
		if (b.gu() & 1) {
			gp_default_0_0_0(_a[0], _b[0], c + 0);
		}
		if (b.gu() & 2) {
			gp_default_0_1_1(_a[0], _b[1], c + 1);
		}
		if (b.gu() & 4) {
			gp_default_0_2_2(_a[0], _b[2], c + 6);
		}
		if (b.gu() & 8) {
			gp_default_0_3_3(_a[0], _b[3], c + 16);
		}
		if (b.gu() & 16) {
			gp_default_0_4_4(_a[0], _b[4], c + 26);
		}
		if (b.gu() & 32) {
			gp_default_0_5_5(_a[0], _b[5], c + 31);
		}
	}
	if (a.gu() & 2) {
		if (b.gu() & 2) {
			gp_default_1_1_0(_a[1], _b[1], c + 0);
		}
		if (b.gu() & 4) {
			gp_default_1_2_1(_a[1], _b[2], c + 1);
		}
		if (b.gu() & 8) {
			gp_default_1_3_2(_a[1], _b[3], c + 6);
		}
		if (b.gu() & 16) {
			gp_default_1_4_3(_a[1], _b[4], c + 16);
		}
		if (b.gu() & 32) {
			gp_default_1_5_4(_a[1], _b[5], c + 26);
		}
	}
	if (a.gu() & 4) {
		if (b.gu() & 4) {
			gp_default_2_2_0(_a[2], _b[2], c + 0);
		}
		if (b.gu() & 8) {
			gp_default_2_3_1(_a[2], _b[3], c + 1);
		}
		if (b.gu() & 16) {
			gp_default_2_4_2(_a[2], _b[4], c + 6);
		}
		if (b.gu() & 32) {
			gp_default_2_5_3(_a[2], _b[5], c + 16);
		}
	}
	if (a.gu() & 8) {
		if (b.gu() & 8) {
			gp_default_3_3_0(_a[3], _b[3], c + 0);
		}
		if (b.gu() & 16) {
			gp_default_3_4_1(_a[3], _b[4], c + 1);
		}
		if (b.gu() & 32) {
			gp_default_3_5_2(_a[3], _b[5], c + 6);
		}
	}
	if (a.gu() & 16) {
		if (b.gu() & 16) {
			gp_default_4_4_0(_a[4], _b[4], c + 0);
		}
		if (b.gu() & 32) {
			gp_default_4_5_1(_a[4], _b[5], c + 1);
		}
	}
	if (a.gu() & 32) {
		if (b.gu() & 32) {
			gp_default_5_5_0(_a[5], _b[5], c + 0);
		}
	}
	return mv_compress(c, 0.0f, 63);
}
float norm(const mv &a)
{
	float n2 = 0.0f;
	float c[1];
	const float* _a[6];
	a.expand(_a);
	c3ga::zero_1(c);
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
	return ((n2 < 0.0f) ? ::sqrtf(-n2) : ::sqrtf(n2));
}
mv unit(const mv &a)
{
	int idx = 0;
	float n = norm_returns_scalar(a);
	int gu = a.gu();
	float c[32];
	
	if (a.gu() & 1) {
		copyDiv_0(a.getC() + idx, c + idx, n);
		idx += 1;
	}
	
	if (a.gu() & 2) {
		copyDiv_1(a.getC() + idx, c + idx, n);
		idx += 5;
	}
	
	if (a.gu() & 4) {
		copyDiv_2(a.getC() + idx, c + idx, n);
		idx += 10;
	}
	
	if (a.gu() & 8) {
		copyDiv_3(a.getC() + idx, c + idx, n);
		idx += 10;
	}
	
	if (a.gu() & 16) {
		copyDiv_4(a.getC() + idx, c + idx, n);
		idx += 5;
	}
	
	if (a.gu() & 32) {
		copyDiv_5(a.getC() + idx, c + idx, n);
	}
	return mv(gu, c);
}
mv applyVersor(const mv &a, const mv &b)
{
	return extractGrade(gp(gp(a, b), versorInverse(a)), b.gu());
}
mv applyUnitVersor(const mv &a, const mv &b)
{
	return extractGrade(gp(gp(a, b), reverse(a)), b.gu());
}
mv versorInverse(const mv &a)
{
	int idx = 0;
	float n2 = norm2_returns_scalar(a);
	int gu = a.gu();
	float c[32];
	
	if (a.gu() & 1) {
		copyDiv_0(a.getC() + idx, c + idx, n2);
		idx += 1;
	}
	
	if (a.gu() & 2) {
		copyDiv_1(a.getC() + idx, c + idx, n2);
		idx += 5;
	}
	
	if (a.gu() & 4) {
		copyDiv_2(a.getC() + idx, c + idx, -n2);
		idx += 10;
	}
	
	if (a.gu() & 8) {
		copyDiv_3(a.getC() + idx, c + idx, -n2);
		idx += 10;
	}
	
	if (a.gu() & 16) {
		copyDiv_4(a.getC() + idx, c + idx, n2);
		idx += 5;
	}
	
	if (a.gu() & 32) {
		copyDiv_5(a.getC() + idx, c + idx, n2);
	}
	return mv(gu, c);
}
bool equals(const mv &a, const mv &b, const float c)
{
	int aidx = 0, bidx = 0;
	
	if (a.gu() & 1) {
		if (b.gu() & 1) {
			if (!equals_0_0(a.getC() + aidx, b.getC() + bidx, c)) return false;
			bidx += 1;
		}
		else if (!zeroGroup_0(a.getC() + bidx, c)) return false;
		aidx += 1;
	}
	else if (b.gu() & 1) {
		if (!zeroGroup_0(b.getC() + bidx, c)) return false;
		bidx += 1;
	}
	
	if (a.gu() & 2) {
		if (b.gu() & 2) {
			if (!equals_1_1(a.getC() + aidx, b.getC() + bidx, c)) return false;
			bidx += 5;
		}
		else if (!zeroGroup_1(a.getC() + bidx, c)) return false;
		aidx += 5;
	}
	else if (b.gu() & 2) {
		if (!zeroGroup_1(b.getC() + bidx, c)) return false;
		bidx += 5;
	}
	
	if (a.gu() & 4) {
		if (b.gu() & 4) {
			if (!equals_2_2(a.getC() + aidx, b.getC() + bidx, c)) return false;
			bidx += 10;
		}
		else if (!zeroGroup_2(a.getC() + bidx, c)) return false;
		aidx += 10;
	}
	else if (b.gu() & 4) {
		if (!zeroGroup_2(b.getC() + bidx, c)) return false;
		bidx += 10;
	}
	
	if (a.gu() & 8) {
		if (b.gu() & 8) {
			if (!equals_3_3(a.getC() + aidx, b.getC() + bidx, c)) return false;
			bidx += 10;
		}
		else if (!zeroGroup_3(a.getC() + bidx, c)) return false;
		aidx += 10;
	}
	else if (b.gu() & 8) {
		if (!zeroGroup_3(b.getC() + bidx, c)) return false;
		bidx += 10;
	}
	
	if (a.gu() & 16) {
		if (b.gu() & 16) {
			if (!equals_4_4(a.getC() + aidx, b.getC() + bidx, c)) return false;
			bidx += 5;
		}
		else if (!zeroGroup_4(a.getC() + bidx, c)) return false;
		aidx += 5;
	}
	else if (b.gu() & 16) {
		if (!zeroGroup_4(b.getC() + bidx, c)) return false;
		bidx += 5;
	}
	
	if (a.gu() & 32) {
		if (b.gu() & 32) {
			if (!equals_5_5(a.getC() + aidx, b.getC() + bidx, c)) return false;
		}
		else if (!zeroGroup_5(a.getC() + bidx, c)) return false;
	}
	else if (b.gu() & 32) {
		if (!zeroGroup_5(b.getC() + bidx, c)) return false;
	}
	return true;
}
bool zero(const mv &a, const float b)
{
	int idx = 0;
	
	if (a.gu() & 1) {
		if (!zeroGroup_0(a.getC() + idx, b)) return false;
		idx += 1;
	}
	
	if (a.gu() & 2) {
		if (!zeroGroup_1(a.getC() + idx, b)) return false;
		idx += 5;
	}
	
	if (a.gu() & 4) {
		if (!zeroGroup_2(a.getC() + idx, b)) return false;
		idx += 10;
	}
	
	if (a.gu() & 8) {
		if (!zeroGroup_3(a.getC() + idx, b)) return false;
		idx += 10;
	}
	
	if (a.gu() & 16) {
		if (!zeroGroup_4(a.getC() + idx, b)) return false;
		idx += 5;
	}
	
	if (a.gu() & 32) {
		if (!zeroGroup_5(a.getC() + idx, b)) return false;
	}
	return true;
}
mv dual(const mv &a)
{
	int idx = 0;
	float c[32];
	c3ga::zero_N(c, 32);
	if (a.gu() & 1) {
		dual_default_0_5(a.getC() + idx, c + 31);
		idx += 1;
	}
	
	if (a.gu() & 2) {
		dual_default_1_4(a.getC() + idx, c + 26);
		idx += 5;
	}
	
	if (a.gu() & 4) {
		dual_default_2_3(a.getC() + idx, c + 16);
		idx += 10;
	}
	
	if (a.gu() & 8) {
		dual_default_3_2(a.getC() + idx, c + 6);
		idx += 10;
	}
	
	if (a.gu() & 16) {
		dual_default_4_1(a.getC() + idx, c + 1);
		idx += 5;
	}
	
	if (a.gu() & 32) {
		dual_default_5_0(a.getC() + idx, c + 0);
	}
	
	return mv_compress(c, 0.0f, 63);
}
mv undual(const mv &a)
{
	int idx = 0;
	float c[32];
	c3ga::zero_N(c, 32);
	if (a.gu() & 1) {
		undual_default_0_5(a.getC() + idx, c + 31);
		idx += 1;
	}
	
	if (a.gu() & 2) {
		undual_default_1_4(a.getC() + idx, c + 26);
		idx += 5;
	}
	
	if (a.gu() & 4) {
		undual_default_2_3(a.getC() + idx, c + 16);
		idx += 10;
	}
	
	if (a.gu() & 8) {
		undual_default_3_2(a.getC() + idx, c + 6);
		idx += 10;
	}
	
	if (a.gu() & 16) {
		undual_default_4_1(a.getC() + idx, c + 1);
		idx += 5;
	}
	
	if (a.gu() & 32) {
		undual_default_5_0(a.getC() + idx, c + 0);
	}
	
	return mv_compress(c, 0.0f, 63);
}


mv exp(const mv &x, int order /*  = 12 */) {
	unsigned long maxC;
	int scale = 1;
	mv xScaled;
	mv tmp1, tmp2; // temp mv used for various purposes
	mv xPow1, xPow2;
	mv *result1 = &tmp1, *result2 = &tmp2;
	float s_x2, a;
	int i;
   
	// First try special cases: check if (x * x) is scalar
	tmp1 = gp(x, x);
	s_x2 = _float(tmp1);
	if ((norm2_returns_scalar(tmp1) - s_x2 * s_x2) < 1E-06f) {
		// OK (x * x == ~scalar), so use special cases:
		if (s_x2 < 0.0f) {
			a = ::sqrtf(-s_x2);
			return sas(x, ::sinf(a) / a, ::cosf(a));
		}
		else if (s_x2 > 0.0f) {
			a = ::sqrtf(s_x2);
			return sas(x, ::sinhf(a) / a, ::coshf(a));
		}
		else {
			return sas(x, 1.0f, 1.0f);
		}
	}

	// else do general series eval . . .

	// result = 1 + ....	
	*result1 = 1.0f;
	if (order == 0) return *result1;

	// find scale (power of 2) such that its norm is < 1
	maxC = (unsigned long)x.largestCoordinate(); // unsigned type is fine, because largest coordinate is absolute
	scale = 1;
	if (maxC > 1) scale <<= 1;
	while (maxC)
	{
		maxC >>= 1;
		scale <<= 1;
	}

	// scale
	xScaled = gp(x, 1.0f / (float)scale); 

	// taylor series approximation
	xPow1 = 1.0f; 
	for (i = 1; i <= order; i++) {
		xPow2 = gp(xPow1, xScaled);
		xPow1 = gp(xPow2, 1.0f / (float)i);
		
		*result2 = add(*result1, xPow1); // result2 = result1 + xPow1
		std::swap(result1, result2); // result is always in 'result1' at end of loop
    }

	// undo scaling
	while (scale > 1)
	{
		*result2 = gp(*result1, *result1);
		std::swap(result1, result2); // result is always in 'result1' at end of loop
		scale >>= 1;
	}
    
    return *result1;
} // end of exp()

mv gp(const mv &a, const float b)
{
	float c[32];
	const float* _a[6];
	const float* _b[1] = {&b};
	a.expand(_a);
	c3ga::zero_N(c, 32);
	if (a.gu() & 1) {
			gp_default_0_0_0(_a[0], _b[0], c + 0);
	}
	if (a.gu() & 2) {
			gp_default_1_0_1(_a[1], _b[0], c + 1);
	}
	if (a.gu() & 4) {
			gp_default_2_0_2(_a[2], _b[0], c + 6);
	}
	if (a.gu() & 8) {
			gp_default_3_0_3(_a[3], _b[0], c + 16);
	}
	if (a.gu() & 16) {
			gp_default_4_0_4(_a[4], _b[0], c + 26);
	}
	if (a.gu() & 32) {
			gp_default_5_0_5(_a[5], _b[0], c + 31);
	}
	return mv_compress(c, 0.0f, 63);
}
float norm2(const mv &a)
{
	float n2 = 0.0f;
	float c[1];
	const float* _a[6];
	a.expand(_a);
	c3ga::zero_1(c);
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
mv sas(const mv &a, const float b, const float c)
{
	int idxa = 0, idxc = 0;
	int gu = a.gu() | ((c != 0.0) ? GRADE_0 : 0); // '| GRADE_0' to make sure the scalar part is included
	float C[32];
	
	if (a.gu() & 1) {
		copyMul_0(a.getC() + idxa, C + idxc, b);
		if (c != 0.0) C[idxc] += c;
		idxa += 1;
		idxc += 1;
	}
	else if (c != 0.0) {
		C[idxc] = c;
		idxc += 1;
	}
	
	if (a.gu() & 2) {
		copyMul_1(a.getC() + idxa, C + idxc, b);
		idxa += 5;
		idxc += 5;
	}
	
	if (a.gu() & 4) {
		copyMul_2(a.getC() + idxa, C + idxc, b);
		idxa += 10;
		idxc += 10;
	}
	
	if (a.gu() & 8) {
		copyMul_3(a.getC() + idxa, C + idxc, b);
		idxa += 10;
		idxc += 10;
	}
	
	if (a.gu() & 16) {
		copyMul_4(a.getC() + idxa, C + idxc, b);
		idxa += 5;
		idxc += 5;
	}
	
	if (a.gu() & 32) {
		copyMul_5(a.getC() + idxa, C + idxc, b);
	}
	return mv(gu, C);
}
} // end of namespace c3ga
