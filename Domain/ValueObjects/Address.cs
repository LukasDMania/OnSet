namespace OnSet.Domain.ValueObjects
{
    /// <summary>Domain model or value object.</summary>
    public class Address : ValueObject
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string ProvinceOrState { get; private set; }
        public string Country { get; private set; }
        public string ZipCode { get; private set; }

        private Address() { }

        public Address(
            string street,
            string city,
            string provinceOrState,
            string country,
            string zipCode)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street cannot be empty.");

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty.");

            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country cannot be empty.");

            if (string.IsNullOrWhiteSpace(zipCode))
                throw new ArgumentException("ZipCode cannot be empty.");

            Street = street.Trim();
            City = city.Trim();
            ProvinceOrState = provinceOrState?.Trim() ?? "";
            Country = country.Trim();
            ZipCode = zipCode.Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street.ToLower();
            yield return City.ToLower();
            yield return ProvinceOrState.ToLower();
            yield return Country.ToLower();
            yield return ZipCode.ToLower();
        }

        public override string ToString() =>
            $"{Street}, {City}, {ProvinceOrState}, {Country}, {ZipCode}";
    }
}
