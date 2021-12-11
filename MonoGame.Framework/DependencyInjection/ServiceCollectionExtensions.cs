using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection" /> type allowing to register MonoGame services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds MonoGame services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="gameFactoryMethod">The factory method creating an instance of the <see cref="Game" /> type.</param>
        public static void AddMonoGame(this IServiceCollection services, GameFactoryMethod gameFactoryMethod)
        {
#if NETSTANDARD && DESKTOPGL
            AddMonoGameInternal(services, gameFactoryMethod);
#endif
        }

        private static Lazy<T> GetLazyInitializedGameService<T>(IServiceProvider serviceProvider, Func<Game, T> serviceFactoryMethod)
            where T : class
        {
            return new Lazy<T>(() =>
            {
                Game game = serviceProvider.GetRequiredService<Game>();
                return serviceFactoryMethod(game);
            });
        }

        #region DesktopGL-Specific Services Initialization

#if NETSTANDARD && DESKTOPGL

        private static void AddMonoGameInternal(IServiceCollection services, GameFactoryMethod gameFactoryMethod)
        {
            // Register an instance of the SdlGameWindow type as GameWindow.
            services.AddSingleton<GameWindow>(serviceProvider =>
            {
                Lazy<GraphicsDeviceManager> graphicsDeviceManagerProvider = serviceProvider.GetRequiredService<Lazy<GraphicsDeviceManager>>();
                Lazy<GraphicsDevice> graphicsDeviceProvider = serviceProvider.GetRequiredService<Lazy<GraphicsDevice>>();
                return new SdlGameWindow(graphicsDeviceManagerProvider, graphicsDeviceProvider);
            });

            // Register a user-defined type derived from the Game type.
            services.AddSingleton(serviceProvider =>
            {
                GameWindow gameWindow = serviceProvider.GetRequiredService<GameWindow>();
                return gameFactoryMethod(serviceProvider, gameWindow);
            });

            // Register instances of the GameComponentCollection and LaunchParameterCollection types.
            services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<Game>().Components);
            services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<Game>().LaunchParameters);

            // Register a default and a lazy-initialized instance of the GamePlatform internal type.
            services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<Game>().Platform);
            services.AddSingleton(serviceProvider => GetLazyInitializedGameService(serviceProvider, game => game.Platform));

            // Register a default and a lazy-initialized instance of the IGraphicsDeviceService type.
            services.AddSingleton<IGraphicsDeviceService>(serviceProvider => serviceProvider.GetRequiredService<Game>().GraphicsDeviceManager);
            services.AddSingleton(serviceProvider => GetLazyInitializedGameService<IGraphicsDeviceService>(serviceProvider, game => game.GraphicsDeviceManager));

            // Register a default and a lazy-initialized instance of the IGraphicsDeviceManager type.
            services.AddSingleton<IGraphicsDeviceManager>(serviceProvider => serviceProvider.GetRequiredService<Game>().GraphicsDeviceManager);
            services.AddSingleton(serviceProvider => GetLazyInitializedGameService<IGraphicsDeviceManager>(serviceProvider, game => game.GraphicsDeviceManager));

            // Register a default and a lazy-initialized instance of the GraphicsDeviceManager type.
            services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<Game>().GraphicsDeviceManager);
            services.AddSingleton(serviceProvider => GetLazyInitializedGameService(serviceProvider, game => game.GraphicsDeviceManager));

            // Register a default and a lazy-initialized instance of the GraphicsDevice type.
            services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<Game>().GraphicsDevice);
            services.AddSingleton(serviceProvider => GetLazyInitializedGameService(serviceProvider, game => game.GraphicsDevice));

            // Register a default and a lazy-initialized instance of the ContentManager type.
            services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<Game>().ContentManager);
            services.AddSingleton(serviceProvider => GetLazyInitializedGameService(serviceProvider, game => game.ContentManager));
        }

#endif

        #endregion
    }
}
