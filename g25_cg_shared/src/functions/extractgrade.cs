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
    /// Generates code for extractGrade0() ... extractGradeN() functions.
    /// 
    /// The function name start with <c>"extractGrade"</c>.
    /// </summary>
    public class Grade : G25.CG.Shared.BaseFunctionGenerator
    {

        /// <returns>Grade index of 'gradeStr', or -1 if what comes after "grade" is not a number</returns>
        public int GetGradeIdx(String gradeStr)
        {
            if (gradeStr == G25.CG.Shared.CANSparts.EXTRACT_GRADE) return -1;

            String str = gradeStr.Substring(G25.CG.Shared.CANSparts.EXTRACT_GRADE.Length);
            try
            {
                return Int32.Parse(str);
            }
            catch (System.Exception E)
            {
                if (str.Length > 0) throw E; // _if_ anything is part of the function name beyond grade, then it must be a number, and nothing else
                else return -1;
            }
        }

        // constants, intermediate results
        protected const int NB_ARGS = 1;
        protected bool m_gmvFunc; ///< is this a function over GMVs?
        protected RefGA.Multivector m_returnValue; ///< returned value (symbolic multivector)
        protected int m_gradeIdx; ///< requested grade (-1 for specify-at-runtime)

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
            if (F.Name.Length < G25.CG.Shared.CANSparts.EXTRACT_GRADE.Length) return false;

            try
            {
                int gradeIdx = GetGradeIdx(F.Name);
                return (F.Name.StartsWith(G25.CG.Shared.CANSparts.EXTRACT_GRADE) && F.MatchNbArguments(1) &&  // get the name & number of arguments right
                    (((gradeIdx >= 0) && (gradeIdx <= S.m_dimension)) || // either specify the grade index
                    ((gradeIdx < 0) && (F.GetArgumentTypeName(0, S.m_GMV.Name) == S.m_GMV.Name)))); // or be a function over general multivectors and leave out the grade
            }
            catch (Exception)
            {
                // we arrive there when F.Name is gradeX where X is not a number.
                return false;
            }
        }

        /// <summary>
        /// If this FunctionGenerator can implement 'F', then this function should complete the (possible)
        /// blanks in 'F'. This means:
        ///  - Fill in F.m_returnTypeName if it is empty
        ///  - Fill in F.m_argumentTypeNames (and m_argumentVariableNames) if it is empty.
        /// </summary>
        public override void CompleteFGS()
        {
            m_gradeIdx = GetGradeIdx(m_fgs.Name);

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
            m_smv = tmpFAI[0].Type as G25.SMV;

            // compute intermediate results, set return type
            if (m_gmvFunc) m_fgs.m_returnTypeName = m_gmv.Name; // grade(gmv) = gmv
            else
            { // compute return value
                m_returnValue = tmpFAI[0].MultivectorValue[0].ExtractGrade(m_gradeIdx);

                // get name of return type
                if (m_fgs.m_returnTypeName.Length == 0)
                    m_fgs.m_returnTypeName = G25.CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, m_returnValue).GetName();
            }
        }

        /// <summary>
        /// Should write the declaration/definitions of 'F' to StringBuffer 'SB', taking into account parameters specified in specification 'S'.
        /// </summary>
        public override void WriteFunction()
        {

            foreach (String floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                bool computeMultivectorValue = true;
                G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

                // generate comment
                Comment comment = new Comment(
                    m_fgs.AddUserComment("Returns grade " + ((m_gradeIdx < 0) ? "groupBitmap" : m_gradeIdx.ToString()) + " of  " + FAI[0].TypeName + "."));

                // if scalar or specialized: generate specialized function
                if (m_gmvFunc)
                {
                    m_funcName[FT.type] = G25.CG.Shared.GmvCASNparts.WriteGradeFunction(m_specification, m_cgd, FT, FAI, m_fgs, comment, m_gradeIdx);
                }
                else
                {// write simple specialized function:
                    // setup instructions
                    System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                    {
                        int nbTabs = 1;
                        bool mustCast = false;
                        G25.VariableType returnType = G25.CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, m_returnValue);
                        I.Add(new G25.CG.Shared.ReturnInstruction(nbTabs, returnType, FT, mustCast, m_returnValue));
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
        protected Dictionary<string, string> m_randomVersorFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomSmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_addGmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_subtractGmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_gradeGmvFuncName = new Dictionary<string, string>();

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
                m_addGmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "add", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);
                m_subtractGmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "subtract", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);

                if (m_gradeIdx >= 0)
                {
                    m_gradeGmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, G25.CG.Shared.CANSparts.EXTRACT_GRADE, new String[] { m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);
                }

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

                System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                argTable["addGmvFuncName"] = m_addGmvFuncName[FT.type];
                argTable["subtractGmvFuncName"] = m_subtractGmvFuncName[FT.type];
                if (m_gradeIdx >= 0)
                {
                    argTable["gradeGmvFuncName"] = m_gradeGmvFuncName[FT.type];
                    { // set "inverseGroupBitmap"
                        // turn grade into inverse group bitmap
                        int groupBitmap = 0;
                        int inverseGroupBitmap = 0;
                        for (int g = 0; g < m_specification.m_GMV.NbGroups; g++)
                            if (m_specification.m_GMV.Group(g)[0].Grade() != m_gradeIdx)
                                inverseGroupBitmap |= (1 << g);
                            else groupBitmap |= (1 << g);
                        argTable["groupBitmap"] = groupBitmap.ToString();
                        argTable["inverseGroupBitmap"] = inverseGroupBitmap.ToString();
                    }
                }
                if (m_gmvFunc) // GMV test
                {
                    testFuncNames.Add(testFuncName);
                    argTable["S"] = m_specification;
                    argTable["FT"] = FT;
                    argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                    argTable["testFuncName"] = testFuncName;
                    argTable["targetFuncName"] = m_funcName[FT.type];
                    argTable["randomVersorFuncName"] = m_randomVersorFuncName[FT.type];
                    if (m_gradeIdx >= 0)
                        m_cgd.m_cog.EmitTemplate(defSB, "testGradeXGMV", argTable);
                    else m_cgd.m_cog.EmitTemplate(defSB, "testGradeGMV", argTable);

                }
                else if ((m_smv != null) && m_smv.CanConvertToGmv(m_specification))
                { // SMV test
                    testFuncNames.Add(testFuncName);
                    argTable["S"] = m_specification;
                    argTable["FT"] = FT;
                    argTable["testFuncName"] = testFuncName;
                    argTable["smv"] = m_smv;
                    argTable["smvName"] = FT.GetMangledName(m_specification, m_smv.Name);
                    argTable["smvRName"] = FT.GetMangledName(m_specification, m_fgs.m_returnTypeName);
                    argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                    argTable["targetFuncName"] = m_funcName[FT.type];
                    argTable["randomSmvFuncName"] = m_randomSmvFuncName[FT.type];

                    m_cgd.m_cog.EmitTemplate(defSB, "testGradeXSMV", argTable);
                }
            }

            return testFuncNames;
        } // end of WriteTestFunction()

    } // end of class ExtractGrade
} // end of namespace G25.CG.Shared.Func

