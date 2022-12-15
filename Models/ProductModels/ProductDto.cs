
using Google.Cloud.Firestore;

namespace eComm.Models.ProductModels
{
    public class ProductyDto
    {
        [FirestoreProperty]
        public string ProductName { get; set; }
        [FirestoreProperty]
        public float price { get; set; }
        [FirestoreProperty]
        public int quantity { get; set; }
        [FirestoreProperty]
        public IFormFile imageFile { get; set; }
        [FirestoreProperty]
        public List<string> categoriesIds { get; set; }

    }
}
