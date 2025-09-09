// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.DependencyInjection;

namespace ASOFT.CoreAI.Abstractions;

/// <summary>Provides a builder for constructing instances of <see cref="Kernel"/>.</summary>
public interface IKernelBuilder
{
    /// <summary>Gets the collection of services to be built into the <see cref="Kernel"/>.</summary>
    IServiceCollection Services { get; }

    /// <summary>Gets a builder for adding collections as singletons to <see cref="Services"/>.</summary>
    IKernelBuilderPlugins Plugins { get; }
}