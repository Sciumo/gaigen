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
using System;
namespace c3ga_ns {

[FlagsAttribute]
public enum GroupBitmap : int
{
	GROUP_0  = 1, //1
	GROUP_1  = 2, //no, e1, e2, e3, ni
	GROUP_2  = 4, //no^e1, no^e2, e1^e2, no^e3, e1^e3, e2^e3, no^ni, e1^ni, e2^ni, e3^ni
	GROUP_3  = 8, //no^e1^e2, no^e1^e3, no^e2^e3, e1^e2^e3, no^e1^ni, no^e2^ni, e1^e2^ni, no^e3^ni, e1^e3^ni, e2^e3^ni
	GROUP_4  = 16, //no^e1^e2^e3, no^e1^e2^ni, no^e1^e3^ni, no^e2^e3^ni, e1^e2^e3^ni
	GROUP_5  = 32, //no^e1^e2^e3^ni

	ALL_GROUPS = 63, // all groups

	GRADE_0 = 1, 
	GRADE_1 = 2, 
	GRADE_2 = 4, 
	GRADE_3 = 8, 
	GRADE_4 = 16, 
	GRADE_5 = 32, 

	ALL_GRADES = 63 // all grades
}

/// <summary>
/// These constants define a unique number for each specialized multivector type.
/// They are used to report usage of non-optimized functions.
/// </summary>
public enum SmvType {
	C3GA_NONE = -1,
	C3GA_MV = 0,
	C3GA_FLOAT = 1,
	C3GA_NORMALIZEDPOINT = 2,
	C3GA_NO_T = 3,
	C3GA_E1_T = 4,
	C3GA_E2_T = 5,
	C3GA_E3_T = 6,
	C3GA_NI_T = 7,
	C3GA_INVALID
}

public class c3ga 
{ 
	public static readonly no_t no = new no_t();
	public static readonly e1_t e1 = new e1_t();
	public static readonly e2_t e2 = new e2_t();
	public static readonly e3_t e3 = new e3_t();
	public static readonly ni_t ni = new ni_t();


	static protected internal string[] typenames = 
		new string[] {
			"mv",
			"float",
			"normalizedPoint",
			"no_t",
			"e1_t",
			"e2_t",
			"e3_t",
			"ni_t"
		};

	/// <summary>The dimension of the space
	/// </summary>
	public const int SpaceDim = 5;
	/// <summary>Number of groups/grades of coordinates in a multivector
	/// </summary>
	public const int NbGroups = 6;
	/// <summary>Is the metric of the space Euclidean? (false or true)
	/// </summary>
	public const bool MetricEuclidean = false;
	/// <summary>Names of the basis vectors.
	/// </summary>
	public static readonly string[] BasisVectorNames = new string[] {
		"no", "e1", "e2", "e3", "ni"
	};
	/// <summary>The constants for the grades, in an array.
	/// </summary>
	public static readonly GroupBitmap[] Grades = {GroupBitmap.GRADE_0, GroupBitmap.GRADE_1, GroupBitmap.GRADE_2, GroupBitmap.GRADE_3, GroupBitmap.GRADE_4, GroupBitmap.GRADE_5, 0, 0, 0, 0, 0, 0};
	/// <summary>The constants for the groups, in an array.
	/// </summary>
	public static readonly GroupBitmap[] Groups = {GroupBitmap.GROUP_0, GroupBitmap.GROUP_1, GroupBitmap.GROUP_2, GroupBitmap.GROUP_3, GroupBitmap.GROUP_4, GroupBitmap.GROUP_5};
	/// <summary>This array can be used to lookup the number of coordinates for a group part of a general multivector.
	/// </summary>
	public static readonly int[] GroupSize = { 1, 5, 10, 10, 5, 1 };
	/// <summary>This array can be used to lookup the number of coordinates based on a group usage bitmap.
	/// </summary>
	public static readonly int[] MvSize = new int[] {
		0, 1, 5, 6, 10, 11, 15, 16, 10, 11, 15, 16, 20, 21, 25, 26, 5, 6, 10, 11, 
		15, 16, 20, 21, 15, 16, 20, 21, 25, 26, 30, 31, 1, 2, 6, 7, 11, 12, 16, 17, 
		11, 12, 16, 17, 21, 22, 26, 27, 6, 7, 11, 12, 16, 17, 21, 22, 16, 17, 21, 22, 
		26, 27, 31, 32	};
	/// <summary>This array of integers contains the 'sign' (even/odd permutation of canonical order) of basis elements in the general multivector.
	/// Use it to answer 'what is the permutation of the coordinate at index [x]'?
	/// </summary>
	public static readonly double[] BasisElementSignByIndex = new double[]
		{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
	/// <summary>This array of integers contains the 'sign' (even/odd permutation of canonical order) of basis elements in the general multivector.
	/// Use it to answer 'what is the permutation of the coordinate of bitmap [x]'?
	/// </summary>
	public static readonly double[] BasisElementSignByBitmap = new double[]
		{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
	/// <summary>This array of integers contains the order of basis elements in the general multivector.
	/// Use it to answer: 'at what index do I find basis element [x] (x = basis vector bitmap)?'
	/// </summary>
	public static readonly int[] BasisElementIndexByBitmap = new int[]
		{0, 1, 2, 6, 3, 7, 8, 16, 4, 9, 10, 17, 11, 18, 19, 26, 5, 12, 13, 20, 14, 21, 22, 27, 15, 23, 24, 28, 25, 29, 30, 31};
	/// <summary>This array of integers contains the indices of basis elements in the general multivector.
	/// Use it to answer: 'what basis element do I find at index [x]'?
	/// </summary>
	public static readonly int[] BasisElementBitmapByIndex = new int[]
		{0, 1, 2, 4, 8, 16, 3, 5, 6, 9, 10, 12, 17, 18, 20, 24, 7, 11, 13, 14, 19, 21, 22, 25, 26, 28, 15, 23, 27, 29, 30, 31};
	/// <summary>This array of grade of each basis elements in the general multivector.
	/// Use it to answer: 'what is the grade of basis element bitmap [x]'?
	/// </summary>
	public static readonly int[] BasisElementGradeByBitmap = new int[]
		{0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4, 1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5};
	/// <summary>This array of group of each basis elements in the general multivector.
	/// Use it to answer: 'what is the group of basis element bitmap [x]'?
	/// </summary>
	public static readonly int[] BasisElementGroupByBitmap = new int[]
		{0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4, 1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5};
	/// <summary>This array of integers contains the order of basis elements in the general multivector.
	/// Use it to answer: 'what basis vectors are in the basis element at position [x]?
	/// </summary>
	public static readonly int[][] BasisElements = new int[][] {
		new int[] {-1},
		new int[] {0, -1},
		new int[] {1, -1},
		new int[] {2, -1},
		new int[] {3, -1},
		new int[] {4, -1},
		new int[] {0, 1, -1},
		new int[] {0, 2, -1},
		new int[] {1, 2, -1},
		new int[] {0, 3, -1},
		new int[] {1, 3, -1},
		new int[] {2, 3, -1},
		new int[] {0, 4, -1},
		new int[] {1, 4, -1},
		new int[] {2, 4, -1},
		new int[] {3, 4, -1},
		new int[] {0, 1, 2, -1},
		new int[] {0, 1, 3, -1},
		new int[] {0, 2, 3, -1},
		new int[] {1, 2, 3, -1},
		new int[] {0, 1, 4, -1},
		new int[] {0, 2, 4, -1},
		new int[] {1, 2, 4, -1},
		new int[] {0, 3, 4, -1},
		new int[] {1, 3, 4, -1},
		new int[] {2, 3, 4, -1},
		new int[] {0, 1, 2, 3, -1},
		new int[] {0, 1, 2, 4, -1},
		new int[] {0, 1, 3, 4, -1},
		new int[] {0, 2, 3, 4, -1},
		new int[] {1, 2, 3, 4, -1},
		new int[] {0, 1, 2, 3, 4, -1}
	};

    // Simple code for multi-threading-safe random number generation (the Random class is not thread-safe).
    // The worst that can happen is that multiple threads get the same random number.
    protected internal static System.Random s_randomGenerator = new System.Random();
    protected internal const int NB_RANDOM_DOUBLES = 32; // must be power of two
    protected internal static double[] s_randomDoubles = new double[NB_RANDOM_DOUBLES];
    protected internal static int s_randomDoublesIdx = NB_RANDOM_DOUBLES;
    protected internal static double NextRandomDouble() {
        if (s_randomDoublesIdx >= s_randomDoubles.Length) {
            lock(s_randomDoubles) {
                for (int i = 0; i < s_randomDoubles.Length; i++) {
                    s_randomDoubles[i] = s_randomGenerator.NextDouble();
                }
                s_randomDoublesIdx = 0;
            }
        }
        int idx = s_randomDoublesIdx & (NB_RANDOM_DOUBLES-1);
        s_randomDoublesIdx++;
        return s_randomDoubles[idx];
	}

    /// <summary>Sets 1 floats to zero.</summary>
	protected internal static void Zero_1(float[] dst) {
		dst[0]=0.0f;
	}
	/// <summary>Copies 1 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_1(float[] dst, float[] src) {
			dst[0] = src[0];
	}
    /// <summary>Sets 2 floats to zero.</summary>
	protected internal static void Zero_2(float[] dst) {
		dst[0]=dst[1]=0.0f;
	}
	/// <summary>Copies 2 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_2(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
	}
    /// <summary>Sets 3 floats to zero.</summary>
	protected internal static void Zero_3(float[] dst) {
		dst[0]=dst[1]=dst[2]=0.0f;
	}
	/// <summary>Copies 3 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_3(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
			dst[2] = src[2];
	}
    /// <summary>Sets 4 floats to zero.</summary>
	protected internal static void Zero_4(float[] dst) {
		dst[0]=dst[1]=dst[2]=dst[3]=0.0f;
	}
	/// <summary>Copies 4 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_4(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
			dst[2] = src[2];
			dst[3] = src[3];
	}
    /// <summary>Sets 5 floats to zero.</summary>
	protected internal static void Zero_5(float[] dst) {
		dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=0.0f;
	}
	/// <summary>Copies 5 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_5(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
			dst[2] = src[2];
			dst[3] = src[3];
			dst[4] = src[4];
	}
    /// <summary>Sets 6 floats to zero.</summary>
	protected internal static void Zero_6(float[] dst) {
		dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=0.0f;
	}
	/// <summary>Copies 6 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_6(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
			dst[2] = src[2];
			dst[3] = src[3];
			dst[4] = src[4];
			dst[5] = src[5];
	}
    /// <summary>Sets 7 floats to zero.</summary>
	protected internal static void Zero_7(float[] dst) {
		dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=0.0f;
	}
	/// <summary>Copies 7 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_7(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
			dst[2] = src[2];
			dst[3] = src[3];
			dst[4] = src[4];
			dst[5] = src[5];
			dst[6] = src[6];
	}
    /// <summary>Sets 8 floats to zero.</summary>
	protected internal static void Zero_8(float[] dst) {
		dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=0.0f;
	}
	/// <summary>Copies 8 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_8(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
			dst[2] = src[2];
			dst[3] = src[3];
			dst[4] = src[4];
			dst[5] = src[5];
			dst[6] = src[6];
			dst[7] = src[7];
	}
    /// <summary>Sets 9 floats to zero.</summary>
	protected internal static void Zero_9(float[] dst) {
		dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=0.0f;
	}
	/// <summary>Copies 9 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_9(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
			dst[2] = src[2];
			dst[3] = src[3];
			dst[4] = src[4];
			dst[5] = src[5];
			dst[6] = src[6];
			dst[7] = src[7];
			dst[8] = src[8];
	}
    /// <summary>Sets 10 floats to zero.</summary>
	protected internal static void Zero_10(float[] dst) {
		dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=0.0f;
	}
	/// <summary>Copies 10 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_10(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
			dst[2] = src[2];
			dst[3] = src[3];
			dst[4] = src[4];
			dst[5] = src[5];
			dst[6] = src[6];
			dst[7] = src[7];
			dst[8] = src[8];
			dst[9] = src[9];
	}
    /// <summary>Sets 11 floats to zero.</summary>
	protected internal static void Zero_11(float[] dst) {
		dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=0.0f;
	}
	/// <summary>Copies 11 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_11(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
			dst[2] = src[2];
			dst[3] = src[3];
			dst[4] = src[4];
			dst[5] = src[5];
			dst[6] = src[6];
			dst[7] = src[7];
			dst[8] = src[8];
			dst[9] = src[9];
			dst[10] = src[10];
	}
    /// <summary>Sets 12 floats to zero.</summary>
	protected internal static void Zero_12(float[] dst) {
		dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=0.0f;
	}
	/// <summary>Copies 12 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_12(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
			dst[2] = src[2];
			dst[3] = src[3];
			dst[4] = src[4];
			dst[5] = src[5];
			dst[6] = src[6];
			dst[7] = src[7];
			dst[8] = src[8];
			dst[9] = src[9];
			dst[10] = src[10];
			dst[11] = src[11];
	}
    /// <summary>Sets 13 floats to zero.</summary>
	protected internal static void Zero_13(float[] dst) {
		dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=dst[12]=0.0f;
	}
	/// <summary>Copies 13 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_13(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
			dst[2] = src[2];
			dst[3] = src[3];
			dst[4] = src[4];
			dst[5] = src[5];
			dst[6] = src[6];
			dst[7] = src[7];
			dst[8] = src[8];
			dst[9] = src[9];
			dst[10] = src[10];
			dst[11] = src[11];
			dst[12] = src[12];
	}
    /// <summary>Sets 14 floats to zero.</summary>
	protected internal static void Zero_14(float[] dst) {
		dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=dst[12]=dst[13]=0.0f;
	}
	/// <summary>Copies 14 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_14(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
			dst[2] = src[2];
			dst[3] = src[3];
			dst[4] = src[4];
			dst[5] = src[5];
			dst[6] = src[6];
			dst[7] = src[7];
			dst[8] = src[8];
			dst[9] = src[9];
			dst[10] = src[10];
			dst[11] = src[11];
			dst[12] = src[12];
			dst[13] = src[13];
	}
    /// <summary>Sets 15 floats to zero.</summary>
	protected internal static void Zero_15(float[] dst) {
		dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=dst[12]=dst[13]=dst[14]=0.0f;
	}
	/// <summary>Copies 15 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_15(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
			dst[2] = src[2];
			dst[3] = src[3];
			dst[4] = src[4];
			dst[5] = src[5];
			dst[6] = src[6];
			dst[7] = src[7];
			dst[8] = src[8];
			dst[9] = src[9];
			dst[10] = src[10];
			dst[11] = src[11];
			dst[12] = src[12];
			dst[13] = src[13];
			dst[14] = src[14];
	}
    /// <summary>Sets 16 floats to zero.</summary>
	protected internal static void Zero_16(float[] dst) {
		dst[0]=dst[1]=dst[2]=dst[3]=dst[4]=dst[5]=dst[6]=dst[7]=dst[8]=dst[9]=dst[10]=dst[11]=dst[12]=dst[13]=dst[14]=dst[15]=0.0f;
	}
	/// <summary>Copies 16 floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_16(float[] dst, float[] src) {
			dst[0] = src[0];
			dst[1] = src[1];
			dst[2] = src[2];
			dst[3] = src[3];
			dst[4] = src[4];
			dst[5] = src[5];
			dst[6] = src[6];
			dst[7] = src[7];
			dst[8] = src[8];
			dst[9] = src[9];
			dst[10] = src[10];
			dst[11] = src[11];
			dst[12] = src[12];
			dst[13] = src[13];
			dst[14] = src[14];
			dst[15] = src[15];
	}
	/// <summary>Sets N floats to zero.</summary>
	protected internal static void Zero_N(float[] dst, int N) {
		for (int i = 0; i < N; i++)
			dst[i] = 0.0f;
	}
	/// <summary>Copies N floats from 'src' to 'dst'. </summary>
	protected internal static void Copy_N(float[] dst, float[] src, int N) {
		for (int i = 0; i < N; i++)
			dst[i] = src[i];
	}


	private const string DEFAULT_FP = "F2";
	private const string DEFAULT_START = "";
	private const string DEFAULT_END = "";
	private const string DEFAULT_MUL = "*";
	private const string DEFAULT_WEDGE = "^";
	private const string DEFAULT_PLUS = " + ";
	private const string DEFAULT_MINUS = " - ";

	// These strings determine how the output of string() is formatted.
	// You can alter them at runtime using setStringFormat(). 
	protected static string string_fp = DEFAULT_FP;
	protected static string string_start = DEFAULT_START;
	protected static string string_end = DEFAULT_END;
	protected static string string_mul = DEFAULT_MUL;
	protected static string string_wedge = DEFAULT_WEDGE;
	protected static string string_plus = DEFAULT_PLUS;
	protected static string string_minus = DEFAULT_MINUS;
	
	public const string STRING_FP = "fp";
	public const string STRING_START = "start";
	public const string STRING_END = "end";
	public const string STRING_MUL = "mul";
	public const string STRING_WEDGE = "wedge";
	public const string STRING_PLUS = "plus";
	public const string STRING_MINUS= "minus";

	/// <summary>
	/// Sets the formatting of toString().
	/// </summary>
	/// 
	/// <param name="what">What formatter to set. Valid values: string_FP, string_START, string_END, string_MUL, string_WEDGE, string_PLUS, string_MINUS.</param>
	/// <param name="format">The value for 'what'. Use 'null' to set the default value.</param>
	public static void SetStringFormat(string what, string format) {
		if (what.Equals(STRING_FP)) 
			string_fp = (format != null) ? format : DEFAULT_FP;
		else if (what.Equals(STRING_START)) 
			string_start = (format != null) ? format : DEFAULT_START;
		else if (what.Equals(STRING_END)) 
			string_end = (format != null) ? format : DEFAULT_END;
		else if (what.Equals(STRING_MUL)) 
			string_mul = (format != null) ? format : DEFAULT_MUL;
		else if (what.Equals(STRING_WEDGE)) 
			string_wedge = (format != null) ? format : DEFAULT_WEDGE;
		else if (what.Equals(STRING_PLUS)) 
			string_plus = (format != null) ? format : DEFAULT_PLUS;
		else if (what.Equals(STRING_MINUS)) 
			string_minus = (format != null) ? format : DEFAULT_MINUS;
		else throw new ArgumentException("invalid argument to setStringFormat(): " + what);
	}
	
    /// <summary>Converts a multivector to a string using default float format.</summary>
	public static string String(mv_if value) {
		return String(value, null);
	}
	
	/// <summary>
	/// Converts a multivector to a string according to a float format like "F".
	/// </summary>
	/// <param name="fp">floating point format. Use 'null' for the default format (see setStringFormat()).</param>
	public static string String(mv_if value, string fp) {
		mv obj = value.to_mv();
		System.Text.StringBuilder result = new System.Text.StringBuilder();
		int ia = 0; // global index into coordinates (runs from 0 to 31)
		int cnt = 0; // how many coordinates printed so far

		// set up the floating point precision
		if (fp == null) fp = string_fp;

		// start the string
		result.Append(string_start);

		// print all coordinates
		for (int g = 0; g < 6; g++) {
			float[] Cg = obj.m_c[g];
			if (Cg != null) {
				for (int b = 0; b < GroupSize[g]; b++) {
					double coord = (double)BasisElementSignByIndex[ia] * Cg[b];
					
					// goal: print [+|-]obj.m_c[k][* basisVector1 ^ ... ^ basisVectorN]
					
					string tmpFloatStr = Math.Abs(coord).ToString(fp);

					if (Double.Parse(tmpFloatStr) != 0.0) {
						// print [+|-]
						result.Append((coord >= 0.0) 
							? ((cnt>0) ? string_plus : "")
							: string_minus);
						// print obj.m_c[k]
						result.Append(tmpFloatStr);

						if (g != 0) { // if not grade 0, print [* basisVector1 ^ ... ^ basisVectorN]
							result.Append(string_mul);

							// print all basis vectors
							int bei = 0;
							while (BasisElements[ia][bei] >= 0) {
								if (bei > 0)
									result.Append(string_wedge);
								result.Append(BasisVectorNames[BasisElements[ia][bei]]);
								bei++;
							}
						}

						cnt++;
					}
					ia++;
				}
			}
			else ia += GroupSize[g];
		}

		// if no coordinates printed: 0
		if (cnt == 0) result.Append("0");

		// end the string
		result.Append(string_end);

		return result.ToString();
	}


	/// <summary>
    /// Simple way to call parser (regardless of whether it is a builtin or ANTLR parser).
    /// 
    /// Throws a ParseException on failure.
    /// 
    /// When an ANTLR based parser throws an exception, 
    /// all details (like line number and cause) are lost. 
    /// If these details are required, call the ANTLR parser directly.
    /// 
    /// </summary>
    /// <param name="str">The multivector string to be parsed (can be output of mv.ToString())</param>
    /// <returns>Multivector value represented by 'str'.</returns>
    public static mv Parse(string str)
    {
        return Parser.Parse(str, "string");
    }
	/// <summary>Computes the partial geometric product of two multivectors (group 0  x  group 0 -> group 0)
	/// </summary>
	protected internal static void gp_default_0_0_0(float[] A, float[] B, float[] C) {
		C[0] += A[0]*B[0];
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 0  x  group 1 -> group 1)
	/// </summary>
	protected internal static void gp_default_0_1_1(float[] A, float[] B, float[] C) {
		C[0] += A[0]*B[0];
		C[1] += A[0]*B[1];
		C[2] += A[0]*B[2];
		C[3] += A[0]*B[3];
		C[4] += A[0]*B[4];
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 0  x  group 2 -> group 2)
	/// </summary>
	protected internal static void gp_default_0_2_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 0  x  group 3 -> group 3)
	/// </summary>
	protected internal static void gp_default_0_3_3(float[] A, float[] B, float[] C) {
		gp_default_0_2_2(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 0  x  group 4 -> group 4)
	/// </summary>
	protected internal static void gp_default_0_4_4(float[] A, float[] B, float[] C) {
		gp_default_0_1_1(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 0  x  group 5 -> group 5)
	/// </summary>
	protected internal static void gp_default_0_5_5(float[] A, float[] B, float[] C) {
		gp_default_0_0_0(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 0 -> group 1)
	/// </summary>
	protected internal static void gp_default_1_0_1(float[] A, float[] B, float[] C) {
		C[0] += A[0]*B[0];
		C[1] += A[1]*B[0];
		C[2] += A[2]*B[0];
		C[3] += A[3]*B[0];
		C[4] += A[4]*B[0];
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 0)
	/// </summary>
	protected internal static void gp_default_1_1_0(float[] A, float[] B, float[] C) {
		C[0] += (-A[0]*B[4]+A[1]*B[1]+A[2]*B[2]+A[3]*B[3]-A[4]*B[0]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 2)
	/// </summary>
	protected internal static void gp_default_1_1_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 1)
	/// </summary>
	protected internal static void gp_default_1_2_1(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[6]-A[1]*B[0]-A[2]*B[1]-A[3]*B[3]);
		C[1] += (A[0]*B[7]-A[2]*B[2]-A[3]*B[4]-A[4]*B[0]);
		C[2] += (A[0]*B[8]+A[1]*B[2]-A[3]*B[5]-A[4]*B[1]);
		C[3] += (A[0]*B[9]+A[1]*B[4]+A[2]*B[5]-A[4]*B[3]);
		C[4] += (A[1]*B[7]+A[2]*B[8]+A[3]*B[9]-A[4]*B[6]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 3)
	/// </summary>
	protected internal static void gp_default_1_2_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 3 -> group 2)
	/// </summary>
	protected internal static void gp_default_1_3_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 3 -> group 4)
	/// </summary>
	protected internal static void gp_default_1_3_4(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[3]-A[1]*B[2]+A[2]*B[1]-A[3]*B[0]);
		C[1] += (A[0]*B[6]-A[1]*B[5]+A[2]*B[4]-A[4]*B[0]);
		C[2] += (A[0]*B[8]-A[1]*B[7]+A[3]*B[4]-A[4]*B[1]);
		C[3] += (A[0]*B[9]-A[2]*B[7]+A[3]*B[5]-A[4]*B[2]);
		C[4] += (A[1]*B[9]-A[2]*B[8]+A[3]*B[6]-A[4]*B[3]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 4 -> group 3)
	/// </summary>
	protected internal static void gp_default_1_4_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 4 -> group 5)
	/// </summary>
	protected internal static void gp_default_1_4_5(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[4]-A[1]*B[3]+A[2]*B[2]-A[3]*B[1]+A[4]*B[0]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 5 -> group 4)
	/// </summary>
	protected internal static void gp_default_1_5_4(float[] A, float[] B, float[] C) {
		C[0] += -A[0]*B[0];
		C[1] += -A[3]*B[0];
		C[2] += A[2]*B[0];
		C[3] += -A[1]*B[0];
		C[4] += -A[4]*B[0];
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 0 -> group 2)
	/// </summary>
	protected internal static void gp_default_2_0_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 1)
	/// </summary>
	protected internal static void gp_default_2_1_1(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[1]+A[1]*B[2]+A[3]*B[3]-A[6]*B[0]);
		C[1] += (A[0]*B[4]+A[2]*B[2]+A[4]*B[3]-A[7]*B[0]);
		C[2] += (A[1]*B[4]-A[2]*B[1]+A[5]*B[3]-A[8]*B[0]);
		C[3] += (A[3]*B[4]-A[4]*B[1]-A[5]*B[2]-A[9]*B[0]);
		C[4] += (A[6]*B[4]-A[7]*B[1]-A[8]*B[2]-A[9]*B[3]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 3)
	/// </summary>
	protected internal static void gp_default_2_1_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 0)
	/// </summary>
	protected internal static void gp_default_2_2_0(float[] A, float[] B, float[] C) {
		C[0] += (-A[0]*B[7]-A[1]*B[8]-A[2]*B[2]-A[3]*B[9]-A[4]*B[4]-A[5]*B[5]+A[6]*B[6]-A[7]*B[0]-A[8]*B[1]-A[9]*B[3]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 2)
	/// </summary>
	protected internal static void gp_default_2_2_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 4)
	/// </summary>
	protected internal static void gp_default_2_2_4(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[5]-A[1]*B[4]+A[2]*B[3]+A[3]*B[2]-A[4]*B[1]+A[5]*B[0]);
		C[1] += (A[0]*B[8]-A[1]*B[7]+A[2]*B[6]+A[6]*B[2]-A[7]*B[1]+A[8]*B[0]);
		C[2] += (A[0]*B[9]-A[3]*B[7]+A[4]*B[6]+A[6]*B[4]-A[7]*B[3]+A[9]*B[0]);
		C[3] += (A[1]*B[9]-A[3]*B[8]+A[5]*B[6]+A[6]*B[5]-A[8]*B[3]+A[9]*B[1]);
		C[4] += (A[2]*B[9]-A[4]*B[8]+A[5]*B[7]+A[7]*B[5]-A[8]*B[4]+A[9]*B[2]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 1)
	/// </summary>
	protected internal static void gp_default_2_3_1(float[] A, float[] B, float[] C) {
		C[0] += (-A[0]*B[4]-A[1]*B[5]-A[2]*B[0]-A[3]*B[7]-A[4]*B[1]-A[5]*B[2]);
		C[1] += (-A[1]*B[6]-A[3]*B[8]-A[5]*B[3]-A[6]*B[4]+A[8]*B[0]+A[9]*B[1]);
		C[2] += (A[0]*B[6]-A[3]*B[9]+A[4]*B[3]-A[6]*B[5]-A[7]*B[0]+A[9]*B[2]);
		C[3] += (A[0]*B[8]+A[1]*B[9]-A[2]*B[3]-A[6]*B[7]-A[7]*B[1]-A[8]*B[2]);
		C[4] += (-A[2]*B[6]-A[4]*B[8]-A[5]*B[9]-A[7]*B[4]-A[8]*B[5]-A[9]*B[7]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 3)
	/// </summary>
	protected internal static void gp_default_2_3_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 5)
	/// </summary>
	protected internal static void gp_default_2_3_5(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[9]-A[1]*B[8]+A[2]*B[7]+A[3]*B[6]-A[4]*B[5]+A[5]*B[4]-A[6]*B[3]+A[7]*B[2]-A[8]*B[1]+A[9]*B[0]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 2)
	/// </summary>
	protected internal static void gp_default_2_4_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 4)
	/// </summary>
	protected internal static void gp_default_2_4_4(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[3]-A[1]*B[2]+A[3]*B[1]-A[6]*B[0]);
		C[1] += (A[3]*B[4]-A[4]*B[3]+A[5]*B[2]-A[9]*B[0]);
		C[2] += (-A[1]*B[4]+A[2]*B[3]-A[5]*B[1]+A[8]*B[0]);
		C[3] += (A[0]*B[4]-A[2]*B[2]+A[4]*B[1]-A[7]*B[0]);
		C[4] += (A[6]*B[4]-A[7]*B[3]+A[8]*B[2]-A[9]*B[1]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 5 -> group 3)
	/// </summary>
	protected internal static void gp_default_2_5_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 0 -> group 3)
	/// </summary>
	protected internal static void gp_default_3_0_3(float[] A, float[] B, float[] C) {
		gp_default_2_0_2(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 1 -> group 2)
	/// </summary>
	protected internal static void gp_default_3_1_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 1 -> group 4)
	/// </summary>
	protected internal static void gp_default_3_1_4(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[3]-A[1]*B[2]+A[2]*B[1]-A[3]*B[0]);
		C[1] += (A[0]*B[4]-A[4]*B[2]+A[5]*B[1]-A[6]*B[0]);
		C[2] += (A[1]*B[4]-A[4]*B[3]+A[7]*B[1]-A[8]*B[0]);
		C[3] += (A[2]*B[4]-A[5]*B[3]+A[7]*B[2]-A[9]*B[0]);
		C[4] += (A[3]*B[4]-A[6]*B[3]+A[8]*B[2]-A[9]*B[1]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 1)
	/// </summary>
	protected internal static void gp_default_3_2_1(float[] A, float[] B, float[] C) {
		C[0] += (-A[0]*B[2]-A[1]*B[4]-A[2]*B[5]-A[4]*B[0]-A[5]*B[1]-A[7]*B[3]);
		C[1] += (A[0]*B[8]+A[1]*B[9]-A[3]*B[5]-A[4]*B[6]-A[6]*B[1]-A[8]*B[3]);
		C[2] += (-A[0]*B[7]+A[2]*B[9]+A[3]*B[4]-A[5]*B[6]+A[6]*B[0]-A[9]*B[3]);
		C[3] += (-A[1]*B[7]-A[2]*B[8]-A[3]*B[2]-A[7]*B[6]+A[8]*B[0]+A[9]*B[1]);
		C[4] += (-A[4]*B[7]-A[5]*B[8]-A[6]*B[2]-A[7]*B[9]-A[8]*B[4]-A[9]*B[5]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 3)
	/// </summary>
	protected internal static void gp_default_3_2_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 5)
	/// </summary>
	protected internal static void gp_default_3_2_5(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[9]-A[1]*B[8]+A[2]*B[7]-A[3]*B[6]+A[4]*B[5]-A[5]*B[4]+A[6]*B[3]+A[7]*B[2]-A[8]*B[1]+A[9]*B[0]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 0)
	/// </summary>
	protected internal static void gp_default_3_3_0(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[6]+A[1]*B[8]+A[2]*B[9]-A[3]*B[3]+A[4]*B[4]+A[5]*B[5]+A[6]*B[0]+A[7]*B[7]+A[8]*B[1]+A[9]*B[2]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 2)
	/// </summary>
	protected internal static void gp_default_3_3_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 4)
	/// </summary>
	protected internal static void gp_default_3_3_4(float[] A, float[] B, float[] C) {
		C[0] += (-A[0]*B[7]+A[1]*B[5]-A[2]*B[4]-A[4]*B[2]+A[5]*B[1]-A[7]*B[0]);
		C[1] += (-A[1]*B[9]+A[2]*B[8]-A[3]*B[7]-A[7]*B[3]+A[8]*B[2]-A[9]*B[1]);
		C[2] += (A[0]*B[9]-A[2]*B[6]+A[3]*B[5]+A[5]*B[3]-A[6]*B[2]+A[9]*B[0]);
		C[3] += (-A[0]*B[8]+A[1]*B[6]-A[3]*B[4]-A[4]*B[3]+A[6]*B[1]-A[8]*B[0]);
		C[4] += (-A[4]*B[9]+A[5]*B[8]-A[6]*B[7]-A[7]*B[6]+A[8]*B[5]-A[9]*B[4]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 1)
	/// </summary>
	protected internal static void gp_default_3_4_1(float[] A, float[] B, float[] C) {
		C[0] += (-A[0]*B[1]-A[1]*B[2]-A[2]*B[3]+A[3]*B[0]);
		C[1] += (-A[2]*B[4]+A[5]*B[1]+A[7]*B[2]+A[9]*B[0]);
		C[2] += (A[1]*B[4]-A[4]*B[1]+A[7]*B[3]-A[8]*B[0]);
		C[3] += (-A[0]*B[4]-A[4]*B[2]-A[5]*B[3]+A[6]*B[0]);
		C[4] += (-A[3]*B[4]+A[6]*B[1]+A[8]*B[2]+A[9]*B[3]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 3)
	/// </summary>
	protected internal static void gp_default_3_4_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 5 -> group 2)
	/// </summary>
	protected internal static void gp_default_3_5_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 0 -> group 4)
	/// </summary>
	protected internal static void gp_default_4_0_4(float[] A, float[] B, float[] C) {
		gp_default_1_0_1(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 3)
	/// </summary>
	protected internal static void gp_default_4_1_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 5)
	/// </summary>
	protected internal static void gp_default_4_1_5(float[] A, float[] B, float[] C) {
		gp_default_1_4_5(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 2)
	/// </summary>
	protected internal static void gp_default_4_2_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 4)
	/// </summary>
	protected internal static void gp_default_4_2_4(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[6]-A[1]*B[3]+A[2]*B[1]-A[3]*B[0]);
		C[1] += (A[0]*B[9]-A[2]*B[5]+A[3]*B[4]-A[4]*B[3]);
		C[2] += (-A[0]*B[8]+A[1]*B[5]-A[3]*B[2]+A[4]*B[1]);
		C[3] += (A[0]*B[7]-A[1]*B[4]+A[2]*B[2]-A[4]*B[0]);
		C[4] += (A[1]*B[9]-A[2]*B[8]+A[3]*B[7]-A[4]*B[6]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 1)
	/// </summary>
	protected internal static void gp_default_4_3_1(float[] A, float[] B, float[] C) {
		C[0] += (-A[0]*B[3]+A[1]*B[0]+A[2]*B[1]+A[3]*B[2]);
		C[1] += (-A[0]*B[9]-A[1]*B[5]-A[2]*B[7]+A[4]*B[2]);
		C[2] += (A[0]*B[8]+A[1]*B[4]-A[3]*B[7]-A[4]*B[1]);
		C[3] += (-A[0]*B[6]+A[2]*B[4]+A[3]*B[5]+A[4]*B[0]);
		C[4] += (-A[1]*B[6]-A[2]*B[8]-A[3]*B[9]+A[4]*B[3]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 3)
	/// </summary>
	protected internal static void gp_default_4_3_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 0)
	/// </summary>
	protected internal static void gp_default_4_4_0(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[4]-A[1]*B[1]-A[2]*B[2]-A[3]*B[3]+A[4]*B[0]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 2)
	/// </summary>
	protected internal static void gp_default_4_4_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 5 -> group 1)
	/// </summary>
	protected internal static void gp_default_4_5_1(float[] A, float[] B, float[] C) {
		C[0] += A[0]*B[0];
		C[1] += A[3]*B[0];
		C[2] += -A[2]*B[0];
		C[3] += A[1]*B[0];
		C[4] += A[4]*B[0];
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 5  x  group 0 -> group 5)
	/// </summary>
	protected internal static void gp_default_5_0_5(float[] A, float[] B, float[] C) {
		gp_default_0_0_0(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 5  x  group 1 -> group 4)
	/// </summary>
	protected internal static void gp_default_5_1_4(float[] A, float[] B, float[] C) {
		C[0] += -A[0]*B[0];
		C[1] += -A[0]*B[3];
		C[2] += A[0]*B[2];
		C[3] += -A[0]*B[1];
		C[4] += -A[0]*B[4];
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 5  x  group 2 -> group 3)
	/// </summary>
	protected internal static void gp_default_5_2_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 5  x  group 3 -> group 2)
	/// </summary>
	protected internal static void gp_default_5_3_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 5  x  group 4 -> group 1)
	/// </summary>
	protected internal static void gp_default_5_4_1(float[] A, float[] B, float[] C) {
		C[0] += A[0]*B[0];
		C[1] += A[0]*B[3];
		C[2] += -A[0]*B[2];
		C[3] += A[0]*B[1];
		C[4] += A[0]*B[4];
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 5  x  group 5 -> group 0)
	/// </summary>
	protected internal static void gp_default_5_5_0(float[] A, float[] B, float[] C) {
		C[0] += -A[0]*B[0];
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 0  x  group 0 -> group 0)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__0_0_0(float[] A, float[] B, float[] C) {
		C[0] += A[0]*B[0];
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 0  x  group 1 -> group 1)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__0_1_1(float[] A, float[] B, float[] C) {
		C[0] += A[0]*B[0];
		C[1] += A[0]*B[1];
		C[2] += A[0]*B[2];
		C[3] += A[0]*B[3];
		C[4] += A[0]*B[4];
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 0  x  group 2 -> group 2)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__0_2_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 0  x  group 3 -> group 3)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__0_3_3(float[] A, float[] B, float[] C) {
		gp__internal_euclidean_metric__0_2_2(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 0  x  group 4 -> group 4)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__0_4_4(float[] A, float[] B, float[] C) {
		gp__internal_euclidean_metric__0_1_1(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 0  x  group 5 -> group 5)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__0_5_5(float[] A, float[] B, float[] C) {
		gp__internal_euclidean_metric__0_0_0(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 0 -> group 1)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__1_0_1(float[] A, float[] B, float[] C) {
		C[0] += A[0]*B[0];
		C[1] += A[1]*B[0];
		C[2] += A[2]*B[0];
		C[3] += A[3]*B[0];
		C[4] += A[4]*B[0];
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 0)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__1_1_0(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[0]+A[1]*B[1]+A[2]*B[2]+A[3]*B[3]+A[4]*B[4]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 1 -> group 2)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__1_1_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 1)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__1_2_1(float[] A, float[] B, float[] C) {
		C[0] += (-A[1]*B[0]-A[2]*B[1]-A[3]*B[3]-A[4]*B[6]);
		C[1] += (A[0]*B[0]-A[2]*B[2]-A[3]*B[4]-A[4]*B[7]);
		C[2] += (A[0]*B[1]+A[1]*B[2]-A[3]*B[5]-A[4]*B[8]);
		C[3] += (A[0]*B[3]+A[1]*B[4]+A[2]*B[5]-A[4]*B[9]);
		C[4] += (A[0]*B[6]+A[1]*B[7]+A[2]*B[8]+A[3]*B[9]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 2 -> group 3)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__1_2_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 3 -> group 2)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__1_3_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 3 -> group 4)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__1_3_4(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[3]-A[1]*B[2]+A[2]*B[1]-A[3]*B[0]);
		C[1] += (A[0]*B[6]-A[1]*B[5]+A[2]*B[4]-A[4]*B[0]);
		C[2] += (A[0]*B[8]-A[1]*B[7]+A[3]*B[4]-A[4]*B[1]);
		C[3] += (A[0]*B[9]-A[2]*B[7]+A[3]*B[5]-A[4]*B[2]);
		C[4] += (A[1]*B[9]-A[2]*B[8]+A[3]*B[6]-A[4]*B[3]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 4 -> group 3)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__1_4_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 4 -> group 5)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__1_4_5(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[4]-A[1]*B[3]+A[2]*B[2]-A[3]*B[1]+A[4]*B[0]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 1  x  group 5 -> group 4)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__1_5_4(float[] A, float[] B, float[] C) {
		C[0] += A[4]*B[0];
		C[1] += -A[3]*B[0];
		C[2] += A[2]*B[0];
		C[3] += -A[1]*B[0];
		C[4] += A[0]*B[0];
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 0 -> group 2)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__2_0_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 1)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__2_1_1(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[1]+A[1]*B[2]+A[3]*B[3]+A[6]*B[4]);
		C[1] += (-A[0]*B[0]+A[2]*B[2]+A[4]*B[3]+A[7]*B[4]);
		C[2] += (-A[1]*B[0]-A[2]*B[1]+A[5]*B[3]+A[8]*B[4]);
		C[3] += (-A[3]*B[0]-A[4]*B[1]-A[5]*B[2]+A[9]*B[4]);
		C[4] += (-A[6]*B[0]-A[7]*B[1]-A[8]*B[2]-A[9]*B[3]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 1 -> group 3)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__2_1_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 0)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__2_2_0(float[] A, float[] B, float[] C) {
		C[0] += (-A[0]*B[0]-A[1]*B[1]-A[2]*B[2]-A[3]*B[3]-A[4]*B[4]-A[5]*B[5]-A[6]*B[6]-A[7]*B[7]-A[8]*B[8]-A[9]*B[9]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 2)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__2_2_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 2 -> group 4)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__2_2_4(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[5]-A[1]*B[4]+A[2]*B[3]+A[3]*B[2]-A[4]*B[1]+A[5]*B[0]);
		C[1] += (A[0]*B[8]-A[1]*B[7]+A[2]*B[6]+A[6]*B[2]-A[7]*B[1]+A[8]*B[0]);
		C[2] += (A[0]*B[9]-A[3]*B[7]+A[4]*B[6]+A[6]*B[4]-A[7]*B[3]+A[9]*B[0]);
		C[3] += (A[1]*B[9]-A[3]*B[8]+A[5]*B[6]+A[6]*B[5]-A[8]*B[3]+A[9]*B[1]);
		C[4] += (A[2]*B[9]-A[4]*B[8]+A[5]*B[7]+A[7]*B[5]-A[8]*B[4]+A[9]*B[2]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 1)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__2_3_1(float[] A, float[] B, float[] C) {
		C[0] += (-A[2]*B[0]-A[4]*B[1]-A[5]*B[2]-A[7]*B[4]-A[8]*B[5]-A[9]*B[7]);
		C[1] += (A[1]*B[0]+A[3]*B[1]-A[5]*B[3]+A[6]*B[4]-A[8]*B[6]-A[9]*B[8]);
		C[2] += (-A[0]*B[0]+A[3]*B[2]+A[4]*B[3]+A[6]*B[5]+A[7]*B[6]-A[9]*B[9]);
		C[3] += (-A[0]*B[1]-A[1]*B[2]-A[2]*B[3]+A[6]*B[7]+A[7]*B[8]+A[8]*B[9]);
		C[4] += (-A[0]*B[4]-A[1]*B[5]-A[2]*B[6]-A[3]*B[7]-A[4]*B[8]-A[5]*B[9]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 3)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__2_3_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 3 -> group 5)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__2_3_5(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[9]-A[1]*B[8]+A[2]*B[7]+A[3]*B[6]-A[4]*B[5]+A[5]*B[4]-A[6]*B[3]+A[7]*B[2]-A[8]*B[1]+A[9]*B[0]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 2)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__2_4_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 4 -> group 4)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__2_4_4(float[] A, float[] B, float[] C) {
		C[0] += (-A[6]*B[4]+A[7]*B[3]-A[8]*B[2]+A[9]*B[1]);
		C[1] += (A[3]*B[4]-A[4]*B[3]+A[5]*B[2]-A[9]*B[0]);
		C[2] += (-A[1]*B[4]+A[2]*B[3]-A[5]*B[1]+A[8]*B[0]);
		C[3] += (A[0]*B[4]-A[2]*B[2]+A[4]*B[1]-A[7]*B[0]);
		C[4] += (-A[0]*B[3]+A[1]*B[2]-A[3]*B[1]+A[6]*B[0]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 2  x  group 5 -> group 3)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__2_5_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 0 -> group 3)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__3_0_3(float[] A, float[] B, float[] C) {
		gp__internal_euclidean_metric__2_0_2(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 1 -> group 2)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__3_1_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 1 -> group 4)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__3_1_4(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[3]-A[1]*B[2]+A[2]*B[1]-A[3]*B[0]);
		C[1] += (A[0]*B[4]-A[4]*B[2]+A[5]*B[1]-A[6]*B[0]);
		C[2] += (A[1]*B[4]-A[4]*B[3]+A[7]*B[1]-A[8]*B[0]);
		C[3] += (A[2]*B[4]-A[5]*B[3]+A[7]*B[2]-A[9]*B[0]);
		C[4] += (A[3]*B[4]-A[6]*B[3]+A[8]*B[2]-A[9]*B[1]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 1)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__3_2_1(float[] A, float[] B, float[] C) {
		C[0] += (-A[0]*B[2]-A[1]*B[4]-A[2]*B[5]-A[4]*B[7]-A[5]*B[8]-A[7]*B[9]);
		C[1] += (A[0]*B[1]+A[1]*B[3]-A[3]*B[5]+A[4]*B[6]-A[6]*B[8]-A[8]*B[9]);
		C[2] += (-A[0]*B[0]+A[2]*B[3]+A[3]*B[4]+A[5]*B[6]+A[6]*B[7]-A[9]*B[9]);
		C[3] += (-A[1]*B[0]-A[2]*B[1]-A[3]*B[2]+A[7]*B[6]+A[8]*B[7]+A[9]*B[8]);
		C[4] += (-A[4]*B[0]-A[5]*B[1]-A[6]*B[2]-A[7]*B[3]-A[8]*B[4]-A[9]*B[5]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 3)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__3_2_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 2 -> group 5)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__3_2_5(float[] A, float[] B, float[] C) {
		C[0] += (A[0]*B[9]-A[1]*B[8]+A[2]*B[7]-A[3]*B[6]+A[4]*B[5]-A[5]*B[4]+A[6]*B[3]+A[7]*B[2]-A[8]*B[1]+A[9]*B[0]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 0)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__3_3_0(float[] A, float[] B, float[] C) {
		gp__internal_euclidean_metric__2_2_0(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 2)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__3_3_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 3 -> group 4)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__3_3_4(float[] A, float[] B, float[] C) {
		C[0] += (A[4]*B[9]-A[5]*B[8]+A[6]*B[7]+A[7]*B[6]-A[8]*B[5]+A[9]*B[4]);
		C[1] += (-A[1]*B[9]+A[2]*B[8]-A[3]*B[7]-A[7]*B[3]+A[8]*B[2]-A[9]*B[1]);
		C[2] += (A[0]*B[9]-A[2]*B[6]+A[3]*B[5]+A[5]*B[3]-A[6]*B[2]+A[9]*B[0]);
		C[3] += (-A[0]*B[8]+A[1]*B[6]-A[3]*B[4]-A[4]*B[3]+A[6]*B[1]-A[8]*B[0]);
		C[4] += (A[0]*B[7]-A[1]*B[5]+A[2]*B[4]+A[4]*B[2]-A[5]*B[1]+A[7]*B[0]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 1)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__3_4_1(float[] A, float[] B, float[] C) {
		C[0] += (A[3]*B[0]+A[6]*B[1]+A[8]*B[2]+A[9]*B[3]);
		C[1] += (-A[2]*B[0]-A[5]*B[1]-A[7]*B[2]+A[9]*B[4]);
		C[2] += (A[1]*B[0]+A[4]*B[1]-A[7]*B[3]-A[8]*B[4]);
		C[3] += (-A[0]*B[0]+A[4]*B[2]+A[5]*B[3]+A[6]*B[4]);
		C[4] += (-A[0]*B[1]-A[1]*B[2]-A[2]*B[3]-A[3]*B[4]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 4 -> group 3)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__3_4_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 3  x  group 5 -> group 2)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__3_5_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 0 -> group 4)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__4_0_4(float[] A, float[] B, float[] C) {
		gp__internal_euclidean_metric__1_0_1(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 3)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__4_1_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 1 -> group 5)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__4_1_5(float[] A, float[] B, float[] C) {
		gp__internal_euclidean_metric__1_4_5(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 2)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__4_2_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 2 -> group 4)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__4_2_4(float[] A, float[] B, float[] C) {
		C[0] += (-A[1]*B[9]+A[2]*B[8]-A[3]*B[7]+A[4]*B[6]);
		C[1] += (A[0]*B[9]-A[2]*B[5]+A[3]*B[4]-A[4]*B[3]);
		C[2] += (-A[0]*B[8]+A[1]*B[5]-A[3]*B[2]+A[4]*B[1]);
		C[3] += (A[0]*B[7]-A[1]*B[4]+A[2]*B[2]-A[4]*B[0]);
		C[4] += (-A[0]*B[6]+A[1]*B[3]-A[2]*B[1]+A[3]*B[0]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 1)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__4_3_1(float[] A, float[] B, float[] C) {
		C[0] += (-A[0]*B[3]-A[1]*B[6]-A[2]*B[8]-A[3]*B[9]);
		C[1] += (A[0]*B[2]+A[1]*B[5]+A[2]*B[7]-A[4]*B[9]);
		C[2] += (-A[0]*B[1]-A[1]*B[4]+A[3]*B[7]+A[4]*B[8]);
		C[3] += (A[0]*B[0]-A[2]*B[4]-A[3]*B[5]-A[4]*B[6]);
		C[4] += (A[1]*B[0]+A[2]*B[1]+A[3]*B[2]+A[4]*B[3]);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 3 -> group 3)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__4_3_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 0)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__4_4_0(float[] A, float[] B, float[] C) {
		gp__internal_euclidean_metric__1_1_0(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 4 -> group 2)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__4_4_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 4  x  group 5 -> group 1)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__4_5_1(float[] A, float[] B, float[] C) {
		gp__internal_euclidean_metric__1_5_4(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 5  x  group 0 -> group 5)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__5_0_5(float[] A, float[] B, float[] C) {
		gp__internal_euclidean_metric__0_0_0(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 5  x  group 1 -> group 4)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__5_1_4(float[] A, float[] B, float[] C) {
		C[0] += A[0]*B[4];
		C[1] += -A[0]*B[3];
		C[2] += A[0]*B[2];
		C[3] += -A[0]*B[1];
		C[4] += A[0]*B[0];
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 5  x  group 2 -> group 3)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__5_2_3(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 5  x  group 3 -> group 2)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__5_3_2(float[] A, float[] B, float[] C) {
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
	/// <summary>Computes the partial geometric product of two multivectors (group 5  x  group 4 -> group 1)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__5_4_1(float[] A, float[] B, float[] C) {
		gp__internal_euclidean_metric__5_1_4(A, B, C);
	}
	/// <summary>Computes the partial geometric product of two multivectors (group 5  x  group 5 -> group 0)
	/// </summary>
	protected internal static void gp__internal_euclidean_metric__5_5_0(float[] A, float[] B, float[] C) {
		gp__internal_euclidean_metric__0_0_0(A, B, C);
	}
	/// <summary>copies coordinates of group 0
	/// </summary>
	protected internal static void copyGroup_0(float[] A, float[] C) {
		C[0] = A[0];
	}
	/// <summary>copies and multiplies (by s) coordinates of group 0
	/// </summary>
	protected internal static void copyMul_0(float[] A, float[] C, float s) {
		C[0] = A[0]*s;
	}
	/// <summary>copies and divides (by s) coordinates of group 0
	/// </summary>
	protected internal static void copyDiv_0(float[] A, float[] C, float s) {
		C[0] = A[0]/s;
	}
	/// <summary>adds coordinates of group 0 from variable A to C
	/// </summary>
	protected internal static void add_0(float[] A, float[] C) {
		C[0] += A[0];
	}
	/// <summary>subtracts coordinates of group 0 in variable A from C
	/// </summary>
	protected internal static void sub_0(float[] A, float[] C) {
		C[0] -= A[0];
	}
	/// <summary>negate coordinates of group 0 of variable A
	/// </summary>
	protected internal static void neg_0(float[] A, float[] C) {
		C[0] = -A[0];
	}
	/// <summary>adds coordinates of group 0 of variables A and B
	/// </summary>
	protected internal static void add2_0_0(float[] A, float[] B, float[] C) {
		C[0] = (A[0]+B[0]);
	}
	/// <summary>subtracts coordinates of group 0 of variables A from B
	/// </summary>
	protected internal static void sub2_0_0(float[] A, float[] B, float[] C) {
		C[0] = (A[0]-B[0]);
	}
	/// <summary>performs coordinate-wise multiplication of coordinates of group 0 of variables A and B
	/// </summary>
	protected internal static void hp_0_0(float[] A, float[] B, float[] C) {
		C[0] = A[0]*B[0];
	}
	/// <summary>performs coordinate-wise division of coordinates of group 0 of variables A and B
	/// (no checks for divide by zero are made)
	/// </summary>
	protected internal static void ihp_0_0(float[] A, float[] B, float[] C) {
		C[0] = A[0]/((B[0]));
	}
	/// <summary>check for equality up to eps of coordinates of group 0 of variables A and B
	/// </summary>
	protected internal static bool equals_0_0(float[] A, float[] B, float eps) {
		if (((A[0] - B[0]) < -eps) || ((A[0] - B[0]) > eps)) return false;
	return true;
	}
	/// <summary>checks if coordinates of group 0 of variable A are zero up to eps
	/// </summary>
	protected internal static bool zeroGroup_0(float[] A, float eps) {
		if ((A[0] < -eps) || (A[0] > eps)) return false;
		return true;
	}
	/// <summary>copies coordinates of group 1
	/// </summary>
	protected internal static void copyGroup_1(float[] A, float[] C) {
		C[0] = A[0];
		C[1] = A[1];
		C[2] = A[2];
		C[3] = A[3];
		C[4] = A[4];
	}
	/// <summary>copies and multiplies (by s) coordinates of group 1
	/// </summary>
	protected internal static void copyMul_1(float[] A, float[] C, float s) {
		C[0] = A[0]*s;
		C[1] = A[1]*s;
		C[2] = A[2]*s;
		C[3] = A[3]*s;
		C[4] = A[4]*s;
	}
	/// <summary>copies and divides (by s) coordinates of group 1
	/// </summary>
	protected internal static void copyDiv_1(float[] A, float[] C, float s) {
		C[0] = A[0]/s;
		C[1] = A[1]/s;
		C[2] = A[2]/s;
		C[3] = A[3]/s;
		C[4] = A[4]/s;
	}
	/// <summary>adds coordinates of group 1 from variable A to C
	/// </summary>
	protected internal static void add_1(float[] A, float[] C) {
		C[0] += A[0];
		C[1] += A[1];
		C[2] += A[2];
		C[3] += A[3];
		C[4] += A[4];
	}
	/// <summary>subtracts coordinates of group 1 in variable A from C
	/// </summary>
	protected internal static void sub_1(float[] A, float[] C) {
		C[0] -= A[0];
		C[1] -= A[1];
		C[2] -= A[2];
		C[3] -= A[3];
		C[4] -= A[4];
	}
	/// <summary>negate coordinates of group 1 of variable A
	/// </summary>
	protected internal static void neg_1(float[] A, float[] C) {
		C[0] = -A[0];
		C[1] = -A[1];
		C[2] = -A[2];
		C[3] = -A[3];
		C[4] = -A[4];
	}
	/// <summary>adds coordinates of group 1 of variables A and B
	/// </summary>
	protected internal static void add2_1_1(float[] A, float[] B, float[] C) {
		C[0] = (A[0]+B[0]);
		C[1] = (A[1]+B[1]);
		C[2] = (A[2]+B[2]);
		C[3] = (A[3]+B[3]);
		C[4] = (A[4]+B[4]);
	}
	/// <summary>subtracts coordinates of group 1 of variables A from B
	/// </summary>
	protected internal static void sub2_1_1(float[] A, float[] B, float[] C) {
		C[0] = (A[0]-B[0]);
		C[1] = (A[1]-B[1]);
		C[2] = (A[2]-B[2]);
		C[3] = (A[3]-B[3]);
		C[4] = (A[4]-B[4]);
	}
	/// <summary>performs coordinate-wise multiplication of coordinates of group 1 of variables A and B
	/// </summary>
	protected internal static void hp_1_1(float[] A, float[] B, float[] C) {
		C[0] = A[0]*B[0];
		C[1] = A[1]*B[1];
		C[2] = A[2]*B[2];
		C[3] = A[3]*B[3];
		C[4] = A[4]*B[4];
	}
	/// <summary>performs coordinate-wise division of coordinates of group 1 of variables A and B
	/// (no checks for divide by zero are made)
	/// </summary>
	protected internal static void ihp_1_1(float[] A, float[] B, float[] C) {
		C[0] = A[0]/((B[0]));
		C[1] = A[1]/((B[1]));
		C[2] = A[2]/((B[2]));
		C[3] = A[3]/((B[3]));
		C[4] = A[4]/((B[4]));
	}
	/// <summary>check for equality up to eps of coordinates of group 1 of variables A and B
	/// </summary>
	protected internal static bool equals_1_1(float[] A, float[] B, float eps) {
		if (((A[0] - B[0]) < -eps) || ((A[0] - B[0]) > eps)) return false;
		if (((A[1] - B[1]) < -eps) || ((A[1] - B[1]) > eps)) return false;
		if (((A[2] - B[2]) < -eps) || ((A[2] - B[2]) > eps)) return false;
		if (((A[3] - B[3]) < -eps) || ((A[3] - B[3]) > eps)) return false;
		if (((A[4] - B[4]) < -eps) || ((A[4] - B[4]) > eps)) return false;
	return true;
	}
	/// <summary>checks if coordinates of group 1 of variable A are zero up to eps
	/// </summary>
	protected internal static bool zeroGroup_1(float[] A, float eps) {
		if ((A[0] < -eps) || (A[0] > eps)) return false;
		if ((A[1] < -eps) || (A[1] > eps)) return false;
		if ((A[2] < -eps) || (A[2] > eps)) return false;
		if ((A[3] < -eps) || (A[3] > eps)) return false;
		if ((A[4] < -eps) || (A[4] > eps)) return false;
		return true;
	}
	/// <summary>copies coordinates of group 2
	/// </summary>
	protected internal static void copyGroup_2(float[] A, float[] C) {
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
	/// <summary>copies and multiplies (by s) coordinates of group 2
	/// </summary>
	protected internal static void copyMul_2(float[] A, float[] C, float s) {
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
	/// <summary>copies and divides (by s) coordinates of group 2
	/// </summary>
	protected internal static void copyDiv_2(float[] A, float[] C, float s) {
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
	/// <summary>adds coordinates of group 2 from variable A to C
	/// </summary>
	protected internal static void add_2(float[] A, float[] C) {
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
	/// <summary>subtracts coordinates of group 2 in variable A from C
	/// </summary>
	protected internal static void sub_2(float[] A, float[] C) {
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
	/// <summary>negate coordinates of group 2 of variable A
	/// </summary>
	protected internal static void neg_2(float[] A, float[] C) {
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
	/// <summary>adds coordinates of group 2 of variables A and B
	/// </summary>
	protected internal static void add2_2_2(float[] A, float[] B, float[] C) {
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
	/// <summary>subtracts coordinates of group 2 of variables A from B
	/// </summary>
	protected internal static void sub2_2_2(float[] A, float[] B, float[] C) {
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
	/// <summary>performs coordinate-wise multiplication of coordinates of group 2 of variables A and B
	/// </summary>
	protected internal static void hp_2_2(float[] A, float[] B, float[] C) {
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
	/// <summary>performs coordinate-wise division of coordinates of group 2 of variables A and B
	/// (no checks for divide by zero are made)
	/// </summary>
	protected internal static void ihp_2_2(float[] A, float[] B, float[] C) {
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
	/// <summary>check for equality up to eps of coordinates of group 2 of variables A and B
	/// </summary>
	protected internal static bool equals_2_2(float[] A, float[] B, float eps) {
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
	/// <summary>checks if coordinates of group 2 of variable A are zero up to eps
	/// </summary>
	protected internal static bool zeroGroup_2(float[] A, float eps) {
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
	/// <summary>copies coordinates of group 3
	/// </summary>
	protected internal static void copyGroup_3(float[] A, float[] C) {
		copyGroup_2(A, C);
	}
	/// <summary>copies and multiplies (by s) coordinates of group 3
	/// </summary>
	protected internal static void copyMul_3(float[] A, float[] C, float s) {
		copyMul_2(A, C, s);
	}
	/// <summary>copies and divides (by s) coordinates of group 3
	/// </summary>
	protected internal static void copyDiv_3(float[] A, float[] C, float s) {
		copyDiv_2(A, C, s);
	}
	/// <summary>adds coordinates of group 3 from variable A to C
	/// </summary>
	protected internal static void add_3(float[] A, float[] C) {
		add_2(A, C);
	}
	/// <summary>subtracts coordinates of group 3 in variable A from C
	/// </summary>
	protected internal static void sub_3(float[] A, float[] C) {
		sub_2(A, C);
	}
	/// <summary>negate coordinates of group 3 of variable A
	/// </summary>
	protected internal static void neg_3(float[] A, float[] C) {
		neg_2(A, C);
	}
	/// <summary>adds coordinates of group 3 of variables A and B
	/// </summary>
	protected internal static void add2_3_3(float[] A, float[] B, float[] C) {
		add2_2_2(A, B, C);
	}
	/// <summary>subtracts coordinates of group 3 of variables A from B
	/// </summary>
	protected internal static void sub2_3_3(float[] A, float[] B, float[] C) {
		sub2_2_2(A, B, C);
	}
	/// <summary>performs coordinate-wise multiplication of coordinates of group 3 of variables A and B
	/// </summary>
	protected internal static void hp_3_3(float[] A, float[] B, float[] C) {
		hp_2_2(A, B, C);
	}
	/// <summary>performs coordinate-wise division of coordinates of group 3 of variables A and B
	/// (no checks for divide by zero are made)
	/// </summary>
	protected internal static void ihp_3_3(float[] A, float[] B, float[] C) {
		ihp_2_2(A, B, C);
	}
	/// <summary>check for equality up to eps of coordinates of group 3 of variables A and B
	/// </summary>
	protected internal static bool equals_3_3(float[] A, float[] B, float eps) {
		return equals_2_2(A, B, eps);
	}
	/// <summary>checks if coordinates of group 3 of variable A are zero up to eps
	/// </summary>
	protected internal static bool zeroGroup_3(float[] A, float eps) {
		return zeroGroup_2(A, eps);
	}
	/// <summary>copies coordinates of group 4
	/// </summary>
	protected internal static void copyGroup_4(float[] A, float[] C) {
		copyGroup_1(A, C);
	}
	/// <summary>copies and multiplies (by s) coordinates of group 4
	/// </summary>
	protected internal static void copyMul_4(float[] A, float[] C, float s) {
		copyMul_1(A, C, s);
	}
	/// <summary>copies and divides (by s) coordinates of group 4
	/// </summary>
	protected internal static void copyDiv_4(float[] A, float[] C, float s) {
		copyDiv_1(A, C, s);
	}
	/// <summary>adds coordinates of group 4 from variable A to C
	/// </summary>
	protected internal static void add_4(float[] A, float[] C) {
		add_1(A, C);
	}
	/// <summary>subtracts coordinates of group 4 in variable A from C
	/// </summary>
	protected internal static void sub_4(float[] A, float[] C) {
		sub_1(A, C);
	}
	/// <summary>negate coordinates of group 4 of variable A
	/// </summary>
	protected internal static void neg_4(float[] A, float[] C) {
		neg_1(A, C);
	}
	/// <summary>adds coordinates of group 4 of variables A and B
	/// </summary>
	protected internal static void add2_4_4(float[] A, float[] B, float[] C) {
		add2_1_1(A, B, C);
	}
	/// <summary>subtracts coordinates of group 4 of variables A from B
	/// </summary>
	protected internal static void sub2_4_4(float[] A, float[] B, float[] C) {
		sub2_1_1(A, B, C);
	}
	/// <summary>performs coordinate-wise multiplication of coordinates of group 4 of variables A and B
	/// </summary>
	protected internal static void hp_4_4(float[] A, float[] B, float[] C) {
		hp_1_1(A, B, C);
	}
	/// <summary>performs coordinate-wise division of coordinates of group 4 of variables A and B
	/// (no checks for divide by zero are made)
	/// </summary>
	protected internal static void ihp_4_4(float[] A, float[] B, float[] C) {
		ihp_1_1(A, B, C);
	}
	/// <summary>check for equality up to eps of coordinates of group 4 of variables A and B
	/// </summary>
	protected internal static bool equals_4_4(float[] A, float[] B, float eps) {
		return equals_1_1(A, B, eps);
	}
	/// <summary>checks if coordinates of group 4 of variable A are zero up to eps
	/// </summary>
	protected internal static bool zeroGroup_4(float[] A, float eps) {
		return zeroGroup_1(A, eps);
	}
	/// <summary>copies coordinates of group 5
	/// </summary>
	protected internal static void copyGroup_5(float[] A, float[] C) {
		copyGroup_0(A, C);
	}
	/// <summary>copies and multiplies (by s) coordinates of group 5
	/// </summary>
	protected internal static void copyMul_5(float[] A, float[] C, float s) {
		copyMul_0(A, C, s);
	}
	/// <summary>copies and divides (by s) coordinates of group 5
	/// </summary>
	protected internal static void copyDiv_5(float[] A, float[] C, float s) {
		copyDiv_0(A, C, s);
	}
	/// <summary>adds coordinates of group 5 from variable A to C
	/// </summary>
	protected internal static void add_5(float[] A, float[] C) {
		add_0(A, C);
	}
	/// <summary>subtracts coordinates of group 5 in variable A from C
	/// </summary>
	protected internal static void sub_5(float[] A, float[] C) {
		sub_0(A, C);
	}
	/// <summary>negate coordinates of group 5 of variable A
	/// </summary>
	protected internal static void neg_5(float[] A, float[] C) {
		neg_0(A, C);
	}
	/// <summary>adds coordinates of group 5 of variables A and B
	/// </summary>
	protected internal static void add2_5_5(float[] A, float[] B, float[] C) {
		add2_0_0(A, B, C);
	}
	/// <summary>subtracts coordinates of group 5 of variables A from B
	/// </summary>
	protected internal static void sub2_5_5(float[] A, float[] B, float[] C) {
		sub2_0_0(A, B, C);
	}
	/// <summary>performs coordinate-wise multiplication of coordinates of group 5 of variables A and B
	/// </summary>
	protected internal static void hp_5_5(float[] A, float[] B, float[] C) {
		hp_0_0(A, B, C);
	}
	/// <summary>performs coordinate-wise division of coordinates of group 5 of variables A and B
	/// (no checks for divide by zero are made)
	/// </summary>
	protected internal static void ihp_5_5(float[] A, float[] B, float[] C) {
		ihp_0_0(A, B, C);
	}
	/// <summary>check for equality up to eps of coordinates of group 5 of variables A and B
	/// </summary>
	protected internal static bool equals_5_5(float[] A, float[] B, float eps) {
		return equals_0_0(A, B, eps);
	}
	/// <summary>checks if coordinates of group 5 of variable A are zero up to eps
	/// </summary>
	protected internal static bool zeroGroup_5(float[] A, float eps) {
		return zeroGroup_0(A, eps);
	}
	/// <summary>Computes the partial dual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void dual_default_0_5(float[] A, float[] C) {
		C[0] = -A[0];
	}
	/// <summary>Computes the partial undual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void undual_default_0_5(float[] A, float[] C) {
		C[0] = A[0];
	}
	/// <summary>Computes the partial dual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void dual_default_1_4(float[] A, float[] C) {
		C[0] = A[0];
		C[1] = A[3];
		C[2] = -A[2];
		C[3] = A[1];
		C[4] = A[4];
	}
	/// <summary>Computes the partial undual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void undual_default_1_4(float[] A, float[] C) {
		C[0] = -A[0];
		C[1] = -A[3];
		C[2] = A[2];
		C[3] = -A[1];
		C[4] = -A[4];
	}
	/// <summary>Computes the partial dual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void dual_default_2_3(float[] A, float[] C) {
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
	/// <summary>Computes the partial undual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void undual_default_2_3(float[] A, float[] C) {
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
	/// <summary>Computes the partial dual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void dual_default_3_2(float[] A, float[] C) {
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
	/// <summary>Computes the partial undual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void undual_default_3_2(float[] A, float[] C) {
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
	/// <summary>Computes the partial dual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void dual_default_4_1(float[] A, float[] C) {
		undual_default_1_4(A, C);
	}
	/// <summary>Computes the partial undual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void undual_default_4_1(float[] A, float[] C) {
		dual_default_1_4(A, C);
	}
	/// <summary>Computes the partial dual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void dual_default_5_0(float[] A, float[] C) {
		undual_default_0_5(A, C);
	}
	/// <summary>Computes the partial undual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void undual_default_5_0(float[] A, float[] C) {
		dual_default_0_5(A, C);
	}
	/// <summary>Computes the partial dual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void dual__internal_euclidean_metric__0_5(float[] A, float[] C) {
		undual_default_0_5(A, C);
	}
	/// <summary>Computes the partial undual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void undual__internal_euclidean_metric__0_5(float[] A, float[] C) {
		undual_default_0_5(A, C);
	}
	/// <summary>Computes the partial dual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void dual__internal_euclidean_metric__1_4(float[] A, float[] C) {
		C[0] = A[4];
		C[1] = -A[3];
		C[2] = A[2];
		C[3] = -A[1];
		C[4] = A[0];
	}
	/// <summary>Computes the partial undual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void undual__internal_euclidean_metric__1_4(float[] A, float[] C) {
		dual__internal_euclidean_metric__1_4(A, C);
	}
	/// <summary>Computes the partial dual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void dual__internal_euclidean_metric__2_3(float[] A, float[] C) {
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
	/// <summary>Computes the partial undual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void undual__internal_euclidean_metric__2_3(float[] A, float[] C) {
		dual__internal_euclidean_metric__2_3(A, C);
	}
	/// <summary>Computes the partial dual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void dual__internal_euclidean_metric__3_2(float[] A, float[] C) {
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
	/// <summary>Computes the partial undual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void undual__internal_euclidean_metric__3_2(float[] A, float[] C) {
		dual__internal_euclidean_metric__3_2(A, C);
	}
	/// <summary>Computes the partial dual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void dual__internal_euclidean_metric__4_1(float[] A, float[] C) {
		dual__internal_euclidean_metric__1_4(A, C);
	}
	/// <summary>Computes the partial undual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void undual__internal_euclidean_metric__4_1(float[] A, float[] C) {
		dual__internal_euclidean_metric__1_4(A, C);
	}
	/// <summary>Computes the partial dual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void dual__internal_euclidean_metric__5_0(float[] A, float[] C) {
		undual_default_0_5(A, C);
	}
	/// <summary>Computes the partial undual (w.r.t. full space) of a multivector.
	/// </summary>
	protected internal static void undual__internal_euclidean_metric__5_0(float[] A, float[] C) {
		undual_default_0_5(A, C);
	}
/// <summary>Returns mv + mv.
/// </summary>
public static mv add(mv_if a, mv_if b)
{
	float[][] ac = a.to_mv().c();
	float[][] bc = b.to_mv().c();
	float[][] cc = new float[6][];
	
	if (ac[0] != null) {
		cc[0] = new float[1];
		if (bc[0] != null) {
			add2_0_0(ac[0], bc[0], cc[0]);
		}
		else copyGroup_0(ac[0], cc[0]);
	}
	else if (bc[0] != null) {
		cc[0] = new float[1];
		copyGroup_0(bc[0], cc[0]);
	}
	
	if (ac[1] != null) {
		cc[1] = new float[5];
		if (bc[1] != null) {
			add2_1_1(ac[1], bc[1], cc[1]);
		}
		else copyGroup_1(ac[1], cc[1]);
	}
	else if (bc[1] != null) {
		cc[1] = new float[5];
		copyGroup_1(bc[1], cc[1]);
	}
	
	if (ac[2] != null) {
		cc[2] = new float[10];
		if (bc[2] != null) {
			add2_2_2(ac[2], bc[2], cc[2]);
		}
		else copyGroup_2(ac[2], cc[2]);
	}
	else if (bc[2] != null) {
		cc[2] = new float[10];
		copyGroup_2(bc[2], cc[2]);
	}
	
	if (ac[3] != null) {
		cc[3] = new float[10];
		if (bc[3] != null) {
			add2_3_3(ac[3], bc[3], cc[3]);
		}
		else copyGroup_3(ac[3], cc[3]);
	}
	else if (bc[3] != null) {
		cc[3] = new float[10];
		copyGroup_3(bc[3], cc[3]);
	}
	
	if (ac[4] != null) {
		cc[4] = new float[5];
		if (bc[4] != null) {
			add2_4_4(ac[4], bc[4], cc[4]);
		}
		else copyGroup_4(ac[4], cc[4]);
	}
	else if (bc[4] != null) {
		cc[4] = new float[5];
		copyGroup_4(bc[4], cc[4]);
	}
	
	if (ac[5] != null) {
		cc[5] = new float[1];
		if (bc[5] != null) {
			add2_5_5(ac[5], bc[5], cc[5]);
		}
		else copyGroup_5(ac[5], cc[5]);
	}
	else if (bc[5] != null) {
		cc[5] = new float[1];
		copyGroup_5(bc[5], cc[5]);
	}
	return new mv(cc);
}
/// <summary>Returns mv - mv.
/// </summary>
public static mv subtract(mv_if a, mv_if b)
{
	float[][] ac = a.to_mv().c();
	float[][] bc = b.to_mv().c();
	float[][] cc = new float[6][];
	
	if (ac[0] != null) {
		cc[0] = new float[1];
		if (bc[0] != null) {
			sub2_0_0(ac[0], bc[0], cc[0]);
		}
		else copyGroup_0(ac[0], cc[0]);
	}
	else if (bc[0] != null) {
		cc[0] = new float[1];
		neg_0(bc[0], cc[0]);
	}
	
	if (ac[1] != null) {
		cc[1] = new float[5];
		if (bc[1] != null) {
			sub2_1_1(ac[1], bc[1], cc[1]);
		}
		else copyGroup_1(ac[1], cc[1]);
	}
	else if (bc[1] != null) {
		cc[1] = new float[5];
		neg_1(bc[1], cc[1]);
	}
	
	if (ac[2] != null) {
		cc[2] = new float[10];
		if (bc[2] != null) {
			sub2_2_2(ac[2], bc[2], cc[2]);
		}
		else copyGroup_2(ac[2], cc[2]);
	}
	else if (bc[2] != null) {
		cc[2] = new float[10];
		neg_2(bc[2], cc[2]);
	}
	
	if (ac[3] != null) {
		cc[3] = new float[10];
		if (bc[3] != null) {
			sub2_3_3(ac[3], bc[3], cc[3]);
		}
		else copyGroup_3(ac[3], cc[3]);
	}
	else if (bc[3] != null) {
		cc[3] = new float[10];
		neg_3(bc[3], cc[3]);
	}
	
	if (ac[4] != null) {
		cc[4] = new float[5];
		if (bc[4] != null) {
			sub2_4_4(ac[4], bc[4], cc[4]);
		}
		else copyGroup_4(ac[4], cc[4]);
	}
	else if (bc[4] != null) {
		cc[4] = new float[5];
		neg_4(bc[4], cc[4]);
	}
	
	if (ac[5] != null) {
		cc[5] = new float[1];
		if (bc[5] != null) {
			sub2_5_5(ac[5], bc[5], cc[5]);
		}
		else copyGroup_5(ac[5], cc[5]);
	}
	else if (bc[5] != null) {
		cc[5] = new float[1];
		neg_5(bc[5], cc[5]);
	}
	return new mv(cc);
}
/// <summary>Returns conformal point.
/// </summary>
public static normalizedPoint cgaPoint(float a, float b, float c)
{
	return new normalizedPoint(normalizedPoint.coord_e1_e2_e3_ni,
			a, // e1
			b, // e2
			c, // e3
			(0.5f*a*a+0.5f*b*b+0.5f*c*c) // ni
		);
}
/// <summary>Returns grade groupBitmap of  mv.
/// </summary>
public static mv extractGrade(mv_if a, GroupBitmap groupBitmap)
{
	GroupBitmap gu = a.to_mv().gu() & groupBitmap;
	float[][] ac = a.to_mv().c();
	float[][] cc = new float[6][];
	
	if ((gu & GroupBitmap.GROUP_0) != 0) {
		cc[0] = new float[1];
		copyGroup_0(ac[0], cc[0]);
	}
	
	if ((gu & GroupBitmap.GROUP_1) != 0) {
		cc[1] = new float[5];
		copyGroup_1(ac[1], cc[1]);
	}
	
	if ((gu & GroupBitmap.GROUP_2) != 0) {
		cc[2] = new float[10];
		copyGroup_2(ac[2], cc[2]);
	}
	
	if ((gu & GroupBitmap.GROUP_3) != 0) {
		cc[3] = new float[10];
		copyGroup_3(ac[3], cc[3]);
	}
	
	if ((gu & GroupBitmap.GROUP_4) != 0) {
		cc[4] = new float[5];
		copyGroup_4(ac[4], cc[4]);
	}
	
	if ((gu & GroupBitmap.GROUP_5) != 0) {
		cc[5] = new float[1];
		copyGroup_5(ac[5], cc[5]);
	}
	return new mv(cc);
}
/// <summary>Returns negation of mv.
/// </summary>
public static mv negate(mv_if a)
{
	float[][] ac = a.to_mv().c();
	float[][] cc = new float[6][];
	
	if (ac[0] != null) {
		cc[0] = new float[1];
		neg_0(ac[0], cc[0]);
	}
	
	if (ac[1] != null) {
		cc[1] = new float[5];
		neg_1(ac[1], cc[1]);
	}
	
	if (ac[2] != null) {
		cc[2] = new float[10];
		neg_2(ac[2], cc[2]);
	}
	
	if (ac[3] != null) {
		cc[3] = new float[10];
		neg_3(ac[3], cc[3]);
	}
	
	if (ac[4] != null) {
		cc[4] = new float[5];
		neg_4(ac[4], cc[4]);
	}
	
	if (ac[5] != null) {
		cc[5] = new float[1];
		neg_5(ac[5], cc[5]);
	}
	return new mv(cc);
}
/// <summary>Returns reverse of mv.
/// </summary>
public static mv reverse(mv_if a)
{
	float[][] ac = a.to_mv().c();
	float[][] cc = new float[6][];
	
	if (ac[0] != null) {
		cc[0] = new float[1];
		copyGroup_0(ac[0], cc[0]);
	}
	
	if (ac[1] != null) {
		cc[1] = new float[5];
		copyGroup_1(ac[1], cc[1]);
	}
	
	if (ac[2] != null) {
		cc[2] = new float[10];
		neg_2(ac[2], cc[2]);
	}
	
	if (ac[3] != null) {
		cc[3] = new float[10];
		neg_3(ac[3], cc[3]);
	}
	
	if (ac[4] != null) {
		cc[4] = new float[5];
		copyGroup_4(ac[4], cc[4]);
	}
	
	if (ac[5] != null) {
		cc[5] = new float[1];
		copyGroup_5(ac[5], cc[5]);
	}
	return new mv(cc);
}
/// <summary>Returns grade involution of mv.
/// </summary>
public static mv gradeInvolution(mv_if a)
{
	float[][] ac = a.to_mv().c();
	float[][] cc = new float[6][];
	
	if (ac[0] != null) {
		cc[0] = new float[1];
		copyGroup_0(ac[0], cc[0]);
	}
	
	if (ac[1] != null) {
		cc[1] = new float[5];
		neg_1(ac[1], cc[1]);
	}
	
	if (ac[2] != null) {
		cc[2] = new float[10];
		copyGroup_2(ac[2], cc[2]);
	}
	
	if (ac[3] != null) {
		cc[3] = new float[10];
		neg_3(ac[3], cc[3]);
	}
	
	if (ac[4] != null) {
		cc[4] = new float[5];
		copyGroup_4(ac[4], cc[4]);
	}
	
	if (ac[5] != null) {
		cc[5] = new float[1];
		neg_5(ac[5], cc[5]);
	}
	return new mv(cc);
}
/// <summary>Returns Clifford conjugate of mv.
/// </summary>
public static mv cliffordConjugate(mv_if a)
{
	float[][] ac = a.to_mv().c();
	float[][] cc = new float[6][];
	
	if (ac[0] != null) {
		cc[0] = new float[1];
		copyGroup_0(ac[0], cc[0]);
	}
	
	if (ac[1] != null) {
		cc[1] = new float[5];
		neg_1(ac[1], cc[1]);
	}
	
	if (ac[2] != null) {
		cc[2] = new float[10];
		neg_2(ac[2], cc[2]);
	}
	
	if (ac[3] != null) {
		cc[3] = new float[10];
		copyGroup_3(ac[3], cc[3]);
	}
	
	if (ac[4] != null) {
		cc[4] = new float[5];
		copyGroup_4(ac[4], cc[4]);
	}
	
	if (ac[5] != null) {
		cc[5] = new float[1];
		neg_5(ac[5], cc[5]);
	}
	return new mv(cc);
}
/// <summary>Returns geometric product of mv and mv.
/// </summary>
public static mv gp(mv_if a, mv_if b)
{
	float[][] ac = a.to_mv().c();
	float[][] bc = b.to_mv().c();
	float[][] cc = new float[6][];
	if (ac[0] != null) {
		if (bc[0] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_0_0_0(ac[0], bc[0], cc[0]);
		}
		if (bc[1] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_0_1_1(ac[0], bc[1], cc[1]);
		}
		if (bc[2] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_0_2_2(ac[0], bc[2], cc[2]);
		}
		if (bc[3] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_0_3_3(ac[0], bc[3], cc[3]);
		}
		if (bc[4] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_0_4_4(ac[0], bc[4], cc[4]);
		}
		if (bc[5] != null) {
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_0_5_5(ac[0], bc[5], cc[5]);
		}
	}
	if (ac[1] != null) {
		if (bc[0] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_1_0_1(ac[1], bc[0], cc[1]);
		}
		if (bc[1] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_1_1_0(ac[1], bc[1], cc[0]);
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_1_1_2(ac[1], bc[1], cc[2]);
		}
		if (bc[2] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_1_2_1(ac[1], bc[2], cc[1]);
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_1_2_3(ac[1], bc[2], cc[3]);
		}
		if (bc[3] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_1_3_2(ac[1], bc[3], cc[2]);
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_1_3_4(ac[1], bc[3], cc[4]);
		}
		if (bc[4] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_1_4_3(ac[1], bc[4], cc[3]);
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_1_4_5(ac[1], bc[4], cc[5]);
		}
		if (bc[5] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_1_5_4(ac[1], bc[5], cc[4]);
		}
	}
	if (ac[2] != null) {
		if (bc[0] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_2_0_2(ac[2], bc[0], cc[2]);
		}
		if (bc[1] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_2_1_1(ac[2], bc[1], cc[1]);
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_2_1_3(ac[2], bc[1], cc[3]);
		}
		if (bc[2] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_2_2_0(ac[2], bc[2], cc[0]);
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_2_2_2(ac[2], bc[2], cc[2]);
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_2_2_4(ac[2], bc[2], cc[4]);
		}
		if (bc[3] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_2_3_1(ac[2], bc[3], cc[1]);
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_2_3_3(ac[2], bc[3], cc[3]);
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_2_3_5(ac[2], bc[3], cc[5]);
		}
		if (bc[4] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_2_4_2(ac[2], bc[4], cc[2]);
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_2_4_4(ac[2], bc[4], cc[4]);
		}
		if (bc[5] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_2_5_3(ac[2], bc[5], cc[3]);
		}
	}
	if (ac[3] != null) {
		if (bc[0] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_3_0_3(ac[3], bc[0], cc[3]);
		}
		if (bc[1] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_3_1_2(ac[3], bc[1], cc[2]);
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_3_1_4(ac[3], bc[1], cc[4]);
		}
		if (bc[2] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_3_2_1(ac[3], bc[2], cc[1]);
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_3_2_3(ac[3], bc[2], cc[3]);
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_3_2_5(ac[3], bc[2], cc[5]);
		}
		if (bc[3] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_3_3_0(ac[3], bc[3], cc[0]);
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_3_3_2(ac[3], bc[3], cc[2]);
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_3_3_4(ac[3], bc[3], cc[4]);
		}
		if (bc[4] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_3_4_1(ac[3], bc[4], cc[1]);
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_3_4_3(ac[3], bc[4], cc[3]);
		}
		if (bc[5] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_3_5_2(ac[3], bc[5], cc[2]);
		}
	}
	if (ac[4] != null) {
		if (bc[0] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_4_0_4(ac[4], bc[0], cc[4]);
		}
		if (bc[1] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_4_1_3(ac[4], bc[1], cc[3]);
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_4_1_5(ac[4], bc[1], cc[5]);
		}
		if (bc[2] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_4_2_2(ac[4], bc[2], cc[2]);
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_4_2_4(ac[4], bc[2], cc[4]);
		}
		if (bc[3] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_4_3_1(ac[4], bc[3], cc[1]);
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_4_3_3(ac[4], bc[3], cc[3]);
		}
		if (bc[4] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_4_4_0(ac[4], bc[4], cc[0]);
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_4_4_2(ac[4], bc[4], cc[2]);
		}
		if (bc[5] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_4_5_1(ac[4], bc[5], cc[1]);
		}
	}
	if (ac[5] != null) {
		if (bc[0] != null) {
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_5_0_5(ac[5], bc[0], cc[5]);
		}
		if (bc[1] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_5_1_4(ac[5], bc[1], cc[4]);
		}
		if (bc[2] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_5_2_3(ac[5], bc[2], cc[3]);
		}
		if (bc[3] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_5_3_2(ac[5], bc[3], cc[2]);
		}
		if (bc[4] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_5_4_1(ac[5], bc[4], cc[1]);
		}
		if (bc[5] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_5_5_0(ac[5], bc[5], cc[0]);
		}
	}
	return new mv(cc);
}
/// <summary>Returns outer product of mv and mv.
/// </summary>
public static mv op(mv_if a, mv_if b)
{
	float[][] ac = a.to_mv().c();
	float[][] bc = b.to_mv().c();
	float[][] cc = new float[6][];
	if (ac[0] != null) {
		if (bc[0] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_0_0_0(ac[0], bc[0], cc[0]);
		}
		if (bc[1] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_0_1_1(ac[0], bc[1], cc[1]);
		}
		if (bc[2] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_0_2_2(ac[0], bc[2], cc[2]);
		}
		if (bc[3] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_0_3_3(ac[0], bc[3], cc[3]);
		}
		if (bc[4] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_0_4_4(ac[0], bc[4], cc[4]);
		}
		if (bc[5] != null) {
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_0_5_5(ac[0], bc[5], cc[5]);
		}
	}
	if (ac[1] != null) {
		if (bc[0] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_1_0_1(ac[1], bc[0], cc[1]);
		}
		if (bc[1] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_1_1_2(ac[1], bc[1], cc[2]);
		}
		if (bc[2] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_1_2_3(ac[1], bc[2], cc[3]);
		}
		if (bc[3] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_1_3_4(ac[1], bc[3], cc[4]);
		}
		if (bc[4] != null) {
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_1_4_5(ac[1], bc[4], cc[5]);
		}
	}
	if (ac[2] != null) {
		if (bc[0] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_2_0_2(ac[2], bc[0], cc[2]);
		}
		if (bc[1] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_2_1_3(ac[2], bc[1], cc[3]);
		}
		if (bc[2] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_2_2_4(ac[2], bc[2], cc[4]);
		}
		if (bc[3] != null) {
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_2_3_5(ac[2], bc[3], cc[5]);
		}
	}
	if (ac[3] != null) {
		if (bc[0] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_3_0_3(ac[3], bc[0], cc[3]);
		}
		if (bc[1] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_3_1_4(ac[3], bc[1], cc[4]);
		}
		if (bc[2] != null) {
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_3_2_5(ac[3], bc[2], cc[5]);
		}
	}
	if (ac[4] != null) {
		if (bc[0] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_4_0_4(ac[4], bc[0], cc[4]);
		}
		if (bc[1] != null) {
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_4_1_5(ac[4], bc[1], cc[5]);
		}
	}
	if (ac[5] != null) {
		if (bc[0] != null) {
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_5_0_5(ac[5], bc[0], cc[5]);
		}
	}
	return new mv(cc);
}
/// <summary>Returns scalar product of mv and mv.
/// </summary>
public static float sp(mv_if a, mv_if b)
{
	float[][] ac = a.to_mv().c();
	float[][] bc = b.to_mv().c();
	float[][] cc = new float[6][];
	cc[0] = new float[1];
	if (ac[0] != null) {
		if (bc[0] != null) {
			gp_default_0_0_0(ac[0], bc[0], cc[0]);
		}
	}
	if (ac[1] != null) {
		if (bc[1] != null) {
			gp_default_1_1_0(ac[1], bc[1], cc[0]);
		}
	}
	if (ac[2] != null) {
		if (bc[2] != null) {
			gp_default_2_2_0(ac[2], bc[2], cc[0]);
		}
	}
	if (ac[3] != null) {
		if (bc[3] != null) {
			gp_default_3_3_0(ac[3], bc[3], cc[0]);
		}
	}
	if (ac[4] != null) {
		if (bc[4] != null) {
			gp_default_4_4_0(ac[4], bc[4], cc[0]);
		}
	}
	if (ac[5] != null) {
		if (bc[5] != null) {
			gp_default_5_5_0(ac[5], bc[5], cc[0]);
		}
	}
	return cc[0][0];
}
/// <summary>Returns Modified Hestenes inner product of mv and mv.
/// </summary>
public static mv mhip(mv_if a, mv_if b)
{
	float[][] ac = a.to_mv().c();
	float[][] bc = b.to_mv().c();
	float[][] cc = new float[6][];
	if (ac[0] != null) {
		if (bc[0] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_0_0_0(ac[0], bc[0], cc[0]);
		}
		if (bc[1] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_0_1_1(ac[0], bc[1], cc[1]);
		}
		if (bc[2] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_0_2_2(ac[0], bc[2], cc[2]);
		}
		if (bc[3] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_0_3_3(ac[0], bc[3], cc[3]);
		}
		if (bc[4] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_0_4_4(ac[0], bc[4], cc[4]);
		}
		if (bc[5] != null) {
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_0_5_5(ac[0], bc[5], cc[5]);
		}
	}
	if (ac[1] != null) {
		if (bc[0] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_1_0_1(ac[1], bc[0], cc[1]);
		}
		if (bc[1] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_1_1_0(ac[1], bc[1], cc[0]);
		}
		if (bc[2] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_1_2_1(ac[1], bc[2], cc[1]);
		}
		if (bc[3] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_1_3_2(ac[1], bc[3], cc[2]);
		}
		if (bc[4] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_1_4_3(ac[1], bc[4], cc[3]);
		}
		if (bc[5] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_1_5_4(ac[1], bc[5], cc[4]);
		}
	}
	if (ac[2] != null) {
		if (bc[0] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_2_0_2(ac[2], bc[0], cc[2]);
		}
		if (bc[1] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_2_1_1(ac[2], bc[1], cc[1]);
		}
		if (bc[2] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_2_2_0(ac[2], bc[2], cc[0]);
		}
		if (bc[3] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_2_3_1(ac[2], bc[3], cc[1]);
		}
		if (bc[4] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_2_4_2(ac[2], bc[4], cc[2]);
		}
		if (bc[5] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_2_5_3(ac[2], bc[5], cc[3]);
		}
	}
	if (ac[3] != null) {
		if (bc[0] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_3_0_3(ac[3], bc[0], cc[3]);
		}
		if (bc[1] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_3_1_2(ac[3], bc[1], cc[2]);
		}
		if (bc[2] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_3_2_1(ac[3], bc[2], cc[1]);
		}
		if (bc[3] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_3_3_0(ac[3], bc[3], cc[0]);
		}
		if (bc[4] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_3_4_1(ac[3], bc[4], cc[1]);
		}
		if (bc[5] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_3_5_2(ac[3], bc[5], cc[2]);
		}
	}
	if (ac[4] != null) {
		if (bc[0] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_4_0_4(ac[4], bc[0], cc[4]);
		}
		if (bc[1] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_4_1_3(ac[4], bc[1], cc[3]);
		}
		if (bc[2] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_4_2_2(ac[4], bc[2], cc[2]);
		}
		if (bc[3] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_4_3_1(ac[4], bc[3], cc[1]);
		}
		if (bc[4] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_4_4_0(ac[4], bc[4], cc[0]);
		}
		if (bc[5] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_4_5_1(ac[4], bc[5], cc[1]);
		}
	}
	if (ac[5] != null) {
		if (bc[0] != null) {
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_5_0_5(ac[5], bc[0], cc[5]);
		}
		if (bc[1] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_5_1_4(ac[5], bc[1], cc[4]);
		}
		if (bc[2] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_5_2_3(ac[5], bc[2], cc[3]);
		}
		if (bc[3] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_5_3_2(ac[5], bc[3], cc[2]);
		}
		if (bc[4] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_5_4_1(ac[5], bc[4], cc[1]);
		}
		if (bc[5] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_5_5_0(ac[5], bc[5], cc[0]);
		}
	}
	return new mv(cc);
}
/// <summary>Returns left contraction of mv and mv.
/// </summary>
public static mv lc(mv_if a, mv_if b)
{
	float[][] ac = a.to_mv().c();
	float[][] bc = b.to_mv().c();
	float[][] cc = new float[6][];
	if (ac[0] != null) {
		if (bc[0] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_0_0_0(ac[0], bc[0], cc[0]);
		}
		if (bc[1] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_0_1_1(ac[0], bc[1], cc[1]);
		}
		if (bc[2] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_0_2_2(ac[0], bc[2], cc[2]);
		}
		if (bc[3] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_0_3_3(ac[0], bc[3], cc[3]);
		}
		if (bc[4] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_0_4_4(ac[0], bc[4], cc[4]);
		}
		if (bc[5] != null) {
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_0_5_5(ac[0], bc[5], cc[5]);
		}
	}
	if (ac[1] != null) {
		if (bc[1] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_1_1_0(ac[1], bc[1], cc[0]);
		}
		if (bc[2] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_1_2_1(ac[1], bc[2], cc[1]);
		}
		if (bc[3] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_1_3_2(ac[1], bc[3], cc[2]);
		}
		if (bc[4] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_1_4_3(ac[1], bc[4], cc[3]);
		}
		if (bc[5] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_1_5_4(ac[1], bc[5], cc[4]);
		}
	}
	if (ac[2] != null) {
		if (bc[2] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_2_2_0(ac[2], bc[2], cc[0]);
		}
		if (bc[3] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_2_3_1(ac[2], bc[3], cc[1]);
		}
		if (bc[4] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_2_4_2(ac[2], bc[4], cc[2]);
		}
		if (bc[5] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_2_5_3(ac[2], bc[5], cc[3]);
		}
	}
	if (ac[3] != null) {
		if (bc[3] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_3_3_0(ac[3], bc[3], cc[0]);
		}
		if (bc[4] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_3_4_1(ac[3], bc[4], cc[1]);
		}
		if (bc[5] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_3_5_2(ac[3], bc[5], cc[2]);
		}
	}
	if (ac[4] != null) {
		if (bc[4] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_4_4_0(ac[4], bc[4], cc[0]);
		}
		if (bc[5] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_4_5_1(ac[4], bc[5], cc[1]);
		}
	}
	if (ac[5] != null) {
		if (bc[5] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_5_5_0(ac[5], bc[5], cc[0]);
		}
	}
	return new mv(cc);
}
/// <summary>Returns norm of mv using default metric.
/// </summary>
public static float norm(mv_if a)
{
	float n2 = 0.0f;
	float[] c = new float[1];
	float[][] ac = a.to_mv().c();
	if (ac[0] != null) { /* group 0 (grade 0) */
		c[0] = 0.0f;
			gp_default_0_0_0(ac[0], ac[0], c);
		n2 += c[0];
	}
	if (ac[1] != null) { /* group 1 (grade 1) */
		c[0] = 0.0f;
			gp_default_1_1_0(ac[1], ac[1], c);
		n2 += c[0];
	}
	if (ac[2] != null) { /* group 2 (grade 2) */
		c[0] = 0.0f;
			gp_default_2_2_0(ac[2], ac[2], c);
		n2 -= c[0];
	}
	if (ac[3] != null) { /* group 3 (grade 3) */
		c[0] = 0.0f;
			gp_default_3_3_0(ac[3], ac[3], c);
		n2 -= c[0];
	}
	if (ac[4] != null) { /* group 4 (grade 4) */
		c[0] = 0.0f;
			gp_default_4_4_0(ac[4], ac[4], c);
		n2 += c[0];
	}
	if (ac[5] != null) { /* group 5 (grade 5) */
		c[0] = 0.0f;
			gp_default_5_5_0(ac[5], ac[5], c);
		n2 += c[0];
	}
	return ((n2 < 0.0f) ? (float)Math.Sqrt(-n2) : (float)Math.Sqrt(n2));
}
/// <summary>internal conversion function
/// </summary>
public static float norm_returns_scalar(mv a) {
	return norm(a);
}
/// <summary>Returns unit of mv using default metric.
/// </summary>
public static mv unit(mv_if a)
{
	float n = norm_returns_scalar(a.to_mv());
	float[][] ac = a.to_mv().c();
	float[][] cc = new float[6][];
	
	if (ac[0] != null) {
		cc[0] = new float[1];
		copyDiv_0(ac[0], cc[0], n);
	}
	
	if (ac[1] != null) {
		cc[1] = new float[5];
		copyDiv_1(ac[1], cc[1], n);
	}
	
	if (ac[2] != null) {
		cc[2] = new float[10];
		copyDiv_2(ac[2], cc[2], n);
	}
	
	if (ac[3] != null) {
		cc[3] = new float[10];
		copyDiv_3(ac[3], cc[3], n);
	}
	
	if (ac[4] != null) {
		cc[4] = new float[5];
		copyDiv_4(ac[4], cc[4], n);
	}
	
	if (ac[5] != null) {
		cc[5] = new float[1];
		copyDiv_5(ac[5], cc[5], n);
	}
	return new mv(cc);
}
/// <summary>Returns a * b * inverse(a) using default metric.
/// </summary>
public static mv applyVersor(mv_if a, mv_if b)
{
	return extractGrade(gp(gp(a, b), versorInverse(a)), b.to_mv().gu());
}
/// <summary>Returns a * b * reverse(a) using default metric. Only gives the correct result when the versor has a positive squared norm.
/// 
/// </summary>
public static mv applyUnitVersor(mv_if a, mv_if b)
{
	return extractGrade(gp(gp(a, b), reverse(a)), b.to_mv().gu());
}
/// <summary>Returns versor inverse of a using default metric.
/// </summary>
public static mv versorInverse(mv_if a)
{
	float n2 = norm2_returns_scalar(a.to_mv());
	float[][] ac = a.to_mv().c();
	float[][] cc = new float[6][];
	
	if (ac[0] != null) {
		cc[0] = new float[1];
		copyDiv_0(ac[0], cc[0], n2);
	}
	
	if (ac[1] != null) {
		cc[1] = new float[5];
		copyDiv_1(ac[1], cc[1], n2);
	}
	
	if (ac[2] != null) {
		cc[2] = new float[10];
		copyDiv_2(ac[2], cc[2], -n2);
	}
	
	if (ac[3] != null) {
		cc[3] = new float[10];
		copyDiv_3(ac[3], cc[3], -n2);
	}
	
	if (ac[4] != null) {
		cc[4] = new float[5];
		copyDiv_4(ac[4], cc[4], n2);
	}
	
	if (ac[5] != null) {
		cc[5] = new float[1];
		copyDiv_5(ac[5], cc[5], n2);
	}
	return new mv(cc);
}
/// <summary>Returns whether input multivectors are equal up to an epsilon c.
/// </summary>
public static bool equals(mv_if a, mv_if b, float c)
{
	float[][] ac = a.to_mv().c();
	float[][] bc = b.to_mv().c();
	
	if (ac[0] != null) {
		if (bc[0] != null) {
			if (!equals_0_0(ac[0], bc[0], c)) return false;
		}
		else if (!zeroGroup_0(ac[0], c)) return false;
	}
		else if (bc[0] != null) {
		if (!zeroGroup_0(bc[0], c)) return false;
	}
	
	if (ac[1] != null) {
		if (bc[1] != null) {
			if (!equals_1_1(ac[1], bc[1], c)) return false;
		}
		else if (!zeroGroup_1(ac[1], c)) return false;
	}
		else if (bc[1] != null) {
		if (!zeroGroup_1(bc[1], c)) return false;
	}
	
	if (ac[2] != null) {
		if (bc[2] != null) {
			if (!equals_2_2(ac[2], bc[2], c)) return false;
		}
		else if (!zeroGroup_2(ac[2], c)) return false;
	}
		else if (bc[2] != null) {
		if (!zeroGroup_2(bc[2], c)) return false;
	}
	
	if (ac[3] != null) {
		if (bc[3] != null) {
			if (!equals_3_3(ac[3], bc[3], c)) return false;
		}
		else if (!zeroGroup_3(ac[3], c)) return false;
	}
		else if (bc[3] != null) {
		if (!zeroGroup_3(bc[3], c)) return false;
	}
	
	if (ac[4] != null) {
		if (bc[4] != null) {
			if (!equals_4_4(ac[4], bc[4], c)) return false;
		}
		else if (!zeroGroup_4(ac[4], c)) return false;
	}
		else if (bc[4] != null) {
		if (!zeroGroup_4(bc[4], c)) return false;
	}
	
	if (ac[5] != null) {
		if (bc[5] != null) {
			if (!equals_5_5(ac[5], bc[5], c)) return false;
		}
		else if (!zeroGroup_5(ac[5], c)) return false;
	}
		else if (bc[5] != null) {
		if (!zeroGroup_5(bc[5], c)) return false;
	}
	return true;
}
/// <summary>Returns true if all coordinates of a are abs <= b
/// </summary>
public static bool zero(mv_if a, float b)
{
	float[][] ac = a.to_mv().c();
	
	if (ac[0] != null) {
		if (!zeroGroup_0(ac[0], b)) return false;
	}
	
	if (ac[1] != null) {
		if (!zeroGroup_1(ac[1], b)) return false;
	}
	
	if (ac[2] != null) {
		if (!zeroGroup_2(ac[2], b)) return false;
	}
	
	if (ac[3] != null) {
		if (!zeroGroup_3(ac[3], b)) return false;
	}
	
	if (ac[4] != null) {
		if (!zeroGroup_4(ac[4], b)) return false;
	}
	
	if (ac[5] != null) {
		if (!zeroGroup_5(ac[5], b)) return false;
	}
	return true;
}
/// <summary>Returns dual of mv using default metric.
/// </summary>
public static mv dual(mv_if a)
{
	float[][] ac = a.to_mv().c();
	float[][] cc = new float[6][];
	if (ac[0] != null) {
		if (cc[5] == null) cc[5] = new float[1];
		dual_default_0_5(ac[0], cc[5]);
	}
	
	if (ac[1] != null) {
		if (cc[4] == null) cc[4] = new float[5];
		dual_default_1_4(ac[1], cc[4]);
	}
	
	if (ac[2] != null) {
		if (cc[3] == null) cc[3] = new float[10];
		dual_default_2_3(ac[2], cc[3]);
	}
	
	if (ac[3] != null) {
		if (cc[2] == null) cc[2] = new float[10];
		dual_default_3_2(ac[3], cc[2]);
	}
	
	if (ac[4] != null) {
		if (cc[1] == null) cc[1] = new float[5];
		dual_default_4_1(ac[4], cc[1]);
	}
	
	if (ac[5] != null) {
		if (cc[0] == null) cc[0] = new float[1];
		dual_default_5_0(ac[5], cc[0]);
	}
	
	return new mv(cc);
}
/// <summary>Returns undual of mv using default metric.
/// </summary>
public static mv undual(mv_if a)
{
	float[][] ac = a.to_mv().c();
	float[][] cc = new float[6][];
	if (ac[0] != null) {
		if (cc[5] == null) cc[5] = new float[1];
		undual_default_0_5(ac[0], cc[5]);
	}
	
	if (ac[1] != null) {
		if (cc[4] == null) cc[4] = new float[5];
		undual_default_1_4(ac[1], cc[4]);
	}
	
	if (ac[2] != null) {
		if (cc[3] == null) cc[3] = new float[10];
		undual_default_2_3(ac[2], cc[3]);
	}
	
	if (ac[3] != null) {
		if (cc[2] == null) cc[2] = new float[10];
		undual_default_3_2(ac[3], cc[2]);
	}
	
	if (ac[4] != null) {
		if (cc[1] == null) cc[1] = new float[5];
		undual_default_4_1(ac[4], cc[1]);
	}
	
	if (ac[5] != null) {
		if (cc[0] == null) cc[0] = new float[1];
		undual_default_5_0(ac[5], cc[0]);
	}
	
	return new mv(cc);
}

/// <summary>Computes exponential of mv up to 12th term.
/// 
/// </summary>
public static mv exp(mv x) {
	return exp(x, 12);
}

/// <summary>Computes exponential of mv.
/// 
/// </summary>
public static mv exp(mv x, int order) {
   
	{ // First try special cases: check if (x * x) is scalar
		mv xSquared = gp(x, x);
		float s_xSquared = xSquared.get_scalar();
		if ((norm2_returns_scalar(xSquared) - s_xSquared * s_xSquared) < 1E-06f) {
			// OK (x * x == ~scalar), so use special cases:
			if (s_xSquared < 0.0f) {
				float a = (float)Math.Sqrt(-s_xSquared);
				return sas(x, (float)Math.Sin(a) / a, (float)Math.Cos(a));
			}
			else if (s_xSquared > 0.0f) {
				float a = (float)Math.Sqrt(s_xSquared);
				return sas(x, (float)Math.Sinh(a) / a, (float)Math.Cosh(a));
			}
			else {
				return sas(x, 1.0f, 1.0f);
			}
		}
	}

	// else do general series eval . . .

	// result = 1 + ....	
	mv result = new mv(1.0f);
	if (order == 0) return result;

	// find scale (power of 2) such that its norm is < 1
	ulong maxC = (ulong)x.LargestCoordinate();
	int scale = 1;
	if (maxC > 1) scale <<= 1;
	while (maxC != 0)
	{
		maxC >>= 1;
		scale <<= 1;
	}

	// scale
	mv xScaled = gp(x, 1.0f / (float)scale); 

	// taylor series approximation
	mv xPow1 = new mv(1.0f); 
	for (int i = 1; i <= order; i++) {
		mv xPow2 = gp(xPow1, xScaled);
		xPow1 = gp(xPow2, 1.0f / (float)i);
		
		result = add(result, xPow1); // result2 = result1 + xPow1
    }

	// undo scaling
	while (scale > 1)
	{
		result = gp(result, result);
		scale >>= 1;
	}
    
    return result;
} // end of exp()

/// <summary>Returns geometric product of mv and float.
/// </summary>
public static mv gp(mv_if a, float b)
{
	float[][] ac = a.to_mv().c();
	float[][] bc = new float[][]{new float[]{b}};
	float[][] cc = new float[6][];
	if (ac[0] != null) {
			if (cc[0] == null) cc[0] = new float[1];
			gp_default_0_0_0(ac[0], bc[0], cc[0]);
	}
	if (ac[1] != null) {
			if (cc[1] == null) cc[1] = new float[5];
			gp_default_1_0_1(ac[1], bc[0], cc[1]);
	}
	if (ac[2] != null) {
			if (cc[2] == null) cc[2] = new float[10];
			gp_default_2_0_2(ac[2], bc[0], cc[2]);
	}
	if (ac[3] != null) {
			if (cc[3] == null) cc[3] = new float[10];
			gp_default_3_0_3(ac[3], bc[0], cc[3]);
	}
	if (ac[4] != null) {
			if (cc[4] == null) cc[4] = new float[5];
			gp_default_4_0_4(ac[4], bc[0], cc[4]);
	}
	if (ac[5] != null) {
			if (cc[5] == null) cc[5] = new float[1];
			gp_default_5_0_5(ac[5], bc[0], cc[5]);
	}
	return new mv(cc);
}
/// <summary>Returns norm2 of mv using default metric.
/// </summary>
public static float norm2(mv_if a)
{
	float n2 = 0.0f;
	float[] c = new float[1];
	float[][] ac = a.to_mv().c();
	if (ac[0] != null) { /* group 0 (grade 0) */
		c[0] = 0.0f;
			gp_default_0_0_0(ac[0], ac[0], c);
		n2 += c[0];
	}
	if (ac[1] != null) { /* group 1 (grade 1) */
		c[0] = 0.0f;
			gp_default_1_1_0(ac[1], ac[1], c);
		n2 += c[0];
	}
	if (ac[2] != null) { /* group 2 (grade 2) */
		c[0] = 0.0f;
			gp_default_2_2_0(ac[2], ac[2], c);
		n2 -= c[0];
	}
	if (ac[3] != null) { /* group 3 (grade 3) */
		c[0] = 0.0f;
			gp_default_3_3_0(ac[3], ac[3], c);
		n2 -= c[0];
	}
	if (ac[4] != null) { /* group 4 (grade 4) */
		c[0] = 0.0f;
			gp_default_4_4_0(ac[4], ac[4], c);
		n2 += c[0];
	}
	if (ac[5] != null) { /* group 5 (grade 5) */
		c[0] = 0.0f;
			gp_default_5_5_0(ac[5], ac[5], c);
		n2 += c[0];
	}
	return n2;
}
/// <summary>internal conversion function
/// </summary>
public static float norm2_returns_scalar(mv a) {
	return norm2(a);
}
/// <summary>Returns float b * mv a + float c.
/// </summary>
public static mv sas(mv_if a, float b, float c)
{
	float[][] ac = a.to_mv().c();
	float[][] cc = new float[6][];
	
	if (ac[0] != null) {
		cc[0] = new float[1];
		copyMul_0(ac[0], cc[0], b);
		cc[0][0] += c;
	}
	else if (c != 0.0) {
		cc[0] = new float[1];
	cc[0][0] = c;
	}
	
	if (ac[1] != null) {
		cc[1] = new float[5];
		copyMul_1(ac[1], cc[1], b);
	}
	
	if (ac[2] != null) {
		cc[2] = new float[10];
		copyMul_2(ac[2], cc[2], b);
	}
	
	if (ac[3] != null) {
		cc[3] = new float[10];
		copyMul_3(ac[3], cc[3], b);
	}
	
	if (ac[4] != null) {
		cc[4] = new float[5];
		copyMul_4(ac[4], cc[4], b);
	}
	
	if (ac[5] != null) {
		cc[5] = new float[1];
		copyMul_5(ac[5], cc[5], b);
	}
	return new mv(cc);
}
} // end of class c3ga
} // end of namespace c3ga_ns
