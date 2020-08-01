using System;
using System.Collections.Generic;
using System.Text;

namespace HPGL2Library
{
    public class Line
    {
        CoOrd _start;
        CoOrd _end;

        public Line()
        { }

        public Line(CoOrd start, CoOrd end)
        {
            _start = start;
            _end = end;       
        }

        public Line(int x1, int y1, int x2, int y2)
        {
            _start = new CoOrd(x1, y1);
            _end = new CoOrd(x2, y2);
        }

        public CoOrd Start
        {
            get
            {
                return (_start);
            }
            set
            {
                _start = value;
            }
        }

        public CoOrd End
        {
            get
            {
                return (_end);
            }
            set
            {
                _end = value;
            }
        }

        public int X1
        {
            get
            {
                return (_start.X);
            }
            set
            {
                _start.X = value;
            }
        }

        public int Y1
        {
            get
            {
                return (_start.Y);
            }
            set
            {
                _start.Y = value;
            }
        }

        public int X2
        {
            get
            {
                return (_end.X);
            }
            set
            {
                _end.X = value;
            }
        }

        public int Y2
        {
            get
            {
                return (_end.Y);
            }
            set
            {
                _end.Y = value;
            }
        }
    }
}
