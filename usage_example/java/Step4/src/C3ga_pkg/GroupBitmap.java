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
public interface GroupBitmap {
	public static final int GROUP_0  = 1; // 1
	public static final int GROUP_1  = 2; // no, e1, e2, e3, ni
	public static final int GROUP_2  = 4; // no^e1, no^e2, e1^e2, no^e3, e1^e3, e2^e3, no^ni, e1^ni, e2^ni, e3^ni
	public static final int GROUP_3  = 8; // no^e1^e2, no^e1^e3, no^e2^e3, e1^e2^e3, no^e1^ni, no^e2^ni, e1^e2^ni, no^e3^ni, e1^e3^ni, e2^e3^ni
	public static final int GROUP_4  = 16; // no^e1^e2^e3, no^e1^e2^ni, no^e1^e3^ni, no^e2^e3^ni, e1^e2^e3^ni
	public static final int GROUP_5  = 32; // no^e1^e2^e3^ni

	public static final int ALL_GROUPS = 63; // all groups

	public static final int GRADE_0 = 1;
	public static final int GRADE_1 = 2;
	public static final int GRADE_2 = 4;
	public static final int GRADE_3 = 8;
	public static final int GRADE_4 = 16;
	public static final int GRADE_5 = 32;

	public static final int ALL_GRADES = 63; // all grades

}
