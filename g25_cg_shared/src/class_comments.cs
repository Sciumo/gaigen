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


namespace G25.CG.Shared
{

    /// <summary>
    /// Provides function which generate class comments which can be shared across output languages.
    /// </summary>
    public class ClassComments
    {
        /// <summary>
        /// Returns the comment for a GMV class.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GMV'.</param>
        /// <param name="gmv">The general multivector for which the class should be written.</param>
        public static Comment GetGmvComment(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GMV gmv)
        {
            StringBuilder SB = new StringBuilder();

            SB.AppendLine("This class can hold a general multivector.");
            SB.AppendLine("");

            SB.AppendLine("The coordinates are stored in type " + FT.type + ".");
            SB.AppendLine("");

            SB.AppendLine("There are " + gmv.NbGroups + " coordinate groups:");
            for (int g = 0; g < gmv.NbGroups; g++)
            {
                SB.Append("group " + g + ":");
                for (int i = 0; i < gmv.Group(g).Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(gmv.Group(g)[i].ToString(S.m_basisVectorNames));

                }
                if (gmv.Group(g).Length > 0)
                    SB.Append("  (grade " + gmv.Group(g)[0].Grade() + ")");

                SB.AppendLine(".");
            }
            SB.AppendLine("");

            switch (S.m_GMV.MemoryAllocationMethod)
            {
                case G25.GMV.MEM_ALLOC_METHOD.PARITY_PURE:
                    SB.AppendLine("" + (gmv.NbCoordinates / 2) + " " + FT.type + "s are allocated inside the struct ('parity pure').");
                    SB.AppendLine("Hence creating a multivector which needs more than that number of coordinates ");
                    SB.AppendLine("will result in unpredictable behaviour (buffer overflow).");
                    break;
                case G25.GMV.MEM_ALLOC_METHOD.FULL:
                    SB.AppendLine("" + gmv.NbCoordinates + " " + FT.type + "s are allocated inside the struct.");
                    break;
            }

            return new Comment(SB.ToString());
        } // end of GetGmvComment()


        /// <summary>
        /// Returns the comment for a SMV class.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Not used yet.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="smv">The specialized multivector for which the struct should be written.</param>
        /// <param name="emitCoordIndices">Whether to emit constants for array indices to coordinates.</param>
        public static Comment GetSmvComment(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv, bool emitCoordIndices)
        {
            StringBuilder SB = new StringBuilder();
            SB.AppendLine("This class can hold a specialized multivector of type " + FT.GetMangledName(S, smv.Name) + ".");
            SB.AppendLine("");

            SB.AppendLine("The coordinates are stored in type  " + FT.type + ".");
            SB.AppendLine("");

            if (smv.NbNonConstBasisBlade > 0)
            {
                SB.AppendLine("The variable non-zero coordinates are:");
                for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                {
                    SB.AppendLine("  - coordinate " + smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames) + "  (array index: " + smv.GetCoordLangID(i, S, COORD_STORAGE.VARIABLES).ToUpper() + " = " + i + ")");
                }
            }
            else SB.AppendLine("The type is constant.");
            SB.AppendLine("");

            if (smv.NbConstBasisBlade > 0)
            {
                SB.AppendLine("The constant non-zero coordinates are:");
                for (int i = 0; i < smv.NbConstBasisBlade; i++)
                    SB.AppendLine("  - " + smv.ConstBasisBlade(i).ToString(S.m_basisVectorNames) + " = " + smv.ConstBasisBladeValue(i).ToString());
            }
            else SB.AppendLine("The type has no constant coordinates.");
            SB.AppendLine("");

            if ((smv.Comment != null) && (smv.Comment.Length > 0))
            {
                SB.AppendLine("");
                SB.AppendLine("" + smv.Comment);
            }
            return new Comment(SB.ToString());
        } // end of GetSmvComment()



        /// <summary>
        /// Returns the comment for the GOM class.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        /// <param name="gom">The general outermorphism for which the class should be written.</param>
        public static Comment GetGomComment(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GOM gom)
        {
            StringBuilder SB = new StringBuilder();
            SB.AppendLine("This class can hold a general outermorphism.");
            SB.AppendLine();

            SB.AppendLine("The coordinates are stored in type " + FT.type + ".");
            SB.AppendLine();

            SB.AppendLine("There are " + gom.Domain.Length + " matrices, one for each grade.");
            SB.AppendLine("The columns of these matrices are the range of the outermorphism.");
            SB.AppendLine("Matrices are stored in row-major order. So the coordinates of rows are stored contiguously.");
            for (int g = 1; g < gom.Domain.Length; g++) // start at '1' in order to skip scalar grade
            {
                SB.Append("Domain grade " + g + ": ");
                for (int i = 0; i < gom.DomainForGrade(g).Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(gom.DomainForGrade(g)[i].ToString(S.m_basisVectorNames));

                }

                SB.AppendLine(".");
            }
            SB.AppendLine();
            if (!gom.DomainAndRangeAreEqual())
            {
                for (int g = 1; g < gom.Range.Length; g++) // start at '1' in order to skip scalar grade
                {
                    SB.Append("Range grade " + g + ": ");
                    for (int i = 0; i < gom.RangeForGrade(g).Length; i++)
                    {
                        if (i > 0) SB.Append(", ");
                        SB.Append(gom.RangeForGrade(g)[i].ToString(S.m_basisVectorNames));

                    }

                    SB.AppendLine(".");
                }
            }
            else SB.AppendLine("The range and domain are equal.");

            return new Comment(SB.ToString());
        } // end of GetGomComment()


        /// <summary>
        /// Returns the comment for the SOM class.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        /// <param name="som">The general outermorphism for which the class should be written.</param>
        public static Comment GetSomComment(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SOM som)
        {
            StringBuilder SB = new StringBuilder();

            SB.AppendLine("This class can hold a specialized outermorphism.");
            SB.AppendLine();

            SB.AppendLine("The coordinates are stored in type " + FT.type + ".");
            SB.AppendLine();

            SB.AppendLine("There are " + som.Domain.Length + " matrices, one for each grade.");
            SB.AppendLine("The columns of these matrices are the range of the outermorphism.");
            SB.AppendLine("Matrices are stored in row-major order. So the coordinates of rows are stored contiguously.");
            for (int g = 1; g < som.Domain.Length; g++) // start at '1' in order to skip scalar grade
            {
                SB.Append("Domain grade " + g + ": ");
                for (int i = 0; i < som.DomainForGrade(g).Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(som.DomainForGrade(g)[i].ToString(S.m_basisVectorNames));

                }

                SB.AppendLine(".");
            }
            SB.AppendLine();
            if (!som.DomainAndRangeAreEqual())
            {
                for (int g = 1; g < som.Range.Length; g++) // start at '1' in order to skip scalar grade
                {
                    SB.Append("Range grade " + g + ": ");
                    for (int i = 0; i < som.RangeForGrade(g).Length; i++)
                    {
                        if (i > 0) SB.Append(", ");
                        SB.Append(som.RangeForGrade(g)[i].ToString(S.m_basisVectorNames));

                    }

                    SB.AppendLine(".");
                }
            }
            else SB.AppendLine("The range and domain are equal.");

            return new Comment(SB.ToString());
        } // end of GetSomComment()

    } // end of class ClassComments
} // end of namespace G25.CG.Shared 