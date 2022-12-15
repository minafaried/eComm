
using eComm.FirebaseConfigModel;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eComm.Models.UserModels;
using eComm.Models.UserTypeModels;
using System.Net;
using System.Numerics;

namespace eComm.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        FireStoreConfig firebaseConfig;
        CollectionReference userCollection;

        public  UsersController()
        {
            firebaseConfig = new FireStoreConfig();
            userCollection = firebaseConfig.firestoreDb.Collection("Users");
           
        }
      
        [HttpGet]
        [Route("getUsers")]
        public async Task<IActionResult> getUsers()
        {
            var userTypes = new Dictionary<string, UserType>();
            QuerySnapshot userTypesFireStore = await firebaseConfig.firestoreDb.Collection("UserTypes").GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in userTypesFireStore.Documents)
            {
                Dictionary<string, object> doc = documentSnapshot.ToDictionary();
                UserType userType = new UserType(documentSnapshot.Id, doc["jobTitle"].ToString());
                userTypes.Add(documentSnapshot.Id, userType);
            }

            QuerySnapshot allCitiesQuerySnapshot = await userCollection.GetSnapshotAsync();
            List<User> users = new List<User>();
            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                Dictionary<string, object> data = documentSnapshot.ToDictionary();
                User user= new User(documentSnapshot.Id,
                    data["email"].ToString(),
                    data["userName"].ToString(),
                    data["password"].ToString(),
                    data["phone"].ToString(),
                    data["address"].ToString(),
                    data["urlAddress"].ToString(),
                    data["userTypeId"].ToString());
                user.userType = userTypes[data["userTypeId"].ToString()];
           
                users.Add(user);
            }

            return Ok(users);
        }
        [HttpPost]
        [Route("logIn")]
        public async Task<IActionResult> logIn([FromForm]LogInDataDto logInDataDto)
        {
            Console.WriteLine(logInDataDto.email);
            var userTypes = new Dictionary<string, UserType>();
            QuerySnapshot userTypesFireStore = await firebaseConfig.firestoreDb.Collection("UserTypes").GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in userTypesFireStore.Documents)
            {
                Dictionary<string, object> doc = documentSnapshot.ToDictionary();
                UserType userType = new UserType(documentSnapshot.Id, doc["jobTitle"].ToString());
                userTypes.Add(documentSnapshot.Id, userType);
            }

            QuerySnapshot allCitiesQuerySnapshot = await userCollection.
                WhereEqualTo("email", logInDataDto.email).
                WhereEqualTo("password", logInDataDto.password).
                GetSnapshotAsync();
           
            if(allCitiesQuerySnapshot.Count == 0)
            {
                return Ok();
            }
            Dictionary<string, object> data = allCitiesQuerySnapshot.Documents[0].ToDictionary();
            User user = new User(allCitiesQuerySnapshot.Documents[0].Id,
                data["email"].ToString(),
                data["userName"].ToString(),
                data["password"].ToString(),
                data["phone"].ToString(),
                data["address"].ToString(),
                data["urlAddress"].ToString(),
                data["userTypeId"].ToString());
            user.userType = userTypes[data["userTypeId"].ToString()];
            return Ok(user);
            

            
        }

        [HttpPost]
       [Route("addUser")]
       public async Task<IActionResult> addUsers(UserDto userDto)
       {
            var userTypes = new Dictionary<string, UserType>();
            QuerySnapshot userTypesFireStore = await firebaseConfig.firestoreDb.Collection("UserTypes").GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in userTypesFireStore.Documents)
            {
                Dictionary<string, object> doc = documentSnapshot.ToDictionary();
                UserType userType = new UserType(documentSnapshot.Id, doc["jobTitle"].ToString());
                userTypes.Add(documentSnapshot.Id, userType);
            }

            DocumentReference DocRef = await userCollection.AddAsync(userDto);
            User user = new User(DocRef.Id,
                    userDto.email,
                    userDto.userName,
                    userDto.password,
                    userDto.phone,
                    userDto.address,
                    userDto.urlAddress,
                    userDto.userTypeId);
            user.userType = userTypes[userDto.userTypeId];

            return Ok(user);
       }
       [HttpPut]
       [Route("editUser")]
       public async Task<IActionResult> editUser(User req)
       {
            var userTypes = new Dictionary<string, UserType>();
            QuerySnapshot userTypesFireStore = await firebaseConfig.firestoreDb.Collection("UserTypes").GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in userTypesFireStore.Documents)
            {
                Dictionary<string, object> doc = documentSnapshot.ToDictionary();
                UserType userType = new UserType(documentSnapshot.Id, doc["jobTitle"].ToString());
                userTypes.Add(documentSnapshot.Id, userType);
            }

            DocumentReference DocRef = userCollection.Document(req.id);
            UserDto userDto = new UserDto();
            userDto.email = req.email;
            userDto.userName = req.userName;
            userDto.password = req.password;
            userDto.phone=req.phone;
            userDto.address = req.address;
            userDto.urlAddress = req.urlAddress;
            userDto.userTypeId = req.userTypeId;
            await DocRef.SetAsync(userDto);
            return Ok(req);
       }
       [HttpDelete]
       [Route("deleteUser")]
       public async Task<IActionResult> deleteUser(string id)
       {
            DocumentReference DocRef = userCollection.Document(id);
            await DocRef.DeleteAsync();
            return Ok();
       }
    }
}
