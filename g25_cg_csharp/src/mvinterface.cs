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
    /// Handles code generation for the type of specialized multivectors 
    /// </summary>
    public class MvInterface
    {
        public static string GetMvInterfaceName(Specification S, FloatType FT)
        {
            return FT.GetMangledName(S, S.m_GMV.Name) + "_if";
        }

        public static string GetRawSourceFilename(Specification S, FloatType FT)
        {
            return MainGenerator.GetClassOutputPath(S, GetMvInterfaceName(S, FT));
        }

        public static List<string> GenerateCode(Specification S, G25.CG.Shared.CGdata cgd)
        {
            // get filename, list of generated filenames
            List<string> generatedFiles = new List<string>();

            foreach (FloatType FT in S.m_floatTypes)
            {
                string sourceFilename = S.GetOutputPath(GetRawSourceFilename(S, FT));
                generatedFiles.Add(sourceFilename);

                // get StringBuilder where all generated code goes
                StringBuilder SB = new StringBuilder();

                // output license, copyright
                G25.CG.Shared.Util.WriteCopyright(SB, S);
                G25.CG.Shared.Util.WriteLicense(SB, S);

                // open namespace
                G25.CG.Shared.Util.WriteOpenNamespace(SB, S);

                SB.AppendLine("public interface " + GetMvInterfaceName(S, FT) + " {");

                string gmvName = FT.GetMangledName(S, S.m_GMV.Name);
                cgd.m_cog.EmitTemplate(SB, "mvInterface", "gmvName=", gmvName);

                SB.AppendLine("}");

                // close namespace
                G25.CG.Shared.Util.WriteCloseNamespace(SB, S);

                // write all to file
                G25.CG.Shared.Util.WriteFile(sourceFilename, SB.ToString());
            }

            return generatedFiles;
        }


    } // end of class GroupBitmap 
} // end of namespace G25.CG.Java

