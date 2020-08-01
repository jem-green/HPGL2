using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace HPGL2Library
{
    /// <summary>
    /// Serialise and deserialise confguration data
    /// </summary>
    public class Serialise
    {
        /*
         * 
         * <?xml version="1.0" encoding="utf-8" ?>
         * <hpgl2>
         *   <configuration>
         *   </configuration>
         *   <page>
         *     <left>0</left>
         *     <bottom>0</bottom>
         *     <width>40</width>
         *     <height>40</height>
         *   </page>
         * </hpgl2>
         * 
         */

        #region Variables

        ILogger _logger;
        string _filename = "";
        string _path = "";

        #endregion
        #region Constructor
        public Serialise(ILogger logger)
        {
            _logger = logger;
        }
        public Serialise(string filename, string path, ILogger logger)
        {
            _logger = logger;
            this._filename = filename;
            this._path = path;
        }
        #endregion
        #region Properties
        public string Filename
        {
            get
            {
                return (_filename);
            }
            set
            {
                _filename = value;
            }
        }

        public string Path
        {
            get
            {
                return (_path);
            }
            set
            {
                _path = value;
            }
        }

        #endregion
        #region Methods

        public HPGL2 FromXML()
        {
            return(FromXML(_filename, _path));
        }

        public HPGL2 FromXML(string filename, string path)
        {
            _logger.LogDebug("In FromXML()");
			HPGL2 hpgl2 = null;
            Page page = null;

            try
            {
                // Point to the file

                string fileLocation = System.IO.Path.Combine(path, filename);
                try
                {
                    FileStream fs = new FileStream(fileLocation, FileMode.Open, FileAccess.Read);

                    // Pass the parameters in

                    XmlReaderSettings xmlSettings = new XmlReaderSettings
                    {

                        // Enable <!ENTITY to be expanded
                        // <!ENTITY chap1 SYSTEM "chap1.xml">
                        // &chap1;

                        DtdProcessing = DtdProcessing.Parse
                    };

                    // Open the file and pass in the settings

                    try
                    {
                        Stack<string> stack = new Stack<string>();
                        string element = "";
                        string text = "";
                        string current = "";    // Used to flag what level we are at
                        int level = 1;          // Indentation level

                        XmlReader xmlReader = XmlReader.Create(fs, xmlSettings);
                        while (xmlReader.Read())
                        {
                            switch (xmlReader.NodeType)
                            {
                                #region Element
                                case XmlNodeType.Element:
                                    {
                                        element = xmlReader.LocalName.ToLower();

                                        if (!xmlReader.IsEmptyElement)
                                        {
                                            _logger.LogInformation(Level(level) + "<" + element + ">");
                                            level = level + 1;
                                        }
                                        else
                                        {
                                            _logger.LogInformation(Level(level) + "<" + element + "/>");
                                        }
                                        switch (element)
                                        {
                                            #region HPGL2
                                            case "hpgl2":
                                                {
                                                    stack.Push(current);
                                                    current = element;
													hpgl2 = new HPGL2(_logger);
                                                    break;
                                                }
                                            case "page":
                                                {
                                                    stack.Push(current);
                                                    current = element;
                                                    page = new Page();
                                                    break;
                                                }
                                            #endregion
                                            default:
                                                {
                                                    stack.Push(current);
                                                    current = element;
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                #endregion
                                #region EndElement
                                case XmlNodeType.EndElement:
                                    {
                                        element = xmlReader.LocalName;
                                        level = level - 1;
                                        _logger.LogInformation(Level(level) + "</" + element + ">");
                                        switch (element)
                                        {
                                            case "hpgl2":
                                                {
                                                    break;
                                                }
                                            case "page":
                                                {
                                                    hpgl2.Page = page;
                                                    break;
                                                }
                                        }
                                        current = stack.Pop();
                                        break;
                                    }
                                #endregion
                                #region Text

                                case XmlNodeType.Text:
                                    {
                                        text = xmlReader.Value;
                                        text = text.Replace("\t", "");
                                        text = text.Replace("\n", "");
                                        text = text.Trim();
                                        _logger.LogInformation(Level(level) + text);

                                        switch (current)
                                        {
                                            case "left":
                                                {
                                                    try
                                                    {
                                                        page.Left = Convert.ToInt32(text);
                                                    }
                                                    catch { }
                                                    break;
                                                }
                                            case "bottom":
                                                {
                                                    try
                                                    {
                                                        page.Bottom = Convert.ToInt32(text);
                                                    }
                                                    catch { }
                                                    break;
                                                }
                                            case "width":
                                                {
                                                    try
                                                    {
                                                        page.Width = Convert.ToInt32(text);
                                                    }
                                                    catch { }
                                                    break;
                                                }
                                            case "height":
                                                {
                                                    try
                                                    {
                                                        page.Height = Convert.ToInt32(text);
                                                    }
                                                    catch { }
                                                    break;
                                                }

                                        }
                                        break;
                                    }
                                #endregion
                                #region Entity
                                case XmlNodeType.Entity:
                                    break;
                                #endregion
                                case XmlNodeType.EndEntity:
                                    break;
                                case XmlNodeType.Whitespace:
                                    break;
                                case XmlNodeType.Comment:
                                    break;
                                case XmlNodeType.Attribute:
                                    break;
                                default:
                                    _logger.LogInformation(xmlReader.NodeType.ToString());
                                    break;

                            }
                        }

                        xmlReader.Close();  // Force the close
                        xmlReader = null;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("XML Error " + ex.Message);
                    }
                    fs.Close();
                    fs.Dispose();   // Force the dispose as it was getting left open
                }
                catch (FileNotFoundException ex)
                {
                    _logger.LogWarning("File Error " + ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("File Error " + ex.Message);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Other Error " + e.Message);
            }

            _logger.LogDebug("Out FromXML()");
            return (hpgl2);
        }
        #endregion
        #region Private
        private static string Level(int level)
        {
            string text = "";
            for (int i = 1; i < level; i++)
            {
                text = text + "  ";
            }
            return (text);
        }
        #endregion
    }
}
