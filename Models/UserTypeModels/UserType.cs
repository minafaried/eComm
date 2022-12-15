using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations.Schema;

namespace eComm.Models.UserTypeModels
{
    [FirestoreData]
    public class UserType
    {

        [FirestoreProperty]
        public string id { get; set; }
        [FirestoreProperty]
        public string jobTitle { get; set; }
        public UserType(string id="", string jobTitle="")
        {
            this.id = id;
            this.jobTitle = jobTitle;
        }
    }
}
