namespace ASOFT.Core.DataAccess.Entities
{
    /// <summary>
    /// The class represent nothing to return.
    /// </summary>
    public sealed class Nothing
    {
        /// <summary>
        /// The static object. Used to for avoid allocated object.
        /// </summary>
        public static readonly Nothing Instance = new Nothing();

        private Nothing()
        {
        }
    }
}