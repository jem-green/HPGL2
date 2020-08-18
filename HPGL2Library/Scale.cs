using Microsoft.Extensions.Logging;
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
        ScaleType _type = ScaleType.Anisotropic;

        public enum ScaleType : int
        {
            Anisotropic = 0,
            Isotropic = 1,
            Factor = 2
        }

        public Scale(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "Scale";
            _instruction = "SC";
            _hpgl2.Logger.LogInformation(_name);
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

        public ScaleType Type
        {
            get
            {
                return (_type);
            }
            set
            {
                _type = value;
            }
        }


        public override int Read()
        {
            // The challenge here is its difficult to match the parameters
            // before reading the data to the end none numeric terminator.

            int read = 0;
            if (!_hpgl2.Match(';'))
            {
                _hpgl2.getChar();
                _xmin = _hpgl2.getDouble();
                if (_hpgl2.Match(','))
                {
                    _hpgl2.getChar();
                    _xmax = _hpgl2.getDouble();
                    if (_hpgl2.Match(','))
                    {
                        _hpgl2.getChar();
                        _ymin = _hpgl2.getDouble();
                        if (_hpgl2.Match(','))
                        {
                            _hpgl2.getChar();
                            _ymax = _hpgl2.getDouble();
                            _hpgl2.Logger.LogDebug(_name + " xmin=" + _xmin + " xmax=" + _xmax + " ymin=" + _ymin + " ymax=" + _ymax);
                            _hpgl2.Logger.LogInformation(_instruction + _xmin + "," + _xmax + "," + _ymin + "," + _ymax + ";");
                        }
                        else
                        {
                            throw new Exception("bad syntax");
                        }
                    }
                    else
                    {
                        throw new Exception("bad syntax");
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
