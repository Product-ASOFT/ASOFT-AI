using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IChatResponseHandlerService
    {
        Task<ItemChatResponse> InvokeAsync(string promptSystem, string question);
        Task<ItemChatResponse> InvokePromptAsync(string promptSystem, string promptContent, KernelArguments? arguments = null);
    }
}
