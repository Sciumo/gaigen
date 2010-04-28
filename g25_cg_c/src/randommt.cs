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

namespace G25.CG.C
{
    /// <summary>
    /// Code generation for Mersenne Twister random number generator.
    /// </summary>
    class RandomMT
    {

        public static string GetRawMtHeaderFilename(Specification S)
        {
            return S.m_namespace + "_mt.h";
        }

        public static string GetRawMtSourceFilename(Specification S)
        {
            return S.m_namespace + "_mt.c";
        }

        /// <summary>
        /// Generates files for random number generation using MT.
        /// </summary>
        /// <param name="S">Specification of algebra.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <returns>a list of filenames which were generated (full path).</returns>
        public static List<string> GenerateCode(Specification S, G25.CG.Shared.CGdata cgd)
        {
            // get filename, list of generated filenames
            List<string> generatedFiles = new List<string>();

            string headerFilename = S.GetOutputPath(G25.CG.C.RandomMT.GetRawMtHeaderFilename(S));
            string sourceFilename = S.GetOutputPath(G25.CG.C.RandomMT.GetRawMtSourceFilename(S));
            generatedFiles.Add(headerFilename);

            { // header
                StringBuilder SB = new StringBuilder();
                cgd.m_cog.EmitTemplate(SB, "mersenneTwisterHeader", "S=", S);
                G25.CG.Shared.Util.WriteFile(headerFilename, SB.ToString());
            }

            { // source
                StringBuilder SB = new StringBuilder();
                cgd.m_cog.EmitTemplate(SB, "mersenneTwisterSource", "S=", S);
                G25.CG.Shared.Util.WriteFile(sourceFilename, SB.ToString());
            }

            return generatedFiles;
        } // end of GenerateCode()


    } // end of class RandomMT
} // end of namespace G25.CG.C

