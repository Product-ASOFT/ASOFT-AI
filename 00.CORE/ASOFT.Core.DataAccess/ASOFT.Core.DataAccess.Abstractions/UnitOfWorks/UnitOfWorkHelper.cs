using ASOFT.Core.DataAccess.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable All

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Helper class cho Unit of work pattern
    /// </summary>
    public static class UnitOfWorkHelper
    {
        /// <summary>
        /// Khi insert, update gọi hàm này để tiến hành xác nhận lưu xuống database.
        /// </summary>
        /// <param name="unitOfWorks"></param>
        /// <returns></returns>
        public static int Complete(params IUnitOfWork[] unitOfWorks)
        {
            if (unitOfWorks == null)
            {
                throw new ArgumentNullException(nameof(unitOfWorks));
            }

            var count = 0;
            foreach (var unitOfWork in new HashSet<IUnitOfWork>(unitOfWorks, UnitOfWorkComparer.Instance))
            {
                count += unitOfWork.Complete();
            }

            return count;
        }

        /// <summary>
        /// Khi insert, update gọi hàm này để tiến hành xác nhận lưu xuống database.
        /// </summary>
        /// <param name="unitOfWorks"></param>
        /// <returns></returns>
        public static async Task<int> CompleteAsync(params IUnitOfWork[] unitOfWorks)
        {
            if (unitOfWorks == null)
            {
                throw new ArgumentNullException(nameof(unitOfWorks));
            }

            var count = 0;
            foreach (var unitOfWork in new HashSet<IUnitOfWork>(unitOfWorks, UnitOfWorkComparer.Instance))
            {
                count += await unitOfWork.CompleteAsync().ConfigureAwait(false);
            }

            return count;
        }
    }
}