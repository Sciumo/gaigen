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
    public class GOM
    {

        /// <summary>
        /// Writes comments of a GOM class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        /// <param name="gom">The general outermorphism for which the class should be written.</param>
        public static void WriteComment(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GOM gom)
        {
            G25.CG.Shared.ClassComments.GetGomComment(S, cgd, FT, gom).Write(SB, S, 0);
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
            int nbTabs = 1;
            for (int g = 1; g < gom.Domain.Length; g++) // start at '1' in order to skip scalar grade
            {
                string comment = "Matrix for grade " + g + "; the size is " + gom.DomainForGrade(g).Length + " x " + gom.RangeForGrade(g).Length;
                new G25.CG.Shared.Comment(comment).Write(SB, S, nbTabs);
                
                SB.AppendLine(new string('\t', nbTabs) + Keywords.PackageProtectedAccessModifier(S) + " " + FT.type + "[] m_m" + g + " = new " +
                    FT.type + "[" + gom.DomainForGrade(g).Length * gom.RangeForGrade(g).Length + "];");
            }
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
        /// Writes a function to set an GOM struct to identity, for all floating point types.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        public static void WriteSetIdentity(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT)
        {
            SB.AppendLine("");

            string omName = "a";
            string matrixName = "m_m"; // todo: centralize this name

            Dictionary<double, List<string>> nonZero = G25.CG.Shared.OMinit.GetGomIdentityInitCode(S, S.m_GOM, omName, matrixName);

            //string className = FT.GetMangledName(S, S.m_GOM.Name);
            string funcName = Util.GetFunctionName(S, "setIdentity");
            SB.AppendLine("\t" + Keywords.PublicAccessModifier(S) + " void " + funcName + "() {");
            for (int g = 1; g < S.m_GOM.Domain.Length; g++)
            {
                int s = S.m_GOM.Domain[g].Length * S.m_GOM.Range[g].Length;
                SB.AppendLine("\t\t" + G25.CG.Shared.Util.GetSetToZeroCode(S, FT, matrixName + g, s));
            } // end of loop over all grades of the OM

            // do the nonZero assignments:
            foreach (KeyValuePair<double, List<string>> kvp in nonZero)
            {
                SB.Append("\t\t");
                int cnt = 0;
                foreach (string coordStr in kvp.Value)
                {
                    SB.Append(coordStr + " = ");
                    cnt++;
                    if ((cnt % 8) == 0)
                        SB.Append("\n\t\t\t");
                }
                SB.AppendLine(FT.DoubleToString(S, kvp.Key) + ";");
            }

            SB.AppendLine("\t}");
        } // end of WriteSetIdentity()

        /// <summary>
        /// Writes a function to copy an GOM class.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        public static void WriteSetCopy(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT)
        {
            SB.AppendLine();

            string srcName = "src";
            string matrixName = "m_m"; // todo: centralize this name

            string className = FT.GetMangledName(S, S.m_GOM.Name);
            string funcName = Util.GetFunctionName(S, "set"); ;
            SB.AppendLine("\tvoid " + funcName + "(" + className + " " + srcName + ") {");
            for (int g = 1; g < S.m_GOM.Domain.Length; g++)
            {
                int s = S.m_GOM.Domain[g].Length * S.m_GOM.Range[g].Length;
                SB.AppendLine("\t\t" + G25.CG.Shared.Util.GetCopyCode(S, FT, srcName + "." + matrixName + g, matrixName + g, s));
            } // end of loop over all grades of the OM
            SB.AppendLine("\t}");
        } // end of WriteCopy()

        /// <summary>
        /// Writes a function to set a GOM class according to vector images.
        /// Output goes into cdg.m_defSB
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GOM'.</param>
        /// <param name="matrixMode">When true, generates code for setting from matrix instead of vector images.</param>
        /// <param name="transpose">When this parameter is true and <c>matrixMode</c> is true, generates code for setting from transpose matrix.</param>
        public static void WriteSetVectorImages(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, bool matrixMode, bool transpose)
        {
            G25.CG.Shared.OMinit.WriteSetVectorImages(S, cgd, FT, matrixMode, transpose);
        } // end of WriteSetVectorImages()

        /// <summary>
        /// Writes functions to copy SOMs to GOM.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float type</param>
        public static void WriteSOMtoGOMcopy(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT)
        {
            G25.CG.Shared.OMinit.WriteSOMtoGOMcopy(S, cgd, FT);
        } // end of WriteSOMtoGOMcopy()

    } // end of class GOM
} // end of namespace G25.CG.CSJ
