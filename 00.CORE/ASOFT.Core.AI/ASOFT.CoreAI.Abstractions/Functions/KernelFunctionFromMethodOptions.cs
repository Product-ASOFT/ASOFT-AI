// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace ASOFT.CoreAI.Abstractions
{
    public sealed class KernelFunctionFromMethodOptions
    {
        /// <summary>
        /// The name to use for the function. If null, it will default to one derived from the method represented by the passed <see cref="Delegate"/> or <see cref="MethodInfo"/>.
        /// </summary>
        public string? FunctionName { get; init; }

        /// <summary>
        /// The description to use for the function. If null, it will default to one derived from the passed <see cref="Delegate"/> or <see cref="MethodInfo"/>, if possible
        /// (e.g. via a <see cref="DescriptionAttribute"/> on the method).
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// Optional parameter descriptions. If null, it will default to one derived from the passed <see cref="Delegate"/> or <see cref="MethodInfo"/>.
        /// </summary>
        public IEnumerable<KernelParameterMetadata>? Parameters { get; init; }

        /// <summary>
        /// Optional return parameter description. If null, it will default to one derived from the passed <see cref="Delegate"/> or <see cref="MethodInfo"/>.
        /// </summary>
        public KernelReturnParameterMetadata? ReturnParameter { get; init; }

        /// <summary>
        /// The <see cref="ILoggerFactory"/> to use for logging. If null, no logging will be performed.
        /// </summary>
        public ILoggerFactory? LoggerFactory { get; init; }

        /// <summary>
        /// Optional metadata in addition to the named values already provided in other arguments.
        /// </summary>
        public ReadOnlyDictionary<string, object?>? AdditionalMetadata { get; init; }
    }
}