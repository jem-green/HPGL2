using System;
using System.IO;
using System.Threading;
using HPGL2Library;
using System.Diagnostics;
using TracerLibrary;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using GcodeLibrary;
using ShapeLibrary;

namespace HPGL2Console
{
    class Program
    {
        #region Fields

        public static bool isClosing = false;
        static private HandlerRoutine ctrlCHandler;
        private static HPGL2Document _hpgl2;

        #endregion
		#region unmanaged

        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);
        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion
        #region Constructor
        #endregion
        #region Methods

        static void Main(string[] args)
        {
            Debug.WriteLine("Enter Main()");

            ctrlCHandler = new HandlerRoutine(ConsoleCtrlCheck);
            SetConsoleCtrlHandler(ctrlCHandler, true);

            // Read in specific configuration

            Parameter<string> appPath = new Parameter<string>("");
            Parameter<string> appName = new Parameter<string>("hpgl2.xml");

            appPath.Value = System.Reflection.Assembly.GetExecutingAssembly().Location;
            int pos = appPath.Value.ToString().LastIndexOf(Path.DirectorySeparatorChar);
            if (pos > 0)
            {
                appPath.Value = appPath.Value.ToString().Substring(0, pos);
                appPath.Source = Parameter<string>.SourceType.App;
            }
            Parameter<string> logPath = new Parameter<string>("");
            Parameter<string> logName = new Parameter<string>("hpgl2console");
            logPath.Value = System.Reflection.Assembly.GetExecutingAssembly().Location;
            pos = logPath.Value.ToString().LastIndexOf(Path.DirectorySeparatorChar);
            if (pos > 0)
            {
                logPath.Value = logPath.Value.ToString().Substring(0, pos);
                logPath.Source = Parameter<string>.SourceType.App;
            }

            Parameter<SourceLevels> traceLevels = new Parameter<SourceLevels>
            {
                Value = TraceInternal.TraceLookup("VERBOSE"),
                Source = Parameter<SourceLevels>.SourceType.App
            };

            // Configure tracer options

            string filenamePath = logPath.Value.ToString() + Path.DirectorySeparatorChar + logName.Value.ToString() + ".log";
            FileStreamWithRolling dailyRolling = new FileStreamWithRolling(filenamePath, new TimeSpan(0, 1, 0, 0), FileMode.Append);
            TextWriterTraceListenerWithTime listener = new TextWriterTraceListenerWithTime(dailyRolling);
            Trace.AutoFlush = true;
            TraceFilter fileTraceFilter = new System.Diagnostics.EventTypeFilter(SourceLevels.Verbose);
            listener.Filter = fileTraceFilter;
            Trace.Listeners.Clear();
            Trace.Listeners.Add(listener);

            ConsoleTraceListener console = new ConsoleTraceListener();
            TraceFilter consoleTraceFilter = new System.Diagnostics.EventTypeFilter(SourceLevels.Information);
            //TraceFilter consoleTraceFilter = new System.Diagnostics.EventTypeFilter(SourceLevels.Verbose);

            console.Filter = consoleTraceFilter;
			Trace.Listeners.Add(console);

            if (IsLinux == false)
            {

                // Check if the registry has been set and overwrite the application defaults

                RegistryKey key = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
                string keys = "software\\green\\hpgl2";
                foreach (string subkey in keys.Split('\\'))
                {
                    key = key.OpenSubKey(subkey);
                    if (key == null)
                    {
                        TraceInternal.TraceVerbose("Failed to open" + subkey);
                        break;
                    }
                }

                // Get the log path

                try
                {
                    if ((key != null) && (key.GetValue("logpath", "").ToString().Length > 0))
                    {
                        logPath.Value = (string)key.GetValue("logpath", logPath);
                        logPath.Source = Parameter<string>.SourceType.Registry;
                        TraceInternal.TraceVerbose("Use registry value; logPath=" + logPath);
                    }
                }
                catch (NullReferenceException)
                {
                    TraceInternal.TraceVerbose("Registry error use default values; logPath=" + logPath.Value);
                }
                catch (Exception e)
                {
                    TraceInternal.TraceError(e.ToString());
                }

                // Get the log name

                try
                {
                    if ((key != null) && (key.GetValue("logname", "").ToString().Length > 0))
                    {
                        logName.Value = (string)key.GetValue("logname", logName);
                        logName.Source = Parameter<string>.SourceType.Registry;
                        TraceInternal.TraceVerbose("Use registry value; LogName=" + logName);
                    }
                }
                catch (NullReferenceException)
                {
                    TraceInternal.TraceVerbose("Registry error use default values; LogName=" + logName.Value);
                }
                catch (Exception e)
                {
                    TraceInternal.TraceError(e.ToString());
                }

                // Get the name

                try
                {
                    if ((key != null) && (key.GetValue("name", "").ToString().Length > 0))
                    {
                        appName.Value = (string)key.GetValue("name", appName);
                        appName.Source = Parameter<string>.SourceType.Registry;
                        TraceInternal.TraceVerbose("Use registry value Name=" + appName);
                    }
                }
                catch (NullReferenceException)
                {
                    TraceInternal.TraceVerbose("Registry error use default values; Name=" + appName.Value);
                }
                catch (Exception e)
                {
                    TraceInternal.TraceError(e.ToString());
                }

                // Get the path

                try
                {
                    if ((key != null) && (key.GetValue("path", "").ToString().Length > 0))
                    {
                        appPath.Value = (string)key.GetValue("path", appPath);
                        appPath.Source = Parameter<string>.SourceType.Registry;
                        TraceInternal.TraceVerbose("Use registry value Path=" + appPath);
                    }
                }
                catch (NullReferenceException)
                {
                    TraceInternal.TraceVerbose("Registry error use default values; Name=" + appPath.Value);
                }
                catch (Exception e)
                {
                    TraceInternal.TraceError(e.ToString());
                }

                // Get the traceLevels

                try
                {
                    if ((key != null) && (key.GetValue("debug", "").ToString().Length > 0))
                    {
                        traceLevels.Value = TraceInternal.TraceLookup((string)key.GetValue("debug", "verbose"));
                        traceLevels.Source = Parameter<SourceLevels>.SourceType.Registry;
                        TraceInternal.TraceVerbose("Use registry value; Debug=" + traceLevels.Value);
                    }
                }
                catch (NullReferenceException)
                {
                    TraceInternal.TraceWarning("Registry error use default values; Debug=" + traceLevels.Value);
                }
                catch (Exception e)
                {
                    TraceInternal.TraceError(e.ToString());
                }
            }

            // Check if the config file has been paased in and overwrite the registry

            filenamePath = "";
            string extension = "";
            int items = args.Length;
            for (int item = 0; item < items; item++)
            {
                string lookup = args[item];
                if (lookup.Length > 1)
                {
                    lookup = lookup.ToLower();
                }
                switch (lookup)
                {
                    case "/D":
                    case "--debug":
						{
							string traceName = args[item + 1];
                            traceName = traceName.TrimStart('"');
                            traceName = traceName.TrimEnd('"');
                            traceLevels.Value = TraceInternal.TraceLookup(traceName);
                            traceLevels.Source = Parameter<SourceLevels>.SourceType.Command;
                            TraceInternal.TraceVerbose("Use command value traceLevels=" + traceLevels);
                            break;
						}
                    case "/N":
                    case "--name":
						{
                        	appName.Value = args[item + 1];
                        	appName.Value = appName.Value.ToString().TrimStart('"');
                        	appName.Value = appName.Value.ToString().TrimEnd('"');
                        	appName.Source = Parameter<string>.SourceType.Command;
                        	TraceInternal.TraceVerbose("Use command value Name=" + appName);
                        	break;
						}
                    case "/P":
                    case "--path":
						{
                        	appPath.Value = args[item + 1];
                        	appPath.Value = appPath.Value.ToString().TrimStart('"');
                        	appPath.Value = appPath.Value.ToString().TrimEnd('"');
                        	appPath.Source = Parameter<string>.SourceType.Command;
                        	TraceInternal.TraceVerbose("Use command value Path=" + appPath);
                        	break;
						}
                    case "/n":
                    case "--logname":
						{
                        	logName.Value = args[item + 1];
                        	logName.Value = logName.Value.ToString().TrimStart('"');
                        	logName.Value = logName.Value.ToString().TrimEnd('"');
                        	logName.Source = Parameter<string>.SourceType.Command;
                        	TraceInternal.TraceVerbose("Use command value logName=" + logName);
                        	break;
						}
                    case "/p":
                    case "--logpath":
						{
                        	logPath.Value = args[item + 1];
                        	logPath.Value = logPath.Value.ToString().TrimStart('"');
                        	logPath.Value = logPath.Value.ToString().TrimEnd('"');
                        	logPath.Source = Parameter<string>.SourceType.Command;
                        	TraceInternal.TraceVerbose("Use command value logPath=" + logPath);
                        	break;
						}
                }
            }
			
		    // Adjust the log location if it has been overridden on the commandline or registry

            Trace.Listeners.Remove(listener);
            listener.Close();
            listener.Dispose();
            filenamePath = logPath.Value.ToString() + Path.DirectorySeparatorChar + logName.Value.ToString() + ".log";
            dailyRolling = new FileStreamWithRolling(filenamePath, new TimeSpan(0, 1, 0, 0), FileMode.Append);
            listener = new TextWriterTraceListenerWithTime(dailyRolling);
            Trace.AutoFlush = true;
            SourceLevels sourceLevels = TraceInternal.TraceLookup(traceLevels.Value.ToString());
            fileTraceFilter = new System.Diagnostics.EventTypeFilter(sourceLevels);
            listener.Filter = fileTraceFilter;
            Trace.Listeners.Add(listener);

            TraceInternal.TraceInformation("Use Name=" + appName.Value);
            TraceInternal.TraceInformation("Use Path=" + appPath.Value);
            TraceInternal.TraceInformation("Use Log Name=" + logName.Value);
            TraceInternal.TraceInformation("Use Log Path=" + logPath.Value);  

            // Read in configuration

            Serialise serialise = new Serialise(appName.Value, appPath.Value);
            _hpgl2 = serialise.FromXML();
            if (_hpgl2 != null)
            {

                // Read in the plot specific parameters

                Parameter<string> filename = new Parameter<string>("");
                Parameter<string> filePath = new Parameter<string>("");
                Parameter<string> outName = new Parameter<string>("");

                filePath.Value = Environment.CurrentDirectory;
                filePath.Source = Parameter<string>.SourceType.App;

                items = args.Length;
                if (items == 1)
                {
                    int index = 0;
                    filenamePath = args[index].Trim('"');
                    pos = filenamePath.LastIndexOf('.');
                    if (pos > 0)
                    {
                        extension = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                        filenamePath = filenamePath.Substring(0, pos);
                    }

                    pos = filenamePath.LastIndexOf('\\');
                    if (pos > 0)
                    {
                        filePath.Value = filenamePath.Substring(0, pos);
                        filePath.Source = Parameter<string>.SourceType.Command;
                        filename.Value = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                        filename.Source = Parameter<string>.SourceType.Command;
                    }
                    else
                    {
                        filename.Value = filenamePath;
                        filename.Source = Parameter<string>.SourceType.Command;
                    }
                    TraceInternal.TraceInformation("Use Filename=" + filename.Value);
                    TraceInternal.TraceInformation("Use Filepath=" + filePath.Value);
                }
                else
                {
                    for (int item = 0; item < items; item++)
                    {
                        {
                            switch (args[item])
                            {
                                case "/f":
                                case "--filename":
                                    {
                                        filename.Value = args[item + 1];
                                        filename.Value = filename.Value.TrimStart('"');
                                        filename.Value = filename.Value.TrimEnd('"');
                                        filename.Source = Parameter<string>.SourceType.Command;
                                        pos = filename.Value.LastIndexOf('.');
                                        if (pos > 0)
                                        {
                                            extension = filename.Value.Substring(pos + 1, filename.Value.Length - pos - 1);
                                            filename.Value = filename.Value.Substring(0, pos);
                                        }
                                        TraceInternal.TraceVerbose("Use command value Filename=" + filename);
                                        break;
                                    }
                                case "/O":
                                case "--output":
                                    {
                                        outName.Value = args[item + 1];
                                        outName.Value = outName.Value.TrimStart('"');
                                        outName.Value = outName.Value.TrimEnd('"');
                                        outName.Source = Parameter<string>.SourceType.Command;
                                        TraceInternal.TraceVerbose("Use command value Output=" + outName);
                                        break;
                                    }
                                case "/p":
                                case "--filepath":
                                    {
                                        filePath.Value = args[item + 1];
                                        filePath.Value = filePath.Value.TrimStart('"');
                                        filePath.Value = filePath.Value.TrimEnd('"');
                                        filePath.Source = Parameter<string>.SourceType.Command;
                                        TraceInternal.TraceVerbose("Use command value Filename=" + filePath);
                                        break;
                                    }
                            }
                        }
                    }
                    TraceInternal.TraceInformation("Use Filename=" + filename.Value);
                    TraceInternal.TraceInformation("Use Filepath=" + filePath.Value);
                }

                if (outName.Value.Length == 0)
                {
                    outName.Value = filename.Value;
                }

                // Read the plot data and create the CNC file

                if (_hpgl2.Read(filePath.Value, filename.Value) == true)
                {
                    _hpgl2.Process();
                }

                // Export to shape

                SHPDocument shape = _hpgl2.ToShape();
                shape.Save(filePath.Value, outName.Value);

                ManualResetEvent manualResetEvent = new ManualResetEvent(false);

                manualResetEvent.WaitOne();
            }

            Debug.WriteLine("Out Main()");
        }
		
		private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            Debug.WriteLine("In ConsoleCtrlCheck()");

            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    {
                        isClosing = true;
                        TraceInternal.TraceInformation("CTRL+C received:");
                        break;
                    }

                case CtrlTypes.CTRL_BREAK_EVENT:
                    {
                        isClosing = true;
                        TraceInternal.TraceInformation("CTRL+BREAK received:");
                        break;
                    }
                case CtrlTypes.CTRL_CLOSE_EVENT:
                    {
                        isClosing = true;
                        TraceInternal.TraceInformation("Program being closed:");
                        break;
                    }
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    {
                        isClosing = true;
                        TraceInternal.TraceInformation("User is logging off:");
                        break;
                    }
            }

            _hpgl2.Dispose();
            Debug.WriteLine("Out ConsoleCtrlCheck:");

            Environment.Exit(0);

            return (true);

        }

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        #endregion
    }
}
