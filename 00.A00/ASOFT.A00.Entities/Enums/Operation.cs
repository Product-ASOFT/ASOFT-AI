// ##################################################################
// # Copyright (C) 2010-2011, ASoft JSC.  All Rights Reserved.                       
// #
// # History：
// #    Date Time       Created	        Content
// #    09/12/2020      Tấn Lộc         Create New
// ##################################################################

using System;
using System.ComponentModel.DataAnnotations;

namespace ASOFT.A00.Entities.Enums
{
    /// <summary>
    /// Danh sách enum các Operation.
    /// </summary>
    /// <history>
    ///     [Tấn Lộc] created on [09/12/2020]
    /// </history>
    public enum Operation
    {
        [Display(Name = "~")]
        include,

        [Display(Name = "|~")]
        start,

        [Display(Name = "~|")]
        end,

        [Display(Name = "=")]
        equal,

        [Display(Name = "<>")]
        other,

        [Display(Name = "<")]
        less,

        [Display(Name = "<=")]
        lessEqual,

        [Display(Name = ">")]
        greater,

        [Display(Name = ">=")]
        greaterEqual,

        [Display(Name = "[]")]
        inside,

        [Display(Name = "][")]
        outside,
    }
    public static class EnumHelper<T>
    {
        public static T GetValueFromName(string name)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    if (attribute.Name == name)
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == name)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentOutOfRangeException("name");
        }
    }
}
