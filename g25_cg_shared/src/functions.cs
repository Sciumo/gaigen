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
using System.Threading;

namespace G25.CG.Shared
{
    /// <summary>
    /// The class contains static functions for code generation of functions.
    /// 
    /// The most general function is WriteFunction() which takes as main input
    /// a set of high level GA-instructions and figures out the rest (return type, etc)
    /// itself.
    /// 
    /// There are other more specialized functions like WriteSpecializedFunction(),
    /// WriteAssignmentFunction() and WriteReturnFunction() but these should be considered
    /// obsolete.
    /// </summary>
    public class Functions
    {
        /// <summary>
        /// Returns true for C# and Java. This is used to determine whether the
        /// user functions are all static (because they must be put in class instead of a namespace).
        /// </summary>
        public static bool OutputStaticFunctions(Specification S) {
            return (S.OutputCSharpOrJava());
        }

        /// <summary>
        /// Writes code for all functions to <c>cgd.m_defSB</c>, <c>cgd.m_inlineDefSB and</c> <c>cgd.m_declSB</c>.
        /// 
        /// This is the regular entrypoint. The other <c>WriteFunctions()</c> takes an explicit list
        /// of functions and is used by <c>TestSuite.GenerateCode()</c>.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Generated code goes here</param>
        /// <param name="FGI">Info about what plugin should implement what function 
        /// The member variables are initialized by this function. <c>FGI</c> can be null when
        /// no testing code is generated. This allows the memory of the FunctionGenerators to be released
        /// earlier on.</param>
        /// <param name="plugins">The available function generators.</param>
        public static void WriteFunctions(Specification S, G25.CG.Shared.CGdata cgd, G25.CG.Shared.FunctionGeneratorInfo FGI, List<G25.CG.Shared.BaseFunctionGenerator> plugins)
        {
            WriteFunctions(S, cgd, FGI, plugins, S.m_functions);
        }

        /// <summary>
        /// Writes code for all functions to <c>cgd.m_defSB</c>, <c>cgd.m_inlineDefSB and</c> <c>cgd.m_declSB</c>.
        /// 
        /// Takes an explicit list of functions (used by  <c>TestSuite.GenerateCode()</c>).
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Generated code goes here</param>
        /// <param name="FGI">Info about what plugin should implement what function 
        /// The member variables are initialized by this function. <c>FGI</c> can be null when
        /// no testing code is generated. This allows the memory of the FunctionGenerators to be released
        /// earlier on.</param>
        /// <param name="plugins">The available function generators.</param>
        /// <param name="functions">List of <c>G25.fgs</c> to implement.</param>
        public static void WriteFunctions(
            Specification S,
            G25.CG.Shared.CGdata cgd,
            G25.CG.Shared.FunctionGeneratorInfo FGI,
            List<G25.CG.Shared.BaseFunctionGenerator> plugins,
            List<G25.fgs> functions)
        {

            // first split functions into converters and regular functions
            List<G25.fgs> converterFGS = new List<fgs>();
            List<G25.fgs> functionFGS = new List<fgs>();
            foreach (G25.fgs F in functions)
            {
                if (F.IsConverter(S)) // is 'F' a converter (underscore constructor)?
                    converterFGS.Add(F);
                else functionFGS.Add(F);
            }

            // store converterFGS and functionFGS in FGI, for later use by TestSuite.GenerateCode()
            if (FGI != null)
            {
                FGI.m_converterFGS = converterFGS;
                FGI.m_functionFGS = functionFGS;
            }

            // start threads to generate code for all converters
            Thread[] converterThreads = new Thread[converterFGS.Count];
            Converter[] converters = new Converter[converterFGS.Count];
            for (int f = 0; f < converterFGS.Count; f++)
            {
                G25.fgs F = converterFGS[f];
                converters[f] = new Converter(S, cgd, F);
                converterThreads[f] = new Thread(converters[f].WriteConverter);
            }
            G25.CG.Shared.Threads.StartThreadArray(converterThreads);

            // find out which plugin can implement which FGS
            Thread[] functionThreads = new Thread[functionFGS.Count];
            G25.CG.Shared.BaseFunctionGenerator[] functionGenerators = new G25.CG.Shared.BaseFunctionGenerator[functionFGS.Count];
            G25.CG.Shared.CGdata[] functionCgd = new G25.CG.Shared.CGdata[functionFGS.Count];
            for (int f = 0; f < functionFGS.Count; f++)
            {
                G25.fgs F = functionFGS[f];

                bool foundSuitablePlugin = false;
                foreach (G25.CG.Shared.BaseFunctionGenerator P in plugins) // check all C plugins
                {
                    if (P.CanImplement(S, F)) // ask them if they can handle 'F'
                    {
                        // get a clean instance of the code generator and initialize it
                        functionGenerators[f] = System.Activator.CreateInstance(P.GetType()) as BaseFunctionGenerator;
                        functionCgd[f] = new G25.CG.Shared.CGdata(cgd); // m_errors and m_missingDependencies will be shared with main cgd!
                        functionGenerators[f].Init(S, F, functionCgd[f]);
                        foundSuitablePlugin = true;
                        break;
                    }
                }
                if (!foundSuitablePlugin)
                {// no plugin could do 'F': complain about it
                    System.Console.WriteLine("Warning no suitable plugin for function " + F.Name + "; XML specification: ");
                    System.Console.WriteLine("    " + XML.FunctionToXmlString(S, F));
                }
            }

            // store functionGenerators in FGI, for later use by TestSuite.GenerateCode()
            if (FGI != null)
            {
                FGI.m_functionGenerators = functionGenerators;
                FGI.m_functionCgd = functionCgd;
            }

            // run threads for fill-in of functions
            for (int f = 0; f < functionFGS.Count; f++)
            {
                if (functionGenerators[f] == null) continue;
                functionThreads[f] = new Thread(functionGenerators[f].CompleteFGSentryPoint);
            }
            G25.CG.Shared.Threads.StartThreadArray(functionThreads);
            G25.CG.Shared.Threads.JoinThreadArray(functionThreads);
            //G25.CG.Shared.Threads.RunThreadArraySerially(functionThreads);


            // runs thread for dependency check of functions
            for (int f = 0; f < functionFGS.Count; f++)
            {
                if (functionGenerators[f] == null) continue;
                functionThreads[f] = new Thread(functionGenerators[f].CheckDepenciesEntryPoint);
            }
            G25.CG.Shared.Threads.StartThreadArray(functionThreads);
            G25.CG.Shared.Threads.JoinThreadArray(functionThreads);
            //G25.CG.Shared.Threads.RunThreadArraySerially(functionThreads);

            // runs thread for actual code generation of functions
            for (int f = 0; f < functionFGS.Count; f++)
            {
                if (functionGenerators[f] == null) continue;
                functionThreads[f] = new Thread(functionGenerators[f].WriteFunctionEntryPoint);
            }
            G25.CG.Shared.Threads.StartThreadArray(functionThreads);
            //G25.CG.Shared.Threads.RunThreadArraySerially(functionThreads);

            // join all the converter threads:
            G25.CG.Shared.Threads.JoinThreadArray(converterThreads);

            // join all function generation threads
            G25.CG.Shared.Threads.JoinThreadArray(functionThreads);

            // collect all the results from the threads:
            for (int f = 0; f < converters.Length; f++)
            {
                cgd.m_declSB.Append(converters[f].m_declSB);
                cgd.m_defSB.Append(converters[f].m_defSB);
                cgd.m_inlineDefSB.Append(converters[f].m_inlineDefSB);
                cgd.MergeErrors(converters[f].m_cgd);
            }

            // collect all the results from the threads:
            for (int f = 0; f < functionCgd.Length; f++)
            {
                if (functionGenerators[f] == null) continue;
                cgd.m_declSB.Append(functionCgd[f].m_declSB);
                cgd.m_defSB.Append(functionCgd[f].m_defSB);
                cgd.m_inlineDefSB.Append(functionCgd[f].m_inlineDefSB);
            }

        } // end of WriteFunctions()


        /// <summary>
        /// Writes a simple function which assigns some value based on specialized multivectors or scalars to some variable.
        /// 
        /// Used by a lot of simple C functions, like gp, op, lc.
        /// 
        /// Somewhat obsolete (preferably, use the more generic, instruction based WriteFunction()).
        /// 
        /// </summary>
        /// <param name="S">Specification of algebra.</param>
        /// <param name="cgd">Results go into cgd.m_defSB, and so on</param>
        /// <param name="F">The function generation specification.</param>
        /// <param name="FT">Default float pointer type.</param>
        /// <param name="FAI">Info on the arguments of the function.</param>
        /// <param name="value">The value to assign.</param>
        /// <param name="comment">Optional comment for function (can be null).</param>
        public static void WriteSpecializedFunction(Specification S, G25.CG.Shared.CGdata cgd, G25.fgs F,
            FloatType FT, G25.CG.Shared.FuncArgInfo[] FAI, RefGA.Multivector value, string comment)
        {
            // get return type (may be a G25.SMV or a G25.FloatType)
            G25.VariableType returnType = G25.CG.Shared.SpecializedReturnType.GetReturnType(S, cgd, F, FT, value);

            if (returnType == null)
                throw new G25.UserException("Missing return type: " + G25.CG.Shared.BasisBlade.MultivectorToTypeDescription(S, value),
                    XML.FunctionToXmlString(S, F));

            bool ptr = true;

            string dstName = G25.fgs.RETURN_ARG_NAME;
            //string dstTypeName = (returnType is G25.SMV) ? FT.GetMangledName((returnType as G25.SMV).Name) : (returnType as G25.FloatType).type;

            string funcName = F.OutputName;

            // write comment to declaration
            if (comment != null) cgd.m_declSB.AppendLine(comment);

            if ((returnType is G25.SMV) && (S.OutputC()))
            {
                bool mustCast = false;
                G25.SMV dstSmv = returnType as G25.SMV;

                G25.CG.Shared.FuncArgInfo returnArgument = null;
                returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, FT, dstSmv.Name, false); // false = compute value

                bool staticFunc = false;
                G25.CG.Shared.Functions.WriteAssignmentFunction(S, cgd,
                    S.m_inlineFunctions, staticFunc, "void", null, funcName, returnArgument, FAI, FT, mustCast, dstSmv, dstName, ptr,
                    value);
            }
            else 
            {
                G25.FloatType returnFT = ((returnType as G25.FloatType) == null) ? FT : (returnType as G25.FloatType);

                bool mustCast = false;
                for (int i = 0; i < FAI.Length; i++)
                    mustCast |= returnFT.MustCastIfAssigned(S, FAI[i].FloatType);

                bool staticFunc = false;
                G25.CG.Shared.Functions.WriteReturnFunction(S, cgd,
                    S.m_inlineSet, staticFunc, funcName, FAI, FT, mustCast, returnType, value);

            }
        } // end of WriteSpecializedFunction()

        /// <summary>
        /// Writes a function declaration to 'SB'.
        /// The closing comma is NOT included so the function can also be used as the start of a definition.
        /// </summary>
        /// <param name="SB">Where declaration goes.</param>
        /// <param name="S">Used for all kinds of stuff.</param>
        /// <param name="cgd">Results go into cgd.m_defSB, and so on</param>
        /// <param name="inline">Should the function we inline?</param>
        /// <param name="staticFunc">Static function?</param>
        /// <param name="returnType">String which speficies the return type.</param>
        /// <param name="functionName">The name of the function which is to be generated.</param>
        /// <param name="returnArgument">FuncArgInfo which describes the optional return argument.</param>
        /// <param name="arguments">Array of FuncArgInfo which describes the arguments of the function.</param>
        public static void WriteDeclaration(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd,
            bool inline, bool staticFunc, string returnType, string functionName,
            FuncArgInfo returnArgument, FuncArgInfo[] arguments)
        {
            if (S.OutputJava())
                SB.Append("public final ");
            else if (S.OutputCSharp())
                SB.Append("public ");

            if (staticFunc) SB.Append("static ");

            SB.Append(G25.CG.Shared.Util.GetInlineString(S, inline, " "));
            if (returnArgument != null) returnType = "void";
            SB.Append(returnType);
            SB.Append(" ");
            SB.Append(functionName);
            SB.Append("(");

            { // write arguments
                bool appendComma = false;

                int nbArgs = (arguments == null) ? 0 : arguments.Length;
                for (int i = -1; i < nbArgs; i++) // start at -1 for return argument
                {
                    FuncArgInfo A = null;
                    if (i == -1)
                    {
                        A = returnArgument;
                        if (A == null) continue;
                    }
                    else A = arguments[i];

                    if (appendComma) SB.Append(", ");

                    if (S.OutputJava())
                        SB.Append("final ");

                    if (A.Constant && S.OutputCppOrC()) 
                        SB.Append("const ");

                    SB.Append(A.MangledTypeName);

                    if (A.Array && S.OutputCSharpOrJava())
                        SB.Append("[]");

                    SB.Append(" ");

                    if (A.Pointer) SB.Append("*");
                    else if (S.OutputCpp() && (A.IsMVorOM())) // append '&'?
                            SB.Append("&");
                    SB.Append(A.Name);

                    if (A.Array && S.OutputCppOrC())
                        SB.Append("[]");

                    appendComma = true;
                }
            }
            SB.Append(")");
        } // end of WriteDeclaration()

        /// <summary>
        /// Writes generic function based on Instructions.
        /// 
        /// This version automatically figures out the return type (so it is recommended over the other WriteFunction()).
        /// </summary>
        /// <param name="S">Specification of algebra.</param>
        /// <param name="cgd">Results go into cgd.m_defSB, and so on</param>
        /// <param name="F">Function specification.</param>
        /// <param name="inline">When true, the code is inlined.</param>
        /// <param name="staticFunc">static function?</param>
        /// <param name="functionName">Name of generated function.</param>
        /// <param name="arguments">Arguments of function (any 'return argument' used for the C language is automatically setup correctly).</param>
        /// <param name="instructions">List of GA-instructions which make up the function.</param>
        /// <param name="comment">Comment to go into generated declration code.</param>
        public static void WriteFunction(
            Specification S, G25.CG.Shared.CGdata cgd, G25.fgs F,
            bool inline, bool staticFunc, string functionName, FuncArgInfo[] arguments,
            List<Instruction> instructions, string comment)
        {
            List<G25.VariableType> returnTypes = new List<G25.VariableType>();
            List<G25.FloatType> returnTypeFT = new List<G25.FloatType>(); // floating point types of return types
            Instruction.GetReturnType(instructions, returnTypes, returnTypeFT);

            // for now, assume only one return type is used?
            string returnType = "void";
            G25.CG.Shared.FuncArgInfo returnArgument = null;
            if (returnTypes.Count > 0)
            {
                G25.VariableType returnVT = returnTypes[0];
                G25.FloatType returnFT = returnTypeFT[0];

                if (returnVT is G25.FloatType) returnVT = returnFT;

                string returnTypeName = (returnVT is G25.MV) ? (returnVT as G25.MV).Name : (returnVT as FloatType).type;

                returnType = returnFT.GetMangledName(S, returnTypeName);

                if (S.OutputC())
                {
                    if (!(returnVT is G25.FloatType))
                    {
                        returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, returnFT, returnTypeName, false); // false = compute value
                    }
                }
            }

            WriteFunction(S, cgd, F, inline, staticFunc, returnType, functionName, returnArgument, arguments, instructions, comment);
        } // end of WriteFunction()


        public static void WriteFunction(
            Specification S, G25.CG.Shared.CGdata cgd, G25.fgs F, 
            bool inline, bool staticFunc, string returnType, string functionName,
            FuncArgInfo returnArgument, FuncArgInfo[] arguments,
            System.Collections.Generic.List<Instruction> instructions, string comment)
        {
            bool writeDecl = S.OutputCppOrC();
            
            WriteFunction(S, cgd, F, inline, staticFunc, returnType, functionName, returnArgument, arguments, instructions, comment, writeDecl);
        }


        /// <summary>
        /// Writes generic function based on Instructions.
        /// 
        /// The other WriteFunction() can figure out the return type automatically, so
        /// it is preferred over this more verbose version.
        /// </summary>
        /// <param name="S">Specification of algebra.</param>
        /// <param name="cgd">Results go into cgd.m_defSB, and so on</param>
        /// <param name="F">Function specification.</param>
        /// <param name="inline">When true, the code is inlined.</param>
        /// <param name="staticFunc">Static function?</param>
        /// <param name="returnType">The type to return (String, can also be e.g. <c>"code"</c>.</param>
        /// <param name="functionName">Name of generated function.</param>
        /// <param name="returnArgument">For use with the 'C' language, an extra argument can be used to return results.</param>
        /// <param name="arguments">Arguments of function (any `return argument' used for the C language is automatically generated).</param>
        /// <param name="instructions">List of GA-instructions which make up the function.</param>
        /// <param name="comment">Comment to go into generated code (used for decl only).</param>
        /// <param name="writeDecl">When false, no declaration is written</param>
        public static void WriteFunction(
            Specification S, G25.CG.Shared.CGdata cgd, G25.fgs F, 
            bool inline, bool staticFunc, string returnType, string functionName,
            FuncArgInfo returnArgument, FuncArgInfo[] arguments,
            System.Collections.Generic.List<Instruction> instructions, string comment, bool writeDecl)
        {
            // where the definition goes:
            StringBuilder defSB = (inline) ? cgd.m_inlineDefSB : cgd.m_defSB;

            // declaration:
            if (writeDecl)
            {
                if (comment != null) cgd.m_declSB.AppendLine(comment);
                WriteDeclaration(cgd.m_declSB, S, cgd, inline, staticFunc, returnType, functionName, returnArgument, arguments);
                cgd.m_declSB.AppendLine(";");
            }

            WriteDeclaration(defSB, S, cgd, inline, staticFunc, returnType, functionName, returnArgument, arguments);


            // open function
            defSB.AppendLine("");
            defSB.AppendLine("{");

            // add extra instruction for reporting usage of SMVs
            if (S.m_reportUsage)
                instructions.Insert(0, ReportUsage.GetReportInstruction(S, F, arguments));

            // write all instructions
            foreach (Instruction I in instructions)
            {
                I.Write(defSB, S, cgd);
            }

            // close function
            defSB.AppendLine("}");
        } // end of WriteFunction()


        /// <summary>
        /// Writes a function to 'SB' which assigns a certain 'value' to a certain 'dstName'.
        /// </summary>
        /// <param name="S">Used for all kinds of stuff.</param>
        /// <param name="cgd">Results go into cgd.m_defSB, and so on</param>
        /// <param name="inline">Should the function we inline?</param>
        /// <param name="staticFunc">Static function?</param>
        /// <param name="returnType">String which speficies the return type.</param>
        /// <param name="returnVarName">The name of the variable which should be returned. Should be one of the argument names.</param>
        /// <param name="functionName">The name of the function which is to be generated.</param>
        /// <param name="arguments">Array of FuncArg which describes the arguments of the function.</param>
        /// <param name="dstFT">Floating point type of destination variable.</param>
        /// <param name="mustCastDst">set to true if coordinates of 'value' must be cast to 'dstFT'.</param>
        /// <param name="dstSmv">G25.SMV type of destination.</param>
        /// <param name="dstName">Name of destination.</param>
        /// <param name="dstPtr">Is the destination a pointer?</param>
        /// <param name="value">Value to be written to the destination.</param>
        /// <param name="returnArgument">For use with the 'C' language, an extra argument can be used to return results.</param>
        public static void WriteAssignmentFunction(
            Specification S, G25.CG.Shared.CGdata cgd, 
            bool inline, bool staticFunc, string returnType, string returnVarName, string functionName,
            FuncArgInfo returnArgument, FuncArgInfo[] arguments,
            FloatType dstFT, bool mustCastDst, G25.SMV dstSmv, string dstName, bool dstPtr, RefGA.Multivector value)
        {
            // where the definition goes:
            StringBuilder defSB = (inline) ? cgd.m_inlineDefSB : cgd.m_defSB;

            // write declaration yes/no?
            bool writeDecl = (!(dstName.Equals(G25.CG.Shared.SmvUtil.THIS) && S.OutputCpp())) && // no declarations for C++ member functions
                (!S.OutputCSharpOrJava());  // no declarations in C# and Java
            if (writeDecl) 
            {
                WriteDeclaration(cgd.m_declSB, S, cgd, inline, staticFunc, returnType, functionName, returnArgument, arguments);
                cgd.m_declSB.AppendLine(";");
            }

            WriteDeclaration(defSB, S, cgd, inline, staticFunc, returnType, functionName, returnArgument, arguments);

            defSB.AppendLine("");
            defSB.AppendLine("{");

            int nbTabs = 1; 
            bool declareVariable = false;
            AssignInstruction AI = new AssignInstruction(nbTabs, dstSmv, dstFT, mustCastDst, value, dstName, dstPtr, declareVariable);
            AI.Write(defSB, S, cgd);

            if (returnVarName != null)
                defSB.AppendLine("return " + returnVarName + ";");
            defSB.AppendLine("}");
        } // end of WriteAssignmentFunction()

        /// <summary>
        /// 
        /// </summary>
        /// <param name="S">Used for all kinds of stuff.</param>
        /// <param name="cgd">Results go into cgd.m_defSB, and so on</param>
        /// <param name="inline">Should the function we inline?</param>
        /// <param name="staticFunc">Static function?</param>
        /// <param name="functionName">The name of the function which is to be generated.</param>
        /// <param name="arguments">Array of FuncArg which describes the arguments of the function.</param>
        /// <param name="returnFT">Floating point type of return variable.</param>
        /// <param name="mustCastDst">set to true if coordinates of 'value' must be cast to 'dstFT'.</param>
        /// <param name="returnType">The type to be returned.</param>
        /// <param name="value">Value to be written to the returned.</param>
        public static void WriteReturnFunction(
            Specification S, G25.CG.Shared.CGdata cgd, 
            bool inline, bool staticFunc, string functionName,
            FuncArgInfo[] arguments,
            FloatType returnFT, bool mustCastDst, G25.VariableType returnType, RefGA.Multivector value)
        {
            string returnTypeName;
            bool returnSMV = returnType is G25.SMV;

            if (returnSMV) returnTypeName = returnFT.GetMangledName(S, (returnType as G25.SMV).Name);
            else returnTypeName = (returnType as G25.FloatType).type;

            // where the definition goes:
            StringBuilder defSB = (inline) ? cgd.m_inlineDefSB : cgd.m_defSB;

            // declaration:
            WriteDeclaration(cgd.m_declSB, S, cgd, false, staticFunc, returnTypeName, functionName, null, arguments);
            WriteDeclaration(defSB, S, cgd, inline, staticFunc, returnTypeName, functionName, null, arguments);
            
            cgd.m_declSB.AppendLine(";");

            defSB.AppendLine("");
            defSB.AppendLine("{");

            int nbTabs = 1;
            ReturnInstruction RI = new ReturnInstruction(nbTabs, returnType, returnFT, mustCastDst, value);
            RI.Write(defSB, S, cgd);

            defSB.Append("\n");

            defSB.AppendLine("}");
        } // end of WriteReturnFunction()

        /// <summary>
        /// Return true if 'F' does not mix arguments of type SMV and GMV
        /// </summary>
        /// <param name="S">Specification of algebra</param>
        /// <param name="F">Function specification</param>
        /// <param name="defaultArgType">Default argument type for all arguments (todo: allow for per-argument type)</param>
        /// <param name="nbArgs">Number of arguments.</param>
        /// <returns>true if 'F' does not mix arguments of type SMV and GMV</returns>
        public static bool NotMixSmvGmv(Specification S, G25.fgs F, int nbArgs, String defaultArgType)
        {
            bool useSMV = false;
            bool useGMV = false;
            for (int a = 0; a < nbArgs; a++)
            {
                String typeName = F.GetArgumentTypeName(a, defaultArgType);
                useSMV |= S.IsSpecializedMultivectorName(typeName);
                useGMV |= S.m_GMV.Name == typeName;
            }
            return (!(useSMV && useGMV));

        }

        /// <summary>
        /// Return true if 'F' does not mix arguments of type scalar and GMV
        /// </summary>
        /// <param name="S">Specification of algebra</param>
        /// <param name="F">Function specification</param>
        /// <param name="defaultArgType">Default argument type for all arguments (todo: allow for per-argument type)</param>
        /// <param name="nbArgs">Number of arguments.</param>
        /// <returns>true if 'F' does not mix arguments of type scalar and GMV</returns>
        public static bool NotMixScalarGmv(Specification S, G25.fgs F, int nbArgs, String defaultArgType)
        {
            bool useScalar = false;
            bool useGMV = false;
            for (int a = 0; a < nbArgs; a++)
            {
                String typeName = F.GetArgumentTypeName(a, defaultArgType);
                useScalar |= S.IsFloatType(typeName);
                useGMV |= S.m_GMV.Name == typeName;
            }
            return (!(useScalar && useGMV));
        }

        /// <summary>
        /// Return true if 'F' does not use outermorphism types
        /// </summary>
        /// <param name="S">Specification of algebra</param>
        /// <param name="F">Function specification</param>
        /// <param name="defaultArgType">Default argument type for all arguments (todo: allow for per-argument type)</param>
        /// <param name="nbArgs">Number of arguments.</param>
        /// <returns>true if 'F' does not mix arguments of type SMV and GMV</returns>
        public static bool NotUseOm(Specification S, G25.fgs F, int nbArgs, String defaultArgType)
        {
            for (int a = 0; a < nbArgs; a++)
            {
                String typeName = F.GetArgumentTypeName(a, defaultArgType);
                if ((S.IsSpecializedOutermorphismName(typeName)) ||
                    ((S.m_GOM != null) && (S.m_GOM.Name == typeName))) return false;
            }
            return true;
        }

        /// <summary>
        /// Return true if argument 'argIdx' in 'F' is an outermorphism type.
        /// </summary>
        /// <param name="S">Specification of algebra</param>
        /// <param name="F">Function specification</param>
        /// <param name="argIdx">Index of argument. If out of range, false is returned.</param>
        /// <returns>true if argument 'argIdx' in 'F' is an outermorphism type.</returns>
        public static bool IsOm(Specification S, G25.fgs F, int argIdx)
        {
            if (argIdx >= F.NbArguments) return false;
            return (S.IsOutermorphismName(F.GetArgumentTypeName(argIdx, "")));
        }

        /// <summary>
        /// Return true if argument 'argIdx' in 'F' is a specialized outermorphism type.
        /// </summary>
        /// <param name="S">Specification of algebra</param>
        /// <param name="F">Function specification</param>
        /// <param name="argIdx">Index of argument. If out of range, false is returned.</param>
        /// <returns>true if argument 'argIdx' in 'F' is an outermorphism type.</returns>
        public static bool IsSom(Specification S, G25.fgs F, int argIdx)
        {
            if (argIdx >= F.NbArguments) return false;
            return (S.IsSpecializedOutermorphismName(F.GetArgumentTypeName(argIdx, "")));
        }
        
        /// <summary>
        /// Return true if argument 'argIdx' in 'F' is a multivector type.
        /// </summary>
        /// <param name="S">Specification of algebra</param>
        /// <param name="F">Function specification</param>
        /// <param name="argIdx">Index of argument. If out of range, false is returned.</param>
        /// <returns>true if argument 'argIdx' in 'F' is an multivector type.</returns>
        public static bool IsMv(Specification S, G25.fgs F, int argIdx)
        {
            if (argIdx >= F.NbArguments) return false;
            return (S.IsMultivectorName(F.GetArgumentTypeName(argIdx, "")));
        }

        /// <summary>
        /// Return true if argument 'argIdx' in 'F' is a multivector type.
        /// </summary>
        /// <param name="S">Specification of algebra</param>
        /// <param name="F">Function specification</param>
        /// <param name="argIdx">Index of argument. If out of range, false is returned.</param>
        /// <returns>true if argument 'argIdx' in 'F' is an multivector type.</returns>
        public static bool IsSmv(Specification S, G25.fgs F, int argIdx)
        {
            if (argIdx >= F.NbArguments) return false;
            return (S.IsSpecializedMultivectorName(F.GetArgumentTypeName(argIdx, "")));
        }

    }// end of class Functions
} // end of namepace G25.CG.Shared
