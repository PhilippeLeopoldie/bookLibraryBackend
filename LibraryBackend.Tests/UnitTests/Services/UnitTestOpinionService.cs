using Moq;
using System.Linq.Expressions;
using LibraryBackend.Core.Entities;
using Services.Contracts;
using LibraryBackend.Core.Contracts;
using LibraryBackend.Services;

namespace LibraryBackend.Tests.UnitTests.Services;


public class UnitTestOpinionService
{
    readonly IOpinionService _opinionService;
    readonly Mock<IBookService> _mockBookService;
    readonly Mock<IUnitOfWork> _uow;


    public UnitTestOpinionService()
    {
        _uow = new Mock<IUnitOfWork>();
        _mockBookService = new Mock<IBookService>();
        _opinionService = new OpinionService(_uow.Object, _mockBookService.Object);
    }

    [Fact]
    public async Task Should_return_averageRate()
    {
        // arrange
        var bookId = 1;
        var mockRoundedAverage = 2.4;
        var book = new Book
        {
            Id = 1,
            Title = "title1",
            Author = "author1",
            Description = "Description1",
            ImageUrl = "ImageUrl1",
            AverageRate = 3.6,
            Opinions = new List<Opinion>
      {
        new Opinion{View="View1", BookId = 1, Rate = 5},
        new Opinion{View="View2", BookId = 1, Rate = 2},
        new Opinion{View="View3", BookId = 1, Rate = 3},

      }
        };

        _uow
          .Setup(uow => uow.OpinionRepository.FindByConditionWithIncludesAsync(
            It.IsAny<Expression<Func<Opinion, bool>>>()))
            .ReturnsAsync(book.Opinions);
        _mockBookService
          .Setup(mockBookService => mockBookService.EditAverageRate(bookId, mockRoundedAverage))
          .ReturnsAsync(book);

        // Act
        var roundedAverage = await _opinionService.AverageOpinionRate(bookId);

        // Assert
        Assert.Equal(3.3, roundedAverage);



    }



}
