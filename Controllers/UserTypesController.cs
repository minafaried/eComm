using eComm.Models.UserTypeModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eComm.FirebaseConfigModel;
using Google.Cloud.Firestore;

namespace eComm.Controller
{
    [Route("api/UserTypes")]
    [ApiController]
    public class UserTypesController : ControllerBase
    {
        FireStoreConfig firebaseConfig;
        CollectionReference userTypeCollection;

        public UserTypesController()
        {
            firebaseConfig = new FireStoreConfig();
            userTypeCollection = firebaseConfig.firestoreDb.Collection("UserTypes");
        }
        [HttpGet]
        [Route("getUserTypes")]
        public async Task<IActionResult> GetUserTypes()
        {

            QuerySnapshot allCitiesQuerySnapshot = await userTypeCollection.GetSnapshotAsync();
            List<UserType> userTypes = new List<UserType>();
            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                Dictionary<string, object> data = documentSnapshot.ToDictionary();
                UserType userType = new UserType(documentSnapshot.Id, data["jobTitle"].ToString());
                userTypes.Add(userType);
            }
              
            return Ok(userTypes);
        }
        [HttpPost]
        [Route("addUserType")]
        public async Task<IActionResult> addUserType(UserTypeDto userTypeDto)
        {
            DocumentReference DocRef =await userTypeCollection.AddAsync(userTypeDto);
            UserType userType = new UserType(DocRef.Id, userTypeDto.jobTitle);
            return Ok(userType);
        }
        [HttpPut]
        [Route("editUserType")]
        public async Task<IActionResult> editUserType(UserType req)
        {
            DocumentReference DocRef = userTypeCollection.Document(req.id);
            UserTypeDto userTypeDto = new UserTypeDto();
            userTypeDto.jobTitle=req.jobTitle;
            await DocRef.SetAsync(userTypeDto);
            UserType userType = new UserType(DocRef.Id, req.jobTitle);
            return Ok(userType);
        }
        [HttpDelete]
        [Route("deleteUserType")]
        public async Task<IActionResult> deleteUserType(string id)
        {
            DocumentReference DocRef = userTypeCollection.Document(id);
            await DocRef.DeleteAsync();
            return Ok();
        }
    }
}
