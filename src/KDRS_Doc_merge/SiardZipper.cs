﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace binFileMerger
{
    class SiardZipper
    {
        public void SiardZip(string folder, string targetName, string jarPath)
        {
            //string javaPath = @"C:\prog\zip64-2.1.58\zip64\lib\zip64.jar";
            string source = "-d=" + folder;
            string target = targetName + ".siard";

            string javaCommand = " -jar " + jarPath + " n -c " + source + " " + target;

            Process proc = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.Arguments = javaCommand;
            startInfo.FileName = "java";

            proc.StartInfo = startInfo;

            Console.WriteLine(javaCommand);

            try
            {
                Console.WriteLine("Creating .siard");
                proc.Start();
                proc.WaitForExit();

                Console.WriteLine(".siard Created");
            }
            catch
            {
                Console.WriteLine("zip error");
            }
        }

        public void SiardUnZip(string sourceFile, string targetFolder, string jarPath)
        {
            //string javaPath = @"C:\prog\zip64_v2.1.58\zip64-2.1.58\lib\zip64.jar";
            string target = "-d=\"" + targetFolder + "\"";
            string source = sourceFile;

            string javaCommand = " -jar " + jarPath + " x " + target + " " + source;

            Process proc = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.Arguments = javaCommand;
            startInfo.FileName = "java";

            proc.StartInfo = startInfo;

            Console.WriteLine(javaCommand);

            System.IO.Directory.CreateDirectory(targetFolder);
            
            try
            {
                Console.WriteLine("Creating .siard");
                proc.Start();
                proc.WaitForExit();
                Console.WriteLine(".siard Created");
            }
            catch
            {
                Console.WriteLine("unzip error");
            }
        }
    }
}
