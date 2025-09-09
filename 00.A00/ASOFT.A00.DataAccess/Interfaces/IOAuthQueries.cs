using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.A00.DataAccess.Interfaces
{
    public interface IOAuthQueries
    {
        Task<string> GetHost(CancellationToken cancellationToken = default);
        //Task<string> GetDomain(string domain, CancellationToken cancellationToken = default);
        //Task<string> GetDomainByRefreshToken(string refreshToken, CancellationToken cancellationToken = default);
    }
}
