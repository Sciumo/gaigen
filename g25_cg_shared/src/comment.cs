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


namespace G25.CG.Shared
{

    /// <summary>
    /// This class is a container for inline documention comments, and can also write those
    /// comment in various formats.
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Constructs a new empty comment.
        /// </summary>
        public Comment()
        {
            SummaryComment = "";
            ReturnComment = "";
        }

        /// <summary>
        /// Constructs a new comment with only the summary comment specified.
        /// Use the member functions to add return and param comments.
        /// </summary>
        /// <param name="summaryComment">The summary comment.</param>
        public Comment(string summaryComment)
        {
            SummaryComment = summaryComment;
            ReturnComment = "";
        }


        /// <summary>
        /// Sets the summary comment.
        /// </summary>
        /// <returns>This Comment.</returns>
        public Comment SetSummaryComment(string comment)
        {
            SummaryComment = comment;
            return this;
        }

        /// <summary>
        /// Adds text to the summary comment.
        /// </summary>
        /// <returns>This Comment.</returns>
        public Comment AddSummaryComment(string comment)
        {
            SummaryComment = SummaryComment + comment;
            return this;
        }

        /// <summary>
        /// Sets the return comment.
        /// </summary>
        /// <returns>This Comment.</returns>
        public Comment SetReturnComment(string comment)
        {
            ReturnComment = comment;
            return this;
        }

        /// <summary>
        /// Adds text to the return comment.
        /// </summary>
        /// <returns>This Comment.</returns>
        public Comment AddReturnComment(string comment)
        {
            ReturnComment = ReturnComment + comment;
            return this;
        }

        /// <summary>
        /// Sets a parameter comment.
        /// </summary>
        /// <returns>This Comment.</returns>
        public Comment SetParamComment(string paramName, string comment)
        {
            m_paramComments[paramName] = comment;
            return this;
        }

        /// <summary>
        /// Adds text to a parameter comment.
        /// </summary>
        /// <returns>This Comment.</returns>
        public Comment AddParamComment(string paramName, string comment)
        {
            if (!m_paramComments.ContainsKey(paramName))
                m_paramComments[paramName] = comment;
            else m_paramComments[paramName] = m_paramComments[paramName] + comment;
            return this;
        }


        /// <summary>
        /// Writes a multiline comment with the correct decoration in front of it.
        /// Opening and closing decorations are not included.
        /// </summary>
        /// <param name="SB"></param>
        /// <param name="S">Used for output language.</param>
        /// <param name="nbTabs">How many tabs to place in front of the comment.</param>
        /// <param name="comment">The comment</param>
        public static void WriteMultilineComment(StringBuilder SB, Specification S, int nbTabs, string comment)
        {
            // how to start each line:
            string lineStart = new string('\t', nbTabs);
            if ((S.OutputJava()) || (S.OutputC()))
                lineStart = lineStart + " * ";
            else lineStart = lineStart + "/// ";

            // split comment on carriage returns, and get rid of '\r'
            string[] splitStr = comment.Replace("\r", "").Split('\n');

            // output each line of comment:
            bool firstLine = true;
            foreach (string lineStr in splitStr)
            {
                if (!firstLine) SB.Append(lineStart);
                firstLine = false;
                SB.AppendLine(lineStr);
            }
        }


        /// <summary>
        /// Writes a function comment according to the output language.
        /// </summary>
        /// <param name="SB">Where the output goes.</param>
        /// <param name="S">Used for output language.</param>
        /// <param name="nbTabs">Number of tabs to put in front of output comment.</param>
        public void Write(StringBuilder SB, Specification S, int nbTabs)
        {
            // open comment, summary comment
            SB.Append('\t', nbTabs);
            switch (S.m_outputLanguage)
            {
                case OUTPUT_LANGUAGE.CPP:
                    SB.Append("/// ");
                    break;
                case OUTPUT_LANGUAGE.C:
                case OUTPUT_LANGUAGE.JAVA:
                    SB.AppendLine("/**");
                    SB.Append('\t', nbTabs);
                    SB.Append(" * ");
                    break;
                case OUTPUT_LANGUAGE.CSHARP:
                    SB.Append("/// <summary>");
                    break;
            }

            WriteMultilineComment(SB, S, nbTabs, SummaryComment);

            if (S.OutputCSharp())
            {
                SB.Append('\t', nbTabs);
                SB.AppendLine("/// </summary>");
            }

            // parameter comments
            foreach (KeyValuePair<string, string> kvp in m_paramComments)
            {
                string paramName = kvp.Key;
                string comment = kvp.Value;
                switch (S.m_outputLanguage)
                {
                    case OUTPUT_LANGUAGE.C:
                        SB.Append(" *  \\param " + paramName + " ");
                        break;
                    case OUTPUT_LANGUAGE.CPP:
                        SB.Append("/// \\param " + paramName + " ");
                        break;
                    case OUTPUT_LANGUAGE.JAVA:
                        SB.Append(" * @param " + paramName + " ");
                        break;
                    case OUTPUT_LANGUAGE.CSHARP:
                        SB.Append("/// <param name=\"" + paramName + "\">");
                        break;
                }
                WriteMultilineComment(SB, S, nbTabs, comment);

                if (S.OutputCSharp())
                {
                    SB.Append('\t', nbTabs);
                    SB.AppendLine("/// </param>");
                }
            }

            // return comment
            if (ReturnComment.Length > 0)
            {
                SB.Append('\t', nbTabs);
                switch (S.m_outputLanguage)
                {
                    case OUTPUT_LANGUAGE.C:
                        SB.Append(" * \\return ");
                        break;
                    case OUTPUT_LANGUAGE.CPP:
                        SB.Append("/// \\return ");
                        break;
                    case OUTPUT_LANGUAGE.JAVA:
                        SB.Append(" * @return ");
                        break;
                    case OUTPUT_LANGUAGE.CSHARP:
                        SB.Append("/// <returns>");
                        break;
                }
                WriteMultilineComment(SB, S, nbTabs, ReturnComment);
                if (S.OutputCSharp())
                {
                    SB.Append('\t', nbTabs);
                    SB.AppendLine("/// </returns>");
                }
            }

            // end of comment
            if ((S.OutputJava()) || (S.OutputC()))
            {
                SB.Append('\t', nbTabs);
                SB.AppendLine(" */");
            }
        }

        /// <summary>
        /// Returns the function comment converted to string, according to the output language.
        /// </summary>
        /// <param name="S">Used for output language.</param>
        /// <param name="nbTabs">Number of tabs to put in front of output comment.</param>
        public string ToString(Specification S, int nbTabs)
        {
            StringBuilder SB = new StringBuilder();
            Write(SB, S, nbTabs);
            return SB.ToString();
        }


        public string SummaryComment { get; set; }
        public string ReturnComment { get; set; }

        private Dictionary<string, string> m_paramComments = new Dictionary<string,string>();

    } // end of class Comment
} // end of namespace G25.CG.Shared
