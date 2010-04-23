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

    /// <summary>
    /// Used as a container for floating point types with prefix and suffix.
    /// </summary>
    public class FloatType : VariableType
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typename">name of the float type (e.g., "float" or "double").</param>
        /// <param name="prefix">prefix which should be applied to typename (e.g. float_vector)</param>
        /// <param name="suffix">suffix which should be applied to typename (e.g. vector_f)</param>
        public FloatType(String typename, String prefix, String suffix) {
            m_floatType = typename;
            m_floatTypeCastStr = "(" + typename + ")";
            m_floatPrefix = (prefix == null) ? "" : prefix;
            m_floatSuffix = (suffix == null) ? "" : suffix;
        }

  /*      /// <summary>
        /// Adds prefix and suffix to type according to the floating point type used, unless the
        /// String already has the prefix or suffix.
        /// </summary>
        /// <returns>'typeName' mangled according to floating point type 'floatTypeIndex'.</returns>
        public string GetMangledNameOld(string typeName)
        {
            if (typeName.Contains(G25.Specification.DONT_MANGLE)) return typeName;

            // if the name already contains the floating point type, do not add any prefix/suffix (this will cause trouble sometimes when the name is in the typeName unintendedly. . .)
            if (typeName.Contains(type)) return typeName;

            if ((prefix.Length > 0) && (!typeName.StartsWith(prefix)))
                typeName = prefix + typeName;
            if ((suffix.Length > 0) && (!typeName.EndsWith(suffix)))
                typeName = typeName + suffix;

            return typeName;
        }*/

        /// <summary>
        /// Adds prefix and suffix to type according to the floating point type used, unless the
        /// String already has the prefix or suffix.
        /// </summary>
        /// <returns>'typeName' mangled according to floating point type 'floatTypeIndex'.</returns>
        public string GetMangledName(Specification S, string typeName)
        {
            if (typeName.Contains(G25.Specification.DONT_MANGLE) || 
                typeName.Contains(type)) 
                return typeName;

            // This change (for example) "double" -> "float"
            if (S.IsFloatType(typeName))
                return type;

            if ((prefix.Length > 0) && (!typeName.StartsWith(prefix)))
                typeName = prefix + typeName;
            if ((suffix.Length > 0) && (!typeName.EndsWith(suffix)))
                typeName = typeName + suffix;

            return typeName;
        }

        /// <summary>
        /// Converts a floating point value to a string using the convention of the output language.
        /// </summary>
        /// <param name="S">Specification (used for output language).</param>
        /// <param name="value">Floating point value to be converted.</param>
        /// <returns>'value' converted to string in output language.</returns>
        public String DoubleToString(Specification S, double value)
        {
            String str = value.ToString("r"); // "r" stands for round-trip and ensures the value is not lost

            // make sure we always have a .something or Esomething part
            if ((str.IndexOf('.') < 0) && 
                (str.IndexOf('e') < 0) && 
                (str.IndexOf('E') < 0)) str = str + ".0";

            if (type == "float") return str + "f";
            else if (type == "double") return str;
            else return castStr + str;
        }

        /// <returns>Approximate maximum value this floating point type can hold</returns>
        public double MaxValue()
        {
            if (type == "double") return 10e305;
            else return 10e35;
        }

        /// <summary>
        /// Returns true if a cast must be performed when 'FT' is assigned to this floatType.
        /// For example, when assigning a float to a double, not cast needs to be performed because
        /// no precision is lost. The other way around, precision _is_ lost, and a should be performed
        /// to avoid compiler warnings.
        /// 
        /// Always returns true when src/dst float type is unknown.
        /// </summary>
        /// <param name="S">Used for output language.</param>
        /// <param name="FT"></param>
        /// <returns>true if a cast must be performed when 'FT' is assigned to this floatType.</returns>
        public bool MustCastIfAssigned(Specification S, FloatType FT)
        {
            if ((type == "float") && (FT.type == "float")) return false;
            else if ((type == "double") && ((FT.type == "float") || (FT.type == "double"))) return false;
            else return true;
        }

        public double PrecisionEpsilon()
        {
            if (type == "double") return 1e-14;
            else return 1e-6;
        }

        public String type {get{return m_floatType;}}
        public String castStr { get { return m_floatTypeCastStr; } }
        public String prefix { get { return m_floatPrefix; } }
        public String suffix { get { return m_floatSuffix; } }

        public virtual VARIABLE_TYPE GetVariableType() {return VARIABLE_TYPE.SCALAR;}

        public virtual String GetName() { return m_floatType; }

        /// <summary>The floating point type of the generated code (e.g., "float" or "double").</summary>
        protected String m_floatType;
        /// <summary>(type)</summary>
        protected String m_floatTypeCastStr;
        /// <summary>The prefix for floating point type(s) in the generated code (e.g., "f_" or "d_" or empty string "").</summary>
        protected String m_floatPrefix;
        /// <summary>The suffix for floating point type(s) in the generated code (e.g., "_f" or "_double" or empty string "").</summary>
        protected String m_floatSuffix;
 
    }
} // end of namespace G25
