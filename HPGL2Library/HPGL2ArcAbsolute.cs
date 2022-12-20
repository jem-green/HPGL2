using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TracerLibrary;

namespace HPGL2Library
{
    internal class HPGL2ArcAbsolute : Instruction
    {
        // Arc Absolute
        // AA Xcent,Ycent,Sweep Angle[,Chord Angle][;]

        #region Fields

        // AA X, Y, S, C

        Point _point;
        double _sweep;
        double _chord;

        #endregion
        #region Constructors
        public HPGL2ArcAbsolute(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "ArcAbosolute";
            _instruction = "AA";
            TraceInternal.TraceInformation(_name);
        }

        #endregion
        #region Properties

        Point Center
        {
            get
            {
                return (_point);
            }
            set
            {
                _point = value;
            }
        }

        double Sweep
        {
            get { return _sweep; }
            set { _sweep = value; }
        }

        double Chord
        {
            get { return (_chord); }
            set { _chord = value; }
        }

        #endregion
        #region Methods

        public override int Read()
        {
            int read = 0;
            if (_hpgl2.Char != ';')
            {
                _point.X = _hpgl2.getInt();
                if (_hpgl2.Match(','))
                {
                    _point.Y = _hpgl2.getInt();
                }
                else
                {
                    read = 1;
                }
            }

            if (_hpgl2.Match(';') == true)
            {
                _hpgl2.GetChar();   // Consume the terminator if it exists
            }
            return (read);
        }
        #endregion
    }
}
