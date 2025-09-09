using ASOFT.Core.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;

namespace ASOFT.Core.DataAccess.ModelBuilderConfiguration
{
    internal class InternalDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
    {
        public InternalDbContextOptionsExtensionInfo(IDbContextOptionsExtension extension, string logFragment) : base(extension)
        {
            LogFragment = logFragment;
        }

        public override int GetServiceProviderHashCode() => 0;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {

        }

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is InternalDbContextOptionsExtensionInfo;
        public override bool IsDatabaseProvider => false;
        public override string LogFragment { get; }
    }
}
