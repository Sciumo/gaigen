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
using System.Threading;

namespace G25.CG.CPP
{


    /// <summary>
    /// C++ utility functions for converters
    /// </summary>
    class Converter
    {
        private const string COMMENT = " Automatically generated converter.";

        public static void AddDefaultGmvConverters(Specification S)
        {

            // get a 'set' of SMVs for which converters to GMV are present
            Dictionary<string, fgs> converterPresent = new Dictionary<string, fgs>();
            foreach (fgs F in S.m_functions)
            {
                if (F.IsConverter(S))
                {
                    if (F.GetArgumentTypeName(0, null) == S.m_GMV.Name)
                    {
                        converterPresent.Add(F.Name.Substring(1), F);
                    }
                }
            }

            // get array of float type names, for use below
            string[] floatNames = new string[S.m_floatTypes.Count];
            for (int i = 0; i < S.m_floatTypes.Count; i++)
                floatNames[i] = S.m_floatTypes[i].type;


            // loop over all SMVs, if no converter to GMV is present, add it
            foreach (G25.SMV smv in S.m_SMV)
            {
                if (converterPresent.ContainsKey(smv.Name)) continue;

                string outputName = null;
                string returnTypeName = null;
                string[] argumentTypeNames = new string[] {S.m_GMV.Name};
                string[] argumentVariableNames = null;
                string metricName = null;
                Dictionary<string, string> options = null;
                fgs F = new fgs("_" + smv.Name, outputName, returnTypeName,
                    argumentTypeNames, argumentVariableNames,
                    floatNames, metricName, COMMENT, options);
                S.m_functions.Add(F);

            }

        }

        public static void AddDefaultSmvConverters(Specification S)
        {

            // get a 'set' of SMVs for which converters to the same type are present
            Dictionary<string, fgs> converterPresent = new Dictionary<string, fgs>();
            foreach (fgs F in S.m_functions)
            {
                if (F.IsConverter(S))
                {
                    if (F.GetArgumentTypeName(0, null) == F.Name.Substring(1))
                    {
                        converterPresent.Add(F.Name.Substring(1), F);
                    }
                }
            }

            // get array of float type names, for use below
            string[] floatNames = new string[S.m_floatTypes.Count];
            for (int i = 0; i < S.m_floatTypes.Count; i++)
                floatNames[i] = S.m_floatTypes[i].type;

            // loop over all SMVs, if no converter to GMV is present, add it
            foreach (G25.SMV smv in S.m_SMV)
            {
                if (converterPresent.ContainsKey(smv.Name)) continue;

                string outputName = null;
                string returnTypeName = null;
                string[] argumentTypeNames = new string[] { smv.Name };
                string[] argumentVariableNames = null;
                string metricName = null;
                Dictionary<string, string> options = null;
                fgs F = new fgs("_" + smv.Name, outputName, returnTypeName,
                    argumentTypeNames, argumentVariableNames,
                    floatNames, metricName, COMMENT, options);
                S.m_functions.Add(F);

            }

        }

    } // end of class Converter
} // end of namespace G25.CG.CPP

