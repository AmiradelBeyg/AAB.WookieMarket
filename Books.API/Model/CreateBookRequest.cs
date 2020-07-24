using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Books.API.Model
{
    ///<Summary>
    /// Book request
    ///</Summary>
    public class CreateBookRequest
    {
        /// <summary>
        /// Title of a book.
        /// </summary>
        [Required]
        [MinLength(2)]
        [MaxLength(1000)]
        public string Title { get; set; }

        /// <summary>
        /// Author of the book.
        /// </summary>
        [Required]
        [MinLength(2)]
        [MaxLength(1000)]
        public string Author { get; set; }

        /// <summary>
        /// Book description or summery.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The book's cover image file.
        /// </summary>
        public IFormFile CoverImage { get; set; }

        /// <summary>
        /// Price of a book(probably in the US dollar)
        /// </summary>
        [Required]
        [DataType(DataType.Currency)]
        public Decimal Price { get; set; }
    }
}
