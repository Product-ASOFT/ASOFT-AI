using ASOFT.Core.DataAccess.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASOFT.Core.DataAccess.EntitiesConfigurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("A00002");
            builder.HasKey(m => new {m.ID, m.LanguageID, m.Module});
            builder.HasQueryFilter(m => m.Deleted == false);
        }
    }
}