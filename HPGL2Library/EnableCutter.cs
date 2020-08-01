using System;
using System.Security;

namespace HPGL2Library
{
    public class EnableCutter : Instruction
    {
        // EC mode[;]
        // EC[;]

        CutterMode _mode = CutterMode.on; //defaults to on
        public enum CutterMode : int
        {
            None = -1,
            off = 0,
            on = 1
        }

        public EnableCutter(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
        }

        public EnableCutter(CutterMode mode)
        {
            _mode = mode;
        }

        public CutterMode Mode
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
            if (!_hpgl2.Match(';') == true)
            {
                if ((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9'))
                {
                    // any value turns the cutter off
                    _mode = CutterMode.off;
                    _hpgl2.getChar();
                }
            }
            return (read);
        }
    }
}
