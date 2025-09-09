using System;
using Microsoft.AspNetCore.Builder;

namespace ASOFT.Core.Common.Localization.DependencyInjection.Extensions
{
    public static class LocalizationApplicationBuilderExtensions
    {
        /// <summary>
        /// Thêm <see cref="RequestLocalizationMiddleware" /> tự động thiết lập culture theo yêu cầu
        /// request của client gửi lên.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestCultureLocalization(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<RequestLocalizationMiddleware>();
        }
    }
}