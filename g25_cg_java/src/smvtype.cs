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
    /// Handles code generation for the type of specialized multivectors 
    /// </summary>
    public class SmvType
    {
        public static string GetRawSourceFilename(Specification S)
        {
            return MainGenerator.GetClassOutputPath(S, "SmvType");
        }

        public static List<string> GenerateCode(Specification S, G25.CG.Shared.CGdata cgd)
        {
            // get filename, list of generated filenames
            List<string> generatedFiles = new List<string>();
            string sourceFilename = S.GetOutputPath(GetRawSourceFilename(S));
            generatedFiles.Add(sourceFilename);

            // get StringBuilder where all generated code goes
            StringBuilder SB = new StringBuilder();

            // output license, copyright
            G25.CG.Shared.Util.WriteCopyright(SB, S);
            G25.CG.Shared.Util.WriteLicense(SB, S);

            // open namespace
            G25.CG.Shared.Util.WriteOpenNamespace(SB, S);

            SB.AppendLine("public enum " + G25.CG.CSJ.GMV.SMV_TYPE + " {");

            Dictionary<string, int> STD = G25.CG.Shared.SmvUtil.GetSpecializedTypeDictionary(S);

            STD.Add("none", -1);

            bool appendComma = false;
            foreach (KeyValuePair<string, int> kvp in STD)
            {
                string name = G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, kvp.Key);
                if (appendComma) SB.AppendLine(",");
                SB.Append("\t" + name + "(" + kvp.Value + ", \"" + kvp.Key + "\")");
                appendComma = true;
            }
            SB.AppendLine(";");

            cgd.m_cog.EmitTemplate(SB, "SmvTypeEnum");

            // close namespace
            G25.CG.Shared.Util.WriteCloseNamespace(SB, S);

            // write all to file
            G25.CG.Shared.Util.WriteFile(sourceFilename, SB.ToString());

            return generatedFiles;
        }


    } // end of class SmvType 
} // end of namespace G25.CG.Java

