﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;

namespace HPGL2Library
{
    internal class UserDefinedLinetype : Instruction
    {
        // User Defined Line
        // UL index[,gapl...gapm][;]
        // UL;
        //
        // sequence of PD,PU...

        #region Fields

        int _index = 1; // 1-8
        List<double> _pattern = new List<double>();
        int parts = 0;

        #endregion
        #region Constructors

        public UserDefinedLinetype(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
        }

        #endregion
        #region Properties

        public int Index
        {
            get
            {
                return (_index);
            }
            set
            {
                _index = value;
            }
        }

        #endregion
        #region Methods
        public void Add(double length)
        {
            _pattern.Add(length);
        }

        public override int Read()
        {
            int read = 0;
            if (!_hpgl2.Match(';'))
            {
                if ((_hpgl2.Char>= '1') && (_hpgl2.Char <= '8'))
                {
                    _index = _hpgl2.getInt();
                    do
                    {
                        if (_hpgl2.Match(','))
                        {
                            _hpgl2.GetChar();
                            _pattern.Add(_hpgl2.getDouble());
                        }

                    } while (((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9')) || (_hpgl2.Char == ','));
                }
            }
            else
            {
                _hpgl2.GetChar();
            }
            return (read);
        }
        #endregion
    }
}
