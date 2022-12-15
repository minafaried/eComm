using eComm.Models.UserTypeModels;
using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations.Schema;

namespace eComm.Models.UserModels
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public string id { get; set; }
        [FirestoreProperty]
        public string email { get; set; }
        [FirestoreProperty]
        public string userName { get; set; }
        [FirestoreProperty]
        public string password { get; set; }
        [FirestoreProperty]
        public string phone { get; set; }
        [FirestoreProperty]
        public string address { get; set; }
        [FirestoreProperty]
        public string urlAddress { get; set; }
        [FirestoreProperty]
        public string userTypeId { get; set; }
        [FirestoreProperty]
        public UserType userType { get; set; }
        public User(string id, string email, string userName, string password, string phone, string address, string urlAddress, string userTypeId)
        {
            this.id = id;
            this.email = email;
            this.userName = userName;
            this.password = password;
            this.phone = phone;
            this.address = address;
            this.urlAddress = urlAddress;
            this.userTypeId = userTypeId;
        }
    }
}
