using ASOFT.CoreAI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ASOFT.CoreAI.Infrastructure.EntityConfigurations
{
    public class ST2111Configuration : IEntityTypeConfiguration<ST2111>
    {
        public void Configure(EntityTypeBuilder<ST2111> builder)
        {
            builder.HasKey(m => m.APK);
            builder.Property(m => m.APK)
                .HasValueGenerator<TemporaryGuidValueGenerator>()
                .ValueGeneratedOnAdd();
        }
    }
}