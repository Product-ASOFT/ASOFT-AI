using ASOFT.CoreAI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure.EntityConfigurations
{
    public class OOT9003Configuration : IEntityTypeConfiguration<OOT9003>
    {
        public void Configure(EntityTypeBuilder<OOT9003> builder)
        {
            builder.HasKey(m => m.APK);
        }
    }
}
