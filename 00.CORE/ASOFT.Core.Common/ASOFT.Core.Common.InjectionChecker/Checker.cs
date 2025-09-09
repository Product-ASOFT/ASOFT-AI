using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace ASOFT.Core.Common.InjectionChecker
{
    /// <summary>
    /// Check valid object or throw error
    /// </summary>
    public static class Checker
    {
        /// <summary>
        /// Thrown error if value is null otherwise return value;
        /// </summary>
        /// <param name="value">value for check can be null.</param>
        /// <param name="parameterName">name of value.</param>
        /// <exception cref="ArgumentNullException">Thrown if value is null or parameterName is null.</exception>
        /// <exception cref="ArgumentException">Thrown if parameterName is empty.</exception>
        [NotNull]
        public static T NotNull<T>([CanBeNull] T value, [NotNull] string parameterName)
        {
#pragma warning disable IDE0041 // Use 'is null' check
            if (ReferenceEquals(value, null))
#pragma warning restore IDE0041 // Use 'is null' check
            {
                NotEmpty(parameterName, nameof(parameterName));
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// Thrown error if value is null or empty
        /// </summary>
        /// <param name="value">value for check can be null or empty.</param>
        /// <param name="parameterName">name of value.</param>
        /// <exception cref="ArgumentNullException">Thrown if value is null or parameterName is null.</exception>
        /// <exception cref="ArgumentException">Thrown if parameterName is empty.</exception>
        [NotNull]
        public static string NotEmpty([CanBeNull] string value, [NotNull] string parameterName)
        {
            Exception error = null;
            if (value is null)
            {
                NotEmpty(parameterName, nameof(parameterName));
                error = new ArgumentNullException(parameterName);
            }
            else if (value.Trim().Length == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));
                error = new ArgumentException($"{parameterName} is empty.");
            }

            if (error != null)
            {
                throw error;
            }

            return value;
        }


        /// <summary>
        /// Check list not empty or thrown.
        /// </summary>
        /// <param name="value">value for check.</param>
        /// <param name="parameterName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if value is null or parameterName is null.</exception>
        /// <exception cref="ArgumentException">Thrown if value is empty or parameterName is empty.</exception>
        [NotNull]
        public static ICollection<T> NotEmpty<T>([CanBeNull] ICollection<T> value, [NotNull] string parameterName)
        {
            NotEmpty(parameterName, nameof(parameterName));
            NotNull(value, parameterName);

            // ReSharper disable once PossibleNullReferenceException
            if (value.Count > 0)
            {
                return value;
            }

            throw new ArgumentException($"{parameterName} cannot be empty.");
        }
    }
}