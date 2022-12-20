using System;
using System.Security;

namespace HPGL2Library
{
    internal class HPGL2TransparencyMode : Instruction
    {
        // TR mode[;]
        // TR[;]

        TransparencyMode _mode = TransparencyMode.On; //false - default, true - All
        public enum TransparencyMode : int
        {
            None = -1,
            Off = 0,
            On = 1
        }

        public HPGL2TransparencyMode(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
        }

        public TransparencyMode Mode
        {
            get
            {
              return (_mode);
            }
            set
            {
                _mode = value;
            }
        }

        public override int Read()
        {
            int read = 0;
            if ((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9'))
            {
                _mode = (HPGL2TransparencyMode.TransparencyMode)_hpgl2.getInt();
            }
            return (read);
        }
    }
}
