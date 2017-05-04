using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANAConversationStudio
{
    /// <summary>
    /// Defines the current state of the mouse handling logic.
    /// </summary>
    internal enum MouseHandlingMode
    {
        /// <summary>
        /// Not in any special mode.
        /// </summary>
        None,

        /// <summary>
        /// Panning has been initiated and will commence when the use drags the cursor further than the threshold distance.
        /// </summary>
        Panning,

        /// <summary>
		/// The user is left-mouse-button-dragging to pan the viewport.
        /// </summary>
        DragPanning,

        /// <summary>
        /// The user is holding down shift and left-clicking or right-clicking to zoom in or out.
        /// </summary>
        Zooming,

        /// <summary>
        /// The user is holding down shift and left-mouse-button-dragging to select a region to zoom to.
        /// </summary>
        DragZooming,
    }
}
