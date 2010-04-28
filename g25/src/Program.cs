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
    class Program
    {
        /// <summary>
        /// Gaigen Version.
        /// </summary>
        public const string Version = "2.5.1";

        /// <summary>
        ///  Command line argument: display help.
        /// </summary>
        public static bool OptionDisplayHelp = false;
        /// <summary>
        ///  Command line argument: display version.
        /// </summary>
        public static bool OptionDisplayVersion = false;
        /// <summary>
        /// When this string is non-null, the loaded spec is save to that file.
        /// </summary>
        public static string OptionSaveSpecXmlFile = null;
        /// <summary>
        /// When this string is non-null, a list of generated files is written to that file.
        /// </summary>
        public static string OptionSaveFileListFile = null;

        static void Main(string[] args)
        {
            // parse command line
            List<string> extraArgs = ParseCommandLineOptions(args);

            // display version if required
            if (OptionDisplayVersion)
                Console.WriteLine("Gaigen " + Version);

            // display help if required
            if (OptionDisplayHelp)
            {
                DisplayHelp();
            }

            // generate (or save XML), but only if there was just one extra argument (the filename).
            if (extraArgs.Count == 1)
            {
                ProcessSpecificationXML(extraArgs[0]);
            }
            else
            {
                DisplayCmdLineError(args, extraArgs);
            }

        } // end of Main()


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
                { "v|version", (string str) => {OptionDisplayVersion = true;} },
                { "s|save=", (string str) => {OptionSaveSpecXmlFile = str;} },
                { "f|filelist=", (string str) => {OptionSaveFileListFile = str;} },
            };

            List<string> extra = p.Parse(args);

            return extra;

        }

        public static void ProcessSpecificationXML(string filename) {
            if (OptionSaveSpecXmlFile != null)
                LoadAndSave(filename, OptionSaveSpecXmlFile);
            else Generate(filename);

        }

        public static void LoadAndSave(string srcFilename, string dstFilename)
        {
            try
            {
                Console.WriteLine("Loading algebra specification: " + srcFilename);
                G25.Specification S = new G25.Specification(srcFilename);

                Console.WriteLine("Saving algebra specification: " + dstFilename);
                string str = S.ToXmlString();
                G25.CG.Shared.Util.WriteFile(dstFilename, str);

                // write list of generated files
                if (OptionSaveFileListFile != null) {
                    List<string> L = new List<string>();
                    L.Add(dstFilename);
                    SaveGenerateFileList(OptionSaveFileListFile, L);
                }

            }
            catch (G25.UserException E)
            {
                System.Console.WriteLine("Error:");
                System.Console.Write(E.GetErrorReport());
            }
            catch (System.Exception E)
            {
                System.Console.WriteLine("Exception:");
                System.Console.WriteLine(E.ToString());
                System.Console.WriteLine(E.Message);
            }

        }

        public static void Generate(string filename)
        {
            try
            {
                G25.Specification S = new G25.Specification(filename);

                // get the code generator and plugins ready
                CodeGeneratorLoader L = new CodeGeneratorLoader(S.GetOutputLanguageString());
                LoadDefaultCodeGenerators(L);
                // todo: load custom plugins (their location will be in the specification XML?)
                if (L.GetMainCodeGenerator() == null)
                    throw new G25.UserException("No code generator for language " + S.GetOutputLanguageString());

                // generate the code
                List<string> generatedFiles = L.GetMainCodeGenerator().GenerateCode(S, L.GetCodeGeneratorPlugins());

                // insert verbatim code:
                S.InsertVerbatimCode(generatedFiles);

                // write list of generated files
                if (OptionSaveFileListFile != null)
                    SaveGenerateFileList(OptionSaveFileListFile, generatedFiles);
            }
            catch (G25.UserException E)
            {
                System.Console.WriteLine("Error:");
                System.Console.Write(E.GetErrorReport());
            }
            catch (System.Exception E)
            {
                System.Console.WriteLine("Exception:");
                System.Console.WriteLine(E.ToString());
                System.Console.WriteLine(E.Message);
            }
        }

        private static void DisplayCmdLineError(string[] args, List<string> extraArgs) {
            if (extraArgs.Count == 0)
            {
                if (!OptionDisplayHelp)
                    Console.WriteLine("Usage: g25 algebra_specification.xml\n");
            }
            else
            {
                Console.WriteLine("Too many command line arguments");
                foreach (string ea in extraArgs)
                {
                    Console.WriteLine(ea);
                }
            }
        }

        private static void DisplayHelp()
        {
            Console.Write(
                "Gaigen " + Version + ". Copyright 2008-2010 Daniel Fontijne, University of Amsterdam.\n" +
                "\n" +
                "Compiles a geometric algebra specification into code.\n" +
                "Output files go into the current directory unless the specification overrides.\n" +
                "\n" +
                "Usage: g25 algebra_specification.xml\n" +
                "\n" +
                "Options:\n" +
                "-h -? -help: display help.\n" +
                "-v -version: display version.\n" +
                "-s file.xml -save file.xml: saves loaded specification back to XML (for testing).\n" +
                "-f list.txt -filelist list.txt: writes names of generated files to text file.\n" +
                "\n");
        }

        /// <summary>
        /// Loads the default plugins for the output language of 'L'
        /// </summary>
        static public void LoadDefaultCodeGenerators(CodeGeneratorLoader L)
        {
            switch (L.Language)
            {
                case Specification.XML_C:
                    L.AddAssembly(typeof(G25.CG.C.MainGenerator).Assembly);
                    break;
                case Specification.XML_CPP:
                    L.AddAssembly(typeof(G25.CG.CPP.MainGenerator).Assembly);
                    break;
                case Specification.XML_JAVA:
                    L.AddAssembly(typeof(G25.CG.Java.MainGenerator).Assembly);
                    break;
                case Specification.XML_CSHARP:
                    L.AddAssembly(typeof(G25.CG.CSharp.MainGenerator).Assembly);
                    break;
                case Specification.XML_PYTHON:
                    L.AddAssembly(typeof(G25.CG.Python.MainGenerator).Assembly);
                    break;
                case Specification.XML_MATLAB:
                    L.AddAssembly(typeof(G25.CG.Matlab.MainGenerator).Assembly);
                    break;
            }
        }

        public static void SaveGenerateFileList(string filename, List<string> generatedFiles) {
            StringBuilder SB = new StringBuilder();

            foreach (string genFilename in generatedFiles)
            {
                SB.AppendLine(genFilename);
            }

            G25.CG.Shared.Util.WriteFile(filename, SB.ToString());
        }


        /*        static void NDeskOptionsTest()
                { // little test of using the NDesk options 
                    string[] args = new string[] { "-foo", "false", "c3ga.xml"};
        //                args = new string[] { "-v", "-v", "-name=\"blah\"" };
                    bool show_help = false;
                    int verbose = 0;
                    List<string> names = new List<string>();

                    NDesk.Options.OptionSet p = new NDesk.Options.OptionSet() {
                        { "foo=", (Boolean B) => Console.WriteLine (B.ToString ()) },
                    };

                    NDesk.Options.OptionSet p = new NDesk.Options.OptionSet()
                      .Add("v|verbose", delegate(string v) { if (v != null) ++verbose; })
                      .Add("h|?|help", delegate(string v) { show_help = v != null; })
                      .Add("n|name=", delegate(string v) { names.Add(v); });
                    List<string> extra = p.Parse(args);


                    int x = 0;
                }*/


    } // end of class Program
}
