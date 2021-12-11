// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Input
{
    internal static class KeyboardUtil
    {
        static Dictionary<int, Key> _map;

        static KeyboardUtil()
        {
            _map = new Dictionary<int, Key>();
            _map.Add(8, Key.Back);
            _map.Add(9, Key.Tab);
            _map.Add(13, Key.Enter);
            _map.Add(27, Key.Escape);
            _map.Add(32, Key.Space);
            _map.Add(39, Key.OemQuotes);
            _map.Add(43, Key.Add);
            _map.Add(44, Key.OemComma);
            _map.Add(45, Key.OemMinus);
            _map.Add(46, Key.OemPeriod);
            _map.Add(47, Key.OemQuestion);
            _map.Add(48, Key.D0);
            _map.Add(49, Key.D1);
            _map.Add(50, Key.D2);
            _map.Add(51, Key.D3);
            _map.Add(52, Key.D4);
            _map.Add(53, Key.D5);
            _map.Add(54, Key.D6);
            _map.Add(55, Key.D7);
            _map.Add(56, Key.D8);
            _map.Add(57, Key.D9);
            _map.Add(59, Key.OemSemicolon);
            _map.Add(60, Key.OemBackslash);
            _map.Add(61, Key.OemPlus);
            _map.Add(91, Key.OemOpenBrackets);
            _map.Add(92, Key.OemPipe);
            _map.Add(93, Key.OemCloseBrackets);
            _map.Add(96, Key.OemTilde);
            _map.Add(97, Key.A);
            _map.Add(98, Key.B);
            _map.Add(99, Key.C);
            _map.Add(100, Key.D);
            _map.Add(101, Key.E);
            _map.Add(102, Key.F);
            _map.Add(103, Key.G);
            _map.Add(104, Key.H);
            _map.Add(105, Key.I);
            _map.Add(106, Key.J);
            _map.Add(107, Key.K);
            _map.Add(108, Key.L);
            _map.Add(109, Key.M);
            _map.Add(110, Key.N);
            _map.Add(111, Key.O);
            _map.Add(112, Key.P);
            _map.Add(113, Key.Q);
            _map.Add(114, Key.R);
            _map.Add(115, Key.S);
            _map.Add(116, Key.T);
            _map.Add(117, Key.U);
            _map.Add(118, Key.V);
            _map.Add(119, Key.W);
            _map.Add(120, Key.X);
            _map.Add(121, Key.Y);
            _map.Add(122, Key.Z);
            _map.Add(127, Key.Delete);
            _map.Add(1073741881, Key.CapsLock);
            _map.Add(1073741882, Key.F1);
            _map.Add(1073741883, Key.F2);
            _map.Add(1073741884, Key.F3);
            _map.Add(1073741885, Key.F4);
            _map.Add(1073741886, Key.F5);
            _map.Add(1073741887, Key.F6);
            _map.Add(1073741888, Key.F7);
            _map.Add(1073741889, Key.F8);
            _map.Add(1073741890, Key.F9);
            _map.Add(1073741891, Key.F10);
            _map.Add(1073741892, Key.F11);
            _map.Add(1073741893, Key.F12);
            _map.Add(1073741894, Key.PrintScreen);
            _map.Add(1073741895, Key.Scroll);
            _map.Add(1073741896, Key.Pause);
            _map.Add(1073741897, Key.Insert);
            _map.Add(1073741898, Key.Home);
            _map.Add(1073741899, Key.PageUp);
            _map.Add(1073741901, Key.End);
            _map.Add(1073741902, Key.PageDown);
            _map.Add(1073741903, Key.Right);
            _map.Add(1073741904, Key.Left);
            _map.Add(1073741905, Key.Down);
            _map.Add(1073741906, Key.Up);
            _map.Add(1073741907, Key.NumLock);
            _map.Add(1073741908, Key.Divide);
            _map.Add(1073741909, Key.Multiply);
            _map.Add(1073741910, Key.Subtract);
            _map.Add(1073741911, Key.Add);
            _map.Add(1073741912, Key.Enter);
            _map.Add(1073741913, Key.NumPad1);
            _map.Add(1073741914, Key.NumPad2);
            _map.Add(1073741915, Key.NumPad3);
            _map.Add(1073741916, Key.NumPad4);
            _map.Add(1073741917, Key.NumPad5);
            _map.Add(1073741918, Key.NumPad6);
            _map.Add(1073741919, Key.NumPad7);
            _map.Add(1073741920, Key.NumPad8);
            _map.Add(1073741921, Key.NumPad9);
            _map.Add(1073741922, Key.NumPad0);
            _map.Add(1073741923, Key.Decimal);
            _map.Add(1073741925, Key.Apps);
            _map.Add(1073741928, Key.F13);
            _map.Add(1073741929, Key.F14);
            _map.Add(1073741930, Key.F15);
            _map.Add(1073741931, Key.F16);
            _map.Add(1073741932, Key.F17);
            _map.Add(1073741933, Key.F18);
            _map.Add(1073741934, Key.F19);
            _map.Add(1073741935, Key.F20);
            _map.Add(1073741936, Key.F21);
            _map.Add(1073741937, Key.F22);
            _map.Add(1073741938, Key.F23);
            _map.Add(1073741939, Key.F24);
            _map.Add(1073741951, Key.VolumeMute);
            _map.Add(1073741952, Key.VolumeUp);
            _map.Add(1073741953, Key.VolumeDown);
            _map.Add(1073742040, Key.OemClear);
            _map.Add(1073742044, Key.Decimal);
            _map.Add(1073742048, Key.LeftControl);
            _map.Add(1073742049, Key.LeftShift);
            _map.Add(1073742050, Key.LeftAlt);
            _map.Add(1073742051, Key.LeftWindows);
            _map.Add(1073742052, Key.RightControl);
            _map.Add(1073742053, Key.RightShift);
            _map.Add(1073742054, Key.RightAlt);
            _map.Add(1073742055, Key.RightWindows);
            _map.Add(1073742082, Key.MediaNextTrack);
            _map.Add(1073742083, Key.MediaPreviousTrack);
            _map.Add(1073742084, Key.MediaStop);
            _map.Add(1073742085, Key.MediaPlayPause);
            _map.Add(1073742086, Key.VolumeMute);
            _map.Add(1073742087, Key.SelectMedia);
            _map.Add(1073742089, Key.LaunchMail);
            _map.Add(1073742092, Key.BrowserSearch);
            _map.Add(1073742093, Key.BrowserHome);
            _map.Add(1073742094, Key.BrowserBack);
            _map.Add(1073742095, Key.BrowserForward);
            _map.Add(1073742096, Key.BrowserStop);
            _map.Add(1073742097, Key.BrowserRefresh);
            _map.Add(1073742098, Key.BrowserFavorites);
            _map.Add(1073742106, Key.Sleep);
        }

        public static Key ToXna(int key)
        {
            Key xnaKey;
            if (_map.TryGetValue(key, out xnaKey))
                return xnaKey;

            return Key.None;
        }
    }
}

