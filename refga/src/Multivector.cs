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

/*! \mainpage Reference GA implementation in C#
 *
 * RefGA by Daniel Fontijne, University of Amsterdam.
 * 
 * Released under GPL license.
 * 
 * \section intro_sec Introduction
 *
 * This package is an implementation of Geometric Algebra written in C#.
 * It is based on the Java reference implementation which I wrote for the book
 * <a href="http://www.geometricalgebra.net">Geometric Algebra for Computer Science</a>.
 * For the original Java version, see <a href="http://www.geometricalgebra.net/reference_impl.html">this page</a>.
 *
 * The implementation can handle geometric algebras over the reals of any dimensional, using
 * any metric. Multivectors are represented as sums of basis blades. No effort for optimization was made
 * (read: this implementation is incredible slow compared to e.g., Gaigen 2).
 * 
 * Originaly the implementation was numerical only, but I have added some support for symbolic scalars. 
 * This allows one to expand equations such as <c>(c1 * e1) ^ (c2 * e2) </c>, where
 * <c>c1</c> and <c>c2</c> are symbolic scalars.
 * You can even expand exponentials, sines and cosines, as long as the square of the bivector is a scalar.
 * I intend to use this symbolic feature for a future version of Gaigen.
 *
 * If you want to write your own GA implementation, you may find this package useful as a reference, or
 * you may use it to generate multiplication tables. This is what I personally would use it for.
 * 
 * This package may also be useful if you simply want a C# implementation of GA, nevermind that it is
 * incredibly inefficient.
 * 
 * \section req_sec Required Packages
 * RefGA requires the <a href="http://www.codeproject.com/KB/recipes/psdotnetmatrix.aspx">DotNetMatrix</a> package.
 * It is used to compute the Eigenvalue Decomposition of the metric matrix. Otherwise there are no dependencies.
 * 
 * 
 * \section metric_sec Metric
 * Lets get the metric out of the way first.
 * If you want to do metric products using a non-Euclidean metric, you need to specify the metric
 * for each product. There are two ways to do so: by specifying the diagonal of the metric matrix,
 * or by creating a RefGA.Metric object.
 * \subsection mt_diagonal 
 * If you want your basis vectors to be orthogonal, you only need to
 * specify the diagonal (i.e., the signature) of your metric matrix. So suppose you want a <a href="http://www.wikipediaorg>Minkowski"</a>
 * metric for 4-D space, then your diagonal should be 
 * <code>
 * double[] m = new double[] { -1.0, 1.0, 1.0, 1.0};
 * </code>.
 * You can pass this <c>m</c> directly to any function which requires a metric (such a RefGA.Multivector.GeometricProduct()).
 * 
 * \subsection mt_matrix 
 * 
 * If you require a non-diagonal metric, such as the (natural) metric 
 * for the conformal model of 3-D Euclidean space, then
 * you need to construct a RefGA.Metric. For example, your metric matrix for the conformal
 * model could be (no = origin, ni = infinity):
 * \htmlonly
 * <table>
 * <tr><td width=35 height=30></td><td><b>no</b></td><td><b>e1</b></td><td><b>e2</b></td><td><b>e3</b></td><td><b>ni</b></td></tr>
 * <tr><td><b>no</b></td><td>0</td><td>0</td><td>0</td><td>0</td><td>-1</td></tr>
 * <tr><td><b>e1</b></td><td>0</td><td>1</td><td>0</td><td>0</td><td>0</td></tr>
 * <tr><td><b>e2</b></td><td>0</td><td>0</td><td>1</td><td>0</td><td>0</td></tr>
 * <tr><td><b>e3</b></td><td>0</td><td>0</td><td>0</td><td>1</td><td>0</td></tr>
 * <tr><td><b>ni</b></td><td>-1</td><td>0</td><td>0</td><td>0</td><td>0</td></tr>
 * </table>
 * \endhtmlonly
 * Then you would construct your RefGA.Metric as:
 * <code>
 * double[] m = new double[]{
 *                         0.0, 0.0, 0.0, 0.0, -1.0, 
 *                         0.0, 1.0, 0.0, 0.0, 0.0,
 *                         0.0, 0.0, 1.0, 0.0, 0.0,
 *                         0.0, 0.0, 0.0, 1.0, 0.0,
 *                         -1.0, 0.0, 0.0, 0.0, 0.0};
 * RefGA.Metric M = new RefGA.Metric(m);
 * </code>
 * Of course, other ways of passing the matrix coordinates are possible, see the list of constructors of RefGA.Metric.
 * 
 * \section basis_blades_sec Basis Blades
 * The RefGA.BasisBlade class implements weighted basis blades and a limited set of GA operations between them.
 * Supported operations are (bi-)linear functions like RefGA.BasisBlade.Reverse() and RefGA.BasisBlade.GeometricProduct().
 * 
 * To construct a new BasisBlade, use code like:
 * <code>
 * uint bitmap = 1 + 2;
 * double scale = 3.0;
 * RefGA.BasisBlade B = new RefGA.BasisBlade(bitmap, scale);
 * </code>
 * This creates a blade <c>B = 3.0 * e1 ^ e2</c>. Each bit in the bitmap stands for a basis blade (<c>e1 = 1, e2 = 2, e3 = 4</c>, and so on).
 * 
 * \subsection bb_String Converting to String
 * 
  * You may then print the value of the basis blade using:
 * <code>
 * System.Console.WriteLine(B.ToString());
 * </code>
 * which would print <c>3*e1^e2</c>. If you want different names for the basis vectors than the default e1, e2, e3,  use something like this example:
 * <code>
 * String[] basisVectorNames = new string[] { "no", "e1", "e2", "e3", "ni" });
 * System.Console.WriteLine(B.ToString(basisVectorNames));
 * </code>
 * which would then print out <c>3*no^e1</c>, since the first basis vector is named <c>no</c> and the second basis vector is named <c>e1</c>.
 * 
 * \subsection bb_GA Geometric Algebra Operations and Products
 * An example of computing a geometric product of basis blades in Euclidean metric is:
 * <code>
 * uint bitmapA = 1 + 2;
 * double scaleA = 2.0;
 * RefGA.BasisBlade A = new RefGA.BasisBlade(bitmapA, scaleA);
 * uint bitmapB = 2 + 4;
 * double scaleB = 3.0;
 * RefGA.BasisBlade B = new RefGA.BasisBlade(bitmapB, scaleB);
 * RefGA.BasisBlade AB = RefGA.BasisBlade.GeometricProduct(A, B);
 * System.Console.WriteLine("AB = " + AB.ToString());
 * </code>
 * which prints <c>AB = 6*e1^e3</c>.
 * 
 * \section multivectors_sec Multivectors
 * Most users are probably only interested in computing with multivectors instead of with basis blades.
 * The RefGA.Multivector class implements multivectors as sum (lists) of weighted basis blades.
 * 
 * \subsection mv_construction Construction
 * To construct a Multivector you can use:
 * <code>
 * RefGA.Multivector scalar = new RefGA.Multivector(2.0); // create multivector with value 2.0
 * RefGA.Multivector vector = new RefGA.Multivector(new RefGA.BasisBlade(1, 2.0)); // create multivector with value 2.0 * e1
 * </code>
 * If you want more complicated Multivectors, use:
 * <code>
 * ArrayList AL = new ArrayList();
 * AL.Add(new BasisBlade(2 + 16, 1.0));
 * AL.Add(new BasisBlade(4 + 16, 2.0));
 * AL.Add(new BasisBlade(8 + 16, 3.0));
 * AL.Add(new BasisBlade(2 + 4, 4.0));
 * AL.Add(new BasisBlade(4 + 8, 5.0));
 * AL.Add(new BasisBlade(8 + 2, 6.0));
 * AL.Sort();// note the 'Sort'
 * RefGA.Multivector A = new RefGA.Multivector(AL); 
 * System.Console.WriteLine("A = " + A.ToString());
 * </code>
 * This prints out <c>A = 4*e2^e3 + 6*e2^e4 + 5*e3^e4 + e2^e5 + 2*e3^e5 + 3*e4^e5</c>.
 * Make sure the ArrayList contains only BasisBlades and is sorted (basis blades listed from low to high bitmap value)
 * and simplified (each basis blade occurs only once).
 * 
 * \subsection mv_String Converting to String
 * 
 * Converting multivectors to strings was already demonstrated above, and the technique for passing
 * the basis vectors names (as used for BasisBlade above) works too.
 * 
 * \subsection mv_GA Geometric Algebra Operations and Products
 * 
 * Here is an example of computing an inner product of two multivectors:
 * <code>
 * ArrayList AL = new ArrayList();
 * AL.Add(new BasisBlade(1, 1.0));
 * AL.Add(new BasisBlade(2, 2.0));
 * AL.Add(new BasisBlade(4, 3.0));
 * Multivector A = new Multivector(AL);
 * Multivector B = new Multivector(new BasisBlade(4 +1, -2.0));
 * System.Console.WriteLine("A = " + A.ToString());
 * System.Console.WriteLine("B = " + B.ToString());
 * System.Console.WriteLine("A . B = " + Multivector.InnerProduct(A, B, BasisBlade.InnerProductType.LEFT_CONTRACTION).ToString());
 * </code>
 * This prints out:
 * <code>
 * A = e1 + 2*e2 + 3*e3
 * B = -2*e1^e3
 * A . B = 6*e1 - 2*e3
 * </code>
 * Note that you must specify the InnerProductType (<c>LEFT_CONTRACTION</c>, <c>RIGHT_CONTRACTION</c>,
 * <c>HESTENES_INNER_PRODUCT</c>, <c>MODIFIED_HESTENES_INNER_PRODUCT</c> or <c>SCALAR_PRODUCT</c>).
 * Otherwise, this code is the general example of how to use any of the bilinear products like
 * RefGA.Multivector.GeometricProduct(), RefGA.Multivector.OuterProduct(), RefGA.Multivector.InnerProduct() or
 * RefGA.Multivector.ScalarProduct().
 * 
 * This is an example of how to compute the exponential of a bivector, how to invert the
 * resulting rotor, and how to apply it to a vector (rotating the vector):
 * <code>
 * // initialize vector
 * ArrayList vectorL = new ArrayList();
 * vectorL.Add(new BasisBlade(1, 1.0));
 * vectorL.Add(new BasisBlade(2, 2.0));
 * vectorL.Add(new BasisBlade(4, 3.0));
 * Multivector vector = new Multivector(vectorL);
 * 
 * // initialize bivector
 * ArrayList bivectorL = new ArrayList();
 * bivectorL.Add(new BasisBlade(1+4, -1.0));
 * bivectorL.Add(new BasisBlade(2+4, 0.5));
 * Multivector bivector = new Multivector(bivectorL);
 * 
 * // compute rotor as exponential of bivector
 * Multivector rotor = bivector.Exp();
 * 
 * // invert rotor
 * Multivector inverseRotor = rotor.VersorInverse();
 * 
 * // rotate vector by applying the rotor to the vector (gp is shorthand for GeometricProduct)
 * Multivector rotatedVector = Multivector.gp(Multivector.gp(rotor, vector), inverseRotor);
 * 
 * System.Console.WriteLine("vector = " + vector.ToString());
 * System.Console.WriteLine("bivector = " + bivector.ToString());
 * System.Console.WriteLine("rotor = " + rotor.ToString());
 * System.Console.WriteLine("rotatedVector = " + rotatedVector.ToString());
 * </code>
 * This prints out:
 * <code>
 * vector = e1 + 2*e2 + 3*e3
 * bivector = -1*e1^e3 + 0.5*e2^e3
 * rotor = 0.437451210732598 - 0.804306627215557*e1^e3 + 0.402153313607778*e2^e3
 * rotatedVector = -1.11106944725419*e1 + 3.0555347236271*e2 - 1.8518186293715*e3 - 1.11022302462516E-16*e1^e2^e3
 * </code>
 * 
 * \section symbolic_sec Using the Symbolic Stuff
 *  
 * You can safely ignore the symbolic scalar part of the implementation if you don't need it.
 * But if you want to experiment with it, try the following:
 * When constructing basis blades, you can specify a symbolic scalar weight. The scalar weight
 * can be any Object, as long as it has HashCode() and ToString() implemented.
 * An example of constructing a BasisBlade with a symbolic scalar:
 * <code>
 * ArrayList AL = new ArrayList();
 * AL.Add(new BasisBlade(1, 1.0, "a1"));
 * AL.Add(new BasisBlade(2, 1.0, "a2"));
 * Multivector A = new Multivector(AL);
 * System.Console.WriteLine("A = " + A.ToString());
 * 
 * ArrayList BL = new ArrayList();
 * BL.Add(new BasisBlade(2, 1.0, "b1"));
 * BL.Add(new BasisBlade(3, 1.0, "b2"));
 * Multivector B = new Multivector(BL);
 * System.Console.WriteLine("B = " + B.ToString());
 * 
 * Multivector AB = Multivector.gp(A, B);
 * System.Console.WriteLine("AB = " + AB.ToString());
 * </code>
 * The output is of this piece of code is 
 * <code>
 * A = a1*e1 + a2*e2
 * B = b1*e2 + b2*e1^e2
 * AB = a2*b1 - a2*b2*e1 + a1*b2*e2 + a1*b1*e1^e2
 * </code>
 * In the example we used Strings like <c>"a1"</c> and <c>"a2"</c> as symbolic scalars.
 *
 * You can substitute symbolic scalars for real values. For example we could add the following
 * lines to the example above: 
 * <code>
 * Symbolic.HashtableSymbolicEvaluator HSE = new Symbolic.HashtableSymbolicEvaluator();
 * HSE.Add("a1", 3.0);
 * HSE.Add("a2", -2.0);
 * HSE.Add("b1", 4.0);
 * HSE.Add("b2", 1.0);
 * Multivector ABeval = AB.SymbolicEval(HSE);
 * System.Console.WriteLine("ABeval = " + ABeval.ToString());
 * </code>
 * The example then prints out the following line:
 * <code>
 * ABeval = -8 + 2*e1 + 3*e2 + 12*e1^e2
 * </code>
 * 
 * 
 * 
 */


/// \namespace RefGA
/// \brief This namespace provides a reference implementation of GA with some support for symbolic scalars.
namespace RefGA
{
    /// <summary>
    /// Immutable multivector class 
    /// </summary>
    public class Multivector : IComparable 
    {
        public static readonly Multivector MINUS_ONE = new Multivector(-1.0);
        public static readonly Multivector ZERO = new Multivector();
        public static readonly Multivector ONE = new Multivector(1.0);


        /// <summary>
        /// Constuctor for zero multivector.
        /// </summary>
        public Multivector()
        {
            m_basisBlades = new BasisBlade[0];
            m_hashCode = ComputeHashCode();
        }

        /// <summary>
        /// Constuctor for scalar Multivector.
        /// </summary>
        /// <param name="s">The scale of the Multivector.</param>
        public Multivector(double s)
        {
            m_basisBlades = new BasisBlade[1] { new BasisBlade(s) };
            m_hashCode = ComputeHashCode();
        }

        /// <summary>
        /// Constuctor for multivector with only a single basis blade.
        /// </summary>
        /// <param name="B">The sole BasisBlade of the Multivector.</param>
        public Multivector(BasisBlade B)
        {
            m_basisBlades = new BasisBlade[1] {B};
            m_hashCode = ComputeHashCode();
        }

        /// <summary>
        /// Constuctor for multivector with symbolic scalar value
        /// </summary>
        /// <param name="symName">Name of symbolic scalar.</param>
        public Multivector(String symName)
        {
            m_basisBlades = new BasisBlade[1] {new BasisBlade(symName)};
            m_hashCode = ComputeHashCode();
        }

        /// <summary>
        /// Constuctor from sorted array of basis blades.
        /// </summary>
        /// <param name="L">Sorted  Array of BasisBlades</param>
        /// <remarks>The basis blades must be sorted because the Multivector class stores
        /// them sorted. If your array is not sorted, use Array.Sort() to sort it.</remarks>
        public Multivector(BasisBlade[] L)
        {
            m_basisBlades = L;
            m_hashCode = ComputeHashCode();
        }

        /// <summary>
        /// Constuctor from ArrayList.
        /// </summary>
        /// <param name="L">Sorted ArrayList of BasisBlades</param>
        /// <remarks>The basis blades must be sorted because the Multivector class stores
        /// them sorted. If your array is not sorted, use ArrayList.Sort() to sort it.</remarks>
        public Multivector(ArrayList L)
        {
            m_basisBlades = new BasisBlade[L.Count];
            for (int i = 0; i < L.Count; i++)
                m_basisBlades[i] = (BasisBlade)L[i];
            m_hashCode = ComputeHashCode();
        }

        /// <summary>Quick 'constructor' for basis vector <paramref name="idx"/>.</summary>
        /// <param name="idx">Index of the basis vector (in range [0, dim)).</param>
        /// <returns>Unit basis vector.</returns>
        public static Multivector GetBasisVector(int idx)
        {
            return new Multivector(new BasisBlade((uint)(1 << idx)));
        }

        /// <summary>Quick 'constructor' for basis vector <paramref name="idx"/>
        /// with specified <paramref name="scale"/> and <paramref name="symScale"/>.</summary>
        /// <param name="idx">Index of the basis vector (in range [0, dim)).</param>
        /// <param name="scale">Floating point scale.</param>
        /// <param name="symScale">Single symbolic scale object.</param>
        /// <returns>Scaled basis vector.</returns>
        public static Multivector GetBasisVector(int idx, double scale, Object symScale)
        {
            return new Multivector(new BasisBlade((uint)(1 << idx), scale, symScale));
        }

        /// <summary>Quick 'constructor' for basis vector <paramref name="idx"/>
        /// with specified <paramref name="scale"/> and <paramref name="symScale"/>.</summary>
        /// <param name="idx">Index of the basis vector (in range [0, dim)).</param>
        /// <param name="scale">Floating point scale.</param>
        /// <param name="symScale">Symbolic scale (array of arrays).</param>
        /// <returns>Scaled basis vector.</returns>
        public static Multivector GetBasisVector(int idx, double scale, Object[][] symScale)
        {
            return new Multivector(new BasisBlade((uint)(1 << idx), scale, symScale));
        }

        /// <summary>Quick 'constructor' for pseudoscalar of a space with dimension <paramref name="N"/>.</summary>
        /// <param name="N">Dimension of space for which you want the pseudoscalar.</param>
        /// <returns>Unit pseudoscalar for dimension <paramref name="N"/>.</returns>
        public static Multivector GetPseudoscalar(int N)
        {
            return new Multivector(new BasisBlade((uint)((1 << N)-1)));
        }


        /// <summary>
        /// Outer product of multivectors.
        /// </summary>
        /// <param name="A">Multivector input (LHS)</param>
        /// <param name="B">Multivector input (RHS)</param>
        /// <returns>outer product of <paramref name="A"/> and <paramref name="B"/></returns>
        public static Multivector OuterProduct(Multivector A, Multivector B)
        {
            ArrayList L = new ArrayList();
            foreach (BasisBlade a in A.BasisBlades)
                foreach (BasisBlade b in B.BasisBlades) {
                    BasisBlade c = BasisBlade.OuterProduct(a, b);
                    if (c.scale != 0.0) L.Add(c);
                }

            return new Multivector(BasisBlade.Simplify(L));
        }

        /// <summary>
        /// Geometric product of multivector and scalar.
        /// </summary>
        /// <param name="A">Multivector input (LHS)</param>
        /// <param name="b">Scalar input (RHS)</param>
        /// <returns>geometric product of <paramref name="A"/> and <paramref name="b"/></returns>
        public static Multivector GeometricProduct(Multivector A, double b)
        {
            if (b == 1.0) return A;
            else if (b == 0.0) return ZERO;
            else {
                BasisBlade[] newA = new BasisBlade[A.BasisBlades.Length];
                for (int i = 0; i < A.BasisBlades.Length; i++)
                    newA[i] = new BasisBlade(A.BasisBlades[i].bitmap, A.BasisBlades[i].scale * b, A.BasisBlades[i].symScale);
                return new Multivector(newA);
            }
        }

        /// <summary>
        /// Geometric product of multivectors.
        /// </summary>
        /// <param name="A">Multivector input (LHS)</param>
        /// <param name="B">Multivector input (RHS)</param>
        /// <returns>geometric product of <paramref name="A"/> and <paramref name="B"/></returns>
        public static Multivector GeometricProduct(Multivector A, Multivector B)
        {
            ArrayList L = new ArrayList();
            foreach (BasisBlade a in A.BasisBlades)
                foreach (BasisBlade b in B.BasisBlades)
                {
                    BasisBlade c = BasisBlade.GeometricProduct(a, b);
                    //System.Console.WriteLine(a.ToString() + " " + b.ToString() + " = " + c.ToString());
                    if (c.scale != 0.0) L.Add(c);
                }

            return new Multivector(BasisBlade.Simplify(L));
        }

        /// <summary>
        /// Geometric product of multivectors in diagonal non-Euclidean metric.
        /// </summary>
        /// <param name="A">Multivector input (LHS)</param>
        /// <param name="B">Multivector input (RHS)</param>
        /// <param name="m">array of doubles giving the metric for each basis vector (i.e., the diagonal of the diagonal metric matrix)</param>
        /// <returns>geometric product of <paramref name="A"/> and <paramref name="B"/></returns>
        public static Multivector GeometricProduct(Multivector A, Multivector B, double[] m)
        {
            ArrayList L = new ArrayList();
            foreach (BasisBlade a in A.BasisBlades)
                foreach (BasisBlade b in B.BasisBlades)
                {
                    BasisBlade c = BasisBlade.GeometricProduct(a, b, m);
                    if (c.scale != 0.0) L.Add(c);
                }

            return new Multivector(BasisBlade.Simplify(L));
        }

        /// <summary>
        /// Computes the geometric product of two basis blades in arbitary non-Euclidean metric.
        /// </summary>
        /// <param name="A">Multivector input (LHS)</param>
        /// <param name="B">Multivector input (RHS)</param>
        /// <param name="M">Metric to be used</param>
        /// <returns>geometric product of <paramref name="A"/> and <paramref name="B"/></returns>
        public static Multivector GeometricProduct(Multivector A, Multivector B, Metric M)
        {
            ArrayList L = new ArrayList();
            foreach (BasisBlade a in A.BasisBlades)
                foreach (BasisBlade b in B.BasisBlades)
                {
                    L.AddRange(BasisBlade.GeometricProduct(a, b, M));
                }

            return new Multivector(BasisBlade.Simplify(L));
        }

        /// <summary>
        /// Computes the inner product of two multivectors in Euclidean metric.
        /// </summary>
        /// <param name="A">input Multivector</param>
        /// <param name="B">input Multivector</param>
        /// <param name="type">inner product type to be used</param>
        /// <returns>Inner product of <paramref name="A"/> and <paramref name="B"/></returns>
        public static Multivector InnerProduct(Multivector A, Multivector B, BasisBlade.InnerProductType type)
        {
            ArrayList L = new ArrayList();
            foreach (BasisBlade a in A.BasisBlades)
                foreach (BasisBlade b in B.BasisBlades)
                {
                    BasisBlade c = BasisBlade.InnerProduct(a, b, type);
                    if (c.scale != 0.0) L.Add(c);
                }

            return new Multivector(BasisBlade.Simplify(L));            
        }

        /// <summary>
        /// Computes the inner product of two multivectors in diagonal non-Euclidean metric.
        /// </summary>
        /// <param name="A">input Multivector</param>
        /// <param name="B">input Multivector</param>
        /// <param name="m">an array of doubles giving the metric for each basis vector</param>
        /// <param name="type">inner product type to be used</param>
        /// <returns>Inner product of <paramref name="A"/> and <paramref name="B"/></returns>
        public static Multivector InnerProduct(Multivector A, Multivector B, double[] m, BasisBlade.InnerProductType type)
        {
            ArrayList L = new ArrayList();
            foreach (BasisBlade a in A.BasisBlades)
                foreach (BasisBlade b in B.BasisBlades)
                {
                    BasisBlade c = BasisBlade.InnerProduct(a, b, m, type);
                    if (c.scale != 0.0) L.Add(c);
                }

            return new Multivector(BasisBlade.Simplify(L));           
        }

        /// <summary>
        /// Computes the inner product of two multivectors in arbitary non-Euclidean metric.
        /// </summary>
        /// <param name="A">input Multivector</param>
        /// <param name="B">input Multivector</param>
        /// <param name="M">metric to be used</param>
        /// <param name="type">inner product type to be used</param>
        /// <returns>Inner product of <paramref name="A"/> and <paramref name="B"/></returns>
        public static Multivector InnerProduct(Multivector A, Multivector B, Metric M, BasisBlade.InnerProductType type)
        {
            ArrayList L = new ArrayList();
            foreach (BasisBlade a in A.BasisBlades)
                foreach (BasisBlade b in B.BasisBlades)
                {
                    L.AddRange(BasisBlade.InnerProduct(a, b, M, type));
                }

            return new Multivector(BasisBlade.Simplify(L));            
        }

        /// <summary>
        /// Computes the scalar product of two multivectors in Euclidean metric.
        /// </summary>
        /// <param name="A">input Multivector</param>
        /// <param name="B">input Multivector</param>
        public static Multivector ScalarProduct(Multivector A, Multivector B)
        {
            return InnerProduct(A, B, BasisBlade.InnerProductType.SCALAR_PRODUCT).ScalarPart();
        }

        /// <summary>
        /// Computes the scalar product of two multivectors in diagonal non-Euclidean metric.
        /// </summary>
        /// <param name="A">input Multivector</param>
        /// <param name="B">input Multivector</param>
        /// <param name="m">an array of doubles giving the metric for each basis vector</param>
        public static Multivector ScalarProduct(Multivector A, Multivector B, double[] m)
        {
            return InnerProduct(A, B, m, BasisBlade.InnerProductType.SCALAR_PRODUCT).ScalarPart();
        }

        /// <summary>
        /// Computes the scalar product of two multivectors in arbitary non-Euclidean metric.
        /// </summary>
        /// <param name="A">input Multivector</param>
        /// <param name="B">input Multivector</param>
        /// <param name="M">metric to be used</param>
        public static Multivector ScalarProduct(Multivector A, Multivector B, Metric M)
        {
            return InnerProduct(A, B, M, BasisBlade.InnerProductType.SCALAR_PRODUCT).ScalarPart();
        }


        /// <summary>Shortcut to OuterProduct() </summary>
        public static Multivector op(Multivector A, Multivector B) {return OuterProduct(A, B);}
        /// <summary>Shortcut to GeometricProduct() </summary>
        public static Multivector gp(Multivector A, double b) { return GeometricProduct(A, b); }
        /// <summary>Shortcut to GeometricProduct() </summary>
        public static Multivector gp(Multivector A, Multivector B) { return GeometricProduct(A, B); }
        /// <summary>Shortcut to GeometricProduct() </summary>
        public static Multivector gp(Multivector A, Multivector B, double[] m) { return GeometricProduct(A, B, m); }
        /// <summary>Shortcut to GeometricProduct() </summary>
        public static Multivector gp(Multivector A, Multivector B, Metric M) { return GeometricProduct(A, B, M); }
        /// <summary>Shortcut to InnerProduct() </summary>
        public static Multivector ip(Multivector A, Multivector B, BasisBlade.InnerProductType type) { return InnerProduct(A, B, type); }
        /// <summary>Shortcut to InnerProduct() </summary>
        public static Multivector ip(Multivector A, Multivector B, double[] m, BasisBlade.InnerProductType type) { return InnerProduct(A, B, m, type); }
        /// <summary>Shortcut to InnerProduct() </summary>
        public static Multivector ip(Multivector A, Multivector B, Metric M, BasisBlade.InnerProductType type) { return InnerProduct(A, B, M, type); }
        /// <summary>Shortcut to ScalarProduct() </summary>
        public static Multivector scp(Multivector A, Multivector B) { return ScalarProduct(A, B); }
        /// <summary>Shortcut to ScalarProduct() </summary>
        public static Multivector scp(Multivector A, Multivector B, double[] m) { return ScalarProduct(A, B, m); }
        /// <summary>Shortcut to ScalarProduct() </summary>
        public static Multivector scp(Multivector A, Multivector B, Metric M) { return ScalarProduct(A, B, M); }


        /// <summary>
        /// Computes the Hadamard product (coordinate-wise multiplication).
        /// </summary>
        /// <param name="A">input Multivector.</param>
        /// <param name="B">input Multivector.</param>
        /// <returns>Hadamard product of A and B.</returns>
        public static Multivector HadamardProduct(Multivector A, Multivector B)
        {
            ArrayList L = new ArrayList();
            foreach (BasisBlade a in A.BasisBlades)
                foreach (BasisBlade b in B.BasisBlades)
                {
                    BasisBlade h = BasisBlade.HadamardProduct(a, b);
                    if (h.scale != 0) L.Add(h);
                }
            return new Multivector(BasisBlade.Simplify(L));            
        }

        /// <summary>
        /// Computes the Hadamard product (coordinate-wise multiplication).
        /// </summary>
        /// <param name="A">input Multivector.</param>
        /// <param name="B">input Multivector.</param>
        /// <returns>Hadamard product of A and B.</returns>
        public static Multivector hp(Multivector A, Multivector B)
        {
            return HadamardProduct(A, B);
        }

        /// <summary>
        /// Computes the Inverse Hadamard product (coordinate-wise division).
        /// </summary>
        /// <param name="A">input Multivector.</param>
        /// <param name="B">input Multivector.</param>
        /// <returns>Inverse Hadamard product of A and B.</returns>
        public static Multivector InverseHadamardProduct(Multivector A, Multivector B)
        {
            ArrayList L = new ArrayList();
            foreach (BasisBlade a in A.BasisBlades)
                foreach (BasisBlade b in B.BasisBlades)
                {
                    BasisBlade h = BasisBlade.InverseHadamardProduct(a, b);
                    if (h.scale != 0) L.Add(h);
                }
            return new Multivector(BasisBlade.Simplify(L));
        }

        /// <summary>
        /// Computes the inverse Hadamard product (coordinate-wise division).
        /// </summary>
        /// <param name="A">input Multivector.</param>
        /// <param name="B">input Multivector.</param>
        /// <returns>Inverse Hadamard product of A and B.</returns>
        public static Multivector ihp(Multivector A, Multivector B)
        {
            return InverseHadamardProduct(A, B);
        }

        /// <summary>Returns only the scalar part of this multivector.</summary>
        /// <returns>The scalar part of this multivector.</returns>
        public Multivector ScalarPart()
        {
            // grade 0 is first in the list, so it must be the scalar or something else
            if ((m_basisBlades.Length > 0) && (m_basisBlades[0].bitmap == 0))
            {
                if (m_basisBlades.Length == 1) return this; // is this only a scalar?
                else return new Multivector(m_basisBlades[0]);
            }
            else return Multivector.ZERO;
        }

        /// <summary>Returns only the numerical scalar part of this multivector; not the symbolic part.</summary>
        /// <returns>The real (numerical) scalar part of this multivector; 
        /// does not return the symbolic scalar part.</returns>
        public double RealScalarPart()
        {
            // grade 0 is first in the list, so it must be the scalar or something else
            if ((m_basisBlades.Length > 0) && (m_basisBlades[0].bitmap == 0))
            {
                return m_basisBlades[0].scale;
            }
            else return 0.0;
        }

        /// <summary>Adds two multivectors.</summary>
        /// <returns>sum of <paramref name="A"/> and <paramref name="b"/></returns>
        public static Multivector Add(Multivector A, double b) {
            if (b == 0.0) return A;
            else if ((A.m_basisBlades.Length > 0) && (A.m_basisBlades[0].bitmap == 0))
            {
                // A is has a scalar part:
                if (A.BasisBlades[0].scale == -b)
                {
                    // scalar part is eliminated
                    BasisBlade[] newArray = new BasisBlade[A.m_basisBlades.Length - 1];
                    Array.Copy(A.m_basisBlades, 1, newArray, 0, A.m_basisBlades.Length-1); // copy, but skip scalar part
//                    newArray[0] = new BasisBlade(newArray[0].scale + b);
                    return new Multivector(newArray);

                }
                else
                {
                    BasisBlade[] newArray = new BasisBlade[A.m_basisBlades.Length];
                    Array.Copy(A.m_basisBlades, newArray, A.m_basisBlades.Length);
                    newArray[0] = new BasisBlade(newArray[0].scale + b);
                    return new Multivector(newArray);
                }
            }
            else
            {
                // no scalar part
                BasisBlade[] newArray = new BasisBlade[A.m_basisBlades.Length+1];
                newArray[0] = new BasisBlade(b);
                Array.Copy(A.m_basisBlades, 0, newArray, 1, A.m_basisBlades.Length);
                return new Multivector(newArray);
            }
        }

        /// <summary>Adds two multivectors.</summary>
        /// <returns>sum of <paramref name="a"/> and <paramref name="B"/></returns>
        public static Multivector Add(double a, Multivector B)
        {
            return Add(B, a);
        }

        /// <summary>Adds two multivectors.</summary>
        /// <returns>sum of <paramref name="A"> and <paramref name="B"/></returns>
        public static Multivector Add(Multivector A, Multivector B)
        {
            ArrayList sum = new ArrayList(); // (A.BasisBlades.Length, B.BasisBlades.Length);
            sum.AddRange(A.BasisBlades);
            sum.AddRange(B.BasisBlades);
            return new Multivector(BasisBlade.Simplify(sum));
        }

        /// <summary>Subtracts two multivectors.</summary>
        /// <returns><paramref name="A"/> - <paramref name="b"/></returns>
        public static Multivector Subtract(Multivector A, double b)
        {
            return Add(A, -b);
        }

        /// <summary>Subtracts two multivectors.</summary>
        /// <returns><paramref name="A"/> - <paramref name="b"/></returns>
        public static Multivector Subtract(double a, Multivector B)
        {
            return Add(a, Negate(B));
        }

        /// <summary>Subtracts two multivectors.</summary>
        /// <returns><paramref name="A"/> - <paramref name="B"/></returns>
        public static Multivector Subtract(Multivector A, Multivector B)
        {
            return Add(A, Negate(B));
        }

        /// <summary>Computes negation of Multivector <paramref name="A"/>.</summary>
        /// <returns>-<paramref name="A"/></returns>
        public static Multivector Negate(Multivector A)
        {
            BasisBlade[] newA = new BasisBlade[A.BasisBlades.Length];
            for (int i = 0; i < A.BasisBlades.Length; i++)
                newA[i] = new BasisBlade(A.BasisBlades[i].bitmap, -A.BasisBlades[i].scale, A.BasisBlades[i].symScale);
            return new Multivector(newA);
        }

        /// <summary>Computes reverse of multivector <paramref name="A"/>.</summary>
        /// <returns>Reverse of <paramref name="A"/></returns>
        public static Multivector Reverse(Multivector A)
        {
            BasisBlade[] newA = new BasisBlade[A.BasisBlades.Length];
            for (int i = 0; i < A.BasisBlades.Length; i++)
                newA[i] = A.BasisBlades[i].Reverse();
            return new Multivector(newA);
        }

        /// <summary>Computes grade involution (inversion) of multivector <paramref name="A"/>.</summary>
        /// <returns>Grade involution (inversion)  of <paramref name="A"/></returns>
        public static Multivector GradeInvolution(Multivector A)
        {
            return GradeInversion(A);
        }

        /// <summary>Computes grade inversion (involution) of multivector <paramref name="A"/>.</summary>
        /// <returns>Grade inversion (involution) of <paramref name="A"/></returns>
        public static Multivector GradeInversion(Multivector A)
        {
            BasisBlade[] newA = new BasisBlade[A.BasisBlades.Length];
            for (int i = 0; i < A.BasisBlades.Length; i++)
                newA[i] = A.BasisBlades[i].GradeInversion();
            return new Multivector(newA);
        }

        /// <summary>Computes Clifford conjugate of multivector <paramref name="A"/>.</summary>
        /// <returns>Clifford conjugate of <paramref name="A"/></returns>
        public static Multivector CliffordConjugate(Multivector A)
        {
            BasisBlade[] newA = new BasisBlade[A.BasisBlades.Length];
            for (int i = 0; i < A.BasisBlades.Length; i++)
                newA[i] = A.BasisBlades[i].CliffordConjugate();
            return new Multivector(newA);
        }

        /// <summary>
        /// Shortcut to Negate(this)
        /// </summary>
        public Multivector Negate() {return Negate(this);}
        /// <summary>
        /// Shortcut to Reverse(this)
        /// </summary>
        public Multivector Reverse() { return Reverse(this); }
        /// <summary>
        /// Shortcut to GradeInversion(this)
        /// </summary>
        public Multivector GradeInversion() { return GradeInversion(this); }
        /// <summary>
        /// Shortcut to CliffordConjugate(this)
        /// </summary>
        public Multivector CliffordConjugate() { return CliffordConjugate(this); }

        /// <summary>
        /// Internal function. Computes the versor inverse given the reverse R and the scale of (R reverse(R))
        /// Throws exception when non-invertible. Can handle symbolic scalars.
        /// </summary>
        /// <param name="R">Reverse of multivector</param>
        /// <param name="s">scale of (R reverse(R))</param>
        /// <returns>Inverse of Multivector reverse(R)</returns>
        private static Multivector VersorInverseInternal(Multivector R, Multivector s) {
            if (s.IsZero()) throw new Exception("non-invertible multivector");

            if (!s.HasSymbolicScalars())
            {
                // noni symbolic scalars: do true inverse
                double ss = s.BasisBlades[0].scale;
                return gp(R, 1.0 / ss);
            }
            else {
                // do symbolic inverse:
                return gp(R, RefGA.Symbolic.ScalarOp.Inverse(s));
            }
        }

        /// <summary>
        /// Computes versor inverse of <paramref name="A"/>.
        /// </summary>
        /// <param name="A">Multivector to be inverted.</param>
        /// <returns>Inverse of versor <paramref name="A"/></returns>
        /// <remarks>Computed using the versor inverse method, so <paramref name="A"/>must be a versor or a blade.</remarks>
        public static Multivector VersorInverse(Multivector A)
        {
            Multivector R = A.Reverse();
            Multivector s = scp(A, R);
            return VersorInverseInternal(R, s);
        }

        /// <summary>
        /// Computes versor inverse of <paramref name="A"/>.
        /// </summary>
        /// <param name="A">Multivector to be inverted.</param>
        /// <param name="m">The metric to be used for the inverse.</param>
        /// <returns>Inverse of versor <paramref name="A"/>.</returns>
        /// <remarks>Computed using the versor inverse method, so <paramref name="A"/>must be a versor or a blade.</remarks>
        public static Multivector VersorInverse(Multivector A, double[] m)
        {
            Multivector R = A.Reverse();
            Multivector s = scp(A, R, m);
            return VersorInverseInternal(R, s);
        }

        /// <summary>
        /// Computes versor inverse of <paramref name="A"/>.
        /// </summary>
        /// <param name="A">Multivector to be inverted.</param>
        /// <param name="M">The metric to be used for the inverse.</param>
        /// <returns>Inverse of versor <paramref name="A"/></returns>
        /// <remarks>Computed using the versor inverse method, so <paramref name="A"/>must be a versor or a blade.</remarks>
        public static Multivector VersorInverse(Multivector A, Metric M)
        {
            Multivector R = A.Reverse();
            Multivector s = scp(A, R, M);
            return VersorInverseInternal(R, s);
        }

        /// <summary>
        /// Computes versor inverse of this.
        /// </summary>
        /// <returns>Inverse of this versor </returns>
        /// <remarks>Computed using the versor inverse method, so <paramref name="A"/>must be a versor or a blade.</remarks>
        public Multivector VersorInverse() { return VersorInverse(this); }
        /// <summary>
        /// Computes versor inverse of this.
        /// </summary>
        /// <param name="m">The metric to be used for the inverse</param>
        /// <returns>Inverse of this versor</returns>
        /// <remarks>Computed using the versor inverse method, so <paramref name="A"/>must be a versor or a blade.</remarks>
        public Multivector VersorInverse(double[] m) { return VersorInverse(this, m); }
        /// <summary>
        /// Computes versor inverse of this.
        /// </summary>
        /// <param name="M">The metric to be used for the inverse</param>
        /// <returns>Inverse of this versor</returns>
        /// <remarks>Computed using the versor inverse method, so <paramref name="A"/>must be a versor or a blade.</remarks>
        public Multivector VersorInverse(Metric M) { return VersorInverse(this, M); }


        /// <summary>Computes the dual of <paramref name="A"/> with respect to the whole space. The
        /// space has dimension <paramref name="dim"/>.</summary>
        /// <param name="dim">Dimension of space with respect to which you want to dualize</param>
        /// <returns>dual of <param name="A"/></returns>
        public static Multivector Dual(Multivector A, int dim)
        {
            Multivector Ii = Multivector.GetPseudoscalar(dim).Reverse();
            return ip(A, Ii, BasisBlade.InnerProductType.LEFT_CONTRACTION);
        }

        /// <summary>Computes the dual of <paramref name="A"/> with respect to the whole space. The
        /// space has the dimension specified by the metric <paramref name="m"/>.</summary>
        /// <param name="m">The metric to be used.</param>
        /// <returns>dual of <param name="A"/></returns>
        public static Multivector Dual(Multivector A, double[] m)
        {
            Multivector Ii = Multivector.GetPseudoscalar(m.Length).VersorInverse(m);
            return ip(A, Ii, m, BasisBlade.InnerProductType.LEFT_CONTRACTION);
        }

        /// <summary>Computes the dual of <paramref name="A"/> with respect to the whole space. The
        /// space has the dimension specified by the metric <paramref name="M"/>.</summary>
        /// <param name="M">The metric to be used</param>
        /// <returns>dual of <param name="A"/></returns>
        public static Multivector Dual(Multivector A, Metric M)
        {
            Multivector Ii = Multivector.GetPseudoscalar(M.EigenMetric.Length).VersorInverse(M);
            return ip(A, Ii, M, BasisBlade.InnerProductType.LEFT_CONTRACTION);
        }

        /// <summary>Computes the dual of this with respect to the whole space. The
        /// space has dimension <paramref name="dim"/>.</summary>
        /// <param name="dim">Dimension of space with respect to which you want to dualize</param>
        /// <returns>dual of this</returns>
        public Multivector Dual(int dim) { return Multivector.Dual(this, dim); }
        /// <summary>Computes the dual of <paramref name="A"/> with respect to the whole space. The
        /// space has the dimension specified by the metric <paramref name="m"/>.</summary>
        /// <param name="m">The metric to be used</param>
        /// <returns>dual of this</returns>
        public Multivector Dual(double[] m) { return Multivector.Dual(this, m); }
        /// <summary>Computes the dual of <paramref name="A"/> with respect to the whole space. The
        /// space has the dimension specified by the metric <paramref name="M"/>.</summary>
        /// <param name="M">The metric to be used</param>
        /// <returns>dual of this</returns>
        public Multivector Dual(Metric M) { return Multivector.Dual(this, M); }

        /// <summary>Computes the undual of <paramref name="A"/> with respect to the whole space. The
        /// space has dimension <paramref name="dim"/>.</summary>
        /// <param name="dim">Dimension of space with respect to which you want to undualize</param>
        /// <returns>dual of <param name="A"/></returns>
        public static Multivector Undual(Multivector A, int dim)
        {
            Multivector Ii = Multivector.GetPseudoscalar(dim);
            return ip(A, Ii, BasisBlade.InnerProductType.LEFT_CONTRACTION);
        }

        /// <summary>Computes the undual of <paramref name="A"/> with respect to the whole space. The
        /// space has the dimension specified by the metric <paramref name="m"/>.</summary>
        /// <param name="m">The metric to be used.</param>
        /// <returns>dual of <param name="A"/></returns>
        public static Multivector Undual(Multivector A, double[] m)
        {
            Multivector Ii = Multivector.GetPseudoscalar(m.Length);
            return ip(A, Ii, m, BasisBlade.InnerProductType.LEFT_CONTRACTION);
        }

        /// <summary>Computes the undual of <paramref name="A"/> with respect to the whole space. The
        /// space has the dimension specified by the metric <paramref name="M"/>.</summary>
        /// <param name="M">The metric to be used</param>
        /// <returns>dual of <param name="A"/></returns>
        public static Multivector Undual(Multivector A, Metric M)
        {
            Multivector Ii = Multivector.GetPseudoscalar(M.EigenMetric.Length);
            return ip(A, Ii, M, BasisBlade.InnerProductType.LEFT_CONTRACTION);
        }

        /// <summary>Computes the undual of this with respect to the whole space. The
        /// space has dimension <paramref name="dim"/>.</summary>
        /// <param name="dim">Dimension of space with respect to which you want to undualize</param>
        /// <returns>dual of this</returns>
        public Multivector Undual(int dim) { return Multivector.Undual(this, dim); }
        /// <summary>Computes the undual of <paramref name="A"/> with respect to the whole space. The
        /// space has the dimension specified by the metric <paramref name="m"/>.</summary>
        /// <param name="m">The metric to be used</param>
        /// <returns>dual of this</returns>
        public Multivector Undual(double[] m) { return Multivector.Undual(this, m); }
        /// <summary>Computes the undual of <paramref name="A"/> with respect to the whole space. The
        /// space has the dimension specified by the metric <paramref name="M"/>.</summary>
        /// <param name="M">The metric to be used</param>
        /// <returns>dual of this</returns>
        public Multivector Undual(Metric M) { return Multivector.Undual(this, M); }

        /// <summary>
        /// Computes the square of 'this' multivector. Used by exp(), sin(), cos()
        /// Can throw exception when symbolic multivector input does not have a scalar square
        /// </summary>
        /// <param name="M">The metric to be used</param>
        /// <param name="squareHint">Used to returned whether square is scalar</param>
        /// <returns>Scalar square (-1, 0, 1) (or when squareHint == false, 0.0) </returns>
        protected double ComputeSquare(Object M, ref bool squareHint) 
        {
            // check if there are symbolic scalars in 'M'
            bool symbolic = HasSymbolicScalars();
            const double EPS = 1e-6;

            double square = 0.0;
            if (symbolic)
            {
                try
                {
                    square = Symbolic.SymbolicUtil.EvaluateRandomSymbolicToScalar(Multivector._gp(this, this, M));
                    squareHint = true;
                }
                catch (System.Exception)
                { // symbolic evalutation failed
                    // cannot evaluate exp(symbolic MV) without hint!
                    squareHint = false;
                }
            }
            else // no symbolic, so just compute the square; if it is a scalar: set squareHint
            {
                Multivector A2 = Multivector._gp(this, this, M);
                double scalarPart = A2.RealScalarPart();
                squareHint = (Multivector.Subtract(A2, scalarPart).Norm_e().RealScalarPart() * EPS < scalarPart);
                if (squareHint) square = (scalarPart < 0.0) ? -1.0 : ((scalarPart > 0.0) ? 1.0 : 0.0);
            }

            return square; // we return 0.0 when squareHint = false; +1, 0, -1 otherwise
        }

        /// <summary>
        /// Calls Exp(null, false, 0.0);
        /// </summary>
        public Multivector Exp()
        {
            Object metric = null;
            bool squareHint = false;
            double square = 0.0;
            return Exp(metric, squareHint, square);
        }
            
        /// <summary>
        /// Calls Exp(M, false, 0.0);
        /// </summary>
        public Multivector Exp(Object M)
        {
            bool squareHint = false;
            double square = 0.0;
            return Exp(M, squareHint, square);
        }

        /// <summary>
        /// Computes the exponential of this multivector
        /// </summary>
        /// <param name="M">Metric object</param>
        /// <param name="squareHint">Whether a value for the square (+1, 0, -1) is provided</param>
        /// <param name="square">The square of this (positive: +1, negative: -1, otherwise: 0</param>
        /// <returns>exponential of this; can throw exception if symbolic scalars are used</returns>
        public Multivector Exp(Object M, bool squareHint, double square)
        {
            // check if there are symbolic scalars in 'M'
            bool symbolic = HasSymbolicScalars();

            if (!squareHint) // user did not provide a hint on the square, so try to compute yourself
                square = ComputeSquare(M, ref squareHint);

            if ((!squareHint) && symbolic)
                throw new Exception("Cannot compute exp() of symbolic multivector with non-scalar square");

            //if (squareHint)
            //    System.Console.WriteLine("exp(): square = " + square);
            //else System.Console.WriteLine("exp(): no square hint!");
            if (squareHint) return ExpScalarSquare(M, square);
            else return ExpSeries(M, 12);
        }


        /// <summary>Computes exponential of this Multivector given its scalar square <paramref name="square"/>.</summary>
        /// <param name="M">The metric to be used.</param>
        /// <param name="square">The scalar square of this (positive: +1, negative: -1, otherwise: 0</param>
        /// <returns>exp(this) given the value of the square of this</returns>
        protected Multivector ExpScalarSquare(Object M, double square) {
            // See page 606 of GA4CS
            if (square == 0.0) // we can test for '0.0' because input is already thresholded
            {
                // return 1 + this
                return Multivector.Add(this, 1.0);
            }
            else
            {
                Multivector A2 = Multivector._gp(this, this, M);
                if (square < 0)
                {
                    Multivector sqrtA2 = Symbolic.ScalarOp.Sqrt(Multivector.Negate(A2));

                    return Multivector.Add(Symbolic.ScalarOp.Cos(sqrtA2),
                            Multivector.gp(Multivector.gp(Symbolic.ScalarOp.Sin(sqrtA2), Symbolic.ScalarOp.Inverse(sqrtA2)), this));
                }
                else
                { // square > 0
                    Multivector sqrtA2 = Symbolic.ScalarOp.Sqrt(A2);

                    return Multivector.Add(Symbolic.ScalarOp.Cosh(sqrtA2),
                            Multivector.gp(Multivector.gp(Symbolic.ScalarOp.Sinh(sqrtA2), Symbolic.ScalarOp.Inverse(sqrtA2)), this));
                }
            }
        }

        /// <summary>Computes exponential of this by evaluating the series up to <paramref name="order"/>.</summary>
        /// <remarks>
        /// 'this' may not have symbolic scalars! (or else will evaluate really slowly!)
        /// </remarks>
        /// <param name="M">The metric to be used.</param>
        /// <param name="order">The 'order' up to which the series is evaluated.</param>
        /// <returns>exp(this) given the value of the square of this</returns>
        protected Multivector ExpSeries(Object M, int order)
        {
		    // first scale by power of 2 so that its norm is ~ 1
		    long scale = 1; 
            {
			    double max = this.Norm_e().RealScalarPart();
			    if (max > 1.0) scale <<= 1;
			    while (max > 1.0) {
				    max = max / 2.0;
				    scale <<= 1;
			    }
		    }

		    Multivector scaled = Multivector.gp(this, 1.0 / scale);

		    // taylor approximation
		    Multivector result = new Multivector(1.0); 
            {
			    Multivector tmp = new Multivector(1.0);

			    for (int i = 1; i < order; i++) {
				    tmp = Multivector._gp(tmp, Multivector.gp(scaled, 1.0 / i), M);
				    result = Multivector.Add(result, tmp);
			    }
		    }

		    // undo scaling
		    while (scale > 1) {
			    result = Multivector._gp(result, result, M);
			    scale >>= 1;
		    }

		    return result;
        }

        /// <summary>
        /// Calls sin(M, false, 0.0);
        /// </summary>
        public Multivector Sin(Object M)
        {
            bool squareHint = false;
            double square = 0.0;
            return Sin(M, squareHint, square);
        }

        /// <summary>
        /// Computes the sine of this multivector
        /// </summary>
        /// <param name="M">Metric object</param>
        /// <param name="squareHint">Whether a value for the square (+1, 0, -1) is provided</param>
        /// <param name="square">The square of this (positive: +1, negative: -1, otherwise: 0</param>
        /// <returns>sine of this; can throw exception if symbolic scalars are used</returns>
        public Multivector Sin(Object M, bool squareHint, double square)
        {
            // check if there are symbolic scalars in 'M'
            bool symbolic = HasSymbolicScalars();

            if (!squareHint) // user did not provide a hint on the square, so try to compute yourself
                square = ComputeSquare(M, ref squareHint);

            if ((!squareHint) && symbolic)
                throw new Exception("Cannot compute Sin() of symbolic multivector with non-scalar square");

            if (squareHint) return SinScalarSquare(M, square);
            else return SinSeries(M, 12);
        }


        /// <summary>Computes sine of this Multivector given its scalar square <paramref name="square"/>.</summary>
        /// <param name="M">The metric to be used.</param>
        /// <param name="square">The scalar square of this (positive: +1, negative: -1, otherwise: 0</param>
        /// <returns>Sin(this) given the value of the square of this</returns>
        protected Multivector SinScalarSquare(Object M, double square)
        {
            // See page 606 of GA4CS
            if (square == 0.0) // we can test for '0.0' because input is already thresholded
            {
                // return this
                return this;
            }
            else
            {
                Multivector A2 = Multivector._gp(this, this, M);
                if (square < 0)
                {
                    Multivector sqrtA2 = Symbolic.ScalarOp.Sqrt(Multivector.Negate(A2));

                    return Multivector.gp(Multivector.gp(Symbolic.ScalarOp.Sinh(sqrtA2), Symbolic.ScalarOp.Inverse(sqrtA2)), this);
                }
                else
                { // square > 0
                    Multivector sqrtA2 = Symbolic.ScalarOp.Sqrt(A2);

                    return Multivector.gp(Multivector.gp(Symbolic.ScalarOp.Sin(sqrtA2), Symbolic.ScalarOp.Inverse(sqrtA2)), this);
                }
            }
        }

        /// <summary>Computes sine of this by evaluating the series up to <paramref name="order"/>.</summary>
        /// <remarks>
        /// 'this' may not have symbolic scalars! (or else will evaluate really slowly!)
        /// </remarks>
        /// <param name="M">The metric to be used.</param>
        /// <param name="order">The 'order' up to which the series is evaluated.</param>
        /// <returns>Sin(this) given the value of the square of this</returns>
        protected Multivector SinSeries(Object M, int order)
        {
            Multivector scaled = this;

            // taylor approximation
            Multivector result = scaled;
            {
                Multivector tmp = scaled;

                int sign = -1;
                for (int i = 2; i < order; i++)
                {
                    tmp = Multivector._gp(tmp, Multivector.gp(scaled, 1.0 / i), M);
                    if ((i & 1) != 0)
                    {// only the odd part of the series
                        result = Multivector.Add(result, Multivector.gp(tmp, (double)sign));
                        sign *= -1;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Calls Cos(M, false, 0.0);
        /// </summary>
        public Multivector Cos(Object M)
        {
            bool squareHint = false;
            double square = 0.0;
            return Cos(M, squareHint, square);
        }

        /// <summary>
        /// Computes the cosine of this multivector
        /// </summary>
        /// <param name="M">Metric object</param>
        /// <param name="squareHint">Whether a value for the square (+1, 0, -1) is provided</param>
        /// <param name="square">The square of this (positive: +1, negative: -1, otherwise: 0</param>
        /// <returns>cosine of this; can throw exception if symbolic scalars are used</returns>
        public Multivector Cos(Object M, bool squareHint, double square)
        {
            // check if there are symbolic scalars in 'M'
            bool symbolic = HasSymbolicScalars();

            if (!squareHint) // user did not provide a hint on the square, so try to compute yourself
                square = ComputeSquare(M, ref squareHint);

            if ((!squareHint) && symbolic)
                throw new Exception("Cannot compute Cos() of symbolic multivector with non-scalar square");

            if (squareHint) return CosScalarSquare(M, square);
            else return CosSeries(M, 12);
        }


        /// <summary>Computes cosine of this Multivector given its scalar square <paramref name="square"/>.</summary>
        /// <param name="M">The metric to be used.</param>
        /// <param name="square">The scalar square of this (positive: +1, negative: -1, otherwise: 0</param>
        /// <returns>Cos(this) given the value of the square of this</returns>
        protected Multivector CosScalarSquare(Object M, double square)
        {
            // See page 606 of GA4CS
            if (square == 0.0) // we can test for '0.0' because input is already thresholded
            {
                // return this
                return this;
            }
            else
            {
                Multivector A2 = Multivector._gp(this, this, M);
                if (square < 0)
                {
                    Multivector sqrtA2 = Symbolic.ScalarOp.Sqrt(Multivector.Negate(A2));

                    return Multivector.gp(Multivector.gp(Symbolic.ScalarOp.Cosh(sqrtA2), Symbolic.ScalarOp.Inverse(sqrtA2)), this);
                }
                else
                { // square > 0
                    Multivector sqrtA2 = Symbolic.ScalarOp.Sqrt(A2);

                    return Multivector.gp(Multivector.gp(Symbolic.ScalarOp.Cos(sqrtA2), Symbolic.ScalarOp.Inverse(sqrtA2)), this);
                }
            }
        }

        /// <summary>Computes cosine of this by evaluating the series up to <paramref name="order"/>.</summary>
        /// <remarks>
        /// 'this' may not have symbolic scalars! (or else will evaluate really slowly!)
        /// </remarks>
        /// <param name="M">The metric to be used.</param>
        /// <param name="order">The 'order' up to which the series is evaluated.</param>
        /// <returns>Cos(this) given the value of the square of this</returns>
        protected Multivector CosSeries(Object M, int order)
        {
            Multivector scaled = this;

            // taylor approximation
            Multivector result = scaled;
            {
                Multivector tmp = scaled;

                int sign = -1;
                for (int i = 2; i < order; i++)
                {
                    tmp = Multivector._gp(tmp, Multivector.gp(scaled, 1.0 / i), M);
                    if ((i & 1) != 0)
                    {// only the odd part of the series
                        result = Multivector.Add(result, Multivector.gp(tmp, (double)sign));
                        sign *= -1;
                    }
                }
            }

            return result;
        }



        /// <summary>For internal use; Computes geometric product using any metric <paramref name="M"/>.
        /// <paramref name="M"/> can be null, Metric or double[]</summary>
        protected static Multivector _gp(Multivector A, Multivector B, Object M) {
		    if (M == null) return gp(A, B);
		    else if (M is Metric) return gp(A, B, (Metric)M);
            else return gp(A, B, (double[])M);
        }

        /// <summary>Computes the norm given the squared norm of this multivector.</summary>
        /// <returns>Norm of 'this', given norm squared 'N2'</returns>
        protected Multivector Norm(Multivector N2, bool positiveDefinite)
        {
            if (N2.IsZero()) return Multivector.ZERO;
            if (!N2.HasSymbolicScalars())
            {
                double s = N2.BasisBlades[0].scale;
                return new Multivector(Math.Sign(s) * Math.Sqrt(Math.Abs(s)));
            }
            else
            {
                // if metric is note positive definite, add a fabs inside the sqrt because otherwise we could do sqrt(negative value)
                // TODO: maybe just also have a ABS because floating point noise can make a (small) positive or zero value negative . . .
                if (!positiveDefinite) N2 = RefGA.Symbolic.ScalarOp.Abs(N2);
                return RefGA.Symbolic.ScalarOp.Sqrt(N2); // NOTE: this is not the same result as the numerical version above!!!
            }
        }

        /// <summary>Computes the Euclidean norm of this multivector.</summary>
        /// <returns>Euclidean norm of this Multivector</returns>
        public Multivector Norm_e()
        {
            bool positiveDefinite = true;
            return Norm(Multivector.scp(this, Reverse()), positiveDefinite);
        }

        /// <summary>Computes the `reverse norm' of this multivector (this.reverse(this)).</summary>
        /// <returns>`reverse norm' of this Multivector in Euclidean metric</returns>
        /// <remarks>The reverse norm is s*sqrt(s * this.reverse(this)), where 's' is either +1 or -1,
        /// such that the square root is always performed on a number >= 0.</remarks>
        public Multivector Norm_r()
        {
            bool positiveDefinite = true;
            return Norm(Multivector.scp(this, Reverse()), positiveDefinite);
        }

        /// <summary>Computes the `reverse norm' of this multivector (this.reverse(this)) given a specific metric <paramref name="m"/></paramref>.</summary>
        /// <param name="m">The metric to be used in the inner product.</param>
        /// <returns>`reverse norm' of this Multivector in specified metric 'm' (can be negative).</returns>
        /// <remarks>The reverse norm is s*sqrt(s * this.reverse(this)), where 's' is either +1 or -1,
        /// such that the square root is always performed on a number >= 0.</remarks>
        public Multivector Norm_r(double[] m)
        {
            bool positiveDefinite = Metric.IsPositiveDefinite(m);
            return Norm(Multivector.scp(this, Reverse(), m), positiveDefinite);
        }

        /// <summary>Computes the `reverse norm' of this multivector (this.reverse(this)) given a specific metric <paramref name="M"/></paramref>.</summary>
        /// <param name="M">The metric to be used in the inner product.</param>
        /// <returns>`reverse norm' of this Multivector in specified metric 'M' (can be negative)</returns>
        /// <remarks>The reverse norm is s*sqrt(s * this.reverse(this)), where 's' is either +1 or -1,
        /// such that the square root is always performed on a number >= 0.</remarks>
        public Multivector Norm_r(Metric M)
        {
            //M.IsPositiveDefinite
            return Norm(Multivector.scp(this, Reverse(), M), M.IsPositiveDefinite());
        }


        /// <summary>Computes the squared `reverse norm' of this multivector (this.reverse(this)) given a specific metric <paramref name="m"/></paramref>.</summary>
        /// <returns>squared `reverse norm' of this Multivector in specified metric 'm' (can be negative).</returns>
        public Multivector Norm_r2()
        {
            return Multivector.scp(this, Reverse());
        }

        /// <summary>Computes the squared `reverse norm' of this multivector (this.reverse(this)) given a specific metric <paramref name="m"/></paramref>.</summary>
        /// <param name="m">The metric to be used in the inner product.</param>
        /// <returns>squared `reverse norm' of this Multivector in specified metric 'm' (can be negative).</returns>
        public Multivector Norm_r2(double[] m)
        {
            return Multivector.scp(this, Reverse(), m);
        }

        /// <summary>Computes the squared `reverse norm' of this multivector (this.reverse(this)) given a specific metric <paramref name="m"/></paramref>.</summary>
        /// <param name="M">The metric to be used in the inner product.</param>
        /// <returns>squared `reverse norm' of this Multivector in specified metric 'm' (can be negative).</returns>
        public Multivector Norm_r2(Metric M)
        {
            return Multivector.scp(this, Reverse(), M);
        }


        /// <summary>Computes unit version of this multivector given its norm <paramref name="N"/>.</summary>
        /// <param name="N">The norm of the multivector.</param>
        /// <returns>Normalized Multivector</returns>
        /// <remarks>Throws exception when norm is 0.</remarks>
        protected Multivector Unit(Multivector N)
        {
            // zero -> throw exception
            if (N.IsZero()) throw new ArgumentException("Multivector.Unit_e(): zero norm");
            // scalar: the unit scalar is 1
            if (this.IsScalar()) return new Multivector(1.0);

            // if no symbolic scalars, then we can actually compute:
            if (!N.HasSymbolicScalars())
                return Multivector.gp(this, 1.0 / N.BasisBlades[0].scale);
            // otherwise: symbolically invert
            else return Multivector.gp(this, RefGA.Symbolic.ScalarOp.Inverse(N));
        }

        /// <summary>Computes unit version of this multivector using Euclidean norm.</summary>
        /// <returns>normalized Multivector, according to Euclidean norm.</returns>
        /// <remarks>Throws exception when norm is 0.</remarks>
        public Multivector Unit_e()
        {
            return Unit(Norm_e());
        }

        /// <summary>Computes unit version of this multivector using Reverse norm.</summary>
        /// <returns>normalized Multivector, according to reverse norm</returns>
        /// <remarks>Throws exception when norm is 0. 
        /// If the norm is negative, the norm of the returned Multivector will be -1.</remarks>
        public Multivector Unit_r()
        {
            return Unit(Norm_r());
        }

        /// <summary>Computes unit version of this multivector using Reverse norm.</summary>
        /// <returns>normalized Multivector, according to specific metric/norm</returns>
        /// <remarks>Throws exception when norm is 0. 
        /// If the norm is negative, the norm of the returned Multivector will be -1</remarks>
        public Multivector Unit_r(double[] m)
        {
            return Unit(Norm_r(m));
        }

        /// <summary>Computes unit version of this multivector using Reverse norm.</summary>
        /// <returns>normalized Multivector, according to specific metric/norm</returns>
        /// <remarks>Throws exception when norm is 0. 
        /// If the norm is negative, the norm of the returned Multivector will be -1</remarks>
        public Multivector Unit_r(Metric M)
        {
            return Unit(Norm_r(M));
        }

        /// <summary>Returns a bitmap of what gradeparts are non-zero.</summary>
        /// <returns>Bitmap of what gradeparts are non-zero. Bit 0 stands for grade 0 (scalar),
        /// bit 1 for grade 1 (vector) and so on.</returns>
        public int GradeUsage()
        {
            int gu = 0;
            foreach (BasisBlade b in BasisBlades)
            {
                if (b.scale == 0.0) continue; // when scale = 0, then does not influence grade
                int bg = b.Grade(); // get grade of BasisBlade
                gu |= 1 << bg;
            }
            return gu;
        }

        /// <summary>Returns the grade of this multivector.</summary>
        /// <returns>Grade of this multivector, or -1 when not of a single grade</returns>
        public int Grade()
        {
            int g = -1; // grade goes here
            foreach (BasisBlade b in BasisBlades)
            {
                if (b.scale == 0.0) continue; // when scale = 0, then does not influence grade
                int bg = b.Grade(); // get grade of BasisBlade
                if (g < 0) g = bg; // if grade 'g' not set yet, then do so now
                else if (g != bg) return -1; // otherwise, if grades do not match, then
            }
            return (g > 0) ? g : 0;  // NOTE: when blade has value '0' (g == -1), then grade = 0;
        }

        /// <summary>Returns the largest grade part (by Euclidean norm) of this multivector.</summary>
        /// <returns>grade of the largest gradepart this multivector, or -1 when multivector is zero.</returns>
        /// <remarks>Cannot handles symbolic scalars (throws Exception).</remarks>
        public int LargestGradePartIndex()
        {
            int lgp = -1; // largest grade part
            double lgpNorm = 0.0;

            int currentGrade = 0;
            double currentGradeNorm = 0.0;
            foreach (BasisBlade b in BasisBlades)
            {
                if (b.scale == 0.0) continue; // when scale = 0, then does not influence grade
                if (b.symScale != null) throw new ArgumentException("Multivector.LargestGradePart(): cannot determine norm of symbolic scalars");
                int bg = b.Grade(); // get grade of BasisBlade
                if (bg != currentGrade)
                {
                    if (currentGradeNorm > lgpNorm)
                    {
                        lgp = currentGrade;
                        lgpNorm = currentGradeNorm;
                    }
                    currentGrade = bg; // set grade of next grade part
                    currentGradeNorm = b.scale * b.scale; // initialize norm of next grade part
                }
                else currentGradeNorm += b.scale * b.scale;
            }

            // round up for next grade part:
            if (currentGradeNorm > lgpNorm)
            {
                lgp = currentGrade;
                currentGradeNorm = lgpNorm;
            }

            return lgp;
        }

        /// <summary>Index of the top grade part (grade part with largest index that is not zero).</summary>
        /// <returns>index of the top grade part that has a non-zero norm. 
        /// E.g. If the multivector is <c>1 + e1+e2</c>, then the function returns 2.
        /// Returns 0 when multivector is zero.</returns>
        /// <remarks>Note: symbolic scales are assumed to be non-zero.
        /// This behavior is different from TopGradePartIndex(epsilon) which 
        /// will throw an exception when it encounters symbolic scales!</remarks>
        public int TopGradePartIndex() 
        {
            if (BasisBlades.Length == 0) return 0;

            // visit all basis blades in reverse order (from high grade to low grade) and return grade when non-zero scale is found
            int i = BasisBlades.Length-1;
            while (i >= 0)
            {
                BasisBlade b = BasisBlades[i];
                if ((b.scale != 0) || (b.symScale != null)) return b.Grade();
                i--;
            }
            return 0;
        }

        /// <summary>Index of the top grade part (grade part with largest index that 
        /// is not zero relative to small <paramref name="epsilon"/>).</summary>
        /// <returns>index of the top grade part that has a (Euclidean) norm >= epsilon</returns>
        /// <remarks>Throws an exception when encounters symbolic scales!</remarks>
        public int TopGradePartIndex(double epsilon) {
            double eps2 = epsilon * epsilon;
            if (BasisBlades.Length == 0) return 0;

            int currentGrade = 0;
            double currentGradeNorm = 0.0;
            for (int i = BasisBlades.Length - 1; i >= 0; i--) 
            {
                BasisBlade b = BasisBlades[i];
                if (b.scale == 0.0) continue; // when scale = 0, then does not influence grade
                if (b.symScale != null) throw new ArgumentException("Multivector.TopGradePartIndex(epsilon): cannot determine norm of symbolic scalars");
                int bg = b.Grade(); // get grade of BasisBlade
                if (bg != currentGrade) 
                {
                    currentGrade = bg;
                    currentGradeNorm = b.scale * b.scale; // initialize norm of next grade part
                }
                else
                {
                    currentGradeNorm += b.scale * b.scale;
                    if (currentGradeNorm > eps2) return currentGrade;
                }
            }

            return 0;
        }

        /// <summary>
        /// Extracts one grade part from a multivector.
        /// 
        /// For search: Selects a grade part. 
        /// </summary>
        /// <param name="g">The index of the grade part to be extracted</param>
        /// <returns>The requested grade part.</returns>
        public Multivector ExtractGrade(int g)
        {
            return ExtractGrade(new int[] { g });
        }

        /// <summary>
        /// Extracts multiple grade parts from a multivector
        /// 
        /// For search: Selects a grade part. 
        /// </summary>
        /// <param name="G">The indices of the grade parts to be extracted</param>
        /// <returns>The requested grade part(s).</returns>
        public Multivector ExtractGrade(int[] G)
        {
            // what is the maximum grade to be extracted?
            int maxG = 0;
            for (int i = 0; i < G.Length; i++)
                if (G[i] > maxG) maxG = G[i];

            // create boolean array of what grade to keep
            bool[] keep = new bool[maxG + 1];
            for (int i = 0; i < G.Length; i++)
                keep[G[i]] = true;

            // extract the grade, store in result:
            ArrayList result = new ArrayList();
            foreach (BasisBlade b in m_basisBlades) 
            {
                int g = b.Grade();
                if (g > maxG) break; // basis blades are sorted by grade, so if we get to grade > maxG, then we're done
                else if (keep[g]) result.Add(b);
            }

            return new Multivector(result);
        }

        public static int[] GradeBitmapToArray(int bitmap)
        {
            // count number of non-zero bits
            int nbBits = (int)Bits.BitCount((uint)bitmap);

            // allocate 
            int[] G = new int[nbBits];

            // enlist each non-zero bit in 'G'
            int idx = 0;
            for (int i = 0; (1 << i) <= bitmap; i++)
            {
                if ((bitmap & (1 << i)) != 0) G[idx++] = i;
            }
            return G;
        }

        /// <summary>Computes whether this Multivector is zero (truly zero, does not use some epsilon value).</summary>
        /// <returns>true when this multivector is zero</returns>
        public bool IsZero()
        {
            foreach (BasisBlade b in m_basisBlades)
            {
                if (b.scale != 0.0) return false;
                if (b.symScale != null) return false;
            }
            return true;
        }

        /// <summary>Computes whether this Multivector is a scalar.</summary>
        /// <returns>True when this multivector a scalar (0 is also considered to be a scalar!)</returns>
        public bool IsScalar()
        {
            foreach (BasisBlade b in m_basisBlades)
                if ((b.bitmap != 0) && (b.scale != 0.0)) return false; // if bitmap does not indicate scalar, and scale is non-zero, then not a scalar
            return true;
        }

        /// <summary>Computes whether this Multivector has any symbolic scalars.</summary>
        /// <returns>whether any symbolic scalars are used in this multivector</returns>
        public bool HasSymbolicScalars()
        {
            foreach (BasisBlade b in m_basisBlades)
                if (b.symScale != null) return true;
            return false;
        }


        /// <summary>outer product operator </summary>
        public static Multivector operator ^(Multivector lhs, Multivector rhs)
        {
            return OuterProduct(lhs, rhs);
        }

        /// <summary>addition operator </summary>
        public static Multivector operator +(Multivector lhs, Multivector rhs)
        {
            return Add(lhs, rhs);
        }

        /// <summary>subtraction operator </summary>
        public static Multivector operator -(Multivector lhs, Multivector rhs)
        {
            return Subtract(lhs, rhs);
        }

        /// <summary>unary negation operator </summary>
        public static Multivector operator -(Multivector arg)
        {
            return Negate(arg);
        }

        /// <summary>unary negation operator </summary>
        public static Multivector operator ~(Multivector arg)
        {
            return Reverse(arg);
        }

        /// <summary>
        /// Symbolically evaluates this multivector.
        /// Substitutes all symbolic scalars for doubles (as provided by 'E') and evaluates
        /// symbolic ScalarOps
        /// </summary>
        /// <param name="E">SymbolicEvaluator used to evaluate the symbolic scalars</param>
        /// <returns></returns>
        public Multivector SymbolicEval(RefGA.Symbolic.SymbolicEvaluator E)
        {
            ArrayList L = new ArrayList();

            foreach (BasisBlade B in m_basisBlades)
            {
                BasisBlade C = B.SymbolicEval(E);
                L.Add(C);
            }
            return new Multivector(BasisBlade.Simplify(L));
        }

        /// <summary>
        /// 'Compresses' a multivector by discarding all coordinates with a |size| smaller-equal than <paramref name="epsilon"/>
        /// </summary>
        /// <param name="epsilon">Threshold for discarding a coordinate</param>
        /// <returns>'Compressed' multivector</returns>
        public Multivector Compress(double epsilon)
        {
            ArrayList L = new ArrayList();
            foreach (BasisBlade a in m_basisBlades) {
                if (Math.Abs(a.scale) > epsilon)
                    L.Add(a);
            }
            return new Multivector(L);
        }


        /// <summary>
        /// 'Compresses' a multivector by discarding all coordinates with a |size| smaller-equal than 1e-13
        /// </summary>
        /// <returns>Compress(1e-13)</returns>
        public Multivector Compress()
        {
            return Compress(1e-13);
        }

        /// <summary>
        /// Rounds the coordinate(s) of this basis blades to multiple of integers.
        /// 
        /// This is used by Gaigen 2.5 for non-diagonal metrics, which may introduce very slight
        /// floating point round off errors which you do not want in the final generated code.
        /// 
        /// If the scale, or the floating point part of a scale, is closer than eps
        /// to an integer, rounds it.
        /// </summary>
        /// <param name="eps">Epsilon threshold for rounding.</param>
        /// <returns>Rounded multivector.</returns>
        public Multivector Round(double eps)
        {
            bool changeMade = false;
            ArrayList L = new ArrayList();
            for (int i = 0; i < BasisBlades.Length; i++)
            {
                BasisBlade R = BasisBlades[i].Round(eps);
                if (R != BasisBlades[i]) changeMade = true;
                if (R.scale != 0.0) L.Add(R);
            }
            if (changeMade) return new Multivector(L);
            else return this;
        }


        /// <summary>
        /// Converts a Multivector to a string. Basis vectors are named e1, e2, e3, etc
        /// </summary>
        /// <returns>a string which represents this Multivector
        public override string ToString()
        {
            String[] bvNames = null;
            return ToString(bvNames);
        }

        /// <summary>
        /// Converts a Multivector to a string given names of the basis vectors.
        /// </summary>
        /// <param name="bvNames">Optional names of basis vectors (may be null, then e1, e2, e3, etc is used)</param>
        /// <returns>a string which represents this Multivector
        public string ToString(String[] bvNames)
        {
            return BasisBlade.ToString(new ArrayList(m_basisBlades), bvNames);
        }

        /// <summary>
        /// <summary>
        /// Converts a Multivector to a string given names of the basis vectors.
        /// </summary>
        /// <param name="bvNames">Optional names of basis vectors (may be null, then e1, e2, e3, etc is used)</param>
        /// <returns>a string which represents this Multivector
        public string ToString(System.Collections.Generic.List<String> bvNames)
        {
            return BasisBlade.ToString(new ArrayList(m_basisBlades), bvNames.ToArray());
        }

        /// IComparable.CompareTo implementation.
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj is Multivector)
            {
                Multivector M = (Multivector)obj;
                int l1 = BasisBlades.Length;
                int l2 = M.BasisBlades.Length;
                for (int i = 0; ((i < l1) && (i < l2)); i++)
                {
                    int c = BasisBlades[i].SymbolicCompareTo(M.BasisBlades[i]);
                    if (c != 0) return c; // if not equal, we're done
                }
                // end of (one of the) arrays reached: compare length of arrays to make comparison
                // end of (one of the) arrays reached: compare length of arrays to make comparison
                return ((l1 < l2) ? -1 : ((l1 > l2) ? 1 : 0));
            }
            else throw new ArgumentException("object is not a Multivector");
        }

        /// <summary>
        /// Must be called by constructor to initialize m_hashCode (after all member variables have been set)
        /// </summary>
        protected int ComputeHashCode()
        {
            int H = 0;
            foreach (BasisBlade B in m_basisBlades)
                if (B != null) H ^= B.GetHashCode();
            return H;
        }

        public override int GetHashCode()
        {
            return m_hashCode;
        }

        /// <summary>
        /// Array of BasisBlades. The Multivector is the sum of these BasisBlades
        /// </summary>
        public BasisBlade[] BasisBlades
        {
            get
            {
                return m_basisBlades;
            }
        }



        /// <summary>
        ///  The BasisBlades that make up this multivector.
        ///  Always sorted using BasisBlade.CompareTo().
        /// </summary>
        private readonly BasisBlade[] m_basisBlades;

        /// <summary>
        ///  Computed at construction-time
        /// </summary>
        private readonly int m_hashCode;
    
    } // end of class Multivector
} // end of namespace RefGA
