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
    /// Generates code to get the grade bitmap of a multivector (up to some epsilon).
    /// 
    /// The function should be called <c>"gradeBitmap"</c>.
    /// </summary>
    public class GradeBitmap : G25.CG.Shared.BaseFunctionGenerator
    {

        /// <returns>true when <c>F.Name == "gradeBitmap"</c>.</returns>
        public static bool IsGradeBitmap(G25.fgs F)
        {
            return F.Name == "gradeBitmap";
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
            return (IsGradeBitmap(F) && (F.MatchNbArguments(2)));
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
                    m_fgs.AddUserComment("Returns a bitmap where each one-bit means that at least one coordinate of the respective grade of  " + FAI[0].Name + " is larger than " + FAI[1].Name) + " */";

                // if scalar or specialized: generate specialized function
                if (m_gmvFunc)
                {
                    m_funcName[FT.type] = G25.CG.Shared.GmvCASNparts.WriteEqualsOrZeroOrGradeBitmapFunction(m_specification, m_cgd, FT, FAI, m_fgs, comment, G25.CG.Shared.CANSparts.EQUALS_ZERO_GRADEBITMAP_TYPE.GRADE_BITMAP);
                }
                else
                {// write simple specialized function:
                    // because of lack of overloading, function names include names of argument types
                    //G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, m_fgs, FAI);

                    // write out the function:
                    WriteGradeBitmapFunction(FT, FAI, m_fgs, comment);
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
        protected void WriteGradeBitmapFunction(FloatType FT, G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F, String comment)
        {
            // setup instructions
            System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();

            // write the function:
            {
                int nbTabs = 1;

                I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, "int bitmap = 0;"));

                // get largest constant coordinate for each grade:
                Dictionary<int, double> largestConstCoord = new Dictionary<int, double>();
                if (m_smv != null)
                {
                    // first find largest const coord: if any const coord above eps, always return 0
                    for (int c = 0; c < m_smv.NbConstBasisBlade; c++)
                    {
                        int g = m_smv.ConstBasisBlade(c).Grade();
                        double absVal = Math.Abs(m_smv.ConstBasisBlade(c).scale);
                        if (!largestConstCoord.ContainsKey(g)) largestConstCoord[g] = absVal;
                        else if (absVal > largestConstCoord[g]) largestConstCoord[g] = absVal;
                    }
                }


                // generate code to check if largest coord of grade > 'eps'
                foreach (KeyValuePair<int, double> kvp in largestConstCoord)
                {
                    if (kvp.Value > 0.0)
                    {
                        I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs,
                            "if (" + FT.DoubleToString(m_specification, kvp.Value) + " > " + FAI[1].Name + ") bitmap |= " + (1 << kvp.Key) + ";"));
                    }
                }

                // generate code to check all coordiantes
                String[] accessStr = G25.CG.Shared.CodeUtil.GetAccessStr(m_specification, m_smv, FAI[0].Name, FAI[0].Pointer);
                for (int i = 0; i < accessStr.Length; i++)
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs,
                        "if ((" + accessStr[i] + " < -" + FAI[1].Name + ") || (" + accessStr[i] + " > " + FAI[1].Name + ")) bitmap |= " + (1 << m_smv.NonConstBasisBlade(i).Grade()) + ";"));

                // finally, return 1 if all check were OK
                I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, "return bitmap;"));
            }

            // because of lack of overloading, function names include names of argument types
            G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, F, FAI);
            m_funcName[FT.type] = CF.OutputName;

            // setup return type and argument:
            String returnTypeName = "int";
            G25.CG.Shared.FuncArgInfo returnArgument = null;

            // write function
            bool staticFunc = Functions.OutputStaticFunctions(m_specification);
            G25.CG.Shared.Functions.WriteFunction(m_specification, m_cgd, F, m_specification.m_inlineFunctions, staticFunc, returnTypeName, CF.OutputName, returnArgument, FAI, I, comment);
        } // end of WriteGradeBitmapFunction()


        // used for testing:
        protected Dictionary<string, string> m_randomScalarFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomBladeFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_addGmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomSmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_gradeBitmapGmvFuncName = new Dictionary<string, string>();


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
                    m_randomBladeFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_blade", new String[0], m_specification.m_GMV.Name, FT, null);
                    m_addGmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "add", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);
                }
                else if (m_smv != null)
                {
                    string defaultReturnTypeName = null;

                    m_randomSmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_smv.Name, new String[0], defaultReturnTypeName, FT, null);
                    m_gradeBitmapGmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gradeBitmap", new String[] { m_specification.m_GMV.Name, FT.type }, defaultReturnTypeName, FT, null);
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
                //m_specification.m_all
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
                    argTable["randomBladeFuncName"] = m_randomBladeFuncName[FT.type];
                    argTable["addGmvFuncName"] = m_addGmvFuncName[FT.type];
                    m_cgd.m_cog.EmitTemplate(defSB, "testGradeBitmapGMV", argTable);
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
                    argTable["testFuncName"] = testFuncName;
                    argTable["targetFuncName"] = m_funcName[FT.type];
                    argTable["randomSmvFuncName"] = m_randomSmvFuncName[FT.type];
                    argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                    argTable["gradeBitmapGmvFuncName"] = m_gradeBitmapGmvFuncName[FT.type];

                    m_cgd.m_cog.EmitTemplate(defSB, "testGradeBitmapSMV", argTable);
                }
            }

            return testFuncNames;
        }


    } // end of class GradeBitmap
} // end of namespace G25.CG.Shared.Func
