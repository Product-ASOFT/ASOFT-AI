using System.Data;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Connection provider cung cấp kết nối đến database
    /// </summary>
    public interface IDbConnectionProvider
    {
        /// <summary>
        /// Cung cấp chuỗi kết nối đến database.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string ProvideConnectionString(string key);

        /// <summary>
        /// Cung cấp một kết nối đến database
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IDbConnection ProvideDbConnection(string key);
    }
}