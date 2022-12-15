using Google.Cloud.Firestore;

namespace eComm.Models.UserTypeModels
{
    [FirestoreData]
    public class UserTypeDto
    {
        [FirestoreProperty]
        public string jobTitle { get; set; }
        
    }
}
