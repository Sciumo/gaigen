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
/// <summary>This class can hold a specialized multivector of type plane.
/// 
/// The coordinates are stored in type float.
/// 
/// The variable non-zero coordinates are:
///   - coordinate e1^e2^e3^ni  (array index: E1_E2_E3_NI = 0)
///   - coordinate no^e2^e3^ni  (array index: NO_E2_E3_NI = 1)
///   - coordinate no^e1^e3^ni  (array index: NO_E1_E3_NI = 2)
///   - coordinate no^e1^e2^ni  (array index: NO_E1_E2_NI = 3)
/// 
/// The type has no constant coordinates.
/// 
/// 
/// </summary>
public class plane  :  mv_if
{ 
	/// <summary> The coordinates (stored in an array).
	/// </summary>
	protected internal float[] m_c = new float[4]; // e1^e2^e3^ni, no^e2^e3^ni, no^e1^e3^ni, no^e1^e2^ni
	/// <summary>Array indices of plane coordinates.
	/// </summary>

	/// <summary>index of coordinate for e1^e2^e3^ni in plane
	/// </summary>
	public const int E1_E2_E3_NI = 0;

	/// <summary>index of coordinate for no^e2^e3^ni in plane
	/// </summary>
	public const int NO_E2_E3_NI = 1;

	/// <summary>index of coordinate for no^e1^e3^ni in plane
	/// </summary>
	public const int NO_E1_E3_NI = 2;

	/// <summary>index of coordinate for no^e1^e2^ni in plane
	/// </summary>
	public const int NO_E1_E2_NI = 3;

	/// <summary>The order of coordinates (this is the type of the first argument of coordinate-handling functions.
	/// </summary>
	public enum CoordinateOrder {
		coord_e1e2e3ni_noe2e3ni_noe1e3ni_noe1e2ni
	};
	public const CoordinateOrder coord_e1e2e3ni_noe2e3ni_noe1e3ni_noe1e2ni = CoordinateOrder.coord_e1e2e3ni_noe2e3ni_noe1e3ni_noe1e2ni;

    /// <summary>	
    /// Converts this multivector to a 'mv' (implementation of interface 'mv_interface')
    /// </summary>
    public mv to_mv()
    {
        return new mv(this);
    }

    /// <summary>
	/// Constructs a new plane with variable coordinates set to 0.
    /// </summary>
	public plane() {Set();}

    /// <summary>
	/// Copy constructor.
    /// </summary>
	public plane(plane A) {Set(A);}



    /// <summary>
	/// Constructs a new plane from mv.
    /// </summary>
	/// <param name="A">The value to copy. Coordinates that cannot be represented are silently dropped. </param>
	public plane(mv A /*, int filler */) {Set(A);}

    /// <summary>
	/// Constructs a new plane. Coordinate values come from 'A'.
    /// </summary>
	public plane(CoordinateOrder co, float[] A) {Set(co, A);}
	
    /// <summary>
	/// Constructs a new plane with each coordinate specified.
    /// </summary>
	public plane(CoordinateOrder co,  float e1_e2_e3_ni, float no_e2_e3_ni, float no_e1_e3_ni, float no_e1_e2_ni) {
		Set(co, e1_e2_e3_ni, no_e2_e3_ni, no_e1_e3_ni, no_e1_e2_ni);
	}

    /// <summary>
	/// Implicit converter from plane to mv.
    /// </summary>
    public static implicit operator mv(plane a)
    {
        return new mv(a);
    }

public void Set()
{
	m_c[0] = m_c[1] = m_c[2] = m_c[3] = 0.0f;

}

public void Set(float scalarVal)
{
	m_c[0] = m_c[1] = m_c[2] = m_c[3] = 0.0f;

}

public void Set(CoordinateOrder co, float _e1_e2_e3_ni, float _no_e2_e3_ni, float _no_e1_e3_ni, float _no_e1_e2_ni)
{
	m_c[0] = _e1_e2_e3_ni;
	m_c[1] = _no_e2_e3_ni;
	m_c[2] = _no_e1_e3_ni;
	m_c[3] = _no_e1_e2_ni;

}

public void Set(CoordinateOrder co, float[] A)
{
	m_c[0] = A[0];
	m_c[1] = A[1];
	m_c[2] = A[2];
	m_c[3] = A[3];

}

public void Set(plane a)
{
	m_c[0] = a.m_c[0];
	m_c[1] = a.m_c[1];
	m_c[2] = a.m_c[2];
	m_c[3] = a.m_c[3];

}
	public void Set(mv src) {
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

	/// <summary>Returns the absolute largest coordinate.
	/// </summary>
	public float LargestCoordinate() {
		float maxValue = Math.Abs(m_c[0]);
		if (Math.Abs(m_c[1]) > maxValue) { maxValue = Math.Abs(m_c[1]); }
		if (Math.Abs(m_c[2]) > maxValue) { maxValue = Math.Abs(m_c[2]); }
		if (Math.Abs(m_c[3]) > maxValue) { maxValue = Math.Abs(m_c[3]); }
		return maxValue;
	}
	/// <summary>Returns the absolute largest coordinate,
	/// and the corresponding basis blade bitmap.
	/// </summary>
	public float LargestBasisBlade(int bm)  {
		float maxValue = Math.Abs(m_c[0]);
		bm = 0;
		if (Math.Abs(m_c[1]) > maxValue) { maxValue = Math.Abs(m_c[1]); bm = 29; }
		if (Math.Abs(m_c[2]) > maxValue) { maxValue = Math.Abs(m_c[2]); bm = 27; }
		if (Math.Abs(m_c[3]) > maxValue) { maxValue = Math.Abs(m_c[3]); bm = 23; }
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
	/// <summary>Returns the e1^e2^e3^ni coordinate.
	/// </summary>
	public float get_e1_e2_e3_ni() { return m_c[0];}
	/// <summary>Sets the e1^e2^e3^ni coordinate.
	/// </summary>
	public void set_e1_e2_e3_ni(float e1_e2_e3_ni) { m_c[0] = e1_e2_e3_ni;}
	/// <summary>Returns the no^e2^e3^ni coordinate.
	/// </summary>
	public float get_no_e2_e3_ni() { return m_c[1];}
	/// <summary>Sets the no^e2^e3^ni coordinate.
	/// </summary>
	public void set_no_e2_e3_ni(float no_e2_e3_ni) { m_c[1] = no_e2_e3_ni;}
	/// <summary>Returns the no^e1^e3^ni coordinate.
	/// </summary>
	public float get_no_e1_e3_ni() { return m_c[2];}
	/// <summary>Sets the no^e1^e3^ni coordinate.
	/// </summary>
	public void set_no_e1_e3_ni(float no_e1_e3_ni) { m_c[2] = no_e1_e3_ni;}
	/// <summary>Returns the no^e1^e2^ni coordinate.
	/// </summary>
	public float get_no_e1_e2_ni() { return m_c[3];}
	/// <summary>Sets the no^e1^e2^ni coordinate.
	/// </summary>
	public void set_no_e1_e2_ni(float no_e1_e2_ni) { m_c[3] = no_e1_e2_ni;}
	/// <summary>Returns the scalar coordinate (which is always 0).
	/// </summary>
	public float get_scalar() { return 0.0f;}
	/// <summary>Returns array of coordinates.
	/// </summary>
	/// <param name="coordOrder">pass the value 'plane.coord_e1e2e3ni_noe2e3ni_noe1e3ni_noe1e2ni'
	/// </param>
	public float[] c(CoordinateOrder coordOrder) { return m_c;}
} // end of class plane
} // end of namespace c3ga_ns
