using System;
using System.Collections.Generic;
using System.Text;
using TracerLibrary;

namespace HPGL2Library
{
    internal class HPGL2AnchorCorner : Instruction
    {
        // Anchor Corner
        // AC X,Y[;]
        // AC [;]

        #region Fields

        // AC X, Y

        Point _point;

        #endregion
        #region Constructors
        public HPGL2AnchorCorner(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "AnchorCorner";
            _instruction = "AC";
            TraceInternal.TraceInformation(_name);
        }

        #endregion
        #region Properties

        Point Anchor
        {
            get
            {
                return(_point);
            }
            set
            {
                _point = value;
            }
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
