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

namespace RefGA
{
    /// <summary>
    ///  Utility class for GA implementation
    /// </summary>
    public static class Util
    {
        /// <returns>true if 'm' is symmetric up to a small epsilon constant</returns>
        public static bool IsSymmetric(DotNetMatrix.GeneralMatrix m)
        {
            const double EPS = 1e-6;
            DotNetMatrix.GeneralMatrix Z = m.Transpose();
            Z = Z.Subtract(m);
            return (Z.NormInf() < EPS);
        }

        public static bool IsDiagonal(DotNetMatrix.GeneralMatrix m)
        {
            const double EPS = 1e-6;
            for (int i = 0; i < m.RowDimension; i++)
                for (int j = 0; j < m.ColumnDimension; j++)
                    if ((i != j) && (Math.Abs(m.GetElement(i, j)) > EPS))
                        return false;
            return true;
        }

        /// <summary>
        /// Returns true when the matrix is diagonal and has only exact 0, +1 and/or -1 entries on the diagonal.
        /// </summary>
        /// <param name="m">the (square) matrix</param>
        /// <returns></returns>
        public static bool IsSimpleDiagonal(DotNetMatrix.GeneralMatrix m)
        {
            for (int i = 0; i < m.RowDimension; i++)
                for (int j = 0; j < m.ColumnDimension; j++)
                {
                    double v = m.GetElement(i, j);
                    if ((i != j) && (v != 0.0))
                        return false;
                    else if (i == j) {
                        if (!((v == 0.0) || (v == 1.0) || (v == -1.0)))
                            return false;
                    }
                }
            return true;
        }

        /// <returns>true when exact 'value' is anywhere on the diagonal.</returns>
        public static bool HasValueOnDiagonal(DotNetMatrix.GeneralMatrix m, double value)
        {
            for (int i = 0; i < Math.Min(m.RowDimension, m.ColumnDimension); i++)
            {
                if (m.GetElement(i, i) == value) return true;
            }
            return false;
        }


        public static DotNetMatrix.GeneralMatrix Diagonal(double[] d)
        {
            double[][] m = new double[d.Length][];

            for (int i = 0; i < d.Length; i++)
            {
                m[i] = new double[d.Length];
                m[i][i] = d[i];
            }
            return DotNetMatrix.GeneralMatrix.Create(m);
        }

        public static String ToString(DotNetMatrix.GeneralMatrix m)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            for (int i = 0; i < m.RowDimension; i++) {
                for (int j = 0; j < m.ColumnDimension; j++) {
                    result.Append(m.GetElement(i, j));
                    result.Append(" ");
                }
                result.Append("\n");
            }
            return result.ToString();
        }


        public static DotNetMatrix.GeneralMatrix CreateMatrix(double[] M, int nbRows, int nbColumns)
        {
            double[][] M2 = new double[nbRows][];

            for (int i = 0; i < nbRows; i++)
            {
                M2[i] = new double[nbColumns];
                Array.Copy(M, i * nbColumns, M2[i], 0, nbColumns);
            }

            return DotNetMatrix.GeneralMatrix.Create(M2);
        }


        /// <returns>pow(-1, i)</returns>
        public static int MinusOnePow(int i)
        {
            return ((i & 1) == 0) ? 1 : -1;
        }


        /// <param name="A">Arrays of arrays of Objects (may be null). The component arrays of A must not be changed.</param>
        /// <param name="B">Arrays of arrays of Objects (may be null). The component arrays of B must not be changed.</param>
        /// <returns>Sum of `symbolic scalar array' A and B; may return null for zero-length array; may return null for zero-length array</returns>
        public static Object[][] AddSymbolicScalars(Object[][] A, Object[][] B)
        {
            int lengthA = ((A == null) ? 0 : A.Length);
            int lengthB = ((B == null) ? 0 : B.Length);
            if (lengthA + lengthB == 0) return null;

            Object[][] C = new Object[lengthA + lengthB][];

            if (lengthA > 0) Array.Copy(A, C, lengthA); // copy 'A' to 'C'
            if (lengthB > 0) // copy 'B' to 'C'
            {
                int srcIdx = 0;
                int dstIdx = lengthA;
                int length = lengthB;
                Array.Copy(B, srcIdx, C, dstIdx, length);
            }

            return C;
        }

        /// <param name="A">Arrays of arrays of Objects (may be null). The component arrays of A must not be changed.</param>
        /// <param name="B">Arrays of arrays of Objects (may be null). The component arrays of B must not be changed.</param>
        /// <returns>Product of `symbolic scalar array' A and B; may return null for zero-length array</returns>
        public static Object[][] MultiplySymbolicScalars(Object[][] A, Object[][] B)
        {
            int lengthA = ((A == null) ? 0 : A.Length);
            int lengthB = ((B == null) ? 0 : B.Length);
            if (lengthA * lengthB == 0)
            {
                if ((lengthA == 0) && (lengthB == 0)) return null;
                else if (lengthA == 0) return B; // symA = 1.0;
                else return A; // symB == 1.0
            }

            Object[][] C = new Object[lengthA * lengthB][];

            // loop over all A[] and B[], merging (`multiplying') arrays
            int idx = 0;
            for (int ia = 0; ia < lengthA; ia++)
            {
                int la = ((A[ia] == null) ? 0 : A[ia].Length);
                if (la == 0) continue; // check for zero length of A[ia]
                for (int ib = 0; ib < lengthB; ib++)
                {
                    int lb = ((B[ib] == null) ? 0 : B[ib].Length);
                    if (lb == 0) continue;  // check for zero length of B[ib]

                    // merge:
                    Object[] c = C[idx++] = new Object[la + lb];

                    Array.Copy(A[ia], c, la); 
                    {
                        int srcIdx = 0;
                        int dstIdx = la;
                        int length = lb;
                        Array.Copy(B[ib], srcIdx, c, dstIdx, length); // copy 'A[ia]' to 'C[idx]'
                    }
                }
            }

            return C;
        } // end of MultiplySymbolicScalars

        static Type DT = new Double().GetType();
        /// <summary>
        /// Implementation of System.Collections.IComparer.
        /// Can be used to sort collections of different classes.
        /// 
        /// Comparision is based on:
        /// 
        /// -class type (of types differ, then they are sorted based oin classname)
        /// 
        /// -content
        /// 
        ///        -if comparable: IComparable is used
        /// 
        ///        -if not comparable: object is converted to string, and then compared
        /// </summary>
        public class SimplifySymbolicScalarsComparer : System.Collections.IComparer
        {

            int System.Collections.IComparer.Compare(Object x, Object y)
            {
                Type xT = x.GetType();
                Type yT = y.GetType();

                if (Object.ReferenceEquals(xT, yT))
                { // same type, check if comparable
                    IComparable xc = x as IComparable;
                    IComparable yc = y as IComparable;
                    if ((xc != null) && (yc != null)) return xc.CompareTo(yc); // comparable? then compare Objects
                    else return x.ToString().CompareTo(y.ToString()); // otherwise, compare String version of the Objects
                }
                else
                { // different types, compare the type names
                    // first check if Double (Doubles go to front)
                    bool xIsDouble = Object.ReferenceEquals(xT, DT);
                    bool yIsDouble = Object.ReferenceEquals(yT, DT);
                    if (!(xIsDouble || yIsDouble))
                    {
                        // not Doubles:, compare the type names
                        return xT.ToString().CompareTo(yT.ToString());
                    }
                    else if (xIsDouble && yIsDouble) return 0;
                    else if (xIsDouble) return -1;
                    else return 1;
                }
            }
        }

        
        public class SimplifySymbolicArrayComparer : System.Collections.IComparer
        {
            static System.Collections.IComparer SSSC = new SimplifySymbolicScalarsComparer();

            /// <summary>
            /// Implementation of System.Collections.IComparer.Compare()
            /// Can be used to compare symbolic arrays
            /// Do not invoke with null arguments!
            /// </summary>
            /// <param name="_X">Object[]</param>
            /// <param name="_Y">Object[]</param>
            /// <returns>-1, 0, or 1</returns>
            int System.Collections.IComparer.Compare(Object _X, Object _Y)
            {
                Object[] X = (Object[])_X;
                Object[] Y = (Object[])_Y;

                // skip the Doubles
                int ix = 0, iy = 0;
                double sx = 1.0, sy = 1.0;
                while ((ix < X.Length) && Object.ReferenceEquals(X[ix].GetType(), DT)) {sx *= (double)X[ix]; ix++;}
                while ((iy < Y.Length) && Object.ReferenceEquals(Y[iy].GetType(), DT)) { sy *= (double)Y[iy]; iy++; }

                while ((ix < X.Length) && (iy < Y.Length)) // compare all elements of the arrays
                {
                    int compVal = SSSC.Compare(X[ix], Y[iy]);
                    if (compVal != 0) return compVal; // values in arrays X and Y are not equal, so return -1 or +1
                    ix++; iy++;
                }
                if ((ix == X.Length) && (iy == Y.Length)) return 0 ;// ((sx < sy) ? -1 : ((sx > sy) ? 1 : 0)); // arrays are identical (up to Doubles)
                else if (ix == X.Length) return -1; // X is smaller than Y
                else return 1; // Y is larger than X
            }

        }


        public static Object[][] SimplifySymbolicScalars(Object[][] A)
        {
            if (A == null) return null;

            // sort all arrays A[i] of A, and add the non-null ones in AL
            // also multiply all doubles
            ArrayList AL = new ArrayList();
            {
                SimplifySymbolicScalarsComparer SSSC = new SimplifySymbolicScalarsComparer();
                for (int i = 0; i < A.Length; i++)
                {
                    if ((A[i] != null) && (A[i].Length > 0))
                    {
                        Array.Sort(A[i], SSSC);

                        // multiply doubles (at the front)
                        if ((A[i].Length > 1) && (A[i][0] is Double) && (A[i][1] is Double)) // are there 2 or more Double at the front of A[i]?
                        {
                            // count number of doubles, multiply them together
                            int nb = 2; // we know the first two are already double!
                            double val = (double)A[i][0] * (double)A[i][1];
                            while ((nb < A[i].Length) && (A[i][nb] is Double))
                            {
                                val *= (double)A[i][nb];
                                nb++;
                            }

                            // form a new array
                            Object[] newAi = new Object[A[i].Length - nb + 1];
                            newAi[0] = val;
                            Array.Copy(A[i], nb, newAi, 1, A[i].Length - nb);
                            A[i] = newAi;
                        }

                        // if zero, then discard the whole thing:
                        if (!((A[i].Length > 1) && (A[i][0] is Double) && ((double)A[i][0] == 0.0)))
                            AL.Add(A[i]);
                    }
                }
            }

            if (AL.Count == 1) return new Object[1][] {(Object[])AL[0] };

            // sort the arrays themselves
            System.Collections.IComparer SSAC = new SimplifySymbolicArrayComparer();
            AL.Sort(SSAC);


            // merge adjacent identical (up to scale) arrays
            ArrayList mergedAL = new ArrayList();
            for (int i = 0; i < AL.Count;) // note: no increment
            {
                // find all arrays which are equal up to scale:
                int j = i+1; // j will become index of first array which is not equal up to scale
                while ((j < AL.Count) && (SSAC.Compare(AL[i], AL[j]) == 0)) j++;

                if (j - i > 1)
                { // at least one array equal up to scale: merge these arrays
                    double scale = 0; // collect scale here
                    int ALiFirstObjectIdx = -1;
                    for (int k = i; k < j; k++)
                    {
                        int l = 0;
                        Object[] ALk = (Object[])AL[k];
                        double ALkScale = 1.0;
                        while ((l < ALk.Length) && (Object.ReferenceEquals(ALk[l].GetType(), DT)))
                        {
                            ALkScale *= (double)ALk[l];
                            l++;
                        }
                        if (k == i) ALiFirstObjectIdx = l; // remember where the Objects start in AL[i]

                        scale += ALkScale;
                    }


                    if (scale != 0.0)
                    {
                        Object[] ALi = (Object[])AL[i];
                        bool scaleNotOne = (scale != 1.0);
                        Object[] newALi = new Object[ALi.Length - ALiFirstObjectIdx + ((scaleNotOne) ? 1 : 0)];
                        if (scaleNotOne) newALi[0] = scale;
                        Array.Copy(ALi, ALiFirstObjectIdx, newALi, (scaleNotOne) ? 1 : 0, ALi.Length - ALiFirstObjectIdx);
                        mergedAL.Add(newALi);
                    }

                    i = j;
                }
                else
                {
                    mergedAL.Add(AL[i]);
                    i++;
                }
            }

            if (mergedAL.Count == 0) return null;

            Object[][] resultA = new Object[mergedAL.Count][];
            for (int i = 0; i < mergedAL.Count; i++)
                resultA[i] = (Object[])mergedAL[i];
            return resultA;
        }

        /// <summary>
        /// Compares scalar multivectors
        /// </summary>
        /// <param name="A">Scalar Multivector</param>
        /// <param name="B">Scalar Multivector</param>
        /// <returns>-1 (A < B), 0 (A = B), or 1 (A>B)</returns>
        public static int CompareScalarMultivector(Multivector A, Multivector B)
        {
            int thisLength = A.BasisBlades.Length;
            int ILength = B.BasisBlades.Length;

            // first get rid of inverse(0) (how can those exist anyway?)
            if ((thisLength == 0) && (ILength == 0)) return 0;
            else if ((thisLength == 0) && (ILength > 0)) return -1;
            else if ((thisLength > 0) && (ILength == 0)) return 1;
            else return A.BasisBlades[0].SymbolicCompareTo(B.BasisBlades[0]);
        }

        public static int ComputeHashCode(Object[][] A)
        {
            if (A == null) return 0;
            int H = 0;
            foreach (Object[] O in A)
            {
                if (O == null) continue;
                else
                {
                    foreach (Object o in O) H ^= o.GetHashCode();
                }
            }
            return H;
        }

        /// <summary>
        /// Names of functions which simply toggle to sign of coordinates based on grade
        /// </summary>
        public enum SIGN_TOGGLE_FUNCTION {
            NONE = -1, 
            NEGATION = 1,
            REVERSION,
            GRADE_INVOLUTION,
            CLIFFORD_CONJUGATION
        };


        public static int[] GetSignToggleMultipliers(SIGN_TOGGLE_FUNCTION F, int nbEntries)
        {
            int[] result = new int[nbEntries];
            if (F == SIGN_TOGGLE_FUNCTION.NEGATION)
            {
                for (int i = 0; i < nbEntries; i++)
                    result[i] = -1;
            }
            else if (F == SIGN_TOGGLE_FUNCTION.REVERSION)
            {
                for (int i = 0; i < nbEntries; i++)
                    result[i] = (((i>>1)&1) == 0) ? 1 : -1;
            }
            else if (F == SIGN_TOGGLE_FUNCTION.GRADE_INVOLUTION)
            {
                for (int i = 0; i < nbEntries; i++)
                    result[i] = ((i & 1) == 0) ? 1 : -1;
            }
            else if (F == SIGN_TOGGLE_FUNCTION.CLIFFORD_CONJUGATION)
            {
                for (int i = 0; i < nbEntries; i++)
                    result[i] = ((((i+1) >> 1) & 1) == 0) ? 1 : -1;
            }

            return result;
        }

        /// <returns>Number of basis blades of grade 'k' in an space of dimension 'n' (n choose k).</returns>
        public static int NbBasisBladesOfGrade(int k, int n) {
            return (int)((Factorial(n) / Factorial(k)) / Factorial(n - k));
        }

        public static long Factorial(long x)
        {
            long fact = 1;
            long i = 1;
            while (i <= x)
            {
                fact = fact * i;
                i++;
            }
            return fact;
        }


    } // end of class Util
} // end of namespace RefGA
