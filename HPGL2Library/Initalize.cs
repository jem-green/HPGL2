using System;
using System.Security;

namespace HPGL2Library
{
    public class Initialise : Instruction
    {
        // Initialise the plot
        // IN mode[;]
        // IN [;]

        InitialiseMode _mode = InitialiseMode.Default; //false - default, true - All
        public enum InitialiseMode : int
        {
            None = -1,
            Default = 0,
            All = 1
        }

        public Initialise(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
        }

        public Initialise(InitialiseMode mode)
        {
            _mode = mode;
        }

        public InitialiseMode Mode
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
                _mode = (Initialise.InitialiseMode)_hpgl2.getInt();
            }      
            return (read);
        }
    }
}
