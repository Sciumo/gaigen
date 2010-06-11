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
