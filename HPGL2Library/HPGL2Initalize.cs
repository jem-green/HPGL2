using TracerLibrary;
using System;
using System.Security;
using System.Diagnostics;

namespace HPGL2Library
{
    public class HPGL2Initialise : Instruction
    {
        #region Fields

        // Initialise the plot
        // IN mode[;]
        // IN [;]

        InitialiseMode _mode = InitialiseMode.Default; //false - default, true - All
        public enum InitialiseMode : int
        {
            None = -1,
            Default = 0,
            All = 1
        }

        #endregion
        #region Constructors

        public HPGL2Initialise(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _name = "Initialise ";
            _instruction = "IN";
            TraceInternal.TraceInformation(_name);
        }

        //public Initialise(InitialiseMode mode)
        //{
        //    _mode = mode;
        //}

        #endregion
        #region Proprties

        public InitialiseMode Mode
        {
            get
            {
              return (_mode);
            }
            set
            {
                _mode = value;
            }
        }

        public override int Read()
        {
            int read = 0;
            if (_hpgl2.Char != ';')
            {
                if ((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9'))
                {
                    _mode = (HPGL2Initialise.InitialiseMode)_hpgl2.getInt();
                    TraceInternal.TraceVerbose(_name + "Mode="+_mode.ToString());
                }
            }

            // Would like to internally re-issue the read with parameters
            // this would mean having a Read(string) method

            // Default the following values
            _hpgl2.Current = new Point(0, 0);
            _hpgl2.Pen.Status = Pen.PenStatus.Up; // PA0,0;
            //**_hpgl2 need to set the pen width**;
            _hpgl2.Page.Rotation.Angle = 0;
            _hpgl2.Page.Input.P1 = new Point(0, 0);
            _hpgl2.Page.Input.P1 = new Point(_hpgl2.Page.Width, _hpgl2.Page.Length);
            //_hpgl2.Pen. **PenWidth units**

            TraceInternal.TraceInformation(_instruction + (int)_mode + ";");
            if (_hpgl2.Match(';') == true)
            {
                _hpgl2.GetChar();   // Consume the terminator if it exists
            }
            return (read);
        }

        #endregion
    }
}
