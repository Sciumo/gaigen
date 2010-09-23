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
    /// Generates code for 'sign toggling functions' which do not use metric.
    /// 
    /// Possible function names are:
    ///   - <c>"negate"</c>: negation.
    ///   - <c>"reverse"</c>: reversion.
    ///   - <c>"cliffordConjugate"</c>: Clifford conjugate.
    ///   - <c>"gradeInvolution"</c>: grade inversion or grade involution.
    /// </summary>
    public class ToggleSign : G25.CG.Shared.BaseFunctionGenerator
    {

        /// <returns>true when F.Name == "negate"</returns>
        public static bool IsNegate(G25.fgs F)
        {
            return F.Name == "negate";
        }

        /// <returns>true when F.Name == "reverse"</returns>
        public static bool IsReverse(G25.fgs F)
        {
            return F.Name == "reverse";
        }

        /// <returns>true when F.Name == "cliffordConjugate"</returns>
        public static bool IsCliffordConjugate(G25.fgs F)
        {
            return F.Name == "cliffordConjugate";
        }

        /// <returns>true when F.Name == "gradeInvolution"</returns>
        public static bool IsGradeInvolution(G25.fgs F)
        {
            return F.Name == "gradeInvolution";
        }

        public static RefGA.Util.SIGN_TOGGLE_FUNCTION GetRefGAFunctionId(G25.fgs F)
        {
            if (IsNegate(F)) return RefGA.Util.SIGN_TOGGLE_FUNCTION.NEGATION;
            else if (IsReverse(F)) return RefGA.Util.SIGN_TOGGLE_FUNCTION.REVERSION;
            else if (IsCliffordConjugate(F)) return RefGA.Util.SIGN_TOGGLE_FUNCTION.CLIFFORD_CONJUGATION;
            else if (IsGradeInvolution(F)) return RefGA.Util.SIGN_TOGGLE_FUNCTION.GRADE_INVOLUTION;
            else return RefGA.Util.SIGN_TOGGLE_FUNCTION.NONE;
        }

        public static String GetDescriptiveName(G25.fgs F)
        {
            if (IsNegate(F)) return "negation";
            else if (IsReverse(F)) return "reverse";
            else if (IsCliffordConjugate(F)) return "Clifford conjugate";
            else if (IsGradeInvolution(F)) return "grade involution";
            else throw new Exception("G25.CG.C.ToggleSign.GetDescriptiveName(): unknown toggle type: " + F.Name);
        }

        public static G25.CG.Shared.CANSparts.UnaryToggleSignType GetToggleType(G25.fgs F)
        {
            if (IsNegate(F)) return G25.CG.Shared.CANSparts.UnaryToggleSignType.NEGATE;
            else if (IsReverse(F)) return G25.CG.Shared.CANSparts.UnaryToggleSignType.REVERSE;
            else if (IsCliffordConjugate(F)) return G25.CG.Shared.CANSparts.UnaryToggleSignType.CLIFFORD_CONJUGATE;
            else if (IsGradeInvolution(F)) return G25.CG.Shared.CANSparts.UnaryToggleSignType.GRADE_INVOLUTION;
            else throw new Exception("G25.CG.C.ToggleSign.GetToggleType(): unknown toggle type: " + F.Name);
        }

        protected const int NB_ARGS = 1;
        protected bool m_gmvFunc; ///< is this a function over GMVs?
        protected RefGA.Multivector m_returnValue; ///< returned value (symbolic multivector)
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
            return ((IsReverse(F) || IsNegate(F) || IsCliffordConjugate(F) || IsGradeInvolution(F)) &&
                (F.MatchNbArguments(1)));
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

            // compute intermediate results, set return type
            if (m_gmvFunc) m_fgs.m_returnTypeName = m_gmv.Name; // toggle_sign(gmv) = gmv
            else
            {
                m_smv = tmpFAI[0].Type as G25.SMV;

                // compute return value
                if (IsNegate(m_fgs)) m_returnValue = RefGA.Multivector.Negate(tmpFAI[0].MultivectorValue[0]);
                else if (IsReverse(m_fgs)) m_returnValue = RefGA.Multivector.Reverse(tmpFAI[0].MultivectorValue[0]);
                else if (IsCliffordConjugate(m_fgs)) m_returnValue = RefGA.Multivector.CliffordConjugate(tmpFAI[0].MultivectorValue[0]);
                else if (IsGradeInvolution(m_fgs)) m_returnValue = RefGA.Multivector.GradeInvolution(tmpFAI[0].MultivectorValue[0]);

                // get name of return type
                if (m_fgs.m_returnTypeName.Length == 0)
                    m_fgs.m_returnTypeName = G25.CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, m_returnValue).GetName();
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
                Comment comment = new Comment(m_fgs.AddUserComment("Returns " + GetDescriptiveName(m_fgs) + " of " + FAI[0].TypeName + "."));

                // if scalar or specialized: generate specialized function
                if (m_gmvFunc)
                {
                    m_funcName[FT.type] = G25.CG.Shared.GmvCASNparts.WriteUnarySignFunction(m_specification, m_cgd, FT, FAI, m_fgs, comment, GetToggleType(m_fgs));
                }
                else
                {// write simple specialized function:
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
        protected Dictionary<string, string> m_randomVersorFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomSmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_extractGradeFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_gmvToggleSignFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_subtractGmvFuncName = new Dictionary<string, string>();



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

                if (m_gmvFunc)
                {
                    m_randomVersorFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_versor", new String[0], m_specification.m_GMV.Name, FT, null);
                    m_extractGradeFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, G25.CG.Shared.CANSparts.EXTRACT_GRADE, new String[] { m_specification.m_GMV.Name, G25.GroupBitmapType.GROUP_BITMAP  }, m_specification.m_GMV.Name, FT, null);
                }
                else if (m_smv != null)
                {
                    string defaultReturnTypeName = null;

                    m_randomSmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_smv.Name, new String[0], defaultReturnTypeName, FT, null);
                    m_gmvToggleSignFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, m_fgs.Name, new String[] { m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);
                    m_subtractGmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "subtract", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, defaultReturnTypeName, FT, null);
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


            // get a table which describes the behavior of the operation
            int NB_ENTRIES = m_specification.m_dimension + 1;
            StringBuilder signArray = new StringBuilder();
            int[] table = RefGA.Util.GetSignToggleMultipliers(GetRefGAFunctionId(m_fgs), NB_ENTRIES);
            for (int i = 0; i < NB_ENTRIES; i++)
            {
                if (i > 0) signArray.Append(", ");
                signArray.Append(table[i]);
            }

            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                string testFuncName = Util.GetTestingFunctionName(m_specification, m_cgd, m_funcName[FT.type]);


                if (m_gmvFunc) // GMV test
                {
                    testFuncNames.Add(testFuncName);
                    System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                    argTable["S"] = m_specification;
                    argTable["FT"] = FT;
                    argTable["gmv"] = m_specification.m_GMV;
                    argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                    argTable["testFuncName"] = testFuncName;
                    argTable["targetFuncName"] = m_funcName[FT.type];
                    argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                    argTable["randomVersorFuncName"] = m_randomVersorFuncName[FT.type];
                    argTable["extractGradeFuncName"] = m_extractGradeFuncName[FT.type];
                    argTable["signArray"] = signArray.ToString();
                    argTable["nbEntries"] = NB_ENTRIES;

                    m_cgd.m_cog.EmitTemplate(defSB, "testToggleSignGMV", argTable);
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
                    argTable["resultSmvName"] = FT.GetMangledName(m_specification, m_fgs.m_returnTypeName);
                    argTable["testFuncName"] = testFuncName;
                    argTable["targetFuncName"] = m_funcName[FT.type];
                    argTable["targetFuncReturnsFloat"] = m_specification.IsFloatType(m_fgs.ReturnTypeName);
                    argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                    argTable["randomSmvFuncName"] = m_randomSmvFuncName[FT.type];
                    argTable["subtractGmvFuncName"] = m_subtractGmvFuncName[FT.type];
                    argTable["gmvToggleSignFuncName"] = m_gmvToggleSignFuncName[FT.type];
                    m_cgd.m_cog.EmitTemplate(defSB, "testToggleSignSMV", argTable);
                }
            }

            return testFuncNames;
        } // end of WriteTestFunction()


    } // end of class ToggleSign
} // end of namespace G25.CG.Shared.Func

