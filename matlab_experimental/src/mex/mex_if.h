#ifndef GAMATLAB_MEX_INTERFACE_FUNCTIONS_HPP_INCLUDED
#define GAMATLAB_MEX_INTERFACE_FUNCTIONS_HPP_INCLUDED

/* This file is part of GAMatlab.
Copyright (C) 2003  Industrial Research Limited

GAMatlab is free software; you can redistribute it and/or modify it
under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation; either version 2.1 of the License, or (at
your option) any later version.

GAMatlab is distributed in the hope that it will be useful, but WITHOUT
ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
License for more details.

You should have received a copy of the GNU Lesser General Public License
along with this library; if not, write to the Free Software Foundation,
Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA */


#include <mex.h>
#include "../g25/e2ga.h"

/** The name of the internal array of the Matlab objects */
extern const char* const cArrayName;

/** The name of the GA Matlab class */
extern const char* cClassNameMV;

/** The name of the Outermorphism Matlab class */
extern const char* cClassNameOM;

#ifdef RIEN
/** The sum of the squared grade sizes, needed by OM */
extern const int   cSumSquaredGradeSizes;

/** The scaling factor for random values */
extern const float cScaleForRandomValues;

/** The number of iterations for the gexp algorithm */
extern const int   cGexpIterations;
#endif

/** Convert Matlab GA object to Gaigen object.
 * Centralised function to convert Matlab GA object to
 * C++ GA object.
 */
void createGAFromMxArray(const mxArray* array, mv *result);

/** Convert Matlab array of GA objecta to array of Gaigen objects.
 * Returns a pointer to an array of ga objects.
 * Caller responsible for deletion of array (delete []) after use.
 */
/*ga_ns::ga* createGaArrayFromMxArray(const mxArray* array, int& numElements);*/

/** Convert Gaigen object to Matlab GA object.
 * Centralised function to convert C++ GA object to
 * Matlab GA object.
 */
mxArray* createMxArrayFromGA(const mv *X);

/** Convert vector of Gaigen objects to Matlab array of GA objects.
 * Centralised function to convert C++ GA objects to
 * Matlab GA objects.
 */
/*mxArray* createMxArrayFromGaArray(const std::vector<ga_ns::ga>& mv_vec);*/

/** Convert Matlab OM object to Gaigen object.
 * Centralised function to convert Matlab Outermorphism object
 * to C++ GA_OM object
 */
/*ga_ns::ga_om* createOMFromMxArray(const mxArray* array);*/

/** Check if parameter is of class GA. */
int isGA(const mxArray* array);

/** Check if parameter is either of class GA or double. */
/*bool isDoubleOrGA(const mxArray* array);*/

/** Return vector images for OM from double array. */
/*ga_ns::ga* getVectorImages(double* data);*/

#endif

