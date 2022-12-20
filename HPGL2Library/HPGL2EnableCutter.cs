using TracerLibrary;
using System;
using System.Security;
using System.Diagnostics;

namespace HPGL2Library
{
    public class HPGL2EnableCutter : Instruction
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

        public HPGL2EnableCutter(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "EnableCutter ";
            _instruction = "EC";
            TraceInternal.TraceInformation(_name);
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
                    TraceInternal.TraceVerbose(_name + _mode.ToString());
                    _mode = CutterMode.off;
                    _hpgl2.GetChar();
                }
            }
            TraceInternal.TraceInformation(_instruction + (int)_mode + ";");
            if (_hpgl2.Match(';') == true)
            {
                _hpgl2.GetChar();   // Consume the terminator if it exists
            }
            return (read);
        }
    }
}
