using System;
using System.Linq;

namespace TqkLibrary.Http.Api.GoogleDocs
{
    internal static class Extensions
    {
        internal static TAttribute? GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var enumType = value.GetType();
            string? name = Enum.GetName(enumType, value);
            if (string.IsNullOrWhiteSpace(name))
                return default(TAttribute);
            return enumType.GetField(name)?.GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }
    }
}
