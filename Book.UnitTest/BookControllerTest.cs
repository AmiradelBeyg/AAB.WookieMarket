using AutoMapper;
using Books.API;
using Books.API.Controllers;
using Books.API.Infrastructure;
using Books.API.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Book.UnitTest
{
    public class BookControllerTest : IDisposable
    {
        private readonly BooksContext _dbContext;
        private readonly BookController _bookController;

        public BookControllerTest()
        {
            var dbOptions = new DbContextOptionsBuilder<BooksContext>()
                .UseInMemoryDatabase(databaseName: "in-memory")
                .Options;

            _dbContext = new BooksContext(dbOptions);
            _dbContext.AddRange(new BooksContextSeed().GetFakeBooks());
            _dbContext.SaveChanges();

            var bookContext = new BooksContext(dbOptions);

            //Real mapper
            var mappingProfile = new MappingProfileUnitTest();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(mappingProfile));
            var mapper = new Mapper(configuration);

            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            //Mocking IWebHostEnvironment
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment
               .Setup(m => m.WebRootPath)
               .Returns(projectDirectory);

            _bookController = new BookController(mockEnvironment.Object, bookContext, mapper);
        }

        [Fact]
        public async Task Get_Books_ReturnOk()
        {
            //Act
            var okResult = await _bookController.GetAsync();

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }
        [Fact]
        public async Task Get_Books_ReturnsAllItems()
        {
            //Arrange
            var pageSize = 10;
            var pageIndex = 0;

            var expectedTotalItems = _dbContext.Books.Count();
            var expectedItemsInPage = expectedTotalItems > 10 ? 10 : expectedTotalItems;
            //Act
            var okResult = await _bookController.GetAsync(pageSize, pageIndex);

            var okObjectResult = okResult.Result as OkObjectResult;

            // Assert
            var page = Assert.IsType<PaginatedItemsViewModel<BookResponse>>(okObjectResult.Value);
            Assert.Equal(expectedTotalItems, page.Count);
            Assert.Equal(pageIndex, page.PageIndex);
            Assert.Equal(pageSize, page.PageSize);
            Assert.Equal(expectedItemsInPage, page.Data.Count());
        }

        [Fact]
        public async Task GetById_UnknownGuidPassed_ReturnsNotFoundResult()
        {
            // Act
            var notFoundResult = await _bookController.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }
        [Fact]
        public async Task GetById_ExistingGuidPassed_ReturnsOkResult()
        {
            // Arrange
            var testGuid = _dbContext.Books.FirstOrDefault().Id;

            // Act
            var okResult = await _bookController.GetByIdAsync(testGuid);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }
        [Fact]
        public async Task GetById_ExistingGuidPassed_ReturnsCurrentValue()
        {
            // Arrange
            var testGuid = _dbContext.Books.FirstOrDefault().Id;

            // Act
            var okResult = await _bookController.GetByIdAsync(testGuid);

            var okObjectResult = okResult.Result as OkObjectResult;

            // Assert
            Assert.IsType<BookResponse>(okObjectResult.Value);
            Assert.Equal(testGuid, (okObjectResult.Value as BookResponse).Id);
        }

        [Fact]
        public async Task Create_ValidObjectPassed_ReturnsCreatedResponse()
        {
            //Arrange
            var cbr = new CreateBookRequest
            {
                Title = "Art of Unit Testing",
                Author = "Roy Osherove",
                Description = "The Art of Unit Testing is a 2009 book by Roy Osherove which " +
                              "covers unit test writing for software.",
                Price = 100,
                CoverImage = null
            };

            //Act
            var actionResult = await _bookController.CreateAsync(cbr);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(actionResult);
        }
        [Fact]
        public async Task Create_ValidObjectPassed_ReturnedResponseHasCreatedItem()
        {
            // Arrange
            var cbr = new CreateBookRequest
            {
                Title = "Art of Unit Testing",
                Author = "Roy Osherove",
                Description = "The Art of Unit Testing is a 2009 book by Roy Osherove which " +
                              "covers unit test writing for software.",
                Price = 100,
                CoverImage = null
            };

            // Act
            var createdResponse = await _bookController.CreateAsync(cbr) as CreatedAtRouteResult;

            var item = createdResponse.Value as BookResponse;

            // Assert
            Assert.IsType<BookResponse>(item);
            Assert.Equal("Art of Unit Testing", item.Title);
        }

        [Fact]
        public async Task Delete_NotExistingGuidPassed_ReturnsNotFoundResponse()
        {
            // Arrange
            var notExistingGuid = Guid.NewGuid();

            // Act
            var badResponse = await _bookController.DeleteAsync(notExistingGuid);

            // Assert
            Assert.IsType<NotFoundResult>(badResponse);
        }
        [Fact]
        public async Task Delete_ExistingGuidPassed_ReturnsOkResult()
        {
            // Arrange
            var existingGuid = _dbContext.Books.FirstOrDefault().Id;

            // Act
            var okResponse = await _bookController.DeleteAsync(existingGuid);

            // Assert
            Assert.IsType<OkResult>(okResponse);
        }
        [Fact]
        public async Task Delete_ExistingGuidPassed_RemovesOneItem()
        {
            // Arrange
            var existingGuid = _dbContext.Books.FirstOrDefault().Id;

            var expectedTotalItems = _dbContext.Books.Count() - 1;

            // Act
            _ = await _bookController.DeleteAsync(existingGuid);

            // Assert
            Assert.Equal(expectedTotalItems, _dbContext.Books.Count());
        }

        [Fact]
        public async Task Update_NotExistingGuidPassed_ReturnsNotFoundResponse()
        {
            // Arrange
            var cbr = new UpdateBookRequest
            {
                Id = new Guid(),
                Title = "Art of Unit Testing",
                Author = "Roy Osherove",
                Price = 100
            };

            // Act
            var badResponse = await _bookController.UpdateAsync(cbr);

            // Assert
            Assert.IsType<NotFoundResult>(badResponse);
        }
        [Fact]
        public async Task Update_ValidObjectPassed_ReturnsUpdatedResponse()
        {
            //Arrange
            var cbr = new UpdateBookRequest
            {
                Id = _dbContext.Books.FirstOrDefault().Id,
                Title = "Art of Unit Testing",
                Author = "Roy Osherove",
                Price = 100
            };

            //Act
            var actionResult = await _bookController.UpdateAsync(cbr);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(actionResult);
        }
        [Fact]
        public async Task Update_ValidObjectPassed_ReturnedResponseHasUpdatedItem()
        {
            // Arrange
            var cbr = new UpdateBookRequest
            {
                Id = _dbContext.Books.FirstOrDefault().Id,
                Title = "Art of Unit Testing. Updated!",
                Author = "Roy Osherove. Updated!",
                Price = 999
            };

            // Act
            var createdResponse = await _bookController.UpdateAsync(cbr) as CreatedAtRouteResult;

            var item = createdResponse.Value as BookResponse;

            // Assert
            Assert.IsType<BookResponse>(item);
            Assert.Equal("Art of Unit Testing. Updated!", item.Title);
            Assert.Equal("Roy Osherove. Updated!", item.Author);
            Assert.Equal(999, item.Price);
        }

        private class TestBookSettings : IOptionsSnapshot<BookSettings>
        {
            public BookSettings Value => new BookSettings
            {
                CoverImageBaseUrl = "http://image-server.com/"
            };

            public BookSettings Get(string name) => Value;
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}
