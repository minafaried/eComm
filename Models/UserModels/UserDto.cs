using eComm.Models.UserTypeModels;
using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations.Schema;

namespace eComm.Models.UserModels
{
    [FirestoreData]
    public class UserDto
    {
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
    }
}
