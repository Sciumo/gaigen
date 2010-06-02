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
    public class Keywords
    {

        public static string PublicAccessModifier(Specification S)
        {
            return (S.OutputJava())
                ? "public final"
                : "public";
        }

        public static string ProtectedStaticAccessModifier(Specification S)
        {
            return (S.OutputCSharp())
                ? "static protected internal"
                : "static protected final";
        }

        public static string PackageProtectedAccessModifier(Specification S)
        {
            return (S.OutputCSharp())
                ? "protected internal"
                : "protected";
        }

        public static string StringType(Specification S)
        {
            return (S.OutputCSharp())
                ? "string"
                : "String";
        }

        public static string GroupBitmapType(Specification S)
        {
            return (S.OutputCSharp())
                ? "GroupBitmap"
                : "int";
        }

        /// <returns>Access modifier for a constant built-in type.</returns>
        public static string ConstAccessModifier(Specification S)
        {
            return (S.OutputCSharp())
            ? "const"
            : "static final";
        }

        /// <returns>Access modifier for a constant class instance.</returns>
        public static string ConstClassInstanceAccessModifier(Specification S)
        {
            return (S.OutputCSharp())
            ? "static readonly"
            : "static final";
        }


        public static string ConstArrayAccessModifier(Specification S)
        {
            return (S.OutputCSharp())
            ? "static readonly"
            : "static final";
        }

    } // end of class Keywords
} // end of namespace G25.CG.CSJ
