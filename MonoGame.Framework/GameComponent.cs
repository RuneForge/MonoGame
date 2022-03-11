// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// An object that can be attached to a <see cref="Game"/> and have its <see cref="Update"/>
    /// method called when <see cref="Game.Update"/> is called.
    /// </summary>
    public class GameComponent : IGameComponent, IUpdateable, IDisposable
    {
        private bool _enabled = true;
        private int _updateOrder;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnEnabledChanged(this, EventArgs.Empty);
                }
            }
        }

        public int UpdateOrder
        {
            get { return _updateOrder; }
            set
            {
                if (_updateOrder != value)
                {
                    _updateOrder = value;
                    OnUpdateOrderChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <inheritdoc />
        public event EventHandler<EventArgs> EnabledChanged;

        /// <inheritdoc />
        public event EventHandler<EventArgs> UpdateOrderChanged;

        /// <summary>
        /// Create a <see cref="GameComponent"/>.
        /// </summary>
        public GameComponent()
        {
        }

        ~GameComponent()
        {
            Dispose(false);
        }

        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Update the component.
        /// </summary>
        /// <param name="gameTime"><see cref="GameTime"/> of the <see cref="Game"/>.</param>
        public virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Called when <see cref="UpdateOrder"/> changed. Raises the <see cref="UpdateOrderChanged"/> event.
        /// </summary>
        /// <param name="sender">This <see cref="GameComponent"/>.</param>
        /// <param name="args">Arguments to the <see cref="UpdateOrderChanged"/> event.</param>
        protected virtual void OnUpdateOrderChanged(object sender, EventArgs args)
        {
            EventHelpers.Raise(sender, UpdateOrderChanged, args);
        }

        /// <summary>
        /// Called when <see cref="Enabled"/> changed. Raises the <see cref="EnabledChanged"/> event.
        /// </summary>
        /// <param name="sender">This <see cref="GameComponent"/>.</param>
        /// <param name="args">Arguments to the <see cref="EnabledChanged"/> event.</param>
        protected virtual void OnEnabledChanged(object sender, EventArgs args)
        {
            EventHelpers.Raise(sender, EnabledChanged, args);
        }

        /// <summary>
        /// Shuts down the component.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Shuts down the component.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
