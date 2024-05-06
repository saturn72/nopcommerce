
namespace Km.Api.Documents
{
    [FirestoreData]
    public record AddressDocument : IDocument
    {
        [FirestoreProperty]
        public string id { get; set; }
        [FirestoreProperty]
        public string address { get; set; }

        [FirestoreProperty]
        public string city { get; set; }
        [FirestoreProperty]
        public string email { get; set; }
        [FirestoreProperty]
        public string fullName { get; set; }
        [FirestoreProperty]
        public string phoneNumber { get; set; }
    }
}
