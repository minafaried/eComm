
using Google.Cloud.Firestore;

namespace eComm.Models.CategoryModels
{
    [FirestoreData]
    public class CategotyDto
    {
        [FirestoreProperty]
        public string categoryName { get; set; }
        [FirestoreProperty]
        public List<string> productIds { get; set; }


    }
}
