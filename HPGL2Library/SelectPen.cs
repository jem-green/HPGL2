using Microsoft.Extensions.Logging;
using System;
using System.Security;

namespace HPGL2Library
{
    internal class SelectPen : Instruction
    {
        // SP pen[;]
        // SP [;]

        int _pen = 0;

        public SelectPen(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "SelectPen ";
            _instruction = "SP";
            _hpgl2.Logger.LogInformation(_name);
        }

        public int Pen
        {
            get
            {
              return (_pen);
            }
            set
            {
                _pen = value;
            }
        }

        public override int Read()
        {
            int read = 0;
            if (!_hpgl2.Match(';') == true)
            {
                if ((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9'))
                {
                    _pen = _hpgl2.getInt();
                    _hpgl2.Logger.LogDebug(_name + "Pen=" + _pen);
                    _hpgl2.Logger.LogInformation(_instruction + _pen + ";");

                    // Select the pen or could just set and index.

                    if ((_pen >= 0) && (_pen <= _hpgl2.Pens.Count))
                    {
                        _hpgl2.Pen = _hpgl2.Pens[_pen];
                    }

                    if (_hpgl2.Match(';') == true)
                    {
                        _hpgl2.getChar();   // Consume the terminator if it exists
                    }
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
