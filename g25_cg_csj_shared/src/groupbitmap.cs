// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

// Copyright 2008-2010, Daniel Fontijne, University of Amsterdam -- fontijne@science.uva.nl

using System;
using System.Collections.Generic;
using System.Text;


namespace G25.CG.CSJ
{
    public class GroupBitmap
    {
        public static string GetGroupBitmapName(int groupIdx)
        {
            return "GROUP_" + groupIdx;
        }

        public static string GetGradeBitmapName(int gradeIdx)
        {
            return "GRADE_" + gradeIdx;
        }

        public static string GetGroupBitmapCode(int groupIdx)
        {
            return "GroupBitmap." + GetGroupBitmapName(groupIdx);
        }

        public static string GetGradeBitmapCode(int gradeIdx)
        {
            return "GroupBitmap." + GetGroupBitmapName(gradeIdx);
        }

        public static string GetBelowGroupBitmapCode(int groupIdx)
        {
            if (groupIdx == 0) return "0";

            StringBuilder SB = new StringBuilder();
            for (int i = 0; i < groupIdx; i++)
            {
                if (i > 0) SB.Append("|");
                SB.Append("GroupBitmap." + GetGroupBitmapName(i));
            }
            if (groupIdx > 1) return "(" + SB.ToString() + ")";
            else return SB.ToString();
        }

        public static string GetBelowGradeBitmapCode(int gradeIdx)
        {
            if (gradeIdx == 0) return "0";

            StringBuilder SB = new StringBuilder();
            for (int i = 0; i < gradeIdx; i++)
            {
                if (i > 0) SB.Append("|");
                SB.Append("GroupBitmap." + GetGradeBitmapName(i));
            }
            if (gradeIdx > 1) return "(" + SB.ToString() + ")";
            else return SB.ToString();
        }

    } // end of class GMV
} // end of namespace G25.CG.CSJ
