using TracerLibrary;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace HPGL2Library
{
    public class Page : Instruction
    {
        #region Fields

        /*
         * Need to consider some concepts
         * Physical limits of a page in mm
         * hard clip lmits so the physical size of the plotter in plotter units
         * user scaling that maps the physical hard clip limits to the user dimensions
         */

        double _plotterUnits = 0.025; // 1/40 mm 

        // These represent the physical page in mm.

        int _left = 0;
        int _bottom = 0;
        int _width = 0;
        int _length = 0;

        // The page will have a plot size

        PlotSize _plotSize;

        // The page will contain rotation information

        Rotate _rotate;

        // The page will contain scaling points

        Input _input;

        // The page will contain a scaling object 

        Scale _scale;

        #endregion
        #region Constructor

        public Page(HPGL2Document hpgl2)
        {
            _hpgl2 = hpgl2;
            _input = new Input(_hpgl2);
            _rotate = new Rotate(_hpgl2);
            _scale = new Scale(_hpgl2);
            _name = "Page ";
            _instruction = "";
        }

        public Page(int left, int bottom, int width, int length)
        {
            _left = left;
            _bottom = bottom;
            _width = width;
            _length = length;

            // Convert the physical page into plotter units

            _input.P1 = new Point(0, 0);
            _input.P2 = new Point((int)(width / _plotterUnits), (int)(length / _plotterUnits));
        }

        public Page(Point x1y1, Point x2y2)
        {
            _left = x1y1.X;
            _bottom = x1y1.Y;
            _width = (x2y2.X - x1y1.X);
            _length = (x2y2.Y - x1y1.Y);
            _input.P1 = new Point(0, 0);
            _input.P2 = new Point((int)(_width / _plotterUnits), (int)(_length / _plotterUnits));
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

        public int Length
        {
            get
            {
                return (_length);
            }
            set
            {
                _length = value;
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
                return (_bottom + _length);
            }
            set
            {
                _length = value - _bottom;
            }
        }

        public double Units
        {
            get
            {
                return (_plotterUnits);
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

        /// <summary>
        /// Size of the page in plotter units
        /// </summary>
        public PlotSize Size
        {
            get
            {
                return (_plotSize);
            }
            set
            {
                _plotSize = value;
            }
        }

        public Rotate Rotation
        {
            get
            {
                return (_rotate);
            }
            set
            {
                _rotate = value;
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

        public Coord ToPhysicalUnits(double x, double y)
        {
            Point plotterUnits = ToPlotterUnits(x, y);
            return (new Coord(plotterUnits.X*_plotterUnits, plotterUnits.Y*_plotterUnits));
        }

        /// <summary>
        /// Convert user units to absolute plotter units
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
            public Point ToPlotterUnits(double x, double y)
        {
            // Need to reference the scale, rotation and the input to determine
            // the plotter units
            Point plotterUnits = new Point();
            switch (_rotate.Angle)
            {
                case 0:
                    {
                        plotterUnits.X = _input.P1.X + (int)((_input.P2.X - _input.P1.X) / (_input.X2 - _input.X1) * (x - _input.P1.X));
                        plotterUnits.Y = _input.P1.Y + (int)((_input.P2.Y - _input.P1.Y) / (_input.Y2 - _input.Y1) * (y - _input.P1.Y));
                        break;
                    }
                case 90:
                    {
                        plotterUnits.X = _input.P1.X - (int)((_input.P2.Y - _input.P1.Y) / (_input.Y2 - _input.Y1) * (y - _input.P1.Y));
                        plotterUnits.Y = _input.P1.Y - (int)((_input.P2.X - _input.P1.X) / (_input.X2 - _input.X1) * (x - _input.P1.X));
                        break;
                    }
            }

            return (plotterUnits);
        }

        public override int Read()
        {
            int read = 0;
            return (read);
        }

        #endregion
    }
}
