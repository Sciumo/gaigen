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
 * This class can hold a specialized multivector of type Plane.
 * 
 * The coordinates are stored in type float.
 * 
 * The variable non-zero coordinates are:
 *   - coordinate e1^e2^e3^ni  (array index: E1_E2_E3_NI = 0)
 *   - coordinate no^e2^e3^ni  (array index: NO_E2_E3_NI = 1)
 *   - coordinate no^e1^e3^ni  (array index: NO_E1_E3_NI = 2)
 *   - coordinate no^e1^e2^ni  (array index: NO_E1_E2_NI = 3)
 * 
 * The type has no constant coordinates.
 * 
 * 
 */
public class Plane  implements  Mv_if
{ 
	/**
	 *  The coordinates (stored in an array).
	 */
	protected float[] m_c = new float[4]; // e1^e2^e3^ni, no^e2^e3^ni, no^e1^e3^ni, no^e1^e2^ni
	/**
	 * Array indices of Plane coordinates.
	 */

	/**
	 * index of coordinate for e1^e2^e3^ni in Plane
	 */
	public static final int E1_E2_E3_NI = 0;

	/**
	 * index of coordinate for no^e2^e3^ni in Plane
	 */
	public static final int NO_E2_E3_NI = 1;

	/**
	 * index of coordinate for no^e1^e3^ni in Plane
	 */
	public static final int NO_E1_E3_NI = 2;

	/**
	 * index of coordinate for no^e1^e2^ni in Plane
	 */
	public static final int NO_E1_E2_NI = 3;

	/**
	 * The order of coordinates (this is the type of the first argument of coordinate-handling functions.
	 */
	private enum CoordinateOrder {
		coord_e1e2e3ni_noe2e3ni_noe1e3ni_noe1e2ni
	};
	public static final CoordinateOrder coord_e1e2e3ni_noe2e3ni_noe1e3ni_noe1e2ni = CoordinateOrder.coord_e1e2e3ni_noe2e3ni_noe1e3ni_noe1e2ni;

	public final Mv to_Mv() {
		return new Mv(this);
	}

    /** Constructs a new Plane with variable coordinates set to 0. */
	public Plane() {set();}

    /** Copy constructor. */
	public Plane(final Plane A) {set(A);}



	/** Constructs a new Plane from Mv.
	 *  @param A The value to copy. Coordinates that cannot be represented
	 *  are silently dropped.
	 */
	public Plane(final Mv A /*, final int filler */) {set(A);}

	/** Constructs a new Plane. Coordinate values come from 'A'. */
	public Plane(final CoordinateOrder co, final float[] A) {set(co, A);}
	
	/** Constructs a new Plane with each coordinate specified. */
	public Plane(final CoordinateOrder co,  final float e1_e2_e3_ni, final float no_e2_e3_ni, final float no_e1_e3_ni, final float no_e1_e2_ni) {
		set(co, e1_e2_e3_ni, no_e2_e3_ni, no_e1_e3_ni, no_e1_e2_ni);
	}

public final void set()
{
	m_c[0] = m_c[1] = m_c[2] = m_c[3] = 0.0f;

}

public final void set(final float scalarVal)
{
	m_c[0] = m_c[1] = m_c[2] = m_c[3] = 0.0f;

}

public final void set(final CoordinateOrder co, final float _e1_e2_e3_ni, final float _no_e2_e3_ni, final float _no_e1_e3_ni, final float _no_e1_e2_ni)
{
	m_c[0] = _e1_e2_e3_ni;
	m_c[1] = _no_e2_e3_ni;
	m_c[2] = _no_e1_e3_ni;
	m_c[3] = _no_e1_e2_ni;

}

public final void set(final CoordinateOrder co, final float[] A)
{
	m_c[0] = A[0];
	m_c[1] = A[1];
	m_c[2] = A[2];
	m_c[3] = A[3];

}

public final void set(final Plane a)
{
	m_c[0] = a.m_c[0];
	m_c[1] = a.m_c[1];
	m_c[2] = a.m_c[2];
	m_c[3] = a.m_c[3];

}
	public final void set(final Mv src) {
		if (src.c()[4] != null) {
			float[] ptr = src.c()[4];
			m_c[0] = ptr[4];
			m_c[1] = ptr[3];
			m_c[2] = ptr[2];
			m_c[3] = ptr[1];
		}
		else {
			m_c[0] = 0.0f;
			m_c[1] = 0.0f;
			m_c[2] = 0.0f;
			m_c[3] = 0.0f;
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
		if (Math.abs(m_c[1]) > maxValue) { maxValue = Math.abs(m_c[1]); bm = 29; }
		if (Math.abs(m_c[2]) > maxValue) { maxValue = Math.abs(m_c[2]); bm = 27; }
		if (Math.abs(m_c[3]) > maxValue) { maxValue = Math.abs(m_c[3]); bm = 23; }
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
	 * Returns the e1^e2^e3^ni coordinate.
	 */
	public final float get_e1_e2_e3_ni() { return m_c[0];}
	/**
	 * Sets the e1^e2^e3^ni coordinate.
	 */
	public final void set_e1_e2_e3_ni(float e1_e2_e3_ni) { m_c[0] = e1_e2_e3_ni;}
	/**
	 * Returns the no^e2^e3^ni coordinate.
	 */
	public final float get_no_e2_e3_ni() { return m_c[1];}
	/**
	 * Sets the no^e2^e3^ni coordinate.
	 */
	public final void set_no_e2_e3_ni(float no_e2_e3_ni) { m_c[1] = no_e2_e3_ni;}
	/**
	 * Returns the no^e1^e3^ni coordinate.
	 */
	public final float get_no_e1_e3_ni() { return m_c[2];}
	/**
	 * Sets the no^e1^e3^ni coordinate.
	 */
	public final void set_no_e1_e3_ni(float no_e1_e3_ni) { m_c[2] = no_e1_e3_ni;}
	/**
	 * Returns the no^e1^e2^ni coordinate.
	 */
	public final float get_no_e1_e2_ni() { return m_c[3];}
	/**
	 * Sets the no^e1^e2^ni coordinate.
	 */
	public final void set_no_e1_e2_ni(float no_e1_e2_ni) { m_c[3] = no_e1_e2_ni;}
	/**
	 * Returns the scalar coordinate (which is always 0).
	 */
	public final float get_scalar() { return 0.0f;}
	/**
	 * Returns array of coordinates.
	 * @param coordOrder pass the value 'Plane.coord_e1e2e3ni_noe2e3ni_noe1e3ni_noe1e2ni'
	 */
	public final float[] c(CoordinateOrder coordOrder) { return m_c;}
} // end of class Plane
