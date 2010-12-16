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

/*! \mainpage c3ga documentation
 *
 * c3ga implementation generated by Gaigen 2.5. 
 * 
 * 
 * License: 
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
  
 * 
 * \section intro_sec Introduction
 *
 * Todo
 * 
 */
#ifndef _C3GA_H_
#define _C3GA_H_

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>

/* group: 1*/
#define GROUP_0 1
/* group: no, e1, e2, e3, ni*/
#define GROUP_1 2
/* group: no^e1, no^e2, e1^e2, no^e3, e1^e3, e2^e3, no^ni, e1^ni, e2^ni, e3^ni*/
#define GROUP_2 4
/* group: no^e1^e2, no^e1^e3, no^e2^e3, e1^e2^e3, no^e1^ni, no^e2^ni, e1^e2^ni, no^e3^ni, e1^e3^ni, e2^e3^ni*/
#define GROUP_3 8
/* group: no^e1^e2^e3, no^e1^e2^ni, no^e1^e3^ni, no^e2^e3^ni, e1^e2^e3^ni*/
#define GROUP_4 16
/* group: no^e1^e2^e3^ni*/
#define GROUP_5 32
#define GRADE_0 1
#define GRADE_1 2
#define GRADE_2 4
#define GRADE_3 8
#define GRADE_4 16
#define GRADE_5 32


/** The dimension of the space. */
extern const int c3ga_spaceDim;

/** Number of groups/grades of coordinates in a multivector. */
extern const int c3ga_nbGroups;

/** The constants for the grades in an array. */
extern const int c3ga_grades[];

/** The constants for the groups in an array. */
extern const int c3ga_groups[];

/** Is the metric of the space Euclidean? (0 or 1). */
extern const int c3ga_metricEuclidean;

/** This array can be used to lookup the number of coordinates for a group part of a general multivector. */
extern const int c3ga_groupSize[6];

/** This array can be used to lookup the number of coordinates based on a group usage bitmap. */
extern const int c3ga_mvSize[64];

/** This array of ASCIIZ strings contains the names of the basis vectors. */
extern const char *c3ga_basisVectorNames[5];

/** This array of integers contains the order of basis elements in the general multivector
  * Use it to answer: 'what basis vectors are in the basis element at position [x]'? */
extern const int c3ga_basisElements[32][6];

/** This array of integers contains the 'sign' (even/odd permutation of canonical order) of basis elements in the general multivector
  * Use it to answer 'what is the permutation of the coordinate at index [x]'? */
extern const double c3ga_basisElementSignByIndex[32];

/** This array of integers contains the 'sign' (even/odd permutation of canonical order) of basis elements in the general multivector
  * Use it to answer 'what is the permutation of the coordinate of bitmap [x]'? */
extern const double c3ga_basisElementSignByBitmap[32];

/** This array of integers contains the order of basis elements in the general multivector
   * Use it to answer: 'at what index do I find basis element [x] (x = basis vector bitmap)?' */
extern const int c3ga_basisElementIndexByBitmap[32];

/** This array of integers contains the indices of basis elements in the general multivector
  * Use it to answer: 'what basis element do I find at index [x]'? */
extern const int c3ga_basisElementBitmapByIndex[32];

/** This array of grade of each basis elements in the general multivector
   * Use it to answer: 'what is the grade of basis element bitmap [x]'? */
extern const int c3ga_basisElementGradeByBitmap[32];

/** This array of group of each basis elements in the general multivector
  * Use it to answer: 'what is the group of basis element bitmap [x]'? */
extern const int c3ga_basisElementGroupByBitmap[32];

/**
 * This struct can hold a general multivector.
 * 
 * The coordinates are stored in type float.
 * 
 * There are 6 coordinate groups:
 * group 0:1  (grade 0).
 * group 1:no, e1, e2, e3, ni  (grade 1).
 * group 2:no^e1, no^e2, e1^e2, no^e3, e1^e3, e2^e3, no^ni, e1^ni, e2^ni, e3^ni  (grade 2).
 * group 3:no^e1^e2, no^e1^e3, no^e2^e3, e1^e2^e3, no^e1^ni, no^e2^ni, e1^e2^ni, no^e3^ni, e1^e3^ni, e2^e3^ni  (grade 3).
 * group 4:no^e1^e2^e3, no^e1^e2^ni, no^e1^e3^ni, no^e2^e3^ni, e1^e2^e3^ni  (grade 4).
 * group 5:no^e1^e2^e3^ni  (grade 5).
 * 
 * 32 floats are allocated inside the struct.
 */
typedef struct 
{
	/** group/grade usage (a bitmap which specifies which groups/grades are stored in 'c', below). */
	int gu;
	/** The coordinates (full). */
	float c[32];
} mv;

/** index of coordinate for e1 in normalizedPoint.c */
#define NORMALIZEDPOINT_E1 0
/** index of coordinate for e2 in normalizedPoint.c */
#define NORMALIZEDPOINT_E2 1
/** index of coordinate for e3 in normalizedPoint.c */
#define NORMALIZEDPOINT_E3 2
/** index of coordinate for ni in normalizedPoint.c */
#define NORMALIZEDPOINT_NI 3

/**
 * This struct can hold a specialized multivector of type normalizedPoint.
 * 
 * The coordinates are stored in type  float.
 * 
 * The variable non-zero coordinates are:
 *   - coordinate e1  (array index: NORMALIZEDPOINT_E1 = 0)
 *   - coordinate e2  (array index: NORMALIZEDPOINT_E2 = 1)
 *   - coordinate e3  (array index: NORMALIZEDPOINT_E3 = 2)
 *   - coordinate ni  (array index: NORMALIZEDPOINT_NI = 3)
 * 
 * The constant non-zero coordinates are:
 *   - no = 1
 * 
 */
typedef struct 
{
	/** The coordinates (stored in an array). */
	float c[4]; /* e1, e2, e3, ni*/
	/* no = 1*/
} normalizedPoint;


/**
 * This struct can hold a specialized multivector of type no_t.
 * 
 * The coordinates are stored in type  float.
 * 
 * The type is constant.
 * 
 * The constant non-zero coordinates are:
 *   - no = 1
 * 
 */
typedef struct 
{
	/** Filler, because C does not allow empty structs. */
	int filler;
	/* no = 1*/
} no_t;


/**
 * This struct can hold a specialized multivector of type e1_t.
 * 
 * The coordinates are stored in type  float.
 * 
 * The type is constant.
 * 
 * The constant non-zero coordinates are:
 *   - e1 = 1
 * 
 */
typedef struct 
{
	/** Filler, because C does not allow empty structs. */
	int filler;
	/* e1 = 1*/
} e1_t;


/**
 * This struct can hold a specialized multivector of type e2_t.
 * 
 * The coordinates are stored in type  float.
 * 
 * The type is constant.
 * 
 * The constant non-zero coordinates are:
 *   - e2 = 1
 * 
 */
typedef struct 
{
	/** Filler, because C does not allow empty structs. */
	int filler;
	/* e2 = 1*/
} e2_t;


/**
 * This struct can hold a specialized multivector of type e3_t.
 * 
 * The coordinates are stored in type  float.
 * 
 * The type is constant.
 * 
 * The constant non-zero coordinates are:
 *   - e3 = 1
 * 
 */
typedef struct 
{
	/** Filler, because C does not allow empty structs. */
	int filler;
	/* e3 = 1*/
} e3_t;


/**
 * This struct can hold a specialized multivector of type ni_t.
 * 
 * The coordinates are stored in type  float.
 * 
 * The type is constant.
 * 
 * The constant non-zero coordinates are:
 *   - ni = 1
 * 
 */
typedef struct 
{
	/** Filler, because C does not allow empty structs. */
	int filler;
	/* ni = 1*/
} ni_t;

/**
This function alters the formatting of 'string()'.
'format' = NULL will give you back the default.
*/
void c3ga_setStringFormat(const char *what, const char *format);

extern const char *c3ga_string_fp; /* = \"%2.2f\" */
extern const char *c3ga_string_start; /* = \"\" */
extern const char *c3ga_string_end; /* = \"\" */
extern const char *c3ga_string_mul; /* = \"*\" */
extern const char *c3ga_string_wedge; /* = \"^\" */
extern const char *c3ga_string_plus; /* = \" + \" */
extern const char *c3ga_string_minus; /* = \" - \" */


/** Writes value of 'V' to 'str' using float point precision 'fp' (e.g. %f). 'maxLength' is the length of 'str'. 'str' is returned. */
const char *toString_mv(const mv *V, char *str, int maxLength, const char *fp);

/** Writes value of 'V' to 'str' using float point precision 'fp' (e.g. %f). 'maxLength' is the length of 'str'. 'str' is returned. */
const char *toString_normalizedPoint(const normalizedPoint *V, char *str, int maxLength, const char *fp);
/** Writes value of 'V' to 'str' using float point precision 'fp' (e.g. %f). 'maxLength' is the length of 'str'. 'str' is returned. */
const char *toString_no_t(const no_t *V, char *str, int maxLength, const char *fp);
/** Writes value of 'V' to 'str' using float point precision 'fp' (e.g. %f). 'maxLength' is the length of 'str'. 'str' is returned. */
const char *toString_e1_t(const e1_t *V, char *str, int maxLength, const char *fp);
/** Writes value of 'V' to 'str' using float point precision 'fp' (e.g. %f). 'maxLength' is the length of 'str'. 'str' is returned. */
const char *toString_e2_t(const e2_t *V, char *str, int maxLength, const char *fp);
/** Writes value of 'V' to 'str' using float point precision 'fp' (e.g. %f). 'maxLength' is the length of 'str'. 'str' is returned. */
const char *toString_e3_t(const e3_t *V, char *str, int maxLength, const char *fp);
/** Writes value of 'V' to 'str' using float point precision 'fp' (e.g. %f). 'maxLength' is the length of 'str'. 'str' is returned. */
const char *toString_ni_t(const ni_t *V, char *str, int maxLength, const char *fp);



extern no_t no;
extern e1_t e1;
extern e2_t e2;
extern e3_t e3;
extern ni_t ni;


/** Sets 1 float to zero */
void c3ga_float_zero_1(float *dst);
/** Copies 1 float from 'src' to 'dst' */
void c3ga_float_copy_1(float *dst, const float *src);
/** Sets 2 floats to zero */
void c3ga_float_zero_2(float *dst);
/** Copies 2 floats from 'src' to 'dst' */
void c3ga_float_copy_2(float *dst, const float *src);
/** Sets 3 floats to zero */
void c3ga_float_zero_3(float *dst);
/** Copies 3 floats from 'src' to 'dst' */
void c3ga_float_copy_3(float *dst, const float *src);
/** Sets 4 floats to zero */
void c3ga_float_zero_4(float *dst);
/** Copies 4 floats from 'src' to 'dst' */
void c3ga_float_copy_4(float *dst, const float *src);
/** Sets 5 floats to zero */
void c3ga_float_zero_5(float *dst);
/** Copies 5 floats from 'src' to 'dst' */
void c3ga_float_copy_5(float *dst, const float *src);
/** Sets 6 floats to zero */
void c3ga_float_zero_6(float *dst);
/** Copies 6 floats from 'src' to 'dst' */
void c3ga_float_copy_6(float *dst, const float *src);
/** Sets 7 floats to zero */
void c3ga_float_zero_7(float *dst);
/** Copies 7 floats from 'src' to 'dst' */
void c3ga_float_copy_7(float *dst, const float *src);
/** Sets 8 floats to zero */
void c3ga_float_zero_8(float *dst);
/** Copies 8 floats from 'src' to 'dst' */
void c3ga_float_copy_8(float *dst, const float *src);
/** Sets 9 floats to zero */
void c3ga_float_zero_9(float *dst);
/** Copies 9 floats from 'src' to 'dst' */
void c3ga_float_copy_9(float *dst, const float *src);
/** Sets 10 floats to zero */
void c3ga_float_zero_10(float *dst);
/** Copies 10 floats from 'src' to 'dst' */
void c3ga_float_copy_10(float *dst, const float *src);
/** Sets 11 floats to zero */
void c3ga_float_zero_11(float *dst);
/** Copies 11 floats from 'src' to 'dst' */
void c3ga_float_copy_11(float *dst, const float *src);
/** Sets 12 floats to zero */
void c3ga_float_zero_12(float *dst);
/** Copies 12 floats from 'src' to 'dst' */
void c3ga_float_copy_12(float *dst, const float *src);
/** Sets 13 floats to zero */
void c3ga_float_zero_13(float *dst);
/** Copies 13 floats from 'src' to 'dst' */
void c3ga_float_copy_13(float *dst, const float *src);
/** Sets 14 floats to zero */
void c3ga_float_zero_14(float *dst);
/** Copies 14 floats from 'src' to 'dst' */
void c3ga_float_copy_14(float *dst, const float *src);
/** Sets 15 floats to zero */
void c3ga_float_zero_15(float *dst);
/** Copies 15 floats from 'src' to 'dst' */
void c3ga_float_copy_15(float *dst, const float *src);
/** Sets 16 floats to zero */
void c3ga_float_zero_16(float *dst);
/** Copies 16 floats from 'src' to 'dst' */
void c3ga_float_copy_16(float *dst, const float *src);
/** Sets N floats to zero */
void c3ga_float_zero_N(float *dst, int N);
/** Copies N floats from 'src' to 'dst' */
void c3ga_float_copy_N(float *dst, const float *src, int N);
/** Sets a mv to zero */
void mv_setZero(mv *M);
/** Sets a mv to a scalar value */
void mv_setScalar(mv *M, float val);
/** Sets a mv to the value in the array. 'gu' is a group usage bitmap. */
void mv_setArray(mv *M, int gu, const float *arr);
/** Copies a mv */
void mv_copy(mv *dst, const mv *src);
/** Copies a mv to a normalizedPoint (coordinates/basis blades which cannot be represented are silenty lost). */
void mv_to_normalizedPoint(normalizedPoint *dst, const mv *src);
/** Copies a mv to a no_t (coordinates/basis blades which cannot be represented are silenty lost). */
void mv_to_no_t(no_t *dst, const mv *src);
/** Copies a mv to a e1_t (coordinates/basis blades which cannot be represented are silenty lost). */
void mv_to_e1_t(e1_t *dst, const mv *src);
/** Copies a mv to a e2_t (coordinates/basis blades which cannot be represented are silenty lost). */
void mv_to_e2_t(e2_t *dst, const mv *src);
/** Copies a mv to a e3_t (coordinates/basis blades which cannot be represented are silenty lost). */
void mv_to_e3_t(e3_t *dst, const mv *src);
/** Copies a mv to a ni_t (coordinates/basis blades which cannot be represented are silenty lost). */
void mv_to_ni_t(ni_t *dst, const mv *src);
/** Copies a normalizedPoint to a mv */
void normalizedPoint_to_mv(mv *dst, const normalizedPoint *src);
/** Copies a no_t to a mv */
void no_t_to_mv(mv *dst, const no_t *src);
/** Copies a e1_t to a mv */
void e1_t_to_mv(mv *dst, const e1_t *src);
/** Copies a e2_t to a mv */
void e2_t_to_mv(mv *dst, const e2_t *src);
/** Copies a e3_t to a mv */
void e3_t_to_mv(mv *dst, const e3_t *src);
/** Copies a ni_t to a mv */
void ni_t_to_mv(mv *dst, const ni_t *src);
/** Allocates memory to store coordinate group 0 */
void mv_reserveGroup_0(mv *A);
/** Allocates memory to store coordinate group 1 */
void mv_reserveGroup_1(mv *A);
/** Allocates memory to store coordinate group 2 */
void mv_reserveGroup_2(mv *A);
/** Allocates memory to store coordinate group 3 */
void mv_reserveGroup_3(mv *A);
/** Allocates memory to store coordinate group 4 */
void mv_reserveGroup_4(mv *A);
/** Allocates memory to store coordinate group 5 */
void mv_reserveGroup_5(mv *A);
/** Returns the 1 coordinate of 'A' */
float mv_scalar(const mv *A);
/** Returns the 1 coordinate of 'A' */
float mv_float(const mv *A);
/** Sets the 1 coordinate of 'A' */
void mv_set_scalar(mv *A, float scalar_coord);
/** Returns the no coordinate of 'A' */
float mv_no(const mv *A);
/** Sets the no coordinate of 'A' */
void mv_set_no(mv *A, float no_coord);
/** Returns the e1 coordinate of 'A' */
float mv_e1(const mv *A);
/** Sets the e1 coordinate of 'A' */
void mv_set_e1(mv *A, float e1_coord);
/** Returns the e2 coordinate of 'A' */
float mv_e2(const mv *A);
/** Sets the e2 coordinate of 'A' */
void mv_set_e2(mv *A, float e2_coord);
/** Returns the e3 coordinate of 'A' */
float mv_e3(const mv *A);
/** Sets the e3 coordinate of 'A' */
void mv_set_e3(mv *A, float e3_coord);
/** Returns the ni coordinate of 'A' */
float mv_ni(const mv *A);
/** Sets the ni coordinate of 'A' */
void mv_set_ni(mv *A, float ni_coord);
/** Returns the no^e1 coordinate of 'A' */
float mv_no_e1(const mv *A);
/** Sets the no^e1 coordinate of 'A' */
void mv_set_no_e1(mv *A, float no_e1_coord);
/** Returns the no^e2 coordinate of 'A' */
float mv_no_e2(const mv *A);
/** Sets the no^e2 coordinate of 'A' */
void mv_set_no_e2(mv *A, float no_e2_coord);
/** Returns the e1^e2 coordinate of 'A' */
float mv_e1_e2(const mv *A);
/** Sets the e1^e2 coordinate of 'A' */
void mv_set_e1_e2(mv *A, float e1_e2_coord);
/** Returns the no^e3 coordinate of 'A' */
float mv_no_e3(const mv *A);
/** Sets the no^e3 coordinate of 'A' */
void mv_set_no_e3(mv *A, float no_e3_coord);
/** Returns the e1^e3 coordinate of 'A' */
float mv_e1_e3(const mv *A);
/** Sets the e1^e3 coordinate of 'A' */
void mv_set_e1_e3(mv *A, float e1_e3_coord);
/** Returns the e2^e3 coordinate of 'A' */
float mv_e2_e3(const mv *A);
/** Sets the e2^e3 coordinate of 'A' */
void mv_set_e2_e3(mv *A, float e2_e3_coord);
/** Returns the no^ni coordinate of 'A' */
float mv_no_ni(const mv *A);
/** Sets the no^ni coordinate of 'A' */
void mv_set_no_ni(mv *A, float no_ni_coord);
/** Returns the e1^ni coordinate of 'A' */
float mv_e1_ni(const mv *A);
/** Sets the e1^ni coordinate of 'A' */
void mv_set_e1_ni(mv *A, float e1_ni_coord);
/** Returns the e2^ni coordinate of 'A' */
float mv_e2_ni(const mv *A);
/** Sets the e2^ni coordinate of 'A' */
void mv_set_e2_ni(mv *A, float e2_ni_coord);
/** Returns the e3^ni coordinate of 'A' */
float mv_e3_ni(const mv *A);
/** Sets the e3^ni coordinate of 'A' */
void mv_set_e3_ni(mv *A, float e3_ni_coord);
/** Returns the no^e1^e2 coordinate of 'A' */
float mv_no_e1_e2(const mv *A);
/** Sets the no^e1^e2 coordinate of 'A' */
void mv_set_no_e1_e2(mv *A, float no_e1_e2_coord);
/** Returns the no^e1^e3 coordinate of 'A' */
float mv_no_e1_e3(const mv *A);
/** Sets the no^e1^e3 coordinate of 'A' */
void mv_set_no_e1_e3(mv *A, float no_e1_e3_coord);
/** Returns the no^e2^e3 coordinate of 'A' */
float mv_no_e2_e3(const mv *A);
/** Sets the no^e2^e3 coordinate of 'A' */
void mv_set_no_e2_e3(mv *A, float no_e2_e3_coord);
/** Returns the e1^e2^e3 coordinate of 'A' */
float mv_e1_e2_e3(const mv *A);
/** Sets the e1^e2^e3 coordinate of 'A' */
void mv_set_e1_e2_e3(mv *A, float e1_e2_e3_coord);
/** Returns the no^e1^ni coordinate of 'A' */
float mv_no_e1_ni(const mv *A);
/** Sets the no^e1^ni coordinate of 'A' */
void mv_set_no_e1_ni(mv *A, float no_e1_ni_coord);
/** Returns the no^e2^ni coordinate of 'A' */
float mv_no_e2_ni(const mv *A);
/** Sets the no^e2^ni coordinate of 'A' */
void mv_set_no_e2_ni(mv *A, float no_e2_ni_coord);
/** Returns the e1^e2^ni coordinate of 'A' */
float mv_e1_e2_ni(const mv *A);
/** Sets the e1^e2^ni coordinate of 'A' */
void mv_set_e1_e2_ni(mv *A, float e1_e2_ni_coord);
/** Returns the no^e3^ni coordinate of 'A' */
float mv_no_e3_ni(const mv *A);
/** Sets the no^e3^ni coordinate of 'A' */
void mv_set_no_e3_ni(mv *A, float no_e3_ni_coord);
/** Returns the e1^e3^ni coordinate of 'A' */
float mv_e1_e3_ni(const mv *A);
/** Sets the e1^e3^ni coordinate of 'A' */
void mv_set_e1_e3_ni(mv *A, float e1_e3_ni_coord);
/** Returns the e2^e3^ni coordinate of 'A' */
float mv_e2_e3_ni(const mv *A);
/** Sets the e2^e3^ni coordinate of 'A' */
void mv_set_e2_e3_ni(mv *A, float e2_e3_ni_coord);
/** Returns the no^e1^e2^e3 coordinate of 'A' */
float mv_no_e1_e2_e3(const mv *A);
/** Sets the no^e1^e2^e3 coordinate of 'A' */
void mv_set_no_e1_e2_e3(mv *A, float no_e1_e2_e3_coord);
/** Returns the no^e1^e2^ni coordinate of 'A' */
float mv_no_e1_e2_ni(const mv *A);
/** Sets the no^e1^e2^ni coordinate of 'A' */
void mv_set_no_e1_e2_ni(mv *A, float no_e1_e2_ni_coord);
/** Returns the no^e1^e3^ni coordinate of 'A' */
float mv_no_e1_e3_ni(const mv *A);
/** Sets the no^e1^e3^ni coordinate of 'A' */
void mv_set_no_e1_e3_ni(mv *A, float no_e1_e3_ni_coord);
/** Returns the no^e2^e3^ni coordinate of 'A' */
float mv_no_e2_e3_ni(const mv *A);
/** Sets the no^e2^e3^ni coordinate of 'A' */
void mv_set_no_e2_e3_ni(mv *A, float no_e2_e3_ni_coord);
/** Returns the e1^e2^e3^ni coordinate of 'A' */
float mv_e1_e2_e3_ni(const mv *A);
/** Sets the e1^e2^e3^ni coordinate of 'A' */
void mv_set_e1_e2_e3_ni(mv *A, float e1_e2_e3_ni_coord);
/** Returns the no^e1^e2^e3^ni coordinate of 'A' */
float mv_no_e1_e2_e3_ni(const mv *A);
/** Sets the no^e1^e2^e3^ni coordinate of 'A' */
void mv_set_no_e1_e2_e3_ni(mv *A, float no_e1_e2_e3_ni_coord);

/** Returns absolute largest coordinate in mv. */
float mv_largestCoordinate(const mv *x);
/** Returns absolute largest coordinate in mv, and the bitmap of the corresponding basis blade (in 'bm'). */
float mv_largestBasisBlade(const mv *x, unsigned int *bm);

/** Sets normalizedPoint to zero */
void normalizedPoint_setZero(normalizedPoint *_dst);


/** Sets normalizedPoint to specified coordinates */
void normalizedPoint_set(normalizedPoint *_dst, const float _e1, const float _e2, const float _e3, const float _ni);

/** Sets normalizedPoint to specified coordinates */
void normalizedPoint_setArray(normalizedPoint *_dst, const float *A);

/** Copies normalizedPoint: a = _dst */
void normalizedPoint_copy(normalizedPoint *_dst, const normalizedPoint *a);


/** Returns abs largest coordinate of normalizedPoint */
float normalizedPoint_largestCoordinate(const normalizedPoint *x);
/** Returns abs largest coordinate of no_t */
float no_t_largestCoordinate(const no_t *x);
/** Returns abs largest coordinate of e1_t */
float e1_t_largestCoordinate(const e1_t *x);
/** Returns abs largest coordinate of e2_t */
float e2_t_largestCoordinate(const e2_t *x);
/** Returns abs largest coordinate of e3_t */
float e3_t_largestCoordinate(const e3_t *x);
/** Returns abs largest coordinate of ni_t */
float ni_t_largestCoordinate(const ni_t *x);

/** Returns scalar part of  normalizedPoint */
float normalizedPoint_float(const normalizedPoint *x);
/** Returns scalar part of  no_t */
float no_t_float(const no_t *x);
/** Returns scalar part of  e1_t */
float e1_t_float(const e1_t *x);
/** Returns scalar part of  e2_t */
float e2_t_float(const e2_t *x);
/** Returns scalar part of  e3_t */
float e3_t_float(const e3_t *x);
/** Returns scalar part of  ni_t */
float ni_t_float(const ni_t *x);
/**
 * Returns mv + mv.
 */
void add_mv_mv(mv *_dst, const mv *a, const mv *b);
/**
 * Returns mv - mv.
 */
void subtract_mv_mv(mv *_dst, const mv *a, const mv *b);
/**
 * Returns conformal point.
 */
void cgaPoint_float_float_float(normalizedPoint *_dst, const float a, const float b, const float c);
/**
 * Returns grade groupBitmap of  mv.
 */
void extractGrade_mv(mv *_dst, const mv *a, const int groupBitmap);
/**
 * Returns negation of mv.
 */
void negate_mv(mv *_dst, const mv *a);
/**
 * Returns reverse of mv.
 */
void reverse_mv(mv *_dst, const mv *a);
/**
 * Returns grade involution of mv.
 */
void gradeInvolution_mv(mv *_dst, const mv *a);
/**
 * Returns Clifford conjugate of mv.
 */
void cliffordConjugate_mv(mv *_dst, const mv *a);
/**
 * Returns geometric product of mv and mv.
 */
void gp_mv_mv(mv *_dst, const mv *a, const mv *b);
/**
 * Returns outer product of mv and mv.
 */
void op_mv_mv(mv *_dst, const mv *a, const mv *b);
/**
 * Returns scalar product of mv and mv.
 */
float sp_mv_mv(const mv *a, const mv *b);
/**
 * Returns Modified Hestenes inner product of mv and mv.
 */
void mhip_mv_mv(mv *_dst, const mv *a, const mv *b);
/**
 * Returns left contraction of mv and mv.
 */
void lc_mv_mv(mv *_dst, const mv *a, const mv *b);
/**
 * Returns norm of mv using default metric.
 */
float norm_mv(const mv *a);
/**
 * internal conversion function
 */
float norm_mv_returns_scalar(const mv *a);
/**
 * Returns unit of mv using default metric.
 */
void unit_mv(mv *_dst, const mv *a);
/**
 * Returns a * b * inverse(a) using default metric.
 */
void applyVersor_mv_mv(mv *_dst, const mv *a, const mv *b);
/**
 * Returns a * b * reverse(a) using default metric. Only gives the correct result when the versor has a positive squared norm.
 * 
 */
void applyUnitVersor_mv_mv(mv *_dst, const mv *a, const mv *b);
/**
 * Returns versor inverse of a using default metric.
 */
void versorInverse_mv(mv *_dst, const mv *a);
/**
 * Returns whether input multivectors are equal up to an epsilon c.
 */
int equals_mv_mv_float(const mv *a, const mv *b, const float c);
/**
 * Returns true if all coordinates of a are abs <= b
 */
int zero_mv_float(const mv *a, const float b);
/**
 * Returns dual of mv using default metric.
 */
void dual_mv(mv *_dst, const mv *a);
/**
 * Returns undual of mv using default metric.
 */
void undual_mv(mv *_dst, const mv *a);

/** Computes exp of mv.
 */
void exp_mv(mv *R, const mv *x, int order);
/**
 * Returns geometric product of mv and float.
 */
void gp_mv_float(mv *_dst, const mv *a, const float b);
/**
 * Returns norm2 of mv using default metric.
 */
float norm2_mv(const mv *a);
/**
 * internal conversion function
 */
float norm2_mv_returns_scalar(const mv *a);
/**
 * Returns float b * mv a + float c.
 */
void sas_mv_float_float(mv *_dst, const mv *a, const float b, const float c);


/** structure used by custom parser */
struct c3gaParseMultivectorData {
	/** the parsed value (initialize this pointer to a valid multivector before calling) */
	mv *value;
	/** this string will contain an error message when error is true */
	char message[256];
};

/** 
Parses 'str' (output of toString_mv()) and stores result in 'val' 
Returns true when 'str' parsed correctly.
*/
int parse_mv(mv *val, const char *str);

/** 
Parses 'str' (output of toString_mv()) and stores result in 'data'. 
'strSourceName' is the name of the source of 'str' (for example, a filename).
It is used for error messages.

Returns true when 'str' parsed correctly.
Otherwise a lexer or parser error occured and you can check 
the errors in 'data->message'.
*/
int parse_mvEx(struct c3gaParseMultivectorData *data, const char *str, const char *strSourceName);
#endif /* _C3GA_H_ */
