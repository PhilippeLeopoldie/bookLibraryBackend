using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryBackend.Models;
using LibraryBackend.Data;



namespace LibraryBackend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class OpinionController : ControllerBase
  {

    private readonly IRepository<Opinion> _OpinionRepository;
    private readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

    public OpinionController(IRepository<Opinion> opinionRepository)
    {
      _OpinionRepository = opinionRepository;
    }

    // GET: api/Opinion
    /* [HttpGet]
    public async Task<ActionResult<IEnumerable<Opinion>>> GetOpinion()
    {
        return await _context.Opinion.ToListAsync();
    } */

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Opinion>>> GetOpinion()
    {
      var Opinions = await _OpinionRepository.GetAllAsync();
      return Ok(Opinions);
    }
    // GET: api/Opinion/5
    [HttpGet("{id}")]
    /* public ActionResult<Opinion> GetOpinionById(int id)
    {
        var opinion = _context.Opinion.Include(x => x.Book).FirstOrDefault(x => x.Id == id);

        if (opinion == null)
        {
            return NotFound();
        }

        return opinion;
    } */

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> GetOpinionById(int id)
    {
      try
      {
        var opinion = await _OpinionRepository.GetByIdAsync(id);
        if (opinion == null) return NotFound();
        return Ok(opinion);

      }
      catch
      {
        return NotFound();
      }

    }

    // PUT: api/Opinion/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    /* [HttpPut("{id}")]
    public async Task<IActionResult> PutOpinion(int id, Opinion opinion)
    {
      if (id != opinion.Id)
      {
        return BadRequest();
      }

      _context.Entry(opinion).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!OpinionExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    } */

    // POST: api/Opinion
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    /* [HttpPost]
    public async Task<ActionResult<Opinion>> PostOpinion(OpinionAddRequest opinion)
    {
      _context.Opinion.Add(new Opinion
      {
        Like = opinion.Like,
        BookId = opinion.BookId,
        View = opinion.View,
        userName = opinion.userName
      });
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetOpinion", new { id = opinion.OpinionId }, opinion);
    } */

    // DELETE: api/Opinion/5
    /* [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOpinion(int id)
    {
      var opinion = await _context.Opinion.FindAsync(id);
      if (opinion == null)
      {
        return NotFound();
      }

      _context.Opinion.Remove(opinion);
      await _context.SaveChangesAsync();

      return NoContent();
    } */

    /* private bool OpinionExists(int id)
    {
      return _context.Opinion.Any(e => e.Id == id);
    } */
  }
}
