using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace Nop.Plugin.Misc.KM.Catalog.Documents
{
    [FirestoreData]
    public record StoreInfo : IDocument
    {
        [FirestoreProperty]
        public string id { get; init; }
        [FirestoreProperty]
        public string name { get; init; }
        [FirestoreProperty]
        public IEnumerable<VendorInfo> vendors { get; init; }
    }
}
