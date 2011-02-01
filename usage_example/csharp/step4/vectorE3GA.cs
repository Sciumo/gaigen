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
/// <summary>This class can hold a specialized multivector of type vectorE3GA.
/// 
/// The coordinates are stored in type float.
/// 
/// The variable non-zero coordinates are:
///   - coordinate e1  (array index: E1 = 0)
///   - coordinate e2  (array index: E2 = 1)
///   - coordinate e3  (array index: E3 = 2)
/// 
/// The type has no constant coordinates.
/// 
/// 
/// </summary>
public class vectorE3GA  :  mv_if
{ 
	/// <summary> The coordinates (stored in an array).
	/// </summary>
	protected internal float[] m_c = new float[3]; // e1, e2, e3
	/// <summary>Array indices of vectorE3GA coordinates.
	/// </summary>

	/// <summary>index of coordinate for e1 in vectorE3GA
	/// </summary>
	public const int E1 = 0;

	/// <summary>index of coordinate for e2 in vectorE3GA
	/// </summary>
	public const int E2 = 1;

	/// <summary>index of coordinate for e3 in vectorE3GA
	/// </summary>
	public const int E3 = 2;

	/// <summary>The order of coordinates (this is the type of the first argument of coordinate-handling functions.
	/// </summary>
	public enum CoordinateOrder {
		coord_e1_e2_e3
	};
	public const CoordinateOrder coord_e1_e2_e3 = CoordinateOrder.coord_e1_e2_e3;

    /// <summary>	
    /// Converts this multivector to a 'mv' (implementation of interface 'mv_interface')
    /// </summary>
    public mv to_mv()
    {
        return new mv(this);
    }

    /// <summary>
	/// Constructs a new vectorE3GA with variable coordinates set to 0.
    /// </summary>
	public vectorE3GA() {Set();}

    /// <summary>
	/// Copy constructor.
    /// </summary>
	public vectorE3GA(vectorE3GA A) {Set(A);}



    /// <summary>
	/// Constructs a new vectorE3GA from mv.
    /// </summary>
	/// <param name="A">The value to copy. Coordinates that cannot be represented are silently dropped. </param>
	public vectorE3GA(mv A /*, int filler */) {Set(A);}

    /// <summary>
	/// Constructs a new vectorE3GA. Coordinate values come from 'A'.
    /// </summary>
	public vectorE3GA(CoordinateOrder co, float[] A) {Set(co, A);}
	
    /// <summary>
	/// Constructs a new vectorE3GA with each coordinate specified.
    /// </summary>
	public vectorE3GA(CoordinateOrder co,  float e1, float e2, float e3) {
		Set(co, e1, e2, e3);
	}

    /// <summary>
	/// Implicit converter from vectorE3GA to mv.
    /// </summary>
    public static implicit operator mv(vectorE3GA a)
    {
        return new mv(a);
    }

public void Set()
{
	m_c[0] = m_c[1] = m_c[2] = 0.0f;

}

public void Set(float scalarVal)
{
	m_c[0] = m_c[1] = m_c[2] = 0.0f;

}

public void Set(CoordinateOrder co, float _e1, float _e2, float _e3)
{
	m_c[0] = _e1;
	m_c[1] = _e2;
	m_c[2] = _e3;

}

public void Set(CoordinateOrder co, float[] A)
{
	m_c[0] = A[0];
	m_c[1] = A[1];
	m_c[2] = A[2];

}

public void Set(vectorE3GA a)
{
	m_c[0] = a.m_c[0];
	m_c[1] = a.m_c[1];
	m_c[2] = a.m_c[2];

}
	public void Set(mv src) {
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

	/// <summary>Returns the absolute largest coordinate.
	/// </summary>
	public float LargestCoordinate() {
		float maxValue = Math.Abs(m_c[0]);
		if (Math.Abs(m_c[1]) > maxValue) { maxValue = Math.Abs(m_c[1]); }
		if (Math.Abs(m_c[2]) > maxValue) { maxValue = Math.Abs(m_c[2]); }
		return maxValue;
	}
	/// <summary>Returns the absolute largest coordinate,
	/// and the corresponding basis blade bitmap.
	/// </summary>
	public float LargestBasisBlade(int bm)  {
		float maxValue = Math.Abs(m_c[0]);
		bm = 0;
		if (Math.Abs(m_c[1]) > maxValue) { maxValue = Math.Abs(m_c[1]); bm = 4; }
		if (Math.Abs(m_c[2]) > maxValue) { maxValue = Math.Abs(m_c[2]); bm = 8; }
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
	/// <summary>Returns the e1 coordinate.
	/// </summary>
	public float get_e1() { return m_c[0];}
	/// <summary>Sets the e1 coordinate.
	/// </summary>
	public void set_e1(float e1) { m_c[0] = e1;}
	/// <summary>Returns the e2 coordinate.
	/// </summary>
	public float get_e2() { return m_c[1];}
	/// <summary>Sets the e2 coordinate.
	/// </summary>
	public void set_e2(float e2) { m_c[1] = e2;}
	/// <summary>Returns the e3 coordinate.
	/// </summary>
	public float get_e3() { return m_c[2];}
	/// <summary>Sets the e3 coordinate.
	/// </summary>
	public void set_e3(float e3) { m_c[2] = e3;}
	/// <summary>Returns the scalar coordinate (which is always 0).
	/// </summary>
	public float get_scalar() { return 0.0f;}
	/// <summary>Returns array of coordinates.
	/// </summary>
	/// <param name="coordOrder">pass the value 'vectorE3GA.coord_e1_e2_e3'
	/// </param>
	public float[] c(CoordinateOrder coordOrder) { return m_c;}
} // end of class vectorE3GA
} // end of namespace c3ga_ns
