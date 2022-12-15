using Google.Cloud.Firestore;

namespace eComm.Models.ProductModels
{
    [FirestoreData]
    public class productForFireStrore
    {

        [FirestoreProperty]
        public string ProductName { get; set; }
        [FirestoreProperty]
        public float price { get; set; }
        [FirestoreProperty]
        public int quantity { get; set; }
        [FirestoreProperty]
        public string productImageUrl { get; set; }
        [FirestoreProperty]
        public List<string> categoriesIds { get; set; }
    }
}
