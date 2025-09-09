using ASOFT.A00.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASOFT.A00.DataAccess.EntityConfigurations
{
    public class IOTT1011Configuration : IEntityTypeConfiguration<IOTT1011>
    {
        public void Configure(EntityTypeBuilder<IOTT1011> builder)
        {
            builder.HasKey(m => new { m.ClientDomain, m.AccessToken });
        }
    }
}
