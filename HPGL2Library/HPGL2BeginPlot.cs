using TracerLibrary;
using System;
using System.Security;
using System.Diagnostics;
using System.Collections.Generic;

namespace HPGL2Library
{
    public class HPGL2BeginPlot : Instruction
    {
        // Begin Plot
        // BP kind,value[,kind,value][;]
        // BP [;]

        #region Fields

        // BP Kind, Value
        KindType _kind = 0;
        object _value = 0;
        int _index = 0;
        KeyValuePair<KindType, object>[] _parameters = null;

        // This i assume should be a collection of key value pairs or
        // reference back to the document

        public enum KindType : int
        {
            None = -1,
            Default = 0,
            PictureName = 1,
            NumberOfCopies = 2,
            FileDispositionCode = 3,
            RenderLastPlot = 4,
            AutoRotation = 5
        }

        #endregion
        #region Constructors
        public HPGL2BeginPlot(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            base._name = "BeginPlot";
            _instruction = "BP";
            _parameters = new KeyValuePair<KindType, object>[10];
            TraceInternal.TraceInformation(base._name);
        }

        #endregion
        #region Properties

        public KindType Kind
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
            if (_hpgl2.Char != ';')
            {
                //

                if ((_hpgl2.Char >= '0') && (_hpgl2.Char <= '9'))
                {
                    _kind = (HPGL2BeginPlot.KindType)_hpgl2.getInt();
                    if (_hpgl2.Match(','))
                    {
                        _hpgl2.GetChar(); // Consume the , if it exists
                        switch (_kind)
                        {
                            case HPGL2BeginPlot.KindType.PictureName:
                                {
                                    string pictureName = ""; //_hpgl2.GetChar();
                                    TraceInternal.TraceInformation(_instruction + " " + _kind + "," + pictureName);
                                    Array.Resize(ref _parameters, _index);
                                    _parameters[_index] = new KeyValuePair<KindType, object>( _kind, pictureName );
                                    break;
                                }
                            case HPGL2BeginPlot.KindType.AutoRotation:
                                {
                                    int autoRotate = _hpgl2.getInt();  // Ignore
                                    TraceInternal.TraceInformation(_instruction + " " + _kind + "," + autoRotate.ToString());
                                    read = 1;
                                    break;
                                }
                            default:
                                {
                                    TraceInternal.TraceInformation(_instruction + " " + _kind);
                                    throw new NotImplementedException();
                                }

                        }
                    }
                    else
                    {
                        read = 1;
                    }
                }
                else
                {
                    TraceInternal.TraceInformation(_instruction + ";");
                }
            }
            if (_hpgl2.Match(';') == true)
            {
                _hpgl2.GetChar();   // Consume the terminator if it exists
            }
            return (read);
        }
        #endregion
    }
}
