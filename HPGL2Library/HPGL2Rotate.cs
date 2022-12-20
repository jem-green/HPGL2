using TracerLibrary;
using System;
using System.Security;
using System.Diagnostics;

namespace HPGL2Library
{
    public class HPGLRotate : Instruction
    {
        // RO angle[;]
        // RO [;]

        int _angle = 0;

        public HPGLRotate(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "Rotate ";
            _instruction = "RO";
            TraceInternal.TraceInformation(_name);
        }

        //public RotateCoordinateSystem(int angle)
        //{
        //    _angle = angle;
        //}

        public int Angle
        {
            get
            {
                return (_angle);
            }
            set
            {
                int angle = value;
                // Consider if this needs rounding to nearest 90 degrees
                _angle = (int)(90 * Math.Round((double)angle / 90));
            }
        }

        public override int Read()
        {
            int read = 0;
            if (!_hpgl2.Match(';') == true)
            {
                int angle = _hpgl2.getInt();
                // Consider if this needs rounding to nearest 90 degrees
                _angle = (int)(90 * Math.Round((double)angle / 90));
                TraceInternal.TraceVerbose(_name + "angle=" + _angle);
                TraceInternal.TraceInformation(_instruction + _angle + ";");
                Point p1 = _hpgl2.Page.Input.P1;
                Point p2 = _hpgl2.Page.Input.P2;
                Point pt = new Point();
                _hpgl2.Page.Rotation = this;

                //switch (_angle)
                //{
                //    case 0:
                //        {
                //            TraceInternal.TraceVerbose(_name + "P1=" + p1 + " P2=" + p2);
                //            break;
                //        }
                //    case 90:
                //        {
                //            pt.X = p1.X;
                //            p1.X = p2.X;
                //            pt.Y = p2.X;
                //            p2.Y = p1.Y + pt.X;
                //            p2.X = p2.X - p2.Y;
                //            TraceInternal.TraceVerbose(_name + "P1=" + p1 + " P2=" + p2);
                //            break;
                //        }
                //    case 180:
                //        {
                //            pt = p1;
                //            p1 = p2;
                //            p2 = pt;
                //            TraceInternal.TraceVerbose(_name + "P1=" + p1 + " P2=" + p2);
                //            break;
                //        }
                //    case 270:
                //        {
                //            pt.Y = p2.Y;
                //            p1.Y = p2.Y;
                //            p2.X = p1.X + p2.Y;
                //            p2.Y = p2.Y - p2.X;
                //            TraceInternal.TraceVerbose(_name + "P1=" + p1 + " P2=" + p2);
                //            break;
                //        }
                //}

            }
            if (_hpgl2.Match(';') == true)
            {
                _hpgl2.GetChar();   // Consume the terminator if it exists
            }
            return (read);
        }
    }
}
