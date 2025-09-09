using ASOFT.Core.Common.InjectionChecker;
using JetBrains.Annotations;
using System;

namespace ASOFT.Core.API.Httpss.Errors.Exceptions
{
    /// <summary>
    /// Exception cho error code
    /// </summary>
    public class ErrorCodeException : Exception
    {
        /// <summary>
        /// Error status code
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// Error model
        /// </summary>
        public ErrorModel ErrorModel { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="errorModel"></param>
        public ErrorCodeException([NotNull] ErrorModel errorModel) : base(errorModel.Message) =>
            ErrorModel = Checker.NotNull(errorModel, nameof(errorModel));

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="errorModel"></param>
        /// <param name="statusCode"></param>
        public ErrorCodeException([NotNull] ErrorModel errorModel, int statusCode) : this(errorModel)
            => StatusCode = statusCode;
    }
}