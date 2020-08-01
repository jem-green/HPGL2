using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HPGL2Library
{
    public class Page
    {
        #region Variables

        /*
         * Need to consider some concepts
         * hard clip lmits so the physical size of the plotter in plotter units
         * user scaling that maps the physical hard clip limits to the user dimensions
         */

        double _plotterUnitits = 0.025; // 1/40 mm 

        // These represent the physical page in mm.

        int _left = 0;
        int _bottom = 0;
        int _width = 0;
        int _height = 0;

        // The page will contain scaling points

        Input _input;

        // The page could contain a scaling object 

        Scale _scale;

        #endregion
        #region Constructor

        public Page()
        {
            _input = new Input(0,0);
        }

        public Page(int left, int bottom, int width, int height)
        {
            _left = left;
            _bottom = bottom;
            _width = width;
            _height = height;
            _input.P1 = new CoOrd(0, 0);
            _input.P2 = new CoOrd((int)(width / _plotterUnitits), (int)(height / _plotterUnitits));
        }

        public Page(CoOrd x1y1, CoOrd x2y2)
        {
            _left = x1y1.X;
            _bottom = x1y1.Y;
            _width = (x2y2.X - x1y1.X);
            _height = (x2y2.Y - x1y1.Y);
            _input.P1 = new CoOrd(0, 0);
            _input.P2 = new CoOrd((int)(_width / _plotterUnitits), (int)(_height / _plotterUnitits));
        }

        #endregion
        #region Properies
        public int Left
        {
            get
            {
                return (_left);
            }
            set
            {
                _left = value;
            }
        }

        public int Bottom
        {
            get
            {
                return (_bottom);
            }
            set
            {
                _bottom = value;
            }
        }

        public int Width
        {
            get
            {
                return (_width);
            }
            set
            {
                _width = value;
            }
        }

        public int Height
        {
            get
            {
                return (_height);
            }
            set
            {
                _height = value;
            }
        }

        public int Right
        {
            get
            {
                return (_left + _width);
            }
            set
            {
                _width = value - _left;
            }
        }

        public int Top
        {
            get
            {
                return (_bottom + _height);
            }
            set
            {
                _height = value - _bottom;
            }
        }

        public Input Input
        {
            get
            {
                return (_input);
            }
            set
            {
                _input = value;
            }
        }

        public Scale Scale
        {
            get
            {
                return (_scale);
            }
            set
            {
                _scale = value;
            }
        }

        #endregion
        #region Methods

        #endregion
    }
}
