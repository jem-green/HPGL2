using System;
using System.Security;

namespace HPGL2Library
{
    public class Input : Instruction
    {
        // IP x1, y1[ ,x2, y2][;]
        // IP[;]

        // Assuming that the location of p1 and p2 are in plotter units

        private CoOrd _p1;
        private CoOrd _p2;

        public Input(HPGL2 hpgl2)
        {

            _hpgl2 = hpgl2;
            _p1 = new CoOrd();
            _p1 = new CoOrd();
        }

        public Input(int x1, int y1, int x2, int y2)
        {
            _p1 = new CoOrd(x1,y1);
            _p2 = new CoOrd(x2,y2);
        }

        public Input(int x1, int y1)
        {
            _p1 = new CoOrd(x1, y1);
            _p2 = new CoOrd(0, 0);
        }

        public Input(CoOrd p1, CoOrd p2)
        {
            _p1 = p1;
            _p2 = p2;
        }

        public CoOrd P1
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

        public CoOrd P2
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
                            }
                            else
                            {
                                // Only 3 parameter
                                read = 2;
                                throw new Exception("bad sytax");
                            }
                        }
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
