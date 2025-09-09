using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.API.Logging
{
    /// <summary>
    /// Logging behavior
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Logging behavior
        /// </summary>
        /// <param name="loggerFactory"></param>
        public LoggingBehavior(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger(GetType());
        }

        /// <summary>
        /// Handle behavior
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            if (null == request)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (null == next)
            {
                throw new ArgumentNullException(nameof(next));
            }

            _logger.LogRequestCommand(request.GetType().Name);

            var response = await next();

            if (null == response)
            {
                _logger.LogResponseCommandIsNull(typeof(TResponse).Name);
            }

            _logger.LogResponseCommand(response?.GetType().Name ?? typeof(TResponse).Name);


            return response;
        }
    }
}