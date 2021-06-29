﻿using System;
using System.IO;
using System.Threading;
using System.Configuration;
using HPGL2Library;
using System.Diagnostics;
using TracerLibrary;
using Microsoft.Win32;

namespace HPGL2Console
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

            // Required for the plot

            Parameter filename = new Parameter("");
            Parameter filePath = new Parameter("");
            Parameter outName = new Parameter("");

            HPGL2Path.Value = System.Reflection.Assembly.GetExecutingAssembly().Location;
            int pos = HPGL2Path.Value.LastIndexOf('\\');
            if (pos > 0)
            {
                HPGL2Path.Value = HPGL2Path.Value.Substring(0, pos);
            }
            HPGL2Path.Source = Parameter.SourceType.App;

            filePath.Value = Environment.CurrentDirectory;
            filePath.Source = Parameter.SourceType.App;

            try
            {
                RegistryKey key = Registry.LocalMachine;
                key = key.OpenSubKey("software\\green\\hpgl2\\");
                if (key != null)
                {
                    if (key.GetValue("path", "").ToString().Length > 0)
                    {
                        HPGL2Path.Value = (string)key.GetValue("path", HPGL2Path);
                        HPGL2Path.Source = Parameter.SourceType.Registry;            
                        TraceInternal.TraceVerbose("Use registry value Path=" + HPGL2Path);
                    }

                    if (key.GetValue("name", "").ToString().Length > 0)
                    {
                        HPGL2Name.Value = (string)key.GetValue("name", HPGL2Name);
                        HPGL2Name.Source = Parameter.SourceType.Registry;
                        TraceInternal.TraceVerbose("Use registry value Name=" + HPGL2Name);
                    }
                }
            }
            catch (NullReferenceException)
            {
               Trace.TraceWarning("Registry error use default values; Name=" + HPGL2Name.Value + " Path=" + HPGL2Path.Value);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }

            // Check if the config file has been paased in and overwrite the registry

            int items = args.Length;
            for (int item = 0; item < items; item++)
            {
                {
                    switch (args[item])
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

            string filenamePath;
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
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            manualResetEvent.WaitOne();

            Debug.WriteLine("Out Main()");
        }
        #endregion
    }
}
