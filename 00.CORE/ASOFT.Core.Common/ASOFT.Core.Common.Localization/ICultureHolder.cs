namespace ASOFT.Core.Common.Localization
{
    /// <summary>
    /// Class sử dụng để lưu <see cref="ICultureResult"/>.
    /// </summary>
    public interface ICultureHolder
    {
        ICultureResult CultureResult { get; }
    }
}