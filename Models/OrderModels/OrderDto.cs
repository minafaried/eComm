
using eComm.Models.ProductModels;
using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations.Schema;

namespace eComm.Models.OrderModels
{
    [FirestoreData]
    public class OrderDto
    {
        [FirestoreProperty]
        public string orderName { get; set; }
        [FirestoreProperty]
        public float totalprice { get; set; }
        [FirestoreProperty]
        public float deliveringPrice { get; set; }
        [FirestoreProperty]
        public string address { get; set; }
        [FirestoreProperty]
        public string UrlAddress { get; set; }
        [FirestoreProperty]
        public string userId { get; set; }
        [FirestoreProperty]
        public Dictionary<string,int> productsIds { get; set; }

    }
}
