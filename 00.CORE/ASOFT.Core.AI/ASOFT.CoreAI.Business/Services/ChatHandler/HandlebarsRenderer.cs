using System.Net;
using ASOFT.CoreAI.Abstractions;
using HandlebarsDotNet;

public static class HandlebarsRenderer
{
    private static bool _helpersRegistered = false;
    private static readonly object _lock = new();


    private static void EnsureHelpersRegistered(KernelArguments arguments)
    {
        if (_helpersRegistered) return;

        lock (_lock)
        {
            if (_helpersRegistered) return;

            Handlebars.RegisterHelper("eq", (context, arguments) =>
            {
                if (arguments.Length < 2)
                    return false;

                var left = arguments[0];
                var right = arguments[1];

                if (left == null && right == null) return true;
                if (left == null || right == null) return false;

                return string.Equals(
                    left.ToString()?.Trim(),
                    right.ToString()?.Trim(),
                    StringComparison.OrdinalIgnoreCase
                );
            });

            Handlebars.RegisterHelper("ne", (context, arguments) =>
            {
                if (arguments.Length < 2)
                    return true;

                var left = arguments[0];
                var right = arguments[1];

                if (left == null && right == null) return false;
                if (left == null || right == null) return true;

                return !string.Equals(
                    left.ToString()?.Trim(),
                    right.ToString()?.Trim(),
                    StringComparison.OrdinalIgnoreCase
                );
            });

            _helpersRegistered = true;
        }
    }

    public static string RenderPrompt(string template, KernelArguments arguments)
    {
        EnsureHelpersRegistered(arguments);

        var compiledTemplate = Handlebars.Compile(template);

        var data = arguments.ToDictionary(x => x.Key, x => x.Value);

        var result = compiledTemplate(data);

        return WebUtility.HtmlDecode(result);
    }
}