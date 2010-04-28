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
using System.Threading;

namespace G25.CG.C
{


    /// <summary>
    /// This interface must be implemented by all classes which can generate implementations
    /// of functions (like gp(), op(), sincosexp()).
    /// 
    /// It allows the class to be asked whether it can implement certain Function Generation Specifications
    /// (G25.fgs). It allows the class to be asked to actually emit the declaration or definition of that
    /// function.
    /// </summary>
    public interface CFunctionGenerator
    {
    } // end of class CFunctionGenerator
        
    /// <summary>
    /// Handles code generation of all converter and algebra functions (G25.fgs).
    /// </summary>
    class Functions
    {
        public static List<G25.CG.Shared.BaseFunctionGenerator> GetFunctionGeneratorPlugins(G25.CG.Shared.CGdata cgd)
        {
            List<G25.CG.Shared.BaseFunctionGenerator> plugins = new List<G25.CG.Shared.BaseFunctionGenerator>();
            { // get only G25.CG.C.FunctionGenerator classes from plugins
                foreach (CodeGeneratorPlugin P in cgd.m_plugins)
                {
                    if (P is CFunctionGenerator) plugins.Add(P as G25.CG.Shared.BaseFunctionGenerator);
                }
            }
            return plugins;
        }
    } // end of class Functions
} // end of namespace G25.CG.C
