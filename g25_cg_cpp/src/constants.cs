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


namespace G25
{
    namespace CG
    {
        namespace CPP
        {
            /// <summary>
            /// Handles code generation of constants.
            /// </summary>
            public class Constants
            {
                public static void WriteDeclarations(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
                {
                    // for each float type
                    foreach (G25.FloatType FT in S.m_floatTypes)
                    {
                        // for each som
                        foreach (G25.Constant C in S.m_constant)
                        {
                            WriteDeclaration(SB, S, cgd, FT, C);
                        }
                        SB.AppendLine("");
                    }
                }

                private static void WriteDeclaration(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.Constant C)
                {
                    // extern MANGLED_TYPENAME MANGLED_CONSTANT_NAME;
                    if (C.Comment.Length > 0)
                        SB.AppendLine("/** " + C.Comment + " */");
                    SB.Append("extern ");
                    SB.Append(FT.GetMangledName(S, C.Type.GetName()));
                    SB.Append(" ");
                    SB.Append(FT.GetMangledName(S, C.Name));
                    SB.AppendLine(";");
                }

                public static void WriteDefinitions(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
                {
                    // for each float type
                    foreach (G25.FloatType FT in S.m_floatTypes)
                    {
                        // for each som
                        foreach (G25.Constant C in S.m_constant)
                        {
                            WriteDefinition(SB, S, cgd, FT, C);
                        }
                        SB.AppendLine("");
                    }
                }

                private static void WriteDefinition(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.Constant C)
                {
                    // assume only SMV constants for now
                    G25.SMV smv = C.Type as G25.SMV;
                    ConstantSMV Csmv = C as ConstantSMV;

                    string className = FT.GetMangledName(S, smv.Name);

                    // MANGLED_TYPENAME MANGLED_CONSTANT_NAME = {...}
                    SB.Append(className);
                    SB.Append(" ");
                    SB.Append(FT.GetMangledName(S, C.Name));

                    if (smv.NbNonConstBasisBlade > 0) {
                        // MANGLED_TYPENAME MANGLED_CONSTANT_NAME(...)
                        SB.Append("(" + className + "::" + G25.CG.Shared.SmvUtil.GetCoordinateOrderConstant(S, smv));

                        for (int c = 0; c < smv.NbNonConstBasisBlade; c++)
                        {
                            SB.Append(", ");
                            SB.Append(FT.DoubleToString(S, Csmv.Value[c]));
                        }

                        SB.Append(")");
                    }

                    SB.AppendLine(";");
                }

            } // end of class Constants
        } // end of namespace CPP
    } // end of namespace CG
} // end of namespace G25
