﻿using System;
using System.Collections.Generic;
using System.Text;


namespace HPGL2Library
{
    public struct Coord
    {

        double _x;
        double _y;

        public Coord(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public double X
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

        public double Y
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

        public static bool operator ==(Coord left, Coord right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Coord left, Coord right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return (_x + "," + _y);
        }
    }
}