// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

using Microsoft.Extensions.DependencyInjection;

#if WINDOWS_UAP
using Windows.UI.ViewManagement;
#endif

namespace Microsoft.Xna.Framework
{
    partial class GamePlatform
    {
        internal static GamePlatform CreatePlatform(IServiceProvider serviceProvider, Game game)
        {
#if DESKTOPGL || ANGLE
            return CreateSdlGamePlatform(serviceProvider, game);
#elif WINDOWS && DIRECTX
            return new MonoGame.Framework.WinFormsGamePlatform(game);
#elif WINDOWS_UAP
            return new UAPGamePlatform(game);
#endif
        }

        #region Platform-Specific Factory Methods

#if DESKTOPGL || ANGLE

        private static GamePlatform CreateSdlGamePlatform(IServiceProvider serviceProvider, Game game)
        {
            GameWindow gameWindow = serviceProvider.GetRequiredService<GameWindow>();

            if (!(gameWindow is SdlGameWindow sdlGameWindow))
                throw new InvalidOperationException($"The DI container does not have a {nameof(SdlGameWindow)} registered.");

            return new SdlGamePlatform(game, sdlGameWindow);
        }

#endif

        #endregion
    }
}
