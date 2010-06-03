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

namespace G25.CG.CSJ
{
    public class Source
    {
        public static void WriteSMVtypenames(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            Dictionary<string, int> STD = G25.CG.Shared.SmvUtil.GetSpecializedTypeDictionary(S);

            string accessModifier = Keywords.ProtectedStaticAccessModifier(S);
            string stringType = Keywords.StringType(S);

            SB.AppendLine("");
            SB.AppendLine("\t" + accessModifier + " " + stringType + "[] typenames = ");
            SB.AppendLine("\t\tnew " + stringType + "[] {");
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
            string accessModifier = Keywords.ConstAccessModifier(S);
            string accessModifierArr = Keywords.ConstArrayAccessModifier(S);
            string stringType = Keywords.StringType(S);
            string boolType = G25.CG.Shared.CodeUtil.GetBoolType(S);

            // dimension of space
            SB.AppendLine("\tpublic " + accessModifier + " int SpaceDim = " + S.m_dimension + ";");

            // number of groups of space
            SB.AppendLine("\tpublic " + accessModifier + " int NbGroups = " + S.m_GMV.NbGroups + ";");

            // Euclidean metric?
            SB.AppendLine("\tpublic " + accessModifier + " " + boolType + " MetricEuclidean = " +
                (S.GetMetric("default").m_metric.IsEuclidean() ? "true" : "false") + ";");

            // basis vector names
            SB.AppendLine("\tpublic " + accessModifierArr + " " + stringType + "[] BasisVectorNames = new " + stringType + "[] {");
            SB.Append("\t\t");
            for (int i = 0; i < S.m_dimension; i++)
            {
                if (i > 0) SB.Append(", ");
                SB.Append("\"" + S.m_basisVectorNames[i] + "\"");
            }
            SB.AppendLine("");
            SB.AppendLine("\t};");
        } // end of GenerateBasicInfo()

        public static void GenerateGradeArray(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            string accessModifierArr = Keywords.ConstArrayAccessModifier(S);
            string groupBitmapType = Keywords.GroupBitmapType(S);

            // constants for the grades in an array:
            SB.Append("\tpublic " + accessModifierArr + " " + groupBitmapType + "[] Grades = {");

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
            string accessModifierArr = Keywords.ConstArrayAccessModifier(S);
            string groupBitmapType = Keywords.GroupBitmapType(S);

            SB.Append("\tpublic " + accessModifierArr + " " + groupBitmapType + "[] Groups = {");

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
            string accessModifierArr = Keywords.ConstArrayAccessModifier(S);

            G25.GMV gmv = S.m_GMV;
            SB.Append("\tpublic " + accessModifierArr + " int[] GroupSize = { ");
            for (int i = 0; i < gmv.NbGroups; i++)
            {
                if (i > 0) SB.Append(", ");
                SB.Append(gmv.Group(i).Length);
            }
            SB.AppendLine(" };");
        }

        public static void GenerateMultivectorSizeArray(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            string accessModifierArr = Keywords.ConstArrayAccessModifier(S);

            G25.GMV gmv = S.m_GMV;

            // size of multivector based on grade usage bitmap
            SB.AppendLine("\tpublic " + accessModifierArr + " int[] MvSize = new int[] {");
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
            string accessModifierArr = Keywords.ConstArrayAccessModifier(S);

            G25.GMV gmv = S.m_GMV;
            // basis vectors in basis elements
            SB.AppendLine("\tpublic " + accessModifierArr + " int[][] BasisElements = new int[][] {");
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
            string accessModifierArr = Keywords.ConstArrayAccessModifier(S);
            G25.GMV gmv = S.m_GMV;

            double[] s = new double[1 << S.m_dimension];
            int[] IndexByBitmap = new int[1 << S.m_dimension];
            int[] BitmapByIndex = new int[1 << S.m_dimension];
            int[] GradeByBitmap = new int[1 << S.m_dimension];
            int[] GroupByBitmap = new int[1 << S.m_dimension];
            SB.AppendLine("\tpublic " + accessModifierArr + " double[] BasisElementSignByIndex = new double[]");
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

            SB.AppendLine("\tpublic " + accessModifierArr + " double[] BasisElementSignByBitmap = new double[]");
            SB.Append("\t\t{");
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(s[i]);
                }
            }
            SB.AppendLine("};");

            SB.AppendLine("\tpublic " + accessModifierArr + " int[] BasisElementIndexByBitmap = new int[]");
            SB.Append("\t\t{");
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(IndexByBitmap[i]);
                }
            }
            SB.AppendLine("};");

            SB.AppendLine("\tpublic " + accessModifierArr + " int[] BasisElementBitmapByIndex = new int[]");
            SB.Append("\t\t{");
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(BitmapByIndex[i]);
                }
            }
            SB.AppendLine("};");

            SB.AppendLine("\tpublic " + accessModifierArr + " int[] BasisElementGradeByBitmap = new int[]");
            SB.Append("\t\t{");
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(GradeByBitmap[i]);
                }
            }
            SB.AppendLine("};");

            SB.AppendLine("\tpublic " + accessModifierArr + " int[] BasisElementGroupByBitmap = new int[]");
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

        public static void WriteRandomGenerator(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            cgd.m_cog.EmitTemplate(SB, "randomNumberGenerator");
        }

        public static void WriteSetZeroCopyFloats(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            // set to zero / copy floats
            foreach (FloatType FT in S.m_floatTypes)
            {
                cgd.m_cog.EmitTemplate(SB, "float_zero_copy_def", "S=", S, "FT=", FT, "MAX_N=", G25.CG.Shared.Main.MAX_EXPLICIT_ZERO);
            }
        }

        public static void WriteToString(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            cgd.m_cog.EmitTemplate(SB, "sourceToString", "S=", S, "FT=", S.m_floatTypes[0], "gmv=", S.m_GMV, "gmvName=", S.m_floatTypes[0].GetMangledName(S, S.m_GMV.Name));
        }

        public static void WriteParser(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            cgd.m_cog.EmitTemplate(SB, "parserShortcut", "S=", S, "gmvName=", S.m_floatTypes[0].GetMangledName(S, S.m_GMV.Name));
        }


        /// <summary>
        /// Writes the code for the runtime computation of the geometric product.
        /// </summary>
        public static void WriteRuntimeGp(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            cgd.m_cog.EmitTemplate(SB, "runtimeGpTablesDefs", "S=", S);
            cgd.m_cog.EmitTemplate(SB, "bitmapGp", "S=", S);
            cgd.m_cog.EmitTemplate(SB, "runtimeGpTable", "S=", S);
            /*foreach (G25.FloatType FT in S.m_floatTypes)
            {
                cgd.m_cog.EmitTemplate(SB, "runtimeComputeGp", "S=", S, "FT=", FT);
            }
            cgd.m_cog.EmitTemplate(SB, "runtimeGpInitTables", "S=", S);
            cgd.m_cog.EmitTemplate(SB, "runtimeGpFreeTables", "S=", S);*/
        }


    } // end of class Source
} // end of namespace G25.CG.CSJ
