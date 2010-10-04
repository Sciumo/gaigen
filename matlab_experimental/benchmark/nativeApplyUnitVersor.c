#include "mex.h"
void mexFunction(int nlhs, mxArray *plhs[],
	int nrhs, const mxArray *prhs[]) {

	double *output;
	const double *rbm;
	const double *pt;

	plhs[0] = mxCreateDoubleMatrix(1,4,mxREAL);
	output = mxGetPr(plhs[0]);


	rbm = mxGetPr(prhs[0]);
	pt = mxGetPr(prhs[1]);

	output[0] = -rbm[1]*rbm[1]*pt[0]+2*rbm[1]*rbm[2]*pt[2]+-2*rbm[1]*rbm[5]+2*rbm[1]*rbm[0]*pt[1]+-2*rbm[7]*rbm[2]+-2*rbm[4]*rbm[0]+rbm[2]*rbm[2]*pt[0]+2*rbm[2]*rbm[3]*pt[1]-rbm[3]*rbm[3]*pt[0]+2*rbm[3]*rbm[6]+-2*rbm[3]*rbm[0]*pt[2]+rbm[0]*rbm[0]*pt[0];
	output[1] = -rbm[1]*rbm[1]*pt[1]+2*rbm[1]*rbm[4]+2*rbm[1]*rbm[3]*pt[2]+-2*rbm[1]*rbm[0]*pt[0]+-2*rbm[7]*rbm[3]-rbm[2]*rbm[2]*pt[1]+2*rbm[2]*rbm[3]*pt[0]+-2*rbm[2]*rbm[6]+2*rbm[2]*rbm[0]*pt[2]+-2*rbm[5]*rbm[0]+rbm[3]*rbm[3]*pt[1]+rbm[0]*rbm[0]*pt[1];
	output[2] = rbm[1]*rbm[1]*pt[2]+-2*rbm[1]*rbm[7]+2*rbm[1]*rbm[2]*pt[0]+2*rbm[1]*rbm[3]*pt[1]+-2*rbm[4]*rbm[3]-rbm[2]*rbm[2]*pt[2]+2*rbm[2]*rbm[5]+-2*rbm[2]*rbm[0]*pt[1]-rbm[3]*rbm[3]*pt[2]+2*rbm[3]*rbm[0]*pt[0]+-2*rbm[6]*rbm[0]+rbm[0]*rbm[0]*pt[2];
	output[3] = rbm[1]*rbm[1]*pt[3]+-2*rbm[1]*rbm[7]*pt[2]+-2*rbm[1]*rbm[4]*pt[1]+2*rbm[1]*rbm[5]*pt[0]+2*rbm[7]*rbm[7]+-2*rbm[7]*rbm[2]*pt[0]+-2*rbm[7]*rbm[3]*pt[1]+2*rbm[4]*rbm[4]+2*rbm[4]*rbm[3]*pt[2]+-2*rbm[4]*rbm[0]*pt[0]+rbm[2]*rbm[2]*pt[3]+-2*rbm[2]*rbm[5]*pt[2]+2*rbm[2]*rbm[6]*pt[1]+2*rbm[5]*rbm[5]+-2*rbm[5]*rbm[0]*pt[1]+rbm[3]*rbm[3]*pt[3]+-2*rbm[3]*rbm[6]*pt[0]+2*rbm[6]*rbm[6]+-2*rbm[6]*rbm[0]*pt[2]+rbm[0]*rbm[0]*pt[3];
}