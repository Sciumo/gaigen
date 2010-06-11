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
    /// Generates code for generating random multivectors (using the geometric product 
    /// or the outer product to generate versors or blades).
    /// The name should be "random_versor" or "random_blade".
    /// 
    /// <code>
    /// <function name="random_blade" floatType="float" metric="euclidean" />
    /// </code>
    /// </summary>
    public class RandomGMV : G25.CG.Shared.BaseFunctionGenerator
    {

        protected Dictionary<string, string> m_randomScalarFunc = new Dictionary<string, string>(); ///< = mangled name of random scalar func 
        protected Dictionary<string, string> m_gpopFunc = new Dictionary<string, string>(); ///< = mangled name of geometric product or outer product func
        protected Dictionary<string, string> m_scalarGpFunc = new Dictionary<string, string>(); ///< = mangled name of geometric product of GMV and scalar
        protected Dictionary<string, string> m_normFunc = new Dictionary<string, string>(); ///< = mangled name of norm func
        protected Dictionary<string, string> m_GMVname = new Dictionary<string, string>(); ///< = mangled name of GMV
        protected Dictionary<string, string> m_funcName = new Dictionary<string, string>();  ///< generated function name with full mangling, etc


        /// <returns>true when F.Name == "random_versor"</returns>
        public static bool IsVersor(G25.fgs F)
        {
            return F.Name == "random_versor";
        }

        /// <returns>true when F.Name == "random_blade"</returns>
        public static bool IsBlade(G25.fgs F)
        {
            return F.Name == "random_blade";
        }


        /// <summary>
        /// Checks if this FunctionGenerator can implement a certain function.
        /// </summary>
        /// <param name="S">The specification of the algebra.</param>
        /// <param name="F">The function to be implemented.</param>
        /// <returns>true if 'F' can be implemented</returns>
        public override bool CanImplement(Specification S, G25.fgs F)
        {
            return IsVersor(F) || IsBlade(F);
        }



        /// <summary>
        /// If this FunctionGenerator can implement 'F', then this function should complete the (possible)
        /// blanks in 'F'. This means:
        ///  - Fill in F.m_returnTypeName if it is empty
        ///  - Fill in F.m_argumentTypeNames (and m_argumentVariableNames) if it is empty.
        /// </summary>
        public override void CompleteFGS()
        {
            // compute intermediate results, set return type
            m_fgs.m_returnTypeName = m_gmv.Name;

            // init argument pointers from the completed typenames (language sensitive);
            m_fgs.InitArgumentPtrFromTypeNames(m_specification);


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

                m_GMVname[floatName] = FT.GetMangledName(m_specification, GMVname);

                //bool returnTrueName = false;
                m_randomScalarFunc[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, RandomScalar.RANDOM + floatName, new string[0], floatName, FT, null);
                //if (m_specification.m_outputLanguage != OUTPUT_LANGUAGE.C)
                  //  m_randomScalarFunc[floatName] = FT.GetMangledName(m_specification, m_randomScalarFunc[floatName]);

                m_normFunc[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "norm", new string[] { GMVname }, FT, m_G25M.m_name) + G25.CG.Shared.CANSparts.RETURNS_SCALAR;

                m_scalarGpFunc[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new string[] { GMVname, floatName }, FT, null); // null = no metric for op required

                if (IsBlade(m_fgs)) m_gpopFunc[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "op", new string[] { GMVname, GMVname }, FT, null); // null = no metric for op required
                else if (IsVersor(m_fgs)) m_gpopFunc[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new string[] { GMVname, GMVname }, FT, m_G25M.m_name);


            }
        }



        /// <summary>
        /// Writes the declaration/definitions of 'F' to StringBuffer 'SB', taking into account parameters specified in specification 'S'.
        /// </summary>
        public override void WriteFunction()
        {
            StringBuilder declSB = m_cgd.m_declSB;
            bool inline = false; // never inline GMV functions
            StringBuilder defSB = (inline) ? m_cgd.m_inlineDefSB : m_cgd.m_defSB;

            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                string funcName = FT.GetMangledName(m_specification, m_fgs.OutputName);
                m_funcName[FT.type] = funcName;

                // setup hashtable with template arguments:
                System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                argTable["S"] = m_specification;
                argTable["functionName"] = funcName;
                argTable["FT"] = FT;
                argTable["mvType"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                argTable["randomScalarFuncName"] = m_randomScalarFunc[floatName];
                argTable["gpopFuncName"] = m_gpopFunc[floatName];
                argTable["normFuncName"] = m_normFunc[floatName];
                argTable["gpScalarFuncName"] = m_scalarGpFunc[floatName];
                argTable["generatorVersor"] = (IsVersor(m_fgs) ? true : false);

                if (m_specification.OutputCppOrC())
                {
                    // header
                    m_cgd.m_cog.EmitTemplate(declSB, "randomBladeVersorHeader", argTable);
                }

                // source
                m_cgd.m_cog.EmitTemplate(defSB, "randomBladeVersor", argTable);
            }
        } // end of WriteFunction

        // used for testing:
        protected Dictionary<string, string> m_randomScalarFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_reverseFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_gpFuncName = new Dictionary<string, string>();

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
                m_gpFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, m_G25M.m_name);
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
                argTable["gmv"] = m_specification.m_GMV;
                argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                argTable["testFuncName"] = testFuncName;
                argTable["targetFuncName"] = m_funcName[FT.type];
                argTable["targetFuncReturnsFloat"] = m_specification.IsFloatType(m_fgs.ReturnTypeName);
                argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                argTable["reverseFuncName"] = m_reverseFuncName[FT.type];
                argTable["gpFuncName"] = m_gpFuncName[FT.type];
                m_cgd.m_cog.EmitTemplate(defSB, "testRandomGMV", argTable);
            }

            return testFuncNames;
        } // end of WriteTestFunction()



    } // end of class RandomGMV
} // end of namespace G25.CG.Shared.Func

