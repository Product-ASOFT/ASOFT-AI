using ASOFT.Core.Business.Common.Entities;
using ASOFT.Core.DataAccess.EntitiesConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASOFT.Core.Business.Common.DataAccsess.EntityConfigurations
{
    public class AT1103Configuration : CategoryEntityTypeConfiguration<AT1103>
    {
        public override void Configure(EntityTypeBuilder<AT1103> builder)
        {
            base.Configure(builder);
            builder.HasKey(m => m.EmployeeID);
        }
    }
}
