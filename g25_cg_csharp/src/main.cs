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

/*! \mainpage Gaigen 2.5 library (g25_cg_csharp) Documentation
 *
 * Gaigen 2.5 by Daniel Fontijne, University of Amsterdam.
 * 
 * Released under GPL license.
 */
namespace G25.CG.CSharp
{
    public class MainGenerator : G25.CG.Shared.Main, G25.CodeGenerator
    {
        /// <returns>what language this code generator generates for.</returns>
        public String Language()
        {
            return G25.XML.XML_CSHARP;
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

            CG.Shared.CGdata cgd = new G25.CG.Shared.CGdata(plugins, cog);
            cgd.SetDependencyPrefix("missing_function_"); // this makes sure that the user sees the function call is a missing dependency
            G25.CG.Shared.FunctionGeneratorInfo FGI = (S.m_generateTestSuite) ? new G25.CG.Shared.FunctionGeneratorInfo() : null; // the fields in this variable are set by Functions.WriteFunctions() and reused by TestSuite.GenerateCode()

            { // pregenerated code that will go into main source
                // generate code for parts of the geometric product, dual, etc (works in parallel internally)
                try
                {
                    bool declOnly = false;
                    G25.CG.Shared.PartsCode.GeneratePartsCode(S, cgd, declOnly);
                }
                catch (G25.UserException E) { cgd.AddError(E); }

                // write set zero, set, copy, copy between float types, extract coordinate, largest coordinate, etc (works in parallel internally)
                /*      try
                      {
                          GenerateSetFunctions(S, plugins, cgd);
                      }
                      catch (G25.UserException E) { cgd.AddError(E); }*/

                // write function (works in parallel internally)
                G25.CG.Shared.Functions.WriteFunctions(S, cgd, FGI, Functions.GetFunctionGeneratorPlugins(cgd));
            }



            return new List<string>();
        }

        /// <summary>
        /// Loads all templates for the 'C#' language into 'cog'. Also loads
        /// shared templates by calling G25.CG.Shared.Util.LoadTemplates(cog);
        /// </summary>
        /// <param name="cog">Templates are loaded into this variable.</param>
        /// <param name="S">Specification. Used to know whether testing code will be generated.</param>
        public override void LoadTemplates(Specification S, CoGsharp.CoG cog)
        {
            // also load shared templates:
            G25.CG.Shared.Util.LoadTemplates(cog);

            cog.LoadTemplates(g25_cg_csharp.Properties.Resources.cg_csharp_templates, "cg_csharp_templates.txt");
            if (S.m_generateTestSuite) // only load when testing code is required
                cog.LoadTemplates(g25_cg_csharp.Properties.Resources.cg_csharp_test_templates, "cg_csharp_test_templates.txt");
        }

    }
} // end of namespaceG25.CG.CSharp