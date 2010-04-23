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
using System.Xml;

namespace G25
{
    /// <summary>
    /// This class implements a Really Simple Basis Blade Parser. It is used to parse
    /// lists of basis blades in XML specifications of algebra. The lists of blades may
    /// also contain assignments (to assign constant values to basis blades). This
    /// is why the main BasisBlade type used in this class is G25.rsbbp.BasisBlade
    /// and not the regular RefGA.BasisBlade.
    /// 
    /// This class also contains some utility functions, such as SortBasisBladeListByGrade() and ListToArray()
    /// 
    /// Some examples of lists of basis blades:
    ///   - "no^e1 no^e2 no^e3 e1^e2 e2^e3 e3^e1 e1^ni e2^ni e3^ni no^ni" (all grade 2 blades of a 5-D conformal algebra)
    ///   - "no=1 e1 e2 e3" (conformal normalizedPoint note the assignment)
    /// 
    /// The parsing of such lists works as follows: first the G25.rsep is used to do a 
    /// basic parse of the lists. The result is a tree of G25.rsep.FunctionApplication .
    /// An example: suppose the original input is: "no^e1 no^e2 no^e3=-1". This would be parsed as 
    /// "concat(op(no, e1), concat(no^e2, concat(assign(no^e3, negate(1)))))".
    /// This result is then converted to lists G25.rsbbp.BasisBlade of by the functions 
    /// G25.rsbbp.ConvertParsedXMLtoBasisBladeList(), G25.rsbbp.ConvertParsedXMLtoBasisBlade() and 
    /// G25.rsbbp.ConvertParsedXMLtoScalar().
    /// These functions also do some sanity checking, for example preventing the parsing of
    /// illegal lists like "no^e1=no^e2=1"
    /// 
    /// 
    /// </summary>
    public class rsbbp
    {

        /// <summary>
        /// This class represents a basis blade with an optional constant value assigned to it.
        /// 
        /// The class is used by G25.rsbbp to return results from parsing basis blade lists.
        /// Basis blade strings can either take the form of for example "e1^e2^e3" or "e1^e2^e3=2".
        /// The '=' assignment indicates a constant coordinate value for that basis blade. 
        /// 
        /// </summary>
        public class BasisBlade
        {
            /// <summary>
            /// Constructor for a BasisBlade without a constant coordinate value
            /// </summary>
            public BasisBlade(RefGA.BasisBlade B)
            {
                m_basisBlade = B;
                m_isConstant = false;
                m_constantValue = 0.0;
            }

            /// <summary>
            /// Constructor for a BasisBlade with a constant coordinate value
            /// </summary>
            /// <param name="B"></param>
            /// <param name="constantValue"></param>
            public BasisBlade(RefGA.BasisBlade B, double constantValue)
            {
                m_basisBlade = B;
                m_isConstant = true;
                m_constantValue = constantValue;
            }

            /// <summary>The RefGA.BasisBlade</summary>
            public RefGA.BasisBlade GetBasisBlade { get { return m_basisBlade; } }

            /// <summary>when true, this basis blade has a constant coordinate.</summary>
            public bool IsConstant { get { return m_isConstant; } }

            /// <summary>The RefGA.BasisBlade</summary>
            public double ConstantValue { get { return m_constantValue; } }

            public override string ToString()
            {
                String bbStr = GetBasisBlade.ToString();
                if (IsConstant) return bbStr + "=" + ConstantValue.ToString();
                else return bbStr;
            }

            /// <summary>
            /// The basis blade 
            /// </summary>
            protected readonly RefGA.BasisBlade m_basisBlade;
            /// <summary>
            /// When this value is true, then a constant value is
            /// assigned to this basis blade.
            /// </summary>
            protected readonly bool m_isConstant;
            /// <summary>
            /// The constant value of the coordinate for this basis blade, if m_isConstant is true.
            /// </summary>
            protected readonly double m_constantValue;
        }



        public rsbbp(Specification S)
        {
            m_spec = S;
            InitBasisBladeParser();
        }


        /// <summary>
        /// Utility function Takes a bunch of basis blades in a double list and sorts them based on grade (keeping the 
        /// original order, for each grade).
        /// </summary>
        /// <returns>Double lists of basis blades. The first index is for grade in range [0 ... m_dimension].</returns>
        public List<List<G25.rsbbp.BasisBlade>> SortBasisBladeListByGrade(List<List<G25.rsbbp.BasisBlade>> B)
        {
            // alloc mem for all grades:
            List<List<G25.rsbbp.BasisBlade>> L = new List<List<G25.rsbbp.BasisBlade>>();
            for (int i = 0; i < m_spec.m_dimension + 1; i++)
                L.Add(new List<G25.rsbbp.BasisBlade>());

            for (int i = 0; i < B.Count; i++)
            {
                for (int j = 0; j < B[i].Count; j++)
                {
                    G25.rsbbp.BasisBlade b = B[i][j];
                    L[b.GetBasisBlade.Grade()].Add(b);
                }
            }

            return L;
        }

        /// <summary>
        /// Parses the basis blades of a general multivector element. 
        /// 'E' must either directly contain a text element, or contain multiple groups.
        /// </summary>
        /// <param name="E">The Specification.XML_MV element</param>
        /// <returns></returns>
        public List<List<G25.rsbbp.BasisBlade>> ParseMVbasisBlades(XmlElement E)
        {
            List<List<G25.rsbbp.BasisBlade>> L = new List<List<G25.rsbbp.BasisBlade>>();

            XmlText T = E.FirstChild as XmlText;
            if (T != null)
            {
                // The contents of 'E' is just a single text element
                L.Add(ParseBasisBlades(T));
            }
            else
            {
                // loop over elements, searching for Specification.XML_GROUP tags
                XmlElement G = E.FirstChild as XmlElement;
                while (G != null)
                {
                    // check if name of element is 'group'
                    if (G.Name != Specification.XML_GROUP)
                        throw new Exception("Invalid element '" + G.Name + "' in element '" + Specification.XML_MV + "'");

                    // check content is text
                    T = G.FirstChild as XmlText;
                    if (T == null) throw new Exception("Invalid contents of element '" + G.Name + "' in element '" + Specification.XML_MV + "'");

                    // parse text, add to list of basis blades
                    L.Add(ParseBasisBlades(T));

                    G = G.NextSibling as XmlElement;
                }
            }
            return L;
        }

        /// <summary>
        /// Parses the space-seperated list of basis blades in 'T'.
        /// </summary>
        /// <param name="T">contains a string like "e1^e2 e2^e3 e3^e1".</param>
        /// <returns>A list of RefGA.BasisBlade, or throws an exception.</returns>
        public List<G25.rsbbp.BasisBlade> ParseBasisBlades(XmlText T)
        {
            // parse XmlText
            String str = T.Value;
            Object O = m_basisBladeParser.Parse(str);

            // construct list
            List<G25.rsbbp.BasisBlade> L = new List<G25.rsbbp.BasisBlade>();

            ConvertParsedXMLtoBasisBladeList(O, L, str);

            return L;
        }

        /// <summary>
        /// Converts a tree of G25.rsep.FunctionApplication to a list of G25.rsbbp.BasisBlade.
        /// The allowed functions are concat(), assign(), op(), negate() and nop()
        /// </summary>
        /// <param name="O">O should be a chain of concat(string, concat(op(...), concat(op(....), or a String (name of basisvector, or "scalar")</param>
        /// <param name="originalStr">The full string (used only for error messages)</param>
        /// <returns>the tree converted to a list of basis blade (with a possible constant values assigned to it).</returns>
        private void ConvertParsedXMLtoBasisBladeList(Object O, List<G25.rsbbp.BasisBlade> L, String originalStr)
        {
            { // is 'O' a string? (can happen when last entry in concat list and the basis blade is just a scalar or a basis vector)
                String Ostr = O as String;
                if (Ostr != null)
                {
                    L.Add(ConvertParsedXMLtoBasisBlade(O, originalStr));
                    return;
                }
            }

            // not a string: O must be a function application (concat(...), assign(...) op(...), negate(...) or nop(...) )
            G25.rsep.FunctionApplication FA = O as G25.rsep.FunctionApplication;
            if (FA == null)
                throw new Exception("Invalid contents of element '" + Specification.XML_MV + "': " + originalStr);

            if ((FA.FunctionName == "op") || (FA.FunctionName == "negate") || (FA.FunctionName == "nop") || (FA.FunctionName == "assign"))
            { // this is the last entry in the concat(...) list
                // assign(...), op(...), negate(...) or nop(...)
                L.Add(ConvertParsedXMLtoBasisBlade(FA, originalStr));
            }
            else if ((FA.FunctionName == "concat") && (FA.NbArguments == 2))
            {
                // concat(..., ....)
                L.Add(ConvertParsedXMLtoBasisBlade(FA.Arguments[0], originalStr));
                ConvertParsedXMLtoBasisBladeList(FA.Arguments[1], L, originalStr);
            }
            else throw new Exception("Error in basis blade list " + originalStr);
        }

        /// <summary>
        /// Converts a tree of G25.rsep.FunctionApplication to a G25.rsbbp.BasisBlade.
        /// The allowed functions are assign(), op(), negate() and nop()
        /// </summary>
        /// <param name="O">Tree of G25.rsep.FunctionApplication, or a String (name of basisvector, or "scalar")</param>
        /// <param name="originalStr">The full string (used only for error messages)</param>
        /// <returns>the tree converted to a basis blade (with a possible constant value assigned to it).</returns>
        private G25.rsbbp.BasisBlade ConvertParsedXMLtoBasisBlade(Object O, String originalStr)
        {
            // O is either a string or assign(...), op(...), negate(...) or nop(...)

            { // is 'O' a string?
                String Ostr = O as String;
                if (Ostr != null)
                {
                    // either a basis blade name, or "scalar"
                    if (Ostr == "scalar") return new G25.rsbbp.BasisBlade(RefGA.BasisBlade.ONE);
                    else return new G25.rsbbp.BasisBlade(new RefGA.BasisBlade((uint)(1 << m_spec.BasisVectorNameToIndex(Ostr))));
                }
            }

            G25.rsep.FunctionApplication FA = O as G25.rsep.FunctionApplication;

            if ((FA.FunctionName == "assign") && (FA.NbArguments == 2))
            {
                // assign(e1^e2^e3, scalarValue)
                G25.rsbbp.BasisBlade basisBlade = ConvertParsedXMLtoBasisBlade(FA.Arguments[0], originalStr);
                if (basisBlade.IsConstant)
                    throw new Exception("Invalid assignment '=' in blade specification: " + originalStr);
                double value = ConvertParsedXMLtoScalar(FA.Arguments[1], originalStr);
                // arg0 = basis blade (not const assignment
                // arg1 = scalar value
                return new G25.rsbbp.BasisBlade(basisBlade.GetBasisBlade, value);
            }
            else if ((FA.FunctionName == "op") && (FA.NbArguments == 2))
            {
                G25.rsbbp.BasisBlade arg1 = ConvertParsedXMLtoBasisBlade(FA.Arguments[0], originalStr);
                G25.rsbbp.BasisBlade arg2 = ConvertParsedXMLtoBasisBlade(FA.Arguments[1], originalStr);

                if (arg1.IsConstant || arg2.IsConstant)
                    throw new Exception("Invalid assignment '=' in blade specification: " + originalStr);

                return new G25.rsbbp.BasisBlade(RefGA.BasisBlade.op(arg1.GetBasisBlade, arg2.GetBasisBlade));
            }
            else if ((FA.FunctionName == "negate") && (FA.NbArguments == 1))
            {
                G25.rsbbp.BasisBlade arg1 = ConvertParsedXMLtoBasisBlade(FA.Arguments[0], originalStr);
                if (arg1.IsConstant)
                    throw new Exception("Invalid assignment '=' in blade specification: " + originalStr);

                return new G25.rsbbp.BasisBlade(RefGA.BasisBlade.op(RefGA.BasisBlade.MINUS_ONE, arg1.GetBasisBlade));
            }
            else if ((FA.FunctionName == "nop") && (FA.NbArguments == 1))
            {
                return ConvertParsedXMLtoBasisBlade(FA.Arguments[0], originalStr);
            }
            else throw new Exception("Error in basis blade list " + originalStr);
        }

        private double ConvertParsedXMLtoScalar(Object O, String originalStr)
        {
            // O is either a string or negate(...) or nop(...)

            { // is 'O' a string?
                String Ostr = O as String;
                if (Ostr != null)
                {
                    // Ostr is a scalar value
                    try
                    {
                        return System.Double.Parse(Ostr);
                    }
                    catch (System.Exception)
                    {
                        throw new Exception("Invalid scalar value '" + Ostr + "' in basis blade list " + originalStr);
                    }
                }
            }

            // it must be a function application (negate or add (nop))
            G25.rsep.FunctionApplication FA = O as G25.rsep.FunctionApplication;

            if ((FA.FunctionName == "negate") && (FA.NbArguments == 1))
                return -ConvertParsedXMLtoScalar(FA.Arguments[0], originalStr);
            else if ((FA.FunctionName == "nop") && (FA.NbArguments == 1))
                return ConvertParsedXMLtoScalar(FA.Arguments[0], originalStr);
            else throw new Exception("Error in basis blade list " + originalStr);
        }
        

        /// <returns>'L' convert to a double array</returns>
        public static RefGA.BasisBlade[][] ListToDoubleArray(List<List<RefGA.BasisBlade>> L)
        {
            RefGA.BasisBlade[][] A = new RefGA.BasisBlade[L.Count][];
            for (int i = 0; i < L.Count; i++)
                A[i] = L[i].ToArray();
            return A;
        }

        /// <returns>'L' convert to a double array</returns>
        public static RefGA.BasisBlade[][] ListToDoubleArray(List<List<G25.rsbbp.BasisBlade>> L)
        {
            RefGA.BasisBlade[][] A = new RefGA.BasisBlade[L.Count][];
            for (int i = 0; i < L.Count; i++) {
                A[i] = new RefGA.BasisBlade[L[i].Count];
                for (int j = 0; j < L[i].Count; j++)
                    A[i][j] = L[i][j].GetBasisBlade;
            }
            return A;
        }

        /// <returns>'L' convert to a single array</returns>
        public static RefGA.BasisBlade[] ListToSingleArray(List<List<G25.rsbbp.BasisBlade>> L)
        {
            // count total number of basis blades
            int cnt = 0;
            for (int i = 0; i < L.Count; i++) cnt += L[i].Count;

            // allocate, copy basis blades
            RefGA.BasisBlade[] A = new RefGA.BasisBlade[cnt];
            int idx = 0;
            for (int i = 0; i < L.Count; i++) {
                for (int j = 0; j < L[i].Count; j++)
                    A[idx++] = L[i][j].GetBasisBlade;
            }
            return A;
        }

        /// <returns>a list of basis blades in canonical order, sorted by grade.
        /// Each entry in the outer list contains the basis blades for that grade.</returns>
        public List<List<G25.rsbbp.BasisBlade>> GetDefaultBasisBlades()
        {
            // allocate list for each grade
            List<List<G25.rsbbp.BasisBlade>> L = new List<List<G25.rsbbp.BasisBlade>>();
            for (int i = 0; i < m_spec.m_dimension + 1; i++)
                L.Add(new List<G25.rsbbp.BasisBlade>());

            // enumerate over all basis blades, add them to respective lists
            for (uint b = 0; b < (1 << m_spec.m_dimension); b++)
            {
                RefGA.BasisBlade B = new RefGA.BasisBlade(b);
                L[B.Grade()].Add(new G25.rsbbp.BasisBlade(B));
            }

            return L;
        }

        /// <returns>true if any of the blades in 'L' has a constant coordinate assignment.</returns>
        public static bool ConstantsInList(List<List<G25.rsbbp.BasisBlade>> L)
        {
            for (int i = 0; i < L.Count; i++)
            {
                for (int j = 0; j < L[i].Count; j++)
                {
                    if (L[i][j].IsConstant) return true;
                }
            }
            return false;
        }




        private void InitBasisBladeParser()
        {
            bool UNARY = true, BINARY = false;
            bool PREFIX = false;
            bool LEFT_ASSOCIATIVE = true, RIGHT_ASSOCIATIVE = false;
            G25.rsep.Operator[] ops = new G25.rsep.Operator[] {
                // symbol, name, precedence, unary, postfix, left associative
                new G25.rsep.Operator(" ", "concat", 4, BINARY, PREFIX, RIGHT_ASSOCIATIVE),
                new G25.rsep.Operator("=", "assign", 3, BINARY, PREFIX, RIGHT_ASSOCIATIVE),
                new G25.rsep.Operator("^", "op", 2, BINARY, PREFIX, LEFT_ASSOCIATIVE),
                new G25.rsep.Operator("-", "negate", 1, UNARY, PREFIX, LEFT_ASSOCIATIVE),
                new G25.rsep.Operator("+", "nop", 0, UNARY, PREFIX, LEFT_ASSOCIATIVE),
            };
            m_basisBladeParser = new G25.rsep(ops);
        }


        /// <summary>Used to parse basis blade specifications (like "scalar no^ni e1^e2")</summary>
        protected G25.rsep m_basisBladeParser;

        /// <summary>
        /// The specification which uses this parser (used for coordinate names, dimension of space.
        /// </summary>
        protected Specification m_spec;

    } // end of class class rsbbp
} // end of namespace G25
