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

// File:         $RCSfile: mexFunction21.cpp,v $
// Part Of:      GAMatlab
// Revision:     $Revision: 1.6 $
// Last Edited:  $Date: 2003/12/15 02:59:18 $, $Author: martina $
// Author:       
// Copyright:    (c) 2003 Industrial Research Limited
//

#include "mex_if.h"

// two input arguments, one output argument
mxArray* calc(const mv* first, const mv* second);


void mexFunction(int nlhs, mxArray *plhs[], 
				 int nrhs, const mxArray *prhs[])
{	    
    /* Check for proper number of arguments */
	
	if (nrhs != 2) 
	{
		mexErrMsgTxt("The function takes two arguments."); 
	} 
	else if (nlhs > 1) 
	{
		mexErrMsgTxt("Too many output arguments, one required."); 
	}

	if (!isDoubleOrGA(prhs[0])) 
	{
		mexErrMsgTxt("First argument must be a GA object or double");
	}
    
	if (!isDoubleOrGA(prhs[1])) 
	{
		mexErrMsgTxt("Second argument must be a GA object or double");
	}

	mv in0, in1;
	createGAFromMxArray(prhs[0], in0);
	createGAFromMxArray(prhs[1], in1);
	plhs[0] = calc(in0, in1);

   return;    
}
