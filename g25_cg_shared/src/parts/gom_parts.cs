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
    /// Contains functions for generating function which compute the application of
    /// general outermorphisms to general multivectors.
    /// 
    /// The class is output language aware (currently: C)
    /// </summary>
    public class GomParts
    {

        /// <summary>
        /// Returns the name of a partial GOM-GMV application function.
        /// </summary>
        /// <param name="FT">Float type (used for mangled name).</param>
        /// <param name="srcGroup">Grade/group of input.</param>
        /// <param name="dstGroup">Grade/group of output.</param>
        public static string GetGomPartFunctionName(Specification S, G25.FloatType FT, int srcGroup, int dstGroup)
        {
            return FT.GetMangledName(S, "applyGomGmv") + "_" + srcGroup + "_" + dstGroup;
        }


        /// <summary>
        /// Generates functions which compute parts of the application of a general outermorphism to a general multivector.
        /// 
        /// This function should be called early on in the code generation process, at least
        /// before any of the <c>???()</c> functions is called.
        /// </summary>
        /// <param name="S">Specification (used for output language, GMV).</param>
        /// <param name="cgd">Where the result goes.</param>
        public static void WriteGomParts(Specification S, CGdata cgd)
        {
            if (S.m_GOM == null) return; // nothing to do if GOM not defiend

            int nbBaseTabs = (S.OutputCSharpOrJava()) ? 1 : 0;
            int nbCodeTabs = nbBaseTabs + 1;

            G25.GMV gmv = S.m_GMV;
            G25.GOM gom = S.m_GOM;

            string nameGOM = "O";
            string nameSrcGMV = "A";
            string nameDstGMV = "C";

            // get symbolic multivector value
            RefGA.Multivector[] M1 = null;
            {
                bool ptr = (S.OutputC());
                int allGroups = -1;
                M1 = G25.CG.Shared.Symbolic.GMVtoSymbolicMultivector(S, gmv, nameSrcGMV, ptr, allGroups);
            }

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                // map from code fragment to name of function
                Dictionary<string, string> generatedCode = new Dictionary<string, string>();

                // loop over all groups of the GMV, multiply with GOM, and assign the result
                for (int srcGroup = 0; srcGroup < gmv.NbGroups; srcGroup++)
                {
                    RefGA.Multivector inputValue = M1[srcGroup];
                    if (inputValue.IsScalar()) continue;

                    // Replace each basis blade in 'inputValue' with its value under the outermorphism.
                    RefGA.Multivector returnValue = RefGA.Multivector.ZERO; // returnValue = gom * gmv[srcGroup]
                    for (int i = 0; i < inputValue.BasisBlades.Length; i++)
                    {
                        // get input blade and domain for that grade
                        RefGA.BasisBlade inputBlade = inputValue.BasisBlades[i];
                        RefGA.BasisBlade[] domainBlades = gom.DomainForGrade(inputBlade.Grade());
                        for (int c = 0; c < domainBlades.Length; c++)
                        {
                            // if a match is found in the domain, add range vector to m_returnValue
                            if (domainBlades[c].bitmap == inputBlade.bitmap)
                            {
                                bool ptr = (S.OutputC());
                                RefGA.Multivector omColumnValue = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(S, gom.DomainSmvForGrade(inputBlade.Grade())[c], nameGOM, ptr);
                                RefGA.Multivector inputBladeScalarMultiplier = new RefGA.Multivector(new RefGA.BasisBlade(inputBlade, 0));
                                RefGA.Multivector domainBladeScalarMultiplier = new RefGA.Multivector(new RefGA.BasisBlade(domainBlades[c], 0));
                                returnValue = RefGA.Multivector.Add(returnValue,
                                    RefGA.Multivector.gp(
                                    RefGA.Multivector.gp(omColumnValue, inputBladeScalarMultiplier),
                                    domainBladeScalarMultiplier));
                                break; // no need to search the other domainBlades too
                            }
                        }
                    } // end of 'compute return value'

                    // assign returnValue to various groups of the gmv 
                    for (int dstGroup = 0; dstGroup < gmv.NbGroups; dstGroup++)
                    {
                        bool mustCast = false;
                        bool writeZeros = false; // no need to generate "+= 0.0;"
                        int dstBaseIdx = 0;
                        string code = G25.CG.Shared.CodeUtil.GenerateGMVassignmentCode(S, FT, mustCast, gmv, nameDstGMV, dstGroup, dstBaseIdx, returnValue, nbCodeTabs, writeZeros);

                        string funcName = GetGomPartFunctionName(S, FT, srcGroup, dstGroup);
                        cgd.m_gmvGomPartFuncNames[new Tuple<string, string>(FT.type, funcName)] = (code.Length > 0);

                        if (code.Length == 0) continue;

                        if (!S.m_GMV.IsGroupedByGrade(S.m_dimension))
                            code = code.Replace("=", "+=");

                        // check if code was already generated, and, if so, reuse it
                        if (generatedCode.ContainsKey(code))
                        {
                            // ready generated: call that function
                            code = "\t" + generatedCode[code] + "(" + nameGOM + ", " + nameSrcGMV + ", " + nameDstGMV + ");\n";
                        }
                        else
                        {
                            // not generated yet: remember code -> function
                            generatedCode[code] = funcName;
                        }

                        // write comment
                        string comment = "Computes the partial application of a general outermorphism to a general multivector";

                        string OM_PTR = "";
                        if (S.OutputC()) OM_PTR = "*";
                        else if (S.OutputCpp()) OM_PTR = "&";

                        string ACCESS = "";
                        if (S.OutputJava()) ACCESS = "protected static ";
                        else if (S.OutputCSharp()) ACCESS = "protected internal static ";

                        string ARR = (S.OutputCSharpOrJava()) ? "[] " : " *";
                        string CONST = (S.OutputCSharpOrJava()) ? "" : "const ";

                        string funcDecl = ACCESS + "void " + funcName + "(" + CONST + FT.GetMangledName(S, gom.Name) + " " + OM_PTR + nameGOM + ", " + CONST + FT.type + ARR + nameSrcGMV + ", " + FT.type + ARR + nameDstGMV + ")";

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


                    } // end of loop over all dest GMV groups
                } // end of loop over all source GMV groups
            } // end of loop over all float types
        } // end of function WriteGomParts()

                        
        /// <summary>
        /// Returns the code for applying a general outermorphism to a general multivector.
        /// 
        /// This function uses <c>cdg.m_gmvGomPartFuncNames</c>.
        /// 
        /// The returned code is only the body. The function declaration is not included.
        /// </summary>
        /// <param name="S">Specification of algebra (used for general multivector type, output language).</param>
        /// <param name="cgd">Used for <c>m_gmvGomPartFuncNames</c>.</param>
        /// <param name="FT">Floating point type.</param>
        /// <param name="FAI">Info about function arguments</param>
        /// <param name="resultName">Name of variable where the result goes (in the generated code).</param>
        public static string GetApplyGomCode(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT,
            G25.CG.Shared.FuncArgInfo[] FAI, string resultName)
        {
            if (S.OutputCppOrC()) return GetApplyGomCodeCppOrC(S, cgd, FT, FAI, resultName);
            else return GetApplyGomCodeCSharpOrJava(S, cgd, FT, FAI, resultName);
        }

        private static string GetApplyGomCodeCppOrC(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT,
            G25.CG.Shared.FuncArgInfo[] FAI, string resultName) 
        {
            G25.GMV gmv = S.m_GMV;
            bool groupedByGrade = gmv.IsGroupedByGrade(S.m_dimension);

            StringBuilder SB = new StringBuilder();

            // allocate memory to store result:
            SB.AppendLine("int idxB = 0, gu = 0, idxC = 0;");
            bool resultIsScalar = false;
            bool initResultToZero = !groupedByGrade;
            SB.Append(GPparts.GetExpandCode(S, cgd, FT, null, resultIsScalar, initResultToZero));

            string bgu = (S.OutputC()) ? FAI[1].Name + "->gu" : FAI[1].Name + ".gu()";
            string bc = (S.OutputC()) ? FAI[1].Name + "->c" : FAI[1].Name + ".getC()";

            // get number of groups:
            int nbGroups = gmv.NbGroups;

            int dstGrade = 0;
            int srcGradeSizeAccumulator = 0;

            // for each combination of groups, check if the OM goes from one to the other
            for (int srcGroup = 0; srcGroup < nbGroups; srcGroup++)
            {
                // increment the index into 'C' when we switch grade
                int srcGrade = gmv.Group(srcGroup)[0].Grade();
                if (srcGrade != dstGrade)
                {
                    if (!groupedByGrade)
                    {
                        SB.AppendLine("idxC += " + srcGradeSizeAccumulator + ";");
                        SB.AppendLine("");
                    }
                    dstGrade = srcGrade;
                    srcGradeSizeAccumulator = 0;
                }
                srcGradeSizeAccumulator += gmv.Group(srcGroup).Length;

                SB.AppendLine("if (" + bgu + " & " + (1 << srcGroup) + ") {");
                int dstGradeSizeAccumulator = 0;
                for (int dstGroup = 0; dstGroup < nbGroups; dstGroup++)
                {
                    string funcName = GetGomPartFunctionName(S, FT, srcGroup, dstGroup);
                    Tuple<string, string> key = new Tuple<string, string>(FT.type, funcName);
                    if (cgd.m_gmvGomPartFuncNames.ContainsKey(key) &&
                        cgd.m_gmvGomPartFuncNames[key])
                    {
                        SB.AppendLine("\t" + funcName + "(" + FAI[0].Name + ", " + bc + " + idxB, c + idxC + " + dstGradeSizeAccumulator + ");");
                    }
                    if (gmv.Group(dstGroup)[0].Grade() == srcGrade)
                        dstGradeSizeAccumulator += gmv.Group(dstGroup).Length;
                }
                if (srcGroup < (nbGroups - 1))
                {
                    SB.AppendLine("\tidxB += " + gmv.Group(srcGroup).Length + ";");
                    if (groupedByGrade)
                    {
                        SB.AppendLine("\tidxC += " + gmv.Group(srcGroup).Length + ";");
                        SB.AppendLine("");
                    }

                }
                SB.AppendLine("}");
                SB.AppendLine("");
            }

            // generate code to compress result
            string cgu = (groupedByGrade) ? bgu : null; // null means: all groups/grades are present
            SB.Append(GPparts.GetCompressCode(S, FT, FAI, resultName, resultIsScalar, cgu)); // todo: bgu should be 63 when not grouped by grade

            return SB.ToString();
        } // end of GetApplyGomCodeCppOrC

        private static string GetApplyGomCodeCSharpOrJava(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT,
            G25.CG.Shared.FuncArgInfo[] FAI, string resultName)
        {
            G25.GMV gmv = S.m_GMV;
            bool groupedByGrade = gmv.IsGroupedByGrade(S.m_dimension);

            StringBuilder SB = new StringBuilder();

            // allocate memory to store result:
            SB.AppendLine(FT.type + "[][] bc = " + FAI[1].Name + ".c();");
            bool resultIsScalar = false;
            bool initResultToZero = !groupedByGrade;
            SB.Append(GPparts.GetExpandCode(S, cgd, FT, null, resultIsScalar, initResultToZero));

            // get number of groups:
            int nbGroups = gmv.NbGroups;

            // for each combination of groups, check if the OM goes from one to the other
            for (int srcGroup = 0; srcGroup < nbGroups; srcGroup++)
            {

                SB.AppendLine("if (bc[" + srcGroup + "] != null) {");
                for (int dstGroup = 0; dstGroup < nbGroups; dstGroup++)
                {
                    string funcName = GetGomPartFunctionName(S, FT, srcGroup, dstGroup);
                    Tuple<string, string> key = new Tuple<string, string>(FT.type, funcName);
                    if (cgd.m_gmvGomPartFuncNames.ContainsKey(key) &&
                        cgd.m_gmvGomPartFuncNames[key])
                    {
                        SB.AppendLine("\t" + funcName + "(" + FAI[0].Name + ", bc[" + srcGroup + "], cc[" + dstGroup + "]);");
                    }
                }
                SB.AppendLine("}");
                SB.AppendLine("");
            }

            SB.AppendLine("return new " + FT.GetMangledName(S, gmv.Name) + "(cc);");

            return SB.ToString();
        } // end of GetApplyGomCodeCSharpOrJava()


    } // end of class GomParts
} // end of namepace G25.CG.Shared
