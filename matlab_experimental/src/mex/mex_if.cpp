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

// File:         $RCSfile: functions.cpp,v $
// Part Of:      GAMatlab
// Revision:     $Revision: 1.16 $
// Last Edited:  $Date: 2003/12/16 01:45:15 $, $Author: thomasl $
// Author:       
// Copyright:    (c) 2003 Industrial Research Limited
//

#include <string.h>
#include "mex_if.hpp"

static int calcSumSquaredGradeSizes();

const char* cArrayName = "m";

#ifdef CYGWIN
const char* cClassNameGA = "ga";
const char* cClassNameOM = "outermorphism";
#else
const char* cClassNameGA = "GA";
const char* cClassNameOM = "Outermorphism";
#endif

const int   cSumSquaredGradeSizes = calcSumSquaredGradeSizes();
const float cScaleForRandomValues = 1.0f;
const int   cGexpIterations = 50;

static int calcSumSquaredGradeSizes()
{
	int sum = 0;
	for (int i = 0; i < ga_ns::ga::dim + 1; ++i)
	{
		sum += ga_ns::gai_gradeSize[i]*ga_ns::gai_gradeSize[i];
	}
	return sum;
}


mxArray* createMxArrayFromGA(const ga_ns::ga& mv) 
{
	mxArray *doubleArray = mxCreateDoubleMatrix(ga_ns::gai::nbCoor, 1, mxREAL);
	double *content = mxGetPr(doubleArray);
	
	for (int grade=0; grade<=ga_ns::gai::dim; ++grade) 
	{
		int gradeFlag = 1 << grade;
		
		/* coordinates takes a gradeflag, but returns only one grade. */
		const double *coordinates = mv.coordinates(gradeFlag);
		
		for (int element=0; element<ga_ns::gai_gradeSize[grade]; ++element)
		{
			*(content++) = coordinates[element];
		}
	}

	mxArray *structArray = mxCreateStructMatrix(1, 1, 1, &cArrayName);
	mxSetFieldByNumber(structArray, 0, 0, doubleArray);

 	mxArray* result;
	mexCallMATLAB(1, &result, 1, &structArray, cClassNameGA);

	return result;
}

#if 0
// mv_array what do we want? 
//   std containers, boost array, ublas  + c array  double[]
// anything a[k], boost::size(
template <class Vec_type>
mxArray* createMxArrayFromGaArray(const Vec_type& mv_array ) 
{
    // check  Vec_type::value_type == ga
    size_t numElements = mv_array.size();

}
#endif

mxArray* createMxArrayFromGaArray(const std::vector<ga_ns::ga>& ga_vec)
{
     size_t num_ga = ga_vec.size();
     mxArray *structArray = mxCreateStructMatrix(num_ga, 1, 1, &cArrayName);

     for (size_t row=0; row!=num_ga; ++row)
     {
	mxArray *doubleArray = mxCreateDoubleMatrix(ga_ns::gai::nbCoor, 1, mxREAL);
	double *content = mxGetPr(doubleArray);

	for (int grade=0; grade<=ga_ns::gai::dim; ++grade)
	{
	    int gradeFlag = 1 << grade;
	
	    /* coordinates takes a gradeflag, but returns only one grade. */
	    const double *coordinates = ga_vec[row].coordinates(gradeFlag);
	
	    for (int element=0; element<ga_ns::gai_gradeSize[grade]; ++element)
	    {
		*(content++) = coordinates[element];
	    }
	}
	mxSetFieldByNumber(structArray, row, 0, doubleArray);
	
     }

     mxArray* result;
     mexCallMATLAB(1, &result, 1, &structArray, cClassNameGA);
     return result;
}

ga_ns::ga* createGAFromMxArray(const mxArray* array)
{
	if (isGA(array))
	{
		const mxArray *fieldArray1 = mxGetField(array, 0, cArrayName);
	
		double* numArray = mxGetPr(fieldArray1);
		
		ga_ns::ga* retVal = new ga_ns::ga((1 << (ga_ns::gai::dim + 1)) - 1, numArray);
		// adjust internal gradeUsage array so that function work correctly
		retVal->compress(0.0f);
		return retVal;
	}
	else if (mxIsDouble(array)) 
	{
		return new ga_ns::ga(mxGetScalar(array));
	}
	else
	{
		char str[100];
		sprintf(str, "class '%s' is not GA or a double.", mxGetClassName(array));
		mexErrMsgTxt(str);
	}
	return 0;
}

ga_ns::ga* createGaArrayFromMxArray(const mxArray* array, int& numElements)
{
    double* numArray = 0;
    mxArray* fieldArray = 0;
    numElements = mxGetNumberOfElements(array);
    ga_ns::ga *retVal = new ga_ns::ga[numElements];
    ga_ns::ga tmp;
   
    for (int i=0; i<numElements; i++)
    {
        fieldArray = mxGetField(array, i, "m");
        numArray = mxGetPr(fieldArray);
        tmp = ga_ns::ga((1 << (ga_ns::gai::dim + 1)) - 1, numArray);
        tmp.compress(0.0f);
        retVal[i] = tmp;
    }

    return retVal;
}

bool isDoubleOrGA(const mxArray* array) 
{
	return isGA(array) 
		|| (mxIsDouble(array) && mxGetM(array) == 1 && mxGetN(array) == 1);
}

// platform compatibility Wrapper around isClass for GA
bool isGA(const mxArray* array)
{
	// mexPrintf("%s == %s: %d\n", mxGetClassName(array), cClassNameGA, mxIsClass(array, cClassNameGA));
	return mxIsClass(array, cClassNameGA);
}

// platform compatibility Wrapper around isClass for Outermorphism
bool isOM(const mxArray* array)
{
	return mxIsClass(array, cClassNameOM);
}

ga_ns::ga* getVectorImages(double* data) 
{
	ga_ns::ga *vectorImages = new ga_ns::ga[ga_ns::ga::dim];
	for (int i = 0; i < ga_ns::ga::dim; ++i)
	{
		for (int j = 0; j < ga_ns::ga::dim; ++j)
		{
			vectorImages[i] = vectorImages[i] + data[i*ga_ns::ga::dim + j]*(*ga_ns::ga::bv[j]);
		}
	}
	return vectorImages;
}

ga_ns::ga_om* createOMFromMxArray(const mxArray* array) 
{
	if (isOM(array)) 
	{
		const mxArray *fieldArray1 = mxGetField(array, 0, cArrayName);

		double *numArray = mxGetPr(fieldArray1);

		ga_ns::ga_om *retVal = new ga_ns::ga_om;
		memcpy(retVal->c, numArray, sizeof(double)*cSumSquaredGradeSizes);

		return retVal;
	} else {
		mexErrMsgTxt("no valid argument supplied");
	}
	return 0;
}
