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
    /// Main code generation class for the source file.
    /// </summary>
    class Source
    {
        public static string GetRawSourceFilename(Specification S)
        {
            return MainGenerator.GetClassOutputPath(S, S.m_namespace); 
        }

        public static void WriteSMVtypeConstants(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            SB.AppendLine("");

            SB.AppendLine("/// <summary>");
            SB.AppendLine("/// These constants define a unique number for each specialized multivector type.");
            SB.AppendLine("/// They are used to report usage of non-optimized functions.");
            SB.AppendLine("/// </summary>");

            Dictionary<string, int> STD = G25.CG.Shared.SmvUtil.GetSpecializedTypeDictionary(S);
            SB.AppendLine("public enum " + G25.CG.CSJ.GMV.SMV_TYPE + " {");
            SB.AppendLine("\t" + G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, "NONE") + " = -1,");

            foreach (KeyValuePair<string, int> kvp in STD)
            {
                string name = G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, kvp.Key);
                SB.AppendLine("\t" + name + " = " + kvp.Value + ",");
            }

            SB.AppendLine("\t" + G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, G25.CG.Shared.ReportUsage.INVALID));
            SB.AppendLine("}");

            SB.AppendLine("");

        }


        public static void WriteSMVtypenames(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            Dictionary<string, int> STD = G25.CG.Shared.SmvUtil.GetSpecializedTypeDictionary(S);

            SB.AppendLine("");
            SB.AppendLine("\tprotected internal string[] typenames = ");
            SB.AppendLine("\t\tnew string[] {");
            {
                bool first = true;
                foreach (KeyValuePair<string, int> kvp in STD)
                {
                    if (!first) SB.AppendLine(",");
                    SB.Append("\t\t\t\"" + kvp.Key + "\"");
                    first = false;
                }

                if (STD.Count == 0) SB.Append("\t\t\t\"There are no specialized types defined\"");
            }
            SB.AppendLine("");
            SB.AppendLine("\t\t};");
        }


        public static void GenerateBasicInfo(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            // dimension of space
            SB.AppendLine("\tpublic const int SpaceDim = " + S.m_dimension + ";");

            // number of groups of space
            SB.AppendLine("\tpublic const int NbGroups = " + S.m_GMV.NbGroups + ";");

            // Euclidean metric?
            SB.AppendLine("\tpublic const bool MetricEuclidean = " +
                (S.GetMetric("default").m_metric.IsEuclidean() ? "true" : "false") + ";");

            // basis vector names
            SB.AppendLine("\tpublic static readonly string[] BasisVectorNames = new string[] {");
            SB.Append("\t\t");
            for (int i = 0; i < S.m_dimension; i++)
            {
                if (i > 0) SB.Append(", ");
                SB.Append("\"" + S.m_basisVectorNames[i] + "\"");
            }
            SB.AppendLine("");
            SB.AppendLine("\t};");
        } // end of GenerateBasicInfo()

        public static void GenerateGroupGradeConstants(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {

            SB.AppendLine();
            SB.AppendLine("[FlagsAttribute]");
            SB.AppendLine("public enum " + G25.CG.CSJ.GMV.GROUP_BITMAP + " : int");
            SB.AppendLine("{");

            // group
            int[] gradeBitmap = new int[S.m_dimension + 1];
            for (int i = 0; i < S.m_GMV.NbGroups; i++)
            {
                gradeBitmap[S.m_GMV.Group(i)[0].Grade()] |= 1 << i;

                SB.Append("\tGROUP_" + i + "  = " + (1 << i) + ", //");

                for (int j = 0; j < S.m_GMV.Group(i).Length; j++)
                {
                    if (j > 0) SB.Append(", ");
                    SB.Append(S.m_GMV.Group(i)[j].ToString(S.m_basisVectorNames));
                }
                SB.AppendLine();
            }
            SB.AppendLine();

            // grade
            for (int i = 0; i <= S.m_dimension; i++)
            {
                SB.Append("\tGRADE_" + i + " = " + gradeBitmap[i]);
                if (i == S.m_dimension) SB.AppendLine();
                else SB.AppendLine(", ");
            }

            SB.AppendLine("}");
        }

        public static void GenerateGradeArray(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            // constants for the grades in an array:
            SB.Append("\tpublic static readonly " + G25.CG.CSJ.GMV.GROUP_BITMAP + "[] Grades = {");

            string gStr = G25.CG.CSJ.GMV.GROUP_BITMAP + ".GRADE_";
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

        public static void GenerateGroupArray(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {// constants for the groups in an array:
            SB.Append("\tpublic static readonly " + G25.CG.CSJ.GMV.GROUP_BITMAP + "[] Groups = {");

            string gStr = G25.CG.CSJ.GMV.GROUP_BITMAP + ".GROUP_";
            for (int i = 0; i < S.m_GMV.NbGroups; i++)
            {
                if (i > 0) SB.Append(", ");
                SB.Append(gStr + i);
            }
            SB.AppendLine("};");
        }


        public static void GenerateGroupSizeArray(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        { // group size
            G25.GMV gmv = S.m_GMV;
            SB.Append("\tpublic static readonly int[] GroupSize = { ");
            for (int i = 0; i < gmv.NbGroups; i++)
            {
                if (i > 0) SB.Append(", ");
                SB.Append(gmv.Group(i).Length);
            }
            SB.AppendLine(" };");
        }

        public static void GenerateMultivectorSizeArray(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            G25.GMV gmv = S.m_GMV;

            // size of multivector based on grade usage bitmap
            SB.AppendLine("\tpublic static readonly int[] MvSize = new int[] {");
            SB.Append("\t\t");
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
                    SB.Append("\t\t");
                }
            }

            SB.AppendLine("\t};");
        }

        public static void GenerateBasisElementsArray(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            G25.GMV gmv = S.m_GMV;
            // basis vectors in basis elements
            SB.AppendLine("\tpublic static readonly int[][] BasisElements = new int[][] {");
            {
                bool comma = false;
                for (int i = 0; i < gmv.NbGroups; i++)
                {
                    for (int j = 0; j < gmv.Group(i).Length; j++)
                    {
                        if (comma) SB.Append(",\n");
                        RefGA.BasisBlade B = gmv.Group(i)[j];
                        SB.Append("\t\tnew int[] {");
                        for (int k = 0; k < S.m_dimension; k++)
                            if ((B.bitmap & (1 << k)) != 0)
                                SB.Append(k + ", ");
                        SB.Append("-1}");
                        comma = true;
                    }
                }
            }
            SB.AppendLine("");
            SB.AppendLine("\t};");
        }

        public static void GenerateBasisElementArrays(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            G25.GMV gmv = S.m_GMV;

            double[] s = new double[1 << S.m_dimension];
            int[] IndexByBitmap = new int[1 << S.m_dimension];
            int[] BitmapByIndex = new int[1 << S.m_dimension];
            int[] GradeByBitmap = new int[1 << S.m_dimension];
            int[] GroupByBitmap = new int[1 << S.m_dimension];
            SB.AppendLine("\tpublic static readonly double[] BasisElementSignByIndex = new double[]");
            SB.Append("\t\t{");
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

            SB.AppendLine("\tpublic static readonly double[] BasisElementSignByBitmap = new double[]");
            SB.Append("\t\t{");
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(s[i]);
                }
            }
            SB.AppendLine("};");

            SB.AppendLine("\tpublic static readonly int[] BasisElementIndexByBitmap = new int[]");
            SB.Append("\t\t{");
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(IndexByBitmap[i]);
                }
            }
            SB.AppendLine("};");

            SB.AppendLine("\tpublic static readonly int[] BasisElementBitmapByIndex = new int[]");
            SB.Append("\t\t{");
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(BitmapByIndex[i]);
                }
            }
            SB.AppendLine("};");

            SB.AppendLine("\tpublic static readonly int[] BasisElementGradeByBitmap = new int[]");
            SB.Append("\t\t{");
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(GradeByBitmap[i]);
                }
            }
            SB.AppendLine("};");

            SB.AppendLine("\tpublic static readonly int[] BasisElementGroupByBitmap = new int[]");
            SB.Append("\t\t{");
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(GroupByBitmap[i]);
                }
            }
            SB.AppendLine("};");

        } // end of GenerateBasisElementArrays()


        public static void GenerateTables(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            SB.AppendLine("");

            GenerateBasicInfo(SB, S, cgd);

            GenerateGradeArray(SB, S, cgd);

            GenerateGroupArray(SB, S, cgd);

            GenerateGroupSizeArray(SB, S, cgd);

            GenerateMultivectorSizeArray(SB, S, cgd);

            GenerateBasisElementsArray(SB, S, cgd);

            GenerateBasisElementArrays(SB, S, cgd);

        } // end of GenerateTables()


        private static void WriteSetZeroCopyFloats(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            // set to zero / copy floats
            foreach (FloatType FT in S.m_floatTypes)
            {
                cgd.m_cog.EmitTemplate(SB, "float_zero_copy_def", "S=", S, "FT=", FT, "MAX_N=", G25.CG.Shared.Main.MAX_EXPLICIT_ZERO);
            }
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

            SB.AppendLine("using System;");
#if RIEN
            { // todo: using .... 
                SB.AppendLine("#include <stdio.h>");
                SB.AppendLine("#include <utility> // for std::swap");
                if (cgd.GetFeedback(G25.CG.Shared.Main.NEED_TIME) == "true")
                    SB.AppendLine("#include <time.h> /* used to seed random generator */");
                SB.AppendLine("#include \"" + S.GetOutputFilename(G25.CG.CSharp.Header.GetRawHeaderFilename(S)) + "\"");
            }
#endif 

            G25.CG.Shared.Util.WriteOpenNamespace(SB, S);

            GenerateGroupGradeConstants(SB, S, cgd);
            WriteSMVtypeConstants(SB, S, cgd);

            G25.CG.Shared.Util.WriteOpenClass(SB, S, G25.CG.Shared.AccessModifier.AM_public, S.m_namespace, null, null);

            WriteSMVtypenames(SB, S, cgd);

            GenerateTables(SB, S, cgd);

            WriteSetZeroCopyFloats(SB, S, cgd);
            

#if RIEN
            // the list of names of smv types
            G25.CG.CSharp.SMV.WriteSMVtypenames(SB, S, cgd);

            // write constant definitions
            G25.CG.CSharp.Constants.WriteDefinitions(SB, S, cgd);

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

            { // write toString 
                bool def = true;
                G25.CG.CSharp.ToString.WriteToString(SB, S, cgd, def);
            }

            // write operators
            if (!S.m_inlineOperators)
                Operators.WriteOperatorDefinitions(SB, S, cgd);

#endif

            //            SB.AppendLine("// def SB:");
  //          SB.Append(cgd.m_defSB);

            // close class
            G25.CG.Shared.Util.WriteCloseClass(SB, S, S.m_namespace);

            // close namespace
            G25.CG.Shared.Util.WriteCloseNamespace(SB, S);                    

            // write all to file
            G25.CG.Shared.Util.WriteFile(sourceFilename, SB.ToString());


            return generatedFiles;
        }

    } // end of class Source
} // end of namespace G25.CG.CSharp

