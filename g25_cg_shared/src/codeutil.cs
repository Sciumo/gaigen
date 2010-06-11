// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

// Copyright 2008-2010, Daniel Fontijne, University of Amsterdam -- fontijne@science.uva.nl

using System;
using System.Collections.Generic;
using System.Text;

using RefGA.Symbolic;

namespace G25.CG.Shared
{

    /// <summary>
    /// Contains various utility functions for generating code.
    /// </summary>
    public class CodeUtil
    {

        static CodeUtil()
        {
            InitOps();
        }


        /// <returns>Name of bool type in output language</returns>
        public static string GetBoolType(Specification S)
        {
            return (S.OutputC()) ? "int" : ((S.OutputJava()) ? "boolean" : "bool");
        }

        /// <returns>Name of false value in output language</returns>
        public static string GetFalseValue(Specification S)
        {
            return (S.OutputC()) ? "0" : "false";
        }

        /// <returns>Name of false value in output language</returns>
        public static string GetTrueValue(Specification S)
        {
            return (S.OutputC()) ? "1" : "true";
        }

        /// <summary>
        /// Returns an array of 'access strings' which are source code expressions that can be
        /// used to access the coordinates of <c>smv</c>. The entries in the array correspond to
        /// the non-const basis blades in <c>smv</c>. 
        /// 
        /// An example of access strings is <c>"A.e1"</c>, <c>"A.e2"</c>, <c>"A.e3"</c>, for a 3-D vector type.
        /// This could also be <c>"A.c[0]"</c>, <c>"A.c[1]"</c>, <c>"A.c[2]"</c> if coordinates are stored in an array.
        /// 
        /// If 'ptr' is true, <c>"->"</c> will be used to access instance variables, otherwise <c>"."</c> is used.
        /// (this may need to be tweaked as more languages are available for output).
        /// </summary>
        /// <param name="S">Used for basis blade names, language, COORD_STORAGE, etc.</param>
        /// <param name="smv">The specialized multivector for which access strings are generated.</param>
        /// <param name="smvName">The variable name to be used in the access strings. For example, specify <c>"A"</c> to get <c>"A.c[0]"</c> and so on.</param>
        /// <param name="ptr">Is the variable a pointer or a reference/value?</param>
        /// <returns>Array of strings that can be used to access the non-constant coordinates of the 'smv'.</returns>
        public static string[] GetAccessStr(Specification S, G25.SMV smv, string smvName, bool ptr)
        {
            string[] AL = new string[smv.NbNonConstBasisBlade];
            string accessStr = (ptr) ? "->" : ".";
            string memberPrefix = "";


            // C++: override "this->" to ""
            if (S.OutputCpp())
            {
                memberPrefix = "m_";
                if ((smvName == SmvUtil.THIS) && (ptr == true))
                {
                    smvName = "";
                    accessStr = "";
                }
            }

            // C#, Java: override "this." to ""
            else if (S.OutputCSharpOrJava())
            {
                memberPrefix = "m_";
                if ((smvName == SmvUtil.THIS) && (ptr == false))
                {
                    smvName = "";
                    accessStr = "";
                }
            }

            string prefix = smvName + accessStr + memberPrefix;

            for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                AL[i] = prefix + smv.GetCoordLangID(i, S);

            return AL;
        } // end of GetAccessStr()

        /// <summary>
        /// Returns an array of 'access strings' for a specific group of the general multivector. 
        /// Access strings are source code expressions that can be
        /// used to access the coordinates of one specific group of coordinates of <c>gmv</c>.
        /// </summary>
        /// <param name="S">Used for basis blade names, language, COORD_STORAGE , etc.</param>
        /// <param name="gmv">The specialized multivector for which access strings are generated.</param>
        /// <param name="gmvName">The variable name of the _array_ of float to be used in the access strings. 
        /// For example, specify <c>"A"</c> to get <c>"A[0]"</c> and so on.</param>
        /// <param name="groupIdx">Specifies for that group index the access strings should be generated.</param>
        /// <param name="baseIdx">Index of first coordinate of group (required for C# and Java)</param>
        /// <returns>Array of strings that can be used to access the coordinates of group <c>groupIdx</c> of the <c>gmv</c>.</returns>
        public static string[] GetAccessStr(Specification S, G25.GMV gmv, String gmvName, int groupIdx, int baseIdx)
        {
            string[] AL = new String[gmv.Group(groupIdx).Length];

            for (int i = 0; i < gmv.Group(groupIdx).Length; i++)
                AL[i] = gmvName + "[" + (baseIdx + i) + "]";

            return AL;
        } // end of GetAccessStr()


        /// <summary>
        /// Generates code for returning a scalar value. The input is a scalar-valued multivector.
        /// </summary>
        /// <param name="S">Specification (used for output language).</param>
        /// <param name="FT">Floating point type which must be returned.</param>
        /// <param name="mustCast">Set to true if the returned value must be cast to <c>FT</c>, that is
        /// if you are not sure that the multivector has the same float type as the return type <c>FT</c>.</param>
        /// <param name="value">The symbolic multivector value to be returned.</param>
        /// <returns>Code for returning a scalar value.</returns>
        public static string GenerateScalarReturnCode(Specification S, FloatType FT, bool mustCast, RefGA.Multivector value)
        {
            if (value.IsZero())
            {
                return "return " + FT.DoubleToString(S, 0.0) + ";";
            }
            else
            {

                return "return " +
                    ((mustCast) ? (FT.castStr + "(") : "") +
                    CodeUtil.ScalarToLangString(S, FT, value.BasisBlades[0]) +
                    ((mustCast) ? ")" : "") +
                    ";";
            }
        } // end of GenerateScalarReturnCode()

        /// <summary>
        /// Generates the code to assign a multivector value (which may have symbolic coordinates) to a specialized multivector.
        /// </summary>
        /// <param name="S">Specification of algebra. Used to known names of basis vector, output language, access strings, etc.</param>
        /// <param name="FT">Floating point type of destination.</param>
        /// <param name="mustCast">Set to true if a cast to 'FT' must be performed before assigned to 'dstName'.</param>
        /// <param name="dstSmv">Type of specialized multivector assigned to.</param>
        /// <param name="dstName">Name of specialized multivector assigned to.</param>
        /// <param name="dstPtr">Is the destination of pointer?</param>
        /// <param name="value">Multivector value to assign to the SMV. Must not contain basis blades inside the symbolic scalars.</param>
        /// <param name="nbTabs">Number of tabs to put before the code.</param>
        /// <param name="writeZeros">Some callers want to skip <c>"= 0.0"</c> assignments because they would be redundant. For
        /// example the caller may know that the destination is already set to zero. If so, set this argument to false and
        /// code for setting coordinates to 0 will not be generated.</param>
        /// <returns>String of code for dstName = value;</returns>
        public static string GenerateSMVassignmentCode(Specification S, FloatType FT, bool mustCast,
            G25.SMV dstSmv, String dstName, bool dstPtr, RefGA.Multivector value, int nbTabs, bool writeZeros)
        {
            RefGA.BasisBlade[] BL = BasisBlade.GetNonConstBladeList(dstSmv);
            string[] accessStr = GetAccessStr(S, dstSmv, dstName, dstPtr);
            string[] assignedStr = GetAssignmentStrings(S, FT, mustCast, BL, value, writeZeros);

            //int nbTabs = 1;
            return GenerateAssignmentCode(S, accessStr, assignedStr, nbTabs, writeZeros);
        } // end of GenerateSMVassignmentCode

        /// <summary>
        /// Generates the code to assign a multivector value (which may have symbolic coordinates) to one specific coordinate group of a multivector.
        /// </summary>
        /// <param name="S">Specification of algebra. Used to known names of basis vector, output language, access strings, etc.</param>
        /// <param name="FT">Floating point type of destination.</param>
        /// <param name="mustCast">set to true if a cast to 'FT' must be performed before assigned to 'dstName'.</param>
        /// <param name="dstGmv">Type of general multivector assigned to.</param>
        /// <param name="dstName">Name of specialized multivector assigned to.</param>
        /// <param name="dstGroupIdx">Write to which group in destination type?</param>
        /// <param name="dstBaseIdx">Base index of coordinate 0 of group (needed for C# and Java)</param>
        /// <param name="value">Multivector value to assign to the GMV. Must not contain basis blades inside the symbolic scalars.</param>
        /// <param name="nbTabs">Number of tabs to put before the code.</param>
        /// <param name="writeZeros">Some callers want to skip "= 0.0" assignments because they would be redundant. So they set this argument to true.</param>
        /// <returns>String of code for dstName = value;</returns>
        public static string GenerateGMVassignmentCode(Specification S, FloatType FT, bool mustCast,
            G25.GMV dstGmv, string dstName, int dstGroupIdx, int dstBaseIdx, RefGA.Multivector value, int nbTabs, bool writeZeros)
        {
            RefGA.BasisBlade[] BL = dstGmv.Group(dstGroupIdx);
            string[] accessStr = GetAccessStr(S, dstGmv, dstName, dstGroupIdx, dstBaseIdx);
            string[] assignedStr = GetAssignmentStrings(S, FT, mustCast, BL, value, writeZeros);

            return GenerateAssignmentCode(S, accessStr, assignedStr, nbTabs, writeZeros);
        } // end of GenerateSMVassignmentCode


        public static string GenerateReturnCode(Specification S, G25.SMV smv, G25.FloatType FT, String[] valueStr, int nbTabs, bool writeZeros)  {
            StringBuilder SB = new StringBuilder();
            string smvName = FT.GetMangledName(S, smv.Name);

            string STATIC_MEMBER_ACCESS = (S.OutputCpp()) ? "::" : ".";

            SB.Append('\t', nbTabs);
            SB.Append("return ");
            if (S.OutputCSharpOrJava())
                SB.Append("new ");
            SB.Append(smvName);
            SB.Append("(");
            if (valueStr.Length > 0)
            {
                SB.AppendLine(smvName + STATIC_MEMBER_ACCESS + SmvUtil.GetCoordinateOrderConstant(S, smv) + ",");
            }
            for (int i = 0; i < valueStr.Length; i++) {
                SB.Append('\t', nbTabs+2);
                SB.Append(valueStr[i]);
                if (i < (valueStr.Length - 1)) SB.Append(",");
                SB.AppendLine(" // " + smv.NonConstBasisBlade(i).ToLangString(S.m_basisVectorNames) );
            }
            SB.Append('\t', nbTabs+1);
            SB.Append(");");

            return SB.ToString();
        }

        /// <summary>
        /// Generates the code to assign each <c>valueStr</c> to its respective  <c>accessStr</c>.
        /// (for example <c>A[0] = B[1]*B[2];</c>)
        /// <c>valueStr</c> and <c>accessStr</c> must have the same length.
        /// </summary>
        /// <param name="S">Used for output language.</param>
        /// <param name="accessStr">Array of strings that allows which can be used to write to specific coordinates
        /// of multivectors. This array must have same length as 'valueStr'. Use GetAccessStr() to generate.</param>
        /// <param name="valueStr">Array of value strings. Must have same length as <c>accessStr</c> Use <c>GetAssignmentStrings()</c> to compute this array.</param>
        /// <param name="nbTabs">How many tabs to put in front of the generated code.</param>
        /// <param name="writeZeros">Some callers want to skip <c>"= 0.0"</c> assignments because they would be redundant. So they set this argument to true.</param>
        /// <returns>String of code for <c>dstName = value;</c></returns>
        public static string GenerateAssignmentCode(Specification S, String[] accessStr, String[] valueStr, int nbTabs, bool writeZeros) 
        {
            StringBuilder SB = new StringBuilder();

            // does the user want to assign zero (this is used by WriteGMVtoSMVcopy())
            if (!writeZeros)
            {
                for (int i = 0; i < valueStr.Length; i++)
                {
                    if ((valueStr[i] != null) && (ValueStringIsZero(S, valueStr[i])))
                        valueStr[i] = null;
                }
            }

            for (int i = 0; i < accessStr.Length; i++)
            {
                if (valueStr[i] == null) continue; // already assigned

                // append tabs
                SB.Append('\t', nbTabs);

                // output dstName.c[0] = 
                SB.Append(accessStr[i] + " = ");

                // see if there any similar assignments
                for (int j = i + 1; j < accessStr.Length; j++)
                {
                    if (valueStr[i] == valueStr[j])
                    {
                        valueStr[j] = null;
                        SB.Append(accessStr[j] + " = ");
                    }
                }
                SB.Append(valueStr[i]);
                SB.AppendLine(";");
            }

            return SB.ToString();
        } // GenerateAssignmentCode()

        /// <summary>
        /// Returns true when 'valueStr' represent a zero value in the target language.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="valueStr"></param>
        /// <returns></returns>
        public static bool ValueStringIsZero(Specification S, string valueStr)
        {
            if (!valueStr.StartsWith("0.0")) return false;
            else if (valueStr.Length < 4) return true;
            else return !Char.IsDigit(valueStr[3]);
        }


        /// <summary>
        /// Generates code to assign <c>value</c> to a variable whose coordinate order is specified by <c>BL</c>.
        /// 
        /// For example, <c>BL</c> could be <c>[e1, e2, e3]</c> and the multivector value <c>[e1 - 2e3]</c>.
        /// Then the returned array would be <c>["1", "0", "-2"]</c>.
        /// 
        /// Parts of <c>value</c> that cannot be assigned to <c>BL</c> are silently ignored. 
        /// 
        /// Possibly, at some point we would like to generate some kind of warning?
        /// </summary>
        /// <param name="S">Specification of algebra (not used yet).</param>
        /// <param name="FT">Floating point type of assigned variable (used for casting strings).</param>
        /// <param name="mustCast">Set to true if a cast to 'FT' must be performed.</param>
        /// <param name="BL">Basis blades of assigned variable.</param>
        /// <param name="value">Multivector value to assign to the list of basis blades. 
        /// Must not contain basis blades inside the symbolic scalars of the RefGA.BasisBlades.</param>
        /// <param name="writeZeros">When true, <c>"0"</c> will be returned when no value should be assigned
        /// to some coordinate. <c>null</c> otherwise.</param>
        /// <returns>An array of strings which tell you what to assign to each coordinate.</returns>
        public static String[] GetAssignmentStrings(Specification S, FloatType FT, bool mustCast, RefGA.BasisBlade[] BL, 
            RefGA.Multivector value, bool writeZeros)
        {
            String[] assignedStr = new String[BL.Length];
            int idx = 0;

            // for each non-const coord, find out what value is (loop through all entries in value)
            foreach (RefGA.BasisBlade B in BL)
            {
                // find same basisblade in 'value'
                foreach (RefGA.BasisBlade C in value.BasisBlades)
                {
                    if (C.bitmap == B.bitmap) // match found: get assignment string
                    {
                        // compute D = inverse(B) . C;
                        RefGA.BasisBlade Bi = (new RefGA.BasisBlade(B.bitmap, 1.0 / B.scale)).Reverse();
                        RefGA.BasisBlade D = RefGA.BasisBlade.scp(Bi, C);

                        if (mustCast) assignedStr[idx] = FT.castStr + "(" + CodeUtil.ScalarToLangString(S, FT, D) + ")";
                        else assignedStr[idx] = CodeUtil.ScalarToLangString(S, FT, D);
                        break;
                    }
                }

                if (writeZeros && (assignedStr[idx] == null)) // has an assignment string been set?
                {
                    // no assignment: simply assign "0"
                    assignedStr[idx] = FT.DoubleToString(S, 0.0);
                }

                idx++;
            }
            return assignedStr;
        } // end of GetAssignmentStrings()

        /// <summary>
        /// Used internally by ScalarToLangString().
        /// 
        /// Emits on term (+ ....) of a blade value. Contains some optimizations
        /// to avoid stuff like <c>+-1.0</c>.
        /// </summary>
        protected static int EmitTerm(G25.Specification S, G25.FloatType FT, Object[] T, int tCnt, StringBuilder symResult)
        {
            // make stuff below a function
            bool plusEmitted = false;
            int pCnt = 0; // number of non-null terms in 'T'
            for (int p = 0; p < T.Length; p++) // for each product ...*
            {
                if (T[p] != null)
                {
                    if ((!plusEmitted) && (tCnt > 0))
                    {
                        symResult.Append("+");
                        plusEmitted = true;
                    }
                    if ((pCnt > 0) && (symResult.Length > 0) && (!((symResult[symResult.Length - 1] == '-') || (symResult[symResult.Length - 1] == '+'))))
                        symResult.Append("*");

                    System.Object O = T[p];
                    if ((O is System.Double) ||
                        (O is System.Single) ||
                        (O is System.Int16) ||
                        (O is System.Int32) ||
                        (O is System.Int64)) // etc . . . (all number types)
                    {
                        double val = (double)O;
                        if ((val == -1.0) && (p == 0) && (T.Length > 1))
                        { // when multiplying with -1.0, output only a '-', IF the term is the first (p==0) and more terms follow (T.Length > 1)
                            if (symResult.Length > 0)
                            {// when val = -1, output code which is better for humans, instead of -1.0 * ... 
                                if (symResult[symResult.Length - 1] == '+') // change '+' to '-'
                                    symResult[symResult.Length - 1] = '-';
                                else symResult.Append("-");
                            }
                            else symResult.Append("-");
                        }
                        else if ((val == 1.0) && (p == 0) && (T.Length > 1))
                        { // when multiplying with 1.0, output nothing IF the term is the first (p==0) and more terms follow (T.Length > 1)
                            // do nothing
                        }
                        else symResult.Append(FT.DoubleToString(S, (double)O));

                    }
                    else if (O is UnaryScalarOp)
                    {
                        UnaryScalarOp USO = (UnaryScalarOp)O;
                        symResult.Append(UnaryScalarOpToLangString(S, FT, USO));
                    }
                    else if (O is BinaryScalarOp)
                    {
                        BinaryScalarOp BSO = (BinaryScalarOp)O;
                        symResult.Append(BinaryScalarOpToLangString(S, FT, BSO));
                    }
                    else if (O is RefGA.BasisBlade)
                    {
                        symResult.Append(ScalarToLangString(S, FT, (RefGA.BasisBlade)O));
                    }
                    else if (O is RefGA.Multivector)
                    {
                        RefGA.Multivector mv = (RefGA.Multivector)O;
                        StringBuilder mvSB = new StringBuilder();
                        mvSB.Append("(");
                        bool first = true;
                        foreach (RefGA.BasisBlade B in mv.BasisBlades)
                        {
                            if (!first) mvSB.Append(" + ");
                            first = false;
                            mvSB.Append(ScalarToLangString(S, FT, B));
                        }

                        mvSB.Append(")");

                        symResult.Append(mvSB);
                    }
                    else
                    {
                        symResult.Append(O.ToString());
                    }

                    pCnt++;
                }
            }
            return pCnt;
        }

        /// <summary>
        /// Used internally by ScalarToLangString().
        /// </summary>
        /// <returns>true when O is a RefGA.Symbolic.ScalarOp and the operations is RefGA.Symbolic.ScalarOp.INVERSE.</returns>
        protected static bool IsScalarOpInverse(Object O)
        {
            return ((O is RefGA.Symbolic.UnaryScalarOp) &&
                    ((O as RefGA.Symbolic.UnaryScalarOp).opName == RefGA.Symbolic.UnaryScalarOp.INVERSE));
        }

        /// <summary>
        /// Used internally.
        /// 
        /// Finds all RefGA.Symbolic.ScalarOp.Inverse and collects them into a new Object[] which is returned.
        /// If there is only one inverse in 'T' and it is the only entry, then it is left as is.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        protected static Object[] IsolateInverses(Object[] T)
        {
            // count number of inverses, other terms
            int nbInvTerms = 0;
            int nbTerms = 0;
            for (int i = 0; i < T.Length; i++)
            {
                if (T[i] == null) continue;
                else if (IsScalarOpInverse(T[i])) nbInvTerms++;
                else nbTerms++;
            }
            // if no inverses, or only one inverse and no other terms, return untouched:
            if ((nbInvTerms == 0) || ((nbTerms == 0) && (nbInvTerms == 1)))
                return null;
            else
            { // separate the inverses from the rest; return inverses in 'TI'
                Object[] TI = new Object[nbInvTerms];
                int tiIdx = 0;
                for (int i = 0; i < T.Length; i++)
                {
                    if (IsScalarOpInverse(T[i]))
                    { // strip the scalarop inverse part, and put T[i] in TI[i]
                        TI[tiIdx++] = (T[i] as RefGA.Symbolic.UnaryScalarOp).value;
                        T[i] = null;
                    }
                }
                return TI;
            }
        }


        /// <summary>
        /// Converts a scalar basis blade (with optional symbolic part) to a string in the output language of <c>S</c>.
        /// 
        /// Some effort is made to output code which is close to that a human would write.
        /// </summary>
        /// <param name="S">Specification of algebra, used for output language, basis vector names, etc.</param>
        /// <param name="FT">Floating point type of output.</param>
        /// <param name="B">The basis blade.</param>
        /// <returns>String code representation of 'B'.</returns>
        public static string ScalarToLangString(G25.Specification S, G25.FloatType FT, RefGA.BasisBlade B)
        {
            string symScaleStr = "";
            { // convert symbolic part
                if (B.symScale != null)
                {
                    // symbolic scalar string goes in  symResult
                    System.Text.StringBuilder symResult = new System.Text.StringBuilder();
                    int tCnt = 0;
                    for (int t = 0; t < B.symScale.Length; t++) // for each term ...*...*...+
                    {
                        Object[] T = (Object[])B.symScale[t].Clone(); // clone 'T' because IsolateInverses() might alter it!
                        if (T != null) // if term is not null
                        {
                            // first find all scalarop inverses
                            // turn those into a new term, but without inverses?
                            Object[] TI = IsolateInverses(T);

                            // emit 'T'
                            int pCnt = EmitTerm(S, FT, T, tCnt, symResult);

                            if (TI != null)  // do we have to divide by a number of terms?
                            {
                                // emit '/(TI)'
                                symResult.Append("/(");
                                EmitTerm(S, FT, TI, 0, symResult);
                                symResult.Append(")");
                            }

                            if (pCnt > 0) tCnt++; // only increment number of terms when T was not empty
                        }
                    }

                    if (tCnt > 0)
                        symScaleStr = ((tCnt > 1) ? "(" : "") + symResult + ((tCnt > 1) ? ")" : "");
                }
            } // end of symbolic part

            // special cases when numerical scale is exactly +- 1
            if (B.scale == 1.0)
            {
                if (symScaleStr.Length > 0) return symScaleStr;
                else return FT.DoubleToString(S, 1.0);
            }
            else if (B.scale == -1.0)
            {
                if (symScaleStr.Length > 0) return "-" + symScaleStr;
                else return FT.DoubleToString(S, -1.0);
            }
            else
            {
                // get numerical part 
                string numScaleStr = FT.DoubleToString(S, B.scale);

                // merge symbolic and numerical
                if (symScaleStr.Length > 0)
                    numScaleStr = numScaleStr + "*" + symScaleStr;

                // done
                return numScaleStr;
            }
        } // end of function ScalarToLangString


        /// <summary>
        /// Converts scalar part of 'value' to ouput language dependent string.
        /// </summary>
        private static string ScalarOpValueToLangString(G25.Specification S, G25.FloatType FT, RefGA.Multivector value)
        {
            if (!value.IsScalar()) throw new Exception("G25.CG.Shared.BasisBlade.ScalarOpValueToLangString(): value should be scalar, found: " + value.ToString(S.m_basisVectorNames));
            if (value.IsZero()) return ScalarToLangString(S, FT, RefGA.BasisBlade.ZERO);
            else return ScalarToLangString(S, FT, value.BasisBlades[0]);
        }
        

        /// <summary>
        /// Converts a <c>RefGA.Symbolic.UnaryScalarOp</c> to code.
        /// 
        /// Handles special cases (such as inversion) and understands floating point
        /// types (e.g., <c>fabsf()</c> is used for floats and <c>fabs()</c> is used for doubles in C.
        /// </summary>
        /// <param name="S">Used for output language.</param>
        /// <param name="FT">Floating point type used.</param>
        /// <param name="Op">The operation to convert to code.</param>
        /// <returns>Code for implementing <c>Op</c>c>.</returns>
        public static string UnaryScalarOpToLangString(G25.Specification S, G25.FloatType FT, RefGA.Symbolic.UnaryScalarOp Op)
        {
            string valueStr = ScalarOpValueToLangString(S, FT, Op.value);
            /*{
                if (!Op.value.IsScalar()) throw new Exception("G25.CG.Shared.BasisBlade.ScalarOpToLangString(): value should be scalar, found: " + Op.value.ToString(S.m_basisVectorNames));
                if (Op.value.IsZero()) valueStr = ScalarToLangString(S, FT, RefGA.BasisBlade.ZERO);
                else valueStr = ScalarToLangString(S, FT, Op.value.BasisBlades[0]);
            }*/

            if (Op.opName == UnaryScalarOp.INVERSE)
            {
                if (FT.type == "float") return "1.0f / (" + valueStr + ")";
                else if (FT.type == "double") return "1.0 / (" + valueStr + ")";
                else return FT.castStr + "1.0 / (" + valueStr + ")";
            }
            else
            {
                return OpNameToLangString(S, FT, Op.opName) + "(" + valueStr + ")";
            }
        } // end of function ScalarOpToLangString()

        /// <summary>
        /// Converts a <c>RefGA.Symbolic.BinaryScalarOpToLangString</c> to code.
        /// 
        /// Handles special cases (such as inversion) and understands floating point
        /// types (e.g., <c>fabsf()</c> is used for floats and <c>fabs()</c> is used for doubles in C.
        /// </summary>
        /// <param name="S">Used for output language.</param>
        /// <param name="FT">Floating point type used.</param>
        /// <param name="Op">The operation to convert to code.</param>
        /// <returns>Code for implementing <c>Op</c>c>.</returns>
        public static string BinaryScalarOpToLangString(G25.Specification S, G25.FloatType FT, RefGA.Symbolic.BinaryScalarOp Op)
        {
            string value1Str = ScalarOpValueToLangString(S, FT, Op.value1);
            string value2Str = ScalarOpValueToLangString(S, FT, Op.value2);
            
            return OpNameToLangString(S, FT, Op.opName) + "(" + value2Str + ", " + value1Str + ")";
        } // end of function BinaryScalarOpToLangString()

        public static string OpNameToLangString(G25.Specification S, G25.FloatType FT, string opName)
        {
            switch (S.m_outputLanguage)
            {
                case OUTPUT_LANGUAGE.C:
                    if (FT.type == "float") return m_floatOpsC[opName];
                    else return m_doubleOpsC[opName];
                case OUTPUT_LANGUAGE.CPP:
                    if (FT.type == "float") return m_floatOpsCpp[opName];
                    else return m_doubleOpsCpp[opName];
                case OUTPUT_LANGUAGE.CSHARP:
                    if (FT.type == "float") return m_floatOpsCSharp[opName];
                    else return m_doubleOpsCSharp[opName];
                case OUTPUT_LANGUAGE.JAVA:
                    if (FT.type == "float") return m_floatOpsJava[opName];
                    else return m_doubleOpsJava[opName];
                default:
                    throw new Exception("G25.CG.Shared.BasisBlade.ScalarOpToLangString(): todo: language " + S.GetOutputLanguageString());
            }
        }

        /// <summary>
        /// Called by ScalarOpToLangString() to initialize some lookup tables.
        /// </summary>
        protected static void InitOps()
        {
            if (m_doubleOpsC != null) return; // check if init already done

            { // scalar math operations for C, double precision
                m_doubleOpsC = new System.Collections.Generic.Dictionary<String, String>();
                m_doubleOpsC[UnaryScalarOp.SQRT] = "sqrt";
                m_doubleOpsC[UnaryScalarOp.EXP] = "exp";
                m_doubleOpsC[UnaryScalarOp.LOG] = "log";
                m_doubleOpsC[UnaryScalarOp.SIN] = "sin";
                m_doubleOpsC[UnaryScalarOp.COS] = "cos";
                m_doubleOpsC[UnaryScalarOp.TAN] = "tan";
                m_doubleOpsC[UnaryScalarOp.SINH] = "sinh";
                m_doubleOpsC[UnaryScalarOp.COSH] = "cosh";
                m_doubleOpsC[UnaryScalarOp.TANH] = "tanh";
                m_doubleOpsC[UnaryScalarOp.ABS] = "fabs";
                m_doubleOpsC[BinaryScalarOp.ATAN2] = "atan2";
            }

            { // scalar math operations for C, single precision (C has not single precision math functions)
                m_floatOpsC = new System.Collections.Generic.Dictionary<String, String>();
                m_floatOpsC[UnaryScalarOp.SQRT] = "(float)sqrt";
                m_floatOpsC[UnaryScalarOp.EXP] = "(float)exp";
                m_floatOpsC[UnaryScalarOp.LOG] = "(float)log";
                m_floatOpsC[UnaryScalarOp.SIN] = "(float)sin";
                m_floatOpsC[UnaryScalarOp.COS] = "(float)cos";
                m_floatOpsC[UnaryScalarOp.TAN] = "(float)tan";
                m_floatOpsC[UnaryScalarOp.SINH] = "(float)sinh";
                m_floatOpsC[UnaryScalarOp.COSH] = "(float)cosh";
                m_floatOpsC[UnaryScalarOp.TANH] = "(float)tanh";
                m_floatOpsC[UnaryScalarOp.ABS] = "(float)fabs";
                m_floatOpsC[BinaryScalarOp.ATAN2] = "(float)atan2";
            }

            { // scalar math operations for C++, double precision
                m_doubleOpsCpp = new System.Collections.Generic.Dictionary<String, String>();
                m_doubleOpsCpp[UnaryScalarOp.SQRT] = "::sqrt";
                m_doubleOpsCpp[UnaryScalarOp.EXP] = "::exp";
                m_doubleOpsCpp[UnaryScalarOp.LOG] = "::log";
                m_doubleOpsCpp[UnaryScalarOp.SIN] = "::sin";
                m_doubleOpsCpp[UnaryScalarOp.COS] = "::cos";
                m_doubleOpsCpp[UnaryScalarOp.TAN] = "::tan";
                m_doubleOpsCpp[UnaryScalarOp.SINH] = "::sinh";
                m_doubleOpsCpp[UnaryScalarOp.COSH] = "::cosh";
                m_doubleOpsCpp[UnaryScalarOp.TANH] = "::tanh";
                m_doubleOpsCpp[UnaryScalarOp.ABS] = "::fabs";
                m_doubleOpsCpp[BinaryScalarOp.ATAN2] = "::atan2";
            }

            { // scalar math operations for C++, single precision
                m_floatOpsCpp = new System.Collections.Generic.Dictionary<String, String>();
                m_floatOpsCpp[UnaryScalarOp.SQRT] = "::sqrtf";
                m_floatOpsCpp[UnaryScalarOp.EXP] = "::expf";
                m_floatOpsCpp[UnaryScalarOp.LOG] = "::logf";
                m_floatOpsCpp[UnaryScalarOp.SIN] = "::sinf";
                m_floatOpsCpp[UnaryScalarOp.COS] = "::cosf";
                m_floatOpsCpp[UnaryScalarOp.TAN] = "::tanf";
                m_floatOpsCpp[UnaryScalarOp.SINH] = "::sinhf";
                m_floatOpsCpp[UnaryScalarOp.COSH] = "::coshf";
                m_floatOpsCpp[UnaryScalarOp.TANH] = "::tanhf";
                m_floatOpsCpp[UnaryScalarOp.ABS] = "::fabsf";
                m_floatOpsCpp[BinaryScalarOp.ATAN2] = "::atan2f";
            }

            { // scalar math operations for C#, double precision
                m_doubleOpsCSharp = new System.Collections.Generic.Dictionary<String, String>();
                m_doubleOpsCSharp[UnaryScalarOp.SQRT] = "Math.Sqrt";
                m_doubleOpsCSharp[UnaryScalarOp.EXP] = "Math.Exp";
                m_doubleOpsCSharp[UnaryScalarOp.LOG] = "Math.Log";
                m_doubleOpsCSharp[UnaryScalarOp.SIN] = "Math.Sin";
                m_doubleOpsCSharp[UnaryScalarOp.COS] = "Math.Cos";
                m_doubleOpsCSharp[UnaryScalarOp.TAN] = "Math.Tan";
                m_doubleOpsCSharp[UnaryScalarOp.SINH] = "Math.Sinh";
                m_doubleOpsCSharp[UnaryScalarOp.COSH] = "Math.Cosh";
                m_doubleOpsCSharp[UnaryScalarOp.TANH] = "Math.Tanh";
                m_doubleOpsCSharp[UnaryScalarOp.ABS] = "Math.Abs";
                m_doubleOpsCSharp[BinaryScalarOp.ATAN2] = "Math.Atan2";
            }

            { // scalar math operations for C#, single precision
                m_floatOpsCSharp = new System.Collections.Generic.Dictionary<String, String>();
                m_floatOpsCSharp[UnaryScalarOp.SQRT] = "(float)Math.Sqrt";
                m_floatOpsCSharp[UnaryScalarOp.EXP] = "(float)Math.Exp";
                m_floatOpsCSharp[UnaryScalarOp.LOG] = "(float)Math.Log";
                m_floatOpsCSharp[UnaryScalarOp.SIN] = "(float)Math.Sin";
                m_floatOpsCSharp[UnaryScalarOp.COS] = "(float)Math.Cos";
                m_floatOpsCSharp[UnaryScalarOp.TAN] = "(float)Math.Tan";
                m_floatOpsCSharp[UnaryScalarOp.SINH] = "(float)Math.Sinh";
                m_floatOpsCSharp[UnaryScalarOp.COSH] = "(float)Math.Cosh";
                m_floatOpsCSharp[UnaryScalarOp.TANH] = "(float)Math.Tanh";
                m_floatOpsCSharp[UnaryScalarOp.ABS] = "Math.Abs";
                m_floatOpsCSharp[BinaryScalarOp.ATAN2] = "(float)Math.Atan2";
            }

            { // scalar math operations for Java, double precision
                m_doubleOpsJava = new System.Collections.Generic.Dictionary<String, String>();
                m_doubleOpsJava[UnaryScalarOp.SQRT] = "Math.sqrt";
                m_doubleOpsJava[UnaryScalarOp.EXP] = "Math.exp";
                m_doubleOpsJava[UnaryScalarOp.LOG] = "Math.log";
                m_doubleOpsJava[UnaryScalarOp.SIN] = "Math.sin";
                m_doubleOpsJava[UnaryScalarOp.COS] = "Math.cos";
                m_doubleOpsJava[UnaryScalarOp.TAN] = "Math.tan";
                m_doubleOpsJava[UnaryScalarOp.SINH] = "Math.sinh";
                m_doubleOpsJava[UnaryScalarOp.COSH] = "Math.cosh";
                m_doubleOpsJava[UnaryScalarOp.TANH] = "Math.tanh";
                m_doubleOpsJava[UnaryScalarOp.ABS] = "Math.abs";
                m_doubleOpsJava[BinaryScalarOp.ATAN2] = "Math.Atan";
            }

            { // scalar math operations for Java, single precision
                m_floatOpsJava = new System.Collections.Generic.Dictionary<String, String>();
                m_floatOpsJava[UnaryScalarOp.SQRT] = "(float)Math.sqrt";
                m_floatOpsJava[UnaryScalarOp.EXP] = "(float)Math.exp";
                m_floatOpsJava[UnaryScalarOp.LOG] = "(float)Math.log";
                m_floatOpsJava[UnaryScalarOp.SIN] = "(float)Math.sin";
                m_floatOpsJava[UnaryScalarOp.COS] = "(float)Math.cos";
                m_floatOpsJava[UnaryScalarOp.TAN] = "(float)Math.tan";
                m_floatOpsJava[UnaryScalarOp.SINH] = "(float)Math.sinh";
                m_floatOpsJava[UnaryScalarOp.COSH] = "(float)Math.cosh";
                m_floatOpsJava[UnaryScalarOp.TANH] = "(float)Math.tanh";
                m_floatOpsJava[UnaryScalarOp.ABS] = "Math.abs";
                m_floatOpsJava[BinaryScalarOp.ATAN2] = "(float)Math.atan2";
            }

        }


        /// <summary>A dictionary from RefGA.Symbolic.ScalarOp op-names to C standard library function names (for doubles).</summary>
        private static System.Collections.Generic.Dictionary<string, string> m_doubleOpsC = null;
        /// <summary>A dictionary from RefGA.Symbolic.ScalarOp op-names to C standard library function names (for floats).</summary>
        private static System.Collections.Generic.Dictionary<string, string> m_floatOpsC = null;

        /// <summary>A dictionary from RefGA.Symbolic.ScalarOp op-names to C++ standard library function names (for doubles).</summary>
        private static System.Collections.Generic.Dictionary<string, string> m_doubleOpsCpp = null;
        /// <summary>A dictionary from RefGA.Symbolic.ScalarOp op-names to C++ standard library function names (for floats).</summary>
        private static System.Collections.Generic.Dictionary<string, string> m_floatOpsCpp = null;

        /// <summary>A dictionary from RefGA.Symbolic.ScalarOp op-names to C# function names (for doubles).</summary>
        private static System.Collections.Generic.Dictionary<string, string> m_doubleOpsCSharp = null;
        /// <summary>A dictionary from RefGA.Symbolic.ScalarOp op-names to C# function names (for floats).</summary>
        private static System.Collections.Generic.Dictionary<string, string> m_floatOpsCSharp = null;

        /// <summary>A dictionary from RefGA.Symbolic.ScalarOp op-names to Java function names (for doubles).</summary>
        private static System.Collections.Generic.Dictionary<string, string> m_doubleOpsJava = null;
        /// <summary>A dictionary from RefGA.Symbolic.ScalarOp op-names to Java function names (for floats).</summary>
        private static System.Collections.Generic.Dictionary<string, string> m_floatOpsJava = null;

    } // end of class CodeUtil
} // end of namepace G25.CG.Shared
