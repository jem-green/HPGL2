using Microsoft.Extensions.Logging;
using System;
using System.Security;

namespace HPGL2Library
{
    internal class PlotSize : Instruction
    {
        // PS length[,width][;]
        // pS [;]

        /*
         * length and width are in plotter units
         * potentiall see how this can link to the page object.
         */


        int _length = 0; // Needs to default to hard clip limits so pass
        int _width = 0;


        public PlotSize(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
        }

        public PlotSize(int length, int width)
        {
            _length = length;
            _width = width;
        }

        public int Length
        {
            get
            {
                return (_length);
            }
            set
            {
                _length = value;
            }
        }

        public int Width
        {
            get
            {
                return (_width);
            }
            set
            {
                _width = value;
            }
        }
        public override int Read()
        {
            int read = 0;
            if (!_hpgl2.Match(';') == true)
            {
                _length = _hpgl2.getInt();
                if (_hpgl2.Match(','))
                {
                    _hpgl2.getChar();
                    _width = _hpgl2.getInt();
                }
                _hpgl2.Logger.LogDebug("PS length=" + _length + " width=" + _width);
            }
            if (_hpgl2.Match(';') == true)
            {
                _hpgl2.getChar();   // Consume the terminator if it exists
            }
            return (read);
        }
    }
}
