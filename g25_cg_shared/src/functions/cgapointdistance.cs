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

namespace G25.CG.Shared.Func
{
    /// <summary>
    /// Generates code for computing the distance of two (normalized) conformal model points,
    /// or the squared distance.
    /// 
    /// The function should be called <c>"cgaPointDistance"</c> or <c>"cgaPointDistance2"</c>.
    /// 
    /// Some examples of XML to request the function:
    /// <code>            
    /// <function name="cgaPointDistance" arg1="normalizedPoint" arg2="normalizedPoint" floatType="float"/>
    /// <function name="cgaPointDistance" arg1="dualSphere" arg2="dualSphere" floatType="double"/>
    /// <function name="cgaPointDistance2" arg1="normalizedPoint" arg2="normalizedPoint" floatType="float"/>
    /// <function name="cgaPointDistance2" arg1="dualSphere" arg2="dualSphere" floatType="double"/>
    /// </code>            
    /// 
    /// </summary>
    public class CgaPointDistance : G25.CG.Shared.BaseFunctionGenerator
    {

        // constants, intermediate results
        protected int NB_ARGS; ///< 1, or dimension of algebra
        protected RefGA.Multivector m_returnValue; ///< returned value (symbolic multivector)
        protected Dictionary<string, string> m_funcName = new Dictionary<string, string>();  ///< generated function name with full mangling, etc
        protected G25.SMV m_smv1 = null; ///< if function over SMV, type goes here
        protected G25.SMV m_smv2 = null; ///< if function over SMV, type goes here

        /// <returns>true when F.Name == "cgaPointDistance".</returns>
        public static bool IsDistance(G25.fgs F)
        {
            return (F.Name == "cgaPointDistance");
        }

        /// <returns>true when F.Name == "cgaPointDistance2".</returns>
        public static bool IsDistance2(G25.fgs F)
        {
            return (F.Name == "cgaPointDistance2");
        }

        /// <summary>
        /// Checks if this FunctionGenerator can implement a certain function.
        /// </summary>
        /// <param name="S">The specification of the algebra.</param>
        /// <param name="F">The function to be implemented.</param>
        /// <returns>true if 'F' can be implemented.</returns>
        public override bool CanImplement(Specification S, G25.fgs F)
        {
            return ((IsDistance(F) || IsDistance2(F)) && (F.m_argumentTypeNames.Length == 2));
        }

        /// <summary>
        /// If this FunctionGenerator can implement 'F', then this function should complete the (possible)
        /// blanks in 'F'. This means:
        ///  - Fill in F.m_returnTypeName if it is empty
        ///  - Fill in F.m_argumentTypeNames (and m_argumentVariableNames) if it is empty.
        /// 
        /// As the return type is required, many functions will also compute the return value and other info 
        /// needed for generating the code inside this function. These intermediate values are then
        /// stored in class variables so they can be reused in WriteFunction()
        /// </summary>
        public override void CompleteFGS()
        {
            NB_ARGS = m_fgs.NbArguments; // all arguments must be explicitly listed

            // in C language, all arguments are pointers
            if (m_fgs.ArgumentPtr.Length == 0)
            {
                bool ptr = m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C;
                m_fgs.m_argumentPtr = new bool[] { ptr, ptr };
            }

            // get all function info
            FloatType FT = m_specification.GetFloatType(m_fgs.FloatNames[0]);
            bool computeMultivectorValue = true;
            G25.CG.Shared.FuncArgInfo[] tmpFAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

            m_smv1 = tmpFAI[0].Type as G25.SMV;
            m_smv2 = tmpFAI[1].Type as G25.SMV;

            { // compute return value
                // compute symbolic distance squared value
                m_returnValue = RefGA.Multivector.gp(
                    RefGA.Multivector.ScalarProduct(tmpFAI[0].MultivectorValue[0], tmpFAI[1].MultivectorValue[0], m_M),
                    -2.0);

                // apply sqrt(fabs()) if distance should not be squared:
                if (IsDistance(m_fgs))
                    m_returnValue = RefGA.Symbolic.ScalarOp.Sqrt(RefGA.Symbolic.ScalarOp.Abs(m_returnValue));

                // round value if required by metric
                if (m_G25M.m_round)
                    m_returnValue = m_returnValue.Round(1e-14);
            }

            // get name of return type
            if (m_fgs.m_returnTypeName.Length == 0)
                m_fgs.m_returnTypeName = G25.CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, m_returnValue).GetName();
        } // end of CompleteFGS()

        /// <summary>
        /// This function should check the dependencies of this function. If dependencies are
        /// missing, the function can complain (throw DependencyException) or fix it (add the required functions).
        /// 
        /// If changes are made to the specification then it must be locked first because
        /// multiple threads run in parallel which may all modify the specification!
        /// 
        /// </summary>
        public override void CheckDepencies()
        {
            // subclass should override
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
                G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT,
                    m_specification.m_GMV.Name, computeMultivectorValue);

                // because of lack of overloading, function names include names of argument types
                G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, m_fgs, FAI);

                m_funcName[FT.type] = CF.OutputName;

                String comment;
                if (IsDistance(m_fgs)) comment = "/** " + m_fgs.AddUserComment("Returns distance of two conformal points.") + " */";
                else comment = "/** " + m_fgs.AddUserComment("Returns distance squared of two conformal points.") + " */";

                // write out the function:
                G25.CG.Shared.Functions.WriteSpecializedFunction(m_specification, m_cgd, CF, FT, FAI, m_returnValue, comment);
            }
        } // end of WriteFunction

        protected Dictionary<string, string> m_randomScalarFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_cgaPointFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_cgaPointTypeName = new Dictionary<string, string>();

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
                string[] cgaPointArgs = new String[m_specification.m_dimension - 2];
                for (int i = 0; i < cgaPointArgs.Length; i++) cgaPointArgs[i] = FT.type;

                string cgaPointFuncName = "cgaPoint";
                string defaultReturnTypeName = null;
                m_cgaPointFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, cgaPointFuncName, cgaPointArgs, defaultReturnTypeName, FT, null);
                fgs cgaPointFGS = m_specification.FindFunctionEx(cgaPointFuncName, cgaPointArgs, null, new String[] { FT.type }, null);
                m_cgaPointTypeName[FT.type] = cgaPointFGS.ReturnTypeName;
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

                if (m_smv1.CanConvertToGmv(m_specification) && m_smv2.CanConvertToGmv(m_specification))
                {
                    string testFuncName = Util.GetTestingFunctionName(m_specification, m_cgd, m_funcName[FT.type]);

                    testFuncNames.Add(testFuncName);
                    System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                    argTable["S"] = m_specification;
                    argTable["FT"] = FT;
                    argTable["testFuncName"] = testFuncName;
                    argTable["targetFuncName"] = m_funcName[FT.type];

                    argTable["squared"] = IsDistance2(m_fgs);

                    argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                    argTable["arg1TypeName"] = FT.GetMangledName(m_specification, m_smv1.Name);
                    argTable["arg2TypeName"] = FT.GetMangledName(m_specification, m_smv2.Name);

                    argTable["cgaPointName"] = FT.GetMangledName(m_specification, m_cgaPointTypeName[FT.type]);
                    argTable["cgaPointType"] = m_specification.GetType(m_cgaPointTypeName[FT.type]);

                    argTable["sqrt"] = CodeUtil.OpNameToLangString(m_specification, FT, ScalarOp.SQRT);
                    argTable["fabs"] = CodeUtil.OpNameToLangString(m_specification, FT, ScalarOp.ABS);

                    argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                    argTable["c3gaPointFuncName"] = m_cgaPointFuncName[FT.type];

                    m_cgd.m_cog.EmitTemplate(defSB, "testCgaPointDistance", argTable);
                }
            }

            return testFuncNames;
        } // end of WriteTestFunction()




    } // end of class CgaPointDistance
} // end of namespace G25.CG.Shared.Func

