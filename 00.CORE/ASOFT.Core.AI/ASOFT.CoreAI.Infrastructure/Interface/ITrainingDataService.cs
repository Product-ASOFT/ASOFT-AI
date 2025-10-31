using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface ITrainingDataService
    {
        Task<IEnumerable<RedisearchResultItem>> GetTrainingDataAsync(ReadFileRequest request, string indexName);
    }
}
