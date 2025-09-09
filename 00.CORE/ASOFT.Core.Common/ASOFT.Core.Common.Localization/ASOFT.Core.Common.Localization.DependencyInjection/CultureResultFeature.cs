using System;

namespace ASOFT.Core.Common.Localization.DependencyInjection
{
    public class CultureResultFeature : ICultureResultFeature
    {
        public ICultureResult CultureResult { get; }

        public CultureResultFeature(ICultureResult cultureResult)
        {
            CultureResult = cultureResult ?? throw new ArgumentNullException(nameof(cultureResult));
        }
    }
}