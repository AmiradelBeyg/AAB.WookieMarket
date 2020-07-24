using Books.API.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Books.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    public class CoverImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly BooksContext _context;

        public CoverImageController(IWebHostEnvironment env, BooksContext catalogContext)
        {
            _env = env;
            _context = catalogContext;
        }

        /// <summary>
        /// For downloading cover image of a book.
        /// </summary>
        /// <remarks>
        /// For downloading cover image of a book.
        /// </remarks>
        /// <param name="bookId"></param>
        /// <returns>returns book's image cover</returns>
        /// <response code="201">book's cover image uploaded</response>
        /// <response code="404">Book not found</response>
        /// <response code="400">Book's id is not valid</response>
        [HttpGet]
        [Route("api/books/{bookId}/CoverImage")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> DownloadImageAsync([FromRoute]Guid bookId)
        {
            var item = await _context.Books.SingleOrDefaultAsync(ci => ci.Id == bookId);

            if (item != null)
            {
                var webRoot = _env.WebRootPath;
                var path = Path.Combine(webRoot + "\\Images\\", item.CoverImageFileName);

                string imageFileExtension = Path.GetExtension(item.CoverImageFileName);
                string mimetype = GetImageMimeTypeFromImageFileExtension(imageFileExtension);

                var buffer = System.IO.File.ReadAllBytes(path);

                return File(buffer, mimetype);
            }

            return NotFound();
        }

        /// <summary>
        /// For uploading cover image of a book.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/books/{bookId:Guid}/CoverImage
        ///     {       
        ///        
        ///     }
        ///
        /// </remarks>
        /// <param name="bookId"></param>
        /// <param name="coverImage"></param>
        /// <returns></returns>
        /// <response code="201">book's cover image uploaded</response>
        /// <response code="404">Book's information not found</response>
        /// <response code="400">Book's id is not valid</response>
        [HttpPost]
        [Route("api/books/{bookId}/CoverImage")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> UpdateImageAsync([FromRoute]Guid bookId, [FromForm] IFormFile coverImage)
        {
            if (string.IsNullOrEmpty(bookId.ToString()))
            {
                return BadRequest();
            }

            var item = await _context.Books.SingleOrDefaultAsync(ci => ci.Id == bookId);

            if (item != null)
            {
                if (!string.IsNullOrEmpty(item.CoverImageFileName))
                {
                    var path = Path.Combine(_env.WebRootPath + "\\Images\\", item.CoverImageFileName);
                    if (coverImage.Length > 0)
                    {
                        using var fileStream = new FileStream(path, FileMode.Create);
                        await coverImage.CopyToAsync(fileStream);
                    }
                    return Ok();
                }
                else
                    return NotFound(new { Message = $"Book with id { bookId } does not have a cover image." });
            }

            return NotFound();
        }

        private string GetImageMimeTypeFromImageFileExtension(string extension)
        {
            string mimetype;

            switch (extension)
            {
                case ".png":
                    mimetype = "image/png";
                    break;
                case ".gif":
                    mimetype = "image/gif";
                    break;
                case ".jpg":
                case ".jpeg":
                    mimetype = "image/jpeg";
                    break;
                case ".bmp":
                    mimetype = "image/bmp";
                    break;
                case ".tiff":
                    mimetype = "image/tiff";
                    break;
                case ".wmf":
                    mimetype = "image/wmf";
                    break;
                case ".jp2":
                    mimetype = "image/jp2";
                    break;
                case ".svg":
                    mimetype = "image/svg+xml";
                    break;
                default:
                    mimetype = "application/octet-stream";
                    break;
            }

            return mimetype;
        }
    }
}