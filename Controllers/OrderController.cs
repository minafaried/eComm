
using eComm.FirebaseConfigModel;
using eComm.Models.CategoryModels;
using eComm.Models.OrderModels;
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
    public class OrderController : ControllerBase
    {
        
        FireStoreConfig firebaseConfig;
        CollectionReference orderCollection;


        public OrderController()
        {
            firebaseConfig = new FireStoreConfig();
            orderCollection = firebaseConfig.firestoreDb.Collection("Orders");
        }
        [HttpGet]
        [Route("getOdersByUserId")]
        public async Task<IActionResult> getOdersByUserId(string id)
        {
            DocumentSnapshot userDoc = await firebaseConfig.firestoreDb.Collection("Users").Document(id).GetSnapshotAsync();

            Dictionary<string, object> userData = userDoc.ToDictionary();
            User user = new User(userDoc.Id,
                userData["email"].ToString(),
                userData["userName"].ToString(),
                userData["password"].ToString(),
                userData["phone"].ToString(),
                userData["address"].ToString(),
                userData["urlAddress"].ToString(),
                userData["userTypeId"].ToString());
            DocumentSnapshot userTypeDoc = await firebaseConfig.firestoreDb.Collection("UserTypes").Document(userData["userTypeId"].ToString()).GetSnapshotAsync();
            Dictionary<string, object> userTypeData = userTypeDoc.ToDictionary();
            user.userType = new UserType(userDoc.Id, userTypeData["jobTitle"].ToString());

            QuerySnapshot allCitiesQuerySnapshot = await firebaseConfig.firestoreDb.Collection("Products").GetSnapshotAsync();
            Dictionary<string,Product> allProducts = new Dictionary<string, Product>();
            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                Dictionary<string, object> data = documentSnapshot.ToDictionary();

                List<string> categoriesIds = new List<string>();
                if (data.ContainsKey("categoriesIds"))
                    foreach (var el in (List<object>)data["categoriesIds"])
                    {
                        categoriesIds.Add(el.ToString());
                    }
                Product product = new Product(documentSnapshot.Id,
                                              data["ProductName"].ToString(),
                                              data["productImageUrl"].ToString(),
                                              float.Parse(data["price"].ToString()),
                                              Int32.Parse(data["quantity"].ToString()),
                                             categoriesIds,
                                             new List<Category>());
                allProducts.Add(documentSnapshot.Id, product);
            }

            allCitiesQuerySnapshot = await orderCollection.WhereEqualTo("userId",id).GetSnapshotAsync();
            List<Order> orders = new List<Order>();
            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                Dictionary<string, object> data = documentSnapshot.ToDictionary();

                Dictionary<string,productQuantity> products = new Dictionary<string, productQuantity>();
                Dictionary<string,int> productIds = new Dictionary<string, int>();
                if (data.ContainsKey("productsIds"))
                    foreach (var el in (Dictionary<string,object>)data["productsIds"])
                    {
                        productQuantity productQuantity = new productQuantity { Product = allProducts[el.Key], quantity = Convert.ToInt32(el.Value) };
                        products.Add(el.Key,productQuantity);
                        productIds.Add(el.Key, Convert.ToInt32(el.Value));
                    }
                Order order = new Order(documentSnapshot.Id,
                                        data["orderName"].ToString(),
                                        float.Parse(data["totalprice"].ToString()),
                                        float.Parse(data["deliveringPrice"].ToString()),
                                        data["address"].ToString(),
                                        data["UrlAddress"].ToString(),
                                        user.id,
                                        user,
                                        productIds,
                                        products);
                orders.Add(order);
            }

            return Ok(orders);
        }
        
        [HttpPost]
        [Route("addOrder")]
        public async Task<IActionResult> addOrder([FromBody] OrderDto orderDto)
        {
            DocumentReference DocRef = await orderCollection.AddAsync(orderDto);
            DocumentSnapshot userDoc = await firebaseConfig.firestoreDb.Collection("Users").Document(orderDto.userId).GetSnapshotAsync();

            Dictionary<string, object> userData = userDoc.ToDictionary();
            User user = new User(userDoc.Id,
                userData["email"].ToString(),
                userData["userName"].ToString(),
                userData["password"].ToString(),
                userData["phone"].ToString(),
                userData["address"].ToString(),
                userData["urlAddress"].ToString(),
                userData["userTypeId"].ToString());
            DocumentSnapshot userTypeDoc = await firebaseConfig.firestoreDb.Collection("UserTypes").Document(userData["userTypeId"].ToString()).GetSnapshotAsync();
            Dictionary<string, object> userTypeData = userTypeDoc.ToDictionary();
            user.userType = new UserType(userDoc.Id, userTypeData["jobTitle"].ToString());


            QuerySnapshot allCitiesQuerySnapshot = await firebaseConfig.firestoreDb.Collection("Products").GetSnapshotAsync();
            Dictionary<string, productQuantity> products = new Dictionary<string, productQuantity>();
            Dictionary<string, int> productIds = new Dictionary<string, int>();
            var orderDtoProductsIds = orderDto.productsIds;
            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                if (!orderDto.productsIds.ContainsKey(documentSnapshot.Id))
                {
                    continue;
                }
                Dictionary<string, object> data = documentSnapshot.ToDictionary();

                List<string> categoriesIds = new List<string>();
                if (data.ContainsKey("categoriesIds"))
                    foreach (var el in (List<object>)data["categoriesIds"])
                    {
                        categoriesIds.Add(el.ToString());
                    }
                Product product = new Product(documentSnapshot.Id,
                                              data["ProductName"].ToString(),
                                              data["productImageUrl"].ToString(),
                                              float.Parse(data["price"].ToString()),
                                              Int32.Parse(data["quantity"].ToString()),
                                             categoriesIds,
                                             new List<Category>());
                //Console.WriteLine(documentSnapshot.Id);
                
                productQuantity productQuantity = new productQuantity { Product = product, quantity = orderDtoProductsIds[documentSnapshot.Id] };
                products.Add(documentSnapshot.Id, productQuantity);
                productIds.Add(documentSnapshot.Id, orderDtoProductsIds[documentSnapshot.Id]);
            }
            Order order = new Order(DocRef.Id,
                                        orderDto.orderName,
                                        orderDto.totalprice,
                                        orderDto.deliveringPrice,
                                        orderDto.address,
                                        orderDto.UrlAddress,
                                        user.id,
                                        user,
                                        productIds,
                                        products);

            return Ok(order);
        }
        [HttpPut]
        [Route("editOrder")]
        public async Task<IActionResult> editOrder(string id,[FromBody] OrderDto orderDto)
        {
            await orderCollection.Document(id).SetAsync(orderDto);
            DocumentSnapshot userDoc = await firebaseConfig.firestoreDb.Collection("Users").Document(orderDto.userId).GetSnapshotAsync();

            Dictionary<string, object> userData = userDoc.ToDictionary();
            User user = new User(userDoc.Id,
                userData["email"].ToString(),
                userData["userName"].ToString(),
                userData["password"].ToString(),
                userData["phone"].ToString(),
                userData["address"].ToString(),
                userData["urlAddress"].ToString(),
                userData["userTypeId"].ToString());
            DocumentSnapshot userTypeDoc = await firebaseConfig.firestoreDb.Collection("UserTypes").Document(userData["userTypeId"].ToString()).GetSnapshotAsync();
            Dictionary<string, object> userTypeData = userTypeDoc.ToDictionary();
            user.userType = new UserType(userDoc.Id, userTypeData["jobTitle"].ToString());


            QuerySnapshot allCitiesQuerySnapshot = await firebaseConfig.firestoreDb.Collection("Products").GetSnapshotAsync();
            Dictionary<string, productQuantity> products = new Dictionary<string, productQuantity>();
            Dictionary<string, int> productIds = new Dictionary<string, int>();
            Dictionary<string, int> orderDtoProductsIds = orderDto.productsIds;

            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                if (!orderDtoProductsIds.ContainsKey(documentSnapshot.Id))
                {
                    continue;
                }
                Dictionary<string, object> data = documentSnapshot.ToDictionary();

                List<string> categoriesIds = new List<string>();
                if (data.ContainsKey("categoriesIds"))
                    foreach (var el in (List<object>)data["categoriesIds"])
                    {
                        categoriesIds.Add(el.ToString());
                    }
                Product product = new Product(documentSnapshot.Id,
                                              data["ProductName"].ToString(),
                                              data["productImageUrl"].ToString(),
                                              float.Parse(data["price"].ToString()),
                                              Int32.Parse(data["quantity"].ToString()),
                                             categoriesIds,
                                             new List<Category>());
                
                productQuantity productQuantity = new productQuantity { Product = product, quantity = orderDtoProductsIds[documentSnapshot.Id] };
                products.Add(documentSnapshot.Id, productQuantity);
                productIds.Add(documentSnapshot.Id, orderDtoProductsIds[documentSnapshot.Id]);
            }
            Order order = new Order(id,
                                        orderDto.orderName,
                                        orderDto.totalprice,
                                        orderDto.deliveringPrice,
                                        orderDto.address,
                                        orderDto.UrlAddress,
                                        user.id,
                                        user,
                                        productIds,
                                        products);

            return Ok(order);
        }
        [HttpDelete]
        [Route("deleteOrder")]
        public async Task<IActionResult> deleteOrder(string id)
        {
            await orderCollection.Document(id).DeleteAsync();
            return Ok();
        }

    }
}
