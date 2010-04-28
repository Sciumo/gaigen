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
    /// Handles code generation for general multivectors (classes, constructors, set functions, etc).
    /// </summary>
    public class GMV
    {
        public GMV(Specification S, CG.Shared.CGdata cgd)
        {
            m_specification = S;
            m_cgd = cgd;
        }

        protected Specification m_specification;
        protected CG.Shared.CGdata m_cgd;

        /// <summary>
        /// Writes the definition of an GMV struct to 'SB' (including comments).
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="gmv">The general multivector for which the struct should be written.</param>
        public static void WriteGMVstruct(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GMV gmv)
        {
            SB.AppendLine("");

            { // comments for type: 
                SB.AppendLine("/**");
                SB.AppendLine(" * This struct can hold a general multivector.");
                SB.AppendLine(" * ");

                SB.AppendLine(" * The coordinates are stored in type " + FT.type + ".");
                SB.AppendLine(" * ");

                SB.AppendLine(" * There are " + gmv.NbGroups + " coordinate groups:");
                for (int g = 0; g < gmv.NbGroups; g++)
                {
                    SB.Append(" * group " + g + ":");
                    for (int i = 0; i < gmv.Group(g).Length; i++)
                    {
                        if (i > 0) SB.Append(", ");
                        SB.Append(gmv.Group(g)[i].ToString(S.m_basisVectorNames));

                    }
                    if (gmv.Group(g).Length > 0)
                        SB.Append("  (grade " + gmv.Group(g)[0].Grade() + ")");

                    SB.AppendLine(".");
                }
                SB.AppendLine(" * ");

                switch (S.m_GMV.MemoryAllocationMethod)
                {
                    case G25.GMV.MEM_ALLOC_METHOD.PARITY_PURE:
                        SB.AppendLine(" * " + (gmv.NbCoordinates / 2) + " " + FT.type + "s are allocated inside the struct ('parity pure').");
                        SB.AppendLine(" * Hence creating a multivector which needs more than that number of coordinates ");
                        SB.AppendLine(" * will result in unpredictable behaviour (buffer overflow).");
                        break;
                    case G25.GMV.MEM_ALLOC_METHOD.FULL:
                        SB.AppendLine(" * " + gmv.NbCoordinates + " " + FT.type + "s are allocated inside the struct.");
                        break;
                }

                SB.AppendLine(" */");
            } // end of comment

            // typedef
            SB.AppendLine("typedef struct ");
            SB.AppendLine("{");
            // group/grade usage
            SB.AppendLine("\tint gu; ///< group/grade usage (a bitmap which specifies which groups/grades are stored in 'c', below).");

            // coordinates
            switch (S.m_GMV.MemoryAllocationMethod)
            {
//                        case G25.GMV.MEM_ALLOC_METHOD.DYNAMIC:
//                          SB.AppendLine("\t" + FT.type + " *c; ///< the coordinates (array is allocated using realloc())");
//                        break;
                case G25.GMV.MEM_ALLOC_METHOD.PARITY_PURE:
                    SB.AppendLine("\t" + FT.type + " c[" + (gmv.NbCoordinates / 2) + "]; ///< the coordinates (note: parity pure) ");
                    break;
                case G25.GMV.MEM_ALLOC_METHOD.FULL:
                    SB.AppendLine("\t" + FT.type + " c[" + (gmv.NbCoordinates) + "]; ///< the coordinates (full)");
                    break;
            }

            // If we report non-optimized function usage, we need to know original type of GMVs:
            if (S.m_reportUsage) 
                SB.AppendLine("\tint t; ///< Specialized multivector type. Used to report about non-optimized function usage.");

            SB.AppendLine("} " + FT.GetMangledName(S, gmv.Name) + ";");

        }

        /// <summary>
        /// Writes structs for GMV to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteGMVstructs(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            // for each float type
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                WriteGMVstruct(SB, S, cgd, FT, S.m_GMV);
            }
        }

        /// <summary>
        /// Writes functions to set the GMV types to zero.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetZero(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string typeName = FT.GetMangledName(S, gmv.Name);
                string funcName = typeName + "_setZero";

                // write comment
                declSB.AppendLine("/** Sets a " + typeName + " to zero */");

                // do we inline this func?
                string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

                string funcDecl = inlineStr + "void " + funcName + "(" + typeName + " *M)";

                declSB.Append(funcDecl);
                declSB.AppendLine(";");

                defSB.Append(funcDecl);
                defSB.AppendLine(" {");
                defSB.AppendLine("\tM->gu = 0;");
                defSB.AppendLine("}");
            }
        }

        /// <summary>
        /// Writes functions to set the GMV types to scalar value.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetScalar(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                String typeName = FT.GetMangledName(S, gmv.Name);
                String funcName = typeName + "_setScalar";

                // write comment
                declSB.AppendLine("/** Sets a " + typeName + " to a scalar value */");

                // do we inline this func?
                String inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

                String funcDecl = inlineStr + "void " + funcName + "(" + typeName + " *M, " + FT.type +" val)";

                declSB.Append(funcDecl);
                declSB.AppendLine(";");

                defSB.Append(funcDecl);
                defSB.AppendLine(" {");
                defSB.AppendLine("\tM->gu = " + (1 << gmv.GetGroupIdx(RefGA.BasisBlade.ONE)) + ";");
                defSB.AppendLine("\tM->c[0] = val;");
                defSB.AppendLine("}");
            }
        }

        /// <summary>
        /// Writes functions to set the GMV types by array value.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetArray(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string typeName = FT.GetMangledName(S, gmv.Name);
                string funcName = typeName + "_setArray";

                // write comment
                declSB.AppendLine("/** Sets a " + typeName + " to the value in the array. 'gu' is a group usage bitmap. */");

                // do we inline this func?
                string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

                string funcDecl = inlineStr + "void " + funcName + "(" + typeName + " *M, int gu, const " + FT.type + " *arr)";

                declSB.Append(funcDecl);
                declSB.AppendLine(";");

                defSB.Append(funcDecl);
                defSB.AppendLine(" {");
                defSB.AppendLine("\tM->gu = gu;");
                defSB.AppendLine("\t" + G25.CG.Shared.Util.GetCopyCode(S, FT, "arr", "M->c", S.m_namespace + "_mvSize[gu]"));
                defSB.AppendLine("}");
            }
        }

        /// <summary>
        /// Writes functions to copy GMVs from one float type to another.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteGMVtoGMVcopy(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;
            foreach (G25.FloatType srcFT in S.m_floatTypes)
            {
                String srcTypeName = srcFT.GetMangledName(S, gmv.Name);
                foreach (G25.FloatType dstFT in S.m_floatTypes)
                {
                    //if (dstFT == srcFT) continue;

                    string dstTypeName = dstFT.GetMangledName(S, gmv.Name);

                    // write comment
                    if (dstFT == srcFT) declSB.AppendLine("/** Copies a " + srcTypeName + " */");
                    else declSB.AppendLine("/** Copies a " + srcTypeName + " (floating point type " + srcFT.type + ") to a " + dstTypeName + " (floating point type " + dstFT.type + ") */");

                    string funcName;
                    if (dstFT == srcFT) funcName = srcTypeName + "_copy";
                    else funcName = srcTypeName + "_to_" + dstTypeName;

                    // do we inline this func?
                    string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

                    string funcDecl = inlineStr + "void " + funcName + "(" + dstTypeName + " *dst, const " + srcTypeName + " *src)";

                    declSB.Append(funcDecl);
                    declSB.AppendLine(";");

                    defSB.Append(funcDecl);
                    defSB.AppendLine(" {");
                    defSB.AppendLine("\tint i;");
                    defSB.AppendLine("\tdst->gu = src->gu;");
                    defSB.AppendLine("\tfor (i = 0; i < " + S.m_namespace + "_mvSize[src->gu]; i++)");
                    defSB.AppendLine("\t\tdst->c[i] = (" + dstFT.type + ")src->c[i];");
                    defSB.AppendLine("}");
                }
            }
        }

        /// <summary>
        /// Writes functions to copy GMVs to SMVs
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteGMVtoSMVcopy(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                String srcTypeName = FT.GetMangledName(S, gmv.Name);
                for (int s = 0; s < S.m_SMV.Count; s++)
                {
                    G25.SMV smv = S.m_SMV[s];
                    String dstTypeName = FT.GetMangledName(S, smv.Name);

                    bool dstPtr = true;
                    String[] smvAccessStr = G25.CG.Shared.CodeUtil.GetAccessStr(S, smv, "dst", dstPtr);


                    // write comment
                    declSB.AppendLine("/** Copies a " + srcTypeName + " to a " + dstTypeName + " (coordinates/basis blades which cannot be represented are silenty lost). */");

                    String funcName = srcTypeName + "_to_" + dstTypeName;

                    // do we inline this func?
                    String inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

                    String funcDecl = inlineStr + "void " + funcName + "(" + dstTypeName + " *dst, const " + srcTypeName + " *src)";

                    declSB.Append(funcDecl);
                    declSB.AppendLine(";");

                    defSB.Append(funcDecl);
                    {
                        defSB.AppendLine(" {");
                        defSB.AppendLine("\tconst " + FT.type + " *ptr = src->c;\n");

                        // get a dictionary which tells you for each basis blade of 'smv' where it is in 'gmv'
                        Dictionary<Tuple<int, int>, Tuple<int, int>> D = G25.MV.GetCoordMap(smv, gmv);

                        // what is the highest group of the 'gmv' that must be (partially) copied to the 'smv'
                        int highestGroup = -1;
                        foreach (KeyValuePair<Tuple<int, int>, Tuple<int, int>> KVP in D)
                            if (KVP.Value.Value1 > highestGroup) highestGroup = KVP.Value.Value1;

                        // generate code for each group
                        for (int g = 0; g <= highestGroup; g++)
                        {
                            // determine if group 'g' is to be copied to smv:
                            bool groupIsUsedBySMV = false;
                            foreach (KeyValuePair<Tuple<int, int>, Tuple<int, int>> KVP in D)
                            {
                                if (KVP.Value.Value1 == g)
                                {
                                    groupIsUsedBySMV = true;
                                    break;
                                }
                            }


                            // if group is present in GMV:
                            defSB.AppendLine("\tif (src->gu & " + (1 << g) + ") {");
                            if (groupIsUsedBySMV)
                            {
                                bool mustCast = false;
                                bool srcPtr = true;
                                int nbTabs = 2;
                                RefGA.Multivector[] value = G25.CG.Shared.Symbolic.GMVtoSymbolicMultivector(S, gmv, "ptr", srcPtr, g);
                                bool writeZeros = false;
                                String str = G25.CG.Shared.CodeUtil.GenerateSMVassignmentCode(S, FT, mustCast, smv, "dst", dstPtr, value[g], nbTabs, writeZeros);
                                defSB.Append(str);
                            }
                            defSB.AppendLine("\t\tptr += " + gmv.Group(g).Length + ";");
                            defSB.AppendLine("\t}");

                            // else, if group is not present in GMV:
                            if (groupIsUsedBySMV)
                            {
                                defSB.AppendLine("\telse {");
                                foreach (KeyValuePair<Tuple<int, int>, Tuple<int, int>> KVP in D)
                                    if ((KVP.Value.Value1 == g) && (!smv.IsCoordinateConstant(KVP.Key.Value2)))
                                    {
                                        // translate KVP.Key.Value2 to non-const idx, because the accessStrs are only about non-const blades blades!
                                        int bladeIdx = smv.BladeIdxToNonConstBladeIdx(KVP.Key.Value2);
                                        defSB.AppendLine("\t\t" + smvAccessStr[bladeIdx] + " = " + FT.DoubleToString(S, 0.0) + ";");
                                    }
                                defSB.AppendLine("\t}");
                            }
                        }
                        defSB.AppendLine("}");
                    }
                } // end of loop over all SMVs
            } // end of loop over all float types
        } // end of WriteGMVtoSMVcopy()


        /// <summary>
        /// Writes functions to copy SMVs to GMVs
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSMVtoGMVcopy(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;
            bool gmvParityPure = (S.m_GMV.MemoryAllocationMethod == G25.GMV.MEM_ALLOC_METHOD.PARITY_PURE);
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string dstTypeName = FT.GetMangledName(S, gmv.Name);
                for (int s = 0; s < S.m_SMV.Count; s++)
                {
                    G25.SMV smv = S.m_SMV[s];

                    // do not generate converter if the GMV cannot hold the type
                    if (gmvParityPure && (!smv.IsParityPure())) continue;


                    string srcTypeName = FT.GetMangledName(S, smv.Name);

                    // write comment
                    declSB.AppendLine("/** Copies a " + srcTypeName + " to a " + dstTypeName + " */");

                    string funcName = srcTypeName + "_to_" + dstTypeName;

                    // do we inline this func?
                    String inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

                    string funcDecl = inlineStr + "void " + funcName + "(" + dstTypeName + " *dst, const " + srcTypeName + " *src)";


                    declSB.Append(funcDecl);
                    declSB.AppendLine(";");

                    defSB.Append(funcDecl);

                    {
                        defSB.AppendLine(" {");

                        // get a dictionary which tells you for each basis blade of 'gmv' where it is in 'smv'
                        Dictionary<Tuple<int, int>, Tuple<int, int>> D = G25.MV.GetCoordMap(smv, gmv);

                        // convert SMV to symbolic Multivector:
                        bool smvPtr = true;
                        RefGA.Multivector value = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(S, smv, "src", smvPtr);

                        // find out which groups are present
                        int gu = 0;
                        foreach (KeyValuePair<Tuple<int, int>, Tuple<int, int>> KVP in D)
                            gu |= 1 << KVP.Value.Value1;

                        // a helper pointer which is incremented
                        string dstArrName = "ptr";
                        defSB.AppendLine("\t" + FT.type + " *" + dstArrName + " = dst->c;");

                        // generate the code to set group usage:
                        defSB.AppendLine("\tdst->gu = " + gu + ";");

                        // for each used group, generate the assignment code
                        for (int g = 0; (1 << g) <= gu; g++)
                        {
                            if (((1 << g) & gu) != 0)
                            {
                                bool mustCast = false;
                                int nbTabs = 1;
                                bool writeZeros = true;
                                string str = G25.CG.Shared.CodeUtil.GenerateGMVassignmentCode(S, FT, mustCast, gmv, dstArrName, g, value, nbTabs, writeZeros);
                                defSB.Append(str);

                                defSB.AppendLine("\tptr += " + gmv.Group(g).Length + ";");
                            }

                        }
                        

                        // find out which elements of those groups are not written (or maybe generate assign code?)

                        defSB.AppendLine("}");
                    }


                } // end of loop over all SMVs
            } // end of loop over all float types
        } // end of WriteGMVtoSMVcopy()


        private static void WriteCoordExtractFunction(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, string gmvTypeName, int groupIdx, int elementIdx, RefGA.BasisBlade B)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            String bladeName = B.ToLangString(S.m_basisVectorNames);

            string varName = "A";

            // do we inline this func?
            string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

            string funcName = gmvTypeName + "_" + bladeName;

            string funcDecl = inlineStr + FT.type + " " + funcName + "(const " + gmvTypeName + " *" + varName + ")";

            string comment = "/** Returns the " + B.ToString(S.m_basisVectorNames) + " coordinate of '" + varName + "' */";
//                    declSB.AppendLine("/* group : " + groupIdx + " element: " + elementIdx + "*/");
            declSB.AppendLine(comment);
            declSB.Append(funcDecl);
            declSB.AppendLine(";");

            defSB.AppendLine("");
            defSB.Append(funcDecl);
            defSB.AppendLine(" {");
            defSB.AppendLine("\treturn (" + varName + "->gu & " + (1 << groupIdx) + ") ? " +
                varName + "->c[" + S.m_namespace + "_mvSize[" + varName + "->gu & " + ((1 << groupIdx) - 1) + "] + " + elementIdx + "] : " + 
                FT.DoubleToString(S, 0.0) + ";");
            defSB.AppendLine("}");

            // add extract coord extract function for scalar
            if (B.Grade() == 0)
            {
                string floatFuncName = gmvTypeName + "_" + FT.type;
                string floatFuncDecl = inlineStr + FT.type + " " + floatFuncName + "(const " + gmvTypeName + " *" + varName + ")";

                declSB.AppendLine(comment);
                declSB.Append(floatFuncDecl);
                declSB.AppendLine(";");

                defSB.Append(floatFuncDecl);
                defSB.AppendLine(" {");
                defSB.AppendLine("\treturn " + funcName + "(" + varName + ");");
                defSB.AppendLine("}");
            }
        }

        private static void WriteCoordSetFunction(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, string gmvTypeName, int groupIdx, int elementIdx, int groupSize, RefGA.BasisBlade B)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            String bladeName = B.ToLangString(S.m_basisVectorNames);

            string varName = "A";
            string coordName = bladeName + "_coord";

            // do we inline this func?
            string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

            string funcName = gmvTypeName + "_set_" + bladeName;

            string funcDecl = inlineStr + "void " + funcName + "(" + gmvTypeName + " *" + varName + ", " + FT.type + " " + coordName  + ")";

            declSB.AppendLine("/** Sets the " + B.ToString(S.m_basisVectorNames) + " coordinate of '" + varName + "' */");
            declSB.Append(funcDecl);
            declSB.AppendLine(";");

            defSB.AppendLine("");
            defSB.Append(funcDecl);
            defSB.AppendLine(" {");

            defSB.AppendLine("\t" + gmvTypeName + "_reserveGroup_" + groupIdx + "(" + varName + ");");
            defSB.AppendLine("\t" + varName + "->c[" + S.m_namespace + "_mvSize[" + varName + "->gu & " + ((1 << groupIdx) - 1) + "] + " + elementIdx + "] = " + coordName + ";");
            defSB.AppendLine("}");

        }

        private static void WriteReserveGroup(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, string gmvTypeName, int groupIdx, int groupSize)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            string varName = "A";

            // do we inline this func?
            string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

            string funcName = gmvTypeName + "_reserveGroup_" + groupIdx;

            string funcDecl = inlineStr + "void " + funcName + "(" + gmvTypeName + " *" + varName + ")";

            declSB.AppendLine("/** Allocates memory to store coordinate group " + groupIdx + " */");
            declSB.Append(funcDecl);
            declSB.AppendLine(";");

            defSB.AppendLine("");
            defSB.Append(funcDecl);
            defSB.AppendLine(" {");

            defSB.AppendLine("\tint groupUsageBelow, groupUsageAbove, newGroupUsage, newGroupUsageBelowNextGroup;");
            if (groupIdx < (S.m_GMV.NbGroups - 1))
            {
                defSB.AppendLine("\tint i;");
                defSB.AppendLine("\t" + FT.type + " *dst, *src;");
            }
            defSB.AppendLine("\tif ((" + varName + "->gu & " + (1 << groupIdx) + ") == 0) {");
            defSB.AppendLine("");

            defSB.AppendLine("\t\tgroupUsageBelow = " + varName + "->gu & " + ((1 << groupIdx)-1) + ";");
            defSB.AppendLine("\t\tgroupUsageAbove = " + varName + "->gu ^ groupUsageBelow;");
            defSB.AppendLine("\t\tnewGroupUsage = " + varName + "->gu | " + (1 << groupIdx) + ";");
            defSB.AppendLine("\t\tnewGroupUsageBelowNextGroup = newGroupUsage & " + ((1 << (groupIdx+1))-1) + ";");
            defSB.AppendLine("");

            if (groupIdx < (S.m_GMV.NbGroups-1)) {
	            // move coordinate beyond new group
	            defSB.AppendLine("\t\tdst = " + varName + "->c + " + S.m_namespace + "_mvSize[newGroupUsageBelowNextGroup];");
                defSB.AppendLine("\t\tsrc = " + varName + "->c + " + S.m_namespace + "_mvSize[groupUsageBelow];");
	            defSB.AppendLine("\t\tfor (i = " + S.m_namespace + "_mvSize[groupUsageAbove]-1; i >= 0; i--) // work from end to start of array to avoid overwriting (dst is always beyond src)");
	            defSB.AppendLine("\t\t\tdst[i] = src[i];");
            }
/*                    defSB.AppendLine("\t\tmemmove(" +
                varName + "->c + " + S.m_namespace + "_mvSize[newGroupUsageBelowNextGroup], " +
                varName + "->c + " + S.m_namespace + "_mvSize[groupUsageBelow], " +
                "sizeof(" + FT.type + ") * " + S.m_namespace + "_mvSize[groupUsageAbove]);");*/

            defSB.AppendLine("\t\t" + G25.CG.Shared.Util.GetSetToZeroCode(S, FT, varName + "->c", groupSize));

            defSB.AppendLine("\t\t" + varName + "->gu = newGroupUsage;");
            defSB.AppendLine("\t}");
            defSB.AppendLine("}");
        }

        /// <summary>
        /// Writes functions to reserve a coordinate group
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd"></param>
        public static void WriteReserveGroups(Specification S, G25.CG.Shared.CGdata cgd) {
            //StringBuilder declSB = cgd.m_declSB;
            //StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string typeName = FT.GetMangledName(S, gmv.Name);

                for (int groupIdx = 0; groupIdx < gmv.NbGroups; groupIdx++)
                {
                    WriteReserveGroup(S, cgd, FT, typeName, groupIdx, gmv.Group(groupIdx).Length);
                }
            }
        }


        /// <summary>
        /// Writes functions to extract coordinates from the GMV
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteCoordAccess(Specification S, G25.CG.Shared.CGdata cgd)
        {
            //StringBuilder declSB = cgd.m_declSB;
            //StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string typeName = FT.GetMangledName(S, gmv.Name);

                for (int groupIdx = 0; groupIdx < gmv.NbGroups; groupIdx++)
                {
                    for (int elementIdx = 0; elementIdx < gmv.Group(groupIdx).Length; elementIdx++)
                    {
                        WriteCoordExtractFunction(S, cgd, FT, typeName, groupIdx, elementIdx, gmv.Group(groupIdx)[elementIdx]);
                        WriteCoordSetFunction(S, cgd, FT, typeName, groupIdx, elementIdx, gmv.Group(groupIdx).Length, gmv.Group(groupIdx)[elementIdx]);
                    }
                }
            }
        }


        /// <summary>
        /// Writes code for abs largest coordinate
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd"></param>
        public static void WriteLargestCoordinateFunctions(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;

            foreach (G25.FloatType FT in S.m_floatTypes) {

                string fabsFunc = (FT.type == "float") ? "fabsf" : "fabs";

                string gmvName = FT.GetMangledName(S, gmv.Name);

                cgd.m_cog.EmitTemplate(declSB, "largestCoordinateDecl", 
                    "FT=", FT,
                    "gmvName=", gmvName);

                cgd.m_cog.EmitTemplate(defSB, "largestCoordinateDef",
                    "S=", S,
                    "FT=", FT,
                    "gmvName=", gmvName,
                    "fabsFunc=", fabsFunc);
            }
        } // end of WriteLargestCoordinateFunctions()

        /// <summary>
        /// Writes set, setZero, copy and copyCrossFloat, coord extract, largest coordinate functions for all general multivector types.
        /// </summary>
        public void WriteSetFunctions()
        {
            WriteSetZero(m_specification, m_cgd);
            WriteSetScalar(m_specification, m_cgd);
            WriteSetArray(m_specification, m_cgd);

            WriteGMVtoGMVcopy(m_specification, m_cgd);
            WriteGMVtoSMVcopy(m_specification, m_cgd);
            WriteSMVtoGMVcopy(m_specification, m_cgd);

            WriteReserveGroups(m_specification, m_cgd);
            WriteCoordAccess(m_specification, m_cgd);

            WriteLargestCoordinateFunctions(m_specification, m_cgd);
        }



    } // end of class GMV
} // end of namespace G25.CG.C

