
using eComm.FirebaseConfigModel;
using eComm.Models.CategoryModels;
using eComm.Models.ProductModels;
using eComm.Models.UserModels;
using eComm.Models.UserTypeModels;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eComm.CategoriesController
{
    [Route("api/Categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        FireStoreConfig firebaseConfig;
        CollectionReference categoryCollection;
       

        public CategoriesController()
        {
            firebaseConfig = new FireStoreConfig();
            categoryCollection = firebaseConfig.firestoreDb.Collection("Categories");
        }
        [HttpGet]
        [Route("getCategories")]
        public async Task<IActionResult> GetCategories()
        {
            QuerySnapshot allCitiesQuerySnapshot = await categoryCollection.GetSnapshotAsync();
            List<Category> categories = new List<Category>();
            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                Dictionary<string, object> data = documentSnapshot.ToDictionary();

                List<string> productIds = new List<string>();
                if (data.ContainsKey("productIds"))
                {
                    foreach (var el in (List<object>)data["productIds"])
                    {
                        productIds.Add(el.ToString());
                    }
                }
                Category category = new Category(documentSnapshot.Id, data["categoryName"].ToString(),productIds);
                categories.Add(category);
            }

            return Ok(categories);
        }
        
        [HttpPost]
        [Route("addCategory")]
        public async Task<IActionResult> addCategory(CategotyDto categotyDto)
        {
            DocumentReference DocRef = await categoryCollection.AddAsync(categotyDto);
            var category = new Category ( DocRef.Id,categotyDto.categoryName, categotyDto.productIds);
            return Ok(category);
        }
       
        [HttpPut]
        [Route("editCategoty")]
        public async Task<IActionResult> editCategoty(Category req)
        {
            DocumentReference DocRef = categoryCollection.Document(req.id);
            CategotyDto categotyDto = new CategotyDto();
            categotyDto.categoryName = req.categoryName;
            categotyDto.productIds = req.productIds;

            await DocRef.SetAsync(categotyDto);
            return Ok(req);
        }
        
        [HttpDelete]
        [Route("deleteCategory")]
        public async Task<IActionResult> deleteCategory(string id)
        {
            DocumentReference DocRef = categoryCollection.Document(id);
            await DocRef.DeleteAsync();
            return Ok();
        }
        
        [HttpGet]
        [Route("getPoductsByCategoryId")]
        public async Task<IActionResult> getPoductsByCategoryId(string id)
        {
            DocumentReference DocRef = categoryCollection.Document(id);
            DocumentSnapshot categoryFirestore= await DocRef.GetSnapshotAsync();
            Dictionary<string, object> data = categoryFirestore.ToDictionary();
            List<string> productIds = new List<string>();
            List<Product> products = new List<Product>();
            foreach (var el in (List<object>)data["productIds"])
            {
                productIds.Add(el.ToString());
            }
            QuerySnapshot allCitiesQuerySnapshot = await firebaseConfig.firestoreDb.Collection("Products").GetSnapshotAsync();


            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                if (!productIds.Exists(el => el == documentSnapshot.Id))
                {
                    continue;
                }
                data = documentSnapshot.ToDictionary();

                List<string> categoriesIds = new List<string>();
                if (data.ContainsKey("categoriesIds"))
                {
                    foreach (var el in (List<object>)data["categoriesIds"])
                    {
                        categoriesIds.Add(el.ToString());
                    }
                }
                Product product = new Product(documentSnapshot.Id,
                                              data["ProductName"].ToString(),
                                              data["productImageUrl"].ToString(),
                                              float.Parse(data["price"].ToString()),
                                              Int32.Parse(data["quantity"].ToString()),
                                             categoriesIds,
                                             new List<Category>());
                products.Add(product);
            }
            return Ok(products);
        }
    }
}
