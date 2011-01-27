/*
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

*/
package C3ga_pkg;
/**
 * This class can hold a specialized multivector of type VectorE3GA.
 * 
 * The coordinates are stored in type float.
 * 
 * The variable non-zero coordinates are:
 *   - coordinate e1  (array index: E1 = 0)
 *   - coordinate e2  (array index: E2 = 1)
 *   - coordinate e3  (array index: E3 = 2)
 * 
 * The type has no constant coordinates.
 * 
 * 
 */
public class VectorE3GA  implements  Mv_if
{ 
	/**
	 *  The coordinates (stored in an array).
	 */
	protected float[] m_c = new float[3]; // e1, e2, e3
	/**
	 * Array indices of VectorE3GA coordinates.
	 */

	/**
	 * index of coordinate for e1 in VectorE3GA
	 */
	public static final int E1 = 0;

	/**
	 * index of coordinate for e2 in VectorE3GA
	 */
	public static final int E2 = 1;

	/**
	 * index of coordinate for e3 in VectorE3GA
	 */
	public static final int E3 = 2;

	/**
	 * The order of coordinates (this is the type of the first argument of coordinate-handling functions.
	 */
	private enum CoordinateOrder {
		coord_e1_e2_e3
	};
	public static final CoordinateOrder coord_e1_e2_e3 = CoordinateOrder.coord_e1_e2_e3;

	public final Mv to_Mv() {
		return new Mv(this);
	}

    /** Constructs a new VectorE3GA with variable coordinates set to 0. */
	public VectorE3GA() {set();}

    /** Copy constructor. */
	public VectorE3GA(final VectorE3GA A) {set(A);}



	/** Constructs a new VectorE3GA from Mv.
	 *  @param A The value to copy. Coordinates that cannot be represented
	 *  are silently dropped.
	 */
	public VectorE3GA(final Mv A /*, final int filler */) {set(A);}

	/** Constructs a new VectorE3GA. Coordinate values come from 'A'. */
	public VectorE3GA(final CoordinateOrder co, final float[] A) {set(co, A);}
	
	/** Constructs a new VectorE3GA with each coordinate specified. */
	public VectorE3GA(final CoordinateOrder co,  final float e1, final float e2, final float e3) {
		set(co, e1, e2, e3);
	}

public final void set()
{
	m_c[0] = m_c[1] = m_c[2] = 0.0f;

}

public final void set(final float scalarVal)
{
	m_c[0] = m_c[1] = m_c[2] = 0.0f;

}

public final void set(final CoordinateOrder co, final float _e1, final float _e2, final float _e3)
{
	m_c[0] = _e1;
	m_c[1] = _e2;
	m_c[2] = _e3;

}

public final void set(final CoordinateOrder co, final float[] A)
{
	m_c[0] = A[0];
	m_c[1] = A[1];
	m_c[2] = A[2];

}

public final void set(final VectorE3GA a)
{
	m_c[0] = a.m_c[0];
	m_c[1] = a.m_c[1];
	m_c[2] = a.m_c[2];

}
	public final void set(final Mv src) {
		if (src.c()[1] != null) {
			float[] ptr = src.c()[1];
			m_c[0] = ptr[1];
			m_c[1] = ptr[2];
			m_c[2] = ptr[3];
		}
		else {
			m_c[0] = 0.0f;
			m_c[1] = 0.0f;
			m_c[2] = 0.0f;
		}
	}

	/**
	 * Returns the absolute largest coordinate.
	 */
	public final float largestCoordinate() {
		float maxValue = Math.abs(m_c[0]);
		if (Math.abs(m_c[1]) > maxValue) { maxValue = Math.abs(m_c[1]); }
		if (Math.abs(m_c[2]) > maxValue) { maxValue = Math.abs(m_c[2]); }
		return maxValue;
	}
	/**
	 * Returns the absolute largest coordinate,
	 * and the corresponding basis blade bitmap.
	 */
	public final float[] largestBasisBlade()  {
		int bm;
		float maxValue = Math.abs(m_c[0]);
		bm = 0;
		if (Math.abs(m_c[1]) > maxValue) { maxValue = Math.abs(m_c[1]); bm = 4; }
		if (Math.abs(m_c[2]) > maxValue) { maxValue = Math.abs(m_c[2]); bm = 8; }
		return new float[]{maxValue, (float)bm};
	}

	/**
	 * Returns this multivector, converted to a string.
	 * The floating point formatter is controlled via C3ga.setStringFormat().
	 */
	public final String toString() {
		return C3ga.string(this);
	}
	
	/**
	 * Returns this multivector, converted to a string.
	 * The floating point formatter is "%f".
	 */
	public final String toString_f() {
		return toString("%f");
	}
	
	/**
	 * Returns this multivector, converted to a string.
	 * The floating point formatter is "%e".
	 */
	public final String toString_e() {
		return toString("%e");
	}
	
	/**
	 * Returns this multivector, converted to a string.
	 * The floating point formatter is "%2.20e".
	 */
	public final String toString_e20() {
		return toString("%2.20e");
	}
	
	/**
	 * Returns this multivector, converted to a string.
	 * @param fp floating point format. Use 'null' for the default format (see C3ga.setStringFormat()).
	 */
	public final String toString(final String fp) {
		return C3ga.string(this, fp);
	}
	/**
	 * Returns the e1 coordinate.
	 */
	public final float get_e1() { return m_c[0];}
	/**
	 * Sets the e1 coordinate.
	 */
	public final void set_e1(float e1) { m_c[0] = e1;}
	/**
	 * Returns the e2 coordinate.
	 */
	public final float get_e2() { return m_c[1];}
	/**
	 * Sets the e2 coordinate.
	 */
	public final void set_e2(float e2) { m_c[1] = e2;}
	/**
	 * Returns the e3 coordinate.
	 */
	public final float get_e3() { return m_c[2];}
	/**
	 * Sets the e3 coordinate.
	 */
	public final void set_e3(float e3) { m_c[2] = e3;}
	/**
	 * Returns the scalar coordinate (which is always 0).
	 */
	public final float get_scalar() { return 0.0f;}
	/**
	 * Returns array of coordinates.
	 * @param coordOrder pass the value 'VectorE3GA.coord_e1_e2_e3'
	 */
	public final float[] c(CoordinateOrder coordOrder) { return m_c;}
} // end of class VectorE3GA
