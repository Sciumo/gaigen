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

namespace G25.CG.Shared
{
    public class PartsCode
    {
        /// <summary>
        /// Generates all code for parts of geometric product, substract, add, negate, dual, and so on.
        /// 
        /// The results go into cgd.m_def and cgd.m_gmvGPpartFuncNames and cgd.m_gmvDualPartFuncNames.
        /// 
        /// Is also called by TestSuite because the declarations are in the source file by default which
        /// makes them unavailable to the testing code.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go into cgd.m_def and cgd.m_gmvGPpartFuncNames and cgd.m_gmvDualPartFuncNames.</param>
        /// <param name="declOnly">When true, only the declarations are generated and written to <c>cgd.m_defSB</c>.</param>
        public static void GeneratePartsCode(Specification S, CG.Shared.CGdata cgd, bool declOnly)
        { // generate all parts of the geometric product (in parallel)
            const int NB_PARTS_CODE = 4;

            // get a temporary cgd for each type of parts code
            CG.Shared.CGdata[] tmpCgd = new G25.CG.Shared.CGdata[NB_PARTS_CODE];
            for (int i = 0; i < NB_PARTS_CODE; i++)
                tmpCgd[i] = new G25.CG.Shared.CGdata(cgd);

            // get parts code generators
            G25.CG.Shared.GmvGpParts p1 = new G25.CG.Shared.GmvGpParts(S, tmpCgd[0]); // [0] = GP
            G25.CG.Shared.GmvCASNparts p2 = new G25.CG.Shared.GmvCASNparts(S, tmpCgd[1]); // [1] = CASN
            G25.CG.Shared.GmvDualParts p3 = new G25.CG.Shared.GmvDualParts(S, tmpCgd[2]); // [2] = DUAL
            G25.CG.Shared.GmvGomParts p4 = new G25.CG.Shared.GmvGomParts(S, tmpCgd[3]); // [3] = GOM X GMV 

            // run threads
            System.Threading.Thread[] T = new System.Threading.Thread[NB_PARTS_CODE];
            T[0] = new Thread(p1.WriteGmvGpParts);
            T[1] = new Thread(p2.WriteGmvCASNparts);
            T[2] = new Thread(p3.WriteGmvDualParts);
            T[3] = new Thread(p4.WriteGmvGomParts);
            G25.CG.Shared.Threads.StartThreadArray(T);
            G25.CG.Shared.Threads.JoinThreadArray(T);

            // merge declarations and definitions go into cgd.m_declSB or cgd.m_defSB, depending on the language
            for (int i = 0; i < NB_PARTS_CODE; i++)
            {
//                        StringBuilder SB = (S.m_outputLanguage == OUTPUT_LANGUAGE.C) ? cgd.m_defSB : cgd.m_declSB;
                StringBuilder SB = cgd.m_defSB;
                SB.Append(tmpCgd[i].m_declSB);
            }
            if (!declOnly) // if only declarations are wanted, don't copy the definitions
                for (int i = 0; i < NB_PARTS_CODE; i++)
                    cgd.m_defSB.Append(tmpCgd[i].m_defSB);

            // copy names of part functions from appropriate temporary cgd
            cgd.m_gmvGPpartFuncNames = tmpCgd[0].m_gmvGPpartFuncNames; // this assumes [0] = GP
            cgd.m_gmvDualPartFuncNames = tmpCgd[2].m_gmvDualPartFuncNames; // this assumes [2] = DUAL
            cgd.m_gmvGomPartFuncNames = tmpCgd[3].m_gmvGomPartFuncNames; // this assumes [3] = GOM
        } // end of GeneratePartsCode()
    } // end of class PartsCode
} // end of namepace G25.CG.Shared
