using Books.API.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Books.API.Infrastructure
{
    public class BooksContextSeed
    {
        public async Task SeedAsync(BooksContext context)
        {
            context.Books.AddRange(GetFakeBooks());

            await context.SaveChangesAsync();
        }

        public List<BookEntity> GetFakeBooks()
        {
            return new List<BookEntity>()
            {
                new BookEntity()
                {
                    Id = new Guid("176924b6-adcc-4ee0-b791-430105b5af51"),
                    Title = "Sapiens: A Brief History of Humankind",
                    Author = "Yuval Noah Harari",
                    Description =
                        "Sapiens: A Brief History of Humankind is a book " +
                        "by Yuval Noah Harari, first published in Hebrew in Israel in 2011[1] based on a series of lectures Harari taught at The Hebrew " +
                        "University of Jerusalem, and in English in 2014.[2][3] The book surveys the history of humankind from the evolution of archaic " +
                        "human species in the Stone Age up to the twenty-first century, focusing on Homo sapiens. The account is situated within a " +
                        "framework that intersects the natural sciences with the social sciences.",
                    CoverImageFileName = "Sapiens.jpg",
                    Price = 200
                },
                new BookEntity()
                {
                    Id = new Guid("17418cd0-ef0d-4523-872b-49c87b55fd3a"),
                    Title = "One Flew Over the Cuckoo's Nest",
                    Author = "Ken Kesey",
                    Description =
                            "Set in an Oregon psychiatric hospital, " +
                            "the narrative serves as a study of institutional processes " +
                            "and the human mind as well as a critique of behaviorism and a tribute to individualistic " +
                            "principles.",
                    CoverImageFileName = "One flow.jpg",
                    Price = 1000
                },
                new BookEntity()
                {
                    Id = new Guid("c143a75a-fa88-4c38-86aa-f0cf3795bf11"),
                    Title = "Book 1",
                    Author = "Author 1",
                    Description = null,
                    CoverImageFileName = null,
                    Price = 300
                },
                new BookEntity()
                {
                    Id = new Guid("63303204-0abf-447f-89f1-95938e9fc7da"),
                    Title = "Book 2",
                    Author = "Author 2",
                    Description = null,
                    CoverImageFileName = null,
                    Price = 400
                }
            };
        }
    }

}
