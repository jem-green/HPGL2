using System;
using System.Security;

namespace HPGL2Library
{
    internal class Rotate : Instruction
    {
        // NP mode
        int _angle = 2;

        public Rotate(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
        }

        public Rotate(int angle)
        {
            _angle = angle;
        }

        public int Angle
        {
            get
            {
                return (_angle);
            }
            set
            {
                _angle = value;
            }
        }

        public override int Read()
        {
            int read = 0;
            _angle = _hpgl2.getInt();
            return (read);
        }

    }
}
