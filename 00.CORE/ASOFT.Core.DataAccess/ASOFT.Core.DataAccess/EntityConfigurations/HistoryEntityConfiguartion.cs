using ASOFT.Core.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ASOFT.Data.Core.EntityConfigurations
{
    public class HistoryEntityConfiguartion<T> : IEntityTypeConfiguration<T>
        where T : HistoryEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(m => m.APK).HasValueGenerator<TemporaryGuidValueGenerator>().ValueGeneratedOnAdd();
            builder.Property(e => e.HistoryID).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}
