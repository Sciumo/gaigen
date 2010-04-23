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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RefGA;

namespace G25
{

    /// <summary>
    /// Superclass for constant values. Currently only constant SMVs are supported.
    /// </summary>
    public class Constant
    {
        public Constant(string name, VariableType type, string comment) {
            m_name = name;
            m_type = type;
            m_comment = (comment == null) ? "" : comment;
        }

        public string Name { get { return m_name; } }
        public VariableType Type { get { return m_type; } }
        public string Comment { get { return m_comment; } }

        protected string m_name;
        protected VariableType m_type;
        protected string m_comment;
    }

    /// <summary>
    /// Constant SMV value.
    /// 
    /// A constant SMV consists of a name, an SMV type, the value and an optional comment.
    /// It results in code being generated for the constant value.
    /// </summary>
    public class ConstantSMV : Constant
    {
        public ConstantSMV(string name, SMV type, List<G25.rsbbp.BasisBlade> value, string comment) : base(name, type, comment) {

            SetValue(value);
        }

        protected void SetValue(List<G25.rsbbp.BasisBlade> value) {
            SMV smv = m_type as SMV;

            // allocate memory for values; coordinates not specified in 'value' are assumed to be 0.
            m_value = new double[smv.NbNonConstBasisBlade];

            if (value == null) value = new List<G25.rsbbp.BasisBlade>();
            foreach (G25.rsbbp.BasisBlade B in value)
            {
                if (!B.IsConstant)
                    throw new Exception("G25.ConstantSMV(): expected only constant values for constant " + m_name + " (basis blade " + B.GetBasisBlade.ToString() + ")");

//                uint bitmap = B.GetBasisBlade.bitmap;
                int elementIdx = smv.GetElementIdx(B.GetBasisBlade);
                if (elementIdx < 0)
                {
                    // This basis blade is not in the type. This is an error unles the value is 0
                    if (B.ConstantValue != 0.0)
                        throw new Exception("G25.ConstantSMV(): constant " + m_name + ", type " + m_type.GetName() + ": cannot represent this basis blade: " + B.GetBasisBlade);
                }

                if (smv.IsCoordinateConstant(elementIdx)) // if constant in SMV: check is values match
                {
                    if (smv.ConstBasisBladeValue(smv.BladeIdxToConstBladeIdx(elementIdx)) != B.ConstantValue)
                        throw new Exception("G25.ConstantSMV(): constant " + m_name + ", type " + m_type.GetName() + ": constant value for basis blade: " + B.GetBasisBlade + " does not match with type.");
                }
                else // if variable coordinate: set the value
                {
                    int nonConstCoordIdx = smv.BladeIdxToNonConstBladeIdx(elementIdx);

                    m_value[nonConstCoordIdx] = B.ConstantValue;
                }
            }
        }

        public double[] Value { get { return m_value; } }


        /// <summary>
        /// The value of each non-constant coordinate of 'm_type'.
        /// </summary>
        protected double[] m_value;

    }


} // end of namespace G25
