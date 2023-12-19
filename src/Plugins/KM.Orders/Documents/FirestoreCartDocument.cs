
namespace KM.Orders.Documents
{
    [FirestoreData]
    public class FirestoreCartDocument : IDocument
    {
        [FirestoreProperty]
        public string id { get; set; }
        [FirestoreProperty]
        public IEnumerable<string> couponCodes { get; set; }
        [FirestoreProperty]
        public UserProfileDocument user { get; set; }
        [FirestoreProperty]
        public decimal customerEnteredPrice { get; internal set; }
        [FirestoreProperty]
        public IEnumerable<CartItemDocument> items { get; set; }
        [FirestoreProperty]
        public string paymentMethod { get; set; }
        [FirestoreProperty]
        public string rentalEndDateUtc { get; set; }
        [FirestoreProperty]
        public string rentalStartDateUtc { get; set; }
        [FirestoreProperty]
        public int storeId { get; set; }
        [FirestoreProperty]
        public string status { get; set; }
        [FirestoreProperty]
        public string userId { get; set; }
        [FirestoreProperty]
        public DateTime createdOnUtc { get; set; }
    }
}
