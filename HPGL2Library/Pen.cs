using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace HPGL2Library
{
    public class Pen
    {
        #region Variables

        PenStatus _status = PenStatus.Up;

        public enum PenStatus : int
        {
            Unknown = -1,
            Down = 0,
            Up = 1
        }

        #endregion
        #region Constructor

        public Pen()
        {
        }

        public Pen(PenStatus status)
        {
            _status = status;
        }

        #endregion
        #region Properties

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

        #endregion

        public override string ToString()
        {
            return (_status.ToString());
        }

    }
}
