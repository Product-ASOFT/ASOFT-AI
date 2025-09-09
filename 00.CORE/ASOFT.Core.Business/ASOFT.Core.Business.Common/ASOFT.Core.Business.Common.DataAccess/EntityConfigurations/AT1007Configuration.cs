using ASOFT.Core.Business.Common.Entities;
using ASOFT.Core.DataAccess.EntitiesConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASOFT.Core.Business.Common.DataAccess.EntityConfigurations
{
    public class AT1007Configuration : CategoryEntityTypeConfiguration<AT1007>
    {
        public override void Configure(EntityTypeBuilder<AT1007> builder)
        {
            base.Configure(builder);
            builder.HasKey(m => new {m.DivisionID, m.VoucherTypeID});
        }
    }
}