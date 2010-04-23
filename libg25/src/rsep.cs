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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace G25
{
    /// <summary>
    /// This class implements a Really Simple Expression Parser. It is used to parse
    /// the simple expressions inside a G25 specification file (such as "e1.e1=1").
    /// It uses G25.rsel to lex the expressions.
    /// 
    /// You have to construct it by passing an array of G25.rsep.Operator which
    /// define the operators which the parser will accept. You can use ' ' (space) as
    /// an operator symbol, but only as a binary operator.
    /// 
    /// The main function is G25.rsep.parse(String) which will parse the String or throw
    /// an Exception. If the String is parsed, the returned result is either a String (a single
    /// identifier) or a G25.rsep.FunctionApplication .
    /// 
    /// The function applications form a simple Abstract Syntax Tree.
    /// For example, if your input string is "a = b + c" then the output would be
    /// assign(a, add(b, c)), given that you'd have the appropriate operator definitions.
    /// 
    /// </summary>
    public class rsep
    {
        /// <summary>
        /// This class represents an operator symbol, the function name it resolves to
        /// and the precedence of the operator. 
        /// 
        /// You have to specify:
        ///   - the symbol (e.g., "+")
        ///   - the name of the function (e.g., "add")
        ///   - precedence (lower = more important, for example the operator  symbol "*" usually gets a lower precedence value than "+").
        ///   - whether the operator is binary (a+b) or unary (a++)
        ///   - if the operator is unary, it can be postfix (a++) or prefix (++a)
        ///   - if the operator is binary, it can be left associative ( "a+b+c" parses as add(add(a, b), c) ) or right associative ( "a=b=c" parses as assign(a, assign(b, c)))
        /// 
        /// 
        /// </summary>
        public class Operator {
            public Operator(String symbol, String functionName, int precedence, bool unary, bool postfix, bool leftAssociative)
            {
                _Symbol = symbol;
                _FunctionName = functionName;
                _Precedence = precedence;
                _Unary = unary;
                _PostFix = postfix;
                _LeftAssociative = leftAssociative;
            }

            private String _Symbol;
            private  String _FunctionName;
            private  int _Precedence;
            private   bool _Unary;
            private  bool _PostFix;
            private  bool _LeftAssociative;


            /// <summary>
            /// The name of the operator
            /// </summary>
            public String Symbol { get { return _Symbol; }}
           /// <summary>
            /// The name of the function
            /// </summary>
            public String FunctionName { get { return _FunctionName; } }
            /// <summary>
            /// The precedence of the operator (lower = more important)
            /// </summary>
            public int Precedence { get { return _Precedence; } }
            /// <summary>
            /// The arity of the operator (unary=true or binary=false)
            /// </summary>
            public bool Unary { get { return _Unary; } }
            /// <summary>
            /// Meaningful for unary operators only: whether the operator is postfix (true) or prefix(false)
            /// </summary>
            public bool Postfix { get { return _PostFix; } }
            /// <summary>
            /// Meaningful for binary operators only: the associativity  (left=true or right=false)
            /// </summary>
            public bool LeftAssociative { get { return _LeftAssociative; } }
        }

        /// <summary>
        /// Used to return parsed results of G25.rsep.Parse
        /// </summary>
        public class FunctionApplication
        {
            /// <summary>
            /// Creates a function application name(arg1)
            /// </summary>
            public FunctionApplication(String name, Object arg1)
            {
                _FunctionName = name;
                _Arguments = new Object[1] {arg1};
            }
            /// <summary>
            /// Creates a function application name(arg1, arg2)
            /// </summary>
            public FunctionApplication(String name, Object arg1, Object arg2)
            {
                _FunctionName = name;
                _Arguments = new Object[2] { arg1, arg2 };
            }

            /// <summary>
            /// Converts to readable string. For example "add(a, b)".
            /// </summary>
            public override String ToString()
            {
                if (NbArguments == 0)
                    return FunctionName + "()";
                else if (NbArguments == 1)
                    return FunctionName + "(" + Arguments[0].ToString() + ")";
                else if (NbArguments == 2)
                    return FunctionName + "(" + Arguments[0].ToString() + ", " + Arguments[1].ToString() + ")";
                else return "invalid function application";
            }

            /// <summary>
            /// The name of the function (for example, "add")
            /// </summary>
            public String FunctionName { get { return _FunctionName; } }
            /// <summary>
            /// The arguments to the function. Arguments may be either Strings or other FunctionApplications.
            /// </summary>
            public Object[] Arguments { get { return _Arguments; } }
            /// <summary>
            /// Returns the number of arguments (1 or 2).
            /// </summary>
            public int NbArguments { get { return (_Arguments == null) ? 0 : _Arguments.Length; } }

            private  String _FunctionName;
            private  Object[] _Arguments;
        }

        /// <summary>
        /// Implementation of System.Collections.IComparer which can only compare
        /// Operators based on precedence. Will throw a null-pointer exception when
        /// compared Objects are not Operators.
        /// </summary>
        public class OperatorPrecedenceComparer : System.Collections.IComparer
        {
            int System.Collections.IComparer.Compare(Object x, Object y)
            {
                if ((x as Operator).Precedence < (y as Operator).Precedence) return -1;
                else if ((x as Operator).Precedence > (y as Operator).Precedence) return 1;
                else return 0;
            }
        }



        /// <summary>
        /// Custom-operator constructor.
        /// </summary>
        /// <param name="operators">Array of operator definitions. Currently, no sanity check is performed on the
        /// operator list, so make sure it is not ambiguous or anything.</param>
        public rsep(Operator[] operators)
        {
            { // copy all (unique) operator names , create lexer
                List<String> opSymbols = new List<String>();
                for (int i = 0; i < operators.Length; i++)
                    if (!opSymbols.Contains(operators[i].Symbol))
                        opSymbols.Add(operators[i].Symbol);
                m_lex = new rsel(opSymbols.ToArray());
            }

            // sort operators based on precedence, store in m_operators
            m_operators = (Operator[])operators.Clone();
            System.Array.Sort(m_operators, new OperatorPrecedenceComparer());
        }

        /// <summary>
        /// Parses 'str' according to the defined operators
        /// </summary>
        /// <param name="str">The string to be parsed</param>
        /// <returns>Either a String (identifier) or a FunctionApplication</returns>
        public Object Parse(String str)
        {
            return Parse(m_lex.Lex(str), str);
        }

        private Object Parse(Object[] lexedStr, String str)
        {
            ArrayList L = new ArrayList(lexedStr.Length);
            // parse all subexpressions; replace the Object[]s with FunctionApplications
            for (int i = 0; i < lexedStr.Length; i++) {
                Object[] lexedSubExprStr = lexedStr[i] as Object[];
                if (lexedSubExprStr != null) // if subexpression, then parse that first:
                    L.Add(Parse(lexedSubExprStr, str));
                else L.Add(lexedStr[i]);
            }


            for (int o = 0; o < m_operators.Length; o++)
            {
                // go from left-to-right or from right-to-left (depending on associativity) through the entire list of lexed objects
                String currentOpSymbol = " " + m_operators[o].Symbol; // lexed operator have a ' ' in front of them

                if (m_operators[o].Symbol == " ")
                    L = InsertSpaceOperators(L);

                int startIdx = (m_operators[o].LeftAssociative ? 0 : L.Count-1);
                int direction = (m_operators[o].LeftAssociative ? +1 : -1);
                for (int l = startIdx; (l >= 0) && (l < L.Count); l += direction)
                {
                    String opSymbol = L[l] as String;
                    // check if operator matches:
                    if ((opSymbol != null) && (opSymbol == currentOpSymbol))
                    {
                        // operator matches, check each possible case
                        if (m_operators[o].Unary)
                        {
                            if (m_operators[o].Postfix)
                            {
                                // postfix unary op
                                if (l < 1) continue; // we cannot match a postfix op if nothing sits before it
                                if (IsIdentifier(L[l - 1]) || IsFunctionApplication(L[l - 1]))
                                {
                                    L[l - 1] = new FunctionApplication(m_operators[o].FunctionName, L[l - 1]);
                                    L.RemoveAt(l);
                                    l -= direction;
                                }
                                else continue;
                            }
                            else
                            {
                                // prefix unary op
                                if (l > L.Count-2) continue; // we cannot match a postfix op if nothing sits after it
                                if (IsIdentifier(L[l + 1]) || IsFunctionApplication(L[l + 1]))
                                {
                                    L[l] = new FunctionApplication(m_operators[o].FunctionName, L[l + 1]);
                                    L.RemoveAt(l+1);
                                    l -= direction;
                                }
                                else continue;
                            }
                        }
                        else
                        {
                            // binary op
                            if ((l < 1) || (l > L.Count - 2)) continue; // we cannot match a binary op if nothing sits left or right
                            if ((IsIdentifier(L[l - 1]) || IsFunctionApplication(L[l - 1])) && 
                                (IsIdentifier(L[l + 1]) || IsFunctionApplication(L[l + 1])))
                            {
                                L[l-1] = new FunctionApplication(m_operators[o].FunctionName, L[l - 1], L[l + 1]);
                                L.RemoveRange(l, 2);

                                if (direction>0)
                                    l -= direction;
                            }
                            else continue;
                        }
                    }
                }
            }

            if (L.Count == 0) return null;
            if (L.Count == 1)
                return L[0];
            else throw new Exception("Error parsing '" + str + "'");
        }

        /// <returns>true if 'o' can represent an operator</returns>
        private bool IsOperator(Object o)
        {
            String str = o as String;
            return ((str != null) && (str.Length > 1) && (str[0] == ' '));
        }

        /// <returns>true if 'o' is an identifier</returns>
        private bool IsIdentifier(Object o)
        {
            String str = o as String;
            return ((str != null) && (str.Length > 0) && (str[0] != ' '));
        }

        /// <returns>true if 'o' is an identifier</returns>
        private bool IsFunctionApplication(Object o)
        {
            FunctionApplication FA = o as FunctionApplication;
            return (FA != null);
        }

        /// <returns>true if there is a binary operator with the symbol ' ' defined in the m_operators array.</returns>
        private bool BinarySpaceOperatorDefined()
        {
            for (int i = 0; i < m_operators.Length; i++)
                if (m_operators[i].Symbol == " ") return true;
            return false;
        }

        /// <summary>
        /// When a binary space operator is defined (see BinarySpaceOperatorDefined() ), inserts
        /// space operator between all symbols which are not.
        /// </summary>
        /// <param name="lexedStr">Output from the G25.rsel (converted to ArrayList)</param>
        /// <returns>'lexedStr', but with space operators inserted, _if_ there is a binary space operator defined</returns>
        private ArrayList InsertSpaceOperators(ArrayList lexedStr)
        {
            if (!BinarySpaceOperatorDefined()) return lexedStr; // only when actually in use
            ArrayList L = new ArrayList();
            for (int i = 0; i < lexedStr.Count; i++)
            {
                // recurse to subexpressions
                //Object[] subExpr = lexedStr[i] as Object[];
                //if (subExpr != null) lexedStr[i] = InsertSpaceOperators(subExpr);

                // add lexed item to new list 
                L.Add(lexedStr[i]);

                // add space operator, if curretn and next objects are not operators
                if ((i < (lexedStr.Count - 1) && (!IsOperator(lexedStr[i])) && (!IsOperator(lexedStr[i + 1]))))
                    L.Add("  "); // note: double space because all operators in the lexed array have an extra space in front
            }
            return L;
        }


        /// <summary>
        /// The lexer used to lex the input strings
        /// </summary>
        private G25.rsel m_lex;

        /// <summary>
        /// User defined operators. Sorted from low to high precedence value.
        /// </summary>
        private Operator[] m_operators;
    }

} // end of namespace G25
