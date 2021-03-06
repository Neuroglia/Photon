﻿using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photon.Media
{

    /// <summary>
    /// This class represents a brush that paints an area with a solid color
    /// </summary>
    public sealed class SolidColorBrush
        : Brush
    {

        /// <summary>
        /// Initializes a new <see cref="SolidColorBrush"/>
        /// </summary>
        public SolidColorBrush()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="SolidColorBrush"/> with the specified <see cref="System.Drawing.Color"/>
        /// </summary>
        /// <param name="color">The <see cref="System.Drawing.Color"/> associated with the brush</param>
        public SolidColorBrush(Color color)
        {
            this.Color = color;
        }

        /// <summary>
        /// Describes the <see cref="SolidColorBrush.Color"/> <see cref="DependencyProperty"/>
        /// </summary>
        public static DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(SolidColorBrush));
        /// <summary>
        /// Gets/Sets the <see cref="System.Drawing.Color"/> associated with the brush
        /// </summary>
        public Color Color
        {
            get
            {
                return this.GetValue<Color>(SolidColorBrush.ColorProperty);
            }
            set
            {
                this.SetValue(SolidColorBrush.ColorProperty, value);
            }
        }

        /// <summary>
        /// Begins using the <see cref="Brush"/> to paint a geometry. Must be followed by a call to the <see cref="Brush.EndUse"/> method
        /// </summary>
        /// <param name="drawing">The <see cref="Drawing"/> to render</param>
        public override void BeginUse(Drawing drawing)
        {
            Color color;
            if (this.Opacity != 1)
            {
                color = Color.FromArgb((int)(this.Color.A * this.Opacity), this.Color.R, this.Color.G, this.Color.B);
            }
            else
            {
                color = this.Color;
            }
            GL.Color4(color);
        }

        /// <summary>
        /// Ends using the <see cref="Brush"/>
        /// </summary>
        public override void EndUse()
        {
            //Nothing to do
        }

        /// <summary>
        /// Clones the brush
        /// </summary>
        /// <returns>The clone of the <see cref="Brush"/></returns>
        public override Brush Clone()
        {
            return new SolidColorBrush(this.Color) { Opacity = this.Opacity };
        }


    }

}
