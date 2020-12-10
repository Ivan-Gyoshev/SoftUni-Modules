namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var topAuthors = context.Authors
                .ToArray()
                .OrderByDescending(a=>a.AuthorsBooks.Count)
                .ThenBy(a=>a.FirstName + " " + a.LastName)
                .Select(a => new
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    Books = a.AuthorsBooks
                    .OrderByDescending(ab=>ab.Book.Price)
                    .Select(ab => new
                    {
                        BookName = ab.Book.Name,
                        BookPrice = ab.Book.Price.ToString("f2")
                    })
                    .ToArray()

                })
                .ToArray();

            string json = JsonConvert.SerializeObject(topAuthors, Formatting.Indented);

            return json;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var oldestBooks = context.Books
                .Where(b => b.PublishedOn < date && b.Genre == (Genre)3)
                .Select(b => new ExportBookDto
                {
                    Name = b.Name,
                    Date = b.PublishedOn.ToString("d",CultureInfo.InvariantCulture),
                    Pages = b.Pages
                })
                .OrderByDescending(b => b.Pages)
                .ThenByDescending(b=>b.Date)
                .Take(10)
                .ToArray();

            string res = XmlConverter.Serialize(oldestBooks, "Books");

            return res;
        }
    }
}