using ASOFT.CoreAI.Abstractions;
using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Business.Services.ChatHandler
{
    public static class HandlebarsRenderer
    {
        public static string RenderPrompt(string template, KernelArguments arguments)
        {
            var compiledTemplate = Handlebars.Compile(template);

            var data = arguments.ToDictionary(x => x.Key, x => x.Value);


            var result = compiledTemplate(data);

            return WebUtility.HtmlDecode(result);
        }
    }
}
