using ASOFT.Core.Business.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASOFT.Core.Business.Common.DataAccess.EntityConfigurations
{
    public class CRMT0099Configuration : IEntityTypeConfiguration<CRMT0099>
    {
        public void Configure(EntityTypeBuilder<CRMT0099> builder)
        {
            builder.HasKey(m => new { m.CodeMaster, m.ID });
        }
    }
}
