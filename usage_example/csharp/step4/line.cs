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
/// <summary>This class can hold a specialized multivector of type line.
/// 
/// The coordinates are stored in type float.
/// 
/// The variable non-zero coordinates are:
///   - coordinate e1^e2^ni  (array index: E1_E2_NI = 0)
///   - coordinate e1^e3^ni  (array index: E1_E3_NI = 1)
///   - coordinate e2^e3^ni  (array index: E2_E3_NI = 2)
///   - coordinate -1*no^e1^ni  (array index: E1_NO_NI = 3)
///   - coordinate -1*no^e2^ni  (array index: E2_NO_NI = 4)
///   - coordinate -1*no^e3^ni  (array index: E3_NO_NI = 5)
/// 
/// The type has no constant coordinates.
/// 
/// 
/// </summary>
public class line  :  mv_if
{ 
	/// <summary> The coordinates (stored in an array).
	/// </summary>
	protected internal float[] m_c = new float[6]; // e1^e2^ni, e1^e3^ni, e2^e3^ni, -1*no^e1^ni, -1*no^e2^ni, -1*no^e3^ni
	/// <summary>Array indices of line coordinates.
	/// </summary>

	/// <summary>index of coordinate for e1^e2^ni in line
	/// </summary>
	public const int E1_E2_NI = 0;

	/// <summary>index of coordinate for e1^e3^ni in line
	/// </summary>
	public const int E1_E3_NI = 1;

	/// <summary>index of coordinate for e2^e3^ni in line
	/// </summary>
	public const int E2_E3_NI = 2;

	/// <summary>index of coordinate for -1*no^e1^ni in line
	/// </summary>
	public const int E1_NO_NI = 3;

	/// <summary>index of coordinate for -1*no^e2^ni in line
	/// </summary>
	public const int E2_NO_NI = 4;

	/// <summary>index of coordinate for -1*no^e3^ni in line
	/// </summary>
	public const int E3_NO_NI = 5;

	/// <summary>The order of coordinates (this is the type of the first argument of coordinate-handling functions.
	/// </summary>
	public enum CoordinateOrder {
		coord_e1e2ni_e1e3ni_e2e3ni_e1noni_e2noni_e3noni
	};
	public const CoordinateOrder coord_e1e2ni_e1e3ni_e2e3ni_e1noni_e2noni_e3noni = CoordinateOrder.coord_e1e2ni_e1e3ni_e2e3ni_e1noni_e2noni_e3noni;

    /// <summary>	
    /// Converts this multivector to a 'mv' (implementation of interface 'mv_interface')
    /// </summary>
    public mv to_mv()
    {
        return new mv(this);
    }

    /// <summary>
	/// Constructs a new line with variable coordinates set to 0.
    /// </summary>
	public line() {Set();}

    /// <summary>
	/// Copy constructor.
    /// </summary>
	public line(line A) {Set(A);}



    /// <summary>
	/// Constructs a new line from mv.
    /// </summary>
	/// <param name="A">The value to copy. Coordinates that cannot be represented are silently dropped. </param>
	public line(mv A /*, int filler */) {Set(A);}

    /// <summary>
	/// Constructs a new line. Coordinate values come from 'A'.
    /// </summary>
	public line(CoordinateOrder co, float[] A) {Set(co, A);}
	
    /// <summary>
	/// Constructs a new line with each coordinate specified.
    /// </summary>
	public line(CoordinateOrder co,  float e1_e2_ni, float e1_e3_ni, float e2_e3_ni, float e1_no_ni, float e2_no_ni, float e3_no_ni) {
		Set(co, e1_e2_ni, e1_e3_ni, e2_e3_ni, e1_no_ni, e2_no_ni, e3_no_ni);
	}

    /// <summary>
	/// Implicit converter from line to mv.
    /// </summary>
    public static implicit operator mv(line a)
    {
        return new mv(a);
    }

public void Set()
{
	m_c[0] = m_c[1] = m_c[2] = m_c[3] = m_c[4] = m_c[5] = 0.0f;

}

public void Set(float scalarVal)
{
	m_c[0] = m_c[1] = m_c[2] = m_c[3] = m_c[4] = m_c[5] = 0.0f;

}

public void Set(CoordinateOrder co, float _e1_e2_ni, float _e1_e3_ni, float _e2_e3_ni, float _e1_no_ni, float _e2_no_ni, float _e3_no_ni)
{
	m_c[0] = _e1_e2_ni;
	m_c[1] = _e1_e3_ni;
	m_c[2] = _e2_e3_ni;
	m_c[3] = _e1_no_ni;
	m_c[4] = _e2_no_ni;
	m_c[5] = _e3_no_ni;

}

public void Set(CoordinateOrder co, float[] A)
{
	m_c[0] = A[0];
	m_c[1] = A[1];
	m_c[2] = A[2];
	m_c[3] = A[3];
	m_c[4] = A[4];
	m_c[5] = A[5];

}

public void Set(line a)
{
	m_c[0] = a.m_c[0];
	m_c[1] = a.m_c[1];
	m_c[2] = a.m_c[2];
	m_c[3] = a.m_c[3];
	m_c[4] = a.m_c[4];
	m_c[5] = a.m_c[5];

}
	public void Set(mv src) {
		if (src.c()[3] != null) {
			float[] ptr = src.c()[3];
			m_c[0] = ptr[6];
			m_c[1] = ptr[8];
			m_c[2] = ptr[9];
			m_c[3] = -ptr[4];
			m_c[4] = -ptr[5];
			m_c[5] = -ptr[7];
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

	/// <summary>Returns the absolute largest coordinate.
	/// </summary>
	public float LargestCoordinate() {
		float maxValue = Math.Abs(m_c[0]);
		if (Math.Abs(m_c[1]) > maxValue) { maxValue = Math.Abs(m_c[1]); }
		if (Math.Abs(m_c[2]) > maxValue) { maxValue = Math.Abs(m_c[2]); }
		if (Math.Abs(m_c[3]) > maxValue) { maxValue = Math.Abs(m_c[3]); }
		if (Math.Abs(m_c[4]) > maxValue) { maxValue = Math.Abs(m_c[4]); }
		if (Math.Abs(m_c[5]) > maxValue) { maxValue = Math.Abs(m_c[5]); }
		return maxValue;
	}
	/// <summary>Returns the absolute largest coordinate,
	/// and the corresponding basis blade bitmap.
	/// </summary>
	public float LargestBasisBlade(int bm)  {
		float maxValue = Math.Abs(m_c[0]);
		bm = 0;
		if (Math.Abs(m_c[1]) > maxValue) { maxValue = Math.Abs(m_c[1]); bm = 26; }
		if (Math.Abs(m_c[2]) > maxValue) { maxValue = Math.Abs(m_c[2]); bm = 28; }
		if (Math.Abs(m_c[3]) > maxValue) { maxValue = Math.Abs(m_c[3]); bm = 19; }
		if (Math.Abs(m_c[4]) > maxValue) { maxValue = Math.Abs(m_c[4]); bm = 21; }
		if (Math.Abs(m_c[5]) > maxValue) { maxValue = Math.Abs(m_c[5]); bm = 25; }
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
	/// <summary>Returns the e1^e2^ni coordinate.
	/// </summary>
	public float get_e1_e2_ni() { return m_c[0];}
	/// <summary>Sets the e1^e2^ni coordinate.
	/// </summary>
	public void set_e1_e2_ni(float e1_e2_ni) { m_c[0] = e1_e2_ni;}
	/// <summary>Returns the e1^e3^ni coordinate.
	/// </summary>
	public float get_e1_e3_ni() { return m_c[1];}
	/// <summary>Sets the e1^e3^ni coordinate.
	/// </summary>
	public void set_e1_e3_ni(float e1_e3_ni) { m_c[1] = e1_e3_ni;}
	/// <summary>Returns the e2^e3^ni coordinate.
	/// </summary>
	public float get_e2_e3_ni() { return m_c[2];}
	/// <summary>Sets the e2^e3^ni coordinate.
	/// </summary>
	public void set_e2_e3_ni(float e2_e3_ni) { m_c[2] = e2_e3_ni;}
	/// <summary>Returns the -1*no^e1^ni coordinate.
	/// </summary>
	public float get_e1_no_ni() { return m_c[3];}
	/// <summary>Sets the -1*no^e1^ni coordinate.
	/// </summary>
	public void set_e1_no_ni(float e1_no_ni) { m_c[3] = e1_no_ni;}
	/// <summary>Returns the -1*no^e2^ni coordinate.
	/// </summary>
	public float get_e2_no_ni() { return m_c[4];}
	/// <summary>Sets the -1*no^e2^ni coordinate.
	/// </summary>
	public void set_e2_no_ni(float e2_no_ni) { m_c[4] = e2_no_ni;}
	/// <summary>Returns the -1*no^e3^ni coordinate.
	/// </summary>
	public float get_e3_no_ni() { return m_c[5];}
	/// <summary>Sets the -1*no^e3^ni coordinate.
	/// </summary>
	public void set_e3_no_ni(float e3_no_ni) { m_c[5] = e3_no_ni;}
	/// <summary>Returns the scalar coordinate (which is always 0).
	/// </summary>
	public float get_scalar() { return 0.0f;}
	/// <summary>Returns array of coordinates.
	/// </summary>
	/// <param name="coordOrder">pass the value 'line.coord_e1e2ni_e1e3ni_e2e3ni_e1noni_e2noni_e3noni'
	/// </param>
	public float[] c(CoordinateOrder coordOrder) { return m_c;}

	/// <summary>shortcut to c3ga.dual(this)
	/// </summary>
	public dualLine dual() {
		return c3ga.dual(this);
	}
} // end of class line
} // end of namespace c3ga_ns
