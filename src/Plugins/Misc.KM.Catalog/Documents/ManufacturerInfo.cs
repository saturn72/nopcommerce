using Google.Cloud.Firestore;

namespace Nop.Plugin.Misc.KM.Catalog.Documents
{
    [FirestoreData]
    public record ManufacturerInfo
    {
        [FirestoreProperty]
        public string name { get; init; }
        [FirestoreProperty]
        public CatalogMediaInfo picture { get; init; }
    }
}
