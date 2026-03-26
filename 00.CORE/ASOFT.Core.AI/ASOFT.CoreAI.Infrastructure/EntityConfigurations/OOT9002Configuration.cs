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
    public class OOT9002Configuration : IEntityTypeConfiguration<OOT9002>
    {
        public void Configure(EntityTypeBuilder<OOT9002> builder)
        {
            builder.HasKey(m => m.APK);
        }
    }
}
