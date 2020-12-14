using System;
using System.Diagnostics;
using System.Threading;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using System.Security.Principal;

namespace ProcSurgeon {
    class ProcSurgeon {
        static int Main(string[] args) {
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            if (!isElevated)
            {
                Console.Error.WriteLine("You must be Administrator to run this.");
                return -2;
            }
            CommandLineParser.CommandLineParser parser = new CommandLineParser.CommandLineParser
            {
                ShowUsageHeader = "ProcSurgeon - Minimal ProcMon logs for when a process starts."
            };
            FileArgument procMon = new FileArgument('p', "procmon", "Path of the procmon executable")
            {
                FileMustExist = true,
                Optional = false
            };
            FileArgument target = new FileArgument('t', "target", "Path of the target executable")
            {
                FileMustExist = true,
                Optional = false
            };
            ValueArgument<string> output = new ValueArgument<string>('o', "output", "Path of the output")
            {
                Optional = false
            };
            ValueArgument<int> time = new ValueArgument<int>('m', "millis", "How many milliseconds to log after starting the target." +
                " If you do not include this parameter, ProcSurgeon will wait for the target process to exit.")
            {
                DefaultValue = 0
            };
            parser.Arguments.Add(procMon);
            parser.Arguments.Add(target);
            parser.Arguments.Add(output);
            parser.Arguments.Add(time);
            try
            {
                parser.ParseCommandLine(args);
            } 
            catch (CommandLineException e)
            {
                Console.Error.WriteLine(e.Message);
                parser.ShowUsage();
                return -1;
            }
            Execute(procMon.Value.FullName, target.Value.FullName, output.Value, time.Value);
            return 0;
        }

        static void Execute(string procMonPath, string targetPath, string outputPath, int millis) {
            Process.Start(procMonPath, "/Terminate").WaitForExit(); //Clear any procmon instances before we begin
            Process.Start(procMonPath, $"/quiet /backingfile {outputPath}");
            Process.Start(procMonPath, "/WaitForIdle").WaitForExit();
            Process target = Process.Start(targetPath);
            if (millis > 0) {
                Thread.Sleep(millis);
            } else {
                target.WaitForExit();
            }
            Process.Start(procMonPath, "/Terminate").WaitForExit();
        }
    }
}
