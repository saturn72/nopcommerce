
namespace KM.Orders.Documents
{
    public record UserDocument
    {
        public string UserId { get; init; }
        public string DisplayName { get; init; }
        public string Email { get; init; }
        public bool EmailVerified { get; init; }
        public string PhoneNumber { get; init; }
        public string PhotoUrl { get; init; }
        public string Provider { get; init; }
        public IEnumerable<KeyValuePair<string, object>> Claims { get; init; }
    }
}
