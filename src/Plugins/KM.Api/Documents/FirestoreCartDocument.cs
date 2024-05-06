
namespace Km.Api.Documents
{
    [FirestoreData]
    public class FirestoreCartDocument : IDocument
    {
        [FirestoreProperty]
        public string id { get; set; }
        [FirestoreProperty]
        public DateTime createdOnUtc { get; set; }
        [FirestoreProperty]
        public string ipAddress { get; set; }
        [FirestoreProperty]
        public IEnumerable<CartItemDocument> items { get; set; }
        [FirestoreProperty]
        public float orderTotal { get; set; }
        [FirestoreProperty]
        public IEnumerable<CartItemDocument> originalSentItems { get; set; }
        [FirestoreProperty]
        public string paymentMethod { get; set; }
        [FirestoreProperty]
        public AddressDocument shippingAddress { get; set; }
        [FirestoreProperty]
        public string status { get; set; }
        [FirestoreProperty]
        public string submitterUserId { get; set; }
        [FirestoreProperty]
        public float totalDiscounts { get; internal set; } 
        [FirestoreProperty]
        public UserProfileDocument user { get; set; }
        [FirestoreProperty]
        public string userId { get; set; }
        [FirestoreProperty]
        public IEnumerable<string> couponCodes { get; set; }
        [FirestoreProperty]
        public float customerEnteredPrice { get; internal set; }
        [FirestoreProperty]
        public string rentalEndDateUtc { get; set; }
        [FirestoreProperty]
        public string rentalStartDateUtc { get; set; }
        [FirestoreProperty]
        public int storeId { get; set; }
    }
}
