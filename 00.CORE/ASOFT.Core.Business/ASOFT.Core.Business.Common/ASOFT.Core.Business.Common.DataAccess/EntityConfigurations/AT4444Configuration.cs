using ASOFT.Core.Business.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ASOFT.Core.Business.Common.DataAccess.EntityConfigurations
{
    public class AT4444Configuration : IEntityTypeConfiguration<AT4444>
    {
        public void Configure(EntityTypeBuilder<AT4444> builder)
        {
            builder.Property(m => m.APK).HasValueGenerator<TemporaryGuidValueGenerator>().ValueGeneratedOnAdd();
            builder.HasKey(m => new {m.APK, m.DivisionID});
        }
    }
}