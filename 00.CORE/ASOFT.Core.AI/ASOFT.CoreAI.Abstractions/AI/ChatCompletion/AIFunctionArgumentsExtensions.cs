// Copyright (c) Microsoft. All rights reserved.

using ASOFT.CoreAI.Common.Diagnostics;
using Microsoft.Extensions.AI;

namespace ASOFT.CoreAI.Abstractions;

internal static class AIFunctionArgumentsExtensions
{
    public const string KernelAIFunctionArgumentKey = $"{nameof(AIFunctionArguments)}_{nameof(Kernel)}";

    internal static AIFunctionArguments AddKernel(this AIFunctionArguments arguments, Kernel kernel)
    {
        Verify.NotNull(arguments);
        arguments[KernelAIFunctionArgumentKey] = kernel;

        return arguments;
    }
}