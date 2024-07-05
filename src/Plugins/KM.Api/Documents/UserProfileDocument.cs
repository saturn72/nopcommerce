
namespace KM.Api.Documents
{
    [FirestoreData]
    public record UserProfileDocument : IDocument
    {
        [FirestoreProperty]
        public string id { get; set; }
        [FirestoreProperty]
        public string userId { get; set; }
        [FirestoreProperty]
        public AddressDocument billingInfo { get; set; }
    }
}
