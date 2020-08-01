using System;
using System.Security;

namespace HPGL2Library
{
    public class BeginPlot : Instruction
    {
        // BP kind,value

        #region Variable

        // BP Kind, Value
        kindType _kind = 0;
        object _value = 0;
        string _name = "";

        public enum kindType : int
        {
            None = -1,
            PictureName = 1,
            NumberOfCopies = 2,
            FileDispositionCode = 3,
            RenderLastPlot = 4,
            Autorotation = 5
        }

        #endregion
        #region Constructors
        public BeginPlot(HPGL2 hpgl2)
        {
            _hpgl2 = hpgl2;
        }

        public BeginPlot(int kind, object value)
        {
            _kind = (kindType)kind;
            _value = value;
        }
        #endregion
        #region Properties

        public kindType Kind
        {
            get
            {
                return (_kind);
            }
            set
            {
                _kind = value;
            }
        }

        public object Value
        {
            get
            {
                return(_value);
            }
            set
            {
                _value = value;
            }
        }

        #endregion
        #region Methods

        public override int Read()
        {
            int read = 0;
            _kind = (BeginPlot.kindType)_hpgl2.getInt();
            if (_hpgl2.Match(','))
            {
                _hpgl2.getChar();
                switch (_kind)
                {
                    case BeginPlot.kindType.Autorotation:
                        {
                            int autoRotate = _hpgl2.getInt();  // Ignore
                            read = 1;
                            break;
                        }
                }
            }
            return (read);
        }
        #endregion
    }
}
