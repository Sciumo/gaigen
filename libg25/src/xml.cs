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
using System.Xml;
using System.IO;

namespace G25
{
    /// <summary>
    /// Provides XML reading and writing for Specification class.
    /// </summary>
    public class XML
    {
        
        public const string XML_C = "c";
        public const string XML_CPP = "cpp";
        public const string XML_JAVA = "java";
        public const string XML_CSHARP = "csharp";
        public const string XML_PYTHON = "python";
        public const string XML_MATLAB = "matlab";
        
        public const string XML_G25_SPEC = "g25spec";
        public const string XML_LICENSE = "license";
        public const string XML_GPL = "gpl";
        public const string XML_BSD = "bsd";
        public const string XML_CUSTOM = "custom";
        public const string XML_LANGUAGE = "language";
        public const string XML_NAMESPACE = "namespace";
        public const string XML_COORD_STORAGE = "coordStorage";
        public const string XML_ARRAY = "array";
        public const string XML_VARIABLES = "variables";
        public const string XML_DEFAULT_OPERATOR_BINDINGS = "defaultOperatorBindings";
        public const string XML_TRUE = "true";
        public const string XML_FALSE = "false";
        public const string XML_DIMENSION = "dimension";
        public const string XML_REPORT_USAGE = "reportUsage";
        public const string XML_GMV_CODE = "gmvCode";
        public const string XML_EXPAND = "expand";
        public const string XML_RUNTIME = "runtime";
        public const string XML_PARSER = "parser";
        public const string XML_TEST_SUITE = "testSuite";
        public const string XML_NONE = "none";
        public const string XML_ANTLR = "antlr";
        public const string XML_BUILTIN = "builtin";
        public const string XML_CUSTOM_LICENSE = "customLicense";
        public const string XML_INLINE = "inline";
        public const string XML_CONSTRUCTORS = "constructors";
        public const string XML_SET = "set";
        public const string XML_ASSIGN = "assign";
        public const string XML_OPERATORS = "operators";
        public const string XML_FUNCTIONS = "functions";
        public const string XML_FLOAT_TYPE = "floatType";
        public const string XML_TYPE = "type";
        public const string XML_PREFIX = "prefix";
        public const string XML_SUFFIX = "suffix";
        public const string XML_UNARY_OPERATOR = "unaryOperator";
        public const string XML_BINARY_OPERATOR = "binaryOperator";
        public const string XML_FUNCTION = "function";
        public const string XML_SYMBOL = "symbol";
        public const string XML_BASIS_VECTOR_NAMES = "basisVectorNames";
        public const string XML_NAME = "name";
        public const string XML_METRIC = "metric";
        public const string XML_ROUND = "round";
        public const string XML_MV = "mv";
        public const string XML_COMPRESS = "compress";
        public const string XML_BY_GRADE = "byGrade";
        public const string XML_BY_GROUP = "byGroup";
        public const string XML_COORDINATE_ORDER = "coordinateOrder";
        public const string XML_DEFAULT = "default";
        public const string XML_MEM_ALLOC = "memAlloc";
        public const string XML_PARITY_PURE = "parityPure";
        public const string XML_DYNAMIC = "dynamic";
        public const string XML_FULL = "full";
        public const string XML_GROUP = "group";
        public const string XML_SMV = "smv";
        public const string XML_CONST = "const";
        public const string XML_CONSTANT = "constant";
        public const string XML_MULTIVECTOR = "multivector";
        public const string XML_BLADE = "blade";
        public const string XML_ROTOR = "rotor";
        public const string XML_VERSOR = "versor";
        public const string XML_COPYRIGHT = "copyright";
        public const string XML_OUTPUT_NAME = "outputName";
        public const string XML_ARG = "arg";
        public const string XML_OPTION = "option";
        public const string XML_ARGNAME = "argName";
        public const string XML_COMMENT = "comment";
        public const string XML_OM = "om";
        public const string XML_SOM = "som";
        public const string XML_DOMAIN = "domain";
        public const string XML_RANGE = "range";
        public const string XML_RETURN_TYPE = "returnType";
        public const string XML_SCALAR = "scalar";
        public const string XML_OUTPUT_DIRECTORY = "outputDirectory";
        public const string XML_PATH = "path";
        public const string XML_OUTPUT_FILENAME = "outputFilename";
        public const string XML_DEFAULT_NAME = "defaultName";
        public const string XML_CUSTOM_NAME = "customName";
        public const string XML_VERBATIM = "verbatim";
        public const string XML_FILENAME = "filename";
        public const string XML_POSITION = "position";
        public const string XML_MARKER = "marker";
        public const string XML_CODE_FILENAME = "codeFilename";
        public const string XML_TOP = "top";
        public const string XML_BOTTOM = "bottom";
        public const string XML_BEFORE_MARKER = "before";
        public const string XML_AFTER_MARKER = "after";

        /// <summary>
        /// Converts this specification to XML (that can be parsed by the constructor)
        /// </summary>
        /// <returns>An XML string that can be parsed by the constructor</returns>
        public static string ToXmlString(Specification S)
        {
            StringBuilder SB = new StringBuilder();
            SB.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n");
            SB.AppendLine(""); // empty line
            SB.Append("<" + XML_G25_SPEC + "\n");

            bool customLicense = false; // used later on to know whether to emit full license text
            { // output attributes of g25spec element
                { // license
                    string licString = S.m_license;
                    if (licString == Licenses.GPL_LICENSE)
                        licString = XML_GPL;
                    else if (licString == Licenses.BSD_LICENSE)
                        licString = XML_BSD;
                    else
                    {
                        licString = XML_CUSTOM;
                        customLicense = true;
                    }
                    SB.Append("\t" + XML_LICENSE + "=\"" + licString + "\"\n");
                }

                { // copyright
                    if ((S.m_copyright != null) && (S.m_copyright.Length > 0))
                        SB.Append("\t" + XML_COPYRIGHT + "=\"" + S.m_copyright + "\"\n");
                }

                { // language
                    SB.Append("\t" + XML_LANGUAGE + "=\"" + S.GetOutputLanguageString() + "\"\n");
                }

                // namespace
                if ((S.m_namespace != null) && (S.m_namespace.Length > 0))
                    SB.Append("\t" + XML_NAMESPACE + "=\"" + S.m_namespace + "\"\n");

                // coordinate storage
                SB.Append("\t" + XML_COORD_STORAGE + "=\"" + ((S.m_coordStorage == COORD_STORAGE.ARRAY) ? XML_ARRAY : XML_VARIABLES) + "\"\n");

                // operator bindings
                SB.Append("\t" + XML_DEFAULT_OPERATOR_BINDINGS + "=\"" + (S.DefaultOperatorBindings() ? XML_TRUE : XML_FALSE) + "\"\n");

                // dimension
                SB.Append("\t" + XML_DIMENSION + "=\"" + S.m_dimension.ToString() + "\"\n");

                // report usage of non-optimized functions
                SB.Append("\t" + XML_REPORT_USAGE + "=\"" + (S.m_reportUsage ? XML_TRUE : XML_FALSE) + "\"\n");

                { // what type of GMV code to generate:
                    SB.Append("\t" + XML_GMV_CODE + "=\"");
                    switch (S.m_gmvCodeGeneration)
                    {
                        case GMV_CODE.EXPAND: SB.Append(XML_EXPAND); break;
                        case GMV_CODE.RUNTIME: SB.Append(XML_RUNTIME); break;
                        default: SB.Append("BAD GMV CODE OPTION"); break;
                    }
                    SB.Append("\"\n");
                }

                { // what type of parser to generate:
                    SB.Append("\t" + XML_PARSER + "=\"");
                    switch (S.m_parserType)
                    {
                        case PARSER.NONE: SB.Append(XML_NONE); break;
                        case PARSER.ANTLR: SB.Append(XML_ANTLR); break;
                        case PARSER.BUILTIN: SB.Append(XML_BUILTIN); break;
                        default: SB.Append("BAD PARSER OPTION"); break;
                    }
                    SB.Append("\"\n");
                }

                // generate test suite
                SB.Append("\t" + XML_TEST_SUITE + "=\"" + (S.m_generateTestSuite ? XML_TRUE : XML_FALSE) + "\"\n");

            }

            SB.Append("\t>\n"); // end of <g25spec> entry

            SB.AppendLine(""); // empty line

            if (customLicense) // output custom license
                SB.Append("<" + XML_CUSTOM_LICENSE + ">" + S.m_license + "</" + XML_CUSTOM_LICENSE + ">\n");

            if (S.m_verbatimCode.Count > 0) // output verbatim code
            {
                foreach (VerbatimCode VC in S.m_verbatimCode)
                {
                    // determine if code is in XML or in file:
                    bool hasCode = ((VC.m_verbatimCode != null) && (VC.m_verbatimCode.Length > 0));
                    // open XML tag
                    SB.Append("<" + XML_VERBATIM);

                    // output all filenames to which verbatim code should be applied
                    for (int f = 0; f < VC.m_filenames.Count; f++)
                    {
                        SB.Append(" " + XML_FILENAME);
                        if (f > 0) SB.Append((f + 1).ToString());
                        SB.Append("=\"" + VC.m_filenames[f] + "\"");
                    }

                    // output the filename where verbatim code comes from
                    if ((VC.m_verbatimCodeFile != null) && (VC.m_verbatimCodeFile.Length > 0))
                        SB.Append(" " + XML_CODE_FILENAME + "=\"" + VC.m_verbatimCodeFile + "\"");

                    // output position
                    SB.Append(" " + XML_POSITION + "=");
                    switch (VC.m_where)
                    {
                        case VerbatimCode.POSITION.TOP:
                            SB.Append("\"" + XML_TOP + "\"");
                            break;
                        case VerbatimCode.POSITION.BOTTOM:
                            SB.Append("\"" + XML_BOTTOM + "\"");
                            break;
                        case VerbatimCode.POSITION.BEFORE_MARKER:
                            SB.Append("\"" + XML_BEFORE_MARKER + "\"");
                            break;
                        case VerbatimCode.POSITION.AFTER_MARKER:
                            SB.Append("\"" + XML_AFTER_MARKER + "\"");
                            break;
                    }

                    if ((VC.m_where == VerbatimCode.POSITION.BEFORE_MARKER) ||
                        (VC.m_where == VerbatimCode.POSITION.AFTER_MARKER))
                    {
                        SB.Append(" " + XML_MARKER + "=\"" + VC.m_customMarker + "\"");
                    }

                    // output verbatim code that goes into XML, if present
                    if (hasCode)
                    {
                        SB.Append(">" + VC.m_verbatimCode + "</" + XML_VERBATIM + ">\n");
                    }
                    else SB.Append("/>\n");
                }
            }


            if (S.m_outputDirectoryExplicitlySet) // output dir
                SB.Append("<" + XML_OUTPUT_DIRECTORY + " " + XML_PATH + "=\"" + S.m_outputDirectory + "\"/>\n");



            { // overrides
                foreach (KeyValuePair<string, string> kvp in S.m_outputFilenameOverrides)
                {
                    SB.Append("<" + XML_OUTPUT_FILENAME + " " +
                        XML_DEFAULT_NAME + "=\"" + kvp.Key + "\" " +
                        XML_CUSTOM_NAME + "=\"" + kvp.Value +
                        "\"/>\n");
                }
            }

            SB.AppendLine(""); // empty line

            { // inline
                SB.Append("<" + XML_INLINE + "\n");
                SB.Append("\t" + XML_CONSTRUCTORS + "=\"" + ((S.m_inlineConstructors) ? XML_TRUE : XML_FALSE) + "\"\n");
                SB.Append("\t" + XML_SET + "=\"" + ((S.m_inlineSet) ? XML_TRUE : XML_FALSE) + "\"\n");
                SB.Append("\t" + XML_ASSIGN + "=\"" + ((S.m_inlineAssign) ? XML_TRUE : XML_FALSE) + "\"\n");
                SB.Append("\t" + XML_OPERATORS + "=\"" + ((S.m_inlineOperators) ? XML_TRUE : XML_FALSE) + "\"\n");
                SB.Append("\t" + XML_FUNCTIONS + "=\"" + ((S.m_inlineFunctions) ? XML_TRUE : XML_FALSE) + "\"\n");
                SB.Append("\t/>\n"); // end of <inline> entry
            }

            SB.AppendLine(""); // empty line

            { // float types
                foreach (FloatType FT in S.m_floatTypes)
                {
                    SB.Append("<" + XML_FLOAT_TYPE + " " + XML_TYPE + "=\"" + FT.type + "\"");
                    if (FT.prefix.Length > 0) SB.Append(" " + XML_PREFIX + "=\"" + FT.prefix + "\"");
                    if (FT.suffix.Length > 0) SB.Append(" " + XML_SUFFIX + "=\"" + FT.suffix + "\"");
                    SB.Append("/>\n"); // end of <floatType> entry
                }
            }

            SB.AppendLine(""); // empty line

            { // basis vector names
                SB.Append("<" + XML_BASIS_VECTOR_NAMES);
                for (int i = 0; i < S.m_basisVectorNames.Count; i++)
                    SB.Append("\n\t" + XML_NAME + (i + 1).ToString() + "=\"" + S.m_basisVectorNames[i] + "\"");
                SB.Append("\n\t/>\n"); // end of <basisVectorNames> entry
            }

            SB.AppendLine(""); // empty line

            { // metric
                // printed out in order of basisvectors
                foreach (Metric M in S.m_metric)
                {
                    if (M.m_name == Specification.INTERNAL_EUCLIDEAN_METRIC) continue; // do not emit auto-generated metric to XML
                    for (int v1 = 0; v1 < S.m_dimension; v1++)
                        for (int v2 = 0; v2 < S.m_dimension; v2++)
                            for (int i = 0; i < M.m_metricBasisVectorIdx1.Count; i++)
                                if ((v1 == M.m_metricBasisVectorIdx1[i]) && (v2 == M.m_metricBasisVectorIdx2[i]))
                                {
                                    SB.Append("<" + XML_METRIC + " " + XML_NAME + "=\"" + M.m_name + "\"");
                                    if (!M.m_round) // default = true, so only print when false
                                        SB.Append(" " + XML_ROUND + "=\"" + XML_FALSE + "\"");
                                    SB.Append(">");
                                    SB.Append(S.m_basisVectorNames[v1] + "." + S.m_basisVectorNames[v2] + "=" + M.m_metricValue[i]);
                                    SB.Append("</" + XML_METRIC + ">\n");
                                }
                }
            }

            SB.AppendLine(""); // empty line

            // operators
            foreach (Operator op in S.m_operators)
            {
                // first check if this isn't a 'default operator'
                if (S.m_defaultOperators.Contains(op)) continue;

                bool unary = (op.NbArguments == 1);
                string opStr = (unary) ? XML_UNARY_OPERATOR : XML_BINARY_OPERATOR;

                SB.Append("<" + opStr + " " + XML_SYMBOL + "=\"" + op.Symbol + "\" " + XML_FUNCTION + "=\"" + op.FunctionName + "\"");

                if (unary && (!op.IsPrefix))
                {
                    SB.Append(" " + XML_PREFIX + "=\"" + XML_FALSE + "\"");
                }

                SB.Append("/>\n");

            }


            SB.AppendLine(""); // empty line

            if (S.m_GMV != null) // general multivector:
            {
                SB.Append("<" + XML_MV);

                // name
                SB.Append(" " + XML_NAME + "=\"" + S.m_GMV.Name + "\"");

                // compression (by grade, group)
                bool compressedByGrade = S.m_GMV.IsGroupedByGrade(S.m_dimension);
                SB.Append(" " + XML_COMPRESS + "=\"");
                if (compressedByGrade) SB.Append(XML_BY_GRADE + "\"");
                else SB.Append(XML_BY_GROUP + "\"");

                // coordinate order
                bool defaultCoordinateOrder = (compressedByGrade && S.m_GMV.CompareBasisBladeOrder(rsbbp.ListToDoubleArray(S.m_basisBladeParser.GetDefaultBasisBlades())));
                SB.Append(" " + XML_COORDINATE_ORDER + "=\"");
                if (defaultCoordinateOrder) SB.Append(XML_DEFAULT + "\"");
                else SB.Append(XML_CUSTOM + "\"");

                // memory allocation method
                SB.Append(" " + XML_MEM_ALLOC + "=\"");
                if (S.m_GMV.MemoryAllocationMethod == GMV.MEM_ALLOC_METHOD.PARITY_PURE)
                    SB.Append(XML_PARITY_PURE + "\"");
                else if (S.m_GMV.MemoryAllocationMethod == GMV.MEM_ALLOC_METHOD.FULL)
                    SB.Append(XML_FULL + "\"");
                else SB.Append(XML_DYNAMIC + "\"");
                SB.Append(">\n");

                if (!defaultCoordinateOrder)
                { // emit coordinate order:
                    string[] bvNames = (string[])S.m_basisVectorNames.ToArray();
                    // loop over all groups:
                    for (int g = 0; g < S.m_GMV.NbGroups; g++)
                    {
                        SB.Append("<" + XML_GROUP + ">");
                        // loop over all basis blades of group
                        for (int i = 0; i < S.m_GMV.Group(g).Length; i++)
                        {
                            if (i > 0) SB.Append(" ");

                            string bbStr = BasisBladeToString(S.m_GMV.BasisBlade(g, i), bvNames);
                            SB.Append(bbStr);
                        }
                        SB.Append("</" + XML_GROUP + ">\n");
                    }
                }

                SB.Append("</" + XML_MV + ">\n");
            }

            SB.AppendLine(""); // empty line

            // specialized multivectors
            for (int i = 0; i < S.m_SMV.Count; i++)
            {
                SB.Append(SMVtoXmlString(S, S.m_SMV[i]));
            }

            SB.AppendLine(""); // empty line

            // constants
            for (int i = 0; i < S.m_constant.Count; i++)
            {
                // assume only SMV constants for now
                ConstantSMV C = S.m_constant[i] as ConstantSMV;
                if (C == null) continue;

                // check if type has name X+CONSTANT_TYPE_SUFFIX and is constant
                if ((C.Type.GetName().Equals(C.Name + Specification.CONSTANT_TYPE_SUFFIX)) && (C.Type as SMV).IsConstant()) continue;

                SB.Append(ConstantToXmlString(S, C));
            }

            SB.AppendLine(""); // empty line

            // outermorphisms
            {
                // i = -1 = m_GOM, the rest is m_SOM
                for (int i = -1; i < S.m_SOM.Count; i++)
                {
                    if (i == 0) SB.AppendLine(""); // empty line

                    OM om = (i == -1) ? S.m_GOM as OM : S.m_SOM[i] as OM;
                    if (om == null) continue;
                    string XMLtag = ((om is GOM) ? XML_OM : XML_SOM);

                    SB.Append("<" + XMLtag);

                    // name
                    SB.Append(" " + XML_NAME + "=\"" + om.Name + "\"");

                    // coordinate order:
                    bool rangeEqualsDomain = om.DomainAndRangeAreEqual();
                    bool defaultCoordOrder = rangeEqualsDomain && om.CompareDomainOrder(rsbbp.ListToDoubleArray(S.m_basisBladeParser.GetDefaultBasisBlades()));
                    SB.Append(" " + XML_COORDINATE_ORDER + "=\"" + ((defaultCoordOrder) ? XML_DEFAULT : XML_CUSTOM) + "\"");

                    // end of XMLtag
                    SB.Append(">\n");

                    if (!defaultCoordOrder)
                    {
                        string[] bvNames = (string[])S.m_basisVectorNames.ToArray();
                        for (int dr = 0; dr < 2; dr++)
                        {
                            string XML_DOMAIN_OR_RANGE = (dr == 0) ? XML_DOMAIN : XML_RANGE;
                            RefGA.BasisBlade[][] B = (dr == 0) ? om.Domain : om.Range;
                            if ((dr == 1) && rangeEqualsDomain) continue;

                            SB.Append("<" + XML_DOMAIN_OR_RANGE + ">");
                            bool first = true;
                            for (int g = 0; g < B.Length; g++)
                            {
                                for (int b = 0; b < B[g].Length; b++)
                                {
                                    if (!first) SB.Append(" ");

                                    String bbStr = BasisBladeToString(B[g][b], bvNames);
                                    SB.Append(bbStr);

                                    first = false;
                                }
                            }


                            SB.Append("</" + XML_DOMAIN_OR_RANGE + ">\n");

                        }
                        // output domain info

                        if (!rangeEqualsDomain)
                        {
                            // output range info
                        }
                    }


                    SB.Append("</" + XMLtag + ">\n");
                }
            }

            SB.AppendLine(""); // empty line

            // function generation specifications
            for (int i = 0; i < S.m_functions.Count; i++)
                SB.AppendLine(FunctionToXmlString(S, S.m_functions[i]));

            SB.AppendLine(""); // empty line

            SB.Append("</" + XML_G25_SPEC + ">\n");

            return SB.ToString();
        } // end of function ToXmlString()


        /// <summary>
        /// Converts a RefGA.BasisBlade to a string for printout in the specification.
        /// The blade should have positive or negative 1 scale. A scalar basis blade
        /// is transformed into "scalar" or "-scalar".
        /// </summary>
        protected static string BasisBladeToString(RefGA.BasisBlade B, string[] bvNames)
        {
            String bbStr = B.ToString(bvNames);
            // convert "-1*" to "-", otherwise the string cannot be parsed back in again
            if (bbStr.StartsWith("-1*")) bbStr = "-" + bbStr.Substring(3);

            if (bbStr == "1") bbStr = "scalar";
            if (bbStr == "-1") bbStr = "-scalar";
            return bbStr;
        }



        /// <summary>
        /// Converts a G25.fgs to an XML string representation.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="F"></param>
        /// <returns>XML string representation of 'F'.</returns>
        public static string FunctionToXmlString(Specification S, G25.fgs F)
        {
            StringBuilder SB = new StringBuilder();
            SB.Append("<" + XML_FUNCTION);

            // name
            SB.Append(" " + XML_NAME + "=\"" + F.Name + "\"");

            // output name, if different
            if (F.Name != F.OutputName)
                SB.Append(" " + XML_OUTPUT_NAME + "=\"" + F.OutputName + "\"");

            // return type, if set
            if (F.ReturnTypeName.Length > 0)
                SB.Append(" " + XML_RETURN_TYPE + "=\"" + F.ReturnTypeName + "\"");

            // argument types, names
            for (int a = 0; a < F.NbArguments; a++)
            {
                string argTypeName = F.ArgumentTypeNames[a];

                if (argTypeName.EndsWith(Specification.CONSTANT_TYPE_SUFFIX)) // check if it is a constant
                {
                    G25.SMV smv = S.GetType(argTypeName) as G25.SMV;
                    if ((smv != null) && smv.IsConstant())
                    { // if constant, remove the "_t" from the typename
                        argTypeName = argTypeName.Substring(0, argTypeName.Length - Specification.CONSTANT_TYPE_SUFFIX.Length);
                    }
                }

                SB.Append(" " + XML_ARG + (a + 1).ToString() + "=\"" + F.ArgumentTypeNames[a] + "\"");
                if (F.ArgumentVariableNames[a] != fgs.DefaultArgumentName(a))
                    SB.Append(" " + XML_ARGNAME + (a + 1).ToString() + "=\"" + F.ArgumentVariableNames[a] + "\"");
            }

            // options
            {
                foreach (KeyValuePair<String, String> KVP in F.Options)
                {
                    SB.Append(" " + XML_OPTION + KVP.Key + "=\"" + KVP.Value + "\"");
                }
            }

            // float names, if not all float names of algebra are used:
            if (!F.UsesAllFloatTypes(S.m_floatTypes))
            {
                for (int f = 0; f < F.NbFloatNames; f++)
                {
                    SB.Append(" " + XML_FLOAT_TYPE + "=\"" + F.FloatNames[f] + "\"");
                }
            }

            // metric name, if not default
            if (F.MetricName != "default")
                SB.Append(" " + XML_METRIC + "=\"" + F.MetricName + "\"");

            // metric name, if not default
            if (F.Comment.Length > 0)
                SB.Append(" " + XML_COMMENT + "=\"" + F.Comment + "\"");

            SB.Append("/>");

            return SB.ToString();
        } // end of FunctionToXmlString()

        /// <summary>
        /// Converts a G25.SMV to an XML string representation.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="smv"></param>
        /// <returns>XML string representation of 'smv'.</returns>
        public static string SMVtoXmlString(Specification S, G25.SMV smv)
        {
            StringBuilder SB = new StringBuilder();
            bool constant = smv.IsConstant() && (S.GetMatchingConstant(smv) != null);
            string name = smv.Name;

            // remove the extra constant suffix?
            if (constant && name.EndsWith(Specification.CONSTANT_TYPE_SUFFIX))
                name = name.Substring(0, name.Length - Specification.CONSTANT_TYPE_SUFFIX.Length);

            SB.Append("<" + XML_SMV);

            // name
            SB.Append(" " + XML_NAME + "=\"" + name + "\"");

            // constant?
            if (constant)
                SB.Append(" " + XML_CONST + "=\"" + XML_TRUE + "\"");

            // type
            SB.Append(" " + XML_TYPE + "=\"" + smv.MvTypeString + "\"");

            // end of XML_SMV tag
            SB.Append(">");

            { // emit coordinate order:
                string[] bvNames = (string[])S.m_basisVectorNames.ToArray();
                // loop over all basis blades
                for (int b = 0; b < smv.Group(0).Length; b++)
                {
                    if (b > 0) SB.Append(" ");
                    string bbStr = BasisBladeToString(smv.BasisBlade(0, b), bvNames);
                    SB.Append(bbStr);

                    // if constant, add '=....'
                    if (smv.IsCoordinateConstant(b))
                        SB.Append("=" + smv.ConstBasisBladeValue(smv.BladeIdxToConstBladeIdx(b)).ToString());
                }
                if (smv.Comment.Length > 0)
                    SB.Append("  <" + XML_COMMENT + ">" + smv.Comment + "</" + XML_COMMENT + ">");
            }

            SB.Append("</" + XML_SMV + ">\n");

            return SB.ToString();
        } // end of SMVtoXmlString()



        /// <summary>
        /// Converts a G25.Constant to an XML string representation.
        /// </summary>
        /// <param name="C"></param>
        /// <param name="S"></param>
        /// <returns>XML string representation of 'smv'.</returns>
        public static string ConstantToXmlString(Specification S, G25.Constant C)
        {
            StringBuilder SB = new StringBuilder();

            SB.Append("<" + XML_CONSTANT);

            // name
            SB.Append(" " + XML_NAME + "=\"" + C.Name + "\"");

            // type
            SB.Append(" " + XML_TYPE + "=\"" + C.Type.GetName() + "\"");

            // end of XML_CONSTANT tag
            SB.Append(">");

            { // emit value (assuming only SMV constants, for now)
                SMV smv = C.Type as SMV;
                ConstantSMV Csmv = C as ConstantSMV;
                string[] bvNames = (string[])S.m_basisVectorNames.ToArray();
                for (int b = 0; b < smv.NbNonConstBasisBlade; b++)
                {
                    RefGA.BasisBlade B = smv.NonConstBasisBlade(b);
                    String basisBladeString = BasisBladeToString(B, bvNames);

                    if (b > 0) SB.Append(" ");
                    SB.Append(basisBladeString);
                    SB.Append("=" + Csmv.Value[b]);
                }
                if (C.Comment.Length > 0)
                    SB.Append("  <" + XML_COMMENT + ">" + C.Comment + "</" + XML_COMMENT + ">");
            }

            SB.Append("</" + XML_CONSTANT + ">\n");

            return SB.ToString();
        } // end of ConstantToXmlString()

        /// <summary>
        /// Initializes an empty Specification from an XML document
        /// </summary>
        public static void InitFromXmlDocument(Specification S, XmlDocument doc)
        {
            XmlElement rootElement = doc.DocumentElement;
            //System.Console.WriteLine(rootElement.Name);
            if (rootElement.Name != XML_G25_SPEC)
                throw new G25.UserException("Missing root element " + XML_G25_SPEC + " in XML file.");

            ParseRootElement(S, rootElement);

            // initializes the RefGA.Metric variables of the m_metrics
            S.FinishMetric();

            // check if specification is sane
            S.CheckSpecificationSanity();
        }

        private static void ParseRootElement(Specification S, XmlElement rootElement)
        {
            ParseRootElementAttributes(S, rootElement.Attributes);

            XmlNode _E = rootElement.FirstChild;
            while (_E != null)
            {
                XmlElement E = _E as XmlElement;
                if (E != null)
                {
                    switch (E.Name)
                    {
                        case XML_CUSTOM_LICENSE:
                            {
                                if (S.m_license != XML_CUSTOM)
                                    throw new G25.UserException("License was not set to '" + XML_CUSTOM + "' but there still is a '" + XML_CUSTOM_LICENSE + "' in the specification");
                                XmlText T = E.FirstChild as XmlText;
                                if (T != null)
                                    S.m_license = T.Value;
                            }
                            break;
                        case XML_OUTPUT_DIRECTORY:
                            S.SetOutputDir(E.GetAttribute(XML_PATH));
                            break;
                        case XML_OUTPUT_FILENAME:
                            S.SetOutputFilename(E.GetAttribute(XML_DEFAULT_NAME), E.GetAttribute(XML_CUSTOM_NAME));
                            break;

                        case XML_INLINE:
                            ParseInlineAttributes(S, E.Attributes);
                            break;
                        case XML_FLOAT_TYPE:
                            ParseFloatTypeAttributes(S, E.Attributes);
                            break;
                        case XML_UNARY_OPERATOR:
                        case XML_BINARY_OPERATOR:
                            ParseOperatorAttributes(S, E.Name, E.Attributes);
                            break;
                        case XML_BASIS_VECTOR_NAMES:
                            ParseBasisVectorNamesAttributes(S, E.Attributes);
                            break;
                        case XML_METRIC:
                            {
                                // name
                                string name = E.GetAttribute(XML_NAME);
                                if (name == null) name = "default";

                                // parse actual metric:
                                XmlText T = E.FirstChild as XmlText;
                                if (T == null) throw new G25.UserException("Invalid  '" + XML_METRIC + "' element in specification.");
                                ParseMetric(S, name, T.Value);

                                // round?
                                if (E.GetAttribute(XML_ROUND) != null)
                                {
                                    S.GetMetric(name).m_round = !(E.GetAttribute(XML_ROUND).ToLower() == XML_FALSE);
                                }

                            }
                            break;
                        case XML_MV:
                            ParseMVelementAndAttributes(S, E);
                            break;
                        case XML_SMV:
                            ParseSMVelementAndAttributes(S, E);
                            break;
                        case XML_OM:
                            ParseGOMelementAndAttributes(S, E);
                            break;
                        case XML_SOM:
                            ParseSOMelementAndAttributes(S, E);
                            break;
                        case XML_CONSTANT:
                            ParseConstantElementAndAttributes(S, E);
                            break;
                        case XML_FUNCTION:
                            ParseFunction(S, E);
                            break;
                        case XML_VERBATIM:
                            ParseVerbatim(S, E);
                            break;
                        default:
                            System.Console.WriteLine("Specification.ParseRootElement(): warning: unknown element '" + E.Name + "' in specification");
                            break;
                    }
                }

                _E = _E.NextSibling;
            }



        }

        /// <summary>
        /// Parses the attributes of the XML_G25_SPEC root element
        /// </summary>
        private static void ParseRootElementAttributes(Specification S, XmlAttributeCollection A)
        {
            // parse all attributes of the root element
            for (int i = 0; i < A.Count; i++)
            {
                switch (A[i].Name)
                {
                    case XML_LICENSE:
                        S.SetLicense(A[i].Value);
                        break;
                    case XML_COPYRIGHT:
                        S.m_copyright = A[i].Value;
                        break;
                    case XML_LANGUAGE:
                        S.SetLanguage(A[i].Value);
                        break;
                    case XML_NAMESPACE:
                        S.m_namespace = A[i].Value;
                        break;
                    case XML_COORD_STORAGE:
                        if (A[i].Value == XML_ARRAY)
                            S.m_coordStorage = COORD_STORAGE.ARRAY;
                        else if (A[i].Value == XML_VARIABLES)
                            S.m_coordStorage = COORD_STORAGE.VARIABLES;
                        else throw new G25.UserException("XML parsing error: Unknown attribute value '" + A[i].Value + "' for attribute '" + XML_COORD_STORAGE + "'.");
                        break;
                    case XML_DEFAULT_OPERATOR_BINDINGS:
                        if (A[i].Value.ToLower() == XML_TRUE)
                            S.SetDefaultOperatorBindings();
                        break;
                    case XML_DIMENSION:
                        int dim;
                        try
                        {
                            dim = System.Int32.Parse(A[i].Value);
                        }
                        catch (System.Exception) { throw new G25.UserException("Invalid dimension for space of algebra: '" + A[i].Value + "'."); }
                        S.SetDimension(dim);
                        break;
                    case XML_REPORT_USAGE:
                        S.m_reportUsage = (A[i].Value.ToLower() == XML_TRUE);
                        break;
                    case XML_GMV_CODE:
                        if (A[i].Value.ToLower() == XML_RUNTIME)
                            S.m_gmvCodeGeneration = GMV_CODE.RUNTIME;
                        else if (A[i].Value.ToLower() == XML_EXPAND)
                            S.m_gmvCodeGeneration = GMV_CODE.EXPAND;
                        else throw new G25.UserException("Invalid value '" + A[i].Value + "' for attribute '" + XML_GMV_CODE + "'.");
                        break;
                    case XML_PARSER:
                        if (A[i].Value.ToLower() == XML_NONE)
                            S.m_parserType = PARSER.NONE;
                        else if (A[i].Value.ToLower() == XML_ANTLR)
                            S.m_parserType = PARSER.ANTLR;
                        else if (A[i].Value.ToLower() == XML_BUILTIN)
                            S.m_parserType = PARSER.BUILTIN;
                        else throw new G25.UserException("Invalid value '" + A[i].Value + "' for attribute '" + XML_PARSER + "'.");
                        break;
                    case XML_TEST_SUITE:
                        S.m_generateTestSuite = (A[i].Value.ToLower() == XML_TRUE);
                        break;
                    default:
                        throw new G25.UserException("XML parsing error: Unknown XML attribute '" + A[i].Name + "' in root element '" + XML_G25_SPEC + "'.");
                }
            }
        }


        /// <summary>
        /// Parses the attributes of the XML_INLINE element
        /// </summary>
        private static void ParseInlineAttributes(Specification S, XmlAttributeCollection A)
        {
            // parse all attributes of the root element
            for (int i = 0; i < A.Count; i++)
            {
                bool val = (A[i].Value.ToLower() == XML_TRUE);
                switch (A[i].Name)
                {
                    case XML_CONSTRUCTORS:
                        S.m_inlineConstructors = val;
                        break;
                    case XML_SET:
                        S.m_inlineSet = val;
                        break;
                    case XML_ASSIGN:
                        S.m_inlineAssign = val;
                        break;
                    case XML_OPERATORS:
                        S.m_inlineOperators = val;
                        break;
                    case XML_FUNCTIONS:
                        S.m_inlineFunctions = val;
                        break;
                    default:
                        throw new G25.UserException("XML parsing error: Unknown attribute '" + A[i].Name + "' in element '" + XML_INLINE + "'.");
                }
            }
        }

        /// <summary>
        /// Parses the attributes of the XML_FLOAT_TYPE element
        /// </summary>
        private static void ParseFloatTypeAttributes(Specification S, XmlAttributeCollection A)
        {
            String floatType = "", floatSuffix = "", floatPrefix = "";
            for (int i = 0; i < A.Count; i++)
            {
                switch (A[i].Name)
                {
                    case XML_TYPE:
                        floatType = A[i].Value;
                        break;
                    case XML_PREFIX:
                        floatPrefix = A[i].Value;
                        break;
                    case XML_SUFFIX:
                        floatSuffix = A[i].Value;
                        break;
                    default:
                        throw new G25.UserException("XML parsing error: Unknown attribute '" + A[i].Name + "' in element '" + XML_FLOAT_TYPE + "'.");
                }
            }

            S.AddFloatType(floatType, floatPrefix, floatSuffix);
        }

        private static void ParseOperatorAttributes(Specification S, string elementName, XmlAttributeCollection A)
        {
            int nbArgs = (elementName == XML_UNARY_OPERATOR) ? 1 : 2;
            String symbol = "";
            String function = "";
            bool prefix = true;
            bool prefixAttributeSpecified = false;

            for (int i = 0; i < A.Count; i++)
            {
                switch (A[i].Name)
                {
                    case XML_PREFIX:
                        prefix = (A[i].Value.ToLower() == XML_TRUE);
                        prefixAttributeSpecified = true;
                        break;
                    case XML_FUNCTION:
                        function = A[i].Value;
                        break;
                    case XML_SYMBOL:
                        symbol = A[i].Value;
                        break;
                    default:
                        throw new G25.UserException("XML parsing error: Unknown attribute '" + A[i].Name + "' in specification.");
                }
            }

            if ((nbArgs != 1) && prefixAttributeSpecified)
                throw new G25.UserException("Prefix specified for operator '" + symbol + "' bound to '" + function + "' (todo: improve this error message).");

            S.AddOperator(new Operator(nbArgs, prefix, symbol, function));
        }

        /// <summary>
        /// Sets all basis vector names to "", then handle XML attributes (nameX = "..") and then checks if all names have been set.
        /// </summary>
        private static void ParseBasisVectorNamesAttributes(Specification S, XmlAttributeCollection A)
        {
            // reset all names to ""
            for (int i = 0; i < S.m_basisVectorNames.Count; i++)
                S.m_basisVectorNames[i] = "";

            // handle all attributes
            for (int i = 0; i < A.Count; i++)
            {
                if (!A[i].Name.StartsWith(XML_NAME))
                    throw new G25.UserException("XML parsing error: Invalid attribute '" + A[i].Name + "' in element '" + XML_BASIS_VECTOR_NAMES + "'.");

                int idx;
                try
                {
                    idx = System.Int32.Parse(A[i].Name.Substring(XML_NAME.Length)) - 1;
                }
                catch (System.Exception)
                {
                    throw new G25.UserException("XML parsing error: Invalid attribute '" + A[i].Name + "' in element '" + XML_BASIS_VECTOR_NAMES + "'.");
                }
                S.SetBasisVectorName(idx, A[i].Value);
            }

            // check if all have been set 
            S.CheckBasisVectorNames();
        }

        /// <summary>
        /// Parses a metric string like "no.ni=-1" and adds it to the metric definitions.
        /// </summary>
        /// <param name="name">name of the metric (e.g., "default" or "euclidean")</param>
        /// <param name="str">A string "no.ni=-1". The string must be of the from id.id=(id.id=)*=(+-)?1. Valid examples are 
        /// "e1.e1=e2.e2=+1.2", "e3.e3=-1.2", 
        /// </param>
        private static void ParseMetric(Specification S, string name, string str)
        {
            Object O = S.m_metricParser.Parse(str);
            if (O == null) throw new G25.UserException("Error parsing metric specification '" + str + "'.");

            // System.Console.WriteLine("str -> " + O.ToString());
            // assign(e1.e1, assign(e2.e2, assign(e3.e3, 1)))

            G25.rsep.FunctionApplication FA = O as G25.rsep.FunctionApplication;
            if (FA == null) throw new G25.UserException("Invalid metric specification '" + str + "'.");

            ParseMetric(S, name, FA, str);
        }

        /// <summary>
        /// Called by ParseMetric(String str). Strips the assign()s until it find the value
        /// </summary>
        /// <param name="name">name of the metric (e.g., "default" or "euclidean")</param>
        /// <returns>The value for X . Y metric</returns>
        private static double ParseMetric(Specification S, string name, Object O, string str)
        {
            // first check if 'O' could be the final value of the metric specification
            G25.rsep.FunctionApplication FA = O as G25.rsep.FunctionApplication;
            if ((FA == null) || (FA.FunctionName == "negate") || (FA.FunctionName == "nop"))
            {
                return ParseMetricValue(S, O, str);
            }

            // This FA should be of the form X.Y=value
            // First argument must be a XdotY function application, and the function name should be "assign"
            G25.rsep.FunctionApplication XdotY = FA.Arguments[0] as G25.rsep.FunctionApplication;
            if ((FA.NbArguments != 2) || (FA.FunctionName != "assign") ||
                (XdotY == null) || (XdotY.NbArguments != 2) ||
                (XdotY.FunctionName != "ip")) throw new G25.UserException("Invalid metric specification '" + str + "'");

            // get value by recursing
            double value = ParseMetric(S, name, FA.Arguments[1], str);

            // get, check names of basis vectors
            string basisVectorName1 = XdotY.Arguments[0] as String;
            string basisVectorName2 = XdotY.Arguments[1] as String;
            if ((basisVectorName1 == null) || (basisVectorName2 == null))
                throw new G25.UserException("Invalid basis vector names in metric specification '" + str + "'");

            int basisVectorIdx1 = S.BasisVectorNameToIndex(basisVectorName1);
            int basisVectorIdx2 = S.BasisVectorNameToIndex(basisVectorName2);

            S.SetMetric(name, basisVectorIdx1, basisVectorIdx2, value);

            return value;
        }

        /// <summary>
        /// Called by ParseMetric(Object O, String str). 
        /// </summary>
        /// <param name="O">Either a string or an Object.</param>
        /// <param name="str">The full metric specification (used only for descriptions in Exceptions).</param>
        /// <returns>The value of the metric specification or throws an Exception.</returns>
        private static double ParseMetricValue(Specification S, Object O, String str)
        {
            // is it a string? If so, parse it
            String Ostr = O as String;
            if (Ostr != null)
            {
                try
                {
                    return System.Double.Parse(Ostr);
                }
                catch (System.Exception)
                {
                    throw new G25.UserException("Invalid value in metric specification '" + str + "'");
                }
            }

            // it must be negate(...) or nop(...)
            G25.rsep.FunctionApplication FA = O as G25.rsep.FunctionApplication;
            if ((FA == null) || (FA.NbArguments != 1) || (!((FA.FunctionName == "negate") || (FA.FunctionName == "nop"))))
                throw new G25.UserException("Invalid value in metric specification '" + str + "'");

            if (FA.FunctionName == "negate")
                return -ParseMetricValue(S, FA.Arguments[0], str);
            else return ParseMetricValue(S, FA.Arguments[0], str);
        }

        /// <summary>
        /// Parses an XML_MV element and stores the result.
        /// 
        /// The XML_MV element should contain the name, compression method (by grade or by group), 
        /// coordinate order (default or custom) 
        /// and memory allocation method of the general multivector.
        /// </summary>
        /// <param name="E">XML_MV element</param>
        private static void ParseMVelementAndAttributes(Specification S, XmlElement E)
        {
            String name = "mv";
            bool compressByGrade = true; // false means 'by group'
            bool defaultCoordinateOrder = true; // false means 'custom'
            GMV.MEM_ALLOC_METHOD memAllocMethod = GMV.MEM_ALLOC_METHOD.FULL;

            { // handle attributes
                XmlAttributeCollection A = E.Attributes;

                // handle all attributes
                for (int i = 0; i < A.Count; i++)
                {
                    // name
                    if (A[i].Name == XML_NAME)
                        name = A[i].Value;

                    // compress method (grade or group
                    else if (A[i].Name == XML_COMPRESS)
                    {
                        if (A[i].Value == XML_BY_GRADE)
                            compressByGrade = true;
                        else if (A[i].Value == XML_BY_GROUP)
                            compressByGrade = false;
                        else throw new G25.UserException("XML parsing error: Invalid compression '" + A[i].Value + "' in element '" + XML_MV + "'.");
                    }

                    // coordinate order
                    else if (A[i].Name == XML_COORDINATE_ORDER)
                    {
                        if (A[i].Value == XML_DEFAULT)
                            defaultCoordinateOrder = true;
                        else if (A[i].Value == XML_CUSTOM)
                            defaultCoordinateOrder = false;
                        else throw new G25.UserException("XML parsing error: Invalid coordinate order '" + A[i].Value + "' in element '" + XML_MV + "'.");
                    }

                    // memory allocation method
                    else if (A[i].Name == XML_MEM_ALLOC)
                    {
                        if (A[i].Value == XML_PARITY_PURE)
                            memAllocMethod = GMV.MEM_ALLOC_METHOD.PARITY_PURE;
                        else if (A[i].Value == XML_FULL)
                            memAllocMethod = GMV.MEM_ALLOC_METHOD.FULL;
                        else if (A[i].Value == XML_DYNAMIC)
                            memAllocMethod = GMV.MEM_ALLOC_METHOD.DYNAMIC;
                        else throw new G25.UserException("XML parsing error: Invalid memory allocation method '" + A[i].Value + "' in element '" + XML_MV + "'.");
                    }
                }

            } // end of 'handle attributes'

            // check for sanity
            if ((compressByGrade == false) && (defaultCoordinateOrder == true))
            {
                throw new G25.UserException("Cannot compress by group without a custom coordinate order. Please specify a coordinate order in the '" + XML_MV + "' element.");
            }

            // basis blades go here
            List<List<G25.rsbbp.BasisBlade>> basisBlades = null;

            if (defaultCoordinateOrder)
            {
                // check for sanity
                if (E.FirstChild != null)
                    throw new G25.UserException("The coordinate order is set to default, but the multivector definition element '" + XML_MV + "' contains a custom coordinate order.");

                basisBlades = S.m_basisBladeParser.GetDefaultBasisBlades();
            }
            else
            {
                basisBlades = S.m_basisBladeParser.ParseMVbasisBlades(E);
                if (compressByGrade)
                    basisBlades = S.m_basisBladeParser.SortBasisBladeListByGrade(basisBlades);
            }

            if (rsbbp.ConstantsInList(basisBlades))
                throw new G25.UserException("Constant coordinate(s) were specified in the general multivector type (XML element '" + XML_MV + "')");

            S.SetGeneralMV(new GMV(name, rsbbp.ListToDoubleArray(basisBlades), memAllocMethod));
        }

        /// <summary>
        /// Parses basis blade list of SMVs and constants.
        /// </summary>
        /// <returns>List of basis blades, or null if 'F' does not contain such a list.</returns>
        private static List<G25.rsbbp.BasisBlade> ParseBasisBladeList(Specification S, XmlNode _F, string parentName)
        {
            while (_F != null)
            {
                XmlText FT = _F as XmlText;

                // is it text?
                if (FT != null)
                {
                    try
                    {
                        return S.m_basisBladeParser.ParseBasisBlades(FT);
                    }
                    catch (Exception Ex)
                    {
                        throw new G25.UserException("While parsing basis blades of '" + parentName + "':\n" + Ex.Message);
                    }
                }

                _F = _F.NextSibling;
            }
            return null;
        }

        /// <summary>
        /// Parses a comment for SMVs and constants.
        /// </summary>
        /// <returns>Comment string, or null if 'F' does not contain a comment.</returns>
        private static  string ParseComment(Specification S, XmlNode _F)
        {
            while (_F != null)
            {
                // or comment?
                XmlElement FE = _F as XmlElement;
                if ((FE != null) && (FE.Name == XML_COMMENT))
                {
                    XmlText CT = FE.FirstChild as XmlText;
                    return CT.Value;
                }

                _F = _F.NextSibling;
            }
            return null;
        }



        /// <summary>
        /// Parses an XML_SMV element and stores the result.
        /// 
        /// The XML_SMV element should contain the name, and optionally the type, const property and constant name.
        /// 
        /// If the const property is true and no name is specified, the regular name is used as the name
        /// of the singleton constant and the name of the type is renamed to name_t.
        /// 
        /// </summary>
        /// <param name="E">XML_SMV element</param>
        private static void ParseSMVelementAndAttributes(Specification S, XmlElement E)
        {
            string typeName = null;
            bool isConstant = false;
            string constantName = null;
            SMV.MULTIVECTOR_TYPE mvType = SMV.MULTIVECTOR_TYPE.MULTIVECTOR;

            { // handle attributes
                XmlAttributeCollection A = E.Attributes;

                // handle all attributes
                for (int i = 0; i < A.Count; i++)
                {
                    // name
                    if (A[i].Name == XML_NAME)
                        typeName = A[i].Value;

                    // const
                    else if (A[i].Name == XML_CONST)
                        isConstant = (A[i].Value.ToLower() == XML_TRUE);

                    // type
                    else if (A[i].Name == XML_TYPE)
                    {
                        if (A[i].Value == XML_MULTIVECTOR)
                            mvType = SMV.MULTIVECTOR_TYPE.MULTIVECTOR;
                        else if (A[i].Value == XML_BLADE)
                            mvType = SMV.MULTIVECTOR_TYPE.BLADE;
                        else if (A[i].Value == XML_ROTOR)
                            mvType = SMV.MULTIVECTOR_TYPE.ROTOR;
                        else if (A[i].Value == XML_VERSOR)
                            mvType = SMV.MULTIVECTOR_TYPE.VERSOR;
                        else throw new G25.UserException("XML parsing error: Invalid value for attribute'" + XML_TYPE + "':'" + A[i].Value + "' in element '" + XML_SMV + "'.");
                    }
                }

                // sanity check on name
                if (typeName == null)
                    throw new G25.UserException("XML parsing error: Missing '" + XML_NAME + "' attribute in element '" + XML_SMV + "'.");

                // if constant and no constantName provided, use the typeName as the constantName, and set typeName to typeName + "_t" 
                if (isConstant && (constantName == null))
                {
                    // if a constant should be generated and no constant name is specified
                    constantName = typeName;
                    typeName = constantName + Specification.CONSTANT_TYPE_SUFFIX;
                }

                // check if name is already present
                if (S.IsTypeName(typeName))
                    throw new G25.UserException("In specialized multivector definition: type '" + typeName + "' already exists.");

            } // end of 'handle attributes'

            // parse list of basis blades and optional comment
            List<G25.rsbbp.BasisBlade> L = ParseBasisBladeList(S, E.FirstChild, typeName);
            string comment = ParseComment(S, E.FirstChild);

            if (L == null)
                throw new G25.UserException("XML parsing error in element '" + XML_SMV + "': Missing basis blade list for specialized multivector '" + typeName + "'");

            SMV smv = new SMV(typeName, L.ToArray(), mvType, comment);

            // add new type to list of specialized multivectors
            S.AddSpecializedMV(smv);

            // todo: add code for adding constant here
            if (constantName != null)
                S.AddConstant(new ConstantSMV(constantName, smv, null, comment));
        } // end of ParseSMVelementAndAttributes()

        /// <summary>
        /// Parses an XML_CONSTANT element and stores the result.
        /// 
        /// The XML_CONSTANT element should contain the name, type and basis blades values.
        /// </summary>
        /// <param name="E">XML_CONSTANT element</param>
        private static void ParseConstantElementAndAttributes(Specification S, XmlElement E)
        {
            string constantName = null;
            string typeName = null;

            { // handle attributes
                XmlAttributeCollection A = E.Attributes;

                // handle all attributes
                for (int i = 0; i < A.Count; i++)
                {
                    // name
                    if (A[i].Name == XML_NAME)
                        constantName = A[i].Value;

                    // type
                    else if (A[i].Name == XML_TYPE)
                        typeName = A[i].Value;
                }

                // sanity check on name
                if (constantName == null)
                    throw new G25.UserException("XML parsing error: Missing '" + XML_NAME + "' attribute in element '" + XML_CONSTANT + "'.");
                if (typeName == null)
                    throw new G25.UserException("XML parsing error: Missing '" + XML_TYPE + "' attribute in element '" + XML_CONSTANT + "'.");

                // check if name is already present
                if (!S.IsSpecializedMultivectorName(typeName))
                    throw new G25.UserException("In constant definition: type '" + typeName + "' is not a specialized multivector.");

            } // end of 'handle attributes'

            // parse list of basis blades and optional comment
            List<G25.rsbbp.BasisBlade> L = ParseBasisBladeList(S, E.FirstChild, constantName);
            string comment = ParseComment(S, E.FirstChild);

            SMV type = S.GetType(typeName) as SMV;

            // add new type to list of specialized multivectors (constuctor should check if all are constant
            S.AddConstant(new ConstantSMV(constantName, type, L, comment));
        } // end of ParseConstantElementAndAttributes()

        private static void ParseGOMelementAndAttributes(Specification S, XmlElement E)
        {
            bool specialized = false;
            GOM gom = ParseOMelementAndAttributes(S, E, specialized) as GOM;
            if (S.IsTypeName(gom.Name))
                throw new G25.UserException("In general outermorphism definition: a type '" + gom.Name + "' already exists.");
            S.SetGeneralOM(gom);
        }


        private static void ParseSOMelementAndAttributes(Specification S, XmlElement E)
        {
            bool specialized = true;
            SOM som = ParseOMelementAndAttributes(S, E, specialized) as SOM;
            if (S.IsTypeName(som.Name))
                throw new G25.UserException("In specialized outermorphism definition: a type '" + som.Name + "' already exists.");
            S.AddSpecializedOM(som);
        }


        /// <summary>
        /// Parses an XML_OM or XML_SOM element and returns the result.
        /// 
        /// The element should contain the name and coordinate order (default or custom) 
        /// of the outermorphism matrix representation.
        /// </summary>
        /// <param name="E">XML_OM or XML_SOM element.</param>
        /// <param name2="specialized">whether a general (false) or specialized (true) outermorphism is being parsed.</param>
        private static OM ParseOMelementAndAttributes(Specification S, XmlElement E, bool specialized)
        {
            String name = null;
            bool defaultCoordinateOrder = false; // false means 'custom'

            { // handle attributes
                XmlAttributeCollection A = E.Attributes;

                // handle all attributes
                for (int i = 0; i < A.Count; i++)
                {
                    // name
                    if (A[i].Name == XML_NAME)
                        name = A[i].Value;

                     // coordinate order
                    else if (A[i].Name == XML_COORDINATE_ORDER)
                    {
                        if (A[i].Value == XML_DEFAULT)
                            defaultCoordinateOrder = true;
                        else if (A[i].Value == XML_CUSTOM)
                            defaultCoordinateOrder = false;
                        else throw new G25.UserException("XML parsing error: Invalid coordinate order '" + A[i].Value + "' in element '" + E.Name + "'.");
                    }
                }
            } // end of 'handle attributes'

            // sanity check name
            if (name == null)
                throw new G25.UserException("XML parsing error: Missing name attribute in element '" + E.Name + "'");

            // domain, range go here 
            List<List<G25.rsbbp.BasisBlade>> domain = null;
            List<List<G25.rsbbp.BasisBlade>> range = null;

            if (defaultCoordinateOrder)
            {
                domain = range = S.m_basisBladeParser.GetDefaultBasisBlades();
            }
            else
            {
                // get domain & range elements , parse their internals
                XmlNode _DR = E.FirstChild;
                while (_DR != null)
                {
                    XmlElement DR = _DR as XmlElement;
                    switch (DR.Name)
                    {
                        case XML_DOMAIN:
                            domain = S.m_basisBladeParser.ParseMVbasisBlades(DR);
                            break;
                        case XML_RANGE:
                            range = S.m_basisBladeParser.ParseMVbasisBlades(DR);
                            break;
                        default:
                            System.Console.WriteLine("XML parsing warning: unknown element '" + E.Name + "' in element '" + E.Name + "'.");
                            break;
                    }
                    _DR = _DR.NextSibling;
                }

            }

            if (domain == null)
                throw new G25.UserException("XML parsing error: Missing element '" + XML_DOMAIN + "' in element '" + E.Name + "' (name=" + name + ").");

            if (rsbbp.ConstantsInList(domain))
                throw new G25.UserException("Constant coordinate(s) in domain of element '" + E.Name + "' (name=" + name + ").");

            if (range == null) range = domain;

            if (specialized)
                return new SOM(name, rsbbp.ListToSingleArray(domain), rsbbp.ListToSingleArray(range), S.m_dimension);
            else return new GOM(name, rsbbp.ListToSingleArray(domain), rsbbp.ListToSingleArray(range), S.m_dimension);
        } // end of ParseOMelementAndAttributes()


        public static void ParseFunction(Specification S, XmlElement E)
        {
            // storage for all info:
            String functionName = null;
            String outputFunctionName = null;
            const int MAX_NB_ARGS = 100;
            String returnTypeName = "";
            String[] argumentTypeNames = new String[MAX_NB_ARGS];
            String[] argumentVariableName = new String[MAX_NB_ARGS];
            List<String> floatNames = new List<string>();
            String metricName = "default";
            String comment = "";
            int nbArgs = 0;
            Dictionary<String, String> options = new Dictionary<string, string>();

            { // handle attributes
                XmlAttributeCollection A = E.Attributes;

                // handle all attributes
                for (int i = 0; i < A.Count; i++)
                {
                    // functionName
                    if (A[i].Name == XML_NAME)
                        functionName = A[i].Value;

                    // functionName
                    else if (A[i].Name == XML_OUTPUT_NAME)
                        outputFunctionName = A[i].Value;

                    // metricName
                    else if (A[i].Name == XML_METRIC)
                        metricName = A[i].Value.ToLower();

                    // comment
                    else if (A[i].Name == XML_COMMENT)
                        comment = A[i].Value;

                    // floatType
                    else if (A[i].Name == XML_FLOAT_TYPE)
                        floatNames.Add(A[i].Value);

                     // return type
                    else if (A[i].Name == XML_RETURN_TYPE)
                    {
                        returnTypeName = A[i].Value;
                        if (!S.IsTypeName(returnTypeName))
                        {
                            if (returnTypeName.ToLower() == XML_SCALAR) // "scalar" is also allowed as returntype
                            {
                                returnTypeName = XML_SCALAR;
                            }
                            else throw new G25.UserException("Error parsing function '" + functionName + "': '" + returnTypeName + "' is not a type (inside element '" + XML_FUNCTION + "').");
                        }
                    }

                    // argNameX
                    else if (A[i].Name.StartsWith(XML_ARGNAME))
                    {
                        int argNameIdx = 0;
                        try
                        {
                            argNameIdx = System.Int32.Parse(A[i].Name.Substring(XML_ARGNAME.Length)) - 1;
                        }
                        catch (System.Exception)
                        {
                            throw new G25.UserException("Error parsing function '" + functionName + "': invalid attribute '" + A[i].Name + "' in element '" + XML_FUNCTION + "'.");
                        }
                        if ((argNameIdx >= argumentVariableName.Length) || (argNameIdx < 0))
                            throw new G25.UserException("Error parsing function '" + functionName + "': invalid attribute index '" + A[i].Name + "' in element '" + XML_FUNCTION + "'.");

                        argumentVariableName[argNameIdx] = A[i].Value;
                    }

                    // argX
                    else if (A[i].Name.StartsWith(XML_ARG))
                    {
                        int argIdx = 0;
                        try
                        {
                            argIdx = System.Int32.Parse(A[i].Name.Substring(XML_ARG.Length)) - 1;
                        }
                        catch (System.Exception)
                        {
                            throw new G25.UserException("Error parsing function '" + functionName + "': invalid attribute '" + A[i].Name + "' in element '" + XML_FUNCTION + "'.");
                        }
                        if ((argIdx >= argumentTypeNames.Length) || (argIdx < 0))
                            throw new G25.UserException("Error parsing function '" + functionName + "': invalid attribute index '" + A[i].Name + "' in element '" + XML_FUNCTION + "'.");

                        string typeName = A[i].Value;
                        if (!S.IsTypeName(typeName))
                        {
                            // it may be a constant, like 'e1', try adding a "_t"
                            if (S.IsTypeName(typeName + Specification.CONSTANT_TYPE_SUFFIX))
                            {
                                typeName = typeName + Specification.CONSTANT_TYPE_SUFFIX;
                            }
                            else throw new G25.UserException("Error parsing function '" + functionName + "': '" + typeName + "' is not a type (inside element '" + XML_FUNCTION + "')");
                        }

                        argumentTypeNames[argIdx] = typeName;
                        if (argIdx >= nbArgs)
                            nbArgs = argIdx + 1;
                    }

                    else if (A[i].Name.StartsWith(XML_OPTION))
                    {
                        String optionName = A[i].Name.Substring(XML_OPTION.Length).ToLower();
                        if (optionName.Length > 0)
                        {
                            String optionValue = A[i].Value;
                            options[optionName] = optionValue;
                        }
                    }
                }

                // check if function name was specified:
                if (functionName == null)
                    throw new G25.UserException("Missing attribute '" + XML_NAME + "' in element '" + XML_FUNCTION + "'");

                // if output function name is missing, just use the regular function name
                if (outputFunctionName == null) outputFunctionName = functionName;

                // if no float type are specified, copy all from specification
                if (floatNames.Count == 0)
                {
                    foreach (FloatType FT in S.m_floatTypes)
                        floatNames.Add(FT.type);
                }

                // resize arguments arrays:
                Array.Resize(ref argumentTypeNames, nbArgs);
                Array.Resize(ref argumentVariableName, nbArgs);

                // check for nulls in argument arrays
                for (int i = 0; i < argumentTypeNames.Length; i++)
                {
                    if (argumentTypeNames[i] == null)
                        throw new G25.UserException("XML parsing error in function '" + functionName + "': Missing attribute '" + XML_ARG + (1 + i).ToString() + "' in element '" + XML_FUNCTION + "'");
                    if (argumentVariableName[i] == null)
                        argumentVariableName[i] = fgs.DefaultArgumentName(i);
                }
            }

            fgs F = new fgs(functionName, outputFunctionName, returnTypeName, argumentTypeNames, argumentVariableName, floatNames.ToArray(), metricName, comment, options);

            S.m_functions.Add(F);
        } // ParseFunction()

        public static void ParseVerbatim(Specification S, XmlElement E)
        {
            List<string> filenames = new List<string>();
            VerbatimCode.POSITION where = VerbatimCode.POSITION.INVALID;
            string customMarker = null;
            string verbatimCode = null;
            string verbatimCodeFile = null;

            { // handle attributes
                XmlAttributeCollection A = E.Attributes;

                // handle all attributes
                for (int i = 0; i < A.Count; i++)
                {
                    // filename
                    if (A[i].Name.StartsWith(XML.XML_FILENAME))
                    {
                        filenames.Add(A[i].Value);
                    }

                    // position
                    else if (A[i].Name == XML.XML_POSITION)
                    {
                        if (A[i].Value == XML.XML_TOP) where = VerbatimCode.POSITION.TOP;
                        else if (A[i].Value == XML.XML_BOTTOM) where = VerbatimCode.POSITION.BOTTOM;
                        else if (A[i].Value == XML.XML_BEFORE_MARKER) where = VerbatimCode.POSITION.BEFORE_MARKER;
                        else if (A[i].Value == XML.XML_AFTER_MARKER) where = VerbatimCode.POSITION.AFTER_MARKER;
                        else throw new G25.UserException("Invalid " + XML.XML_POSITION + " '" + A[i].Value + "'  in element '" + XML.XML_VERBATIM + "'.");
                    }

                    // marker
                    else if (A[i].Name == XML.XML_MARKER)
                    {
                        customMarker = A[i].Value;
                    }

                    // codeFilename
                    else if (A[i].Name == XML.XML_CODE_FILENAME)
                    {
                        if (A[i].Value.Length > 0)
                            verbatimCodeFile = A[i].Value;
                    }

                    else throw new G25.UserException("Invalid attribute '" + A[i].Name + "'  in element '" + XML.XML_VERBATIM + "'.");
                }

                { // get verbatim code from _inside_ the element:
                    XmlText T = E.FirstChild as XmlText;
                    if ((T != null) && (T.Length > 0)) verbatimCode = T.Value;
                }

                // check if function name was specified:
                if (filenames.Count == 0)
                    throw new G25.UserException("Missing attribute '" + XML.XML_FILENAME + "' in element '" + XML.XML_VERBATIM + "'");

                if (where == VerbatimCode.POSITION.INVALID)
                    throw new G25.UserException("Missing attribute '" + XML.XML_POSITION + "' in element '" + XML.XML_VERBATIM + "'");

                if (((where == VerbatimCode.POSITION.BEFORE_MARKER) ||
                    (where == VerbatimCode.POSITION.AFTER_MARKER)) &&
                    (customMarker == null))
                {
                    throw new G25.UserException("Missing attribute '" + XML.XML_MARKER + "' in element '" + XML.XML_VERBATIM + "'");
                }

                if ((verbatimCode == null) && (verbatimCodeFile == null))
                    throw new G25.UserException("Missing/empty verbatim code or verbatim code filename in element '" + XML.XML_VERBATIM + "'");
            } // end of 'handle attributes'

            S.m_verbatimCode.Add(new VerbatimCode(filenames, where, customMarker, verbatimCode, verbatimCodeFile));
        } // end of ParseVerbatim()




    } // end of class XML

} // end of namespace G25
