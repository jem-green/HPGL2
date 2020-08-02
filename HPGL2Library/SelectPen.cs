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
        }

        public SelectPen(int pen)
        {
            _pen = pen;
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
                    _hpgl2.Logger.LogDebug("SP " + _pen);
                    // **Need to select the pen, im assuming **
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
