using System;
using System.Security;

namespace HPGL2Library
{
    internal class PenWidthUnit : Instruction
    {
        // BP Kind, Value
        WidthUnit _widthUnit = 0;   // Default to metric

        public enum WidthUnit : int
        {
            None = -1,
            Metric = 0,
            Relative = 1,
        }

        public PenWidthUnit(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
        }

        public PenWidthUnit(int widthUnit)
        {
            _widthUnit = (WidthUnit)widthUnit;
        }

        public int Unit
        {
            get
            {
                return ((int)_widthUnit);
            }
            set
            {
                _widthUnit = (WidthUnit)value;
            }
        }
        public override int Read()
        {
            int read = 0;
            _widthUnit = (WidthUnit)_hpgl2.getInt();
            return (read);
        }
    }
}
