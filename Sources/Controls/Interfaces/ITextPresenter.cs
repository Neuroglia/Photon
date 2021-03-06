﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photon.Controls
{

    /// <summary>
    /// This interface is implemented by all <see cref="UIElement"/>s that present text
    /// </summary>
    public interface ITextPresenter
        : IPaddedElement, Markup.IAddChild
    {

        /// <summary>
        /// Gets/sets the <see cref="Media.Brush"/> with which to render the text
        /// </summary>
        Media.Brush Foreground { get; set; }

        /// <summary>
        /// Gets/sets the text's <see cref="Media.FontFamily"/>
        /// </summary>
        Media.FontFamily FontFamily { get; set; }

        /// <summary>
        /// Gets/sets the text's <see cref="System.Drawing.FontStyle"/>
        /// </summary>
        FontStyle FontStyle { get; set; }

        /// <summary>
        /// Gets/sets the text's EM size
        /// </summary>
        double FontSize { get; set; }

        /// <summary>
        /// Gets the text's <see cref="System.Drawing.Font"/>
        /// </summary>
        Font Font { get; }

        /// <summary>
        /// Gets/sets the text presented by the element
        /// </summary>
        string Text { get; set; }

    }

}
