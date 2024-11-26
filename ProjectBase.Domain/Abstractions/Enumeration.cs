using System.Reflection;

namespace ProjectBase.Domain.Abstractions
{
    public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>>
        where TEnum : Enumeration<TEnum>
    {
        private static readonly Dictionary<int, TEnum> Enumerations = GetEnumerations();

        public static TEnum? FromValue(int value)
        {
            return Enumerations.TryGetValue(value,
                    out TEnum? enumeration)
                        ? enumeration
                        : default;
        }

        public static TEnum? FromName(string name)
        {
            return Enumerations.Values
                    .FirstOrDefault(x => x.Name == name);
        }

        protected Enumeration(int value, string name)
        {
            Value = value;
            Name = name;
        }

        public int Value { get; set; }
        public string Name { get; set; }

        public bool Equals(Enumeration<TEnum>? other)
        {
            return GetType() == other!.GetType() &&
                Value == other.Value;
        }

        public override string? ToString()
        {
            return Name;
        }

        private static Dictionary<int, TEnum> GetEnumerations()
        {
            var enumerationType = typeof(TEnum);

            var fieldForType = enumerationType
                .GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.FlattenHierarchy)
                .Where(fieldInfo =>
                    enumerationType.IsAssignableFrom(fieldInfo.FieldType))
                .Select(fieldInfo =>
                    (TEnum)fieldInfo.GetValue(default)!);

            return fieldForType.ToDictionary(x => x.Value);
        }
    }
}
