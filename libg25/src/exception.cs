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
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace G25
{


    /// <summary>
    /// This class should be thrown by code generation plugins when an error due to user input occurs.
    /// At least a error message (intended for the user) should be specified.
    /// 
    /// Optionally, a reproduction of the XML which caused the error can be included,
    /// or the filename, line and column.
    /// 
    /// Do not use this exception for system errors which the user cannot fix by
    /// changing the input file.
    /// 
    /// Gaigen 2.5 is setup such that the exception will be caught as the highest level,
    /// but it is preferable for the code generators to catch and collect them. In that 
    /// way, multiple errors can be presented in one run, instead of just one error per run.
    /// 
    /// To implement this, the current code generator plugins catch G25.UserExceptions and
    /// collected them in <c>G25.CG.Shared.CGdata</c>). 
    /// When code generation is finished, the errors are reported to the user, using 
    /// <c>G25.CG.Shared.CGdata.PrintErrors()</c>.
    /// </summary>
    public class UserException : System.Exception
    {
        /// <summary>
        /// Initializes a G25.UserException.
        /// </summary>
        /// <param name="message">Error message (must always be filled in)</param>
        public UserException(String message)
            : base(message)
        {
            m_message = message;
            m_XMLerrorSource = "";
            m_filename = "";
            m_line = -1;
            m_column = -1;
        }

        /// <summary>
        /// Initializes a G25.UserException.
        /// </summary>
        /// <param name="message">Error message (must always be filled in)</param>
        /// <param name="XMLerrorSource">XML source of error, if available ("" otherwise).</param>
        public UserException(String message, String XMLerrorSource)
            : base(message)
        {
            m_message = message;
            m_XMLerrorSource = XMLerrorSource;
            m_filename = "";
            m_line = -1;
            m_column = -1;
        }

        /// <summary>
        /// Initializes a G25.UserException.
        /// </summary>
        /// <param name="message">Error message (must always be filled in)</param>
        /// <param name="filename">Filename source of error, if available ("" otherwise).</param>
        /// <param name="line">Line in filename, if available (-1 otherwise). The first line is line 0. Error reporting adds 1 for user presentation.</param>
        /// <param name="column">Column in filename, if available (-1 otherwise). The first column is line 0. Error reporting adds 1 for user presentation.</param>
        public UserException(String message, String filename, int line, int column)
            : base(message)
        {
            m_message = message;
            m_XMLerrorSource = "";
            m_filename = filename;
            m_line = line;
            m_column = column;
        }

        /// <summary>
        /// Initializes a G25.UserException.
        /// </summary>
        /// <param name="message">Error message (must always be filled in)</param>
        /// <param name="XMLerrorSource">XML source of error, if available ("" otherwise).</param>
        /// <param name="filename">Filename source of error, if available ("" otherwise).</param>
        /// <param name="line">Line in filename, if available (-1 otherwise).</param>
        /// <param name="column">Column in filename, if available (-1 otherwise).</param>
        public UserException(String message, String XMLerrorSource, String filename, int line, int column)
            : base(message)
        {
            m_message = message;
            m_XMLerrorSource = XMLerrorSource;
            m_filename = filename;
            m_line = line;
            m_column = column;
        }

        /// <summary>Includes carriage returns.</summary>
        /// <returns>Human readable error (possibly multi-line) string, composed of all the available information.</returns>
        public String GetErrorReport()
        {
            StringBuilder SB = new StringBuilder();

            // filename, if available:
            if (m_filename.Length > 0)
            {
                SB.Append(m_filename);
                if (m_line >= 0)
                {
                    SB.Append("(" + (m_line+1));
                    if (m_column >= 0)
                        SB.Append("," + (m_column + 1));
                    else SB.Append(",0");
                    SB.Append(")");
                }
                SB.Append(":");
            }

            // the error:
            SB.AppendLine(m_message);

            // XML error, if available
            if (m_XMLerrorSource.Length > 0)
            {
                SB.Append("XML source of error: ");
                SB.AppendLine(m_XMLerrorSource);
            }

            return SB.ToString();
        }


        /// <summary>
        /// If known, the XML which caused the problem ("" if unknown).
        /// </summary>
        public readonly String m_message;

        /// <summary>
        /// If known, the XML which caused the problem ("" if unknown).
        /// </summary>
        public readonly String m_XMLerrorSource;

        /// <summary>
        /// Filename which caused the problem ("" if unknown).
        /// </summary>
        public readonly String m_filename;

        /// <summary>
        /// Line which caused the trouble (-1 if unknown). The first line is line 0. Error reporting adds 1 for user presentation.
        /// </summary>
        public readonly int m_line;

        /// <summary>
        /// Column which caused the trouble (-1 if unknown). The first column is line 0. Error reporting adds 1 for user presentation.
        /// </summary>
        public readonly int m_column;

    } // end of class UserException
} // end of namespace G25

