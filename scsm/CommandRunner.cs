using System;

namespace scsm
{
    public class CommandRunner
    {
        private const string Success = "success";
        private const string Scsm = "scsm.exe";

        public bool RunOptionWithArguments(string option, string arg)
        {
            try
            {
                var args = option + " " + arg;
                var result = RunCommand(Scsm, args);

                return result.StartsWith(Success) || string.IsNullOrWhiteSpace(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool CheckCommandLine()
        {
            return RunOptionWithArguments(string.Empty, string.Empty);
        }

        private static string RunCommand(string command, string args)
        {
            var process = new System.Diagnostics.Process
                               {
                                   StartInfo =
                                       {
                                           FileName = command,
                                           Arguments = args,
                                           UseShellExecute = false,
                                           RedirectStandardOutput = true,
                                           CreateNoWindow = true
                                       }
                               };

            process.Start();
            var result = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return result;
        }

        public string GetCommandResult(string optsargs)
        {
            return RunCommand(Scsm, optsargs);
        }
    }

}
