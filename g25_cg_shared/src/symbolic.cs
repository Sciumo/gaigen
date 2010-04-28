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

namespace G25.CG.Shared
{
    
    /// <summary>
    /// Contains functions for converting a G25.VariableType (scalar, specialized or general multivector)
    /// to a RefGA.Multivector with symbolic coordinates.
    /// </summary>
    public class Symbolic
    {

        /// <summary>
        /// Takes a specialized multivector specification (G25.SMV) and converts it into a symbolic multivector.
        /// The symbolic weights of the multivector are the coordinates of the SMV, labelled according to 'smvName'.
        /// 
        /// An example is <c>A.c[0]*e1 + A.c[1]*e2 + A.c[2]*e3</c>.
        /// </summary>
        /// <param name="S">Specification of the algebra. Used for the access convention (. or ->) and for how to name the coordinates ([0] or e1, e2, e3).</param>
        /// <param name="smv">The specification of the specialized multivector.</param>
        /// <param name="smvName">Name the variable should have.</param>
        /// <param name="ptr">Is 'smvName' a pointer? This changes the way the variable is accessed.</param>
        /// <returns></returns>
        public static RefGA.Multivector SMVtoSymbolicMultivector(Specification S, G25.SMV smv, string smvName, bool ptr)
        {
            int idx = 0; // index into 'L'
            RefGA.BasisBlade[] L = new RefGA.BasisBlade[smv.NbConstBasisBlade + smv.NbNonConstBasisBlade];


            string[] AL = CodeUtil.GetAccessStr(S, smv, smvName, ptr);
            // get non-const coords
            for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
            {
                // get basis blade of coordinate
                RefGA.BasisBlade B = smv.NonConstBasisBlade(i);
                    
                // merge
                L[idx++] = new RefGA.BasisBlade(B.bitmap, B.scale, AL[i]);
            }
            
            // get const coords
            for (int i = 0; i < smv.NbConstBasisBlade; i++)
            {
                // get value of coordinate
                double value = smv.ConstBasisBladeValue(i);

                // get basis blade of coordinate
                RefGA.BasisBlade B = smv.ConstBasisBlade(i);
                
                // merge
                L[idx++] = new RefGA.BasisBlade(B.bitmap, B.scale * value);
            }

            return new RefGA.Multivector(L);
        } // end of SMVtoSymbolicMultivector()


        /// <summary>
        /// Returns a symbolic multivector with a symbolic scalar value 1.0 * 'name'.
        /// </summary>
        /// <param name="S">The specification of the algebra (not used currently).</param>
        /// <param name="FT">Floating point type of scalar (not used currently).</param>
        /// <param name="name">Name of scalar.</param>
        /// <returns>a symbolic multivector with value 1.0 * 'name' * scalar.</returns>
        public static RefGA.Multivector ScalarToSymbolicMultivector(Specification S, G25.FloatType FT, String name)
        {
            return new RefGA.Multivector(new RefGA.BasisBlade(0, 1.0, name));
        }


        /// <summary>
        /// Takes a general multivector specification (G25.GMV) and converts it into a one symbolic multivector per group/grade part.
        /// 
        /// The symbolic weights of the multivector are the coordinates of the GMV, labelled according to 'gmvName'.
        /// Currently, the indices start at zero for each group.
        /// </summary>
        /// <param name="S">Specification of the algebra. Used for the access convention (. or ->) and for how to name the coordinates ([0] or e1, e2, e3).</param>
        /// <param name="gmv">The specification of the general multivector.</param>
        /// <param name="gmvName">Name the variable should have.</param>
        /// <param name="ptr">Is 'gmvName' a pointer? (not used currently)</param>
        /// <param name="groupIdx">Index of group/grade to convert (use -1 for all groups)</param>
        /// <returns></returns>
        public static RefGA.Multivector[] GMVtoSymbolicMultivector(Specification S, G25.GMV gmv, String gmvName, bool ptr, int groupIdx)
        {
            RefGA.Multivector[] R = new RefGA.Multivector[gmv.NbGroups];

            //String accessStr = (ptr) ? "->" : ".";

            for (int g = 0; g < gmv.NbGroups; g++) {
                if ((groupIdx >= 0) && (g != groupIdx)) continue; // only one group requested?

                RefGA.BasisBlade[] B = gmv.Group(g);
                RefGA.BasisBlade[] L = new RefGA.BasisBlade[B.Length];
                for (int i = 0; i < B.Length; i++) 
                {
                    RefGA.BasisBlade b = B[i];
                    String fullCoordName = gmvName + "[" + i + "]";
                    // merge
                    L[i] = new RefGA.BasisBlade(b.bitmap, b.scale, fullCoordName);
                }
                R[g] = new RefGA.Multivector(L);
            }

            return R;
        } // end of function GMVtoSymbolicMultivector()

    } // end of class Symbolic
} // end of namepace G25.CG.Shared
