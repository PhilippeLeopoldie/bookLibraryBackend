using System.Linq.Expressions;
using LibraryBackend.Common;
using LibraryBackend.Controllers;
using LibraryBackend.Data;
using LibraryBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LibraryBackend.Tests
{
  public class UnitTestOpinionController
  {
    readonly OpinionController _opinionController;
    readonly Mock<IRepository<Opinion>> _mockOpinionRepository;

    public UnitTestOpinionController()
    {
      _mockOpinionRepository = new Mock<IRepository<Opinion>>();
      _opinionController = new OpinionController(_mockOpinionRepository.Object);
    }

    List<Opinion> mockOpinionData = new List<Opinion>
    {
      new Opinion { View="View1", BookId = 1, Rate = 2 },
      new Opinion { View="View2", BookId = 2, Rate = 5 },
      new Opinion { View="View3", BookId = 3, Rate = 4 }
    };

    [Fact]
    public async Task Should_get_all_opinions_in_GetOpinions()
    {
      // Arrange
      _mockOpinionRepository!.Setup(mockRepository => mockRepository.GetAllAsync()).ReturnsAsync(mockOpinionData);

      // Act
      var result = await _opinionController.GetOpinions();

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var opinions = Assert.IsAssignableFrom<IEnumerable<Opinion>>(okResult.Value);
      Assert.Equal(3,opinions.Count());
    }

    [Fact]
    public async Task Should_return_not_found_for_null_data_in_GetOpinions()
    {
      // Arrange
      List<Opinion>? nullOpinionData = null;
      #pragma warning disable CS8604
      _mockOpinionRepository
        .Setup(mockRepository => mockRepository.GetAllAsync())
        .ReturnsAsync(nullOpinionData);
      #pragma warning restore CS8604

      // Act
      var result = await _opinionController.GetOpinions();

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
      _mockOpinionRepository.Verify(mockRepository => mockRepository.GetAllAsync(),Times.Once);
    }

    [Fact]
    public async Task Should_get_all_opinions_in_GetOpinionsByBookId()
    {
      // Arrange
      var bookIdToSearch = 1;
      var expectedOpinions = from opinion in mockOpinionData
                             where opinion.BookId == bookIdToSearch
                             select opinion;
      _mockOpinionRepository.Setup(mockRepository => mockRepository.FindByConditionAsync(
        It.IsAny<Expression<Func<Opinion, bool>>>()
      )).ReturnsAsync(expectedOpinions);

      // Act
      var result = await _opinionController.GetOpinionsByBookId(bookIdToSearch);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var listOpinions = Assert.IsAssignableFrom<IEnumerable<Opinion>>(okResult.Value);
      _mockOpinionRepository.Verify(mockRepository => mockRepository.FindByConditionAsync(
        It.IsAny<Expression<Func<Opinion, bool>>>()
        ), Times.Once);
    }


  }
}