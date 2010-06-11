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
    /// Immutable class used to represent (Symbolic) scalar operation, like inversion, cosine, sine, exp, etc.
    /// To create an instance, use one of the static functions like Sqrt() and Exp().
    /// 
    /// TODO: also have a class like this for binary functions (frac, atan2, etc)
    /// Maybe make this class the interface and have a UnaryScalarOp and a BinaryScalarOp
    /// </summary>
    public class ScalarOp : IComparable 
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
        public ScalarOp(string operationName, Multivector A)
        {
            m_opName = operationName;
            m_value = A.ScalarPart();
        }

        /// <returns>Multivector(inverse(A))</returns>
        public static Multivector Inverse(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new ScalarOp(INVERSE, A)));
        }

        /// <returns>Multivector(sqrt(A))</returns>
        public static Multivector Sqrt(Multivector A) 
        {
            return new Multivector(new BasisBlade(0, 1.0, new ScalarOp(SQRT, A)));
        }

        /// <returns>Multivector(exp(A))</returns>
        public static Multivector Exp(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new ScalarOp(EXP, A)));
        }

        /// <returns>Multivector(log(A))</returns>
        public static Multivector Log(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new ScalarOp(LOG, A)));
        }

        /// <returns>Multivector(sin(A))</returns>
        public static Multivector Sin(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new ScalarOp(SIN, A)));
        }

        /// <returns>Multivector(cos(A))</returns>
        public static Multivector Cos(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new ScalarOp(COS, A)));
        }

        /// <returns>Multivector(tan(A))</returns>
        public static Multivector Tan(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new ScalarOp(TAN, A)));
        }

        /// <returns>Multivector(sinh(A))</returns>
        public static Multivector Sinh(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new ScalarOp(SINH, A)));
        }

        /// <returns>Multivector(cosh(A))</returns>
        public static Multivector Cosh(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new ScalarOp(COSH, A)));
        }

        /// <returns>Multivector(tanh(A))</returns>
        public static Multivector Tanh(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new ScalarOp(TANH, A)));
        }

        /// <returns>Multivector(abs(A))</returns>
        public static Multivector Abs(Multivector A)
        {
            return new Multivector(new BasisBlade(0, 1.0, new ScalarOp(ABS, A)));
        }

        public ScalarOp Round(double epsilon)
        {
            Multivector V = m_value.Round(epsilon);
            if (V != m_value) return new ScalarOp(m_opName, V);
            else return this;
        }


        /// <returns>human readable string that represents the value of this ScalarOp</returns>
        public override string ToString()
        {
            string valueStr = value.ToString();
            bool par = (valueStr[0] == '(');

            return m_opName  + (par ? "" : "(") + value.ToString() + (par ? "" : ")");
        }

        /// <summary>
        /// IComparable.CompareTo implementation.
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj is ScalarOp)
            {
                ScalarOp I = (ScalarOp)obj;
                int opNameComp = m_opName.CompareTo(I.m_opName);
                if (opNameComp != 0) return opNameComp;
                else return Util.CompareScalarMultivector(this.value, I.value);
            }

            throw new ArgumentException("ScalarOp.CompareTo(): object is not an ScalarOp");
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
            if (!V.IsScalar()) throw new ArgumentException("ScalarOp.SymbolicEval: argument is not scalar");
            double v = V.RealScalarPart();
            if (m_opName == INVERSE) {
                if (v == 0.0) throw new ArgumentException("ScalarOp.SymbolicEval: divide by zero");
                return 1.0 / v;
            }
            else if (m_opName == SQRT) {
                if (v < 0.0) throw new ArgumentException("ScalarOp.SymbolicEval: square root of negative value");
                else return Math.Sqrt(v);
            }
            else if (m_opName == EXP) {
                return Math.Exp(v);
            }
            else if (m_opName == LOG) {
                if (v <= 0.0) throw new ArgumentException("ScalarOp.SymbolicEval: logarithm of value <= 0");
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
            else throw new ArgumentException("ScalarOp.SymbolicEval: unknown opname " + m_opName);
        }


        /// <summary>
        /// Multivector value on which this ScalarOp operates.
        /// </summary>
        public Multivector value { get { return m_value; } }

        /// <summary>
        /// The name of the scalar operation (e.g., "sqrt").
        /// </summary>
        public String opName { get { return m_opName; } }

        private String m_opName;

        private Multivector m_value;

    } // end of class ScalarOp


    /// <summary>
    /// Interface used to 'evaluate' symbolic scalars to real scalar values.
    /// This interface is used for example by Multivector.SymbolicEval()
    /// </summary>
    public interface SymbolicEvaluator
    {
        /// <summary>
        /// Evaluates Object O to a scalar value.
        /// </summary>
        /// <param name="O">The Object to be evaluated. </param>
        /// <returns>A scalar value</returns>
        /// <remarks>Throw exception when evaluation is not possible.</remarks>
        double Evaluate(Object O);
    };


    /// <summary>
    /// Class used to 'evaluate' symbolic scalars to real scalar values by looking them up in a Hashtable
    /// </summary>
    public class HashtableSymbolicEvaluator : SymbolicEvaluator
    {
        /// <summary>
        /// Map from symbolic values -> double
        /// </summary>
        protected System.Collections.Hashtable m_map;

        public HashtableSymbolicEvaluator()
        {
            m_map = new System.Collections.Hashtable();
        }

        /// <summary>
        /// Evaluates Object <paramref name="O"/> to a scalar value. Evaluates by looking up <paramref name="O"/> in a hash table.
        /// Add items to this hashtable by using Add()
        /// </summary>
        /// <param name="O">The Object to be evaluated (must implement Object.GetHashCode()). </param>
        /// <returns>A scalar value (may throw exception when evaluation is not possible)</returns>
        public virtual double Evaluate(Object O)
        {
            if (m_map.ContainsKey(O))
            {
                return (Double)m_map[O];
            }
            else throw new ArgumentException("HashtableSymbolicEvaluator.Evaluate(): unknown object " + O.ToString());
        }

        /// <summary>
        /// Adds a mapping from O to value
        /// </summary>
        /// <param name="O">key</param>
        /// <param name="value">value</param>
        public void Add(Object O, double value)
        {
            m_map.Add(O, value);
        }
    }


    /// <summary>
    ///  This Symbolic evaluator will evaluate symbolic scalars to random values
    /// in a certain range. When first asked to evaluate some Object, a random value
    /// is made up. When asked for the same Object,
    /// the same random scalar value will be returned, until reset() is called.
    /// </summary>
    public class RandomSymbolicEvaluator : HashtableSymbolicEvaluator
    {
        /// <summary>
        /// Random number generator for random values which are substituted
        /// </summary>
        protected System.Random m_randy;

        /// <summary>
        /// Lower part of range of random numbers
        /// </summary>
        protected double m_minRandomVal;
        /// <summary>
        /// Upper part of range of random numbers
        /// </summary>
        protected double m_maxRandomVal;

        /// <summary>
        /// Constructs a new RandomSymbolicEvaluator which will generate random values
        /// in a certain range (uniform distribution)
        /// </summary>
        /// <param name="minRandomVal">Minimal value to which an Object will evaluate.</param>
        /// <param name="maxRandomVal">Maximal value to which an Object will evaluate.</param>
        public RandomSymbolicEvaluator(double minRandomVal, double maxRandomVal)
        {
            m_minRandomVal = minRandomVal;
            m_maxRandomVal = maxRandomVal;
            m_randy = new System.Random();
        }

        public override double Evaluate(Object O)
        {
            if (m_map.ContainsKey(O))
            {
                return (Double)m_map[O];
            }
            else
            {
                double randomValue = m_minRandomVal + m_randy.NextDouble() * (m_maxRandomVal - m_minRandomVal);
                Add(O, randomValue);
                return randomValue;
            }
        }

        /// <summary>
        /// Resets this RandomSymbolicEvaluator. I.e., makes it forget (by clearing the Hashtable)
        /// all previously mappings from Objects to random values
        /// </summary>
        public void reset()
        {
            m_map.Clear();
        }
    };

    /// <summary>
    /// Utility class for Symbolic evaluation.
    /// </summary>
    public static class SymbolicUtil {
        /// <summary>
        /// Evaluates a Multivector to a summary of its expected value (-1, 0, 1).
        /// The function uses a Symbolic.RandomSymbolicEvaluator to randomly evaluate
        /// <paramref name="A"/>. If the results are consistently negative, zero or 
        /// positive then -1, 0 or 1 is returned. Otherwise an exception is thrown.
        /// </summary>
        /// <param name="A">The multivector to evaluate</param>
        /// <returns>-1, 0 or 1</returns>
        /// <remarks>
        /// Used by Multivector.Exp(), Multivector.Cos() and Multivector.Sin() to see
        /// if the input bivector has a scalar square.
        /// </remarks>
        public static double EvaluateRandomSymbolicToScalar(Multivector A)
        {
            double EPS = 1e-4;
            int NB_ITER = 100;
            int REQ = 98;

            Symbolic.RandomSymbolicEvaluator RSE = new Symbolic.RandomSymbolicEvaluator(-100.0, 100.0);
            int pos = 0, neg = 0, zero = 0;
            for (int i = 0; i < NB_ITER; i++)
            {
                Multivector E = A.SymbolicEval(RSE);
                double scalarPart = E.RealScalarPart();
                Multivector theRest = Multivector.Subtract(E, scalarPart);
                if (Math.Abs(theRest.Norm_e().RealScalarPart()) > Math.Abs(scalarPart) * EPS)
                    throw new Exception("Multivector did not evaluate to scalar");

                if (scalarPart > EPS) pos++;
                else if (scalarPart < -EPS) neg++;
                else zero++;
            }

            if (pos >= REQ) return 1.0;
            else if (zero >= REQ) return 0.0;
            else if (neg >= REQ) return -1.0;
            else throw new Exception("Multivector did not evaluate to a consistent value");
        }
    }


} // end of namespace RefGA.Symbolic
