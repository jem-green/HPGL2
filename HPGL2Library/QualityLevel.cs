using System;
using System.Security;

namespace HPGL2Library
{
    public class QualityLevel : Instruction
    {
        // QL Value
        int _qualityLevel = 0;   // 0 - 100


        public QualityLevel()
        { }

        public QualityLevel(int level)
        {
            _qualityLevel = level;
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
