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
    /// Output language for generated code.
    /// </summary>
    // Define an Enum with FlagsAttribute.
    [FlagsAttribute]
    public enum AccessModifier : short
    {
        AM_protected = 1,
        AM_public = 2,
        AM_internal = 4,
        AM_private = 8
    };

    /// <summary>
    /// Utility class for C# code generation
    /// </summary>
    class Util
    {

        public void WriteOpenClass(StringBuilder SB, Specification S, AccessModifier accessMod, string className, string[] extends, string[] implements)
        {
            if ((accessMod & AccessModifier.AM_protected) != 0) SB.Append("protected ");
            if ((accessMod & AccessModifier.AM_public) != 0) SB.Append("public ");
            if ((accessMod & AccessModifier.AM_internal) != 0) SB.Append("internal ");
            if ((accessMod & AccessModifier.AM_private) != 0) SB.Append("private ");

            SB.Append(className);
            SB.Append(" ");

            SB.Append(GetClassExtendsImplements(S, extends, implements));

        }

        private string GetClassExtendsImplements(Specification S, string[] extends, string[] implements)
            bool extendsOpened = false;
            bool implementsOpened = false;

            if (extends != null)
            {
                foreach (string E in extends)
                {
                    if (!extendsOpened) {
                        if (S
                    }

                    SB.Append(" " + 
                }
            }
        }


        public void WriteCloseClass(StringBuilder SB, Specification S, string className)
        {
        }

    } // end of class Util

} // end of namespace G25.CG.CSharp 
