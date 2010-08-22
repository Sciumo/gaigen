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

namespace RefGA
{
    /// <summary>
    /// Immutable class that represents a weighted basis blade.
    /// </summary>
   public class BasisBlade : IComparable 
    {
       public static readonly BasisBlade ZERO = new BasisBlade();
       public static readonly BasisBlade ONE = new BasisBlade(1.0);
       public static readonly BasisBlade MINUS_ONE = new BasisBlade(-1.0);

       public enum InnerProductType : int
       {
           LEFT_CONTRACTION = 1,
           RIGHT_CONTRACTION = 2,
           HESTENES_INNER_PRODUCT = 3,
           MODIFIED_HESTENES_INNER_PRODUCT = 4,
           SCALAR_PRODUCT = 5
       }

        /// <summary>Creates a new basis blade from its bitmap and scale</summary>
        /// <param name="b">Basis blade bitmap</param>
        /// <param name="s">Basis blade scale</param>
        public BasisBlade(uint b, double s) 
        {
            m_bitmap = b;
            m_scale = s;
            m_symScale = null;
            m_hashCode = ComputeHashCode();
        }

        /// <summary>Creates a new unit basis blade from its bitmap</summary>
        /// <param name="b">Basis blade bitmap</param>
        public BasisBlade(uint b)
        {
            m_bitmap = b;
            m_scale = 1.0;
            m_symScale = null;
            m_hashCode = ComputeHashCode();
        }

        /// <summary>Creates a new scalar basis blade from its scale</summary>
        /// <param name="s">Basis blade scale</param>
        public BasisBlade(double s)
        {
            m_bitmap = 0;
            m_scale = s;
            m_symScale = null;
            m_hashCode = ComputeHashCode();
        }

        /// <summary>Creates a new zero basis blade</summary>
        public BasisBlade()
        {
            m_bitmap = 0;
            m_scale = 0.0;
            m_symScale = null;
            m_hashCode = ComputeHashCode();
        }

        /// <summary>Creates a new basis blade from its bitmap, scale and a symbolic scale</summary>
        /// <param name="b">Basis blade bitmap</param>
        /// <param name="s">Basis blade scale</param>
        /// <param name="symScale">Symbolic scale (array of arrays).
        /// The first array is a sum, the second array a product, eg:
        /// symScale[0][0] * symScale[0][1] + symScale[1][0] * symScale[1][1] + symScale[2][0]
        /// </param>
        public BasisBlade(uint b, double s, Object[][] symScale)
        {
            m_bitmap = b;
            m_scale = s;
            m_symScale = Util.SimplifySymbolicScalars(symScale);
            if (symScale != null) { // if non-zero symScale turned into null m_symScale, blade must be zero!
                if (m_symScale == null)
                    m_scale = 0.0;
                else
                {
                    if ((m_symScale.Length == 1) && // check for pure scalar 'symbolic scale'
                        (m_symScale[0].Length == 1) &&
                        (m_symScale[0][0] is Double))
                    {
                        double v = (Double)m_symScale[0][0];
                        m_symScale = null;
                        m_scale *= v;
                    }
                    else if ((m_scale != 1.0) &&
                        (m_symScale.Length == 1) && // check for first scalar 'symbolic scale'
                        (m_symScale[0].Length >= 1) &&
                        (m_symScale[0][0] is Double))
                    {
                        // set m_scale to product of m_scale and m_symScale[0][0], remove m_symScale[0][0] from m_symScale[0]
                        m_scale = (double)m_symScale[0][0] * m_scale;
                        Object[] newSS = new Object[m_symScale[0].Length-1];
                        System.Array.Copy(m_symScale[0], 1, newSS, 0, newSS.Length);
                        m_symScale[0] = newSS;
                    }
                }
            }
            m_hashCode = ComputeHashCode();
        }

        /// <summary>Creates a new basis blade from its bitmap, scale and a symbolic scale</summary>
        /// <param name="b">Basis blade bitmap</param>
        /// <param name="s">Basis blade scale</param>
        /// <param name="symScale">Single symbolic scale Object. I.e., it would be symScale[0][0] in the constructor which accepts an array of ararys.</param>
        public BasisBlade(uint b, double s, Object symScale)
       {
           m_bitmap = b;
           m_scale = s;

           if (symScale != null)
           {
               m_symScale = new Object[1][];
               m_symScale[0] = new Object[1] { symScale };
           }
           else m_symScale = null;
           m_hashCode = ComputeHashCode();
       }

       /// <summary>
       /// Returns a copy of <c>B</c>, but with the basis blade bitmap set to 'b'.
       /// </summary>
        /// <param name="B">Basis blade</param>
        /// <param name="b">Basis blade bitmap</param>
        public BasisBlade(BasisBlade B, uint b)
        {
            m_bitmap = b;
            m_scale = B.m_scale;
            m_symScale = B.m_symScale;
            m_hashCode = ComputeHashCode();
        }

        /// <summary>Creates a new basis blade from a symbolic scale</summary>
        /// <param name="symScale">Single symbolic scale String. I.e., it would be symScale[0][0] in the constructor which accepts an array of ararys.</param>
       public BasisBlade(String symScale) : this(0, 1.0, symScale) { }

        /// <summary>
        /// IComparable.CompareTo implementation.
        /// Does not sort based on symbolic scale. (todo: why?)
        /// </summary>
        /// <param name="obj">The object to which 'this' is compared</param>
        public int CompareTo(object obj) {
            if(obj is BasisBlade) {
                BasisBlade B = (BasisBlade) obj;

                // first sort on grade
                if (Grade() < B.Grade()) return -1;
                else if (Grade() > B.Grade()) return 1;
                else { // grades are equal
                    // then sort on bitmap
                    if (bitmap < B.bitmap) return -1;
                    else if (bitmap > B.bitmap) return 1;
                    else { // bitmaps are equal
                        return scale.CompareTo(B.scale); // finally sort on scalar
                    }
                }
            }
            throw new ArgumentException("object is not a BasisBlade");    
        }

        /// <summary>
        /// Version of CompareTo() that also compares the symbolic scalar part (the standard CompareTo() does not do so)
        /// </summary>
        /// <param name="B">The object to which 'this' is compared</param>
        public int SymbolicCompareTo(BasisBlade B)
        {
            { // first do regular comparison:
                int c = CompareTo(B);
                if (c != 0) return c;
            }
            { // now try symbolic part:
                System.Collections.IComparer SSAC = new Util.SimplifySymbolicArrayComparer();
                int l1 = (symScale == null) ? 0 : symScale.Length;
                int l2 = (B.symScale == null) ? 0 : B.symScale.Length;
                for (int i = 0; ((i < l1) && (i < l2)); i++)
                {
                    // compare both arrays of scalar values:
                    int c = SSAC.Compare(symScale[i], B.symScale[i]);
                    if (c != 0) return c; // if not equal, we're done
                }
                // end of (one of the) arrays reached: compare length of arrays to make comparison
                return ((l1 < l2) ? -1 :((l1 > l2) ? 1 : 0));
            }
        }

       /// <summary>
       /// If 'val' is closer than 'eps' to an integer, rounds val to that int.
       /// Otherwise, leaves it unchanged.
       /// </summary>
       /// <param name="eps">Epsilon threshold for rounding.</param>
       /// <param name="val">Value to be rounded.</param>
       /// <returns>rounded 'val'.</returns>
       public static double Round(double eps, double val)
       {
           if (Math.Abs((Math.Round(val) - val)) < eps) return Math.Round(val);
           else return val;
       }

       /// <summary>
       /// Rounds the coordinate(s) of this basis blade to multiple of integers.
       /// 
       /// If the scale, or the floating point part of a scale, is closer than eps
       /// to an integer, rounds it.
       /// </summary>
       /// <param name="eps">Epsilon threshold for rounding.</param>
       /// <returns>Rounded basis blade.</returns>
       public BasisBlade Round(double eps)
       {
           // round scale 
           double roundedScale = Round(eps, scale);
           bool changeScale = !(roundedScale == scale);
           if (symScale == null) // early exit if no symbolic scale
           {
               if (changeScale) return new BasisBlade(bitmap, roundedScale);
               else return this;
           }

           // round symbolic scale
           bool changeSymScale = false;
           Object[][] roundedSymScale = new Object[symScale.Length][];
           for (int i = 0; i < symScale.Length; i++)
           {
               Object[] SS =  (Object[])symScale[i].Clone();
               roundedSymScale[i] = SS;

               // check if first entry is scalar, round it
               if ((SS != null) && (SS.Length > 0) && (SS[0] is Double))
               {
                   double v = (double)SS[0];
                   double r = Round(eps, v);
                   if (v != r) {
                       changeSymScale = true;
                       SS[0] = r;
                   }
               }

               // recurse to scalar op, round their values too
               for (int j = 0; j < SS.Length; j++)
               {
                   if (SS[j] is RefGA.Symbolic.UnaryScalarOp)
                   {
                       // get scalar op, value, round value
                       RefGA.Symbolic.UnaryScalarOp SA = (SS[j] as RefGA.Symbolic.UnaryScalarOp).Round(eps);
                       if (SA != SS[j])
                       {
                           SS[j] = SA;
                           changeSymScale = true;
                       }
                   }
               }
           }

           // return
           if (changeScale || changeSymScale)
               return new BasisBlade(bitmap, roundedScale, roundedSymScale);
           else return this;
       }

        /// <summary>Computes the grade of this basis blade.</summary>
        /// <returns>the grade of this blade</returns>
        public int Grade()
        {
            return (int)Bits.BitCount(bitmap);
        }


        /// <summary>Computes the negation of this basis blade (-this).</summary>
        /// <returns>Negation of this basis blade (always a newly constructed blade).</returns>
        public BasisBlade Negate()
        {
            // multiplier = -1
            return new BasisBlade(bitmap, -scale, m_symScale);
        }

        /// <summary>Computes the reverse of this basis blade.</summary>
        /// <returns>Reverse of this basis blade (always a newly constructed blade)</returns>
        public BasisBlade Reverse()
        {
            // multiplier = (-1)^(x(x-1)/2)
            double multiplier = Util.MinusOnePow((Grade() * (Grade() - 1)) / 2);
            if (multiplier == 1.0) return this;
            else return new BasisBlade(bitmap, multiplier * scale, m_symScale);
        }

        /// <summary>Computes the grade involution (or grade inversion)  of this basis blade.</summary>
        /// <returns>grade inversion (or grade involution) of this basis blade (always a newly constructed blade)</returns>
        public BasisBlade GradeInvolution()
        {
            return GradeInversion();
        }

        /// <summary>Compute the grade inversion (or grade involution)  of this basis blade.</summary>
        /// <returns>grade inversion (or grade involution) of this basis blade (always a newly constructed blade)</returns>
        public BasisBlade GradeInversion()
        {
            // multiplier is (-1)^x
            double multiplier = Util.MinusOnePow(Grade());
            if (multiplier == 1.0) return this;
            else return new BasisBlade(bitmap, multiplier * scale, m_symScale);
        }

        /// <summary>Computes the grade of this basis blade.</summary>
        /// <returns>clifford conjugate of this basis blade (always a newly constructed blade)</returns>
        public BasisBlade CliffordConjugate()
        {
            // multiplier is ((-1)^(x(x+1)/2)
            double multiplier = Util.MinusOnePow((Grade() * (Grade() + 1)) / 2);
            if (multiplier == 1.0) return this;
            else return new BasisBlade(bitmap, multiplier * scale, m_symScale);
        }

       /// <summary>Alternative way of calling B.Negate()</summary>
       public static BasisBlade Negate(BasisBlade B) { return B.Negate(); }
       /// <summary>Alternative way of calling B.Reverse()</summary>
       public static BasisBlade Reverse(BasisBlade B) { return B.Reverse(); }
       /// <summary>Alternative way of calling B.GradeInvolution()</summary>
       public static BasisBlade GradeInvolution(BasisBlade B) { return B.GradeInvolution(); }
       /// <summary>Alternative way of calling B.GradeInversion()</summary>
       public static BasisBlade GradeInversion(BasisBlade B) { return B.GradeInversion(); }
       /// <summary>Alternative way of calling B.CliffordConjugate()</summary>
       public static BasisBlade CliffordConjugate(BasisBlade B) { return B.CliffordConjugate(); }



        /// <summary>
        /// Returns sign change due to putting the basis blades represented
        /// by <paramref name="a"/> and <paramref name="b"/> into canonical order.</summary>
        /// <param name="a">Basis blade bitmap</param>
        /// <param name="b">Basis blade bitmap</param>
        /// <remarks>The canonical order of a basis blade is the order in which the basis
       /// vectors are listed in alphabetical order. E.g. <c>e1^e2^e4</c> is in canonical order,
       /// and <c>e1^e4^e2</c> is not.</remarks>
        public static double CanonicalReorderingSign(uint a, uint b)
        {
            // Count the number of basis vector flips required to
            // get a and b into canonical order.
            a >>= 1;
            uint sum = 0;
            while (a != 0)
            {
                sum += Bits.BitCount(a & b);
                a >>= 1;
            }

            // even number of flips -> return 1
            // odd number of flips -> return 1
            return ((sum & 1) == 0) ? 1.0 : -1.0;
        }

        /// <summary>
        /// Shortcut to OuterProduct()
        /// </summary>
        public static BasisBlade op(BasisBlade a, BasisBlade b)
        {
            return OuterProduct(a, b);
        }

        /// <summary>Computes the outer product of two basis blades.</summary>
        /// <returns>Outer product of two basis blades <paramref name="a"/> and <paramref name="b"/></returns>
        public static BasisBlade OuterProduct(BasisBlade a, BasisBlade b)
        {
            const bool outer = true;
            return gp_op(a, b, outer);
        }

        /// <summary>
        /// Shortcut to GeometricProduct()
        /// </summary>
        public static BasisBlade gp(BasisBlade a, BasisBlade b)
        {
            return GeometricProduct(a, b);
        }

        /// <summary>Computes the geometric product of two basis blades.</summary>
        /// <returns>geometric product of two basis blades  <paramref name="a"/> and <paramref name="b"/></returns>
        public static BasisBlade GeometricProduct(BasisBlade a, BasisBlade b)
        {
            const bool outer = false;
            return gp_op(a, b, outer);
        }


        /// <summary>
        /// Computes either the geometric product or the outer product of two basis blades <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">Basisblade</param>
        /// <param name="b">Basisblade</param>
        /// <param name="outer">If true, the outer product is returned, otherwise the geometric product</param>
        /// <returns>A new basis blade which is either the geometric product or the outer product of two basis blades <paramref name="a"/> and <paramref name="b"/>.</returns>
        protected static BasisBlade gp_op(BasisBlade a, BasisBlade b, bool outer)
        {
            // if outer product: check for independence
            if (outer && ((a.bitmap & b.bitmap) != 0))
                return new BasisBlade(0.0);

            // compute the bitmap:
            uint bitmap = a.bitmap ^ b.bitmap;

            // compute the sign change due to reordering:
            double sign = CanonicalReorderingSign(a.bitmap, b.bitmap);

            // return result:
            return new BasisBlade(bitmap, sign * a.scale * b.scale, 
                Util.MultiplySymbolicScalars(a.symScale, b.symScale));
        }

        /// <summary>Shortcut to GeometricProduct()</summary>
        public static BasisBlade gp(BasisBlade a, BasisBlade b, double[] m)
        {
            return GeometricProduct(a, b, m);
        }

        /// <summary>Computes the geometric product of two basis blades in diagonal non-Euclidean metric.</summary>
        /// <param name="a">Basisblade</param>
        /// <param name="b">Basisblade</param>
        /// <param name="m">array of doubles giving the metric for each basis vector (i.e., the diagonal of the diagonal metric matrix)</param>
        /// <returns>a new basis blade which is either the geometric product or the outer product of two basis blades <paramref name="a"/> and <paramref name="b"/>.</returns>
        public static BasisBlade GeometricProduct(BasisBlade a, BasisBlade b, double[] m)
        {
            // compute the geometric product in Euclidean metric:
            BasisBlade result = GeometricProduct(a, b);
            double resultScale = result.scale;

            // compute the meet (bitmap of annihilated vectors):
            uint bitmap = a.bitmap & b.bitmap;

            // change the scale according to the metric:
            uint i = 0;
            while (bitmap != 0)
            {
                if ((bitmap & 1) != 0) resultScale *= m[i];
                i++;
                bitmap = bitmap >> 1;
            }

            // return correctly scaled result
            return new BasisBlade(result.bitmap, resultScale, result.symScale);
        }

        /// <summary>Computes the geometric product of two basis blades in arbitary non-Euclidean metric.</summary>
        /// <param name="_A">Basisblade</param>
        /// <param name="_B">Basisblade</param>
        /// <param name="M">Metric to be used</param>
        /// <returns>An ArrayList, because the result may not be a single BasisBlade anymore. For example,
        /// <c>no ni = no.ni + no^ni = -1 + no^ni</c>. </returns>
       public static ArrayList GeometricProduct(BasisBlade _A, BasisBlade _B, Metric M)
        {
            // convert arguments to 'eigenbasis'
            ArrayList A = M.ToEigenbasis(_A);
            ArrayList B = M.ToEigenbasis(_B);

            ArrayList result = new ArrayList();
            double[] EM = M.EigenMetric;
            foreach (BasisBlade bA in A)
                foreach (BasisBlade bB in B)
                {
                    BasisBlade C = GeometricProduct(bA, bB, EM);
                    if (C.scale != 0.0) result.Add(C);
                }

            return M.ToMetricBasis(result);
        }


        /// <summary>
        /// Computes the inner product of two basis blades in Euclidean metric.
        /// </summary>
        /// <param name="A">input blade</param>
        /// <param name="B">input blade</param>
        /// <param name="type">inner product type to be used. Use one of the InnerProductType constants:
        /// LEFT_CONTRACTION, RIGHT_CONTRACTION, HESTENES_INNER_PRODUCT, MODIFIED_HESTENES_INNER_PRODUCT or SCALAR_PRODUCT.</param>
        /// <returns>Inner product of <paramref name="A"/> and <paramref name="B"/>.</returns>
        public static BasisBlade InnerProduct(BasisBlade A, BasisBlade B, InnerProductType type)
        {
            return InnerProductFilter(A.Grade(), B.Grade(), GeometricProduct(A, B), type);
        }

        /// <summary>
        /// Computes the inner product of two basis blades in Euclidean metric.
        /// </summary>
        /// <param name="A">input blade</param>
        /// <param name="B">input blade</param>
        /// <param name="m">an array of doubles giving the metric for each basis vector.</param>
        /// <param name="type">inner product type to be used. Use one of the InnerProductType constants:
        /// LEFT_CONTRACTION, RIGHT_CONTRACTION, HESTENES_INNER_PRODUCT, MODIFIED_HESTENES_INNER_PRODUCT or SCALAR_PRODUCT.</param>
        /// <returns>Inner product of <paramref name="A"/> and <paramref name="B"/>.</returns>
        public static BasisBlade InnerProduct(BasisBlade A, BasisBlade B, double[] m, InnerProductType type)
        {
            return InnerProductFilter(A.Grade(), B.Grade(), GeometricProduct(A, B, m), type);
        }

        /// <summary>
        /// Computes the inner product of two basis blades in Euclidean metric.
        /// </summary>
        /// <param name="A">input blade</param>
        /// <param name="B">input blade</param>
        /// <param name="M">The Metric to be used.</param>
        /// <param name="type">inner product type to be used. Use one of the InnerProductType constants:
        /// LEFT_CONTRACTION, RIGHT_CONTRACTION, HESTENES_INNER_PRODUCT, MODIFIED_HESTENES_INNER_PRODUCT or SCALAR_PRODUCT.</param>
        /// <returns>Inner product of <paramref name="A"/> and <paramref name="B"/>.</returns>
       public static ArrayList InnerProduct(BasisBlade A, BasisBlade B, Metric M, InnerProductType type)
        {
            return InnerProductFilter(A.Grade(), B.Grade(), GeometricProduct(A, B, M), type);
        }

        /// <summary>Shortcut to InnerProduct() </summary>
        public static BasisBlade ip(BasisBlade A, BasisBlade B, InnerProductType type) { return InnerProduct(A, B, type); }

        /// <summary>Shortcut to InnerProduct() </summary>
        public static BasisBlade ip(BasisBlade A, BasisBlade B, double[] m, InnerProductType type) { return InnerProduct(A, B, m, type); }

        /// <summary>Shortcut to InnerProduct() </summary>
        public static ArrayList ip(BasisBlade A, BasisBlade B, Metric M, InnerProductType type) { return InnerProduct(A, B, M, type); }

        /// <summary>
        /// Computes the inner product of two basis blades in Euclidean metric.
        /// </summary>
        /// <param name="A">input blade</param>
        /// <param name="B">input blade</param>
        /// <returns>Scalar product of <paramref name="A"/> and <paramref name="B"/>.</returns>
        public static BasisBlade ScalarProduct(BasisBlade A, BasisBlade B)
        {
            InnerProductType type = InnerProductType.SCALAR_PRODUCT;
            return InnerProductFilter(A.Grade(), B.Grade(), GeometricProduct(A, B), type);
        }

        /// <summary>
        /// Computes the inner product of two basis blades in diagonal non-Euclidean metric.
        /// </summary>
        /// <param name="A">input blade</param>
        /// <param name="B">input blade</param>
        /// <param name="m">an array of doubles giving the metric for each basis vector</param>
        /// <returns>Scalar product of <paramref name="A"/> and <paramref name="B"/>.</returns>
       public static BasisBlade ScalarProduct(BasisBlade A, BasisBlade B, double[] m)
        {
            InnerProductType type = InnerProductType.SCALAR_PRODUCT;
            return InnerProductFilter(A.Grade(), B.Grade(), GeometricProduct(A, B, m), type);
        }

        /// <summary>
        /// Computes the inner product of two basis blades in arbitary non-Euclidean metric.
        /// </summary>
        /// <param name="A">input blade</param>
        /// <param name="B">input blade</param>
        /// <param name="M">The Metric to be used</param>
        /// <returns>Scalar product of <paramref name="A"/> and <paramref name="B"/>.</returns>
       public static ArrayList ScalarProduct(BasisBlade A, BasisBlade B, Metric M)
        {
            InnerProductType type = InnerProductType.SCALAR_PRODUCT;
            return InnerProductFilter(A.Grade(), B.Grade(), GeometricProduct(A, B, M), type);
        }

       /// <summary>Shortcut to ScalarProduct() </summary>
       public static BasisBlade scp(BasisBlade A, BasisBlade B) { return ScalarProduct(A, B); }
       /// <summary>Shortcut to ScalarProduct() </summary>
       public static BasisBlade scp(BasisBlade A, BasisBlade B, double[] m) { return ScalarProduct(A, B, m); }
       /// <summary>Shortcut to ScalarProduct() </summary>
       public static ArrayList scp(BasisBlade A, BasisBlade B, Metric M) { return ScalarProduct(A, B, M); }


       /// <summary>
       /// Returns the 'Hadamard' product of A and B.
       /// If A and B are the same basis blades up to scale, then multiplies
       /// the scalar parts of A and B with the unit basis blade.
       /// Otherwise returns 0.
       /// </summary>
       /// <param name="A">input blade.</param>
       /// <param name="B">input blade.</param>
       /// <returns>'Hadamard' product of A and B.</returns>
       public static BasisBlade hp(BasisBlade A, BasisBlade B)
       {
           return HadamardProduct(A, B);
       }

       /// <summary>
       /// Returns the 'Hadamard product' of A and B.
       /// If A and B are the same basis blades up to scale, then multiplies
       /// the scalar parts of A and B with the unit basis blade.
       /// Otherwise returns 0.
       /// </summary>
       /// <param name="A">input blade.</param>
       /// <param name="B">input blade.</param>
       /// <returns>'Hadamard' product of A and B.</returns>
       public static BasisBlade HadamardProduct(BasisBlade A, BasisBlade B)
       {
           if (A.bitmap != B.bitmap) return BasisBlade.ZERO;
           uint bitmap = 0;
           B = new BasisBlade(B, bitmap); // get a copy of 'B', but as a scalar (0), so we can multiply the scalar part
           return BasisBlade.gp(A, B);
       }


       /// <summary>
       /// Returns the 'Inverse Hadamard product' of A and B.
       /// If A and B are the same basis blades up to scale, then divides
       /// the scalar part of A by the scalar part of B with the unit basis blade.
       /// Otherwise returns 0.
       /// </summary>
       /// <param name="A">input blade.</param>
       /// <param name="B">input blade.</param>
       /// <returns>'Inverse Hadamard' product of A and B.</returns>
       public static BasisBlade ihp(BasisBlade A, BasisBlade B)
       {
           return InverseHadamardProduct(A, B);
       }

       /// <summary>
       /// Returns the 'Inverse Hadamard product' of A and B.
       /// If A and B are the same basis blades up to scale, then divides
       /// the scalar part of A by the scalar part of B with the unit basis blade.
       /// Otherwise returns 0.
       /// 
       /// Do not call when scale of B is zero (returned result will be zero).
       /// </summary>
       /// <param name="A">input blade.</param>
       /// <param name="B">input blade. Must not be zero.</param>
       /// <returns>'Inverse Hadamard' product of A and B.</returns>
       public static BasisBlade InverseHadamardProduct(BasisBlade A, BasisBlade B)
       {
           if (A.bitmap != B.bitmap) return BasisBlade.ZERO;
           uint bitmap = 0;
           if (B.IsSymbolic())
           {
               B = new BasisBlade(B, bitmap); // get a copy of 'B', but as a scalar (0), so we can multiply the scalar part
               return BasisBlade.gp(A, new BasisBlade(0, 1.0, new Symbolic.UnaryScalarOp(Symbolic.UnaryScalarOp.INVERSE, new Multivector(B))));
           }
           else
           {
               double newScale = (B.scale == 0.0) ? 0.0 : A.scale / B.scale;
               return new BasisBlade(A.bitmap, newScale, A.symScale);
           }
       }

       /// <returns>true if this basis blade has a symbolic scale</returns>
       public bool IsSymbolic()
       {
           return (symScale != null) && (symScale.Length > 0);
       }

        /// <summary>
       ///  Internal function which adds scalar (double) and symbolic scales.
       ///  If no symbolic scales are present, the scalar scales are just added (if sum == 0, then null is returned).
       ///  Otherwise the scalars are 'incorporated' into the symbolic scales and a blade with scale 1 is returned.
       /// </summary>
       static private BasisBlade SumScales(uint bitmap, double[] scales, Object[][][] symScales, int nbScales) {
           if (nbScales == 0) return null;

           // check if any symbolic scales
           bool symScalesPresent = false;
           for (int i = 0; i < nbScales; i++)
           {
               if (symScales[i] != null)
               {
                   symScalesPresent = true;
                   break;
               }
           }

           if (symScalesPresent)
           {
               // overall scale = 1, incorporate scalars into symScales
               double scale = 1.0;

               Object[][] symScale = null;
               Object[][] scaleI = new Object[1][];
               scaleI[0] = new Object[1];

               for (int i = 0; i < nbScales; i++)
               {
                   Object[][] symScaleI = symScales[i];
                   if (symScaleI == null)
                   {
                       symScaleI = new Object[1][];
                       symScaleI[0] = new Object[]{new BasisBlade(scales[i])};
                       scales[i] = 1.0;
                   }
                   if (scales[i] != 1.0) { // only multiply with scalar when not 1.0
                       scaleI[0][0] = scales[i];
                       symScaleI = Util.MultiplySymbolicScalars(scaleI, symScaleI);
                   }

                   if ((symScaleI != null) && (symScaleI.Length > 0))
                        symScale = Util.AddSymbolicScalars(symScale, symScaleI);
               }

               return new BasisBlade(bitmap, scale, symScale);
           }
           else // no symbolic scales
           {
               // overall scale = sum of scales
               double scale = scales[0];
               for (int i = 1; i < nbScales; i++) scale += scales[i];
               return new BasisBlade(bitmap, scale);
           }
       }

       /// <summary>
       ///  'Simplifies' an ArrayList of BasisBlades. I.e., sums scale of BasisBlades with identical bitmaps.
       /// Can also process symbolic scalars.
       /// </summary>
       /// <param name="A">ArrayList of BasisBlades (no other types of objects should be in the array).</param>
       /// <returns>Sorted, Simplified ArrayList of BasisBlades</returns>
       static public ArrayList Simplify(ArrayList A)
       {
           if (A.Count == 0) return A;

           // sort  basis blade in A such that those blade with the same bitmap are next to eachother
           A.Sort();


           // loop over all basis blades in A, summing basis blades with identical bitmaps, storing the result in 'result'
           ArrayList result = new ArrayList();
           {
               // allocate memory to store the scales
               int scalesIdx = 0;
               double[] scales = new double[A.Count];
               Object[][][] symScales = new Object[A.Count][][];

               uint currentBitmap = ((BasisBlade)A[0]).bitmap;

               // keep scale and sym scale in arrays?
               scales[scalesIdx] = ((BasisBlade)A[0]).scale;
               symScales[scalesIdx] = ((BasisBlade)A[0]).symScale;
               scalesIdx++;

               for (int i = 1; i < A.Count; i++) // loop over all basis blades in A, summing basis blades with identical bitmaps, storing the result in 'result'
               {
                   BasisBlade B = (BasisBlade)A[i];
                   if (B.bitmap == currentBitmap)
                   {
                       scales[scalesIdx] = B.scale;
                       symScales[scalesIdx] = B.symScale;
                       scalesIdx++;
                   }
                   else
                   {
                       BasisBlade sum = SumScales(currentBitmap, scales, symScales, scalesIdx);
                       if ((sum != null) && (sum.scale != 0.0)) result.Add(sum);

                       currentBitmap = B.bitmap;

                       scalesIdx = 0;
                       scales[scalesIdx] = B.scale;
                       symScales[scalesIdx] = B.symScale;
                       scalesIdx++;
                   }
               }

               { // handle leftover from loop:
                   BasisBlade sum = SumScales(currentBitmap, scales, symScales, scalesIdx);
                   if ((sum != null) && (sum.scale != 0.0)) result.Add(sum);
               }
           }
           return result;
       }


       /// <param name="ga">grade of left hand side argument of inner product.</param>
       /// <param name="gb">grade of right hand side argument of inner product.</param>
       /// <param name="R">Array of Basis Blades to which the filter should be applied.</param>
       /// <param name="type">InnerProductType (one of LEFT_CONTRACTION, RIGHT_CONTRACTION, HESTENES_INNER_PRODUCT, MODIFIED_HESTENES_INNER_PRODUCT or SCALAR_PRODUCT)</param>
       /// <summary>
       ///  Applies the inner product 'filter'  to an array of  BasisBlades in order turn 
       /// a geometric product into an inner product.
       /// </summary>
       private static ArrayList InnerProductFilter(int ga, int gb, ArrayList R, InnerProductType type)
       {
           ArrayList result = new ArrayList();
           foreach (BasisBlade r in R) {
               BasisBlade B = InnerProductFilter(ga, gb, r, type);
               if (B.scale != 0.0)
                   result.Add(B);
           }
           return result;
       }

       /// <summary>
       ///   Applies the inner product 'filter' to a BasisBlade in order to turn a geometric product into an inner product
       /// </summary>
       /// <param name="ga">Grade of argument 'a'</param>
       /// <param name="gb">Grade of argument 'b'</param>
       /// <param name="r">the basis blade to be filtered</param>
       /// <param name="type">type the type of inner product required</param>
       /// <returns>Either <paramref name="r"/> or ZERO, or null when <paramref name="type"/> is invalid</returns>
       private static BasisBlade InnerProductFilter(int ga, int gb, BasisBlade r, InnerProductType type)
       {
           switch (type)
           {
               case InnerProductType.LEFT_CONTRACTION:
                   if ((ga > gb) || (r.Grade() != (gb - ga)))
                       return ZERO; // return zero
                   else return r; // return input blade

               case InnerProductType.RIGHT_CONTRACTION:
                   if ((ga < gb) || (r.Grade() != (ga - gb)))
                       return ZERO; // return zero
                   else return r; // return input blade

               case InnerProductType.HESTENES_INNER_PRODUCT:
                   if ((ga == 0) || (gb == 0)) return new BasisBlade();
                   if (Math.Abs(ga - gb) == r.Grade()) return r; // return input blade
                   else return ZERO; // return zero

               case InnerProductType.MODIFIED_HESTENES_INNER_PRODUCT:
                   if (Math.Abs(ga - gb) == r.Grade()) return r; // return input blade
                   else return ZERO; // return zero
               case InnerProductType.SCALAR_PRODUCT:
                   if (r.Grade() == 0) return r;
                   else return ZERO; // return zero
               default:
                   return null;
           }
       }

       /// <summary>
       /// Substitutes all symbolic scalars for doubles (as provided by <paramref name="E"/> and evaluates
       /// symbolic ScalarOps.
       /// </summary>
       /// <param name="E">RefGA.Symbolic.SymbolicEvaluator used to evaluate the symbolic scalars</param>
       /// <returns></returns>
       public BasisBlade SymbolicEval(RefGA.Symbolic.SymbolicEvaluator E)
       {
           if (m_symScale == null) return this;

           Object[][] newSymScale = new Object[m_symScale.Length][];
           for (int i = 0; i < m_symScale.Length; i++)
           {
               if (m_symScale[i] == null) newSymScale[i] = null;
               else {
                   newSymScale[i] = new Object[m_symScale[i].Length];
                   for (int j = 0; j < m_symScale[i].Length; j++)
                   {
                       double value;
                       if (m_symScale[i][j] == null) value = 0.0;
                       else if (m_symScale[i][j] is Double) value = (double)m_symScale[i][j];
                       else if (m_symScale[i][j] is Symbolic.UnaryScalarOp) // evaluate scalar operation (like log, exp, sin, cos, etc)
                           value = ((Symbolic.UnaryScalarOp)m_symScale[i][j]).SymbolicEval(E);
                       else value = E.Evaluate(m_symScale[i][j]); // substitute symbolic scalars for doubles

                       newSymScale[i][j] = value;
                   }
                }
           }
           return new BasisBlade(bitmap, scale, newSymScale);
       }



        /// <returns>Human readable string that represents the value of this BasisBlade.</returns>
        /// <remarks>Uses e1, e2, e3 and so on as the names of the basis vectors.</remarks>
        public override string ToString()
        {
            return ToString((String[])null);
        }


        /// <summary>
        /// Converts list to array, calls ToString(String[] bvNames).
        /// </summary>
        /// <param name="bvNames">Names of basis vectors. May be null; in that cases e1, e2, etc is used</param>
        /// <returns>Human readable string that represents the value of this BasisBlade, and allows you to specifiy the names of the basis vectors.</returns>
        public string ToString(System.Collections.Generic.List<string> bvNames)
        {
            return ToString(bvNames.ToArray());
        }

        /// <param name="bvNames">Names of basis vectors. May be null; in that cases e1, e2, etc is used</param>
        /// <returns>Human readable string that represents the value of this BasisBlade, and allows you to specifiy the names of the basis vectors.</returns>
        public string ToString(String[] bvNames)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            if (scale == 0.0) return "0";

            // symbolic scalars:
            //bool symPart = false;
            if (symScale != null)
            {
                // symbolic scalar string goes in  symResult
                System.Text.StringBuilder symResult = new System.Text.StringBuilder();
                int tCnt = 0;
                for (int t = 0; t < symScale.Length; t++) // for each term ...*...*...+
                {
                    if (symScale[t] != null) // if term is not null
                    {
                        bool plusEmitted = false;
                        int pCnt = 0;
                        for (int p = 0; p < symScale[t].Length; p++) // for each product ...*
                        {
                            if (symScale[t][p] != null)
                            {
                                if ((!plusEmitted) && (tCnt > 0))
                                {
                                    symResult.Append("+");
                                    plusEmitted = true;
                                }
                                if (pCnt > 0) symResult.Append("*");
                                symResult.Append(symScale[t][p].ToString());
                                pCnt++;
                            }
                        }
                        if (pCnt > 0) tCnt++;
                    }
                }

                if (tCnt > 0)
                {
                    //symPart = true;
                    if (tCnt > 1) result.Append("("); // do we need to add parentheses?
                    result.Append(symResult);
                    if (tCnt > 1) result.Append(")"); // do we need to add parentheses?
                    if (bitmap != 0) result.Append("*"); // append * for basis vector part (if any basis vectors in bitmap)
                }
            }


            // basis vectors
            {
                int bvCnt = 0;
                uint i = 1;
                uint b = bitmap;
                while (b != 0)
                {
                    if ((b & 1) != 0)
                    {
                        if (bvCnt > 0) result.Append('^');
                        if ((bvNames == null) || (i > bvNames.Length) || (bvNames[i - 1] == null))
                        {
                            result.Append("e" + i);
                        }
                        else result.Append(bvNames[i - 1]);
                        bvCnt++;
                    }
                    b >>= 1;
                    i++;
                }
            }

            // return (+ add regular scalar)
            if (result.Length == 0) return "" + scale;
            else if (scale == 1.0) return result.ToString();
            else return scale + "*" + result.ToString();
        }

       /// <summary>
       /// Converts a list of BasisBlades to a string. Does not simplify the arraylist, so
       /// if you pass [0, e1, 0, -2*e1] you get "0 + 1.0 * e1 + 0 - 2.0 * e2" as a result
       /// If you pass an empty list, "0" is returned
       /// </summary>
       /// <param name="L">List of BasisBlades</param>
       /// <param name="bvNames">Optional names of basis vectors (may be null, then e1, e2, e3, etc is used)</param>
       /// <returns>A string which represents <paramref name="L"/></returns>
       public static string ToString(ArrayList L, String[] bvNames) {
           if (L.Count == 0) return "0";
           System.Text.StringBuilder result = new System.Text.StringBuilder();
           bool first = true;
           foreach(BasisBlade B in L) {
               double s = B.scale;
               uint b = B.bitmap;

               if (!first) {
                   if (s < 0) {
                       result.Append(" - ");
                       s = -s;
                   }
                   else result.Append(" + ");
               }
               first = false;
               result.Append(new BasisBlade(b, s, B.symScale).ToString(bvNames));
           }
           return result.ToString();
       }

       /// <summary>
       /// Converts a limited non-symbolic basis blade to a human readable string that can be used as an identifier in programming languages.
       /// 
       /// Certain restrictions apply. The symbolic scale is ignored, and the scalar scale can be either +1 or -1.
       /// If it is zero, an exception to thrown. Otherwise, negative scalar scale is treated as -1, and positive scale as +1.
       /// Wedges are converted to underscores. 
       /// 
       /// If the scalar scale is negative, the first two basis vector are swapped. If only one basis vector is available, "neg_"
       /// is prefixed to indicate negation.
       /// </summary>
       /// <param name="bvNames">Names of basis vectors. May be null; in that cases e1, e2, etc is used</param>
       /// <returns>a human readable string that can be used as an identifier in programming languages. </returns>
       public string ToLangString(List<String> bvNames) {
           return ToLangString(bvNames, "_");
       }

       public string ToLangString(List<String> bvNames, String wedgeSymbol)
       {
           if (scale == 0.0) throw new Exception("G25.BasisBlade.ToLangString(): zero basis blade");

           // basis vectors
           List<String> bvs = new List<String>();
           {
               int bvCnt = 0;
               uint i = 1;
               uint b = bitmap;
               while (b != 0)
               {
                   if ((b & 1) != 0)
                   {
                       if ((bvNames == null) || (i > bvNames.Count) || (bvNames[(int)i - 1] == null))
                       {
                           bvs.Add("e" + i);
                       }
                       else bvs.Add(bvNames[(int)i - 1]);
                       bvCnt++;
                   }
                   b >>= 1;
                   i++;
               }
           }

           if (bvs.Count == 0)
               return (scale > 0.0) ? "scalar" : "neg_scalar";
           else if (bvs.Count == 1)
               return (scale > 0.0) ? bvs[0] : "neg_" + bvs[0];
           else
           {
               System.Text.StringBuilder result = new System.Text.StringBuilder();
               if (scale < 0.0)
               {// swap bvs[0] and bvs[1]
                   String tmp = bvs[0]; bvs[0] = bvs[1]; bvs[1] = tmp;
               }

               for (int i = 0; i < bvs.Count; i++)
               {
                   if (i > 0) result.Append(wedgeSymbol);
                   result.Append(bvs[i]);
               }
               return result.ToString();
           }
       } // end of function ToLangString


       /// <summary>
       /// Must be called by constructor to initialize m_hashCode (after all member variables have been set)
       /// </summary>
       protected int ComputeHashCode()
       {
           return m_bitmap.GetHashCode() ^ m_scale.GetHashCode() ^ Util.ComputeHashCode(m_symScale);
       }

       public override int GetHashCode()
       {
           return m_hashCode;
       }

        /// <summary>
        /// Bitmap of basis blades present in this basis blade. 
        /// If bit 0 is true, the e1 is present.
        /// If bit 1 is true, the e2 is present, etc.
        /// So the value '5' represents e1^e3
        /// </summary>
        /// <param name="s">Basis blade scale</param>
        /// <seealso cref="m_bitmap"/>
        public uint bitmap
        {
            get
            {
                return m_bitmap;
            }
        }

        /// <summary>
        /// Scale of this basis blade
        /// </summary>
        /// <seealso cref="m_scale"/>
        public double scale
        {
            get
            {
                return m_scale;
            }
        }

        /// <summary>
        /// Symbolic  of this basis blade (do not modify returned array)
        /// </summary>
        /// <seealso cref="m_symScale"/>
        public Object[][] symScale
        {
            get
            {
                return m_symScale;
            }
        }

        /// <summary>
        /// Bitmap of basis blades present in this basis blade. 
        /// </summary>
       private readonly uint m_bitmap;

        /// <summary>
        /// Scale of this basis blade
        /// </summary>
       private readonly double m_scale;

       /// <summary>
       /// Symbolic scalar multipliers of this blade.
       /// An array (sum) of arrays (multiply)
       /// Can be null for no symbolic scalar multipliers
       /// 
       /// TODO: what can be expected in here except for user input????
       /// </summary>
       private readonly Object[][] m_symScale;

       /// <summary>
       ///  Computed at construction-time
       /// </summary>
       private readonly int m_hashCode;

   } // end of class BasisBlade
} // end of namespace RefGA
