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
    /// Contains utility functions for dealing with arrays of threads.
    /// </summary>
    public class Threads
    {

        /// <summary>
        /// Starts all threads in <c>T</c> (a check for null threads is made).
        /// </summary>
        /// <param name="T">Array of threads.</param>
        public static void StartThreadArray(System.Threading.Thread[] T)
        {
            for (int t = 0; t < T.Length; t++)
            {
                if (T[t] != null)
                    T[t].Start();
            }
        }

        /// <summary>
        /// Joins all threads in 'T' (a check for null threads is made).
        /// </summary>
        /// <param name="T">Array of threads.</param>
        public static void JoinThreadArray(System.Threading.Thread[] T)
        {
            for (int t = 0; t < T.Length; t++)
            {
                if (T[t] != null)
                    T[t].Join();
            }
        }

        /// <summary>
        /// Starts and joins all threads in 'T' one after another (this useful for debugging).
        /// </summary>
        /// <param name="T">Array of threads.</param>
        public static void RunThreadArraySerially(System.Threading.Thread[] T)
        {
            System.Console.WriteLine("Warning: use of RunThreadArraySerially()");
            for (int t = 0; t < T.Length; t++)
            {
                if (T[t] != null)
                {
                    T[t].Start();
                    T[t].Join();
                }
            }
        }

    } // end of class Threads
} // end of namepace G25.CG.Shared