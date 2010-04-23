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
using RefGA;

namespace G25
{
    /// <summary>
    /// This class contains the specification of a outermorphism matrix representations.
    /// This can be either a general ( G25.GOM )
    /// or specialized ( G25.SOM ) outermorphism. The class contains:
    ///   - the name of the outermorphism (for example "om" or "flatPointOM").
    ///   - order of coordinates in domain, sorted by grade.
    ///   - order of coordinates in range, sorted by grade.
    ///   - whether the outermorphism is specialized or general.
    /// 
    /// Domain and range do not have to be equal although they are by default (i.e., if you set range=null
    /// on the constructor, it is assumed to be equal to the domain).
    /// </summary>
    public class OM : VariableType
    {
        /// <summary>
        /// Constructor. Do not use directly. Use the constructors of G25.GOM and
        /// G25.SOM instead.
        /// </summary>
        /// <param name="specialized">Whether this is a specialized G25.OM or not.</param>
        /// <param name="name">The name of the outermorphism, for example "om" or "flatPointOM".</param>
        /// <param name="domain">The basis blades of the domain. These are sorted by grade by the constructor.</param>
        /// <param name="range">The basis blades of the range (can be null, then the domain is copied). These are sorted by grade by the constructor.</param>
        /// <param name="spaceDim">The dimension of the space of the algebra (used to know what the maximum grade is).</param>
        protected OM(bool specialized, String name, RefGA.BasisBlade[] domain, RefGA.BasisBlade[] range, int spaceDim)
        {
            m_specialized = specialized;
            m_name = name;

            m_domain = SortByGrade(domain, spaceDim);
            if (range != null) m_range = SortByGrade(range, spaceDim);
            else range = domain;

            // initializes one SMV for each domain element
            m_domainSmv = new SMVOM[m_domain.Length][];
            for (int g = 0; g < m_domainSmv.Length; g++)
            {
                m_domainSmv[g] = new SMVOM[m_domain[g].Length];
                for (int c = 0; c < m_domain[g].Length; c++)
                {
                    string smvName = name + "_smv_grade_" + g + "_column_" + c;
                    m_domainSmv[g][c] = new SMVOM(smvName, m_range[g], this, g, c);
                }
            }

            m_domainVectors = GetVectors(m_domain, null);
            m_rangeVectors = GetVectors(m_domain, "c");
        } // end of OM constructor

        public virtual VARIABLE_TYPE GetVariableType() { return VARIABLE_TYPE.OM; }
        public virtual String GetName() { return Name; }

        /// <summary>Name of this multivector</summary>
        public String Name {get{return m_name;}}


        /// <returns>basis blade array for grade 'g' of the domain.</returns>
        public RefGA.BasisBlade[] DomainForGrade(int g)
        {
            return m_domain[g];
        }

        /// <returns>basis blade array for grade 'g' of the domain.</returns>
        public G25.SMVOM[] DomainSmvForGrade(int g)
        {
            return m_domainSmv[g];
        }

        /// <returns>basis blade array for grade 'g' of the range.</returns>
        public RefGA.BasisBlade[] RangeForGrade(int g)
        {
            return m_range[g];
        }

        /// <summary>All basis blades of domain.</summary>
        public RefGA.BasisBlade[][] Domain { get { return m_domain; } }

        /// <summary>All basis blades of range.</summary>
        public RefGA.BasisBlade[][] Range { get { return m_range; } }

        /// <summary>
        /// All the vectors which span the range. These have a symbolic multiplier "c" + idx for easy
        /// use with FindTighestMatch.
        /// </summary>
        public RefGA.BasisBlade[] RangeVectors { get { return m_rangeVectors; } }

        /// <summary>
        /// All the vectors which span the domain.
        /// </summary>
        public RefGA.BasisBlade[] DomainVectors { get { return m_domainVectors; } }

        protected void SanityCheck(RefGA.BasisBlade[][] L, int spaceDim, String[] bvNames, String domainOrRangeStr)
        {
            bool[] present = new bool[1 << spaceDim];
            for (int i = 0; i < L.Length; i++) {
                for (int j = 0; j < L[i].Length; j++)
                {
                    if (L[i][j].bitmap > (1 << spaceDim))
                        throw new G25.UserException("In outermorphism type " + Name + ":\n" + 
                            "Basis blade is not contained within the space of the algebra in " + domainOrRangeStr + ".");
                    if (present[L[i][j].bitmap])
                        throw new G25.UserException("In outermorphism type " + Name + ":\n" + 
                            "Basis blade '" + L[i][j].ToString(bvNames) + "'is listed twice in " + domainOrRangeStr + ".");
                    present[L[i][j].bitmap] = true;
                }
            }
        }

        /// <summary>
        /// Throws an exception when the same basis blade is listed twice in either the domain or the range, or
        /// when a basis blade is not contained in the algebra space. 
        /// </summary>
        /// <param name="spaceDim">Dimension of space of the algebra.</param>
        /// <param name="bvNames">Names of all basis vectors (used for converting basis blades to strings when throwing the exception).</param>
        public virtual void SanityCheck(int spaceDim, String[] bvNames)
        {
            SanityCheck(m_domain, spaceDim, bvNames, "domain");
            SanityCheck(m_range, spaceDim, bvNames, "range");
        }

        /// <returns>true if order of domain basis blades matches 'L' (used to see if basis blades are in the default order and orientation).</returns>
        public bool CompareDomainOrder(RefGA.BasisBlade[][] L)
        {
            if (L.Length != m_domain.Length) return false;
            for (int g = 0; g < L.Length; g++)
            {
                if (L[g].Length != m_domain[g].Length) return false;
                for (int j = 0; j < L[g].Length; j++)
                    if ((L[g][j].CompareTo(m_domain[g][j])) != 0)
                        return false;
            }
            return true;
        }

        /// <returns>true when domain and range are equal.</returns>
        public bool DomainAndRangeAreEqual()
        {
            if (m_domain.Length != m_range.Length)
                return false;

            for (int g = 0; g < m_domain.Length; g++)
            {
                if (m_domain[g].Length != m_range[g].Length)
                    return false;
                for (int i = 0; i < m_domain[g].Length; i++)
                {
                    if (m_domain[g][i].CompareTo(m_range[g][i]) != 0)
                        return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Helper function which takes the blades listed in 'B' and returns them sorted by grade
        /// in a double array. The relative order of blades of each grade left constant.
        /// </summary>
        /// <param name="B"></param>
        /// <param name="spaceDim">dimension of space of the algebra; used to determine the length of the returned double array.</param>
        /// <returns>'B' in an array of arrays, sorted by grade.</returns>
        private RefGA.BasisBlade[][] SortByGrade(RefGA.BasisBlade[] B, int spaceDim)
        {
            RefGA.BasisBlade[][] L = new RefGA.BasisBlade[spaceDim + 1][];
            { // count, allocate number of elements of each grade
                // count number of blades for each grade
                int[] count = new int[spaceDim + 1];
                for (int i = 0; i < B.Length; i++)
                {
                    if (B[i].Grade() >= count.Length)
                        throw new Exception("G25.OM.SortByGrade(): unexpectedly high grade of basis blade: " + B[i].Grade());
                    else count[B[i].Grade()]++;
                }

                // allocate
                for (int g = 0; g <= spaceDim; g++)
                {
                    L[g] = new RefGA.BasisBlade[count[g]];
                }
            }

            // sort all blades:
            int[] idx = new int[spaceDim + 1];
            for (int i = 0; i < B.Length; i++)
            {
                L[B[i].Grade()][idx[B[i].Grade()]] = B[i];
                idx[B[i].Grade()]++;
            }

            return L;
        }

        /// <summary>
        /// This function takes the domain or range arrays (<c>RefGA.BasisBlade[][]</c>)
        /// and returns the basis vectors which span this domain or range.
        /// 
        /// If <c>symbolicName</c> is not null, the vectors are given symbolic coordinates,
        /// <c>symbolicName0</c>, <c>symbolicName1</c>, and so on.
        /// </summary>
        /// <param name="D">The double array (typically domain or range of this OM)</param>
        /// <param name="symbolicName">Can be null. Optional symbolic coordinate base.</param>
        /// <returns></returns>
        protected static RefGA.BasisBlade[] GetVectors(RefGA.BasisBlade[][] D, string symbolicName)
        { // find out which vectors are in range, and create array of basis blades representing it as a vector type
            // get union of all bitmaps
            uint unionBitmap = 0;
            for (int g = 0; g < D.Length; g++)
            {
                for (int c = 0; c < D[g].Length; c++)
                {
                    unionBitmap |= D[g][c].bitmap;
                }
            }
            // allocate memory for vectors
            uint nbVectors = RefGA.Bits.BitCount(unionBitmap);
            RefGA.BasisBlade[] vectors = new RefGA.BasisBlade[nbVectors];
            // init vectors
            uint v = 1;
            int idx = 0;
            while (v <= unionBitmap)
            {
                if ((v & unionBitmap) != 0)
                {
                    if (symbolicName == null)
                        vectors[idx] = new RefGA.BasisBlade(v);
                    else vectors[idx] = new RefGA.BasisBlade(v, 1.0, symbolicName + idx); // note the symbolic coordinate (for easy FindTightestMatch())
                    idx++;
                }
                v = v << 1;
            }
            return vectors;
        }

        /// <summary>
        /// Returns a mapping from this OM to dstOM
        /// </summary>
        /// <param name="dstOm"></param>
        /// <returns>A dictionary from source(grade, column, row) to dest(column, row, multiplier). 
        /// column = domainIdx, row = rangeIdx.
        /// </returns>
        public Dictionary<Tuple<int, int, int>, Tuple<int, int, double>> getMapping(OM dstOm)
        {
            Dictionary<Tuple<int, int, int>, Tuple<int, int, double>> map = new Dictionary<Tuple<int, int, int>, Tuple<int, int, double>>();
            // loop over all grades, loop over all entries of each grade to find match
            for (int gradeIdx = 0; gradeIdx < dstOm.Domain.Length; gradeIdx++)
            {
                for (int dstDomainIdx = 0; dstDomainIdx < dstOm.Domain[gradeIdx].Length; dstDomainIdx++)
                {
                    for (int srcDomainIdx = 0; srcDomainIdx < this.Domain[gradeIdx].Length; srcDomainIdx++)
                    {
                        // check if domain matches
                        if (this.Domain[gradeIdx][srcDomainIdx].bitmap != dstOm.Domain[gradeIdx][dstDomainIdx].bitmap) continue;
                        for (int dstRangeIdx = 0; dstRangeIdx < dstOm.Range[gradeIdx].Length; dstRangeIdx++)
                        {
                            for (int srcRangeIdx = 0; srcRangeIdx < this.Range[gradeIdx].Length; srcRangeIdx++)
                            {
                                // check if range matches
                                if (this.Range[gradeIdx][srcRangeIdx].bitmap != dstOm.Range[gradeIdx][dstRangeIdx].bitmap) continue;

                                // compute multiplier 
                                double multiplier = (dstOm.Range[gradeIdx][dstRangeIdx].scale / this.Range[gradeIdx][srcRangeIdx].scale) *
                                    (dstOm.Domain[gradeIdx][dstDomainIdx].scale / this.Domain[gradeIdx][srcDomainIdx].scale);

                                map.Add(new Tuple<int, int, int>(gradeIdx, srcDomainIdx, srcRangeIdx),
                                    new Tuple<int, int, double>(dstDomainIdx, dstRangeIdx, multiplier));

                            } // end of loop over srcRangeIdx
                        } // end of loop over dstRangeIdx
                    } // end of loop over srcDomainIdx
                } // end of loop over dstDomainIdx
            } // end of loop over gradeIdx

            return map;
        }


        public override string ToString()
        {
            return Name;
        }


        /// <summary>
        /// Returns index into matrix of gradeIdx for entry [domainIdx, rangeIdx]  or 
        /// if matrix is not transposed: [column, row]
        /// </summary>
        public int getCoordinateIndex(int gradeIdx, int domainIdx, int rangeIdx) {
            return DomainForGrade(gradeIdx).Length * rangeIdx + domainIdx;
        }


        /// <summary>Whether this multivector is specialized (true) or general (false).</summary>
        public bool Specialized { get { return m_specialized; } }

        /// <summary>
        /// Whether this multivector is specialized or not.
        /// </summary>
        protected readonly bool m_specialized;

        /// <summary>
        /// The name of the multivector
        /// </summary>
        protected readonly String m_name;

        protected readonly RefGA.BasisBlade[][] m_domain;
        protected readonly RefGA.BasisBlade[][] m_range;

        protected readonly G25.SMVOM[][] m_domainSmv;

        /// <summary>
        /// The vectors that span the domain of this OM.
        /// </summary>
        protected readonly RefGA.BasisBlade[] m_domainVectors;

        /// <summary>
        /// The vectors that span the range of this OM. They all have a symbolic coordinate.
        /// This allows for easy lookup of a vector type which can store them.
        /// </summary>
        protected readonly RefGA.BasisBlade[] m_rangeVectors;

    } // end of class OM


    /// <summary>
    /// This class contains the specification of a general outermorphism. 
    /// 
    /// The only difference so far is in the sanity check (both range and domain must be full).
    /// </summary>
    public class GOM : OM
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the outermorphism, for example "om" or "flatPointOM".</param>
        /// <param name="domain">The basis blades of the domain. These are sorted by grade by the constructor.</param>
        /// <param name="range">The basis blades of the range (can be null, then the domain is copied). These are sorted by grade by the constructor.</param>
        /// <param name="spaceDim">The dimension of the space of the algebra (used to know what the maximum grade is).</param>
        public GOM(String name, RefGA.BasisBlade[] domain, RefGA.BasisBlade[] range, int spaceDim)
            :
            base(false, name, domain, range, spaceDim)  // false means 'not specialized'
        {
        }

        public override VARIABLE_TYPE GetVariableType() { return VARIABLE_TYPE.GOM; }

        protected void SanityCheckAllBladesPresent(RefGA.BasisBlade[][] L, int spaceDim, String[] bvNames, String domainOrRangeStr)
        {
            bool[] present = new bool[1 << spaceDim];
            for (int i = 0; i < L.Length; i++) {
                for (int j = 0; j < L[i].Length; j++)
                {
                    present[L[i][j].bitmap] = true;
                }
            }
            for (int i = 0; i < present.Length; i++)
                if (!present[i])
                    throw new G25.UserException("In outermorphism type " + Name + ":\n" + 
                        "Missing basis blade '" + new RefGA.BasisBlade((uint)i).ToString(bvNames) + "' in " + domainOrRangeStr);
        }

        /// <summary>
        /// Throws an exception when the same basis blade is listed twice in either the domain or the range, or
        /// when a basis blade is not contained in the algebra space, or when not all
        /// basis blades are present in both range and domain.
        /// </summary>
        /// <param name="spaceDim">Dimension of space of the algebra.</param>
        /// <param name="bvNames">Names of all basis vectors (used for converting basis blades to strings when throwing the exception).</param>
        public override void SanityCheck(int spaceDim, String[] bvNames)
        {
            base.SanityCheck(spaceDim, bvNames);
            SanityCheckAllBladesPresent(m_domain, spaceDim, bvNames, "domain");
            SanityCheckAllBladesPresent(m_range, spaceDim, bvNames, "range");
        }
    } // end of class GOM

    /// <summary>
    /// This class contains the specification of a specialized outermorphism. 
    /// 
    /// (no difference with G25.OM so far.
    /// </summary>
    public class SOM : OM
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the outermorphism, for example "om" or "flatPointOM".</param>
        /// <param name="domain">The basis blades of the domain. These are sorted by grade by the constructor.</param>
        /// <param name="range">The basis blades of the range (can be null, then the domain is copied). These are sorted by grade by the constructor.</param>
        /// <param name="spaceDim">The dimension of the space of the algebra (used to know what the maximum grade is).</param>
        public SOM(String name, RefGA.BasisBlade[] domain, RefGA.BasisBlade[] range, int spaceDim)
            :
            base(true, name, domain, range, spaceDim)  // true means 'specialized'
        {
        }

        /// <param name="g">grade index</param>
        /// <returns>true when domain and/or range for grade 'd' is empty.</returns>
        public bool EmptyGrade(int g)
        {
            return ((DomainForGrade(g).Length * RangeForGrade(g).Length) == 0);
        }

        public override VARIABLE_TYPE GetVariableType() { return VARIABLE_TYPE.SOM; }

    } // end of class SOM


} // end of namespace G25
