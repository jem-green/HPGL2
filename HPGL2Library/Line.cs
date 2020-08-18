using System;
using System.Collections.Generic;
using System.Text;

namespace HPGL2Library
{
    /// <summary>
    /// Lines are in user units and need converting
    /// to plotter units and then eventually into metic units
    /// </summary>
    public class Line
    {
        Point _start;
        Point _end;

        public Line()
        { }

        public Line(Point start, Point end)
        {
            _start = start;
            _end = end;       
        }

        public Line(int x1, int y1, int x2, int y2)
        {
            _start = new Point(x1, y1);
            _end = new Point(x2, y2);
        }

        public Point Start
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

        public Point End
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
