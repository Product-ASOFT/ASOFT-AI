namespace ASOFT.Core.Common.Security
{
    /// <summary>
    /// Class hash password
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <history>
        ///     [Luan Le] Created [22/07/2019]
        /// </history>
        string Encrypt(string password);

        /// <summary>
        /// Chứng thực password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        /// <history>
        ///     [Luan Le] Created [22/07/2019]
        /// </history>
        bool VerifyHash(string hashedPassword, string password);
    }
}