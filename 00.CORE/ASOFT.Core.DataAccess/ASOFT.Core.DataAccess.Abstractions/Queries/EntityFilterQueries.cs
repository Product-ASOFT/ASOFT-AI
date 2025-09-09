using ASOFT.Core.DataAccess.Entities;
using System;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Extension class for specification query' filters
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public static class EntityFilterQueries
    {
        /// <summary>
        /// Query filter for getting entity by division.
        /// </summary>
        /// <param name="divisionId"></param>
        /// <typeparam name="T">The entity type</typeparam>
        /// <returns></returns>
        public static FilterQuery<T> ByDivision<T>(string divisionId) where T : BaseEntity
            => new FilterQuery<T>(m => m.DivisionID == divisionId);

        /// <summary>
        /// Query filter for getting entity by apk.
        /// </summary>
        /// <param name="apk"></param>
        /// <typeparam name="T">The entity type</typeparam>
        /// <returns></returns>
        public static FilterQuery<T> ByApk<T>(Guid apk) where T : BaseEntity
            => new FilterQuery<T>(m => m.APK == apk);

        /// <summary>
        /// Query filter for getting entity by division and apk.
        /// </summary>
        /// <param name="divisionId"></param>
        /// <param name="apk"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FilterQuery<T> ByDivisionAndApk<T>(string divisionId, Guid apk) where T : BaseEntity
            => new FilterQuery<T>(ByDivision<T>(divisionId) && ByApk<T>(apk));

        /// <summary>
        /// Entity not disabled
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FilterQuery<T> NotDisabled<T>() where T : CategoryEntity
            => new FilterQuery<T>(m => m.Disabled == 0);

        /// <summary>
        /// Filter that rows are not deleted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FilterQuery<T> NotDeleted<T>() where T : BusinessEntity
            => new FilterQuery<T>(m => m.DeleteFlg == 0);
    }
}