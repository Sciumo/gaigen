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
    /// Main code generation class for the source file.
    /// </summary>
    class Source
    {
        public static string GetRawSourceFilename(Specification S)
        {
            return MainGenerator.GetClassOutputPath(S, S.m_namespace);
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

            G25.CG.Shared.Util.WriteOpenNamespace(SB, S);

            // open class
            G25.CG.Shared.Util.WriteOpenClass(SB, S, G25.CG.Shared.AccessModifier.AM_public, S.m_namespace, null, null);

            G25.CG.CSJ.Source.WriteSMVtypenames(SB, S, cgd);

            G25.CG.CSJ.Constants.WriteDefinitions(SB, S, cgd);

            G25.CG.CSJ.Source.GenerateTables(SB, S, cgd);

            G25.CG.CSJ.Source.WriteRandomGenerator(SB, S, cgd);

            G25.CG.CSJ.Source.WriteSetZeroCopyFloats(SB, S, cgd);

            G25.CG.CSJ.Source.WriteToString(SB, S, cgd);

            G25.CG.CSJ.Source.WriteParser(SB, S, cgd);

            if (S.m_gmvCodeGeneration == GMV_CODE.RUNTIME)
                G25.CG.CSJ.Source.WriteRuntimeGp(SB, S, cgd);

#if RIEN
            GenerateTables(S, cgd, SB);

            // the list of names of smv types
            G25.CG.CPP.SMV.WriteSMVtypenames(SB, S, cgd);

            // write constant definitions
            G25.CG.CPP.Constants.WriteDefinitions(SB, S, cgd);

            // write report usage
            cgd.m_cog.EmitTemplate(SB, (S.m_reportUsage) ? "ReportUsageSource" : "NoReportUsageSource");

            if (S.m_gmvCodeGeneration == GMV_CODE.RUNTIME)
            {
                G25.CG.Shared.Util.WriteOpenNamespace(SB, S, G25.CG.Shared.Main.RUNTIME_NAMESPACE);

                cgd.m_cog.EmitTemplate(SB, "runtimeGpTablesDefs", "S=", S);
                cgd.m_cog.EmitTemplate(SB, "bitmapGp", "S=", S);
                cgd.m_cog.EmitTemplate(SB, "runtimeGpTable", "S=", S);
                foreach (G25.FloatType FT in S.m_floatTypes)
                {
                    cgd.m_cog.EmitTemplate(SB, "runtimeComputeGp", "S=", S, "FT=", FT);
                }
                cgd.m_cog.EmitTemplate(SB, "runtimeGpInitTables", "S=", S);
                cgd.m_cog.EmitTemplate(SB, "runtimeGpFreeTables", "S=", S);
                G25.CG.Shared.Util.WriteCloseNamespace(SB, S, G25.CG.Shared.Main.RUNTIME_NAMESPACE);
            }


            // write operators
            if (!S.m_inlineOperators)
                Operators.WriteOperatorDefinitions(SB, S, cgd);
#endif
            SB.Append(cgd.m_defSB);

            // close class
            G25.CG.Shared.Util.WriteCloseClass(SB, S, S.m_namespace);

            // close namespace
            G25.CG.Shared.Util.WriteCloseNamespace(SB, S);                    

            // write all to file
            G25.CG.Shared.Util.WriteFile(sourceFilename, SB.ToString());


            return generatedFiles;
        }

    } // end of class Source

} // end of namespace G25.CG.Java

