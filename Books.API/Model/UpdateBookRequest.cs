using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Books.API.Model
{
    ///<Summary>
    /// Book request
    ///</Summary>
    public class UpdateBookRequest
    {
        /// <summary>
        /// Id of book (It is used in getbyId)
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Title of a book.
        /// </summary>
        [MinLength(2)]
        [MaxLength(1000)]
        public string Title { get; set; }

        /// <summary>
        /// Author of the book.
        /// </summary>
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
        [DataType(DataType.Currency)]
        public Nullable<Decimal> Price { get; set; }
    }
}
