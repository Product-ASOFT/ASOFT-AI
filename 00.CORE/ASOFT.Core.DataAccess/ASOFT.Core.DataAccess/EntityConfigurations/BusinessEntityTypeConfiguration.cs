using ASOFT.Core.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASOFT.Core.DataAccess.EntitiesConfigurations
{
    /// <summary>
    /// Base configuration entity type for Entity that extends <see cref="TBusinessEntity"/>
    /// </summary>
    /// <history>
    /// Luan Le [Created] 2019/09/12
    /// </history>
    /// <typeparam name="TBusinessEntity"></typeparam>
    public abstract class
        BusinessEntityTypeConfiguration<TBusinessEntity> : BaseEntityTypeConfiguration<TBusinessEntity>
        where TBusinessEntity : BusinessEntity
    {
        /// <summary>
        /// Thiết lập cho business entity
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(EntityTypeBuilder<TBusinessEntity> builder)
        {
            base.Configure(builder);
            builder.HasQueryFilter(m => m.DeleteFlg == 0);
            // HasDefaultValue must be cast to byte, otherwise the default value understand is integer type 
            // and it's thrown the error bacause DeleteFlg property is byte type.
            builder.Property(m => m.DeleteFlg).HasDefaultValue((byte) 0);
        }
    }
}