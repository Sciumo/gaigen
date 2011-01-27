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
 * This class can hold a general multivector.
 * 
 * The coordinates are stored in type float.
 * 
 * There are 6 coordinate groups:
 * group 0:1  (grade 0).
 * group 1:no, e1, e2, e3, ni  (grade 1).
 * group 2:no^e1, no^e2, e1^e2, no^e3, e1^e3, e2^e3, no^ni, e1^ni, e2^ni, e3^ni  (grade 2).
 * group 3:no^e1^e2, no^e1^e3, no^e2^e3, e1^e2^e3, no^e1^ni, no^e2^ni, e1^e2^ni, no^e3^ni, e1^e3^ni, e2^e3^ni  (grade 3).
 * group 4:no^e1^e2^e3, no^e1^e2^ni, no^e1^e3^ni, no^e2^e3^ni, e1^e2^e3^ni  (grade 4).
 * group 5:no^e1^e2^e3^ni  (grade 5).
 * 
 * 
 */
public class Mv  implements  Mv_if
{ 

    /** the coordinates */
	protected float[][] m_c = new float[6][]; 
	


    /**
	* Constructs a new Mv with value 0.
     */
	public Mv() {set();}

    /**
	 * Copy constructor.
     */
	public Mv(final Mv A) {set(A);}


    /**
	 * Constructs a new Mv with scalar value 'scalar'.
     */
	public Mv(final float scalar) {set(scalar);}

    /** 
     * Constructs a new Mv from compressed 'coordinates'.
 	 * @param gu bitwise OR of the GRADEs or GROUPs that are non-zero.
	 * @param coordinates compressed coordinates.
     */
	public Mv(final int gu, final float[] coordinates) {set(gu, coordinates);}

    /** 
     * Constructs a new Mv from array of array of 'coordinates'.
	 * @param coordinates The coordinates, one array for each group/grade. Make sure the
	 * array length match the size of the groups. Entries may be null.
     */
	public Mv(final float[][] coordinates) {set(coordinates);}
	
    /** Converts a NormalizedPoint to a Mv. */
	public Mv(final NormalizedPoint A) {set(A);}
    /** Converts a FlatPoint to a Mv. */
	public Mv(final FlatPoint A) {set(A);}
    /** Converts a Line to a Mv. */
	public Mv(final Line A) {set(A);}
    /** Converts a Plane to a Mv. */
	public Mv(final Plane A) {set(A);}
    /** Converts a no_t to a Mv. */
	public Mv(final no_t A) {set(A);}
    /** Converts a e1_t to a Mv. */
	public Mv(final e1_t A) {set(A);}
    /** Converts a e2_t to a Mv. */
	public Mv(final e2_t A) {set(A);}
    /** Converts a e3_t to a Mv. */
	public Mv(final e3_t A) {set(A);}
    /** Converts a ni_t to a Mv. */
	public Mv(final ni_t A) {set(A);}


	/** returns group usage bitmap. */
	public final int gu() {
		return 
			((m_c[0] == null) ? 0 : GroupBitmap.GROUP_0) |
			((m_c[1] == null) ? 0 : GroupBitmap.GROUP_1) |
			((m_c[2] == null) ? 0 : GroupBitmap.GROUP_2) |
			((m_c[3] == null) ? 0 : GroupBitmap.GROUP_3) |
			((m_c[4] == null) ? 0 : GroupBitmap.GROUP_4) |
			((m_c[5] == null) ? 0 : GroupBitmap.GROUP_5) |
			0;
	}
	
    /**
	 * Returns array of array of coordinates.
	 * Each entry contain the coordinates for one group/grade.
     */
	public final float[][] c() { return m_c; }
	
	/**
	 * sets this to 0.
	 */
	public void set() {
		m_c[0] = null;
		m_c[1] = null;
		m_c[2] = null;
		m_c[3] = null;
		m_c[4] = null;
		m_c[5] = null;
	}
	/**
	 * sets this to scalar value.
	 */
	public void set(float val) {
		allocateGroups(GroupBitmap.GROUP_0);
		m_c[0][0] = val;
	}
	/**
	 * sets this coordinates in 'arr'.
	 * @param gu bitwise or of the GROUPs and GRADEs which are present in 'arr'.
	 * @param arr compressed coordinates.
	 */
	public void set(int gu, float[] arr) {
		allocateGroups(gu);
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
	/**
	 * sets this coordinates in 'arr'. 
	 * 'arr' is kept, so changes to 'arr' will be reflected in the value of this multivector. Make sure 'arr' has length 6 and each subarray has the length of the respective group/grade
	 * @param arr coordinates.
	 */
	public void set(float[][] arr) {
		m_c = arr;
	}
	/**
	 * sets this to multivector value.
	 */
	public void set(Mv src) {
		allocateGroups(src.gu());
		if (m_c[0] != null) {
			C3ga.copy_1(m_c[0], src.m_c[0]);
		}
		if (m_c[1] != null) {
			C3ga.copy_5(m_c[1], src.m_c[1]);
		}
		if (m_c[2] != null) {
			C3ga.copy_10(m_c[2], src.m_c[2]);
		}
		if (m_c[3] != null) {
			C3ga.copy_10(m_c[3], src.m_c[3]);
		}
		if (m_c[4] != null) {
			C3ga.copy_5(m_c[4], src.m_c[4]);
		}
		if (m_c[5] != null) {
			C3ga.copy_1(m_c[5], src.m_c[5]);
		}
	}

	/**
	 * sets this to NormalizedPoint value.
	 */
	public void set(NormalizedPoint src) {
		allocateGroups(GroupBitmap.GROUP_1);
		float[] ptr;

		ptr = m_c[1];
		ptr[0] = 1.0f;
		ptr[1] = src.m_c[0];
		ptr[2] = src.m_c[1];
		ptr[3] = src.m_c[2];
		ptr[4] = src.m_c[3];
	}

	/**
	 * sets this to FlatPoint value.
	 */
	public void set(FlatPoint src) {
		allocateGroups(GroupBitmap.GROUP_2);
		float[] ptr;

		ptr = m_c[2];
		ptr[0] = ptr[1] = ptr[2] = ptr[3] = ptr[4] = ptr[5] = 0.0f;
		ptr[6] = src.m_c[3];
		ptr[7] = src.m_c[0];
		ptr[8] = src.m_c[1];
		ptr[9] = src.m_c[2];
	}

	/**
	 * sets this to Line value.
	 */
	public void set(Line src) {
		allocateGroups(GroupBitmap.GROUP_3);
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

	/**
	 * sets this to Plane value.
	 */
	public void set(Plane src) {
		allocateGroups(GroupBitmap.GROUP_4);
		float[] ptr;

		ptr = m_c[4];
		ptr[0] = 0.0f;
		ptr[1] = src.m_c[3];
		ptr[2] = src.m_c[2];
		ptr[3] = src.m_c[1];
		ptr[4] = src.m_c[0];
	}

	/**
	 * sets this to no_t value.
	 */
	public void set(no_t src) {
		allocateGroups(GroupBitmap.GROUP_1);
		float[] ptr;

		ptr = m_c[1];
		ptr[0] = 1.0f;
		ptr[1] = ptr[2] = ptr[3] = ptr[4] = 0.0f;
	}

	/**
	 * sets this to e1_t value.
	 */
	public void set(e1_t src) {
		allocateGroups(GroupBitmap.GROUP_1);
		float[] ptr;

		ptr = m_c[1];
		ptr[0] = ptr[2] = ptr[3] = ptr[4] = 0.0f;
		ptr[1] = 1.0f;
	}

	/**
	 * sets this to e2_t value.
	 */
	public void set(e2_t src) {
		allocateGroups(GroupBitmap.GROUP_1);
		float[] ptr;

		ptr = m_c[1];
		ptr[0] = ptr[1] = ptr[3] = ptr[4] = 0.0f;
		ptr[2] = 1.0f;
	}

	/**
	 * sets this to e3_t value.
	 */
	public void set(e3_t src) {
		allocateGroups(GroupBitmap.GROUP_1);
		float[] ptr;

		ptr = m_c[1];
		ptr[0] = ptr[1] = ptr[2] = ptr[4] = 0.0f;
		ptr[3] = 1.0f;
	}

	/**
	 * sets this to ni_t value.
	 */
	public void set(ni_t src) {
		allocateGroups(GroupBitmap.GROUP_1);
		float[] ptr;

		ptr = m_c[1];
		ptr[0] = ptr[1] = ptr[2] = ptr[3] = 0.0f;
		ptr[4] = 1.0f;
	}
	/**
	 * Returns the scalar coordinate of this Mv
	 */
	public final float get_scalar()  {
		return (m_c[0] == null) ? 0.0f: m_c[0][0];
	}
	/**
	 * Returns the no coordinate of this Mv
	 */
	public final float get_no()  {
		return (m_c[1] == null) ? 0.0f: m_c[1][0];
	}
	/**
	 * Returns the e1 coordinate of this Mv
	 */
	public final float get_e1()  {
		return (m_c[1] == null) ? 0.0f: m_c[1][1];
	}
	/**
	 * Returns the e2 coordinate of this Mv
	 */
	public final float get_e2()  {
		return (m_c[1] == null) ? 0.0f: m_c[1][2];
	}
	/**
	 * Returns the e3 coordinate of this Mv
	 */
	public final float get_e3()  {
		return (m_c[1] == null) ? 0.0f: m_c[1][3];
	}
	/**
	 * Returns the ni coordinate of this Mv
	 */
	public final float get_ni()  {
		return (m_c[1] == null) ? 0.0f: m_c[1][4];
	}
	/**
	 * Returns the no_e1 coordinate of this Mv
	 */
	public final float get_no_e1()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][0];
	}
	/**
	 * Returns the no_e2 coordinate of this Mv
	 */
	public final float get_no_e2()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][1];
	}
	/**
	 * Returns the e1_e2 coordinate of this Mv
	 */
	public final float get_e1_e2()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][2];
	}
	/**
	 * Returns the no_e3 coordinate of this Mv
	 */
	public final float get_no_e3()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][3];
	}
	/**
	 * Returns the e1_e3 coordinate of this Mv
	 */
	public final float get_e1_e3()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][4];
	}
	/**
	 * Returns the e2_e3 coordinate of this Mv
	 */
	public final float get_e2_e3()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][5];
	}
	/**
	 * Returns the no_ni coordinate of this Mv
	 */
	public final float get_no_ni()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][6];
	}
	/**
	 * Returns the e1_ni coordinate of this Mv
	 */
	public final float get_e1_ni()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][7];
	}
	/**
	 * Returns the e2_ni coordinate of this Mv
	 */
	public final float get_e2_ni()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][8];
	}
	/**
	 * Returns the e3_ni coordinate of this Mv
	 */
	public final float get_e3_ni()  {
		return (m_c[2] == null) ? 0.0f: m_c[2][9];
	}
	/**
	 * Returns the no_e1_e2 coordinate of this Mv
	 */
	public final float get_no_e1_e2()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][0];
	}
	/**
	 * Returns the no_e1_e3 coordinate of this Mv
	 */
	public final float get_no_e1_e3()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][1];
	}
	/**
	 * Returns the no_e2_e3 coordinate of this Mv
	 */
	public final float get_no_e2_e3()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][2];
	}
	/**
	 * Returns the e1_e2_e3 coordinate of this Mv
	 */
	public final float get_e1_e2_e3()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][3];
	}
	/**
	 * Returns the no_e1_ni coordinate of this Mv
	 */
	public final float get_no_e1_ni()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][4];
	}
	/**
	 * Returns the no_e2_ni coordinate of this Mv
	 */
	public final float get_no_e2_ni()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][5];
	}
	/**
	 * Returns the e1_e2_ni coordinate of this Mv
	 */
	public final float get_e1_e2_ni()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][6];
	}
	/**
	 * Returns the no_e3_ni coordinate of this Mv
	 */
	public final float get_no_e3_ni()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][7];
	}
	/**
	 * Returns the e1_e3_ni coordinate of this Mv
	 */
	public final float get_e1_e3_ni()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][8];
	}
	/**
	 * Returns the e2_e3_ni coordinate of this Mv
	 */
	public final float get_e2_e3_ni()  {
		return (m_c[3] == null) ? 0.0f: m_c[3][9];
	}
	/**
	 * Returns the no_e1_e2_e3 coordinate of this Mv
	 */
	public final float get_no_e1_e2_e3()  {
		return (m_c[4] == null) ? 0.0f: m_c[4][0];
	}
	/**
	 * Returns the no_e1_e2_ni coordinate of this Mv
	 */
	public final float get_no_e1_e2_ni()  {
		return (m_c[4] == null) ? 0.0f: m_c[4][1];
	}
	/**
	 * Returns the no_e1_e3_ni coordinate of this Mv
	 */
	public final float get_no_e1_e3_ni()  {
		return (m_c[4] == null) ? 0.0f: m_c[4][2];
	}
	/**
	 * Returns the no_e2_e3_ni coordinate of this Mv
	 */
	public final float get_no_e2_e3_ni()  {
		return (m_c[4] == null) ? 0.0f: m_c[4][3];
	}
	/**
	 * Returns the e1_e2_e3_ni coordinate of this Mv
	 */
	public final float get_e1_e2_e3_ni()  {
		return (m_c[4] == null) ? 0.0f: m_c[4][4];
	}
	/**
	 * Returns the no_e1_e2_e3_ni coordinate of this Mv
	 */
	public final float get_no_e1_e2_e3_ni()  {
		return (m_c[5] == null) ? 0.0f: m_c[5][0];
	}

	/** 
	 * Reserves memory for the groups specified by 'gu'.
	 * Keeps old memory (and values) when possible. 
	 */
	private final void allocateGroups(final int gu) {
		for (int i = 0; (1 << i) <= gu; i++) {
			if (((1 << i) & gu) != 0) {
				if (m_c[i] == null)
					m_c[i] = new float[C3ga.MvSize[1 << i]];
			}
			else m_c[i] = null;
		}		
	}

	/**
	 *  Reserves memory for coordinate GROUP_0.
	 *  If the group is already present, nothing changes.
	 *  If the group is not present, memory is allocated for the new group,
	 *  and the coordinates for the group are set to zero.
	 */
	private final void reserveGroup_0() {
		if (m_c[0] == null) {
			m_c[0] = new float[1];
		}
	}
	/**
	 *  Reserves memory for coordinate GROUP_1.
	 *  If the group is already present, nothing changes.
	 *  If the group is not present, memory is allocated for the new group,
	 *  and the coordinates for the group are set to zero.
	 */
	private final void reserveGroup_1() {
		if (m_c[1] == null) {
			m_c[1] = new float[5];
		}
	}
	/**
	 *  Reserves memory for coordinate GROUP_2.
	 *  If the group is already present, nothing changes.
	 *  If the group is not present, memory is allocated for the new group,
	 *  and the coordinates for the group are set to zero.
	 */
	private final void reserveGroup_2() {
		if (m_c[2] == null) {
			m_c[2] = new float[10];
		}
	}
	/**
	 *  Reserves memory for coordinate GROUP_3.
	 *  If the group is already present, nothing changes.
	 *  If the group is not present, memory is allocated for the new group,
	 *  and the coordinates for the group are set to zero.
	 */
	private final void reserveGroup_3() {
		if (m_c[3] == null) {
			m_c[3] = new float[10];
		}
	}
	/**
	 *  Reserves memory for coordinate GROUP_4.
	 *  If the group is already present, nothing changes.
	 *  If the group is not present, memory is allocated for the new group,
	 *  and the coordinates for the group are set to zero.
	 */
	private final void reserveGroup_4() {
		if (m_c[4] == null) {
			m_c[4] = new float[5];
		}
	}
	/**
	 *  Reserves memory for coordinate GROUP_5.
	 *  If the group is already present, nothing changes.
	 *  If the group is not present, memory is allocated for the new group,
	 *  and the coordinates for the group are set to zero.
	 */
	private final void reserveGroup_5() {
		if (m_c[5] == null) {
			m_c[5] = new float[1];
		}
	}
	/// Sets the scalar coordinate of this Mv.
	public final void set_scalar(float val)  {
		reserveGroup_0();
		m_c[0][0] =  val;
	}
	/// Sets the no coordinate of this Mv.
	public final void set_no(float val)  {
		reserveGroup_1();
		m_c[1][0] =  val;
	}
	/// Sets the e1 coordinate of this Mv.
	public final void set_e1(float val)  {
		reserveGroup_1();
		m_c[1][1] =  val;
	}
	/// Sets the e2 coordinate of this Mv.
	public final void set_e2(float val)  {
		reserveGroup_1();
		m_c[1][2] =  val;
	}
	/// Sets the e3 coordinate of this Mv.
	public final void set_e3(float val)  {
		reserveGroup_1();
		m_c[1][3] =  val;
	}
	/// Sets the ni coordinate of this Mv.
	public final void set_ni(float val)  {
		reserveGroup_1();
		m_c[1][4] =  val;
	}
	/// Sets the no_e1 coordinate of this Mv.
	public final void set_no_e1(float val)  {
		reserveGroup_2();
		m_c[2][0] =  val;
	}
	/// Sets the no_e2 coordinate of this Mv.
	public final void set_no_e2(float val)  {
		reserveGroup_2();
		m_c[2][1] =  val;
	}
	/// Sets the e1_e2 coordinate of this Mv.
	public final void set_e1_e2(float val)  {
		reserveGroup_2();
		m_c[2][2] =  val;
	}
	/// Sets the no_e3 coordinate of this Mv.
	public final void set_no_e3(float val)  {
		reserveGroup_2();
		m_c[2][3] =  val;
	}
	/// Sets the e1_e3 coordinate of this Mv.
	public final void set_e1_e3(float val)  {
		reserveGroup_2();
		m_c[2][4] =  val;
	}
	/// Sets the e2_e3 coordinate of this Mv.
	public final void set_e2_e3(float val)  {
		reserveGroup_2();
		m_c[2][5] =  val;
	}
	/// Sets the no_ni coordinate of this Mv.
	public final void set_no_ni(float val)  {
		reserveGroup_2();
		m_c[2][6] =  val;
	}
	/// Sets the e1_ni coordinate of this Mv.
	public final void set_e1_ni(float val)  {
		reserveGroup_2();
		m_c[2][7] =  val;
	}
	/// Sets the e2_ni coordinate of this Mv.
	public final void set_e2_ni(float val)  {
		reserveGroup_2();
		m_c[2][8] =  val;
	}
	/// Sets the e3_ni coordinate of this Mv.
	public final void set_e3_ni(float val)  {
		reserveGroup_2();
		m_c[2][9] =  val;
	}
	/// Sets the no_e1_e2 coordinate of this Mv.
	public final void set_no_e1_e2(float val)  {
		reserveGroup_3();
		m_c[3][0] =  val;
	}
	/// Sets the no_e1_e3 coordinate of this Mv.
	public final void set_no_e1_e3(float val)  {
		reserveGroup_3();
		m_c[3][1] =  val;
	}
	/// Sets the no_e2_e3 coordinate of this Mv.
	public final void set_no_e2_e3(float val)  {
		reserveGroup_3();
		m_c[3][2] =  val;
	}
	/// Sets the e1_e2_e3 coordinate of this Mv.
	public final void set_e1_e2_e3(float val)  {
		reserveGroup_3();
		m_c[3][3] =  val;
	}
	/// Sets the no_e1_ni coordinate of this Mv.
	public final void set_no_e1_ni(float val)  {
		reserveGroup_3();
		m_c[3][4] =  val;
	}
	/// Sets the no_e2_ni coordinate of this Mv.
	public final void set_no_e2_ni(float val)  {
		reserveGroup_3();
		m_c[3][5] =  val;
	}
	/// Sets the e1_e2_ni coordinate of this Mv.
	public final void set_e1_e2_ni(float val)  {
		reserveGroup_3();
		m_c[3][6] =  val;
	}
	/// Sets the no_e3_ni coordinate of this Mv.
	public final void set_no_e3_ni(float val)  {
		reserveGroup_3();
		m_c[3][7] =  val;
	}
	/// Sets the e1_e3_ni coordinate of this Mv.
	public final void set_e1_e3_ni(float val)  {
		reserveGroup_3();
		m_c[3][8] =  val;
	}
	/// Sets the e2_e3_ni coordinate of this Mv.
	public final void set_e2_e3_ni(float val)  {
		reserveGroup_3();
		m_c[3][9] =  val;
	}
	/// Sets the no_e1_e2_e3 coordinate of this Mv.
	public final void set_no_e1_e2_e3(float val)  {
		reserveGroup_4();
		m_c[4][0] =  val;
	}
	/// Sets the no_e1_e2_ni coordinate of this Mv.
	public final void set_no_e1_e2_ni(float val)  {
		reserveGroup_4();
		m_c[4][1] =  val;
	}
	/// Sets the no_e1_e3_ni coordinate of this Mv.
	public final void set_no_e1_e3_ni(float val)  {
		reserveGroup_4();
		m_c[4][2] =  val;
	}
	/// Sets the no_e2_e3_ni coordinate of this Mv.
	public final void set_no_e2_e3_ni(float val)  {
		reserveGroup_4();
		m_c[4][3] =  val;
	}
	/// Sets the e1_e2_e3_ni coordinate of this Mv.
	public final void set_e1_e2_e3_ni(float val)  {
		reserveGroup_4();
		m_c[4][4] =  val;
	}
	/// Sets the no_e1_e2_e3_ni coordinate of this Mv.
	public final void set_no_e1_e2_e3_ni(float val)  {
		reserveGroup_5();
		m_c[5][0] =  val;
	}

	/** returns the absolute largest coordinate.*/
	public float largestCoordinate() {
		float maxValue = 0.0f, C;
		for (int g = 0; g < m_c.length; g++) {
			if (m_c[g] != null) {
				float[] Cg = m_c[g];
				for (int b = 0; b < Cg.length; b++) {
					C = Math.abs(Cg[b]);
					if (C > maxValue) {
						maxValue = C;
					}
				}
			}
		}
		return maxValue;
	}
	
	/** returns the absolute largest coordinate (entry [0]), and the corresponding basis blade bitmap (entry [1])  */
	public float[] largestBasisBlade() {
		float maxC = -1.0f, C;

		int idx = 0; // global index into coordinates (run from 0 to 32).
		int bm; // bitmap of basis blade
		bm = 0;
		
		for (int g = 0; g < m_c.length; g++) {
			if (m_c[g] != null) {
				float[] Cg = m_c[g];
				for (int b = 0; b < m_c[g].length; b++) {
					C = Math.abs(Cg[b]);
					if (C > maxC) {
						maxC = C;
						bm = C3ga.BasisElementBitmapByIndex[idx];
					}
					idx++;
				}
			
			}
			else idx += C3ga.GroupSize[g];
		}

		return new float[]{maxC, (float)bm};
	} // end of largestBasisBlade()
	
	/**
	 * Releases memory for (near-)zero groups/grades.
	 * This also speeds up subsequent operations, because those do not have to process the released groups/grades anymore.
	 * @param eps A positive threshold value.
	 * Coordinates which are smaller than epsilon are considered to be zero.
	 */
	public final void compress(float eps)  {
		if ((m_c[0] != null) && C3ga.zeroGroup_0(m_c[0], eps))
			m_c[0] = null;
		if ((m_c[1] != null) && C3ga.zeroGroup_1(m_c[1], eps))
			m_c[1] = null;
		if ((m_c[2] != null) && C3ga.zeroGroup_2(m_c[2], eps))
			m_c[2] = null;
		if ((m_c[3] != null) && C3ga.zeroGroup_3(m_c[3], eps))
			m_c[3] = null;
		if ((m_c[4] != null) && C3ga.zeroGroup_4(m_c[4], eps))
			m_c[4] = null;
		if ((m_c[5] != null) && C3ga.zeroGroup_5(m_c[5], eps))
			m_c[5] = null;
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

	public final Mv to_Mv() {
		return this;
	}

	/**
	 * shortcut to C3ga.add(this, b)
	 */
	public final Mv add(final Mv_if b) {
		return C3ga.add(this, b);
	}

	/**
	 * shortcut to C3ga.subtract(this, b)
	 */
	public final Mv subtract(final Mv_if b) {
		return C3ga.subtract(this, b);
	}

	/**
	 * shortcut to C3ga.extractGrade(this, groupBitmap)
	 */
	public final Mv extractGrade(final int groupBitmap) {
		return C3ga.extractGrade(this, groupBitmap);
	}

	/**
	 * shortcut to C3ga.negate(this)
	 */
	public final Mv negate() {
		return C3ga.negate(this);
	}

	/**
	 * shortcut to C3ga.reverse(this)
	 */
	public final Mv reverse() {
		return C3ga.reverse(this);
	}

	/**
	 * shortcut to C3ga.gradeInvolution(this)
	 */
	public final Mv gradeInvolution() {
		return C3ga.gradeInvolution(this);
	}

	/**
	 * shortcut to C3ga.cliffordConjugate(this)
	 */
	public final Mv cliffordConjugate() {
		return C3ga.cliffordConjugate(this);
	}

	/**
	 * shortcut to C3ga.gp(this, b)
	 */
	public final Mv gp(final Mv_if b) {
		return C3ga.gp(this, b);
	}

	/**
	 * shortcut to C3ga.op(this, b)
	 */
	public final Mv op(final Mv_if b) {
		return C3ga.op(this, b);
	}

	/**
	 * shortcut to C3ga.sp(this, b)
	 */
	public final float sp(final Mv_if b) {
		return C3ga.sp(this, b);
	}

	/**
	 * shortcut to C3ga.mhip(this, b)
	 */
	public final Mv mhip(final Mv_if b) {
		return C3ga.mhip(this, b);
	}

	/**
	 * shortcut to C3ga.lc(this, b)
	 */
	public final Mv lc(final Mv_if b) {
		return C3ga.lc(this, b);
	}

	/**
	 * shortcut to C3ga.norm(this)
	 */
	public final float norm() {
		return C3ga.norm(this);
	}

	/**
	 * shortcut to C3ga.unit(this)
	 */
	public final Mv unit() {
		return C3ga.unit(this);
	}

	/**
	 * shortcut to C3ga.applyVersor(this, b)
	 */
	public final Mv applyVersor(final Mv_if b) {
		return C3ga.applyVersor(this, b);
	}

	/**
	 * shortcut to C3ga.applyUnitVersor(this, b)
	 */
	public final Mv applyUnitVersor(final Mv_if b) {
		return C3ga.applyUnitVersor(this, b);
	}

	/**
	 * shortcut to C3ga.versorInverse(this)
	 */
	public final Mv versorInverse() {
		return C3ga.versorInverse(this);
	}

	/**
	 * shortcut to C3ga.equals(this, b, c)
	 */
	public final boolean equals(final Mv_if b, final float c) {
		return C3ga.equals(this, b, c);
	}

	/**
	 * shortcut to C3ga.zero(this, b)
	 */
	public final boolean zero(final float b) {
		return C3ga.zero(this, b);
	}

	/**
	 * shortcut to C3ga.dual(this)
	 */
	public final Mv dual() {
		return C3ga.dual(this);
	}

	/**
	 * shortcut to C3ga.undual(this)
	 */
	public final Mv undual() {
		return C3ga.undual(this);
	}

	/**
	 * shortcut to C3ga.exp(this)
	 */
	public final Mv exp() {
		return C3ga.exp(this);
	}

	/**
	 * shortcut to C3ga.gp(this, b)
	 */
	public final Mv gp(final float b) {
		return C3ga.gp(this, b);
	}

	/**
	 * shortcut to C3ga.norm2(this)
	 */
	public final float norm2() {
		return C3ga.norm2(this);
	}

	/**
	 * shortcut to C3ga.sas(this, b, c)
	 */
	public final Mv sas(final float b, final float c) {
		return C3ga.sas(this, b, c);
	}
} // end of class Mv
