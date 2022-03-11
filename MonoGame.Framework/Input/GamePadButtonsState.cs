// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Input
{
    /// <summary>
    /// A struct that represents the current button states for the controller.
    /// </summary>
    public struct GamePadButtonsState
    {
        internal readonly GamePadButtons _buttons;

        /// <summary>
        /// Gets a value indicating if the button A is pressed.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the button A is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public ButtonState A
        {
            get
            {
                return ((_buttons & GamePadButtons.A) == GamePadButtons.A) ? ButtonState.Pressed : ButtonState.Released;
            }
        }

        /// <summary>
        /// Gets a value indicating if the button B is pressed.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the button B is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public ButtonState B
        {
            get
            {
                return ((_buttons & GamePadButtons.B) == GamePadButtons.B) ? ButtonState.Pressed : ButtonState.Released;
            }
        }

        /// <summary>
        /// Gets a value indicating if the button Back is pressed.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the button Back is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public ButtonState Back
        {
            get
            {
                return ((_buttons & GamePadButtons.Back) == GamePadButtons.Back) ? ButtonState.Pressed : ButtonState.Released;
            }
        }

        /// <summary>
        /// Gets a value indicating if the button X is pressed.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the button X is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public ButtonState X
        {
            get
            {
                return ((_buttons & GamePadButtons.X) == GamePadButtons.X) ? ButtonState.Pressed : ButtonState.Released;
            }
        }

        /// <summary>
        /// Gets a value indicating if the button Y is pressed.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the button Y is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public ButtonState Y
        {
            get
            {
                return ((_buttons & GamePadButtons.Y) == GamePadButtons.Y) ? ButtonState.Pressed : ButtonState.Released;
            }
        }

        /// <summary>
        /// Gets a value indicating if the button Start is pressed.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the button Start is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public ButtonState Start
        {
            get
            {
                return ((_buttons & GamePadButtons.Start) == GamePadButtons.Start) ? ButtonState.Pressed : ButtonState.Released;
            }
        }

        /// <summary>
        /// Gets a value indicating if the left shoulder button is pressed.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the left shoulder button is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public ButtonState LeftShoulder
        {
            get
            {
                return ((_buttons & GamePadButtons.LeftShoulder) == GamePadButtons.LeftShoulder) ? ButtonState.Pressed : ButtonState.Released;
            }
        }

        /// <summary>
        /// Gets a value indicating if the left stick button is pressed.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the left stick button is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public ButtonState LeftStick
        {
            get
            {
                return ((_buttons & GamePadButtons.LeftStick) == GamePadButtons.LeftStick) ? ButtonState.Pressed : ButtonState.Released;
            }
        }

        /// <summary>
        /// Gets a value indicating if the right shoulder button is pressed.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the right shoulder button is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public ButtonState RightShoulder
        {
            get
            {
                return ((_buttons & GamePadButtons.RightShoulder) == GamePadButtons.RightShoulder) ? ButtonState.Pressed : ButtonState.Released;
            }
        }

        /// <summary>
        /// Gets a value indicating if the right stick button is pressed.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the right stick button is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public ButtonState RightStick
        {
            get
            {
                return ((_buttons & GamePadButtons.RightStick) == GamePadButtons.RightStick) ? ButtonState.Pressed : ButtonState.Released;
            }
        }

        /// <summary>
        /// Gets a value indicating if the guide button is pressed.
        /// </summary>
        /// <value><see cref="ButtonState.Pressed"/> if the guide button is pressed; otherwise, <see cref="ButtonState.Released"/>.</value>
        public ButtonState BigButton
        {
            get
            {
                return ((_buttons & GamePadButtons.BigButton) == GamePadButtons.BigButton) ? ButtonState.Pressed : ButtonState.Released;
            }
        }
        
        public GamePadButtonsState(GamePadButtons buttons)
        {
            _buttons = buttons;
        }

        internal GamePadButtonsState(params GamePadButtons[] buttons) : this()
        {
            foreach (GamePadButtons b in buttons)
                _buttons |= b;
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GamePadButtonsState"/> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(GamePadButtonsState left, GamePadButtonsState right)
        {
            return left._buttons == right._buttons;
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GamePadButtonsState"/> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(GamePadButtonsState left, GamePadButtonsState right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare to this instance.</param>
        /// <returns>true if <paramref name="obj"/> is a <see cref="GamePadButtonsState"/> and has the same value as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is GamePadButtonsState) && (this == (GamePadButtonsState)obj);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="T:Microsoft.Xna.Framework.Input.GamePadButtons"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return (int)_buttons;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Microsoft.Xna.Framework.Input.GamePadButtons"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Microsoft.Xna.Framework.Input.GamePadButtons"/>.</returns>
        public override string ToString()
        {
            return "[GamePadButtons:" +
                " A=" + (int)A +
                ", B=" + (int)B +
                ", Back=" + (int)Back +
                ", X=" + (int)X +
                ", Y=" + (int)Y +
                ", Start=" + (int)Start +
                ", LeftShoulder=" + (int)LeftShoulder +
                ", LeftStick=" + (int)LeftStick +
                ", RightShoulder=" + (int)RightShoulder +
                ", RightStick=" + (int)RightStick +
                ", BigButton=" + (int)BigButton +
                "]";
        }
    }
}