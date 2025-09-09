using ASOFT.Core.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.DataAccess.Interfaces
{
    public interface IHistoryQueries<H> where H : HistoryEntity
    {
        Task<int> InstallHistory(H history);
    }
}
