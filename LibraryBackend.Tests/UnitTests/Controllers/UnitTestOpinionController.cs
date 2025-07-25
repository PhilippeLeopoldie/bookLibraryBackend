using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using LibraryBackend.Core.Entities;
using Services.Contracts;
using LibraryBackend.Core.Contracts;
using LibraryBackend.Presentation.Controllers;
using LibraryBackend.Core.Dtos.Opinions;
using LibraryBackend.Core.Exceptions;

namespace LibraryBackend.Tests.UnitTests.Controllers;

public class UnitTestOpinionController
{
    readonly OpinionController _opinionController;
    //readonly Mock<IUnitOfWork> _uow;
    readonly Mock<IOpinionService> _mockOpinionService;

    private const string notFoundErrorMessage = "No opinion found!";
    public UnitTestOpinionController()
    {
    //    _uow = new Mock<IUnitOfWork>();
        _mockOpinionService = new Mock<IOpinionService>();
        _opinionController = new OpinionController(_mockOpinionService.Object);
    }

    List<Opinion> mockOpinionData = new List<Opinion>
{
  new Opinion { Id = 1, View = "View1", BookId = 1, Rate = 2.0, UserName = "Lise"},
  new Opinion { Id = 2, View = "View2", BookId = 2, Rate = 5.2, UserName = "Paul"},
  new Opinion { Id = 5, View = "View2", BookId = 2, Rate = 3.3, UserName = "Lisa"},
  new Opinion { Id = 6, View = "View2", BookId = 2, Rate = 4.4, UserName = "Paula"},
  new Opinion { Id = 3, View = "View3", BookId = 3, Rate = 4.0, UserName = "Frank"},
  new Opinion { Id = 4, View = "View4", BookId = 1, Rate = 3.0, UserName = "Louis"},
};

    [Fact]
    public async Task Should_get_all_opinions_in_GetOpinions()
    {
        // Arrange
        _mockOpinionService.Setup(service => service.GetAllAsync()).ReturnsAsync(mockOpinionData);

        // Act
        var result = await _opinionController.GetOpinions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var opinions = Assert.IsAssignableFrom<IEnumerable<Opinion>>(okResult.Value);
        Assert.Equal(6, opinions.Count());
    }

    [Fact]
    public async Task Should_return_not_found_for_null_data_in_GetOpinions()
    {
        // Arrange
        List<Opinion>? nullOpinionData = null;
#pragma warning disable CS8604
        _mockOpinionService
          .Setup(service => service.GetAllAsync())
          .ReturnsAsync(nullOpinionData);
#pragma warning restore CS8604

        // Act
        var actionResult = await _opinionController.GetOpinions();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal(notFoundErrorMessage, notFoundResult.Value);
        _mockOpinionService.Verify(service => service.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Should_return_not_found_for_empty_data_in_GetOpinions()
    {
        // Arrange
        List<Opinion>? emptyOpinionData = new List<Opinion>();
        _mockOpinionService 
          .Setup(service => service.GetAllAsync())
          .ReturnsAsync(emptyOpinionData);

        // Act
        var actionResult = await _opinionController.GetOpinions();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal(notFoundErrorMessage, notFoundResult.Value);
        _mockOpinionService.Verify(service => service.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Should_get_all_opinions_in_GetOpinionsByBookId()
    {
        // Arrange
        var bookIdToSearch = 1;
        var ActualOpinions = from opinion in mockOpinionData
                             where opinion.BookId == bookIdToSearch
                             select opinion;
        _mockOpinionService.Setup(service => service.GetOpinionsByBookId(
          It.IsAny<int>()
        ))
        .ReturnsAsync(ActualOpinions);

        // Act
        var result = await _opinionController.GetOpinionsByBookId(bookIdToSearch);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var listOpinions = Assert.IsAssignableFrom<IEnumerable<Opinion>>(okResult.Value);
        _mockOpinionService.Verify(service => service.GetOpinionsByBookId(
          It.IsAny<int>()
          ), Times.Once);
        Assert.Equal(2, ActualOpinions.Count());
        var actualLastOpinion = listOpinions.Last();

        var expectedLastOpinion = new Opinion { Id = 4, View = "View4", BookId = 1, Rate = 3, UserName = "Louis" };

        Assert.Equivalent(expectedLastOpinion, actualLastOpinion);
        Assert.Equal(DateTime.Now.ToString("yyyy/MM/dd"), actualLastOpinion.PostDate);
    }

    [Fact]
    public async Task Should_return_not_found_for_non_existing_bookId_in_GetOpinionsByBookId()
    {
        // Arrange
        var nonExistingId = 99;
        var emptyList = new List<Opinion>();
        _mockOpinionService
          .Setup(service => service.GetOpinionsByBookId(
            It.IsAny<int>()
          ))
          .ReturnsAsync(emptyList);

        // Act 
        var actionResult = await _opinionController.GetOpinionsByBookId(nonExistingId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal(notFoundErrorMessage, notFoundResult.Value);
        _mockOpinionService.Verify(service => service.GetOpinionsByBookId(
          It.IsAny<int>()
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
            View = "amazing! I'm warmly recommend it",
            UserName = "Lise"
        };
        _mockOpinionService
          .Setup(service => service.GetByIdAsync(opinionId))
          .ReturnsAsync(opinionById);

        _mockOpinionService
          .Setup(service => service.Update(opinionById!))
          .ReturnsAsync(opinionById!);

        // Act
        var actionResult = await _opinionController.UpdateOpinion(opinionId, opinionDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var opinionResult = Assert.IsAssignableFrom<Opinion>(okResult.Value);

        var expectedOpinion = (1, 5, "amazing! I'm warmly recommend it", "Lise");
        var actualOpinion = (opinionResult.Id, opinionResult.Rate, opinionResult.View, opinionResult.UserName);

        Assert.Equal(expectedOpinion, actualOpinion);

        _mockOpinionService
          .Verify(service => service.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _mockOpinionService
          .Verify(service => service.Update(It.IsAny<Opinion>()), Times.Once);
    }

    [Fact]
    public async Task Should_get_Opinion_with_the_average_rate_in_GetAverageRateByBookId()
    {
        var bookId = 2;
        // Arrange
        var expectedAverageRate = 3.0;
        _mockOpinionService
          .Setup(mockService => mockService.AverageOpinionRate(bookId))
          .ReturnsAsync(expectedAverageRate);

        // Act
        var resultOpinion = await _opinionController.GetAverageRateByBookId(bookId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultOpinion.Result);
        var opinionResponses = Assert.IsType<double>(okResult.Value);
        Assert.Equal(expectedAverageRate, opinionResponses);
    }

    [Fact]
    public async Task Should_return_NotFound_when_no_Opinion_in_GetAverageRateByBookId()
    {
        var bookId = 2;
        // Arrange
        var expectedAverageRate = 0.0;
        _mockOpinionService
          .Setup(mockService => mockService.AverageOpinionRate(bookId))
          .ReturnsAsync(expectedAverageRate);

        // Act
        var resultOpinion = await _opinionController.GetAverageRateByBookId(bookId);

        // Assert
        var NotFoundResult = Assert.IsType<NotFoundObjectResult>(resultOpinion.Result);
        Assert.Equal(notFoundErrorMessage, NotFoundResult.Value);
    }

    [Fact]
    public async Task Should_return_badRequest_for_empty_fields_in_UpdateOpinion()
    {
        // Arrange
        var opinionId = 1;
        var opinionById = mockOpinionData.FirstOrDefault(opinion => opinion.Id == opinionId);
        var opinionDto = new OpinionDtoRequest
        {

            View = "",
            UserName = ""
        };
        _mockOpinionService
          .Setup(service => service.GetByIdAsync(opinionId))
          .ReturnsAsync(opinionById);

        _mockOpinionService
          .Setup(service => service.Update(opinionById!))
          .ReturnsAsync(opinionById!);

        // Act
        var actionResult = await _opinionController.UpdateOpinion(opinionId, opinionDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var error = Assert.IsAssignableFrom<ApiError>(badRequestResult.Value);


        Assert.Equal("View or UserName cannot be empty", error.Detail);

        _mockOpinionService
          .Verify(service => service.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockOpinionService
          .Verify(service => service.Update(It.IsAny<Opinion>()), Times.Never);

    }

    [Fact]
    public async Task Should_return_notFound_when_no_opinion_in_UpdateOpinion()
    {
        // Arrange
        var opinionId = 1;
        var nullOpinion = null as Opinion;
        var opinion = new Opinion
        {
            Id = 1,
            View = "view",
            UserName = "username"
        };
        var opinionDto = new OpinionDtoRequest
        {
            View = "viewdTO",
            UserName = "usernameDTO"
        };
        _mockOpinionService
          .Setup(service => service.GetByIdAsync(opinionId))
          .ReturnsAsync(nullOpinion);

        _mockOpinionService
          .Setup(service => service.Update(opinion))
          .ReturnsAsync(opinion);

        // Act
        var actionResult = await _opinionController.UpdateOpinion(opinionId, opinionDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal($"Opinion with Id {opinionId} not found", notFoundResult.Value);
        _mockOpinionService
          .Verify(service => service.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _mockOpinionService
          .Verify(service => service.Update(It.IsAny<Opinion>()), Times.Never);

    }

    [Fact]
    public async Task Should_create_one_opinion_with_valid_input_in_CreateOpinion()
    {
        // Arrange
        var opinionDtoRequest = new OpinionDtoRequest
        {
            BookId = 2,
            Rate = 3,
            View = "view2",
            UserName = "userName2"
        };
        var opinionToCreate = new Opinion
        {
            BookId = opinionDtoRequest.BookId,
            Rate = opinionDtoRequest.Rate,
            View = opinionDtoRequest.View,
            UserName = opinionDtoRequest.UserName
        };
        _mockOpinionService
          .Setup(service => service.Create(It.IsAny<Opinion>()))
          .ReturnsAsync(opinionToCreate);

        // Act
        var result = await _opinionController.CreateOpinion(opinionDtoRequest);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(StatusCodes.Status201Created, createdAtActionResult.StatusCode);
        var createdOpinion = Assert.IsType<Opinion>(createdAtActionResult.Value);
        var expectedOpinion = (opinionDtoRequest.BookId, opinionDtoRequest.Rate, opinionDtoRequest.View, opinionDtoRequest.UserName);
        var actualOpinion = (createdOpinion.BookId, createdOpinion.Rate, createdOpinion.View, createdOpinion.UserName);
        Assert.Equal(expectedOpinion, actualOpinion);
        _mockOpinionService.Verify(service => service.Create(It.IsAny<Opinion>()), Times.Once);
    }

    [Fact]
    public async Task Should_return_badRequest_when_empty_fields_in_CreateOpinion()
    {
        // Arrange
        var opinionDtoRequest = new OpinionDtoRequest
        {
            BookId = 2,
            Rate = 3,
            View = "",
            UserName = ""
        };
        var opinionToCreate = new Opinion
        {
            BookId = opinionDtoRequest.BookId,
            Rate = opinionDtoRequest.Rate,
            View = opinionDtoRequest.View,
            UserName = opinionDtoRequest.UserName
        };
        _mockOpinionService
          .Setup(service => service.Create(It.IsAny<Opinion>()))
          .ReturnsAsync(opinionToCreate);

        // Act
        var result = await _opinionController.CreateOpinion(opinionDtoRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
        Assert.Equal("Validation Error", apiError.Message);
        Assert.Equal("View, UserName or Rate cannot be empty", apiError.Detail);
        _mockOpinionService.Verify(service => service.Create(It.IsAny<Opinion>()), Times.Never);
    }

    [Fact]
    public async Task Should_return_badRequest_when_Rate_0_in_CreateOpinion()
    {
        // Arrange
        var opinionDtoRequest = new OpinionDtoRequest
        {
            BookId = 2,
            Rate = 0,
            View = "",
            UserName = ""
        };
        var opinionToCreate = new Opinion
        {
            BookId = opinionDtoRequest.BookId,
            Rate = opinionDtoRequest.Rate,
            View = opinionDtoRequest.View,
            UserName = opinionDtoRequest.UserName
        };
        _mockOpinionService
          .Setup(service => service.Create(It.IsAny<Opinion>()))
          .ReturnsAsync(opinionToCreate);

        // Act
        var result = await _opinionController.CreateOpinion(opinionDtoRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
        Assert.Equal("Validation Error", apiError.Message);
        Assert.Equal("View, UserName or Rate cannot be empty", apiError.Detail);
        _mockOpinionService.Verify(service => service.Create(It.IsAny<Opinion>()), Times.Never);
    }
}