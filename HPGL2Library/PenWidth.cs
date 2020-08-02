using Microsoft.Extensions.Logging;
using System;
using System.Security;

namespace HPGL2Library
{
    internal class PenWidth : Instruction
    {
        // PW width[,pen][;]
        // PW [;]

        double _width = 0;
        int _pen = 0;


        public PenWidth(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
        }

        public PenWidth(double width)
        {
            _width = width;
        }

        public PenWidth(int pen, double width)
        {
            _pen = pen;
            _width = width;
        }

        public double Width
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

        #region Methods

        public override int Read()
        {
            int read = 0;
            if (!_hpgl2.Match(';') == true)
            {
                if ((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9'))
                {
                    do
                    {
                        _hpgl2.getChar();
                        _width = _hpgl2.getDouble();
                        if (_hpgl2.Match(','))
                        {
                            _hpgl2.getChar();
                            _pen = _hpgl2.getInt();
                            _hpgl2.Logger.LogDebug("PW Pen=" + _pen + "Width=" + _width);
                        }
                        else
                        {
                            _hpgl2.Logger.LogDebug("PW Width=" + _width);
                        }
                    } while (((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9')) || (_hpgl2.Char == ','));
                }
            }
            return (read);
        }

        #endregion

    }
}
