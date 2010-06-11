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
    /// Immutable class used to represent (Symbolic) unary scalar operations, like inversion, cosine, sine, exp, etc.
    /// To create an instance, use one of the static functions like Sqrt() and Exp().
    /// </summary>
    public class UnaryScalarOp : IComparable 
    {

        /*
         * To add a new operation: 
         * -add name
         * - add 'constructor'
         * - add SymbolicEval() code
         * */
        public static readonly string INVERSE = "inverse";
        public static readonly string SQRT = "sqrt";
        public static readonly string EXP = "exp";
        public static readonly string LOG = "log";
        public static readonly string SIN = "sin";
        public static readonly string COS = "cos";
        public static readonly string TAN = "tan";
        public static readonly string SINH = "sinh";
        public static readonly string COSH = "cosh";
        public static readonly string TANH = "tanh";
        public static readonly string ABS = "abs";

        /// <summary>Used to construct a scalar operations</summary>
        /// <param name="operationName">The name of the operation (e.g. "inverse", "sqrt", "log"; use one of the supplied constants like INVERSE, SQRT or LOG.)</param>
        /// <param name="A">The value upon which the ScalarOp will operate. 
        /// Only the scalarpart is used of A is used.</param>
        public UnaryScalarOp(string operationName, Multivector A)
        {
            m_opName = operationName;
            m_value = A.ScalarPart();
        }

        /// <returns>Multivector(inverse(A))</returns>
        public static Multivector Inverse(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new UnaryScalarOp(INVERSE, A)));
        }

        /// <returns>Multivector(sqrt(A))</returns>
        public static Multivector Sqrt(Multivector A) 
        {
            return new Multivector(new BasisBlade(0, 1.0, new UnaryScalarOp(SQRT, A)));
        }

        /// <returns>Multivector(exp(A))</returns>
        public static Multivector Exp(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new UnaryScalarOp(EXP, A)));
        }

        /// <returns>Multivector(log(A))</returns>
        public static Multivector Log(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new UnaryScalarOp(LOG, A)));
        }

        /// <returns>Multivector(sin(A))</returns>
        public static Multivector Sin(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new UnaryScalarOp(SIN, A)));
        }

        /// <returns>Multivector(cos(A))</returns>
        public static Multivector Cos(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new UnaryScalarOp(COS, A)));
        }

        /// <returns>Multivector(tan(A))</returns>
        public static Multivector Tan(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new UnaryScalarOp(TAN, A)));
        }

        /// <returns>Multivector(sinh(A))</returns>
        public static Multivector Sinh(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new UnaryScalarOp(SINH, A)));
        }

        /// <returns>Multivector(cosh(A))</returns>
        public static Multivector Cosh(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new UnaryScalarOp(COSH, A)));
        }

        /// <returns>Multivector(tanh(A))</returns>
        public static Multivector Tanh(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new UnaryScalarOp(TANH, A)));
        }

        /// <returns>Multivector(abs(A))</returns>
        public static Multivector Abs(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new UnaryScalarOp(ABS, A)));
        }

        public UnaryScalarOp Round(double epsilon)
        {
            Multivector V = m_value.Round(epsilon);
            if (V != m_value) return new UnaryScalarOp(m_opName, V);
            else return this;
        }


        /// <returns>human readable string that represents the value of this ScalarOp</returns>
        public override string ToString()
        {
            string valueStr = value.ToString();
            bool par = (valueStr[0] == '(');

            return m_opName + (par ? "" : "(") + valueStr + (par ? "" : ")");
        }

        /// <summary>
        /// IComparable.CompareTo implementation.
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj is UnaryScalarOp)
            {
                UnaryScalarOp I = (UnaryScalarOp)obj;
                int opNameComp = m_opName.CompareTo(I.m_opName);
                if (opNameComp != 0) return opNameComp;
                else return Util.CompareScalarMultivector(this.value, I.value);
            }

            throw new ArgumentException("UnaryScalarOp.CompareTo(): object is not an UnaryScalarOp");
        }

       /// <summary>
       /// Substitutes all symbolic scalars for doubles (as evaluated by <paramref name="E"/>)
       /// and evaluates the symbolic ScalarOps. E.g. sqrt(2.0) would evaluate to 1.4142...
       /// </summary>
       /// <param name="E">SymbolicEvaluator used to evaluate the symbolic scalars</param>
       /// <returns></returns>
        public double SymbolicEval(RefGA.Symbolic.SymbolicEvaluator E)
        {
            Multivector V = m_value.SymbolicEval(E);
            if (!V.IsScalar()) throw new ArgumentException("UnaryScalarOp.SymbolicEval: argument is not scalar");
            double v = V.RealScalarPart();
            if (m_opName == INVERSE) {
                if (v == 0.0) throw new ArgumentException("UnaryScalarOp.SymbolicEval: divide by zero");
                return 1.0 / v;
            }
            else if (m_opName == SQRT) {
                if (v < 0.0) throw new ArgumentException("UnaryScalarOp.SymbolicEval: square root of negative value");
                else return Math.Sqrt(v);
            }
            else if (m_opName == EXP) {
                return Math.Exp(v);
            }
            else if (m_opName == LOG) {
                if (v <= 0.0) throw new ArgumentException("UnaryScalarOp.SymbolicEval: logarithm of value <= 0");
                else return Math.Log(v);
            }
            else if (m_opName == SIN) {
                return Math.Sin(v);
            }
            else if (m_opName == COS) {
                return Math.Cos(v);
            }
            else if (m_opName == TAN) {
                // how to detect bad input (1/2 pi, etc)?
                return Math.Tan(v);
            }
            else if (m_opName == SINH)
            {
                return Math.Sinh(v);
            }
            else if (m_opName == COSH)
            {
                return Math.Cosh(v);
            }
            else if (m_opName == TANH)
            {
                return Math.Tanh(v);
            }
            else if (m_opName == ABS)
            {
                return Math.Abs(v);
            }
            else throw new ArgumentException("UnaryScalarOp.SymbolicEval: unknown opname " + m_opName);
        }


        /// <summary>
        /// Multivector value on which this UnaryScalarOp operates.
        /// </summary>
        public Multivector value { get { return m_value; } }

        /// <summary>
        /// The name of the scalar operation (e.g., "sqrt").
        /// </summary>
        public string opName { get { return m_opName; } }

        private string m_opName;

        private Multivector m_value;

    } // end of class ScalarOp

} // end of namespace RefGA.Symbolic
