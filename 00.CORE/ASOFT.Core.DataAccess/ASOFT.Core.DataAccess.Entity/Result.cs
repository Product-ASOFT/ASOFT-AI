namespace ASOFT.Core.DataAccess.Entities
{
    /// <summary>
    /// The class represent state of success or error.
    /// </summary>
    /// <typeparam name="S">The value when succeeded.</typeparam>
    /// <typeparam name="E">The value when has error.</typeparam>
    public sealed class Result<S, E>
    {
        /// <summary>
        /// Is succeed or error.
        /// </summary>
        public readonly bool IsSucceed;

        /// <summary>
        /// The value of success status.
        /// This value can be null if <see cref="IsSucceed"/> is <code>false</code>.
        /// </summary>
        public readonly S Success;

        /// <summary>
        /// The value of error status.
        /// This value can be null if <see cref="IsSucceed"/> is <code>true</code>.
        /// </summary>
        public readonly E Error;

        private Result(S success) : this(true) => Success = success;

        private Result(E error) : this(false) => Error = error;

        private Result(bool isSucceed) => IsSucceed = isSucceed;

        /// <summary>
        /// Create new <see cref="Result{S,E}"/> from success status <see cref="IsSucceed"/>.
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        public static Result<S, E> FromSuccess(S success) => new Result<S, E>(success);

        /// <summary>
        /// Create new <see cref="Result{S,E}"/> from error status <see cref="IsSucceed"/>.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Result<S, E> FromError(E error) => new Result<S, E>(error);
    }
}