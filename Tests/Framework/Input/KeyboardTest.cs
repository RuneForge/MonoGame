// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;

namespace MonoGame.Tests.Input
{
    class KeyboardTest
    {

        [TestCase(new[] { Key.Up, Key.A, Key.Left, Key.Oem8, Key.Apps })]
        public void CtorParams(Key[] keys)
        {
            var state = new KeyboardState(keys);
            CollectionAssert.AreEquivalent(state.GetPressedKeys(), keys);

            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                var keyDown = keys.Contains(key);
                Assert.AreEqual(keyDown ? KeyState.Down : KeyState.Up, state[key]);
                Assert.AreEqual(keyDown, state.IsKeyDown(key));
                Assert.AreEqual(!keyDown, state.IsKeyUp(key));
            }
        }

#if !XNA
        [TestCase(new[] { Key.Up, Key.A, Key.Left, Key.Oem8, Key.Apps }, true, false)]
        [TestCase(new[] { Key.Right, Key.Down, Key.LeftAlt, Key.LeftShift }, true, true)]
        [TestCase(new[] { Key.Delete, Key.U, Key.RightWindows, Key.L, Key.NumPad2 }, false, false)]
        [TestCase(new[] { Key.F9, Key.F12, Key.VolumeUp, Key.OemAuto, Key.NumPad3 }, false, false)]
        [TestCase(new[] { Key.OemMinus, Key.OemTilde, Key.Tab, Key.Zoom }, true, false)]
        public void TestState(Key[] keys, bool capsLock, bool numLock)
        {
            var keyList = keys.ToList();
            var state = new KeyboardState(keys, capsLock, numLock);

            CollectionAssert.AreEquivalent(state.GetPressedKeys(), keys);
            Assert.AreEqual(state.CapsLock, capsLock);
            Assert.AreEqual(state.NumLock, numLock);

            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                var keyDown = keyList.Contains(key);
                Assert.AreEqual(state.IsKeyDown(key), keyDown);
                Assert.AreEqual(state.IsKeyUp(key), !keyDown);
            }
        }

#endif

        [Test]
        public void TestGetState()
        {
            Keyboard.GetState();
        }

        [TestCase(new[] { Key.Up, Key.A, Key.Left, Key.Oem8, Key.Apps })]
        public void TestGetPressedKeysGarbageless(Key[] keys)
        {
            var state = new KeyboardState(keys);

            int count = state.GetPressedKeyCount();
            Assert.AreEqual(keys.Length, count);

            Key[] newKeysArray = new Key[count];

            state.GetPressedKeys(newKeysArray);

            CollectionAssert.AreEquivalent(keys, newKeysArray);
        }
    }
}
