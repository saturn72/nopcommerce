using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace Nop.Plugin.Misc.KM.Catalog.Documents
{
    [FirestoreData]
    public record VendorInfo : IDocument
    {
        [FirestoreProperty]
        public string id { get; init; }
        [FirestoreProperty]
        public string name { get; init; }
        [FirestoreProperty]
        public VendorStoreInfo store { get; init; }
        [FirestoreProperty]
        public IEnumerable<ProductInfoDocument> products { get; init; }
        [FirestoreProperty]
        public CatalogMediaInfo logo { get; init; }
    }
}
