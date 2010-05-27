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
    /// Handles code generation for specialized outermorphisms (classes, constructors, set functions, etc).
    /// </summary>
    class SOM
    {
        public SOM(Specification S, CG.Shared.CGdata cgd)
        {
            m_specification = S;
            m_cgd = cgd;
        }

        protected Specification m_specification;
        protected CG.Shared.CGdata m_cgd;

        /// <summary>
        /// Writes the definition of an SOM struct to 'SB' (including comments).
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="som">The general outermorphism for which the struct should be written.</param>
        public static void WriteSOMstruct(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SOM som)
        {
            SB.AppendLine("");

            { // comments for type: 
                SB.AppendLine("/**");
                SB.AppendLine(" * This struct can hold a specialized outermorphism.");
                SB.AppendLine(" * ");

                SB.AppendLine(" * The coordinates are stored in type " + FT.type + ".");
                SB.AppendLine(" * ");

                SB.AppendLine(" * There are " + som.Domain.Length + " matrices, one for each grade.");
                SB.AppendLine(" * The columns of these matrices are the range of the outermorphism.");
                SB.AppendLine(" * Matrices are stored in row-major order. So the coordinates of rows are stored contiguously.");
                for (int g = 1; g < som.Domain.Length; g++) // start at '1' in order to skip scalar grade
                {
                    SB.Append(" * Domain grade " + g + ": ");
                    for (int i = 0; i < som.DomainForGrade(g).Length; i++)
                    {
                        if (i > 0) SB.Append(", ");
                        SB.Append(som.DomainForGrade(g)[i].ToString(S.m_basisVectorNames));

                    }

                    SB.AppendLine(".");
                }
                SB.AppendLine(" * ");
                if (!som.DomainAndRangeAreEqual())
                {
                    for (int g = 1; g < som.Range.Length; g++) // start at '1' in order to skip scalar grade
                    {
                        SB.Append(" * Range grade " + g + ": ");
                        for (int i = 0; i < som.RangeForGrade(g).Length; i++)
                        {
                            if (i > 0) SB.Append(", ");
                            SB.Append(som.RangeForGrade(g)[i].ToString(S.m_basisVectorNames));

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
            for (int g = 1; g < som.Domain.Length; g++) // start at '1' in order to skip scalar grade
            {
                if (!som.EmptyGrade(g))
                {
                    SB.AppendLine("\t/// Matrix for grade " + g + "; the size is " + som.DomainForGrade(g).Length + " x " + som.RangeForGrade(g).Length);
                    SB.AppendLine("\t" + FT.type + " m" + g + "[" +
                        som.DomainForGrade(g).Length * som.RangeForGrade(g).Length + "];");
                }
            }

            SB.AppendLine("} " + FT.GetMangledName(S, som.Name) + ";");

        }

        /// <summary>
        /// Writes structs for SOM to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSOMstructs(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            // for each float type
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                // for each som
                foreach (G25.SOM som in S.m_SOM)
                {
                    WriteSOMstruct(SB, S, cgd, FT, som);
                }
            }
        }


        /// <summary>
        /// Writes setIdentity, copy and init from vector and matrix for all specialized outermorphism types.
        /// </summary>
        public void WriteSetFunctions()
        {
            G25.CG.Shared.OMinit.WriteSetIdentity(m_specification, m_cgd);
            G25.CG.Shared.OMinit.WriteSetCopy(m_specification, m_cgd);
            G25.CG.Shared.OMinit.WriteSetVectorImages(m_specification, m_cgd);
            G25.CG.Shared.OMinit.WriteSetMatrix(m_specification, m_cgd, false); // false = !transpose
            G25.CG.Shared.OMinit.WriteSetMatrix(m_specification, m_cgd, true); // true = transpose 
        }


    } // end of class SOM
} // end of namespace G25.CG.C

