using ASOFT.CoreAI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ASOFT.CoreAI.Infrastructure.EntityConfigurations
{
    public class ST2121Configuration : IEntityTypeConfiguration<ST2121>
    {
        public void Configure(EntityTypeBuilder<ST2121> builder)
        {
            builder.HasKey(m => m.APK);
            builder.Property(m => m.APK)
                .HasValueGenerator<TemporaryGuidValueGenerator>()
                .ValueGeneratedOnAdd();
        }
    }
}