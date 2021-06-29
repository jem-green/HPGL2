using TracerLibrary;
using System;
using System.Security;
using System.Diagnostics;

namespace HPGL2Library
{
    public class PlotSize : Instruction
    {
        // PS length[,width][;]
        // pS [;]

        /*
         * length and width are in plotter units
         * potentiall see how this can link to the page object.
         */


        int _length = 0; // Needs to default to hard clip limits so pass
        int _width = 0;


        public PlotSize(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "PlotSize ";
            _instruction = "PS";
            Trace.TraceInformation(_name);
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
                    _hpgl2.GetChar();
                    _width = _hpgl2.getInt();
                }
                TraceInternal.TraceVerbose(_name + "length=" + _length + " width=" + _width);
                Trace.TraceInformation(_instruction + _length + "," + _width + ";");

                _hpgl2.Page.Size = this;

                // Should this override the physical page
                // Need to convert the plotter size to physical size

                _hpgl2.Page.Left = 0;
                _hpgl2.Page.Bottom = 0;
                _hpgl2.Page.Length = (int)(_length * _hpgl2.Page.Units);
                _hpgl2.Page.Width = (int)(_width * _hpgl2.Page.Units);

                // Set P1 amd P2 to the plot size in plotter units

                Point p1 = new Point(0, 0);
                Point p2 = new Point(_width, _length);
                _hpgl2.Page.Input.P1 = p1;
                _hpgl2.Page.Input.P2 = p2;
                TraceInternal.TraceVerbose(_name + "P1=" + p1 + " P2=" + p2);

                // Defautl cursor to P1

                _hpgl2.Current = p1;

            }
            if (_hpgl2.Match(';') == true)
            {
                _hpgl2.GetChar();   // Consume the terminator if it exists
            }
            return (read);
        }
    }
}
