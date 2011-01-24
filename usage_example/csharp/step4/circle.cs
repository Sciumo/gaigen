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
using System;
namespace c3ga_ns {
/// <summary>This class can hold a specialized multivector of type circle.
/// 
/// The coordinates are stored in type float.
/// 
/// The variable non-zero coordinates are:
///   - coordinate no^e1^e2  (array index: NO_E1_E2 = 0)
///   - coordinate no^e1^e3  (array index: NO_E1_E3 = 1)
///   - coordinate no^e2^e3  (array index: NO_E2_E3 = 2)
///   - coordinate e1^e2^e3  (array index: E1_E2_E3 = 3)
///   - coordinate no^e1^ni  (array index: NO_E1_NI = 4)
///   - coordinate no^e2^ni  (array index: NO_E2_NI = 5)
///   - coordinate e1^e2^ni  (array index: E1_E2_NI = 6)
///   - coordinate no^e3^ni  (array index: NO_E3_NI = 7)
///   - coordinate e1^e3^ni  (array index: E1_E3_NI = 8)
///   - coordinate e2^e3^ni  (array index: E2_E3_NI = 9)
/// 
/// The type has no constant coordinates.
/// 
/// 
/// </summary>
public class circle  :  mv_if
{ 
	/// <summary> The coordinates (stored in an array).
	/// </summary>
	protected internal float[] m_c = new float[10]; // no^e1^e2, no^e1^e3, no^e2^e3, e1^e2^e3, no^e1^ni, no^e2^ni, e1^e2^ni, no^e3^ni, e1^e3^ni, e2^e3^ni
	/// <summary>Array indices of circle coordinates.
	/// </summary>

	/// <summary>index of coordinate for no^e1^e2 in circle
	/// </summary>
	public const int NO_E1_E2 = 0;

	/// <summary>index of coordinate for no^e1^e3 in circle
	/// </summary>
	public const int NO_E1_E3 = 1;

	/// <summary>index of coordinate for no^e2^e3 in circle
	/// </summary>
	public const int NO_E2_E3 = 2;

	/// <summary>index of coordinate for e1^e2^e3 in circle
	/// </summary>
	public const int E1_E2_E3 = 3;

	/// <summary>index of coordinate for no^e1^ni in circle
	/// </summary>
	public const int NO_E1_NI = 4;

	/// <summary>index of coordinate for no^e2^ni in circle
	/// </summary>
	public const int NO_E2_NI = 5;

	/// <summary>index of coordinate for e1^e2^ni in circle
	/// </summary>
	public const int E1_E2_NI = 6;

	/// <summary>index of coordinate for no^e3^ni in circle
	/// </summary>
	public const int NO_E3_NI = 7;

	/// <summary>index of coordinate for e1^e3^ni in circle
	/// </summary>
	public const int E1_E3_NI = 8;

	/// <summary>index of coordinate for e2^e3^ni in circle
	/// </summary>
	public const int E2_E3_NI = 9;

	/// <summary>The order of coordinates (this is the type of the first argument of coordinate-handling functions.
	/// </summary>
	public enum CoordinateOrder {
		coord_noe1e2_noe1e3_noe2e3_e1e2e3_noe1ni_noe2ni_e1e2ni_noe3ni_e1e3ni_e2e3ni
	};
	public const CoordinateOrder coord_noe1e2_noe1e3_noe2e3_e1e2e3_noe1ni_noe2ni_e1e2ni_noe3ni_e1e3ni_e2e3ni = CoordinateOrder.coord_noe1e2_noe1e3_noe2e3_e1e2e3_noe1ni_noe2ni_e1e2ni_noe3ni_e1e3ni_e2e3ni;

    /// <summary>	
    /// Converts this multivector to a 'mv' (implementation of interface 'mv_interface')
    /// </summary>
    public mv to_mv()
    {
        return new mv(this);
    }

    /// <summary>
	/// Constructs a new circle with variable coordinates set to 0.
    /// </summary>
	public circle() {Set();}

    /// <summary>
	/// Copy constructor.
    /// </summary>
	public circle(circle A) {Set(A);}



    /// <summary>
	/// Constructs a new circle from mv.
    /// </summary>
	/// <param name="A">The value to copy. Coordinates that cannot be represented are silently dropped. </param>
	public circle(mv A /*, int filler */) {Set(A);}

    /// <summary>
	/// Constructs a new circle. Coordinate values come from 'A'.
    /// </summary>
	public circle(CoordinateOrder co, float[] A) {Set(co, A);}
	
    /// <summary>
	/// Constructs a new circle with each coordinate specified.
    /// </summary>
	public circle(CoordinateOrder co,  float no_e1_e2, float no_e1_e3, float no_e2_e3, float e1_e2_e3, float no_e1_ni, float no_e2_ni, float e1_e2_ni, float no_e3_ni, float e1_e3_ni, float e2_e3_ni) {
		Set(co, no_e1_e2, no_e1_e3, no_e2_e3, e1_e2_e3, no_e1_ni, no_e2_ni, e1_e2_ni, no_e3_ni, e1_e3_ni, e2_e3_ni);
	}

    /// <summary>
	/// Implicit converter from circle to mv.
    /// </summary>
    public static implicit operator mv(circle a)
    {
        return new mv(a);
    }

public void Set()
{
	m_c[0] = m_c[1] = m_c[2] = m_c[3] = m_c[4] = m_c[5] = m_c[6] = m_c[7] = m_c[8] = m_c[9] = 0.0f;

}

public void Set(float scalarVal)
{
	m_c[0] = m_c[1] = m_c[2] = m_c[3] = m_c[4] = m_c[5] = m_c[6] = m_c[7] = m_c[8] = m_c[9] = 0.0f;

}

public void Set(CoordinateOrder co, float _no_e1_e2, float _no_e1_e3, float _no_e2_e3, float _e1_e2_e3, float _no_e1_ni, float _no_e2_ni, float _e1_e2_ni, float _no_e3_ni, float _e1_e3_ni, float _e2_e3_ni)
{
	m_c[0] = _no_e1_e2;
	m_c[1] = _no_e1_e3;
	m_c[2] = _no_e2_e3;
	m_c[3] = _e1_e2_e3;
	m_c[4] = _no_e1_ni;
	m_c[5] = _no_e2_ni;
	m_c[6] = _e1_e2_ni;
	m_c[7] = _no_e3_ni;
	m_c[8] = _e1_e3_ni;
	m_c[9] = _e2_e3_ni;

}

public void Set(CoordinateOrder co, float[] A)
{
	m_c[0] = A[0];
	m_c[1] = A[1];
	m_c[2] = A[2];
	m_c[3] = A[3];
	m_c[4] = A[4];
	m_c[5] = A[5];
	m_c[6] = A[6];
	m_c[7] = A[7];
	m_c[8] = A[8];
	m_c[9] = A[9];

}

public void Set(circle a)
{
	m_c[0] = a.m_c[0];
	m_c[1] = a.m_c[1];
	m_c[2] = a.m_c[2];
	m_c[3] = a.m_c[3];
	m_c[4] = a.m_c[4];
	m_c[5] = a.m_c[5];
	m_c[6] = a.m_c[6];
	m_c[7] = a.m_c[7];
	m_c[8] = a.m_c[8];
	m_c[9] = a.m_c[9];

}
	public void Set(mv src) {
		if (src.c()[3] != null) {
			float[] ptr = src.c()[3];
			m_c[0] = ptr[0];
			m_c[1] = ptr[1];
			m_c[2] = ptr[2];
			m_c[3] = ptr[3];
			m_c[4] = ptr[4];
			m_c[5] = ptr[5];
			m_c[6] = ptr[6];
			m_c[7] = ptr[7];
			m_c[8] = ptr[8];
			m_c[9] = ptr[9];
		}
		else {
			m_c[0] = 0.0f;
			m_c[1] = 0.0f;
			m_c[2] = 0.0f;
			m_c[3] = 0.0f;
			m_c[4] = 0.0f;
			m_c[5] = 0.0f;
			m_c[6] = 0.0f;
			m_c[7] = 0.0f;
			m_c[8] = 0.0f;
			m_c[9] = 0.0f;
		}
	}

	/// <summary>Returns the absolute largest coordinate.
	/// </summary>
	public float LargestCoordinate() {
		float maxValue = Math.Abs(m_c[0]);
		if (Math.Abs(m_c[1]) > maxValue) { maxValue = Math.Abs(m_c[1]); }
		if (Math.Abs(m_c[2]) > maxValue) { maxValue = Math.Abs(m_c[2]); }
		if (Math.Abs(m_c[3]) > maxValue) { maxValue = Math.Abs(m_c[3]); }
		if (Math.Abs(m_c[4]) > maxValue) { maxValue = Math.Abs(m_c[4]); }
		if (Math.Abs(m_c[5]) > maxValue) { maxValue = Math.Abs(m_c[5]); }
		if (Math.Abs(m_c[6]) > maxValue) { maxValue = Math.Abs(m_c[6]); }
		if (Math.Abs(m_c[7]) > maxValue) { maxValue = Math.Abs(m_c[7]); }
		if (Math.Abs(m_c[8]) > maxValue) { maxValue = Math.Abs(m_c[8]); }
		if (Math.Abs(m_c[9]) > maxValue) { maxValue = Math.Abs(m_c[9]); }
		return maxValue;
	}
	/// <summary>Returns the absolute largest coordinate,
	/// and the corresponding basis blade bitmap.
	/// </summary>
	public float LargestBasisBlade(int bm)  {
		float maxValue = Math.Abs(m_c[0]);
		bm = 0;
		if (Math.Abs(m_c[1]) > maxValue) { maxValue = Math.Abs(m_c[1]); bm = 11; }
		if (Math.Abs(m_c[2]) > maxValue) { maxValue = Math.Abs(m_c[2]); bm = 13; }
		if (Math.Abs(m_c[3]) > maxValue) { maxValue = Math.Abs(m_c[3]); bm = 14; }
		if (Math.Abs(m_c[4]) > maxValue) { maxValue = Math.Abs(m_c[4]); bm = 19; }
		if (Math.Abs(m_c[5]) > maxValue) { maxValue = Math.Abs(m_c[5]); bm = 21; }
		if (Math.Abs(m_c[6]) > maxValue) { maxValue = Math.Abs(m_c[6]); bm = 22; }
		if (Math.Abs(m_c[7]) > maxValue) { maxValue = Math.Abs(m_c[7]); bm = 25; }
		if (Math.Abs(m_c[8]) > maxValue) { maxValue = Math.Abs(m_c[8]); bm = 26; }
		if (Math.Abs(m_c[9]) > maxValue) { maxValue = Math.Abs(m_c[9]); bm = 28; }
		return maxValue;
	}

	/// <summary>
	/// Returns this multivector, converted to a string.
	/// The floating point formatter is controlled via c3ga.setStringFormat().
	/// </summary>
	public override string ToString() {
		return c3ga.String(this);
	}
	
	/// <summary>
	/// Returns this multivector, converted to a string.
	/// The floating point formatter is "F".
	/// </summary>
	public string ToString_f() {
		return ToString("F");
	}
	
	/// <summary>
	/// Returns this multivector, converted to a string.
	/// The floating point formatter is "E".
	/// </summary>
	public string ToString_e() {
		return ToString("E");
	}
	
	/// <summary>
	/// Returns this multivector, converted to a string.
	/// The floating point formatter is "E20".
	/// </summary>
	public string ToString_e20() {
		return ToString("E20");
	}
	
	/// <summary>
	/// Returns this multivector, converted to a string.
	/// <param name="fp">floating point format. Use 'null' for the default format (see setStringFormat()).</param>
	/// </summary>
	public string ToString(string fp) {
		return c3ga.String(this, fp);
	}
	/// <summary>Returns the no^e1^e2 coordinate.
	/// </summary>
	public float get_no_e1_e2() { return m_c[0];}
	/// <summary>Sets the no^e1^e2 coordinate.
	/// </summary>
	public void set_no_e1_e2(float no_e1_e2) { m_c[0] = no_e1_e2;}
	/// <summary>Returns the no^e1^e3 coordinate.
	/// </summary>
	public float get_no_e1_e3() { return m_c[1];}
	/// <summary>Sets the no^e1^e3 coordinate.
	/// </summary>
	public void set_no_e1_e3(float no_e1_e3) { m_c[1] = no_e1_e3;}
	/// <summary>Returns the no^e2^e3 coordinate.
	/// </summary>
	public float get_no_e2_e3() { return m_c[2];}
	/// <summary>Sets the no^e2^e3 coordinate.
	/// </summary>
	public void set_no_e2_e3(float no_e2_e3) { m_c[2] = no_e2_e3;}
	/// <summary>Returns the e1^e2^e3 coordinate.
	/// </summary>
	public float get_e1_e2_e3() { return m_c[3];}
	/// <summary>Sets the e1^e2^e3 coordinate.
	/// </summary>
	public void set_e1_e2_e3(float e1_e2_e3) { m_c[3] = e1_e2_e3;}
	/// <summary>Returns the no^e1^ni coordinate.
	/// </summary>
	public float get_no_e1_ni() { return m_c[4];}
	/// <summary>Sets the no^e1^ni coordinate.
	/// </summary>
	public void set_no_e1_ni(float no_e1_ni) { m_c[4] = no_e1_ni;}
	/// <summary>Returns the no^e2^ni coordinate.
	/// </summary>
	public float get_no_e2_ni() { return m_c[5];}
	/// <summary>Sets the no^e2^ni coordinate.
	/// </summary>
	public void set_no_e2_ni(float no_e2_ni) { m_c[5] = no_e2_ni;}
	/// <summary>Returns the e1^e2^ni coordinate.
	/// </summary>
	public float get_e1_e2_ni() { return m_c[6];}
	/// <summary>Sets the e1^e2^ni coordinate.
	/// </summary>
	public void set_e1_e2_ni(float e1_e2_ni) { m_c[6] = e1_e2_ni;}
	/// <summary>Returns the no^e3^ni coordinate.
	/// </summary>
	public float get_no_e3_ni() { return m_c[7];}
	/// <summary>Sets the no^e3^ni coordinate.
	/// </summary>
	public void set_no_e3_ni(float no_e3_ni) { m_c[7] = no_e3_ni;}
	/// <summary>Returns the e1^e3^ni coordinate.
	/// </summary>
	public float get_e1_e3_ni() { return m_c[8];}
	/// <summary>Sets the e1^e3^ni coordinate.
	/// </summary>
	public void set_e1_e3_ni(float e1_e3_ni) { m_c[8] = e1_e3_ni;}
	/// <summary>Returns the e2^e3^ni coordinate.
	/// </summary>
	public float get_e2_e3_ni() { return m_c[9];}
	/// <summary>Sets the e2^e3^ni coordinate.
	/// </summary>
	public void set_e2_e3_ni(float e2_e3_ni) { m_c[9] = e2_e3_ni;}
	/// <summary>Returns the scalar coordinate (which is always 0).
	/// </summary>
	public float get_scalar() { return 0.0f;}
	/// <summary>Returns array of coordinates.
	/// </summary>
	/// <param name="coordOrder">pass the value 'circle.coord_noe1e2_noe1e3_noe2e3_e1e2e3_noe1ni_noe2ni_e1e2ni_noe3ni_e1e3ni_e2e3ni'
	/// </param>
	public float[] c(CoordinateOrder coordOrder) { return m_c;}
} // end of class circle
} // end of namespace c3ga_ns
