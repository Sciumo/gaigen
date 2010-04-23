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
    /// This class contains the specification of a multivector. This can be either a general ( G25.GMV )
    /// or specialized ( G25.SMV ) multivector. The superclass contains:
    ///   - the name of the multivector (for example "mv", "vector" or "rotor").
    ///   - order of coordinates, and what coordinates should be present.
    /// 
    /// Coordinates can be stored in 'groups'. Specialized multivectors contains only
    /// one group, while general multivectors can have multiple groups.
    /// </summary>
    public class MV : VariableType
    {
        /// <summary>
        /// Constructor. Do not use directly. Use the constructors of G25.GMV and
        /// G25.SMV instead.
        /// 
        /// If the multivector is specialized, basisBlades.Length must be 1.
        /// </summary>
        /// <param name="specialized">Whether this is a specialized MV or not.</param>
        /// <param name="name">The name of the multivector, for example "mv" or "rotor".</param>
        /// <param name="basisBlades">The basis blades, by group. Each entry in the array is a group of coordinates. 
        /// Specialized multivector have only one group.</param>
        protected MV(bool specialized, String name, RefGA.BasisBlade[][] basisBlades)
        {
            if (specialized && (basisBlades.Length != 1))
                throw new Exception("G25.MV(): specialized multivector classes must have one group");

            m_specialized = specialized;
            m_name = name;
            m_basisBlades = new RefGA.BasisBlade[basisBlades.Length][];
            for (int i = 0; i < basisBlades.Length; i++)
            {
                m_basisBlades[i] = (RefGA.BasisBlade[])basisBlades[i].Clone();
            }

            { // init m_bitmapToGroup
                // get maximum dimension used in any of the basis blades:
                int maxDim = 0;
                for (int i = 0; i < m_basisBlades.Length; i++)
                {
                    for (int j = 0; j < m_basisBlades[i].Length; j++)
                    {
                        int d = RefGA.Bits.HighestOneBit(m_basisBlades[i][j].bitmap);
                        if (d > maxDim) maxDim = d;
                    }
                }
                // allocate m_bitmapToGroup, set all to -1
                m_bitmapToGroup = new int[1 << (maxDim+1)];
                for (int i = 0; i < m_bitmapToGroup.Length; i++)
                    m_bitmapToGroup[i] = -1;

                // mark used position with group and element index
                for (int i = 0; i < m_basisBlades.Length; i++)
                    for (int j = 0; j < m_basisBlades[i].Length; j++)
                        m_bitmapToGroup[m_basisBlades[i][j].bitmap] = (i << 16) | j;
            }
        }

        public virtual VARIABLE_TYPE GetVariableType() { return VARIABLE_TYPE.MV; }

        public virtual string GetName() { return Name; }

        /// <summary>Name of this multivector</summary>
        public string Name { get { return m_name; } }

        /// <summary>Number of groups</summary>
        public int NbGroups { get { return m_basisBlades.Length; } }

        /// <summary>
        /// Number of non-constant coordinates.
        /// </summary>
        public int NbCoordinates { 
         get {
             int cnt = 0;
             for (int i = 0; i < NbGroups; i++) cnt += Group(i).Length;
             return cnt;
        } 
        }

        /// <summary>Index is not checked.</summary>
        /// <returns>basis blade array for group 'idx'.</returns>
        public RefGA.BasisBlade[] Group(int idx)
        {
            return m_basisBlades[idx];
        }

        /// <summary>Indices are not checked.</summary>
        /// <returns>basis blade indexed by group, basis blade index.</returns>
        public RefGA.BasisBlade BasisBlade(int groupIdx, int basisBladeIdx)
        {
            return m_basisBlades[groupIdx][basisBladeIdx];
        }

        /// <summary>All basis blades.</summary>
        public RefGA.BasisBlade[][] BasisBlades { get { return m_basisBlades; } }

        
        /// <summary>
        /// Throws an exception when the same basis blade is listed twice, or
        /// when a basis blade is not contained in the algebra space. 
        /// </summary>
        /// <param name="S">Specification of the algebra.</param>
        /// <param name="bvNames">Names of all basis vectors (used for converting basis blades to strings when throwing the exception).</param>
        public virtual void SanityCheck(Specification S, String[] bvNames)
        {
            int spaceDim = S.m_dimension;
            bool[] present = new bool[1 << spaceDim];
            for (int i = 0; i < m_basisBlades.Length; i++) {
                for (int j = 0; j < m_basisBlades[i].Length; j++)
                {
                    if (m_basisBlades[i][j].bitmap > (1 << spaceDim))
                        throw new Exception("MV.SanityCheck: basis blade is not contained within the space of the algebra");
                    if (present[m_basisBlades[i][j].bitmap])
                        throw new G25.UserException("Basis blade '" + m_basisBlades[i][j].ToString(bvNames) + "' is listed more than once in multivector type " + Name + ".");
                    present[m_basisBlades[i][j].bitmap] = true;
                }
            }
        }

        /// <returns>true if m_basisBlades matches 'L' (used to see if basis blades are in the default order and orientation).</returns>
        public bool CompareBasisBladeOrder(RefGA.BasisBlade[][] L)
        {
            if (L.Length != m_basisBlades.Length) return false;
            for (int i = 0; i < L.Length; i++)
            {
                if (L[i].Length != m_basisBlades[i].Length) return false;
                for (int j = 0; j < L[i].Length; j++)
                    if ((L[i][j].CompareTo(m_basisBlades[i][j])) != 0)
                        return false;
            }
            return true;

        }

        /// <param name="dim">Dimension of the space of the algebra.</param>
        /// <returns>true if this multivector is grouped according to grade.</returns>
        public bool IsGroupedByGrade(int dim) {
            if (m_basisBlades.Length != (dim + 1)) return false;
            for (int i = 0; i < m_basisBlades.Length; i++)
            {
                for (int j = 0; j < m_basisBlades[i].Length; j++)
                {
                    if (m_basisBlades[i][j].Grade() != i)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns the sum of all lengths of groups with index smaller than 'groupIdx'. This is
        /// used by functions over GMVs which need to know where the coordinates for a certain
        /// grade start.
        /// </summary>
        /// <param name="groupIdx">group whose starting index you want to know.</param>
        /// <returns>sum of all lengths of groups with index smaller than 'groupIdx'.</returns>
        public int GroupStartIdx(int groupIdx)
        {
            int idx = 0;
            for (int g = 0; g < groupIdx; g++)
            {
                idx += Group(g).Length;
            }
            return idx;
        }

        /// <summary>Whether this multivector is specialized (true) or general (false).</summary>
        public bool Specialized { get { return m_specialized; } }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Returns the group index of basis blade 'B' or -1 when not represented by this MV.
        /// 
        /// Example: suppose you want to know the group and element index of B=e2^e3^no,
        /// then (assuming it is represented), it is in m_basisBlades[GetGroupIdx(B)][GetElementIdx(B)].
        /// </summary>
        /// <param name="B">The basis blade whose bitmap is looked up.</param>
        /// <returns>-1 if 'B' is cannot be represented by this MV, otherwise, the index of the group.</returns>
        public int GetGroupIdx(RefGA.BasisBlade B)
        {
            if ((B.bitmap >= m_bitmapToGroup.Length) || (m_bitmapToGroup[B.bitmap] < 0)) return -1;
            else return m_bitmapToGroup[B.bitmap] >> 16;
        }

        /// <summary>
        /// Returns the element index of basis blade 'B' or -1 when not represented by this MV.
        /// 
        /// Example: suppose you want to know the group and element index of B=e2^e3^no,
        /// then (assuming it is represented), it is in BasisBlade(groupIdx, elemIdx).
        /// </summary>
        /// <param name="B">The basis blade whose bitmap is looked up.</param>
        /// <returns>-1 if 'B' is cannot be represented by this MV, otherwise, the index of the element in the group.</returns>
        public int GetElementIdx(RefGA.BasisBlade B)
        {
            if ((B.bitmap >= m_bitmapToGroup.Length) || (m_bitmapToGroup[B.bitmap] < 0)) return -1;
            else return m_bitmapToGroup[B.bitmap] & 0xFFFF;
        }

        /// <summary>
        /// Computes which groups would be used to store the basis blades used in 'B'.
        /// </summary>
        /// <param name="B">A list of basis blades.</param>
        /// <returns>A bitmap representing the groups that would be used to store 'B'.</returns>
        public int GetGroupUsageBitmap(RefGA.BasisBlade[] B)
        {
            int gu = 0;
            foreach (BasisBlade b in B)
            {
                gu |= 1 << GetGroupIdx(b);
            }
            return gu;
        }

        /// <returns>a bitmap. For each basis vector used in the basis blades, the respective bit is 1.</returns>
        public uint BasisVectorBitmap()
        {
            uint bitmap = 0;
            for (int g = 0; g < m_basisBlades.Length; g++)
            {
                for (int e = 0; e < m_basisBlades[g].Length; e++)
                {
                    bitmap |= m_basisBlades[g][e].bitmap;
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Generates a map. For each coordinate in 'src', tells you where it is found in 'dst',
        /// or that it is not present.
        /// </summary>
        /// <param name="src">Src multivector (can be SMV or GMV).</param>
        /// <param name="dst">Dst multivector (can be SMV or GMV).</param>
        public static Dictionary<Tuple<int, int>, Tuple<int, int>> GetCoordMap(MV src, MV dst)
        {
            Dictionary<Tuple<int, int>, Tuple<int, int>> D = new Dictionary<Tuple<int, int>, Tuple<int, int>>();
            for (int g = 0; g < src.m_basisBlades.Length; g++)
            {
                for (int e = 0; e < src.m_basisBlades[g].Length; e++)
                {
                    RefGA.BasisBlade srcB = src.m_basisBlades[g][e];
                    int groupIdx = dst.GetGroupIdx(srcB);
                    if (groupIdx >= 0)
                    {
                        int elementIdx = dst.GetElementIdx(srcB);
                        D[new Tuple<int, int>(g, e)] = new Tuple<int, int>(groupIdx, elementIdx);
                    }
                }
            }
            return D;
        }

        /// <returns>true if the basis blade scales of A and B match. Blades which are not present in both A and B are ignored</returns>
        public static bool BasisBladeScalesMatch(MV A, MV B)
        {
            if (A == null)
            {
                Console.WriteLine("Arrr!");
            }
            if (B == null)
            {
                Console.WriteLine("Arrr!");
            }
            Dictionary<Tuple<int, int>, Tuple<int, int>> D = GetCoordMap(A, B);
            foreach (KeyValuePair<Tuple<int, int>, Tuple<int, int>> kvp in D) 
            {
                BasisBlade a = A.Group(kvp.Key.Value1)[kvp.Key.Value2];
                BasisBlade b = B.Group(kvp.Value.Value1)[kvp.Value.Value2];

                if (a.scale != b.scale) return false;
            }
 
            return true;
        }

        /// <summary>
        /// Returns lowest grade basis blade in this MV.
        /// </summary>
        /// <returns>lowest grade basis blade in this MV.</returns>
        public int LowestGrade()
        {
            int lg = int.MaxValue;
            for (int g = 0; g < m_basisBlades.Length; g++)
            {
                for (int e = 0; e < m_basisBlades[g].Length; e++)
                {
                    RefGA.BasisBlade srcB = m_basisBlades[g][e];
                    if (srcB.Grade() < lg) lg = srcB.Grade();
                }
            }
            return lg;
        }

        /// <summary>
        /// Returns highest grade basis blade in this MV.
        /// </summary>
        /// <returns>highest grade basis blade in this MV.</returns>
        public int HighestGrade()
        {
            int hg = -1;
            for (int g = 0; g < m_basisBlades.Length; g++)
            {
                for (int e = 0; e < m_basisBlades[g].Length; e++)
                {
                    RefGA.BasisBlade srcB = m_basisBlades[g][e];
                    if (srcB.Grade() > hg) hg = srcB.Grade();
                }
            }
            return hg;
        }

        /// <param name="P">true = odd, false = even.</param>
        /// <returns>Whether this MV has a parity of 'P'.</returns>
        private bool TestParity(Boolean P)
        {
            for (int g = 0; g < m_basisBlades.Length; g++)
            {
                for (int e = 0; e < m_basisBlades[g].Length; e++)
                {
                    if (((m_basisBlades[g][e].Grade() & 1) == 0) != P) return false;
                }
            }
            return true;
        }

        /// <returns>true when this multivector has only even basis blades</returns>
        public Boolean IsEven()
        {
            return TestParity(false);
        }

        /// <returns>true when this multivector has only even basis blades</returns>
        public bool IsOdd()
        {
            return TestParity(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True when this MV is either even or odd. </returns>
        public bool IsParityPure()
        {
            return IsEven() || IsOdd();
        }

        /// <returns>true when this type can be converted to a general multivector
        /// This depends on whether this type is parity pure, and parity pure memory allocation is being used for GMV.</returns>
        public bool CanConvertToGmv(Specification S)
        {
            if (S.m_GMV.MemoryAllocationMethod == GMV.MEM_ALLOC_METHOD.PARITY_PURE)
                return IsParityPure();
            else return true;
        }

        /// <summary>
        /// Returns 'unit' multivector value (i.e., a multivector constructed from the unit basis 
        /// blades). 
        /// </summary>
        /// <returns></returns>
        public RefGA.Multivector ToMultivectorValue()
        {
            RefGA.Multivector sum = RefGA.Multivector.ZERO;

            foreach (RefGA.BasisBlade[] BL in BasisBlades)
            {
                if (BL != null)
                    sum = RefGA.Multivector.Add(sum, new RefGA.Multivector(BL));
            }

            return sum;
        }


        /// <summary>
        /// Whether this multivector is specialized or not.
        /// </summary>
        protected readonly bool m_specialized;

        /// <summary>
        /// The name of the multivector
        /// </summary>
        protected readonly string m_name;

        /// <summary>
        /// The basis blade order and grouping.
        /// The first index represents the group (specialized multivectors have only one group),
        /// the second index represents the basis blade in that group;
        /// </summary>
        protected readonly RefGA.BasisBlade[][] m_basisBlades;

        /// <summary>
        /// A 'map' which translates a basis blade bitmap to an index in a group.
        /// Suppose you want to know where e1^e2, then m_bitmapToGroup[1 ^ 2] will
        /// tell you where. Upper 16 bits: group, lower 16 bits: index inside the group.
        /// 
        /// Use .. to access this information.
        /// </summary>
        protected readonly int[] m_bitmapToGroup;


    } // end of class MV

    /// <summary>
    /// This class contains the specification of a general multivector. 
    /// 
    /// The only new property is the memory allocation method.
    /// </summary>
    public class GMV : MV
    {
        public enum MEM_ALLOC_METHOD {
            PARITY_PURE = 1, // allocate half the memory (for general multivector coordinates)
            FULL = 2, // allocate full memory (for general multivector coordinates)
            DYNAMIC = 3 // dynamically allocate memory as required (for general multivector coordinates)
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the multivector, for example "mv" or "rotor".</param>
        /// <param name="basisBlades">The basis blades, by group. Each entry in the array is a group of coordinates.</param>
        /// <param name="m">Memory allocation method.</param>
        public GMV(String name, RefGA.BasisBlade[][] basisBlades, MEM_ALLOC_METHOD m)
            : 
            base(false, name, basisBlades)  // false means 'not specialized'
        {
            m_memoryAllocationMethod = m;
        }

        public override  VARIABLE_TYPE GetVariableType() { return VARIABLE_TYPE.GMV; }

        /// <summary>
        /// Throws an exception when MV.SanityCheck(int spaceDim, String[] bvNames) throws an exception, or when not all basis blades are listed,
        /// or when blades of different grades are in the same group.
        /// </summary>
        /// <param name="S">Specification of the algebra.</param>
        /// <param name="bvNames">Names of all basis vectors (used for converting basis blades to strings when throwing the exception).</param>
        public override void SanityCheck(Specification S, String[] bvNames)
        {
            base.SanityCheck(S, bvNames);

            int spaceDim = S.m_dimension;
            bool[] present = new bool[1 << spaceDim];
            for (int i = 0; i < m_basisBlades.Length; i++)
            {
                int grade = -1;
                for (int j = 0; j < m_basisBlades[i].Length; j++)
                {
                    if (j == 0) grade = m_basisBlades[i][j].Grade();
                    else if (m_basisBlades[i][j].Grade() != grade)
                        throw new G25.UserException("In the general multivector type " + Name + ":\n"+
                            "Group " + (i+1) + " contains blades of different grades.");

                    present[m_basisBlades[i][j].bitmap] = true;
                }
            }

            if ((S.m_outputLanguage == OUTPUT_LANGUAGE.C) && 
                (MemoryAllocationMethod == MEM_ALLOC_METHOD.DYNAMIC))
                throw new G25.UserException("Dynamic memory allocation of general multivector type " + Name + " is not supported for the 'C' language.");


            for (int i = 0; i < (1 << spaceDim); i++)
                if (!present[i]) 
                    throw new G25.UserException("In the general multivector type " + Name + ":\n"+
                    "Missing basis blade '" + (new RefGA.BasisBlade((uint)i)).ToString(bvNames) + "'.");
        }

        
        /// <summary>
        /// Computes whether the group <c>gd</c> part of a geometric product of group <c>g1</c> and group <c>g2</c>
        /// would be zero in general. Result is based on grades. So there may be some false positives.
        /// To be non-zero, <c>|grade1 - grade2| <=  gradeD <= (grade1 - grade2)</c> and gradeD must be an
        /// even number of steps away from  <c>(grade1 - grade2)</c>
        /// </summary>
        /// <remarks>Assumes that one group contains only one grade.</remarks>
        /// <param name="g1">Input group 1.</param>
        /// <param name="g2">Input group 2.</param>
        /// <param name="gd">Destination group.</param>
        /// <returns>true if part 'gd' of g1 x g2 == 0</returns>
        public bool IsZeroGP(int g1, int g2, int gd) 
        {
            int grade1 = Group(g1)[0].Grade();
            int grade2 = Group(g2)[0].Grade();
            int gradeD = Group(gd)[0].Grade();

            if (gradeD < Math.Abs(grade1 - grade2)) return true;
            else if (gradeD > (grade1 + grade2)) return true;
            else if ((((grade1 + grade2) - gradeD) & 1) == 1) return true;
            else return false;

            /*
            //          int grade1 = Group(g1)[0].Grade();
            //            int grade2 = Group(g2)[0].Grade();
            int gradeD = Group(gd)[0].Grade();

            if (gradeD < Math.Abs(g1 - g2)) return true;
            else if (gradeD > (g1 + g2)) return true;
            else if ((((g1 + g2) - gd) & 1) == 1) return true;
            else return false;
             */
        }


        /// <summary>Memory allocation method</summary>
        public MEM_ALLOC_METHOD MemoryAllocationMethod { get { return m_memoryAllocationMethod; } }

        /// <summary>
        /// How much memory is allocated for storing coordinates in the generated code.
        ///  - PARITY_PURE means only half the amount of memory required for all coordinates is allocated.
        ///  - FULL means all coordinates are allocated.
        /// </summary>
        protected readonly MEM_ALLOC_METHOD m_memoryAllocationMethod;
    } // end of class GMV

    /// <summary>
    /// This class contains the specification of a specialized multivector. 
    /// 
    /// Extra properties: whether the SMV is constant or not, and what the type
    /// of the multivector variable should be (blade, rotor, versor)
    /// </summary>
    public class SMV : MV
    {
        public override VARIABLE_TYPE GetVariableType() { return VARIABLE_TYPE.SMV; }

        /// <summary>
        /// Multivector types. May be used by some code generators to 
        /// take shortcuts (e.g., if a variable is a rotor, then the reverse is guaranteed to be equal to the inverse, etc),
        /// or to avoid generating invalid code (e.g., don't try to outer-factor a versor).
        /// 
        /// In the specification, the type is set using the type="..." attribute.
        /// </summary>
        public enum MULTIVECTOR_TYPE
        {
            MULTIVECTOR = 0, // a general multivector
            BLADE = 1, // outer-factorizable homogeneous multivector.
            ROTOR = 2, // invertible, even grade geometric-product-factorizable multivector with unit norm, can be written as exponential of bivector.
            VERSOR = 3 // invertible geometric product of vectors (the rotors are a subclass of the versors).
        }

        /// <summary>
        /// Little helper function for the constructor.
        /// </summary>
        /// <returns>'basisBlades', but inside a newly allocated array of length 1.</returns>
        protected static RefGA.BasisBlade[][] ToDoubleArray(RefGA.BasisBlade[] basisBlades) {
            RefGA.BasisBlade[][] B = new RefGA.BasisBlade[1][];
            B[0] = basisBlades;
            return B;
        }

        /// <summary>
        /// Little helper function for constructor.
        /// </summary>
        /// <returns>'basisBlades', but inside a newly allocated array of length 1.</returns>
        protected static RefGA.BasisBlade[][] ToDoubleArray(G25.rsbbp.BasisBlade[] basisBlades)
        {
            RefGA.BasisBlade[][] B = new RefGA.BasisBlade[1][];
            B[0] = new RefGA.BasisBlade[basisBlades.Length];
            for (int i = 0; i < basisBlades.Length; i++)
                B[0][i] = basisBlades[i].GetBasisBlade;
            return B;
        }



        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the multivector, for example "mv" or "rotor".</param>
        /// <param name="basisBlades">The basis blades.</param>
        /// <param name="isConstant">Some coordinates may have constant values. If a coordinate is constant, its respective entry in isConstant is true (May be null).</param>
        /// <param name="constantValues">Some coordinates may have constant values. This array lists the values for each basis blade (May be null).</param>
        /// <param name="mvType">Type of multivector (</param>
        /// <param name="comment">A comment on the multivector type that may appear in the generated documentation. May be null.</param>
        public SMV(string name, RefGA.BasisBlade[] basisBlades, MULTIVECTOR_TYPE mvType, 
            bool[] isConstant, double[] constantValues, String comment)
            :
            base(true, name, ToDoubleArray(basisBlades))  // true means 'specialized'
        {

            // sanity check
            if (comment == null) comment = "";
            if (((isConstant != null) && (basisBlades.Length != isConstant.Length)) ||
                ((constantValues != null) && (basisBlades.Length != constantValues.Length)))
                throw new Exception("G25.SMV(): the 'isConstant' array or the 'constantValues' array do not match the length of the 'basisBlades' array");

            m_mvType = mvType;
            m_isConstant = (isConstant == null) ? new bool[basisBlades.Length]: (bool[])isConstant.Clone();
            m_constantValues = (constantValues == null) ? new double[basisBlades.Length] : (double[])constantValues.Clone();
            m_comment = comment;

            // count number of constant coordinates
            int nbConstant = GetNbConstantCoordinates();

            // sanity check number of constant coordinates
            /*if ((m_constantName != null) &&
                (nbConstant != basisBlades.Length) &&
                (nbConstant != m_constantValues.Length))
                throw new Exception("G25.SMV(): not all coordinates of the constant '" + m_constantName + "' are constant");*/

            InitConstBasisBladeIdx(nbConstant);
        }

        /// <summary>
        /// Alternative constructor which uses G25.rsbbp.BasisBlade to carry both BasisBlade and constant info
        /// in one list of variables.
        /// </summary>
        /// <param name="name">The name of the multivector, for example "mv" or "rotor".</param>
        /// <param name="basisBlades">The basis blades and possibly constant values.</param>
        /// <param name="mvType">Type of multivector (</param>
        /// <param name="comment">A comment on the multivector type that may appear in the generated documentation. May be null.</param>
        public SMV(string name, G25.rsbbp.BasisBlade[] basisBlades, MULTIVECTOR_TYPE mvType, string comment)
            :
            base(true, name, ToDoubleArray(basisBlades))  // true means 'specialized'
        {
            if (comment == null) comment = "";

            m_mvType = mvType;
            m_isConstant = new bool[basisBlades.Length];
            m_constantValues = new double[basisBlades.Length];
            for (int i = 0; i < basisBlades.Length; i++)
            {
                m_isConstant[i] = basisBlades[i].IsConstant;
                m_constantValues[i] = (m_isConstant[i]) ? basisBlades[i].ConstantValue : 0.0;
            }
            m_comment = comment;

            // count number of constant coordinates
            int nbConstant = GetNbConstantCoordinates();

            /*// sanity check number of constant coordinates
            if ((m_constantName != null) &&
                (nbConstant != basisBlades.Length) &&
                (nbConstant != m_constantValues.Length))
                throw new Exception("G25.SMV(): not all coordinates of the constant '" + m_constantName + "' are constant");*/

            InitConstBasisBladeIdx(nbConstant);
        }

        /// <summary>
        /// Returns true if only grade 2 basis blades in this SMV.
        /// </summary>
        /// <returns>true if only grade 2 basis blades in this SMV.</returns>
        public bool IsBivector()
        {
            foreach (RefGA.BasisBlade B in BasisBlades[0])
            {
                if (B.Grade() != 2) return false;
            }
            return true;
        }


        /// <summary>
        /// Loops through m_isConstant, counts number of constant coordinates.
        /// </summary>
        /// <returns>number of constant coordinates.</returns>
        private int GetNbConstantCoordinates()
        {
            int nbConstant = 0;
            for (int i = 0; i < m_isConstant.Length; i++)
                if (m_isConstant[i]) nbConstant++;
            return nbConstant;
        }

        /// <summary>
        /// Initializes m_constBasisBladeIdx, m_nonConstBasisBladeIdx (called by constructor).
        /// </summary>
        private void InitConstBasisBladeIdx(int nbConstant)
        { 
            m_constBasisBladeIdx = new int[nbConstant];
            m_nonConstBasisBladeIdx = new int[m_isConstant.Length - nbConstant];

            int idxConst = 0, idxNonConst = 0;
            for (int i = 0; i < m_isConstant.Length; i++)
            {
                if (m_isConstant[i])
                {
                    m_constBasisBladeIdx[idxConst++] = i;
                }
                else m_nonConstBasisBladeIdx[idxNonConst++] = i;
            }
        }

        /// <summary>
        /// Throws an exception when MV.SanityCheck(int spaceDim, String[] bvNames) throws an exception.
        /// </summary>
        /// <param name="S">Specification of the algebra.</param>
        /// <param name="bvNames">Names of all basis vectors (used for converting basis blades to strings when throwing the exception).</param>
        public override void SanityCheck(Specification S, String[] bvNames)
        {
            base.SanityCheck(S, bvNames);

            // complain when a converter to GMV cannot be written
            if (!CanConvertToGmv(S)) {
                System.Console.WriteLine("Warning: the type");
                System.Console.WriteLine(ToString());
                System.Console.WriteLine("is not parity pure. The general multivector uses parity pure memory allocation, so " + Name + " type cannot be converted to a general multivector.");
                System.Console.WriteLine("");
            }
        }

        /// <returns>true when this specialized multivector is fully constant.</returns>
        //public bool IsTypeConstant { get { return (m_constantName != null); } }
        /// <returns>the name of the singleton constant that should be generated.</returns>
        //public String ConstantName { get { return m_constantName; } }

        /// <returns>the number of non-constant (variable) blade coordinates.</returns>
        public int NbNonConstBasisBlade { get { return m_nonConstBasisBladeIdx.Length; } }
        /// <returns>non-constant (variable) blade number 'idx'.</returns>
        public BasisBlade NonConstBasisBlade(int idx) {
            return m_basisBlades[0][m_nonConstBasisBladeIdx[idx]]; 
        }
        /// <returns>Index of non-constant (variable) blade number 'idx' (which is <= 'idx').</returns>
        public int BladeIdxToNonConstBladeIdx(int idx)
        {
            int cnt = 0;
            for (int i = 0; i < idx; i++)
            {
                if (!IsCoordinateConstant(i)) cnt++;
            }
            return cnt;
        }

        /// <returns>Index of constant (variable) blade number 'idx' (which is <= 'idx').</returns>
        public int BladeIdxToConstBladeIdx(int idx)
        {
            int cnt = 0;
            for (int i = 0; i < idx; i++)
            {
                if (IsCoordinateConstant(i)) cnt++;
            }
            return cnt;
        }

        public int ConstBladeIdxToBladeIdx(int idx)
        {
            return m_constBasisBladeIdx[idx];
        }

        /// <returns>true when all coordinates are constant.</returns>
        public bool IsConstant()
        {
            return NbNonConstBasisBlade == 0;
        }

        /// <returns>the number of constant blade coordinates.</returns>
        public int NbConstBasisBlade { get { return m_constBasisBladeIdx.Length; } }
        /// <returns>constant (variable) blade number 'idx'.</returns>
        public BasisBlade ConstBasisBlade(int idx) {
            return m_basisBlades[0][m_constBasisBladeIdx[idx]]; 
        }

        /// <returns>the comment on the type (used for documentation). May be "".</returns>
        public string Comment { get { return m_comment; } }

        /// <returns>the value of constant (variable) blade number 'idx'.</returns>
        public bool IsCoordinateConstant(int idx)
        {
            return m_isConstant[idx];
        }

        /// <param name="idx">Index (based on constant, not on absolute)</param>
        /// <returns>the value of constant (variable) blade number 'idx'.</returns>
        public double ConstBasisBladeValue(int idx) {
            return m_constantValues[m_constBasisBladeIdx[idx]];
        }

        /// <summary>Return multivector type (blade, rotor, versor, multivector)</summary>
        public MULTIVECTOR_TYPE MvType { get { return m_mvType; } }

        /// <summary>Return multivector type (blade, rotor, versor, multivector)</summary>
        public string MvTypeString { get {
            switch (m_mvType) {
                case MULTIVECTOR_TYPE.MULTIVECTOR:
                    return Specification.XML_MULTIVECTOR;
                case MULTIVECTOR_TYPE.BLADE:
                    return Specification.XML_BLADE;

                case MULTIVECTOR_TYPE.ROTOR:
                    return Specification.XML_ROTOR;

                case MULTIVECTOR_TYPE.VERSOR:
                    return Specification.XML_VERSOR;
                default:
                    throw new Exception("G25.SMV.MvTypeString(): invalid multivector type: " + m_mvType);
            }
        } 
        }

        public override string ToString()
        {
            // name[type](...=...)
            StringBuilder SB = new StringBuilder();
            SB.Append(Name + "[" + MvTypeString + "](");
            for (int i = 0; i < Group(0).Length; i++)
            {
                if (i > 0) SB.Append(" ");
                SB.Append(BasisBlade(0, i).ToString());
                if (IsCoordinateConstant(i))
                {
                    SB.Append("=");
                    SB.Append(ConstBasisBladeValue(BladeIdxToConstBladeIdx(i)));
                }
            }
            SB.Append(")");
            return SB.ToString();
        }

        /// <summary>
        /// Returns the language identifier of non-constant coordinate 'nonConstCoordIdx'. Depending
        /// on the specification, the may be a string like "c[1]" or a string like "e1_e2";
        /// </summary>
        /// <param name="nonConstCoordIdx">index of the coordinate (counting only variable coordinates)</param>
        /// <param name="S">The specification (only S.m_coordStorage and S.m_basisVectorNames are used).</param>
        /// <returns>the language identifier of non-constant coordinate 'nonConstCoordIdx'.</returns>
        public string GetCoordLangID(int nonConstCoordIdx, Specification S)
        {
            return GetCoordLangID(nonConstCoordIdx, S, S.m_coordStorage);
        }

        /// <summary>
        /// Returns the language identifier of non-constant coordinate 'nonConstCoordIdx'. Depending
        /// on 'CS', the may be a string like "c[1]" or a string like "e1_e2";
        /// </summary>
        /// <param name="nonConstCoordIdx">index of the coordinate (counting only variable coordinates)</param>
        /// <param name="S">The specification (only S.m_coordStorage and S.m_basisVectorNames are used).</param>
        /// <param name="CS">The way coordinates are stored (VARIABLES or ARRAY).</param>
        /// <returns>the language identifier of non-constant coordinate 'nonConstCoordIdx'.</returns>
        public virtual String GetCoordLangID(int nonConstCoordIdx, Specification S, COORD_STORAGE CS)
        {
            // todo: make adjustment for SMVOM here (or make this function virtual)
            if (CS == COORD_STORAGE.ARRAY)
            {
                //string coordArrayName = (S.m_outputLanguage == OUTPUT_LANGUAGE.C)  ? "c" : "m_c";
                string coordArrayName = "c";
                return coordArrayName + "[" + nonConstCoordIdx + "]";
            }
            else return NonConstBasisBlade(nonConstCoordIdx).ToLangString(S.m_basisVectorNames);
        }

        public double AbsoluteLargestConstantCoordinate()
        {
            double maxValue = 0.0;
            for (int c = 0; c < NbConstBasisBlade; c++)
                maxValue = Math.Max(Math.Abs(ConstBasisBlade(c).scale), maxValue);
            return maxValue;
        }

        public BasisBlade AbsoluteLargestConstantBasisBlade()
        {
            double maxValue = -1.0;
            BasisBlade B = null;
            for (int c = 0; c < NbConstBasisBlade; c++)
            {
                if (Math.Abs(ConstBasisBlade(c).scale) > maxValue)
                {
                    maxValue = Math.Abs(ConstBasisBlade(c).scale);
                    B = ConstBasisBlade(c);
                }
            }
            return B;
        }

        /// <summary>
        /// If a basis blade in m_basisBlades[0] has a constant value assigned to it,
        /// then the respective entry in m_isConstant is true.
        /// </summary>
        protected readonly bool[] m_isConstant;

        /// <summary>
        /// If a basis blade in m_basisBlades[0] has a constant value assigned to it,
        /// then the respective entry in m_constantValues carries that constant coordinate.
        /// </summary>
        protected readonly double[] m_constantValues;

        /// <summary>
        /// Indices of constant basis blades (points into m_basisBlades[0] and m_constantValues)
        /// </summary>
        protected int[] m_constBasisBladeIdx;

        /// <summary>
        /// Indices of non-constant (variable) basis blades (points into m_basisBlades[0] and m_constantValues)
        /// </summary>
        protected int[] m_nonConstBasisBladeIdx;

        /// <summary>
        /// When not null, this specialized multivector is constant. 'm_constantName' is the generated, singleton constant.
        /// </summary>
        //protected readonly String m_constantName;

        /// <summary>
        /// The type of the multivector (MULTIVECTOR, BLADE, ROTOR, VERSOR).
        /// </summary>
        protected readonly MULTIVECTOR_TYPE m_mvType;

        /// <summary>
        /// A comment on the multivector type that may appear in the generated documentation. May be "".
        /// </summary>
        protected readonly string m_comment;
        
    } // end of class SMV

    /// <summary>
    /// This class contains the specification of a specialized multivector which is the column of a outermorphim matrix. 
    /// 
    /// Extra properties: the outermorphism (G25.OM), which grade of the OM, and which column.
    /// The GetCoordLangID() function is overridden.
    /// </summary>
    public class SMVOM : SMV
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the multivector (typically smvom_gradeX_columnY.</param>
        /// <param name="basisBlades">The basis blades.</param>
        /// <param name="parentOM">Parent outermorphism this SMVOM belongs to.</param>
        /// <param name="gradeOM">The grade this SMVOM is for.</param>
        /// <param name="columnOM">The column of the OM matrix this SMVOM  is for.</param>
        public SMVOM(String name, RefGA.BasisBlade[] basisBlades, G25.OM parentOM,
            int gradeOM, int columnOM)
            :
            base(name, basisBlades, MULTIVECTOR_TYPE.MULTIVECTOR, null, null, null)
        {
            m_parentOM = parentOM;
            m_gradeOM = gradeOM;
            m_columnOM = columnOM;
        }

        /// <summary>
        /// Returns the language identifier of non-constant coordinate 'nonConstCoordIdx'. 
        /// This is approximately "m" + m_gradeOM + "[" + (m_columnOM + nonConstCoordIdx * domainSize + "]";
        /// </summary>
        /// <param name="nonConstCoordIdx">index of the coordinate (counting only variable coordinates)</param>
        /// <param name="S">The specification (only S.m_coordStorage and S.m_basisVectorNames are used).</param>
        /// <param name="CS">The way coordinates are stored (VARIABLES or ARRAY).</param>
        /// <returns>the language identifier of non-constant coordinate 'nonConstCoordIdx'.</returns>
        public override String GetCoordLangID(int nonConstCoordIdx, Specification S, COORD_STORAGE CS)
        {
            return "m" + m_gradeOM +
                "[" +
                m_parentOM.getCoordinateIndex(m_gradeOM, m_columnOM, nonConstCoordIdx) +
//                (m_columnOM + m_parentOM.DomainForGrade(m_gradeOM).Length * nonConstCoordIdx) + 
                "]";
        }



        /// <summary>
        /// The 'parent' outermorphism. This SMV represents one column out of one matrix of this OM.
        /// </summary>
        public G25.OM m_parentOM;

        /// <summary>
        /// This SMV represents one column out of one matrix of this OM. 
        /// <c>m_gradeOM</c> determines which matrix (grade).
        /// </summary>
        public int m_gradeOM;

        /// <summary>
        /// This SMV represents one column out of one matrix of this OM. 
        /// <c>m_columnOM</c> determines which column.
        /// </summary>
        public int m_columnOM;

    } // end of class SMVOM

} // end of namespace G25
