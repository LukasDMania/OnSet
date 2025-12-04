namespace OnSet.Domain.ValueObjects
{
    public class SignatureInfo : ValueObject
    {
        public string SignedByUserId { get; private set; }
        public DateTime SignedAt { get; private set; }

        private SignatureInfo() { }

        public SignatureInfo(string signedByUserId, DateTime signedAt)
        {
            SignedByUserId = signedByUserId;
            SignedAt = signedAt;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return SignedByUserId;
            yield return SignedAt;
        }
    }

}
