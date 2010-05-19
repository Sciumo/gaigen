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
    /// Generates code for generating random scalars.
    /// The name should be "random_" + the name of a float type, for example,
    /// random_float
    /// 
    /// The generator can be set using the <c>optionGen="generator"</c> attribute,
    /// where <c>generator</c> can be <c>mt</c> for mersenne twister or <c>libc</c>
    /// for the standard lib c random number generator.
    /// 
    /// <code>  
    /// <function name="random_double" outputName="genrand" optionGen="mt"/>
    /// <function name="random_float" optionGen="libc"/>
    /// </code>
    /// </summary>
    public class RandomScalar : G25.CG.Shared.BaseFunctionGenerator
    {

        /// <summary>
        /// Types of pseudo random generators.
        /// </summary>
        public enum PRGtype
        {
            LIBC = 1, ///< libc rand()
            MT ///< mersenne twister by Matsumoto.
        };

        public static string RANDOM = "random_";
        protected string m_functionNameFloatType;
        protected PRGtype m_generatorType;

        /// <summary>
        /// Checks if this FunctionGenerator can implement a certain function.
        /// </summary>
        /// <param name="S">The specification of the algebra.</param>
        /// <param name="F">The function to be implemented.</param>
        /// <returns>true if 'F' can be implemented</returns>
        public override bool CanImplement(Specification S, G25.fgs F)
        {
            return F.Name.StartsWith(RANDOM) &&
                S.IsFloatType(F.Name.Substring(RANDOM.Length));
        }


        /// <summary>
        /// If this FunctionGenerator can implement 'm_fgs', then this function should complete the (possible)
        /// blanks in 'm_fgs'. This means:
        ///  - Fill in m_fgs.m_returnTypeName if it is empty
        ///  - Fill in m_fgs.m_argumentTypeNames (and m_argumentVariableNames) if it is empty.
        /// </summary>
        public override void CompleteFGS()
        {
            m_functionNameFloatType = m_fgs.Name.Substring(RANDOM.Length);

            if (m_fgs.m_returnTypeName.Length == 0)
                m_fgs.m_returnTypeName = m_functionNameFloatType;

            string genTypeStr = m_fgs.GetOption("Gen");
            if ((genTypeStr == null) || (genTypeStr == "libc"))
                m_generatorType = PRGtype.LIBC;
            else if (genTypeStr == "mt")
                m_generatorType = PRGtype.MT;
            else throw new G25.UserException("Invalid random number generator type (optionGen) '" + genTypeStr + "'");

            /// tell code generator we are going to need MT
            if (m_generatorType == PRGtype.MT)
                m_cgd.SetFeedback(Main.MERSENNE_TWISTER, "true");
            m_cgd.SetFeedback(Main.NEED_TIME, "true");

        }

        /// <summary>
        /// Writes the declaration/definitions of 'F' to StringBuffer 'SB', taking into account parameters specified in specification 'S'.
        /// </summary>
        public override void WriteFunction()
        {
            StringBuilder declSB = m_cgd.m_declSB;
            StringBuilder defSB = (m_specification.m_inlineFunctions) ? m_cgd.m_inlineDefSB : m_cgd.m_defSB;
            string inlineStr = G25.CG.Shared.Util.GetInlineString(m_specification, m_specification.m_inlineFunctions, " ");


            G25.FloatType FT = m_specification.GetFloatType(m_functionNameFloatType);
            string funcName = FT.GetMangledName(m_specification, m_fgs.OutputName);

            // what random number generator is being used (for the comment)
            string generatorType = " unknown method";
            if (m_specification.OutputCSharp())
                generatorType = "System.Random class";
            else if (m_specification.OutputJava())
                generatorType = "java.util.Random";
            else if (m_generatorType == PRGtype.LIBC)
                generatorType = "c library rand() function";
            else if (m_generatorType == PRGtype.LIBC)
                generatorType = "mersenne twister method";

            // get comment
            string comment = "Generates a random " + m_functionNameFloatType +
                " in [0.0 1.0) interval using the " + generatorType;

            // emit declaration / comment
            if (m_specification.OutputCppOrC()) {
                Util.WriteFunctionComment(declSB, m_specification, 0, comment, null, null);
                declSB.Append(m_functionNameFloatType + " " + funcName + "();\n");
            }
            else Util.WriteFunctionComment(defSB, m_specification, 0, comment, null, null);

            // get access modifier
            string ACCESS = "";
            if (m_specification.OutputCSharp())
                ACCESS = "public static ";
            else if (m_specification.OutputJava())
                ACCESS = "public final static ";

            // emit definition {
            defSB.Append(inlineStr + ACCESS + m_functionNameFloatType + " " + funcName + "() {\n");

            // emit actual code
            if (m_specification.OutputCSharpOrJava())
            {
                defSB.Append("\treturn (" + m_functionNameFloatType + ")NextRandomDouble();\n");
            }
            else if (m_generatorType == PRGtype.LIBC)
            {
                // write template to file?
                if (m_functionNameFloatType == "float")
                    defSB.Append("\treturn (" + m_functionNameFloatType + ")(rand() & 0x7FFF) / 32768.0f  + (" + m_functionNameFloatType + ")(rand() & 0x7FFF) / (32768.0f * 32768.0f);\n");
                else
                {
                    defSB.Append("return (" + m_functionNameFloatType + ")((double)(rand() & 0x7FFF) / 32768.0) + \n");
                    defSB.Append("\t(" + m_functionNameFloatType + ")((double)(rand() & 0x7FFF) / (32768.0 * 32768.0)) + \n");
                    defSB.Append("\t(" + m_functionNameFloatType + ")((double)(rand() & 0x7FFF) / (32768.0 * 32768.0 * 32768.0)) + \n");
                    defSB.Append("\t(" + m_functionNameFloatType + ")((double)(rand() & 0x7FFF) / (32768.0 * 32768.0 * 32768.0 * 32768.0)); \n");
                }
            }
            else if (m_generatorType == PRGtype.MT)
            {
                if (m_functionNameFloatType == "float")
                    defSB.Append("\treturn (" + m_functionNameFloatType + ")genrand_real2();\n");
                else
                    defSB.Append("\treturn (" + m_functionNameFloatType + ")genrand_res53();\n");
            }
            defSB.Append("}\n");

            // seeder decls:
            string seedComment = "Seeds the random number generator for  " + m_functionNameFloatType;
            string timeSeedComment = "Seeds the random number generator for  " + m_functionNameFloatType + " with the current time";
            if (m_specification.OutputCppOrC())
            {
                Util.WriteFunctionComment(declSB, m_specification, 0, seedComment, null, null);
                declSB.AppendLine("void " + funcName + "_seed(unsigned int seed);");
                Util.WriteFunctionComment(declSB, m_specification, 0, timeSeedComment, null, null);
                declSB.AppendLine("void " + funcName + "_timeSeed();");
            }

            string SEED_TYPE = (m_specification.OutputCppOrC()) ? "unsigned int" : "int";
            // seeder defs:
            if (m_specification.OutputCSharpOrJava())
            {
                Util.WriteFunctionComment(declSB, m_specification, 0, seedComment, null, null);
            }
            defSB.AppendLine(inlineStr + ACCESS + "void " + funcName + "_seed(" + SEED_TYPE + " seed) {");
            if (m_specification.OutputCSharp())
            {
                defSB.AppendLine("\ts_randomGenerator = new System.Random(seed);");
            }
            else if (m_specification.OutputJava())
            {
                defSB.AppendLine("\ts_randomGenerator.setSeed(seed);");
            }
            else
            {
                if (m_generatorType == PRGtype.LIBC)
                    defSB.AppendLine("\tsrand(seed);");
                else if (m_generatorType == PRGtype.MT)
                    defSB.AppendLine("\tinit_genrand(seed);");
            }
            defSB.AppendLine("}\n");


            defSB.AppendLine(inlineStr + ACCESS + "void " + funcName + "_timeSeed() {");
            if (m_specification.OutputCSharp())
                defSB.AppendLine("\t" + funcName + "_seed((int)DateTime.Now.Ticks);");
            else if (m_specification.OutputJava())
                defSB.AppendLine("\t" + funcName + "_seed((int)System.currentTimeMillis());");
            else defSB.AppendLine("\t" + funcName + "_seed((unsigned int)time(NULL));");
            
            defSB.AppendLine("}\n");


        } // end of WriteFunction

        /// <summary>
        /// This function checks the dependencies for the _testing_ code of this function. If dependencies are
        /// missing, the function adds the required functions (this is done simply by asking for them . . .).
        /// </summary>
        public override void CheckTestingDepencies()
        {
            // no dependencies?
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

            // get name of test function
            string testFuncName = "test_" + m_fgs.OutputName + "_" + m_functionNameFloatType;
            testFuncNames.Add(testFuncName);

            FloatType FT = m_specification.GetFloatType(m_functionNameFloatType);
            string funcName = FT.GetMangledName(m_specification, m_fgs.OutputName);

            m_cgd.m_cog.EmitTemplate(defSB, "testRandomScalar",
                "S=", m_specification,
                "FT=", FT,
                "testFuncName=", testFuncName,
                "randomScalarFuncName=", funcName);

            return testFuncNames;
        }

    } // end of class RandomScalar
} // end of namespace G25.CG.Shared.Func

