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
    public class SOM
    {
        /// <summary>
        /// Writes members variables of a SOM class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        /// <param name="som">The general outermorphism for which the class should be written.</param>
        public static void WriteMemberVariables(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SOM som)
        {
            int nbTabs = 1;
            for (int g = 1; g < som.Domain.Length; g++) // start at '1' in order to skip scalar grade
            {
                if (!som.EmptyGrade(g))
                {
                    string comment = "Matrix for grade " + g + "; the size is " + som.DomainForGrade(g).Length + " x " + som.RangeForGrade(g).Length;
                    new G25.CG.Shared.Comment(comment).Write(SB, S, nbTabs);
                    SB.AppendLine(new string('\t', nbTabs) + Keywords.PackageProtectedAccessModifier(S) + " " + FT.type + "[] m_m" + g + " = new " +
                        FT.type + "[" + som.DomainForGrade(g).Length * som.RangeForGrade(g).Length + "];");
                }
            }
        }

        /// <summary>
        /// Writes comments of a SOM class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        /// <param name="som">The general outermorphism for which the class should be written.</param>
        public static void WriteComment(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SOM som)
        {
            G25.CG.Shared.ClassComments.GetSomComment(S, cgd, FT, som).Write(SB, S, 0);
        }

        /// <summary>
        /// Writes constructors of a SOM class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SOM'.</param>
        /// <param name="som">The specialized outermorphism for which the class should be written.</param>
        /// <param name="rangeVectorSMVname">The name of the SMV which can represent a column of the OM.</param>
        public static void WriteConstructors(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SOM som, string rangeVectorSMVname)
        {
            string className = FT.GetMangledName(S, som.Name);
            string gomClassName = (S.m_GOM == null) ? "" : FT.GetMangledName(S, S.m_GOM.Name);
            cgd.m_cog.EmitTemplate(SB, "SOMconstructors", "S=", S, "FT=", FT, "som=", som, "className=", className, "gomClassName=", gomClassName, "rangeVectorSMVname=", rangeVectorSMVname);
        }

        /// <summary>
        /// Writes functions to copy SOMs to GOM.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float type</param>
        public static void WriteGOMtoSOMcopy(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SOM som)
        {
            if (S.m_GOM != null)
                G25.CG.Shared.OMinit.WriteOMtoOMcopy(S, cgd, FT, S.m_GOM, som);
        } // end of WriteGOMtoSOMcopy()




    } // end of class SOM
} // end of namespace G25.CG.CSJ
