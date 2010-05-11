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
    public class SMV
    {

        /// <summary>
        /// Writes the SMV class to 'SB' (including comments).
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Not used yet.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="smv">The specialized multivector for which the struct should be written.</param>
        public static void WriteMemberVariables(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv)
        {
            int nbTabs = 1;
            string accessModifier = Keywords.PackageProtectedAccessModifier(S);

            // individual coordinates or one array?
            if (S.m_coordStorage == COORD_STORAGE.VARIABLES)
            {
                for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                {
                    string comment = "The " + smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames) + " coordinate.";
                    G25.CG.Shared.Util.WriteFunctionComment(SB, S, nbTabs, comment, null, null);
                    SB.AppendLine("\t" + accessModifier + " " + FT.type + " m_" + smv.GetCoordLangID(i, S) + ";");
                }
            }
            else
            {
                if (smv.NbNonConstBasisBlade > 0)
                {
                    // emit: float c[3]; // e1, e2, e3
                    string comment = " The coordinates (stored in an array).";
                    G25.CG.Shared.Util.WriteFunctionComment(SB, S, nbTabs, comment, null, null);

                    SB.Append("\t" + accessModifier + " " + FT.type + "[] m_c = new " +  FT.type + "[" + smv.NbNonConstBasisBlade + "]; // ");
                    for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                    {
                        if (i > 0) SB.Append(", ");
                        SB.Append(smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames));
                    }
                    SB.AppendLine("");
                }
            }
        }


        /// <summary>
        /// Returns a symbolic name for the coordinate 'idx' of 'smv' which can be used in the
        /// output code.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="smv">The specialized multivector for which the coordinate indices should be written.</param>
        /// <param name="idx">The specialized multivector for which the coordinate indices should be written.</param>
        /// <param name="FT"></param>
        /// <returns>The symbol for the define for coordinate index 'idx' of 'smv'.</returns>
        public static string GetCoordIndexDefine(Specification S, FloatType FT, G25.SMV smv, int idx)
        {
            return smv.GetCoordLangID(idx, S, COORD_STORAGE.VARIABLES).ToUpper();
        }

        /// <summary>
        /// Writes the <c>defines</c> for indices of the smv struct to 'SB'. For example,  <c>define VECTOR_E1 0</c>.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names.</param>
        /// <param name="FT"></param>
        /// <param name="smv">The specialized multivector for which the coordinate indices should be written.</param>
        public static void WriteSMVcoordIndices(StringBuilder SB, Specification S, FloatType FT, G25.SMV smv)
        {
            string className = FT.GetMangledName(S, smv.Name);

            int nbTabs = 1;
            G25.CG.Shared.Util.WriteFunctionComment(SB, S, nbTabs, "Array indices of " + className + " coordinates.", null, null);


            string AccessModifier = Keywords.ConstAccessModifier(S);

            for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
            {
                SB.AppendLine();
                G25.CG.Shared.Util.WriteFunctionComment(SB, S, nbTabs, "index of coordinate for " + smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames) + " in " + className, null, null);
                SB.AppendLine("\tpublic " + AccessModifier + " int " + GetCoordIndexDefine(S, FT, smv, i) + " = " + i + ";");
            }


        }

        /// <summary>
        /// Writes the <c>defines</c> for indices of the smv struct to 'SB'. For example,  <c>define VECTOR_E1 0</c>.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names.</param>
        /// <param name="FT"></param>
        /// <param name="smv">The specialized multivector for which the coordinate indices should be written.</param>
        public static void WriteCoordinateOrder(StringBuilder SB, Specification S, FloatType FT, G25.SMV smv)
        {

            string AccessModifier = Keywords.ConstAccessModifier(S);
            string typeName = G25.CG.Shared.SmvUtil.COORDINATE_ORDER_ENUM;
            string constantName = G25.CG.Shared.SmvUtil.GetCoordinateOrderConstant(S, smv);

            SB.AppendLine();

            int nbTabs = 1;
            G25.CG.Shared.Util.WriteFunctionComment(SB, S, nbTabs, "The order of coordinates (this is the type of the first argument of coordinate-handling functions.", null, null);

            string enumAccessModifier = (S.m_outputLanguage == OUTPUT_LANGUAGE.CSHARP) ? "public" : "private";

            SB.AppendLine("\t" + enumAccessModifier + " enum " + typeName + " {");
            SB.AppendLine("\t\t" + constantName);
            SB.AppendLine("\t};");
            SB.AppendLine("\tpublic " + AccessModifier + " " + typeName + " " + constantName + " = " + typeName + "." + constantName + ";");


        }

        /// <summary>
        /// Writes the function to get the array of coordinates.
        /// Does nothing when coordinate storage is not array based or when the type has no variable coordinates.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names.</param>
        /// <param name="FT"></param>
        /// <param name="smv">The specialized multivector for which the coordinate indices should be written.</param>
        public static void WriteGetCoordinates(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv)
        {
            if (S.m_coordStorage != COORD_STORAGE.ARRAY) return;
            if (smv.NbNonConstBasisBlade == 0) return;

            //string typeName = G25.CG.Shared.SmvUtil.COORDINATE_ORDER_ENUM;
            string constantName = G25.CG.Shared.SmvUtil.GetCoordinateOrderConstant(S, smv);

            cgd.m_cog.EmitTemplate(SB, "SMVgetCoords", "FT=", FT, "COORD_TYPE_STRING=", constantName);
        }
            

#if RIEN
        /// <summary>
        /// Writes the SMV class comment 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Not used yet.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="smv">The specialized multivector for which the struct should be written.</param>
        /// <param name="emitCoordIndices">Whether to emit constants for array indices to coordinates.</param>
        public static void WriteComment(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv, bool emitCoordIndices)
        {
            SB.AppendLine("/**");
            SB.AppendLine(" * This class can hold a specialized multivector of type " + FT.GetMangledName(S, smv.Name) + ".");
            SB.AppendLine(" * ");

            SB.AppendLine(" * The coordinates are stored in type  " + FT.type + ".");
            SB.AppendLine(" * ");

            if (smv.NbNonConstBasisBlade > 0)
            {
                SB.AppendLine(" * The variable non-zero coordinates are:");
                for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                {
                    SB.AppendLine(" *   - coordinate " + smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames) + "  (array index: " + GetCoordIndexDefine(S, FT, smv, i) + " = " + i + ")");
                }
            }
            else SB.AppendLine(" * The type is constant.");
            SB.AppendLine(" * ");

            if (smv.NbConstBasisBlade > 0)
            {
                SB.AppendLine(" * The constant non-zero coordinates are:");
                for (int i = 0; i < smv.NbConstBasisBlade; i++)
                    SB.AppendLine(" *   - " + smv.ConstBasisBlade(i).ToString(S.m_basisVectorNames) + " = " + smv.ConstBasisBladeValue(i).ToString());
            }
            else SB.AppendLine(" * The type has no constant coordinates.");
            SB.AppendLine(" * ");

            if ((smv.Comment != null) && (smv.Comment.Length > 0))
            {
                SB.AppendLine(" * ");
                SB.AppendLine(" * " + smv.Comment);
            }
            SB.AppendLine(" */");
        }
#endif


    } // end of class SMV
} // end of namespace G25.CG.CSJ
