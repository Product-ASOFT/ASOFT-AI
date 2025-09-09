using ASOFT.Core.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASOFT.Core.DataAccess.EntitiesConfigurations
{
    /// <summary>
    /// Base entity configuration for entity that extend category entity
    /// </summary>
    /// <history>
    /// Luan Le [Created] 2019/09/12
    /// </history>
    /// <typeparam name="TCategoryEntity"></typeparam>
    public class CategoryEntityTypeConfiguration<TCategoryEntity> : BaseEntityTypeConfiguration<TCategoryEntity>
        where TCategoryEntity : CategoryEntity
    {
        public override void Configure(EntityTypeBuilder<TCategoryEntity> builder)
        {
            base.Configure(builder);
            builder.HasQueryFilter(m => m.Disabled == 0);
            builder.Property(m => m.Disabled).HasDefaultValue((byte) 0);
        }
    }
}