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

namespace G25.CG.CPP
{
    public class MainGenerator : G25.CG.Shared.Main, G25.CodeGenerator
    {
        /// <returns>what language this code generator generates for.</returns>
        public String Language()
        {
            return G25.XML.XML_CPP;
        }

        /// <summary>
        /// Should generate the code according to the specification of the algebra.
        /// </summary>
        /// <param name="S">The specification of the algebra. The specification also lists the names of the files
        /// to be generated, or at least the base path.</param>
        /// <param name="plugins">The plugins which Gaigen found that support the same language as this code generator.</param>
        /// <returns>a list of filenames; the names of the files that were generated. This may be used
        /// for post processing.</returns>
        public List<string> GenerateCode(Specification S, List<CodeGeneratorPlugin> plugins)
        {
            CoGsharp.CoG cog = InitCog(S);

            // for C++: add converters from any MV -> SMV (like _vector(mv))
            Converter.AddDefaultGmvConverters(S);
            // for C++: add converters from any SMV -> SMV (like _vector(vector))
            Converter.AddDefaultSmvConverters(S);


            CG.Shared.CGdata cgd = new G25.CG.Shared.CGdata(plugins, cog);
            cgd.SetDependencyPrefix("missing_function_"); // this makes sure that the user sees the function call is a missing dependency
            G25.CG.Shared.FunctionGeneratorInfo FGI = (S.m_generateTestSuite) ? new G25.CG.Shared.FunctionGeneratorInfo() : null; // the fields in this variable are set by Functions.WriteFunctions() and reused by TestSuite.GenerateCode()

            { // pregenerated code that will go into header, source
                // generate code for parts of the geometric product, dual, etc (works in parallel internally)
                 try
                 {
                     bool declOnly = false;
                     G25.CG.Shared.PartsCode.GeneratePartsCode(S, cgd, declOnly);
                 }
                 catch (G25.UserException E) { cgd.AddError(E); }

                // write set zero, set, copy, copy between float types, extract coordinate, largest coordinate, etc (works in parallel internally)
                try
                {
                    GenerateSetFunctions(S, plugins, cgd);
                }
                catch (G25.UserException E) { cgd.AddError(E); }

                // write function (works in parallel internally)
                G25.CG.Shared.Functions.WriteFunctions(S, cgd, FGI, Functions.GetFunctionGeneratorPlugins(cgd));
            }

            List<string> generatedFiles = new List<string>();

            // generate Doxyfile
            generatedFiles.Add(G25.CG.Shared.Util.GenerateDoxyfile(S, cgd));
            // generate header
            generatedFiles.AddRange(Header.GenerateCode(S, cgd));
            // generate source
            generatedFiles.AddRange(Source.GenerateCode(S, cgd));
            // generate parser
            generatedFiles.AddRange(Parser.GenerateCode(S, cgd));
            

            // report errors and missing deps to user
            cgd.PrintErrors(S);
            cgd.PrintMissingDependencies(S);
            if ((cgd.GetNbErrors() == 0) && (cgd.GetNbMissingDependencies() == 0) && S.m_generateTestSuite)
            {
                // if no errors, then generate testing code
                TestSuite.GenerateCode(S, cgd, FGI);
            }

            // Generate random number generator source code (Mersenne Twister).
            // This must be done last since the testing code may require it!
            if (cgd.GetFeedback(G25.CG.Shared.Main.MERSENNE_TWISTER) == "true")
            {
                generatedFiles.AddRange(RandomMT.GenerateCode(S, cgd)); // todo
            }

            return generatedFiles;
        } // end of GenerateCode()


        /// <summary>
        /// Writes code for all 'set' function and various other function (largest coordinate, coordinate extraction, etc)
        /// </summary>
        /// <param name="S"></param>
        /// <param name="plugins"></param>
        /// <param name="cgd"></param>
        protected void GenerateSetFunctions(Specification S, List<CodeGeneratorPlugin> plugins, CG.Shared.CGdata cgd)
        {
            const int NB_SET_CODE = 4;

            // get a temporary cgd for each type of parts code
            CG.Shared.CGdata[] tmpCgd = new G25.CG.Shared.CGdata[] 
                {new G25.CG.Shared.CGdata(cgd), 
                    new G25.CG.Shared.CGdata(cgd), 
                    new G25.CG.Shared.CGdata(cgd),
                    new G25.CG.Shared.CGdata(cgd)
                };

            // get parts code generators
            G25.CG.CPP.GMV p1 = new G25.CG.CPP.GMV(S, tmpCgd[0]); // [0] = GMV
            G25.CG.CPP.SMV p2 = new G25.CG.CPP.SMV(S, tmpCgd[1]); // [1] = SMV
            G25.CG.CPP.GOM p3 = new G25.CG.CPP.GOM(S, tmpCgd[2]); // [2] = GOM
            G25.CG.CPP.SOM p4 = new G25.CG.CPP.SOM(S, tmpCgd[3]); // [3] = SOM

            // run threads
            System.Threading.Thread[] T = new System.Threading.Thread[NB_SET_CODE];
            T[0] = new Thread(p1.WriteSetFunctions);
            T[1] = new Thread(p2.WriteSetFunctions);
            T[2] = new Thread(p3.WriteSetFunctions);
            T[3] = new Thread(p4.WriteSetFunctions);
            G25.CG.Shared.Threads.StartThreadArray(T);
            G25.CG.Shared.Threads.JoinThreadArray(T);

            // merge declarations and definitions
            for (int i = 0; i < NB_SET_CODE; i++)
            {
                cgd.m_declSB.Append(tmpCgd[i].m_declSB);
                cgd.m_defSB.Append(tmpCgd[i].m_defSB);
                cgd.m_inlineDefSB.Append(tmpCgd[i].m_inlineDefSB);
            }
        }

        /// <summary>
        /// Loads all templates for the 'CPP' language into 'cog'. Also loads
        /// shared templates by calling G25.CG.Shared.Util.LoadTemplates(cog);
        /// </summary>
        /// <param name="cog">Templates are loaded into this variable.</param>
        /// <param name="S">Specification. Used to know whether testing code will be generated.</param>
        public override void LoadTemplates(Specification S, CoGsharp.CoG cog)
        {
            // also load shared templates:
            G25.CG.Shared.Util.LoadTemplates(cog);
            G25.CG.Shared.Util.LoadCTemplates(cog);

            cog.LoadTemplates(g25_cg_cpp.Properties.Resources.cg_cpp_templates, "cg_cpp_templates.txt");
            if (S.m_generateTestSuite) // only load when testing code is required
                cog.LoadTemplates(g25_cg_cpp.Properties.Resources.cg_cpp_test_templates, "cg_cpp_test_templates.txt");
        }



    } // end of class MainGenerator
} // end of namespace G25.CG.CPP
