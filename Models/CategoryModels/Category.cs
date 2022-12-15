
using eComm.Models.ProductModels;
using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations.Schema;

namespace eComm.Models.CategoryModels
{
    [FirestoreData]
    public class Category
    {
        [FirestoreProperty]
        public string id { get; set; }
        [FirestoreProperty]
        public string categoryName { get; set; }
        [FirestoreProperty]
        public List<string> productIds { get; set; }
        public Category(string id, string categoryName, List<string> productIds)
        {
            this.id = id;
            this.categoryName = categoryName;
            this.productIds = productIds;
        }
    }
}
