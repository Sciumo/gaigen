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
using System.Reflection;

namespace G25
{

    /// <summary>
    /// All code generation backends for Gaigen 2.5 must implement this interface.
    /// This allows Gaigen to recognize the class as a code generator, to ask it for 
    /// what language it generates code, and to ask it to actually generate the code.
    /// </summary>
    public interface CodeGenerator
    {
        /// <returns>what language this code generator generates for (case insensitive).</returns>
        String Language();

        /// <summary>
        /// Should generate the code according to the specification of the algebra.
        /// </summary>
        /// <param name="S">The specification of the algebra. The specification also lists the names of the files
        /// to be generated, or at least the base path.</param>
        /// <param name="plugins">The plugins which Gaigen found that support the same language as this code generator.</param>
        /// <returns>a list of filenames; the names of the files that were generated. This may be used
        /// for post processing.</returns>
        List<string> GenerateCode(Specification S, List<CodeGeneratorPlugin> plugins);
    } // end of interface CodeGenerator

    /// <summary>
    /// All code generation plugins for Gaigen 2.5 must implement this interface.
    /// This allows Gaigen to recognize the class as a code generator and  to ask it for 
    /// what language it generates code.
    /// 
    /// The actual link between the CodeGenerator and the CodeGeneratorPlugin is handled
    /// using an interface that is defined by the code generator itself.
    /// </summary>
    public interface CodeGeneratorPlugin
    {
        /// <returns>what language this code generator generates for (case insensitive).</returns>
        String Language();
    } // end of interface CodeGeneratorPlugin

    // temp classes for some testing
    public class TmpCppGenerator : G25.CodeGenerator
    {
        public String Language()
        {
            return "cpp";
        }

        public List<string> GenerateCode(Specification S, List<CodeGeneratorPlugin> plugins)
        {
            return null;
        }
    }

    // temp classes for some testing
    public class TmpCppGeneratorPlugin : G25.CodeGeneratorPlugin
    {
        public String Language()
        {
            return "cpp";
        }
    }
    


    /// <summary>
    /// This class loads and stores code generation classes (plugins) for a certain language.
    /// 
    /// Code generation in Gaigen 2.5 is handled by language-specific classes.
    /// Each `main' code generator class (backend) must implement the G25.CodeGenerator interface. 
    /// A `main' code generator class or is the class that is mainly responsible for code generation.
    /// When a class implements the G25.CodeGenerator interface, Gaigen recognizes it as a 
    /// code generation backend, and can ask it for what language it generates code.
    /// The interface also allows Gaigen to ask the class to actually generate code.
    /// 
    /// A code generator back-end may be able to handle plugins. These plugins may extend
    /// the capabilities of the plain code generator. This may be useful to extend the functionility
    /// of a code generator when you are not the maintainer of that code generator. 
    /// 
    /// Plugins must implement the CodeGeneratorPlugin interface.
    /// This allows Gaigen to recognize the class as a code generator plugin, and to ask it for what 
    /// language it generates. The actual link between the code generator and the plugin is handled
    /// by an interface that is defined by the developer of the actual  code generator. 
    /// 
    /// </summary>
    public class CodeGeneratorLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="language"></param>
        public CodeGeneratorLoader(string language)
        {
            m_language = language.ToLower();
        }

        /// <summary>
        /// Calls AddDirectory(path, ".dll");
        /// </summary>
        public void AddDirectory(String path)
        {
            AddDirectory(path, ".dll");
        }

        /// <summary>
        /// Find all code generator (plugins) in the directory 'path' and keeps
        /// references to the types.
        /// 
        /// Seaches the directory for dlls (assemblies) and tries to load them.
        /// Then it searches the assembly for classes which either implement
        /// the CodeGenerator or the CodeGeneratorPlugin interface.
        /// 
        /// When it finds such classes, it asks them for what language they generate.
        /// If the language matches Language, the types of the classes are retained
        /// in m_mainCodeGenerator or m_codeGeneratorPlugins.
        /// </summary>
        /// <param name="path">The path where to search for assemblies</param>
        /// <param name="assemblyExtension">The extension of assembly files (is this always ".dll", even on Linux?)</param>
        public void AddDirectory(String path, String assemblyExtension)
        {
            if (!assemblyExtension.StartsWith(".")) assemblyExtension = "." + assemblyExtension;
            
            System.IO.DirectoryInfo DI = new System.IO.DirectoryInfo(path);
            FileInfo[] FI = DI.GetFiles();
            for (int i = 0; i < FI.Length; i++) {
                if (FI[i].Name.EndsWith(assemblyExtension)) // could be an assembly
                    AddAssembly(FI[i].DirectoryName + System.IO.Path.DirectorySeparatorChar + FI[i].Name);
            }
        }

        /// <summary>
        /// Loads the assembly, see AddDirectory()
        /// </summary>
        /// <param name="filename">Filename of the assembly</param>
        public void AddAssembly(String filename)
        {
            Assembly A = Assembly.LoadFile(filename);
            AddAssembly(A);
        }

        /// <summary>
        /// Class AddClass() for all public classes in 'ass'
        /// </summary>
        /// <param name="A"></param>
        public void AddAssembly(Assembly A)
        {
            Type[] TA = A.GetExportedTypes();
            for (int i = 0; i < TA.Length; i++)
            {
                AddClass(TA[i]);
            }
        }

        /// <summary>
        /// Adds a class for code generation or CG plugin.
        /// 
        /// Checks if the class implements G25.CodeGenerator. If so, creates an instance,
        /// checks if it supports the required Language, and if so, keeps the instance in m_codeGenerator.
        /// 
        /// Also checks if the class implements G25.CodeGeneratorPlugin. If so, creates an instance,
        /// checks if it supports the required Language, and if so, keeps the instance in m_codeGeneratorPlugins.
        /// </summary>
        /// <param name="T">Any type (if the type does not support the CG interrface(s), nothing happens)</param>
        public void AddClass(Type T)
        {
            if (typeof(G25.CodeGenerator).IsAssignableFrom(T)) {
                try
                {
                    G25.CodeGenerator CG = System.Activator.CreateInstance(T) as G25.CodeGenerator;
                    if ((CG != null) && (CG.Language().ToLower() == Language))
                        SetMainCodeGenerator(CG);
                }
                catch (System.Exception)
                {
                }
            }
            if (typeof(G25.CodeGeneratorPlugin).IsAssignableFrom(T)) {
                try
                {
                    G25.CodeGeneratorPlugin P = System.Activator.CreateInstance(T) as G25.CodeGeneratorPlugin;
                    if ((P != null) && (P.Language().ToLower() == Language))
                        AddCodeGeneratorPlugin(P);
                }
                catch (System.Exception)
                {
                }
            }
        }

        /// <summary>
        /// Sets the main code generator to 'CG'. If CG does not support the right language, throws an exception.
        /// </summary>
        public void SetMainCodeGenerator(G25.CodeGenerator CG) {
            if (CG.Language() != Language)
                throw new Exception("G25.CodeGeneratorLoader.SetMainCodeGenerator(): the code generator does not generate for language '" + Language + "' but for language '" + CG.Language() + "' instead");

            if (m_codeGenerator != null)
            {
                if (m_codeGenerator.GetType() == CG.GetType())
                    return;
                // else: TO DO; // a duplicate of the code generator was detected. Now what? throw an exception? write an error message to console? TO DO!
            }

            m_codeGenerator = CG;
        }

        /// <returns>The main code generator for the language, or null if not set.</returns>
        public G25.CodeGenerator GetMainCodeGenerator()
        {
            return m_codeGenerator;
        }

        /// <returns>The plugins for code generator for the language (may be empty list).</returns>
        public List<CodeGeneratorPlugin> GetCodeGeneratorPlugins() {
            return m_codeGeneratorPlugins;
        }

        /// <summary>
        /// Adds 'P' to the list of code generator plugins'. 
        /// If P does not support the right language, throws an exception.
        /// Does nothing if 'P' is already on the list.
        /// </summary>
        public void AddCodeGeneratorPlugin(G25.CodeGeneratorPlugin P)
        {
            if (P.Language() != Language)
                throw new Exception("G25.CodeGeneratorLoader.AddCodeGeneratorPlugin(): the code generator does not generate for language '" + Language + "' but for language '" + P.Language() + "' instead");
            // if already on list simply return
            for (int i = 0; i < m_codeGeneratorPlugins.Count; i++)
                if (m_codeGeneratorPlugins[i].GetType() == P.GetType())
                    return;
            m_codeGeneratorPlugins.Add(P);
        }

        /// <summary>
        /// The language for which the code generator and plugins are loaded (case insensitive).
        /// </summary>
        public string Language { get { return m_language; } }

        /// <summary>
        /// The language for which the code generator and plugins are loaded.
        /// </summary>
        protected string m_language;
        /// <summary>
        /// The main code generator for the language.
        /// </summary>
        protected G25.CodeGenerator m_codeGenerator;
        /// <summary>
        /// The code generator plugins for the language.
        /// </summary>
        protected List<CodeGeneratorPlugin> m_codeGeneratorPlugins = new List<G25.CodeGeneratorPlugin>();

    } // end of class CodeGeneratorLoader


} // end of namespace G25
