using LibraryBackend.Repositories;
using Microsoft.AspNetCore.Mvc;
using LibraryBackend.Models;
using LibraryBackend.Services;



namespace LibraryBackend.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class GenreController : ControllerBase
{
    private readonly IRepository<Genre> _genreRepository;
    private readonly IGenreService? _genreService;

    public GenreController (IRepository<Genre> genreRepository)
    {
        _genreRepository = genreRepository;
    }

    // GET: api/<GenreController>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<IEnumerable<GenreDtoResponse>>> GetGenres()
    {
        throw new NotImplementedException();
    }

    // GET api/<GenreController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        throw new NotImplementedException();
    }

    // POST api/<GenreController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
        throw new NotImplementedException();
    }

    // PUT api/<GenreController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
        throw new NotImplementedException();
    }

    // DELETE api/<GenreController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        throw new NotImplementedException();
    }
}
