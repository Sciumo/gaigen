using System;
using System.Collections.Generic;
using System.Text;
using CoGsharp;


namespace g25_test_generator
{
    class Program
    {
        // how to detect OS:
        //System.PlatformID id = System.Environment.OSVersion.Platform;

        /// <summary>
        /// Where the output goes (this name is overwritten with the full directory name by InitOutputDirectory);
        /// </summary>
        public static string OutputDirectory = "TestG25";

        /// <summary>
        /// The number of test algebras generated is reduced by a factor of (approximately) 'ReduceNbTestsBy'.
        /// </summary>
        public static int ReduceNbTestsBy = 1000;

        /// <summary>
        /// Whether to shuffle the order in which the algebras are compiled, tested, etc.
        /// </summary>
        public static bool Shuffle = false;

        /// <summary>
        /// When true, four sample algebras are generated with fixed settings.
        /// These sample algebras are included with the G25 user manual.
        /// </summary>
        public static bool SampleAlgebras = false;

        /// <summary>
        /// What algebras to generate for (command-line option, otherwise the default is used.
        /// </summary>
        public static List<string> Algebras = new List<string>();
		
		/// <summary>
        ///  Command line argument: display version.
        /// </summary>
		public static bool OptionDisplayHelp = false;


        public static int GeneratorCount = 0;

        public const string BUILD_CMD = "build";
        public const string CLEAN_CMD = "clean";
        public const string TEST_CMD = "test";
        public const string XML_TEST_CMD = "xml_test";

        public static List<string> Languages = new List<string> { G25.XML.XML_C, G25.XML.XML_CPP, G25.XML.XML_CSHARP };

        static void Main(string[] args)
        {
            // parse command line
            List<string> extraArgs = ParseCommandLineOptions(args);
			
			if (extraArgs.Count > 0) {
				Console.WriteLine("Stray arguments detected: " + extraArgs);
			}

			// display help if required
            if (OptionDisplayHelp)
            {
                DisplayHelp();
            }
			
            // set the defaults if the user did not ask for any algebra
            if (Algebras.Count == 0)
                    Algebras = new List<string>{ "e2ga", "e3ga", "p3ga", "c3ga", "enga"};

            CoG cog = InitCoG();

            try
            {
                InitOutputDirectory(OutputDirectory);
            } catch (Exception ex) {
                Console.WriteLine("Error initializing the output directory (" + OutputDirectory + "):");
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }

            // init command list
            Dictionary<string, List<string>> commands = new Dictionary<string, List<string>>();
            commands[BUILD_CMD] = new List<string>();
            commands[CLEAN_CMD] = new List<string>();
            commands[TEST_CMD] = new List<string>();
            commands[XML_TEST_CMD] = new List<string>();

            // generate the specifications, collect commands
            try
            {
                if (SampleAlgebras) GenerateSampleAlgebras(cog, commands);
                else GenerateVariations(cog, commands);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error generating algebra specifications:");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Source: " + ex.Source);
                Environment.Exit(-1);
            }

            // write scripts from commands
            WriteTopLevelScripts(cog, commands);

            Console.WriteLine("");
            Console.WriteLine("Generated " + GeneratorCount + " test algebras.");
        }

        /// <summary>
        /// Parses the command line options. Returns extra arguments that were not 
        /// part of the options. In practice, one extra argument should be present:
        /// the name of the XML file.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static List<string> ParseCommandLineOptions(string[] args)
        {
            NDesk.Options.OptionSet p = new NDesk.Options.OptionSet() {
				{ "h|?|help", (string str) => {OptionDisplayHelp = true;} },
                { "a|algebra=", (string s) => { Algebras.Add(s.ToLower()); } },
                { "s|shuffle", (string s) => {Shuffle = true;} },
                { "sa|sample_algebras", (string s) => {SampleAlgebras = true;} },
                { "r|reduce=", (int r) => {ReduceNbTestsBy = r;} },
            };

            List<string> extra = p.Parse(args);

            return extra;

        }
		
		private static void DisplayHelp()
        {
            Console.Write(
                "Gaigen 2.5 Test Generator\n" + 
			    "Copyright 2010 Daniel Fontijne, University of Amsterdam.\n" +
                "\n" +
			    "Used to generate sample algebras specifications and for regression testing.\n" +
                "\n" +
                "A directory named TestG25 is created in the current directory, where a set of sample/test algebras written.\n" +
                "Build scripts, test scripts and makefiles are also generated\n" + 
                "\n" +
                "Usage: g25_test_generator\n" +
                "\n" +
                "Options:\n" +
                "-h -? -help: display help.\n" +
			    "-sa -sample_algebras: generate the sample algebras instead of the test algebras.\n" +       
                "-a -algebra: what algebras to generate (e2ga, e3ga, p3ga, c3ga, enga).\n" +
                "-s -shuffle: shuffle the order of the algebras inside the scripts.\n" +
                "-r -reduce: reduce the number of test algebras by a factor of N (default is 1000).\n" +
                "\n");
        }
		
		

        public static void ShuffleList<T>(IList<T> list)
        {
            Random rng = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < list.Count; i++)
            {
                int k = rng.Next(list.Count);
                T value = list[k];
                list[k] = list[i];
                list[i] = value;
            }
        }

        public static List<string> InterleaveCommandList(IList<string> list1, IList<string> list2)
        {
            List<string> resultList = new List<string>();
            for (int i = 0; i < list1.Count; i++)
            {
                string str = list1[i];
                if (i < list2.Count) str = str + list2[i];
                resultList.Add(str);
            }
            return resultList;
        }

        public static void InsertProgressIndicator(List<string> commands)
        {
            int cnt = commands.Count;
            for (int i = cnt; i > 0; i--)
            {
                commands.Insert(i, "echo \"Progress: " + i + " / " + cnt + " done\"\n");
            }
        }


        public static void AppendCommands(StringBuilder SB, List<string> commands)
        {
            if (Shuffle)
            {
                ShuffleList(commands);
            }

            InsertProgressIndicator(commands);

            foreach (string cmd in commands)
            {
                SB.Append(cmd);
            }
        }

        public static void WriteBuildScript(StringBuilder SB, List<string> commands) {
			if (IsUnix()) {
           		SB.AppendLine("#!/bin/sh");
				SB.AppendLine("");
				AppendCommands(SB, commands);
				SB.AppendLine("exit 0");
			}
			else {
				if (IsWindows())
           			SB.AppendLine("@echo off");
	            AppendCommands(SB, commands);
	            SB.AppendLine("exit /B 0");
	            SB.AppendLine(":error");
	            SB.AppendLine("exit /B -1");
			}
        }

        public static void WriteXmlTestScript(StringBuilder SB, List<string> commands)
        {
            SB.AppendLine("@echo off");
            AppendCommands(SB, commands);
            SB.AppendLine("exit /B 0");
            SB.AppendLine(":error");
            SB.AppendLine("exit /B -1");
        }

        public static void WriteTopLevelScripts(CoG cog, Dictionary<string, List<string>> commands)
        {
            { // build and test
                List<string> interleavedCommands = InterleaveCommandList(commands[BUILD_CMD], commands[TEST_CMD]);

                StringBuilder SB = new StringBuilder();
                WriteBuildScript(SB, interleavedCommands);

                string compileScriptFlename = System.IO.Path.Combine(OutputDirectory, "build_and_test." + GetScriptExtension());
                G25.CG.Shared.Util.WriteFile(compileScriptFlename, SB.ToString());
            }

            { // build
                StringBuilder SB = new StringBuilder();
                WriteBuildScript(SB, commands[BUILD_CMD]);

                string compileScriptFlename = System.IO.Path.Combine(OutputDirectory, "build." + GetScriptExtension());
                G25.CG.Shared.Util.WriteFile(compileScriptFlename, SB.ToString());
            }

            { // clean
                StringBuilder SB = new StringBuilder();
				if (IsUnix()) SB.Append("#!/bin/sh\n\n");
                AppendCommands(SB, commands[CLEAN_CMD]);
                string runScriptFlename = System.IO.Path.Combine(OutputDirectory, "clean." + GetScriptExtension());
                G25.CG.Shared.Util.WriteFile(runScriptFlename, SB.ToString());
            }

            { // test
                StringBuilder SB = new StringBuilder();
				if (IsUnix()) SB.Append("#!/bin/sh\n\n");
                AppendCommands(SB, commands[TEST_CMD]);
                string runScriptFlename = System.IO.Path.Combine(OutputDirectory, "test." + GetScriptExtension());
                G25.CG.Shared.Util.WriteFile(runScriptFlename, SB.ToString());
            }

            { // XML test
                StringBuilder SB = new StringBuilder();
				if (IsUnix()) SB.Append("#!/bin/sh\n\n");
                WriteXmlTestScript(SB, commands[XML_TEST_CMD]);
                string runScriptFlename = System.IO.Path.Combine(OutputDirectory, "xml_test." + GetScriptExtension());
                G25.CG.Shared.Util.WriteFile(runScriptFlename, SB.ToString());
            }
        }

        public static CoG InitCoG()
        {
            CoG cog = new CoG();

            cog.AddReference((new SpecVars()).GetType().Assembly.Location); // add reference to this assembly
            cog.AddReference((new G25.CG.Shared.Util()).GetType().Assembly.Location); // add reference for g25_cg_shared
            cog.AddReference((new G25.Tuple<string, string>()).GetType().Assembly.Location); // add reference for libg25

            cog.LoadTemplates(Properties.Resources.test_suite_templates, "test_suite_templates.txt");
            return cog;
        }


        public static void InitOutputDirectory(string dirName)
        {
            // set BaseDirName to full path
            OutputDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), OutputDirectory);
            Console.WriteLine("Output is written to " + OutputDirectory);

            if (System.IO.Directory.Exists(OutputDirectory))
            {
                try
                {
                    System.IO.Directory.Delete(OutputDirectory, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Warning: could not cleanup "  + OutputDirectory + ":");
                    Console.WriteLine(ex.Message);
                }
            }
            System.IO.Directory.CreateDirectory(OutputDirectory);
        }


        public static void GenerateSampleAlgebras(CoG cog, Dictionary<string, List<string>> commands)
        {
            Console.WriteLine("Generating the sample algebras");

			ReduceNbTestsBy = 1;
			Shuffle = false;
			
            SpecVars SV = new SpecVars();
            SV.HaveGom = true;
            SV.Inline = true;
            SV.ShortNameLangOnly = true;
            List<SpecVars> vars = SV.VaryOutputLanguage(Languages);

            GenerateE2gaVariations(cog, commands, vars);

            GenerateE3gaVariations(cog, commands, vars);

            GenerateP3gaVariations(cog, commands, vars);

            GenerateC3gaVariations(cog, commands, vars);
        }

        public static void GenerateVariations(CoG cog, Dictionary<string, List<string>> commands)
        {
            if (Algebras.Contains("e2ga")) GenerateE2gaVariations(cog, commands);
            if (Algebras.Contains("e3ga")) GenerateE3gaVariations(cog, commands);
            if (Algebras.Contains("p3ga")) GenerateP3gaVariations(cog, commands);
            if (Algebras.Contains("c3ga")) GenerateC3gaVariations(cog, commands);
            if (Algebras.Contains("enga")) GenerateENgaVariations(cog, commands);
            // todo? if (Algebras.Contains("cnga")) GenerateNdConformalVariations(cog, commands);
        }
                 
        /// <summary>
        /// Generates variations of Gaigen 2.5 specifications and Makefiles for the E2GA template.
        /// <param name="cog">Used for code generation.</param>
        /// <param name="commands">Used to return the commands to build and run each algebra/test.</param>
        /// </summary>
        public static void GenerateE2gaVariations(CoG cog, Dictionary<string, List<string>> commands)
        {
            int minDim = 2, maxDim = 2;
            bool groupAlternative = false;
            string altGmvName = "multivector";
            string altScalarName = "scalarClass";
            bool varyGmvCode = true;
            bool varyInline = true;
            bool varyCoordStorage = true;
            List<SpecVars> vars = GetVariations(minDim, maxDim, groupAlternative, altGmvName, altScalarName, varyGmvCode, varyInline, varyCoordStorage);

            vars = Reduce(vars);

            GenerateE2gaVariations(cog, commands, vars);
        }

        public static void GenerateE2gaVariations(CoG cog, Dictionary<string, List<string>> commands, List<SpecVars> vars)
        {
            foreach (SpecVars V in vars)
            {
                V.Dimension = 2;
                GenerateFromVar(cog, commands, "e2ga_spec", GetMakefileTemplateName(V.OutputLanguage), "e2ga", V);
            }
        }

        /// <summary>
        /// Generates variations of Gaigen 2.5 specifications and Makefiles for the E3GA template.
        /// <param name="cog">Used for code generation.</param>
        /// <param name="commands">Used to return the commands to build and run each algebra/test.</param>
        /// </summary>
        public static void GenerateE3gaVariations(CoG cog, Dictionary<string, List<string>> commands)
        {
            int minDim = 3, maxDim = 3;
            bool groupAlternative = false;
            string altGmvName = "multivector";
            string altScalarName = "scalarClass";
            bool varyGmvCode = true;
            bool varyInline = true;
            bool varyCoordStorage = true;
            List<SpecVars> vars = GetVariations(minDim, maxDim, groupAlternative, altGmvName, altScalarName, varyGmvCode, varyInline, varyCoordStorage);

            vars = Reduce(vars);

            GenerateE3gaVariations(cog, commands, vars);
        }

        public static void GenerateE3gaVariations(CoG cog, Dictionary<string, List<string>> commands, List<SpecVars> vars)
        {
            foreach (SpecVars V in vars)
            {
                V.Dimension = 3;
                GenerateFromVar(cog, commands, "e3ga_spec", GetMakefileTemplateName(V.OutputLanguage), "e3ga", V);
            }
        }

        /// <summary>
        /// Generates variations of Gaigen 2.5 specifications and Makefiles for the P3GA template.
        /// <param name="cog">Used for code generation.</param>
        /// <param name="commands">Used to return the commands to build and run each algebra/test.</param>
        /// </summary>
        public static void GenerateP3gaVariations(CoG cog, Dictionary<string, List<string>> commands)
        {
            int minDim = 4, maxDim = 4;
            bool groupAlternative = true;
            string altGmvName = "multivector";
            string altScalarName = "scalarClass";
            bool varyGmvCode = true;
            bool varyInline = true;
            bool varyCoordStorage = true;
            List<SpecVars> vars = GetVariations(minDim, maxDim, groupAlternative, altGmvName, altScalarName, varyGmvCode, varyInline, varyCoordStorage);

            vars = Reduce(vars);

            GenerateP3gaVariations(cog, commands, vars);
        }

        public static void GenerateP3gaVariations(CoG cog, Dictionary<string, List<string>> commands, List<SpecVars> vars)
        {
            foreach (SpecVars V in vars)
            {
                V.Dimension = 4;
                GenerateFromVar(cog, commands, "p3ga_spec", GetMakefileTemplateName(V.OutputLanguage), "p3ga", V);
            }
        }

        /// <summary>
        /// Generates variations of Gaigen 2.5 specifications and Makefiles for the E3GA template.
        /// <param name="cog">Used for code generation.</param>
        /// <param name="commands">Used to return the commands to build and run each algebra/test.</param>
        /// </summary>
        public static void GenerateC3gaVariations(CoG cog, Dictionary<string, List<string>> commands)
        {
            int minDim = 5, maxDim = 5;
            bool groupAlternative = true;
            string altGmvName = "multivector";
            string altScalarName = "scalarClass";
            bool varyGmvCode = false; // cannot varywhen metric is not diagonal
            bool varyInline = false; // attempt to be able to compile _huge_ code
            bool varyCoordStorage = true;
            List<SpecVars> vars = GetVariations(minDim, maxDim, groupAlternative, altGmvName, altScalarName, varyGmvCode, varyInline, varyCoordStorage);

            vars = Reduce(vars);

            foreach (SpecVars V in vars)
            {
                V.Dimension = 5;
                GenerateFromVar(cog, commands, "c3ga_spec", GetMakefileTemplateName(V.OutputLanguage), "c3ga", V);
            }
        }

        public static void GenerateC3gaVariations(CoG cog, Dictionary<string, List<string>> commands, List<SpecVars> vars)
        {
            foreach (SpecVars V in vars)
            {
                V.Dimension = 5;
                GenerateFromVar(cog, commands, "c3ga_spec", GetMakefileTemplateName(V.OutputLanguage), "c3ga", V);
            }
        }

        /// <summary>
        /// Generates variations of Gaigen 2.5 specifications and Makefiles for the ENGA template.
        /// <param name="cog">Used for code generation.</param>
        /// <param name="commands">Used to return the commands to build and run each algebra/test.</param>
        /// </summary>
        public static void GenerateENgaVariations(CoG cog, Dictionary<string, List<string>> commands)
        {
            int minDim = 1, maxDim = 7;
            bool groupAlternative = false;
            string altGmvName = "multivector";
            string altScalarName = "scalarClass";
            bool varyGmvCode = false; 
            bool varyInline = true; // attempt to be able to compile _huge_ code
            bool varyCoordStorage = true;
            List<SpecVars> lowDvars = GetVariations(minDim, maxDim, groupAlternative, altGmvName, altScalarName, varyGmvCode, varyInline, varyCoordStorage);

            minDim = 8;
            maxDim = 11;
            varyInline = false;
            varyCoordStorage = false;
            List<SpecVars> highDvars = GetVariations(minDim, maxDim, groupAlternative, altGmvName, altScalarName, varyGmvCode, varyInline, varyCoordStorage);

            // force runtime code, array storage for all high-D
            for (int i = 0; i < highDvars.Count; i++)
            {
                highDvars[i].GmvCode = G25.GMV_CODE.RUNTIME;
                highDvars[i].CoordStorage = G25.COORD_STORAGE.VARIABLES;
            }

            List<SpecVars> vars = new List<SpecVars>(lowDvars);
            vars.AddRange(highDvars);
            vars = Reduce(vars);


            foreach (SpecVars V in vars)
            {
                GenerateFromVar(cog, commands, "eNga_spec", GetMakefileTemplateName(V.OutputLanguage), "e" + V.Dimension + "ga", V);
            }
        }

        public static List<SpecVars> Reduce(List<SpecVars> vars)
        {
            if (ReduceNbTestsBy <= 1) return vars;

			int seed = (int)DateTime.Now.Ticks;
            System.Random R = new Random(seed);

            // randomly select about ReduceNbTestsBy less variation from vars
            List<SpecVars> selectedVars = new List<SpecVars>();
            int prevIdx = -1;
            for (double idx = 2.0 * R.NextDouble() * ReduceNbTestsBy; idx < (double)vars.Count + 0.99999; idx += 2.0 * R.NextDouble() * ReduceNbTestsBy) {
                int i = (int)idx;
                if (i != prevIdx) selectedVars.Add(vars[i]);
            }

            // make sure at least 1 is selected:
            if ((vars.Count > 0) && (selectedVars.Count == 0))
                selectedVars.Add(vars[R.Next(vars.Count-1)]);

            return selectedVars;
        }


        public static string GenerateFromVar(CoG cog, 
            Dictionary<string, List<string>> commands, 
            string specTemplateName, 
            string makefileTemplateName, 
            string specBaseName, 
            SpecVars SV)
        {
            GeneratorCount++;

            // get name of specification
            string specName = specBaseName + "_" + SV.GetShortName();

            // get full directory name, create directory
            string dirName = System.IO.Path.Combine(OutputDirectory, specName);
            try
            {
                System.IO.Directory.CreateDirectory(dirName);
            }
            catch (Exception)
            {
                Console.WriteLine("Warning: this directory is not clean: " + dirName);
            }

            // get filename of spec, write it
            string specFilename = System.IO.Path.Combine(dirName, specName + ".xml");
            GenerateSpecFromVar(cog, specTemplateName, specName, specFilename, SV);

            // get filename of makefile, write it
            string makefileFilename = System.IO.Path.Combine(dirName, "Makefile");
            GenerateMakefileFromVar(cog, makefileTemplateName, specName, makefileFilename, specBaseName, SV);

            // if myDouble is used, generate it
            if (SV.FloatTypes.Contains("myDouble"))
            {
                string myDoubleFilename = System.IO.Path.Combine(dirName, "my_double.h");
                GenerateMyDoubleFromVar(cog, myDoubleFilename, SV);
            }


            commands[BUILD_CMD].Add(GenerateMakeCommands(cog, specName, SV, ""));
            commands[TEST_CMD].Add(GenerateRunCommands(cog, specName, SV));
            commands[CLEAN_CMD].Add(GenerateMakeCommands(cog, specName, SV, " clean"));
            commands[XML_TEST_CMD].Add(GenerateXmlTestCommands(cog, specName, SV));

            return specName;
        }


        public static string GenerateMakeCommands(CoG cog, string specName, SpecVars SV, string target)
        {
            StringBuilder SB = new StringBuilder();

            SB.AppendLine("");
            SB.AppendLine("cd " + specName);
			
			if (IsUnix()) {
				SB.AppendLine("make" + target);
				SB.AppendLine("if [ $? -eq 0 ]; ");
				SB.AppendLine("then");
				SB.AppendLine("echo \"build ok\"");			
				SB.AppendLine("else");			
				SB.AppendLine("\texit $?");			
				SB.AppendLine("fi");			
			}
			else {
				SB.AppendLine("nmake" + target);
				SB.AppendLine("if not %errorlevel%==0 goto :error");
			}
			
			SB.AppendLine("cd ..");
			
            return SB.ToString();
        }

        public static string GenerateRunCommands(CoG cog, string specName, SpecVars SV)
        {
            StringBuilder SB = new StringBuilder();

            SB.AppendLine("");
            SB.AppendLine("cd " + specName);
            SB.AppendLine("echo \"Testing " + specName + " \"");			
			if (IsUnix()) SB.AppendLine("./test");
			else SB.AppendLine("test.exe");
            SB.AppendLine("cd ..");

            return SB.ToString();
        }

        public static string GenerateXmlTestCommands(CoG cog, string specName, SpecVars SV)
        {
            StringBuilder SB = new StringBuilder();
            string xmlTestdirName = XML_TEST_CMD;
            string fileListName = "g25_file_list.txt";

            SB.AppendLine("");
            SB.AppendLine("cd " + specName);
            SB.AppendLine("g25 -f " + fileListName + " " + specName + ".xml");
            SB.AppendLine("mkdir " + xmlTestdirName);
            SB.AppendLine("g25 -s " + xmlTestdirName + "\\" + specName + ".xml " + specName + ".xml");
            SB.AppendLine("cd " + xmlTestdirName);
            SB.AppendLine("g25 -f " + fileListName + " " + specName + ".xml");

            SB.AppendLine("g25_diff " + fileListName + " ..\\" + fileListName);
            SB.AppendLine("if not %errorlevel%==0 goto :error");

            SB.AppendLine("cd .."); // leave xmlTestdirName

            SB.AppendLine("cd .."); // leave algebra dir

            return SB.ToString();
        }

        public static void GenerateMyDoubleFromVar(CoG cog, string filename, SpecVars SV)
        {
            // get name of template
            string templateName = "not_set";
            if (SV.OutputLanguage == "cpp")
                templateName = "myDouble_cpp_header";

            // write template to StringBuilder
            StringBuilder SB = new StringBuilder();
            cog.EmitTemplate(SB, templateName);

            // write template to file
            G25.CG.Shared.Util.WriteFile(filename, SB.ToString());
        }


        public static void GenerateSpecFromVar(CoG cog, string templateName, string specName, string specFilename, SpecVars SV)
        {
            // write template to StringBuilder
            StringBuilder SB = new StringBuilder();
            cog.EmitTemplate(SB, templateName, "SV=", SV);

            // write template to file
            G25.CG.Shared.Util.WriteFile(specFilename, SB.ToString());
        }

        public static void GenerateMakefileFromVar(CoG cog, string templateName, string specName, string makefileFilename, string algebraName, SpecVars SV)
        {
            // write template to StringBuilder
            StringBuilder SB = new StringBuilder();
            cog.EmitTemplate(SB, templateName, 
			                 "SV=", SV, 
			                 "SPEC_NAME=", specName, 
			                 "ALGEBRA_NAME=", algebraName,
			                 "WINDOWS=", IsWindows(),
			                 "MACOSX=", GetPlatformID() == PlatformID.MacOSX,
			                 "LINUX=", GetPlatformID() == PlatformID.Unix
			                 );

            // write template to file
            G25.CG.Shared.Util.WriteFile(makefileFilename, SB.ToString());
        }


        public static List<SpecVars> GetVariations(int minDim, int maxDim, bool groupAlternative, string alternativeGmvName, string alternativeScalarName, bool varyGmvCode, bool varyInline, bool varyCoordStorage)
        {
            List<SpecVars> returnList = new List<SpecVars>();

            foreach (string lang in Languages)
            {
                List<SpecVars> list = new List<SpecVars>();
                list.Add(new SpecVars());

                // vary output language
                list[0].OutputLanguage = lang;

                // vary dimension
                List<int> dims = new List<int>();
                for (int d = minDim; d <= maxDim; d++) dims.Add(d);
                list = SpecVars.VaryDimension(list, dims);

                // vary scalar type
                list = SpecVars.VaryHaveScalarType(list, new List<bool> { false, true });

                // vary GMV coordinate storage
                if (varyCoordStorage)
                    list = SpecVars.VaryCoordStorage(list, new List<G25.COORD_STORAGE> { G25.COORD_STORAGE.ARRAY, G25.COORD_STORAGE.VARIABLES });

                // vary grouping of GMV
                if (groupAlternative)
                    list = SpecVars.VaryGroupAlternative(list, new List<bool> { false, true });

                // vary GOM
                list = SpecVars.VaryHaveGom(list, new List<bool> { false, true });

                // vary parser
                list = SpecVars.VaryBuiltInParser(list, new List<bool> { false, true });

                // vary GMV name
                if ((alternativeGmvName != null) && (alternativeGmvName.Length > 0))
                    list = SpecVars.VaryGmvNames(list, new List<string> { "mv", alternativeGmvName });

                // vary scalar type name
                if ((alternativeScalarName != null) && (alternativeScalarName.Length > 0))
                    list = SpecVars.VaryScalarNames(list, new List<string> { "scalar", alternativeScalarName });

                // vary GMV function coding 
                if (varyGmvCode) 
                    list = SpecVars.VaryGmvCode(list, new List<G25.GMV_CODE> { G25.GMV_CODE.EXPAND, G25.GMV_CODE.RUNTIME });

                // vary GMV function coding 
                if ((lang == G25.XML.XML_CPP) || (lang == G25.XML.XML_C))
                {
                    list = SpecVars.VaryRandomGenerator(list, new List<string> { "libc", "mt" });
                }
                else list = SpecVars.VaryRandomGenerator(list, new List<string> { "libc" });

                // vary inline, report usage
                if ((lang != G25.XML.XML_C) && varyInline)
                {
                    list = SpecVars.VaryInline(list, new List<bool> { false, true });
                    list = SpecVars.VaryReportUsage(list, new List<bool> { false, true });
                }

                // vary GMV memory allocation
                List<G25.GMV.MEM_ALLOC_METHOD> AL = new List<G25.GMV.MEM_ALLOC_METHOD> { G25.GMV.MEM_ALLOC_METHOD.FULL, G25.GMV.MEM_ALLOC_METHOD.PARITY_PURE };
                if (lang != G25.XML.XML_C) AL.Add(G25.GMV.MEM_ALLOC_METHOD.DYNAMIC);
                list = SpecVars.VaryGmvMemAlloc(list, AL);

                // vary floating point types
                List<List<string>> FT = null;
                if (lang ==G25.XML.XML_CPP) {
                    if (minDim < 5)
                        FT = new List<List<string>>{
                            new List<string>{"float"},
                            new List<string>{"double"},
                            new List<string>{"double", "float", "myDouble"}
                        };
                    else
                        FT = new List<List<string>>{
                            new List<string>{"float"},
                            new List<string>{"myDouble"},
                            new List<string>{"double", "myDouble"}
                        };
                }
                else
                {
                    FT = new List<List<string>>{
                        new List<string>{"float"},
                        new List<string>{"double"},
                        new List<string>{"double", "float"}
                    };
                }

                list = SpecVars.VaryFloatTypes(list, FT);

                returnList.AddRange(list);
            }

            return returnList;
        }

		
		public static string GetMakefileTemplateName(string language) {
            if (language.Equals(G25.XML.XML_C) || language.Equals(G25.XML.XML_CPP))
                return GetMakefileTemplateNameCppOrC();
            if (language.Equals(G25.XML.XML_CSHARP))
                return GetMakefileTemplateNameCSharp();
            else return "makefile_unknown_language";
        }

        public static string GetMakefileTemplateNameCppOrC()
        {
			switch (GetPlatformID()) {
			case PlatformID.Win32NT:
			case PlatformID.Win32Windows:
			case PlatformID.Win32S:
			case PlatformID.WinCE:
			case PlatformID.Xbox:
				return "makefile_c_cpp_vs";
			case PlatformID.Unix:
			case PlatformID.MacOSX:
				return "makefile_c_cpp_unix";
			default:
				return "makefile_unknown_platform";
			}
		}

        public static string GetMakefileTemplateNameCSharp()
        {
            switch (GetPlatformID())
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32Windows:
                case PlatformID.Win32S:
                case PlatformID.WinCE:
                case PlatformID.Xbox:
                    return "makefile_csharp_vs";
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    return "makefile_csharp_unix";
                default:
                    return "makefile_unknown_platform";
            }
        }

		public static string GetScriptExtension() {
			return (IsUnix()) ? "sh" : "bat";
		}
		
		public static bool IsUnix() {
			return (GetPlatformID() == PlatformID.MacOSX) ||
			    (GetPlatformID() == PlatformID.Unix);
		}
		
		public static bool IsWindows() {
			System.PlatformID p = GetPlatformID();
			return (p == PlatformID.Win32NT) || 
				(p == PlatformID.Win32Windows) ||
				(p == PlatformID.Win32S) ||
				(p == PlatformID.WinCE);
		}
		
		public static System.PlatformID GetPlatformID() {
			// temporary hack until mono gets it's platform ID right:
			if ((Environment.OSVersion.Platform == PlatformID.Unix) && 
			    (Environment.OSVersion.Version.Major == 10)) return PlatformID.MacOSX;
			else return Environment.OSVersion.Platform;
		}



    }
}




			    