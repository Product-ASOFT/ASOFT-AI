using ASOFT.CoreAI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ASOFT.CoreAI.Infrastructure.EntityConfigurations
{
    public class BEMT2005Configuration : IEntityTypeConfiguration<BEMT2005>
    {
        public void Configure(EntityTypeBuilder<BEMT2005> builder)
        {
            builder.HasKey(m => m.APK);
            builder.Property(m => m.APK)
                .HasValueGenerator<TemporaryGuidValueGenerator>()
                .ValueGeneratedOnAdd();
        }
    }
}