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
    namespace CG
    {
        namespace Shared
        {
            public class GmvDualParts
            {
                public GmvDualParts(Specification S, G25.CG.Shared.CGdata cgd)
                {
                    m_specification = S;
                    m_cgd = cgd;
                }

                protected Specification m_specification;
                protected G25.CG.Shared.CGdata m_cgd;


                public void WriteGmvDualParts()
                {
                    G25.CG.Shared.DualParts.WriteDualParts(m_specification, m_cgd);
                }


                /// <summary>
                /// Writes any dual function for general multivectors,
                /// based on dual parts code.
                /// </summary>
                /// <param name="S"></param>
                /// <param name="cgd"></param>
                /// <param name="FT"></param>
                /// <param name="M"></param>
                /// <param name="FAI"></param>
                /// <param name="F"></param>
                /// <param name="comment"></param>
                /// <param name="dual">When true, 'dual' is generated, otherwise, 'undual' is generated.</param>
                /// <returns>name of generated function.</returns>
                public static string WriteDualFunction(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.Metric M, 
                    G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F, String comment, bool dual)
                {
                    // setup instructions
                    System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                    int nbTabs = 1;

                    // write this function:
                    String code = G25.CG.Shared.DualParts.GetDualCode(S, cgd, FT, M, FAI, fgs.RETURN_ARG_NAME, dual);

                    // add one instruction (verbatim code)
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, code));

                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(S, FT, F, FAI);

                    // setup return type and argument:
                    string returnTypeName = FT.GetMangledName(S, S.m_GMV.Name);

                    G25.CG.Shared.FuncArgInfo returnArgument = null;
                    if (S.m_outputLanguage == OUTPUT_LANGUAGE.C) 
                        returnArgument = new G25.CG.Shared.FuncArgInfo(S, CF, -1, FT, S.m_GMV.Name, false); // false = compute value

                    // write function
                    bool inline = false; // never inline GMV functions
                    G25.CG.Shared.Functions.WriteFunction(S, cgd, F, inline, returnTypeName, CF.OutputName, returnArgument, FAI, I, comment);

                    return CF.OutputName;
                }

            } // end of class GmvDualParts
        } // end of namespace 'C'
    } // end of namespace CG
} // end of namespace G25
