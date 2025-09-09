// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.DataAccess.EntitiesConfigurations;
using ASOFT.Core.DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASOFT.Core.Business.Files.DataAccess.EntityConfiguarations
{
    /// <summary>
    /// Class config entiti cho bảng CRMT00002
    /// </summary>
    public class CRMT00002Configuaration : BaseEntityTypeConfiguration<CRMT00002>
    {
        public override void Configure(EntityTypeBuilder<CRMT00002> builder)
        {
            base.Configure(builder);
            builder.HasKey(m => new { m.DivisionID, m.AttachID });
            builder.Property(m => m.AttachID).ValueGeneratedOnAdd();
        }
    }
}
