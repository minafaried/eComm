
using eComm.FirebaseConfigModel;
using eComm.Models.CategoryModels;
using eComm.Models.ProductModels;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Firebase.Storage;
using Firebase.Auth;
using System.IO;

namespace eComm.CategoriesController
{
    [Route("api/Categories")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        FireStoreConfig firebaseConfig;
        CollectionReference productCollection;


        public ProductController()
        {
            firebaseConfig = new FireStoreConfig();
            productCollection = firebaseConfig.firestoreDb.Collection("Products");
        }
        
        
        [HttpGet]
        [Route("getProducts")]
        public async Task<IActionResult> getProducts()
        {
            QuerySnapshot allCitiesQuerySnapshot = await firebaseConfig.firestoreDb.Collection("Categories").GetSnapshotAsync();
            Dictionary<string, Category> categories = new Dictionary<string, Category>();
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
                Category category = new Category(documentSnapshot.Id, data["categoryName"].ToString(), productIds);
                categories.Add(documentSnapshot.Id, category);
            }

            allCitiesQuerySnapshot = await productCollection.GetSnapshotAsync();
            List<Product> products = new List<Product>();
            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                Dictionary<string, object> data = documentSnapshot.ToDictionary();

                List<string> categoriesIds = new List<string>();
                List<Category> productCategories = new List<Category>();
                if(data.ContainsKey("categoriesIds"))
                    foreach (var el in (List<object>)data["categoriesIds"])
                    {
                        categoriesIds.Add(el.ToString());
                        productCategories.Add(categories[el.ToString()]);
                    }
                Product product = new Product(documentSnapshot.Id,
                                              data["ProductName"].ToString(),
                                              data["productImageUrl"].ToString(),
                                              float.Parse(data["price"].ToString()),
                                              Int32.Parse(data["quantity"].ToString()),
                                             categoriesIds,
                                             productCategories);
                products.Add(product);
            }

            return Ok(products);
        }

         [HttpPost]
         [Route("addProduct")]
         public async Task<IActionResult> addProduct([FromForm]ProductyDto productyDto)
         {
            productForFireStrore productForFireStrore = new productForFireStrore {
                ProductName= productyDto.ProductName,
                price= productyDto.price,
                quantity= productyDto.quantity,
                productImageUrl="",
                categoriesIds= productyDto.categoriesIds};
            DocumentReference DocRef = await productCollection.AddAsync(productForFireStrore);

            var task = new FirebaseStorage(
                "ecomm-3623d.appspot.com"
                )
                .Child("productsImages")
                .Child(DocRef.Id+".png")
                .PutAsync(productyDto.imageFile.OpenReadStream());
            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            // await the task to wait until upload completes and get the download url
            var downloadUrl = await task;
            productForFireStrore.productImageUrl = downloadUrl;
            await DocRef.SetAsync(productForFireStrore);

            QuerySnapshot allCitiesQuerySnapshot = await firebaseConfig.firestoreDb.Collection("Categories").GetSnapshotAsync();
            List< Category> categories = new List<Category>();

            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                if (!productyDto.categoriesIds.Exists(el => el == documentSnapshot.Id))
                {
                    continue;
                }
                Dictionary<string, object> data = documentSnapshot.ToDictionary();

                List<string> productIds = new List<string>();
                if (data.ContainsKey("productIds"))
                {
                    foreach (var el in (List<object>)data["productIds"])
                    {
                        productIds.Add(el.ToString());
                    }
                }
                productIds.Add(DocRef.Id);
                CategotyDto tempCategory = new CategotyDto { categoryName= data["categoryName"].ToString(),productIds= productIds};
                await firebaseConfig.firestoreDb.Collection("Categories").Document(documentSnapshot.Id).SetAsync(tempCategory);
                
                Category category = new Category(documentSnapshot.Id, data["categoryName"].ToString(), productIds);
                categories.Add(category);
            }

            Product product = new Product(
                DocRef.Id,
                productyDto.ProductName,
                downloadUrl,
                productyDto.price,
                productyDto.quantity,
                productyDto.categoriesIds,
                categories);
            return Ok(product);
         }
         
         [HttpPut]
         [Route("editProduct")]
         public async Task<IActionResult> editProduct([FromForm]EditProductDto editProductDto)
         {

            productForFireStrore productForFireStrore = new productForFireStrore
            {
                ProductName = editProductDto.ProductName,
                price = editProductDto.price,
                quantity = editProductDto.quantity,
                productImageUrl = editProductDto.productImageUrl,
                categoriesIds = editProductDto.categoriesIds
            };
            await productCollection.Document(editProductDto.id).SetAsync(productForFireStrore);

            var task = new FirebaseStorage(
                "ecomm-3623d.appspot.com"
                )
                .Child("productsImages")
                .Child(editProductDto.id + ".png")
                .PutAsync(editProductDto.imageFile.OpenReadStream());
            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            // await the task to wait until upload completes and get the download url
            var downloadUrl = await task;
            productForFireStrore.productImageUrl = downloadUrl;
            await productCollection.Document(editProductDto.id).SetAsync(productForFireStrore);

            QuerySnapshot allCitiesQuerySnapshot = await firebaseConfig.firestoreDb.Collection("Categories").GetSnapshotAsync();
            List<Category> categories = new List<Category>();
            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                if (!editProductDto.categoriesIds.Exists(el => el == documentSnapshot.Id))
                {
                    continue;
                }
                Dictionary<string, object> data = documentSnapshot.ToDictionary();

                List<string> productIds = new List<string>();

                if (data.ContainsKey("productIds"))
                {
                    foreach (var el in (List<object>)data["productIds"])
                    {
                        productIds.Add(el.ToString());
                    }
                }
                Category category = new Category(documentSnapshot.Id, data["categoryName"].ToString(), productIds);
                categories.Add( category);
            }
            Product product = new Product(
                editProductDto.id,
                editProductDto.ProductName,
                editProductDto.productImageUrl,
                editProductDto.price,
                editProductDto.quantity,
                editProductDto.categoriesIds,
                categories);
            return Ok(product);
        }
         [HttpDelete]
         [Route("deleteProduct")]
         public async Task<IActionResult> deleteProduct(string id)
         {
            DocumentSnapshot productDoc = await productCollection.Document(id).GetSnapshotAsync();
            Dictionary<string, object> data = productDoc.ToDictionary();

            if (data.ContainsKey("categoriesIds"))
            {
                foreach (var el in (List<object>)data["categoriesIds"])
                {
                    DocumentReference DocRef = firebaseConfig.firestoreDb.Collection("Categories").Document(el.ToString());
                    DocumentSnapshot categoryDoc = await DocRef.GetSnapshotAsync();
                    Dictionary<string, object> categoryData = categoryDoc.ToDictionary();
                    CategotyDto categotyDto = new CategotyDto();
                    categotyDto.categoryName = categoryData["categoryName"].ToString();
                    List<string> productIds = new List<string>();
                    if (categoryData.ContainsKey("productIds"))
                    {
                        foreach (var p in (List<object>)data["productIds"])
                        {
                            if(el.ToString()!=id)
                                productIds.Add(el.ToString());
                        }
                    }
                    categotyDto.productIds = productIds;
                    await DocRef.SetAsync(categotyDto);

                }
            }
            await productCollection.Document(id).DeleteAsync();
            return Ok();

        }

    }
}
