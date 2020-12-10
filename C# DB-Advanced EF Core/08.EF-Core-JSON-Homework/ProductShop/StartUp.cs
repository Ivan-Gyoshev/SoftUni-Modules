using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var db = new ProductShopContext();


            string inputJason = File.ReadAllText("../../../Datasets/users.json");
           

            string json = GetUsersWithProducts(db);

            File.WriteAllText("../../../JsonExports/users-and-products.json", json);

        }

        private static void ResetDb(ProductShopContext context)
        {
            context.Database.EnsureDeleted();
            Console.WriteLine("Database was successfully deleted!");
            context.Database.EnsureCreated();
            Console.WriteLine("Database was successfully created!");
        }

        // >>Importing<< //

        //Problem 01
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {

            var users = JsonConvert.DeserializeObject<User[]>(inputJson);

            context.Users.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Length}";
        }
        //Problem 02
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }
        //Problem 03
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<Category[]>(inputJson)
            .Where(c => c.Name != null)
            .ToArray();


            context.Categories.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Length}";
        }
        //Problem 04
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();
            return $"Successfully imported {categoryProducts.Length}";
        }
        // >>Exporting<< //

        //Problem 05 
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .ToList();

            string json = JsonConvert.SerializeObject(products, Formatting.Indented);

            return json;
        }
        //Problem 06
        public static string GetSoldProducts(ProductShopContext context)
        {
            var products = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.LastName)
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold
                    .Where(p => p.Buyer != null)
                    .Select(p => new
                    {
                        name = p.Name,
                        price = p.Price,
                        buyerFirstName = p.Buyer.FirstName,
                        buyerLastName = p.Buyer.LastName
                    })
                      .ToList()
                })
                .ToList();

            string json = JsonConvert.SerializeObject(products, Formatting.Indented);

            return json;
        }

        //Problem 07
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoryProducts.Count(),
                    averagePrice = c.CategoryProducts.Average(cp => cp.Product.Price).ToString("f2"),
                    totalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price).ToString("f2")
                })
                .OrderByDescending(c => c.productsCount)
                .ToList();

            string json = JsonConvert.SerializeObject(categories);

            return json;
        }
        //Problem 08 
        public static string GetUsersWithProducts(ProductShopContext context)
        {

            var curentUsers = context.Users
                             .AsEnumerable()
                             .Where(p => p.ProductsSold.Any(b => b.Buyer != null))
                             .OrderByDescending(p => p.ProductsSold.Count(c => c.Buyer != null))
                             .Select(c => new
                             {
                                 firstName = c.FirstName,
                                 lastName = c.LastName,
                                 age = c.Age,
                                 soldProducts = new
                                 {
                                     count = c.ProductsSold.Count(b => b.Buyer != null),
                                     products = c.ProductsSold
                                                 .Where(x => x.Buyer != null)
                                                 .Select(y => new
                                                 {
                                                     name = y.Name,
                                                     price = y.Price
                                                 })
                                                 .ToList()
                                 }
                             })
                             .ToList();

            var usersWithCoiunt = new
            {
                usersCount = curentUsers.Count,
                users = curentUsers
            };

            var setingJSON = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            var usersToJSON = JsonConvert.SerializeObject(usersWithCoiunt, setingJSON);

            return usersToJSON;

        }
    }
}