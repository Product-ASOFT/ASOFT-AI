namespace ASOFT.Core.API.Paging
{
    /// <summary>
    /// Adapter cho việc convert model sang paging model
    /// </summary>
    public interface INumberSizePagingAdapter
    {
        /// <summary>
        /// Tạo model phân trang
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagingEntity"></param>
        /// <returns></returns>
        PagingResponseModel<T> Create<T>(NumberSizePagingEntity<T> pagingEntity);
    }
}