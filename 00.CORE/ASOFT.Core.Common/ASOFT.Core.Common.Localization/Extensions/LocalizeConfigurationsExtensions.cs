using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace ASOFT.Core.Common.Localization.Extensions
{
    public static class LocalizeConfigurationsExtensions
    {
        /// <summary>
        /// Thêm culture handler xử lý trả về culture
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="localizationConfigurations"></param>
        /// <param name="cultureHandler"></param>
        public static void AddInitialCultureHandlers<TInput>(
            this ILocalizationConfigurations<TInput> localizationConfigurations,
            Func<TInput, IEnumerable<CultureInfo>, StringSegment, ValueTask<CultureInfo>> cultureHandler)
        {
            if (localizationConfigurations == null)
            {
                throw new ArgumentNullException(nameof(localizationConfigurations));
            }

            if (cultureHandler == null)
            {
                throw new ArgumentNullException(nameof(cultureHandler));
            }

            localizationConfigurations.AddInitialCultureHandlers(new InternalCultureHandler<TInput>(cultureHandler));
        }

        /// <summary>
        /// Thêm culture handler xử lý trả về culture
        /// </summary>
        /// <param name="localizationConfigurations"></param>
        /// <param name="cultureHandler"></param>
        /// <typeparam name="TInput"></typeparam>
        public static void AddInitialCultureHandlers<TInput>(
            this ILocalizationConfigurations<TInput> localizationConfigurations,
            ICultureHandler<TInput> cultureHandler)
        {
            if (localizationConfigurations == null)
            {
                throw new ArgumentNullException(nameof(localizationConfigurations));
            }

            if (cultureHandler == null)
            {
                throw new ArgumentNullException(nameof(cultureHandler));
            }

            localizationConfigurations.CultureHandlers?.Insert(0, cultureHandler);
        }

        /// <summary>
        /// Thêm culture handler xử lý trả về ui culture
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="localizationConfigurations"></param>
        /// <param name="uiCultureHandler"></param>
        public static void AddInitialUICultureHandlers<TInput>(
            this ILocalizationConfigurations<TInput> localizationConfigurations,
            Func<TInput, IEnumerable<CultureInfo>, StringSegment, ValueTask<CultureInfo>> uiCultureHandler)
        {
            if (localizationConfigurations == null)
            {
                throw new ArgumentNullException(nameof(localizationConfigurations));
            }

            if (uiCultureHandler == null)
            {
                throw new ArgumentNullException(nameof(uiCultureHandler));
            }

            localizationConfigurations.AddInitialUICultureHandlers(
                new InternalCultureHandler<TInput>(uiCultureHandler));
        }

        /// <summary>
        /// Thêm culture handler xử lý trả về ui culture
        /// </summary>
        /// <param name="localizationConfigurations"></param>
        /// <param name="uiCultureHandler"></param>
        /// <typeparam name="TInput"></typeparam>
        public static void AddInitialUICultureHandlers<TInput>(
            this ILocalizationConfigurations<TInput> localizationConfigurations,
            ICultureHandler<TInput> uiCultureHandler)
        {
            if (localizationConfigurations == null)
            {
                throw new ArgumentNullException(nameof(localizationConfigurations));
            }

            if (uiCultureHandler == null)
            {
                throw new ArgumentNullException(nameof(uiCultureHandler));
            }

            localizationConfigurations.UICultureHandlers?.Insert(0, uiCultureHandler);
        }

        /// <summary>
        /// Thêm culture provider
        /// </summary>
        /// <param name="localizationConfigurations"></param>
        /// <param name="requestCultureProvider"></param>
        /// <typeparam name="TInput"></typeparam>
        public static void AddInitialResultCultureProvider<TInput>(
            this ILocalizationConfigurations<TInput> localizationConfigurations,
            IRequestCultureProvider<TInput> requestCultureProvider)
        {
            if (localizationConfigurations == null)
            {
                throw new ArgumentNullException(nameof(localizationConfigurations));
            }

            if (requestCultureProvider == null)
            {
                throw new ArgumentNullException(nameof(requestCultureProvider));
            }

            localizationConfigurations.CultureProviders?.Insert(0, requestCultureProvider);
        }

        private class InternalCultureHandler<TInput> : ICultureHandler<TInput>
        {
            private readonly Func<TInput, IEnumerable<CultureInfo>, StringSegment, ValueTask<CultureInfo>> _handler;

            public InternalCultureHandler(
                Func<TInput, IEnumerable<CultureInfo>, StringSegment, ValueTask<CultureInfo>> handler)
            {
                _handler = handler ?? throw new ArgumentNullException(nameof(handler));                
            }

            public async ValueTask<CultureInfo> HandleAsync(TInput input, IEnumerable<CultureInfo> supportedCultures,
                StringSegment cultureSegment)
                => await _handler(input, supportedCultures, cultureSegment);
        }
    }
}