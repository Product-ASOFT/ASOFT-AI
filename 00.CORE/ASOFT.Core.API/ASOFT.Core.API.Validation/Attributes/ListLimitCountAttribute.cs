using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace ASOFT.Core.API.Validation.Attributes
{
    /// <summary>
    /// Limit the count of <see cref="IList"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class ListLimitCountAttribute : ValidationAttribute
    {
        /// <summary>
        /// Minimum count allowed of <see cref="IList"/>.
        /// </summary>
        public int Min { get; set; }


        /// <summary>
        /// Maximum count allowed of <see cref="IList"/>.
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min">Minimum count allowed of <see cref="IList"/></param>
        /// <param name="max">Maximum count allowed of <see cref="IList"/></param>
        public ListLimitCountAttribute(int min, int max) : this(min, max, () => VSR.Format_ListCountMustBeInRange)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min">Minimum count of <see cref="IList"/></param>
        /// <param name="max">Maximum count of <see cref="IList"/></param>
        /// <param name="messageAccessor">Error message function.</param>
        public ListLimitCountAttribute(int min, int max, Func<string> messageAccessor) : base(messageAccessor)
        {
            Min = min;
            Max = max;
        }

        public override bool IsValid(object value)
        {
            ValidOrThrown();
            if (value is IList list)
            {
                int count = list.Count;
                return Min <= count && count <= Max;
            }

            return true;
        }

        private void ValidOrThrown()
        {
            if (Min < 0)
            {
                string minName = nameof(Min);
                throw new ArgumentOutOfRangeException(minName, Min,
                    string.Format(VSR.Format_MustBeGreatOrEqualsThan, minName, 0));
            }

            if (Max < Min)
            {
                string maxName = nameof(Max);
                string minName = nameof(Min);
                throw new ArgumentOutOfRangeException(maxName, Max,
                    string.Format(VSR.Format_MustBeGreatOrEqualsThan, maxName, minName));
            }
        }

        public override string FormatErrorMessage(string name) => string.Format(ErrorMessageString, name, Min, Max);
    }
}