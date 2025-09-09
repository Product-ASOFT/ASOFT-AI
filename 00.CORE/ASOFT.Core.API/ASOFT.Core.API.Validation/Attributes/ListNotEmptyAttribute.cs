using JetBrains.Annotations;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace ASOFT.Core.API.Validation.Attributes
{
    /// <summary>
    /// Check <see cref="IList"/> is not empty.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class ListNotEmptyAttribute : ValidationAttribute
    {
        public ListNotEmptyAttribute() : base(() => VSR.Format_ListNotEmpty)
        {
        }

        public override bool IsValid([CanBeNull] object value)
        {
            if (value is IList list)
            {
                return list.Count > 0;
            }

            return true;
        }
    }
}