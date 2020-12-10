namespace ProductShop
{
    using System;
    using System.IO;
    using System.Collections.Generic;


    using ProductShop.Data;
    using ProductShop.Models;
    using ProductShop.Dtos.Import;
    using System.Linq;
    using ProductShop.XMLHelper;
    using ProductShop.Dtos.Export;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using var db = new ProductShopContext();



            var inputXml = File.ReadAllText(@"../../../Datasets/categories-products.xml");

            var result = ImportCategoryProducts(db, inputXml);

            Console.WriteLine(result);
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
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            const string root = "Users";
            var users = XMLConverter.Deserializer<ImportUserDTO>(inputXml, root);

            var usersResult = new List<User>();
            foreach (var user in users)
            {
                var currrentUser = new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Age = user.Age

                };

                usersResult.Add(currrentUser);
            }
            context.Users.AddRange(usersResult);
            context.SaveChanges();

            return $"Successfully imported {usersResult.Count}";
        }

        //Problem 02
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var productsFromXML = XMLConverter.Deserializer<ImportProductDTO>(inputXml, "Products");

            var products = productsFromXML
                           .Select(p => new Product
                           {
                               Name = p.Name,
                               Price = p.Price,
                               SellerId = p.SellerId,
                               BuyerId = p.BuyerId
                           })
                           .ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        //Problem 03
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            const string root = "Categories";

            var categories = XMLConverter.Deserializer<ImportCategoryDTO>(inputXml, root)
                .Where(c => c.Name != null)
                .ToList();

            var categoriesResult = categories
                .Select(c => new Category
                {
                    Name = c.Name
                })
                .ToList();

            context.Categories.AddRange(categoriesResult);
            context.SaveChanges();

            return $"Successfully imported {categoriesResult.Count}";
        }

        //Problem 04
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            const string root = "CategoryProducts";

            var categoryProducts = XMLConverter.Deserializer<ImportCategoryAndProdcutDTO>(inputXml, root);

            var categoryProductsResult = categoryProducts
                   .Where(c=>context.Categories.Any(s=>s.Id == c.CategoryId) && context.Products.Any(s => s.Id == c.ProductId))
                   .Select(c => new CategoryProduct
                   {
                       CategoryId = c.CategoryId,
                       ProductId = c.ProductId
                   })
                   .ToList();

            context.CategoryProducts.AddRange(categoryProductsResult);
            context.SaveChanges();

            return $"Successfully imported {categoryProductsResult.Count}";
        }

        // Problem 05 
        public static string GetProductsInRange(ProductShopContext context)
        {
            const string rootElement = "Products";

            var productsInRange = context
                        .Products
                        .Where(p => p.Price >= 500 && p.Price <= 1000)
                        .Select(p => new ExportProductInfoDto
                        {
                            Name = p.Name,
                            Price = p.Price,
                            Buyer = $"{ p.Buyer.FirstName} {p.Buyer.LastName}"
                        })
                        .OrderBy(x => x.Price)
                        .Take(10)
                        .ToList();

            var xmlOutput = XMLConverter.Serialize(productsInRange, rootElement);

            return xmlOutput;
        }
        // Problem 06 
        public static string GetSoldProducts(ProductShopContext context)
        {
            const string rootElement = "Users";

            var soldProducts = context
                .Users
                .Where(u => u.ProductsSold.Count >= 1)
                .Select(u => new ExportUserProductInfoDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold.Select(p => new UserProductDto
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
               .ToArray()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ToList();

            var xmlOutput = XMLConverter.Serialize(soldProducts, rootElement);

            return xmlOutput;
        }

        // Problem 07 
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            const string rootElement = "Categories";

            var categoriesByProductsCount = context
                .Categories
                .Select(c => new ExportCategoriesByProductsCountDto
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count(),
                    AveragePrice = c.CategoryProducts.Average(p => p.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(p => p.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToList();

            var xmlOutput = XMLConverter.Serialize(categoriesByProductsCount, rootElement);

            return xmlOutput;
        }

        // Problem 08 
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var targetUsers = context.Users
                .ToArray()
                .Where(x => x.ProductsSold.Any())
                .Select(x => new UserInfo
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProducts = new SoldProductCount
                    {
                        Count = x.ProductsSold.Count,
                        Products = x.ProductsSold.Select(p => new SoldProduct
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToList()
                    }
                })
                .OrderByDescending(x => x.SoldProducts.Count)
                .Take(10)
                .ToList();

            var finalObj = new ExportUserCountDto
            {
                Count = context.Users.Count(x => x.ProductsSold.Any()),
                Users = targetUsers
            };

            const string rootName = "Users";
            var resultXml = XMLConverter.Serialize(finalObj, rootName);
            return resultXml;
        }
    }
}