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
#if RIEN
        public GOM(Specification S, CG.Shared.CGdata cgd)
        {
            m_specification = S;
            m_cgd = cgd;
        }

        protected Specification m_specification;
        protected CG.Shared.CGdata m_cgd;


        /// <summary>
        /// Writes comments of a GOM class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        /// <param name="gom">The general outermorphism for which the class should be written.</param>
        public static void WriteComment(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GOM gom) {
            SB.AppendLine("/**");
            SB.AppendLine(" * This class can hold a general outermorphism.");
            SB.AppendLine(" * ");

            SB.AppendLine(" * The coordinates are stored in type " + FT.type + ".");
            SB.AppendLine(" * ");

            SB.AppendLine(" * There are " + gom.Domain.Length + " matrices, one for each grade.");
            SB.AppendLine(" * The columns of these matrices are the range of the outermorphism.");
            SB.AppendLine(" * Matrices are stored in row-major order. So the coordinates of rows are stored contiguously.");
            for (int g = 1; g < gom.Domain.Length; g++) // start at '1' in order to skip scalar grade
            {
                SB.Append(" * Domain grade " + g + ": ");
                for (int i = 0; i < gom.DomainForGrade(g).Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(gom.DomainForGrade(g)[i].ToString(S.m_basisVectorNames));

                }

                SB.AppendLine(".");
            }
            SB.AppendLine(" * ");
            if (!gom.DomainAndRangeAreEqual())
            {
                for (int g = 1; g < gom.Range.Length; g++) // start at '1' in order to skip scalar grade
                {
                    SB.Append(" * Range grade " + g + ": ");
                    for (int i = 0; i < gom.RangeForGrade(g).Length; i++)
                    {
                        if (i > 0) SB.Append(", ");
                        SB.Append(gom.RangeForGrade(g)[i].ToString(S.m_basisVectorNames));

                    }

                    SB.AppendLine(".");
                }
            }
            else SB.AppendLine(" * The range and domain are equal.");
            SB.AppendLine(" * ");

            SB.AppendLine(" */");
        }


        /// <summary>
        /// Writes members variables of a GOM class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        /// <param name="gom">The general outermorphism for which the class should be written.</param>
        public static void WriteMemberVariables(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GOM gom)
        {
            SB.AppendLine("public:");
            for (int g = 1; g < gom.Domain.Length; g++) // start at '1' in order to skip scalar grade
            {
                SB.AppendLine("\t/// Matrix for grade " + g + "; the size is " + gom.DomainForGrade(g).Length + " x " + gom.RangeForGrade(g).Length);
                SB.AppendLine("\t" + FT.type + " m_m" + g + "[" +
                    gom.DomainForGrade(g).Length * gom.RangeForGrade(g).Length + "];");
            }
        }

        /// <summary>
        /// Writes members variables of a GOM class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        /// <param name="gom">The general outermorphism for which the class should be written.</param>
        /// <param name="className"></param>
        public static void WriteFloatType(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GOM gom, string className)
        {
            cgd.m_cog.EmitTemplate(SB, "classFloatType", "S=", S, "FT=", FT, "className=", className);
        }

        /// <summary>
        /// Writes constructors of a GOM class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        /// <param name="gom">The general outermorphism for which the class should be written.</param>
        /// <param name="className">Mangled name of GOM class.</param>
        /// <param name="rangeVectorSMVname">The name of the SMV which can represent a column of the OM.</param>
        public static void WriteConstructors(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GOM gom, string className, string rangeVectorSMVname)
        {
            cgd.m_cog.EmitTemplate(SB, "GOMconstructors", "S=", S, "FT=", FT, "className=", className, "rangeVectorSMVname=", rangeVectorSMVname);
        }

        /// <summary>
        /// Writes assignment operators of a GOM class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        /// <param name="gom">The general outermorphism for which the class should be written.</param>
        /// <param name="className">Mangled name of GOM class.</param>
        /// <param name="rangeVectorSMVname">The name of the SMV which can represent a column of the OM.</param>
        public static void WriteAssignmentOps(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GOM gom, string className, string rangeVectorSMVname)
        {
            cgd.m_cog.EmitTemplate(SB, "GOMassignmentOps", "S=", S, "FT=", FT, "className=", className, "rangeVectorSMVname=", rangeVectorSMVname);
        }

        /// <summary>
        /// Writes 'set()' declarations of a GOM class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        /// <param name="gom">The general outermorphism for which the class should be written.</param>
        /// <param name="className">Mangled name of GOM class.</param>
        /// <param name="rangeVectorSMVname">The name of the SMV which can represent a column of the OM.</param>
        public static void WriteSetDeclarations(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GOM gom, string className, string rangeVectorSMVname)
        {
            cgd.m_cog.EmitTemplate(SB, "GOMsetDecl", "S=", S, "FT=", FT, "className=", className, "rangeVectorSMVname=", rangeVectorSMVname);
        }

        /// <summary>
        /// Writes the definition of an GOM struct to 'SB' (including comments).
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        /// <param name="gom">The general outermorphism for which the struct should be written.</param>
        public static void WriteGOMclass(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GOM gom)
        {
            SB.AppendLine("");

            string className = FT.GetMangledName(S, gom.Name);

            // get range vector type
            G25.SMV rangeVectorType = G25.CG.Shared.OMinit.getRangeVectorType(S, FT, cgd, gom);
            string rangeVectorSMVname = FT.GetMangledName(S, rangeVectorType.Name);

            WriteComment(SB, S, cgd, FT, gom);

            // typedef
            SB.AppendLine("class " + className);
            SB.AppendLine("{");

            // member vars
            WriteMemberVariables(SB, S, cgd, FT, gom);

            SB.AppendLine("public:");

            // Float type
            WriteFloatType(SB, S, cgd, FT, gom, className);

            // constructors
            WriteConstructors(SB, S, cgd, FT, gom, className, rangeVectorSMVname);

            // operator=
            WriteAssignmentOps(SB, S, cgd, FT, gom, className, rangeVectorSMVname);

            // set(...)
            WriteSetDeclarations(SB, S, cgd, FT, gom, className, rangeVectorSMVname);

            SB.AppendLine("}; // end of " + className);

        }

        /// <summary>
        /// Writes structs for GOM to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteGOMclasses(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            // for each float type
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                WriteGOMclass(SB, S, cgd, FT, S.m_GOM);
            }
        }

        /// <summary>
        /// Writes typenames of all GMV to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteGOMtypes(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            // for each float type
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string className = FT.GetMangledName(S, S.m_GOM.Name);
                // typedef
                SB.AppendLine("class " + className + ";");
            }
        }

        /// <summary>
        /// Writes a function to set an GOM struct to identity, for all floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetIdentity(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

            defSB.AppendLine("");

            string omName = "a";
            string matrixName = "m_m";

            Dictionary<double, List<string>> nonZero = G25.CG.Shared.OMinit.GetGomIdentityInitCode(S, S.m_GOM, omName, matrixName);
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string className = FT.GetMangledName(S, S.m_GOM.Name);
                string funcName = className + "::setIdentity";
                defSB.AppendLine(inlineStr + "void " + funcName + "() {");
                for (int g = 1; g < S.m_GOM.Domain.Length; g++)
                {
                    int s = S.m_GOM.Domain[g].Length * S.m_GOM.Range[g].Length;
                    defSB.AppendLine("\t" + G25.CG.Shared.Util.GetSetToZeroCode(S, FT, matrixName + g, s));
                } // end of loop over all grades of the OM

                // do the nonZero assignments:
                foreach (KeyValuePair<double, List<string>> kvp in nonZero)
                {
                    defSB.Append("\t");
                    int cnt = 0;
                    foreach (string coordStr in kvp.Value)
                    {
                        defSB.Append(coordStr + " = ");
                        cnt++;
                        if ((cnt % 8) == 0)
                            defSB.Append("\n\t\t");
                    }
                    defSB.AppendLine(FT.DoubleToString(S, kvp.Key) + ";");
                }

                defSB.AppendLine("}");
            }
        } // end of WriteSetIdentity()

        /// <summary>
        /// Writes a function to copy an GOM struct
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteCopy(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

            defSB.AppendLine("");

            string srcName = "src";
            string matrixName = "m_m";

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string className = FT.GetMangledName(S, S.m_GOM.Name);
                string funcName = className + "::set";
                defSB.AppendLine(inlineStr + "void " + funcName + "(const " + className + " &" + srcName + ") {");
                for (int g = 1; g < S.m_GOM.Domain.Length; g++)
                {
                    int s = S.m_GOM.Domain[g].Length * S.m_GOM.Range[g].Length;
                    defSB.AppendLine("\t" + G25.CG.Shared.Util.GetCopyCode(S, FT, srcName + "." + matrixName + g, matrixName + g, s));
                } // end of loop over all grades of the OM
                defSB.AppendLine("}");
            }
        } // end of WriteCopy()

        /// <summary>
        /// Writes a function to set a GOM struct according to vector images, for all floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="matrixMode">When true, generates code for setting from matrix instead of vector images.</param>
        /// <param name="transpose">When this parameter is true and <c>matrixMode</c> is true, generates code for setting from transpose matrix.</param>
        public static void WriteSetVectorImages(Specification S, G25.CG.Shared.CGdata cgd, bool matrixMode, bool transpose)
        {
            G25.CG.Shared.OMinit.WriteSetVectorImages(S, cgd, matrixMode, transpose);
        } // end of WriteSetVectorImages()


        private static void WriteOMtoOMcopy(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, OM srcOm, OM dstOm)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            string srcTypeName = FT.GetMangledName(S, srcOm.Name);
            string dstTypeName = FT.GetMangledName(S, dstOm.Name);

            // write comment
            declSB.AppendLine("/** Copies a " + srcTypeName + " to a " + dstTypeName);
            declSB.AppendLine(" * Warning 1: coordinates which cannot be represented are silenty lost");
            declSB.AppendLine(" * Warning 2: coordinates which are not present in 'src' are set to zero in 'dst'.");
            declSB.AppendLine(" */");

            string funcName = srcTypeName + "_to_" + dstTypeName;

            // do we inline this func?
            string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

            string funcDecl = inlineStr + "void " + funcName + "(" + dstTypeName + " *dst, const " + srcTypeName + " *src)";

            declSB.Append(funcDecl);
            declSB.AppendLine(";");

            defSB.Append(funcDecl);
            {
                defSB.AppendLine(" {");

                Dictionary<Tuple<int, int, int>, Tuple<int, int, double>> D = dstOm.getMapping(srcOm);

                StringBuilder copySB = new StringBuilder(); 
                List<string> setToZero = new List<string>(); 

                // For all grades of som, for all columns, for all rows, check D, get entry, set; otherwise set to null
                // Do not use foreach() on D because we want to fill in coordinates in their proper order.
                for (int gradeIdx = 1; gradeIdx < dstOm.Domain.Length; gradeIdx++)
                {
                    for (int somRangeIdx = 0; somRangeIdx < dstOm.Range[gradeIdx].Length; somRangeIdx++)
                    {
                        for (int somDomainIdx = 0; somDomainIdx < dstOm.Domain[gradeIdx].Length; somDomainIdx++)
                        {
                            Tuple<int, int, int> key = new Tuple<int, int, int>(gradeIdx, somDomainIdx, somRangeIdx);
                            
                            int somMatrixIdx = dstOm.getCoordinateIndex(gradeIdx, somDomainIdx, somRangeIdx);
                            string dstString = "dst->m" + gradeIdx + "[" + somMatrixIdx + "] = ";
                            if (D.ContainsKey(key))
                            {
                                Tuple<int, int, double> value = D[key];
                                int gomMatrixIdx = srcOm.getCoordinateIndex(gradeIdx, value.Value1, value.Value2);
                                double multiplier = value.Value3;
                                string multiplierString = (multiplier == 1.0) ? "" : (FT.DoubleToString(S, multiplier) + " * ");

                                copySB.AppendLine("\t" + dstString + multiplierString + " src->m" + gradeIdx + "[" + gomMatrixIdx + "];");
                            }
                            else
                            {
                                setToZero.Add(dstString);
                            }
                        }
                    }
                }

                // append copy statements
                defSB.Append(copySB);

                // append statements to set coordinates to zero
                if (setToZero.Count > 0)
                {
                    int cnt = 0;
                    defSB.Append("\t");
                    foreach (string str in setToZero)
                    {
                        defSB.Append(str);
                        cnt++;
                        if (cnt > 8)
                        {
                            cnt = 0;
                            defSB.AppendLine("");
                            defSB.Append("\t\t"); 
                        }
                    }
                    defSB.AppendLine(FT.DoubleToString(S, 0.0) + ";");
                }

                defSB.AppendLine("}");
            }
        }

        /// Writes functions to copy GOM to SOMs and back again.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteGOMtoSOMcopy(Specification S, G25.CG.Shared.CGdata cgd)
        {
            G25.CG.Shared.OMinit.WriteGOMtoSOMcopy(S, cgd);
        } // end of WriteGOMtoSOMcopy()



        /// <summary>
        /// Writes setIdentity, copy and init from vector and matrix for general outermorphism type.
        /// </summary>
        public void WriteSetFunctions()
        {
            if (m_specification.m_GOM == null) return;

            WriteSetIdentity(m_specification, m_cgd);
            WriteCopy(m_specification, m_cgd);
            WriteSetVectorImages(m_specification, m_cgd, false, false); // false, false = matrixMode, transpose
            WriteSetVectorImages(m_specification, m_cgd, true, false); // true, false = matrixMode, transpose
            WriteGOMtoSOMcopy(m_specification, m_cgd);
            
        }

#endif
    } // end of class GOM
} // end of namespace G25.CG.CSharp

