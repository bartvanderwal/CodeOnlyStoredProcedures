﻿using System;

namespace CodeOnlyStoredProcedure.DataTransformation
{
    /// <summary>
    /// Removes all whitespace from a string value
    /// </summary>
    public class TrimAttribute : DataTransformerAttributeBase
    {
        /// <summary>
        /// Creates a TrimAttribute, with the given application order.
        /// </summary>
        /// <param name="order">The order in which to apply the attribute. Defaults to 0.</param>
        public TrimAttribute(int order = 0)
            : base(order)
        {
        }

        /// <summary>
        /// Removes all whitespace from the input
        /// </summary>
        /// <param name="value">The string to trim</param>
        /// <param name="targetType">The type to transform to. Only string types are supported.</param>
        /// <param name="isNullable">If the target property is a nullable of type <paramref name="targetType"/></param>
        /// <returns>The trimmed string, or empty if a null value was passed.</returns>
        public override object Transform(object value, Type targetType, bool isNullable)
        {
            if (targetType != typeof(string))
                throw new NotSupportedException("Can only set the TrimAttribute on a String property");

            if (!(value is string))
            {
                if (ReferenceEquals(null, value))
                    return string.Empty;

                throw new NotSupportedException("Can only trim a string value");
            }

            var str = (string)value;
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            return str.Trim();
        }
    }
}
