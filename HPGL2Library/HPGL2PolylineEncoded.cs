using TracerLibrary;
using System;
using System.Security;
using System.Diagnostics;

namespace HPGL2Library
{
    internal class HPGL2PolylineEncoded : Instruction
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

        public HPGL2PolylineEncoded(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "PolylineEncoded";
            _instruction = "PE";
            TraceInternal.TraceInformation(_name);
        }

        public override int Read()
        {
            // PE [[flag][,value]][,cord]...[[flag][,value]][,cord];
            // PE;

            int read = 0;               // Read sucessful
            bool penUp = false;         // Start with pen down
            bool exit = false;          // Exit flag
            int id = 0;                 // Pen id
            bool encoding64bit = true;  // Indicate using 64 bit encoding

            do
            {
                switch (_hpgl2.Char)
                {
                    case ':': // Select pen
                        {
                            TraceInternal.TraceVerbose(_name + " " + "Select Pen");
                            TraceInternal.TraceInformation(_instruction + " " + ":");
                            if (encoding64bit)
                            {
                                id = FromBase64encoding();
                            }
                            else
                            {
                                id = FromBase32encoding();
                            }
                            TraceInternal.TraceVerbose(_name + "Pen=" + id);
                            break;
                        }
                    case '<': // Pen up
                        {
                            TraceInternal.TraceVerbose(_name + " " + "Pen up");
                            TraceInternal.TraceInformation(_instruction + "<");
                            penUp = true;
                            _hpgl2.GetChar();
                            _hpgl2.Pen.Status = Pen.PenStatus.Up;
                            TraceInternal.TraceVerbose(_name + "Pen^");
                            break;
                        }
                    case '>': // Fractional Data
                        {
                            TraceInternal.TraceVerbose(_name + " " + "Fractional data");
                            TraceInternal.TraceInformation(_instruction + ">");
                            double fractional = 0;
                            TraceInternal.TraceVerbose(_name + " " + fractional);
                            break;
                        }
                    case '=': // Plot absolute
                        {
                            TraceInternal.TraceVerbose(_name + " " + "Plot absolute");
                            _hpgl2.GetChar();
                            Point coOrd = new Point();

                            if (encoding64bit)
                            {
                                coOrd.X = FromBase64encoding();
                                coOrd.Y = FromBase64encoding();
                            }
                            else
                            {
                                coOrd.X = FromBase32encoding();
                                coOrd.Y = FromBase32encoding();
                            }

                            if (penUp == true)
                            {
                                TraceInternal.TraceVerbose(_name + " " + "Move from=" + _hpgl2.Current + " to=" + coOrd);
                            }
                            else
                            {
                                // create a line and add it to the list
                                Line line = new Line(_hpgl2.Current, coOrd);
                                TraceInternal.TraceVerbose(_name + " " + "Line from=" + _hpgl2.Current + " to=" + coOrd);
                                _hpgl2.Lines.Add(line);
                            }
                            TraceInternal.TraceInformation(_instruction + "=" + coOrd);
                            // update the current position for the next line segment
                            _hpgl2.Current = coOrd;
                            penUp = false;
                            break;
                        }
                    case '7':
                        {
                            encoding64bit = false;
                            break;
                        }
                    case ';':   // There must be a ; terminator
                        {
                            exit = true;
                            read = 1;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            } while (exit == false);

            return (read);
        }

        private int FromBase64encoding()
        {
            int value = 0;
            int i = 0;  // This is 64^i
            do
            {
                int digit = (int)_hpgl2.Char;
                _hpgl2.GetChar();
                if ((digit > 190) && (digit < 255))// terminating digit
                {
                    digit = digit - 191;
                    value = value + (int)Math.Pow(64, i) * digit;
                    break;
                }
                else if ((digit > 62) && (digit < 127))
                {
                    digit = digit - 63;
                    value = value + (int)Math.Pow(64, i) * digit;
                    i++;
                }
                else if (((digit > 127) && (digit < 161)) || (digit == 255))
                {
                    // Igone
                    value = value;
                }
                else
                { 
                    throw new NotImplementedException();
                }
            } while (i < 6);    // This represnets the maximum 64^6
            return (value);
        }

        private int FromBase32encoding()
        {
            int value = 0;
            int i = 0;  // This is 32^i
            do
            {
                int digit = (int)_hpgl2.Char;
                _hpgl2.GetChar();
                if ((digit > 94) && (digit < 127))// terminating digit
                {
                    digit = digit - 95;
                    value = value + (int)Math.Pow(32, i) * digit;
                    break;
                }
                else if ((digit > 62) && (digit < 95))
                {
                    digit = digit - 63;
                    value = value + (int)Math.Pow(32, i) * digit;
                    i++;
                }
                else
                {
                    throw new NotImplementedException();
                }
            } while (i < 6);    // This represnets the maximum 64^6
            return (value);
        }
    }
}
