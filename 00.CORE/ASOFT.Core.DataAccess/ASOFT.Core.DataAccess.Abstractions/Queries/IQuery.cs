namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Specification cho query
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Id của specification
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Option cho query
        /// </summary>
        IQueryOptions QueryOptions { get; }

        /// <summary>
        /// Replace query options mới.
        /// </summary>
        /// <param name="queryOptions"></param>
        void ReplaceQueryOptions(IQueryOptions queryOptions);
    }

    /// <summary>
    /// Specification of generic type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQuery<T> : IQuery
    {
    }
}