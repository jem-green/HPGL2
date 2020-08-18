using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace HPGL2Library
{
    internal class PenUp : Instruction
    {
        // PU x1, y1[ ,x1, y1][;]
        // PU[;]

        List<Point> _coOrds;

        public PenUp(HPGL2 hpgl2)
        {

            _coOrds = new List<Point>();
            _hpgl2 = hpgl2;
            _name = "PenUp ";
            _instruction = "PU";
            _hpgl2.Logger.LogInformation(_name);
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
            _hpgl2.Logger.LogDebug(_name + "Pen=" + _hpgl2.Pen.Status.ToString());
            Point coOrd = new Point();
            if (!_hpgl2.Match(';') == true)
            {
                if ((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9'))
                {
                    do
                    {
                        _hpgl2.getChar();
                        coOrd.X = _hpgl2.getInt();
                        if (_hpgl2.Match(','))
                        {
                            _hpgl2.getChar();
                            coOrd.Y = _hpgl2.getInt();
                            // update the current position for the next line segment
                            _hpgl2.Logger.LogDebug(_name + "X=" + coOrd.X + " Y=" + coOrd.Y);
                            _hpgl2.Logger.LogInformation(_instruction + coOrd.ToString() + ";");
                            _hpgl2.Current = coOrd;
                        }
                        else
                        {
                            throw new Exception("Bad syntax");
                        }

                    } while (((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9')) || (_hpgl2.Char == ','));
                }
            }
            return (read);
        }
    }
}
