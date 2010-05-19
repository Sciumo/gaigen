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
    public class GmvGpParts
    {
        public GmvGpParts(Specification S, G25.CG.Shared.CGdata cgd)
        {
            m_specification = S;
            m_cgd = cgd;
        }

        protected Specification m_specification;
        protected G25.CG.Shared.CGdata m_cgd;

        /// <summary>
        /// Writes little pieces of code which compute the geometric product of general multivectors,
        /// on a group by group basis.
        /// </summary>
        public void WriteGmvGpParts()
        {
            G25.CG.Shared.GPparts.WriteGmvGpParts(m_specification, m_cgd);
        }

        /// <summary>
        /// Writes any geometric product based product for general multivectors,
        /// based on gp parts code.
        /// </summary>
        /// <param name="SB"></param>
        /// <param name="S"></param>
        /// <param name="cgd"></param>
        /// <param name="FT"></param>
        /// <param name="M"></param>
        /// <param name="FAI"></param>
        /// <param name="F"></param>
        /// <param name="comment"></param>
        /// <param name="declOnly"></param>
        /// <param name="productType"></param>
        /// <returns>name of generated function</returns>
        public static string WriteGMVgpFunction(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.Metric M, 
            G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F, 
            String comment, G25.CG.Shared.GPparts.ProductTypes productType)
        {
            // setup instructions
            System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
            int nbTabs = 1;

            // write this function:
            string code = G25.CG.Shared.GPparts.GetGPcode(S, cgd, FT, M, productType, FAI, fgs.RETURN_ARG_NAME);

            // add one instruction (verbatim code)
            I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, code));

            // because of lack of overloading, function names include names of argument types
            G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(S, FT, F, FAI);

            // setup return type and argument:
            string returnTypeName = null;

            G25.CG.Shared.FuncArgInfo returnArgument = null;
            if (productType == G25.CG.Shared.GPparts.ProductTypes.SCALAR_PRODUCT)
            {
                // return scalar
                returnTypeName = FT.type;
            }
            else
            {
                // return GMV (in C, via 'return argument')
                returnTypeName = FT.GetMangledName(S, S.m_GMV.Name);

                if (S.OutputC())
                    returnArgument = new G25.CG.Shared.FuncArgInfo(S, CF, -1, FT, S.m_GMV.Name, false); // false = compute value
            }

            string funcName = CF.OutputName;
            //if (S.OutputC())
              //  funcName = FT.GetMangledName(S, funcName);

            // write function
            bool inline = false; // never inline GMV functions
            bool staticFunc = Functions.OutputStaticFunctions(S);
            G25.CG.Shared.Functions.WriteFunction(S, cgd, F, inline, staticFunc, returnTypeName, funcName, returnArgument, FAI, I, comment);

            return funcName;
        } // end of WriteGMVgpFunction

        /// <summary>
        /// Writes the inverse geometric product for general multivectors (A * versorInverse(B)).
        /// </summary>
        /// <param name="SB"></param>
        /// <param name="S"></param>
        /// <param name="cgd"></param>
        /// <param name="FT"></param>
        /// <param name="M"></param>
        /// <param name="FAI"></param>
        /// <param name="F"></param>
        /// <param name="comment"></param>
        /// <param name="declOnly"></param>
        /// <returns>name of generated function</returns>
        public static string WriteGMVigpFunction(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.Metric M,
            G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F, string comment)
        {
            // setup instructions
            System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
            int nbTabs = 1;

            // write this function:
            string code = G25.CG.Shared.GPparts.GetIGPcode(S, cgd, FT, M, FAI, fgs.RETURN_ARG_NAME);

            // add one instruction (verbatim code)
            I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, code));

            // because of lack of overloading, function names include names of argument types
            G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(S, FT, F, FAI);

            // setup return type and argument:
            string returnTypeName = FT.GetMangledName(S, S.m_GMV.Name); ;
            G25.CG.Shared.FuncArgInfo returnArgument = null;

            // return GMV (in C, via 'return argument')
            if (S.OutputC())
                returnArgument = new G25.CG.Shared.FuncArgInfo(S, CF, -1, FT, S.m_GMV.Name, false); // false = compute value

            string funcName = CF.OutputName;
            //if (S.OutputC())
              //  funcName = FT.GetMangledName(S, funcName);

            // write function
            bool inline = false; // never inline GMV functions
            bool staticFunc = Functions.OutputStaticFunctions(S);
            G25.CG.Shared.Functions.WriteFunction(S, cgd, F, inline, staticFunc, returnTypeName, funcName, returnArgument, FAI, I, comment);

            return funcName;
        } // end of WriteGMVigpFunction

        /// <summary>
        /// Writes any norm function for general multivectors, based on gp parts code.
        /// </summary>
        /// <param name="SB"></param>
        /// <param name="S"></param>
        /// <param name="cgd"></param>
        /// <param name="FT"></param>
        /// <param name="M"></param>
        /// <param name="FAI"></param>
        /// <param name="F"></param>
        /// <param name="comment"></param>
        /// <param name="declOnly"></param>
        /// <param name="squared"></param>
        /// <returns>name of generated function.</returns>
        public static string WriteGMVnormFunction(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.Metric M,
            G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F,
            string comment, bool squared)
        {
            // setup instructions
            System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
            int nbTabs = 1;

            // because of lack of overloading, function names include names of argument types
            G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(S, FT, F, FAI);

            string funcName = CF.OutputName;

            // get return info
            G25.SMV returnType = null;
            String returnTypeName = null;
            G25.CG.Shared.FuncArgInfo returnArgument = null;
            { // try to get scalar type 
                returnType = S.GetScalarSMV();
                if (returnType == null)
                {
                    returnTypeName = FT.type;
                }
                else
                {
                    if (S.OutputC())
                        returnArgument = new G25.CG.Shared.FuncArgInfo(S, CF, -1, FT, returnType.Name, false); // false = compute value
                    returnTypeName = returnType.GetName();
                }
            }

            // write this function:
            string code = G25.CG.Shared.GPparts.GetNormCode(S, cgd, FT, M, squared, FAI, returnType, G25.fgs.RETURN_ARG_NAME);

            // add the verbatim code
            I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, code));

            // write function
            bool inline = false; // never inline GMV functions
            bool staticFunc = Functions.OutputStaticFunctions(S);
            G25.CG.Shared.Functions.WriteFunction(S, cgd, F, inline, staticFunc, returnTypeName, funcName, returnArgument, FAI, I, comment);

            return funcName;
        } // end of WriteGMVnormFunction


        /// <summary>
        /// Writes any versor application function for general multivectors, 
        /// based on norm2 and geometric product.
        /// </summary>
        /// <param name="SB"></param>
        /// <param name="S"></param>
        /// <param name="cgd"></param>
        /// <param name="FT"></param>
        /// <param name="M"></param>
        /// <param name="FAI"></param>
        /// <param name="F"></param>
        /// <param name="comment"></param>
        /// <param name="declOnly"></param>
        /// <param name="productType"></param>
        /// <returns>name of generated function.</returns>
        public static string WriteGMVapplyVersorFunction(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.Metric M,
            G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F,
            String comment, G25.CG.Shared.GPparts.ApplyVersorTypes AVtype)
        {
            // setup instructions
            System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
            int nbTabs = 1;

            // write this function:
            String code = G25.CG.Shared.GPparts.GetVersorApplicationCode(S, cgd, FT, M, AVtype, FAI, fgs.RETURN_ARG_NAME);

            // add one instruction (verbatim code)
            I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, code));

            // because of lack of overloading, function names include names of argument types
            G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(S, FT, F, FAI);

            string funcName = CF.OutputName;

            // setup return type and argument:
            string returnTypeName = FT.GetMangledName(S, S.m_GMV.Name);

            G25.CG.Shared.FuncArgInfo returnArgument = null;
            if (S.OutputC())
                returnArgument = new G25.CG.Shared.FuncArgInfo(S, CF, -1, FT, S.m_GMV.Name, false); // false = compute value

            // write function
            bool inline = false; // never inline GMV functions
            bool staticFunc = Functions.OutputStaticFunctions(S);
            G25.CG.Shared.Functions.WriteFunction(S, cgd, F, inline, staticFunc, returnTypeName, FT.GetMangledName(S, CF.OutputName), returnArgument, FAI, I, comment);

            return funcName;
        }

    } // end of class GmvGpParts



} // end of namespace G25.CG.Shared
