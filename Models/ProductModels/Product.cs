using eComm.Models.CategoryModels;
using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations.Schema;

namespace eComm.Models.ProductModels
{
    [FirestoreData]
    public class Product
    {
        [FirestoreProperty]
        public string id { get; set; }
        [FirestoreProperty]
        public string ProductName { get; set; }
        [FirestoreProperty]
        public string productImageUrl { get; set; }
        [FirestoreProperty]
        public float price { get; set; }
        [FirestoreProperty]
        public int quantity { get; set; }
        [FirestoreProperty]
        public List<string> categoriesIds { get; set; }
        [FirestoreProperty]
        public List<Category> categories { get; set; }
        public Product(string id, string productName, string productImageUrl, float price, int quantity, List<string> categoriesIds, List<Category> categories)
        {
            this.id = id;
            ProductName = productName;
            this.productImageUrl = productImageUrl;
            this.price = price;
            this.quantity = quantity;
            this.categoriesIds = categoriesIds;
            this.categories = categories;
        }
    }
}
