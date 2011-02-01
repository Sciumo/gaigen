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
import java.util.Hashtable;
import java.util.TreeSet;
import java.util.Map;


public class ReportUsage implements Comparable<ReportUsage>
{
    protected static Hashtable<String, ReportUsage> s_reportUsage = new Hashtable<String, ReportUsage>();

    /// <summary>
    ///  Adds a report to the global report usage hash table.
    /// </summary>
    public static void mergeReport(ReportUsage RU)
    {
		// Note: s_reportUsage (Hashtable<>) is thread-safe, so no locking required.
        if (s_reportUsage.containsKey(RU.getReportString())) {
            s_reportUsage.get(RU.getReportString()).incrementCount();
        }
        else {
             s_reportUsage.put(RU.getReportString(), RU);
        }
    }

    /// prints out all reports, sorted by m_str
    public static String getReport(boolean includeCount)
    {
        StringBuilder sb = new StringBuilder();
        
        TreeSet<ReportUsage> sortedSet = new TreeSet<ReportUsage>();
        for (Map.Entry<String, ReportUsage> kvp : s_reportUsage.entrySet()) {
        	sortedSet.add(kvp.getValue());
        }
        
        for (ReportUsage ru : sortedSet) {
            sb.append(ru.getReportString());
            if (includeCount)
            {
                sb.append("  <!-- used " + ru.getReportCount() + " time" + ((ru.getReportCount() == 1) ? "" : "s") + " -->");
            }
            sb.append("\n");
        }
        
        if (s_reportUsage.isEmpty()) {
				sb.append("  <!-- no general multivector function usage -->");
		}

        return sb.toString();
    }

    public ReportUsage(String xmlStr)
    {
        m_str = xmlStr;
        m_count = 1;
    }

	@Override
	public int compareTo(ReportUsage arg0) {
		
		return m_str.compareTo(((ReportUsage)arg0).getReportString());
	}
	
    public String getReportString()
    {
        return m_str;
    }

    public int getReportCount()
    {
        return m_count;
    }

    protected void incrementCount()
    {
        m_count++;
    }

    /// <summary>
    /// XML string describing the report.
    /// </summary>
    protected String m_str;

    /// <summary>
    /// Number of times report has been reported.
    /// </summary>
    protected int m_count;
}

