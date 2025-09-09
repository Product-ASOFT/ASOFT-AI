using ASOFT.Core.Business.Common.DataAccess.Interfaces;
using ASOFT.Core.Business.Common.Entities;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.DataAccess.Contexts
{
    public class CRMT90031Context : BusinessContext<CRMT90031>, ICRMT90031Context
    {
        public CRMT90031Context(BusinessDbContext context) : base(context)
        {

        }
        public async Task<CRMT90031> GetByAPK(Guid apk, string divisionID,
            CancellationToken cancellationToken = default(CancellationToken))
            => await EntitySet().FindAsync(apk, cancellationToken);
    }
}
