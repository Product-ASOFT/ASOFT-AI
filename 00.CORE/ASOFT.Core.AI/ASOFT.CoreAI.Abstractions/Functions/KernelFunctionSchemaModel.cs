// Copyright (c) Microsoft. All rights reserved.

namespace ASOFT.CoreAI.Abstractions
{
    internal class KernelFunctionSchemaModel
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public object Properties { get; internal set; }
        public object Required { get; internal set; }
    }
}