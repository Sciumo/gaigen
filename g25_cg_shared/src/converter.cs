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

namespace G25.CG.Shared
{

    /// <summary>
    /// Handles code generation of all converter and algebra functions (G25.fgs).
    /// </summary>
    public class Converter
    {
        public Converter(Specification S, G25.CG.Shared.CGdata cgd, G25.fgs F)
        {
            m_specification = S;
            m_cgd = cgd;
            m_fgs = F;
            m_fgs.InitArgumentPtrFromTypeNames(S);
        }

        /// <summary>
        /// Writes code for all converters (as specified in Function Generation Specification).
        /// </summary>
        public void WriteConverter()
        {
            try
            {
                string rawSrcTypeName = m_fgs.GetArgumentTypeName(0, null); // null = no default name
                string rawDstTypeName = m_fgs.Name.Substring(1); // dest = function name minus the underscore.
                bool srcIsGMV = m_specification.m_GMV.GetName().Equals(rawSrcTypeName);

                const int nbArgs = 1;
                foreach (string floatName in m_fgs.FloatNames) 
                {
                    FloatType FT = m_specification.GetFloatType(floatName);

                    string srcTypeName = FT.GetMangledName(m_specification, rawSrcTypeName);
                    string dstTypeName = FT.GetMangledName(m_specification, rawDstTypeName);

                    bool computeMultivectorValue = true;
                    G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, nbArgs, FT, m_specification.m_GMV.Name, computeMultivectorValue);

                    // comment:
                    Comment comment = GetComment(m_specification, srcTypeName, dstTypeName, FAI[0].Name, m_fgs.Comment);
                    comment.Write((m_specification.OutputCSharpOrJava()) ? m_defSB : m_declSB, m_specification, 1);

                    string funcName = G25.CG.Shared.Converter.GetConverterName(m_specification, m_fgs, srcTypeName, dstTypeName);

                    if (srcIsGMV)
                    {
                        // convert GMV to SMV
                        writeGmvToSmvConverter(FT, srcTypeName, dstTypeName, comment, funcName, FAI);
                    }
                    else
                    {
                        // convert SMV/scalar to SMV
                        // if scalar or specialized: generate specialized function: first get symbolic result
                        RefGA.Multivector value = FAI[0].MultivectorValue[0];

                        G25.CG.Shared.CGdata localCGD = new G25.CG.Shared.CGdata(m_cgd, m_declSB, m_defSB, m_inlineDefSB);

                        G25.CG.Shared.FuncArgInfo returnArgument =
                            new G25.CG.Shared.FuncArgInfo(m_specification, m_fgs, -1, FT, rawDstTypeName, false); // false = compute value

                        bool staticFunc = Functions.OutputStaticFunctions(m_specification);
                        bool mustCast = false;
                        if (m_specification.OutputC())
                        {
                            Functions.WriteAssignmentFunction(m_specification, localCGD, m_specification.m_inlineSet, staticFunc,
                                "void", null, funcName, returnArgument, FAI, FT, mustCast, returnArgument.Type as G25.SMV,
                                returnArgument.Name,
                                returnArgument.Pointer, value);
                        }
                        else
                        {
                            Functions.WriteReturnFunction(
                                   m_specification, localCGD, m_specification.m_inlineSet, staticFunc,
                                   funcName, FAI, FT, mustCast, returnArgument.Type as G25.SMV, value);
                        }
                    }
                }
            }
            catch (G25.UserException E)
            {
                if (E.m_XMLerrorSource.Length == 0)
                {
                    string XMLstring = XML.FunctionToXmlString(m_specification, m_fgs);
                    m_cgd.AddError(new G25.UserException(E.m_message, XMLstring, E.m_filename, E.m_line, E.m_column));
                }
                else m_cgd.AddError(E);
            }

        } // end of function WriteConverter()

        protected void writeGmvToSmvConverter(FloatType FT, string srcTypeName, string dstTypeName, Comment comment, string funcName,  G25.CG.Shared.FuncArgInfo[] FAI) {

            // verbatim code
            List<Instruction> instructions = new List<Instruction>();
            int nbTabs = 1;
            instructions.Add(new VerbatimCodeInstruction(nbTabs, "return " + dstTypeName + "(" + FAI[0].Name + ", 0);"));
            G25.CG.Shared.CGdata localCGD = new G25.CG.Shared.CGdata(m_cgd, m_declSB, m_defSB, m_inlineDefSB);

            string returnType = dstTypeName;
            bool staticFunc = Functions.OutputStaticFunctions(m_specification);
            FuncArgInfo returnArgument = null;
            Functions.WriteFunction(m_specification, localCGD, this.m_fgs, m_specification.m_inlineSet,
                staticFunc, returnType, funcName, returnArgument, FAI, instructions, comment);

        }


        /// <summary>
        /// Returns the name of a converter function. For example, <c>"dualSphere_to_vectorE3GA"</c>.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="F">Used for OutputName. Can be null for default name.</param>
        /// <param name="mangledSrcTypename">Source type (e.g. <c>"dualSphere"</c>).</param>
        /// <param name="mangledDstTypename">Destination type (e.g. <c>"vectorE3GA"</c>).</param>
        /// <returns>the name of a converter function.</returns>
        public static string GetConverterName(G25.Specification S, G25.fgs F, string mangledSrcTypename, string mangledDstTypename)
        {
            if ((F != null) && (F.OutputName != F.Name)) return F.OutputName;

            switch (S.m_outputLanguage)
            {
                case OUTPUT_LANGUAGE.C:
                    return mangledSrcTypename + "_to_" + mangledDstTypename;
                case OUTPUT_LANGUAGE.CPP:
                case OUTPUT_LANGUAGE.CSHARP:
                case OUTPUT_LANGUAGE.JAVA:
                    return "_" + mangledDstTypename;
                default:
                    throw new Exception("Not implemented yet");
            }
            
        }

        public static Comment GetComment(Specification S, string srcTypeName, string dstTypeName, string argName, string extraComment)
        {
            Comment comment = new Comment("Converts " + srcTypeName + " to " + dstTypeName + ": " + "dst" + " = " + argName + ".");
            if ((extraComment != null) && (extraComment.Length > 0))
            {
                comment = comment.AddSummaryComment(extraComment);
            }
            return comment;
        }


        public static void WriteMemberConverter(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.fgs fgs, SMV srcSmv, SMV dstSmv)
        {

            if (S.OutputCSharp())
            {
                WriteExplicitConverterCSharp(SB, S, FT, fgs, srcSmv, dstSmv);
            }

            if (S.OutputCSharpOrJava())
            {
                WriteConvertingConstructor(SB, S, cgd, FT, fgs, srcSmv, dstSmv);
            }

        }

        private static void WriteExplicitConverterCSharp(StringBuilder SB, Specification S, FloatType FT, G25.fgs fgs, SMV srcSmv, SMV dstSmv)
        {
            string srcTypeName = FT.GetMangledName(S, srcSmv.GetName());
            string dstTypeName = FT.GetMangledName(S, dstSmv.GetName());
            string argName = "x";
            string extraComment = null;
            Comment comment = GetComment(S, srcTypeName, dstTypeName, argName, extraComment);
            comment.Write(SB, S, 1);
            SB.AppendLine("\tpublic static explicit operator " + dstTypeName + " (" + srcTypeName + " " + argName + ") {");
            SB.AppendLine("\t\treturn " + S.m_namespace + "." + FT.GetMangledName(S, fgs.m_outputName) + /*dstTypeName*/ "(" + argName + ");");
            SB.AppendLine("\t}");
        }

        private static void WriteConvertingConstructor(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.fgs fgs, SMV srcSmv, SMV dstSmv)
        {
            string srcTypeName = FT.GetMangledName(S, srcSmv.GetName());
            string dstTypeName = FT.GetMangledName(S, dstSmv.GetName());
            string argName = "x";
            Comment comment = new Comment("Converting constructor, from " + srcTypeName + " to " + dstTypeName);
            comment.Write(SB, S, 1);

            SB.AppendLine("\tpublic " + dstTypeName + "(" + srcTypeName + " " + argName + ") {");

            int nbTabs = 2;
            bool declareVariable = false;
            bool cast = false;
            bool srcPtr = false;
            bool dstPtr = false;
            RefGA.Multivector value = Symbolic.SMVtoSymbolicMultivector(S, srcSmv, argName, srcPtr);
            AssignInstruction AI = new AssignInstruction(nbTabs, dstSmv, FT, cast, value, SmvUtil.THIS, dstPtr, declareVariable);
            AI.Write(SB, S, cgd);

            SB.AppendLine("\t}");
        }

        protected Specification m_specification;
        public G25.CG.Shared.CGdata m_cgd; // todo: this cgd and the SBs below should be handled the same way the functions do!
        protected G25.fgs m_fgs;

        /// <summary>
        /// Generated declarations go here.
        /// </summary>
        public StringBuilder m_declSB = new StringBuilder();
        /// <summary>
        /// Generated definitions go here.
        /// </summary>
        public StringBuilder m_defSB = new StringBuilder();
        /// <summary>
        /// Generated inline definitions go here.
        /// </summary>
        public StringBuilder m_inlineDefSB = new StringBuilder();

    } // end of class Converter

} // end of namepace G25.CG.Shared
