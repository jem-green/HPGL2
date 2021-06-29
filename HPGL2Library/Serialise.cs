using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;

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
         *     <length>40</length>
         *   </page>
         * </hpgl2>
         * 
         */

        #region Fields

        string _filename = "";
        string _path = "";

        #endregion
        #region Constructor
        public Serialise(string filename, string path)
        {
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

        public HPGL2Document FromXML()
        {
            return(FromXML(_filename, _path));
        }

        public HPGL2Document FromXML(string filename, string path)
        {
            Debug.WriteLine("In FromXML()");
			HPGL2Document hpgl2 = null;
            Page page = null;
            Pen pen = null;

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
                                            Trace.TraceInformation(Level(level) + "<" + element + ">");
                                            level = level + 1;
                                        }
                                        else
                                        {
                                            Trace.TraceInformation(Level(level) + "<" + element + "/>");
                                        }
                                        switch (element)
                                        {
                                            #region HPGL2
                                            case "hpgl2":
                                                {
                                                    stack.Push(current);
                                                    current = element;
													hpgl2 = new HPGL2Document();
                                                    break;
                                                }
                                            case "page":
                                                {
                                                    stack.Push(current);
                                                    current = element;
                                                    page = new Page(hpgl2);

                                                    

                                                  break;
                                                }
                                            case "pen":
                                                {
                                                    stack.Push(current);
                                                    current = element;
                                                    pen = new Pen(hpgl2);

                                                    if (xmlReader.HasAttributes == true)
                                                    {
                                                        while (xmlReader.MoveToNextAttribute())
                                                        {
                                                            text = xmlReader.Value.ToLower();
                                                            switch (xmlReader.Name.ToLower())
                                                            {
                                                                case "id":
                                                                    {
                                                                        try
                                                                        {
                                                                            pen.Id = Convert.ToInt32(text);
                                                                        }
                                                                        catch { }
                                                                        break;
                                                                    }
                                                            }
                                                        }
                                                    }

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
                                        Trace.TraceInformation(Level(level) + "</" + element + ">");
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
                                            case "pen":
                                                {
                                                    hpgl2.Pens.Add(pen.Id,pen);
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
                                        Trace.TraceInformation(Level(level) + text);

                                        switch (current)
                                        {
                                            case "bottom":
                                                {
                                                    try
                                                    {
                                                        page.Bottom = Convert.ToInt32(text);
                                                    }
                                                    catch { }
                                                    break;
                                                }
                                            case "id":
                                                {
                                                    try
                                                    {
                                                        pen.Id = Convert.ToInt32(text);
                                                    }
                                                    catch { }
                                                    break;
                                                }
                                            case "left":
                                                {
                                                    try
                                                    {
                                                        page.Left = Convert.ToInt32(text);
                                                    }
                                                    catch { }
                                                    break;
                                                }
                                            case "length":
                                                {
                                                    try
                                                    {
                                                        page.Length = Convert.ToInt32(text);
                                                    }
                                                    catch { }
                                                    break;
                                                }
                                            case "width":
                                                {
                                                    if (stack.Peek() == "page")
                                                    {
                                                        try
                                                        {
                                                            page.Width = Convert.ToInt32(text);
                                                        }
                                                        catch { }
                                                    }
                                                    else if (stack.Peek() == "pen")
                                                    {
                                                        try
                                                        {
                                                            pen.PenWidth.Width = Convert.ToInt32(text);
                                                        }
                                                        catch { }
                                                    }
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
                                    Trace.TraceInformation(xmlReader.NodeType.ToString());
                                    break;

                            }
                        }

                        xmlReader.Close();  // Force the close
                        xmlReader = null;
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning("XML Error " + ex.Message);
                    }
                    fs.Close();
                    fs.Dispose();   // Force the dispose as it was getting left open
                }
                catch (FileNotFoundException ex)
                {
                    Trace.TraceWarning("File Error " + ex.Message);
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("File Error " + ex.Message);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Other Error " + e.Message);
            }

            Debug.WriteLine("Out FromXML()");
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
