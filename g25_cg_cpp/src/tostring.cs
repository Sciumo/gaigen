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
            /// Handles code generation for toString() conversion.
            /// </summary>
            class ToString
            {
                public static string[][] STRING_PARAMETERS = new string[][]{
                    new string[]{"fp", "%2.2f"},
                    new string[]{"start", ""},
                    new string[]{"end", ""},
                    new string[]{"mul", "*"},
                    new string[]{"wedge", "^"},
                    new string[]{"plus", " + "},
                    new string[]{"minus", " - "}
                };


                /// <summary>
                /// Write code for conversion to string.
                /// </summary>
                /// <param name="SB">Where the code goes.</param>
                /// <param name="S">Specification.</param>
                /// <param name="cgd">Code generation data.</param>
                /// <param name="def">Whether to generate code for definition or declaration.</param>
                public static void WriteToString(StringBuilder SB, G25.Specification S, G25.CG.Shared.CGdata cgd, bool def)
                {
                    string TEMPLATE_NAME = (def) ? "toStringSource"  : "toStringHeader";

                    cgd.m_cog.EmitTemplate(SB, TEMPLATE_NAME, "S=", S, "STRING_PARAMETERS=", STRING_PARAMETERS);
                }

            } // end of class ToString
        } // end of namespace CPP
    } // end of namespace CG
} // end of namespace G25

