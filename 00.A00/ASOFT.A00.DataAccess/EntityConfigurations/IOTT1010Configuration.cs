using ASOFT.A00.Entities;
using ASOFT.Core.DataAccess.EntitiesConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASOFT.A00.DataAccess.EntityConfigurations
{
    public class IOTT1010Configuration : BaseEntityTypeConfiguration<IOTT1010>
    {
        public override void Configure(EntityTypeBuilder<IOTT1010> builder)
        {
            base.Configure(builder);
            builder.HasKey(m => m.ClientDomain);
            builder.Property(m => m.APK)
                .HasValueGenerator<TemporaryGuidValueGenerator>()
                .ValueGeneratedOnAdd();
        }
    }
}
