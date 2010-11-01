/*
Copyright (C) 2008 Some Random Person
*/
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
using System.Collections.Generic;
using System.Text;
namespace c3ga_ns {


class ReportUsage
{
    protected static Dictionary<string, ReportUsage> s_reportUsage = new Dictionary<string, ReportUsage>();

    /// <summary>
    ///  Adds a report to the global report usage hash table.
    /// </summary>
    public static void MergeReport(ReportUsage RU)
    {
        if (s_reportUsage.ContainsKey(RU.GetReportString())) {
            s_reportUsage[RU.GetReportString()].IncrementCount();
        }
        else {
             s_reportUsage.Add(RU.GetReportString(), RU);
        }
    }

    /// prints out all reports, sorted by m_str
    public static string GetReport(bool includeCount)
    {
        StringBuilder sb = new StringBuilder();
        SortedDictionary<string, ReportUsage> sortedReportUsage = new SortedDictionary<string, ReportUsage>(s_reportUsage);
        foreach (KeyValuePair<string, ReportUsage> kvp in sortedReportUsage ) {
            ReportUsage ru = kvp.Value;
            sb.Append(ru.GetReportString());
            if (includeCount)
            {
                sb.AppendFormat("  <!-- used %d time%s -->", ru.GetReportCount(), (ru.GetReportCount() == 1) ? "" : "s");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public ReportUsage(string xmlStr)
    {
        m_str = xmlStr;
        m_count = 1;
    }

    public string GetReportString()
    {
        return m_str;
    }

    public int GetReportCount()
    {
        return m_count;
    }

    protected void IncrementCount()
    {
        m_count++;
    }

    /// <summary>
    /// XML string describing the report.
    /// </summary>
    protected string m_str;

    /// <summary>
    /// Number of times report has been reported.
    /// </summary>
    int m_count;
}

} // end of namespace c3ga_ns
