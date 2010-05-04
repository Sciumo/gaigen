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
    /// Generates code for constructing a conformal model point for the dimension of the space of the algebra.
    /// 
    /// The function should be called <c>"cgaPoint"</c>.
    /// 
    /// The position can be specified either as a Euclidean vector or a set of coordinates.
    /// 
    /// Assumes that origin=no, infinity=ni, Euclidean basis=e1, e2, ...
    /// Origin and infinity can be set using <c>option</c> attributes, see these examples:
    /// Some examples of usage:
    /// <code>            
    /// <function name="cgaPoint" arg1="vectorE3GA" floatType="double"/>
    /// <function name="cgaPoint" arg1="vectorE3GA" floatType="float"/>
    /// <function name="cgaPoint" arg1="double" arg2="double" arg3="double" floatType="double"/>
    /// <function name="cgaPoint" arg1="float" arg2="float" arg3="float" optionOrigin="no" 
    ///     optionInfinity="ni" floatType="double"/>
    /// <function name="randomCgaPoint" floatType="float"/>
    /// </code>
    /// 
    /// </summary>
    public class CgaPoint : G25.CG.Shared.BaseFunctionGenerator
    {

        /// <returns>true when input to function is coordinates.</returns>
        public bool IsRandom(Specification S, G25.fgs F)
        {
            return (F.Name == "randomCgaPoint") && F.MatchNbArguments(0);
        }

        /// <returns>true when input to function is coordinates.</returns>
        public bool IsCoordBased(Specification S, G25.fgs F, FloatType FT)
        {
            return F.MatchNbArguments(S.m_dimension - 2) &&
                S.IsFloatType(F.GetArgumentTypeName(0, FT.type)) &&
                (!IsRandom(S, F));
        }

        /// <returns>true when input to function has one argument which is not a floating point type.</returns>
        public bool IsVectorBased(Specification S, G25.fgs F, FloatType FT)
        {
            return F.MatchNbArguments(1) && (!IsCoordBased(S, F, FT));
        }

        /// <returns>the RefGA.Multivector which represents the origin ("no" by default, but can be set by <c>optionOrigin="..."</c>).</returns>
        public RefGA.Multivector GetOrigin(Specification S, G25.fgs F)
        {
            String originName = "no";
            if (F.GetOption("origin") != null)
                originName = F.GetOption("origin");
            int bvIdx = S.GetBasisVectorIndex(originName);
            if (bvIdx < 0)
                throw new G25.UserException("Unknown basis vector specified for origin: " + originName,
                    XML.FunctionToXmlString(S, F));
            else return new RefGA.Multivector(new RefGA.BasisBlade((uint)(1 << bvIdx)));
        }

        /// <returns>the RefGA.Multivector which represents the origin ("ni" by default, but can be set by <c>optionInfinity="..."</c>).</returns>
        public RefGA.Multivector GetInfinity(Specification S, G25.fgs F)
        {
            String originName = "ni";
            if (F.GetOption("infinity") != null)
                originName = F.GetOption("infinity");
            int bvIdx = S.GetBasisVectorIndex(originName);
            if (bvIdx < 0)
                throw new G25.UserException("Unknown basis vector specified for infinity: " + originName,
                    XML.FunctionToXmlString(S, F));
            else return new RefGA.Multivector(new RefGA.BasisBlade((uint)(1 << bvIdx)));
        }


        // constants, intermediate results
        protected int NB_ARGS; ///< 1 float (random), 1 (vector), or dimension of algebra (coordinates)
        protected RefGA.Multivector m_returnValue; ///< returned value (symbolic multivector)
        protected Dictionary<string, string> m_randomScalarFunc = new Dictionary<string, string>(); ///< = mangled name of random scalar func 
        protected Dictionary<string, string> m_cgaPointFunc = new Dictionary<string, string>(); ///< = mangled name of random scalar func 
        protected Dictionary<string, string> m_funcName = new Dictionary<string, string>();  ///< generated function name with full mangling, etc
        protected G25.VariableType m_vectorType;

        /// <summary>
        /// Checks if this FunctionGenerator can implement a certain function.
        /// </summary>
        /// <param name="S">The specification of the algebra.</param>
        /// <param name="F">The function to be implemented.</param>
        /// <returns>true if 'F' can be implemented.</returns>
        public override bool CanImplement(Specification S, G25.fgs F)
        {
            return IsRandom(S, F) ||
                (((F.Name == "cgaPoint") && (F.MatchNbArguments(1) || F.MatchNbArguments(S.m_dimension - 2))));
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

            // init argument pointers from the completed typenames (language sensitive);
            m_fgs.InitArgumentPtrFromTypeNames(m_specification);

            // get all function info
            FloatType FT = m_specification.GetFloatType(m_fgs.FloatNames[0]);
            bool computeMultivectorValue = true;
            G25.CG.Shared.FuncArgInfo[] tmpFAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

            { // compute return value
                RefGA.Multivector no = GetOrigin(m_specification, m_fgs);
                RefGA.Multivector ni = GetInfinity(m_specification, m_fgs);

                // compose a vector value (the Euclidean part)
                RefGA.Multivector vectorValue = RefGA.Multivector.ZERO;
                if ((IsRandom(m_specification, m_fgs)) || (IsCoordBased(m_specification, m_fgs, FT)))
                {
                    m_returnValue = no;
                    for (int i = 0; i < m_specification.m_dimension - 2; i++)
                    {
                        string bvName = "e" + (i + 1).ToString(); // todo: this assumes a fixed name for the basis vectors. Make these names options?
                        int bvIdx = m_specification.GetBasisVectorIndex(bvName);
                        if (bvIdx < 0)
                            throw new G25.UserException("Cannot find basis vector " + bvName, XML.FunctionToXmlString(m_specification, m_fgs));

                        RefGA.Multivector basisVector = RefGA.Multivector.GetBasisVector(bvIdx);

                        if (IsRandom(m_specification, m_fgs))
                            vectorValue = RefGA.Multivector.Add(vectorValue, RefGA.Multivector.gp(new RefGA.Multivector("ce" + (i + 1).ToString()), basisVector));
                        else vectorValue = RefGA.Multivector.Add(vectorValue, RefGA.Multivector.gp(tmpFAI[i].MultivectorValue[0], basisVector));
                    }
                }
                else if (IsVectorBased(m_specification, m_fgs, FT))
                {
                    vectorValue = tmpFAI[0].MultivectorValue[0];
                    m_vectorType = tmpFAI[0].Type;
                }
                else
                {
                    throw new G25.UserException("Invalid arguments specified.", XML.FunctionToXmlString(m_specification, m_fgs));
                }

                { // add no and 0.5 vectorValue^2 ni
                    RefGA.Multivector vectorValueSquared = RefGA.Multivector.scp(vectorValue, vectorValue);
                    RefGA.Multivector niPart = RefGA.Multivector.gp(
                        RefGA.Multivector.gp(vectorValueSquared, ni), 0.5);
                    m_returnValue = RefGA.Multivector.Add(RefGA.Multivector.Add(no, vectorValue), niPart);
                }
            } // end of compute return value

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
            if (IsRandom(m_specification, m_fgs))
            {
                // check dependencies for all float types
                foreach (String floatName in m_fgs.FloatNames)
                {
                    FloatType FT = m_specification.GetFloatType(floatName);
                    string[] argTypeNames = new string[m_specification.m_dimension - 2];
                    for (int i = 0; i < argTypeNames.Length; i++)
                        argTypeNames[i] = floatName;
                    //bool returnTrueName = false;
                    m_randomScalarFunc[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, RandomScalar.RANDOM + floatName, new string[0], floatName, FT, null);
                    m_cgaPointFunc[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "cgaPoint", argTypeNames, m_fgs.m_returnTypeName, FT, null);
                }

            }
        }


        /// <summary>
        /// Writes the declaration/definitions of 'F' to StringBuffer 'SB', taking into account parameters specified in specification 'S'.
        /// </summary>
        public override void WriteFunction()
        {
            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                if (IsRandom(m_specification, m_fgs)) // generate function for point from random coordinates
                {
                    StringBuilder declSB = m_cgd.m_declSB;
                    StringBuilder defSB = (m_specification.m_inlineFunctions) ? m_cgd.m_inlineDefSB : m_cgd.m_defSB;

                    string funcName = m_fgs.OutputName;
                    if (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C)
                        funcName = FT.GetMangledName(m_specification, m_fgs.OutputName);

                    System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                    argTable["S"] = m_specification;
                    argTable["functionName"] = funcName;
                    argTable["pointType"] = FT.GetMangledName(m_specification, m_fgs.m_returnTypeName);
                    argTable["FT"] = FT;
                    argTable["randomScalarFuncName"] = m_randomScalarFunc[floatName];
                    argTable["cgaPointFunc"] = m_cgaPointFunc[floatName];

                    m_funcName[FT.type] = funcName;

                    // header
                    m_cgd.m_cog.EmitTemplate(declSB, "randomCgaPointHeader", argTable);
                    // source
                    m_cgd.m_cog.EmitTemplate(defSB, "randomCgaPoint", argTable);
                }
                else // generate function for point from vector or from coordinates
                {
                    bool computeMultivectorValue = true;
                    G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT,
                        m_specification.m_GMV.Name, computeMultivectorValue);

                    // change the output name (only required for language 'C' because no overloading)
                    //string funcName = m_fgs.OutputName;

                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, m_fgs, FAI);

                    m_funcName[FT.type] = CF.OutputName;

                    // generate comment
                    String comment = "/** " + m_fgs.AddUserComment("Returns conformal point.") + " */";

                    // write out the function:
                    G25.CG.Shared.Functions.WriteSpecializedFunction(m_specification, m_cgd, CF, FT, FAI, m_returnValue, comment);
                }
            }
        } // end of WriteFunction

        protected Dictionary<string, string> m_randomScalarFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomVectorFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_spFuncName = new Dictionary<string, string>();

        /// <summary>
        /// This function checks the dependencies for the _testing_ code of this function. If dependencies are
        /// missing, the function adds the required functions (this is done simply by asking for them . . .).
        /// </summary>
        public override void CheckTestingDepencies()
        {
            //bool returnTrueName = true;
            foreach (string floatName in m_fgs.FloatNames)
            {
                string defaultReturnTypeName = null;
                FloatType FT = m_specification.GetFloatType(floatName);

                m_randomScalarFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + FT.type, new String[0], FT.type, FT, null);

                if (m_vectorType != null)
                {
                    m_randomVectorFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_vectorType.GetName(), new String[0], defaultReturnTypeName, FT, null);
                    m_randomVectorFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_vectorType.GetName(), new String[0], defaultReturnTypeName, FT, null);
                }
                else m_randomVectorFuncName[FT.type] = "not_set";

                m_spFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "sp", new String[] { m_fgs.m_returnTypeName, m_fgs.m_returnTypeName }, defaultReturnTypeName, FT, m_G25M.m_name);
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

                testFuncNames.Add(testFuncName);
                System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                argTable["S"] = m_specification;
                argTable["FT"] = FT;
                argTable["testFuncName"] = testFuncName;
                argTable["targetFuncName"] = m_funcName[FT.type];
                argTable["coordBased"] = IsCoordBased(m_specification, m_fgs, FT);
                argTable["vectorBased"] = IsVectorBased(m_specification, m_fgs, FT);
                argTable["random"] = IsRandom(m_specification, m_fgs);
                argTable["cgaPointName"] = FT.GetMangledName(m_specification, m_fgs.m_returnTypeName);
                argTable["cgaPointType"] = m_specification.GetType(m_fgs.m_returnTypeName);
                argTable["vectorName"] = (m_vectorType == null) ? "not_set" : FT.GetMangledName(m_specification, m_vectorType.GetName());
                argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                argTable["randomVectorFuncName"] = m_randomVectorFuncName[FT.type];
                argTable["spFuncName"] = m_spFuncName[FT.type];

                m_cgd.m_cog.EmitTemplate(defSB, "testCgaPoint", argTable);
            }

            return testFuncNames;
        } // end of WriteTestFunction()

    } // end of class CgaPoint
} // end of namespace G25.CG.Shared.Func


