﻿using TracerLibrary;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace HPGL2Library
{
    internal class HPGL2PenUp : Instruction
    {
        // PU x1, y1[ ,x1, y1][;]
        // PU[;]

        List<Point> _coOrds;

        public HPGL2PenUp(HPGL2Document hpgl2)
        {

            _coOrds = new List<Point>();
            _hpgl2 = hpgl2;
            _name = "PenUp";
            _instruction = "PU";
            TraceInternal.TraceInformation(_name);
        }

        public void Add(Point coOrd)
        {
            _coOrds.Add(coOrd);
        }

        public void Add(int x, int y)
        {
            _coOrds.Add(new Point(x,y));
        }

        public override int Read()
        {
            int read = 0;


            _hpgl2.Pen.Status = Pen.PenStatus.Up;
            TraceInternal.TraceVerbose(_name + "Pen=" + _hpgl2.Pen.Status.ToString());
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
                            // update the current position for the next line segment
                            TraceInternal.TraceVerbose(_name + " X=" + coOrd.X + " Y=" + coOrd.Y);
                            TraceInternal.TraceInformation(_instruction + coOrd.ToString() + ";");
                            _hpgl2.Current = coOrd;
                        }
                        else
                        {
                            throw new Exception("Bad syntax");
                        }

                    } while (((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9')) || (_hpgl2.Char == ','));
                }
                else
                {
                    TraceInternal.TraceInformation(_instruction + ";");
                }
            }
            return (read);
        }
    }
}
