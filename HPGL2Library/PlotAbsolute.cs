using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace HPGL2Library
{
    public class PlotAbsolute : Instruction
    {
        // PU x1, y1[ ,x1, y1][;]
        // PU[;]

        List<CoOrd> _coOrds;

        public PlotAbsolute(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
        _coOrds = new List<CoOrd>();
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
                            _coOrds.Add(coOrd);
                            Line line = new Line(_hpgl2.Current, coOrd);
                            _hpgl2.Logger.LogDebug("Line " + _hpgl2.Current.ToString() + " to " + coOrd.ToString());
                            _hpgl2.Lines.Add(line);
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
