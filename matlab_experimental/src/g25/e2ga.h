/*
Copyright (C) 2008 Some Random Person*/
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

/*! \mainpage e2ga documentation
 *
 * e2ga implementation generated by Gaigen 2.5. 
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
#ifndef _E2GA_H_
#define _E2GA_H_

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>

// group: 1
#define GROUP_0 1
// group: e1, e2
#define GROUP_1 2
// group: e1^e2
#define GROUP_2 4
#define GRADE_0 1
#define GRADE_1 2
#define GRADE_2 4


/// The dimension of the space:
extern const int e2ga_spaceDim;

/// Number of groups/grades of coordinates in multivector:
extern const int e2ga_nbGroups;

/// The constants for the grades in an array:
extern const int e2ga_grades[];

/// The constants for the groups in an array:
extern const int e2ga_groups[];

/// Is the metric of the space Euclidean? (0 or 1)
extern const int e2ga_metricEuclidean;

/// This array can be used to lookup the number of coordinates for a group part of a general multivector
extern const int e2ga_groupSize[3];

/// This array can be used to lookup the number of coordinates based on a group usage bitmap
extern const int e2ga_mvSize[8];

/// This array of ASCIIZ strings contains the names of the basis vectors
extern const char *e2ga_basisVectorNames[2];

/// This array of integers contains the order of basis elements in the general multivector
/// Use it to answer: 'what basis vectors are in the basis element at position [x]'?
extern const int e2ga_basisElements[4][3];

/// This array of integers contains the 'sign' (even/odd permutation of canonical order) of basis elements in the general multivector
/// Use it to answer 'what is the permutation of the coordinate at index [x]'?
extern const double e2ga_basisElementSignByIndex[4];

/// This array of integers contains the 'sign' (even/odd permutation of canonical order) of basis elements in the general multivector
/// Use it to answer 'what is the permutation of the coordinate of bitmap [x]'?
extern const double e2ga_basisElementSignByBitmap[4];

/// This array of integers contains the order of basis elements in the general multivector
/// Use it to answer: 'at what index do I find basis element [x] (x = basis vector bitmap)?'
extern const int e2ga_basisElementIndexByBitmap[4];

/// This array of integers contains the indices of basis elements in the general multivector
/// Use it to answer: 'what basis element do I find at index [x]'?
extern const int e2ga_basisElementBitmapByIndex[4];

/// This array of grade of each basis elements in the general multivector
/// Use it to answer: 'what is the grade of basis element bitmap [x]'?
extern const int e2ga_basisElementGradeByBitmap[4];

/// This array of group of each basis elements in the general multivector
/// Use it to answer: 'what is the group of basis element bitmap [x]'?
extern const int e2ga_basisElementGroupByBitmap[4];

/**
 * This struct can hold a general multivector.
 * 
 * The coordinates are stored in type double.
 * 
 * There are 3 coordinate groups:
 * group 0:1  (grade 0).
 * group 1:e1, e2  (grade 1).
 * group 2:e1^e2  (grade 2).
 * 
 * 4 doubles are allocated inside the struct.
 */
typedef struct 
{
	int gu; ///< group/grade usage (a bitmap which specifies which groups/grades are stored in 'c', below).
	double c[4]; ///< the coordinates (full)
} mv;

/** index of coordinate for e1 in vector.c */
#define VECTOR_E1 0
/** index of coordinate for e2 in vector.c */
#define VECTOR_E2 1

/**
 * This struct can hold a specialized multivector of type vector.
 * 
 * The coordinates are stored in type  double.
 * 
 * The variable non-zero coordinates are:
 *   - coordinate e1  (array index: VECTOR_E1 = 0)
 *   - coordinate e2  (array index: VECTOR_E2 = 1)
 * 
 * The type has no constant coordinates.
 * 
 */
typedef struct 
{
	/** The coordinates (stored in an array). */
	double c[2]; // e1, e2
} vector;

/** index of coordinate for 1 in rotor.c */
#define ROTOR_SCALAR 0
/** index of coordinate for e1^e2 in rotor.c */
#define ROTOR_E1_E2 1

/**
 * This struct can hold a specialized multivector of type rotor.
 * 
 * The coordinates are stored in type  double.
 * 
 * The variable non-zero coordinates are:
 *   - coordinate 1  (array index: ROTOR_SCALAR = 0)
 *   - coordinate e1^e2  (array index: ROTOR_E1_E2 = 1)
 * 
 * The type has no constant coordinates.
 * 
 */
typedef struct 
{
	/** The coordinates (stored in an array). */
	double c[2]; // 1, e1^e2
} rotor;


/**
 * This struct can hold a specialized multivector of type e1_t.
 * 
 * The coordinates are stored in type  double.
 * 
 * The type is constant.
 * 
 * The constant non-zero coordinates are:
 *   - e1 = 1
 * 
 */
typedef struct 
{
	int filler; ///< Filler, because C does not allow empty structs.
	// e1 = 1
} e1_t;


/**
 * This struct can hold a specialized multivector of type e2_t.
 * 
 * The coordinates are stored in type  double.
 * 
 * The type is constant.
 * 
 * The constant non-zero coordinates are:
 *   - e2 = 1
 * 
 */
typedef struct 
{
	int filler; ///< Filler, because C does not allow empty structs.
	// e2 = 1
} e2_t;


/**
 * This struct can hold a specialized multivector of type I2_t.
 * 
 * The coordinates are stored in type  double.
 * 
 * The type is constant.
 * 
 * The constant non-zero coordinates are:
 *   - e1^e2 = 1
 * 
 */
typedef struct 
{
	int filler; ///< Filler, because C does not allow empty structs.
	// e1^e2 = 1
} I2_t;

/**
This function alters the formatting of 'string()'.
'format' = NULL will give you back the default.
*/
void e2ga_setStringFormat(const char *what, const char *format);

extern const char *e2ga_string_fp; /* = \"%2.2f\" */
extern const char *e2ga_string_start; /* = \"\" */
extern const char *e2ga_string_end; /* = \"\" */
extern const char *e2ga_string_mul; /* = \"*\" */
extern const char *e2ga_string_wedge; /* = \"^\" */
extern const char *e2ga_string_plus; /* = \" + \" */
extern const char *e2ga_string_minus; /* = \" - \" */


/// Writes value of 'V' to 'str' using float point precision 'fp' (e.g. %f). 'maxLength' is the length of 'str'. 'str' is returned.
const char *toString_mv(const mv *V, char *str, int maxLength, const char *fp);

/// Writes value of 'V' to 'str' using float point precision 'fp' (e.g. %f). 'maxLength' is the length of 'str'. 'str' is returned.
const char *toString_vector(const vector *V, char *str, int maxLength, const char *fp);
/// Writes value of 'V' to 'str' using float point precision 'fp' (e.g. %f). 'maxLength' is the length of 'str'. 'str' is returned.
const char *toString_rotor(const rotor *V, char *str, int maxLength, const char *fp);
/// Writes value of 'V' to 'str' using float point precision 'fp' (e.g. %f). 'maxLength' is the length of 'str'. 'str' is returned.
const char *toString_e1_t(const e1_t *V, char *str, int maxLength, const char *fp);
/// Writes value of 'V' to 'str' using float point precision 'fp' (e.g. %f). 'maxLength' is the length of 'str'. 'str' is returned.
const char *toString_e2_t(const e2_t *V, char *str, int maxLength, const char *fp);
/// Writes value of 'V' to 'str' using float point precision 'fp' (e.g. %f). 'maxLength' is the length of 'str'. 'str' is returned.
const char *toString_I2_t(const I2_t *V, char *str, int maxLength, const char *fp);



extern e1_t e1;
extern e2_t e2;
extern I2_t I2;


/** Sets 1 double to zero */
void e2ga_double_zero_1(double *dst);
/** Copies 1 double from 'src' to 'dst' */
void e2ga_double_copy_1(double *dst, const double *src);
/** Sets 2 doubles to zero */
void e2ga_double_zero_2(double *dst);
/** Copies 2 doubles from 'src' to 'dst' */
void e2ga_double_copy_2(double *dst, const double *src);
/** Sets 3 doubles to zero */
void e2ga_double_zero_3(double *dst);
/** Copies 3 doubles from 'src' to 'dst' */
void e2ga_double_copy_3(double *dst, const double *src);
/** Sets 4 doubles to zero */
void e2ga_double_zero_4(double *dst);
/** Copies 4 doubles from 'src' to 'dst' */
void e2ga_double_copy_4(double *dst, const double *src);
/** Sets 5 doubles to zero */
void e2ga_double_zero_5(double *dst);
/** Copies 5 doubles from 'src' to 'dst' */
void e2ga_double_copy_5(double *dst, const double *src);
/** Sets 6 doubles to zero */
void e2ga_double_zero_6(double *dst);
/** Copies 6 doubles from 'src' to 'dst' */
void e2ga_double_copy_6(double *dst, const double *src);
/** Sets 7 doubles to zero */
void e2ga_double_zero_7(double *dst);
/** Copies 7 doubles from 'src' to 'dst' */
void e2ga_double_copy_7(double *dst, const double *src);
/** Sets 8 doubles to zero */
void e2ga_double_zero_8(double *dst);
/** Copies 8 doubles from 'src' to 'dst' */
void e2ga_double_copy_8(double *dst, const double *src);
/** Sets 9 doubles to zero */
void e2ga_double_zero_9(double *dst);
/** Copies 9 doubles from 'src' to 'dst' */
void e2ga_double_copy_9(double *dst, const double *src);
/** Sets 10 doubles to zero */
void e2ga_double_zero_10(double *dst);
/** Copies 10 doubles from 'src' to 'dst' */
void e2ga_double_copy_10(double *dst, const double *src);
/** Sets 11 doubles to zero */
void e2ga_double_zero_11(double *dst);
/** Copies 11 doubles from 'src' to 'dst' */
void e2ga_double_copy_11(double *dst, const double *src);
/** Sets 12 doubles to zero */
void e2ga_double_zero_12(double *dst);
/** Copies 12 doubles from 'src' to 'dst' */
void e2ga_double_copy_12(double *dst, const double *src);
/** Sets 13 doubles to zero */
void e2ga_double_zero_13(double *dst);
/** Copies 13 doubles from 'src' to 'dst' */
void e2ga_double_copy_13(double *dst, const double *src);
/** Sets 14 doubles to zero */
void e2ga_double_zero_14(double *dst);
/** Copies 14 doubles from 'src' to 'dst' */
void e2ga_double_copy_14(double *dst, const double *src);
/** Sets 15 doubles to zero */
void e2ga_double_zero_15(double *dst);
/** Copies 15 doubles from 'src' to 'dst' */
void e2ga_double_copy_15(double *dst, const double *src);
/** Sets 16 doubles to zero */
void e2ga_double_zero_16(double *dst);
/** Copies 16 doubles from 'src' to 'dst' */
void e2ga_double_copy_16(double *dst, const double *src);
/** Sets N doubles to zero */
void e2ga_double_zero_N(double *dst, int N);
/** Copies N doubles from 'src' to 'dst' */
void e2ga_double_copy_N(double *dst, const double *src, int N);
// decl SB:
/** Sets a mv to zero */
void mv_setZero(mv *M);
/** Sets a mv to a scalar value */
void mv_setScalar(mv *M, double val);
/** Sets a mv to the value in the array. 'gu' is a group usage bitmap. */
void mv_setArray(mv *M, int gu, const double *arr);
/** Copies a mv */
void mv_copy(mv *dst, const mv *src);
/** Copies a mv to a vector (coordinates/basis blades which cannot be represented are silenty lost). */
void mv_to_vector(vector *dst, const mv *src);
/** Copies a mv to a rotor (coordinates/basis blades which cannot be represented are silenty lost). */
void mv_to_rotor(rotor *dst, const mv *src);
/** Copies a mv to a e1_t (coordinates/basis blades which cannot be represented are silenty lost). */
void mv_to_e1_t(e1_t *dst, const mv *src);
/** Copies a mv to a e2_t (coordinates/basis blades which cannot be represented are silenty lost). */
void mv_to_e2_t(e2_t *dst, const mv *src);
/** Copies a mv to a I2_t (coordinates/basis blades which cannot be represented are silenty lost). */
void mv_to_I2_t(I2_t *dst, const mv *src);
/** Copies a vector to a mv */
void vector_to_mv(mv *dst, const vector *src);
/** Copies a rotor to a mv */
void rotor_to_mv(mv *dst, const rotor *src);
/** Copies a e1_t to a mv */
void e1_t_to_mv(mv *dst, const e1_t *src);
/** Copies a e2_t to a mv */
void e2_t_to_mv(mv *dst, const e2_t *src);
/** Copies a I2_t to a mv */
void I2_t_to_mv(mv *dst, const I2_t *src);
/** Allocates memory to store coordinate group 0 */
void mv_reserveGroup_0(mv *A);
/** Allocates memory to store coordinate group 1 */
void mv_reserveGroup_1(mv *A);
/** Allocates memory to store coordinate group 2 */
void mv_reserveGroup_2(mv *A);
/** Returns the 1 coordinate of 'A' */
double mv_scalar(const mv *A);
/** Returns the 1 coordinate of 'A' */
double mv_double(const mv *A);
/** Sets the 1 coordinate of 'A' */
void mv_set_scalar(mv *A, double scalar_coord);
/** Returns the e1 coordinate of 'A' */
double mv_e1(const mv *A);
/** Sets the e1 coordinate of 'A' */
void mv_set_e1(mv *A, double e1_coord);
/** Returns the e2 coordinate of 'A' */
double mv_e2(const mv *A);
/** Sets the e2 coordinate of 'A' */
void mv_set_e2(mv *A, double e2_coord);
/** Returns the e1^e2 coordinate of 'A' */
double mv_e1_e2(const mv *A);
/** Sets the e1^e2 coordinate of 'A' */
void mv_set_e1_e2(mv *A, double e1_e2_coord);

/// Returns absolute largest coordinate in mv.
double mv_largestCoordinate(const mv *x);
/// Returns absolute largest coordinate in mv, and the bitmap of the corresponding basis blade (in 'bm').
double mv_largestBasisBlade(const mv *x, unsigned int *bm);

/** Sets vector to zero */
void vector_setZero(vector *_dst);
/** Sets rotor to zero */
void rotor_setZero(rotor *_dst);

/** Sets rotor to a scalar value */
void rotor_setScalar(rotor *_dst, const double scalarVal);

/** Sets vector to specified coordinates */
void vector_set(vector *_dst, const double _e1, const double _e2);
/** Sets rotor to specified coordinates */
void rotor_set(rotor *_dst, const double _scalar, const double _e1_e2);

/** Sets vector to specified coordinates */
void vector_setArray(vector *_dst, const double *A);
/** Sets rotor to specified coordinates */
void rotor_setArray(rotor *_dst, const double *A);

/** Copies vector: a = _dst */
void vector_copy(vector *_dst, const vector *a);
/** Copies rotor: a = _dst */
void rotor_copy(rotor *_dst, const rotor *a);


/** Returns abs largest coordinate of vector */
double vector_largestCoordinate(const vector *x);
/** Returns abs largest coordinate of rotor */
double rotor_largestCoordinate(const rotor *x);
/** Returns abs largest coordinate of e1_t */
double e1_t_largestCoordinate(const e1_t *x);
/** Returns abs largest coordinate of e2_t */
double e2_t_largestCoordinate(const e2_t *x);
/** Returns abs largest coordinate of I2_t */
double I2_t_largestCoordinate(const I2_t *x);

/** Returns scalar part of  vector */
double vector_double(const vector *x);
/** Returns scalar part of  rotor */
double rotor_double(const rotor *x);
/** Returns scalar part of  e1_t */
double e1_t_double(const e1_t *x);
/** Returns scalar part of  e2_t */
double e2_t_double(const e2_t *x);
/** Returns scalar part of  I2_t */
double I2_t_double(const I2_t *x);


/** Returns mv + mv. */
void add_mv_mv(mv *_dst, const mv *a, const mv *b);
/** Returns vector + vector. */
void add_vector_vector(vector *_dst, const vector *a, const vector *b);
/** Returns geometric product of mv and mv. */
void gp_mv_mv(mv *_dst, const mv *a, const mv *b);
/** Returns geometric product of vector and vector. */
void gp_vector_vector(rotor *_dst, const vector *a, const vector *b);
/** Returns a * b * reverse(a) using default metric. Only gives the correct result when the versor has a positive squared norm.
 */
void applyUnitVersor_rotor_vector(vector *_dst, const rotor *a, const vector *b);
/** Returns grade groupBitmap of  mv. */
void extractGrade_mv(mv *_dst, const mv *a, int groupBitmap);
/** Returns reverse of mv. */
void reverse_mv(mv *_dst, const mv *a);
// inline def SB:


/** structure used by custom parser */
struct e2gaParseMultivectorData {
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
int parse_mvEx(struct e2gaParseMultivectorData *data, const char *str, const char *strSourceName);
#endif /* _E2GA_H_ */
