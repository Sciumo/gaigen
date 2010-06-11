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
using System.Collections;


/// This namespace provides some support for symbolic scalars.
namespace RefGA.Symbolic
{
    /// <summary>
    /// Immutable class used to represent (Symbolic) binary scalar operations, like atan2, etc.
    /// To create an instance, use one of the static functions like Atan2().
    /// 
    /// </summary>
    public class BinaryScalarOp : IComparable 
    {

        /*
         * To add a new operation: 
         * -add name
         * - add 'constructor'
         * - add SymbolicEval() code
         * */
        public static readonly string ATAN2 = "atan2";

        /// <summary>Used to construct a scalar operations</summary>
        /// <param name="operationName">The name of the operation (e.g. "atan2"; use one of the supplied constants like ATAN2.)</param>
        /// <param name="A">The first operand. Only the scalarpart is used.</param>
        /// <param name="B">The second operand. Only the scalarpart is used.</param>
        public BinaryScalarOp(string operationName, Multivector A, Multivector B)
        {
            m_opName = operationName;
            m_value1 = A.ScalarPart();
            m_value2 = B.ScalarPart();
        }

        /// <returns>Multivector(inverse(A))</returns>
        public static Multivector Atan2(Multivector A, Multivector B)
        {
            return new Multivector(new BasisBlade(0, 1.0, new BinaryScalarOp(ATAN2, A, B)));
        }

        public BinaryScalarOp Round(double epsilon)
        {
            Multivector A = m_value1.Round(epsilon);
            Multivector B = m_value2.Round(epsilon);
            if ((A != m_value1) || (B != m_value2)) return new BinaryScalarOp(m_opName, A, B);
            else return this;
        }


        /// <returns>human readable string that represents the value of this ScalarOp</returns>
        public override string ToString()
        {
            string value1str = value1.ToString();
            string value2str = value2.ToString();
            bool par = true;

            return m_opName + (par ? "" : "(") + value1str + ", " + value2str + (par ? "" : ")");
        }

        /// <summary>
        /// IComparable.CompareTo implementation.
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj is BinaryScalarOp)
            {
                BinaryScalarOp I = (BinaryScalarOp)obj;
                int opNameComp = m_opName.CompareTo(I.m_opName);
                if (opNameComp != 0) return opNameComp;

                int value1Comp = Util.CompareScalarMultivector(this.value1, I.value1);
                if (value1Comp != 0) return value1Comp;

                else return Util.CompareScalarMultivector(this.value2, I.value2);
            }

            throw new ArgumentException("BinaryScalarOp.CompareTo(): object is not an BinaryScalarOp");
        }

       /// <summary>
       /// Substitutes all symbolic scalars for doubles (as evaluated by <paramref name="E"/>)
       /// and evaluates the symbolic ScalarOps. E.g. sqrt(2.0) would evaluate to 1.4142...
       /// </summary>
       /// <param name="E">SymbolicEvaluator used to evaluate the symbolic scalars</param>
       /// <returns></returns>
        public double SymbolicEval(RefGA.Symbolic.SymbolicEvaluator E)
        {
            Multivector A = m_value1.SymbolicEval(E);
            Multivector B = m_value2.SymbolicEval(E);
            if (!(A.IsScalar() && B.IsScalar())) throw new ArgumentException("BinaryScalarOp.SymbolicEval: argument is not scalar");
            double a = A.RealScalarPart();
            double b = B.RealScalarPart();
            if (m_opName == ATAN2)
            {
                return Math.Atan2(a, b);
            }
            else throw new ArgumentException("BinaryScalarOp.SymbolicEval: unknown opname " + m_opName);
        }


        /// <summary>
        /// First operand of BinaryScalarOp.
        /// </summary>
        public Multivector value1 { get { return m_value1; } }

        /// <summary>
        /// Second operand of BinaryScalarOp.
        /// </summary>
        public Multivector value2 { get { return m_value2; } }

        /// <summary>
        /// The name of the scalar operation (e.g., "atan2").
        /// </summary>
        public string opName { get { return m_opName; } }

        private string m_opName;

        private Multivector m_value1;
        private Multivector m_value2;

    } // end of class BinaryScalarOp



} // end of namespace RefGA.Symbolic
