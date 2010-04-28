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

/*! \mainpage Gaigen 2.5 library (libg25) Documentation
 *
 * 
 * Released under GPL license.
 * 
 * \section intro_sec Introduction
 *
 * Gaigen 2.5 is a geometric algebra code generator. It turns a specification of geometric algebra into
 * an implementation of that algebra in a specific language.
 * 
 * This library handles all the internals: reading the specification, generating the code, etc.
 * The other programs such as G25 are just shells around this library. The actual code generation
 * functionality is handled by plugins which implement the interface G25.CodeGenerator.
 * 
 * \section classes_sec The classes
 * 
 * The G25.Specification class carries all information on the specification of an algebra. It can
 * read and write XML files containing the specification (see the g25_user_manual for a description).
 * 
 * The G25.rsep class is a simple expression parser that Gaigen uses to parse parts of the XML
 * specification, such as the metric specification (<c>"e1.e1=1"</c>)and the definition of basis vectors (<c>"e1^e2^e3"</c>).
 * The G25.rsel class is a simple expression lexer used by the parser.
 * 
 * The G25.rsbbp class is a parser for list of basis blades as they are stored in the specification file.
 * It uses G25.rsep to parse simple strings like <c>"no=1 e1 e2 e3"</c> or <c>"e1^e2 e2^e3 e3^e1"</c>.
 * 
 * The G25.MV class (and its subclasses G25.SMV and G25.GMV) represent multivector classes
 * which will be generated. The main properties of these classes are their names and the order
 * and possibly grouping of coordinates.
 * 
 * The G25.OM class (and its subclasses G25.SOM and G25.GOM) represent outermorphism matrix
 * representations which will be generated. The main properties of these classes are their names 
 * and the order and coordinates in the domain and the range.
 * 
 * The G25.fgs class holds Function Generation Specifications. This means a request of the
 * code generator to generate a specific function over specific multivector/outermorphism arguments,
 * with specific floating point types.
 * 
 * The G25.CodeGeneratorLoader handles the loading of G25.CodeGenerator and G25.CodeGeneratorPlugin  
 * classes.These code generators generate the code for a specific language, given the G25.Specification.
 * 
 * The G25.UserException should be thrown when an error occurs which is due to the user (i.e., and error in the input XML file).
 * These are caught and presented to the user as errors.
 * 
 * \section xml_format XML file format.
 * 
 * The fileformat of specifications is described in the Gaigen 2.5 user manual.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;


namespace G25
{

    /// <summary>
    /// Output language for generated code.
    /// </summary>
    public enum OUTPUT_LANGUAGE {
        NONE = -1,
        C = 0,
        CPP,
        JAVA,
        CSHARP,
        PYTHON,
        MATLAB
    }

    /// <summary>
    /// How to store coordinates of specialized multivectors: in arrays or as single variables.
    /// </summary>
    public enum COORD_STORAGE
    {
        ARRAY = 1,
        VARIABLES = 2
    }

    /// <summary>
    /// Expand general multivector code, or compute tables it at run-time
    /// </summary>
    public enum GMV_CODE
    {
        EXPAND = 1,
        RUNTIME = 2
    }

    /// <summary>
    /// What type of parser to use. <c>CUSTOM</c> means a hand-written
    /// parser.
    /// </summary>
    public enum PARSER
    {
        NONE=0,
        BUILTIN=1,
        ANTLR=2
    }

    /// <summary>
    /// This class represents a Gaigen 2.5 specification.
    /// 
    /// A specification contains all information about the algebra to be generated: license, namespace,
    /// specialized multivectors, what functions to generate, optimize, and so on.
    /// 
    /// The constructor G25.Specification.Specification(string filename) constructs a new specification
    /// from an XML file.
    /// 
    /// A specification can be converted to XML using G25.Specification.ToXmlString(). This function returns
    /// an XML string representing the specification, formatted for human readability.
    /// 
    /// All specification items are currently stored in public member variables, so in theory you could
    /// set them directly. For most members, there are appropriate function to set them, such as
    /// SetDimension(), SetBasisVectorName(), SetMetric() and AddOperator().
    /// </summary>
    public class Specification
    {
        public static string FullGaigenName = "Gaigen 2.5";

        /// <summary>
        /// This string prevents mangling of type and function names.
        /// This is used in some places to make the auto-dependency system works correctly.
        /// </summary>
        public const string DONT_MANGLE = "_dont_mangle_";
        public const string CONSTANT_TYPE_SUFFIX = "_t";



        /// <summary>
        /// Used as the name of the auto-generated euclidean metric.
        /// </summary>
        public const string INTERNAL_EUCLIDEAN_METRIC = "_internal_euclidean_metric_";



        /// <summary>
        /// Constructor which sets up a default specification. Can be used to
        /// construct a specification 'by hand' (i.e, using function calls and setting public
        /// members).
        /// </summary>
        public Specification()
        {
            m_inputDirectory = System.IO.Directory.GetCurrentDirectory();

            InitFloatTypes();
            m_copyright = "";
            m_outputLanguage = OUTPUT_LANGUAGE.NONE;
            m_coordStorage = COORD_STORAGE.ARRAY;
            SetDimension(1);
            InitMetric();
            InitBasisBladeParser();

            System.Console.WriteLine("Specification.Specification(): todo: set members to default values");

        }

        /// <summary>
        /// Constructor which reads an XML specification file.
        /// </summary>
        /// <param name="filename">Filename of XML file.</param>
        public Specification(string filename)
        {
            try
            {
                m_inputDirectory = System.IO.Path.GetDirectoryName(filename);
            }
            catch (Exception)
            {
                m_inputDirectory = System.IO.Directory.GetCurrentDirectory();
            }

            InitFloatTypes();
            m_copyright = "";
            m_outputLanguage = OUTPUT_LANGUAGE.NONE;
            InitMetric();
            InitBasisBladeParser();

            SetOutputDir(System.IO.Directory.GetCurrentDirectory());
            m_outputDirectoryExplicitlySet = false; // force to false

            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            // todo: maybe remember what 'filename' refers to, so we can use it as
            // a relative path for e.g., licenses.

            XML.InitFromXmlDocument(this, doc);
        }

        /// <summary>
        /// Throws exception when specification is not consistent, missing details, etc.
        /// For example, a floating point type must be set.
        /// 
        /// Also patches some minor errors/annoyances, such as a m_floatSuffix being null (sets it to "")
        /// </summary>
        public void CheckSpecificationSanity() {
            // dimension
            if (m_dimension < 1) throw new G25.UserException("Invalid dimension of space " + m_dimension);

            // output language
            if (m_outputLanguage == OUTPUT_LANGUAGE.NONE)
                throw new G25.UserException("No output language set (use XML attribute '" + XML.XML_LANGUAGE + "').");

            // namespace
            if ((m_namespace == null) || (m_namespace.Length < 1))
                throw new G25.UserException("No namespace set (use XML attribute '" + XML.XML_NAMESPACE + "').");

            // float types
            if ((m_floatTypes == null) || (m_floatTypes.Count == 0))
                throw new G25.UserException("No float type set (use XML element '" + XML.XML_FLOAT_TYPE + "').");
            for (int i = 0; i < m_floatTypes.Count; i++) {
                if ((m_floatTypes[i].type == null) || (m_floatTypes[i].type.Length == 0))
                    throw new Exception("Specification.CheckSpecificationSanity(): empty float type"); // this is an internal error, not a user error
            }

            // basis vector names
            CheckBasisVectorNames();

            // check general multivector
            if (m_GMV == null)
                throw new G25.UserException("Missing general multivector specification (use XML element '" + XML.XML_MV + "').");

            m_GMV.SanityCheck(this, m_basisVectorNames.ToArray());

            // check all SMVs
            foreach (G25.SMV smv in m_SMV)
            {
                smv.SanityCheck(this, m_basisVectorNames.ToArray());
            }

            // check general outermorphism
            if (m_GOM != null)
            {
                if (m_gmvCodeGeneration == GMV_CODE.RUNTIME)
                    throw new Exception("Defining a general outermorphism with 'runtime' code enabled is not supported yet.");
                m_GOM.SanityCheck(m_dimension, m_basisVectorNames.ToArray());
            }

            // check all SOMs
            foreach (G25.SOM som in m_SOM)
            {
                som.SanityCheck(m_dimension, m_basisVectorNames.ToArray());
            }

            // check if metric is diagonal +- 1 when using 
            if (m_gmvCodeGeneration == GMV_CODE.RUNTIME)
            {
                foreach (Metric M in m_metric)
                {
                    if (!M.m_metric.IsSimpleDiagonal())
                        throw new G25.UserException("Only a diagonal metric with -1, 0, +1 values on the diagonal can be used when '" + 
                            XML.XML_GMV_CODE + "' is set to '" + XML.XML_RUNTIME + "'.");
                }
            }

            // check function generation specifications
            CheckFGS();
        }

        /// <summary>
        /// Checks all functions. Throws an Exception when an error is found.
        /// Currently, only the existance of the metric is checked.
        /// </summary>
        private void CheckFGS()
        {
            foreach (fgs F in m_functions)
            {
                if (!IsMetric(F.MetricName))
                    throw new G25.UserException("Unknown metric " + F.MetricName,
                        XML.FunctionToXmlString(this, F));
            }
        }

        /// <summary>
        /// Finds and returns the function (fgs) that matches the description.
        /// Returns null if not found.
        /// </summary>
        /// <param name="functionName">Name (as 'recognized' by the code generator) of the function.</param>
        /// <param name="argumentTypes">Argument types (can be null or empty for default args).</param>
        /// <param name="floatName">Floating point types to generate for (can be a subset of the returned function).</param>
        /// <param name="metricName">Name of metric used in function (can be null if you don't care about the metric).</param>
        /// <returns>The function generation specification, or null if no matching FGS found.</returns>
        public G25.fgs FindFunctionEx(String functionName, String[] argumentTypes, String floatName, String metricName)
        {
            return FindFunctionEx(functionName, argumentTypes, new String[] { floatName }, metricName);
        }

        /// <summary>
        /// Finds and returns the function (fgs) that matches the description.
        /// Returns null if not found.
        /// </summary>
        /// <param name="functionName">Name (as 'recognized' by the code generator) of the function.</param>
        /// <param name="argumentTypes">Argument types (can be null or empty for default args).</param>
        /// <param name="floatNames">Floating point types to generate for (can be a subset of the returned function).</param>
        /// <param name="metricName">Name of metric used in function (can be null if you don't care about the metric).</param>
        /// <returns>The function generation specification, or null if no matching FGS found.</returns>
        public G25.fgs FindFunctionEx(string functionName, string[] argumentTypes, string[] floatNames, string metricName)
        {
            String returnTypeName = null;
            return FindFunctionEx(functionName, argumentTypes, returnTypeName, floatNames, metricName);
        }

        /// <summary>
        /// Finds and returns the function (fgs) that matches the description.
        /// Returns null if not found. Pointers for argument types are not compared??
        /// </summary>
        /// <param name="functionName">Name (as 'recognized' by the code generator) of the function.</param>
        /// <param name="argumentTypes">Argument types (can be null or empty for default args).</param>
        /// <param name="returnTypeName">Name of the return type (can be null or "" for default return type).</param>
        /// <param name="floatNames">Floating point types to generate for (can be a subset of the returned function).</param>
        /// <param name="metricName">Name of metric used in function (can be null if you don't care about the metric).</param>
        /// <returns>The function generation specification, or null if no matching FGS found.</returns>
        public G25.fgs FindFunctionEx(string functionName, string[] argumentTypes, string returnTypeName, string[] floatNames, string metricName)
        {
            foreach (fgs F in m_functions) 
            {
                // check name
                if (F.Name != functionName) continue;

                // check argument types
                if ((argumentTypes != null) && (argumentTypes.Length > 0)) 
                {
                    if (argumentTypes.Length != F.NbArguments) continue;
                    bool match = true;
                    for (int a = 0; a < argumentTypes.Length; a++) 
                    {
                        if (F.ArgumentTypeNames[a] != argumentTypes[a])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match == false) continue;
                }

                // find all float names
                bool allFloatTypesFound = true;
                for (int f = 0; f < floatNames.Length; f++) {
                    bool found = false;
                    for (int g = 0; g < F.FloatNames.Length; g++)
                        if (F.FloatNames[g] == floatNames[f])
                            found = true;
                    if (!found)
                    {
                        allFloatTypesFound = false;
                        break;
                    }
                }
                if (!allFloatTypesFound) continue;

                // check metric
                if (metricName != null)
                    if (metricName != F.MetricName) continue;

                if ((returnTypeName != null) && (returnTypeName.Length > 0))
                {
                    //??? TODO!!!! How do we know the default . . . . 
                    if (F.ReturnTypeName != returnTypeName) continue;
                }
                
                return F;
            }
            return null;
        } // end of FindFunction()



        private void InitBasisBladeParser()
        {
            m_basisBladeParser = new rsbbp(this);
        }

        /// <summary>
        /// Called by constructors; allocates empty lists for float types
        /// </summary>
        private void InitFloatTypes()
        {
            if (m_floatTypes == null) m_floatTypes = new List<FloatType>();
        }

        /// <summary>
        /// Called by constructors; allocates empty arrays for metric info.
        /// Also initializes parser for metric 
        /// </summary>
        private void InitMetric()
        {
            // init list of metrics and add default metric
            m_metric = new List<Metric>();
            m_metric.Add(new Metric("default"));

            { // initialize parser
                bool UNARY = true, BINARY = false;
                bool PREFIX = false;
                bool LEFT_ASSOCIATIVE = true, RIGHT_ASSOCIATIVE = false;
                G25.rsep.Operator[] ops = new G25.rsep.Operator[] {
                    // symbol, name, precedence, unary, postfix, left associative
                    new G25.rsep.Operator("-", "negate", 0, UNARY, PREFIX, LEFT_ASSOCIATIVE),
                    new G25.rsep.Operator("+", "nop", 0, UNARY, PREFIX, LEFT_ASSOCIATIVE), // unary + is a nop
                    new G25.rsep.Operator(".", "ip", 1, BINARY, PREFIX, LEFT_ASSOCIATIVE),
                    new G25.rsep.Operator("=", "assign", 2, BINARY, PREFIX, RIGHT_ASSOCIATIVE) 
                };
                m_metricParser = new G25.rsep(ops);
            }
        }

        /// <summary>
        /// Initializes all RefGA.Metric variables inside each m_metric.
        /// 
        /// If there is not Euclidean metric, adds one, named "_internal_euclidean_". 
        /// This system generated metric will not be written back to XML.
        /// </summary>
        protected internal void FinishMetric()
        {
            bool hasEucl = false;
            foreach (Metric M in m_metric)
            {
                M.Init(m_dimension);
                hasEucl |= M.m_metric.IsEuclidean();
            }

            if (!hasEucl)
            {
                Metric M = null;
                for (int i = 0; i < m_dimension; i++)
                    M = SetMetric(INTERNAL_EUCLIDEAN_METRIC, i, i, 1.0);
                if (M != null) M.Init(m_dimension);
            }
        }

        /// <returns>true when 'metricName' is an existing metric</returns>
        public bool IsMetric(String metricName)
        {
            metricName = metricName.ToLower();
            {
                foreach (Metric M in m_metric)
                {
                    if (M.m_name == metricName)
                        return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Return the metric with name 'metricName'. Creates it if it does not already exist.
        /// Case insensitive.
        /// </summary>
        /// <param name="metricName">Name of the metric (e.g., "default", or "euclidean"). </param>
        /// <returns>requested metric.</returns>
        public Metric GetMetric(String metricName)
        {
            metricName = metricName.ToLower();
            {
                foreach (Metric M in m_metric)
                {
                    if (M.m_name == metricName)
                        return M;
                }
            }
            {
                Metric M = new Metric(metricName);
                m_metric.Add(M);
                return M;
            }
        }

        /// <summary>
        /// Return a Euclidean metric. Returns null if it does not exist (this would be internal error, since it should always exist
        /// after the spec has been initialized.
        /// Case insensitive.
        /// </summary>
        /// <returns>euclidean metric.</returns>
        public Metric GetEuclideanMetric()
        {
            foreach (Metric M in m_metric)
            {
                if (M.m_metric.IsEuclidean())
                    return M;
            }
            return null;
        }

        /// <summary>
        /// Sets basisVectorIdx1.basisVectorIdx2=value.
        /// Overrides existing definitions. The order of basisVectorIdx1 and basisVectorIdx2 doesn't matter.
        /// </summary>
        /// <param name="metricName">Name of the metric (e.g., "default", or "euclidean"). </param>
        /// <param name="basisVectorIdx1">Index of first basis vector.</param>
        /// <param name="basisVectorIdx2">Index of second basis vector.</param>
        /// <param name="value">the value of (basisVectorIdx1.basisVectorIdx2)</param>
        /// <returns>the metric which was set.</returns>
        public Metric SetMetric(String metricName, int basisVectorIdx1, int basisVectorIdx2, double value)
        {
            Metric M = GetMetric(metricName);

            // make sure first index is smallest one:
            if (basisVectorIdx2 < basisVectorIdx1)
            {
                int tmp = basisVectorIdx2;
                basisVectorIdx2 = basisVectorIdx1;
                basisVectorIdx2 = tmp;
            }
                
            // check if already exists (both ways); if so: overwrite
            for (int i = 0; i < M.m_metricBasisVectorIdx1.Count; i++)
            {
                if ((M.m_metricBasisVectorIdx1[i] == basisVectorIdx1) &&
                    (M.m_metricBasisVectorIdx2[i] == basisVectorIdx2))
                {
                    // this metric was already defined; override it:
                    M.m_metricValue[i] = value;
                    return M;
                }
            }

            // not defined yet; simply add new metric
            M.m_metricBasisVectorIdx1.Add(basisVectorIdx1);
            M.m_metricBasisVectorIdx2.Add(basisVectorIdx2);
            M.m_metricValue.Add(value);

            return M;
        }



        /// <summary>
        /// Adds a floating point type to the specification.
        /// </summary>
        /// <param name="floatType">Name of float type (e.g. "float" or "double")</param>
        /// <param name="floatPrefix">Prefix for multivector types using this float (e.g. "" or "float_")</param>
        /// <param name="floatSuffix">Suffix for multivector types using this float (e.g. "" or "_f")</param>
        public void AddFloatType(String floatType, String floatPrefix, String floatSuffix)
        {
            if (floatType.Length == 0) throw new Exception("Specification.AddFloatType(): empty floating point type"); // internal error?

            if (floatPrefix == null) floatPrefix = "";
            if (floatSuffix == null) floatSuffix = "";

            // check if prefix, suffix is unique:
            for (int i = 0; i < m_floatTypes.Count; i++)
            {
                if ((m_floatTypes[i].prefix == floatPrefix) && (m_floatTypes[i].suffix == floatSuffix))
                    throw new G25.UserException("While adding a new floating point type '" + floatType + "': a floating point type '" + m_floatTypes[i] + "'with the same suffix and prefix already exists.");
            }

            m_floatTypes.Add(new FloatType(floatType, floatPrefix, floatSuffix));
        }

        /// <summary>
        /// Searches through m_floatTypes for the type that has the name 'floatType'.
        /// Returns it.
        /// </summary>
        /// <param name="floatType">The name of the float type.</param>
        /// <returns>The G25.FloatType with the name 'floatType', or null if not found.</returns>
        public FloatType GetFloatType(String floatType)
        {
            foreach (FloatType FT in m_floatTypes) {
                if (FT.type == floatType) return FT;
            }
            return null;
        }

        /// <summary>
        /// Adds an operator binding. If the operator was already in use, the old binding
        /// is overwritten.
        /// </summary>
        public void AddOperator(Operator newOp)
        {
            if ((newOp.Symbol == null) || (newOp.Symbol.Length == 0))
                throw new Exception("Specification.AddOperator(): empty operator symbol"); // internal error?
            if ((newOp.FunctionName == null) || (newOp.FunctionName.Length == 0))
                throw new Exception("Specification.AddOperator(): empty operator function"); // internal error?
            if ((newOp.NbArguments < 1) || (newOp.NbArguments > 2))
                throw new Exception("Specification.AddOperator(): invalid number of arguments for operator '" + newOp.Symbol + "' bound to '" + newOp.FunctionName + "' (" + newOp.NbArguments + ")"); // internal error?

            int pos = -1;
            // check if operator is already in used
            for (int i = 0; i < m_operators.Count; i++)
            {
                Operator otherOp = m_operators[i];
                bool prefixMatch = false;
                if ((newOp.NbArguments == 1) && (otherOp.IsPrefix == newOp.IsPrefix)) prefixMatch = true;

                if ((otherOp.NbArguments == newOp.NbArguments) && (otherOp.Symbol == newOp.Symbol) && prefixMatch) 
                    pos = i;
            }

            // if not already defined, create new position, otherwise overwrite the existing operator
            if (pos < 0) {
                pos = m_operators.Count;
                m_operators.Add(null);
            }

            m_operators[pos] = newOp;
        }

        /// <returns>true if 'typeName' is a floating point type, general multivector, specialized multivector, general outermorphism or specialized outermorphism name.</returns>
        public bool IsTypeName(String typeName)
        {
            
            return (IsFloatType(typeName) || 
                (m_GMV != null) && (m_GMV.Name == typeName)) ||
                IsSpecializedMultivectorName(typeName) ||
                ((m_GOM != null) && (m_GOM.Name == typeName)) ||
                IsSpecializedOutermorphismName(typeName) ||
                IsFloatType(typeName);
        }

        /// <summary>
        /// Returns the type according to a typeName.
        /// </summary>
        /// <param name="typeName">(non-mangled) name of type. For example "float" or "mv".</param>
        /// <returns>type specified by 'typeName', or null if not found.</returns>
        public G25.VariableType GetType(string typeName)
        {
            // float?
            G25.VariableType VT = GetFloatType(typeName);
            if (VT != null) return VT;
            // smv?
            VT = GetSMV(typeName);
            if (VT != null) return VT;
            // som?
            VT = GetSOM(typeName);
            if (VT != null) return VT;
            // gmv?
            if ((m_GMV != null) && (m_GMV.Name == typeName)) return m_GMV;
            // gom?
            if ((m_GOM != null) && (m_GOM.Name == typeName)) return m_GOM;

            if (typeName.Equals("CoordinateOrder"))
                return new G25.EnumType("CoordinateOrder");

            return null;
        }

        /// <returns>true if 'typeName' is a specialized multivector name.</returns>
        public bool IsSpecializedMultivectorName(String typeName)
        {
            for (int i = 0; i < m_SMV.Count; i++)
            {
                if (m_SMV[i].Name == typeName) return true;
            }
            return false;
        }

        /// <returns>true if 'typeName' is a general or specialized multivector name.</returns>
        public bool IsMultivectorName(String typeName)
        {
            return (m_GMV.Name == typeName) || 
                IsSpecializedMultivectorName(typeName);
        }

        /// <returns>specialized multivector with name 'typeName', or null if none found.</returns>
        public G25.SMV GetSMV(String typeName)
        {
            for (int i = 0; i < m_SMV.Count; i++)
            {
                if (m_SMV[i].Name == typeName) return m_SMV[i];
            }
            return null;
        }

        /// <returns>specialized multivector with name 'typeName', or null if none found.</returns>
        public G25.SOM GetSOM(String typeName)
        {
            for (int i = 0; i < m_SOM.Count; i++)
            {
                if (m_SOM[i].Name == typeName) return m_SOM[i];
            }
            return null;
        }

        /// <returns>true if 'typeName' is a specialized multivector name.</returns>
        public bool IsSpecializedOutermorphismName(String typeName)
        {
            for (int i = 0; i < m_SOM.Count; i++)
            {
                if (m_SOM[i].Name == typeName) return true;
            }
            return false;
        }

        /// <returns>true if 'typeName' is a general or specialized outermorphism name.</returns>
        public bool IsOutermorphismName(String typeName)
        {
            return ((m_GOM != null) && (m_GOM.Name == typeName)) ||
                IsSpecializedOutermorphismName(typeName);
        }


        /// <returns>true if 'typeName' is a floating point type listed in m_floatTypes.</returns>
        public bool IsFloatType(String typeName)
        {
            foreach (FloatType FT in m_floatTypes)
                if (FT.type == typeName) return true;
            return false;
        }

        /// <summary>
        /// Returns the non-constant scalar SMV, if any. Otherwise return null.
        /// </summary>
        /// <returns>The scalar SMV type, or null if it was not defined.</returns>
        public G25.SMV GetScalarSMV()
        {
            foreach (G25.SMV smv in m_SMV) 
            {
                if ((smv.NbNonConstBasisBlade == 1) && (smv.Group(0)[0].Grade() == 0))
                    return smv;
            }
            return null;
        }

        /// <summary>
        /// Sets license to 'license'. If 'license' is "gpl" (XML_GPL), then the license is set to the full
        /// GPL license. If 'license' is "bsd" (XML_BSD), then the full BSD license is set. Otherwise the
        /// license is simply set to the value of 'license'.
        /// </summary>
        /// <param name="license"></param>
        public void SetLicense(String license)
        {
            if (license.ToLower() == XML.XML_GPL)
            {
                m_license = Licenses.GPL_LICENSE;
            }
            else if (license.ToLower() == XML.XML_BSD)
            {
                m_license = Licenses.BSD_LICENSE;
            }
            else if (license.ToLower() == XML.XML_CUSTOM)
            {
                m_license = XML.XML_CUSTOM; // the license will be filled in by the XML_CUSTOM_LICENSE later on
            }
            else throw new G25.UserException("Unknown license '" + license + "' specified.");
        }

        /// <returns>license text (may be multiple lines).</returns>
        public string GetLicense() {
            return m_license;
        }

        /// <summary>
        /// Sets language of emitted code.
        /// </summary>
        /// <param name="language">valid values are "cpp", "java" and "csharp".
        /// (XML_C, XML_CPP, XML_JAVA, XML_CSHARP, XML_PYTHON, XML_MATLAB)
        /// </param>
        public void SetLanguage(string language)
        {
            switch (language.ToLower()) {
                case XML.XML_C: m_outputLanguage = OUTPUT_LANGUAGE.C;
                    break;
                case XML.XML_CPP: m_outputLanguage = OUTPUT_LANGUAGE.CPP;
                    break;
                case XML.XML_CSHARP: m_outputLanguage = OUTPUT_LANGUAGE.CSHARP;
                    break;
                case XML.XML_JAVA: m_outputLanguage = OUTPUT_LANGUAGE.JAVA;
                    break;
                case XML. XML_MATLAB: m_outputLanguage = OUTPUT_LANGUAGE.MATLAB;
                    break;
                case XML.XML_PYTHON: m_outputLanguage = OUTPUT_LANGUAGE.PYTHON;
                    break;
                default:
                    throw new G25.UserException("Unknown output language " + language);
            }
        }

        /// <returns>true if operator bindings as defined are the default for the output language.</returns>
        public bool DefaultOperatorBindings()
        {
            return m_defaultOperators.Count != 0;
        }

        /// <summary>
        /// (re)sets the operator bindings to the default for the current language.
        /// </summary>
        /// <remarks>The list <c>m_defaultOperators</c> must be set with the default operators, because this is
        /// used by the ToXmlString() function to determine whether the default operators were set.</remarks>
        public void SetDefaultOperatorBindings()
        {
            if (m_outputLanguage == OUTPUT_LANGUAGE.NONE)
            {
                throw new G25.UserException("No output language has been set (use XML attribute '" + XML.XML_LANGUAGE + "').");
            }
            else if (m_outputLanguage == OUTPUT_LANGUAGE.C)
            {
                System.Console.WriteLine("Warning: No operator bindings are possible for output language C");
            }
            else if (m_outputLanguage == OUTPUT_LANGUAGE.CPP)
            {
                SetDefaultOperatorBindingsCpp();
            }
            else System.Console.WriteLine("Internal error: Specification.SetDefaultOperatorBindings(): todo: implement this function !");
        }

        /// <summary>
        /// Set the default operator bindings for C++.
        /// </summary>
        private void SetDefaultOperatorBindingsCpp()
        {
            AddOperator(Operator.Binary("+", "add"));
            AddOperator(Operator.Binary("-", "subtract"));
            AddOperator(Operator.UnaryPrefix("-", "negate"));

            AddOperator(Operator.Binary("%", "sp"));
            AddOperator(Operator.Binary("<<", "lc"));
            AddOperator(Operator.Binary(">>", "rc"));
            AddOperator(Operator.Binary("^", "op"));
            AddOperator(Operator.Binary("*", "gp"));
            AddOperator(Operator.Binary("/", "igp"));

            AddOperator(Operator.Binary("&", "meet"));
            AddOperator(Operator.Binary("|", "join"));

            AddOperator(Operator.UnaryPrefix("++", "increment"));
            AddOperator(Operator.UnaryPostfix("++", "increment"));
            AddOperator(Operator.UnaryPrefix("--", "decrement"));
            AddOperator(Operator.UnaryPostfix("--", "decrement"));

            AddOperator(Operator.UnaryPrefix("*", "dual"));
            AddOperator(Operator.UnaryPrefix("!", "versorInverse"));
            AddOperator(Operator.UnaryPrefix("~", "reverse"));

            // remember the default operators (this is used by ToXmlString())
            m_defaultOperators = new List<Operator>(m_operators);
        }

        /// <summary>
        /// Sets the dimension of the algebra.
        /// Also set basis vector to default names (e1, e2, ...)
        /// </summary>
        /// <param name="dim">The dimension of space. Must be >= 1</param>
        public void SetDimension(int dim)
        {
            if (dim < 1) throw new G25.UserException("Invalid dimension for space: " + dim + " (attribute '" + XML.XML_DIMENSION + "').");

            m_dimension = dim;
            if (m_basisVectorNames == null) m_basisVectorNames = new List<string>();
            else m_basisVectorNames.Clear();
            for (int i = 0; i < dim; i++)
            {
                m_basisVectorNames.Add("e" + (i + 1).ToString());
            }
        }

        
        /// <summary>
        /// Searches for basis vector with name 'name', returns its index.
        /// </summary>
        /// <param name="name">Name of the basis vector (e.g. <c>"e1"</c>).</param>
        /// <returns>Index of basis vector, or -1 if not found.</returns>
        public int GetBasisVectorIndex(String name) 
        {
            for (int i = 0; i < m_basisVectorNames.Count; i++)
                if (m_basisVectorNames[i] == name) return i;
            return -1;
        }

        /// <summary>
        /// Sets the general multivector type (checks if name is not already in use, throws if it is.).
        /// </summary>
        public void SetGeneralMV(GMV gmv)
        {
            if (IsTypeName(gmv.Name))
                throw new G25.UserException("While setting the general multivector type: the name '" + gmv.Name + "' is already a typename (XML element '" + XML.XML_MV + "'.");
            m_GMV = gmv;
        }

        /// <summary>
        /// Adds a new specialized multivector (checks if name is not already in use, throws if it is.).
        /// </summary>
        public void AddSpecializedMV(SMV smv)
        {
            if (IsTypeName(smv.Name))
                throw new G25.UserException("While adding a specialized multivector type: the name '" + smv.Name + "' is already a typename.");
            else m_SMV.Add(smv);
        }

        /// <summary>
        /// Sets the general outermorphism type (checks if name is not already in use, throws if it is.).
        /// </summary>
        public void SetGeneralOM(GOM gom)
        {
            if (IsTypeName(gom.Name))
                throw new Exception("While setting the general outermorphism type: the name '" + gom.Name + "' is already a typename (XML element '" + XML.XML_OM + "'.");
            m_GOM = gom;
        }

        /// <summary>
        /// Adds a new specialized outermorphism (checks if name is not already in use, throws if it is.).
        /// </summary>
        public void AddSpecializedOM(SOM som)
        {
            if (IsTypeName(som.Name))
                throw new G25.UserException("While adding a specialized outermorphism type: the name '" + som.Name + "' is already a typename (XML element '" + XML.XML_SOM + "'.");
            else m_SOM.Add(som);
        }

        /// <summary>
        /// Adds a new constant to the specification.
        /// </summary>
        public void AddConstant(Constant C)
        {
            if (IsConstant(C.Name))
                throw new G25.UserException("While adding a specialized multivector type: the name '" + C.Name + "' is already a constant.");
            m_constant.Add(C);
        }

        /// <returns>Constant with name 'name', or null if no such constant.</returns>
        public Constant GetConstant(string name)
        {
            foreach (Constant C in m_constant)
            {
                if (C.Name.Equals(name))
                    return C;
            }
            return null;
        }

        /// <summary>
        /// Returns true when 'name' is a constant.
        /// </summary>
        public bool IsConstant(string name)
        {
            foreach (Constant C in m_constant)
            {
                if (C.Name.Equals(name))
                    return true;
            }
            return false;
        }

        public Constant GetMatchingConstant(VariableType T)
        {
            if (!T.GetName().EndsWith(CONSTANT_TYPE_SUFFIX)) return null;
            string constantName = T.GetName().Substring(0, T.GetName().Length - CONSTANT_TYPE_SUFFIX.Length);

            Constant C = GetConstant(constantName);

            if (C.Type == T) return C;
            else return null;
        }


        /// <summary>
        /// Checks if all basis vector names are unique and valid. Throws Exception when this is
        /// not that case.
        /// </summary>
        protected internal void CheckBasisVectorNames()
        {
            if (m_basisVectorNames.Count != m_dimension)
                throw new G25.UserException("The number of basis vector names does not match dimension of space (see XML element '" + XML.XML_BASIS_VECTOR_NAMES + "').");

            for (int i = 0; i < m_basisVectorNames.Count; i++)
                if ((m_basisVectorNames[i] == null) ||
                    (m_basisVectorNames[i].Length == 0))
                    throw new G25.UserException("Missing (null) or empty basis vector name (see XML element '" + XML.XML_BASIS_VECTOR_NAMES + "').");

            for (int i = 0; i < m_basisVectorNames.Count; i++)
                for (int j = i+1; j < m_basisVectorNames.Count; j++)
                    if (m_basisVectorNames[i] == m_basisVectorNames[j])
                        throw new G25.UserException("Identical basis vector names (" + m_basisVectorNames[i] + "), see XML element '" + XML.XML_BASIS_VECTOR_NAMES + "'.");
        }

        /// <summary>
        /// Sets basis vector name; throws exception when name is invalid or already in use
        /// </summary>
        /// <param name="idx">Index of name (int range [0, m_dimension-1]</param>
        /// <param name="name">name of basis vector</param>
        public void SetBasisVectorName(int idx, String name)
        {
            if ((name == null) || (name.Length == 0))
                throw new G25.UserException("Empty basis vector name (see XML element '" + XML.XML_BASIS_VECTOR_NAMES + "').");
            if ((idx < 0) || (idx >= m_basisVectorNames.Count))
                throw new G25.UserException("Basis vector index (" + idx + ") for basisvector '" + name + "' out of range (see XML element '" + XML.XML_BASIS_VECTOR_NAMES + "').");
            for (int i = 0; i < m_basisVectorNames.Count; i++)
                if (m_basisVectorNames[i] == name)
                    throw new G25.UserException("Duplicate basis vector names (" + name + "), see XML element '" + XML.XML_BASIS_VECTOR_NAMES + "'.");

            m_basisVectorNames[idx] = name;
        }

        /// <param name="name">The name of a basis vector</param>
        /// <returns>index (in range [0, dim) ) of basis vector with name 'name', or throws an Exception when basis vector 'name' is not defined.</returns>
        public int BasisVectorNameToIndex(String name)
        {
            for (int i = 0; i < m_basisVectorNames.Count; i++)
                if (m_basisVectorNames[i] == name)
                    return i;
            throw new Exception("Invalid basis vector name: '" + name +"'");
        }

        /// <summary>
        /// Sets the output directory to 'dirName'.
        /// Also sets 'm_outputDirectoryExplicitlySet' to true (used for XML output to know whether to emit the dir to XML)
        /// </summary>
        public void SetOutputDir(String dirName)
        {
            m_outputDirectory = dirName;
            m_outputDirectoryExplicitlySet = true;
        }

        /// <returns>the directory name for file output.</returns>
        public String GetOutputDir()
        {
            return m_outputDirectory;
        }

        /// <summary>
        /// Sets an override from 'defaultName' to 'customName'.
        /// </summary>
        /// <param name="defaultName">Default </param>
        /// <param name="customName"></param>
        public void SetOutputFilename(string defaultName, string customName)
        {
            m_outputFilenameOverrides[defaultName] = customName;
        }

        /// <returns>output filename of file 'defaultName', with possible override.</returns>
        public string GetOutputFilename(string defaultName)
        {
            if (m_outputFilenameOverrides.ContainsKey(defaultName))
                return m_outputFilenameOverrides[defaultName];
            else return defaultName;
        }

        /// <remarks>First defaultName is possibly overriden using GetOutputFilename().
        /// Then if the filename is rooted (absolute) it is returned as is. Otherwise 
        /// GetOutputDir() is appended in front of it.</remarks>
        /// <returns>Full path of output filename 'defaultName'.</returns>
        public string GetOutputPath(string defaultName)
        {
            string outputFilename = GetOutputFilename(defaultName);
            if (System.IO.Path.IsPathRooted(outputFilename)) return outputFilename;
            else return System.IO.Path.Combine(GetOutputDir(), outputFilename);
        }

        /// <returns>string representation of m_outputLanguage.</returns>
        public string GetOutputLanguageString() {
            if (m_outputLanguage == OUTPUT_LANGUAGE.C) return XML.XML_C;
            else if (m_outputLanguage == OUTPUT_LANGUAGE.CPP) return XML.XML_CPP;
            else if (m_outputLanguage == OUTPUT_LANGUAGE.JAVA) return XML.XML_JAVA;
            else if (m_outputLanguage == OUTPUT_LANGUAGE.CSHARP) return XML.XML_CSHARP;
            else if (m_outputLanguage == OUTPUT_LANGUAGE.PYTHON) return XML.XML_PYTHON;
            else if (m_outputLanguage == OUTPUT_LANGUAGE.MATLAB) return XML.XML_MATLAB;
            return "invalid";
        }

        /**
         * Inserts the verbatim code (in <c>m_verbatimCode</c>) into the generated files.
         * The list <c>generatedFiles</c> is used to find the names of the files.
         * 
         * Warnings are issued when code could not be inserted.
         * */
        public void InsertVerbatimCode(List<string> generatedFiles)
        {
            foreach (VerbatimCode VC in m_verbatimCode) {
                VC.InsertCode(m_inputDirectory, generatedFiles);
            }
        }


        /// <summary>
        /// Sets all m_inlineX to false. Used by the 'C' language.
        /// </summary>
        public void SetInlineNone()
        {
            m_inlineConstructors = 
                m_inlineSet =
                m_inlineAssign = 
                m_inlineOperators =
                m_inlineFunctions = false;
        }




        /// <summary>
        ///  The copyright of the generated code.
        /// </summary>
        public string m_copyright;

        /// <summary>
        ///  The license of the generated code.
        /// </summary>
        public string m_license;

        /// <summary>
        /// The namespace for the generated code (can be empty string for no namespace).
        /// </summary>
        public string m_namespace;

        /// <summary>
        /// The language if the generated implementation?
        /// </summary>
        public OUTPUT_LANGUAGE m_outputLanguage;

        /// <summary>
        /// If m_coordStorage is ARRAY, coordinates of specialized multivectors will be stored in arrays.
        /// If m_coordStorage is VARIABLES, coordinates of specialized multivectors will be stored seperate variables.
        /// </summary>
        public COORD_STORAGE m_coordStorage;

        /// <summary>
        /// The dimension of the generated space.
        /// </summary>
        public int m_dimension;

        /// <summary>
        /// Whether to report usage of non-optimized functions (adds printfs to the generated code).
        /// </summary>
        public bool m_reportUsage;

        /// <summary>
        /// What type of code to generate for general multivector functions
        /// (fully expand, or do stuff at run-time to save code size)
        /// </summary>
        public GMV_CODE m_gmvCodeGeneration = GMV_CODE.EXPAND;

        /// <summary>
        /// What type of parser to generate: none, custom (hand-written)
        /// or ANTLR (resulting grammar needs to be compiled and linked to ANTLR runtime library).
        /// </summary>
        public PARSER m_parserType = PARSER.NONE;

        /// <summary>
        /// When true, testing code will be generated.
        /// </summary>
        public bool m_generateTestSuite = false;



        /// <summary>Whether to inline the constructors.</summary>
        public bool m_inlineConstructors;
        /// <summary>Whether to inline the 'set' functions.</summary>
        public bool m_inlineSet;
        /// <summary>Whether to inline the assignment functions.</summary>
        public bool m_inlineAssign;
        /// <summary>Whether to inline the operators.</summary>
        public bool m_inlineOperators;
        /// <summary>Whether to inline the regular functions (like geometric product).</summary>
        public bool m_inlineFunctions;

        /// <summary>The floating point type(s) of the generated code.</summary>
        public List<FloatType> m_floatTypes;

        /// <summary>Whether to add the default operator bindings on init.</summary>
        protected internal bool m_defaultOperatorBindings = false;

        /// <summary>Operator bindings.</summary>
        public List<Operator> m_operators = new List<Operator>();

        /// <summary>Default operator bindings (used to know which operators in <c>m_operators</c>are used-defined and which are default).</summary>
        public List<Operator> m_defaultOperators = new List<Operator>();

        /// <summary>Names of basis vectors (e.g., "e1", "e2", ...)</summary>
        public List<string> m_basisVectorNames;

        /// <summary>
        /// List of Metric objects. The first entry is always the default algebra metric.
        /// </summary>
        public List<Metric> m_metric;

        /// <summary>Used to parse metric specifications (like "no.ni=-1")</summary>
        protected internal G25.rsep m_metricParser;

        /// <summary>Used to parse list of basis blades</summary>
        protected internal G25.rsbbp m_basisBladeParser;

        /// <summary>General multivector specification.</summary>
        public GMV m_GMV;

        /// <summary>Specialized multivector specifications.</summary>
        public List<SMV> m_SMV = new List<SMV>();

        /// <summary>General outermorphism matrix representation specification.
        /// Can be null if the user does not want any GOM.</summary>
        public GOM m_GOM;

        /// <summary>Specialized outermorphism matrix representation specifications.</summary>
        public List<SOM> m_SOM = new List<SOM>();

        /// <summary>Constants</summary>
        public List<Constant> m_constant = new List<Constant>();

        /// <summary>List of functions (G25.fgs) to instantiate.</summary>
        public List<fgs> m_functions = new List<fgs>();

        /// <summary>Where the specification came from, or when this is unknown, the current working directory.
        /// This directory is used to resolve relative paths (e.g., for verbatim code to be read from user-specified files).
        /// </summary>
        public string m_inputDirectory;

        /// <summary>Where the generated files go by default.</summary>
        public string m_outputDirectory;

        /// <summary>Is used to determine whether the output directory should be written to XML</summary>
        public bool m_outputDirectoryExplicitlySet;

        /// <summary>
        /// Overrides for filenames. A map from default names to custom, user specified names.
        /// </summary>
        public Dictionary<String, String> m_outputFilenameOverrides = new Dictionary<String, String>();

        /// <summary>
        /// Verbatim code fragments that should be inserted into output files.
        /// </summary>
        public List<VerbatimCode> m_verbatimCode = new List<VerbatimCode>();

    } // end of class Specification


} // end of namespace G25
