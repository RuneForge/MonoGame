﻿using System;

namespace Microsoft.Xna.Framework.DependencyInjection
{
    /// <summary>
    /// Represents a factory method creating an instance of the <see cref="Game" /> type.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>An instance of the <see cref="Game" /> type.</returns>
    public delegate Game GameFactoryMethod(IServiceProvider serviceProvider);
}