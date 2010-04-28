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
    /// Main code generation class for the source file.
    /// </summary>
    class Source
    {

        public static string GetRawSourceFilename(Specification S)
        {
            return S.m_namespace + ".c";
        }

        public static void WriteCompressSource(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            G25.GMV gmv = S.m_GMV;
            
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                //cgd.m_cog.ClearOutput();
                cgd.m_cog.EmitTemplate(SB, "compress", "S=", S, "FT=", FT, "gmv=", gmv);
                //SB.Append(cgd.m_cog.GetOutputAndClear());
            }
        }


        public static void GenerateBasicInfo(Specification S, G25.CG.Shared.CGdata cgd, StringBuilder SB) 
        {
            // dimension of space
            SB.AppendLine("const int " + S.m_namespace + "_spaceDim = " + S.m_dimension + ";");

            // number of groups of space
            SB.AppendLine("const int " + S.m_namespace + "_nbGroups = " + S.m_GMV.NbGroups + ";");

            // Euclidean metric?
            SB.AppendLine("const int " + S.m_namespace + "_metricEuclidean = " +
                (S.GetMetric("default").m_metric.IsEuclidean() ? "1" : "0") + ";");

            // basis vector names
            SB.AppendLine("const char *" + S.m_namespace + "_basisVectorNames[" + S.m_dimension + "] = {");
            SB.Append("\t");
            for (int i = 0; i < S.m_dimension; i++)
            {
                if (i > 0) SB.Append(", ");
                SB.Append("\"" + S.m_basisVectorNames[i] + "\"");
            }
            SB.AppendLine("");
            SB.AppendLine("};");
        } // end of GenerateBasicInfo()

        public static void GenerateGradeArray(Specification S, G25.CG.Shared.CGdata cgd, StringBuilder SB) 
        {
            // constants for the grades in an array:
            SB.Append("const int " + S.m_namespace + "_grades[] = {");

            String gStr = "GRADE_";
            for (int i = 0; i <= S.m_dimension; i++)
            {
                if (i > 0) SB.Append(", ");
                SB.Append(gStr + i);
            }

            // append extra zeros to simplify some grade math (grades above S.m_dimension do not exist so they are zero)
            for (int i = 0; i <= S.m_dimension; i++)
            {
                SB.Append(", 0");
            }
            SB.AppendLine("};");
        }

        public static void GenerateGroupArray(Specification S, G25.CG.Shared.CGdata cgd, StringBuilder SB) 
        {// constants for the groups in an array:
            SB.Append("const int " + S.m_namespace + "_groups[] = {");
        
            String gStr = "GROUP_";
            for (int i = 0; i < S.m_GMV.NbGroups; i++)
            {
                if (i > 0) SB.Append(", ");
                SB.Append(gStr + i);
            }

            SB.AppendLine("};");
        }


        public static void GenerateGroupSizeArray(Specification S, G25.CG.Shared.CGdata cgd, StringBuilder SB) 
        { // group size
            G25.GMV gmv = S.m_GMV;
            SB.AppendLine("const int " + S.m_namespace + "_groupSize[" + gmv.NbGroups + "] = {");
            SB.Append("\t");
            for (int i = 0; i < gmv.NbGroups; i++)
            {
                if (i > 0) SB.Append(", ");
                SB.Append(gmv.Group(i).Length);
            }
            SB.AppendLine("");
            SB.AppendLine("};");
        }

        public static void GenerateMultivectorSizeArray(Specification S, G25.CG.Shared.CGdata cgd, StringBuilder SB) 
        {
            G25.GMV gmv = S.m_GMV;

            // size of multivector based on grade usage bitmap
            SB.AppendLine("const int " + S.m_namespace + "_mvSize[" + (1 << gmv.NbGroups) + "] = {");
            SB.Append("\t");
            for (int i = 0; i < (1 << gmv.NbGroups); i++)
            {
                int s = 0;
                for (int j = 0; j < gmv.NbGroups; j++)
                    if ((i & (1 << j)) != 0)
                        s += gmv.Group(j).Length;
                SB.Append(s);
                if (i != ((1 << gmv.NbGroups) - 1)) SB.Append(", ");
                if ((i % 20) == 19)
                {
                    SB.AppendLine("");
                    SB.Append("\t");
                }
            }

            SB.AppendLine("};");
        }

        public static void GenerateBasisElementsArray(Specification S, G25.CG.Shared.CGdata cgd, StringBuilder SB)
        {
            G25.GMV gmv = S.m_GMV;
            // basis vectors in basis elements
            SB.AppendLine("const int " + S.m_namespace + "_basisElements[" + (1 << S.m_dimension) + "][" + (S.m_dimension + 1) + "] = {");
            {
                bool comma = false;
                for (int i = 0; i < gmv.NbGroups; i++)
                {
                    for (int j = 0; j < gmv.Group(i).Length; j++)
                    {
                        if (comma) SB.Append(",\n");
                        RefGA.BasisBlade B = gmv.Group(i)[j];
                        SB.Append("\t{");
                        for (int k = 0; k < S.m_dimension; k++)
                            if ((B.bitmap & (1 << k)) != 0)
                                SB.Append(k + ", ");
                        SB.Append("-1}");
                        comma = true;
                    }
                }
            }
            SB.AppendLine("");
            SB.AppendLine("};");
        }

        public static void GenerateBasisElementArrays(Specification S, G25.CG.Shared.CGdata cgd, StringBuilder SB) 
        { 
            G25.GMV gmv = S.m_GMV;

            double[] s = new double[1 << S.m_dimension];
            int[] IndexByBitmap = new int[1 << S.m_dimension];
            int[] BitmapByIndex = new int[1 << S.m_dimension];
            int[] GradeByBitmap = new int[1 << S.m_dimension];
            int[] GroupByBitmap = new int[1 << S.m_dimension];
            SB.AppendLine("const double " + S.m_namespace + "_basisElementSignByIndex[" + (1 << S.m_dimension) + "] =");
            SB.Append("\t{");
            {
                bool comma = false;
                int idx = 0;
                for (int i = 0; i < gmv.NbGroups; i++)
                {
                    for (int j = 0; j < gmv.Group(i).Length; j++)
                    {
                        if (comma) SB.Append(", ");
                        RefGA.BasisBlade B = gmv.Group(i)[j];
                        s[gmv.Group(i)[j].bitmap] = B.scale;
                        IndexByBitmap[B.bitmap] = idx;
                        BitmapByIndex[idx] = (int)B.bitmap;
                        GradeByBitmap[B.bitmap] = B.Grade();
                        GroupByBitmap[B.bitmap] = i;
                        SB.Append(B.scale);
                        comma = true;
                        idx++;
                    }
                }
            }
            SB.AppendLine("};");

            SB.AppendLine("const double " + S.m_namespace + "_basisElementSignByBitmap[" + (1 << S.m_dimension) + "] =");
            SB.Append("\t{");
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(s[i]);
                }
            }
            SB.AppendLine("};");

            SB.AppendLine("const int " + S.m_namespace + "_basisElementIndexByBitmap[" + (1 << S.m_dimension) + "] =");
            SB.Append("\t{");
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(IndexByBitmap[i]);
                }
            }
            SB.AppendLine("};");

            SB.AppendLine("const int " + S.m_namespace + "_basisElementBitmapByIndex[" + (1 << S.m_dimension) + "] =");
            SB.Append("\t{");
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(BitmapByIndex[i]);
                }
            }
            SB.AppendLine("};");

            SB.AppendLine("const int " + S.m_namespace + "_basisElementGradeByBitmap[" + (1 << S.m_dimension) + "] =");
            SB.Append("\t{");
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(GradeByBitmap[i]);
                }
            }
            SB.AppendLine("};");

            SB.AppendLine("const int " + S.m_namespace + "_basisElementGroupByBitmap[" + (1 << S.m_dimension) + "] =");
            SB.Append("\t{");
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(GroupByBitmap[i]);
                }
            }
            SB.AppendLine("};");

        } // end of GenerateBasisElementArrays()


        public static void GenerateTables(Specification S, G25.CG.Shared.CGdata cgd, StringBuilder SB)
        {
            SB.AppendLine("");

            GenerateBasicInfo(S, cgd, SB);

            GenerateGradeArray(S, cgd, SB);

            GenerateGroupArray(S, cgd, SB);

            GenerateGroupSizeArray(S, cgd, SB);

            GenerateMultivectorSizeArray(S, cgd, SB);

            GenerateBasisElementsArray(S, cgd, SB);

            GenerateBasisElementArrays(S, cgd, SB);

        } // end of GenerateTables()

        public static List<string> GenerateCode(Specification S, G25.CG.Shared.CGdata cgd)
        {
            // get filename, list of generated filenames
            List<string> generatedFiles = new List<string>();
            string sourceFilename = S.GetOutputPath(G25.CG.C.Source.GetRawSourceFilename(S));
            generatedFiles.Add(sourceFilename);

            // get StringBuilder where all generated code goes
            StringBuilder SB = new StringBuilder();

            // output license, copyright
            G25.CG.Shared.Util.WriteCopyright(SB, S);
            G25.CG.Shared.Util.WriteLicense(SB, S);

            { // #includes
                SB.AppendLine("#include <stdio.h>");
                if (cgd.GetFeedback(G25.CG.Shared.Main.NEED_TIME) == "true")
                    SB.AppendLine("#include <time.h> /* used to seed random generator */");
                SB.AppendLine("#include \"" + S.GetOutputFilename(G25.CG.C.Header.GetRawHeaderFilename(S)) + "\"");
            }

            GenerateTables(S, cgd, SB);

            // the list of names of smv types
            G25.CG.C.SMV.WriteSMVtypenames(SB, S, cgd);

            // write constant definitions
            G25.CG.C.Constants.WriteDefinitions(SB, S, cgd);

            // set to zero / copy floats
            cgd.m_cog.EmitTemplate(SB, "float_zero_copy_def", "S=", S, "MAX_N=", G25.CG.Shared.Main.MAX_EXPLICIT_ZERO);

            if (S.m_gmvCodeGeneration == GMV_CODE.RUNTIME)
            {
                cgd.m_cog.EmitTemplate(SB, "runtimeGpTablesDefs", "S=", S);
                cgd.m_cog.EmitTemplate(SB, "bitmapGp", "S=", S);
                cgd.m_cog.EmitTemplate(SB, "runtimeGpTable", "S=", S);
                foreach (G25.FloatType FT in S.m_floatTypes)
                {
                    cgd.m_cog.EmitTemplate(SB, "runtimeComputeGp", "S=", S, "FT=", FT);
                }
                cgd.m_cog.EmitTemplate(SB, "runtimeGpInitTables", "S=", S);
                cgd.m_cog.EmitTemplate(SB, "runtimeGpFreeTables", "S=", S);
            }

            // write compress source code
            WriteCompressSource(SB, S, cgd);

            cgd.m_cog.EmitTemplate(SB, "swapPointer");

            { // write toString 
                bool def = true;
                G25.CG.C.ToString.WriteToString(SB, S, cgd, def);
            }

            SB.AppendLine("// def SB:");
            SB.Append(cgd.m_defSB);

            // write all to file
            G25.CG.Shared.Util.WriteFile(sourceFilename, SB.ToString());


            return generatedFiles;
        }

    } // end of class Source
} // end of namespace G25.CG.C

