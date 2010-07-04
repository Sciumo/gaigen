using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace g25_copy_resource
{
    class Program
    {
        static void Main(string[] args)
        {
            // check command line arguments
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: g25_copy_resource source destination");
                Environment.Exit(0);
            }

            // get path to where g25 is installed
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string g25InstallDir = Path.GetDirectoryName(exePath);

            // get path to source file
            string source = Path.Combine(g25InstallDir, args[0]);

            // get path to destination file
            string dest = args[1];
            if (System.IO.Directory.Exists(dest))
                dest = Path.Combine(dest, args[0]);


            // make sure dest goes away (copy will not overwrite)
            File.Delete(dest);

            // copy file
            File.Copy(source, dest);
        }
    }
}
