namespace ASOFT.CoreAI.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);

        Task<IEnumerable<T>> GetAllAsync();

        Task<bool> AddAsync(T entity, CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id);
    }
}