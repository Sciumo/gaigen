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
using System.Collections;
using System.Text;
using DotNetMatrix;

namespace RefGA
{
    /// <summary>
    /// Immutable class that represents the metric of an N dimensional geometric algebra.
    /// </summary>
    public class Metric
    {

        /// <summary>Creates a new instance of Metric.</summary>
        /// <param name="m">The NxN metric matrix in array form. Row 0 is at m[0] to m[N-1], etc</param> 
        public Metric(double[] m)
        {
            int dim;
            { // get dimension of space, sanity check
                double doubleDim = Math.Sqrt(m.Length);
                if ((doubleDim * doubleDim - m.Length) != 0.0)
                    throw new Exception("Invalid length of metric array (length must be square of integer)");
                dim = (int)doubleDim;
            }

            init(Util.CreateMatrix(m, dim, dim));
        }

        /// <summary>Creates a new instance of Metric</summary>
        /// <param name="m">The NxN metric matrix in array of array form. First index is rows, second index is columns</param> 
        public Metric(double[][] m)
        {
            init(DotNetMatrix.GeneralMatrix.Create(m));
        }

        /// <summary>Creates a new instance of Metric</summary>
        /// <param name="m">The NxN metric matrix</param> 
        public Metric(DotNetMatrix.GeneralMatrix m)
        {
            init(m); // forward call to init()
        }

        /// <summary>
        /// Constructor where caller determines the value of all fields (used internally by RoundEigenMetric())
        /// </summary>
        protected Metric(DotNetMatrix.GeneralMatrix matrix,
                DotNetMatrix.EigenvalueDecomposition eig, double[] eigenMetric,
                DotNetMatrix.GeneralMatrix invEigMatrix, bool isDiagonal,
                bool isEuclidean, bool isAntiEuclidean)
        {
            m_eigenMetric = eigenMetric;
            m_matrix = matrix;
            m_eig = eig;
            m_eigenMetric = eigenMetric;
            m_invEigMatrix = invEigMatrix;
            m_isDiagonal = isDiagonal;
            m_isEuclidean = isEuclidean;
            m_isAntiEuclidean = isAntiEuclidean;
        }

        /// <returns>true when metric is Euclidean</returns>
        public bool IsEuclidean()
        {
            return m_isEuclidean;
        }
        /// <returns>true when metric is anti-Euclidean</returns>
        public bool IsAntiEuclidean()
        {
            return m_isAntiEuclidean;
        }
        /// <returns>true when metric is diagonal</returns>
        public bool IsDiagonal()
        {
            return m_isDiagonal;
        }

        /// <returns>true when metric is diagonal and all entries on the diagonal are either +1 or -1 or 0.</returns>
        public bool IsSimpleDiagonal()
        {
            return Util.IsSimpleDiagonal(m_matrix);
        }

        /// <returns>true when exact 'value' is anywhere on the diagonal.</returns>
        public bool HasValueOnDiagonal(double value)
        {
            return Util.HasValueOnDiagonal(m_matrix, value);
        }

        /// <returns>value of diagonal entry 'idx'</returns>
        public double DiagonalValue(int idx)
        {
            return m_matrix.GetElement(idx, idx);
        }

        /// <returns>entry of the metric matrix. Since the matrix is symmetric, <c>i</c> and <c>j</c> can be swapped.</returns>
        public double GetEntry(int i, int j)
        {
            return m_matrix.GetElement(i, j);
        }

        public bool IsPositiveDefinite()
        {
            foreach (double m in m_eigenMetric)
                if (m <= 0.0) return false;
            return true;
        }

        public static bool IsPositiveDefinite(double[] m)
        {
            for (int i = 0; i < m.Length; i++)
                if (m[i] <= 0.0) return false;
            return true;
        }

        /// <returns>true when there are null eigenvalues for the metric matrix.</returns>
        public bool IsDegenerate()
        {
            for (int i = 0; i < EigenMetric.Length; i++)
                if (EigenMetric[i] == 0.0) return true;
            return false;

        }

        /// <summary>Initializes this Metric object from metric matrix (called by constructor) </summary>
        /// <param name="m">The NxN metric matrix</param> 
        private void init(DotNetMatrix.GeneralMatrix m)
        {
            if (!Util.IsSymmetric(m))
                throw new Exception("The metric matrix must be symmetric");

            m_matrix = m.Copy();

//            System.Console.WriteLine("m_matrix: " + Util.ToString(m_matrix));

            // compute eigen value decomposition
            m_eig = new DotNetMatrix.EigenvalueDecomposition(m_matrix);

//            System.Console.WriteLine("m_eig: " + Util.ToString(m_eig.GetV()));

            m_invEigMatrix = m_eig.GetV().Transpose();

            m_eigenMetric = m_eig.RealEigenvalues;

//            {
  //              DotNetMatrix.GeneralMatrix D = Util.Diagonal(m_eigenMetric);
    //            DotNetMatrix.GeneralMatrix tmp = m_eig.GetV().Multiply(D).Multiply(m_invEigMatrix);
//                System.Console.WriteLine("input_matrix = " + Util.ToString(tmp)); 
      //      }

            m_isDiagonal = Util.IsDiagonal(m_matrix);
            if (!m_isDiagonal)
            {
                m_isEuclidean = m_isAntiEuclidean = false;
            }
            else
            {
                m_isEuclidean = m_isAntiEuclidean = true;
                for (int i = 0; i < m.RowDimension; i++)
                {
                    if (m_matrix.GetElement(i, i) != 1.0)
                        m_isEuclidean = false;
                    if (m_matrix.GetElement(i, i) != -1.0)
                        m_isAntiEuclidean = false;
                }
            }

        }

        /// <summary>
        /// Transforms BasisBlade <paramref name="a"/> to the orthogonal basis of this metric.
        /// </summary>
        /// <param name="a">The BasisBlade (must be on the regular basis).</param>
        /// <returns><paramref name="a"/> on the new basis.</returns>
        public ArrayList ToEigenbasis(BasisBlade a)
        {
            return Transform(a, m_invEigMatrix);
        }

        /// <summary>
        /// Transforms a list of BasisBlades <paramref name="A"/> to the orthogonal 'eigen' basis of this metric.
        /// </summary>
        /// <param name="A">The list of BasisBlades (each BasisBlade must be on the regular basis).</param>
        /// <returns><paramref name="A"/> on the new basis.</returns>
        public ArrayList ToEigenbasis(ArrayList A)
        {
            ArrayList result = new ArrayList();
            for (int i = 0; i < A.Count; i++)
            {
                ArrayList tmp = ToEigenbasis((BasisBlade)A[i]);
                result.AddRange(tmp);
            }
            return BasisBlade.Simplify(result);
        }

        /// <summary>
        /// Transforms BasisBlade <paramref name="a"/> to the regular basis of this metric.
        /// </summary>
        /// <param name="a">The BasisBlade (must be on the orthogonal 'eigen' basis).</param>
        /// <returns><paramref name="a"/> on the new basis.</returns>
        public ArrayList ToMetricBasis(BasisBlade a)
        {
            return Transform(a, m_eig.GetV());
        }

        /// <summary>
        /// Transforms <paramref name="A"/> to the metric basis (<paramref name="A"/> must be on eigenbasis).
        /// </summary>
        /// <param name="A">The list of BasisBlades (must be on the orthogonal 'eigen' basis).</param>
        /// <returns><paramref name="A"/> on the new basis.</returns>
        public ArrayList ToMetricBasis(ArrayList A)
        {
            ArrayList result = new ArrayList();
            for (int i = 0; i < A.Count; i++) {
                ArrayList tmp = ToMetricBasis((BasisBlade)A[i]);
                result.AddRange(tmp);
            }
            return BasisBlade.Simplify(result);
        }


        /// <summary>
        ///  Transforms a basis blade to another basis which is represented by the columns of M.
        /// </summary>
        /// <param name="a">The basis blade to transform according to <paramref name="M"/>.</param>
        /// <param name="M">The matrix to use to transform <paramref name="a"/>.</param>
        /// <returns>a list of BasisBlade whose sum represents <paramref name="a"/> on the new basis.</returns>
        public static ArrayList Transform(BasisBlade a, DotNetMatrix.GeneralMatrix M)
        {
            ArrayList A = new ArrayList();
            A.Add(new BasisBlade(0, a.scale, a.symScale)); // start with just the scalar part;

            int dim = M.RowDimension;

           // for each 1 bit: convert to list of blades
            int i = 0;
            uint b = a.bitmap;
            while (b != 0) {
                if ((b & 1) != 0) {
                    // take column 'i' out of the matrix, wedge it to 'A'
                    ArrayList tmp = new ArrayList();
                    for (int j = 0; j < dim; j++) {
                        double m = M.GetElement(j, i);
                        if (m != 0.0) {
                            for (int k = 0; k < A.Count; k++)
                            {
                                BasisBlade o = BasisBlade.op((BasisBlade)A[k], new BasisBlade((uint)(1 << j), m));
                                if (o.scale != 0.0) tmp.Add(o);
                            }
                        }
                    }
                    A = tmp;
                }
                b >>= 1;
                i++;
            }
            return A;
        }

        public Metric RoundEigenMetric(double roundingEpsilon)
        {
            double[] newEigenMetric = (double[])m_eigenMetric.Clone();
            bool changed = false;
            for (int i = 0; i < newEigenMetric.Length; i++) {
                double r = BasisBlade.Round(roundingEpsilon, newEigenMetric[i]);
                if (r != newEigenMetric[i]) 
                {
                    newEigenMetric[i] = r;
                    changed = true;
                }
            }

            if (changed)
                return new Metric(m_matrix, m_eig, newEigenMetric, m_invEigMatrix, m_isDiagonal, m_isEuclidean, m_isAntiEuclidean);
            else return this;
        }

        public int GetDimension()
        {
            return m_matrix.ColumnDimension;
        }

        /// <returns>
        /// Eigenmetric (metric of basis vectors in 'eigenspace')
        /// </returns>
        /// <remarks>Do not modify the returned array!</remarks>
        /// <seealso cref="m_eigenMetric"/>
        public double[] EigenMetric
        {
            get
            {
                return m_eigenMetric;
            }
        }

        /// <summary>
        /// The metric matrix
        /// </summary>
        private DotNetMatrix.GeneralMatrix m_matrix;

        /// <summary>
        /// The eigenvectors matrix & eigenvalues of m_matrix
        /// </summary>
        protected DotNetMatrix.EigenvalueDecomposition m_eig;


        /// <summary>
        /// The eigenvalues, stored in an array of doubles.
        /// </summary>
        protected double[] m_eigenMetric;

        /// <summary>
        /// Inverse of the eigenvector matrix.
        /// </summary>
        protected DotNetMatrix.GeneralMatrix m_invEigMatrix;

        /// <summary>
        /// True when m_matrix is diagonal
        /// </summary>
        protected bool m_isDiagonal;

        /// <summary>
        /// True when each entry in m_eigenMetric is 1.0
        /// </summary>
        protected bool m_isEuclidean;
        /// <summary>
        /// True when each entry in m_eigenMetric is -1.0
        /// </summary>
        protected bool m_isAntiEuclidean;
    } // end of class Metric

} // end of namespace RefGA
