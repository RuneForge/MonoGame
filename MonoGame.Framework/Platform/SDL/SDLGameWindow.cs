// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

#error SdlGameWindow constuctor should be refactored so it's parameterless.
#error GamePlatform.m_game field should be refactored out.
#error Create IGraphicsDeviceManager, IGraphicsDeviceProvider and implement proper injection.

using System;
using System.IO;
using System.Reflection;

using Microsoft.Xna.Framework.Graphics;

using MonoGame.Framework.Utilities;

namespace Microsoft.Xna.Framework
{
    internal class SdlGameWindow : GameWindow, IDisposable
    {
        #region Fields

        public static GameWindow Instance;

        private readonly IntPtr m_icon;
        private IntPtr m_handle;
        private bool m_disposed;
        private bool m_resizable;
        private bool m_borderless;
        private bool m_willBeFullScreen;
        private bool m_mouseVisible;
        private bool m_hardwareSwitch;
        private string m_screenDeviceName;
        private int m_width;
        private int m_height;
        private bool m_wasMoved;
        private bool m_supressMoved;

        internal readonly Game m_game;

        public uint? Id;
        public bool IsFullScreen;

        #endregion

        #region Constructors, Destructors

        public SdlGameWindow(Game game)
        {
            m_game = game;
            m_screenDeviceName = "";

            Instance = this;

            m_width = GraphicsDeviceManager.DefaultBackBufferWidth;
            m_height = GraphicsDeviceManager.DefaultBackBufferHeight;

            Sdl.SetHint("SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS", "0");
            Sdl.SetHint("SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS", "1");

            // when running NUnit tests entry assembly can be null
            if (Assembly.GetEntryAssembly() != null)
            {
                using (
                    Stream stream =
                        Assembly.GetEntryAssembly().GetManifestResourceStream(Assembly.GetEntryAssembly().EntryPoint.DeclaringType.Namespace + ".Icon.bmp") ??
                        Assembly.GetEntryAssembly().GetManifestResourceStream("Icon.bmp") ??
                        Assembly.GetExecutingAssembly().GetManifestResourceStream("MonoGame.bmp"))
                {
                    if (stream != null)
                        using (BinaryReader br = new BinaryReader(stream))
                        {
                            try
                            {
                                IntPtr src = Sdl.RwFromMem(br.ReadBytes((int)stream.Length), (int)stream.Length);
                                m_icon = Sdl.LoadBMP_RW(src, 1);
                            }
                            catch { }
                        }
                }
            }

            m_handle = Sdl.Window.Create("", 0, 0,
                GraphicsDeviceManager.DefaultBackBufferWidth, GraphicsDeviceManager.DefaultBackBufferHeight,
                Sdl.Window.State.Hidden | Sdl.Window.State.FullscreenDesktop);
        }

        ~SdlGameWindow()
        {
            Dispose(false);
        }

        #endregion

        #region Manually-Implemented Properties

        public override IntPtr Handle
        {
            get { return m_handle; }
        }

        public override string ScreenDeviceName
        {
            get { return m_screenDeviceName; }
        }

        public override Rectangle ClientBounds
        {
            get
            {
                int x = 0, y = 0;
                Sdl.Window.GetPosition(Handle, out x, out y);
                return new Rectangle(x, y, m_width, m_height);
            }
        }

        public override DisplayOrientations CurrentOrientation
        {
            get { return DisplayOrientations.Default; }
        }

        public override bool AllowUserResizing
        {
            get { return !Borderless && m_resizable; }
            set
            {
                if (Sdl.Patch > 4)
                    Sdl.Window.SetResizable(m_handle, value);
                else
                    throw new Exception("SDL 2.0.4 does not support changing resizable parameter of the window after it's already been created, please use a newer version of it.");

                m_resizable = value;
            }
        }

        public override Point Position
        {
            get
            {
                int x = 0, y = 0;

                if (!IsFullScreen)
                    Sdl.Window.GetPosition(Handle, out x, out y);

                return new Point(x, y);
            }
            set
            {
                Sdl.Window.SetPosition(Handle, value.X, value.Y);
                m_wasMoved = true;
            }
        }

        public override bool Borderless
        {
            get { return m_borderless; }
            set
            {
                Sdl.Window.SetBordered(m_handle, value ? 0 : 1);
                m_borderless = value;
            }
        }

        #endregion

        #region Methods

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
            m_willBeFullScreen = willBeFullScreen;
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
            m_screenDeviceName = screenDeviceName;

            Rectangle prevBounds = ClientBounds;
            int displayIndex = Sdl.Window.GetDisplayIndex(Handle);

            Sdl.Rectangle displayRect;
            Sdl.Display.GetBounds(displayIndex, out displayRect);

            if (m_willBeFullScreen != IsFullScreen || m_hardwareSwitch != m_game.GraphicsDeviceManager.HardwareModeSwitch)
            {
                int fullscreenFlag = m_game.GraphicsDeviceManager.HardwareModeSwitch ? Sdl.Window.State.Fullscreen : Sdl.Window.State.FullscreenDesktop;
                Sdl.Window.SetFullscreen(Handle, (m_willBeFullScreen) ? fullscreenFlag : 0);
                m_hardwareSwitch = m_game.GraphicsDeviceManager.HardwareModeSwitch;
            }
            // If going to exclusive full-screen mode, force the window to minimize on focus loss (Windows only)
            if (CurrentPlatform.OS == OS.Windows)
            {
                Sdl.SetHint("SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS", m_willBeFullScreen && m_hardwareSwitch ? "1" : "0");
            }

            if (!m_willBeFullScreen || m_game.GraphicsDeviceManager.HardwareModeSwitch)
            {
                Sdl.Window.SetSize(Handle, clientWidth, clientHeight);
                m_width = clientWidth;
                m_height = clientHeight;
            }
            else
            {
                m_width = displayRect.Width;
                m_height = displayRect.Height;
            }

            int ignore, minx = 0, miny = 0;
            Sdl.Window.GetBorderSize(m_handle, out miny, out minx, out ignore, out ignore);

            int centerX = Math.Max(prevBounds.X + ((prevBounds.Width - clientWidth) / 2), minx);
            int centerY = Math.Max(prevBounds.Y + ((prevBounds.Height - clientHeight) / 2), miny);

            if (IsFullScreen && !m_willBeFullScreen)
            {
                // We need to get the display information again in case
                // the resolution of it was changed.
                Sdl.Display.GetBounds(displayIndex, out displayRect);

                // This centering only occurs when exiting fullscreen
                // so it should center the window on the current display.
                centerX = displayRect.X + displayRect.Width / 2 - clientWidth / 2;
                centerY = displayRect.Y + displayRect.Height / 2 - clientHeight / 2;
            }

            // If this window is resizable, there is a bug in SDL 2.0.4 where
            // after the window gets resized, window position information
            // becomes wrong (for me it always returned 10 8). Solution is
            // to not try and set the window position because it will be wrong.
            if ((Sdl.Patch > 4 || !AllowUserResizing) && !m_wasMoved)
                Sdl.Window.SetPosition(Handle, centerX, centerY);

            if (IsFullScreen != m_willBeFullScreen)
                OnClientSizeChanged();

            IsFullScreen = m_willBeFullScreen;

            m_supressMoved = true;
        }

        public void ClientResize(int width, int height)
        {
            // SDL reports many resize events even if the Size didn't change.
            // Only call the code below if it actually changed.
            if (m_game.GraphicsDevice.PresentationParameters.BackBufferWidth == width &&
                m_game.GraphicsDevice.PresentationParameters.BackBufferHeight == height)
            {
                return;
            }
            m_game.GraphicsDevice.PresentationParameters.BackBufferWidth = width;
            m_game.GraphicsDevice.PresentationParameters.BackBufferHeight = height;
            m_game.GraphicsDevice.Viewport = new Viewport(0, 0, width, height);

            Sdl.Window.GetSize(Handle, out m_width, out m_height);

            OnClientSizeChanged();
        }

        public void SetCursorVisible(bool visible)
        {
            m_mouseVisible = visible;
            Sdl.Mouse.ShowCursor(visible ? 1 : 0);
        }

        protected override void OnBeforeTitleSet(string title)
        {
            Sdl.Window.SetTitle(m_handle, title);
        }

        protected internal override void SetSupportedOrientations(DisplayOrientations orientations) { }

        internal void CreateWindow()
        {
            int initflags =
                Sdl.Window.State.OpenGL |
                Sdl.Window.State.Hidden |
                Sdl.Window.State.InputFocus |
                Sdl.Window.State.MouseFocus;

            if (m_handle != IntPtr.Zero)
                Sdl.Window.Destroy(m_handle);

            int winx = Sdl.Window.PosCentered;
            int winy = Sdl.Window.PosCentered;

            // if we are on Linux, start on the current screen
            if (CurrentPlatform.OS == OS.Linux)
            {
                winx |= GetMouseDisplay();
                winy |= GetMouseDisplay();
            }

            m_handle = Sdl.Window.Create(AssemblyHelper.GetDefaultWindowTitle(),
                winx, winy, m_width, m_height, initflags);

            Id = Sdl.Window.GetWindowId(m_handle);

            if (m_icon != IntPtr.Zero)
                Sdl.Window.SetIcon(m_handle, m_icon);

            Sdl.Window.SetBordered(m_handle, m_borderless ? 0 : 1);
            Sdl.Window.SetResizable(m_handle, m_resizable);

            SetCursorVisible(m_mouseVisible);
        }

        internal void OnWindowMoved()
        {
            if (m_supressMoved)
            {
                m_supressMoved = false;
                return;
            }

            m_wasMoved = true;
        }

        private static int GetMouseDisplay()
        {
            Sdl.Rectangle rect = new Sdl.Rectangle();

            int x, y;
            Sdl.Mouse.GetGlobalState(out x, out y);

            int displayCount = Sdl.Display.GetNumVideoDisplays();
            for (int i = 0; i < displayCount; i++)
            {
                Sdl.Display.GetBounds(i, out rect);

                if (x >= rect.X && x < rect.X + rect.Width &&
                    y >= rect.Y && y < rect.Y + rect.Height)
                {
                    return i;
                }
            }

            return 0;
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            Sdl.Window.Destroy(m_handle);
            m_handle = IntPtr.Zero;

            if (m_icon != IntPtr.Zero)
                Sdl.FreeSurface(m_icon);

            m_disposed = true;
        }

        #endregion
    }
}
