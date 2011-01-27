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
 * This class can hold a specialized multivector of type DualLine.
 * 
 * The coordinates are stored in type float.
 * 
 * The variable non-zero coordinates are:
 *   - coordinate e1^e2  (array index: E1_E2 = 0)
 *   - coordinate e1^e3  (array index: E1_E3 = 1)
 *   - coordinate e2^e3  (array index: E2_E3 = 2)
 *   - coordinate e1^ni  (array index: E1_NI = 3)
 *   - coordinate e2^ni  (array index: E2_NI = 4)
 *   - coordinate e3^ni  (array index: E3_NI = 5)
 * 
 * The type has no constant coordinates.
 * 
 * 
 */
public class DualLine  implements  Mv_if
{ 
	/**
	 *  The coordinates (stored in an array).
	 */
	protected float[] m_c = new float[6]; // e1^e2, e1^e3, e2^e3, e1^ni, e2^ni, e3^ni
	/**
	 * Array indices of DualLine coordinates.
	 */

	/**
	 * index of coordinate for e1^e2 in DualLine
	 */
	public static final int E1_E2 = 0;

	/**
	 * index of coordinate for e1^e3 in DualLine
	 */
	public static final int E1_E3 = 1;

	/**
	 * index of coordinate for e2^e3 in DualLine
	 */
	public static final int E2_E3 = 2;

	/**
	 * index of coordinate for e1^ni in DualLine
	 */
	public static final int E1_NI = 3;

	/**
	 * index of coordinate for e2^ni in DualLine
	 */
	public static final int E2_NI = 4;

	/**
	 * index of coordinate for e3^ni in DualLine
	 */
	public static final int E3_NI = 5;

	/**
	 * The order of coordinates (this is the type of the first argument of coordinate-handling functions.
	 */
	private enum CoordinateOrder {
		coord_e1e2_e1e3_e2e3_e1ni_e2ni_e3ni
	};
	public static final CoordinateOrder coord_e1e2_e1e3_e2e3_e1ni_e2ni_e3ni = CoordinateOrder.coord_e1e2_e1e3_e2e3_e1ni_e2ni_e3ni;

	public final Mv to_Mv() {
		return new Mv(this);
	}

    /** Constructs a new DualLine with variable coordinates set to 0. */
	public DualLine() {set();}

    /** Copy constructor. */
	public DualLine(final DualLine A) {set(A);}



	/** Constructs a new DualLine from Mv.
	 *  @param A The value to copy. Coordinates that cannot be represented
	 *  are silently dropped.
	 */
	public DualLine(final Mv A /*, final int filler */) {set(A);}

	/** Constructs a new DualLine. Coordinate values come from 'A'. */
	public DualLine(final CoordinateOrder co, final float[] A) {set(co, A);}
	
	/** Constructs a new DualLine with each coordinate specified. */
	public DualLine(final CoordinateOrder co,  final float e1_e2, final float e1_e3, final float e2_e3, final float e1_ni, final float e2_ni, final float e3_ni) {
		set(co, e1_e2, e1_e3, e2_e3, e1_ni, e2_ni, e3_ni);
	}

public final void set()
{
	m_c[0] = m_c[1] = m_c[2] = m_c[3] = m_c[4] = m_c[5] = 0.0f;

}

public final void set(final float scalarVal)
{
	m_c[0] = m_c[1] = m_c[2] = m_c[3] = m_c[4] = m_c[5] = 0.0f;

}

public final void set(final CoordinateOrder co, final float _e1_e2, final float _e1_e3, final float _e2_e3, final float _e1_ni, final float _e2_ni, final float _e3_ni)
{
	m_c[0] = _e1_e2;
	m_c[1] = _e1_e3;
	m_c[2] = _e2_e3;
	m_c[3] = _e1_ni;
	m_c[4] = _e2_ni;
	m_c[5] = _e3_ni;

}

public final void set(final CoordinateOrder co, final float[] A)
{
	m_c[0] = A[0];
	m_c[1] = A[1];
	m_c[2] = A[2];
	m_c[3] = A[3];
	m_c[4] = A[4];
	m_c[5] = A[5];

}

public final void set(final DualLine a)
{
	m_c[0] = a.m_c[0];
	m_c[1] = a.m_c[1];
	m_c[2] = a.m_c[2];
	m_c[3] = a.m_c[3];
	m_c[4] = a.m_c[4];
	m_c[5] = a.m_c[5];

}
	public final void set(final Mv src) {
		if (src.c()[2] != null) {
			float[] ptr = src.c()[2];
			m_c[0] = ptr[2];
			m_c[1] = ptr[4];
			m_c[2] = ptr[5];
			m_c[3] = ptr[7];
			m_c[4] = ptr[8];
			m_c[5] = ptr[9];
		}
		else {
			m_c[0] = 0.0f;
			m_c[1] = 0.0f;
			m_c[2] = 0.0f;
			m_c[3] = 0.0f;
			m_c[4] = 0.0f;
			m_c[5] = 0.0f;
		}
	}

	/**
	 * Returns the absolute largest coordinate.
	 */
	public final float largestCoordinate() {
		float maxValue = Math.abs(m_c[0]);
		if (Math.abs(m_c[1]) > maxValue) { maxValue = Math.abs(m_c[1]); }
		if (Math.abs(m_c[2]) > maxValue) { maxValue = Math.abs(m_c[2]); }
		if (Math.abs(m_c[3]) > maxValue) { maxValue = Math.abs(m_c[3]); }
		if (Math.abs(m_c[4]) > maxValue) { maxValue = Math.abs(m_c[4]); }
		if (Math.abs(m_c[5]) > maxValue) { maxValue = Math.abs(m_c[5]); }
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
		if (Math.abs(m_c[1]) > maxValue) { maxValue = Math.abs(m_c[1]); bm = 10; }
		if (Math.abs(m_c[2]) > maxValue) { maxValue = Math.abs(m_c[2]); bm = 12; }
		if (Math.abs(m_c[3]) > maxValue) { maxValue = Math.abs(m_c[3]); bm = 18; }
		if (Math.abs(m_c[4]) > maxValue) { maxValue = Math.abs(m_c[4]); bm = 20; }
		if (Math.abs(m_c[5]) > maxValue) { maxValue = Math.abs(m_c[5]); bm = 24; }
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
	 * Returns the e1^e2 coordinate.
	 */
	public final float get_e1_e2() { return m_c[0];}
	/**
	 * Sets the e1^e2 coordinate.
	 */
	public final void set_e1_e2(float e1_e2) { m_c[0] = e1_e2;}
	/**
	 * Returns the e1^e3 coordinate.
	 */
	public final float get_e1_e3() { return m_c[1];}
	/**
	 * Sets the e1^e3 coordinate.
	 */
	public final void set_e1_e3(float e1_e3) { m_c[1] = e1_e3;}
	/**
	 * Returns the e2^e3 coordinate.
	 */
	public final float get_e2_e3() { return m_c[2];}
	/**
	 * Sets the e2^e3 coordinate.
	 */
	public final void set_e2_e3(float e2_e3) { m_c[2] = e2_e3;}
	/**
	 * Returns the e1^ni coordinate.
	 */
	public final float get_e1_ni() { return m_c[3];}
	/**
	 * Sets the e1^ni coordinate.
	 */
	public final void set_e1_ni(float e1_ni) { m_c[3] = e1_ni;}
	/**
	 * Returns the e2^ni coordinate.
	 */
	public final float get_e2_ni() { return m_c[4];}
	/**
	 * Sets the e2^ni coordinate.
	 */
	public final void set_e2_ni(float e2_ni) { m_c[4] = e2_ni;}
	/**
	 * Returns the e3^ni coordinate.
	 */
	public final float get_e3_ni() { return m_c[5];}
	/**
	 * Sets the e3^ni coordinate.
	 */
	public final void set_e3_ni(float e3_ni) { m_c[5] = e3_ni;}
	/**
	 * Returns the scalar coordinate (which is always 0).
	 */
	public final float get_scalar() { return 0.0f;}
	/**
	 * Returns array of coordinates.
	 * @param coordOrder pass the value 'DualLine.coord_e1e2_e1e3_e2e3_e1ni_e2ni_e3ni'
	 */
	public final float[] c(CoordinateOrder coordOrder) { return m_c;}

	/**
	 * shortcut to C3ga.lc(this, b)
	 */
	public final FlatPoint lc(final Plane b) {
		return C3ga.lc(this, b);
	}
} // end of class DualLine
