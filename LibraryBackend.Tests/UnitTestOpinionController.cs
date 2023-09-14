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
      new Opinion { Id = 1, View = "View1", BookId = 1, Rate = 2, UserName = "Lise"},
      new Opinion { Id = 2, View = "View2", BookId = 2, Rate = 5, UserName = "Paul"},
      new Opinion { Id = 3, View = "View3", BookId = 3, Rate = 4, UserName = "Frank"}
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
      Assert.Equal(3, opinions.Count());
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
      var actionResult = await _opinionController.GetOpinions();

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
      Assert.Equal("No opinion found!", notFoundResult.Value);
      _mockOpinionRepository.Verify(mockRepository => mockRepository.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Should_return_not_found_for_empty_data_in_GetOpinions()
    {
      // Arrange
      List<Opinion>? emptyOpinionData = new List<Opinion>();
      _mockOpinionRepository
        .Setup(mockRepository => mockRepository.GetAllAsync())
        .ReturnsAsync(emptyOpinionData);

      // Act
      var actionResult = await _opinionController.GetOpinions();

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
      Assert.Equal("No opinion found!", notFoundResult.Value);
      _mockOpinionRepository.Verify(mockRepository => mockRepository.GetAllAsync(), Times.Once);
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
      ))
      .ReturnsAsync(expectedOpinions);

      // Act
      var result = await _opinionController.GetOpinionsByBookId(bookIdToSearch);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var listOpinions = Assert.IsAssignableFrom<IEnumerable<Opinion>>(okResult.Value);
      _mockOpinionRepository.Verify(mockRepository => mockRepository.FindByConditionAsync(
        It.IsAny<Expression<Func<Opinion, bool>>>()
        ), Times.Once);
    }

    [Fact]
    public async Task Should_return_not_found_for_non_existing_bookId_in_GetOpinionsByBookId()
    {
      // Arrange
      var nonExistingId = 99;
      var emptyList = new List<Opinion>();
      _mockOpinionRepository
        .Setup(mockRepository => mockRepository.FindByConditionAsync(
          It.IsAny<Expression<Func<Opinion, bool>>>()
        ))
        .ReturnsAsync(emptyList);

      // Act 
      var actionResult = await _opinionController.GetOpinionsByBookId(nonExistingId);

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
      Assert.Equal("No opinion found!", notFoundResult.Value);
      _mockOpinionRepository.Verify(mockRepository => mockRepository.FindByConditionAsync(
        It.IsAny<Expression<Func<Opinion, bool>>>()
      ), Times.Once);
    }

    [Fact]
    public async Task Should_modify_opinion_By_OpinionId_in_UpdateOpinion()
    {
      // Arrange
      var opinionId = 1;
      var opinionById = mockOpinionData.FirstOrDefault(opinion => opinion.Id == opinionId);
      var opinionDto = new OpinionDtoRequest
      { 
        Rate = 5,
        View = "amazing! I'm warmly recommand it",
        UserName = "Lise" 
      };
      _mockOpinionRepository
        .Setup(opinionRepository => opinionRepository.GetByIdAsync(opinionId))
        .ReturnsAsync(opinionById);

      _mockOpinionRepository
        .Setup(mockOpinion => mockOpinion.Update(opinionById!))
        .ReturnsAsync(opinionById!);

      // Act
      var actionResult = await _opinionController.UpdateOpinion(opinionId, opinionDto);

      // Arrange
      var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
      var opinionResult = Assert.IsAssignableFrom<Opinion>(okResult.Value);

      var expectedOpinion = (1, 5, "amazing! I'm warmly recommend it", "Lise");
      var actualOpinion = (opinionResult.Id, opinionResult.Rate, opinionResult.View, opinionResult.UserName);

      Assert.Equal(expectedOpinion, actualOpinion);

      _mockOpinionRepository
        .Verify(mockRepository => mockRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
      _mockOpinionRepository
        .Verify(mockRepository => mockRepository.Update(It.IsAny<Opinion>()), Times.Once);
    }
  }
}