using System;
using System.Security;

namespace HPGL2Library
{
    internal class MergeControl : Instruction
    {
        // MC mode[, opcode][;]
        // MC [;]

        mergeType _merge = 0;
        int _opcode = 0;

        public enum mergeType : int
        {
            None = -1,
            On = 1,
            Off = 2,
        }

        public MergeControl(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
        }

        public MergeControl(mergeType merge, int opcode)
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
            _merge = (MergeControl.mergeType)_hpgl2.getInt();
            if (_hpgl2.Match(',') == true)
            {
                _hpgl2.getChar();
                _opcode = _hpgl2.getInt();
            }
            return (read);
        }
    }
}
