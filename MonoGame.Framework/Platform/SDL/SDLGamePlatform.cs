// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Framework.Utilities;

namespace Microsoft.Xna.Framework
{
    internal class SdlGamePlatform : GamePlatform
    {
        #region Fields

        private readonly Game m_game;
        private readonly List<Keys> m_keys;
        private int m_isExiting;
        private SdlGameWindow m_gameWindow;

        #endregion

        #region Constructors

        public SdlGamePlatform(Game game, SdlGameWindow gameWindow)
            : base(game)
        {
            m_game = game;
            m_keys = new List<Keys>();
            Keyboard.SetKeys(m_keys);

            Sdl.Version sversion;
            Sdl.GetVersion(out sversion);

            Sdl.Major = sversion.Major;
            Sdl.Minor = sversion.Minor;
            Sdl.Patch = sversion.Patch;

            int version = (100 * Sdl.Major) + (10 * Sdl.Minor) + Sdl.Patch;

            if (version <= 204)
                Debug.WriteLine("Please use SDL 2.0.5 or higher.");

            // Needed so VS can debug the project on Windows
            if (version >= 205 && CurrentPlatform.OS == OS.Windows && Debugger.IsAttached)
                Sdl.SetHint("SDL_WINDOWS_DISABLE_THREAD_NAMING", "1");

            Sdl.Init((int)(
                Sdl.InitFlags.Video |
                Sdl.InitFlags.Joystick |
                Sdl.InitFlags.GameController |
                Sdl.InitFlags.Haptic
            ));

            Sdl.DisableScreenSaver();

            GamePad.InitDatabase();
            Window = m_gameWindow = gameWindow;
        }

        #endregion

        #region Manually-Implemented Properties

        public override GameRunBehavior DefaultRunBehavior
        {
            get { return GameRunBehavior.Synchronous; }
        }

        #endregion

        #region Methods

        public override void BeforeInitialize()
        {
            SdlRunLoop();

            base.BeforeInitialize();
        }

        public override bool BeforeUpdate(GameTime gameTime)
        {
            return true;
        }

        public override bool BeforeDraw(GameTime gameTime)
        {
            return true;
        }

        public override void EnterFullScreen() { }

        public override void ExitFullScreen() { }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
            m_gameWindow.BeginScreenDeviceChange(willBeFullScreen);
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
            m_gameWindow.EndScreenDeviceChange(screenDeviceName, clientWidth, clientHeight);
        }

        public override void StartRunLoop()
        {
            throw new NotSupportedException("The desktop platform does not support asynchronous run loops");
        }

        public override void RunLoop()
        {
            Sdl.Window.Show(Window.Handle);

            while (true)
            {
                SdlRunLoop();
                Game.Tick();
                Threading.Run();
                GraphicsDevice.DisposeContexts();

                if (m_isExiting > 0)
                    break;
            }
        }

        public override void Present()
        {
            if (Game.GraphicsDevice != null)
                Game.GraphicsDevice.Present();
        }

        public override void Exit()
        {
            Interlocked.Increment(ref m_isExiting);
        }

        protected override void OnMouseVisibleChanged()
        {
            m_gameWindow.SetCursorVisible(m_game.MouseVisible);
        }

        internal override void OnPresentationChanged(PresentationParameters pp)
        {
            int displayIndex = Sdl.Window.GetDisplayIndex(Window.Handle);
            string displayName = Sdl.Display.GetDisplayName(displayIndex);
            BeginScreenDeviceChange(pp.IsFullScreen);
            EndScreenDeviceChange(displayName, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        private void SdlRunLoop()
        {
            Sdl.Event ev;

            while (Sdl.PollEvent(out ev) == 1)
            {
                switch (ev.Type)
                {
                    case Sdl.EventType.Quit:
                        m_isExiting++;
                        break;

                    case Sdl.EventType.JoyDeviceAdded:
                        Joystick.AddDevice(ev.JoystickDevice.Which);
                        break;

                    case Sdl.EventType.JoyDeviceRemoved:
                        Joystick.RemoveDevice(ev.JoystickDevice.Which);
                        break;

                    case Sdl.EventType.ControllerDeviceRemoved:
                        GamePad.RemoveDevice(ev.ControllerDevice.Which);
                        break;

                    case Sdl.EventType.ControllerButtonUp:
                    case Sdl.EventType.ControllerButtonDown:
                    case Sdl.EventType.ControllerAxisMotion:
                        GamePad.UpdatePacketInfo(ev.ControllerDevice.Which, ev.ControllerDevice.TimeStamp);
                        break;

                    case Sdl.EventType.MouseWheel:
                        const int wheelDelta = 120;
                        Mouse.ScrollY += ev.Wheel.Y * wheelDelta;
                        Mouse.ScrollX += ev.Wheel.X * wheelDelta;
                        break;

                    case Sdl.EventType.MouseMotion:
                        Window.m_mouseState.X = ev.Motion.X;
                        Window.m_mouseState.Y = ev.Motion.Y;
                        break;

                    case Sdl.EventType.KeyDown:
                        {
                            Keys key = KeyboardUtil.ToXna(ev.Key.Keysym.Sym);
                            if (!m_keys.Contains(key))
                                m_keys.Add(key);
                            char character = (char)ev.Key.Keysym.Sym;
                            m_gameWindow.OnKeyDown(new InputKeyEventArgs(key));
                            if (char.IsControl(character))
                                m_gameWindow.OnTextTyped(new TextTypedEventArgs(character, key));
                        }
                        break;

                    case Sdl.EventType.KeyUp:
                        {
                            Keys key = KeyboardUtil.ToXna(ev.Key.Keysym.Sym);
                            m_keys.Remove(key);
                            m_gameWindow.OnKeyUp(new InputKeyEventArgs(key));
                        }
                        break;

                    case Sdl.EventType.TextInput:
                        if (m_gameWindow.HandleTextInput)
                        {
                            int len = 0;
                            int utf8character = 0; // using an int to encode multibyte characters longer than 2 bytes
                            byte currentByte = 0;
                            int charByteSize = 0; // UTF8 char lenght to decode
                            int remainingShift = 0;
                            unsafe
                            {
                                while ((currentByte = Marshal.ReadByte((IntPtr)ev.Text.Text, len)) != 0)
                                {
                                    // we're reading the first UTF8 byte, we need to check if it's multibyte
                                    if (charByteSize == 0)
                                    {
                                        if (currentByte < 192)
                                            charByteSize = 1;
                                        else if (currentByte < 224)
                                            charByteSize = 2;
                                        else if (currentByte < 240)
                                            charByteSize = 3;
                                        else
                                            charByteSize = 4;

                                        utf8character = 0;
                                        remainingShift = 4;
                                    }

                                    // assembling the character
                                    utf8character <<= 8;
                                    utf8character |= currentByte;

                                    charByteSize--;
                                    remainingShift--;

                                    if (charByteSize == 0) // finished decoding the current character
                                    {
                                        utf8character <<= remainingShift * 8; // shifting it to full UTF8 scope

                                        // SDL returns UTF8-encoded characters while C# char type is UTF16-encoded (and limited to the 0-FFFF range / does not support surrogate pairs)
                                        // so we need to convert it to Unicode codepoint and check if it's within the supported range
                                        int codepoint = UTF8ToUnicode(utf8character);

                                        if (codepoint >= 0 && codepoint < 0xFFFF)
                                        {
                                            m_gameWindow.OnTextTyped(new TextTypedEventArgs((char)codepoint, KeyboardUtil.ToXna(codepoint)));
                                            // UTF16 characters beyond 0xFFFF are not supported (and would require a surrogate encoding that is not supported by the char type)
                                        }
                                    }

                                    len++;
                                }
                            }
                        }
                        break;

                    case Sdl.EventType.WindowEvent:
                        switch (ev.Window.EventID)
                        {
                            case Sdl.Window.EventId.Resized:
                            case Sdl.Window.EventId.SizeChanged:
                                m_gameWindow.ClientResize(ev.Window.Data1, ev.Window.Data2);
                                break;
                            case Sdl.Window.EventId.FocusGained:
                                Active = true;
                                break;
                            case Sdl.Window.EventId.FocusLost:
                                Active = false;
                                break;
                            case Sdl.Window.EventId.Moved:
                                m_gameWindow.OnWindowMoved();
                                break;
                            case Sdl.Window.EventId.Close:
                                m_isExiting++;
                                break;
                        }
                        break;
                }
            }
        }

        private int UTF8ToUnicode(int utf8)
        {
            int
                byte4 = utf8 & 0xFF,
                byte3 = (utf8 >> 8) & 0xFF,
                byte2 = (utf8 >> 16) & 0xFF,
                byte1 = (utf8 >> 24) & 0xFF;

            if (byte1 < 0x80)
                return byte1;
            else if (byte1 < 0xC0)
                return -1;
            else if (byte1 < 0xE0 && byte2 >= 0x80 && byte2 < 0xC0)
                return (byte1 % 0x20) * 0x40 + (byte2 % 0x40);
            else if (byte1 < 0xF0 && byte2 >= 0x80 && byte2 < 0xC0 && byte3 >= 0x80 && byte3 < 0xC0)
                return (byte1 % 0x10) * 0x40 * 0x40 + (byte2 % 0x40) * 0x40 + (byte3 % 0x40);
            else if (byte1 < 0xF8 && byte2 >= 0x80 && byte2 < 0xC0 && byte3 >= 0x80 && byte3 < 0xC0 && byte4 >= 0x80 && byte4 < 0xC0)
                return (byte1 % 0x8) * 0x40 * 0x40 * 0x40 + (byte2 % 0x40) * 0x40 * 0x40 + (byte3 % 0x40) * 0x40 + (byte4 % 0x40);
            else
                return -1;
        }

        #endregion

        #region IDisposable implementation

        protected override void Dispose(bool disposing)
        {
            if (m_gameWindow != null)
            {
                m_gameWindow.Dispose();
                m_gameWindow = null;

                Joystick.CloseDevices();

                Sdl.Quit();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
