using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess.ModelBuilderConfiguration;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.Relational;
using ASOFT.Core.DataAccess.Relational.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.DataAccess.EFCore.Relational
{
    /// <summary>
    /// Base DbContext of ASOFT.
    /// </summary>
    public class RelationalDbContext : DbContext, IRelationalUnitOfWork
    {
        private readonly DbContextOptions _options;

        /// <inheritdoc />
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public RelationalDbContext(DbContextOptions options) : base(options)
        {
            _options = options;
        }

        /// <inheritdoc />
        public int Complete() => SaveChanges();

        /// <inheritdoc />
        public Task<int> CompleteAsync() => SaveChangesAsync();

        /// <summary>
        /// Default configs in ASOFT database.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Binding DbFunction
            foreach (var methodInfo in typeof(SqlServerDbFunctions).GetMethods(
                BindingFlags.Static | BindingFlags.Public))
            {
                modelBuilder.HasDbFunction(methodInfo, builder => { builder.HasSchema(string.Empty); });
            }

            var modelBuilderConfigurationProvider =
                _options.Extensions.FirstOrDefault(m => m is IModelBuilderConfigurationProvider);

            // Model builder configuration for multiple modules
            if (modelBuilderConfigurationProvider is IModelBuilderConfigurationProvider provider)
            {
                foreach (var modelBuilderConfiguration in provider.ProvideModelBuilderConfigurations())
                {
                    modelBuilderConfiguration.ConfigureModel(modelBuilder);
                }
            }
        }

        /// <summary>
        /// Thực thi transaction.
        /// </summary>
        /// <param name="taskCreator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ExecuteInTransactionAsync(Func<TransactionHolder, Task> taskCreator,
            CancellationToken cancellationToken = default)
        {
            Checker.NotNull(taskCreator, nameof(taskCreator));

            using (var dbContextTransaction =
                await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    await taskCreator(new TransactionHolder(dbContextTransaction.GetDbTransaction()))
                        .ConfigureAwait(false);
                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    await dbContextTransaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
        }

        /// <summary>
        /// Thực thi transaction.
        /// </summary>
        /// <param name="taskCreator"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> ExecuteInTransactionAsync<T>(Func<TransactionHolder, Task<T>> taskCreator,
            CancellationToken cancellationToken = default)
        {
            Checker.NotNull(taskCreator, nameof(taskCreator));

            using (var dbContextTransaction =
                await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    var result = await taskCreator(new TransactionHolder(dbContextTransaction.GetDbTransaction()))
                        .ConfigureAwait(false);
                    dbContextTransaction.Commit();
                    return result;
                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
        }
    }
}