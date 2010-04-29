using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace g25_diff
{
    /// <summary>
    /// This is a small utility program that reads two lists of files (as saved by g25 -f filename.txt)
    /// and compares them using diff. Returns -1 when filelists / files are not equal.
    /// 
    /// Functions in this class exits when they find a problem.
    /// </summary>
    class Program
    {

        public static void Main(string[] args)
        {
            // check command line arguments
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: g25_diff file1.txt file2.txt");
            		Environment.Exit(0);
			}

            // read lists of files
            string[] L1 = ReadFileList(args[0]);
            string[] L2 = ReadFileList(args[1]);

            // see if file lists have the same length
            if (L1.Length != L2.Length)
            {
                Console.WriteLine("File lists do not have the same length");
                Environment.Exit(-1);
            }

            // compare all files for equality (will print differences to the console)
            bool allEqual = true;
            for (int i = 0; i < L1.Length; i++)
            {
                allEqual = allEqual && Equal(L1[i], L2[i]);
            }

            // if not all equal, then exit with error code.
            if (!allEqual) Environment.Exit(-1);

        }

        public static bool Equal(string file1, string file2)
        {
            // read files
            string str1, str2;
            try
            {
                str1 = System.IO.File.ReadAllText(file1);
            }
            catch (Exception)
            {
                Console.WriteLine("Error reading: " + file1);
                Environment.Exit(-1);
                return false;
            }

            try
            {
                str2 = System.IO.File.ReadAllText(file2);
            }
            catch (Exception)
            {
                Console.WriteLine("Error reading: " + file2);
                Environment.Exit(-1);
                return false;
            }
            

            bool trimSpace = false;
            bool ignoreSpace = false;
            bool ignoreCase = false;
            G25.Util.Diff.Item[] diffs = G25.Util.Diff.DiffText(str1, str2, trimSpace, ignoreSpace, ignoreCase);

            if (diffs.Length == 0) return true; // equal
            else
            {
                PrintDiffs(file1, file2, diffs);
                return false;
            }
            
        }

        public static void PrintDiffs(string file1, string file2, G25.Util.Diff.Item[] diffs)
        {
            Console.WriteLine("Found differences between the following files:");
            Console.WriteLine("file 1: " + file1);
            Console.WriteLine("file 2: " + file2);

            foreach (G25.Util.Diff.Item diff in diffs)
            {
                Console.WriteLine("File 1, line " + diff.StartA);
                Console.WriteLine("File 2, line " + diff.StartB);
            }
        }



        public static string[]  ReadFileList(string filename)
        {
            try {
                return System.IO.File.ReadAllLines(filename);
            } catch (Exception) {
                Console.WriteLine("Error reading " + filename);
                Environment.Exit(-1);
                return null; 
            }
        }

    }
}
