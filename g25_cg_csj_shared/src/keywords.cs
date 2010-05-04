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
        public static string ProtectedStaticAccessModifier(Specification S)
        {
            return (S.m_outputLanguage == OUTPUT_LANGUAGE.CSHARP)
                ? "static protected internal"
                : "static protected final";
        }

        public static string StringType(Specification S) {
            return (S.m_outputLanguage == OUTPUT_LANGUAGE.CSHARP)
                ? "string"
                : "String";
        }

        public static string GroupBitmapType(Specification S)
        {
            return (S.m_outputLanguage == OUTPUT_LANGUAGE.CSHARP)
                ? "GroupBitmap"
                : "int";
        }

        public static string ConstAccessModifier(Specification S)
        {
            return (S.m_outputLanguage == OUTPUT_LANGUAGE.CSHARP)
            ? "const"
            : "static final";
        }

        public static string ConstArrayAccessModifier(Specification S)
        {
            return (S.m_outputLanguage == OUTPUT_LANGUAGE.CSHARP)
            ? "static readonly"
            : "static final";
        }

        public static string BoolType(Specification S)
        {
            return (S.m_outputLanguage == OUTPUT_LANGUAGE.CSHARP)
            ? "bool"
            : "boolean";
        }


    } // end of class GMV
} // end of namespace G25.CG.CSJ
