using System.Collections.Generic;

namespace ASOFT.Core.DataAccess.UnitOfWorks
{
    /// <summary>
    /// So sánh 2 đối tượng <see cref="IUnitOfWork"/> là cùng 1 đối tượng hay khác.
    /// </summary>
    public class UnitOfWorkComparer : IEqualityComparer<IUnitOfWork>
    {
        /// <summary>
        /// Instance của <see cref="UnitOfWorkComparer"/>.
        /// </summary>
        public static readonly UnitOfWorkComparer Instance = new UnitOfWorkComparer();

        /// <summary>
        /// So sánh
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(IUnitOfWork x, IUnitOfWork y)
        {
            if (ReferenceEquals(x, y))
                return true;
            return x?.Id == y?.Id;
        }

        /// <summary>
        /// Lấy hash code
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(IUnitOfWork obj) => obj.GetHashCode();
    }
}