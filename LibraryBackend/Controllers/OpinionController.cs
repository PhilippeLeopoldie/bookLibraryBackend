using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryBackend.Models;
using LibraryBackend.Data;
using System.Linq.Expressions;
using LibraryBackend.Common;
using Microsoft.IdentityModel.Tokens;


namespace LibraryBackend.Controllers
{
  [Route("api/[controller]s")]
  [ApiController]
  public class OpinionController : ControllerBase
  {
    private readonly IRepository<Opinion> _OpinionRepository;
    private readonly IOpinionService _opinionService;

    private const string notFoundErrorMessage = "No opinion found!";
    public OpinionController(IRepository<Opinion> opinionRepository, IOpinionService opinionService)
    {
      _OpinionRepository = opinionRepository;
      _opinionService = opinionService;
    }

    // GET: api/Opinion
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Opinion>>> GetOpinions()
    {
      var Opinions = await _OpinionRepository.GetAllAsync();
      if (Opinions == null || !Opinions.Any())
      {
        return NotFound(notFoundErrorMessage);
      }
      return Ok(Opinions);
    }

    // GET: api/Opinion/5
    [HttpGet("BookId={bookId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Opinion>>> GetOpinionsByBookId(int bookId)
    {
      Expression<Func<Opinion, bool>> condition = opinion => opinion.BookId == bookId;
      var opinions = await _OpinionRepository.FindByConditionAsync(condition);
      if (opinions == null || !opinions.Any())
      {
        return NotFound(notFoundErrorMessage);
      }
      return Ok(opinions);
    }

    // GET: api/Opinion/averageRate
    [HttpGet("averageRate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public  async Task<ActionResult<double>> GetAverageRateByBookId(int bookId)
    {
      var opinionAverageRate = await _opinionService.AverageOpinionRate(bookId);
      if(opinionAverageRate == 0.0) 
      {
        return NotFound(notFoundErrorMessage);
      } 
      
        return Ok(opinionAverageRate);
    }
    

    //PUT: api/Opinion/2
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> UpdateOpinion(int id, OpinionDtoRequest opinionToUpdate)
    {
      if (string.IsNullOrWhiteSpace(opinionToUpdate.UserName) || string.IsNullOrWhiteSpace(opinionToUpdate.View))
      {
        var emptyDataError = new ApiError
        {
          Message = "Validation Error",
          Detail = "View or UserName cannot be empty"
        };
        return BadRequest(emptyDataError);
      }

      var opinionById = await _OpinionRepository.GetByIdAsync(id);
      if (opinionById == null)
      {
        return NotFound($"Opinion with Id {id} not found");
      }
      opinionById.Rate = opinionToUpdate.Rate;
      opinionById.View = opinionToUpdate.View;
      opinionById.UserName = opinionToUpdate.UserName;

      var updatedOpinion = await _OpinionRepository.Update(opinionById);
      return Ok(updatedOpinion);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Opinion>> CreateOpinion (OpinionDtoRequest newOpinion)
    {
      if(newOpinion == null || newOpinion.Rate?.Equals(0) == true || 
        string.IsNullOrWhiteSpace(newOpinion.View) || string.IsNullOrWhiteSpace(newOpinion.UserName))
      {
        var error = new ApiError
        {
          Message = "Validation Error",
          Detail = "View, UserName or Rate cannot be empty"
        };
        return BadRequest(error);
      }
      var opinionCreated = await _OpinionRepository.Create(
        new Opinion 
        {
          BookId = newOpinion.BookId,
          Rate = newOpinion.Rate,
          View = newOpinion.View,
          UserName = newOpinion.UserName
        }
      );
      
      
        await _opinionService.AverageOpinionRate(newOpinion.BookId);
      
      return CreatedAtAction(nameof(GetOpinions) , new {id = opinionCreated.Id }, opinionCreated );  
    }
  }
}
