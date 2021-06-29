using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;
using TracerLibrary;

namespace HPGL2Library
{
    public class HPGL2Document
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
        List<Line> _lines = new List<Line>();       // Initialise here
        Pen _currentPen;                            // Pen status, assuming that there is only one pen

        // Storage

        List<object> _instructions = new List<object>();

        #endregion
        #region Constructors

        public HPGL2Document()
        {
            _lineTypes = new List<UserDefinedLinetype>();
            _pens = new Dictionary<int, Pen>();
            _page = new Page(this);
            _currentPen = new Pen(this);
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
                    Trace.TraceError("Exception:" + ex.ToString());
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

            BeginPlot beginPlot;
            Initialise initialise;
            PenWidthUnit widthUnit;
            NumberPens numberPens;
            QualityLevel qualityLevel;
            PlotSize plotSize;
            Rotate rotate;
            Scale scale;
            Input input;
            InputRelative inputRelative;
            MergeControl mergeControl;
            EnableCutter enableCutter;
            LineAttributes lineAttributes;
            UserDefinedLinetype userDefinedLinetype;
            Transparency transparency;
            SelectPen selectPen;
            PenUp penUp;
            PenDown penDown;
            PlotAbsolute plotAbsolute;
            PenWidth penWidth;
            PolylineEncoded polylineEncoded;

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
                            beginPlot = new BeginPlot(this);
                            beginPlot.Read();
                            _instructions.Add(beginPlot);
                            break;
                        }
                    case "EC": // Enable cutter
                        {
                            enableCutter = new EnableCutter(this);
                            enableCutter.Read();
                            _instructions.Add(enableCutter);
                            break;
                        }
                    case "IN": // Initialise
                        {
                            initialise = new Initialise(this);
                            initialise.Read();
                            _instructions.Add(initialise);
                            break;
                        }
                    case "IP": // Input P1 and P2
                        {
                            input = new Input(this);
                            input.Read();
                            _instructions.Add(input);
                            break;
                        }
                    case "IR": // Input relative P1 and P2
                        {
                            inputRelative = new InputRelative(this);
                            inputRelative.Read();
                            _instructions.Add(inputRelative);
                            break;
                        }
                    case "LA": // Line Attributes
                        {
                            lineAttributes = new LineAttributes(this);
                            lineAttributes.Read();
                            _instructions.Add(lineAttributes);
                            break;
                        }
                    case "MC": // Merge control
                        {
                            mergeControl = new MergeControl(this);
                            mergeControl.Read();
                            _instructions.Add(mergeControl);
                            break;
                        }
                    case "NP": // Number of Pens
                        {
                            numberPens = new NumberPens(this);
                            numberPens.Read();
                            _instructions.Add(numberPens);
                            break;
                        }
                    case "PA": // Plot Absolute
                        {
                            plotAbsolute = new PlotAbsolute(this);
                            plotAbsolute.Read();
                            _instructions.Add(plotAbsolute);
                            break;
                        }
                    case "PD": // Pen Down
                        {
                            penDown = new PenDown(this);
                            penDown.Read();
                            _instructions.Add(penDown);
                            break;
                        }
                    case "PE": // Polyline Encoded
                        {
                            polylineEncoded = new PolylineEncoded(this);
                            polylineEncoded.Read();
                            _instructions.Add(polylineEncoded);
                            break;
                        }
                    case "PG": // Advance Full Page
                        {
                            polylineEncoded = new PolylineEncoded(this);
                            polylineEncoded.Read();
                            _instructions.Add(polylineEncoded);
                            break;
                        }

                    case "PS": // Plot Size
                        {
                            plotSize = new PlotSize(this);
                            plotSize.Read();
                            _instructions.Add(plotSize);
                            break;
                        }
                    case "PU":  // Pen Up
                        {
                            penUp = new PenUp(this);
                            penUp.Read();
                            _instructions.Add(penUp);
                            break;
                        }
                    case "PW":  // Pen Width
                        {
                            penWidth = new PenWidth(this);
                            penWidth.Read();
                            _instructions.Add(penWidth);
                            break;
                        }
                    case "QL": // Quality Level
                        {
                            qualityLevel = new QualityLevel();
                            qualityLevel.Level = getInt();
                            _instructions.Add(qualityLevel);
                            break;
                        }
                    case "RO":  // Rotate
                        {
                            rotate = new Rotate(this);
                            rotate.Read();
                            _instructions.Add(rotate);
                            break;
                        }
                    case "SC":  // Scale
                        {
                            scale = new Scale(this);
                            scale.Read();
                            _instructions.Add(scale);
                            break;
                        }
                    case "SP":  // Selection Pen
                        {
                            selectPen = new SelectPen(this);
                            selectPen.Read();
                            _instructions.Add(selectPen);
                            break;
                        }
                    case "TR":  // Transparency Mode
                        {
                            transparency = new Transparency(this);
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
                            widthUnit = new PenWidthUnit(this);
                            widthUnit.Read();
                            _instructions.Add(widthUnit);
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

        internal void GetChar()
        {
            if (_count <= _data.Length)
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
