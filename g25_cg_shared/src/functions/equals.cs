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
    /// Generates code for the check for equality of two multivectors.
    /// 
    /// The function should be called <c>"equals"</c>.
    /// </summary>
    public class Equals : G25.CG.Shared.BaseFunctionGenerator
    {

        // constants, intermediate results
        protected const int NB_ARGS = 3;
        protected bool m_gmvFunc; ///< is this a function over GMVs?
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
            //String type F.GetArgumentTypeName(0, S.m_GMV.Name);
            return ((F.Name == "equals") && (F.MatchNbArguments(NB_ARGS)) &&
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
                m_fgs.m_argumentTypeNames = new String[] { m_gmv.Name, m_gmv.Name, m_fgs.FloatNames[0] };

            // init argument pointers from the completed typenames (language sensitive);
            m_fgs.InitArgumentPtrFromTypeNames(m_specification);

            // get all function info
            FloatType FT = m_specification.GetFloatType(m_fgs.FloatNames[0]);
            bool computeMultivectorValue = true;
            G25.CG.Shared.FuncArgInfo[] tmpFAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

            m_gmvFunc = !(tmpFAI[0].IsScalarOrSMV() && tmpFAI[1].IsScalarOrSMV());
            m_smv1 = tmpFAI[0].Type as G25.SMV;
            m_smv2 = tmpFAI[1].Type as G25.SMV;
        }

        /// <summary>
        /// Writes the declaration/definitions of 'F' to StringBuffer 'SB', taking into account parameters specified in specification 'S'.
        /// </summary>
        public override void WriteFunction()
        {
            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                bool computeMultivectorValue = true;
                G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_gmv.Name, computeMultivectorValue);

                // comment
                String comment = "/** " + m_fgs.AddUserComment("Returns whether input multivectors are equal up to an epsilon " + FAI[2].Name + ".") + " */";

                // if scalar or specialized: generate specialized function
                if (m_gmvFunc)
                {
                    m_funcName[FT.type] = G25.CG.Shared.GmvCASNparts.WriteEqualsOrZeroOrGradeBitmapFunction(m_specification, m_cgd, FT, FAI, m_fgs, comment, G25.CG.Shared.CANSparts.EQUALS_ZERO_GRADEBITMAP_TYPE.EQUALS);
                }
                else
                { // write simple specialized function:
                    // because of lack of overloading, function names include names of argument types
                    //G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, m_fgs, FAI);

                    // write out the function:
                    WriteEqualsFunction(FT, FAI, m_fgs, comment);
                }
            }
        } // end of WriteFunction

        /// <summary>
        /// Writes an equality test function for specialized multivectors
        /// </summary>
        /// <param name="FT"></param>
        /// <param name="FAI"></param>
        /// <param name="F"></param>
        /// <param name="comment"></param>
        protected void WriteEqualsFunction(FloatType FT, G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F, String comment)
        {
            // setup instructions
            List<G25.CG.Shared.Instruction> I = new List<G25.CG.Shared.Instruction>();

            // get a basis blade list which contains all basis blades (this is used in conjunction with GetAssignmentStrings())
            RefGA.BasisBlade[] BL = new RefGA.BasisBlade[1 << m_specification.m_dimension];
            for (uint bitmap = 0; bitmap < (uint)BL.Length; bitmap++)
                BL[bitmap] = new RefGA.BasisBlade(bitmap);

            // get string to access the coordinates of each basis blade wrt to the basis
            bool mustCast = false;
            bool writeZeros = false;
            string[] assStr1 = G25.CG.Shared.CodeUtil.GetAssignmentStrings(m_specification, FT, mustCast, BL, FAI[0].MultivectorValue[0], writeZeros);
            string[] assStr2 = G25.CG.Shared.CodeUtil.GetAssignmentStrings(m_specification, FT, mustCast, BL, FAI[1].MultivectorValue[0], writeZeros);

            string BOOL = (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C) ? "int" : "bool";
            string TRUE = (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C) ? "1" : "true";
            string FALSE = (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C) ? "0" : "false";

            { // setup instructions
                // get tmp storage for 'difference'
                int nbTabs = 1;
                bool dDeclared = false;

                // for each assStr, subtract the other, see if difference is within limits
                for (int i = 0; i < BL.Length; i++)
                {
                    if ((assStr1[i] == null) && (assStr2[i] == null)) continue;
                    else if (assStr1[i] == null)
                        I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs,
                            "if ((" + assStr2[i] + " < -" + FAI[2].Name + ") || (" + assStr2[i] + " > " + FAI[2].Name + ")) return " + FALSE + "; // " + BL[i].ToString(m_specification.m_basisVectorNames)));
                    else if (assStr2[i] == null)
                        I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs,
                            "if ((" + assStr1[i] + " < -" + FAI[2].Name + ") || (" + assStr1[i] + " > " + FAI[2].Name + ")) return " + FALSE + "; // " + BL[i].ToString(m_specification.m_basisVectorNames)));
                    else
                    {
                        if (!dDeclared)
                        {
                            // declare a variable 'd', but only if required
                            I.Insert(0, new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, FT.type + " d;"));
                            dDeclared = true;
                        }

                        I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs,
                            "d = " + assStr1[i] + " - " + assStr2[i] + "; " +
                            "if ((d < -" + FAI[2].Name + ") || (d > " + FAI[2].Name + ")) return " + FALSE + "; // " + BL[i].ToString(m_specification.m_basisVectorNames)));
                    }
                }
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
        }

        // used for testing:
        protected Dictionary<string, string> m_randomScalarFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomVersorFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomSmv1FuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomSmv2FuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_gmvAddFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_gmvSubtractFuncName = new Dictionary<string, string>();

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
                    m_gmvAddFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "add", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);
                }
                else if ((m_smv1 != null) && (m_smv2 != null))
                {
                    string defaultReturnTypeName = null;

                    m_randomSmv1FuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_smv1.Name, new String[0], defaultReturnTypeName, FT, null);
                    m_randomSmv2FuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_smv2.Name, new String[0], defaultReturnTypeName, FT, null);
                    
                    m_gmvSubtractFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "subtract", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);
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
                    m_cgd.m_cog.EmitTemplate(defSB, "testEqualsGMV",
                        "S=", m_specification,
                        "FT=", FT,
                        "gmvName=", FT.GetMangledName(m_specification, m_specification.m_GMV.Name),
                        "testFuncName=", testFuncName,
                        "targetFuncName=", m_funcName[FT.type],
                        "randomScalarFuncName=", m_randomScalarFuncName[FT.type],
                        "randomGmvFuncName=", m_randomVersorFuncName[FT.type],
                        "addGmvFuncName=", m_gmvAddFuncName[FT.type]
                        );
                }
                else if ((m_smv1 != null) &&
                    (m_smv2 != null) &&
                    m_smv1.CanConvertToGmv(m_specification) &&
                    m_smv2.CanConvertToGmv(m_specification) &&
                    MV.CanConvertSumToGmv(m_specification, m_smv1, m_smv2))
                { // SMV test
                    testFuncNames.Add(testFuncName);
                    System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                    argTable["S"] = m_specification;
                    argTable["FT"] = FT;
                    argTable["testFuncName"] = testFuncName;
                    argTable["smv1"] = m_smv1;
                    argTable["smv2"] = m_smv2;
                    argTable["smv1Name"] = FT.GetMangledName(m_specification, m_smv1.Name);
                    argTable["smv2Name"] = FT.GetMangledName(m_specification, m_smv2.Name);
                    argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                    argTable["targetFuncName"] = m_funcName[FT.type];
                    argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                    argTable["randomSmv1FuncName"] = m_randomSmv1FuncName[FT.type];
                    argTable["randomSmv2FuncName"] = m_randomSmv2FuncName[FT.type];
                    argTable["subtractGmvFuncName"] = m_gmvSubtractFuncName[FT.type];

                    m_cgd.m_cog.EmitTemplate(defSB, "testEqualsSMV", argTable);
                }
            }

            return testFuncNames;
        } // end of WriteTestFunction()


    } // end of class Equals
} // end of namespace G25.CG.Shared.Func

