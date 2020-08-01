using System;
using System.Security;

namespace HPGL2Library
{
    public class Scale : Instruction
    {
        // SC xmin, xmax, ymin, ymax, [type,[left,bottom]][;]
        // SC xmin, xfactor, ymin, yfactor, type [;]
        // SC [;]

        double _xmin = 0;
        double _ymin = 0;
        double _xmax = 0;
        double _ymax = 0;

        public Scale(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
        }

        public Scale(double xmin, double ymin)
        {
            _xmin = xmin;
            _ymin = ymin;
            _xmax = 0;
            _ymax = 0;
        }

        public Scale(double xmin, double ymin, double xmax, double ymax)
        {
            _xmin = xmin;
            _ymin = ymin;
            _xmax = 0;
            _ymax = 0;
        }

        public double Xmin
        {
            get
            {
                return (_xmin);
            }
            set
            {
                _xmin = value;
            }
        }

        public double Ymin
        {
            get
            {
                return(_xmin);
            }
            set
            {
                _ymin = value;
            }
        }

        public double Xmax
        {
            get
            {
                return (_xmax);
            }
            set
            {
                _xmax = value;
            }
        }

        public double Ymax
        {
            get
            {
                return (_xmax);
            }
            set
            {
                _ymax = value;
            }
        }

        public override int Read()
        {
            int read = 0;
            if (!_hpgl2.Match(';'))
            {
                _hpgl2.getChar();
                _xmin = _hpgl2.getDouble();
                if (_hpgl2.Match(','))
                {
                    _hpgl2.getChar();
                    _xmax = _hpgl2.getDouble();
                    if (!_hpgl2.Match(';'))
                    {
                        _hpgl2.getChar();
                        _ymin = _hpgl2.getDouble();
                        if (_hpgl2.Match(','))
                        {
                            _hpgl2.getChar();
                            _ymax = _hpgl2.getDouble();
                        }
                        else
                        {
                            throw new Exception("bad syntax");
                        }
                    }
                }
                else
                {
                    throw new Exception("bad syntax");
                }
            }
            else
            {
                // Turn off scaling
            }
            if (_hpgl2.Match(';') == true)
            {
                _hpgl2.getChar();   // Consume the terminator if it exists
            }
            return (read);
        }
    }
}
