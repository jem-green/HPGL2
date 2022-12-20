using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;
using TracerLibrary;
using GcodeLibrary;
using DXFLibrary;
using ShapeLibrary;

namespace HPGL2Library
{
    public class HPGL2Document : IDisposable
    {
        #region Fields

        // Data

        private string _filename = "";
        private string _path = "";

        // File

        private char _look;
        private int _count = 0;
        private byte[] _data = new byte[1024 * 8];

        // consider the page size, pens, linetypes

        Page _page;                                 // Page dimensions
        List<UserDefinedLinetype> _lineTypes;       // List of line type **check if there are standard ones**
        Dictionary<int,Pen> _pens;                  // List of pens
        Point _current = new Point(0, 0);           // Assume that this defauts to zero **check**
        List<Line> _lines;                          // List of lines
        Pen _currentPen;                            // Pen status, assuming that there is only one pen

        // Storage

        List<object> _instructions = new List<object>();
        private bool disposedValue;

        #endregion
        #region Constructors

        public HPGL2Document()
        {
            _lineTypes = new List<UserDefinedLinetype>();
            _pens = new Dictionary<int, Pen>();
            _page = new Page(this);
            _currentPen = new Pen(this);
            List<Line> _lines = new List<Line>();
        }

        #endregion
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Page Page
        {
            get
            {
                return (_page);
            }
            set
            {
                _page = value;
            }
        }

        public Pen Pen
        {
            get
            {
                return (_currentPen);
            }
            set
            {
                _currentPen = value;
            }
        }

        public Dictionary<int,Pen> Pens
        {
            get
            {
                return (_pens);
            }
            set
            {
                _pens = value;
            }
        }

        public List<Line> Lines
        {
            get
            {
                return (_lines);
            }
        }

        public Point Current
        {
            get
            {
                return (_current);
            }
            set
            {
                _current = value;
            }
        }

        public char Char
        {
            get
            {
                return (_look);
            }
        }

        #endregion
        #region Methods

        public Gcode ToGCode()
        {
            Gcode gcode = new Gcode();
            return (gcode);
        }

        public SHPDocument ToShape()
        {
            SHPDocument shape = new SHPDocument();
            foreach(Line line in _lines)
            {
                SHPLine l = new SHPLine();
                shape.AddLine(new SHPLine(line.X1, line.Y1,0,line.X2, line.Y2,0));
            }
            return (shape);
        }

        public void FromDXF(DXFDocument dXFDocument)
        {
            
        }

        public void Read()
        {
            Read(_path, _filename);
        }

        /// <summary>
        /// Read in the plot file
        /// </summary>
        public bool Read(string path, string filename)
        {
            bool read = false;
            Debug.WriteLine("In Read()");

            string filenamePath = path.ToString() + Path.DirectorySeparatorChar + filename + ".plt";

            if (File.Exists(filenamePath) == true)
            {
                try
                {
                    TraceInternal.TraceVerbose(filename + " added");
                    FileStream fs = new FileStream(filenamePath, FileMode.Open);
                    fs.Seek(0, SeekOrigin.Begin);
                    _data = new byte[fs.Length];

                    // Read and verify the data.
                    for (int i = 0; i < fs.Length; i++)
                    {
                        _data[i] = (byte)fs.ReadByte();
                    }
                    read = true;
                    fs.Close();
                    fs.Dispose();
                }
                catch (Exception ex)
                {
                    TraceInternal.TraceVerbose("Could not add file " + filename);
                    TraceInternal.TraceError("Exception:" + ex.ToString());
                }
            }
            Debug.WriteLine("Out Read()");
            return (read);
        }

        /// <summary>
        /// Process the plot file
        /// </summary>
        public void Process()
        {
            // Read in plot data

            Debug.WriteLine("In Process()");

            HPGL2AdvanceFullPage advanceFullPage;
            HPGL2BeginPlot beginPlot;
            HPGL2Initialise initialise;
            HPGL2PenWidthUnit widthUnit;
            HPGL2NumberOfPens numberPens;
            HPGL2QualityLevel qualityLevel;
            HPGL2PlotSize plotSize;
            HPGLRotate rotate;
            HPL2Scale scale;
            HPGL2Input input;
            HPGL2InputRelative inputRelative;
            HPGL2MergeControl mergeControl;
            HPGL2EnableCutter enableCutter;
            HPGL2LineAttributes lineAttributes;
            UserDefinedLinetype userDefinedLinetype;
            HPGL2TransparencyMode transparency;
            HPL2SelectPen selectPen;
            HPGL2PenUp penUp;
            HPGL2PenDown penDown;
            HPGL2PlotAbsolute plotAbsolute;
            HPGL2PenWidth penWidth;
            HPGL2PolylineEncoded polylineEncoded;

            GetChar();
            do
            {
                string instruction = "";

                if ((_look >= 'A') && (_look <= 'Z'))
                {
                    instruction = _look.ToString();
                    GetChar();
                    if ((_look >= 'A') && (_look <= 'Z'))
                    {
                        instruction = instruction + _look.ToString();
                        GetChar();
                    }
                }
                else if (_look == '')
                {
                    // Escape codes
                    GetChar();
                    if (((_look >= 'A') && (_look <= 'Z')) || (_look == '%'))
                    {
                        instruction = _look.ToString();
                        GetChar();
                    }
                }

                TraceInternal.TraceVerbose(instruction + "->");

                switch (instruction)
                {
                    case "E":   // (Esc)E - Reset
                        {
                            //resetDevice = new ResetDevice();
                            break;
                        }
                    case "&":   // (Esc)%
                        {
                            // I2A
                            // I0O
                            // I0E
                            break;
                        }
                    case "%":
                        {
                            //0B - Enter HP-GL/2 mode
                            switch (_look)
                            {
                                case '0':
                                case '1':
                                    {
                                        GetChar();
                                        GetChar();
                                        break;
                                    }
                            }
                            //1B
                            break;
                        }
                    case "*":   // (Esc)*
                        {
                            //p0x0Y
                            //c5760x7920Y
                            //*c0T
                            break;
                        }
                    case "BP": // Begin Plot
                        {
                            beginPlot = new HPGL2BeginPlot(this);
                            beginPlot.Read();
                            _instructions.Add(beginPlot);
                            break;
                        }
                    case "EC": // Enable cutter
                        {
                            enableCutter = new HPGL2EnableCutter(this);
                            enableCutter.Read();
                            _instructions.Add(enableCutter);
                            break;
                        }
                    case "IN": // Initialise
                        {
                            initialise = new HPGL2Initialise(this);
                            initialise.Read();
                            _instructions.Add(initialise);
                            break;
                        }
                    case "IP": // Input P1 and P2
                        {
                            input = new HPGL2Input(this);
                            input.Read();
                            _instructions.Add(input);
                            break;
                        }
                    case "IR": // Input relative P1 and P2
                        {
                            inputRelative = new HPGL2InputRelative(this);
                            inputRelative.Read();
                            _instructions.Add(inputRelative);
                            break;
                        }
                    case "LA": // Line Attributes
                        {
                            lineAttributes = new HPGL2LineAttributes(this);
                            lineAttributes.Read();
                            _instructions.Add(lineAttributes);
                            break;
                        }
                    case "MC": // Merge control
                        {
                            mergeControl = new HPGL2MergeControl(this);
                            mergeControl.Read();
                            _instructions.Add(mergeControl);
                            break;
                        }
                    case "NP": // Number of Pens
                        {
                            numberPens = new HPGL2NumberOfPens(this);
                            numberPens.Read();
                            _instructions.Add(numberPens);
                            break;
                        }
                    case "PA": // Plot Absolute
                        {
                            plotAbsolute = new HPGL2PlotAbsolute(this);
                            plotAbsolute.Read();
                            _instructions.Add(plotAbsolute);
                            break;
                        }
                    case "PD": // Pen Down
                        {
                            penDown = new HPGL2PenDown(this);
                            penDown.Read();
                            _instructions.Add(penDown);
                            break;
                        }
                    case "PE": // Polyline Encoded
                        {
                            polylineEncoded = new HPGL2PolylineEncoded(this);
                            polylineEncoded.Read();
                            _instructions.Add(polylineEncoded);
                            break;
                        }
                    case "PG":  // PG Advance Full Page
                        {
                            advanceFullPage = new HPGL2AdvanceFullPage(this);
                            advanceFullPage.Read();
                            _instructions.Add(advanceFullPage);
                            break;
                        }
                    case "PS": // Plot Size
                        {
                            plotSize = new HPGL2PlotSize(this);
                            plotSize.Read();
                            _instructions.Add(plotSize);
                            break;
                        }
                    case "PU":  // Pen Up
                        {
                            penUp = new HPGL2PenUp(this);
                            penUp.Read();
                            _instructions.Add(penUp);
                            break;
                        }
                    case "PW":  // Pen Width
                        {
                            penWidth = new HPGL2PenWidth(this);
                            penWidth.Read();
                            _instructions.Add(penWidth);
                            break;
                        }
                    case "QL": // Quality Level
                        {
                            qualityLevel = new HPGL2QualityLevel(this);
                            qualityLevel.Read();
                            _instructions.Add(qualityLevel);
                            break;
                        }
                    case "RO":  // Rotate
                        {
                            rotate = new HPGLRotate(this);
                            rotate.Read();
                            _instructions.Add(rotate);
                            break;
                        }
                    case "SC":  // Scale
                        {
                            scale = new HPL2Scale(this);
                            scale.Read();
                            _instructions.Add(scale);
                            break;
                        }
                    case "SP":  // Selection Pen
                        {
                            selectPen = new HPL2SelectPen(this);
                            selectPen.Read();
                            _instructions.Add(selectPen);
                            break;
                        }
                    case "TR":  // Transparency Mode
                        {
                            transparency = new HPGL2TransparencyMode(this);
                            transparency.Read();
                            _instructions.Add(transparency);
                            break;
                        }
                    case "UL":  // User-Defined Linetype
                        {
                            userDefinedLinetype = new UserDefinedLinetype(this);
                            userDefinedLinetype.Read();
                            _instructions.Add(userDefinedLinetype);
                            break;
                        }
                    case "WU": // Pen Width Unit
                        {
                            widthUnit = new HPGL2PenWidthUnit(this);
                            widthUnit.Read();
                            _instructions.Add(widthUnit);
                            break;
                        }
                    case "":
                        {
                            // skip
                            break;
                        }
                    default:
                        {
                            throw new Exception("Unknown instruction " + instruction);
                        }
                }
                
            } while (_count < _data.Length);

            Debug.WriteLine("Out Process()");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~HPGL2Document()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        #endregion
        #region Internal

        internal int getInt()
        {
            int data = 0;
            string value = "";
            do
            {
                if (((_look >= '0') && (_look <= '9')) || (_look == '-'))
                {
                    value = value + _look.ToString();
                    GetChar();
                }
            } while (((_look >= '0') && (_look <= '9')) || (_look == '-'));
            if (value.Length > 0)
            {
                data = Convert.ToInt32(value);
            }
            return (data);
        }

        internal double getDouble()
        {
            double data = 0;
            string value = "";
            do
            {
                if (((_look >= '0') && (_look <= '9')) || (_look == '.') || (_look == '-'))
                {
                    value = value + _look.ToString();
                    GetChar();
                }
            } while (((_look >= '0') && (_look <= '9')) || (_look == '.') || (_look == '-'));
            if (value.Length > 0)
            {
                data = Convert.ToDouble(value);
            }
            return (data);
        }


        internal string GetQuotedString()
        {
            string data = "";
            if (_look == '"')
            {
                do
                {
                    data = data + _look.ToString();
                    GetChar();
                } while ((_look != '"') || (_count < _data.Length));
            }
            return (data);
        }

        internal void GetChar()
        {
            if (_count < _data.Length)
            {

                _look = Convert.ToChar(_data[_count]);
                _count++;
            }
            else
            {
                _look = (char)0;
            }
        }


        internal bool Match(char x)
        {
            if (_look == x)
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        #endregion
    }
}
