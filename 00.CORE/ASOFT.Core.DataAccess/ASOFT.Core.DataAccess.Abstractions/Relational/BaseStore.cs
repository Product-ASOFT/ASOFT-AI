using ASOFT.Core.DataAccess;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.DataAccess.Relational
{
    /// <summary>
    /// Base store data access
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public abstract class BaseStore<TInput, TOutput> : RelationalDataAccess
    {
        /// <summary>
        /// Logger error action template mặc định cho store
        /// </summary>
        protected static readonly Action<ILogger, string, TInput, Exception> LogDebugBeforeStoreExecutingAction =
            LoggerMessage.Define<string, TInput>(LogLevel.Debug,
                new EventId(-1, nameof(BaseStore<TInput, TOutput>)),
                "Store {StoreName} executing with {@Params}.");

        /// <summary>
        /// Logger error action template mặc định cho store
        /// </summary>
        protected static readonly Action<ILogger, string, TInput, string, Exception> LogErrorOnStoreExecutingAction =
            LoggerMessage.Define<string, TInput, string>(LogLevel.Error,
                new EventId(-1, nameof(BaseStore<TInput, TOutput>)),
                "Executing Store {StoreName} is error with Params: {@Params} ==> ErrorMessage: {Message}.");

        private readonly ILogger _logger;

        /// <summary>
        /// The name of store
        /// </summary>
        public string StoreName { get; }

        /// <inheritdoc />
        protected BaseStore(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
            StoreName = InternalGetStoreName();
        }

        /// <inheritdoc />
        protected BaseStore(string storeName, IDbConnectionProvider connectionProvider) : base(
            connectionProvider)
        {
            if (string.IsNullOrWhiteSpace(storeName))
            {
                throw new ArgumentNullException(nameof(storeName));
            }

            StoreName = storeName;
        }

        /// <inheritdoc />
        protected BaseStore(IDbConnectionProvider connectionProvider, ILoggerFactory loggerFactory) : this(
            connectionProvider)
        {
            StoreName = InternalGetStoreName();

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger(GetType());
        }

        /// <inheritdoc />
        protected BaseStore(string storeName, IDbConnectionProvider connectionProvider,
            ILoggerFactory loggerFactory)
            : this(connectionProvider)
        {
            if (string.IsNullOrWhiteSpace(storeName))
            {
                throw new ArgumentNullException(nameof(storeName));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            StoreName = storeName;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        /// <summary>
        /// Log debug for store params before executing.
        /// </summary>
        /// <param name="paramsEntry"></param>
        protected virtual void LogDebugStoreBeforeExecuting(TInput paramsEntry)
        {
            if (_logger != null)
            {
                LogDebugBeforeStoreExecutingAction(_logger, StoreName, paramsEntry, null);
            }
        }

        /// <summary>
        /// Log error for store before executing.
        /// </summary>
        /// <param name="paramsEntry"></param>
        /// <param name="ex"></param>
        protected virtual void LogErrorStoreExecuting(TInput paramsEntry, Exception ex)
        {
            if (_logger != null)
            {
                LogErrorOnStoreExecutingAction(_logger, StoreName, paramsEntry, ex?.Message, ex);
            }
        }

        /// <summary>
        /// Get store name
        /// </summary>
        /// <returns></returns>
        private string InternalGetStoreName()
        {
            var type = GetType();

            var methodInfo = type.GetMethod(nameof(ExecuteAsync));

            if (methodInfo == null)
            {
                throw new InvalidOperationException($"Cannot find method: {nameof(ExecuteAsync)}");
            }

            var storeAttribute = methodInfo.GetCustomAttribute<StoreAttribute>(false) ??
                                 type.GetCustomAttribute<StoreAttribute>(false);

            return storeAttribute == null ? GetNameWithoutGeneric(type) : storeAttribute.StoreName;
        }

        /// <summary>
        /// Abstract function for class implement execute with it.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<TOutput> ExecuteAsync(TInput entry, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handle for get name of generic type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetNameWithoutGeneric(Type type)
        {
            var name = type.Name;
            if (!type.IsGenericType) return name;
            var indexOfBackTick = name.IndexOf('`');
            return indexOfBackTick > 0 ? name.Remove(indexOfBackTick) : name;
        }
    }
}