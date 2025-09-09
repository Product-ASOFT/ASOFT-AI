// Copyright (c) Microsoft. All rights reserved.

using ASOFT.CoreAI.Abstractions.Filters.AutoFunctionInvocation;
using ASOFT.CoreAI.Abstractions.Filters.Function;
using ASOFT.CoreAI.Abstractions.Filters.Prompt;
using ASOFT.CoreAI.Common.Diagnostics;
using ASOFT.CoreAI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Serialization;

namespace ASOFT.CoreAI.Abstractions;

public sealed class Kernel
{
    public const string KernelServiceTypeToKeyMappings = nameof(KernelServiceTypeToKeyMappings);

    private Dictionary<string, object?>? _data;
    private CultureInfo _culture = CultureInfo.InvariantCulture;
    private KernelPluginCollection? _plugins;
    private NonNullCollection<IFunctionInvocationFilter>? _functionInvocationFilters;
    private NonNullCollection<IPromptRenderFilter>? _promptRenderFilters;
    private NonNullCollection<IAutoFunctionInvocationFilter>? _autoFunctionInvocationFilters;

    public Kernel(
        IServiceProvider? services = null,
        KernelPluginCollection? plugins = null)
    {
        this.Services = services ?? EmptyServiceProvider.Instance;
        this._plugins = plugins ?? this.Services.GetService<KernelPluginCollection>();
        if (this._plugins is null)
        {
            IEnumerable<KernelPlugin> registeredPlugins = this.Services.GetServices<KernelPlugin>();
            if (IsNotEmpty(registeredPlugins))
            {
                this._plugins = new(registeredPlugins);
            }
        }
        this.AddFilters();
    }

    public static IKernelBuilder CreateBuilder() => new KernelBuilder();

    public Kernel Clone() =>
        new(this.Services, this._plugins is { Count: > 0 } ? new KernelPluginCollection(this._plugins) : null)
        {
            _data = this._data is { Count: > 0 } ? new Dictionary<string, object?>(this._data) : null,
            _culture = this._culture,
        };

    public KernelPluginCollection Plugins =>
        this._plugins ??
        Interlocked.CompareExchange(ref this._plugins, [], null) ??
        this._plugins;

    public IList<IAutoFunctionInvocationFilter> AutoFunctionInvocationFilters =>
        this._autoFunctionInvocationFilters ??
        Interlocked.CompareExchange(ref this._autoFunctionInvocationFilters, [], null) ??
        this._autoFunctionInvocationFilters;

    public IServiceProvider Services { get; }

    [JsonIgnore]
    [AllowNull]
    public CultureInfo Culture
    {
        get => this._culture;
        set => this._culture = value ?? CultureInfo.InvariantCulture;
    }

    [JsonIgnore]
    public ILoggerFactory LoggerFactory =>
        this.Services.GetService<ILoggerFactory>() ??
        NullLoggerFactory.Instance;

    [JsonIgnore]
    public IAIServiceSelector ServiceSelector =>
        this.Services.GetService<IAIServiceSelector>() ??
        OrderedAIServiceSelector.Instance;

    public IDictionary<string, object?> Data =>
        this._data ??
        Interlocked.CompareExchange(ref this._data, [], null) ??
        this._data;

    public T GetRequiredService<T>(object? serviceKey = null) where T : class
    {
        T? service = null;
        if (serviceKey is not null)
        {
            if (this.Services is IKeyedServiceProvider)
            {
                service = this.Services.GetKeyedService<T>(serviceKey);
            }
        }
        else
        {
            service = this.Services.GetService<T>();
            if (service is null && this.Services is IKeyedServiceProvider)
            {
                service = this.GetAllServices<T>().LastOrDefault();
            }
        }
        if (service is null)
        {
            string message =
                serviceKey is null ? $"Service of type '{typeof(T)}' not registered." :
                this.Services is not IKeyedServiceProvider ? $"Key '{serviceKey}' specified but service provider '{this.Services}' is not a {nameof(IKeyedServiceProvider)}." :
                $"Service of type '{typeof(T)}' and key '{serviceKey}' not registered.";
            throw new KernelException(message);
        }
        return service;
    }

    public IEnumerable<T> GetAllServices<T>() where T : class
    {
        if (this.Services is IKeyedServiceProvider)
        {
            if (this.Services.GetKeyedService<Dictionary<Type, HashSet<object?>>>(KernelServiceTypeToKeyMappings) is { } typeToKeyMappings)
            {
                if (typeToKeyMappings.TryGetValue(typeof(T), out HashSet<object?>? keys))
                {
                    return keys.SelectMany(this.Services.GetKeyedServices<T>);
                }
                return [];
            }
        }
        return this.Services.GetServices<T>();
    }

    private void AddFilters()
    {
        IEnumerable<IFunctionInvocationFilter> functionInvocationFilters = this.Services.GetServices<IFunctionInvocationFilter>();
        if (IsNotEmpty(functionInvocationFilters))
        {
            this._functionInvocationFilters = new(functionInvocationFilters);
        }
        IEnumerable<IPromptRenderFilter> promptRenderFilters = this.Services.GetServices<IPromptRenderFilter>();
        if (IsNotEmpty(promptRenderFilters))
        {
            this._promptRenderFilters = new(promptRenderFilters);
        }
        IEnumerable<IAutoFunctionInvocationFilter> autoFunctionInvocationFilters = this.Services.GetServices<IAutoFunctionInvocationFilter>();
        if (IsNotEmpty(autoFunctionInvocationFilters))
        {
            this._autoFunctionInvocationFilters = new(autoFunctionInvocationFilters);
        }
    }

    internal async Task<FunctionInvocationContext> OnFunctionInvocationAsync(
        KernelFunction function,
        KernelArguments arguments,
        FunctionResult functionResult,
        bool isStreaming,
        Func<FunctionInvocationContext, Task> functionCallback,
        CancellationToken cancellationToken)
    {
        FunctionInvocationContext context = new(this, function, arguments, functionResult)
        {
            CancellationToken = cancellationToken,
            IsStreaming = isStreaming
        };
        await InvokeFilterOrFunctionAsync(this._functionInvocationFilters, functionCallback, context).ConfigureAwait(false);
        return context;
    }

    private static async Task InvokeFilterOrFunctionAsync(
        NonNullCollection<IFunctionInvocationFilter>? functionFilters,
        Func<FunctionInvocationContext, Task> functionCallback,
        FunctionInvocationContext context,
        int index = 0)
    {
        if (functionFilters is { Count: > 0 } && index < functionFilters.Count)
        {
            await functionFilters[index].OnFunctionInvocationAsync(context,
                (context) => InvokeFilterOrFunctionAsync(functionFilters, functionCallback, context, index + 1)).ConfigureAwait(false);
        }
        else
        {
            await functionCallback(context).ConfigureAwait(false);
        }
    }

    public async Task<PromptRenderContext> OnPromptRenderAsync(
        KernelFunction function,
        KernelArguments arguments,
        bool isStreaming,
        PromptExecutionSettings? executionSettings,
        Func<PromptRenderContext, Task> renderCallback,
        CancellationToken cancellationToken)
    {
        PromptRenderContext context = new(this, function, arguments)
        {
            CancellationToken = cancellationToken,
            IsStreaming = isStreaming,
            ExecutionSettings = executionSettings
        };
        await InvokeFilterOrPromptRenderAsync(this._promptRenderFilters, renderCallback, context).ConfigureAwait(false);
        return context;
    }

    private static async Task InvokeFilterOrPromptRenderAsync(
        NonNullCollection<IPromptRenderFilter>? promptFilters,
        Func<PromptRenderContext, Task> renderCallback,
        PromptRenderContext context,
        int index = 0)
    {
        if (promptFilters is { Count: > 0 } && index < promptFilters.Count)
        {
            await promptFilters[index].OnPromptRenderAsync(context,
                (context) => InvokeFilterOrPromptRenderAsync(promptFilters, renderCallback, context, index + 1)).ConfigureAwait(false);
        }
        else
        {
            await renderCallback(context).ConfigureAwait(false);
        }
    }

    public Task<FunctionResult> InvokeAsync(
        KernelFunction function,
        KernelArguments? arguments = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNull(function);
        return function.InvokeAsync(this, arguments, cancellationToken);
    }

    public Task<FunctionResult> InvokeAsync(
        string? pluginName,
        string functionName,
        KernelArguments? arguments = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNullOrWhiteSpace(functionName);
        var function = this.Plugins.GetFunction(pluginName, functionName);
        return function.InvokeAsync(this, arguments, cancellationToken);
    }

    public async Task<TResult?> InvokeAsync<TResult>(
        KernelFunction function,
        KernelArguments? arguments = null,
        CancellationToken cancellationToken = default)
    {
        FunctionResult result = await this.InvokeAsync(function, arguments, cancellationToken).ConfigureAwait(false);
        return result.GetValue<TResult>();
    }

    public async Task<TResult?> InvokeAsync<TResult>(
        string? pluginName,
        string functionName,
        KernelArguments? arguments = null,
        CancellationToken cancellationToken = default)
    {
        FunctionResult result = await this.InvokeAsync(pluginName, functionName, arguments, cancellationToken).ConfigureAwait(false);
        return result.GetValue<TResult>();
    }

    public IAsyncEnumerable<StreamingKernelContent> InvokeStreamingAsync(
        KernelFunction function,
        KernelArguments? arguments = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNull(function);
        return function.InvokeStreamingAsync<StreamingKernelContent>(this, arguments, cancellationToken);
    }

    public IAsyncEnumerable<StreamingKernelContent> InvokeStreamingAsync(
        string? pluginName,
        string functionName,
        KernelArguments? arguments = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNullOrWhiteSpace(functionName);
        var function = this.Plugins.GetFunction(pluginName, functionName);
        return function.InvokeStreamingAsync<StreamingKernelContent>(this, arguments, cancellationToken);
    }

    public IAsyncEnumerable<T> InvokeStreamingAsync<T>(
        KernelFunction function,
        KernelArguments? arguments = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNull(function);
        return function.InvokeStreamingAsync<T>(this, arguments, cancellationToken);
    }

    public IAsyncEnumerable<T> InvokeStreamingAsync<T>(
        string? pluginName,
        string functionName,
        KernelArguments? arguments = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNullOrWhiteSpace(functionName);
        var function = this.Plugins.GetFunction(pluginName, functionName);
        return function.InvokeStreamingAsync<T>(this, arguments, cancellationToken);
    }

    private static bool IsNotEmpty<T>(IEnumerable<T> enumerable) =>
        enumerable is not ICollection<T> collection || collection.Count != 0;
}

/// <summary>Provides a builder for adding plugins as singletons to a service collection.</summary>
public interface IKernelBuilderPlugins
{
    IServiceCollection Services { get; }
}

public interface IAIServiceSelector
{
#pragma warning disable CA1716

    bool TrySelectAIService<T>(
        Kernel kernel,
        KernelFunction function,
        KernelArguments arguments,
        [NotNullWhen(true)] out T? service,
        out PromptExecutionSettings? serviceSettings) where T : class, IAIService;

#pragma warning restore CA1716
}