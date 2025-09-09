using ASOFT.Core.DataAccess.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASOFT.Core.DataAccess.EntitiesConfigurations
{
    public class SysTableConfiguration : IEntityTypeConfiguration<SysTable>
    {
        public void Configure(EntityTypeBuilder<SysTable> builder)
        {
            builder.HasKey(m => m.TableName);
        }
    }
}
