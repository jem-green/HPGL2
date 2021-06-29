using TracerLibrary;
using System;
using System.Security;
using System.Diagnostics;

namespace HPGL2Library
{
    public class PenWidth : Instruction
    {
        // PW width[,pen][;]
        // PW [;]

        double _width = 0;
        int _pen = 0;


        public PenWidth(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "PenWidth ";
            _instruction = "PW";
            Trace.TraceInformation(_name);
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
                        _hpgl2.GetChar();
                        _width = _hpgl2.getDouble();
                        if (_hpgl2.Match(','))
                        {
                            _hpgl2.GetChar();
                            _pen = _hpgl2.getInt();
                            TraceInternal.TraceVerbose(_name + "Pen=" + _pen + "Width=" + _width);
                            TraceInternal.TraceVerbose(_instruction + _pen + "," + _width + ";");
                            if ((_pen >= 0) && (_pen <= _hpgl2.Pens.Count))
                            {
                                _hpgl2.Pens[_pen].PenWidth.Width = _width;
                            }
                        }
                        else
                        {
                            // not sure if it apples the pen width to all pens of just the
                            // current pen. Says both pens **can a plotter only have two** pens?
                            // if that is the case then need to iterate through the pens.
                            TraceInternal.TraceVerbose(_name + "Width=" + _width);
                            TraceInternal.TraceVerbose(_instruction + _width + ";");
                            _hpgl2.Pen.PenWidth.Width = _width;
                        }
                    } while (((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9')) || (_hpgl2.Char == ','));
                }
            }
            return (read);
        }

        #endregion

    }
}
