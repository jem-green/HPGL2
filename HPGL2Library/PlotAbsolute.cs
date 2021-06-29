using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using TracerLibrary;

namespace HPGL2Library
{
    internal class PlotAbsolute : Instruction
    {
        // PA x1, y1[ ,x1, y1][;]
        // PU[;]

        List<Point> _coOrds;

        public PlotAbsolute(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "PlotAbsolute ";
            _instruction = "PA";
            _coOrds = new List<Point>();
            TraceInternal.TraceVerbose(base._name);
        }

        public override int Read()
        {
            int read = 0;
            Point coOrd = new Point();
            if (!_hpgl2.Match(';') == true)
            {
                if ((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9'))
                {
                    do
                    {
                        _hpgl2.GetChar();
                        coOrd.X = _hpgl2.getInt();
                        if (_hpgl2.Match(','))
                        {
                            _hpgl2.GetChar();
                            coOrd.Y = _hpgl2.getInt();
                            _coOrds.Add(coOrd);
                            
                            if (_hpgl2.Pen.Status == Pen.PenStatus.Down)
                            {
                                Line line = new Line(_hpgl2.Current, coOrd);
                                TraceInternal.TraceVerbose(_name + "Line " + _hpgl2.Current.ToString() + " to " + coOrd.ToString());
                                _hpgl2.Lines.Add(line);
                            }
                            else
                            {
                                TraceInternal.TraceVerbose(_name + "Move " + _hpgl2.Current.ToString() + " to " + coOrd.ToString());
                            }
                            Trace.TraceInformation(_instruction + coOrd.ToString() + ";");

                        }
                        else
                        {
                            throw new Exception("Bad syntax");
                        }

                    } while (((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9')) || (_hpgl2.Char == ','));
                    if (_hpgl2.Match(';') == true)
                    {
                        _hpgl2.GetChar();   // Consume the terminator if it exists
                    }
                }
            }
            return (read);
        }
    }
}
