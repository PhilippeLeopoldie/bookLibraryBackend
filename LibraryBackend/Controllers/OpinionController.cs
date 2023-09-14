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


namespace LibraryBackend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class OpinionController : ControllerBase
  {
    private readonly IRepository<Opinion> _OpinionRepository;
    public OpinionController(IRepository<Opinion> opinionRepository)
    {
      _OpinionRepository = opinionRepository;
    }

    // GET: api/Opinion
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Opinion>>> GetOpinions()
    {
      var Opinions = await _OpinionRepository.GetAllAsync();
      if(Opinions == null || !Opinions.Any())
      {
        return NotFound("No opinion found!");
      }
      return Ok(Opinions); 
    }

    // GET: api/Opinion/5
    [HttpGet("{bookId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Opinion>>> GetOpinionsByBookId(int bookId)
    {
      Expression<Func<Opinion, bool>> condition = opinion => opinion.BookId == bookId;
      var opinions = await _OpinionRepository.FindByConditionAsync(condition);
      if (opinions == null || !opinions.Any())
      {
        return NotFound("No opinion found!");
      }
      return Ok(opinions);
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
          Detail = "View and UserName cannot be empty"
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
  }
}
