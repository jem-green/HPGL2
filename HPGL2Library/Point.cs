using System;

namespace HPGL2Library
{
    /// <summary>
    /// Points are in the user defined coordinate system
    /// and adjusted with scaling and input parameters, and
    /// need converting to plotter units and then eventually
    /// into metic units.
    /// </summary>

    public struct Point
    {
        int _x;
        int _y;

        public Point(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public int X
        {
            get
            {
                return (_x);
            }
            set
            {
                _x = value;
            }
        }

        public int Y
        {
            get
            {
                return (_y);
            }
            set
            {
                _y = value;
            }
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return (_x + "," + _y);
        }
    }
}
