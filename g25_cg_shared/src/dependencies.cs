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

    public class DependencyException : Exception
    {
        public DependencyException(String str) : base(str) {}
    }
    
    /// <summary>
    /// Contains various utility functions.
    /// </summary>
    public class Dependencies
    {

        public Dependencies() { }


        /// <summary>
        /// Returns the name of a generated function (for example <c>gp_mv_mv</c>).
        /// If the function is not found, a DependencyException is thrown.
        /// 
        /// The function is found by looking through all G25.FGS in the specification.
        /// </summary>
        /// <param name="S">The spec.</param>
        /// <param name="functionName">Basic name of the function to be found.</param>
        /// <param name="argumentTypes">Names of the arguments types (not mangled).</param>
        /// <param name="FT">Floating point type.</param>
        /// <param name="metricName">(optional, can be null for don't care)</param>
        /// <returns>The mangled name of the function.</returns>
        public static String GetFunctionName(Specification S, String functionName, String[] argumentTypes, G25.FloatType FT, String metricName)
        {
            String returnTypeName = null;
            return GetFunctionName(S, functionName, argumentTypes, returnTypeName, FT, metricName);
        }

        /// <summary>
        /// Returns the name of a generated function (for example <c>gp_mv_mv</c>).
        /// If the function is not found, a DependencyException is thrown.
        /// 
        /// The function is found by looking through all G25.FGS in the specification.
        /// </summary>
        /// <param name="S">The spec.</param>
        /// <param name="functionName">Basic name of the function to be found.</param>
        /// <param name="argumentTypes">Names of the arguments types (not mangled).</param>
        /// <param name="returnTypeName">Name of the return type (can be null or "" for default return type).</param>
        /// <param name="FT">Floating point type.</param>
        /// <param name="metricName">(optional, can be null for don't care)</param>
        /// <returns>The mangled name of the function.</returns>
        public static string GetFunctionName(Specification S, string functionName, string[] argumentTypes, string returnTypeName, G25.FloatType FT, string metricName)
        {
            fgs F = S.FindFunctionEx(functionName, argumentTypes, returnTypeName, new String[] { FT.type }, metricName);

            if (F == null) // error: function not found
            {
                string exStr = "G25.CG.Shared.Util.GetFunctionName(): cannot find function " + functionName + " with arguments (";
                for (int i = 0; i < argumentTypes.Length; i++)
                {
                    if (i > 0) exStr = exStr + ", ";
                    exStr = exStr + argumentTypes[i];
                }
                exStr = exStr + ") and using floating point type " + FT.type;
                if (metricName != null)
                {
                    exStr = exStr + " and using metric " + metricName;
                }
                throw new DependencyException(exStr);
            }
            else
            {
                argumentTypes = F.ArgumentTypeNames;


                string mangledFuncName = F.OutputName;
                if (S.OutputC())
                {
                    // add mangled argument types to function name
                    string[] mangledArgumentTypes = new string[argumentTypes.Length];
                    for (int i = 0; i < argumentTypes.Length; i++)
                        mangledArgumentTypes[i] = (S.IsFloatType(argumentTypes[i])) ? FT.type : FT.GetMangledName(S, argumentTypes[i]);
                    mangledFuncName = Util.AppendTypenameToFuncName(S, FT, F.OutputName, mangledArgumentTypes);
                    //mangledFuncName = FT.GetMangledName(S, mangledFuncName);
                }
                else if (argumentTypes.Length == 0)
                {
                    // test to apply mangling when no arguments are present.
                    mangledFuncName = FT.GetMangledName(S, mangledFuncName);
                }

                return mangledFuncName;
            }
        } // end of GetFunctionName()

        /// <summary>
        /// Returns the name of a generated function (for example <c>gp_mv_mv</c>).
        /// The function is found by looking through all G25.FGS in the Specification.
        /// 
        /// If the function is not found, a fictional name is returned, i.e. "missingFunction_" + functionName.
        /// This name will then show up in the generated code, which will not compile as a result.
        /// 
        /// If the function is not found, this is also enlisted in cgd.m_missingDependencies.
        /// Call cgd.PrintMissingDependencies() should be called to report the missing dependencies
        /// to the end-user.
        /// </summary>
        /// <param name="S">The spec.</param>
        /// <param name="cgd">Missing dependencies go into cgd.m_missingDependencies.</param>
        /// <param name="functionName">Basic name of the function to be found.</param>
        /// <param name="argumentTypes">Names of the arguments types (not mangled).</param>
        /// <param name="FT">Floating point type.</param>
        /// <param name="metricName">(optional, can be null for don't care)</param>
        /// <returns>The mangled name of the function.</returns>
        public static string GetDependency(Specification S, CGdata cgd, string functionName, string[] argumentTypes, G25.FloatType FT, string metricName)
        {
            string returnTypeName = null;
            return GetDependency(S, cgd, functionName, argumentTypes, returnTypeName, FT, metricName);
        }

        /// <summary>
        /// Returns the name of a generated function (for example <c>gp_mv_mv</c>).
        /// The function is found by looking through all G25.FGS in the Specification.
        /// 
        /// If the function is not found, a fictional name is returned, i.e. "missingFunction_" + functionName.
        /// This name will then show up in the generated code, which will not compile as a result.
        /// 
        /// If the function is not found, this is also enlisted in cgd.m_missingDependencies.
        /// Call cgd.PrintMissingDependencies() should be called to report the missing dependencies
        /// to the end-user.
        /// </summary>
        /// <param name="S">The spec.</param>
        /// <param name="cgd">Missing dependencies go into cgd.m_missingDependencies.</param>
        /// <param name="functionName">Basic name of the function to be found.</param>
        /// <param name="argumentTypes">Names of the arguments types (not mangled).</param>
        /// <param name="returnTypeName">Name of the return type (can be null or "" for default return type).</param>
        /// <param name="FT">Floating point type.</param>
        /// <param name="metricName">(optional, can be null for don't care)</param>
        /// <returns>The mangled name of the function.</returns>
        public static string GetDependency(Specification S, CGdata cgd, string functionName, 
            string[] argumentTypes, string returnTypeName, G25.FloatType FT, string metricName)
        {
            // bool returnTrueName = dependent on cgd.mode
            try
            {
                return GetFunctionName(S, functionName, argumentTypes, returnTypeName, FT, metricName);
            }
            catch (DependencyException)
            { // function not found, return a fictional name, and remember dependency
                G25.fgs F = null;
                {
                    // get name of dep, and make sure it does not get mangled
                    string outputName = functionName + cgd.GetDontMangleUniqueId();
                    if (returnTypeName != null) outputName = outputName + "_returns_" + returnTypeName;

                    // add dependency to list of missing deps:
                    string[] argVarNames = null;
                    Dictionary<String, String> options = null;
                    string comment = null;
                    G25.fgs tmpF = new G25.fgs(functionName, outputName, returnTypeName, argumentTypes, argVarNames, new String[] { FT.type }, metricName, comment, options);

                    F = cgd.AddMissingDependency(S, tmpF);
                }

                return cgd.m_dependencyPrefix + F.OutputName;
            }
        } // end of GetDependency()

        

        /// <summary>
        /// Resolves a converter (underscore constructor) dependency.
        /// Searches for a converter from 'fromType' to 'toType'.
        /// 
        /// If the function is not found, this is also enlisted in cgd.m_missingDependencies.
        /// Call cgd.PrintMissingDependencies() should be called to report the missing dependencies
        /// to the end-user.
        /// </summary>
        /// <param name="S">The spec.</param>
        /// <param name="cgd">Missing dependencies go into cgd.m_missingDependencies.</param>
        /// <param name="fromType"></param>
        /// <param name="toType"></param>
        /// <param name="FT"></param>
        /// <returns></returns>
        public static string GetConverterDependency(Specification S, CGdata cgd, string fromType, string toType, G25.FloatType FT)
        {
            // look for 'funcName' in all G25.fgs in the spec
//                    string funcName = "_" + FT.GetMangledName(S, toType);
            string funcName = "_" + toType;
            foreach (G25.fgs F in S.m_functions)
            {
                if (F.IsConverter(S)) // is 'F' a converter (underscore constructor)?
                {
                    if ((F.Name == funcName) &&
                        (F.ArgumentTypeNames[0] == fromType))
                    {
                        return G25.CG.Shared.Converter.GetConverterName(S, F, FT.GetMangledName(S, fromType), FT.GetMangledName(S, toType));
                    }
                }
            }

            // converter not found: add it to missing deps:
            {
                // add dependency to list of missing deps:
                string outputName = null;
                string[] argumentTypes = new string[] { fromType };
                string[] argVarNames = null;
                string returnTypeName = null;
                string metricName = null;
                string comment = null;
                Dictionary<string, string> options = null;
                G25.fgs F = new G25.fgs(funcName, outputName, returnTypeName, argumentTypes, argVarNames, new string[] { FT.type }, metricName, comment, options);
                cgd.AddMissingDependency(S, F);
            }

            // return fictional name:
            G25.fgs tmpF = null;
            return "missingFunction_" + G25.CG.Shared.Converter.GetConverterName(S, tmpF, FT.GetMangledName(S, fromType), FT.GetMangledName(S, toType));
        }


    } // end of class Dependencies
} // end of namepace G25.CG.Shared
