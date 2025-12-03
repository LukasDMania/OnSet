namespace OnSet.Domain.ValueObjects
{
    public class LastName : ValueObject
    {
        public string Value { get; }

        private LastName() { }

        public LastName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Last name cannot be empty.");
            }

            if (value.Length > 50)
            {
                throw new ArgumentException("Last name cannot exceed 50 characters.");
            }

            Value = value.Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLower();
        }

        public override string ToString() => Value;

        public static implicit operator string(LastName name) => name.Value;
        public static implicit operator LastName(string value) => new(value);
    }
}
