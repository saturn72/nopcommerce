using Google.Cloud.Firestore;

namespace Nop.Plugin.Misc.KM.Orders.Documents
{
    [FirestoreData]
    public class CartItemDocument
    {
        [FirestoreProperty]
        public string addedOnUtc { get; set; }
        [FirestoreProperty]
        public int orderedQuantity { get; set; }
        [FirestoreProperty]
        public ProductDocument product { get; set; }
    }
}
