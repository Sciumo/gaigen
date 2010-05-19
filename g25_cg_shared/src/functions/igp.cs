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
    /// Generates code for the inverse geometric product (A * versorInverse(B))
    /// 
    /// The function should be called <c>"igp"</c>.
    /// 
    /// The metric can be specified using the <c>metric="metricName"</c> attribute.
    /// </summary>
    public class IGP : G25.CG.Shared.BaseFunctionGenerator
    {

        // constants, intermediate results
        protected const int NB_ARGS = 2;
        protected bool m_gmvFunc; ///< is this a function over GMVs and optionally floats?
        protected bool m_trueGmvFunc; ///< is this a function over only GMVs?
        protected bool m_arg1isGmv; ///< is first argument a GMV?
        protected bool m_arg2isGmv; ///< is second argument a GMV?
        protected RefGA.Multivector m_reverseValue;
        protected RefGA.Multivector m_n2Value;
        protected RefGA.Multivector m_returnValue; ///< returned value (symbolic multivector)
        protected String m_normSquaredName = "_n2_";
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
            int NB_ARGS = 2;
            //String type F.GetArgumentTypeName(0, S.m_GMV.Name);
            return ((F.Name == "igp") && (F.MatchNbArguments(NB_ARGS)) &&
                G25.CG.Shared.Functions.NotMixSmvGmv(S, F, NB_ARGS, S.m_GMV.Name) &&
                G25.CG.Shared.Functions.NotUseOm(S, F, NB_ARGS, S.m_GMV.Name));
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
                m_fgs.m_argumentTypeNames = new String[] { m_gmv.Name, m_gmv.Name };

            // init argument pointers from the completed typenames (language sensitive);
            m_fgs.InitArgumentPtrFromTypeNames(m_specification);

            // get all function info
            FloatType FT = m_specification.GetFloatType(m_fgs.FloatNames[0]);
            bool computeMultivectorValue = true;
            G25.CG.Shared.FuncArgInfo[] tmpFAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

            m_gmvFunc = !(tmpFAI[0].IsScalarOrSMV() && tmpFAI[1].IsScalarOrSMV());
            m_trueGmvFunc = (!tmpFAI[0].IsScalarOrSMV()) && (!tmpFAI[1].IsScalarOrSMV());
            m_arg1isGmv = !tmpFAI[0].IsScalarOrSMV();
            m_arg2isGmv = !tmpFAI[1].IsScalarOrSMV();
            m_smv1 = tmpFAI[0].Type as G25.SMV;
            m_smv2 = tmpFAI[1].Type as G25.SMV;

            // compute intermediate results, set return type
            if (m_gmvFunc) m_fgs.m_returnTypeName = m_gmv.Name;// gmv * gmv = gmv
            else
            {
                // compute return value
                m_reverseValue = RefGA.Multivector.Reverse(tmpFAI[1].MultivectorValue[0]);
                m_n2Value = RefGA.Multivector.gp(m_reverseValue, tmpFAI[1].MultivectorValue[0], m_M);
                if (m_G25M.m_round) m_n2Value = m_n2Value.Round(1e-14);

                //m_returnValue = RefGA.Multivector.gp(RefGA.Multivector.gp(tmpFAI[0].MultivectorValue[0], m_reverseValue, m_M),
                  //  RefGA.Symbolic.ScalarOp.Inverse(new RefGA.Multivector(new RefGA.BasisBlade(0, 1.0, m_normSquaredName))));
                m_returnValue = RefGA.Multivector.gp(tmpFAI[0].MultivectorValue[0], m_reverseValue, m_M);

                // round value if required by metric
                if (m_G25M.m_round) m_returnValue = m_returnValue.Round(1e-14);

                // get name of return type
                if (m_fgs.m_returnTypeName.Length == 0)
                    m_fgs.m_returnTypeName = G25.CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, m_returnValue).GetName();
            }
        }

        /// <summary>
        /// This function should check the dependencies of this function. If dependencies are
        /// missing, the function can complain (throw exception) or fix it (add the required functions).
        /// 
        /// If changes are made to the specification then it must be locked first because
        /// multiple threads run in parallel which may all modify the specification!
        /// </summary>
        public override void CheckDepencies()
        {
            if (m_gmvFunc)
            {
                // check dependencies for all float types
                foreach (String floatName in m_fgs.FloatNames)
                {
                    FloatType FT = m_specification.GetFloatType(floatName);
                    //bool returnTrueName = false;
                    G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "div", new String[] { m_gmv.Name, FT.type }, FT, null);
                    if (m_arg2isGmv)
                    {
                        G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "reverse", new String[] { m_gmv.Name }, FT, null);
                        G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "norm2", new String[] { m_gmv.Name }, FT, m_G25M.m_name);
                        if (m_arg1isGmv)
                        {
                            G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new String[] { m_gmv.Name, m_gmv.Name }, FT, m_G25M.m_name);
                        }
                        else
                        {
                            G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new String[] { m_gmv.Name, FT.type }, FT, m_G25M.m_name);
                        }
                    }
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

                // comment
                String comment = "/** " + m_fgs.AddUserComment("Returns " + FAI[0].Name + " * versorInverse(" + FAI[1].Name + ").") + " */";

                // if scalar or specialized: generate specialized function
                if (m_gmvFunc)
                {
                    m_funcName[FT.type] = G25.CG.Shared.GmvGpParts.WriteGMVigpFunction(m_specification, m_cgd, FT, m_G25M, FAI, m_fgs, comment);
                }
                else
                { // write simple specialized function:
                    // setup instructions
                    System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                    {
                        int nbTabs = 1;
                        bool mustCast = false;
                        bool n2Ptr = false;
                        bool declareN2 = true;

                        // n2 = reverse(input) * input
                        I.Add(new G25.CG.Shared.AssignInstruction(nbTabs, FT, FT, mustCast, m_n2Value, m_normSquaredName, n2Ptr, declareN2));

                        // return A * reverse(B) / norm2(B)
                        I.Add(new G25.CG.Shared.ReturnInstruction(nbTabs, m_specification.GetType(m_fgs.m_returnTypeName), FT, mustCast, m_returnValue, "/", new RefGA.Multivector(new RefGA.BasisBlade(0, 1.0, m_normSquaredName))));
                    }

                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, m_fgs, FAI);

                    m_funcName[FT.type] = CF.OutputName;

                    bool staticFunc = Functions.OutputStaticFunctions(m_specification);
                    G25.CG.Shared.Functions.WriteFunction(m_specification, m_cgd, CF, m_specification.m_inlineFunctions, staticFunc, CF.OutputName, FAI, I, comment);
                }
            }
        } // end of WriteFunction

        // used for testing:
        protected Dictionary<string, string> m_randomScalarFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomSmv1FuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomSmv2FuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomVersorFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_subtractGmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_gpFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_versorInverseFuncName = new Dictionary<string, string>();

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
                m_versorInverseFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "versorInverse", new String[] { m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, m_G25M.m_name);
                m_gpFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, m_G25M.m_name);

                // actual requirements:
                if (m_trueGmvFunc)
                {
                    m_randomVersorFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_versor", new String[0], m_specification.m_GMV.Name, FT, m_G25M.m_name);
                }
                else if ((m_smv1 != null) && (m_smv2 != null))
                {
                    string defaultReturnTypeName = null;

                    m_randomSmv1FuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_smv1.Name, new String[0], defaultReturnTypeName, FT, null);
                    m_randomSmv2FuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_smv2.Name, new String[0], defaultReturnTypeName, FT, null);
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

                if (m_trueGmvFunc) // GMV test
                {
                    testFuncNames.Add(testFuncName);
                    System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                    argTable["S"] = m_specification;
                    argTable["FT"] = FT;
                    argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                    argTable["testFuncName"] = testFuncName;
                    argTable["targetFuncName"] = m_funcName[FT.type];
                    argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                    argTable["randomVersorFuncName"] = m_randomVersorFuncName[FT.type];
                    argTable["subtractFuncName"] = m_subtractGmvFuncName[FT.type];
                    argTable["gpFuncName"] = m_gpFuncName[FT.type];
                    argTable["versorInverseFuncName"] = m_versorInverseFuncName[FT.type];

                    m_cgd.m_cog.EmitTemplate(defSB, "testIGP_GMV", argTable);
                }
                else if ((m_smv1 != null) && (m_smv2 != null) && 
                    m_smv1.CanConvertToGmv(m_specification) && m_smv2.CanConvertToGmv(m_specification) &&
                    (m_specification.GetType(m_fgs.m_returnTypeName) as G25.MV).CanConvertToGmv(m_specification))
                { // SMV test
                    testFuncNames.Add(testFuncName);
                    System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                    argTable["S"] = m_specification;
                    argTable["FT"] = FT;
                    argTable["testFuncName"] = testFuncName;
                    argTable["targetFuncName"] = m_funcName[FT.type];
                    argTable["smv1"] = m_smv1;
                    argTable["smv2"] = m_smv2;
                    argTable["smv1Name"] = FT.GetMangledName(m_specification, m_smv1.Name);
                    argTable["smv2Name"] = FT.GetMangledName(m_specification, m_smv2.Name);
                    argTable["smvRName"] = FT.GetMangledName(m_specification, m_fgs.m_returnTypeName);
                    argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                    argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                    argTable["randomSmv1FuncName"] = m_randomSmv1FuncName[FT.type];
                    argTable["randomSmv2FuncName"] = m_randomSmv2FuncName[FT.type];
                    argTable["gpFuncName"] = m_gpFuncName[FT.type];
                    argTable["subtractFuncName"] = m_subtractGmvFuncName[FT.type];
                    argTable["versorInverseFuncName"] = m_versorInverseFuncName[FT.type];

                    m_cgd.m_cog.EmitTemplate(defSB, "testIGP_SMV", argTable);
                }
            }

            return testFuncNames;
        } // end of WriteTestFunction()
       
    } // end of class IGP
} // end of namespace G25.CG.Shared.Func

