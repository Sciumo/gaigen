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

/*! \mainpage Code Generation for C# (CoGsharp) Documentation
 *
 * CoGsharp by Daniel Fontijne, University of Amsterdam.
 * 
 * Released under GPL license.
 * 
 * \section intro_sec Introduction
 *
 * This package provides a way to do code generation in C#. The main class is CoGsharp.CoG.
 * CoG stands for Code Generator. This package is a C# port of an earlier Java version.
 * 
 * The user of CoG can load templates into an instance of CoG. That instance can then 'emit' 
 * these templates with supplied arguments. To emit the templates, the CoG class compiles 
 * them (or loads templates that were previously compiled) and invokes them using the user-supplied
 * arguments.
 * 
 * \section templates_sec Templates
 * 
 * Templates are a mix of verbatim text and executable C# code. They are stored in regular text files.
 * Each file may contain multiple templates.
 * A template starts with
 * <code>
 * ${CODEBLOCK template_name}
 * </code>
 * and ends with
 * <code>
 * ${ENDCODEBLOCK template_name}
 * </code>
 * The name <c>CODEBLOCK</c> is used because CoG was originally used to generate
 * source code. Between the two delimiters you put the verbatim text and C# code.
 * 
 * The C# code is embedded inside the verbatim text between <c><%</c> and <c>%></c> delimiters.
 * The code can be 
 *   - control code (<c>for</c> loops and such)
 *   - <c>using</c> directives which tells the compiler that this code template uses a certain class
 *   - <c>implements</c> directives which tells the compiler that this code template implements a certain interface.
 *   - `value code' which returns a value. This value will be converted to a string (using the ToString() method) and emitted to the output.
 * 
 * An example of a template which uses all these features (except <c>implements</c>):
 * <code>
  * // This sample template prints out the hashtable argument named 'HT'
 * // Arguments:
 * // HT =  System.Collections.Hashtable
 * ${CODEBLOCK sample_template}
 * <%using System.Collections%>
 * The Hashtable contains <%HT.Count%> entries.
 * The key->value of the entries is:
 * 
 * <%foreach (DictionaryEntry DE in HT) {%>
 * <%DE.Key%> -> <%DE.Value%>.
 * <%}%>
 * 
 * That's all
 * ${ENDCODEBLOCK}
 * </code>
 * 
 * If <c>HT</c> is a hashtable containing three camera names and their respective price in Euros, then
 * the output of the template is:
 * <code>
 * The Hashtable contains 3 entries.
 * The key->value of the entries is:
 * 
 * Canon Eos 5D Mark II body -> 2176.
 * Nikon D700 body -> 1997.
 * Sony DSLR-A900 body -> 2345.
 * 
 * That's all
 * </code>
 * Note that for each block of code between <c><%</c> and <c>%></c> 
 * delimiters, CoG determines  * whether it is control code or value code (or <c>using</c> or <c>implements</c> code). 
 * This is determined by looking at the start of the block of code.
 * If it starts a statement like <c>for</c> or <c>{</c>}, it is assumed to be control code. If the code starts
 * with a regular identifier or <c>(</c>, then it is assumed to be value code. So to print out all
 * entries of the Hashmap, the <c>foreach</c> loop has to be cut into several parts, some of which are value
 * code, and some of which are control code.
 * 
 * \section cog_sec Using the CoG class.
 * 
 * To use the CoG class, first construct a new instance and load the template file(s). Template files are regular text
 * files which can contain multiple templates (<c>CODEBLOCK</c>s).
 * <code>
 * CoG C = new CoG();
 * C.LoadTemplates("sample_template_file.txt");
 * C.LoadTemplates("another_template_file.txt");
 * </code>
 * Multiple template files can be loaded. In the above example two files are loaded. 
 * The filename of the template file does not have to be related to the name(s) of the codeblock(s) inside the file.
 * Templates are firstparsed when used, not when loaded. When a template file is loaded, only the <c>CODEBLOCK</c>s are 
 * found and stored in a hashtable (a map from name to verbatim content of the template). 
 * So if there are errors in the templates themselves, they are not detected at load-time, but when the template is emitted.
 * 
 * One way to emit a template is:
 * <code>
 * C.EmitTemplate("sample_template", "HT=", HT);
 * </code>
 * Here, <c>HT</c> is the <c>Hashtable </c> from the above example. The <c>=</c> symbol in the string
 * is optional. It is intended to make clear that the argument <c>HT</c> of the template is set to the variable
 * (which happens to be called <c>HT</c> too in this example, but this is not a requirement).
 * 
 * Up to eight parameters can be passed
 * to <c>EmitTemplate()</c> this way. <c>EmitTemplate()</c> write the output goes into the <c>m_output</c> member 
 * variable inside the CoG class. You can retrieve the output using <c>CoGsharp.CoG.GetOutput()</c>, see below
 * 
 * If you'd like to get the output right back, you can provide your own StringBuilder like this:
 * <code>
 * StringBuilder outputStringBuilder = new StringBuilder();
 * C.EmitTemplate(outputStringBuilder, "sample_template", "HT=", HT);
 * </code>
 * Now the result will go into <c>outputStringBuilder</c>. This variant of <c>EmitTemplate()</c> supports
 * up to eight arguments too.
 * 
 * To use up to eight arguments, call <c>EmitTemplate()</c> like this:
 * <code>
 * C.EmitTemplate("template_name", "arg1name=", arg1value, ..., "arg8name=", arg8value);
 * </code>
 * If even more arguments are required, then store them in a <c>Hashtable</c>. Each key in the Hashtable should be the
 * name of the argument in the template and the value would be the value of the argument. The key must be a string.
 * The value can be any <c>Object</c>.
 * <code>
 * System.Collections.Hashtable hashtableContainingTheArguments = 
 *      new System.Collections.Hashtable();
 * hashtableContainingTheArguments["arg1"] = arg1value;
 * // . . .
 * hashtableContainingTheArguments["arg20"] = arg20value;
 * C.EmitTemplate("template_name", hashtableContainingTheArguments);
 * </code>
 * To actually emit a template, CoG compiles the template into executable code. At this point compilation
 * errors may occur if there is something wrong with your template or the arguments you pass it.
 * 
 * \section error_sec Template compilation errors.
 * 
 * Some knowledge of how the templates are converted into the emitted text is required in order to be able
 * to properly handle errors in templates. 
 * 
 * When the time has come to emit a template, it is converted into <c>.cs</c> source code, which is then compiled to a <c>.dll</c> file. 
 * The resulting <c>.cs</c> and the <c>.dll</c> files are stored in the system-wide temporary directory in a subdirectory named
 * <c>cogsharp</c>. On my current system, that would be <c>"C:\Documents and Settings\Administrator\Local Settings\Temp\cogsharp"</c>.
 * In principle, compilation could be done in memory without writing this file, but the <c>.cs</c> file is useful for debugging your template (see below). 
 * 
 * The name of these files is based on the hashcodes of the template string and the arguments (converted to string).
 * If this hashcode happens to clash with an existing filename, the next (increment) hashcode is used, until an unused hashcode is found.
 * 
 * When you call <c>EmitTemplate()</c>, CoG first searches its cache of loaded dlls. If the specific template is not found in the cache,
 * it tries to load it from the temporary directory (verifying that it gets the correct template). If it succeeds, the loaded dll is used
 * and also stored in the cache. Otherwise, CoG will generate the source code from the template, and compile it. 
 * At this point, you may get compilation errors if your template contains an error or if you pass the
 * wrong type of arguments (e.g., the argument does not support the member function you call on it inside the template). 
 * In either case, an Exception is thrown containing a description of the error.
 * If that happens, you can inspect the respective <c>.cs</c> file to find out what caused the error.
 * If compilation succeeds, the resulting dll is loaded and stored in the cache.
 * 
 * Note that for each (combination) of argument types and/or names you get a different compiled template (with a different dll name), even
 * though 99% of the resulting code will be identical.
 * 
 * Below is the code which gets generated for the <c>sample_template</c> from above, when called with
 * one argument <c>System.Collections.Hashtable HT</c>. The <c>GetTemplate()</c> and <c>GetArguments()</c> functions are
 * member of the CoGsharp.CoGclass interface. They are used to verify that the correct template was loaded from a previously compiled
 * dll. The <c>Emit()</c> function emits the actual template.
 * <code>
 * // Generated by CoGsharp from template 'sample_template' on 11/19/2008 3:13:18 PM
 * using System;
 * using CoGsharp;
 * using System.Collections;
 * namespace CoGgenerated {
 * public class CoG_3532804186 : CoGsharp.CoGclass {
 * 	public CoG_3532804186() {
 * 	}
 * 	public String GetTemplate() {
 * 		return
 * 			"\r\n<%using System.Collections%>\r\nThe Hashtable contains <%" + 
 * 			"HT.Count%> entries.\r\nThe key->value of the entries is:\r\n\r" + 
 * 			"\n<%foreach (DictionaryEntry DE in HT) {%>\r\n<%DE.Key%> -> <" + 
 * 			"%DE.Value%>.\r\n<%}%>\r\n\r\nThat's all\r\n";
 * 	}
 * 	public String GetArguments() {
 * 		return "System.Collections.Hashtable HT";
 * 	}
 * 	public void Emit(System.Text.StringBuilder __SB__, System.Collections.Hashtable HT) {
 * 		__SB__.Append("\r");
 * 		__SB__.Append("\n");
 * 		__SB__.Append("The Hashtable contains ");
 * 		__SB__.Append("" + (HT.Count));
 * 		__SB__.Append(" entries.\r");
 * 		__SB__.Append("\n");
 * 		__SB__.Append("The key->value of the entries is:\r");
 * 		__SB__.Append("\n");
 * 		__SB__.Append("\r");
 * 		__SB__.Append("\n");
 *         foreach (DictionaryEntry DE in HT) {
 * 		    __SB__.Append("" + (DE.Key));
 * 	    	__SB__.Append(" -> ");
 * 		    __SB__.Append("" + (DE.Value));
 * 	    	__SB__.Append(".\r");
 * 		    __SB__.Append("\n");
 *         }
 *         __SB__.Append("\r");
 *         __SB__.Append("\n");
 * 		__SB__.Append("That's all\r");
 * 		__SB__.Append("\n");
 *     }
 * } // end of class
 * } // end of namespace
 * </code>
 * 
 * 
 * \section cogsharp_dll__sec Requirement on the location of the cogsharp.dll file.
 * 
 * The <c>cogsharp.dll</c> file must be in the same directory where the entry assembly (<c>.exe</c>)
 * resides. This is because CoG needs give the absolute path as an assembly reference to the compiler when
 * it compiles a template. To do: maybe a neater solution for this problem exists?
 * 
 * \section emitted_sec The emitted text.
 * 
 * When the <c>EmitTemplate()</c> function is used without a <c>StringBuilder</c> argument, 
 * the emitted text is stored inside the CoG class. To retrieve it use <c>CoGsharp.CoG.GetOutput()</c>. 
 * This member function returns the output so far, but does not clear it.
 * So on the next call, you'd get the new output plus the previous output.
 * 
 * To clear the internal output storage, use CoGsharp.CoG.ClearOutput(). An alternative is to use function 
 * <c>CoGsharp.CoG.GetOutputAndClear()</c>, which both returns the output and clears the internal 
 * output storage.
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace CoGsharp
{
    /// <summary>
    /// Interface for all code generation classes generated by CoG.
    /// The CoG class generates these classes in a temp directory.
    /// It uses GetTemplate() and GetArguments() to determine if it
    /// has found the right class (because classes are 'found' by hashcode).
    /// 
    /// CoGclasses also have an Emit() method, but because the arguments are
    /// not fixed, this method cannot be part of this interface. It is called using
    /// InvokeMember() instead.
    /// </summary>
    public interface CoGclass
    {
        /** returns the template code that this class generates */
        String GetTemplate();

        /** returns a string that represents the arguments used to generate this class */
        String GetArguments();
    }

    /// <summary>
    /// This is the main class of the package. You should instantiate it, and then load
    /// templates using LoadTemplates(). Multiple template files, streams or strings
    /// can be loaded. They are stored internally in the class. They are not analyzed
    /// until they are actually used.
    /// 
    /// To emit a template, use one of the variant of EmitTemplate().
    /// It should be called using a StringBuilder where the result should be stored,
    /// and the user defined arguments. The arguments are pairs of argument name
    /// and argument Object. The Object can have any type.
    /// 
    /// To emit a template, CoG turns the template into executable source code
    /// (the actual code depends on the types of the arguments) and compiles
    /// this source code using the C# compiler. Compiled classes are stored in the
    /// system-wide temporary directory in a subdirectory called cogsharp.
    /// 
    /// The generated classes are then invoked to generate the actual code,
    /// which is written to the StringBuilder.
    /// </summary>
    public class CoG
    {
        /// <summary>
        /// The only constructor. Checks for the system temporary directory
        /// and tries to create a 'cogsharp' directory in it, if it does not already exist.
        /// </summary>
        public CoG()
        {
            m_output = new StringBuilder();
            m_templates = new System.Collections.Hashtable();
            m_loadedCoGclasses = new System.Collections.Hashtable();

            m_tempDir = System.IO.Path.GetTempPath() + "cogsharp";
//			Console.WriteLine("CoG temp dir: " + m_tempDir);
			
            if (!System.IO.Directory.Exists(m_tempDir))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(m_tempDir);
                }
                catch (System.Exception)
                {
                    throw new Exception("CoGsharp.CoGsharp(): could not create temporary directory " + m_tempDir);
                }
            }
        }

        /// <summary>
        /// Returns all emitted output since the past call to ClearOutput() or GetOutputAndClear().
        /// </summary>
        public String GetOutput()
        {
            return m_output.ToString();
        }

        /// <summary>
        /// Clears m_output
        /// </summary>
        public void ClearOutput()
        {
            m_output.Remove(0, m_output.Length);
        }

        /// <summary>
        /// Returns all emitted output since the past call to ClearOutput() or GetOutputAndClear(),
        /// and clears m_output
        /// </summary>
        public String GetOutputAndClear()
        {
            String tmpStr = GetOutput();
            ClearOutput();
            return tmpStr;
        }

        /// <summary>
        ///  Emits template called 'templateName' with given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        public void EmitTemplate(String templateName, String arg1Name, Object arg1)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(arg1Name, arg1);
            EmitTemplate(templateName, argTable);
        }

        /// <summary>
        ///  Emits template called 'templateName' with given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        /// <param name="arg2Name">Name of argument 2 (may optionally end in '=')</param>
        /// <param name="arg2">argument 2</param>
        public void EmitTemplate(String templateName,
            String arg1Name, Object arg1, String arg2Name, Object arg2)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(
                arg1Name, arg1, arg2Name, arg2);
            EmitTemplate(templateName, argTable);
        }

        /// <summary>
        ///  Emits template called 'templateName' with given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        /// <param name="arg2Name">Name of argument 2 (may optionally end in '=')</param>
        /// <param name="arg2">argument 2</param>
        /// <param name="arg3Name">Name of argument 3 (may optionally end in '=')</param>
        /// <param name="arg3">argument 3</param>
        public void EmitTemplate(String templateName,
            String arg1Name, Object arg1, String arg2Name, Object arg2,
            String arg3Name, Object arg3)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(
                arg1Name, arg1, arg2Name, arg2,
                arg3Name, arg3);
            EmitTemplate(templateName, argTable);
        }

        /// <summary>
        ///  Emits template called 'templateName' with given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        /// <param name="arg2Name">Name of argument 2 (may optionally end in '=')</param>
        /// <param name="arg2">argument 2</param>
        /// <param name="arg3Name">Name of argument 3 (may optionally end in '=')</param>
        /// <param name="arg3">argument 3</param>
        /// <param name="arg4Name">Name of argument 4 (may optionally end in '=')</param>
        /// <param name="arg4">argument 4</param>
        public void EmitTemplate(String templateName,
            String arg1Name, Object arg1, String arg2Name, Object arg2,
            String arg3Name, Object arg3, String arg4Name, Object arg4)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(
                arg1Name, arg1, arg2Name, arg2,
                arg3Name, arg3, arg4Name, arg4);
            EmitTemplate(templateName, argTable);
        }

        /// <summary>
        ///  Emits template called 'templateName' with given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        /// <param name="arg2Name">Name of argument 2 (may optionally end in '=')</param>
        /// <param name="arg2">argument 2</param>
        /// <param name="arg3Name">Name of argument 3 (may optionally end in '=')</param>
        /// <param name="arg3">argument 3</param>
        /// <param name="arg4Name">Name of argument 4 (may optionally end in '=')</param>
        /// <param name="arg4">argument 4</param>
        /// <param name="arg5Name">Name of argument 5 (may optionally end in '=')</param>
        /// <param name="arg5">argument 5</param>
        public void EmitTemplate(String templateName,
            String arg1Name, Object arg1, String arg2Name, Object arg2,
            String arg3Name, Object arg3, String arg4Name, Object arg4,
            String arg5Name, Object arg5)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(
                arg1Name, arg1, arg2Name, arg2,
                arg3Name, arg3, arg4Name, arg4,
                arg5Name, arg5);
            EmitTemplate(templateName, argTable);
        }

        /// <summary>
        ///  Emits template called 'templateName' with given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        /// <param name="arg2Name">Name of argument 2 (may optionally end in '=')</param>
        /// <param name="arg2">argument 2</param>
        /// <param name="arg3Name">Name of argument 3 (may optionally end in '=')</param>
        /// <param name="arg3">argument 3</param>
        /// <param name="arg4Name">Name of argument 4 (may optionally end in '=')</param>
        /// <param name="arg4">argument 4</param>
        /// <param name="arg5Name">Name of argument 5 (may optionally end in '=')</param>
        /// <param name="arg5">argument 5</param>
        /// <param name="arg6Name">Name of argument 6 (may optionally end in '=')</param>
        /// <param name="arg6">argument 6</param>
        public void EmitTemplate(String templateName,
            String arg1Name, Object arg1, String arg2Name, Object arg2,
            String arg3Name, Object arg3, String arg4Name, Object arg4,
            String arg5Name, Object arg5, String arg6Name, Object arg6)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(
                arg1Name, arg1, arg2Name, arg2,
                arg3Name, arg3, arg4Name, arg4,
                arg5Name, arg5, arg6Name, arg6);
            EmitTemplate(templateName, argTable);
        }

        /// <summary>
        ///  Emits template called 'templateName' with given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        /// <param name="arg2Name">Name of argument 2 (may optionally end in '=')</param>
        /// <param name="arg2">argument 2</param>
        /// <param name="arg3Name">Name of argument 3 (may optionally end in '=')</param>
        /// <param name="arg3">argument 3</param>
        /// <param name="arg4Name">Name of argument 4 (may optionally end in '=')</param>
        /// <param name="arg4">argument 4</param>
        /// <param name="arg5Name">Name of argument 5 (may optionally end in '=')</param>
        /// <param name="arg5">argument 5</param>
        /// <param name="arg6Name">Name of argument 6 (may optionally end in '=')</param>
        /// <param name="arg6">argument 6</param>
        /// <param name="arg7Name">Name of argument 7 (may optionally end in '=')</param>
        /// <param name="arg7">argument 7</param>
        public void EmitTemplate(String templateName,
            String arg1Name, Object arg1, String arg2Name, Object arg2,
            String arg3Name, Object arg3, String arg4Name, Object arg4,
            String arg5Name, Object arg5, String arg6Name, Object arg6,
            String arg7Name, Object arg7)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(
                arg1Name, arg1, arg2Name, arg2,
                arg3Name, arg3, arg4Name, arg4,
                arg5Name, arg5, arg6Name, arg6,
                arg7Name, arg7);
            EmitTemplate(templateName, argTable);
        }


        /// <summary>
        ///  Emits template called 'templateName' with given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        /// <param name="arg2Name">Name of argument 2 (may optionally end in '=')</param>
        /// <param name="arg2">argument 2</param>
        /// <param name="arg3Name">Name of argument 3 (may optionally end in '=')</param>
        /// <param name="arg3">argument 3</param>
        /// <param name="arg4Name">Name of argument 4 (may optionally end in '=')</param>
        /// <param name="arg4">argument 4</param>
        /// <param name="arg5Name">Name of argument 5 (may optionally end in '=')</param>
        /// <param name="arg5">argument 5</param>
        /// <param name="arg6Name">Name of argument 6 (may optionally end in '=')</param>
        /// <param name="arg6">argument 6</param>
        /// <param name="arg7Name">Name of argument 7 (may optionally end in '=')</param>
        /// <param name="arg7">argument 7</param>
        /// <param name="arg8Name">Name of argument 8 (may optionally end in '=')</param>
        /// <param name="arg8">argument 8</param>
        public void EmitTemplate(String templateName, 
            String arg1Name, Object arg1, String arg2Name, Object arg2,
            String arg3Name, Object arg3, String arg4Name, Object arg4,
            String arg5Name, Object arg5, String arg6Name, Object arg6,
            String arg7Name, Object arg7, String arg8Name, Object arg8)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(
                arg1Name, arg1, arg2Name, arg2, 
                arg3Name, arg3, arg4Name, arg4, 
                arg5Name, arg5, arg6Name, arg6, 
                arg7Name, arg7, arg8Name, arg8);
            EmitTemplate(templateName, argTable);
        }

        /// <summary>
        ///  Emits to the StringBuilder 'outputSB' the template called 'templateName', using given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="outputSB">Where the output goes.</param>
        public void EmitTemplate(StringBuilder outputSB, String templateName)
        {
            EmitTemplate(outputSB, templateName, new System.Collections.Hashtable());
        }

        /// <summary>
        ///  Emits to the StringBuilder 'outputSB' the template called 'templateName', using given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="outputSB">Where the output goes.</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        public void EmitTemplate(StringBuilder outputSB, String templateName, String arg1Name, Object arg1)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(arg1Name, arg1);
            EmitTemplate(outputSB, templateName, argTable);
        }

        /// <summary>
        ///  Emits to the StringBuilder 'outputSB' the template called 'templateName', using given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="outputSB">Where the output goes.</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        /// <param name="arg2Name">Name of argument 2 (may optionally end in '=')</param>
        /// <param name="arg2">argument 2</param>
        public void EmitTemplate(StringBuilder outputSB, String templateName,
            String arg1Name, Object arg1, String arg2Name, Object arg2)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(
                arg1Name, arg1, arg2Name, arg2);
            EmitTemplate(outputSB, templateName, argTable);
        }

        /// <summary>
        ///  Emits to the StringBuilder 'outputSB' the template called 'templateName', using given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="outputSB">Where the output goes.</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        /// <param name="arg2Name">Name of argument 2 (may optionally end in '=')</param>
        /// <param name="arg2">argument 2</param>
        /// <param name="arg3Name">Name of argument 3 (may optionally end in '=')</param>
        /// <param name="arg3">argument 3</param>
        public void EmitTemplate(StringBuilder outputSB, String templateName,
            String arg1Name, Object arg1, String arg2Name, Object arg2,
            String arg3Name, Object arg3)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(
                arg1Name, arg1, arg2Name, arg2,
                arg3Name, arg3);
            EmitTemplate(outputSB, templateName, argTable);
        }

        /// <summary>
        ///  Emits to the StringBuilder 'outputSB' the template called 'templateName', using given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="outputSB">Where the output goes.</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        /// <param name="arg2Name">Name of argument 2 (may optionally end in '=')</param>
        /// <param name="arg2">argument 2</param>
        /// <param name="arg3Name">Name of argument 3 (may optionally end in '=')</param>
        /// <param name="arg3">argument 3</param>
        /// <param name="arg4Name">Name of argument 4 (may optionally end in '=')</param>
        /// <param name="arg4">argument 4</param>
        public void EmitTemplate(StringBuilder outputSB, String templateName,
            String arg1Name, Object arg1, String arg2Name, Object arg2,
            String arg3Name, Object arg3, String arg4Name, Object arg4)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(
                arg1Name, arg1, arg2Name, arg2,
                arg3Name, arg3, arg4Name, arg4);
            EmitTemplate(outputSB, templateName, argTable);
        }

        /// <summary>
        ///  Emits to the StringBuilder 'outputSB' the template called 'templateName', using given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="outputSB">Where the output goes.</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        /// <param name="arg2Name">Name of argument 2 (may optionally end in '=')</param>
        /// <param name="arg2">argument 2</param>
        /// <param name="arg3Name">Name of argument 3 (may optionally end in '=')</param>
        /// <param name="arg3">argument 3</param>
        /// <param name="arg4Name">Name of argument 4 (may optionally end in '=')</param>
        /// <param name="arg4">argument 4</param>
        /// <param name="arg5Name">Name of argument 5 (may optionally end in '=')</param>
        /// <param name="arg5">argument 5</param>
        public void EmitTemplate(StringBuilder outputSB, String templateName,
            String arg1Name, Object arg1, String arg2Name, Object arg2,
            String arg3Name, Object arg3, String arg4Name, Object arg4,
            String arg5Name, Object arg5)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(
                arg1Name, arg1, arg2Name, arg2,
                arg3Name, arg3, arg4Name, arg4,
                arg5Name, arg5);
            EmitTemplate(outputSB, templateName, argTable);
        }

        /// <summary>
        ///  Emits to the StringBuilder 'outputSB' the template called 'templateName', using given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="outputSB">Where the output goes.</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        /// <param name="arg2Name">Name of argument 2 (may optionally end in '=')</param>
        /// <param name="arg2">argument 2</param>
        /// <param name="arg3Name">Name of argument 3 (may optionally end in '=')</param>
        /// <param name="arg3">argument 3</param>
        /// <param name="arg4Name">Name of argument 4 (may optionally end in '=')</param>
        /// <param name="arg4">argument 4</param>
        /// <param name="arg5Name">Name of argument 5 (may optionally end in '=')</param>
        /// <param name="arg5">argument 5</param>
        /// <param name="arg6Name">Name of argument 6 (may optionally end in '=')</param>
        /// <param name="arg6">argument 6</param>
        public void EmitTemplate(StringBuilder outputSB, String templateName,
            String arg1Name, Object arg1, String arg2Name, Object arg2,
            String arg3Name, Object arg3, String arg4Name, Object arg4,
            String arg5Name, Object arg5, String arg6Name, Object arg6)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(
                arg1Name, arg1, arg2Name, arg2,
                arg3Name, arg3, arg4Name, arg4,
                arg5Name, arg5, arg6Name, arg6);
            EmitTemplate(outputSB, templateName, argTable);
        }

        /// <summary>
        ///  Emits to the StringBuilder 'outputSB' the template called 'templateName', using given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="outputSB">Where the output goes.</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        /// <param name="arg2Name">Name of argument 2 (may optionally end in '=')</param>
        /// <param name="arg2">argument 2</param>
        /// <param name="arg3Name">Name of argument 3 (may optionally end in '=')</param>
        /// <param name="arg3">argument 3</param>
        /// <param name="arg4Name">Name of argument 4 (may optionally end in '=')</param>
        /// <param name="arg4">argument 4</param>
        /// <param name="arg5Name">Name of argument 5 (may optionally end in '=')</param>
        /// <param name="arg5">argument 5</param>
        /// <param name="arg6Name">Name of argument 6 (may optionally end in '=')</param>
        /// <param name="arg6">argument 6</param>
        /// <param name="arg7Name">Name of argument 7 (may optionally end in '=')</param>
        /// <param name="arg7">argument 7</param>
        public void EmitTemplate(StringBuilder outputSB, String templateName,
            String arg1Name, Object arg1, String arg2Name, Object arg2,
            String arg3Name, Object arg3, String arg4Name, Object arg4,
            String arg5Name, Object arg5, String arg6Name, Object arg6,
            String arg7Name, Object arg7)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(
                arg1Name, arg1, arg2Name, arg2,
                arg3Name, arg3, arg4Name, arg4,
                arg5Name, arg5, arg6Name, arg6,
                arg7Name, arg7);
            EmitTemplate(outputSB, templateName, argTable);
        }


        /// <summary>
        ///  Emits to the StringBuilder 'outputSB' the template called 'templateName', using given arguments.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="outputSB">Where the output goes.</param>
        /// <param name="arg1Name">Name of argument 1 (may optionally end in '=')</param>
        /// <param name="arg1">argument 1</param>
        /// <param name="arg2Name">Name of argument 2 (may optionally end in '=')</param>
        /// <param name="arg2">argument 2</param>
        /// <param name="arg3Name">Name of argument 3 (may optionally end in '=')</param>
        /// <param name="arg3">argument 3</param>
        /// <param name="arg4Name">Name of argument 4 (may optionally end in '=')</param>
        /// <param name="arg4">argument 4</param>
        /// <param name="arg5Name">Name of argument 5 (may optionally end in '=')</param>
        /// <param name="arg5">argument 5</param>
        /// <param name="arg6Name">Name of argument 6 (may optionally end in '=')</param>
        /// <param name="arg6">argument 6</param>
        /// <param name="arg7Name">Name of argument 7 (may optionally end in '=')</param>
        /// <param name="arg7">argument 7</param>
        /// <param name="arg8Name">Name of argument 8 (may optionally end in '=')</param>
        /// <param name="arg8">argument 8</param>
        public void EmitTemplate(StringBuilder outputSB, String templateName,
            String arg1Name, Object arg1, String arg2Name, Object arg2,
            String arg3Name, Object arg3, String arg4Name, Object arg4,
            String arg5Name, Object arg5, String arg6Name, Object arg6,
            String arg7Name, Object arg7, String arg8Name, Object arg8)
        {
            // get table of arguments
            System.Collections.Hashtable argTable = GetHashtableOfArguments(
                arg1Name, arg1, arg2Name, arg2,
                arg3Name, arg3, arg4Name, arg4,
                arg5Name, arg5, arg6Name, arg6,
                arg7Name, arg7, arg8Name, arg8);
            EmitTemplate(outputSB, templateName, argTable);
        }

        /// <summary>
        /// Emits template called 'templateName' with given arguments.
        /// The results go into m_output.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="argTable">Hashtable contains argument keys, Object value</param>
        public void EmitTemplate(String templateName, System.Collections.Hashtable argTable)
        {
            EmitTemplate(m_output, templateName, argTable);
        }

        /// <summary>
        /// Emits template called 'templateName' with given arguments.
        /// The results are written to 'outputSB'.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="argTable">Hashtable contains argument keys, Object value</param>
        /// <param name="outputSB">Where the output goes.</param>
        public void EmitTemplate(StringBuilder outputSB, String templateName, System.Collections.Hashtable argTable)
        {
            // get template
            if (!m_templates.Contains(templateName))
                throw new Exception("CoGsharp.EmitTemplate(): no template named " + templateName);
            String templateString = (String)m_templates[templateName];

            // get argument description string 
            String argString = GenerateArgsString(argTable);

            // get hashcode for template + arguments
            uint hashCode = (uint)(templateString.GetHashCode() ^ argString.GetHashCode());

            // get class (from loaded or from disc) 
            CoGclass CG = GetCoGclass(templateName, templateString, argString, hashCode);

            // get array of arguments
            Object[] argArray = GetArgArray(argTable, outputSB);
            CG.GetType().InvokeMember("Emit", BindingFlags.InvokeMethod, null, CG, argArray);
        }

        /// <summary>
        /// Gets the CoG class which corresponds to the arguments.
        /// Either gets it from the m_loadedCoGclasses, from disc, or compiles it in memory (and stores on disc, in cache)
        /// </summary>
        /// <param name="templateString">The full contents of the template</param>
        /// <param name="argString">The argument string to be used with the template (from GenerateArgsString()) </param>
        /// <param name="hashCode">Combined hashcode for both templateString and argString</param>
        /// <returns>the requested class</returns>
        private CoGclass GetCoGclass(String templateName, String templateString, String argString, uint hashCode)
        {
            // look in cache first
            lock(m_loadedCoGclasses) {
                uint h = hashCode;
                while (m_loadedCoGclasses.Contains(h))
                {
                    CoGclass CG = (CoGclass)m_loadedCoGclasses[h];
                    if ((CG.GetTemplate() == templateString) &&
                        (CG.GetArguments() == argString))
                        return CG;
                    h++; // not found, continue
                }
            }

            // then try compiled assemblies in the temp dir:
            {
                uint h = hashCode;
                while (true)
                { // while we can continue loading assemblies
                    String filename = GetCoGclassFilename(h);
                    try
                    {
                        Assembly ass = Assembly.LoadFile(filename);
                        string className = "CoGgenerated.CoG_" + h.ToString();
                        CoGclass CG = (CoGclass)ass.CreateInstance(className);
                        if ((CG != null) &&
                            (CG.GetTemplate() == templateString) &&
                            (CG.GetArguments() == argString))
                        {
                            StoreInLoadedCoGclasses(h, CG);
                            return CG;
                        }
                        else
                        {
                            h++;
                            continue;
                        }
                    }
                    catch (System.Exception)
                    {
                        break;
                    }
                }
            }

            // try to generate class
            return GenerateCoGclass(templateName, templateString, argString, hashCode);
        }


        private String GetCoGclassFilename(uint hashCode)
        {
            return m_tempDir + System.IO.Path.DirectorySeparatorChar + "cogclass_" + hashCode.ToString() + ".dll";
        }
        private String GetCoGcsFilename(uint hashCode)
        {
            return m_tempDir + System.IO.Path.DirectorySeparatorChar + "cogclass_" + hashCode.ToString() + ".cs";
        }

        /// <summary>
        /// Stores 'CG' in m_loadedCoGclasses at position h or higher;
        /// </summary>
        /// <param name="hashCode"></param>
        private void StoreInLoadedCoGclasses(uint hashCode, CoGclass CG)
        {
            // skip all positions that are taken:
            lock (m_loadedCoGclasses)
            {
                while (m_loadedCoGclasses.Contains(hashCode))
                {
                    hashCode++;
                }
                m_loadedCoGclasses[hashCode] = CG;
            }
        }

        private CoGclass GenerateCoGclass(String templateName, String templateString, String argString, uint hashCode)
        {
            // first find empty position for hashcode
            String csFilename;

            // find a hashCode which is 'free', starting at user-supplied hashcode
            lock (m_generatedHashCodes)
            {
                while (true)
                {
                    csFilename = GetCoGcsFilename(hashCode);

                    // file must not exist and hash code must not have been used yet (this avoids multi-threading problems).
                    bool hashIsFree = !m_generatedHashCodes.ContainsKey(hashCode);
                    bool fileIsFree = !System.IO.File.Exists(csFilename);
                    if (hashIsFree && fileIsFree) break;
                    else hashCode++;
                }
                m_generatedHashCodes[hashCode] = hashCode;
            }

            String dllFilename = GetCoGclassFilename(hashCode);

            // build code:
            String className = "CoG_" + hashCode.ToString();
            StringBuilder code = new StringBuilder();
            code.Append("// Generated by CoGsharp from template '" + templateName + "' on " + DateTime.Now.ToString() + "\n");
            code.Append("using System;\n");
            code.Append("using CoGsharp;\n");
            EmitTemplateCodeOrImport(code, templateString, EMIT_TYPE.EMIT_USING);

            code.Append("namespace CoGgenerated {\n");
            //            code.Append("public class " + className + " : CoGsharp.CoGclass {\n");
            code.Append("public class " + className + " : CoGsharp.CoGclass");
            EmitTemplateCodeOrImport(code, templateString, EMIT_TYPE.EMIT_IMPLEMENTS);
            code.Append(" {\n");
            code.Append("\tpublic " + className + "() {\n");
            code.Append("\t}\n");

            code.Append("\tpublic String GetTemplate() {\n");
            code.Append("\t\treturn\n" + LongStringToCode(templateString, 3) + "\t\t\t;\n");
            code.Append("\t}\n");


            code.Append("\tpublic String GetArguments() {\n");
            code.Append("\t\treturn\n" + LongStringToCode(argString, 3) + "\t\t\t;\n");
            code.Append("\t}\n");

            code.Append("\tpublic void Emit(System.Text.StringBuilder __SB__" + argString + ") {\n");
            EmitTemplateCodeOrImport(code, templateString, EMIT_TYPE.EMIT_CODE);
            code.Append("\t}\n");

            code.Append("} // end of class\n");
            code.Append("} // end of namespace\n");

            { // write code to .cs file (only for user debugging; not required for actual compiling)
                try { System.IO.File.Delete(csFilename); }
                catch (System.Exception) { }
                StreamWriter W = new StreamWriter(csFilename);
                W.Write(code.ToString());
                W.Close();
            }

			Assembly ass = null;
			lock (s_compilerLock) {
           		ICodeCompiler compiler = new CSharpCodeProvider().CreateCompiler();
	            CompilerParameters compilerParameters = new CompilerParameters();
	            compilerParameters.ReferencedAssemblies.Add("System.dll");
	            String CoGassemblyName = GetType().Assembly.Location;
	
	            compilerParameters.ReferencedAssemblies.Add(CoGassemblyName);
	
	            foreach (String str in m_referencedAssemblies)
	                compilerParameters.ReferencedAssemblies.Add(str);
	
	            compilerParameters.GenerateInMemory = false;
	            compilerParameters.OutputAssembly = dllFilename;
	
	            CompilerResults compileResults = compiler.CompileAssemblyFromSource(compilerParameters, code.ToString());
	            if (compileResults.Errors.HasErrors)
	            {
	                string errorMsg = "Errors while compiling " + csFilename + "(template " + templateName + ")\n";
	                errorMsg = errorMsg + "The errors are:";
	                for (int x = 0; x < compileResults.Errors.Count; x++)
	                    errorMsg = errorMsg + "\r\nLine: " +
	                                 compileResults.Errors[x].Line.ToString() + " - " +
	                                 compileResults.Errors[x].ErrorText;
	
	                throw new Exception(errorMsg);
	            }
	
	            ass = compileResults.CompiledAssembly;
			} // end of lock(s_compiler)
	
            Object O = ass.CreateInstance("CoGgenerated." + className);
            CoGclass CG = (CoGclass)O;
            StoreInLoadedCoGclasses(hashCode, CG);
			
        	return CG;

        }

        enum EMIT_TYPE { EMIT_CODE = 1, EMIT_USING = 2, EMIT_IMPLEMENTS = 3 };

        /// <summary>
        /// Called multiple times by EmitTemplateCode, EmitTemplateUsing, EmitTemplateImplements.
        /// Scans the entire template and emits either regular code, import directives or interface implements
        /// </summary>
        static private void EmitTemplateCodeOrImport(StringBuilder SB, String str, EMIT_TYPE what)
        {
            // search for <%  %> \<\%  \%\>
            // correct \<\% to <%  and \%\> to %>
            int idx = 0;
            while (idx < str.Length)
            {
                // find the next code block"
                int ncbIdx = str.IndexOf("<%", idx);
                // find the next escape sequence:
                int nesIdx1 = str.IndexOf("\\<\\%", idx);
                int nesIdx2 = str.IndexOf("\\%\\>", idx);
                if ((nesIdx2 >= 0) && ((nesIdx1 < 0) || (nesIdx2 < nesIdx1))) nesIdx1 = nesIdx2;

                // find next character that will go bad if emited with an escape \
                int neIdx = -1;
                char neChar = ' ';
                for (int i = 0; i < m_escape.Length; i += 2)
                {
                    int j = str.IndexOf(m_escape[i], idx);
                    if ((j >= 0) && ((neIdx < 0) || (j < neIdx)))
                    {
                        neChar = m_escape[i + 1];
                        neIdx = j;
                    }
                }

                // emit code . . .
                if ((neIdx >= 0) &&
                    ((nesIdx1 < 0) || (neIdx < nesIdx1)) &&
                    ((ncbIdx < 0) || (neIdx < ncbIdx)))
                {
                    if (what == EMIT_TYPE.EMIT_CODE)
                    {
                        // emit code up to escape sequence, emit corrected escape sequence
                        SB.Append("\t\t__SB__.Append(\"" + str.Substring(idx, neIdx - idx) + "\\" + neChar + "\");\n");
                    }
                    idx = neIdx + 1;
                }
                else if ((nesIdx1 >= 0) && ((ncbIdx < 0) || (neIdx < ncbIdx)))
                {
                    // emit code up to escape sequence, emit corrected escape sequence
                    if (what == EMIT_TYPE.EMIT_CODE)
                    {
                        if (nesIdx1 > idx)
                            SB.Append("\t\t__SB__.Append(\"" + str.Substring(idx, nesIdx1 - idx) + "\");\n");
                        SB.Append("\t\t__SB__.Append(\"" + ((str[nesIdx1 + 1] == '<') ? "<%" : "%>") + "\");\n");
                    }
                    idx = nesIdx1 + "\\<\\%".Length;
                }
                else if (ncbIdx >= 0)
                {
                    // emit up to code block, emit code
                    if (what == EMIT_TYPE.EMIT_CODE)
                        SB.Append("\t\t__SB__.Append(\"" + str.Substring(idx, ncbIdx - idx) + "\");\n");

                    // get code block
                    ncbIdx += "<%".Length; // skip over <%
                    int ecbIdx = str.IndexOf("%>", ncbIdx);
                    if (ecbIdx < 0) ecbIdx = str.Length;
                    String code = str.Substring(ncbIdx, ecbIdx - ncbIdx);

                    // if the code returns void, emit it differently than when it returns a value
                    if (what == EMIT_TYPE.EMIT_USING)
                    {
                        if (IsUsing(code))
                        {
                            SB.Append(code + ";\n");
                        }
                    }
                    else if (what == EMIT_TYPE.EMIT_IMPLEMENTS)
                    {
                        if (IsImplements(code))
                        {
                            String tmp = code.Substring(code.IndexOf("implements") + "implements".Length + 1);
                            SB.Append(", " + tmp);
                        }
                    }
                    else if (what == EMIT_TYPE.EMIT_CODE)
                    {
                        if (IsUsing(code)) { }
                        else if (IsImplements(code)) { }
                        else if (ReturnsVoid(code)) SB.Append(code);
                        else SB.Append("\t\t__SB__.Append(\"\" + (" + code + "));\n");
                    }
                    idx = ecbIdx + "%>".Length;

                    // skip over carriage return directly following a code block.
                    if ((str.Length > idx) && (str[idx] == 13)) idx++;
                    if ((str.Length > idx) && (str[idx] == 10)) idx++;
                }
                else
                {
                    // emit all
                    if (what == EMIT_TYPE.EMIT_CODE)
                        SB.Append("\t\t__SB__.Append(\"" + str.Substring(idx) + "\");\n");
                    idx = str.Length; // done . . .
                }
            }
        }


        /// <returns>true if 'code' is a using directive</returns>
        private static bool IsUsing(String code)
        {
            if (code.Length == 0) return false;

            int idx = 0;

            // skip whitespace:
            while ((idx < code.Length) && (code[idx] <= ' ')) idx++;

            // detect { or }
            if ((code[idx] == '{') || (code[idx] == '}'))
                return false;

            // get the first token
            if (Char.IsLetter(code[idx]))
            {
                int tidx = idx + 1;
                while ((tidx < code.Length) &&
                Char.IsLetter(code[tidx])) tidx++;
                String token = code.Substring(idx, tidx - idx);
                if (token == "using") return true;
            }
            return false;
        }

        /// <returns>true if 'code' is a implements directive</returns>
        private static bool IsImplements(String code)
        {
            if (code.Length == 0) return false;

            int idx = 0;

            // skip whitespace:
            while ((idx < code.Length) && (code[idx] <= ' ')) idx++;

            // detect { or }
            if ((code[idx] == '{') || (code[idx] == '}'))
                return false;

            // get the first token
            if (Char.IsLetter(code[idx]))
            {
                int tidx = idx + 1;
                while ((tidx < code.Length) &&
                Char.IsLetter(code[tidx])) tidx++;
                String token = code.Substring(idx, tidx);
                if (token == "implements") return true;
            }
            return false;
        }

        private static bool IsCSharpIdentifierStart(char c)
        {
            return Char.IsLetter(c) || ("_".IndexOf(c) >= 0);
        }
        private static bool IsCSharpIdentifierPart(char c)
        {
            return Char.IsLetterOrDigit(c) || ("_".IndexOf(c) >= 0);
        }



        /// <returns>true if 'code' would return void</returns>
        private static bool ReturnsVoid(String code)
        {
            // detects for, do, while, if, else, case, switch, break, default, throw, catch, {, }
            if (code.Length == 0) return false;

            int idx = 0;

            while (true)
            {
                // skip whitespace and comments:
                while ((idx < code.Length) && (code[idx] <= ' ')) idx++;

                // detect comment "//" (and assume 'returns void')
                if ((code[idx] == '/') && (code.Length > idx + 1) && (code[idx] == '/'))
                    return true;

                    // detect and skip comment "/*"
                else if ((code[idx] == '/') && (code.Length > idx + 1) && (code[idx] == '*'))
                {
                    for (; idx < code.Length - 1; idx++)
                        if ((code[idx] == '*') && (code[idx + 1] == '/'))
                        {
                            idx += 2;
                            break;
                        }
                }

                else break;
            }


            // detect { or }
            if ((code[idx] == '{') || (code[idx] == '}'))
                return true;

            // detect ( or )
            if ((code[idx] == '(') || (code[idx] == ')'))
                return false;

            // get the first token
            if (IsCSharpIdentifierStart(code[idx]))
            {
                int tidx = idx + 1;
                while ((tidx < code.Length) &&
                    IsCSharpIdentifierPart(code[tidx])) tidx++;

                String token = code.Substring(idx, tidx - idx);

                // detect keywords
                if (token.Equals("import") ||
                    token.Equals("for") ||
                    token.Equals("foreach") ||
                    token.Equals("do") ||
                    token.Equals("while") ||
                    token.Equals("if") ||
                    token.Equals("else") ||
                    token.Equals("case") ||
                    token.Equals("switch") ||
                    token.Equals("break") ||
                    token.Equals("default") ||
                    token.Equals("try") ||
                    token.Equals("throw") ||
                    token.Equals("catch"))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Iterates over entries in 'H', returns a string "typeName1 argName1, typeName2 argName2, etc"
        /// </summary>
        private static String GenerateArgsString(System.Collections.Hashtable H)
        {
            StringBuilder SB = new StringBuilder();
            foreach (DictionaryEntry DE in H)
            {
                SB.Append(", ");
                SB.Append(DE.Value.GetType().ToString());
                SB.Append(" ");
                SB.Append(DE.Key.ToString());
            }
            return SB.ToString();
        }

        /// <summary>
        /// Iterates over entries in 'H', returns array of all values.
        /// ('outputSB' is the first entry of the returned array, so total length is H.Count+1)
        /// </summary>
        private static Object[] GetArgArray(System.Collections.Hashtable H, StringBuilder outputSB)
        {
            Object[] argArray = new Object[H.Count+1];
            argArray[0] = outputSB;
            int idx = 1;
            foreach (DictionaryEntry DE in H)
            {
                argArray[idx] = DE.Value;
                idx++;
            }
            return argArray;
        }

        /// <returns>a hashtable which contains key, value pairs of (a1Name, a1)</returns>
        private static System.Collections.Hashtable GetHashtableOfArguments(
            String a1Name, Object a1)
        {
            System.Collections.Hashtable M = new System.Collections.Hashtable();
            M[RemoveAssignSign(a1Name)] = a1;
            return M;
        }

        /// <returns>a hashtable which contains key, value pairs of (a1Name, a1)</returns>
        private static System.Collections.Hashtable GetHashtableOfArguments(
            String a1Name, Object a1, String a2Name, Object a2)
        {
            System.Collections.Hashtable M = new System.Collections.Hashtable();
            M[RemoveAssignSign(a1Name)] = a1;
            M[RemoveAssignSign(a2Name)] = a2;
            return M;
        }

        /// <returns>a hashtable which contains key, value pairs of (a1Name, a1)</returns>
        private static System.Collections.Hashtable GetHashtableOfArguments(
            String a1Name, Object a1, String a2Name, Object a2,
            String a3Name, Object a3)
        {
            System.Collections.Hashtable M = new System.Collections.Hashtable();
            M[RemoveAssignSign(a1Name)] = a1;
            M[RemoveAssignSign(a2Name)] = a2;
            M[RemoveAssignSign(a3Name)] = a3;
            return M;
        }

        /// <returns>a hashtable which contains key, value pairs of (a1Name, a1)</returns>
        private static System.Collections.Hashtable GetHashtableOfArguments(
            String a1Name, Object a1, String a2Name, Object a2,
            String a3Name, Object a3, String a4Name, Object a4)
        {
            System.Collections.Hashtable M = new System.Collections.Hashtable();
            M[RemoveAssignSign(a1Name)] = a1;
            M[RemoveAssignSign(a2Name)] = a2;
            M[RemoveAssignSign(a3Name)] = a3;
            M[RemoveAssignSign(a4Name)] = a4;
            return M;
        }

        /// <returns>a hashtable which contains key, value pairs of (a1Name, a1)</returns>
        private static System.Collections.Hashtable GetHashtableOfArguments(
            String a1Name, Object a1, String a2Name, Object a2,
            String a3Name, Object a3, String a4Name, Object a4,
            String a5Name, Object a5)
        {
            System.Collections.Hashtable M = new System.Collections.Hashtable();
            M[RemoveAssignSign(a1Name)] = a1;
            M[RemoveAssignSign(a2Name)] = a2;
            M[RemoveAssignSign(a3Name)] = a3;
            M[RemoveAssignSign(a4Name)] = a4;
            M[RemoveAssignSign(a5Name)] = a5;
            return M;
        }

        /// <returns>a hashtable which contains key, value pairs of (a1Name, a1)</returns>
        private static System.Collections.Hashtable GetHashtableOfArguments(
            String a1Name, Object a1, String a2Name, Object a2,
            String a3Name, Object a3, String a4Name, Object a4,
            String a5Name, Object a5, String a6Name, Object a6)
        {
            System.Collections.Hashtable M = new System.Collections.Hashtable();
            M[RemoveAssignSign(a1Name)] = a1;
            M[RemoveAssignSign(a2Name)] = a2;
            M[RemoveAssignSign(a3Name)] = a3;
            M[RemoveAssignSign(a4Name)] = a4;
            M[RemoveAssignSign(a5Name)] = a5;
            M[RemoveAssignSign(a6Name)] = a6;
            return M;
        }

        /// <returns>a hashtable which contains key, value pairs of (a1Name, a1)</returns>
        private static System.Collections.Hashtable GetHashtableOfArguments(
            String a1Name, Object a1, String a2Name, Object a2,
            String a3Name, Object a3, String a4Name, Object a4,
            String a5Name, Object a5, String a6Name, Object a6,
            String a7Name, Object a7)
        {
            System.Collections.Hashtable M = new System.Collections.Hashtable();
            M[RemoveAssignSign(a1Name)] = a1;
            M[RemoveAssignSign(a2Name)] = a2;
            M[RemoveAssignSign(a3Name)] = a3;
            M[RemoveAssignSign(a4Name)] = a4;
            M[RemoveAssignSign(a5Name)] = a5;
            M[RemoveAssignSign(a6Name)] = a6;
            M[RemoveAssignSign(a7Name)] = a7;
            return M;
        }

        /// <returns>a hashtable which contains key, value pairs of (a1Name, a1)</returns>
        private static System.Collections.Hashtable GetHashtableOfArguments(
            String a1Name, Object a1, String a2Name, Object a2,
            String a3Name, Object a3, String a4Name, Object a4,
            String a5Name, Object a5, String a6Name, Object a6,
            String a7Name, Object a7, String a8Name, Object a8)
        {
            System.Collections.Hashtable M = new System.Collections.Hashtable();
            M[RemoveAssignSign(a1Name)] = a1;
            M[RemoveAssignSign(a2Name)] = a2;
            M[RemoveAssignSign(a3Name)] = a3;
            M[RemoveAssignSign(a4Name)] = a4;
            M[RemoveAssignSign(a5Name)] = a5;
            M[RemoveAssignSign(a6Name)] = a6;
            M[RemoveAssignSign(a7Name)] = a7;
            M[RemoveAssignSign(a8Name)] = a8;
            return M;
        }



        /**
         * <summary>Removes anything trailing the last '=' sign in a string (including the '=' itself)</summary>
         * <returns>'str' with the '=' removed'</returns>
         * <remarks>This method is used to remove the '=' sign at the end of argument names which are passed
         * to EmitTemplate(). The user can call CoGsharp.EmitTemplate("MyTemplate", "argName1=", someObject) 
         * which should make the code a bit more readable.</remarks>
         */
        private static String RemoveAssignSign(String str)
        {
            int idx = str.LastIndexOf('=');
            if (idx >= 0)
            {
                idx--;
                while ((idx >= 0) && (Char.IsWhiteSpace(str[idx]))) idx--;
                return str.Substring(0, idx + 1);
            }
            else return str;
        }


        private static String LongStringToCode(String str, int nbTabs)
        {
            StringBuilder SB = new StringBuilder(str.Length * 2);
            int cnt = 0; // cnt = length of current line
            { // open first line
                for (int t = 0; t < nbTabs; t++)
                    SB.Append('\t');
                SB.Append('"');
            }

            // handle zero-length Strings
            if (str.Length == 0)
                SB.Append('"');

            // output chars:
            for (int i = 0; i < str.Length; i++)
            {
                // emit chars (but fix escape seqs)
                int j;
                for (j = 0; j < m_escape.Length; j += 2)
                {
                    if (str[i] == m_escape[j])
                    {
                        SB.Append('\\');
                        SB.Append(m_escape[j + 1]);
                        cnt += 2;
                        break;
                    }
                }
                if (j == m_escape.Length)
                {
                    SB.Append(str[i]);
                    cnt++;
                }

                // end line?
                if ((i == (str.Length - 1)) || (cnt > 60))
                {
                    cnt = 0;
                    SB.Append('"');
                    if (i != (str.Length - 1)) SB.Append(" + ");
                    SB.Append('\n');

                    // open next line, if required
                    if (i != (str.Length - 1))
                    {
                        for (int t = 0; t < nbTabs; t++)
                            SB.Append('\t');
                        SB.Append('"');
                    }
                }
            }
            return SB.ToString();
        }

        /// <summary>
        /// Loads templates from a file.
        /// </summary>
        /// <param name="filename">The name of the file</param>
        public void LoadTemplates(string filename)
        {
            //FileStream F = new System.IO.FileStream(filename, FileMode.Open, FileAccess.Read);
            StreamReader R;
            try
            {
                R = new StreamReader(filename);
            }
            catch (System.Exception)
            {
                throw new Exception("CoGsharp.LoadTemplates(): Error reading " + filename);
            }
            LoadTemplates(R, filename);
            R.Close();
        }

        /// <summary>
        /// Loads templates from a stream. The function first reads the entire stream and
        /// then processes the data by calling LoadTemplates(String S, String source).
        /// </summary>
        /// <param name="R">The stream</param>
        /// <param name="source">The source of the stream (e.g., the filename).</param>
        public void LoadTemplates(StreamReader R, String source)
        {
            StringBuilder SB = new StringBuilder();

            char[] buf = new char[1024];
            while (true)
            {
                int l = R.Read(buf, 0, buf.Length);
                if (l <= 0) break;
                else SB.Append(buf, 0, l);
            }

            if (SB.Length == 0) return;
            else LoadTemplates(SB.ToString(), source);
        }

        /// <summary>
        /// Loads templates from a string
        /// </summary>
        /// <param name="S">The template string.</param>
        /// <param name="source">The source of the string (e.g., the filename).</param>
        public void LoadTemplates(string S, string source)
        {
            int pidx = -1;
            int i = 0;
            int line = 1;
            string blockname = null;
            while (i < S.Length)
            {
                // find index of next code block:
                int nidx = S.IndexOf("${", i);   // nidx = ${
                if (nidx < 0)
                {
                    if (pidx >= 0) throw new Exception(source + ":line " + line + ": Excepted an ${ENDCODEBLOCK}");
                    return;
                }
                else
                {
                    // count lines
                    int j = i - 1;
                    while (((j = S.IndexOf('\n', j + 1)) >= 0) && (j < nidx)) line++;

                    // find closing }
                    nidx += 2;
                    int eidx = S.IndexOf('}', nidx);    // eidx = }
                    if (eidx < 0) throw new Exception(source + ":line " + line + ": Excepted a }");

                    // get first token following ${
                    j = nidx;   // find j, k such that [j ... k) = TOKEN
                    while ((j < eidx) && Char.IsWhiteSpace(S[j])) j++;
                    int k = j;
                    while ((k < eidx) && (!Char.IsWhiteSpace(S[k]))) k++;
                    if (k == j)
                    {
                        // ignore
                        i = eidx + 1;
                        continue;
                    }
                    string token = S.Substring(j, k - j);

                    if (token == "CODEBLOCK")
                    {
                        // got name of block
                        // [k ... l] = name
                        while ((k < eidx) && Char.IsWhiteSpace(S[k])) k++;
                        int l = eidx - 1;
                        while ((l > k) && Char.IsWhiteSpace(S[l])) l--;
                        if (k == l) throw new Exception(source + ":line " + line + ": Excepted a name inside ${CODEBLOCK}");
                        blockname = S.Substring(k, l - k + 1);

                        // set pidx;
                        pidx = eidx + 1;
                    }
                    else if (token == "ENDCODEBLOCK")
                    {
                        if (pidx < 0) throw new Exception(source + ":line " + line + ": ${ENDCODEBLOCK} without mathing ${CODEBLOCK}");
                        string code = S.Substring(pidx, nidx - pidx - 2);
                        m_templates[blockname] = code;

                        pidx = -1;
                        blockname = null;
                    }
                    else
                    {
                        // ignore . .
                    }

                    i = eidx + 1;
                }
            }
        }

        /// <summary>
        /// Adds a reference for compiler of generated code.
        /// </summary>
        /// <param name="path">Path to dll</param>
        public void AddReference(string path) 
        {
            m_referencedAssemblies.Add(path);
        }

        /// <summary>
        /// Removes a reference for compiler of generated code.
        /// </summary>
        /// <param name="path">Path to dll</param>
        public void RemoveReference(string path)
        {
            m_referencedAssemblies.Remove(path);
        }

        /// <summary>
        /// Contains all loaded raw templates; maps from String (template name) to String (template value)
        /// </summary>
        private System.Collections.Hashtable m_templates;

        /// <summary>
        /// Contains all loaded CoGclasses; maps from uint (hash code) to CoGclass
        /// </summary>
        private System.Collections.Hashtable m_loadedCoGclasses;


        /// <summary>
        /// When you call EmitTemplate, the output goes here. Use GetOutput() to retrieve,
        /// ClearOutput() to clear, and GetAndClearOutput() to do both.
        /// </summary>
        private StringBuilder m_output;

        /// <summary>
        /// Path to temp directory where all generated .cs and .dll files are stored (and searched).
        /// </summary>
        private String m_tempDir;

        /// <summary>
        /// List of assemblies that should be referenced by compiled code.
        /// 
        /// Use AddReference() and RemoveReference() to add/remove entries.
        /// </summary>
        private List<String> m_referencedAssemblies = new List<String>();

        /// <summary>
        /// Hashcodes for which .cs files have been generated. Used to avoid
        /// clashes when multiple threads use the same CogSharp at the same
        /// time.
        /// </summary>
        private Dictionary<UInt32, UInt32> m_generatedHashCodes = new Dictionary<UInt32, UInt32>();

        /// <summary>
        ///  Special characters and their escape codes
        /// </summary>
        static private char[] m_escape = {
	        (char)0x0A, 'n', // \n
	        (char)0x08, 'b', // \b
	        (char)0x0C, 'f', // \f
	        (char)0x0D, 'r', // \r
	        (char)0x09, 't', // \t
	        '\\', '\\', // \\
	        '"', '"' // \"
        };
		
		/// <summary>
		/// The object used to lock the compiler.
		/// Only one template can be compiled concurrently, because 
		/// compiling many templates at the same time led to crashes on Mono.
		/// </summary>
		private static Object s_compilerLock = new Object();


    } // end of class CoGsharp
} // end of namespace CoGsharp
