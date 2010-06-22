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

#include "mex_if.h"

// no input argument, one output argument
mxArray* calc(void);


void mexFunction(int nlhs, mxArray *plhs[], 
				 int nrhs, const mxArray *prhs[])
{	    
    /* Check for proper number of arguments */
	
	if (nrhs != 0) 
	{
		mexErrMsgTxt("The function takes no arguments."); 
	}
	else if (nlhs > 1) 
	{
		mexErrMsgTxt("Too many output arguments, one required."); 
	}
    
	plhs[0] = calc();

    return;    
}

