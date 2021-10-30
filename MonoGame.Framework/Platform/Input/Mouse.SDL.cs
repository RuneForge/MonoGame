// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Input
{
    public static partial class Mouse
    {
        internal static int ScrollX;
        internal static int ScrollY;

        private static IntPtr PlatformGetWindowHandle()
        {
            return PrimaryWindow.Handle;
        }

        private static void PlatformSetWindowHandle(IntPtr windowHandle)
        {
        }

        private static MouseState PlatformGetState(GameWindow window)
        {
            int x, y;
            int winFlags = Sdl.Window.GetWindowFlags(window.Handle);
            Sdl.Mouse.Button state = Sdl.Mouse.GetGlobalState(out x, out y);

            if ((winFlags & Sdl.Window.State.MouseFocus) != 0)
            {
                // Window has mouse focus, position will be set from the motion event
                window.m_mouseState.LeftButton = (state & Sdl.Mouse.Button.Left) != 0 ? ButtonState.Pressed : ButtonState.Released;
                window.m_mouseState.MiddleButton = (state & Sdl.Mouse.Button.Middle) != 0 ? ButtonState.Pressed : ButtonState.Released;
                window.m_mouseState.RightButton = (state & Sdl.Mouse.Button.Right) != 0 ? ButtonState.Pressed : ButtonState.Released;
                window.m_mouseState.XButton1 = (state & Sdl.Mouse.Button.X1Mask) != 0 ? ButtonState.Pressed : ButtonState.Released;
                window.m_mouseState.XButton2 = (state & Sdl.Mouse.Button.X2Mask) != 0 ? ButtonState.Pressed : ButtonState.Released;

                window.m_mouseState.HorizontalScrollWheelValue = ScrollX;
                window.m_mouseState.ScrollWheelValue = ScrollY;
            }
            else
            {
                // Window does not have mouse focus, we need to manually get the position
                Rectangle clientBounds = window.ClientBounds;
                window.m_mouseState.X = x - clientBounds.X;
                window.m_mouseState.Y = y - clientBounds.Y;
            }

            return window.m_mouseState;
        }

        private static void PlatformSetPosition(int x, int y)
        {
            PrimaryWindow.m_mouseState.X = x;
            PrimaryWindow.m_mouseState.Y = y;

            Sdl.Mouse.WarpInWindow(PrimaryWindow.Handle, x, y);
        }

        private static void PlatformSetCursor(MouseCursor cursor)
        {
            Sdl.Mouse.SetCursor(cursor.Handle);
        }
    }
}
