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
    /// The Specification.m_fgs split into converter and functions.
    /// 
    /// For each function, what plugin can implement it (can be null if
    /// no plugin is available).
    /// 
    /// Initialized and used by Functions.WriteFunctions().
    /// The information is re-used by TestSuite.GenerateCode().
    /// </summary>
    public class FunctionGeneratorInfo
    {
        public FunctionGeneratorInfo() { }

        public List<G25.fgs> m_converterFGS = null;
        public List<G25.fgs> m_functionFGS = null;
        /// <summary>
        ///  For each m_functionFGS, the FunctionGenerator that can implement it.
        ///  This can be null when no FunctionGenerator is available for a particular
        ///  FGS.
        /// </summary>
        public G25.CG.Shared.BaseFunctionGenerator[] m_functionGenerators = null;
        public G25.CG.Shared.CGdata[] m_functionCgd = null;
    } // end of class FunctionGeneratorInfo

} // end of namespace G25.CG.Shared
