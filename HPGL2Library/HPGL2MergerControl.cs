using TracerLibrary;
using System;
using System.Security;
using System.Diagnostics;

namespace HPGL2Library
{
    internal class HPGL2MergeControl : Instruction
    {
        // MC mode[, opcode][;]
        // MC [;]

        mergeType _merge = 0;
        int _opcode = 0;

        public enum mergeType : int
        {
            None = -1,
            Off = 0,
            On = 1
        }

        public HPGL2MergeControl(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "MergeControl ";
            _instruction = "MC";
            TraceInternal.TraceInformation(_name);
        }

        public HPGL2MergeControl(mergeType merge, int opcode)
        {
            _merge = (mergeType)merge;
            _opcode = opcode;
        }

        public mergeType Merge
        {
            get
            {
                return (_merge);
            }
            set
            {
                _merge = value;
            }
        }

        public int OpCode
        {
            get
            {
                return(_opcode);
            }
            set
            {
                _opcode = value;
            }
        }
        public override int Read()
        {
            int read = 0;
            if (!_hpgl2.Match(';') == true)
            {
                if ((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9'))
                {
                    _merge = (HPGL2MergeControl.mergeType)_hpgl2.getInt();
                    if (_hpgl2.Match(',') == true)
                    {
                        _hpgl2.GetChar();
                        _opcode = _hpgl2.getInt();
                        TraceInternal.TraceVerbose(_name + _merge);
                        TraceInternal.TraceInformation(_instruction + (int)_merge + "," + _opcode + ";");
                    }
                    else
                    {
                        TraceInternal.TraceVerbose(_name + _merge);
                        TraceInternal.TraceInformation(_instruction + (int)_merge + ";");
                    }
                    if (_hpgl2.Match(';') == true)
                    {
                        _hpgl2.GetChar();   // Consume the terminator if it exists
                    }
                }
                else
                {
                    TraceInternal.TraceInformation(_name + ";");
                }
            }
            return (read);
        }
    }
}
