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

namespace G25.CG.CSharp
{
    /// <summary>
    /// Handles code generation for general outermorphisms (classes, constructors, set functions, etc).
    /// </summary>
    class GOM
    {

        /// <summary>
        /// Generates a source file with the GOM class definition.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd"></param>
        /// <param name="FT"></param>
        /// <returns></returns>
        public static string GenerateCode(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT)
        {
            G25.GOM gom = S.m_GOM;
            string className = FT.GetMangledName(S, gom.Name);

            // get range vector type
            G25.SMV rangeVectorType = G25.CG.Shared.OMinit.GetRangeVectorType(S, FT, cgd, gom);
            string rangeVectorSMVname = FT.GetMangledName(S, rangeVectorType.Name);

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
            G25.CG.CSJ.GOM.WriteComment(SB, S, cgd, FT, gom);

            // open class
            G25.CG.Shared.Util.WriteOpenClass(SB, S, G25.CG.Shared.AccessModifier.AM_public, className, null, null);

            // write member variables
            G25.CG.CSJ.GOM.WriteMemberVariables(SB, S, cgd, FT, gom);

            // write constructors
            G25.CG.CSJ.GOM.WriteConstructors(SB, S, cgd, FT, gom, className, rangeVectorSMVname);

            // write set functions
            G25.CG.CSJ.GOM.WriteSetIdentity(SB, S, cgd, FT);
            G25.CG.CSJ.GOM.WriteSetCopy(SB, S, cgd, FT);
            G25.CG.CSJ.GOM.WriteSetVectorImages(S, cgd, FT, false, false); // false, false = matrixMode, transpose
            G25.CG.CSJ.GOM.WriteSetVectorImages(S, cgd, FT, true, false); // true, false = matrixMode, transpose
            G25.CG.CSJ.GOM.WriteSOMtoGOMcopy(S, cgd, FT);

            // write shortcuts for functions
            G25.CG.Shared.Shortcut.WriteFunctionShortcuts(SB, S, cgd, FT, gom);

            // close class
            G25.CG.Shared.Util.WriteCloseClass(SB, S, className);

            // close namespace
            G25.CG.Shared.Util.WriteCloseNamespace(SB, S);

            // write all to file
            G25.CG.Shared.Util.WriteFile(sourceFilename, SB.ToString());

            return sourceFilename;
        }

    } // end of class GOM
} // end of namespace G25.CG.CSharp

