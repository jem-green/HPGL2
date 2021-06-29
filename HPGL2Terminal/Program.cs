using System;
using System.IO;
using System.Threading;
using System.Configuration;
using HPGL2Library;
using System.Diagnostics;
using TracerLibrary;

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

            Parameter HPGL2Path = new Parameter("");
            Parameter HPGL2Name = new Parameter("hpgl2.xml");

            HPGL2Path.Value = System.Reflection.Assembly.GetExecutingAssembly().Location;
            int pos = HPGL2Path.Value.ToString().LastIndexOf(Path.DirectorySeparatorChar);
            if (pos > 0)
            {
                HPGL2Path.Value = HPGL2Path.Value.ToString().Substring(0, pos);
                HPGL2Path.Source = Parameter.SourceType.App;
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
                    case "/N":
                    case "--name":
                        {
                            HPGL2Name.Value = args[item + 1];
                            HPGL2Name.Value = HPGL2Name.Value.TrimStart('"');
                            HPGL2Name.Value = HPGL2Name.Value.TrimEnd('"');
                            HPGL2Name.Source = Parameter.SourceType.Command;
                            TraceInternal.TraceVerbose("Use command value Name=" + HPGL2Name);
                            break;
                        }
                    case "/P":
                    case "--path":
                        {
                            HPGL2Path.Value = args[item + 1];
                            HPGL2Path.Value = HPGL2Path.Value.TrimStart('"');
                            HPGL2Path.Value = HPGL2Path.Value.TrimEnd('"');
                            HPGL2Path.Source = Parameter.SourceType.Command;
                            TraceInternal.TraceVerbose("Use command value Path=" + HPGL2Path);
                            break;
                        }
                }
            }

            Trace.TraceInformation("Use HPGL2Name=" + HPGL2Name.Value);
            Trace.TraceInformation("Use HPGL2Path=" + HPGL2Path.Value);

            // Read in configuration

            Serialise serialise = new Serialise(HPGL2Name.Value, HPGL2Path.Value);
            _hpgl2 = serialise.FromXML();
            if (_hpgl2 != null)
            {

            }

            // Read in the plot specific parameters

            Parameter filename = new Parameter("");
            Parameter filePath = new Parameter("");
            Parameter outName = new Parameter("");

            filePath.Value = Environment.CurrentDirectory;
            filePath.Source = Parameter.SourceType.App;

            string extension;
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
                Trace.TraceInformation("Use Filename=" + filename.Value);
                Trace.TraceInformation("Use Filepath=" + filePath.Value);
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
                Trace.TraceInformation("Use Filename=" + filename.Value);
                Trace.TraceInformation("Use Filepath=" + filePath.Value);
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

            Debug.WriteLine("Out Main()");
        }
        #endregion
    }
}
