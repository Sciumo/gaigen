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
        namespace CPP
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
                    string headerFilename = S.GetOutputPath(GetRawHeaderFilename(S));
                    generatedFiles.Add(headerFilename);

                    // get StringBuilder where all generated code goes
                    StringBuilder SB = new StringBuilder();

                    // output license, copyright
                    G25.CG.Shared.Util.WriteCopyright(SB, S);
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
                        SB.AppendLine("#include <string>");
                        if (cgd.GetFeedback(G25.CG.Shared.Main.NEED_TIME) == "true")
                            SB.AppendLine("#include <time.h> /* used to seed random generator */");
                        if (cgd.GetFeedback(G25.CG.Shared.Main.MERSENNE_TWISTER) == "true")
                            SB.AppendLine("#include \"" + S.m_namespace + "_mt.h\"");
                        if (S.m_reportUsage)
                        {
                            SB.AppendLine("#include <map>");
                        }
                    }

                    G25.CG.Shared.Util.WriteOpenNamespace(SB, S);

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
                        {
                            G25.CG.Shared.Util.WriteOpenNamespace(SB, S, G25.CG.Shared.Main.RUNTIME_NAMESPACE);
                            cgd.m_cog.EmitTemplate(SB, "runtimeGpTablesHeader", "S=", S);
                            G25.CG.Shared.Util.WriteCloseNamespace(SB, S, G25.CG.Shared.Main.RUNTIME_NAMESPACE);
                        }
                    }

                    // #define for all specialized MV types
                    if (S.m_reportUsage)
                        G25.CG.CPP.SMV.WriteSMVtypeConstants(SB, S, cgd);

                    { // write all class types
                        // write types for all GMV (all float types)
                        G25.CG.CPP.GMV.WriteGMVtypes(SB, S, cgd);

                        // write types for all SMVs (all float types)
                        G25.CG.CPP.SMV.WriteSMVtypes(SB, S, cgd);

                        // write types for all GOM (all float types)
                        if (S.m_GOM != null)
                            G25.CG.CPP.GOM.WriteGOMtypes(SB, S, cgd);

                        // write types for all SOMs (all float types)
                        G25.CG.CPP.SOM.WriteSOMtypes(SB, S, cgd);
                    }

                    { // write toString 
                        bool def = false;
                        G25.CG.CPP.ToString.WriteToString(SB, S, cgd, def);
                    }

                    // write classes for all GMV (all float types)
                    G25.CG.CPP.GMV.WriteGMVclasses(SB, S, cgd);

                    // write classes for all SMVs (all float types)
                    G25.CG.CPP.SMV.WriteSMVclasses(SB, S, cgd);


                    // write classes for all GOM (all float types)
                    if (S.m_GOM != null)
                        G25.CG.CPP.GOM.WriteGOMclasses(SB, S, cgd);

                    // write classes for all SOMs (all float types)
                    G25.CG.CPP.SOM.WriteSOMclasses(SB, S, cgd);

                    // write constant declarations
                    G25.CG.CPP.Constants.WriteDeclarations(SB, S, cgd);

                    // write report usage
                    cgd.m_cog.EmitTemplate(SB, (S.m_reportUsage) ? "ReportUsageHeader" : "NoReportUsageHeader");

                    // set to zero / copy floats
                    bool hasDouble = false;
                    foreach (FloatType FT in S.m_floatTypes)
                    {
                        if (FT.type == "double") hasDouble = true;
                        cgd.m_cog.EmitTemplate(SB, "float_zero_copy_decl", "S=", S, "FT=", FT, "MAX_N=", G25.CG.Shared.Main.MAX_EXPLICIT_ZERO);
                    }
                    if (S.m_parserType == PARSER.ANTLR && (!hasDouble))
                        cgd.m_cog.EmitTemplate(SB, "float_zero_copy_decl", "S=", S, "FT=", new G25.FloatType("double", "", ""), "MAX_N=", G25.CG.Shared.Main.MAX_EXPLICIT_ZERO);

                    SB.AppendLine("// decl SB:");
                    SB.Append(cgd.m_declSB);

                    // write operators
                    if (S.m_inlineOperators)
                        Operators.WriteOperatorDefinitions(SB, S, cgd);
                    else Operators.WriteOperatorDeclarations(SB, S, cgd);

                    // set to zero / copy floats
                    foreach (FloatType FT in S.m_floatTypes)
                    {
                        cgd.m_cog.EmitTemplate(SB, "float_zero_copy_def", "S=", S, "FT=", FT, "MAX_N=", G25.CG.Shared.Main.MAX_EXPLICIT_ZERO);
                    }
                    if (S.m_parserType == PARSER.ANTLR && (!hasDouble))
                        cgd.m_cog.EmitTemplate(SB, "float_zero_copy_def", "S=", S, "FT=", new G25.FloatType("double", "", ""), "MAX_N=", G25.CG.Shared.Main.MAX_EXPLICIT_ZERO);

                    SB.AppendLine("// inline def SB:");
                    SB.Append(cgd.m_inlineDefSB);

                    // parser declarations:
                    if (S.m_parserType == PARSER.ANTLR)
                        cgd.m_cog.EmitTemplate(SB, "ANTLRparserHeader", "S=", S, "FT=", S.m_floatTypes[0]);
                    else if (S.m_parserType == PARSER.BUILTIN)
                        cgd.m_cog.EmitTemplate(SB, "CustomParserHeader", "S=", S, "FT=", S.m_floatTypes[0]);

                    // close namespace
                    G25.CG.Shared.Util.WriteCloseNamespace(SB, S);

                    // All this thank to Jim Lazy^H^H^H^HIdle who's too lazy to write a true C++ target for ANTLR. Thanks Jim.
                    if (S.m_parserType == PARSER.ANTLR)
                    {
                        FloatType FT = Parser.GetANTLRfloatType(S);
                        cgd.m_cog.EmitTemplate(SB, "ANTLRparserHeader_outsideNamespace", "S=", S, "FT=", FT);
                    }

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
                }

            } // end of class Header
        } // end of namespace CPP
    } // end of namespace CG
} // end of namespace G25

