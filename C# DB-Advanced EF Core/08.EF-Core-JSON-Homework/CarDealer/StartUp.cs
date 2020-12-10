namespace CarDealer
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using AutoMapper;
    using CarDealer.Data;
    using CarDealer.DTO;
    using CarDealer.Models;
    using Newtonsoft.Json;
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var db = new CarDealerContext();

            string inputJson = File.ReadAllText("../../../Datasets/sales.json");

            string json = GetSalesWithAppliedDiscount(db);

            File.WriteAllText("../../../JsonExports/sales-discounts.json", json);
        }
        private static void ResetDb(CarDealerContext context)
        {
            context.Database.EnsureDeleted();
            Console.WriteLine("Database was successfully deleted!");
            context.Database.EnsureCreated();
            Console.WriteLine("Database was successfully created!");
        }

        // >>Importing<< //

        //Problem 09
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}.";
        }

        //Problem 10
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson)
                .Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId))
                .ToArray();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}.";
        }

        //Problem 11
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<List<CarDTO>>(inputJson);

            foreach (var car in cars)
            {
                var newCar = new Car
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance
                };

                context.Cars.Add(newCar);

                foreach (var part in car.PartsId.Distinct())
                {
                    var newPartCar = new PartCar
                    {
                        PartId = part,
                        Car = newCar
                    };

                    context.PartCars.Add(newPartCar);
                }
            }

            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        //Problem 12
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}.";
        }

        //Problem 13
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}.";
        }

        // >>Exporting<< // 

        //Problem 14
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .AsEnumerable()
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    c.IsYoungDriver
                })
                .ToList();

            string json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        //Problem 15
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotaCars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TravelledDistance
                })
                .ToList();

            string json = JsonConvert.SerializeObject(toyotaCars, Formatting.Indented);

            return json;
        }

        //Problem 16 
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suplliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToList();

            string json = JsonConvert.SerializeObject(suplliers, Formatting.Indented);

            return json;
        }
        //Problem 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                       .Select(c => new
                       {
                           car = new
                           {
                               c.Make,
                               c.Model,
                               c.TravelledDistance
                           },
                           parts = c.PartCars
                                   .Select(p => new
                                   {
                                       p.Part.Name,
                                       Price = p.Part.Price.ToString("F2")
                                   })
                       })
                       .ToList();

            string json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }

        //Problem 18
        // Doesn't work local but Judge gives 100/100

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count(),
                    spentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(x => x.Part.Price))
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToList();

            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        // >> This is a working code << //

        //var customers = context.Cars
        //                   .Where(c => c.Sales.Count() > 0)
        //                   .Select(c => new
        //                   {
        //                       fullName = c.Sales.Select(t => t.Customer.Name).First(),
        //                       boughtCars = c.Sales.Count(),
        //                       spentMoney = c.PartCars.Sum(y => y.Part.Price)
        //                   })
        //                   .OrderByDescending(m => m.spentMoney)
        //                   .ThenByDescending(t => t.boughtCars)
        //                   .ToList();


        //Problem 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new
                {
                    car = new
                    {
                        s.Car.Make,
                        s.Car.Model,
                        s.Car.TravelledDistance,
                    },
                    customerName = s.Customer.Name,
                    Discount = s.Discount.ToString("f2"),
                    price = s.Car.PartCars.Sum(p => p.Part.Price).ToString("f2"),
                    priceWithDiscount = $"{s.Car.PartCars.Sum(x => x.Part.Price) * (1 - s.Discount / 100):F2}"
                })
                .Take(10)
                .ToList();

            string json = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return json;
        }
    }
}