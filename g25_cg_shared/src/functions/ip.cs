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
                /// Generates code for the inner product (five variants: hip, mhip, lc, rc, sp).
                /// 
                /// The following function names are possible:
                ///   - <c>"hip"</c>: Hestenes inner product.
                ///   - <c>"mhip"</c>: Modified Hestenes inner product.
                ///   - <c>"lc"</c>: left contraction.
                ///   - <c>"rc"</c>: right contraction.
                ///   - <c>"sp"</c>: scalar product.
                /// 
                /// The metric can be specified using the <c>optionMetric="metricName"</c> attribute.
                /// </summary>
                public class IP : G25.CG.Shared.BaseFunctionGenerator
                {

                    /// <returns>true when F.Name == "hip" (Hestenes Inner Product)</returns>
                    public static bool IsHip(G25.fgs F)
                    {
                        return F.Name == "hip";
                    }
                    /// <returns>true when F.Name == "mhip" (Modified Hestenes Inner Product)</returns>
                    public static bool IsMHip(G25.fgs F)
                    {
                        return F.Name == "mhip";
                    }
                    /// <returns>true when F.Name == "lc" (left contraction)</returns>
                    public static bool IsLc(G25.fgs F)
                    {
                        return F.Name == "lc";
                    }
                    /// <returns>true when F.Name == "rc" (right contraction)</returns>
                    public static bool IsRc(G25.fgs F)
                    {
                        return F.Name == "rc";
                    }
                    /// <returns>true when F.Name == "sp" (Scalar Product)</returns>
                    public static bool IsSp(G25.fgs F)
                    {
                        return F.Name == "sp";
                    }

                    /// <summary>
                    /// Converts the name of the function to a RefGA.BasisBlade.InnerProductType.
                    /// </summary>
                    /// <param name="S"></param>
                    /// <param name="F">Function (used for <c>F.name</c>).</param>
                    /// <returns>identifier for inner product type.</returns>
                    public static RefGA.BasisBlade.InnerProductType GetIpType(Specification S, G25.fgs F)
                    {
                        if (IsHip(F)) return RefGA.BasisBlade.InnerProductType.HESTENES_INNER_PRODUCT;
                        else if (IsMHip(F)) return RefGA.BasisBlade.InnerProductType.MODIFIED_HESTENES_INNER_PRODUCT;
                        else if (IsLc(F)) return RefGA.BasisBlade.InnerProductType.LEFT_CONTRACTION;
                        else if (IsRc(F)) return RefGA.BasisBlade.InnerProductType.RIGHT_CONTRACTION;
                        else if (IsSp(F)) return RefGA.BasisBlade.InnerProductType.SCALAR_PRODUCT;
                        else throw new G25.UserException("Unknown inner product type: " + F.Name, S.FunctionToXmlString(F));
                    }

                    /// <summary>
                    /// Converts the name of the function to a human readable name.
                    /// </summary>
                    /// <param name="S"></param>
                    /// <param name="F">Function (used for <c>F.name</c>).</param>
                    /// <returns>Human readable version of Function <c>F.name</c>.</returns>
                    public static String GetIpName(Specification S, G25.fgs F)
                    {
                        if (IsHip(F)) return "Hestenes inner product";
                        else if (IsMHip(F)) return "Modified Hestenes inner product";
                        else if (IsLc(F)) return "left contraction";
                        else if (IsRc(F)) return "right contraction";
                        else if (IsSp(F)) return "scalar product";
                        else throw new G25.UserException("G25.CG.C.IP.GetIpName(): unknown inner product type: " + F.Name, S.FunctionToXmlString(F));
                    }

                    // constants, intermediate results
                    protected const int NB_ARGS = 2;
                    protected bool m_gmvFunc; ///< is this a function over GMVs?
                    protected RefGA.Multivector m_returnValue; ///< returned value (symbolic multivector)
                    RefGA.BasisBlade.InnerProductType m_ipType; ///< type of inner product (left, right, Hestenes, etc)
                    protected G25.SMV m_smv1 = null; ///< if function over SMV, type goes here
                    protected G25.SMV m_smv2 = null; ///< if function over SMV, type goes here
                    protected Dictionary<string, string> m_funcName = new Dictionary<string, string>();  ///< generated function name with full mangling, etc

                    /// <summary>
                    /// Checks if this FunctionGenerator can implement a certain function.
                    /// </summary>
                    /// <param name="S">The specification of the algebra.</param>
                    /// <param name="F">The function to be implemented.</param>
                    /// <returns>true if 'F' can be implemented</returns>
                    public override bool CanImplement(Specification S, G25.fgs F)
                    {
                        return ((IsHip(F) || IsMHip(F) || IsLc(F) || IsRc(F) || IsSp(F)) &&
                            (F.MatchNbArguments(2)));
                    }

                    /// <summary>
                    /// If this FunctionGenerator can implement 'F', then this function should complete the (possible)
                    /// blanks in 'F'. This means:
                    ///  - Fill in F.m_returnTypeName if it is empty
                    ///  - Fill in F.m_argumentTypeNames (and m_argumentVariableNames) if it is empty.
                    /// </summary>
                    public override void CompleteFGS()
                    {
                        m_ipType = GetIpType(m_specification, m_fgs);

                        // fill in ArgumentTypeNames
                        if (m_fgs.ArgumentTypeNames.Length == 0)
                            m_fgs.m_argumentTypeNames = new String[] { m_gmv.Name, m_gmv.Name };

                        // init argument pointers from the completed typenames (language sensitive);
                        m_fgs.InitArgumentPtrFromTypeNames(m_specification);

                        // get all function info
                        FloatType FT = m_specification.GetFloatType(m_fgs.FloatNames[0]);
                        bool computeMultivectorValue = true;
                        G25.CG.Shared.FuncArgInfo[] tmpFAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

                        m_gmvFunc = !(tmpFAI[0].IsScalarOrSMV() && tmpFAI[1].IsScalarOrSMV());
                        m_smv1 = tmpFAI[0].Type as G25.SMV;
                        m_smv2 = tmpFAI[1].Type as G25.SMV;

                        // compute intermediate results, set return type
                        if (m_gmvFunc) m_fgs.m_returnTypeName = m_gmv.Name;// gmv * gmv = gmv
                        else
                        {
                            // compute return value
                            m_returnValue = RefGA.Multivector.ip(tmpFAI[0].MultivectorValue[0], tmpFAI[1].MultivectorValue[0], m_M, m_ipType);

                            // round value if required by metric
                            if (m_G25M.m_round) m_returnValue = m_returnValue.Round(1e-14);

                            // get name of return type
                            if ((m_fgs.ReturnTypeName.Length == 0) && (m_ipType != RefGA.BasisBlade.InnerProductType.SCALAR_PRODUCT))
                            {
                                m_fgs.ReturnTypeName = G25.CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, m_returnValue).GetName();
                            }
                        }
                    }



                    /// <summary>
                    /// Writes the declaration/definitions of 'F' to StringBuffer 'SB', taking into account parameters specified in specification 'S'.
                    /// </summary>
                    public override void WriteFunction()
                    {
                        foreach (String floatName in m_fgs.FloatNames)
                        {
                            FloatType FT = m_specification.GetFloatType(floatName);

                            bool computeMultivectorValue = true;
                            G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_gmv.Name, computeMultivectorValue);

                            // generate comment
                            String comment = "/** " +
                                m_fgs.AddUserComment("Returns " + GetIpName(m_specification, m_fgs) + " of " + FAI[0].TypeName + " and " + FAI[1].TypeName + ".") + " */";

                            // if scalar or specialized: generate specialized function
                            if (m_gmvFunc)
                            {
                                // translate the IP type:
                                G25.CG.Shared.GPparts.ProductTypes T;
                                switch (m_ipType)
                                {
                                    case RefGA.BasisBlade.InnerProductType.LEFT_CONTRACTION:
                                        T = G25.CG.Shared.GPparts.ProductTypes.LEFT_CONTRACTION;
                                        break;
                                    case RefGA.BasisBlade.InnerProductType.RIGHT_CONTRACTION:
                                        T = G25.CG.Shared.GPparts.ProductTypes.RIGHT_CONTRACTION;
                                        break;
                                    case RefGA.BasisBlade.InnerProductType.HESTENES_INNER_PRODUCT:
                                        T = G25.CG.Shared.GPparts.ProductTypes.HESTENES_INNER_PRODUCT;
                                        break;
                                    case RefGA.BasisBlade.InnerProductType.MODIFIED_HESTENES_INNER_PRODUCT:
                                        T = G25.CG.Shared.GPparts.ProductTypes.MODIFIED_HESTENES_INNER_PRODUCT;
                                        break;
                                    case RefGA.BasisBlade.InnerProductType.SCALAR_PRODUCT:
                                        T = G25.CG.Shared.GPparts.ProductTypes.SCALAR_PRODUCT;
                                        break;
                                    default:
                                        throw new Exception("G25.CG.C.IP.WriteFunction(): unknown inner product type: " + m_fgs.Name);
                                }

                                // write function
                                m_funcName[FT.type] = G25.CG.Shared.GmvGpParts.WriteGMVgpFunction(m_specification, m_cgd, FT, m_G25M, FAI, m_fgs, comment, T);
                            }
                            else
                            {   // write simple specialized function:
                                // because of lack of overloading, function names include names of argument types
                                G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, m_fgs, FAI);
                                m_funcName[FT.type] = CF.OutputName;

                                if ((CF.ReturnTypeName.Length == 0) && (m_ipType == RefGA.BasisBlade.InnerProductType.SCALAR_PRODUCT))
                                    CF.ReturnTypeName = FT.type;

                                // write out the function:
                                G25.CG.Shared.Functions.WriteSpecializedFunction(m_specification, m_cgd, CF, FT, FAI, m_returnValue, comment);
                            }

                        }
                    } // end of WriteFunction

                    // used for testing:
                    protected Dictionary<string, string> m_randomScalarFuncName = new Dictionary<string, string>();
                    protected Dictionary<string, string> m_randomBladeFuncName = new Dictionary<string, string>();
                    protected Dictionary<string, string> m_subtractGmvFuncName = new Dictionary<string, string>();
                    protected Dictionary<string, string> m_gpGmvFuncName = new Dictionary<string, string>();
                    protected Dictionary<string, string> m_extractGradeFuncName = new Dictionary<string, string>();
                    protected Dictionary<string, string> m_randomSmv1FuncName = new Dictionary<string, string>();
                    protected Dictionary<string, string> m_randomSmv2FuncName = new Dictionary<string, string>();
                    protected Dictionary<string, string> m_gmvProductFuncName = new Dictionary<string, string>();


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

                            // actual requirements:
                            if (m_gmvFunc)
                            {
                                m_randomBladeFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_blade", new String[0], m_specification.m_GMV.Name, FT, null);
                                m_gpGmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, m_G25M.m_name);
                                m_extractGradeFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "extractGrade", new String[] { m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);
                            }
                            else if ((m_smv1 != null) && (m_smv2 != null))
                            {
                                string defaultReturnTypeName = null;

                                m_randomSmv1FuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_smv1.Name, new String[0], defaultReturnTypeName, FT, null);
                                m_randomSmv2FuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_smv2.Name, new String[0], defaultReturnTypeName, FT, null);
                                m_gmvProductFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, m_fgs.Name, new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, m_G25M.m_name);
                            }
                        }
                    } // end of CheckTestingDepencies()

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

                            if (m_gmvFunc) // GMV test
                            {
                                System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                                testFuncNames.Add(testFuncName);

                                argTable["S"] = m_specification;
                                argTable["FT"] = FT;
                                argTable["gmv"] = m_specification.m_GMV;
                                argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                                argTable["testFuncName"] = testFuncName;
                                argTable["targetFuncName"] = m_funcName[FT.type];
                                argTable["targetFuncReturnsFloat"] = m_specification.IsFloatType(m_fgs.ReturnTypeName) || (m_ipType == RefGA.BasisBlade.InnerProductType.SCALAR_PRODUCT);
                                argTable["productName"] = m_fgs.Name;
                                argTable["targetFuncName"] = m_funcName[FT.type];
                                argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                                argTable["randomBladeFuncName"] = m_randomBladeFuncName[FT.type];
                                argTable["subtractGmvFuncName"] = m_subtractGmvFuncName[FT.type];
                                argTable["gpGmvFuncName"] = m_gpGmvFuncName[FT.type];
                                argTable["extractGradeFuncName"] = m_extractGradeFuncName[FT.type];

                                m_cgd.m_cog.EmitTemplate(defSB, "testFilteredGpGMV", argTable);
                            }
                            else if ((m_smv1 != null) && (m_smv2 != null) && m_smv1.CanConvertToGmv(m_specification) && m_smv2.CanConvertToGmv(m_specification))
                            { // SMV test
                                testFuncNames.Add(testFuncName);
                                System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                                argTable["S"] = m_specification;
                                argTable["FT"] = FT;
                                argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                                argTable["testFuncName"] = testFuncName;
                                argTable["targetFuncName"] = m_funcName[FT.type];
                                argTable["targetFuncReturnsFloat"] = m_specification.IsFloatType(m_fgs.ReturnTypeName) || (m_ipType == RefGA.BasisBlade.InnerProductType.SCALAR_PRODUCT);
                                argTable["smv1"] = m_smv1;
                                argTable["smv1Name"] = FT.GetMangledName(m_specification, m_smv1.Name);
                                argTable["smv2"] = m_smv2;
                                argTable["smv2Name"] = FT.GetMangledName(m_specification, m_smv2.Name);
                                argTable["resultSmvName"] = FT.GetMangledName(m_specification, m_fgs.m_returnTypeName);
                                argTable["productName"] = m_fgs.Name;
                                argTable["targetFuncName"] = m_funcName[FT.type];
                                argTable["randomSmv1FuncName"] = m_randomSmv1FuncName[FT.type];
                                argTable["randomSmv2FuncName"] = m_randomSmv2FuncName[FT.type];
                                argTable["subtractGmvFuncName"] = m_subtractGmvFuncName[FT.type];
                                argTable["gmvProductFuncName"] = m_gmvProductFuncName[FT.type];

                                m_cgd.m_cog.EmitTemplate(defSB, "testFilteredGpSMV", argTable);
                            }
                        }

                        return testFuncNames;
                    } // end of WriteTestFunction()


                } // end of class IP
            } // end of namespace Functions
        } // end of namespace Shared
    } // end of namespace CG
} // end of namespace G25

