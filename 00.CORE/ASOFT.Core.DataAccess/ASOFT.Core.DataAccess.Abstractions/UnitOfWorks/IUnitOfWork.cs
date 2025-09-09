using System;
using System.Threading.Tasks;

namespace ASOFT.Core.DataAccess.UnitOfWorks
{
    /// <summary>
    /// The pattern for repository data access.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Unique for identify object.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Notify change is completed.
        /// </summary>
        /// <returns></returns>
        int Complete();

        /// <summary>
        /// Notify change is completed async.
        /// </summary>
        /// <returns></returns>
        Task<int> CompleteAsync();
    }
}