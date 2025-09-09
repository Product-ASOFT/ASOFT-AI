using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.Relational.Transactions;
using ASOFT.Core.DataAccess.Relational.Transactions.Extensions;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace ASOFT.Core.DataAccess.Relational
{
    /// <summary>
    /// Base data access for interaction with relational database.
    /// </summary>
    public abstract class RelationalDataAccess : IRelationDataExecutor
    {
        private IDbConnectionProvider _connectionProvider;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private TransactionScope _transactionScope;
        private bool ConnectionExisted => _connection != null;
        private bool ConnectionProviderExisted => _connectionProvider != null;
        private bool TransactionExisted => _transaction != null;
        private bool TransactionScopeExisted => _transactionScope != null;

        /// <summary>
        /// Constructor cho <see cref="IDbConnectionProvider"/>
        /// </summary>
        /// <param name="connectionProvider">The provider connection.</param>
        protected RelationalDataAccess(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider =
                connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }

        /// <summary>
        /// Constructor cho <see cref="IDbConnection"/>.
        /// </summary>
        /// <param name="connection"></param>
        protected RelationalDataAccess(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <summary>
        /// Constructor cho <see cref="IDbTransaction"/>.
        /// </summary>
        /// <param name="transaction"></param>
        protected RelationalDataAccess(IDbTransaction transaction)
        {
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        /// <summary>
        /// Run query and return data with connection.
        /// </summary>
        /// <param name="taskCreator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UseConnectionAsync(Func<IDbConnection, Task> taskCreator,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Nếu db connection tồn tại
            if (ConnectionExisted)
            {
                await UseConnectionAsync(_connection, taskCreator, cancellationToken)
                    .ConfigureAwait(false);
                return;
            }

            // Nếu db connection provider tồn tại
            if (ConnectionProviderExisted)
            {
                using (var connection = CreateConnectionIfNeed())
                {
                    await UseConnectionAsync(connection, taskCreator, cancellationToken).ConfigureAwait(false);
                    return;
                }
            }

            // Nếu transaction tồn tại
            if (TransactionExisted)
            {
                await UseConnectionAsync(_transaction.Connection, taskCreator, cancellationToken)
                    .ConfigureAwait(false);
                return;
            }

            throw AllConnectionNotExist();
        }

        /// <summary>
        /// Run query and return data with connection.
        /// </summary>
        /// <param name="taskCreator"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> UseConnectionAsync<T>(Func<IDbConnection, Task<T>> taskCreator,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Nếu db connection tồn tại
            if (ConnectionExisted)
            {
                return await UseConnectionAsync(_connection, taskCreator, cancellationToken)
                    .ConfigureAwait(false);
            }

            // Nếu db connection provider tồn tại
            if (ConnectionProviderExisted)
            {
                using (var connection = CreateConnectionIfNeed())
                {
                    return await UseConnectionAsync(connection, taskCreator, cancellationToken).ConfigureAwait(false);
                }
            }

            // Nếu transaction tồn tại
            if (TransactionExisted)
            {
                return await UseConnectionAsync(_transaction.Connection, taskCreator, cancellationToken)
                    .ConfigureAwait(false);
            }

            throw AllConnectionNotExist();
        }

        /// <summary>
        /// Sử dụng transaction.
        /// </summary>
        /// <param name="taskCreator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task UseTransactionAsync(Func<IDbConnection, TransactionHolder, Task> taskCreator,
            CancellationToken cancellationToken = default)
        {
            // Nếu transaction scope tồn tại
            if (TransactionScopeExisted)
            {
                // Nếu db connection tồn tại thì ưu tiên sử dụng
                if (ConnectionExisted)
                {
                    await UseTransactionAsync(_connection, new TransactionHolder(_transactionScope),
                        taskCreator,
                        cancellationToken);
                    return;
                }

                // Nếu db connection provider tồn tại thì sử dụng
                if (ConnectionProviderExisted)
                {
                    using (var connection = CreateConnectionIfNeed())
                    {
                        await UseTransactionAsync(connection, new TransactionHolder(_transactionScope),
                            taskCreator,
                            cancellationToken);
                        return;
                    }
                }

                // Cả DbConnection và DbConnectionProvider đều không tồn tại.
                throw new InvalidOperationException(
                    $"{nameof(_connection)}, {nameof(_connectionProvider)} all both not exist.");
            }

            // Nếu tồn tại db transaction
            if (TransactionExisted)
            {
                if (ConnectionExisted)
                {
                    await UseTransactionAsync(_connection, new TransactionHolder(_transaction),
                        taskCreator,
                        cancellationToken);
                    return;
                }

                if (ConnectionProviderExisted)
                {
                    using (var connection = CreateConnectionIfNeed())
                    {
                        await UseTransactionAsync(connection, new TransactionHolder(_transaction),
                            taskCreator,
                            cancellationToken);
                        return;
                    }
                }

                await UseTransactionAsync(_transaction.Connection,
                    new TransactionHolder(_transaction), taskCreator,
                    cancellationToken);
                return;
            }

            // Nếu tồn tại db connection
            if (ConnectionExisted)
            {
                var connection = _connection ?? throw new ArgumentNullException(nameof(_connection));

                // Flag để kiểm tra connection được open hay chưa.
                // Nếu chưa được open thì sẽ open connection rồi sau đó sẽ tự động đóng connection.
                // Nếu connection được open từ tác nhân phía bên ngoài thì giữ nguyên hiện trạng.
                var shouldCloseConnection = connection.State == ConnectionState.Closed;
                try
                {
                    if (shouldCloseConnection)
                    {
                        connection.Open();
                    }

                    // Transaction sử tự động commit sau đó dispose do được tự động khởi tạo
                    using (var transaction = CreateTransactionIfNeed(connection))
                    {
                        try
                        {
                            await UseTransactionAsync(connection, new TransactionHolder(transaction),
                                taskCreator,
                                cancellationToken);
                            transaction.TryCommit();
                            return;
                        }
                        catch (Exception)
                        {
                            transaction.TryRollback();
                            throw;
                        }
                    }
                }
                finally
                {
                    // Nếu connection được tự động open và trạng thái đang open
                    if (shouldCloseConnection && connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }

            // Nếu tồn tại db connection provider
            if (ConnectionProviderExisted)
            {
                using (var connection = CreateConnectionIfNeed())
                {
                    connection.Open();
                    using (var transaction = CreateTransactionIfNeed(connection))
                    {
                        try
                        {
                            await UseTransactionAsync(connection, new TransactionHolder(transaction),
                                taskCreator,
                                cancellationToken);
                            transaction.TryCommit();
                            return;
                        }
                        catch
                        {
                            transaction.TryRollback();
                            throw;
                        }
                    }
                }
            }

            throw AllConnectionNotExist();
        }

        /// <summary>
        /// Sử dụng transaction.
        /// </summary>
        /// <param name="taskCreator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<T> UseTransactionAsync<T>(Func<IDbConnection, TransactionHolder, Task<T>> taskCreator,
            CancellationToken cancellationToken = default)
        {
            // Nếu transaction scope tồn tại
            if (TransactionScopeExisted)
            {
                // Nếu db connection tồn tại thì ưu tiên sử dụng
                if (ConnectionExisted)
                {
                    return await UseTransactionAsync(_connection, new TransactionHolder(_transactionScope),
                        taskCreator,
                        cancellationToken);
                }

                // Nếu db connection provider tồn tại thì sử dụng
                if (ConnectionProviderExisted)
                {
                    using (var connection = CreateConnectionIfNeed())
                    {
                        return await UseTransactionAsync(connection, new TransactionHolder(_transactionScope),
                            taskCreator,
                            cancellationToken);
                    }
                }

                // Cả DbConnection và DbConnectionProvider đều không tồn tại.
                throw new InvalidOperationException(
                    $"{nameof(_connection)}, {nameof(_connectionProvider)} all both not exist.");
            }

            // Nếu tồn tại db transaction
            if (TransactionExisted)
            {
                if (ConnectionExisted)
                {
                    return await UseTransactionAsync(_connection, new TransactionHolder(_transaction),
                        taskCreator,
                        cancellationToken);
                }

                if (ConnectionProviderExisted)
                {
                    using (var connection = CreateConnectionIfNeed())
                    {
                        return await UseTransactionAsync(connection, new TransactionHolder(_transaction),
                            taskCreator,
                            cancellationToken);
                    }
                }

                return await UseTransactionAsync(_transaction.Connection,
                    new TransactionHolder(_transaction),
                    taskCreator,
                    cancellationToken);
            }

            // Nếu tồn tại db connection
            if (ConnectionExisted)
            {
                var connection = _connection ?? throw new ArgumentNullException(nameof(_connection));

                // Flag để kiểm tra connection được open hay chưa.
                // Nếu chưa được open thì sẽ open connection rồi sau đó sẽ tự động đóng connection.
                // Nếu connection được open từ tác nhân phía bên ngoài thì giữ nguyên hiện trạng.
                var shouldCloseConnection = connection.State == ConnectionState.Closed;
                try
                {
                    if (shouldCloseConnection)
                    {
                        connection.Open();
                    }

                    using (var transaction = CreateTransactionIfNeed(connection))
                    {
                        try
                        {
                            var result = await UseTransactionAsync(connection, new TransactionHolder(transaction),
                                taskCreator,
                                cancellationToken);
                            transaction.TryCommit();
                            return result;
                        }
                        catch (Exception)
                        {
                            transaction.TryRollback();
                            throw;
                        }
                    }
                }
                finally
                {
                    // Nếu connection được tự động open và trạng thái đang open
                    if (shouldCloseConnection && connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }

            // Nếu tồn tại db connection provider
            if (ConnectionProviderExisted)
            {
                using (var connection = CreateConnectionIfNeed())
                {
                    connection.Open();
                    using (var transaction = CreateTransactionIfNeed(connection))
                    {
                        try
                        {
                            var result = await UseTransactionAsync(connection, new TransactionHolder(transaction),
                                taskCreator,
                                cancellationToken);
                            transaction.TryCommit();
                            return result;
                        }
                        catch (Exception)
                        {
                            transaction.TryRollback();
                            throw;
                        }
                    }
                }
            }

            throw AllConnectionNotExist();
        }

        /// <summary>
        /// Tạo <see cref="IDbTransaction"/> nếu cần thiết.
        /// </summary>
        /// <returns></returns>
        protected virtual IDbTransaction CreateTransactionIfNeed(IDbConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            return connection.BeginTransaction();
        }

        /// <summary>
        /// Get db connection key for db connection provider.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetDbConnectionProviderKey();

        /// <summary>
        /// Set <see cref="IDbTransaction"/>.
        /// </summary>
        /// <param name="dbTransaction"></param>
        public void SetTransaction(IDbTransaction dbTransaction) => _transaction = dbTransaction;

        /// <summary>
        /// Set <see cref="IDbConnection"/>.
        /// </summary>
        /// <param name="dbConnection"></param>
        public void SetConnection(IDbConnection dbConnection) => _connection = dbConnection;

        /// <summary>
        /// Set <see cref="IDbConnectionProvider"/>.
        /// </summary>
        /// <param name="dbConnectionProvider"></param>
        public void SetDbConnectionProvider(IDbConnectionProvider dbConnectionProvider)
            => _connectionProvider = dbConnectionProvider;

        /// <summary>
        /// Set <see cref="TransactionScope"/>.
        /// </summary>
        /// <param name="transactionScope"></param>
        public void SetTransactionScope(TransactionScope transactionScope)
            => _transactionScope = transactionScope;

        /// <summary>
        /// See <see cref="TransactionHolder"/>.
        /// </summary>
        /// <param name="transactionHolder"></param>
        public void SetTransactionHolder(TransactionHolder transactionHolder)
        {
            SetTransaction(transactionHolder.GetTransactionOrDefault());
            SetTransactionScope(transactionHolder.GetTransactionScopeOrDefault());
        }

        private IDbConnection CreateConnectionIfNeed() =>
            (_connectionProvider ?? throw new ArgumentNullException(nameof(_connectionProvider)))
            .ProvideDbConnection(GetDbConnectionProviderKey());

        /// <summary>
        /// Run query with connection.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="taskCreator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static async Task UseConnectionAsync(IDbConnection connection,
            Func<IDbConnection, Task> taskCreator, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (taskCreator == null)
            {
                throw new ArgumentNullException(nameof(taskCreator));
            }

            await taskCreator(connection).ConfigureAwait(false);
        }

        /// <summary>
        /// Run query and return data with connection.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="taskCreator"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static async Task<T> UseConnectionAsync<T>(IDbConnection connection,
            Func<IDbConnection, Task<T>> taskCreator, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (taskCreator == null)
            {
                throw new ArgumentNullException(nameof(taskCreator));
            }

            return await taskCreator(connection).ConfigureAwait(false);
        }

        private static async Task UseTransactionAsync(IDbConnection connection,
            TransactionHolder transactionHolder,
            Func<IDbConnection, TransactionHolder, Task> taskCreator,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (taskCreator == null)
            {
                throw new ArgumentNullException(nameof(taskCreator));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await taskCreator(connection, transactionHolder).ConfigureAwait(false);
        }

        private static async Task<T> UseTransactionAsync<T>(IDbConnection connection,
            TransactionHolder transactionHolder,
            Func<IDbConnection, TransactionHolder, Task<T>> taskCreator,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (taskCreator == null)
            {
                throw new ArgumentNullException(nameof(taskCreator));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return await taskCreator(connection, transactionHolder).ConfigureAwait(false);
        }

        private InvalidOperationException AllConnectionNotExist() => new InvalidOperationException(
            $"{nameof(_connection)}, {nameof(_connectionProvider)}, {nameof(_transaction)} all both not exist.");
    }
}