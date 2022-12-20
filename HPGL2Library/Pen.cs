using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace HPGL2Library
{
    public class Pen
    {
        #region Fields

        int _id = 0;
        PenStatus _status = PenStatus.Up;
        HPGL2PenWidth _penWidth;

        public enum PenStatus : int
        {
            Unknown = -1,
            Down = 0,
            Up = 1
        }

        #endregion
        #region Constructor

        public Pen(HPGL2Document hpgl2)
        {
            _penWidth = new HPGL2PenWidth(hpgl2);
        }

        #endregion
        #region Properties

        public int Id
        {
            get
            {
                return (_id);
            }
            set
            {
                _id = value;
            }
        }

        public PenStatus Status
        {
            get
            {
                return(_status);
            }
            set
            {
                _status = value;
            }
        }

        public HPGL2PenWidth PenWidth
        {
            get
            {
                return (_penWidth);
            }
            set
            {
                _penWidth = value;
            }
        }

        #endregion

        public override string ToString()
        {
            return (_status.ToString());
        }
    }
}
