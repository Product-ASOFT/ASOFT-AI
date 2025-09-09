namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Combine specification
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICombineQuery<T> : ISourceQuery<T>
    {
        /// <summary>
        /// Thêm một specification cho việc combine.
        /// </summary>
        /// <param name="spec"></param>
        void Add(ISourceQuery<T> spec);
    }
}