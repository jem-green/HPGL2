using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace HPGL2Library
{
    public class PenDown : Instruction
    {
        // PD x1, y1[ ,x1, y1][;]
        // PD[;]

        List<CoOrd> _coOrds;

        public PenDown(HPGL2 hpgl2)
        {
            _coOrds = new List<CoOrd>();
            _hpgl2 = hpgl2;
        }

        public void Add(CoOrd coOrd)
        {
            _coOrds.Add(coOrd);
        }

        public void Add(int x, int y)
        {
            _coOrds.Add(new CoOrd(x,y));
        }

        public override int Read()
        {
            int read = 0;
            _hpgl2.Pen.Status = Pen.PenStatus.Down;
            _hpgl2.Logger.LogDebug("PD " + _hpgl2.Pen.ToString());
            CoOrd coOrd = new CoOrd();
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
                            _hpgl2.Logger.LogDebug("PU " + coOrd.ToString());
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
