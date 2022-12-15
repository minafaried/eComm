
using eComm.Models.ProductModels;
using eComm.Models.UserModels;
using eComm.Models.UserTypeModels;
using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations.Schema;

namespace eComm.Models.OrderModels
{
    [FirestoreData]
    public class Order
    {
        [FirestoreProperty]
        public string id { get; set; }
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
        public User user { get; set; }
        [FirestoreProperty]
        public Dictionary<string,int> productsIds { get; set; }
        [FirestoreProperty]
        public Dictionary< string,productQuantity> products { get; set; }
        public Order(string id, string orderName, float totalprice, float deliveringPrice, string address, string urlAddress, string userId, User user, Dictionary<string,int> productsIds, Dictionary<string, productQuantity> products)
        {
            this.id = id;
            this.orderName = orderName;
            this.totalprice = totalprice;
            this.deliveringPrice = deliveringPrice;
            this.address = address;
            this.UrlAddress = urlAddress;
            this.userId = userId;
            this.user = user;
            this.productsIds = productsIds;
            this.products = products;
        }
    }
}
