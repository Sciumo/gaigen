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
    /// Generates code for check whether a multivector is zero (up to some epsilon).
    /// 
    /// The function should be called <c>"zero"</c>.
    /// </summary>
    public class Zero : G25.CG.Shared.BaseFunctionGenerator
    {

        /// <returns>true when <c>F.Name == "zero"</c>.</returns>
        public static bool IsZero(G25.fgs F)
        {
            return F.Name == "zero";
        }

        // constants, intermediate results
        protected const int NB_ARGS = 2;
        protected bool m_gmvFunc; ///< is this a function over GMVs?
        protected G25.SMV m_smv = null; ///< if function over SMV, type goes here
        protected Dictionary<string, string> m_funcName = new Dictionary<string, string>();  ///< generated function name with full mangling, etc

        /// <summary>
        /// Checks if this FunctionGenerator can implement a certain function.
        /// </summary>
        /// <param name="S">The specification of the algebra.</param>
        /// <param name="F">The function to be implemented.</param>
        /// <returns>true if 'F' can be implemented.</returns>
        public override bool CanImplement(Specification S, G25.fgs F)
        {
            return (IsZero(F) && (F.MatchNbArguments(2)));
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
                m_fgs.m_argumentTypeNames = new String[] { m_gmv.Name, m_fgs.FloatNames[0] };

            // init argument pointers from the completed typenames (language sensitive);
            m_fgs.InitArgumentPtrFromTypeNames(m_specification);

            // get all function info
            FloatType FT = m_specification.GetFloatType(m_fgs.FloatNames[0]);
            bool computeMultivectorValue = true;
            G25.CG.Shared.FuncArgInfo[] tmpFAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

            m_gmvFunc = !tmpFAI[0].IsScalarOrSMV();
            m_smv = tmpFAI[0].Type as G25.SMV;
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
                G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

                // comment
                String comment = "/** " +
                    m_fgs.AddUserComment("Returns true if all coordinates of " + FAI[0].Name + " are abs <= " + FAI[1].Name) + " */";

                // if scalar or specialized: generate specialized function
                if (m_gmvFunc)
                {
                    m_funcName[FT.type] = G25.CG.Shared.GmvCASNparts.WriteEqualsOrZeroOrGradeBitmapFunction(m_specification, m_cgd, FT, FAI, m_fgs, comment, G25.CG.Shared.CANSparts.EQUALS_ZERO_GRADEBITMAP_TYPE.ZERO);
                }
                else
                {// write simple specialized function:
                    // because of lack of overloading, function names include names of argument types
                    //G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, m_fgs, FAI);

                    // write out the function:
                    WriteZeroFunction(FT, FAI, m_fgs, comment);
                }

            }
        } // end of WriteFunction


        /// <summary>
        /// Writes an zero test function for specialized multivectors.
        /// </summary>
        /// <param name="FT"></param>
        /// <param name="FAI"></param>
        /// <param name="F"></param>
        /// <param name="comment"></param>
        protected void WriteZeroFunction(FloatType FT, G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F, String comment)
        {
            // setup instructions
            System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();

            string BOOL = (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C) ? "int" : "bool";
            string TRUE = (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C) ? "1" : "true";
            string FALSE = (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C) ? "0" : "false";

            // write the function:
            {
                int nbTabs = 1;
                // add one instruction (verbatim code)
                //I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, code));

                // get largest constant coordinate:
                double largestConstCoord = 0.0;
                if (m_smv != null)
                {
                    // first find largest const coord: if any const coord above eps, always return 0
                    for (int c = 0; c < m_smv.NbConstBasisBlade; c++)
                    {
                        if (Math.Abs(m_smv.ConstBasisBlade(c).scale) > largestConstCoord)
                            largestConstCoord = Math.Abs(m_smv.ConstBasisBlade(c).scale);
                    }
                }

                if (largestConstCoord > 0.0) // generate code to check if largest coord > 'eps'
                {

                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs,
                        "if (" + FT.DoubleToString(m_specification, largestConstCoord) + " > " + FAI[1].Name + ") return " + FALSE + ";"));
                }

                // generate code to check all coordiantes
                string[] accessStr = G25.CG.Shared.CodeUtil.GetAccessStr(m_specification, m_smv, FAI[0].Name, FAI[0].Pointer);
                for (int i = 0; i < accessStr.Length; i++)
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs,
                        "if ((" + accessStr[i] + " < -" + FAI[1].Name + ") || (" + accessStr[i] + " > " + FAI[1].Name + ")) return " + FALSE + ";"));

                // finally, return 1 if all check were OK
                I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, "return " + TRUE + ";"));
            }

            // because of lack of overloading, function names include names of argument types
            G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, F, FAI);
            m_funcName[FT.type] = CF.OutputName;

            // setup return type and argument:
            string returnTypeName = BOOL;
            G25.CG.Shared.FuncArgInfo returnArgument = null;

            // write function
            bool staticFunc = Functions.OutputStaticFunctions(m_specification);
            G25.CG.Shared.Functions.WriteFunction(m_specification, m_cgd, F, m_specification.m_inlineFunctions, staticFunc, returnTypeName, CF.OutputName, returnArgument, FAI, I, comment);
        } // end of WriteZeroFunction()

        // used for testing:
        protected Dictionary<string, string> m_randomScalarFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomVersorFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomSmvFuncName = new Dictionary<string, string>();

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
                    m_randomVersorFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_versor", new String[0], m_specification.m_GMV.Name, FT, null);
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

                if (m_gmvFunc) // GMV test
                {
                    testFuncNames.Add(testFuncName);
                    m_cgd.m_cog.EmitTemplate(defSB, "testZeroGMV",
                        "S=", m_specification,
                        "FT=", FT,
                        "gmvName=", FT.GetMangledName(m_specification, m_specification.m_GMV.Name),
                        "testFuncName=", testFuncName,
                        "targetFuncName=", m_funcName[FT.type],
                        "randomScalarFuncName=", m_randomScalarFuncName[FT.type],
                        "randomVersorFuncName=", m_randomVersorFuncName[FT.type]
                        );
                }
                else if (m_smv != null) // SMV test
                {
                    testFuncNames.Add(testFuncName);
                    m_cgd.m_cog.EmitTemplate(defSB, "testZeroSMV",
                        "S=", m_specification,
                        "FT=", FT,
                        "smv=", m_smv,
                        "smvName=", FT.GetMangledName(m_specification, m_smv.Name),
                        "testFuncName=", testFuncName,
                        "targetFuncName=", m_funcName[FT.type],
                        "randomScalarFuncName=", m_randomScalarFuncName[FT.type],
                        "randomSmvFuncName=", m_randomSmvFuncName[FT.type]
                        );

                }
            }

            return testFuncNames;
        }

    } // end of class Zero
} // end of namespace G25.CG.Shared.Func

