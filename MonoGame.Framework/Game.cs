// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

#if WINDOWS_UAP
using System.Threading.Tasks;

using Windows.ApplicationModel.Activation;
#endif

#error Game.Services should be refactored out as well as the field. .NET Standard DI container should be used instead.
#error Get rid of GameServiceContainer dependency in the ContentManager type.

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// This class is the entry point for most games. Handles setting up
    /// a window and graphics and runs a game loop that calls <see cref="Update"/> and <see cref="Draw"/>.
    /// </summary>
    public partial class Game : IDisposable
    {
        #region Fields

        private static readonly Action<IUpdateable, GameTime> s_updateAction = (updateable, gameTime) => updateable.Update(gameTime);
        private static readonly Action<IDrawable, GameTime> s_drawAction = (drawable, gameTime) => drawable.Draw(gameTime);

        private static TimeSpan s_targetElapsedTime = TimeSpan.FromTicks(166667); // frame time for 60fps
        private static TimeSpan s_inactiveSleepTime = TimeSpan.FromSeconds(0.02);
        private static TimeSpan s_maxElapsedTime = TimeSpan.FromMilliseconds(500);
        private static Game s_instance = null;

        private readonly IServiceProvider m_serviceProvider;
        private readonly GameWindow m_gameWindow;
        private readonly SortingFilteringCollection<IUpdateable> m_updateableComponents;
        private readonly SortingFilteringCollection<IDrawable> m_drawableComponents;
        private readonly GameServiceContainer m_services;
        private readonly GameTime m_gameTime;
        private GameComponentCollection m_components;
        private IGraphicsDeviceManager m_graphicsDeviceManager;
        private IGraphicsDeviceService m_graphicsDeviceService;
        private ContentManager m_contentManager;
        private Stopwatch m_gameTimer;
        private TimeSpan m_accumulatedElapsedTime;
        private long m_previousTicks = 0;
        private int m_updateFrameLag = 0;
        private bool m_initialized = false;
        private bool m_fixedTimeStep = true;
        private bool m_shouldExit;
        private bool m_suppressDraw;
        private bool m_disposed;

        internal GamePlatform Platform;

#if WINDOWS_UAP
        private readonly object m_locker = new object();
#endif

        #endregion

        #region Auto-Implemented Properties

        /// <summary>
        /// The start up parameters for this <see cref="Game"/>.
        /// </summary>
        public LaunchParameterCollection LaunchParameters { get; private set; }

#if ANDROID
        [CLSCompliant(false)]
        public static AndroidGameActivity Activity { get; internal set; }
#endif

        #endregion

        #region Constructors, Destructors

        /// <summary>
        /// Creates an instance of the <see cref="Game"/> type.
        /// </summary>
        public Game(IServiceProvider serviceProvider, GameWindow gameWindow)
        {
            s_instance = this;

            LaunchParameters = new LaunchParameterCollection();

            m_serviceProvider = serviceProvider;
            m_gameWindow = gameWindow;
            m_services = new GameServiceContainer();
            m_gameTime = new GameTime();
            m_components = new GameComponentCollection();
            m_contentManager = new ContentManager(m_services);

            m_updateableComponents = new SortingFilteringCollection<IUpdateable>(
                u => u.Enabled,
                (u, handler) => u.EnabledChanged += handler,
                (u, handler) => u.EnabledChanged -= handler,
                (u1, u2) => Comparer<int>.Default.Compare(u1.UpdateOrder, u2.UpdateOrder),
                (u, handler) => u.UpdateOrderChanged += handler,
                (u, handler) => u.UpdateOrderChanged -= handler
                );
            m_drawableComponents = new SortingFilteringCollection<IDrawable>(
                d => d.Visible,
                (d, handler) => d.VisibleChanged += handler,
                (d, handler) => d.VisibleChanged -= handler,
                (d1, d2) => Comparer<int>.Default.Compare(d1.DrawOrder, d2.DrawOrder),
                (d, handler) => d.DrawOrderChanged += handler,
                (d, handler) => d.DrawOrderChanged -= handler
                );

            Platform = GamePlatform.CreatePlatform(serviceProvider, this);
            Platform.Activated += OnActivated;
            Platform.Deactivated += OnDeactivated;
            m_services.AddService(typeof(GamePlatform), Platform);

            // Calling Update() for first time initializes some systems
            FrameworkDispatcher.Update();

            // Allow some optional per-platform construction to occur too.
            PlatformConstruct();

        }

        /// <summary>
        /// Releases unmanaged resources.
        /// </summary>
        ~Game()
        {
            Dispose(false);
        }

        #endregion

        #region Manually-Implemented Properties

        /// <summary>
        /// A collection of game components attached to this <see cref="Game"/>.
        /// </summary>
        public GameComponentCollection Components { get { return m_components; } }

        /// <summary>
        /// Get a container holding service providers attached to this <see cref="Game"/>.
        /// </summary>
        public GameServiceContainer Services { get { return m_services; } }

        /// <summary>
        /// The system window that this game is displayed on.
        /// </summary>
        [CLSCompliant(false)]
        public GameWindow Window { get { return Platform.Window; } }

        /// <summary>
        /// The <see cref="Content.ContentManager"/> of this <see cref="Game"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">If Content is set to <code>null</code>.</exception>
        public ContentManager ContentManager
        {
            get { return m_contentManager; }
            set { m_contentManager = value ?? throw new ArgumentNullException(); }
        }

        /// <summary>
        /// Gets the <see cref="GraphicsDevice"/> used for rendering by this <see cref="Game"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// There is no <see cref="Graphics.GraphicsDevice"/> attached to this <see cref="Game"/>.
        /// </exception>
        public GraphicsDevice GraphicsDevice
        {
            get
            {
                if (m_graphicsDeviceService == null)
                {
                    m_graphicsDeviceService = (IGraphicsDeviceService)Services.GetService(typeof(IGraphicsDeviceService));

                    if (m_graphicsDeviceService == null)
                        throw new InvalidOperationException("No Graphics Device Service");
                }
                return m_graphicsDeviceService.GraphicsDevice;
            }
        }

        /// <summary>
        /// The time between ticks when the game is in inactive state.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Inactive sleep time must be larger or equal to zero.</exception>
        public TimeSpan InactiveSleepTime
        {
            get { return s_inactiveSleepTime; }
            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("The time must be positive.", default(Exception));

                s_inactiveSleepTime = value;
            }
        }

        /// <summary>
        /// The maximum amount of time we will frameskip over and only perform Update calls with no Draw calls.
        /// MonoGame extension.
        /// </summary>
        public TimeSpan MaxElapsedTime
        {
            get { return s_maxElapsedTime; }
            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("The time must be positive.", default(Exception));
                if (value < s_targetElapsedTime)
                    throw new ArgumentOutOfRangeException("The time must be at least TargetElapsedTime", default(Exception));

                s_maxElapsedTime = value;
            }
        }

        /// <summary>
        /// The time between frames when running with a fixed time step. <seealso cref="FixedTimeStep"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Target elapsed time must be strictly larger than zero.</exception>
        public TimeSpan TargetElapsedTime
        {
            get { return s_targetElapsedTime; }
            set
            {
                // Give GamePlatform implementations an opportunity to override
                // the new value.
                value = Platform.TargetElapsedTimeChanging(value);

                if (value <= TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("The time must be positive and non-zero.", default(Exception));

                if (value != s_targetElapsedTime)
                {
                    s_targetElapsedTime = value;
                    Platform.TargetElapsedTimeChanged();
                }
            }
        }

        /// <summary>
        /// Indicates if the game is the focused application.
        /// </summary>
        public bool Active
        {
            get { return Platform.Active; }
        }

        /// <summary>
        /// Indicates if the mouse cursor is visible on the game screen.
        /// </summary>
        public bool MouseVisible
        {
            get { return Platform.MouseVisible; }
            set { Platform.MouseVisible = value; }
        }

        /// <summary>
        /// Indicates if this game is running with a fixed time between frames.
        /// 
        /// When set to <code>true</code> the target time between frames is
        /// given by <see cref="TargetElapsedTime"/>.
        /// </summary>
        public bool FixedTimeStep
        {
            get { return m_fixedTimeStep; }
            set { m_fixedTimeStep = value; }
        }

        internal static Game Instance { get { return s_instance; } }

        /// <remarks>
        /// FIXME: Internal members should be eliminated.
        /// Currently Game.Initialized is used by the Mac game window class to
        /// determine whether to raise DeviceResetting and DeviceReset on GraphicsDeviceManager.
        /// </remarks>
        internal bool Initialized { get { return m_initialized; } }

        internal GraphicsDeviceManager GraphicsDeviceManager
        {
            get
            {
                if (m_graphicsDeviceManager == null)
                {
                    m_graphicsDeviceManager = (IGraphicsDeviceManager)Services.GetService(typeof(IGraphicsDeviceManager));
                }
                return (GraphicsDeviceManager)m_graphicsDeviceManager;
            }
            set
            {
                if (m_graphicsDeviceManager != null)
                    throw new InvalidOperationException("GraphicsDeviceManager already registered for this Game object");
                m_graphicsDeviceManager = value;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the game gains focus.
        /// </summary>
        public event EventHandler<EventArgs> Activated;

        /// <summary>
        /// Raised when the game loses focus.
        /// </summary>
        public event EventHandler<EventArgs> Deactivated;

        /// <summary>
        /// Raised when this game is being disposed.
        /// </summary>
        public event EventHandler<EventArgs> Disposed;

        /// <summary>
        /// Raised when this game is exiting.
        /// </summary>
        public event EventHandler<EventArgs> Exiting;

#if WINDOWS_UAP
        [CLSCompliant(false)]
        public ApplicationExecutionState PreviousExecutionState { get; internal set; }
#endif

        #endregion

        #region Methods

        /// <summary>
        /// Exit the game at the end of this tick.
        /// </summary>
#if IOS
        [Obsolete("This platform's policy does not allow programmatically closing.", true)]
#endif
        public void Exit()
        {
            m_shouldExit = true;
            m_suppressDraw = true;
        }

        /// <summary>
        /// Reset the elapsed game time to <see cref="TimeSpan.Zero"/>.
        /// </summary>
        public void ResetElapsedTime()
        {
            Platform.ResetElapsedTime();
            m_gameTimer.Reset();
            m_gameTimer.Start();
            m_accumulatedElapsedTime = TimeSpan.Zero;
            m_gameTime.ElapsedGameTime = TimeSpan.Zero;
            m_previousTicks = 0L;
        }

        /// <summary>
        /// Supress calling <see cref="Draw"/> in the game loop.
        /// </summary>
        public void SuppressDraw()
        {
            m_suppressDraw = true;
        }

        /// <summary>
        /// Run the game for one frame, then exit.
        /// </summary>
        public void RunOneFrame()
        {
            if (Platform == null)
                return;

            if (!Platform.BeforeRun())
                return;

            if (!m_initialized)
            {
                DoInitialize();
                m_gameTimer = Stopwatch.StartNew();
                m_initialized = true;
            }

            BeginRun();

            //Not quite right..
            Tick();

            EndRun();
        }

        /// <summary>
        /// Run the game using the default <see cref="GameRunBehavior"/> for the current platform.
        /// </summary>
        public void Run()
        {
            Run(Platform.DefaultRunBehavior);
        }

        /// <summary>
        /// Run the game.
        /// </summary>
        /// <param name="runBehavior">Indicate if the game should be run synchronously or asynchronously.</param>
        public void Run(GameRunBehavior runBehavior)
        {
            AssertNotDisposed();
            if (!Platform.BeforeRun())
            {
                BeginRun();
                m_gameTimer = Stopwatch.StartNew();
                return;
            }

            if (!m_initialized)
            {
                DoInitialize();
                m_initialized = true;
            }

            BeginRun();
            m_gameTimer = Stopwatch.StartNew();
            switch (runBehavior)
            {
                case GameRunBehavior.Asynchronous:
                    Platform.AsyncRunLoopEnded += OnAsyncRunLoopEnded;
                    Platform.StartRunLoop();
                    break;
                case GameRunBehavior.Synchronous:
                    // XNA runs one Update even before showing the window
                    DoUpdate(new GameTime());

                    Platform.RunLoop();
                    EndRun();
                    DoExit();
                    break;
                default:
                    throw new ArgumentException(string.Format(
                        "Handling for the run behavior {0} is not implemented.", runBehavior));
            }
        }

        /// <summary>
        /// Run one iteration of the game loop.
        ///
        /// Makes at least one call to <see cref="Update"/>
        /// and exactly one call to <see cref="Draw"/> if drawing is not supressed.
        /// When <see cref="FixedTimeStep"/> is set to <code>false</code> this will
        /// make exactly one call to <see cref="Update"/>.
        /// </summary>
        public void Tick()
        {
        // NOTE: This code is very sensitive and can break very badly
        // with even what looks like a safe change.  Be sure to test 
        // any change fully in both the fixed and variable timestep 
        // modes across multiple devices and platforms.

        RetryTick:

            if (!Active && (InactiveSleepTime.TotalMilliseconds >= 1.0))
            {
#if WINDOWS_UAP
                lock (m_locker)
                    System.Threading.Monitor.Wait(m_locker, (int)InactiveSleepTime.TotalMilliseconds);
#else
                System.Threading.Thread.Sleep((int)InactiveSleepTime.TotalMilliseconds);
#endif
            }

            // Advance the accumulated elapsed time.
            long currentTicks = m_gameTimer.Elapsed.Ticks;
            m_accumulatedElapsedTime += TimeSpan.FromTicks(currentTicks - m_previousTicks);
            m_previousTicks = currentTicks;

            if (FixedTimeStep && m_accumulatedElapsedTime < TargetElapsedTime)
            {
                // Sleep for as long as possible without overshooting the update time
                double sleepTime = (TargetElapsedTime - m_accumulatedElapsedTime).TotalMilliseconds;
                // We only have a precision timer on Windows, so other platforms may still overshoot
#if WINDOWS && !DESKTOPGL
                MonoGame.Framework.Utilities.TimerHelper.SleepForNoMoreThan(sleepTime);
#elif WINDOWS_UAP
                lock (m_locker)
                {
                    if (sleepTime >= 2.0)
                        System.Threading.Monitor.Wait(m_locker, 1);
                }
#elif DESKTOPGL || ANDROID || IOS
                if (sleepTime >= 2.0)
                    System.Threading.Thread.Sleep(1);
#endif
                // Keep looping until it's time to perform the next update
                goto RetryTick;
            }

            // Do not allow any update to take longer than our maximum.
            if (m_accumulatedElapsedTime > s_maxElapsedTime)
                m_accumulatedElapsedTime = s_maxElapsedTime;

            if (FixedTimeStep)
            {
                m_gameTime.ElapsedGameTime = TargetElapsedTime;
                int stepCount = 0;

                // Perform as many full fixed length time steps as we can.
                while (m_accumulatedElapsedTime >= TargetElapsedTime && !m_shouldExit)
                {
                    m_gameTime.TotalGameTime += TargetElapsedTime;
                    m_accumulatedElapsedTime -= TargetElapsedTime;
                    ++stepCount;

                    DoUpdate(m_gameTime);
                }

                //Every update after the first accumulates lag
                m_updateFrameLag += Math.Max(0, stepCount - 1);

                //If we think we are running slowly, wait until the lag clears before resetting it
                if (m_gameTime.IsRunningSlowly)
                {
                    if (m_updateFrameLag == 0)
                        m_gameTime.IsRunningSlowly = false;
                }
                else if (m_updateFrameLag >= 5)
                {
                    //If we lag more than 5 frames, start thinking we are running slowly
                    m_gameTime.IsRunningSlowly = true;
                }

                //Every time we just do one update and one draw, then we are not running slowly, so decrease the lag
                if (stepCount == 1 && m_updateFrameLag > 0)
                    m_updateFrameLag--;

                // Draw needs to know the total elapsed time
                // that occured for the fixed length updates.
                m_gameTime.ElapsedGameTime = TimeSpan.FromTicks(TargetElapsedTime.Ticks * stepCount);
            }
            else
            {
                // Perform a single variable length update.
                m_gameTime.ElapsedGameTime = m_accumulatedElapsedTime;
                m_gameTime.TotalGameTime += m_accumulatedElapsedTime;
                m_accumulatedElapsedTime = TimeSpan.Zero;

                DoUpdate(m_gameTime);
            }

            // Draw unless the update suppressed it.
            if (m_suppressDraw)
                m_suppressDraw = false;
            else
            {
                DoDraw(m_gameTime);
            }

            if (m_shouldExit)
            {
                Platform.Exit();
                m_shouldExit = false; //prevents perpetual exiting on platforms supporting resume.
            }
        }

        /// <summary>
        /// Called right before <see cref="Draw"/> is normally called. Can return <code>false</code>
        /// to let the game loop not call <see cref="Draw"/>.
        /// </summary>
        /// <returns>
        /// <code>true</code> if <see cref="Draw"/> should be called, <code>false</code> if it should not.
        /// </returns>
        protected virtual bool BeginDraw()
        {
            return true;
        }

        /// <summary>
        /// Called right after <see cref="Draw"/>. Presents the rendered frame in the <see cref="GameWindow"/>.
        /// </summary>
        protected virtual void EndDraw()
        {
            Platform.Present();
        }

        /// <summary>
        /// Called after <see cref="Initialize"/>, but before the first call to <see cref="Update"/>.
        /// </summary>
        protected virtual void BeginRun() { }

        /// <summary>
        /// Called when the game loop has been terminated before exiting.
        /// </summary>
        protected virtual void EndRun() { }

        /// <summary>
        /// Override this to load graphical resources required by the game.
        /// </summary>
        protected virtual void LoadContent() { }

        /// <summary>
        /// Override this to unload graphical resources loaded by the game.
        /// </summary>
        protected virtual void UnloadContent() { }

        /// <summary>
        /// Override this to initialize the game and load any needed non-graphical resources.
        ///
        /// Initializes attached <see cref="GameComponent"/> instances and calls <see cref="LoadContent"/>.
        /// </summary>
        protected virtual void Initialize()
        {
            // TODO: This should be removed once all platforms use the new GraphicsDeviceManager
#if !(WINDOWS && DIRECTX)
            ApplyChanges(GraphicsDeviceManager);
#endif

            // According to the information given on MSDN (see link below), all
            // GameComponents in Components at the time Initialize() is called
            // are initialized.
            // http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.game.initialize.aspx
            // Initialize all existing components
            InitializeExistingComponents();

            m_graphicsDeviceService = (IGraphicsDeviceService)
                Services.GetService(typeof(IGraphicsDeviceService));

            if (m_graphicsDeviceService != null &&
                m_graphicsDeviceService.GraphicsDevice != null)
            {
                LoadContent();
            }
        }

        /// <summary>
        /// Called when the game should update.
        ///
        /// Updates the <see cref="GameComponent"/> instances attached to this game.
        /// Override this to update your game.
        /// </summary>
        /// <param name="gameTime">The elapsed time since the last call to <see cref="Update"/>.</param>
        protected virtual void Update(GameTime gameTime)
        {
            m_updateableComponents.ForEachFilteredItem(s_updateAction, gameTime);
        }

        /// <summary>
        /// Called when the game should draw a frame.
        ///
        /// Draws the <see cref="DrawableGameComponent"/> instances attached to this game.
        /// Override this to render your game.
        /// </summary>
        /// <param name="gameTime">A <see cref="GameTime"/> instance containing the elapsed time since the last call to <see cref="Draw"/> and the total time elapsed since the game started.</param>
        protected virtual void Draw(GameTime gameTime)
        {
            m_drawableComponents.ForEachFilteredItem(s_drawAction, gameTime);
        }

        /// <summary>
        /// Called when the game is exiting. Raises the <see cref="Exiting"/> event.
        /// </summary>
        /// <param name="sender">This <see cref="Game"/>.</param>
        /// <param name="args">The arguments to the <see cref="Exiting"/> event.</param>
        protected virtual void OnExiting(object sender, EventArgs args)
        {
            EventHelpers.Raise(sender, Exiting, args);
        }

        /// <summary>
        /// Called when the game gains focus. Raises the <see cref="Activated"/> event.
        /// </summary>
        /// <param name="sender">This <see cref="Game"/>.</param>
        /// <param name="args">The arguments to the <see cref="Activated"/> event.</param>
        protected virtual void OnActivated(object sender, EventArgs args)
        {
            AssertNotDisposed();
            EventHelpers.Raise(sender, Activated, args);
        }

        /// <summary>
        /// Called when the game loses focus. Raises the <see cref="Deactivated"/> event.
        /// </summary>
        /// <param name="sender">This <see cref="Game"/>.</param>
        /// <param name="args">The arguments to the <see cref="Deactivated"/> event.</param>
        protected virtual void OnDeactivated(object sender, EventArgs args)
        {
            AssertNotDisposed();
            EventHelpers.Raise(sender, Deactivated, args);
        }

        private void OnComponentAdded(object sender, GameComponentCollectionEventArgs e)
        {
            // Since we only subscribe to ComponentAdded after the graphics
            // devices are set up, it is safe to just blindly call Initialize.
            e.GameComponent.Initialize();
            CategorizeComponent(e.GameComponent);
        }

        private void OnComponentRemoved(object sender, GameComponentCollectionEventArgs e)
        {
            DecategorizeComponent(e.GameComponent);
        }

        private void OnAsyncRunLoopEnded(object sender, EventArgs e)
        {
            AssertNotDisposed();

            GamePlatform platform = (GamePlatform)sender;
            platform.AsyncRunLoopEnded -= OnAsyncRunLoopEnded;
            EndRun();
            DoExit();
        }

        /// <remarks>
        /// InitializeExistingComponents really should only be called once.
        /// Game.Initialize is the only method in a position to guarantee
        /// that no component will get a duplicate Initialize call.
        /// Further calls to Initialize occur immediately in response to Components.ComponentAdded.
        /// </remarks>
        private void InitializeExistingComponents()
        {
            for (int i = 0; i < Components.Count; ++i)
                Components[i].Initialize();
        }

        private void CategorizeComponents()
        {
            DecategorizeComponents();
            for (int i = 0; i < Components.Count; ++i)
                CategorizeComponent(Components[i]);
        }

        private void DecategorizeComponents()
        {
            m_updateableComponents.Clear();
            m_drawableComponents.Clear();
        }

        private void CategorizeComponent(IGameComponent component)
        {
            if (component is IUpdateable)
                m_updateableComponents.Add((IUpdateable)component);
            if (component is IDrawable)
                m_drawableComponents.Add((IDrawable)component);
        }

        private void DecategorizeComponent(IGameComponent component)
        {
            if (component is IUpdateable)
                m_updateableComponents.Remove((IUpdateable)component);
            if (component is IDrawable)
                m_drawableComponents.Remove((IDrawable)component);
        }

        partial void PlatformConstruct();

        internal void DoUpdate(GameTime gameTime)
        {
            AssertNotDisposed();
            if (Platform.BeforeUpdate(gameTime))
            {
                FrameworkDispatcher.Update();

                Update(gameTime);

                //The TouchPanel needs to know the time for when touches arrive
                TouchPanelState.CurrentTimestamp = gameTime.TotalGameTime;
            }
        }

        internal void DoDraw(GameTime gameTime)
        {
            AssertNotDisposed();
            // Draw and EndDraw should not be called if BeginDraw returns false.
            // http://stackoverflow.com/questions/4054936/manual-control-over-when-to-redraw-the-screen/4057180#4057180
            // http://stackoverflow.com/questions/4235439/xna-3-1-to-4-0-requires-constant-redraw-or-will-display-a-purple-screen
            if (Platform.BeforeDraw(gameTime) && BeginDraw())
            {
                Draw(gameTime);
                EndDraw();
            }
        }

        internal void DoInitialize()
        {
            AssertNotDisposed();
            if (GraphicsDevice == null && GraphicsDeviceManager != null)
                m_graphicsDeviceManager.CreateDevice();

            Platform.BeforeInitialize();
            Initialize();

            // We need to do this after virtual Initialize(...) is called.
            // 1. Categorize components into IUpdateable and IDrawable lists.
            // 2. Subscribe to Added/Removed events to keep the categorized
            //    lists synced and to Initialize future components as they are
            //    added.            
            CategorizeComponents();
            m_components.ComponentAdded += OnComponentAdded;
            m_components.ComponentRemoved += OnComponentRemoved;
        }

        internal void DoExit()
        {
            OnExiting(this, EventArgs.Empty);
            UnloadContent();
        }

#if !(WINDOWS && DIRECTX)

        /// <remarks>
        /// We should work toward eliminating internal methods. They break entirely the possibility that additional platforms
        /// could be added by third parties without changing MonoGame itself.
        /// </remarks>
        internal void ApplyChanges(GraphicsDeviceManager manager)
        {
            Platform.BeginScreenDeviceChange(GraphicsDevice.PresentationParameters.IsFullScreen);

            if (GraphicsDevice.PresentationParameters.IsFullScreen)
                Platform.EnterFullScreen();
            else
                Platform.ExitFullScreen();
            Viewport viewport = new Viewport(0, 0,
                                        GraphicsDevice.PresentationParameters.BackBufferWidth,
                                        GraphicsDevice.PresentationParameters.BackBufferHeight);

            GraphicsDevice.Viewport = viewport;
            Platform.EndScreenDeviceChange(string.Empty, viewport.Width, viewport.Height);
        }

#endif

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            EventHelpers.Raise(this, Disposed, EventArgs.Empty);
        }

        /// <summary>
        /// Releases unmanaged resources.
        /// </summary>
        /// <param name="disposing">The value indicating whether managed resources should be released.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    // Dispose loaded game components
                    for (int i = 0; i < m_components.Count; i++)
                    {
                        IDisposable disposable = m_components[i] as IDisposable;
                        if (disposable != null)
                            disposable.Dispose();
                    }
                    m_components = null;

                    if (m_contentManager != null)
                    {
                        m_contentManager.Dispose();
                        m_contentManager = null;
                    }

                    if (m_graphicsDeviceManager != null)
                    {
                        (m_graphicsDeviceManager as GraphicsDeviceManager)?.Dispose();
                        m_graphicsDeviceManager = null;
                    }

                    if (Platform != null)
                    {
                        Platform.Activated -= OnActivated;
                        Platform.Deactivated -= OnDeactivated;
                        m_services.RemoveService(typeof(GamePlatform));

                        Platform.Dispose();
                        Platform = null;
                    }

                    ContentTypeReaderManager.ClearTypeCreators();

                    if (SoundEffect._systemState == SoundEffect.SoundSystemState.Initialized)
                        SoundEffect.PlatformShutdown();
                }
#if ANDROID
                Activity = null;
#endif
                m_disposed = true;
                s_instance = null;
            }
        }

        [DebuggerNonUserCode]
        private void AssertNotDisposed()
        {
            if (m_disposed)
            {
                string name = GetType().Name;
                throw new ObjectDisposedException(
                    name, string.Format("The {0} object was used after being Disposed.", name));
            }
        }

        #endregion IDisposable Implementation

        #region Nested Types

        /// <summary>
        /// The SortingFilteringCollection class provides efficient, reusable
        /// sorting and filtering based on a configurable sort comparer, filter
        /// predicate, and associate change events.
        /// </summary>
        private class SortingFilteringCollection<T> : ICollection<T>
        {
            private readonly List<T> _items;
            private readonly List<AddJournalEntry<T>> _addJournal;
            private readonly Comparison<AddJournalEntry<T>> _addJournalSortComparison;
            private readonly List<int> _removeJournal;
            private readonly List<T> _cachedFilteredItems;
            private bool _shouldRebuildCache;

            private readonly Predicate<T> _filter;
            private readonly Comparison<T> _sort;
            private readonly Action<T, EventHandler<EventArgs>> _filterChangedSubscriber;
            private readonly Action<T, EventHandler<EventArgs>> _filterChangedUnsubscriber;
            private readonly Action<T, EventHandler<EventArgs>> _sortChangedSubscriber;
            private readonly Action<T, EventHandler<EventArgs>> _sortChangedUnsubscriber;

            public SortingFilteringCollection(
                Predicate<T> filter,
                Action<T, EventHandler<EventArgs>> filterChangedSubscriber,
                Action<T, EventHandler<EventArgs>> filterChangedUnsubscriber,
                Comparison<T> sort,
                Action<T, EventHandler<EventArgs>> sortChangedSubscriber,
                Action<T, EventHandler<EventArgs>> sortChangedUnsubscriber)
            {
                _items = new List<T>();
                _addJournal = new List<AddJournalEntry<T>>();
                _removeJournal = new List<int>();
                _cachedFilteredItems = new List<T>();
                _shouldRebuildCache = true;

                _filter = filter;
                _filterChangedSubscriber = filterChangedSubscriber;
                _filterChangedUnsubscriber = filterChangedUnsubscriber;
                _sort = sort;
                _sortChangedSubscriber = sortChangedSubscriber;
                _sortChangedUnsubscriber = sortChangedUnsubscriber;

                _addJournalSortComparison = CompareAddJournalEntry;
            }

            private int CompareAddJournalEntry(AddJournalEntry<T> x, AddJournalEntry<T> y)
            {
                int result = _sort(x.Item, y.Item);
                if (result != 0)
                    return result;
                return x.Order - y.Order;
            }

            public void ForEachFilteredItem<TUserData>(Action<T, TUserData> action, TUserData userData)
            {
                if (_shouldRebuildCache)
                {
                    ProcessRemoveJournal();
                    ProcessAddJournal();

                    // Rebuild the cache
                    _cachedFilteredItems.Clear();
                    for (int i = 0; i < _items.Count; ++i)
                        if (_filter(_items[i]))
                            _cachedFilteredItems.Add(_items[i]);

                    _shouldRebuildCache = false;
                }

                for (int i = 0; i < _cachedFilteredItems.Count; ++i)
                    action(_cachedFilteredItems[i], userData);

                // If the cache was invalidated as a result of processing items,
                // now is a good time to clear it and give the GC (more of) a
                // chance to do its thing.
                if (_shouldRebuildCache)
                    _cachedFilteredItems.Clear();
            }

            public void Add(T item)
            {
                // NOTE: We subscribe to item events after items in _addJournal
                //       have been merged.
                _addJournal.Add(new AddJournalEntry<T>(_addJournal.Count, item));
                InvalidateCache();
            }

            public bool Remove(T item)
            {
                if (_addJournal.Remove(AddJournalEntry<T>.CreateKey(item)))
                    return true;

                int index = _items.IndexOf(item);
                if (index >= 0)
                {
                    UnsubscribeFromItemEvents(item);
                    _removeJournal.Add(index);
                    InvalidateCache();
                    return true;
                }
                return false;
            }

            public void Clear()
            {
                for (int i = 0; i < _items.Count; ++i)
                {
                    _filterChangedUnsubscriber(_items[i], Item_FilterPropertyChanged);
                    _sortChangedUnsubscriber(_items[i], Item_SortPropertyChanged);
                }

                _addJournal.Clear();
                _removeJournal.Clear();
                _items.Clear();

                InvalidateCache();
            }

            public bool Contains(T item)
            {
                return _items.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                _items.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return _items.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((System.Collections.IEnumerable)_items).GetEnumerator();
            }

            private static readonly Comparison<int> RemoveJournalSortComparison =
                (x, y) => Comparer<int>.Default.Compare(y, x); // Sort high to low
            private void ProcessRemoveJournal()
            {
                if (_removeJournal.Count == 0)
                    return;

                // Remove items in reverse.  (Technically there exist faster
                // ways to bulk-remove from a variable-length array, but List<T>
                // does not provide such a method.)
                _removeJournal.Sort(RemoveJournalSortComparison);
                for (int i = 0; i < _removeJournal.Count; ++i)
                    _items.RemoveAt(_removeJournal[i]);
                _removeJournal.Clear();
            }

            private void ProcessAddJournal()
            {
                if (_addJournal.Count == 0)
                    return;

                // Prepare the _addJournal to be merge-sorted with _items.
                // _items is already sorted (because it is always sorted).
                _addJournal.Sort(_addJournalSortComparison);

                int iAddJournal = 0;
                int iItems = 0;

                while (iItems < _items.Count && iAddJournal < _addJournal.Count)
                {
                    T addJournalItem = _addJournal[iAddJournal].Item;
                    // If addJournalItem is less than (belongs before)
                    // _items[iItems], insert it.
                    if (_sort(addJournalItem, _items[iItems]) < 0)
                    {
                        SubscribeToItemEvents(addJournalItem);
                        _items.Insert(iItems, addJournalItem);
                        ++iAddJournal;
                    }
                    // Always increment iItems, either because we inserted and
                    // need to move past the insertion, or because we didn't
                    // insert and need to consider the next element.
                    ++iItems;
                }

                // If _addJournal had any "tail" items, append them all now.
                for (; iAddJournal < _addJournal.Count; ++iAddJournal)
                {
                    T addJournalItem = _addJournal[iAddJournal].Item;
                    SubscribeToItemEvents(addJournalItem);
                    _items.Add(addJournalItem);
                }

                _addJournal.Clear();
            }

            private void SubscribeToItemEvents(T item)
            {
                _filterChangedSubscriber(item, Item_FilterPropertyChanged);
                _sortChangedSubscriber(item, Item_SortPropertyChanged);
            }

            private void UnsubscribeFromItemEvents(T item)
            {
                _filterChangedUnsubscriber(item, Item_FilterPropertyChanged);
                _sortChangedUnsubscriber(item, Item_SortPropertyChanged);
            }

            private void InvalidateCache()
            {
                _shouldRebuildCache = true;
            }

            private void Item_FilterPropertyChanged(object sender, EventArgs e)
            {
                InvalidateCache();
            }

            private void Item_SortPropertyChanged(object sender, EventArgs e)
            {
                T item = (T)sender;
                int index = _items.IndexOf(item);

                _addJournal.Add(new AddJournalEntry<T>(_addJournal.Count, item));
                _removeJournal.Add(index);

                // Until the item is back in place, we don't care about its
                // events.  We will re-subscribe when _addJournal is processed.
                UnsubscribeFromItemEvents(item);
                InvalidateCache();
            }
        }

        private struct AddJournalEntry<T>
        {
            public readonly int Order;
            public readonly T Item;

            public AddJournalEntry(int order, T item)
            {
                Order = order;
                Item = item;
            }

            public static AddJournalEntry<T> CreateKey(T item)
            {
                return new AddJournalEntry<T>(-1, item);
            }

            public override int GetHashCode()
            {
                return Item.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is AddJournalEntry<T>))
                    return false;

                return object.Equals(Item, ((AddJournalEntry<T>)obj).Item);
            }
        }

        #endregion
    }
}
