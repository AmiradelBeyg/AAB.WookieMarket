using AutoMapper;
using Books.API.Entity;
using Books.API.Infrastructure;
using Books.API.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Books.API.Controllers
{
    [Route("api/books")]
    [ApiVersion("1.0")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly BooksContext _context;
        private readonly IMapper _mapper;

        public BookController(IWebHostEnvironment env, BooksContext booksContext, IMapper mapper)
        {
            _context = booksContext;
            _env = env;
            _mapper = mapper;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// Gets a list of all books.
        /// </summary>
        /// <remarks>
        /// Gets a list of all books with pagination.
        /// </remarks>
        /// <returns>List of books with page information</returns>
        /// <response code="200">Returns a list of books successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<BookResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsViewModel<BookResponse>>> GetAsync([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            var totalBooks = await _context.Books
                .LongCountAsync();

            var booksOnPage = await _context.Books
                .OrderBy(c => c.Id)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            //Convert to the model
            var BookListResponse = _mapper.Map<List<BookResponse>>(booksOnPage);

            var model = new PaginatedItemsViewModel<BookResponse>(pageIndex, pageSize, totalBooks, BookListResponse);

            return Ok(model);
        }

        /// <summary>
        ///  Returns a books
        /// </summary>
        /// <remarks>
        /// Gets a book by it's Id
        /// </remarks>
        /// <returns>returns a book</returns>
        /// <response code="400">Input parameter is empty or not in current format</response>
        /// <response code="404">The record not found</response>
        /// <response code="200">Returns a book successfully</response>
        [HttpGet("{id}", Name = "GetByIdAsync")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BookResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BookResponse>> GetByIdAsync(Guid id)
        {
            var bookEntity = await _context.Books.SingleOrDefaultAsync(ci => ci.Id == id);

            if (bookEntity != null)
            {
                var bookResponse = _mapper.Map<BookResponse>(bookEntity);

                return Ok(bookResponse);
            }

            return NotFound();
        }

        /// <summary>
        /// Creates a new book.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/books
        ///     {
        ///        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///        "title": "The Adventures of Tom Sawyer",
        ///        "Author": "Mark Twain",
        ///        "Description" : "a description or summery of the book"
        ///        "Price" : "50"
        ///     }
        ///
        /// </remarks>
        /// <param name="book"></param>
        /// <returns>A newly created book</returns>
        /// <response code="201">Returns the a created book</response>
        /// <response code="400">If input parameter is not correct</response>  
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateAsync([FromForm]CreateBookRequest book)
        {
            //Convert to the model
            var bookEntity = _mapper.Map<BookEntity>(book);

            if (book.CoverImage != null)
            {
                // Saving Image on Server
                var coverImage = book.CoverImage;
                var serverFileName = bookEntity.CoverImageFileName;
                var path = Path.Combine(_env.WebRootPath + "\\Images\\", serverFileName);
                if (coverImage.Length > 0)
                {
                    using var fileStream = new FileStream(path, FileMode.Create);
                    await coverImage.CopyToAsync(fileStream);
                }
            }

            _context.Books.Add(bookEntity);
            await _context.SaveChangesAsync();

            var bookResponse = _mapper.Map<BookResponse>(bookEntity);

            return CreatedAtRoute("GetByIdAsync", new { id = bookEntity.Id }, bookResponse);
        }

        /// <summary>
        /// Edits a book.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/books
        ///     {
        ///        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///        "title": "The Adventures of Tom Sawyer",
        ///        "Author": "Mark Twain",
        ///        "Description" : "a description or summery of the book"
        ///        "Price" : "50"
        ///     }
        ///
        /// </remarks>
        /// <param name="book"></param>
        /// <returns>A updated book</returns>
        /// <response code="201">Returns the updated book successfully</response>
        /// <response code="404">The book not found</response>
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> UpdateAsync([FromForm]UpdateBookRequest book)
        {
            var bookEntity = await _context.Books.SingleOrDefaultAsync(i => i.Id == book.Id);

            if (bookEntity == null)
            {
                return NotFound();
            }

            string preFileName = bookEntity.CoverImageFileName;
            string newFileName = null;

            if (book.CoverImage != null)
            {
                //Deleting the previous cover image
                if (!string.IsNullOrEmpty(preFileName))
                {
                    var webRoot = _env.WebRootPath;
                    var path = Path.Combine(webRoot + "\\Images\\", preFileName);

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                var coverImage = book.CoverImage;

                // Saving Image on Server
                newFileName = Guid.NewGuid() + Path.GetExtension(book.CoverImage.FileName);
                var newPath = Path.Combine(_env.WebRootPath + "\\Images\\", newFileName);
                if (coverImage.Length > 0)
                {
                    using var fileStream = new FileStream(newPath, FileMode.Create);
                    await coverImage.CopyToAsync(fileStream);
                }
            }

            //Should fix part
            if (!string.IsNullOrEmpty(book.Title)) bookEntity.Title = book.Title;
            if (!string.IsNullOrEmpty(book.Author)) bookEntity.Author = book.Author;
            if (!string.IsNullOrEmpty(book.Description)) bookEntity.Description = book.Description;
            if (book.Price.HasValue) bookEntity.Price = book.Price.Value;
            if (!string.IsNullOrEmpty(newFileName)) bookEntity.CoverImageFileName = newFileName;

            _context.Books.Update(bookEntity);

            await _context.SaveChangesAsync();

            var bookResponse = _mapper.Map<BookResponse>(bookEntity);

            return CreatedAtRoute("GetByIdAsync", new { id = bookEntity.Id }, bookResponse);
        }

        /// <summary>
        /// Deletes a book.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/books/640d5ca1-db52-4e79-b644-9b0e3e82af3d
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Deletes a book successfully</response>
        /// <response code="404">a book not found</response>  
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var book = await _context.Books.SingleOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(book.CoverImageFileName))
            {
                var webRoot = _env.WebRootPath;
                var path = Path.Combine(webRoot + "\\Images\\", book.CoverImageFileName);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            _context.Books.Remove(book);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}