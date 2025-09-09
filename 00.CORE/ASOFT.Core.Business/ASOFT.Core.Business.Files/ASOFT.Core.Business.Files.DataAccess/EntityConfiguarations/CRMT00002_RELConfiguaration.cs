// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ASOFT.Core.Business.Files.DataAccess.EntityConfiguarations
{
    /// <summary>
    /// Class config entiti cho bảng CRMT00002_REL
    /// </summary>
    public class CRMT00002_RELConfiguaration: IEntityTypeConfiguration<CRMT00002_REL>
    {
        public void Configure(EntityTypeBuilder<CRMT00002_REL> builder)
        {
            builder.HasKey(m => new { m.DivisionID, m.AttachID, m.RelatedToID, m.RelatedToTypeID_REL });
            builder.Property(m => m.APK)
                  .HasValueGenerator<TemporaryGuidValueGenerator>()
                  .ValueGeneratedOnAdd();
        }
    }
}
