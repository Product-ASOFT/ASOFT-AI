using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace ASOFT.Core.Common.Localization
{
    /// <summary>
    /// Request culture theo yêu cầu của client
    /// </summary>
    public interface IRequestCulture
    {
        IList<StringSegment> Cultures { get; }
        IList<StringSegment> UICultures { get; }
    }
}