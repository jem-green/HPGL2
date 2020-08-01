using System;
using System.Security;

namespace HPGL2Library
{
    public class AdvanceFullPage : Instruction
    {
        // PG pen[;]
        // PG [;]

        AdvanceType _advance = AdvanceType.Plotted;
        public enum AdvanceType : int
        {
            None = -1,
            Plotted = 1,
            Any = 2,
        }

        public AdvanceFullPage(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
        }

        public AdvanceFullPage(AdvanceType advance)
        {
            _advance = advance;
        }

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

        public override int Read()
        {
            int read = 0;
            if (!_hpgl2.Match(';') == true)
            {
                if ((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9'))
                {
                    _advance = (AdvanceType)_hpgl2.getInt();
                    if (_hpgl2.Match(';') == true)
                    {
                        _hpgl2.getChar();   // Consume the terminator if it exists
                    }
                }
            }
            else
            {
                _hpgl2.getChar();
            }
            return (read);
        }
    }
}
