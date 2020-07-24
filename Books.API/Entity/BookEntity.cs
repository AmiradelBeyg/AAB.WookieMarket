using System;
using System.ComponentModel.DataAnnotations;

namespace Books.API.Entity
{
    public class BookEntity
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Title of a book
        /// </summary>
        [Required]
        [MinLength(2)]
        [MaxLength(1000)]
        public string Title { get; set; }

        /// <summary>
        /// Name of Author.
        /// </summary>
        [Required]
        [MinLength(2)]
        [MaxLength(1000)]
        public string Author { get; set; }

        /// <summary>
        /// A summery of the book and information about it.
        /// </summary>
        [MinLength(2)]
        [MaxLength(1000)]
        public string Description { get; set; }

        public string CoverImageFileName { get; set; }

        /// <summary>
        /// Probably Wookies use the US dollar.
        /// </summary>
        [Required]
        [DataType(DataType.Currency)]
        public Decimal Price { get; set; }
    }
}
