using P03_SalesDatabase.Data;
using P03_SalesDatabase.Data.Models;
using P03_SalesDatabase.Data.Seeding.Contracts;

namespace P03_SalesDatabase.Data.Seeding
{
    public class StoreSeeder : ISeeder
    {
        private readonly SalesContext dbContext;

        public StoreSeeder(SalesContext context)
        {
            this.dbContext = context;
        }

        public void Seed()
        {
            Store[] stores = new Store[]
            {
                new Store() {Name = "PcTech Sofia" },
                new Store() {Name = "PcTech Plovdiv" },
                new Store() {Name = "TechInovation Sofia" },
                new Store() {Name = "TechInovation Plovdiv" },
                new Store() {Name = "PCMarket Sofia" }
            };

            this.dbContext.Stores.AddRange(stores);
            this.dbContext.SaveChanges();
        }
    }
}
