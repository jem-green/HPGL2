using TracerLibrary;
using System;
using System.Security;
using System.Diagnostics;

namespace HPGL2Library
{
    internal class SelectPen : Instruction
    {
        // SP pen[;]
        // SP [;]

        int _pen = 0;

        public SelectPen(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "SelectPen ";
            _instruction = "SP";
            Trace.TraceInformation(_name);
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
                    TraceInternal.TraceVerbose(_name + "Pen=" + _pen);
                    Trace.TraceInformation(_instruction + _pen + ";");

                    // Select the pen or could just set and index.

                    if ((_pen >= 0) && (_pen <= _hpgl2.Pens.Count))
                    {
                        _hpgl2.Pen = _hpgl2.Pens[_pen];
                    }

                    if (_hpgl2.Match(';') == true)
                    {
                        _hpgl2.GetChar();   // Consume the terminator if it exists
                    }
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
