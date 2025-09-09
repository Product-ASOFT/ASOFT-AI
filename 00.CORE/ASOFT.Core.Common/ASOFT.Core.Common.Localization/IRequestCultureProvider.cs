using System.Threading.Tasks;

namespace ASOFT.Core.Common.Localization
{
    /// <summary>
    /// Cung cấp request culture
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface IRequestCultureProvider<in TInput>
    {
        ValueTask<IRequestCulture> ProvideRequestCultureAsync(TInput input);
    }
}