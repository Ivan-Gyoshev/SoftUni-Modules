namespace BookShop
{
    using Data;
    using System;
    using System.Linq;
    using System.Text;
    using Initializer;
    using BookShop.Models.Enums;
    using System.Collections.Generic;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //int input = int.Parse(Console.ReadLine());
            //string input = Console.ReadLine();

            var result = RemoveBooks(db);

            Console.WriteLine(result);
        }

        //Problem 01
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {

            var result = context.Books
                .AsEnumerable()
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            return string.Join(Environment.NewLine, result);
        }
        //Problem 02
        public static string GetGoldenBooks(BookShopContext context)
        {
            var result = context
                .Books
                .AsEnumerable()
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, result);
        }
        //Problem 03
        public static string GetBooksByPrice(BookShopContext context)
        {
            var sb = new StringBuilder();

            var result = context
                .Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList();

            foreach (var book in result)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 04
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var result = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, result);
        }
        //Problem 05
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();

            var result = new List<string>();

            foreach (var category in categories)
            {
                var bookTitles = context
                    .Books
                    .Where(b => b.BookCategories.Any(bc => bc.Category.Name.ToLower() == category))
                    .Select(b => b.Title)
                    .ToList();

                result.AddRange(bookTitles);
            }

            result = result
                .OrderBy(b => b)
                .ToList();

            return string.Join(Environment.NewLine, result);
        }
        //Problem 06
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var sb = new StringBuilder();

            var exactDate = DateTime.ParseExact(date, "dd-MM-yyyy", null);

            var result = context
              .Books
              .Where(b => b.ReleaseDate < exactDate)
              .OrderByDescending(b => b.ReleaseDate)
              .Select(b => new
              {
                  b.Title,
                  b.EditionType,
                  b.Price
              })
              .ToList();

            foreach (var book in result)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 07
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var sb = new StringBuilder();

            var result = context
                .Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    Name = a.FirstName + " " + a.LastName
                })
                .OrderBy(a => a.Name)
                .ToList();



            foreach (var author in result)
            {
                sb.AppendLine($"{author.Name}");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 08
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {

            var result = context
                .Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            return string.Join(Environment.NewLine, result);
        }
        //Problem 09
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var sb = new StringBuilder();

            var result = context
                .Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    AuthorName = b.Author.FirstName + " " + b.Author.LastName
                })
                .ToList();

            foreach (var book in result)
            {
                sb.AppendLine($"{book.Title} ({book.AuthorName})");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 10
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var result = context
                .Books
                .AsEnumerable()
                .Where(b => b.Title.Count() > lengthCheck)
                .Count();

            return result;
        }
        //Problem 11
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var sb = new StringBuilder();

            var result = context
                .Authors
                .Select(a => new
                {
                    Name = a.FirstName + " " + a.LastName,
                    Coppies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(b => b.Coppies)
                .ToList();

            foreach (var book in result)
            {
                sb.AppendLine($"{book.Name} - {book.Coppies}");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 12
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var sb = new StringBuilder();

            var result = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    TotalProfit = c.CategoryBooks
                                   .Select(cb => new
                                   {

                                       BookProfit = cb.Book.Copies * cb.Book.Price
                                   })
                                   .Sum(ct => ct.BookProfit)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.Name)
                .ToList();

            foreach (var cat in result)
            {
                sb.AppendLine($"{cat.Name} ${cat.TotalProfit:f2}");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 13
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var sb = new StringBuilder();

            var result = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    MostRecentBook = c.CategoryBooks
                .OrderByDescending(cb => cb.Book.ReleaseDate)
                .Take(3)
                .Select(cb => new
                {
                    cb.Book.Title,
                    cb.Book.ReleaseDate.Value.Year
                })
                })
                .OrderBy(c => c.Name)
                .ToList();

            foreach (var cat in result)
            {
                sb.AppendLine($"--{cat.Name}");
                foreach (var catBook in cat.MostRecentBook)
                {
                    sb.AppendLine($"{catBook.Title} ({catBook.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 14
        public static void IncreasePrices(BookShopContext context)
        {
            var booksToUpdate = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var book in booksToUpdate)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }
        //Problem 15
        public static int RemoveBooks(BookShopContext context)
        {          
            var booksToRemove = context
                .Books
                .Where(b => b.Copies < 4200)
                .ToList();

            context.RemoveRange(booksToRemove);
            context.SaveChanges();

            return booksToRemove.Count;
        }
    }
}
