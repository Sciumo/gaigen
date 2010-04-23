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

namespace G25
{
    namespace CG
    {
        namespace C
        {
            /// <summary>
            /// Main code generation class for the header file.
            /// </summary>
            class Header
            {


                public static string GetRawHeaderFilename(Specification S)
                {
                    return S.m_namespace + ".h";
                }


                /// <summary>
                /// Generates a header file named S.m_namespace.h.
                /// </summary>
                /// <param name="S">Specification of algebra.</param>
                /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
                /// <returns>a list of filenames which were generated (full path).</returns>
                public static List<string> GenerateCode(Specification S, G25.CG.Shared.CGdata cgd)
                {
                    // get filename, list of generated filenames
                    List<string> generatedFiles = new List<string>();
                    string headerFilename = S.GetOutputPath(G25.CG.C.Header.GetRawHeaderFilename(S));
                    generatedFiles.Add(headerFilename);

                    
                    // get StringBuilder where all generated code goes
                    StringBuilder SB = new StringBuilder();

                    // output license
                    G25.CG.Shared.Util.WriteLicense(SB, S);

                    // write main documentation
                    WriteDoxyMainPage(SB, S, cgd);

                    // open include guard
                    G25.CG.Shared.Util.WriteOpenIncludeGuard(SB, GetRawHeaderFilename(S));

                    { // #includes
                        SB.AppendLine("");
                        SB.AppendLine("#include <stdio.h>");
                        SB.AppendLine("#include <stdlib.h>");
                        SB.AppendLine("#include <string.h>");
                        SB.AppendLine("#include <math.h>");
                        if (cgd.GetFeedback(G25.CG.C.MainGenerator.MERSENNE_TWISTER) == "true")
                            SB.AppendLine("#include \"" + S.m_namespace + "_mt.h\"");
                    }

                    { // basic info

                        {    // #define GROUP_ and GRADE_
                            SB.AppendLine("");

                            // group
                            int[] gradeBitmap = new int[S.m_dimension + 1];
                            for (int i = 0; i < S.m_GMV.NbGroups; i++)
                            {
                                gradeBitmap[S.m_GMV.Group(i)[0].Grade()] |= 1 << i;

                                SB.Append("// group: ");
                                for (int j = 0; j < S.m_GMV.Group(i).Length; j++)
                                {
                                    if (j > 0) SB.Append(", ");
                                    SB.Append(S.m_GMV.Group(i)[j].ToString(S.m_basisVectorNames));
                                }
                                SB.AppendLine("");
                                SB.AppendLine("#define GROUP_" + i + " " + (1 << i));
                            }

                            // grade
                            for (int i = 0; i <= S.m_dimension; i++)
                                SB.AppendLine("#define GRADE_" + i + " " + gradeBitmap[i]);
                        }


                        cgd.m_cog.EmitTemplate(SB, "basicInfo", "S=", S);

                        if (S.m_gmvCodeGeneration == GMV_CODE.RUNTIME)
                            cgd.m_cog.EmitTemplate(SB, "runtimeGpTablesHeader", "S=", S);
                    }

                    // #define for all specialized MV types
                    if (S.m_reportUsage)
                        G25.CG.C.SMV.WriteSMVtypeConstants(SB, S, cgd);

                    // write structs for all GMV (all float types)
                    G25.CG.C.GMV.WriteGMVstructs(SB, S, cgd);

                    // write structs for all SMVs (all float types)
                    G25.CG.C.SMV.WriteSMVstructs(SB, S, cgd);

                    // write structs for all GOM (all float types)
                    if (S.m_GOM != null)
                        G25.CG.C.GOM.WriteGOMstructs(SB, S, cgd);

                    // write structs for all SOMs (all float types)
                    G25.CG.C.SOM.WriteSOMstructs(SB, S, cgd);

                    { // write toString 
                        bool def = false;
                        G25.CG.C.ToString.WriteToString(SB, S, cgd, def);
                    }

                    // write constant declarations
                    G25.CG.C.Constants.WriteDeclarations(SB, S, cgd);

                    // set to zero / copy floats
                    cgd.m_cog.EmitTemplate(SB, "float_zero_copy_decl", "S=", S, "MAX_N=", G25.CG.Shared.Main.MAX_EXPLICIT_ZERO);

                    SB.AppendLine("// decl SB:");
                    SB.Append(cgd.m_declSB);

                    SB.AppendLine("// inline def SB:");
                    SB.Append(cgd.m_inlineDefSB);

                    // parser declarations:
                    if (S.m_parserType == PARSER.ANTLR)
                        cgd.m_cog.EmitTemplate(SB, "ANTLRparserHeader", "S=", S, "FT=", S.m_floatTypes[0]);
                    else if (S.m_parserType == PARSER.BUILTIN)
                        cgd.m_cog.EmitTemplate(SB, "CustomParserHeader", "S=", S, "FT=", S.m_floatTypes[0]);
                    

                    // close include guard
                    G25.CG.Shared.Util.WriteCloseIncludeGuard(SB, GetRawHeaderFilename(S));


                    // write all to file
                    G25.CG.Shared.Util.WriteFile(headerFilename, SB.ToString());


                    return generatedFiles;
                } // end of GenerateCode()


                /// <summary>
                /// Writes the main doxygen documentation for the whole algebra to the header file.
                /// </summary>
                /// <param name="SB"></param>
                /// <param name="S"></param>
                /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
                public static void WriteDoxyMainPage(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
                {
                    //cgd.m_cog.ClearOutput();
                    cgd.m_cog.EmitTemplate(SB, "doxyMainPage", 
                        "Namespace=", S.m_namespace, 
                        "Gaigen=", G25.Specification.FullGaigenName,
                        "License=", S.GetLicense());
                    //SB.Append(cgd.m_cog.GetOutputAndClear());
                }



            } // end of class Header
        } // end of namespace 'C'
    } // end of namespace CG
} // end of namespace G25

