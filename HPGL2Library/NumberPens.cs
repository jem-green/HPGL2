using TracerLibrary;
using System;
using System.Security;
using System.Diagnostics;

namespace HPGL2Library
{
    internal class NumberPens : Instruction
    {
        // NP mode
        int _pens = 2; // number of pens n = x^2 so 2 4 8 16 32 64

        public NumberPens(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "NumberPens ";
            _instruction = "NP";
            Trace.TraceInformation(_name);
        }

        public NumberPens(int number)
        {
            _pens = number;
        }

        public int Pens
        {
            get
            {
                return (_pens);
            }
            set
            {
                _pens = value;
                //check if minimum base 2
                int check = (int)Math.Log(_pens, 2);
                if (_pens < Math.Pow(2, check))
                {
                    _pens = (int)Math.Pow(2, check);
                }

            }
        }

        public override int Read()
        {
            int read = 0;
            _pens = _hpgl2.getInt();
            return (read);
        }

    }
}
