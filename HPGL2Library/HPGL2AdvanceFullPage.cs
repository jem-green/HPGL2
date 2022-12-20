using System;
using System.Security;
using TracerLibrary;

namespace HPGL2Library
{
    public class HPGL2AdvanceFullPage : Instruction
    {
        // Advance Full Page
        // PG advance[;]
        // PG [;]

        #region Fields

        AdvanceType _advance = AdvanceType.Plotted;
        public enum AdvanceType : int
        {
            None = -1,
            Plotted = 1,
            Any = 2,
        }

        #endregion
        #region Constructor

        public HPGL2AdvanceFullPage(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "AdvanceFullPage";
            _instruction = "PG";
            TraceInternal.TraceInformation(_name);
        }

        #endregion
        #region Properties

        public AdvanceType Advance
        {
            get
            {
              return (_advance);
            }
            set
            {
                _advance = value;
            }
        }

        #endregion
        #region Methods

        public override int Read()
        {
            int read = 0;
            if (!_hpgl2.Match(';') == true)
            {
                if ((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9'))
                {
                    _advance = (AdvanceType)_hpgl2.getInt();
                    TraceInternal.TraceVerbose(_name + " " + _advance);
                    TraceInternal.TraceInformation(_instruction + (int)_advance + ";");
                }
            }
            else
            {
                TraceInternal.TraceInformation(_instruction + ";");
            }

            if (_hpgl2.Match(';') == true)
            {
                _hpgl2.GetChar();   // Consume the terminator if it exists
            }

            return (read);
        }
    }
    #endregion
}
