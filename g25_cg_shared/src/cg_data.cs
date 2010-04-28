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
    /// Contains various utility functions.
    /// </summary>
    public class CGdata
    {
        public CGdata(List<CodeGeneratorPlugin> plugins, CoGsharp.CoG cog)
        {
            m_plugins = plugins;
            m_cog = cog;
        }

        /// <summary>
        /// Constructor that allows you to set StringBuilders for each possible destination
        /// of the generated code (decl, def, inline def).
        /// </summary>
        /// <param name="cgd"></param>
        public CGdata(CGdata cgd)
        {
            // copy all from cgd
            m_plugins = cgd.m_plugins;
            m_cog = cgd.m_cog;
            m_gmvGPpartFuncNames = cgd.m_gmvGPpartFuncNames;
            m_gmvDualPartFuncNames = cgd.m_gmvDualPartFuncNames;
            m_gmvGomPartFuncNames = cgd.m_gmvGomPartFuncNames;
            m_missingDependencies = cgd.m_missingDependencies;
            m_errors = cgd.m_errors;
            m_feedback = cgd.m_feedback;
            m_dependencyId = cgd.m_dependencyId;
            m_dependencyPrefix = cgd.m_dependencyPrefix;
        }

        /// <summary>
        /// Constructor that allows you to set StringBuilders for each possible destination
        /// of the generated code (decl, def, inline def).
        /// </summary>
        /// <param name="cgd"></param>
        /// <param name="declSB"></param>
        /// <param name="defSB"></param>
        /// <param name="inlineDefSB"></param>
        public CGdata(CGdata cgd, StringBuilder declSB, StringBuilder defSB, StringBuilder inlineDefSB) : this(cgd)
        {
            m_declSB = declSB;
            m_defSB = defSB;
            m_inlineDefSB = inlineDefSB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>a unique ID which will also prevents mangling.</returns>
        public string GetDontMangleUniqueId()
        {
            int Id;
            lock(m_dependencyId)
            {
                if (m_dependencyId.Length > 0)
                    Id = Int32.Parse(m_dependencyId.ToString());
                else Id = 0;
                Id++;
                m_dependencyId.Remove(0, m_dependencyId.Length);
                m_dependencyId.Append(Id.ToString());
            }

            return G25.Specification.DONT_MANGLE + Id.ToString();
        }

        /// <summary>
        /// Sets <c>m_dependencyPrefix</c> to <c>str</c>.
        /// </summary>
        /// <param name="str">New value for <c>m_dependencyPrefix</c></param>
        public void SetDependencyPrefix(string str)
        {
            m_dependencyPrefix.Remove(0, m_dependencyPrefix.Length);
            m_dependencyPrefix.Append(str);
        }

        /// <summary>
        /// Adds a missing dependency 'F' to m_missingDependencies.
        /// </summary>
        /// <param name="S">Specification of algebra (not used currently).</param>
        /// <param name="F">The missing dependency.</param>
        /// <returns>F, or an G25.fgs which is to F if it was already added by an earlier call.</returns>
        public G25.fgs AddMissingDependency(Specification S, G25.fgs F) 
        {
            lock (m_missingDependencies)
            {
                if (!m_missingDependencies.ContainsKey(F))
                {
                    m_missingDependencies[F] = F;
                }
                else F = m_missingDependencies[F];
            }
            return F;
        }

        /// <summary>
        /// Returns the number of missing dependencies.
        /// </summary>
        /// <returns>the number of missing dependencies.</returns>
        public int GetNbMissingDependencies()
        {
            int nb = 0;
            lock (m_missingDependencies)
            {
                nb = m_missingDependencies.Count;
            }
            return nb;
        }

        /// <summary>
        /// Returns the number of errors.
        /// </summary>
        /// <returns>the number of errors.</returns>
        public int GetNbErrors()
        {
            int nb = 0;
            lock (m_errors)
            {
                nb = m_errors.Count;
            }
            return nb;
        }

        /// <summary>
        /// Takes all keys of <c>m_missingDependencies</c> and returns them in a list.
        /// </summary>
        /// <param name="excludeDeps">Can be used to exclude dependencies. The test suite code uses
        /// this to exclude dependencies which have already been generated. Can be <c>null</c> if not used.</param>
        /// <returns>List of missing dependencies.</returns>
        public List<fgs> GetMissingDependenciesList(List<fgs> excludeDeps)
        {
            List<fgs> L = new List<fgs>();
            foreach (KeyValuePair<G25.fgs, G25.fgs> KVP in m_missingDependencies)
            {
                bool exclude = false;
                if (excludeDeps != null) // see if on exclude list?
                { 
                    for (int i = 0; i < excludeDeps.Count; i++)
                    {
                        if (excludeDeps[i].Equals(KVP.Key))
                        {
                            exclude = true;
                            break;
                        }
                    }
                }

                if (!exclude)
                    L.Add(KVP.Key);
            }
            return L;
        }


        /// <summary>
        ///  Prints out a report of all the missing dependencies recorded in m_missingDependencies.
        /// 
        /// The report consists of XML that can be added directly to the specification XML.
        /// </summary>
        /// <param name="S"></param>
        public void PrintMissingDependencies(Specification S)
        {
            lock (m_missingDependencies)
            {
                if (m_missingDependencies.Count != 0)
                {
                    System.Console.WriteLine("");
                    System.Console.WriteLine("Missing dependencies detected.");
                    System.Console.WriteLine("Add the following XML to the specification to fix them:");
                    System.Console.WriteLine("");

                    // sort missing deps (this is nicer for the user who pastes them into the specification XML
                    G25.fgs[] D = new G25.fgs[m_missingDependencies.Count];
                    {
                        int idx = 0;
                        foreach (KeyValuePair<G25.fgs, G25.fgs> KVP in m_missingDependencies)
                            D[idx++] = KVP.Key;
                        Array.Sort(D);
                    }

                    // print deps to console:
                    for (int i = 0; i < D.Length; i++)
                    {
                        System.Console.WriteLine(S.FunctionToXmlString(D[i]));
                    }
                }
            }
        } // end of PrintMissingDependencies

        /// <summary>
        /// Adds an error to the list of errors.
        /// Use PrintErrors() to report them to the user.
        /// </summary>
        /// <param name="E">The error exception.</param>
        public void AddError(G25.UserException E)
        {
            lock (m_errors)
            {
                m_errors.Add(E);
            }
        }

        /// <summary>
        /// Merges all errors from cgd into this CGdata.
        /// </summary>
        /// <param name="cgd"></param>
        public void MergeErrors(CGdata cgd)
        {
            lock (m_errors)
            {
                m_errors.AddRange(cgd.m_errors);
            }
        }

        /// <summary>
        /// Prints all errors to the console.
        /// </summary>
        /// <param name="S">Not used currently.</param>
        public void PrintErrors(Specification S)
        {
            lock (m_errors)
            {
                if (m_errors.Count > 0)
                {
                    System.Console.WriteLine(m_errors.Count + " error" + ((m_errors.Count > 1) ? "s" : "") + ":");
                    foreach (G25.UserException E in m_errors)
                    {
                        System.Console.Write(E.GetErrorReport());
                        System.Console.WriteLine();
                        System.Console.WriteLine();
                    }
                }
            }
        }

        /// <summary>
        /// Sets a key-value entry in m_feedback.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void SetFeedback(string key, string value)
        {
            lock (m_feedback)
            {
                m_feedback[key] = value;
            }
        }

        /// <summary>
        /// Gets a value entry in from m_feedback.
        /// Returns null when value is not set.
        /// </summary>
        /// <param name="key">Key</param>
        public string GetFeedback(string key)
        {
            string value = null;
            lock (m_feedback)
            {
                if (m_feedback.ContainsKey(key)) value = m_feedback[key];
            }
            return value;
        }

        /// <summary>
        /// Reset all StringBuilders  (<c>m_declSB</c> and so on).
        /// </summary>
        public void ResetSB()
        {
            m_declSB = new StringBuilder();
            m_defSB = new StringBuilder();
            m_inlineDefSB = new StringBuilder();
        }


        /// <summary>
        /// Map from FloatType X Metric X function name -> bool 
        /// Tells you whether a geometric product part with name 'function name' has been generated.
        /// </summary>
        public Dictionary<G25.Tuple<string, string, string>, bool> m_gmvGPpartFuncNames = new Dictionary<G25.Tuple<string, string, string>, bool>();

        /// <summary>
        /// Map from FloatType X Metric X function name -> bool 
        /// Tells you whether a dual part with name 'function name' has been generated.
        /// </summary>
        public Dictionary<G25.Tuple<string, string, string>, bool> m_gmvDualPartFuncNames = new Dictionary<G25.Tuple<string, string, string>, bool>();

        /// <summary>
        ///  TODO: some kind of map which tells you: if you want to combine apply GOM to GMV, do it like this. . .
        /// </summary>
        public Dictionary<G25.Tuple<string, string>, bool> m_gmvGomPartFuncNames = new Dictionary<G25.Tuple<string, string>, bool>();

        public List<CodeGeneratorPlugin> m_plugins;
        public CoGsharp.CoG m_cog;

        /// <summary>
        /// Generated declarations go here
        /// </summary>
        public StringBuilder m_declSB = new StringBuilder();
        /// <summary>
        /// Generated definitions go here
        /// </summary>
        public StringBuilder m_defSB = new StringBuilder();
        /// <summary>
        /// Generated inline definitions go here
        /// </summary>
        public StringBuilder m_inlineDefSB = new StringBuilder();

        /// <summary>
        /// Names of generated test functions go here.
        /// </summary>
        public List<string> m_generatedTestFunctions;

        /// <summary>
        /// Set of missing dependencies.
        /// </summary>
        public Dictionary<G25.fgs, G25.fgs> m_missingDependencies = new Dictionary<fgs, G25.fgs>();

        /// <summary>
        /// Errors are collected here. Use PrintErrors() to report them all.
        /// </summary>
        public List<G25.UserException> m_errors = new List<G25.UserException>();
        

        /// <summary>
        /// Flags to communicate information from code generator plugins to the main
        /// generator. This is used for example to allow the random scalar generator
        /// to tell the main generator to generate code for the mersenne twister.
        /// 
        /// User SetFeedback() to set.
        /// </summary>
        public Dictionary<string, string> m_feedback = new Dictionary<string, string>();

        /// <summary>
        /// (optional) prefix for the names of missing functions returned by GetDependency().
        /// This can be used to make clear that a function is missing.
        /// 
        /// A StringBuilder is used such that the value can be changed centrally and be
        /// 'inherited' by all derived CGdatas
        /// </summary>
        public StringBuilder m_dependencyPrefix = new StringBuilder();

        /// <summary>
        /// This 'counter' is used to give each dependency its own unique number.
        /// This is done to prevent nameclashes in language which do not support
        /// overloading.
        /// </summary>
        public StringBuilder m_dependencyId = new StringBuilder();

    } // end of class CGdata
} // end of namepace G25.CG.Shared