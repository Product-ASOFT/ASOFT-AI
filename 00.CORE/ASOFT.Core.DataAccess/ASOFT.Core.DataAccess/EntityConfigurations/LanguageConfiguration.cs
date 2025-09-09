using ASOFT.Core.DataAccess.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASOFT.Core.DataAccess.EntitiesConfigurations
{
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("A00001");
            builder.HasKey(m => new {m.ID, m.LanguageID, m.Module});
            builder.HasQueryFilter(m => m.Deleted == false);
        }
    }
}