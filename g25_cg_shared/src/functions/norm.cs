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

namespace G25
{
    namespace CG
    {
        namespace Shared
        {
            namespace Func
            {
                /// <summary>
                /// Generates code for norm and norm2.
                /// 
                /// Norm function should be called <c>"norm"</c> and norm squared should be called <c>"norm2"</c>.
                /// 
                /// The metric can be specified using the <c>metric="metricName"</c> attribute.
                /// </summary>
                public class Norm : G25.CG.Shared.BaseFunctionGenerator
                {
                    /// <returns>true when F.Name == "norm"</returns>
                    public static bool IsNorm(G25.fgs F)
                    {
                        return F.Name == "norm";
                    }

                    /// <returns>true when F.Name == "norm2"</returns>
                    public static bool IsNorm2(G25.fgs F)
                    {
                        return F.Name == "norm2";
                    }

                    // constants, intermediate results
                    protected const int NB_ARGS = 1;
                    protected bool m_gmvFunc; ///< is this a function over GMVs?
                    protected RefGA.Multivector m_returnValue; ///< returned value (symbolic multivector)
                    protected bool m_isNorm2; ///< squared norm (true)? or regular norm (false)
                    protected G25.SMV m_smv = null; ///< if function over SMV, type goes here
                    protected Dictionary<string, string> m_funcName = new Dictionary<string, string>();  ///< generated function name with full mangling, etc

                    /// <summary>
                    /// Checks if this FunctionGenerator can implement a certain function.
                    /// </summary>
                    /// <param name="S">The specification of the algebra.</param>
                    /// <param name="F">The function to be implemented.</param>
                    /// <returns>true if 'F' can be implemented</returns>
                    public override bool CanImplement(Specification S, G25.fgs F)
                    {
                        return ((IsNorm(F) || IsNorm2(F)) && (F.MatchNbArguments(1)));
                    }

                    /// <summary>
                    /// If this FunctionGenerator can implement 'F', then this function should complete the (possible)
                    /// blanks in 'F'. This means:
                    ///  - Fill in F.m_returnTypeName if it is empty
                    ///  - Fill in F.m_argumentTypeNames (and m_argumentVariableNames) if it is empty.
                    /// </summary>
                    public override void CompleteFGS()
                    {
                        // fill in ArgumentTypeNames
                        if (m_fgs.ArgumentTypeNames.Length == 0)
                            m_fgs.m_argumentTypeNames = new String[] { m_gmv.Name };

                        // init argument pointers from the completed typenames (language sensitive);
                        m_fgs.InitArgumentPtrFromTypeNames(m_specification);

                        // get all function info
                        FloatType FT = m_specification.GetFloatType(m_fgs.FloatNames[0]);
                        bool computeMultivectorValue = true;
                        G25.CG.Shared.FuncArgInfo[] tmpFAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

                        m_gmvFunc = !tmpFAI[0].IsScalarOrSMV();

                        m_isNorm2 = IsNorm2(m_fgs);

                        // compute intermediate results, set return type
                        if (m_gmvFunc) m_fgs.m_returnTypeName = G25.CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, new RefGA.Multivector("x")).GetName(); // norm(gmv) = scalar
                        else
                        {
                            m_smv = tmpFAI[0].Type as G25.SMV;

                            // compute return value
                            if (m_isNorm2) m_returnValue = tmpFAI[0].MultivectorValue[0].Norm_r2(m_M);
                            else m_returnValue = RefGA.Symbolic.ScalarOp.Abs(tmpFAI[0].MultivectorValue[0].Norm_r(m_M));

                            // round value if required by metric
                            if (m_G25M.m_round) m_returnValue = m_returnValue.Round(1e-14);

                            // avoid null return value because that might return in getting the wrong return type
                            RefGA.Multivector nonZeroReturnValue = (m_returnValue.IsZero()) ? RefGA.Multivector.ONE : m_returnValue;

                            // get name of return type
                            if (m_fgs.m_returnTypeName.Length == 0)
                                m_fgs.m_returnTypeName = G25.CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, nonZeroReturnValue).GetName();
                        }
                    }

                    /// <summary>
                    /// Writes the declaration/definitions of 'F' to StringBuffer 'SB', taking into account parameters specified in specification 'S'.
                    /// </summary>
                    public override void WriteFunction()
                    {
                        G25.SMV scalarSMV = m_specification.GetScalarSMV();
                        bool scalarTypePresent = scalarSMV != null;

                        foreach (string floatName in m_fgs.FloatNames)
                        {
                            FloatType FT = m_specification.GetFloatType(floatName);

                            // hack to override floating point return type
                            if (m_specification.IsFloatType(m_fgs.ReturnTypeName))
                                m_fgs.ReturnTypeName = FT.type;

                            bool computeMultivectorValue = true;
                            G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_gmv.Name, computeMultivectorValue);

                            // generate comment
                            string comment = "/** " +
                                m_fgs.AddUserComment("Returns " + m_fgs.Name + " of " + FAI[0].TypeName + " using " + m_G25M.m_name + " metric.") + " */";

                            // because of lack of overloading, function names include names of argument types
                            G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, m_fgs, FAI);

                            // if scalar or specialized: generate specialized function
                            if (m_gmvFunc)
                            {
                                m_funcName[FT.type] = G25.CG.Shared.GmvGpParts.WriteGMVnormFunction(m_specification, m_cgd, FT, m_G25M, FAI, m_fgs, comment, m_isNorm2);
                            }
                            else
                            {
                                m_funcName[FT.type] = CF.OutputName;

                                // write out the function:
                                G25.CG.Shared.Functions.WriteSpecializedFunction(m_specification, m_cgd, CF, FT, FAI, m_returnValue, comment);
                            }

                            // write 'returns float' versions of norm
                            if (scalarTypePresent)
                            {
                                WriteNormReturnsScalar(FT, FAI, CF.OutputName, scalarSMV);
                            }
                            else
                            {
                                WriteNormReturnsScalarPassThrough(m_specification, FT, FAI, CF.OutputName);
                            }

                        }
                    } // end of WriteFunction

                    protected void WriteNormReturnsScalar(FloatType FT, G25.CG.Shared.FuncArgInfo[] FAI, String funcName, G25.SMV scalarSMV)
                    {
                        StringBuilder declSB = m_cgd.m_declSB;
                        StringBuilder defSB = (m_specification.m_inlineFunctions) ? m_cgd.m_inlineDefSB : m_cgd.m_defSB;

                        string inlineStr = G25.CG.Shared.Util.GetInlineString(m_specification, m_specification.m_inlineFunctions, " ");
                        string refPtrStr = (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C) ? "*" : "&";

                        string funcDecl = FT.type + " " + funcName + G25.CG.Shared.CANSparts.RETURNS_SCALAR + "(const " + FAI[0].MangledTypeName + " " + refPtrStr + FAI[0].Name + ")";
                        declSB.AppendLine("/** internal conversion function (this is just a pass through) */");
                        declSB.Append(funcDecl);
                        declSB.AppendLine(";");

                        defSB.Append(inlineStr + funcDecl);
                        defSB.AppendLine(" {");
                        if (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C)
                        {
                            defSB.AppendLine("\t" + FT.GetMangledName(m_specification, scalarSMV.Name) + " tmp;");
                            defSB.AppendLine("\t" + funcName + "(&tmp, " + FAI[0].Name + ");");
                        }
                        else
                        {
                            defSB.AppendLine("\t" + FT.GetMangledName(m_specification, scalarSMV.Name) + " tmp(" + funcName + "(" + FAI[0].Name + "));");
                        }

                        string[] accessStr;
                        {
                            bool ptr = false;
                            accessStr = G25.CG.Shared.CodeUtil.GetAccessStr(m_specification, scalarSMV, "tmp", ptr);
                        }

                        defSB.AppendLine("\treturn " + accessStr[0] + ";");

                        defSB.AppendLine("}");
                    }

                    protected void WriteNormReturnsScalarPassThrough(Specification S, FloatType FT, G25.CG.Shared.FuncArgInfo[] FAI, string funcName)
                    {
                        StringBuilder declSB = m_cgd.m_declSB;
                        StringBuilder defSB = (m_specification.m_inlineFunctions) ? m_cgd.m_inlineDefSB : m_cgd.m_defSB;

                        string inlineStr = G25.CG.Shared.Util.GetInlineString(m_specification, m_specification.m_inlineFunctions, " ");
                        string ptrSymbol = "";
                        if (S.m_outputLanguage == OUTPUT_LANGUAGE.C) ptrSymbol = "*";
                        else if (S.m_outputLanguage == OUTPUT_LANGUAGE.CPP) ptrSymbol = "&";

                        string funcDecl = FT.type + " " + funcName + G25.CG.Shared.CANSparts.RETURNS_SCALAR + "(const " + FAI[0].MangledTypeName + " " + ptrSymbol + FAI[0].Name + ")";
                        declSB.AppendLine("/** internal conversion function */");
                        declSB.Append(funcDecl);
                        declSB.AppendLine(";");

                        defSB.Append(inlineStr + funcDecl);
                        defSB.AppendLine(" {");
                        defSB.AppendLine("\treturn " + funcName + "(" + FAI[0].Name + ");");
                        defSB.AppendLine("}");
                    }

                    // used for testing:
                    protected Dictionary<string, string> m_randomScalarFuncName = new Dictionary<string, string>();
                    protected Dictionary<string, string> m_randomBladeFuncName = new Dictionary<string, string>();
                    protected Dictionary<string, string> m_subtractGmvFuncName = new Dictionary<string, string>();
                    protected Dictionary<string, string> m_randomSmvFuncName = new Dictionary<string, string>();
                    protected Dictionary<string, string> m_reverseFuncName = new Dictionary<string, string>();
                    protected Dictionary<string, string> m_gpFuncName = new Dictionary<string, string>();

                    /// <summary>
                    /// This function checks the dependencies for the _testing_ code of this function. If dependencies are
                    /// missing, the function adds the required functions (this is done simply by asking for them . . .).
                    /// </summary>
                    public override void CheckTestingDepencies()
                    {
                        //bool returnTrueName = true;
                        foreach (string floatName in m_fgs.FloatNames)
                        {
                            FloatType FT = m_specification.GetFloatType(floatName);

                            m_randomScalarFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + FT.type, new String[0], FT.type, FT, null);
                            m_subtractGmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "subtract", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);
                            m_reverseFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "reverse", new String[] { m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);
                            m_gpFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, m_G25M.m_name);

                            if (m_gmvFunc)
                            {
                                m_randomBladeFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_blade", new String[0], m_specification.m_GMV.Name, FT, null);
                            }
                            else if (m_smv != null)
                            {
                                string defaultReturnTypeName = null;
                                m_randomSmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_smv.Name, new String[0], defaultReturnTypeName, FT, null);
                            }
                        }
                    }

                    /// <summary>
                    /// Writes the testing function for 'F' to 'm_defSB'.
                    /// The generated function returns success (1) or failure (0).
                    /// </summary>
                    /// <returns>The list of name name of the int() function which tests the function.</returns>
                    public override List<string> WriteTestFunction()
                    {
                        StringBuilder defSB = (m_specification.m_inlineFunctions) ? m_cgd.m_inlineDefSB : m_cgd.m_defSB;

                        List<string> testFuncNames = new List<string>();


                        foreach (string floatName in m_fgs.FloatNames)
                        {
                            FloatType FT = m_specification.GetFloatType(floatName);

                            string testFuncName = Util.GetTestingFunctionName(m_specification, m_cgd, m_funcName[FT.type]);

                            string returnTypeName = m_fgs.m_returnTypeName;
                            // hack to override floating point return type
                            if (m_specification.IsFloatType(returnTypeName))
                                returnTypeName = FT.type;

                            if (m_gmvFunc) // GMV test
                            {
                                testFuncNames.Add(testFuncName);
                                System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                                argTable["S"] = m_specification;
                                argTable["FT"] = FT;
                                argTable["gmv"] = m_specification.m_GMV;
                                argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                                argTable["returnTypeName"] = FT.GetMangledName(m_specification, returnTypeName);
                                argTable["testFuncName"] = testFuncName;
                                argTable["targetFuncName"] = m_funcName[FT.type];
                                argTable["targetFuncReturnsFloat"] = m_specification.IsFloatType(m_fgs.ReturnTypeName);
                                argTable["normSquared"] = IsNorm2(m_fgs);
                                argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                                argTable["randomBladeFuncName"] = m_randomBladeFuncName[FT.type];
                                argTable["reverseFuncName"] = m_reverseFuncName[FT.type];
                                argTable["gpFuncName"] = m_gpFuncName[FT.type];
                                argTable["subtractGmvFuncName"] = m_subtractGmvFuncName[FT.type];
                                m_cgd.m_cog.EmitTemplate(defSB, "testNormGMV", argTable);
                            }
                            else if ((m_smv != null) && m_smv.CanConvertToGmv(m_specification)) // SMV test
                            {
                                testFuncNames.Add(testFuncName);
                                System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                                argTable["S"] = m_specification;
                                argTable["FT"] = FT;
                                argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                                argTable["smv"] = m_smv;
                                argTable["smvName"] = FT.GetMangledName(m_specification, m_smv.Name);

                                argTable["resultSmvName"] = FT.GetMangledName(m_specification, returnTypeName);

                                argTable["testFuncName"] = testFuncName;
                                argTable["targetFuncName"] = m_funcName[FT.type];
                                argTable["targetFuncReturnsFloat"] = m_specification.IsFloatType(m_fgs.ReturnTypeName);
                                argTable["normSquared"] = IsNorm2(m_fgs);
                                argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                                argTable["randomSmvFuncName"] = m_randomSmvFuncName[FT.type];
                                argTable["reverseFuncName"] = m_reverseFuncName[FT.type];
                                argTable["gpFuncName"] = m_gpFuncName[FT.type];
                                argTable["subtractGmvFuncName"] = m_subtractGmvFuncName[FT.type];
                                m_cgd.m_cog.EmitTemplate(defSB, "testNormSMV", argTable);
                            }
                        }

                        return testFuncNames;
                    } // end of WriteTestFunction()

                } // end of class Norm
            } // end of namespace Func
        } // end of namespace Shared
    } // end of namespace CG
} // end of namespace G25

