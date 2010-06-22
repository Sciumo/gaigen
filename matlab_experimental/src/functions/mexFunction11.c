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

// File:         $RCSfile: mexFunction11.cpp,v $
// Part Of:      GAMatlab
// Revision:     $Revision: 1.7 $
// Last Edited:  $Date: 2003/12/15 02:59:18 $, $Author: martina $
// Author:       
// Copyright:    (c) 2003 Industrial Research Limited
//

#include "mex_if.hpp"

// one input argument, one output argument
mxArray* calc(ga_ns::ga* arg);


void mexFunction(int nlhs, mxArray *plhs[], 
				 int nrhs, const mxArray *prhs[])
{	    
    /* Check for proper number of arguments */
	
	if (nrhs != 1) 
	{
		mexErrMsgTxt("The function takes one arguments."); 
	}
	else if (nlhs > 1) 
	{
		mexErrMsgTxt("Too many output arguments, one required."); 
	}

	if (!isDoubleOrGA(prhs[0])) 
	{
		mexErrMsgTxt("First argument must be a GA object or double");
	}
	ga_ns::ga* in = createGAFromMxArray(prhs[0]);
	plhs[0] = calc(in);
	delete in;

    return;
}
