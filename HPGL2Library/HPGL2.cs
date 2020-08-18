using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;

namespace HPGL2Library
{
    public class HPGL2
    {
        private ILogger _logger;

        // Data

        private string _filename = "";
        private string _path = "";

        // File

        private char _look;
        private int _count = 0;
        private string _data = "";

        // consider the page size, pens, linetypes

        Page _page;
        List<UserDefinedLinetype> _lineTypes;       // List of line type **check if there are standard ones**
        Dictionary<int,Pen> _pens;               // List of pens
        Point _current = new Point(0, 0);           // Assume that this defauts to zero **check**
        List<Line> _lines = new List<Line>();       // Initialise here
        Pen _currentPen;                            // Pen status, assuming that there is only one pen

        public HPGL2(ILogger logger)
        {
            //_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (logger != null)
            {
                _logger = logger;
            }
            else
            {
                throw new ArgumentNullException(nameof(logger));
            }
            _lineTypes = new List<UserDefinedLinetype>();
            _pens = new Dictionary<int, Pen>();
            _page = new Page(this);
            _currentPen = new Pen(this);
        }

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

        public ILogger Logger
        {
            get
            {
                return (_logger);
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
            _logger.LogDebug("In Read()");

            string filenamePath = path.ToString() + Path.DirectorySeparatorChar + filename + ".plt";

            if (File.Exists(filenamePath) == true)
            {
                try
                {
                    _logger.LogDebug(filename + " added");
                    FileStream fs = new FileStream(filenamePath, FileMode.Open);
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        _data = sr.ReadToEnd();
                    }
                    read = true;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("Coulnd not add file " + filename);
                    _logger.LogError("Exception:" + ex.ToString());
                }
            }
            _logger.LogDebug("Out Convert()");
            return (read);
        }

        /// <summary>
        /// Process the plot file
        /// </summary>
        public void Process()
        {
            // Read in plot data

            _logger.LogDebug("In Process()");

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

            List<object> instructions = new List<object>();
            getChar();
            do
            {
                string instruction = "";

                if ((_look >= 'A') && (_look <= 'Z'))
                {
                    instruction = _look.ToString();
                    getChar();
                    if ((_look >= 'A') && (_look <= 'Z'))
                    {
                        instruction = instruction + _look.ToString();
                        getChar();
                    }
                }
                else if (_look == '')
                {
                    // Escape codes
                    getChar();
                    if (((_look >= 'A') && (_look <= 'Z')) || (_look == '%'))
                    {
                        instruction = _look.ToString();
                        getChar();
                    }
                }

                _logger.LogDebug(instruction + "->");

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
                                        getChar();
                                        getChar();
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
                            break;
                        }
                    case "EC": // Enable cutter
                        {
                            enableCutter = new EnableCutter(this);
                            enableCutter.Read();
                            break;
                        }
                    case "IN": // Initialise
                        {
                            initialise = new Initialise(this);
                            initialise.Read();
                            break;
                        }
                    case "IP": // Input P1 and P2
                        {
                            input = new Input(this);
                            input.Read();
                            break;
                        }
                    case "IR": // Input relative P1 and P2
                        {
                            inputRelative = new InputRelative(this);
                            inputRelative.Read();
                            break;
                        }
                    case "LA": // Line Attributes
                        {
                            lineAttributes = new LineAttributes(this);
                            lineAttributes.Read();
                            break;
                        }
                    case "MC": // Merge control
                        {
                            mergeControl = new MergeControl(this);
                            mergeControl.Read();
                            break;
                        }
                    case "NP": // Number of Pens
                        {
                            numberPens = new NumberPens(this);
                            numberPens.Read();
                            break;
                        }
                    case "PA": // Plot Absolute
                        {
                            plotAbsolute = new PlotAbsolute(this);
                            plotAbsolute.Read();
                            break;
                        }
                    case "PD": // Pen Down
                        {
                            penDown = new PenDown(this);
                            penDown.Read();
                            break;
                        }
                    case "PE": // Polyline Encoded
                        {
                            polylineEncoded = new PolylineEncoded(this);
                            polylineEncoded.Read();
                            break;
                        }
                    case "PG": // Advance Full Page
                        {
                            polylineEncoded = new PolylineEncoded(this);
                            polylineEncoded.Read();
                            break;
                        }

                    case "PS": // Plot Size
                        {
                            plotSize = new PlotSize(this);
                            plotSize.Read();
                            break;
                        }
                    case "PU":  // Pen Up
                        {
                            penUp = new PenUp(this);
                            penUp.Read();
                            break;
                        }
                    case "PW":  // Pen Width
                        {
                            penWidth = new PenWidth(this);
                            penWidth.Read();
                            break;
                        }
                    case "QL": // Quality Level
                        {
                            qualityLevel = new QualityLevel();
                            qualityLevel.Level = getInt();
                            break;
                        }
                    case "RO":  // Rotate
                        {
                            rotate = new Rotate(this);
                            rotate.Read();
                            break;
                        }
                    case "SC":  // Scale
                        {
                            scale = new Scale(this);
                            scale.Read();
                            break;
                        }
                    case "SP":  // Selection Pen
                        {
                            selectPen = new SelectPen(this);
                            selectPen.Read();
                            break;
                        }
                    case "TR":  // Transparency Mode
                        {
                            transparency = new Transparency(this);
                            transparency.Read();
                            break;
                        }
                    case "UL":  // User-Defined Linetype
                        {
                            userDefinedLinetype = new UserDefinedLinetype(this);
                            userDefinedLinetype.Read();
                            break;
                        }
                    case "WU": // Pen Width Unit
                        {
                            widthUnit = new PenWidthUnit(this);
                            widthUnit.Read();
                            break;
                        }
                    default:
                        {
                            throw new Exception("Unknown instruction " + instruction);
                            break;
                        }
                }
                
            } while (_count < _data.Length);

            _logger.LogDebug("Out Process()");
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
                    getChar();
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
                    getChar();
                }
            } while (((_look >= '0') && (_look <= '9')) || (_look == '.') || (_look == '-'));
            if (value.Length > 0)
            {
                data = Convert.ToDouble(value);
            }
            return (data);
        }

        internal void getChar()
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
