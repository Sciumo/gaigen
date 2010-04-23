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

namespace G25
{

    /// <summary>
    /// Immutable class which represents operator bindings.
    /// 
    /// </summary>
    public class Operator
    {
        public static Operator Binary(string symbol, string functionName)
        {
            int nbArguments = 2;
            return new Operator(nbArguments, symbol, functionName);
        }

        public static Operator UnaryPrefix(string symbol, string functionName)
        {
            int nbArguments = 1;
            bool isPrefix =true;
            return new Operator(nbArguments, isPrefix, symbol, functionName);
        }

        public static Operator UnaryPostfix(string symbol, string functionName)
        {
            int nbArguments = 1;
            bool isPrefix = false;
            return new Operator(nbArguments, isPrefix, symbol, functionName);
        }


        /// <summary>
        /// Calls the other constructor with <c>isPrefix=false</c>.
        /// </summary>
        public Operator(int nbArguments, string symbol, string functionName)
            : this(nbArguments, false, symbol, functionName)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nbArguments">Number of arguments for the operator definitions (i.e., 1 or 2).</param>
        /// <param name="isPrefix">For unary operator definitions, whether op is prefix or postfix.</param>
        /// <param name="symbol">Symbol for operator definitions (e.g., "+", "-", "*", "++").</param>
        /// <param name="functionName">Function name to bind the operator too.</param>
        public Operator(int nbArguments, bool isPrefix, string symbol, string functionName)
        {
            m_nbArguments = nbArguments;
            m_isPrefix = isPrefix;
            m_symbol = symbol;
            m_functionName = functionName;
        }

        public bool IsBinary()
        {
            return NbArguments == 2;
        }

        public bool IsUnary()
        {
            return NbArguments == 1;
        }

        public bool IsPrefixUnary()
        {
            return IsUnary() && IsPrefix;
        }

        public bool IsPostfixUnary()
        {
            return IsUnary() && (!IsPrefix);
        }

        /// <summary>
        /// For C++, return true when unary and symbol is ++ or --.
        /// </summary>
        /// <returns></returns>
        public bool IsUnaryInPlace()
        {
            return IsUnary() && (Symbol.Equals("++") || Symbol.Equals("--"));
        }

        /// <summary>Number of arguments for the operator definitions (i.e., 1 or 2).</summary>
        public int NbArguments { get { return m_nbArguments; } }
        /// <summary>For unary operator definitions, whether op is prefix or postfix.</summary>
        public bool IsPrefix { get { return m_isPrefix; } }
        /// <summary>Symbol for operator definitions (e.g., "+", "-", "*", "++").</summary>
        public string Symbol { get { return m_symbol; } }
        /// <summary>Function name to bind the operator too.</summary>
        public string FunctionName { get { return m_functionName; } }


        /// <summary>Number of arguments for the operator definitions (i.e., 1 or 2).</summary>
        private int m_nbArguments;
        /// <summary>For unary operator definitions, whether op is prefix or postfix.</summary>
        private bool m_isPrefix;
        /// <summary>Symbol for operator definitions (e.g., "+", "-", "*", "++").</summary>
        private string m_symbol;
        /// <summary>Function name to bind the operator too.</summary>
        private string m_functionName;
    }



} // end of namespace G25
