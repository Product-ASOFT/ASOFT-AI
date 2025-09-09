using ASOFT.Core.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;

namespace ASOFT.Core.DataAccess.EntitiesConfigurations
{
    /// <summary>
    /// Base configuration entity type for Entity that extends.
    /// </summary>
    /// <history>
    /// Luan Le [Created] 2019/09/12
    /// </history>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class BaseEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        /// <summary>
        /// Thiết lập cho base entity.
        /// </summary>
        /// <param name="builder"></param>
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // The property APK has default temporary guid value
            // when new record is added. This temporary guid value will be replaced
            // by database when it's be inserted to database.
            builder.Property(m => m.APK)
                .HasValueGenerator<TemporaryGuidValueGenerator>()
                .ValueGeneratedOnAdd();
        }
    }
}