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
    /// Makes info about a function argument of a G25.fgs more easily accessible.
    /// 
    /// Use GetAllFuncArgInfo() to get an array of info. Each entry contains the type, typename, etc
    /// of the respective argument to the function.
    /// </summary>
    public class FuncArgInfo
    {
        /// <summary>
        /// Constructs a new FuncArgInfo class for a specific argument 'argIdx' of function 'F'.
        /// </summary>
        /// <param name="S">Used for retrieving the G25.VariableType of 'm_typeName'.</param>
        /// <param name="F">Function for which this FuncArgInfo describes an argument.</param>
        /// <param name="argIdx">Index of argument. Use -1 for artificial 'return argument' used for the C language.</param>
        /// <param name="FT">Floating point type of the type of the argument.</param>
        /// <param name="defaultTypeName">Name of the type of the argument.</param>
        /// <param name="computeMultivectorValue">Set to true to convert the type into symbolic code. Uses 'F' to obtain the actual name of the variable to use inside the symbolic multivector.</param>
        public FuncArgInfo(G25.Specification S, G25.fgs F, int argIdx, G25.FloatType FT, string defaultTypeName, bool computeMultivectorValue)
        {
            m_name = F.GetArgumentName(argIdx);
            m_typeName = F.GetArgumentTypeName(argIdx, defaultTypeName);
            m_type = S.GetType(m_typeName);
            m_varType = m_type.GetVariableType();
            if (m_varType != VARIABLE_TYPE.SCALAR) m_floatType = FT;
            else m_floatType = S.GetFloatType(m_typeName);

            // set mangled type name (depends on whether type is scalar or not)
            if ((m_varType == VARIABLE_TYPE.SCALAR) || (m_varType == VARIABLE_TYPE.ENUM)) {
                m_mangledTypeName = m_typeName;
            }
            else {
                m_mangledTypeName = FT.GetMangledName(S, m_typeName);
            }

            // set pointer / non pointer flag
            m_pointer = F.GetArgumentPtr(S, argIdx);

            // set array  flag
            m_array = F.GetArgumentArr(S, argIdx);

            m_constant = (argIdx >= 0);
            

            if (computeMultivectorValue) {
                if (m_varType == VARIABLE_TYPE.SMV)
                {
                    m_multivectorValue = new RefGA.Multivector[1] { Symbolic.SMVtoSymbolicMultivector(S, (G25.SMV)m_type, m_name, m_pointer) };
                }
                else if (m_varType == VARIABLE_TYPE.GMV)
                    m_multivectorValue = Symbolic.GMVtoSymbolicMultivector(S, (G25.GMV)m_type, m_name, m_pointer, -1); // -1 = sym mv for all groups
                else if (m_varType == VARIABLE_TYPE.SCALAR)
                    m_multivectorValue = new RefGA.Multivector[1] { Symbolic.ScalarToSymbolicMultivector(S, (G25.FloatType)m_type, m_name) };
                else
                {
                    // OM: do nothing?
                    m_multivectorValue = null;
                }
            }
        }

        /// <summary>
        /// Constructs an array of func arg info for all arguments of an G25.fgs.
        /// </summary>
        /// <param name="S">Used for retrieving the G25.VariableType of 'm_typeName'.</param>
        /// <param name="F">Function for which you want an array of FuncArgInfo.</param>
        /// <param name="nbArgs">Default number of arguments. (the user may not specified arguments and then a default is used).</param>
        /// <param name="FT">Floating point type of arguments.</param>
        /// <param name="defaultTypeName">When the user does not specify arguments, this is the default type used.</param>
        /// <param name="computeMultivectorValue">Set to true to expand all multivector values to symbolic RefGA.Multivectors.</param>
        /// <returns>Array of FuncArgInfo describing the arguments of 'F'.</returns>
        public static FuncArgInfo[] GetAllFuncArgInfo(G25.Specification S, G25.fgs F, int nbArgs, G25.FloatType FT, String defaultTypeName, bool computeMultivectorValue)
        {
            FuncArgInfo[] FAI = new FuncArgInfo[nbArgs];
            for (int i = 0; i < nbArgs; i++)
            {
                FAI[i] = new FuncArgInfo(S, F, i, FT, defaultTypeName, computeMultivectorValue);
            }
            return FAI;
        }

        public bool IsGMV()
        {
            return (m_varType == VARIABLE_TYPE.GMV);
        }

        /// <returns>true when this is a scalar or specialized multivector.</returns>
        public bool IsScalarOrSMV()
        {
            return (m_varType == VARIABLE_TYPE.SCALAR) || (m_varType == VARIABLE_TYPE.SMV);
        }

        /// <returns>true when this is a scalar or specialized multivector.</returns>
        public bool IsMVorOM()
        {
            return (m_varType == VARIABLE_TYPE.GMV) ||
                (m_varType == VARIABLE_TYPE.SMV) ||
                (m_varType == VARIABLE_TYPE.GOM) ||
                (m_varType == VARIABLE_TYPE.SOM);
        }

        /// <returns>true when this is a scalar.</returns>
        public bool IsScalar()
        {
            return (m_varType == VARIABLE_TYPE.SCALAR);
        }

        /// <summary>Name of argument (e.g., "A").</summary>
        public string Name { get { return m_name; } }
        /// <summary>Name of type, e.g., "float" or "vectorE3GA".</summary>
        public string TypeName { get { return m_typeName; } }
        /// <summary>Mangled name of type, e.g., "float" or "vectorE3GA_f".</summary>
        public string MangledTypeName { get { return m_mangledTypeName; } }

        /// <summary>G25.FloatType, G25.OM or G25.MV.</summary>
        public G25.VariableType Type { get { return m_type; } }

        /// <summary>If m_type is not a G25.FloatType, then this is the floating point type used for m_type. 
        /// Otherwise it is equal to Type.</summary>
        public FloatType FloatType { get { return m_floatType; } }

        /// <summary>Whether this is a float or an SMV, or a GMV, etc.</summary>
        public G25.VARIABLE_TYPE VarType { get { return m_varType; } }

        /// <summary>Symbolic multivector value (based on m_name and access (pointer or ref/value). Can be null.</summary>
        public RefGA.Multivector[] MultivectorValue { get { return m_multivectorValue; } }

        /// <summary>Set to true when variable is constant.</summary>
        public bool Constant { get { return m_constant; } }

        /// <summary>True when variable is a pointer.</summary>
        public bool Pointer { get { return m_pointer; } }

        /// <summary>True when variable is a array.</summary>
        public bool Array { get { return m_array; } }

        /// <summary>Name of argument (e.g., "A").</summary>
        protected string m_name;

        /// <summary>Name of type, e.g., "float" or "vectorE3GA".</summary>
        protected string m_typeName;

        /// <summary>Mangled version of m_typeName, e.g., "float" or "vectorE3GA_f".</summary>
        protected String m_mangledTypeName;

        /// <summary>G25.FloatType, G25.OM or G25.MV.</summary>
        protected G25.VariableType m_type;

        /// <summary>If m_type is not a G25.FloatType, then this is the floating point type used for m_type.</summary>
        protected FloatType m_floatType;

        /// <summary>Whether this is a float or an SMV, or a GMV, etc.</summary>
        protected G25.VARIABLE_TYPE m_varType;

        /// <summary>
        /// Symbolic multivector value (based on m_name and access (pointer or ref/value). 
        /// Split per grade/group (so for SMVs, only the first entry is valid).
        /// Can be null.
        /// </summary>
        protected RefGA.Multivector[] m_multivectorValue;

        /// <summary>
        /// Set to true when variable is constant.
        /// </summary>
        protected bool m_constant;

        /// <summary>
        /// True when variable is a pointer.
        /// </summary>
        protected bool m_pointer;

        /// <summary>
        /// True when variable is an array.
        /// </summary>
        protected bool m_array;

    }// end of class FuncArgInfo
} // end of namepace G25.CG.Shared
