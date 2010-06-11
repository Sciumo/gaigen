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
    /// Generates code for log of Euclidean rotor.
    /// 
    /// The function name should be <c>"log"</c> with <c>optionType="euclidean"</c> 
    /// 
    /// The metric should be a Euclidean metric.
    /// </summary>
    public class LogEuclidean : G25.CG.Shared.BaseFunctionGenerator
    {
        protected const int NB_ARGS = 1;
        protected bool m_gmvFunc; ///< is this a function over GMVs?
        protected RefGA.Multivector m_grade0Value;
        protected RefGA.Multivector m_grade2Value;
        RefGA.Multivector m_grade2norm2Value;
        protected RefGA.Multivector m_mulValue;
        protected RefGA.Multivector m_returnValue; ///< returned value (symbolic multivector)
        protected G25.SMV m_smv = null; ///< if function over SMV, type goes here
        protected Dictionary<string, string> m_funcName = new Dictionary<string, string>();  ///< generated function name with full mangling, etc
        protected G25.VariableType m_returnType; ///< return type

        protected Dictionary<string, string> m_norm2Func = new Dictionary<string, string>(); ///< = mangled name of norm2 func
        protected Dictionary<string, string> m_scalarGpFunc = new Dictionary<string, string>(); ///< = mangled name of gp(mv, scalar) func
        protected Dictionary<string, string> m_grade2Func = new Dictionary<string, string>(); ///< = mangled name of grade2(mv) func

        protected const string scalarPartName = "_scalarPart_";
        protected const string norm2Name = "_g2norm2_";
        protected const string normName = "_g2norm_";
        protected const string mulName = "_mul_";

        /// <summary>
        /// Checks if this FunctionGenerator can implement a certain function.
        /// </summary>
        /// <param name="S">The specification of the algebra.</param>
        /// <param name="F">The function to be implemented.</param>
        /// <returns>true if 'F' can be implemented</returns>
        public override bool CanImplement(Specification S, G25.fgs F)
        {
            if (!((F.Name == "log") && (F.MatchNbArguments(1)))) return false;

            string type = F.GetOption("type");
            if (type == null) return false;

            return type.ToLower().Equals("euclidean");
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
                m_fgs.m_argumentTypeNames = new string[] { m_gmv.Name };

            // init argument pointers from the completed typenames (language sensitive);
            m_fgs.InitArgumentPtrFromTypeNames(m_specification);

            // get all function info
            FloatType FT = m_specification.GetFloatType(m_fgs.FloatNames[0]);
            bool computeMultivectorValue = true;
            G25.CG.Shared.FuncArgInfo[] tmpFAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

            m_gmvFunc = !tmpFAI[0].IsScalarOrSMV();

            // compute intermediate results, set return type
            if (m_gmvFunc) m_fgs.m_returnTypeName = m_gmv.Name; // unit(gmv) = gmv
            else
            {
                m_smv = tmpFAI[0].Type as G25.SMV;

                // get symbolic result
                m_grade0Value = tmpFAI[0].MultivectorValue[0].ExtractGrade(0);
                m_grade2Value = tmpFAI[0].MultivectorValue[0].ExtractGrade(2);
                RefGA.Multivector reverseGrade2Value = RefGA.Multivector.Reverse(m_grade2Value);
                m_grade2norm2Value = RefGA.Multivector.gp(reverseGrade2Value, m_grade2Value, m_M);

                m_returnValue = RefGA.Multivector.gp(m_grade2Value, new RefGA.Multivector(mulName)); // where mulName = atan2(sqrt(grade2norm2), grade0) / sqrt(grade2norm2)

                // get name of return type
                if (m_fgs.m_returnTypeName.Length == 0)
                    m_fgs.m_returnTypeName = G25.CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, m_returnValue).GetName();
                 
            }
            m_returnType = m_specification.GetType(m_fgs.m_returnTypeName);

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
            string GMVname = m_specification.m_GMV.Name;

            // check dependencies for all float types
            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                m_norm2Func[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "norm2", new string[] { GMVname }, FT, m_G25M.m_name) + G25.CG.Shared.CANSparts.RETURNS_SCALAR;

                m_scalarGpFunc[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new string[] { GMVname, floatName }, FT, null); // null = no metric for op required

                m_grade2Func[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, G25.CG.Shared.CANSparts.EXTRACT_GRADE + "2", new string[] { GMVname }, FT, null); // null = no metric for op required


            }
        }


        /// <summary>
        /// Should write the declaration/definitions of 'F' to StringBuffer 'SB', taking into account parameters specified in specification 'S'.
        /// </summary>
        public override void WriteFunction()
        {
            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                bool computeMultivectorValue = true;
                G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_gmv.Name, computeMultivectorValue);

                // generate comment
                Comment comment = new Comment(
                    m_fgs.AddUserComment("Returns logarithm of " + FAI[0].TypeName + " using " + m_G25M.m_name + " metric, assuming a 3D Euclidean rotor."));

                // if scalar or specialized: generate specialized function
                if (m_gmvFunc)
                {
                    StringBuilder declSB = m_cgd.m_declSB;
                    bool inline = false; // never inline GMV functions
                    StringBuilder defSB = (inline) ? m_cgd.m_inlineDefSB : m_cgd.m_defSB;

                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, m_fgs, FAI);

//                    string funcName = FT.GetMangledName(m_specification, m_fgs.OutputName);
                    m_funcName[FT.type] = CF.OutputName;

                    // setup hashtable with template arguments:
                    System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                    argTable["S"] = m_specification;
                    argTable["functionName"] = m_funcName[FT.type];
                    argTable["FT"] = FT;
                    argTable["mvType"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                    argTable["setPlaneFuncName"] = getSetPlaneFuncName();
                    argTable["scalarGpFuncName"] = m_scalarGpFunc[floatName];
                    argTable["norm2FuncName"] = m_norm2Func[floatName];
                    argTable["grade2FuncName"] = m_grade2Func[floatName];

                    if (m_specification.OutputCppOrC())
                    {
                        // header
                        m_cgd.m_cog.EmitTemplate(declSB, "logEuclideanHeader", argTable);
                    }

                    // source
                    m_cgd.m_cog.EmitTemplate(defSB, "logEuclidean", argTable);
               
                }
                else
                {// write simple specialized function:

                    // setup instructions
                    System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                    {
                        int nbTabs = 1;
                        bool mustCast = false;
                        bool nPtr = false;
                        bool declareN = true;

                        
                        // get grade 2 norm, scalar part
                        I.Add(new G25.CG.Shared.AssignInstruction(nbTabs, FT, FT, mustCast, m_grade2norm2Value, norm2Name, nPtr, declareN));
                        I.Add(new G25.CG.Shared.AssignInstruction(nbTabs, FT, FT, mustCast, m_grade0Value, scalarPartName, nPtr, declareN));

                        // setup checks for grade 2 == 0, grade 0 < 0.0
                        System.Collections.Generic.List<G25.CG.Shared.Instruction> ifGrade2ZeroI = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();

                        // setup checks for grade 0 < 0.0
                        System.Collections.Generic.List<G25.CG.Shared.Instruction> ifI = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                        System.Collections.Generic.List<G25.CG.Shared.Instruction> elseI = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();

                        // either return PI * ANY_GRADE2_BLADE or 0
                        RefGA.Multivector arbRot360 = new RefGA.Multivector(new RefGA.BasisBlade(m_grade2Value.BasisBlades[0].bitmap, Math.PI));
                        ifI.Add(new G25.CG.Shared.ReturnInstruction(nbTabs + 2, m_returnType, FT, mustCast, arbRot360)); // return 360 degree rotation in arbitrary plane
                        elseI.Add(new G25.CG.Shared.ReturnInstruction(nbTabs+2, m_returnType, FT, mustCast, RefGA.Multivector.ZERO)); // return zero if grade2 == 0 and grade0 >= 0

                        ifGrade2ZeroI.Add(new G25.CG.Shared.IfElseInstruction(nbTabs+1, scalarPartName + " < " + FT.DoubleToString(m_specification, 0.0), ifI, elseI));

                        I.Add(new G25.CG.Shared.IfElseInstruction(nbTabs, norm2Name + " <= " + FT.DoubleToString(m_specification, 0.0), ifGrade2ZeroI, null));

                        // where mulName = atan2(sqrt(grade2norm2), grade0) / sqrt(grade2norm2)
                        RefGA.Multivector normValue = new RefGA.Multivector(normName);
                        RefGA.Multivector grade0Value = new RefGA.Multivector(scalarPartName);
                        I.Add(new G25.CG.Shared.AssignInstruction(nbTabs, FT, FT, mustCast, RefGA.Symbolic.UnaryScalarOp.Sqrt(new RefGA.Multivector(norm2Name)), normName, nPtr, declareN));
                        I.Add(new G25.CG.Shared.AssignInstruction(nbTabs, FT, FT, mustCast, RefGA.Symbolic.BinaryScalarOp.Atan2(normValue, grade0Value), mulName, nPtr, declareN, "/", normValue));

                        // result = input / n2
                        I.Add(new G25.CG.Shared.ReturnInstruction(nbTabs, m_returnType, FT, mustCast, m_returnValue));
                    }

                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, m_fgs, FAI);

                    m_funcName[FT.type] = CF.OutputName;

                    bool staticFunc = Functions.OutputStaticFunctions(m_specification);
                    G25.CG.Shared.Functions.WriteFunction(m_specification, m_cgd, CF, m_specification.m_inlineFunctions, staticFunc, CF.OutputName, FAI, I, comment);
                }
            }
        } // end of WriteFunction

        /// <returns>Name of function to set coordinate of some Euclidean bivector coordinate.</returns>
        private string getSetPlaneFuncName() 
        {
            int euclBitmap = m_G25M.GetEuclideanBasisVectorBitmap();
            string setFuncName = "set";

            int cnt = 0; // how many euclidean vectors found so far

            // search for euclidean basis vectors.
            for (int i = 0; i < m_specification.m_dimension; i++) {
                int b = 1 << i;
                if ((euclBitmap & b) != 0) {
                    setFuncName = setFuncName + "_" + m_specification.m_basisVectorNames[i];
                    cnt++;
                }

                if (cnt >= 2) break; // only the first two
            }

            if (cnt != 2) throw new G25.UserException("Euclidean logarithm needs at least two Euclidean basis vectors.");

            return setFuncName;
        }


        // used for testing:
        protected Dictionary<string, string> m_randomScalarFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomBladeFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_reverseFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_spFuncName = new Dictionary<string, string>();
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
                m_reverseFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "reverse", new String[] { m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);
                m_spFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "sp", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, m_G25M.m_name);

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

                if (m_gmvFunc) // GMV test
                {
                    testFuncNames.Add(testFuncName);
                    System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                    argTable["S"] = m_specification;
                    argTable["FT"] = FT;
                    argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                    argTable["testFuncName"] = testFuncName;
                    argTable["targetFuncName"] = m_funcName[FT.type];
                    argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                    argTable["randomBladeFuncName"] = m_randomBladeFuncName[FT.type];
                    argTable["reverseFuncName"] = m_reverseFuncName[FT.type];
                    argTable["spFuncName"] = m_spFuncName[FT.type];
                    m_cgd.m_cog.EmitTemplate(defSB, "testEuclideanLogGMV", argTable);
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
                    argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                    argTable["randomSmvFuncName"] = m_randomSmvFuncName[FT.type];
                    argTable["reverseFuncName"] = m_reverseFuncName[FT.type];
                    argTable["spFuncName"] = m_spFuncName[FT.type];
                    m_cgd.m_cog.EmitTemplate(defSB, "testEuclideanLogSMV", argTable);
                }
            }

            return testFuncNames;
        } // end of WriteTestFunction()

    } // end of class Log
} // end of namespace G25.CG.Shared.Func
