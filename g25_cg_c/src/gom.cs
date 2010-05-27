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
    /// Handles code generation for general outermorphisms (classes, constructors, set functions, etc).
    /// </summary>
    class GOM
    {
        public GOM(Specification S, CG.Shared.CGdata cgd)
        {
            m_specification = S;
            m_cgd = cgd;
        }

        protected Specification m_specification;
        protected CG.Shared.CGdata m_cgd;

        /// <summary>
        /// Writes the definition of an GOM struct to 'SB' (including comments).
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="gom">The general outermorphism for which the struct should be written.</param>
        public static void WriteGOMstruct(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GOM gom)
        {
            SB.AppendLine("");

            { // comments for type: 
                SB.AppendLine("/**");
                SB.AppendLine(" * This struct can hold a general outermorphism.");
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
            } // end of comment

            // typedef
            SB.AppendLine("typedef struct ");
            SB.AppendLine("{");
            for (int g = 1; g < gom.Domain.Length; g++) // start at '1' in order to skip scalar grade
            {
                SB.AppendLine("\t/// Matrix for grade " + g + "; the size is " + gom.DomainForGrade(g).Length + " x " + gom.RangeForGrade(g).Length);
                SB.AppendLine("\t" + FT.type + " m" + g + "[" + 
                    gom.DomainForGrade(g).Length * gom.RangeForGrade(g).Length + "];");
            }

            SB.AppendLine("} " + FT.GetMangledName(S, gom.Name) + ";");

        }

        /// <summary>
        /// Writes structs for GOM to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteGOMstructs(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            // for each float type
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                WriteGOMstruct(SB, S, cgd, FT, S.m_GOM);
            }
        }

        /// <summary>
        /// Writes a function to set an GOM struct to identity, for all floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetIdentity(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            declSB.AppendLine("");
            defSB.AppendLine("");

            string omName = "a";
            string matrixName = "m";

            Dictionary<double, List<string>> nonZero = G25.CG.Shared.OMinit.GetGomIdentityInitCode(S, S.m_GOM, omName, matrixName);

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string typeName = FT.GetMangledName(S, S.m_GOM.Name);
                string funcName = typeName + "_setIdentity";
                //string comment = "/** Sets " + typeName + " to identity */";
                declSB.AppendLine("void " + funcName + "(" + typeName + "*" + omName + ");");
                defSB.AppendLine("void " + funcName + "(" + typeName + "*" + omName + ") {");
                for (int g = 1; g < S.m_GOM.Domain.Length; g++)
                {
                    int s = S.m_GOM.Domain[g].Length * S.m_GOM.Range[g].Length;
                    defSB.AppendLine("\t" + G25.CG.Shared.Util.GetSetToZeroCode(S, FT, omName + "->" + matrixName + g, s));
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
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            declSB.AppendLine("");
            defSB.AppendLine("");

            string dstName = "dst";
            string srcName = "src";
            string matrixName = "m";

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                String typeName = FT.GetMangledName(S, S.m_GOM.Name);
                String funcName = typeName + "_copy";
                //string comment = "/** Copies " + typeName + " */";
                declSB.AppendLine("void " + funcName + "(" + typeName + "*" + dstName + ", const " + typeName + "*" + srcName + ");");
                defSB.AppendLine("void " + funcName + "(" + typeName + "*" + dstName + ", const " + typeName + "*" + srcName + ") {");
                for (int g = 1; g < S.m_GOM.Domain.Length; g++)
                {
                    int s = S.m_GOM.Domain[g].Length * S.m_GOM.Range[g].Length;
                    defSB.AppendLine("\t" + G25.CG.Shared.Util.GetCopyCode(S, FT, srcName + "->" + matrixName + g, dstName + "->" + matrixName + g, s));
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



        /// Writes functions to copy GOM to SOMs and back again.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteGOMtoSOMcopy(Specification S, G25.CG.Shared.CGdata cgd)
        {
            G25.CG.Shared.OMinit.WriteGOMtoSOMcopy(S, cgd);
            G25.CG.Shared.OMinit.WriteSOMtoGOMcopy(S, cgd);
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

    } // end of class GOM
} // end of namespace G25.CG.C

