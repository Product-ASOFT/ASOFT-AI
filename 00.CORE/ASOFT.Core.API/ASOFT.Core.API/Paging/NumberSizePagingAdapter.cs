using ASOFT.Core.Common.InjectionChecker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASOFT.Core.API.Paging
{
    /// <summary>
    /// Class này phải dùng với scope service vì xài HttpContext.
    /// Dùng để tạo cấu trúc phân trang response bao gồm next link and previous link.
    /// </summary>
    public class NumberSizePagingAdapter : INumberSizePagingAdapter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="linkGenerator"></param>
        public NumberSizePagingAdapter(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            _httpContextAccessor = Checker.NotNull(httpContextAccessor, nameof(httpContextAccessor));
            _linkGenerator = Checker.NotNull(linkGenerator, nameof(linkGenerator));
        }

        /// <summary>
        /// Tạo paging response model
        /// </summary>
        /// <param name="pagingEntity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual PagingResponseModel<T> Create<T>(NumberSizePagingEntity<T> pagingEntity)
        {
            Checker.NotNull(pagingEntity, nameof(pagingEntity));

            var model = new PagingResponseModel<T>
            {
                TotalCount = pagingEntity.TotalCount,
                Items = pagingEntity.Items
            };

            if (pagingEntity.HasNextPage || pagingEntity.HasPreviousPage)
            {
                model.Paging = new PagingLinks
                {
                    Next = pagingEntity.HasNextPage
                        ? GetPageLink(pagingEntity.NextPageNumber, _linkGenerator, _httpContextAccessor.HttpContext)
                        : null,
                    Previous = pagingEntity.HasPreviousPage
                        ? GetPageLink(pagingEntity.PreviousPageNumber, _linkGenerator, _httpContextAccessor.HttpContext)
                        : null
                };
            }

            return model;
        }

        private static string GetPageLink(int pageNumber, LinkGenerator linkGenerator, HttpContext httpContext)
        {
            var query = httpContext.Request.Query;
            var pageNumberKey =
                query.Keys.FirstOrDefault(m => m.Equals("PageNumber", StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(pageNumberKey))
            {
                return null;
            }

            var values = query.Select(keyValue => keyValue.Key.Equals(pageNumberKey, StringComparison.OrdinalIgnoreCase)
                    ? new KeyValuePair<string, object>(keyValue.Key, pageNumber)
                    : new KeyValuePair<string, object>(keyValue.Key, keyValue.Value.FirstOrDefault()))
                .ToList();

            return linkGenerator.GetUriByRouteValues(httpContext, null, values);
        }
    }
}