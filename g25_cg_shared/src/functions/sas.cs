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

namespace G25.CG.Shared.Func
{
    /// <summary>
    /// Generates code for 'scale and add scalar' (sas) function.
    /// 
    /// This function computes ms * a + as where
    /// ms: multiplying scalar
    /// a: general multivector
    /// as: adding scale.
    /// 
    /// It is used directly by exp(), sin(), cos() functions for general multivectors, but possible
    /// it also has uses for the end user.
    /// 
    /// The function name should be <c>"sas"</c>.
    /// The first argument should be a MV, the other two arguments scalars.
    /// </summary>
    public class SAS : G25.CG.Shared.BaseFunctionGenerator
    {

        // constants, intermediate results
        protected const int NB_ARGS = 3;
        protected bool m_gmvFunc; ///< is this a function over GMVs?
        protected RefGA.Multivector m_returnValue; ///< returned value (symbolic multivector)

        /// <summary>
        /// Checks if this FunctionGenerator can implement a certain function.
        /// </summary>
        /// <param name="S">The specification of the algebra.</param>
        /// <param name="F">The function to be implemented.</param>
        /// <returns>true if 'F' can be implemented</returns>
        public override bool CanImplement(Specification S, G25.fgs F)
        {
            String arg1Type = F.GetArgumentTypeName(0, S.m_GMV.Name);
            return ((F.Name == "sas") && (F.MatchNbArguments(3) &&
                (S.IsSpecializedMultivectorName(arg1Type) || (arg1Type == S.m_GMV.Name)) &&
                S.IsFloatType(F.GetArgumentTypeName(1, S.m_GMV.Name)) &&
                S.IsFloatType(F.GetArgumentTypeName(2, S.m_GMV.Name))));
        }

        /// <summary>
        /// If this FunctionGenerator can implement 'F', then this function should complete the (possible)
        /// blanks in 'F'. This means:
        ///  - Fill in F.m_returnTypeName if it is empty
        ///  - Fill in F.m_argumentTypeNames (and m_argumentVariableNames) if it is empty.
        /// </summary>
        public override void CompleteFGS()
        {
            // get all function info
            FloatType FT = m_specification.GetFloatType(m_fgs.FloatNames[0]);

            // fill in ArgumentTypeNames
            if (m_fgs.ArgumentTypeNames.Length == 0)
                m_fgs.m_argumentTypeNames = new String[] { m_gmv.Name, FT.type, FT.type };

            // init argument pointers from the completed typenames (language sensitive);
            m_fgs.InitArgumentPtrFromTypeNames(m_specification);

            bool computeMultivectorValue = true;
            G25.CG.Shared.FuncArgInfo[] tmpFAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

            m_gmvFunc = !(tmpFAI[0].IsScalarOrSMV());

            // compute intermediate results, set return type
            if (m_gmvFunc) m_fgs.m_returnTypeName = m_gmv.Name; // gmv * scalar + scalar = gmv
            else
            {
                // compute return value
                m_returnValue = RefGA.Multivector.Add(
                        RefGA.Multivector.gp(tmpFAI[0].MultivectorValue[0], tmpFAI[1].MultivectorValue[0]),
                        tmpFAI[2].MultivectorValue[0]);

                // get name of return type
                if (m_fgs.m_returnTypeName.Length == 0)
                    m_fgs.m_returnTypeName = G25.CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, m_returnValue).GetName();
            }
        }

        /// <summary>
        /// Should write the declaration/definitions of 'F' to StringBuffer 'SB', taking into account parameters specified in specification 'S'.
        /// </summary>
        public override void WriteFunction()
        {
            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                bool computeMultivectorValue = true;
                G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_gmv.Name, computeMultivectorValue);

                // comment
                Comment comment = new Comment(
                    m_fgs.AddUserComment("Returns " + FAI[1].TypeName + " " + FAI[1].Name + " * " +
                    FAI[0].TypeName + " " + FAI[0].Name + " + " +
                    FAI[2].TypeName + " " + FAI[2].Name + "."));


                // if scalar or specialized: generate specialized function
                if (m_gmvFunc)
                {
                    G25.CG.Shared.GmvCASNparts.WriteSASfunction(m_specification, m_cgd, FT, FAI, m_fgs, comment);
                }
                else
                {// write specialized function:
                    // setup instructions
                    System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                    {
                        int nbTabs = 1;
                        bool mustCast = false;
                        G25.VariableType returnType = G25.CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, m_returnValue);
                        I.Add(new G25.CG.Shared.ReturnInstruction(nbTabs, returnType, FT, mustCast, m_returnValue));
                    }

                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, m_fgs, FAI);

                    bool staticFunc = Functions.OutputStaticFunctions(m_specification);
                    G25.CG.Shared.Functions.WriteFunction(m_specification, m_cgd, CF, m_specification.m_inlineFunctions, staticFunc, CF.OutputName, FAI, I, comment);
                }
            }
        } // end of WriteFunction


        // Testing is done via sin/cos/exp.


    } // end of class SAS
} // end of namespace G25.CG.Shared.Func
