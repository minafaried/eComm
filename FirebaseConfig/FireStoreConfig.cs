using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using System.Reflection;
using System.Threading.Channels;

namespace eComm.FirebaseConfigModel
{
    public class FireStoreConfig
    {
        string projectId;
        public FirestoreDb firestoreDb;
        public FireStoreConfig()
        {
           
            this.projectId = "ecomm-3623d";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", GetPathRelativeToExecutingAssemblyLocation());
            this.firestoreDb=FirestoreDb.Create(projectId);
        }
        public string GetPathRelativeToExecutingAssemblyLocation()
        {
            string pathOfExecutingAssembly = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string settingsPath = pathOfExecutingAssembly + "\\FirebaseConfig\\ecomm-3623d-firebase-adminsdk-6pdon-e7957d9bd7.json";
            return settingsPath;
        }
    }
}
