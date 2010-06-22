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
#include "mex_if.h"

static int calcSumSquaredGradeSizes();

const char *const cArrayName = "c";
const char *const tFieldName = "t";
const char *const gFieldName = "g";

const int MX_C_FIELD = 0;
const int MX_T_FIELD = 1;
const int MX_G_FIELD = 2;


// the 'type name' of the mv type
const int TYPE_ID_MV = 0;
// the 'type name' of the mv type
const int TYPE_ID_VECTOR = 1;

const char* cClassNameMV = "MV";
const char* cClassNameOM = "OM";

/*
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
*/


mxArray* createMxArrayFromGA(const mv *X) 
{
	int nbCoords = e2ga_mvSize[X->gu];
	
	// init the coordinate array
	mxArray *doubleArray = mxCreateDoubleMatrix(nbCoords, 1, mxREAL);
	double *content = mxGetPr(doubleArray);
	memcpy(content, X->c, sizeof(double) * nbCoords);

	// init the type array
	mxArray *typeArray = mxCreateDoubleMatrix(1, 1, mxREAL);
	mxGetPr(typeArray)[0] = TYPE_ID_MV;

	// init the type array
	mxArray *groupUsageArray = mxCreateDoubleMatrix(1, 1, mxREAL);
	mxGetPr(groupUsageArray)[0] = X->gu;

	const char **fieldNames = malloc(3 * sizeof(char*));
	fieldNames[0] = cArrayName;
	fieldNames[1] = tFieldName;
	fieldNames[2] = gFieldName;
	
	
	// init the struct
 	mxArray *structArray = mxCreateStructMatrix(1, 1, 3, fieldNames);
	mxSetFieldByNumber(structArray, 0, MX_C_FIELD, doubleArray);
	mxSetFieldByNumber(structArray, 1, MX_T_FIELD, typeArray);
	mxSetFieldByNumber(structArray, 2, MX_G_FIELD, groupUsageArray);
	
	free(fieldNames);

	// this makes it into a GA class (MUST CALL)
 	mxArray* result;
	mexCallMATLAB(1, &result, 1, &structArray, cClassNameMV);

	return result;
}

/*
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
	
	    // coordinates takes a gradeflag, but returns only one grade.
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
*/

void createGAFromMxArray(const mxArray* array, mv *result)
{
	if (isGA(array))
	{
		const mxArray *coordinateField = mxGetField(array, 0, cArrayName);	
		double* coordinateArray = mxGetPr(coordinateField);
		
		const mxArray *typeField = mxGetField(array, 0, tFieldName);	
		double* typeArray = mxGetPr(typeField);
	
		const mxArray *groupUsageField = mxGetField(array, 0, gFieldName);	
		double* groupUsageArray = mxGetPr(groupUsageField);
	
		mv_setArray(result, (int)groupUsageArray[0], coordinateArray);
		
		return;
	}
	else if (mxIsDouble(array)) 
	{
		mv_setScalar(result, mxGetScalar(array));
		return;
	}
	else
	{
		char str[100];
		sprintf(str, "class '%s' is not GA or a double.", mxGetClassName(array));
		mexErrMsgTxt(str);
		return;
	}
}
/*
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
*/
// platform compatibility Wrapper around isClass for GA
int isGA(const mxArray* array)
{
	// mexPrintf("%s == %s: %d\n", mxGetClassName(array), cClassNameGA, mxIsClass(array, cClassNameGA));
	return mxIsClass(array, cClassNameMV);
}
/*
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
*/
