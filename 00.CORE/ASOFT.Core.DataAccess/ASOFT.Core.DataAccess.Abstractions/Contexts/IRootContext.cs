using ASOFT.Core.DataAccess.UnitOfWorks;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Interface for Repository pattern.
    /// </summary>
    public interface IRootContext<out TUnitOfWork> where TUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// The interface for transaction or manage with db.
        /// </summary>
        TUnitOfWork UnitOfWork { get; }
    }
}