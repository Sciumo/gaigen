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
 * This class can hold a specialized multivector of type no_t.
 * 
 * The coordinates are stored in type float.
 * 
 * The type is constant.
 * 
 * The constant non-zero coordinates are:
 *   - no = 1
 * 
 * 
 */
public class no_t  implements  Mv_if
{ 
	/**
	 * Array indices of no_t coordinates.
	 */

	/**
	 * The order of coordinates (this is the type of the first argument of coordinate-handling functions.
	 */
	private enum CoordinateOrder {
		coord
	};
	public static final CoordinateOrder coord = CoordinateOrder.coord;

	public final Mv to_Mv() {
		return new Mv(this);
	}

    /** Constructs a new no_t with variable coordinates set to 0. */
	public no_t() {set();}

    /** Copy constructor. */
	public no_t(final no_t A) {set(A);}



	/** Constructs a new no_t from Mv.
	 *  @param A The value to copy. Coordinates that cannot be represented
	 *  are silently dropped.
	 */
	public no_t(final Mv A /*, final int filler */) {set(A);}


public final void set()
{

}

public final void set(final float scalarVal)
{

}

public final void set(final CoordinateOrder co)
{

}

public final void set(final CoordinateOrder co, final float[] A)
{

}

public final void set(final no_t a)
{

}
	public final void set(final Mv src) {
	}

	/**
	 * Returns the absolute largest coordinate.
	 */
	public final float largestCoordinate() {
		float maxValue = 1.0f;
		return maxValue;
	}
	/**
	 * Returns the absolute largest coordinate,
	 * and the corresponding basis blade bitmap.
	 */
	public final float[] largestBasisBlade()  {
		int bm;
		float maxValue = 1.0f;
		bm = 1;
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
	 * Returns the no coordinate.
	 */
	public final float get_no() { return 1.0f;}
	/**
	 * Returns the scalar coordinate (which is always 0).
	 */
	public final float get_scalar() { return 0.0f;}
} // end of class no_t
