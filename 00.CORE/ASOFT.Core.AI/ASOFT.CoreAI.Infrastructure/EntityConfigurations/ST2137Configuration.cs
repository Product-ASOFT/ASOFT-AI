using ASOFT.CoreAI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ASOFT.CoreAI.Infrastructure.EntityConfigurations
{
    public class ST2137Configuration : IEntityTypeConfiguration<ST2137>
    {
        public void Configure(EntityTypeBuilder<ST2137> builder)
        {
            builder.HasKey(m => m.APK);
            builder.Property(m => m.APK)
                .HasValueGenerator<TemporaryGuidValueGenerator>()
                .ValueGeneratedOnAdd();
        }
    }
}