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
    public class GMV
    {
        public const string SMV_TYPE = "SmvType";
        public const string GROUP_BITMAP = "GroupBitmap";
        public const string SET_GROUP_USAGE = "SetGroupUsage";

        /// <summary>
        /// Writes functions to set the GMV types to zero.
        /// </summary>
        /// <param name="SB">Where the output goes.</param>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT"></param>
        public static void WriteSetZero(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT)
        {
            G25.GMV gmv = S.m_GMV;

            string className = FT.GetMangledName(S, gmv.Name);
            string funcName = "Set";

            string funcDecl = "\tpublic void " + funcName + "()";

            SB.Append(funcDecl);
            SB.AppendLine(" {");
            SB.AppendLine("\t\t" + SET_GROUP_USAGE + "(0);");

            if (S.m_reportUsage)
            {
                SB.AppendLine("\t\tm_t = " + G25.CG.CSJ.GMV.SMV_TYPE + "." + G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, gmv.Name) + ";");
            }

            SB.AppendLine("\t}");
        }

        /// <summary>
        /// Writes functions to set the GMV types to scalar value.
        /// </summary>
        /// <param name="SB">Where the output goes.</param>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT"></param>
        public static void WriteSetScalar(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT)
        {
            G25.GMV gmv = S.m_GMV;
            string className = FT.GetMangledName(S, gmv.Name);
            string funcName = "Set";

            string funcDecl = "\tpublic void " + funcName + "(" + FT.type + " val)";

            SB.Append(funcDecl);
            SB.AppendLine(" {");
            SB.AppendLine("\t\t" + SET_GROUP_USAGE + "(GroupBitmap.GROUP_" + (1 << gmv.GetGroupIdx(RefGA.BasisBlade.ONE)) + ");");
            SB.AppendLine("\t\tm_c[0] = val;");

            if (S.m_reportUsage)
            {
                SB.AppendLine("\t\tm_t = " + G25.CG.CSJ.GMV.SMV_TYPE + "." + G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, FT.type) + ";");
            }

            SB.AppendLine("\t}");
        }

        /// <summary>
        /// Writes functions to set the GMV types by array value.
        /// </summary>
        /// <param name="SB">Where the output goes.</param>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT"></param>
        public static void WriteSetArray(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT)
        {
            G25.GMV gmv = S.m_GMV;

            string className = FT.GetMangledName(S, gmv.Name);
            string funcName = "Set";

            string funcDecl = "\tpublic void " + funcName + "(GroupBitmap gu, " + FT.type + "[] arr)";

            SB.Append(funcDecl);
            SB.AppendLine(" {");
            SB.AppendLine("\t\t" + SET_GROUP_USAGE + "(gu);");
            SB.AppendLine("\t\t" + G25.CG.Shared.Util.GetCopyCode(S, FT, "arr", "m_c", S.m_namespace + ".MvSize[(int)gu]"));

            if (S.m_reportUsage)
            {
                SB.AppendLine("\t\tm_t = " + G25.CG.CSJ.GMV.SMV_TYPE + "." + G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, gmv.Name) + ";");
            }

            SB.AppendLine("\t}");
        }

        /// <summary>
        /// Writes functions to copy GMVs from one float type to another.
        /// </summary>
        /// <param name="SB">Where the output goes.</param>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="dstFT"></param>
        public static void WriteGMVtoGMVcopy(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType dstFT)
        {
            G25.GMV gmv = S.m_GMV;
            foreach (G25.FloatType srcFT in S.m_floatTypes)
            {
                string srcClassName = srcFT.GetMangledName(S, gmv.Name);

                string dstClassName = dstFT.GetMangledName(S, gmv.Name);

                string funcName = "Set";

                string funcDecl = "\tpublic void " + funcName + "(" + srcClassName + " src)";

                SB.Append(funcDecl);
                SB.AppendLine(" {");
                SB.AppendLine("\t\t" + SET_GROUP_USAGE + "(src.gu());");
                SB.AppendLine("\t\t" + srcFT.type + "[] srcC = src.c();");
                if (dstFT == srcFT)
                {
                    SB.AppendLine("\t\t" + G25.CG.Shared.Util.GetCopyCode(S, dstFT, "srcC", "m_c", S.m_namespace + ".MvSize[(int)src.gu()]"));
                }
                else
                {
                    SB.AppendLine("\t\tfor (int i = 0; i < " + S.m_namespace + ".MvSize[(int)src.gu()]; i++)");
                    SB.AppendLine("\t\t\t\tm_c[i] = (" + dstFT.type + ")srcC[i];");
                }

                if (S.m_reportUsage)
                {
                    SB.AppendLine("\t\tm_t = src.m_t;");
                }

                SB.AppendLine("\t}");
            }
        }

        /// <summary>
        /// Writes functions to copy SMVs to GMVs
        /// </summary>
        /// <param name="SB">Where the output goes.</param>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT"></param>
        public static void WriteSMVtoGMVcopy(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT)
        {
            G25.GMV gmv = S.m_GMV;
            bool gmvParityPure = (S.m_GMV.MemoryAllocationMethod == G25.GMV.MEM_ALLOC_METHOD.PARITY_PURE);

            string dstClassName = FT.GetMangledName(S, gmv.Name);
            for (int s = 0; s < S.m_SMV.Count; s++)
            {
                G25.SMV smv = S.m_SMV[s];

                // do not generate converter if the GMV cannot hold the type
                if (gmvParityPure && (!smv.IsParityPure())) continue;

                string srcClassName = FT.GetMangledName(S, smv.Name);

                string funcName = "Set";

                string funcDecl = "\tpublic void " + funcName + "(" + srcClassName + " src)";

                SB.Append(funcDecl);

                {
                    SB.AppendLine(" {");

                    // get a dictionary which tells you for each basis blade of 'gmv' where it is in 'smv'
                    Dictionary<Tuple<int, int>, Tuple<int, int>> D = G25.MV.GetCoordMap(smv, gmv);

                    // convert SMV to symbolic Multivector:
                    bool smvPtr = false;
                    RefGA.Multivector value = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(S, smv, "src", smvPtr);

                    // find out which groups are present
                    StringBuilder guSB = new StringBuilder();
                    int gu = 0;
                    foreach (KeyValuePair<Tuple<int, int>, Tuple<int, int>> KVP in D)
                    {
                        int bit = 1 << KVP.Value.Value1;
                        if ((gu & bit) == 0)
                        {
                            gu |= 1 << KVP.Value.Value1;
                            if (guSB.Length > 0) guSB.Append("|");
                            guSB.Append("GroupBitmap.GROUP_" + KVP.Value.Value1);
                        }
                    }

                    // generate the code to set group usage:
                    SB.AppendLine("\t\t" + SET_GROUP_USAGE + "(" + guSB.ToString() + ");");

                    // a helper pointer which is incremented
                    string dstArrName = "ptr";
                    SB.AppendLine("\t\t" + FT.type + "[] " + dstArrName + " = m_c;");


                    // for each used group, generate the assignment code
                    int dstBaseIdx = 0;
                    for (int g = 0; (1 << g) <= gu; g++)
                    {
                        if (((1 << g) & gu) != 0)
                        {
                            bool mustCast = false;
                            int nbTabs = 2;
                            bool writeZeros = true;
                            string str = G25.CG.Shared.CodeUtil.GenerateGMVassignmentCode(S, FT, mustCast, gmv, dstArrName, g, dstBaseIdx, value, nbTabs, writeZeros);
                            SB.Append(str);

                            if ((1 << (g + 1)) <= gu)
                                dstBaseIdx += gmv.Group(g).Length;
                        }

                    }

                    if (S.m_reportUsage)
                    {
                        SB.AppendLine("\t\tm_t = " + G25.CG.CSJ.GMV.SMV_TYPE + "." + G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, smv.Name) + ";");
                    }

                    SB.AppendLine("\t}");
                }
            } // end of loop over all SMVs
        } // end of WriteGMVtoSMVcopy()




    } // end of class GMV
} // end of namespace G25.CG.CSJ
