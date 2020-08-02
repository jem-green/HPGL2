using Microsoft.Extensions.Logging;
using System;
using System.Security;

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


        object _value = 0;
        string _name = "";

        public PolylineEncoded(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
        }

        public object Value
        {
            get
            {
                return (_value);
            }
            set
            {
                _value = value;
            }
        }

        public override int Read()
        {
            int read = 0;
            // PE [[flag][,value]][,cord]...[[flag][,value]][,cord][;]
            // PE[;]
            bool penUp = false;  // Start with pen down
            bool exit = false;       // Exit flag

            do
            {
                switch (_hpgl2.Char)
                {
                    case ':': // Select pen
                        {
                            _hpgl2.getChar();
                            _hpgl2.Logger.LogDebug("PE Select Pen");
                            break;
                        }
                    case '<': // Pen up
                        {
                            penUp = true;
                            _hpgl2.getChar();
                            _hpgl2.Pen.Status = Pen.PenStatus.Up;
                            _hpgl2.Logger.LogDebug("PE Pen " + _hpgl2.Pen.ToString());
                            break;
                        }
                    case '=': // Plot absolute
                        {
                            penUp = false;
                            _hpgl2.getChar();
                            CoOrd coOrd = new CoOrd();
                            coOrd.X = FromBase64();
                            coOrd.Y = FromBase64();
                            // create a line and add it to the list
                            Line line = new Line(_hpgl2.Current, coOrd);
                            _hpgl2.Logger.LogDebug("PE Line " + _hpgl2.Current.ToString() + " to " + coOrd.ToString());
                            _hpgl2.Lines.Add(line);
                            // update the current position for the next line segment
                            _hpgl2.Current = coOrd;
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
                _hpgl2.getChar();
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
