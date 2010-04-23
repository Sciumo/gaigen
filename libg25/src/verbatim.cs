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
using System.IO;

namespace G25
{

    /// <summary>
    /// 
    /// This class is a container for verbatim code XML entries.
    /// 
    /// </summary>
    public class VerbatimCode
    {
        /// <summary>
        ///  Where to position the verbatim code.
        ///    - TOP: at the top of the file (may be after the license).
        ///    - BOTTOM: at the bottom of the file.
        ///    - CUSTOM_BEFORE: in the line before the specified marker.
        ///    - CUSTOM_AFTER: in the line after the specified marker.
        /// </summary>
        public enum POSITION {
            INVALID = -1,
            TOP = 1,
            BOTTOM,
            BEFORE_MARKER,
            AFTER_MARKER
        }

        /// <summary>
        /// Constructor.
        /// 
        /// There are two ways to specify the verbatim code: directly (verbatimCode)
        /// or from a file (verbatimCodeFile).
        /// <param name="filenames">Into what file(s) should the code be inserted. 
        /// Multiple files can be specified (e.g., when multiple files need to include the same header).
        /// If a filename is not an absolute path, then the output directory will be 
        /// prefixed (at verbatim-code-insertion-time).</param>
        /// <param name="where">Where to put the custom code.</param>
        /// <param name="customMarker">A custom string that is used to locate 'where' when 
        /// 'where' is BEFORE_MARKER or AFTER_MARKER.</param>
        /// <param name="verbatimCode">The verbatim code the be inserted.</param>
        /// <param name="verbatimCodeFile">The file to read the verbatim code from, to be inserted.
        /// If the path is not absolute, the filename is relative to the dir where the specification came from.</param>
        /// </summary>
        public VerbatimCode(
            List<string> filenames,
            POSITION where,
            string customMarker,
            string verbatimCode,
            string verbatimCodeFile)
        {
            m_filenames = filenames;
            m_where = where;
            m_customMarker = customMarker;
            m_verbatimCode = verbatimCode;
            m_verbatimCodeFile = verbatimCodeFile;
        }

        /**
         * Returns verbatim code, either from m_verbatimCode or from
         * m_verbatimCodeFile. Can throw G25.UserException.
         * <param name="inputPath">Is used to resolve relative paths of <c>m_verbatimCodeFile</c>,
         * if required. The value should be Specification.m_inputDirectory in normal use.</param>
         */
        public string GetVerbatimCode(String inputPath)
        {
            if ((m_verbatimCode != null) && (m_verbatimCode.Length > 0))
                return m_verbatimCode;
            else {
                if ((m_verbatimCodeFile == null) || (m_verbatimCodeFile.Length == 0))
                    throw new G25.UserException("VerbatimCode.GetVerbatimCode(): no verbatim code an no verbatim code filename either");

                try
                {
                    String filename = m_verbatimCodeFile;
                    if (!System.IO.Path.IsPathRooted(filename))
                        filename = System.IO.Path.Combine(inputPath, filename);

                    // create reader & open file
                    System.IO.TextReader tr = new System.IO.StreamReader(filename);
                    String code = tr.ReadToEnd();

                    // close the stream
                    tr.Close();

                    return code;
                }
                catch (Exception)
                {
                    throw new G25.UserException("VerbatimCode.GetVerbatimCode(): cannot read verbatim code from file " + m_verbatimCodeFile);
                }
            }
        } // end of GetVerbatimCode


        /**
         * Inserts the verbatim code (in m_verbatimCode</c>) into the generated files.
         * The list <c>generatedFiles</c> is used to find the names of the files.
         * 
         * Warnings are issued when code could not be inserted.
         * <param name="generatedFiles">The m_filenames are matched against the end of generatedFiles.
         * If a match is found, the code is inserted into the file. The match must include a full file/directory name,
         * so 'in.cpp' cannot match './main.cpp'.</param>
         * <param name="inputPath">The path relative to which the m_verbatimCodeFile is searched.</param>
         * */
        public void InsertCode(String inputPath, List<string> generatedFiles)
        {
            // get the verbatim code to be inserted
            String code = GetVerbatimCode(inputPath);

            // look for matching filenames
            foreach(string targetFilename in m_filenames) {
                bool found = false;
                foreach(string generatedFilename in generatedFiles) {
                    // see if the end of the generatedFilename matches:
                    if (generatedFilename.EndsWith(targetFilename)) 
                    {
                        // see if it is a full dir/filename:
                        int idx = generatedFilename.Length - targetFilename.Length - 1;
                        if ((idx < 0) ||
                            (generatedFilename[idx] == System.IO.Path.DirectorySeparatorChar))
                        {
                            // yes:
                            found = true;
                            InsertCode(code, generatedFilename);
                            break;
                        }
                    }
                }
                if (!found)
                {
                    System.Console.WriteLine("Error: cannot find file '" + targetFilename + "' for verbatim code insertion.");
                }
            }

        } // end of InsertCode()

        /**
         * Inserts 'code' into 'filename' according to the other 
         * specifications (m_where and m_customMarker) in 
         * this VerbatimCode instance.
         */
        protected void InsertCode(string code, string filename)
        {
            // read whole file
            string fullFileContents;
            try
            {
                // create reader & open file & read it
                System.IO.TextReader tr = new System.IO.StreamReader(filename);
                fullFileContents = tr.ReadToEnd();

                // close the stream
                tr.Close();
            }
            catch (Exception)
            {
                System.Console.WriteLine("Error: cannot read file '" + filename + "' for verbatim code insertion.");
                return;
            }

            // get index for insertion
            int insertIdx = -1;
            switch (m_where)
            {
                case POSITION.TOP:
                    insertIdx = 0;
                    break;
                case POSITION.BOTTOM:
                    insertIdx = fullFileContents.Length - 1;
                    break;
                case POSITION.BEFORE_MARKER:
                case POSITION.AFTER_MARKER:
                    insertIdx = fullFileContents.IndexOf(m_customMarker);
                    if (insertIdx < 0)
                    {
                        System.Console.WriteLine("Error: cannot find marker '" + m_customMarker + "' in file '" + filename + "' for verbatim code insertion.");
                        return;
                    }
                    if (m_where == POSITION.AFTER_MARKER) insertIdx += m_customMarker.Length;
                    break;
            }


            // split input file into before and after insert
            string beforeInsert = fullFileContents.Substring(0, insertIdx);
            string afterInsert = fullFileContents.Substring(insertIdx);

            try
            {
                // create write & open file & write it
                // since the file will always grow or stay equal size, no need to clear file first
                System.IO.TextWriter tw = new System.IO.StreamWriter(filename);
                tw.Write(beforeInsert);
                tw.Write(code);
                tw.Write(afterInsert);

                // close the stream
                tw.Close();
            }
            catch (Exception)
            {
                System.Console.WriteLine("Error: cannot write file '" + filename + "' for verbatim code insertion.");
                return;
            }


        }


        /// <summary>
        /// Into what file(s) should the code be inserted. 
        /// Multiple files can be specified (e.g., when multiple files need to include the same header).
        /// If a filename is not an absolute path, then the output directory will be 
        /// prefixed (at verbatim-code-insertion-time).
        /// </summary>
        public List<string> m_filenames;
        /// <summary>
        /// Where to put the custom code.
        /// </summary>
        public POSITION m_where;
        /// <summary>
        /// A custom string that is used to locate 'where' when 
        /// 'where' is BEFORE_MARKER or AFTER_MARKER.
        /// </summary>
        public string m_customMarker;
        /// <summary>
        /// The verbatim code the be inserted.
        /// </summary>
        public string m_verbatimCode;
        /// <summary>
        /// The file to read the verbatim code from, to be inserted.
        /// If the path is not absolute, the filename is relative to the dir where the specification came from.
        /// </summary>
        public string m_verbatimCodeFile;

    } // end of class VerbatimCode
} // end of namespace G25
