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
    /// Used to store metric specifications read from XML.
    /// </summary>
    public class Metric
    {
        public Metric(string name)
        {
            m_name = name;
            m_metricBasisVectorIdx1 = new List<int>();
            m_metricBasisVectorIdx2 = new List<int>();
            m_metricValue = new List<double>();
            m_roundingEpsilon = 1e-14;
            m_round = true;
        }

        /// <summary>
        /// Initializes m_metric. 
        /// </summary>
        /// <param name="dim">Dimension of space</param>
        public void Init(int dim)
        {
            // create metric matrix
            double[] m = new double[dim * dim];
            for (int i = 0; i < m_metricValue.Count; i++) {
                int idx1 = m_metricBasisVectorIdx1[i];
                int idx2 = m_metricBasisVectorIdx2[i];

                if ((idx1 >= 0) && (idx2 >= 0) && (idx1 < dim) && (idx2 < dim)) {
                    m[idx1 * dim + idx2] = m[idx2 * dim + idx1] = m_metricValue[i];
                }
            }

            // create RefGA.metric
            m_metric = new RefGA.Metric(m);

            // force rounding off when metric is diagonal
            if (m_metric.IsDiagonal())
                m_round = false;

            // if rounding is enabled, round the eigenvalues of the metric.
            if (m_round)
            {
                m_metric = m_metric.RoundEigenMetric(m_roundingEpsilon);
            }
        }


        /// <summary>
        /// This function is used to known which vectors can be used to generate random
        /// versors which 'behave' reasonably. In the conformal model, non-Euclidean versors cause inprecision.
        /// </summary>
        /// <returns>a bitmap of all basis vectors which have a euclidean metric.</returns>
        public int GetEuclideanBasisVectorBitmap()
        {
            int bitmap = 0;

            for (int i = 0; i < m_metric.GetDimension(); i++)
            {
                bool basisVectorOK = true;
                for (int j = 0; j < m_metric.GetDimension(); j++)
                {
                    if (i == j)
                    {
                        if (m_metric.GetEntry(i, i) <= 0.0) basisVectorOK = false;
                    }
                    else
                    {
                        if ((m_metric.GetEntry(i, j) != 0.0) ||
                            (m_metric.GetEntry(j, i) != 0.0)) basisVectorOK = false;
                    }
                }
                if (basisVectorOK)
                    bitmap |= (1 << i);
            }

            return bitmap;
        }


        /// <summary>
        /// Name of the metric (e.g., 'default' or 'Euclidean')
        /// </summary>
        public string m_name;

        /// <summary>Part of the metric specification (m_metricBasisVectorIdx1[i] . m_metricBasisVectorIdx2[i] = m_metricValue[i])</summary>
        public List<int> m_metricBasisVectorIdx1;
        /// <summary>Part of the metric specification (m_metricBasisVectorIdx1[i] . m_metricBasisVectorIdx2[i] = m_metricValue[i])</summary>
        public List<int> m_metricBasisVectorIdx2;
        /// <summary>Part of the metric specification (m_metricBasisVectorIdx1[i] . m_metricBasisVectorIdx2[i] = m_metricValue[i])</summary>
        public List<double> m_metricValue;

        /// <summary>
        /// Whether to round multivector coordinates after a metric product or not.
        /// 
        /// Due to floating point roundoff errors in eigenvalue computation, a value or
        /// coordinates that should be (e.g.)1.0 may become (e.g.)(1.0+-1e-16).
        /// This makes the generated code less efficient, is annoiying to read and propagates
        /// the roundoff errors.
        /// 
        /// The default is true (round), but when the final metric is diagonal, Init() forces
        /// it to false because there is not need to use it in that case. 
        /// The user can explicitly specify the rounding using <c>round</c> attributes,
        /// but Init() will still override it.
        /// 
        /// When rounding is enabled, coordinates which are very close to an integer
        /// value are 'rounded' to that value. The threshold for being 'very close' is m_roundingEpsilon (1e-14 by default).
        /// </summary>
        public bool m_round;

        /// <summary>
        /// Threshold for rounding. Default is 1e-14. Cannot be set from XML yet.
        /// </summary>
        public double m_roundingEpsilon;

        /// <summary>
        /// RefGA.Metric class for use with RefGA. This member is initialized by calling Init() after the
        /// other members have been set.
        /// </summary>
        public RefGA.Metric m_metric;

    } // end of class Metric
} // end of namespace G25