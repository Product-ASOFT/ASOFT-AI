// Copyright (c) Microsoft. All rights reserved.

using ASOFT.CoreAI.Common.Diagnostics;
using Microsoft.Extensions.AI;
using System.Diagnostics.CodeAnalysis;

namespace ASOFT.CoreAI.Abstractions;

public static class AIFunctionExtensions
{
    [Experimental("SKEXP0001")]
    public static KernelFunction AsKernelFunction(this AIFunction aiFunction)
    {
        Verify.NotNull(aiFunction);
        return aiFunction is KernelFunction kf
            ? kf
            : new AIFunctionKernelFunction(aiFunction);
    }
}