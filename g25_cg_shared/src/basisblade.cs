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

using RefGA.Symbolic;

namespace G25.CG.Shared
{
    /// <summary>
    /// Utility functions for handling RefGA.BasisBlade.
    /// </summary>
    public class BasisBlade
    {

        /// <summary>
        /// Utility function which takes a specialized multivector and returns an array of its variable
        /// RefGA.BasisBlades. I.e., extracts 'smv.NonConstBasisBlade()'
        /// </summary>
        /// <param name="smv">Specialized multivector.</param>
        /// <returns>Array of non-constant basis blades of 'smv'.</returns>
        public static RefGA.BasisBlade[] GetNonConstBladeList(G25.SMV smv)
        {
            RefGA.BasisBlade[] BL = new RefGA.BasisBlade[smv.NbNonConstBasisBlade];

            for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                BL[i] = smv.NonConstBasisBlade(i);

            return BL;
        }


        /// <summary>
        /// Converts a symbolic multivector value to a textual description of the specialized multivector 
        /// type that would be required to store that value. This is used for presenting error messages 
        /// to users when a suitable specialized type cannot be found for a specific multivector value.
        /// </summary>
        /// <param name="S">Specification, used for basis vector names.</param>
        /// <param name="value">The value to describe.</param>
        /// <returns>Textual description of the type that can store 'value'.</returns>
        public static String MultivectorToTypeDescription(G25.Specification S, RefGA.Multivector value)
        {
            StringBuilder SB = new StringBuilder();

            bool appendSpace = false;
            foreach (RefGA.BasisBlade B in value.BasisBlades)
            {
                if (appendSpace) SB.Append(" ");

                SB.Append(new RefGA.BasisBlade(B.bitmap).ToString(S.m_basisVectorNames));
                if (B.symScale == null)
                    SB.Append("=" + B.scale);

                appendSpace = true;
            }
            return SB.ToString();
        }

    } // end of class BasisBlade
} // end of namepace G25.CG.Shared