using OnSet.Application.Exceptions;

namespace OnSet.Utils
{
    /// <summary>Parses string values to enums for profile and edit forms.</summary>
    public static class EnumConverter
    {
        /// <summary>Converts a sequence of strings to enum values; throws <see cref="Application.Exceptions.EnumConversionException"/> on invalid input.</summary>
        public static List<T> ToEnumList<T>(IEnumerable<string> values) where T : struct, Enum
        {
            var list = new List<T>();

            foreach (var value in values ?? Enumerable.Empty<string>())
            {
                if (Enum.TryParse<T>(value, true, out var enumValue))
                {
                    list.Add(enumValue);
                }
                else
                {
                    throw new EnumConversionException($"Invalid value '{value}' for enum {typeof(T).Name}");
                }
            }

            return list;
        }

        public static T ToEnum<T>(string value) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new EnumConversionException($"Value cannot be null or empty for enum {typeof(T).Name}");

            if (Enum.TryParse<T>(value, true, out var enumValue))
                return enumValue;

            throw new EnumConversionException($"Invalid value '{value}' for enum {typeof(T).Name}");
        }


        public static List<string> ToStringList<T>(IEnumerable<T> enumValues) where T : struct, Enum
        {
            return enumValues?.Select(e => e.ToString()).ToList() ?? new List<string>();
        }
    }
}
