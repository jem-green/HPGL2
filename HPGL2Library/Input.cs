using System;
using System.Security;
using Microsoft.Extensions.Logging;

namespace HPGL2Library
{
    public class Input : Instruction
    {
        // IP x1, y1[ ,x2, y2][;]
        // IP[;]

        // Assuming that the location of p1 and p2 are in plotter units

        private Point _p1;
        private Point _p2;

        public Input(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
            _p1 = new Point();
            _p1 = new Point();
            _name = "Input ";
            _instruction = "IP";
            _hpgl2.Logger.LogInformation(_name);
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
            int read = 0;
            if (!_hpgl2.Match(';') == true)
            {
                _p1.X = _hpgl2.getInt();
                if (_hpgl2.Match(','))
                {
                    _hpgl2.getChar();
                    _p1.Y = _hpgl2.getInt();
                    _hpgl2.Logger.LogDebug(_name + "P1 X1=" + _p1.X + " Y1=" + _p1.Y);
                    _hpgl2.Page.Input.P1 = _p1;
                    read = 1;
                    if (!_hpgl2.Match(';') == true)
                    {
                        if (_hpgl2.Match(',') == true)
                        {
                            _hpgl2.getChar();
                            _p2.X = _hpgl2.getInt();
                            if (_hpgl2.Match(','))
                            {
                                _hpgl2.getChar();
                                _p2.Y = _hpgl2.getInt();
                                read = 1;
                                _hpgl2.Logger.LogDebug(_name + "P2 X2=" + _p2.X + " Y2=" + _p2.Y);
                                _hpgl2.Logger.LogInformation(_instruction + _p1 + "," + _p2);
                                _hpgl2.Page.Input.P2 = _p2;
                            }
                            else
                            {
                                // Only 3 parameters
                                read = 2;
                                throw new Exception("bad sytax");
                            }
                        }                      
                    }
                    else
                    {
                        _hpgl2.Logger.LogInformation(_instruction + _p1);
                    }
                }
                else
                {
                    // Only 1 parameter
                    read = 2;
                    throw new Exception("bad sytax");
                }
            }
            if (_hpgl2.Match(';') == true)
            {
                _hpgl2.getChar();   // Consume the terminator if it exists
            }
            return (read);
        }

    }
}
