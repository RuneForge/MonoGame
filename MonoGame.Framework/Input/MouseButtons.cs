using System;

namespace Microsoft.Xna.Framework.Input
{
    /// <summary>
    /// Defines the buttons on a mouse.
    /// </summary>
    [Flags]
    public enum MouseButtons
    {
        /// <summary>
        /// Left mouse button.
        /// </summary>
        LeftButton = 1,

        /// <summary>
        /// Middle mouse button.
        /// </summary>
        MiddleButton = 2,

        /// <summary>
        /// Right mouse button.
        /// </summary>
        RightButton = 4,

        /// <summary>
        /// First extended button.
        /// </summary>
        XButton1 = 8,

        /// <summary>
        /// Second extended button.
        /// </summary>
        XButton2 = 16,
    }
}
