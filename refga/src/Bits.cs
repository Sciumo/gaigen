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

namespace RefGA
{
    /// <summary>
    /// Utility class for manipulating bitmaps
    /// </summary>
    /// <remarks>
    /// Many of these functions adapted from:
    /// Henry S. Warren, Jr.'s Hacker's Delight, (Addison Wesley, 2002)
    /// </remarks>
    public static class Bits
    {
        /// <summary>
        /// Counts number of 1 bits in <paramref name="i"/>
        /// </summary>
        /// <returns>number of 1 bits in <paramref name="i"/></returns>
        public static uint BitCount(uint i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            i = (i + (i >> 4)) & 0x0F0F0F0F;
            i = i + (i >> 8);
            i = i + (i >> 16);
            return i & 0x0000003F;
        }

        /// <param name="i"></param>
        /// <returns>the index [-1 31] of the highest bit that is on in <c>i</c> (-1 is returned if no bit is on at all (i == 0)).</returns>
        public static int HighestOneBit(uint i)
        {
            return 31 - (int)NumberOfLeadingZeroBits(i);
        }

        /// <param name="i"></param>
        /// <returns>the index [0 32] of the lowest bit that is on in <c>i</c> (32 is returned if no bit is on at all (i == 0)).</returns>
        public static int LowestOneBit(uint i)
        {
            return (int)NumberOfTrailingZeroBits(i);
        }


        /// <summary>
        /// Returns the number of 0 bits before the first 1 bit in <c>i</c>.
        /// For example if i = 4 (100 binary), then 29 (31 - 2) is returned.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>the number of 0 bits before the first 1 bit in <c>i</c>.</returns>
        public static uint NumberOfLeadingZeroBits(uint i)
        {
            i = i | (i >> 1);
            i = i | (i >> 2);
            i = i | (i >> 4);
            i = i | (i >> 8);
            i = i | (i >> 16);
            return BitCount(~i);
        }

        /// <summary>
        /// The number of 0 bits after the last 1 bit in <c>i</c>.
        /// For example if i = 4 (100 binary), then 2 is returned.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>the number of 0 bits after the last 1 bit in <c>i</c>.</returns>
        public static uint NumberOfTrailingZeroBits(uint i)
        {
            return BitCount(~i & (i - 1));
        }
    }
}
