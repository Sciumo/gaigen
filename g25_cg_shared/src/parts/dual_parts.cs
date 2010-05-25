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
    /// Contains functions for generating function which compute the (un)dual of the general multivector for various
    /// metrics and floating point types. A function is generated for each group of the general multivector.
    /// 
    /// Use the <c>GetDualCode()</c> to generate code which uses these function to actually compute
    /// the (un)dual of a full multivector.
    /// 
    /// The class is output language aware (currently: C)
    /// </summary>
    public class DualParts
    {


        /// <summary>
        /// Returns the name of a partial dual function.
        /// </summary>
        /// <param name="FT">Float type (used for mangled name).</param>
        /// <param name="M">Metric of dual.</param>
        /// <param name="gi">Grade/group of input.</param>
        /// <param name="go">Grade/group of output.</param>
        /// <returns>name of a partial geometric product function.</returns>
        public static string GetDualPartFunctionName(Specification S, G25.FloatType FT, G25.Metric M, int gi, int go)
        {
            return FT.GetMangledName(S, "dual") + "_" + M.m_name + "_" + gi + "_" + go;
        }

        /// <summary>
        /// Returns the name of a partial undual function.
        /// </summary>
        /// <param name="FT">Float type (used for mangled name).</param>
        /// <param name="M">Metric of dual.</param>
        /// <param name="gi">Grade/group of input.</param>
        /// <param name="go">Grade/group of output.</param>
        /// <returns>name of a partial geometric product function.</returns>
        public static string GetUndualPartFunctionName(Specification S, G25.FloatType FT, G25.Metric M, int gi, int go)
        {
            return FT.GetMangledName(S, "undual") + "_" + M.m_name + "_" + gi + "_" + go;
        }

        /// <param name="S"></param>
        /// <param name="FT"></param>
        /// <param name="M"></param>
        /// <param name="d">1 -> generate dual, d = 0 -> generate undual</param>
        /// <param name="g1"></param>
        /// <param name="gd"></param>
        /// <param name="name1"></param>
        /// <param name="name2"></param>
        /// <param name="name3"></param>
        /// <returns>The code to compute a (un)dual at runtime using the geometric product tables.</returns>
        public static string GetRuntimeDualCode(G25.Specification S, G25.FloatType FT, G25.Metric M, 
            int d, int g1, int gd,
            string name1, string name2, string name3)
        {
            // get pseudoscalar
            RefGA.Multivector I = RefGA.Multivector.GetPseudoscalar(S.m_dimension);
            if (d == 1) // if dual: use inverse I
                I = RefGA.Multivector.VersorInverse(I, M.m_metric);

            // get grade/group of pseudoscalar in GMV
            int g2 = S.m_GMV.GetGroupIdx(I.BasisBlades[0]);

            // get sign of pseudo scalar basis blade in GMV:
            double IbladeSign = S.m_GMV.Group(g2)[0].scale;

            return 
                "\t" + FT.type + " " + name2 + "[1] = {" + FT.DoubleToString(S, IbladeSign * I.BasisBlades[0].scale) + "};\n" +
                "\t" + GPparts.GetGPpartFunctionName(S, FT, M, g1, g2, gd) + "(" + name1 + ", " + name2 + ", " + name3 + ");\n";
        }


        /// <summary>
        /// Generates functions which compute the dual or undual of general multivectors, on a group by group basis.
        /// 
        /// This function should be called early on in the code generation process, at least
        /// before any of the <c>GetDualCode()</c> functions is called.
        /// </summary>
        /// <param name="S">Specification (used for output language, GMV).</param>
        /// <param name="cgd">Where the result goes.</param>
        public static void WriteDualParts(Specification S, CGdata cgd)
        {
            G25.GMV gmv = S.m_GMV;

            string name1 = "A";
            string name2 = "B";
            string name3 = "C";
            bool ptr = true;
            int allGroups = -1;
            bool mustCast = false;
            int nbBaseTabs = (S.OutputCSharpOrJava()) ? 1 : 0;
            int nbCodeTabs = nbBaseTabs + 1;
            bool writeZeros = false;

            // get symbolic multivectors
            RefGA.Multivector[] M1 = null;
            if (S.m_gmvCodeGeneration == GMV_CODE.EXPAND)
                M1 = G25.CG.Shared.Symbolic.GMVtoSymbolicMultivector(S, gmv, name1, ptr, allGroups);

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                // map from code fragment to name of function
                Dictionary<string, string> generatedCode = new Dictionary<string, string>();

                foreach (G25.Metric M in S.m_metric)
                {
                    if (M.m_metric.IsDegenerate()) continue; // do not generate code for degenerate metrics

                    for (int g1 = 0; g1 < gmv.NbGroups; g1++)
                    {
                        for (int d = 1; d >= 0; d--) // d = 1 -> generate dual, d = 0 -> generate undual
                        {
                            // get value of operation, name of function:
                            RefGA.Multivector value = null;
                            if (S.m_gmvCodeGeneration == GMV_CODE.EXPAND)
                            {
                                try
                                {
                                    value = (d == 0) ? RefGA.Multivector.Undual(M1[g1], M.m_metric) : RefGA.Multivector.Dual(M1[g1], M.m_metric);
                                }
                                catch (Exception)
                                {
                                    cgd.AddError(new G25.UserException("Non-invertable pseudoscalar. Do not generate (un)dual functions for degenerate metrics."));
                                    return;
                                }
                                if (M.m_round) value = value.Round(1e-14);
                            }


                            int grade = gmv.Group(g1)[0].Grade();
                            int dualGrade = S.m_dimension - grade;
                            for (int g3 = 0; g3 < gmv.NbGroups; g3++)
                            {
                                if (gmv.Group(g3)[0].Grade() == dualGrade)
                                {
                                    string funcName = (d == 0) ? GetUndualPartFunctionName(S, FT, M, g1, g3) : GetDualPartFunctionName(S, FT, M, g1, g3);

                                    // get assignment code
                                    string code = "";

                                    if (S.m_gmvCodeGeneration == GMV_CODE.EXPAND)
                                    { // full code expansion
                                        int dstBaseIdx = 0;
                                        code = G25.CG.Shared.CodeUtil.GenerateGMVassignmentCode(S, FT, mustCast, gmv, name3, g3, dstBaseIdx, value, nbCodeTabs, writeZeros);
                                    }
                                    else if (S.m_gmvCodeGeneration == GMV_CODE.RUNTIME)
                                    { // runtime code
                                        code = GetRuntimeDualCode(S, FT, M, d, g1, g3, name1, name2, name3);
                                    }

                                    cgd.m_gmvDualPartFuncNames[new Tuple<string, string, string>(FT.type, M.m_name, funcName)] = (code.Length > 0);

                                    if (code.Length == 0) continue; // only if code is non-empty

                                    // is the following ever required: (i.e. the dual of a basis blade wrt to the full space is not a single basis blade?)
                                    //if (!M.m_metric.IsDiagonal())
                                    //    code = code.Replace("=", "+=");

                                    // check if code was already generated, and, if so, reuse it
                                    if (generatedCode.ContainsKey(code))
                                    {
                                        // ready generated: call that function
                                        code = new string('\t', nbCodeTabs) + generatedCode[code] + "(" + name1 + ", " + name3 + ");\n";
                                    }
                                    else
                                    {
                                        // not generated yet: remember code -> function
                                        generatedCode[code] = funcName;
                                    }

                                    // write comment
                                    string comment = "Computes the partial " + ((d == 0) ? "un" : "") + "dual (w.r.t. full space) of a multivector.";

                                    string ACCESS = "";
                                    if (S.OutputJava()) ACCESS = "protected final static ";
                                    else if (S.OutputCSharp()) ACCESS  = "protected internal static ";
                                    string ARR = (S.OutputCSharpOrJava()) ? "[] " : " *";
                                    string CONST = (S.OutputCSharpOrJava()) ? "" : "const ";

                                    string funcDecl = ACCESS + "void " + funcName + "(" + CONST + FT.type + ARR + name1 + ", " + FT.type + ARR + name3 + ")";

                                    if (S.OutputCppOrC())
                                    {
                                        new Comment(comment).Write(cgd.m_declSB, S, nbBaseTabs);
                                        cgd.m_declSB.Append(funcDecl); cgd.m_declSB.AppendLine(";");
                                    }
                                    else
                                    {
                                        new Comment(comment).Write(cgd.m_defSB, S, nbBaseTabs);
                                    }

                                    // emit def
                                    cgd.m_defSB.Append('\t', nbBaseTabs);
                                    cgd.m_defSB.Append(funcDecl);
                                    cgd.m_defSB.AppendLine(" {");
                                    cgd.m_defSB.Append(code);
                                    cgd.m_defSB.Append('\t', nbBaseTabs);
                                    cgd.m_defSB.AppendLine("}");
                                }
                            } // end of loop over all output groups
                        } // end of loop over undual (0) and dual(1)
                    } // end of loop over all input groups
                } // end of loop over all metric
            } // end of loop over all float types
        } // end of function WriteDualParts()

                        
        /// <summary>
        /// Returns the code for dualization wrt to whole space using metric <c>M</c>.
        /// The code is composed of calls to functions generated by <c>WriteGmvDualParts()</c>.
        /// 
        /// This function uses <c>cdg.m_gmvDualPartFuncNames</c>, but only to check whether a
        /// geometric product of some group with the pseudoscalar will get non-zero results in some
        /// other group.
        /// 
        /// The returned code is only the body. The function declaration is not included.
        /// </summary>
        /// <param name="S">Specification of algebra (used for general multivector type, output language).</param>
        /// <param name="cgd">Used for <c>m_gmvDualPartFuncNames</c>.</param>
        /// <param name="FT">Floating point type.</param>
        /// <param name="M">The metric of the dual.</param>
        /// <param name="FAI">Info about function arguments</param>
        /// <param name="resultName">Name of variable where the result goes (in the generated code).</param>
        /// <param name="dual">When true, 'dual' is generated, otherwise, 'undual' is generated.</param>
        /// <returns>code for the requested product type.</returns>
        public static string GetDualCode(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, 
            G25.Metric M, G25.CG.Shared.FuncArgInfo[] FAI, string resultName, bool dual)
        {
            if (S.OutputCppOrC())
                return GetDualCodeCppOrC(S, cgd, FT, M, FAI, resultName, dual);
            else return GetDualCodeCSharpOrJava(S, cgd, FT, M, FAI, resultName, dual);
        }

        private static string GetDualCodeCppOrC(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, 
            G25.Metric M, G25.CG.Shared.FuncArgInfo[] FAI, string resultName, bool dual)
        {
            G25.GMV gmv = S.m_GMV;

            StringBuilder SB = new StringBuilder();

            string agu = (S.OutputC()) ? FAI[0].Name + "->gu" : FAI[0].Name + ".gu()";
            string ac = (S.OutputC()) ? FAI[0].Name + "->c" : FAI[0].Name + ".getC()";

            // allocate memory to store result:
            SB.AppendLine("int idx = 0;");
            bool resultIsScalar = false;
            bool initResultToZero = true; // must init to zero because of compression
            SB.Append(GPparts.GetExpandCode(S, cgd, FT, null, resultIsScalar, initResultToZero));

            // get number of groups:
            int nbGroups = gmv.NbGroups;

            // for each combination of groups, check if the dual goes from one to the other
            for (int gi = 0; gi < nbGroups; gi++)
            {
                SB.AppendLine("if (" + agu + " & " + (1 << gi) + ") {");
                for (int go = 0; go < nbGroups; go++)
                {
                    string funcName = (dual) ? GetDualPartFunctionName(S, FT, M, gi, go) : GetUndualPartFunctionName(S, FT, M, gi, go);
                    Tuple<string, string, string> key = new Tuple<string, string, string>(FT.type, M.m_name, funcName);
                    if (cgd.m_gmvDualPartFuncNames.ContainsKey(key) &&
                        cgd.m_gmvDualPartFuncNames[key]) {
                        SB.AppendLine("\t" + funcName + "(" + ac + " + idx, c + " + gmv.GroupStartIdx(go) +");");
                    }
                }
                if (gi < (nbGroups - 1)) SB.AppendLine("\tidx += " + gmv.Group(gi).Length + ";");
                SB.AppendLine("}");
                SB.AppendLine("");
            }

            // compress result
            SB.Append(GPparts.GetCompressCode(S, FT, FAI, resultName, resultIsScalar));

            return SB.ToString();
        } // end of GetDualCodeCppOrC()

        private static string GetDualCodeCSharpOrJava(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT,
            G25.Metric M, G25.CG.Shared.FuncArgInfo[] FAI, string resultName, bool dual)
        {
            G25.GMV gmv = S.m_GMV;

            StringBuilder SB = new StringBuilder();

            bool resultIsScalar = false;
            bool initResultToZero = true; // must init to zero because of compression
            SB.Append(GPparts.GetExpandCode(S, cgd, FT, FAI, resultIsScalar, initResultToZero));

            // get number of groups:
            int nbGroups = gmv.NbGroups;

            // for each combination of groups, check if the dual goes from one to the other
            for (int gi = 0; gi < nbGroups; gi++)
            {
                SB.AppendLine("if (ac[" + gi + "] != null) {");
                for (int go = 0; go < nbGroups; go++)
                {
                    string funcName = (dual) ? GetDualPartFunctionName(S, FT, M, gi, go) : GetUndualPartFunctionName(S, FT, M, gi, go);
                    Tuple<string, string, string> key = new Tuple<string, string, string>(FT.type, M.m_name, funcName);
                    if (cgd.m_gmvDualPartFuncNames.ContainsKey(key) &&
                        cgd.m_gmvDualPartFuncNames[key])
                    {
                        SB.AppendLine("\tif (cc[" + go + "] == null) cc[" + go + "] = new " + FT.type + "[" + gmv.Group(go).Length + "];");
                        
                        SB.AppendLine("\t" + funcName + "(ac[" + gi + "], cc[" + go + "]);");
                    }
                }
                SB.AppendLine("}");
                SB.AppendLine("");
            }

            SB.AppendLine("return new " + FT.GetMangledName(S, gmv.Name) + "(cc);");

            return SB.ToString();
        } // end of GetDualCodeCppOrC()

    } // end of class DualParts
} // end of namepace G25.CG.Shared
