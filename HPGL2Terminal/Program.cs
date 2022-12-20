using System;
using System.IO;
using System.Threading;
using System.Configuration;
using HPGL2Library;
using System.Diagnostics;
using TracerLibrary;
using GcodeLibrary;

namespace HPGL2Terminal
{
    class Program
    {
        #region Fields

        private static HPGL2Document _hpgl2;

        #endregion
        #region Constructor
        #endregion
        #region Methods

        static void Main(string[] args)
        {
            Debug.WriteLine("In Main()");

            // Read in specific configuration

            Parameter appPath = new Parameter("");
            Parameter appName = new Parameter("hpgl2.xml");

            appPath.Value = System.Reflection.Assembly.GetExecutingAssembly().Location;
            int pos = appPath.Value.ToString().LastIndexOf(Path.DirectorySeparatorChar);
            if (pos > 0)
            {
                appPath.Value = appPath.Value.ToString().Substring(0, pos);
                appPath.Source = Parameter.SourceType.App;
            }
			
			Parameter logPath = new Parameter("");
            Parameter logName = new Parameter("hpgl2terminal");

            logPath.Value = System.Reflection.Assembly.GetExecutingAssembly().Location;
            pos = logPath.Value.ToString().LastIndexOf(Path.DirectorySeparatorChar);
            if (pos > 0)
            {
                logPath.Value = logPath.Value.ToString().Substring(0, pos);
                logPath.Source = Parameter.SourceType.App;
            }

            Parameter traceLevels = new Parameter("");
            traceLevels.Value = "verbose";
            traceLevels.Source = Parameter.SourceType.App;

            // Configure tracer options

            string filenamePath = logPath.Value.ToString() + Path.DirectorySeparatorChar + logName.Value.ToString() + ".log";
            FileStreamWithRolling dailyRolling = new FileStreamWithRolling(filenamePath, new TimeSpan(1, 0, 0, 0), FileMode.Append);
            TextWriterTraceListenerWithTime listener = new TextWriterTraceListenerWithTime(dailyRolling);
            Trace.AutoFlush = true;
            TraceFilter fileTraceFilter = new System.Diagnostics.EventTypeFilter(SourceLevels.Verbose);
            listener.Filter = fileTraceFilter;
            Trace.Listeners.Add(listener);

            ConsoleTraceListener console = new ConsoleTraceListener();
            TraceFilter consoleTraceFilter = new System.Diagnostics.EventTypeFilter(SourceLevels.Verbose);
            console.Filter = consoleTraceFilter;
            Trace.Listeners.Add(console);

            // Check if the config file has been paased in and overwrite the defaults

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
                        	traceLevels.Value = args[item + 1];
                        	traceLevels.Value = traceLevels.Value.ToString().TrimStart('"');
                        	traceLevels.Value = traceLevels.Value.ToString().TrimEnd('"');
                        	traceLevels.Source = Parameter.SourceType.Command;
                        	TraceInternal.TraceVerbose("Use command value Name=" + traceLevels);
                        	break;
						}
                    case "/N":
                    case "--name":
                        {
                            appName.Value = args[item + 1];
                        	appName.Value = appName.Value.ToString().TrimStart('"');
                        	appName.Value = appName.Value.ToString().TrimEnd('"');
                            appName.Source = Parameter.SourceType.Command;
                            TraceInternal.TraceVerbose("Use command value Name=" + appName);
                            break;
                        }
                    case "/P":
                    case "--path":
                        {
                            appPath.Value = args[item + 1];
                        	appPath.Value = appPath.Value.ToString().TrimStart('"');
                        	appPath.Value = appPath.Value.ToString().TrimEnd('"');
                            appPath.Source = Parameter.SourceType.Command;
                            TraceInternal.TraceVerbose("Use command value Path=" + appPath);
                            break;
						}
                    case "/n":
                    case "--logname":
						{
                        	logName.Value = args[item + 1];
                        	logName.Value = logName.Value.ToString().TrimStart('"');
                        	logName.Value = logName.Value.ToString().TrimEnd('"');
                        	logName.Source = Parameter.SourceType.Command;
                        	TraceInternal.TraceVerbose("Use command value logName=" + logName);
                        	break;
						}
                    case "/p":
                    case "--logpath":
						{
                        	logPath.Value = args[item + 1];
                        	logPath.Value = logPath.Value.ToString().TrimStart('"');
                        	logPath.Value = logPath.Value.ToString().TrimEnd('"');
                        	logPath.Source = Parameter.SourceType.Command;
                        	TraceInternal.TraceVerbose("Use command value logPath=" + logPath);
                        	break;
						}
                }
            }
			
		    // Adjust the log location if it has been overridden in the registry

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

                Parameter filename = new Parameter("");
                Parameter filePath = new Parameter("");
                Parameter outName = new Parameter("");

                filePath.Value = Environment.CurrentDirectory;
                filePath.Source = Parameter.SourceType.App;

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
                        filePath.Source = Parameter.SourceType.Command;
                        filename.Value = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                        filename.Source = Parameter.SourceType.Command;
                    }
                    else
                    {
                        filename.Value = filenamePath;
                        filename.Source = Parameter.SourceType.Command;
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
                                        filename.Source = Parameter.SourceType.Command;
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
                                        outName.Source = Parameter.SourceType.Command;
                                        TraceInternal.TraceVerbose("Use command value Output=" + outName);
                                        break;
                                    }
                                case "/p":
                                case "--filepath":
                                    {
                                        filePath.Value = args[item + 1];
                                        filePath.Value = filePath.Value.TrimStart('"');
                                        filePath.Value = filePath.Value.TrimEnd('"');
                                        filePath.Source = Parameter.SourceType.Command;
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

                // Export to gcode

                Gcode gcode = _hpgl2.ToGCode();

                ManualResetEvent manualResetEvent = new ManualResetEvent(false);

                manualResetEvent.WaitOne();
            }

            Debug.WriteLine("Out Main()");
        }

        #endregion
    }
}
