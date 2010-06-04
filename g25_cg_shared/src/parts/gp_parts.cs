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

namespace G25.CG.Shared
{

    /// <summary>
    /// Contains functions for general multivector geometric product, split up in parts.
    /// Each part evaluates the geometric product of two groups of the general multivector type.
    /// But each part only computes the result for one specific group.  
    /// Mathematically speaking: <c>group_i X group_j -> group_k</c>
    /// 
    /// All these parts of the geometric product can be combined to compute the whole geometric product,
    /// outer product, various inner product, or the commutator product.
    /// 
    /// To use, this class, first call <c>WriteGmvGpParts()</c> to write the parts to the output file.
    /// Then use <c>GetGPcode()</c> to get the code to actually compute a product of two
    /// general multivectors. <c>GetGPcode()</c> has an argument <c>T</c> which determines 
    /// the <c>ProductTypes</c> (e.g., geometric, outer, etc).
    /// 
    /// Using the same geometric product parts code, functions can also be written to:
    ///   - Compute the norm (squared) of general multivector. Use <c>GetNormCode()</c>
    ///     to generate code for that.
    ///   - Apply versors to general multivector. Use <c>GetVersorApplicationCode()</c> for that.
    /// 
    /// The class is output language aware (currently supported languages: C).
    /// 
    /// With respect to the actual code, there are two code generation modes, as determined
    /// by <c>G25.Specification.m_gmvCodeGeneration</c>. One mode is <c>EXPAND</c>, where all
    /// code is fully expanded. This leads to huge source files for large dimensional spaces.
    /// The other mode is <c>RUNTIME</c>. This mode computes tables at runtime, and uses
    /// these table to compute the geometric products at runtime.
    /// </summary>
    public class GPparts
    {
        /// <summary>
        /// Types of product that can be generates by GetGPcode(), based on geometric product parts code.
        /// Values are <c>GEOMETRIC_PRODUCT</c>, 
        /// <c>OUTER_PRODUCT</c>, 
        /// <c>LEFT_CONTRACTION</c>, 
        /// <c>RIGHT_CONTRACTION</c>, 
        /// <c>HESTENES_INNER_PRODUCT</c>, 
        /// <c>MODIFIED_HESTENES_INNER_PRODUCT</c>, 
        /// <c>SCALAR_PRODUCT</c>, 
        /// <c>COMMUTATOR_PRODUCT</c>, 
        /// </summary>
        public enum ProductTypes : int
        {
            GEOMETRIC_PRODUCT = 1,
            OUTER_PRODUCT = 2,
            LEFT_CONTRACTION = 3,
            RIGHT_CONTRACTION = 4,
            HESTENES_INNER_PRODUCT = 5,
            MODIFIED_HESTENES_INNER_PRODUCT = 6,
            SCALAR_PRODUCT = 7,
            COMMUTATOR_PRODUCT = 8
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ApplyVersorTypes : int
        {
            REVERSE = 1, ///< to get inverse = use reverse
            INVERSE = 2, ///<  to get inverse, use versor inverse
            EXPLICIT_INVERSE = 3 ///< the inverse is explicitly passed as the third argument
        }
        

        /// <summary>
        /// Returns the name of a partial geometric product function. 
        /// 
        /// Does not check whether this
        /// combination of groups actually produces any results (i.e., it may be that <c>g1 X g2 -> g3 = 0</c>,
        /// but in that case a name will still be returned.
        /// </summary>
        /// <param name="FT">Float type (used to mangle the name).</param>
        /// <param name="M">The metric (used for the name).</param>
        /// <param name="g1">Grade/group of argument 1.</param>
        /// <param name="g2">Grade/group of argument 2.</param>
        /// <param name="g3">Grade/group of result.</param>
        /// <returns>name of a partial geometric product function.</returns>
        public static string GetGPpartFunctionName(Specification S, G25.FloatType FT, G25.Metric M, int g1, int g2, int g3)
        {
            return FT.GetMangledName(S, "gp") + "_" + M.m_name + "_" + g1 + "_" + g2 + "_" + g3;
        }


        /// <param name="S">Specification.</param>
        /// <param name="FT">Float type</param>
        /// <param name="fromOutsideRuntimeNamespace">When calling the function from outside the runtime namespace, set this param to true.</param>
        /// <returns>The name of the function used to compute the geometric product in real-time (depends only on the floating point type)</returns>
        public static string GetRuntimeComputeGpFuncName(G25.Specification S, G25.FloatType FT, bool fromOutsideRuntimeNamespace)
        {
            if (S.OutputCppOrC())
            {
                string prefix = "";
                if (fromOutsideRuntimeNamespace && (S.OutputCpp()))
                    prefix = Main.RUNTIME_NAMESPACE + "::";
                return prefix + S.m_namespace + "_runtimeComputeGp_" + FT.type;
            }
            else if (S.OutputCSharp())
            {
                return "RuntimeComputeGp_" + FT.type;
            }
            else
            {
                return "runtimeComputeGp_" + FT.type;
            }
        }

        /// <param name="S">Specification.</param>
        /// <param name="M">Metric</param>
        /// <param name="g1">Input grade/group 1.</param>
        /// <param name="g2">Input grade/group 2.</param>
        /// <param name="gd">Destination grade/group.</param>
        /// <param name="fromOutsideRuntimeNamespace">When calling the function from outside the runtime namespace, set this param to true.</param>
        /// <returns>The name of the table which is used to compute <c>gd = g1 g2</c> using metric <c>M</c></returns>
        public static string GetRuntimeGpTableName(G25.Specification S, G25.Metric M, int g1, int g2, int gd, bool fromOutsideRuntimeNamespace)
        {
            if (S.OutputCppOrC())
            {
                string prefix = "";
                if (fromOutsideRuntimeNamespace && (S.OutputCpp()))
                    prefix = Main.RUNTIME_NAMESPACE + "::";
                return prefix + S.m_namespace + "_runtimeGpProductTable_" + M.m_name + "_" + g1 + "_" + g2 + "_" + gd;
            }
            else
            {
                return "runtimeGpProductTable_" + M.m_name + "_" + g1 + "_" + g2 + "_" + gd;
            }
        }


        /// <summary>
        /// Writes pieces of code to <c>cgd</c> which compute the geometric product of general multivectors,
        /// on a group by group basis. The function is output language aware.
        /// </summary>
        /// <param name="S">Specification (used for output language, GMV).</param>
        /// <param name="cgd">Result goes here, and <c>cgd.m_gmvGPpartFuncNames</c> is set.</param>
        public static void WriteGmvGpParts(Specification S, CGdata cgd)
        {
            G25.GMV gmv = S.m_GMV;

            string name1 = "A";
            string name2 = "B";
            string name3 = "C";
            bool ptr = true;
            int allGroups = -1;
            bool mustCast = false;
            int nbBaseTabs = (S.OutputCSharpOrJava()) ? 1 : 0;
            int nbCodeTabs = 1 + nbBaseTabs;
            bool writeZeros = false;

            // get two symbolic multivectors (with different symbolic names):
            RefGA.Multivector[] M1 = null, M2 = null;
            if (S.m_gmvCodeGeneration == GMV_CODE.EXPAND)
            {
                M1 = G25.CG.Shared.Symbolic.GMVtoSymbolicMultivector(S, gmv, name1, ptr, allGroups);
                M2 = G25.CG.Shared.Symbolic.GMVtoSymbolicMultivector(S, gmv, name2, ptr, allGroups);
            }

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                bool fromOutsideRuntimeNamespace = true;
                string runtimeComputeGpFuncName = GetRuntimeComputeGpFuncName(S, FT, fromOutsideRuntimeNamespace);

                int metricId = 0;
                foreach (G25.Metric M in S.m_metric)
                {
                    // map from code fragment to name of function
                    Dictionary<string, string> generatedCode = new Dictionary<string, string>();

                    for (int g1 = 0; g1 < gmv.NbGroups; g1++)
                    {
                        for (int g2 = 0; g2 < gmv.NbGroups; g2++)
                        {
                            RefGA.Multivector M3 = null;
                            if (S.m_gmvCodeGeneration == GMV_CODE.EXPAND)
                            {
                                M3 = RefGA.Multivector.gp(M1[g1], M2[g2], M.m_metric);
                                // round value if required by metric
                                if (M.m_round) M3 = M3.Round(1e-14);
                            }

                            for (int gd = 0; gd < gmv.NbGroups; gd++)
                            {
                                // get function name
                                string funcName = GetGPpartFunctionName(S, FT, M, g1, g2, gd);

                                // get assignment code
                                string code = ""; // empty string means 'no code for this combo of g1, g2, gd and metric'.

                                if (S.m_gmvCodeGeneration == GMV_CODE.EXPAND) 
                                { // code for full expansion
                                    int dstBaseIdx = 0;
                                    code = G25.CG.Shared.CodeUtil.GenerateGMVassignmentCode(S, FT, mustCast, gmv, name3, gd, dstBaseIdx, M3, nbCodeTabs, writeZeros);
                                    // replace '=' with '+='
                                    code = code.Replace("=", "+=");
                                }
                                else if (S.m_gmvCodeGeneration == GMV_CODE.RUNTIME)
                                { // code for runtime geometric product
                                    string EMP = (S.OutputCppOrC()) ? "&" : "";

                                    if (!S.m_GMV.IsZeroGP(g1, g2, gd))
                                    {
                                        fromOutsideRuntimeNamespace = true;
                                        string runtimeGpTableName = GetRuntimeGpTableName(S, M, g1, g2, gd, fromOutsideRuntimeNamespace);
                                        if (S.OutputCSharpOrJava())
                                        {
                                            string initFunc = S.OutputJava() ? "initRuntimeGpTable" : "InitRuntimeGpTable";

                                            code = "\t\tif(" + runtimeGpTableName + " == null) " + 
                                              runtimeGpTableName + " = " + initFunc + "(" + metricId + ", " + g1 + ", " + g2 + ", " + gd + ");\n\t";
                                        }
                                        code += "\t" + runtimeComputeGpFuncName + "(" + name1 + ", " + name2 + ", " + name3 +
                                            ", " + EMP + runtimeGpTableName + ", " + metricId + ", " + g1 + ", " + g2 + ", " + gd + ");\n";
                                    }
                                }

                                // set the function names of parts of the geometric product
                                cgd.m_gmvGPpartFuncNames[new Tuple<string, string, string>(FT.type, M.m_name, funcName)] = (code.Length > 0);

                                if (code.Length > 0)
                                {
                                    // check if code was already generated, and, if so, reuse it
                                    if ((S.m_gmvCodeGeneration == GMV_CODE.EXPAND) && generatedCode.ContainsKey(code))
                                    {
                                        // ready generated: call that function
                                        code = new string('\t', nbCodeTabs) + generatedCode[code] + "(" + name1 + ", " + name2 + ", " + name3 + ");\n";
                                    }
                                    else
                                    {
                                        // not generated yet: remember code -> function
                                        generatedCode[code] = funcName;
                                    }

                                    string comment = "Computes the partial geometric product of two multivectors (group " + g1 + "  x  group " + g2 + " -> group " + gd + ")";
                                    string funcDecl;

                                    if (S.OutputCppOrC())
                                    {
                                        funcDecl = "void " + funcName + "(const " + FT.type + " *" + name1 + ", const " + FT.type + " *" + name2 + ", " + FT.type + " *" + name3 + ")";

                                        // write comment
                                        int nbCommentTabs = nbBaseTabs;
                                        new Comment(comment).Write(cgd.m_declSB, S, nbCommentTabs);
                                        // emit decl
                                        cgd.m_declSB.Append(funcDecl);
                                        cgd.m_declSB.AppendLine(";");
                                    }
                                    else
                                    {
                                        string ACCESS = (S.OutputJava()) ? "protected final static " : "protected internal static ";
                                        funcDecl = ACCESS + "void " + funcName + "(" + FT.type + "[] " + name1 + ", " + FT.type + "[] " + name2 + ", " + FT.type + "[] " + name3 + ")";

                                        // write comment
                                        int nbCommentTabs = nbBaseTabs;
                                        new Comment(comment).Write(cgd.m_defSB, S, nbCommentTabs);
                                    }

                                    // emit def
                                    cgd.m_defSB.Append('\t', nbBaseTabs);
                                    cgd.m_defSB.Append(funcDecl);
                                    cgd.m_defSB.AppendLine(" {");
                                    cgd.m_defSB.Append(code);
                                    cgd.m_defSB.Append('\t', nbBaseTabs);
                                    cgd.m_defSB.AppendLine("}");
                                } // end of 'if code not empty'
                            } // end of loop over the grade of 'C'
                        } // end of loop over the grade of 'B'
                    } // end of loop over the grade of 'A'
                    metricId++;
                } // end of loop over all metrics
            } // end of loop over all float types
        } // end of function WriteGmvGpParts()


        /// <summary>
        /// Generates 'expand general multivector code'. This code makes the general multivector
        /// available as a list of pointers to grade parts.
        /// 
        /// Used internally by <c>WriteGmvGpParts()</c>,
        /// but also by e.g. <c>DualParts.GetDualCode()</c>.
        /// </summary>
        /// <param name="S">Used for output language and dimension of space.</param>
        /// <param name="cgd">Currently not used.</param>
        /// <param name="FT">Code for this float type is generated.</param>
        /// <param name="FAI">Used to determine whether an argument is scalar or general multivector.</param>
        /// <param name="resultIsScalar">The expand code also contains code to allocate coordinates of the result.
        /// When a scalar is going to be returned, only '1' needs to be allocated, otherwise <c>2^S.m_dimension</c>
        /// coordinates should be allocated.</param>
        /// <param name="initResultToZero">Whether to set the result coordinates 'c' to 0.</param>
        /// <returns></returns>
        public static string GetExpandCode(Specification S, G25.CG.Shared.CGdata cgd, 
            G25.FloatType FT, G25.CG.Shared.FuncArgInfo[] FAI, bool resultIsScalar, bool initResultToZero)
        {
            if (S.OutputCppOrC())
                return GetExpandCodeCppOrC(S, cgd, FT, FAI, resultIsScalar, initResultToZero);
            else return GetExpandCodeCSharpOrJava(S, cgd, FT, FAI, resultIsScalar, initResultToZero);
        }

        private static string GetExpandCodeCppOrC(Specification S, G25.CG.Shared.CGdata cgd, 
            G25.FloatType FT, G25.CG.Shared.FuncArgInfo[] FAI, bool resultIsScalar, bool initResultToZero)
        {
            StringBuilder SB = new StringBuilder();

            // result coordinates code:
            int nbCoords = (resultIsScalar) ? 1 : (1 << S.m_dimension);
            SB.AppendLine(FT.type + " c[" + nbCoords + "];");

            // expand code
            if (FAI != null)
            {
                for (int i = 0; i < FAI.Length; i++)
                {
                    if (FAI[i].IsScalar())
                    {
                        // 'expand' scalar
                        SB.AppendLine("const " + FAI[i].FloatType.type + "* _" + FAI[i].Name + "[1] = {&" + FAI[i].Name + "};");
                    }
                    else
                    {
                        // expand general multivector
//                                SB.AppendLine("const " + FAI[i].FloatType.type + "* _" + FAI[i].Name + "[" + (S.m_dimension + 1) + "];");
                        SB.AppendLine("const " + FAI[i].FloatType.type + "* _" + FAI[i].Name + "[" + (S.m_GMV.NbGroups) + "];");
                    }
                }
                for (int i = 0; i < FAI.Length; i++)
                {
                    if (!FAI[i].IsScalar())
                    {
                        if (S.OutputC())
                            SB.AppendLine(FT.GetMangledName(S, "expand") + "(_" + FAI[i].Name + ", " + FAI[i].Name + ");");
                        else SB.AppendLine(FAI[i].Name + ".expand(_" + FAI[i].Name + ");");
                    }
                }
            }

            // set coordinates of 'c' to zero
            if (initResultToZero)
            {
                SB.Append(Util.GetSetToZeroCode(S, FT, "c", nbCoords));
            }

            return SB.ToString();
        } // end of GetExpandCodeCppOrC()

        private static string GetExpandCodeCSharpOrJava(Specification S, G25.CG.Shared.CGdata cgd,
            G25.FloatType FT, G25.CG.Shared.FuncArgInfo[] FAI, bool resultIsScalar, bool initResultToZero)
        {
            StringBuilder SB = new StringBuilder();

            // result coordinates code:
            int nbGroups = S.m_GMV.NbGroups;

            // expand code
            if (FAI != null)
            {
                for (int i = 0; i < Math.Min(FAI.Length, 2); i++) // max 2 arguments
                {
                    char name = (char)((int)'a' + i); // always name vars 'a', 'b' 
                    if (FAI[i].IsScalar())
                    {
                        // 'expand' scalar
                        //int nb = 1; // nbGroups
                        //SB.AppendLine(FT.type + "[][] " + FAI[i].Name + "c = new " + FT.type + "[1][]{new " + FT.type + "[" + nb + "]{" + FAI[i].Name + "}};");
                        SB.AppendLine(FT.type + "[][] " + name + "c = new " + FT.type + "[][]{new " + FT.type + "[]{" + FAI[i].Name + "}};");
                    }
                    else
                    {
                        // expand general multivector
                        SB.AppendLine(FT.type + "[][] " + name + "c = " + FAI[i].Name + ".c();");
                    }
                }
            }
            SB.AppendLine(FT.type + "[][] cc = new " + FT.type + "[" + nbGroups + "][];");


            return SB.ToString();
        } // end of GetExpandCodeCSharpOrJava()


        public static string GetCompressCode(Specification S, G25.FloatType FT,
            G25.CG.Shared.FuncArgInfo[] FAI, string resultName, bool resultIsScalar, string GUstr)
        {
            if ((GUstr == null) || (GUstr.Length == 0))
            {
                GUstr = ((1 << S.m_GMV.NbGroups) - 1).ToString();
            }

            StringBuilder SB = new StringBuilder();
            if (resultIsScalar)
            {
                SB.AppendLine("return c[0];");
            }
            else
            {
                string funcName = (S.OutputC())
                    ? FT.GetMangledName(S, "compress")
                    : FT.GetMangledName(S, S.m_GMV.Name) + "_compress";

                if (S.m_outputLanguage != OUTPUT_LANGUAGE.C)
                    SB.Append("return ");

                SB.Append(funcName + "(c, ");
                if (S.OutputC())
                    SB.Append(resultName + "->c, &(" + resultName + "->gu), ");
                SB.AppendLine(FT.DoubleToString(S, 0.0) + ", " + GUstr + ");");
            }
            return SB.ToString();
        }

        /// <summary>
        /// Generates 'compress' code. This code takes an non-compressed multivector (i.e., 
        /// <c>2^S.m_dimension</c> coordinates and compresses it into a regular general
        /// multivector. This code is usually placed at the end of such functions as the geometric product.
        /// </summary>
        /// <param name="S">Used for dimension of space.</param>
        /// <param name="FT">Used to mangle the name of the <c>compress function</c>.</param>
        /// <param name="FAI">Not used currently.</param>
        /// <param name="resultName">Name of variable where compressed code goes.</param>
        /// <param name="resultIsScalar">When a scalar should be returned, the generated code is simply <c>return c[0]</c>.</param>
        /// <returns></returns>
        public static string GetCompressCode(Specification S, G25.FloatType FT, 
            G25.CG.Shared.FuncArgInfo[] FAI, string resultName, bool resultIsScalar)
        {
            string GUstr = null; // use the default (full group usage)
            return GetCompressCode(S, FT, FAI, resultName, resultIsScalar, GUstr);
        }


        /// <summary>
        /// Determines whether the group <c>g3</c> part of the geometric product of group <c>g1</c> and 
        /// group <c>g2</c> is zero for sure. 
        /// 
        /// This is done by checking whether a function is listed in 
        /// <c>cgd.m_gmvGPpartFuncNames</c>, so <c>WriteGmvGpParts()</c> should be called before using this
        /// function.
        /// </summary>
        /// <param name="S">Specification. Used to obtain general multivector type.</param>
        /// <param name="cgd">Used to check if code was generated for this combination of g1 and g2 (<c>m_gmvGPpartFuncNames</c>).</param>
        /// <param name="FT">Float type of function.</param>
        /// <param name="M">Metric used for function. Cannot be null.</param>
        /// <param name="g1">Grade/group of argument 1.</param>
        /// <param name="g2">Grade/group of argument 2.</param>
        /// <param name="g3">Grade/group of result.</param>
        /// <param name="T">Type of product (gp, op, etc)</param>
        /// <returns>true if this the g3 part of gp(g1, g2) is zero for sure.</returns>
        public static bool zero(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, G25.Metric M,  
            int g1, int g2, int g3, ProductTypes T)
        {
            G25.GMV gmv = S.m_GMV;

            // get function name
            String funcName = GetGPpartFunctionName(S, FT, M, g1, g2, g3); 
            
            // check if any code generated:
            Tuple<string, string, string> key = new Tuple<string, string, string>(FT.type, M.m_name, funcName);
            if (!cgd.m_gmvGPpartFuncNames.ContainsKey(key)) return true;
            if (!cgd.m_gmvGPpartFuncNames[key]) return true;
            
            // convert the groups into grades:
            g1 = gmv.Group(g1)[0].Grade();
            g2 = gmv.Group(g2)[0].Grade();
            g3 = gmv.Group(g3)[0].Grade();

            // filter on the product type:
            switch (T)
            {
                case ProductTypes.GEOMETRIC_PRODUCT:
                    return false;
                case ProductTypes.OUTER_PRODUCT:
                    return !((g1 + g2) == g3);
                case ProductTypes.LEFT_CONTRACTION:
                    return !((g1 <= g2) && ((g2-g1) == g3));
                case ProductTypes.RIGHT_CONTRACTION:
                    return !((g1 >= g2) && ((g1 - g2) == g3));
                case ProductTypes.HESTENES_INNER_PRODUCT:
                    if ((g1 == 0) || (g2 == 0)) return true;
                    else return !(Math.Abs(g1 - g2) == g3);
                case ProductTypes.MODIFIED_HESTENES_INNER_PRODUCT:
                    return !(Math.Abs(g1 - g2) == g3);
                case ProductTypes.SCALAR_PRODUCT:
                    return !((g3 == 0) && (g1 == g2));
                case ProductTypes.COMMUTATOR_PRODUCT:
                    { // note: not tested yet
                        for (int s = 0; s >= Math.Min(g1, g2); s++)
                        {
                            int f1 = g1-s;
                            int f2 = g2-s;
                            if ((f1 + f2) != g3) continue; // out of range
                            if (((f1 * f2) & 1) != 0) return false; // if grade*grade == odd, then non-zero
                        }
                        return true;
                    }
            }
            return true;
        } // end of zero()

        /// <summary>
        /// Returns the code for a product. This can be a geometric, outer, inner or commutator product.
        /// The code is composed of calls to functions generated by <c>WriteGmvGpParts()</c>.
        /// 
        /// The returned code is only the body. The function declaration is not included.
        /// </summary>
        /// <param name="S">Specification of algebra (used for output language and to obtain general multivector type).</param>
        /// <param name="cgd">Used for <c>m_gmvGPpartFuncNames</c>.</param>
        /// <param name="FT">Floating point type.</param>
        /// <param name="M">Metric type.</param>
        /// <param name="T">The product (e.g., geometric, outer, etc)</param>
        /// <param name="FAI">Info about function arguments. Used to know whether arguments are general multivectors or scalars.</param>
        /// <param name="resultName">Name of variable where the result goes (in the generated code).</param>
        /// <returns>code for the requested product type.</returns>
        public static string GetGPcode(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, G25.Metric M,
            ProductTypes T,
            G25.CG.Shared.FuncArgInfo[] FAI, string resultName)
        {
            if (S.OutputCppOrC()) 
                return GetGPcodeCppOrC(S, cgd, FT, M, T, FAI, resultName);
            else return GetGPcodeCSharpOrJava(S, cgd, FT, M, T, FAI, resultName);
        }

        private static string GetGPcodeCppOrC(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, G25.Metric M, 
            ProductTypes T,
            G25.CG.Shared.FuncArgInfo[] FAI, string resultName)
        {
            G25.GMV gmv = S.m_GMV;

            StringBuilder SB = new StringBuilder();
            bool resultIsScalar = (T == ProductTypes.SCALAR_PRODUCT);
            bool initResultToZero = true;
            SB.Append(GetExpandCode(S, cgd, FT, FAI, resultIsScalar, initResultToZero));

            // get number of groups, and possible assurances that a group is always present:
            int nbGroups1 = (FAI[0].IsScalar()) ? 1 : gmv.NbGroups;
            int nbGroups2 = (FAI[1].IsScalar()) ? 1 : gmv.NbGroups;
            bool[] GroupAlwaysPresent1 = new bool[nbGroups1];
            bool[] GroupAlwaysPresent2 = new bool[nbGroups2];
            if (FAI[0].IsScalar()) GroupAlwaysPresent1[0] = true;
            if (FAI[1].IsScalar()) GroupAlwaysPresent2[0] = true;

            string agu = (S.OutputC()) ? FAI[0].Name + "->gu" : FAI[0].Name + ".gu()";
            string bgu = (S.OutputC()) ? FAI[1].Name + "->gu" : FAI[1].Name + ".gu()";

            int g1Cond = -1; // grade 1 conditional which is open (-1 = none)
            int g2Cond = -1; // grade 2 conditional which is open (-1 = none)

            for (int g1 = 0; g1 < nbGroups1; g1++)
            {
                for (int g2 = 0; g2 < nbGroups2; g2++)
                {
                    for (int g3 = 0; g3 < gmv.NbGroups; g3++)
                    {
                        if (!zero(S, cgd, FT, M, g1, g2, g3, T))
                        {
                            // close conditionals if required
                            if ((((g1Cond != g1) && (g1Cond >= 0)) || (g2Cond != g2)) && (g2Cond >= 0))
                            {
                                SB.AppendLine("\t}");
                                g2Cond = -1;
                            }
                            if ((g1Cond != g1) && (g1Cond >= 0))
                            {
                                SB.AppendLine("}");
                                g1Cond = -1;
                            }

                            // open conditionals if required (group not currently open, and not guaranteed to be present)
                            if ((!GroupAlwaysPresent1[g1]) && (g1Cond != g1))
                            {
                                SB.AppendLine("if (" + agu + " & " + (1 << g1) + ") {");
                                g1Cond = g1;
                            }
                            if ((!GroupAlwaysPresent2[g2]) && (g2Cond != g2))
                            {
                                SB.AppendLine("\tif (" + bgu + " & " + (1 << g2) + ") {");
                                g2Cond = g2;
                            }

                            // get function name
                            string funcName = GetGPpartFunctionName(S, FT, M, g1, g2, g3);

                            SB.AppendLine("\t\t" + funcName + "(_" + FAI[0].Name + "[" + g1 + "], _" + FAI[1].Name + "[" + g2 + "], c + " + gmv.GroupStartIdx(g3) + ");");
                        }
                    }
                }
            }


            // close any open conditionals
            if (g2Cond >= 0)
            {
                SB.AppendLine("\t}");
                g2Cond = -1;
            }
            if (g1Cond >= 0)
            {
                SB.AppendLine("}");
                g1Cond = -1;
            }


            // compress / return result
            SB.Append(GetCompressCode(S, FT, FAI, resultName, T == ProductTypes.SCALAR_PRODUCT));

            return SB.ToString();

        } // end of GetGPcodeCppOrC()


        private static string GetGPcodeCSharpOrJava(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, G25.Metric M,
            ProductTypes T,
            G25.CG.Shared.FuncArgInfo[] FAI, string resultName)
        {
            G25.GMV gmv = S.m_GMV;

            StringBuilder SB = new StringBuilder();
            bool resultIsScalar = (T == ProductTypes.SCALAR_PRODUCT);
            bool initResultToZero = true;
            SB.Append(GetExpandCode(S, cgd, FT, FAI, resultIsScalar, initResultToZero));

            // get number of groups
            int nbGroups1 = (FAI[0].IsScalar()) ? 1 : gmv.NbGroups;
            int nbGroups2 = (FAI[1].IsScalar()) ? 1 : gmv.NbGroups;
            bool[] GroupAlwaysPresent1 = new bool[nbGroups1];
            bool[] GroupAlwaysPresent2 = new bool[nbGroups2];
            if (FAI[0].IsScalar()) GroupAlwaysPresent1[0] = true;
            if (FAI[1].IsScalar()) GroupAlwaysPresent2[0] = true;

            int g1Cond = -1; // grade 1 conditional which is open (-1 = none)
            int g2Cond = -1; // grade 2 conditional which is open (-1 = none)

            for (int g1 = 0; g1 < nbGroups1; g1++)
            {
                for (int g2 = 0; g2 < nbGroups2; g2++)
                {
                    for (int g3 = 0; g3 < gmv.NbGroups; g3++)
                    {
                        if (!zero(S, cgd, FT, M, g1, g2, g3, T))
                        {
                            // close conditionals if required
                            if ((((g1Cond != g1) && (g1Cond >= 0)) || (g2Cond != g2)) && (g2Cond >= 0))
                            {
                                SB.AppendLine("\t}");
                                g2Cond = -1;
                            }
                            if ((g1Cond != g1) && (g1Cond >= 0))
                            {
                                SB.AppendLine("}");
                                g1Cond = -1;
                            }

                            // open conditionals if required (group not currently open, and not guaranteed to be present)
                            if ((!GroupAlwaysPresent1[g1]) && (g1Cond != g1))
                            {
                                SB.AppendLine("if (ac[" + g1 + "] != null) {");
                                g1Cond = g1;
                            }
                            if ((!GroupAlwaysPresent2[g2]) && (g2Cond != g2))
                            {
                                SB.AppendLine("\tif (bc[" + g2+ "] != null) {");
                                g2Cond = g2;
                            }

                            SB.AppendLine("\t\tif (cc[" + g3+ "] == null) cc[" + g3+ "] = new " + FT.type + "[" + gmv.Group(g3).Length + "];");

                            // get function name
                            string funcName = GetGPpartFunctionName(S, FT, M, g1, g2, g3);

                            SB.AppendLine("\t\t" + funcName + "(ac[" + g1 + "], bc[" + g2 + "], cc[" + g3 + "]);");
                        }
                    }
                }
            }


            // close any open conditionals
            if (g2Cond >= 0)
            {
                SB.AppendLine("\t}");
                g2Cond = -1;
            }
            if (g1Cond >= 0)
            {
                SB.AppendLine("}");
                g1Cond = -1;
            }


            if (resultIsScalar)
                SB.AppendLine("return cc[0][0];");
            else SB.AppendLine("return new " + FT.GetMangledName(S, gmv.Name) + "(cc);");
            
            return SB.ToString();

        } // end of GetGPcodeCSharpOrJava()

        /// <summary>
        /// Returns the code for a inverse geometric product. 
        /// 
        /// The returned code is only the body. The function declaration is not included.
        /// </summary>
        /// <param name="S">Specification of algebra (used for output language and to obtain general multivector type).</param>
        /// <param name="cgd">Used for <c>m_gmvGPpartFuncNames</c>.</param>
        /// <param name="FT">Floating point type.</param>
        /// <param name="M">Metric type.</param>
        /// <param name="T">The product (e.g., geometric, outer, etc)</param>
        /// <param name="FAI">Info about function arguments. Used to know whether arguments are general multivectors or scalars.</param>
        /// <param name="resultName">Name of variable where the result goes (in the generated code).</param>
        /// <returns>code for the requested product type.</returns>
        public static string GetIGPcode(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, G25.Metric M,
            G25.CG.Shared.FuncArgInfo[] FAI, string resultName)
        {
            G25.GMV gmv = S.m_GMV;

            StringBuilder SB = new StringBuilder();

            string divFuncName = G25.CG.Shared.Dependencies.GetDependency(S, cgd, "div", new String[] { gmv.Name, FT.type }, FT, null);

            System.Collections.Hashtable argTable = new System.Collections.Hashtable();
            argTable["S"] = S;
            argTable["gmvName"] = FT.GetMangledName(S, gmv.Name);
            argTable["FT"] = FT;
            argTable["arg1name"] = FAI[0].Name;
            argTable["arg2name"] = FAI[1].Name;
            argTable["divFuncName"] = divFuncName;

            if (S.OutputC())
                argTable["dstName"] = resultName;

            bool arg1isGmv = FAI[0].Type is G25.GMV;
            bool arg2isGmv = FAI[1].Type is G25.GMV;
            if (arg2isGmv)
            {
                string norm2FuncName = G25.CG.Shared.Dependencies.GetDependency(S, cgd, "norm2", new String[] { gmv.Name }, FT, M.m_name) + CANSparts.RETURNS_SCALAR;
                string reverseFuncName = G25.CG.Shared.Dependencies.GetDependency(S, cgd, "reverse", new String[] { gmv.Name }, FT, null);
                argTable["reverseFuncName"] = reverseFuncName;
                argTable["norm2FuncName"] = norm2FuncName;

                if (arg1isGmv)
                {
                    string gpFuncName = G25.CG.Shared.Dependencies.GetDependency(S, cgd, "gp", new String[] { gmv.Name, gmv.Name }, FT, M.m_name);
                    argTable["gpFuncName"] = gpFuncName;
                    cgd.m_cog.EmitTemplate(SB, "igp_GMV_GMV_body", argTable);
                }
                else
                {
                    string gpFuncName = G25.CG.Shared.Dependencies.GetDependency(S, cgd, "gp", new String[] { gmv.Name, FT.type }, FT, M.m_name);
                    argTable["mulFuncName"] = gpFuncName;
                    cgd.m_cog.EmitTemplate(SB, "igp_float_GMV_body", argTable);
                }
            }
            else if (arg1isGmv)
            {
                cgd.m_cog.EmitTemplate(SB, "igp_GMV_float_body", argTable);
            }

            return SB.ToString();
        }

        /// <summary>
        /// Returns the code for computing the norm (squared). 
        /// 
        /// The code is composed of calls to functions generated by <c>WriteGmvGpParts()</c>.
        /// 
        /// By default, the function returns a scalar, but when both <c>returnType</c> and <c>returnName</c>
        /// are used, the result is written to a specialized multivector variable <c>returnName</c> of type <c>returnType</c>.
        /// 
        /// The returned code is only the body. The function declaration is not included.
        /// </summary>
        /// <param name="S">Specification of algebra (used for output language).</param>
        /// <param name="cgd">Used for <c>m_gmvGPpartFuncNames</c>.</param>
        /// <param name="FT">Floating point type.</param>
        /// <param name="M">Metric to be used for the norm.</param>
        /// <param name="squared">Whether the norm or norm squared should be generated.</param>
        /// <param name="FAI">Info about function arguments. All arguments must be general multivectors.</param>
        /// <param name="returnType">The type to be used to return the scalar (can be null).</param>
        /// <param name="returnName">Name of return argument (can be null).</param>
        /// <returns>code for the requested product type.</returns>
        public static string GetNormCode(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, 
            G25.Metric M, bool squared,
            G25.CG.Shared.FuncArgInfo[] FAI, G25.SMV returnType, string returnName)
        {
            if (M.m_metric.IsDiagonal())
            {
                return GetDiagonalMetricNormCode(S, cgd, FT, M, squared, FAI, returnType, returnName);
            }
            else
            {
                return GetAnyMetricNormCode(S, cgd, FT, M, squared, FAI, returnType, returnName);
            }
        }
        
        private static string GetDiagonalMetricNormCode(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, 
            G25.Metric M, bool squared,
            G25.CG.Shared.FuncArgInfo[] FAI, G25.SMV returnType, string returnName)
        {

            G25.GMV gmv = S.m_GMV;

            StringBuilder SB = new StringBuilder();

            if (S.OutputCSharpOrJava())
            {
                SB.AppendLine(FT.type + "[] c = new " + FT.type + "[1];");
                SB.AppendLine(FT.type + "[][] ac = " + FAI[0].Name + ".c();");
            }
            else SB.AppendLine(FT.type + " c[1];");

            SB.AppendLine(FT.type + " n2 = " + FT.DoubleToString(S, 0.0) + ";");
            if (S.OutputCppOrC())
                SB.AppendLine("int idx = 0;");

            string agu = (S.OutputC()) ? FAI[0].Name + "->gu" : FAI[0].Name + ".gu()";
            string ac = (S.OutputC()) ? FAI[0].Name + "->c" : FAI[0].Name + ".getC()";

            for (int g = 0; g < gmv.NbGroups; g++)
            {
                // get function name
                string funcName = GetGPpartFunctionName(S, FT, M, g, g, 0); // grade 0 part of gp(g, g)

                // get multiplier
                double m = gmv.Group(g)[0].Reverse().scale / gmv.Group(g)[0].scale;

                SB.AppendLine("");
                if (S.OutputCSharpOrJava())
                    SB.Append("if (ac[" + g + "] != null) {");
                else SB.Append("if (" + agu + " & " + (1 << g) + ") {");
                SB.AppendLine(" // group " + g + " (grade " + gmv.Group(g)[0].Grade() + ")");

                // check if funcName (gp part) exists)
                Tuple<string, string, string> key = new Tuple<string, string, string>(FT.type, M.m_name, funcName);
                if (cgd.m_gmvGPpartFuncNames.ContainsKey(key) && cgd.m_gmvGPpartFuncNames[key])
                {
                    SB.AppendLine("\tc[0] = " + FT.DoubleToString(S, 0.0) + ";");

                    if (S.OutputCSharpOrJava())
                        SB.AppendLine("\t" + funcName + "(ac[" + g + "], ac[ " + g + "], c);");
                    else SB.AppendLine("\t" + funcName + "(" + ac + " + idx, " + ac + " + idx, c);");

                    if (m == 1.0) SB.AppendLine("\tn2 += c[0];");
                    else if (m == -1.0) SB.AppendLine("\tn2 -= c[0];");
                    else SB.AppendLine("\tn2 += " + FT.DoubleToString(S, m) + " * c[0];");
                }
                if ((g < (gmv.NbGroups - 1)) && S.OutputCppOrC())
                    SB.AppendLine("\tidx += " + gmv.Group(g).Length + ";");
                SB.AppendLine("}");
            }

            string returnVal;
            { // get return value
                if (squared) returnVal = "n2";
                else
                {
                    string sqrtFuncName = G25.CG.Shared.CodeUtil.OpNameToLangString(S, FT, RefGA.Symbolic.ScalarOp.SQRT);
                    if (M.m_metric.IsPositiveDefinite()) // if PD, then negative values are impossible
                        returnVal = sqrtFuncName + "(n2)";
                    else returnVal = "((n2 < " + FT.DoubleToString(S, 0.0) + ") ? " + sqrtFuncName + "(-n2) : " + sqrtFuncName + "(n2))";
                }
            }

            if ((returnType == null) || (returnName == null))
            {
                SB.AppendLine("return " + returnVal + ";");
            }
            else 
            {
                if (S.OutputC())
                {
                    // get assign code, assign it 
                    bool mustCast = false;
                    bool dstPtr = true;
                    int nbTabs = 0;
                    bool writeZeros = true;
                    SB.Append(CodeUtil.GenerateSMVassignmentCode(S, FT, mustCast,
                        returnType, returnName, dstPtr, new RefGA.Multivector(returnVal), nbTabs, writeZeros));
                }
                else
                {
                    bool mustCast = false;
                    int nbTabs = 0;
                    new ReturnInstruction(nbTabs, returnType, FT, mustCast, new RefGA.Multivector(returnVal)).Write(SB, S, cgd);
                }
            }


            return SB.ToString();
        } // end of GetDiagonalMetricNormCode();

        private static string GetAnyMetricNormCode(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, 
            G25.Metric M, bool squared,
            G25.CG.Shared.FuncArgInfo[] FAI, G25.SMV returnType, string returnName)
        {
            G25.GMV gmv = S.m_GMV;

            StringBuilder SB = new StringBuilder();
             
            SB.AppendLine(FT.type + " n2 = " + FT.DoubleToString(S, 0.0) + ";");

            if (S.OutputCSharpOrJava())
            {
                SB.AppendLine(FT.type + "[] c = new " + FT.type + "[1];");
                SB.AppendLine(FT.type + "[][] ac = " + FAI[0].Name + ".c();");
            }
            else {
                bool resultIsScalar = true;
                bool initResultToZero = true;
                SB.Append(GetExpandCode(S, cgd, FT, FAI, resultIsScalar, initResultToZero));
            }

            string exaName = "_" + FAI[0].Name;

            for (int g1 = 0; g1 < gmv.NbGroups; g1++)
            {
                bool funcFound = false;

                for (int g2 = 0; g2 < gmv.NbGroups; g2++) {
                    // get function name
                    string funcName = GetGPpartFunctionName(S, FT, M, g1, g2, 0); // grade 0 part of gp(g, g)

                    // check if funcName (gp part) exists)
                    Tuple<string, string, string> key = new Tuple<string, string, string>(FT.type, M.m_name, funcName);
                    if (cgd.m_gmvGPpartFuncNames.ContainsKey(key) && cgd.m_gmvGPpartFuncNames[key])
                    {
                        if (!funcFound)
                        {
                            if (S.OutputCSharpOrJava())
                                SB.Append("if (ac[" + g1 + "] != null) {");
                            else SB.Append("if (" + exaName + "[" + g1 + "] != NULL) { ");
                            SB.AppendLine(" // group " + g1 + " (grade " + gmv.Group(g1)[0].Grade() + ")");
                            SB.AppendLine("\tc[0] = " + FT.DoubleToString(S, 0.0) + ";");
                            funcFound = true;
                        }

                        if (g1 != g2)
                        {
                            if (S.OutputCSharpOrJava())
                                SB.Append("\tif (ac[" + g2 + "] != null) {");
                            else SB.Append("\tif (" + exaName + "[" + g2 + "] != NULL) {");
                            SB.AppendLine(" // group " + g2 + " (grade " + gmv.Group(g2)[0].Grade() + ")");
                        }

                        if (S.OutputCSharpOrJava())
                            SB.AppendLine("\t\t" + funcName + "(ac[" + g1 + "], ac[" + g2 + "], c);");
                        else SB.AppendLine("\t\t" + funcName + "(" + exaName + "[" + g1 + "], " + exaName + "[" + g2 + "], c);");

                        if (g1 != g2)
                            SB.AppendLine("\t}");
                    }
                }

                if (funcFound) // only do this if any matching gp function for this group was found
                {
                    // get multiplier
                    double m = gmv.Group(g1)[0].Reverse().scale / gmv.Group(g1)[0].scale; // must always have the same grade 
                    if (m == 1.0) SB.AppendLine("\tn2 += c[0];");
                    else if (m == -1.0) SB.AppendLine("\tn2 -= c[0];");
                    else SB.AppendLine("\tn2 += " + FT.DoubleToString(S, m) + " * c[0];");
                    SB.AppendLine("}");
                }

            }

            string returnVal;
            { // get return value
                if (squared) returnVal = "n2";
                else
                {
                    string sqrtFuncName = G25.CG.Shared.CodeUtil.OpNameToLangString(S, FT, RefGA.Symbolic.ScalarOp.SQRT);
                    if (M.m_metric.IsPositiveDefinite()) // if PD, then negative values are impossible
                        returnVal = sqrtFuncName + "(n2)";
                    else returnVal = "((n2 < " + FT.DoubleToString(S, 0.0) + ") ? " + sqrtFuncName + "(-n2) : " + sqrtFuncName + "(n2))";
                }
            }

            // can be shared with other GetNormCode
            if ((returnType == null) || (returnName == null))
            {
                SB.AppendLine("return " + returnVal + ";");
            }
            else 
            {
                if (S.OutputC())
                {
                    // get assign code, assign it 
                    bool mustCast = false;
                    bool dstPtr = true;
                    int nbTabs = 0;
                    bool writeZeros = true;
                    SB.Append(CodeUtil.GenerateSMVassignmentCode(S, FT, mustCast,
                        returnType, returnName, dstPtr, new RefGA.Multivector(returnVal), nbTabs, writeZeros));
                }
                else
                {
                    bool mustCast = false;
                    int nbTabs = 0;
                    new ReturnInstruction(nbTabs, returnType, FT, mustCast, new RefGA.Multivector(returnVal)).Write(SB, S, cgd);
                }
            }

            return SB.ToString();
        } // end of GetAnyMetricNormCode();


        /// <summary>
        /// Returns the code for applying a versor <c>V</c>to some multivector <c>M</c> (V M / V).
        /// The code is composed of calls to functions generated by <c>WriteGmvGpParts()</c>.
        /// 
        /// This function does not explicitly use the geometric product parts code, but makes calls
        /// to the geometric product and the reverse/versor inverse. As such it generates dependencies
        /// on <c>gp</c>, <c>reverse</c>, <c>versorInverse</c> and <c>grade</c>.
        /// 
        /// Three types of code can be generated, depending on the value of 
        /// argument <c>T</c>:
        ///   - <c>REVERSE</c>: the versor is unit, so the reverse can be used.
        ///   - <c>INVERSE</c>: the versor inverse is used to compute the inverse.
        ///   - <c>EXPLICIT_INVERSE</c>: the inverse is explicitly provided.
        /// 
        /// The returned code is only the body. The function declaration is not included.
        /// </summary>
        /// <param name="S">Specification of algebra (used for output language).</param>
        /// <param name="cgd">Used for resolving dependencies.</param>
        /// <param name="FT">Floating point type.</param>
        /// <param name="M">Metric type used in the geometric products.</param>
        /// <param name="T">The product (e.g., geometric, outer, etc).</param>
        /// <param name="FAI">Info about function arguments. All arguments must be general multivectors.</param>
        /// <param name="resultName">Name of variable where the result goes (in the generated code).</param>
        /// <returns>code for the requested product type.</returns>
        public static string GetVersorApplicationCode(Specification S, G25.CG.Shared.CGdata cgd, 
            G25.FloatType FT, G25.Metric M, ApplyVersorTypes T,
            G25.CG.Shared.FuncArgInfo[] FAI, String resultName)
        {
            G25.GMV gmv = S.m_GMV;

            StringBuilder SB = new StringBuilder();

            // get geometric product function
            string gpFuncName = G25.CG.Shared.Dependencies.GetDependency(S, cgd, "gp", new String[] { gmv.Name, gmv.Name }, FT, M.m_name);

            // get reverse or inverse function
            string invFuncName = null;
            if ((T == ApplyVersorTypes.INVERSE) || (T == ApplyVersorTypes.REVERSE))
            {
                string inputFuncName = (T == ApplyVersorTypes.INVERSE) ? "versorInverse" : "reverse";

                invFuncName = G25.CG.Shared.Dependencies.GetDependency(S, cgd,
                     inputFuncName, new String[] { gmv.Name }, FT, (T == ApplyVersorTypes.INVERSE) ? M.m_name : null);
            }

            // get grade function
            string gradeFuncName = G25.CG.Shared.Dependencies.GetDependency(S, cgd, G25.CG.Shared.CANSparts.EXTRACT_GRADE, new String[] { gmv.Name }, FT, null);

            // get string to be used for grade extraction
            string gradeUsageString;
            string bgu = (S.OutputC()) ? FAI[1].Name + "->gu" : FAI[1].Name + ".gu()";
            if (!gmv.IsGroupedByGrade(S.m_dimension))
            {
                SB.AppendLine("int gradeUsageBitmap;");
                gradeUsageString = "gradeUsageBitmap";
            }
            else
            {
                gradeUsageString = bgu;
                /*if (S.OutputC())
                    gradeUsageString = FAI[1].Name + "->gu";
                else gradeUsageString = FAI[1].Name + ".gu()";*/
            }

            // select grade parts!
            StringBuilder gradeUsageCode = new StringBuilder();
            if (!gmv.IsGroupedByGrade(S.m_dimension))
            {
                gradeUsageCode.Append(gradeUsageString + " = ");
                for (int g = 0; g <= S.m_dimension; g++)
                {
                    if (g > 0) gradeUsageCode.Append(" | ");
                    gradeUsageCode.Append("((" + bgu + " & GRADE_" + g + ") ? GRADE_" + g + " : 0)");
                }
                gradeUsageCode.AppendLine(";");
            }



            if (S.OutputC())
            {
                // get name of inverse, decl tmp variable for inverse if required
                string inverseInputName;
                if (T == ApplyVersorTypes.EXPLICIT_INVERSE) inverseInputName = FAI[2].Name;
                else if (T == ApplyVersorTypes.INVERSE)
                {
                    inverseInputName = "&inv";
                    SB.AppendLine(FT.GetMangledName(S, gmv.Name) + " inv; // temp space for reverse");
                }
                else // (T == ApplyVersorTypes.REVERSE)
                {
                    inverseInputName = "&rev";
                    SB.AppendLine(FT.GetMangledName(S, gmv.Name) + " rev; // temp space for reverse");
                }

                // get temp space for input * object
                SB.AppendLine(FT.GetMangledName(S, gmv.Name) + " tmp, tmp2; // temp variables");

                // compute inverse or reverse, if required
                if ((T == ApplyVersorTypes.INVERSE) || (T == ApplyVersorTypes.REVERSE))
                {
                    SB.AppendLine(invFuncName + "(" + inverseInputName + ", " + FAI[0].Name + "); // compute inverse or reverse");
                }

                // compute input * object
                SB.AppendLine(gpFuncName + "(&tmp, " + FAI[0].Name + ", " + FAI[1].Name + "); // compute geometric product " + FAI[0].Name + " " + FAI[1].Name);

                // compute (input * object) * inverse
                SB.AppendLine(gpFuncName + "(&tmp2, &tmp, " + inverseInputName + "); // compute geometric product (" + FAI[0].Name + " " + FAI[1].Name + ") " + inverseInputName);

                // select grade parts!
                SB.AppendLine(gradeUsageCode.ToString());
                SB.AppendLine(gradeFuncName + "(" + resultName + ", &tmp2, " + gradeUsageString + "); // ditch grade parts which were not in " + FAI[1].Name);
            } // end of 'C' version
            else // C++ version
            {
                if (gradeUsageCode.Length > 0)
                    SB.AppendLine(gradeUsageCode.ToString());
                string inverseCode = ((T == ApplyVersorTypes.INVERSE) || (T == ApplyVersorTypes.REVERSE))
                    ? (invFuncName + "(" + FAI[0].Name + ")")
                    : FAI[2].Name;

                SB.AppendLine("return " + gradeFuncName + "(" + gpFuncName + "(" + gpFuncName + "(" + FAI[0].Name + ", " + FAI[1].Name + "), " + inverseCode + "), " + gradeUsageString + ");");
            }

            return SB.ToString();
        } // end of GetVersorApplicationCode()


    } // end of class GPparts
} // end of namepace G25.CG.Shared
