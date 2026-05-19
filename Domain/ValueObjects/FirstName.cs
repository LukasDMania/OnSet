namespace OnSet.Domain.ValueObjects
{
    /// <summary>Domain model or value object.</summary>
    public class FirstName : ValueObject
    {
        public string Value { get; }

        private FirstName() { }

        public FirstName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("First name cannot be empty.");
            }

            if (value.Length > 50)
            {
                throw new ArgumentException("First name cannot exceed 100 characters.");
            }
            Value = value.Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLower();
        }

        public override string ToString() => Value;

        public static implicit operator string(FirstName name) => name.Value;
        public static implicit operator FirstName(string value) => new(value);
    }
}
