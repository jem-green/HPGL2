﻿using System;
using System.Diagnostics;
using System.Security;
using TracerLibrary;

namespace HPGL2Library
{
    internal class HPGL2LineAttributes : Instruction
    {
        // LA kind,value[,kind,value[,kind,value]][;]
        // LA [;]

        AttributeType _lineAttribute = AttributeType.None;
        LineEnds _lineEnds = LineEnds.Butt;
        LineJoins _lineJoins = LineJoins.Beveled;
        Double _mitreLimit = 5;

        AttributeType _mode = AttributeType.None;
        public enum AttributeType : int
        {
            None = -1,
            LineEnd = 1,
            LineJoins = 2,
            MiterLimit = 3
        }

        public enum LineEnds : int
        {
            Butt = 1,
            Square = 2,
            Triangular = 3,
            Round = 4
        }

        public enum LineJoins : int
        {
            Mitered = 1,
            MitredBeveled = 2,
            Triangular = 3,
            Round = 4,
            Beveled = 5,
            None = 6
        }

        public HPGL2LineAttributes(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "LineAttributes ";
            _instruction = "LA";
            TraceInternal.TraceInformation(_name);
        }

        public AttributeType Attribute
        {
            get
            {
                return (_lineAttribute);
            }
            set
            {
                _lineAttribute = value;
            }
        }

        public LineEnds End
        {
            get
            {
                return (_lineEnds);
            }
            set
            {
                _lineEnds = value;
            }
        }

        public LineJoins Join
        {
            get
            {
                return (_lineJoins);
            }
            set
            {
                _lineJoins = value;
            }
        }

        public double Limit
        {
            get
            {
                return (_mitreLimit);
            }
            set
            {
                _mitreLimit = value;
            }
        }

        public override int Read()
        {
            int read = 0;
            if (!_hpgl2.Match(';') == true)
            {
                _lineAttribute = (HPGL2LineAttributes.AttributeType)_hpgl2.getInt();
                _hpgl2.getInt();
                if (_hpgl2.Match(','))
                {
                    _hpgl2.GetChar();
                    if (_lineAttribute == HPGL2LineAttributes.AttributeType.LineEnd)
                    {
                        _lineEnds = (HPGL2LineAttributes.LineEnds)_hpgl2.getInt();
                    }
                    else if (_lineAttribute == HPGL2LineAttributes.AttributeType.LineJoins)
                    {
                        _lineJoins = (HPGL2LineAttributes.LineJoins)_hpgl2.getInt();
                    }

                    if (_hpgl2.Match(','))
                    {
                        _hpgl2.GetChar();
                        _lineAttribute = (HPGL2LineAttributes.AttributeType)_hpgl2.getInt();

                        if (_hpgl2.Match(','))
                        {
                            _hpgl2.GetChar();
                            if (_lineAttribute == HPGL2LineAttributes.AttributeType.LineEnd)
                            {
                                _lineEnds = (HPGL2LineAttributes.LineEnds)_hpgl2.getInt();
                            }
                            else if (_lineAttribute == HPGL2LineAttributes.AttributeType.LineJoins)
                            {
                                _lineJoins = (HPGL2LineAttributes.LineJoins)_hpgl2.getInt();
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Bad sytax");
                }
            }
            if (_hpgl2.Match(';') == true)
            {
                _hpgl2.GetChar();   // Consume the terminator if it exists
            }
            return (read);
        }
    }
}
