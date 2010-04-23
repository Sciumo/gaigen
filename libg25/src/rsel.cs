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
    /// <summary>
    /// This class implements a Really Simple Expression Lexer. It is used to lex
    /// the simple expressions inside a G25 specification file (such as "e1.e1=1").
    /// It is not a pure lexer because it recognizes matching opening and closing parentheses.
    /// 
    /// The Lex() function chops up strings into arrays of numbers, identifiers, operators and subexpressions 
    /// (expressions delimited by '(' and ')').
    /// 
    /// The Lex() function returns an array of Objects.
    /// Each Object in the array is either a String or another array of Objects.
    /// If it is a String, it is either an operator, a number or an identifier. The difference is
    /// easy to tell: operators String start with a phony whitespace, followed by the operator
    /// character(s). Numbers start with a number, and the rest is identifiers.
    /// If the Object is another array of Objects, it is a subexpression, i.e., it was
    /// between () parentheses in the input String.
    /// 
    /// All whitespace and control chars (i.e., chars with value below ' ') are ignored.
    /// 
    /// An identifier is a string starting with a letter or '_', and is continued with letters,
    /// digits and/or '_'.
    /// 
    /// Operators are searched for before identifiers, so in you can use identifiers as operators
    /// if you wanted to (but they need to be trailed by non-identifier characters for them
    /// to be found).
    /// 
    /// Operators are searched before numbers, so if a number starting with '.' it is not recognized
    /// as a number when '.' is an operator.
    /// 
    /// The default constructor constructs an instance which uses 
    ///  "*", "-", "+", "^", "%", ".", "&", "|", "~", "!", "++", "--" as operator symbols.
    /// 
    /// The one argument constructor allows you to specify an array of operator symbols.
    /// </summary>
    public class rsel
    {
        /// <summary>
        /// Operator strings used by default constructor.
        /// </summary>
        private static String[] m_defaultOps = new String[]{
            "*", "-", "+", "^", "%", ".", "&", "|", "~", "!", "++", "--"
        };

        /// <summary>
        /// Default constructor, creates instance which uses default operator symbols.
        /// </summary>
        public rsel()
        {
            SetOperator(m_defaultOps);
        }

        /// <summary>
        /// Custom-operator constructor.
        /// </summary>
        /// <param name="operators">The strings you want to use as operators.</param>
        public rsel(String[] operators)
        {
            SetOperator(operators);
        }

        /// <summary>
        /// Called by constructors to set operators. Sets m_operators and m_longestOperatorLength
        /// </summary>
        private void SetOperator(String[] operators)
        {
            // alloc mem for operator strings
            m_operators = new String[operators.Length];

            // find length of longest operator string
            m_longestOperatorLength = 0;
            for (int i = 0; i < operators.Length; i++) {
                if (operators[i].Length > m_longestOperatorLength)
                    m_longestOperatorLength = operators[i].Length;
            }

            // copy operator string to m_operators, the longest ops first
            int idx = 0;
            for (int l = m_longestOperatorLength; l >= 0; l--) {
                for (int i = 0; i < operators.Length; i++) {
                    if (operators[i].Length == l) 
                        m_operators[idx++] = operators[i];
                }
            }
        }

        /// <returns>true if 'c' is the start of an identifier (letter or '_')</returns>
        private static bool IsIdentifierStart(char c)
        {
            return Char.IsLetter(c) || (c == '_');
        }
        /// <returns>true if 'c' is part of an identifier (letter, digit, or '_')</returns>
        private static bool IsIdentifierPart(char c)
        {
            return Char.IsLetterOrDigit(c) || (c == '_');
        }

        /// <returns>true if 'c' is the start of an number (digit or '.')</returns>
        private static bool IsNumberStart(char c)
        {
            return Char.IsDigit(c) || (c == '.');
        }
        /// <returns>true if 'c' is part of an identifier (digit, '.', 'e', '-' or '+')</returns>
        private static bool IsNumberPart(char c)
        {
            return Char.IsDigit(c) || (".e-+".IndexOf(c) >= 0);
        }

        private static bool IsWhitespace(char c)
        {
            return (c <= ' ');
        }


        /// <summary>
        /// Chops a string up into identifiers, operators and subexpressions.
        /// The returned value is an array of Objects. Each Object is either a String (identifier or operator)
        /// or a Object[] (subexpression). A operatort string start with a phony space (so you can tell it
        /// from an identifier easily).
        /// </summary>
        /// <param name="str">The string to be lexed</param>
        /// <returns>lexed string (array of Objects)</returns>
        public Object[] Lex(String str)
        {
            System.Collections.ArrayList L = new System.Collections.ArrayList();

            int idx = 0;
            while (idx < str.Length) {
                String opId;

                // skip spaces, control characters
                while ((idx < str.Length) && IsWhitespace(str[idx])) idx++;

                if (idx == str.Length) break; // end of string?
                
                if (str[idx] == '(') { // start of subexpression?
                    // find end of subexpression
                    int endIdx = FindClosingParenthesis(str, idx);
                    // lex subepxression
                    String subExprStr = str.Substring(idx + 1, endIdx - idx - 1);
                    Object[] subExpr = Lex(subExprStr);
                    // add result
                    L.Add(subExpr);
                    // skip over used characters
                    idx = endIdx + 1;
                }
                else if (str[idx] == ')') { // end of subexpression?
                    throw new Exception("rsel.Lex(): unexpected closing parenthesis in expression '" + str + "' at position " + idx);
                }
                else if ((opId = IsOperator(str, idx)) != null) // must be opertor
                {
                    // add result
                    L.Add(" " + opId);
                    // skip over used characters
                    idx += opId.Length;
                }
                else if (IsNumberStart(str[idx])) // start of a number identifier?
                {
                    // get number from string 
                    int endIdx = idx + 1;
                    while ((endIdx < str.Length) && (IsNumberPart(str[endIdx]))) endIdx++;
                    String id = str.Substring(idx, endIdx - idx);
                    if (!Char.IsDigit(id[0])) id = "0" + id; // add leading '0' if number starts with '.'
                    // add result
                    L.Add(id);
                    // skip over used characters
                    idx = endIdx;
                }
                else if (IsIdentifierStart(str[idx])) // start of identifier?
                {
                    // get identifier from string 
                    int endIdx = idx + 1;
                    while ((endIdx < str.Length) && (IsIdentifierPart(str[endIdx]))) endIdx++;
                    String id = str.Substring(idx, endIdx - idx);
                    // add result
                    L.Add(id);
                    // skip over used characters
                    idx = endIdx;
                }
                else throw new Exception("rsel.Lex(): unexpected character expression '" + str + "' at position " + idx);
            }

            return L.ToArray();
        }

        /// <summary>
        /// Find the closing ')' parenthesis in a string str, starting at position startIdx+1
        /// </summary>
        /// <param name="str">The string</param>
        /// <param name="startIdx">The position where the '(' is</param>
        /// <returns>index of closing ')'</returns>
        private int FindClosingParenthesis(String str, int startIdx) {
            int idx = startIdx + 1;
            int parCount = 0;
            while (idx < str.Length) {
                if (str[idx] == ')') {
                    if (parCount == 0) return idx;
                    else parCount--;
                }
                else if (str[idx] == '(') {
                    parCount++;
                }
                idx++;
            }
            throw new Exception("rsel.FindClosingParenthesis(): missing closing parenthesis in expression '"+
                str + "' at position " + startIdx);

        }

        /// <summary>
        /// Searches str for operator listed in m_operators.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="idx"></param>
        /// <returns>the operator (string) found at position 'idx' in 'str'</returns>
        private String IsOperator(String str, int idx)
        {
            String possibleOpStr = str.Substring(idx, m_longestOperatorLength);
            for (int i = 0; i < m_operators.Length; i++)
            {
                if (possibleOpStr.StartsWith(m_operators[i]))
                    return m_operators[i];
            }
            return null;
        }


        /// <summary>
        /// Operators, sorted from longest to shortest operator length
        /// </summary>
        private String[] m_operators;
        private int m_longestOperatorLength;
    }

} // end of namespace G25
