using System;
using System.Diagnostics;
using System.Security;
using TracerLibrary;

namespace HPGL2Library
{
    internal class InputRelative : Instruction
    {
        // IR x1, y1[ ,x2, y2][;]
        // IR[;]

        // Assuming that the location of p1 and p2 are in plotter units

        private int _x1;
        private int _y1;
        private int _x2;
        private int _y2;

        private Point _p1;
        private Point _p2;

        public InputRelative(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _p1 = new Point();
            _p1 = new Point();
            _name = "InputRelative ";
            _instruction = "IR";
            Trace.TraceInformation(_name);
        }

        public Point P1
        {
            get
            {
                return (_p1);
            }
            set
            {
                _p1 = value;
            }
        }

        public Point P2
        {
            get
            {
                return (_p2);
            }
            set
            {
                _p2 = value;
            }
        }

        public int X1
        {
            get
            {
                return (_p1.X);
            }
            set
            {
                _p1.X = value;
            }
        }

        public int Y1
        {
            get
            {
                return (_p1.Y);
            }
            set
            {
                _p1.Y = value;
            }
        }

        public int X2
        {
            get
            {
                return (_p2.X);
            }
            set
            {
                _p2.X = value;
            }
        }

        public int Y2
        {
            get
            {
                return (_p2.Y);
            }
            set
            {
                _p2.Y = value;
            }
        }

        public override int Read()
        {
            // Need to convert the page size in mm to page size in plotter untis
            // Perhaps this shoudl be a property of the page.

            int pageLeft = (int)(_hpgl2.Page.Left / _hpgl2.Page.Units);
            int pageBottom = (int)(_hpgl2.Page.Bottom / _hpgl2.Page.Units);
            int pageWidth = (int)(_hpgl2.Page.Width / _hpgl2.Page.Units);
            int pageLength = (int)(_hpgl2.Page.Length / _hpgl2.Page.Units);

            // These dimensions probably should come from the PageSize

            pageLeft = 0;
            pageBottom = 0;
            pageWidth = _hpgl2.Page.Size.Width;
            pageLength = _hpgl2.Page.Size.Length;

            int read = 0;
            if (!_hpgl2.Match(';') == true)
            {
                _hpgl2.GetChar();
                _x1 = _hpgl2.getInt();
                if (_hpgl2.Match(','))
                {
                    _hpgl2.GetChar();
                    _y1 = _hpgl2.getInt();
                    TraceInternal.TraceVerbose(_name + "X1%=" + _x1 + " Y1%=" + _y1);
                    // Need to calcualte P1 from the scaling %
                    _p1.X = pageLeft + pageWidth * _x1 / 100;
                    _p1.Y = pageBottom + pageLength * _y1 / 100;
                    TraceInternal.TraceVerbose(_name + "P1 X1=" + _p1.X + " Y1=" + _p1.Y);
                    

                    if (_hpgl2.Match(','))
                    {
                        _hpgl2.GetChar();
                        _x2 = _hpgl2.getInt();
                        if (_hpgl2.Match(','))
                        {
                            _hpgl2.GetChar();
                            _y2 = _hpgl2.getInt();
                            TraceInternal.TraceVerbose(_name + "X2=" + _x2 + " Y2=" + _y2);

                            // Need to calcualte P1 from the scaling %
                            _p2.X = pageLeft + pageWidth * _x2 / 100;
                            _p2.Y = pageBottom + pageLength * _y2 / 100;
                            TraceInternal.TraceVerbose(_name + "P2 X2=" + _p2.X + " Y2=" + _p2.Y);
                            Trace.TraceInformation(_instruction + _x1 + "," + _y1 + "," + _x2 + "," + _y2 + ";");
                        }
                        else
                        {
                            // Only 3 parameters
                            read = 2;
                            throw new Exception("bad sytax");
                        }
                    }
                    else
                    {
                        //Not sure how the tracking works as descibed, it would imply
                        //that this is the distance from P1 to P2 in plotter untis.

                        int width = _hpgl2.Page.Input.P2.X - _hpgl2.Page.Input.P1.X;
                        int length = _hpgl2.Page.Input.P2.Y - _hpgl2.Page.Input.P1.Y;
                        _p2.X = _p1.X + width;
                        _p2.Y = _p1.Y + length;
                        TraceInternal.TraceVerbose(_name + "P2 X2=" + _p2.X + " Y2=" + _p2.Y);
                        Trace.TraceInformation(_instruction + _x1 + "," + _y1 + ";");
                    }
                    _hpgl2.Page.Input.P1 = _p1;
                    _hpgl2.Page.Input.P2 = _p2;
                    TraceInternal.TraceVerbose(_name + _p1 + "," + _p2);
                }
                else
                {
                    // Only 1 parameter
                    read = 1;
                    throw new Exception("bad sytax");
                }
            }
            else
            {
                _hpgl2.GetChar();
            }
            return (read);
        }
    }
}
