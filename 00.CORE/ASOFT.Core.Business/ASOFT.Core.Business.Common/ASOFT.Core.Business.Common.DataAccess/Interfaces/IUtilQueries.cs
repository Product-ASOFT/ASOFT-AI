using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.DataAccess.Interfaces
{
    public interface IUtilQueries
    {
        Task<T> GetEntity<T>(string table, string id, string idValue);
    }
}
