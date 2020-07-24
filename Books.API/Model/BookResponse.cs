using System;
using System.ComponentModel.DataAnnotations;

namespace Books.API.Model
{
    ///<Summary>
    /// Book response
    ///</Summary>
    public class BookResponse
    {
        /// <summary>
        /// Id of book.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of a book.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Author of the book.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Book description or summery.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// URL of book's cover image
        /// </summary>
        public string CoverImageURL { get; set; }

        /// <summary>
        /// Price of a book(probably in the US dollar)
        /// </summary>
        [DataType(DataType.Currency)]
        public Decimal Price { get; set; }
    }
}
