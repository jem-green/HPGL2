using TracerLibrary;
using System;
using System.Security;
using System.Diagnostics;

namespace HPGL2Library
{
    internal class PolylineEncoded : Instruction
    {
        // PE [flag[value]]x1,y1..[flag[value]]xn,yn;

        /*
         * flag
         * 
         * : - select pen
         * 
         * < - pen up
         *  no value
         *  
         * > - fractional data
         * = - absolute
         * 7 - seven bit mode
         * 
         * PE<=¿¿;AC0,0%1A*p0x0Y*pR*v6W *v1N*l240O&a0N%1BIW0,0,4960,7015LA1,4LA2,4
         * 
         */

        public PolylineEncoded(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "PolylineEncoded ";
            _instruction = "PE";
            Trace.TraceInformation(_name);
        }

        public override int Read()
        {
            int read = 0;
            // PE [[flag][,value]][,cord]...[[flag][,value]][,cord][;]
            // PE[;]
            bool penUp = false;     // Start with pen down
            bool exit = false;      // Exit flag

            do
            {
                switch (_hpgl2.Char)
                {
                    case ':': // Select pen
                        {
                            _hpgl2.GetChar();
                            TraceInternal.TraceVerbose(_name + "Select Pen");
                            Trace.TraceInformation(_instruction + ":");
                            break;
                        }
                    case '<': // Pen up
                        {
                            penUp = true;
                            _hpgl2.GetChar();
                            _hpgl2.Pen.Status = Pen.PenStatus.Up;
                            TraceInternal.TraceVerbose(_name + "Pen=" + _hpgl2.Pen);
                            Trace.TraceInformation(_instruction + "<");
                            break;
                        }
                    case '=': // Plot absolute
                        {
                            _hpgl2.GetChar();
                            Point coOrd = new Point();
                            coOrd.X = FromBase64();
                            coOrd.Y = FromBase64();
                            if (penUp == true)
                            {
                                TraceInternal.TraceVerbose(_name + "Move from=" + _hpgl2.Current + " to=" + coOrd);
                            }
                            else
                            {
                                // create a line and add it to the list
                                Line line = new Line(_hpgl2.Current, coOrd);
                                TraceInternal.TraceVerbose(_name + "Line from=" + _hpgl2.Current + " to=" + coOrd);                         
                                _hpgl2.Lines.Add(line);
                            }
                            Trace.TraceInformation(_instruction + "=" + coOrd);
                            // update the current position for the next line segment
                            _hpgl2.Current = coOrd;
                            penUp = false;
                            break;
                        }
                    case ';':
                    default:
                        {
                            exit = true;
                            read = 1;
                            break;
                        }
                }
            } while (exit == false);
            return (read);
        }

        int FromBase64()
        {
            int value = 0;
            int i = 0;  // This is 64^i
            do
            {
                int digit = (int)_hpgl2.Char;
                _hpgl2.GetChar();
                if (digit > 190) // terminating digit
                {
                    digit = digit - 191;
                    value = value + (int)Math.Pow(64, i) * digit;
                    break;
                }
                else if (digit > 62)
                {
                    digit = digit - 63;
                    value = value + (int)Math.Pow(64, i) * digit;
                    i++;
                }
            } while (i < 6);    // This represnets the maximum 64^6
            return (value);
        }

    }
}
