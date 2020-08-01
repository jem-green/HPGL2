using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HPGL2Library
{
    public struct CoOrd
    {
        int _x;
        int _y;

        public CoOrd(int x, int y)
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

        public static bool operator ==(CoOrd left, CoOrd right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CoOrd left, CoOrd right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return (_x + "," + _y);
        }
    }
}
