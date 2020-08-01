using System;
using System.Collections.Generic;
using System.Text;

namespace HPGL2Library
{
    public abstract class Instruction
    {
        #region Variables
        private protected HPGL2 _hpgl2;
        #endregion
        #region Methods
        public abstract int Read();
        #endregion
    }
}
