namespace OnSet.Domain.ValueObjects
{
    /// <summary>Domain model or value object.</summary>
    public class EmergencyContact : ValueObject
    {
        public string Name { get; private set; }
        public string Phone { get; private set; }

        private EmergencyContact() { }

        public EmergencyContact(string name, string phone)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Emergency contact name is required.");

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Emergency contact phone is required.");

            Name = name.Trim();
            Phone = phone.Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name.ToLower();
            yield return Phone.ToLower();
        }

        public override string ToString() => $"{Name} ({Phone})";
    }
}
