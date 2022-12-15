using eComm.Models.CategoryModels;
using Google.Cloud.Firestore;

namespace eComm.Models.ProductModels
{
    [FirestoreData]
    public class EditProductDto
    {
        [FirestoreProperty]
        public string id { get; set; }
        [FirestoreProperty]
        public string ProductName { get; set; }
        [FirestoreProperty]
        public string productImageUrl { get; set; }
        [FirestoreProperty]
        public IFormFile imageFile { get; set; }
        [FirestoreProperty]
        public float price { get; set; }
        [FirestoreProperty]
        public int quantity { get; set; }
        [FirestoreProperty]
        public List<string> categoriesIds { get; set; }
        [FirestoreProperty]
        public List<Category> categories { get; set; }
    }
}
