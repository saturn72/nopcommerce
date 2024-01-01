
namespace KM.Orders.Documents
{
    [FirestoreData]
    public record AddressDocument
    {
        [FirestoreProperty]
        public string address { get; set; }
        [FirestoreProperty]
        public string city { get; set; }
        [FirestoreProperty]
        public string email { get; set; }
        [FirestoreProperty]
        public string fullName{ get; set; }
        [FirestoreProperty]
        public string phoneNumber { get; set; }
    }
}
