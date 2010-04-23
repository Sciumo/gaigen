using System;
using System.Collections.Generic;
using System.Text;


namespace g25_test_generator
{
    /// <summary>
    /// This class is used to describe the variable attributes algebra templates.
    /// Things that can vary are:
    ///   - language
    ///   - dimension
    ///   - scalar type, no scalar type
    ///   - store coords in arrays, variables
    ///   - group by grade, conformal: group by grade&no&ni
    ///   - GOM defined, not defined
    ///   - builtin parser / antlr parser
    ///   - different name than 'mv' for multivector
    ///   - different name than 'scalar' for multivector
    ///   - whether to inline or not
    ///   - report usage
    ///   - memory allocation method of GMV
    ///   - number of float types, what float types
    ///   - what random generator is used
    /// 
    /// </summary>
    public class SpecVars : ICloneable
    {
        public SpecVars()
        {
            OutputLanguage = "not set";
            Dimension = -1;
            HaveScalarType = false;
            CoordStorage = G25.COORD_STORAGE.VARIABLES;
            GroupAlternative = false;
            HaveGom = false;
            BuiltInParser = true;
            GmvName = "mv";
            GmvCode = G25.GMV_CODE.EXPAND;
            ScalarName = "scalar";
            GmvMemAlloc = G25.GMV.MEM_ALLOC_METHOD.FULL;
            Inline = false;
            ReportUsage = false;
            FloatTypes = new List<string>{"double"};
            RandomGenerator = "libc";
        }

        object ICloneable.Clone()
        {
            // make memberwise copy
            return this.Clone();
        }

        public SpecVars Clone()
        {
            return (SpecVars)this.MemberwiseClone();
        }

        public string GetShortName()
        {
            string gmvMemAllocStr = "x";
            if (GmvMemAlloc == G25.GMV.MEM_ALLOC_METHOD.DYNAMIC) gmvMemAllocStr = "D";
            else if (GmvMemAlloc == G25.GMV.MEM_ALLOC_METHOD.PARITY_PURE) gmvMemAllocStr = "Y";
            else if (GmvMemAlloc == G25.GMV.MEM_ALLOC_METHOD.FULL) gmvMemAllocStr = "P";

            string floatTypesStr = "" + FloatTypes.Count;
            foreach (string str in FloatTypes)
                floatTypesStr = floatTypesStr + str[0];

            return OutputLanguage + "_" +
                Dimension +
                ((HaveScalarType) ? "S" : "x") +
                ((CoordStorage == G25.COORD_STORAGE.ARRAY) ? "A" : "V") +
                ((GroupAlternative) ? "A" : "x") +
                ((HaveGom) ? "G" : "x") +
                ((BuiltInParser) ? "B" : "A") + "_" +
                ((Inline) ? "I" : "x") + "_" +
                ((ReportUsage) ? "R" : "x") + "_" +
                gmvMemAllocStr +
                GmvName + "_" +
                floatTypesStr + "_" +
                RandomGenerator + "_" +
                ScalarName;
        }

        /// <summary>
        /// Varies the output language of 'this'.
        /// </summary>
        /// <param name="languages">List of variations on the output language.</param>
        /// <returns>a list of variations.</returns>
        public List<SpecVars> VaryOutputLanguage(List<string> languages)
        {
            List<SpecVars> list = new  List<SpecVars>();

            foreach (string lang in languages) {
                SpecVars V = this.Clone();
                V.OutputLanguage = lang;
                list.Add(V);
            }

            return list;
        }

        public static List<SpecVars> VaryOutputLanguage(List<SpecVars> inputList, List<string> languages)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryOutputLanguage(languages));
            return outputList;
        }


        /// <summary>
        /// Varies the dimension of 'this'.
        /// </summary>
        /// <param name="dimensions">List of variations on the dimension.</param>
        /// <returns>a list of variations.</returns>
        public List<SpecVars> VaryDimension(List<int> dimensions)
        {
            List<SpecVars> list = new List<SpecVars>();

            foreach (int dim in dimensions)
            {
                SpecVars V = this.Clone();
                V.Dimension = dim;
                list.Add(V);
            }

            return list;
        }

        public static List<SpecVars> VaryDimension(List<SpecVars> inputList, List<int> dimensions)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryDimension(dimensions));
            return outputList;
        }


        /// <summary>
        /// Varies the whether a scalar type is present.
        /// </summary>
        /// <param name="haveScalar">List of variations on the scalar type.</param>
        /// <returns>a list of variations.</returns>
        public List<SpecVars> VaryHaveScalarType(List<bool> haveScalar)
        {
            List<SpecVars> list = new List<SpecVars>();

            foreach (bool hs in haveScalar)
            {
                SpecVars V = this.Clone();
                V.HaveScalarType = hs;
                list.Add(V);
            }

            return list;
        }

        public static List<SpecVars> VaryHaveScalarType(List<SpecVars> inputList, List<bool> haveScalar)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryHaveScalarType(haveScalar));
            return outputList;
        }


        /// <summary>
        /// Varies the coordinate storage method of 'this'.
        /// </summary>
        /// <param name="coordStorage">List of variations on the coordinate storage method.</param>
        /// <returns>Returns a list of variations.</returns>
        public List<SpecVars> VaryCoordStorage(List<G25.COORD_STORAGE> coordStorage)
        {
            List<SpecVars> list = new List<SpecVars>();

            foreach (G25.COORD_STORAGE cs in coordStorage)
            {
                SpecVars V = this.Clone();
                V.CoordStorage = cs;
                list.Add(V);
            }

            return list;
        }

        public static List<SpecVars> VaryCoordStorage(List<SpecVars> inputList, List<G25.COORD_STORAGE> coordStorage)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryCoordStorage(coordStorage));
            return outputList;
        }

        /// <summary>
        /// Varies the grouping method (regular or 'alternative') of 'this'.
        /// </summary>
        /// <param name="alternativeGrouping">List of variations on the  grouping method.</param>
        /// <returns>Returns a list of variations.</returns>
        public List<SpecVars> VaryGroupAlternative(List<bool> alternativeGrouping)
        {
            List<SpecVars> list = new List<SpecVars>();

            foreach (bool ac in alternativeGrouping)
            {
                SpecVars V = this.Clone();
                V.GroupAlternative = ac;
                list.Add(V);
            }

            return list;
        }

        public static List<SpecVars> VaryGroupAlternative(List<SpecVars> inputList, List<bool> alternativeGrouping)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryGroupAlternative(alternativeGrouping));
            return outputList;
        }

        /// <summary>
        /// Varies the whether to a have a GOM or not.
        /// </summary>
        /// <param name="haveGOM">List of variations on the GOM yes/no.</param>
        /// <returns>Returns a list of variations.</returns>
        public List<SpecVars> VaryHaveGom(List<bool> haveGom)
        {
            List<SpecVars> list = new List<SpecVars>();

            foreach (bool hg in haveGom)
            {
                SpecVars V = this.Clone();
                V.HaveGom = hg;
                list.Add(V);
            }

            return list;
        }

        public static List<SpecVars> VaryHaveGom(List<SpecVars> inputList, List<bool> haveGom)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryHaveGom(haveGom));
            return outputList;
        }


        /// <summary>
        /// Varies the parser (builtin or ANTLR) of 'this'.
        /// </summary>
        /// <param name="useBuiltinParser">List of variations on the parser.</param>
        /// <returns>Returns a list of variations.</returns>
        public List<SpecVars> VaryBuiltInParser(List<bool> useBuiltinParser)
        {
            List<SpecVars> list = new List<SpecVars>();

            foreach (bool bip  in useBuiltinParser)
            {
                SpecVars V = this.Clone();
                V.BuiltInParser = bip;
                list.Add(V);
            }

            return list;
        }

        public static List<SpecVars> VaryBuiltInParser(List<SpecVars> inputList, List<bool> useBuiltinParser)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryBuiltInParser(useBuiltinParser));
            return outputList;
        }

        /// <summary>
        /// Varies the GMV name of 'this'.
        /// </summary>
        /// <param name="mvNames">List of variations on the name of the GMV.</param>
        /// <returns>Returns a list of variations.</returns>
        public List<SpecVars> VaryGmvNames(List<string> gmvNames)
        {
            List<SpecVars> list = new List<SpecVars>();

            foreach (string name in gmvNames)
            {
                SpecVars V = this.Clone();
                V.GmvName = name;
                list.Add(V);
            }

            return list;
        }

        public static List<SpecVars> VaryGmvNames(List<SpecVars> inputList, List<string> gmvNames)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryGmvNames(gmvNames));
            return outputList;
        }

        /// <summary>
        /// Varies the way GMV code is generated (runtime, or expand).
        /// </summary>
        /// <param name="mvNames">List of variations on the code generation for GMVs.</param>
        /// <returns>Returns a list of variations.</returns>
        public List<SpecVars> VaryGmvCode(List<G25.GMV_CODE> gmvCode)
        {
            List<SpecVars> list = new List<SpecVars>();

            foreach (G25.GMV_CODE c in gmvCode)
            {
                SpecVars V = this.Clone();
                V.GmvCode = c;
                list.Add(V);
            }

            return list;
        }

        public static List<SpecVars> VaryGmvCode(List<SpecVars> inputList, List<G25.GMV_CODE> gmvCode)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryGmvCode(gmvCode));
            return outputList;
        }

        /// <summary>
        /// Varies the scalar name of 'this'.
        /// </summary>
        /// <param name="scalarNames">List of variations on the name of the scalar type.</param>
        /// <returns>Returns a list of variations.</returns>
        public List<SpecVars> VaryScalarNames(List<string> scalarNames)
        {
            List<SpecVars> list = new List<SpecVars>();

            foreach (string name in scalarNames)
            {
                SpecVars V = this.Clone();
                V.ScalarName = name;
                list.Add(V);
            }

            return list;
        }

        public static List<SpecVars> VaryScalarNames(List<SpecVars> inputList, List<string> scalarNames)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryScalarNames(scalarNames));
            return outputList;
        }

        /// <summary>
        /// Varies the way GMV memory is allocate (pp, full, dynamic)
        /// </summary>
        /// <param name="mvNames">List of variations on the memory allocation for GMVs.</param>
        /// <returns>Returns a list of variations.</returns>
        public List<SpecVars> VaryGmvMemAlloc(List<G25.GMV.MEM_ALLOC_METHOD> gmvMemAlloc)
        {
            List<SpecVars> list = new List<SpecVars>();

            foreach (G25.GMV.MEM_ALLOC_METHOD a in gmvMemAlloc)
            {
                SpecVars V = this.Clone();
                V.GmvMemAlloc = a;
                list.Add(V);
            }

            return list;
        }

        public static List<SpecVars> VaryGmvMemAlloc(List<SpecVars> inputList, List<G25.GMV.MEM_ALLOC_METHOD> gmvMemAlloc)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryGmvMemAlloc(gmvMemAlloc));
            return outputList;
        }



        /// <summary>
        /// Varies whether to inline code.
        /// </summary>
        /// <param name="doInline">List of variations on inlining.</param>
        /// <returns>Returns a list of variations.</returns>
        public List<SpecVars> VaryInline(List<bool> doInline)
        {
            List<SpecVars> list = new List<SpecVars>();

            foreach (bool bip in doInline)
            {
                SpecVars V = this.Clone();
                V.Inline = bip;
                list.Add(V);
            }

            return list;
        }

        public static List<SpecVars> VaryInline(List<SpecVars> inputList, List<bool> doInline)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryInline(doInline));
            return outputList;
        }

        /// <summary>
        /// Varies whether to report usage of converted SMV in GMV functions.
        /// </summary>
        /// <param name="reportUsage">List of variations on inlining.</param>
        /// <returns>Returns a list of variations.</returns>
        public List<SpecVars> VaryReportUsage(List<bool> reportUsage)
        {
            List<SpecVars> list = new List<SpecVars>();

            foreach (bool bip in reportUsage)
            {
                SpecVars V = this.Clone();
                V.ReportUsage = bip;
                list.Add(V);
            }

            return list;
        }

        public static List<SpecVars> VaryReportUsage(List<SpecVars> inputList, List<bool> reportUsage)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryReportUsage(reportUsage));
            return outputList;
        }

        /// <summary>
        /// Varies the float types. 
        /// </summary>
        /// <param name="reportUsage">List of float types.</param>
        /// <returns>Returns a list of variations.</returns>
        public List<SpecVars> VaryFloatTypes(List<List<string>> floatTypes)
        {
            List<SpecVars> list = new List<SpecVars>();

            foreach (List<string> FT in floatTypes)
            {
                SpecVars V = this.Clone();
                V.FloatTypes = FT;
                list.Add(V);
            }

            return list;
        }

        public static List<SpecVars> VaryFloatTypes(List<SpecVars> inputList, List<List<string>> floatTypes)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryFloatTypes(floatTypes));
            return outputList;
        }

        public static List<SpecVars> VaryRandomGenerator(List<SpecVars> inputList, List<string> randomGenerators)
        {
            List<SpecVars> outputList = new List<SpecVars>();
            foreach (SpecVars SV in inputList)
                outputList.AddRange(SV.VaryRandomGenerator(randomGenerators));
            return outputList;
        }

        /// <summary>
        /// Varies the random generator (built-in, Mersenne Twister)
        /// </summary>
        /// <param name="mvNames">List of variations on the random generator.</param>
        /// <returns>Returns a list of variations.</returns>
        public List<SpecVars> VaryRandomGenerator(List<string> randomGenerators)
        {
            List<SpecVars> list = new List<SpecVars>();

            foreach (string g in randomGenerators)
            {
                SpecVars V = this.Clone();
                V.RandomGenerator = g;
                list.Add(V);
            }

            return list;
        }


        public string OutputLanguage { get; set; }
        public int Dimension { get; set; }
        public bool HaveScalarType { get; set; }
        public G25.COORD_STORAGE CoordStorage { get; set; }
        public bool GroupAlternative { get; set; }
        public bool HaveGom { get; set; }
        public bool BuiltInParser { get; set; }
        public string GmvName { get; set; }
        public G25.GMV_CODE GmvCode { get; set; }
        public string ScalarName { get; set; }
        public G25.GMV.MEM_ALLOC_METHOD GmvMemAlloc { get; set; }
        public bool Inline { get; set; }
        public bool ReportUsage { get; set; }
        public List<string> FloatTypes { get; set; }
        public string RandomGenerator { get; set; }



    }
}
