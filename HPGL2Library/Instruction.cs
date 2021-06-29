using System;
using System.Collections.Generic;
using System.Text;

namespace HPGL2Library
{
    public abstract class Instruction
    {
        #region Fields
        private protected HPGL2Document _hpgl2;
        private protected string _name = "";
        private protected string _instruction = "";

        #endregion
        #region Methods
        public abstract int Read();
        #endregion
    }
}
