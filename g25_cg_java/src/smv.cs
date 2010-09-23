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

namespace G25.CG.Java
{
    /// <summary>
    /// Handles code generation for specialized multivectors (classes, constructors, set functions, etc).
    /// </summary>
    public class SMV
    {

        /// <summary>
        /// Generates a source file with the SMV class definition.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd"></param>
        /// <param name="smv"></param>
        /// <param name="FT"></param>
        /// <returns></returns>
        public static string GenerateCode(Specification S, G25.CG.Shared.CGdata cgd, G25.SMV smv, FloatType FT)
        {
            string className = FT.GetMangledName(S, smv.Name);

            // get filename, list of generated filenames
            List<string> generatedFiles = new List<string>();
            string sourceFilename = MainGenerator.GetClassOutputPath(S, className);
            generatedFiles.Add(sourceFilename);

            // get StringBuilder where all generated code goes
            StringBuilder SB = new StringBuilder();

            // get a new 'cgd' where all ouput goes to the one StringBuilder SB 
            cgd = new G25.CG.Shared.CGdata(cgd, SB, SB, SB);

            // output license, copyright
            G25.CG.Shared.Util.WriteCopyright(SB, S);
            G25.CG.Shared.Util.WriteLicense(SB, S);

            // open namespace
            G25.CG.Shared.Util.WriteOpenNamespace(SB, S);

            // write class comment
            G25.CG.CSJ.SMV.WriteComment(SB, S, cgd, FT, smv);

            // open class
            string[] implements = new string[] { MvInterface.GetMvInterfaceName(S, FT) };
            G25.CG.Shared.Util.WriteOpenClass(SB, S, G25.CG.Shared.AccessModifier.AM_public, className, null, implements);

            // member variables
            G25.CG.CSJ.SMV.WriteMemberVariables(SB, S, cgd, FT, smv);

            // indices into arrays
            G25.CG.CSJ.SMV.WriteSMVcoordIndices(SB, S, FT, smv);

            // special type to safeguard coordinates order in functions
            G25.CG.CSJ.SMV.WriteCoordinateOrder(SB, S, FT, smv);

            // get coordinates
            //G25.CG.CSJ.SMV.WriteGetCoordinates(SB, S, cgd, FT, smv);

            // write multivector interface implementation
            G25.CG.CSJ.SMV.WriteMultivectorInterface(SB, S, cgd, FT, smv);

            // write constructors
            G25.CG.CSJ.SMV.WriteConstructors(SB, S, cgd, FT, smv);

            // write set functions
            G25.CG.CSJ.SMV.WriteSetFunctions(SB, S, cgd, FT, smv);

            // write largest coordinate functions
            G25.CG.CSJ.SMV.WriteLargestCoordinateFunctions(S, cgd, FT, smv);

            // write 'toString' functions
            G25.CG.CSJ.GMV.WriteToString(SB, S, cgd, FT, smv);

            // write get/set coords
            G25.CG.CSJ.SMV.WriteGetSetCoord(SB, S, cgd, FT, smv);

            // write shortcuts for functions
            G25.CG.Shared.Shortcut.WriteFunctionShortcuts(SB, S, cgd, FT, smv);

            // close class
            G25.CG.Shared.Util.WriteCloseClass(SB, S, className);

            // close namespace
            G25.CG.Shared.Util.WriteCloseNamespace(SB, S);

            // write all to file
            G25.CG.Shared.Util.WriteFile(sourceFilename, SB.ToString());

            return sourceFilename;
        }

    } // end of class SMV 
} // end of namespace G25.CG.Java

