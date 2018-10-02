using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using WhileFalseStudios.Falsh.BuiltinCommands;

namespace WhileFalseStudios.Falsh
{
    class ShellEnvironment
    {
        public static ShellEnvironment Instance { get; private set; }

        const string HISTORY_FILE_NAME = ".falsh_history";
        static string FalshHistoryFile
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), HISTORY_FILE_NAME);
            }
        }

        private int m_windowSize = 40;
        private readonly List<string> m_args;
        private Process m_currentProcess;
        private EventWaitHandle m_processExitWaitHandle = new AutoResetEvent(false);
        private Queue<string> m_historyBuffer = new Queue<string>();
        public ulong HistoryBufferSize { get; set; } = 20;
        private Dictionary<string, IBuiltInCommand> m_builtinCommands = new Dictionary<string, IBuiltInCommand>()
        {
            { "cd", new ChangeDirectoryCommand() },
            { "pwd", new PrintWorkingDirCommand() },
            { "dir", new PrintDirectoryContentsCommand() },
            { "cls", new ClearScreenCommand() },
            { "history", new PrintHistoryCommand() },
            { "hsz", new SetHistoryBufferSizeCommand() },
            { "exit", new ExitCommand() },
            { "help", new HelpCommand() },
        };

        public bool WantsToQuit { get; set; }

        public static char PathSeperatorChar
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return ';';
                }
                else
                {
                    return ':';
                }
            }
        }

        public ShellEnvironment(string[] args)
        {
            Instance = this;
            m_args = args.ToList();
            //ConstructBuiltInCommandList();
        }

        #region Built-in Commands

        private void ConstructBuiltInCommandList()
        {
            var commands = ReflectionUtility.GetTypesWithAttribute<CommandAttribute>(Assembly.GetExecutingAssembly());
            foreach (var command in commands)
            {
                if (command.Key.IsAssignableFrom(typeof(IBuiltInCommand)))
                {
                    Console.WriteLine($"Found command: {command.Value.Name}");
                    m_builtinCommands.Add(command.Value.Name, Activator.CreateInstance(command.Key) as IBuiltInCommand);
                }
            }
        }

        private bool BuiltInCommandExists(string cmd) => m_builtinCommands.ContainsKey(cmd);

        private void ExecBuiltInCommand(string cmd, params string[] args)
        {
            if (BuiltInCommandExists(cmd))
            {
                m_builtinCommands[cmd].Exec(args);
            }
        }

        #endregion

        public IEnumerable<string> CommandHistory
        {
            get
            {
                return m_historyBuffer.AsEnumerable();
            }
        }

        public void Run()
        {
            LoadHistory();
            SetupPrompt();

            while (!WantsToQuit)
            {
                ReadAndProcessInput();
            }

            SaveHistory();
        }

        private void LoadHistory()
        {
            if (File.Exists(FalshHistoryFile))
            {
                m_historyBuffer.Clear();
                var fh = File.ReadAllLines(FalshHistoryFile).ToList();
                if (ulong.TryParse(fh.First(), out ulong result))
                {
                    HistoryBufferSize = result;
                    fh.RemoveAt(0);
                }

                foreach (var cmd in fh)
                {
                    m_historyBuffer.Enqueue(cmd);
                }
            }
        }

        private void SaveHistory()
        {
            try
            {
                using (var fs = File.OpenWrite(FalshHistoryFile))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        WriteNormalLine("Saving session history...");
                        sw.WriteLine(HistoryBufferSize);
                        while (m_historyBuffer.TryDequeue(out string result))
                        {
                            sw.WriteLine(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLine($"Failed to save session history: {ex.Message}");
            }
        }

        private void SetupPrompt()
        {
            Console.Title = $"{Environment.UserName}@falsh";
            if (!m_args.Contains("-s"))
            {
                WriteColorLine("Falsh Shell (C) Andrew Castillo 2018", ConsoleColor.Yellow);

                try
                {
                    int consoleWidth = Console.BufferWidth;
                }
                catch (Exception ex)
                {
                    WriteNormal("Your terminal does not support automatic detection of the column count.\nPlease enter the width of your terminal: ");
                    bool valid = false;
                    while (!valid)
                    {
                        string cin = Console.ReadLine();
                        if (uint.TryParse(cin, out uint sz))
                        {
                            valid = true;
                            m_windowSize = (int)sz;
                        }
                        else
                        {
                            WriteNormal($"Please enter a valid number between 0 and {int.MaxValue}: ");
                        }
                    }
                }

                WriteSeparator();
            }
        }

        private void ReadAndProcessInput()
        {
            WriteColor($"{Environment.UserName}@falsh", ConsoleColor.Green);
            WriteColor($":{Directory.GetCurrentDirectory()}$ ", ConsoleColor.DarkGray);
            Console.ForegroundColor = ConsoleColor.Gray;
            string cmdInput = Console.ReadLine();            

            //Tokenise it
            List<string> tokens = cmdInput.Split(' ', options: StringSplitOptions.RemoveEmptyEntries).ToList();

            if (tokens.Count == 0) //Nothing to execute
                return;

            string cmd = tokens[0];
            tokens.RemoveAt(0);

            if (BuiltInCommandExists(cmd)) //Run built-in command instead if it exists.
            {
                ExecBuiltInCommand(cmd, tokens.ToArray());
            }
            else
            {
                string realAppPath = FindApplicationInPath(cmd);
                if (realAppPath == string.Empty)
                {
                    WriteErrorLine($"{cmd} is not a built-in command and could not be found on the PATH or in the current directory.");
                }
                else
                {
                    RunProgram(realAppPath, tokens.ToArray());
                }
            }

            if (!WantsToQuit)
            {
                if ((ulong)m_historyBuffer.Count + 1 > HistoryBufferSize)
                {
                    m_historyBuffer.Dequeue();
                }

                m_historyBuffer.Enqueue(cmdInput); //Add it after we run in case the shell would exit, in which case we don't want to add that command to the history
            }

            WriteSeparator();
        }

        private void RunProgram(string appPath, params string[] args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(appPath, string.Join(' ', args));
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            startInfo.ErrorDialog = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardError = true;
            try
            {
                m_currentProcess = new Process();

                m_currentProcess.EnableRaisingEvents = true;
                m_currentProcess.StartInfo = startInfo;
                m_currentProcess.OutputDataReceived += ProcessOnWriteStdOut;
                m_currentProcess.ErrorDataReceived += ProcessOnWriteStdErr;
                m_currentProcess.Exited += ProcessOnExit;                

                m_currentProcess.Start();
                m_currentProcess.BeginOutputReadLine(); //async evil    
                m_currentProcess.BeginErrorReadLine();
                m_processExitWaitHandle.WaitOne();
            }
            catch (Exception ex)
            {
                WriteErrorLine($"Failed to exec {Path.GetFileNameWithoutExtension(appPath)}: {ex.Message}");
            }            
        }

        #region Process I/O reads

        private void ProcessOnWriteStdOut(object sender, DataReceivedEventArgs e)
        {
            WriteNormalLine(e.Data);
        }

        private void ProcessOnWriteStdErr(object sender, DataReceivedEventArgs e)
        {
            WriteErrorLine(e.Data);
        }

        #endregion

        private void ProcessOnExit(object sender, EventArgs e)
        {
            m_currentProcess.CancelOutputRead();
            m_currentProcess.CancelErrorRead();
            m_currentProcess.OutputDataReceived -= ProcessOnWriteStdOut;
            m_currentProcess.ErrorDataReceived -= ProcessOnWriteStdErr;
            m_currentProcess.Exited -= ProcessOnExit;            

            WriteNormalLine($"Exited with code {m_currentProcess.ExitCode}");            
            m_currentProcess = null;
            m_processExitWaitHandle.Set();
        }

        #region Util

        private string FindApplicationInPath(string appName)
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            List<string> pathList = path.Split(PathSeperatorChar).ToList();
            pathList.Add(Directory.GetCurrentDirectory());
            foreach (var p in pathList)
            {                
                string app = Path.Combine(p, GetPlatformNativeProgramExt(appName));
                if (File.Exists(app))
                {
                    return app;
                }
            }

            // Return nothing, rest of code will handle as not found.
            return string.Empty;
        }

        private string GetPlatformNativeProgramExt(string appName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) //On windows we want to add .exe if we don't specify so we are really sure of what we're running.
            {
                if (Path.GetExtension(appName) != "") return appName;
                else return $"{Path.GetFileNameWithoutExtension(appName)}.exe";
            }
            else
            {
                return appName;
            }
        }

        private void WriteSeparator()
        {
            int csz = 0;
            try
            {
                csz = Console.BufferWidth - 1;
            }
            catch
            {
                csz = m_windowSize;
            }

            for (int i = 0; i < csz; i++)
            {
                WriteColor("-", ConsoleColor.DarkGray);
            }

            Console.Write("\n");
        }

        #endregion

        #region Console Write Utils

        public static void WriteNormalLine(string line)
        {
            WriteColorLine(line, ConsoleColor.Gray);
        }

        public static void WriteErrorLine(string line)
        {
            WriteColorLine(line, ConsoleColor.Red);
        }

        public static void WriteNormal(string line)
        {
            WriteColor(line, ConsoleColor.Gray);
        }

        public static void WriteError(string line)
        {
            WriteColor(line, ConsoleColor.Red);
        }

        public static void WriteColorLine(string line, ConsoleColor color)
        {
            var fgc = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ForegroundColor = fgc;
        }

        public static void WriteColor(string line, ConsoleColor color)
        {
            var fgc = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(line);
            Console.ForegroundColor = fgc;
        }

        #endregion
    }
}
