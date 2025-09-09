using System.Threading.Tasks;

namespace ASOFT.Core.Common.Localization
{
    /// <summary>
    /// Transform culture
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface ICultureTransformer<in TInput>
    {
        ValueTask<ICultureResult> TransformAsync(TInput input);
    }
}