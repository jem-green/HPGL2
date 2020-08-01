using System;
using System.Collections.Generic;
using System.Text;

namespace HPGL2Library
{
    public class Parameter
    {
        #region Variables

        string value = "";
        SourceType source = SourceType.None;
        public enum SourceType
        {
            None = 0,
            Command = 1,
            Registry = 2,
            File = 3,
            App = 4
        }

        #endregion
        #region Constructor
        public Parameter(string value)
        {
            this.value = value;
            source = SourceType.None;
        }
        public Parameter(string value, SourceType source)
        {
            this.value = value;
            this.source = source;
        }
        #endregion
        #region Parameters
        public string Value
        {
            set
            {
                this.value = value;
            }
            get
            {
                return (value);
            }
        }

        public SourceType Source
        {
            set
            {
                source = value;
            }
            get
            {
                return (source);
            }
        }
        #endregion
        #region Methods
        public override string ToString()
        {
            return (value);
        }
        #endregion
    }
}
