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
public enum SmvType {
	C3GA_MV(0, "Mv"),
	C3GA_FLOAT(1, "float"),
	C3GA_NORMALIZEDPOINT(2, "NormalizedPoint"),
	C3GA_FLATPOINT(3, "FlatPoint"),
	C3GA_LINE(4, "Line"),
	C3GA_PLANE(5, "Plane"),
	C3GA_NO_T(6, "no_t"),
	C3GA_E1_T(7, "e1_t"),
	C3GA_E2_T(8, "e2_t"),
	C3GA_E3_T(9, "e3_t"),
	C3GA_NI_T(10, "ni_t"),
	C3GA_INVALID(11, "invalid"),
	C3GA_NONE(-1, "none");

	private final int id;
    private final String label;

    SmvType(final int id, final String label) {
        this.id = id;
        this.label = label;
    }
    
    public final int getId() {
        return id;
    }
 
    public final String toString() {
        return label;
    }
}
