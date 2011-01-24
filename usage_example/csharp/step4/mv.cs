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
/// <summary>This class can hold a general multivector.
/// 
/// The coordinates are stored in type float.
/// 
/// There are 6 coordinate groups:
/// group 0:1  (grade 0).
/// group 1:no, e1, e2, e3, ni  (grade 1).
/// group 2:no^e1, no^e2, e1^e2, no^e3, e1^e3, e2^e3, no^ni, e1^ni, e2^ni, e3^ni  (grade 2).
/// group 3:no^e1^e2, no^e1^e3, no^e2^e3, e1^e2^e3, no^e1^ni, no^e2^ni, e1^e2^ni, no^e3^ni, e1^e3^ni, e2^e3^ni  (grade 3).
/// group 4:no^e1^e2^e3, no^e1^e2^ni, no^e1^e3^ni, no^e2^e3^ni, e1^e2^e3^ni  (grade 4).
/// group 5:no^e1^e2^e3^ni  (grade 5).
/// 
/// 
/// </summary>
public class mv  :  mv_if
{ 

    /// <summary>
	/// the coordinates
    /// </summary>
	protected internal float[][] m_c = new float[6][]; 
	


    /// <summary>
	/// Constructs a new mv with value 0.
    /// </summary>
	public mv() {Set();}

    /// <summary>
	/// Copy constructor.
    /// </summary>
	public mv(mv A) {Set(A);}


    /// <summary>
	/// Constructs a new mv with scalar value 'scalar'.
    /// </summary>
	public mv(float scalar) {Set(scalar);}

    /// <summary>
	/// Constructs a new mv from compressed 'coordinates'.
	/// <param name="gu">bitwise OR of the GRADEs or GROUPs that are non-zero.</param>
	/// <param name="coordinates"> compressed coordinates.</param>
    /// </summary>
	public mv(GroupBitmap gu, float[] coordinates) {Set(gu, coordinates);}

    /// <summary>
	/// Constructs a new mv from 'coordinates'.
	/// <param name="coordinates">The coordinates (one array for each group, entries may be null). The arrays are kept.</param>
    /// </summary>
	public mv(float[][] coordinates) {Set(coordinates);}
	
    /// <summary>
	/// Converts a normalizedPoint to a mv.
    /// </summary>
	public mv(normalizedPoint A) {Set(A);}
    /// <summary>
	/// Converts a flatPoint to a mv.
    /// </summary>
	public mv(flatPoint A) {Set(A);}
    /// <summary>
	/// Converts a line to a mv.
    /// </summary>
	public mv(line A) {Set(A);}
    /// <summary>
	/// Converts a dualLine to a mv.
    /// </summary>
	public mv(dualLine A) {Set(A);}
    /// <summary>
	/// Converts a plane to a mv.
    /// </summary>
	public mv(plane A) {Set(A);}
    /// <summary>
	/// Converts a no_t to a mv.
    /// </summary>
	public mv(no_t A) {Set(A);}
    /// <summary>
	/// Converts a e1_t to a mv.
    /// </summary>
	public mv(e1_t A) {Set(A);}
    /// <summary>
	/// Converts a e2_t to a mv.
    /// </summary>
	public mv(e2_t A) {Set(A);}
    /// <summary>
	/// Converts a e3_t to a mv.
    /// </summary>
	public mv(e3_t A) {Set(A);}
    /// <summary>
	/// Converts a ni_t to a mv.
    /// </summary>
	public mv(ni_t A) {Set(A);}
    /// <summary>
	/// Converts a pointPair to a mv.
    /// </summary>
	public mv(pointPair A) {Set(A);}
    /// <summary>
	/// Converts a circle to a mv.
    /// </summary>
	public mv(circle A) {Set(A);}


    /// <summary>
	/// returns group usage bitmap
    /// </summary>
	public GroupBitmap gu() {
		return 
			((m_c[0] == null) ? 0 : GroupBitmap.GROUP_0) |
			((m_c[1] == null) ? 0 : GroupBitmap.GROUP_1) |
			((m_c[2] == null) ? 0 : GroupBitmap.GROUP_2) |
			((m_c[3] == null) ? 0 : GroupBitmap.GROUP_3) |
			((m_c[4] == null) ? 0 : GroupBitmap.GROUP_4) |
			((m_c[5] == null) ? 0 : GroupBitmap.GROUP_5) |
			0;
	}
    /// <summary>
	/// Returns array of array of coordinates.
	/// Each entry contain the coordinates for one group/grade.
    /// </summary>
	public float[][] c() { return m_c; }
	
	/// <summary>sets this to 0.
	/// </summary>
	public void Set() {
		m_c[0] = null;
		m_c[1] = null;
		m_c[2] = null;
		m_c[3] = null;
		m_c[4] = null;
		m_c[5] = null;
	}
	/// <summary>sets this to scalar value.
	/// </summary>
	public void Set(float val) {
		AllocateGroups(GroupBitmap.GROUP_0);
		m_c[0][0] = val;
	}
	/// <summary>sets this coordinates in 'arr'.
	/// </summary>
	/// <param name="gu">bitwise or of the GROUPs and GRADEs which are present in 'arr'.
	/// </param>
	/// <param name="arr">compressed coordinates.
	/// </param>
	public void Set(GroupBitmap gu, float[] arr) {
		AllocateGroups(gu);
		int idx = 0;
		if ((gu & GroupBitmap.GROUP_0) != 0) {
			for (int i = 0; i < 1; i++)
				m_c[0][i] = arr[idx + i];
			idx += 1;
		}
		if ((gu & GroupBitmap.GROUP_1) != 0) {
			for (int i = 0; i < 5; i++)
				m_c[1][i] = arr[idx + i];
			idx += 5;
		}
		if ((gu & GroupBitmap.GROUP_2) != 0) {
			for (int i = 0; i < 10; i++)
				m_c[2][i] = arr[idx + i];
			idx += 10;
		}
		if ((gu & GroupBitmap.GROUP_3) != 0) {
			for (int i = 0; i < 10; i++)
				m_c[3][i] = arr[idx + i];
			idx += 10;
		}
		if ((gu & GroupBitmap.GROUP_4) != 0) {
			for (int i = 0; i < 5; i++)
				m_c[4][i] = arr[idx + i];
			idx += 5;
		}
		if ((gu & GroupBitmap.GROUP_5) != 0) {
			for (int i = 0; i < 1; i++)
				m_c[5][i] = arr[idx + i];
			idx += 1;
		}
	}
	/// <summary>sets this coordinates in 'arr'. 
	/// 'arr' is kept, so changes to 'arr' will be reflected in the value of this multivector. Make sure 'arr' has length 6 and each subarray has the length of the respective group/grade
	/// </summary>
	/// <param name="arr">coordinates.
	/// </param>
	public void Set(float[][] arr) {
		m_c = arr;
	}
	/// <summary>sets this to multivector value.
	/// </summary>
	public void Set(mv src) {
		AllocateGroups(src.gu());
		if (m_c[0] != null) {
			c3ga.Copy_1(m_c[0], src.m_c[0]);
		}
		if (m_c[1] != null) {
			c3ga.Copy_5(m_c[1], src.m_c[1]);
		}
		if (m_c[2] != null) {
			c3ga.Copy_10(m_c[2], src.m_c[2]);
		}
		if (m_c[3] != null) {
			c3ga.Copy_10(m_c[3], src.m_c[3]);
		}
		if (m_c[4] != null) {
			c3ga.Copy_5(m_c[4], src.m_c[4]);
		}
		if (m_c[5] != null) {
			c3ga.Copy_1(m_c[5], src.m_c[5]);
		}
	}

	/// <summary>sets this to normalizedPoint value.
	/// </summary>
	public void Set(normalizedPoint src) {
		AllocateGroups(GroupBitmap.GROUP_1);
		float[] ptr;

		ptr = m_c[1];
		ptr[0] = 1.0f;
		ptr[1] = src.m_c[0];
		ptr[2] = src.m_c[1];
		ptr[3] = src.m_c[2];
		ptr[4] = src.m_c[3];
	}

	/// <summary>sets this to flatPoint value.
	/// </summary>
	public void Set(flatPoint src) {
		AllocateGroups(GroupBitmap.GROUP_2);
		float[] ptr;

		ptr = m_c[2];
		ptr[0] = ptr[1] = ptr[2] = ptr[3] = ptr[4] = ptr[5] = 0.0f;
		ptr[6] = src.m_c[3];
		ptr[7] = src.m_c[0];
		ptr[8] = src.m_c[1];
		ptr[9] = src.m_c[2];
	}

	/// <summary>sets this to line value.
	/// </summary>
	public void Set(line src) {
		AllocateGroups(GroupBitmap.GROUP_3);
		float[] ptr;

		ptr = m_c[3];
		ptr[0] = ptr[1] = ptr[2] = ptr[3] = 0.0f;
		ptr[4] = -src.m_c[3];
		ptr[5] = -src.m_c[4];
		ptr[6] = src.m_c[0];
		ptr[7] = -src.m_c[5];
		ptr[8] = src.m_c[1];
		ptr[9] = src.m_c[2];
	}

	/// <summary>sets this to dualLine value.
	/// </summary>
	public void Set(dualLine src) {
		AllocateGroups(GroupBitmap.GROUP_2);
		float[] ptr;

		ptr = m_c[2];
		ptr[0] = ptr[1] = ptr[3] = ptr[6] = 0.0f;
		ptr[2] = src.m_c[0];
		ptr[4] = src.m_c[1];
		ptr[5] = src.m_c[2];
		ptr[7] = src.m_c[3];
		ptr[8] = src.m_c[4];
		ptr[9] = src.m_c[5];
	}

	/// <summary>sets this to plane value.
	/// </summary>
	public void Set(plane src) {
		AllocateGroups(GroupBitmap.GROUP_4);
		float[] ptr;

		ptr = m_c[4];
		ptr[0] = 0.0f;
		ptr[1] = src.m_c[3];
		ptr[2] = src.m_c[2];
		ptr[3] = src.m_c[1];
		ptr[4] = src.m_c[0];
	}

	/// <summary>sets this to no_t value.
	/// </summary>
	public void Set(no_t src) {
		AllocateGroups(GroupBitmap.GROUP_1);
		float[] ptr;

		ptr = m_c[1];
		ptr[0] = 1.0f;
		ptr[1] = ptr[2] = ptr[3] = ptr[4] = 0.0f;
	}

	/// <summary>sets this to e1_t value.
	/// </summary>
	public void Set(e1_t src) {
		AllocateGroups(GroupBitmap.GROUP_1);
		float[] ptr;

		ptr = m_c[1];
		ptr[0] = ptr[2] = ptr[3] = ptr[4] = 0.0f;
		ptr[1] = 1.0f;
	}

	/// <summary>sets this to e2_t value.
	/// </summary>
	public void Set(e2_t src) {
		AllocateGroups(GroupBitmap.GROUP_1);
		float[] ptr;

		ptr = m_c[1];
		ptr[0] = ptr[1] = ptr[3] = ptr[4] = 0.0f;
		ptr[2] = 1.0f;
	}

	/// <summary>sets this to e3_t value.
	/// </summary>
	public void Set(e3_t src) {
		AllocateGroups(GroupBitmap.GROUP_1);
		float[] ptr;

		ptr = m_c[1];
		ptr[0] = ptr[1] = ptr[2] = ptr[4] = 0.0f;
		ptr[3] = 1.0f;
	}

	/// <summary>sets this to ni_t value.
	/// </summary>
	public void Set(ni_t src) {
		AllocateGroups(GroupBitmap.GROUP_1);
		float[] ptr;

		ptr = m_c[1];
		ptr[0] = ptr[1] = ptr[2] = ptr[3] = 0.0f;
		ptr[4] = 1.0f;
	}

	/// <summary>sets this to pointPair value.
	/// </summary>
	public void Set(pointPair src) {
		AllocateGroups(GroupBitmap.GROUP_2);
		float[] ptr;

		ptr = m_c[2];
		ptr[0] = src.m_c[0];
		ptr[1] = src.m_c[1];
		ptr[2] = src.m_c[2];
		ptr[3] = src.m_c[3];
		ptr[4] = src.m_c[4];
		ptr[5] = src.m_c[5];
		ptr[6] = src.m_c[6];
		ptr[7] = src.m_c[7];
		ptr[8] = src.m_c[8];
		ptr[9] = src.m_c[9];
	}

	/// <summary>sets this to circle value.
	/// </summary>
	public void Set(circle src) {
		AllocateGroups(GroupBitmap.GROUP_3);
		float[] ptr;

		ptr = m_c[3];
		ptr[0] = src.m_c[0];
		ptr[1] = src.m_c[1];
		ptr[2] = src.m_c[2];
		ptr[3] = src.m_c[3];
		ptr[4] = src.m_c[4];
		ptr[5] = src.m_c[5];
		ptr[6] = src.m_c[6];
		ptr[7] = src.m_c[7];
		ptr[8] = src.m_c[8];
		ptr[9] = src.m_c[9];
	}
	/// <summary>Returns the scalar coordinate of this mv
	/// </summary>
	public float get_scalar()  {
		return (m_c[0] == null) ? 0.0f: m_c[0][0];
	}
	/// <summary>Returns the no coordinate of this mv
	/// </summary>
	public float get_no()  {
		return (m_c[1] == null) ? 0.0f: m_c[1][0];
	}
	/// <summary>Returns the e1 coordinate of this mv
	/// </summary>
	public float get_e1()  {
		return (m_c[1] == null) ? 0.0f: m_c[1][1];
	}
	/// <summary>Returns the e2 coordinate of this mv
	/// </summary>
	public float get_e2()  {
		return (m_c[1] == null) ? 0.0f: m_c[1][2];
	}
	/// <summary>Returns the e3 coordinate of this mv
	/// </summary>
	public float get_e3()  {
		return (m_c[1] == null) ? 0.0f: m_c[1][3];
	}
	/// <summary>Returns the ni coordinate of this mv
	/// </summary>
	public float get_ni()  {
		return (m_c[1] == null) ? 0.0f: m_c[1][4];
	}
	/// <summary>Returns the no_e1 coordinate of this mv
	/// </summary>
	public float get_no_e1()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][0];
	}
	/// <summary>Returns the no_e2 coordinate of this mv
	/// </summary>
	public float get_no_e2()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][1];
	}
	/// <summary>Returns the e1_e2 coordinate of this mv
	/// </summary>
	public float get_e1_e2()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][2];
	}
	/// <summary>Returns the no_e3 coordinate of this mv
	/// </summary>
	public float get_no_e3()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][3];
	}
	/// <summary>Returns the e1_e3 coordinate of this mv
	/// </summary>
	public float get_e1_e3()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][4];
	}
	/// <summary>Returns the e2_e3 coordinate of this mv
	/// </summary>
	public float get_e2_e3()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][5];
	}
	/// <summary>Returns the no_ni coordinate of this mv
	/// </summary>
	public float get_no_ni()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][6];
	}
	/// <summary>Returns the e1_ni coordinate of this mv
	/// </summary>
	public float get_e1_ni()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][7];
	}
	/// <summary>Returns the e2_ni coordinate of this mv
	/// </summary>
	public float get_e2_ni()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][8];
	}
	/// <summary>Returns the e3_ni coordinate of this mv
	/// </summary>
	public float get_e3_ni()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][9];
	}
	/// <summary>Returns the no_e1_e2 coordinate of this mv
	/// </summary>
	public float get_no_e1_e2()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][0];
	}
	/// <summary>Returns the no_e1_e3 coordinate of this mv
	/// </summary>
	public float get_no_e1_e3()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][1];
	}
	/// <summary>Returns the no_e2_e3 coordinate of this mv
	/// </summary>
	public float get_no_e2_e3()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][2];
	}
	/// <summary>Returns the e1_e2_e3 coordinate of this mv
	/// </summary>
	public float get_e1_e2_e3()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][3];
	}
	/// <summary>Returns the no_e1_ni coordinate of this mv
	/// </summary>
	public float get_no_e1_ni()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][4];
	}
	/// <summary>Returns the no_e2_ni coordinate of this mv
	/// </summary>
	public float get_no_e2_ni()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][5];
	}
	/// <summary>Returns the e1_e2_ni coordinate of this mv
	/// </summary>
	public float get_e1_e2_ni()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][6];
	}
	/// <summary>Returns the no_e3_ni coordinate of this mv
	/// </summary>
	public float get_no_e3_ni()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][7];
	}
	/// <summary>Returns the e1_e3_ni coordinate of this mv
	/// </summary>
	public float get_e1_e3_ni()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][8];
	}
	/// <summary>Returns the e2_e3_ni coordinate of this mv
	/// </summary>
	public float get_e2_e3_ni()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][9];
	}
	/// <summary>Returns the no_e1_e2_e3 coordinate of this mv
	/// </summary>
	public float get_no_e1_e2_e3()  {
		return (m_c[4] == null) ? 0.0f: m_c[4][0];
	}
	/// <summary>Returns the no_e1_e2_ni coordinate of this mv
	/// </summary>
	public float get_no_e1_e2_ni()  {
		return (m_c[4] == null) ? 0.0f: m_c[4][1];
	}
	/// <summary>Returns the no_e1_e3_ni coordinate of this mv
	/// </summary>
	public float get_no_e1_e3_ni()  {
		return (m_c[4] == null) ? 0.0f: m_c[4][2];
	}
	/// <summary>Returns the no_e2_e3_ni coordinate of this mv
	/// </summary>
	public float get_no_e2_e3_ni()  {
		return (m_c[4] == null) ? 0.0f: m_c[4][3];
	}
	/// <summary>Returns the e1_e2_e3_ni coordinate of this mv
	/// </summary>
	public float get_e1_e2_e3_ni()  {
		return (m_c[4] == null) ? 0.0f: m_c[4][4];
	}
	/// <summary>Returns the no_e1_e2_e3_ni coordinate of this mv
	/// </summary>
	public float get_no_e1_e2_e3_ni()  {
		return (m_c[5] == null) ? 0.0f: m_c[5][0];
	}

    /// <summary>
	/// Reserves memory for the groups specified by 'gu'.
	/// Keeps old memory (and values) when possible.
    /// </summary>
	private void AllocateGroups(GroupBitmap gu) {
		for (int i = 0; (1 << i) <= (int)gu; i++) {
			if (((1 << i) & (int)gu) != 0) {
				if (m_c[i] == null)
					m_c[i] = new float[c3ga.MvSize[1 << i]];
			}
			else m_c[i] = null;
		}		
	}

	/// <summary>
	/// Reserves memory for coordinate GROUP_0.
	/// If the group is already present, nothing changes.
	/// If the group is not present, memory is allocated for the new group,
	/// and the coordinates for the group are set to zero.
	/// </summary>
	private void ReserveGroup_0() {
		if (m_c[0] == null) {
			m_c[0] = new float[1];
		}
	}
	/// <summary>
	/// Reserves memory for coordinate GROUP_1.
	/// If the group is already present, nothing changes.
	/// If the group is not present, memory is allocated for the new group,
	/// and the coordinates for the group are set to zero.
	/// </summary>
	private void ReserveGroup_1() {
		if (m_c[1] == null) {
			m_c[1] = new float[5];
		}
	}
	/// <summary>
	/// Reserves memory for coordinate GROUP_2.
	/// If the group is already present, nothing changes.
	/// If the group is not present, memory is allocated for the new group,
	/// and the coordinates for the group are set to zero.
	/// </summary>
	private void ReserveGroup_2() {
		if (m_c[2] == null) {
			m_c[2] = new float[10];
		}
	}
	/// <summary>
	/// Reserves memory for coordinate GROUP_3.
	/// If the group is already present, nothing changes.
	/// If the group is not present, memory is allocated for the new group,
	/// and the coordinates for the group are set to zero.
	/// </summary>
	private void ReserveGroup_3() {
		if (m_c[3] == null) {
			m_c[3] = new float[10];
		}
	}
	/// <summary>
	/// Reserves memory for coordinate GROUP_4.
	/// If the group is already present, nothing changes.
	/// If the group is not present, memory is allocated for the new group,
	/// and the coordinates for the group are set to zero.
	/// </summary>
	private void ReserveGroup_4() {
		if (m_c[4] == null) {
			m_c[4] = new float[5];
		}
	}
	/// <summary>
	/// Reserves memory for coordinate GROUP_5.
	/// If the group is already present, nothing changes.
	/// If the group is not present, memory is allocated for the new group,
	/// and the coordinates for the group are set to zero.
	/// </summary>
	private void ReserveGroup_5() {
		if (m_c[5] == null) {
			m_c[5] = new float[1];
		}
	}
	/// Sets the scalar coordinate of this mv.
	public void set_scalar(float val)  {
		ReserveGroup_0();
		m_c[0][0] =  val;
	}
	/// Sets the no coordinate of this mv.
	public void set_no(float val)  {
		ReserveGroup_1();
		m_c[1][0] =  val;
	}
	/// Sets the e1 coordinate of this mv.
	public void set_e1(float val)  {
		ReserveGroup_1();
		m_c[1][1] =  val;
	}
	/// Sets the e2 coordinate of this mv.
	public void set_e2(float val)  {
		ReserveGroup_1();
		m_c[1][2] =  val;
	}
	/// Sets the e3 coordinate of this mv.
	public void set_e3(float val)  {
		ReserveGroup_1();
		m_c[1][3] =  val;
	}
	/// Sets the ni coordinate of this mv.
	public void set_ni(float val)  {
		ReserveGroup_1();
		m_c[1][4] =  val;
	}
	/// Sets the no_e1 coordinate of this mv.
	public void set_no_e1(float val)  {
		ReserveGroup_2();
		m_c[2][0] =  val;
	}
	/// Sets the no_e2 coordinate of this mv.
	public void set_no_e2(float val)  {
		ReserveGroup_2();
		m_c[2][1] =  val;
	}
	/// Sets the e1_e2 coordinate of this mv.
	public void set_e1_e2(float val)  {
		ReserveGroup_2();
		m_c[2][2] =  val;
	}
	/// Sets the no_e3 coordinate of this mv.
	public void set_no_e3(float val)  {
		ReserveGroup_2();
		m_c[2][3] =  val;
	}
	/// Sets the e1_e3 coordinate of this mv.
	public void set_e1_e3(float val)  {
		ReserveGroup_2();
		m_c[2][4] =  val;
	}
	/// Sets the e2_e3 coordinate of this mv.
	public void set_e2_e3(float val)  {
		ReserveGroup_2();
		m_c[2][5] =  val;
	}
	/// Sets the no_ni coordinate of this mv.
	public void set_no_ni(float val)  {
		ReserveGroup_2();
		m_c[2][6] =  val;
	}
	/// Sets the e1_ni coordinate of this mv.
	public void set_e1_ni(float val)  {
		ReserveGroup_2();
		m_c[2][7] =  val;
	}
	/// Sets the e2_ni coordinate of this mv.
	public void set_e2_ni(float val)  {
		ReserveGroup_2();
		m_c[2][8] =  val;
	}
	/// Sets the e3_ni coordinate of this mv.
	public void set_e3_ni(float val)  {
		ReserveGroup_2();
		m_c[2][9] =  val;
	}
	/// Sets the no_e1_e2 coordinate of this mv.
	public void set_no_e1_e2(float val)  {
		ReserveGroup_3();
		m_c[3][0] =  val;
	}
	/// Sets the no_e1_e3 coordinate of this mv.
	public void set_no_e1_e3(float val)  {
		ReserveGroup_3();
		m_c[3][1] =  val;
	}
	/// Sets the no_e2_e3 coordinate of this mv.
	public void set_no_e2_e3(float val)  {
		ReserveGroup_3();
		m_c[3][2] =  val;
	}
	/// Sets the e1_e2_e3 coordinate of this mv.
	public void set_e1_e2_e3(float val)  {
		ReserveGroup_3();
		m_c[3][3] =  val;
	}
	/// Sets the no_e1_ni coordinate of this mv.
	public void set_no_e1_ni(float val)  {
		ReserveGroup_3();
		m_c[3][4] =  val;
	}
	/// Sets the no_e2_ni coordinate of this mv.
	public void set_no_e2_ni(float val)  {
		ReserveGroup_3();
		m_c[3][5] =  val;
	}
	/// Sets the e1_e2_ni coordinate of this mv.
	public void set_e1_e2_ni(float val)  {
		ReserveGroup_3();
		m_c[3][6] =  val;
	}
	/// Sets the no_e3_ni coordinate of this mv.
	public void set_no_e3_ni(float val)  {
		ReserveGroup_3();
		m_c[3][7] =  val;
	}
	/// Sets the e1_e3_ni coordinate of this mv.
	public void set_e1_e3_ni(float val)  {
		ReserveGroup_3();
		m_c[3][8] =  val;
	}
	/// Sets the e2_e3_ni coordinate of this mv.
	public void set_e2_e3_ni(float val)  {
		ReserveGroup_3();
		m_c[3][9] =  val;
	}
	/// Sets the no_e1_e2_e3 coordinate of this mv.
	public void set_no_e1_e2_e3(float val)  {
		ReserveGroup_4();
		m_c[4][0] =  val;
	}
	/// Sets the no_e1_e2_ni coordinate of this mv.
	public void set_no_e1_e2_ni(float val)  {
		ReserveGroup_4();
		m_c[4][1] =  val;
	}
	/// Sets the no_e1_e3_ni coordinate of this mv.
	public void set_no_e1_e3_ni(float val)  {
		ReserveGroup_4();
		m_c[4][2] =  val;
	}
	/// Sets the no_e2_e3_ni coordinate of this mv.
	public void set_no_e2_e3_ni(float val)  {
		ReserveGroup_4();
		m_c[4][3] =  val;
	}
	/// Sets the e1_e2_e3_ni coordinate of this mv.
	public void set_e1_e2_e3_ni(float val)  {
		ReserveGroup_4();
		m_c[4][4] =  val;
	}
	/// Sets the no_e1_e2_e3_ni coordinate of this mv.
	public void set_no_e1_e2_e3_ni(float val)  {
		ReserveGroup_5();
		m_c[5][0] =  val;
	}

	/// <summary>returns the absolute largest coordinate.</summary>
	public float LargestCoordinate() {
		float maxValue = 0.0f, C;
		for (int g = 0; g < m_c.Length; g++) {
			if (m_c[g] != null) {
				float[] Cg = m_c[g];
				for (int b = 0; b < Cg.Length; b++) {
					C = Math.Abs(Cg[b]);
					if (C > maxValue) {
						maxValue = C;
					}
				}
			}
		}
		return maxValue;
	}
	
	/// <summary>returns the absolute largest coordinate and the corresponding basis blade bitmap (in 'bm') .</summary>
	public float LargestBasisBlade(ref int bm) {
		float maxC = -1.0f, C;

		int idx = 0; // global index into coordinates (run from 0 to 32).
		bm = 0;
		
		for (int g = 0; g < m_c.Length; g++) {
			if (m_c[g] != null) {
				float[] Cg = m_c[g];
				for (int b = 0; b < m_c[g].Length; b++) {
					C = Math.Abs(Cg[b]);
					if (C > maxC) {
						maxC = C;
						bm = c3ga.BasisElementBitmapByIndex[idx];
					}
					idx++;
				}
			
			}
			else idx += c3ga.GroupSize[g];
		}

		return maxC;
	} // end of LargestBasisBlade()
	
	/// <summary>Releases memory for (near-)zero groups/grades.
	/// This also speeds up subsequent operations, because those do not have to process the released groups/grades anymore.
	/// </summary>
	/// <param name="eps">A positive threshold value.
	/// Coordinates which are smaller than epsilon are considered to be zero.
	/// </param>
	public void Compress(float eps)  {
		if ((m_c[0] != null) && c3ga.zeroGroup_0(m_c[0], eps))
			m_c[0] = null;
		if ((m_c[1] != null) && c3ga.zeroGroup_1(m_c[1], eps))
			m_c[1] = null;
		if ((m_c[2] != null) && c3ga.zeroGroup_2(m_c[2], eps))
			m_c[2] = null;
		if ((m_c[3] != null) && c3ga.zeroGroup_3(m_c[3], eps))
			m_c[3] = null;
		if ((m_c[4] != null) && c3ga.zeroGroup_4(m_c[4], eps))
			m_c[4] = null;
		if ((m_c[5] != null) && c3ga.zeroGroup_5(m_c[5], eps))
			m_c[5] = null;
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

    /// <summary>	
    /// Converts this multivector to a 'mv' (implementation of interface 'mv_interface')
    /// </summary>
    public mv to_mv()
    {
        return this;
    }

	/// <summary>shortcut to c3ga.add(this, b)
	/// </summary>
	public mv add(mv_if b) {
		return c3ga.add(this, b);
	}

	/// <summary>operator for c3ga.add(a, b)
	/// </summary>
	public static mv operator +(mv a, mv b) {
		return c3ga.add(a, b);
	}

	/// <summary>shortcut to c3ga.subtract(this, b)
	/// </summary>
	public mv subtract(mv_if b) {
		return c3ga.subtract(this, b);
	}

	/// <summary>operator for c3ga.subtract(a, b)
	/// </summary>
	public static mv operator -(mv a, mv b) {
		return c3ga.subtract(a, b);
	}

	/// <summary>shortcut to c3ga.extractGrade(this, groupBitmap)
	/// </summary>
	public mv extractGrade(GroupBitmap groupBitmap) {
		return c3ga.extractGrade(this, groupBitmap);
	}

	/// <summary>shortcut to c3ga.negate(this)
	/// </summary>
	public mv negate() {
		return c3ga.negate(this);
	}

	/// <summary>operator for c3ga.negate(a)
	/// </summary>
	public static mv operator -(mv a) {
		return c3ga.negate(a);
	}

	/// <summary>shortcut to c3ga.reverse(this)
	/// </summary>
	public mv reverse() {
		return c3ga.reverse(this);
	}

	/// <summary>operator for c3ga.reverse(a)
	/// </summary>
	public static mv operator ~(mv a) {
		return c3ga.reverse(a);
	}

	/// <summary>shortcut to c3ga.gradeInvolution(this)
	/// </summary>
	public mv gradeInvolution() {
		return c3ga.gradeInvolution(this);
	}

	/// <summary>shortcut to c3ga.cliffordConjugate(this)
	/// </summary>
	public mv cliffordConjugate() {
		return c3ga.cliffordConjugate(this);
	}

	/// <summary>shortcut to c3ga.gp(this, b)
	/// </summary>
	public mv gp(mv_if b) {
		return c3ga.gp(this, b);
	}

	/// <summary>operator for c3ga.gp(a, b)
	/// </summary>
	public static mv operator *(mv a, mv b) {
		return c3ga.gp(a, b);
	}

	/// <summary>shortcut to c3ga.op(this, b)
	/// </summary>
	public mv op(mv_if b) {
		return c3ga.op(this, b);
	}

	/// <summary>operator for c3ga.op(a, b)
	/// </summary>
	public static mv operator ^(mv a, mv b) {
		return c3ga.op(a, b);
	}

	/// <summary>shortcut to c3ga.sp(this, b)
	/// </summary>
	public float sp(mv_if b) {
		return c3ga.sp(this, b);
	}

	/// <summary>operator for c3ga.sp(a, b)
	/// </summary>
	public static float operator %(mv a, mv b) {
		return c3ga.sp(a, b);
	}

	/// <summary>shortcut to c3ga.mhip(this, b)
	/// </summary>
	public mv mhip(mv_if b) {
		return c3ga.mhip(this, b);
	}

	/// <summary>shortcut to c3ga.lc(this, b)
	/// </summary>
	public mv lc(mv_if b) {
		return c3ga.lc(this, b);
	}

	/// <summary>shortcut to c3ga.norm(this)
	/// </summary>
	public float norm() {
		return c3ga.norm(this);
	}

	/// <summary>shortcut to c3ga.unit(this)
	/// </summary>
	public mv unit() {
		return c3ga.unit(this);
	}

	/// <summary>shortcut to c3ga.applyVersor(this, b)
	/// </summary>
	public mv applyVersor(mv_if b) {
		return c3ga.applyVersor(this, b);
	}

	/// <summary>shortcut to c3ga.applyUnitVersor(this, b)
	/// </summary>
	public mv applyUnitVersor(mv_if b) {
		return c3ga.applyUnitVersor(this, b);
	}

	/// <summary>shortcut to c3ga.versorInverse(this)
	/// </summary>
	public mv versorInverse() {
		return c3ga.versorInverse(this);
	}

	/// <summary>operator for c3ga.versorInverse(a)
	/// </summary>
	public static mv operator !(mv a) {
		return c3ga.versorInverse(a);
	}

	/// <summary>shortcut to c3ga.equals(this, b, c)
	/// </summary>
	public bool equals(mv_if b, float c) {
		return c3ga.equals(this, b, c);
	}

	/// <summary>shortcut to c3ga.zero(this, b)
	/// </summary>
	public bool zero(float b) {
		return c3ga.zero(this, b);
	}

	/// <summary>shortcut to c3ga.dual(this)
	/// </summary>
	public mv dual() {
		return c3ga.dual(this);
	}

	/// <summary>shortcut to c3ga.undual(this)
	/// </summary>
	public mv undual() {
		return c3ga.undual(this);
	}

	/// <summary>shortcut to c3ga.exp(this)
	/// </summary>
	public mv exp() {
		return c3ga.exp(this);
	}

	/// <summary>shortcut to c3ga.gp(this, b)
	/// </summary>
	public mv gp(float b) {
		return c3ga.gp(this, b);
	}

	/// <summary>operator for c3ga.gp(a, b)
	/// </summary>
	public static mv operator *(mv a, float b) {
		return c3ga.gp(a, b);
	}

	/// <summary>shortcut to c3ga.norm2(this)
	/// </summary>
	public float norm2() {
		return c3ga.norm2(this);
	}

	/// <summary>shortcut to c3ga.sas(this, b, c)
	/// </summary>
	public mv sas(float b, float c) {
		return c3ga.sas(this, b, c);
	}
} // end of class mv
} // end of namespace c3ga_ns
