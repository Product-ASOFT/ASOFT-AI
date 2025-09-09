using ASOFT.Core.Business.Common.DataAccess.Interfaces;
using ASOFT.Core.Business.Common.Entities;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess;

namespace ASOFT.Core.Business.Common.DataAccess.Contexts
{
    public class CRMT90031_RELContext : BusinessContext<CRMT90031_REL>, ICRMT90031_RELContext
    {
        public CRMT90031_RELContext(BusinessDbContext context) : base(context)
        {

        }
    }
}
