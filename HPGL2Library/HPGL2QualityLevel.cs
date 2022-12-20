using System;
using System.Security;
using TracerLibrary;

namespace HPGL2Library
{
    public class HPGL2QualityLevel : Instruction
    {
        // Quality Level
        // QL Value
        // QL[;]
        int _qualityLevel = 0;   // 0 - 100


        public HPGL2QualityLevel(HPGL2Document hpgl2)
        {
            _hpgl2= hpgl2;
            _name = "Quality Level";
            _instruction = "QL";
            TraceInternal.TraceInformation(_name);
        }

        public int Level
        {
            get
            {
                return (_qualityLevel);
            }
            set
            {
                _qualityLevel = value;
            }
        }

        public override int Read()
        {
            int read = 0;
            return (read);
        }
    }
}
