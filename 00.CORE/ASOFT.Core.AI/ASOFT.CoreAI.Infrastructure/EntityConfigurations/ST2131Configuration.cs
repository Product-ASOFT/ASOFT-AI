using ASOFT.CoreAI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ASOFT.CoreAI.Infrastructure.EntityConfigurations
{
    public class ST2131Configuration : IEntityTypeConfiguration<ST2131>
    {
        public void Configure(EntityTypeBuilder<ST2131> builder)
        {
            builder.HasKey(m => m.APK);
            builder.Property(m => m.APK)
                .HasValueGenerator<TemporaryGuidValueGenerator>()
                .ValueGeneratedOnAdd();
        }
    }
}