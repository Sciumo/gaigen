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

namespace G25.CG.Shared.Func
{
    /// <summary>
    /// Generates code applying an outermorphism to a multivector.
    /// 
    /// The function should be called <c>"applyOM"</c>.
    /// </summary>
    public class ApplyOM : G25.CG.Shared.BaseFunctionGenerator
    {

        // constants, intermediate results
        protected const int NB_ARGS = 2;
        protected bool m_gmvFunc;
        protected G25.OM m_om;
        protected G25.MV m_mv;
        protected RefGA.Multivector m_returnValue; ///< returned value (symbolic multivector)
        protected Dictionary<string, string> m_funcName = new Dictionary<string, string>();  ///< generated function name with full mangling, etc

        /// <summary>
        /// Checks if this FunctionGenerator can implement a certain function.
        /// </summary>
        /// <param name="S">The specification of the algebra.</param>
        /// <param name="F">The function to be implemented.</param>
        /// <returns>true if 'F' can be implemented.</returns>
        public override bool CanImplement(Specification S, G25.fgs F)
        {
            int NB_ARGS = 2;
            // do a basic check first:
            bool OK1 = ((F.Name == "applyOM") &&
                (F.MatchNbArguments(NB_ARGS)) &&
                G25.CG.Shared.Functions.IsOm(S, F, 0) && // argument 0 must be an outermorphism
                G25.CG.Shared.Functions.IsMv(S, F, 1)); // argument 1 must be an multivector
            if (!OK1) return false;

            // specialized outermorphisms can only be applied to specialized multivectors
            if (G25.CG.Shared.Functions.IsSom(S, F, 0) &&
                (!G25.CG.Shared.Functions.IsSmv(S, F, 1))) return false;

            return true;
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
            {
                if (m_specification.m_GOM == null)
                    throw new G25.UserException("No general outermorphism type defined, while it is required because the type of the first argument was unspecified.");
                m_fgs.m_argumentTypeNames = new String[] { m_specification.m_GOM.Name, m_gmv.Name };
            }

            // init argument pointers from the completed typenames (language sensitive);
            m_fgs.InitArgumentPtrFromTypeNames(m_specification);

            // get all function info
            FloatType FT = m_specification.GetFloatType(m_fgs.FloatNames[0]);
            bool computeMultivectorValue = true;
            G25.CG.Shared.FuncArgInfo[] tmpFAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

            m_om = (G25.OM)tmpFAI[0].Type;
            m_mv = (G25.MV)tmpFAI[1].Type;
            m_gmvFunc = !tmpFAI[1].IsScalarOrSMV();

            // compute intermediate results, set return type
            if (m_gmvFunc) m_fgs.m_returnTypeName = m_gmv.Name;// gmv + gmv = gmv
            else
            {
                RefGA.Multivector inputValue = tmpFAI[1].MultivectorValue[0];

                // Compute m_returnValue:
                // Replace each basis blade in 'inputValue' with its value under the outermorphism.
                m_returnValue = RefGA.Multivector.ZERO;
                for (int i = 0; i < inputValue.BasisBlades.Length; i++)
                {
                    // get input blade and domain for that grade
                    RefGA.BasisBlade inputBlade = inputValue.BasisBlades[i];
                    RefGA.BasisBlade[] domainBlades = m_om.DomainForGrade(inputBlade.Grade());
                    for (int c = 0; c < domainBlades.Length; c++)
                    {
                        // if a match is found in the domain, add range vector to m_returnValue
                        if (domainBlades[c].bitmap == inputBlade.bitmap)
                        {
                            bool ptr = m_specification.OutputC();
                            RefGA.Multivector omColumnValue = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(m_specification, m_om.DomainSmvForGrade(inputBlade.Grade())[c], tmpFAI[0].Name, ptr);
                            RefGA.Multivector inputBladeScalarMultiplier = new RefGA.Multivector(new RefGA.BasisBlade(inputBlade, 0));
                            RefGA.Multivector domainBladeScalarMultiplier = new RefGA.Multivector(new RefGA.BasisBlade(domainBlades[c], 0));
                            m_returnValue = RefGA.Multivector.Add(m_returnValue,
                                RefGA.Multivector.gp(
                                RefGA.Multivector.gp(omColumnValue, inputBladeScalarMultiplier),
                                domainBladeScalarMultiplier));
                            break; // no need to search the other domainBlades too
                        }
                    }
                }

                // get name of return type
                if (m_fgs.m_returnTypeName.Length == 0)
                    m_fgs.m_returnTypeName = G25.CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, m_returnValue).GetName();
            }
        }


        /// <summary>
        /// Write the declaration/definition of 'F' to 'm_declSB', 'm_defSB' and 'm_inlineDefSB',
        /// taking into account parameters specified in specification 'S'.
        /// </summary>
        public override void WriteFunction()
        {
            foreach (String floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);
                bool computeMultivectorValue = true;
                G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

                // comment
                String comment = "/** " +
                    m_fgs.AddUserComment("Returns " + FAI[0].TypeName + " * " + FAI[1].TypeName + ".") + " */";

                // if scalar or specialized: generate specialized function
                if (m_gmvFunc)
                {
                    m_funcName[FT.type] = G25.CG.Shared.GmvGomParts.WriteApplyOmFunction(m_specification, m_cgd, FT, FAI, m_fgs, comment);
                }
                else
                { // write simple specialized function:
                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, m_fgs, FAI);
                    m_funcName[FT.type] = CF.OutputName;

                    // write out the function:
                    G25.CG.Shared.Functions.WriteSpecializedFunction(m_specification, m_cgd, CF, FT, FAI, m_returnValue, comment);
                }
            }

        } // end of WriteFunction



        // used for testing:
        protected Dictionary<string, string> m_randomScalarFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomBladeFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_subtractGmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_opGmvFuncName = new Dictionary<string, string>();

        protected Dictionary<string, string> m_randomRangeVectorFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomDomainSmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_gmvApplyOmFuncName = new Dictionary<string, string>();

        private G25.SMV getRangeVectorType(FloatType FT)
        {
            return (G25.SMV)G25.CG.Shared.SpecializedReturnType.FindTightestMatch(m_specification, new RefGA.Multivector(m_om.RangeVectors), FT);
        }

        private G25.SMV getGomRangeVectorType(FloatType FT)
        {
            return (G25.SMV)G25.CG.Shared.SpecializedReturnType.FindTightestMatch(m_specification, new RefGA.Multivector(m_specification.m_GOM.RangeVectors), FT);
        }

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

                // actual requirements:
                if (m_gmvFunc)
                {
                    m_randomBladeFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_blade", new String[0], m_specification.m_GMV.Name, FT, null);
                    m_subtractGmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "subtract", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);
                    m_opGmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "op", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);
                }
                else if (m_specification.m_GOM != null) // test for applyOM(SMV) can only be performed when GOM exists
                {
                    string defaultReturnTypeName = null;

                    G25.SMV gomRangeVectorType = getGomRangeVectorType(FT);

                    m_randomRangeVectorFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + gomRangeVectorType.Name, new String[0], defaultReturnTypeName, FT, null);
                    m_randomDomainSmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_mv.Name, new String[0], defaultReturnTypeName, FT, null);
                    m_subtractGmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "subtract", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, defaultReturnTypeName, FT, null);
                    m_gmvApplyOmFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "applyOM", new String[] { m_specification.m_GOM.Name, m_specification.m_GMV.Name }, defaultReturnTypeName, FT, null);
                }
            }
        }




        public override List<string> WriteTestFunction()
        {
            StringBuilder defSB = (m_specification.m_inlineFunctions) ? m_cgd.m_inlineDefSB : m_cgd.m_defSB;

            List<string> testFuncNames = new List<string>();

            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                string testFuncName = Util.GetTestingFunctionName(m_specification, m_cgd, m_funcName[FT.type]);

                if (m_gmvFunc)
                {
                    testFuncNames.Add(testFuncName);

                    System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                    argTable["S"] = m_specification;
                    argTable["FT"] = FT;
                    argTable["gmv"] = m_specification.m_GMV;
                    argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                    argTable["gomName"] = FT.GetMangledName(m_specification, m_specification.m_GOM.Name);
                    argTable["testFuncName"] = testFuncName;
                    argTable["targetFuncName"] = m_funcName[FT.type];
                    argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                    argTable["randomBladeFuncName"] = m_randomBladeFuncName[FT.type];
                    argTable["subtractGmvFuncName"] = m_subtractGmvFuncName[FT.type];
                    argTable["opGmvFuncName"] = m_opGmvFuncName[FT.type];
                    m_cgd.m_cog.EmitTemplate(defSB, "testApplyOM_GMV", argTable);
                }
                else if (m_specification.m_GOM != null)
                {
                    testFuncNames.Add(testFuncName);

//                                G25.SMV rangeVectorType = getRangeVectorType(FT);
                    G25.SMV gomRangeVectorType = getGomRangeVectorType(FT);

                    System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                    argTable["S"] = m_specification;
                    argTable["FT"] = FT;
                    argTable["testFuncName"] = testFuncName;
                    argTable["targetFuncName"] = m_funcName[FT.type];
                    argTable["gmv"] = m_specification.m_GMV;
                    argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                    argTable["gomName"] = FT.GetMangledName(m_specification, m_specification.m_GOM.Name);
                    argTable["testOm"] = m_om;
                    argTable["testOmName"] = FT.GetMangledName(m_specification, m_om.Name);
                    argTable["rangeVectorName"] = FT.GetMangledName(m_specification, gomRangeVectorType.Name);
                    argTable["domainSmv"] = m_mv;
                    argTable["domainSmvName"] = FT.GetMangledName(m_specification, m_mv.Name);
                    argTable["rangeSmvName"] = FT.GetMangledName(m_specification, m_fgs.m_returnTypeName);
                    argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                    argTable["randomRangeVectorFunc"] = m_randomRangeVectorFuncName[FT.type];
                    argTable["randomDomainSmvFunc"] = m_randomDomainSmvFuncName[FT.type];
                    argTable["subtractGmvFuncName"] = m_subtractGmvFuncName[FT.type];
                    argTable["gmvApplyOmFunc"] = m_gmvApplyOmFuncName[FT.type];
                    m_cgd.m_cog.EmitTemplate(defSB, "testApplyOM_SMV", argTable);
                }
            }

            return testFuncNames;
        }



    } // end of class ApplyOM
} // end of namespace G25.CG.Shared.Func
