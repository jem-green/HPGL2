using System;
using System.Security;

namespace HPGL2Library
{
    public class InputRelative : Instruction
    {
        // IR;
        // IR x1, y1[ ,x2, y2]
        int _x1 = 0; // Needs to default to hard clip limits so pass
        int _y1 = 0;
        int _x2 = 0;
        int _y2 = 0;

        public InputRelative(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
        }

        public InputRelative(int x1, int y1, int x2, int y2)
        {
            _x1 = x1;
            _y1 = y1;
            _x2 = x2;
            _y2 = y2;
        }

        public InputRelative(int x1, int y1)
        {
            _x1 = x1;
            _y1 = y1;
            _x2 = 0;
            _y2 = 0;
        }

        public int X1
        {
            get
            {
                return (_x1);
            }
            set
            {
                _x1 = value;
            }
        }

        public int Y1
        {
            get
            {
                return (_y1);
            }
            set
            {
                _y1 = value;
            }
        }

        public int X2
        {
            get
            {
                return (_x2);
            }
            set
            {
                _x2 = value;
            }
        }

        public int Y2
        {
            get
            {
                return (_y2);
            }
            set
            {
                _y2 = value;
            }
        }

        public override int Read()
        {
            int read = 0;
            if (!_hpgl2.Match(';') == true)
            {
                _hpgl2.getChar();
                _x1 = _hpgl2.getInt();
                if (_hpgl2.Match(','))
                {
                    _hpgl2.getChar();
                    _y1 = _hpgl2.getInt();

                    //if (Match(','))
                    //{
                    //    getChar();
                    //    inputRelative.X2 = getInt();
                    //    if (Match(','))
                    //    {
                    //        getChar();
                    //        inputRelative.Y2 = getInt();
                    //    }
                    //}
                }
            }
            else
            {
                _hpgl2.getChar();
            }
            return (read);
        }
    }
}
