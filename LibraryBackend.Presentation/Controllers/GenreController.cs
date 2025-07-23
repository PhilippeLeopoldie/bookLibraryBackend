using LibraryBackend.Core.Contracts;
using LibraryBackend.Core.Dtos.Genres;
using LibraryBackend.Core.Entities;
using LibraryBackend.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Threading.Tasks;



namespace LibraryBackend.Presentation.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class GenreController : ControllerBase
{
    private readonly IGenreService _genreService;
    private readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

    public GenreController(IUnitOfWork uow, IGenreService genreService)
    {
        _genreService = genreService;
    }

    // GET: api/<GenreController>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GenreListDtoResponse>> GetGenres()
    {
        var genres = await _genreService.ListOfGenresAsync();
        if (genres == null || !genres.Any())
        {
            return NotFound("No genre found!");
        }
        var genresResponse = new GenreListDtoResponse
        {
            Genres = genres,
            TotalGenreCount = genres.Count(),
            RequestedAt = DateTime.Now.ToString(dateTimeFormat),
        };
        return Ok(genresResponse);
    }

    // GET api/<GenreController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Genre>> GetGenreById(int id)
    {
        var genre = await _genreService.GetGenreByIdAsync(id);
        if (genre == null)
        {
            return NotFound($"No genre with id: '{id}' found!");
        }
        return Ok(genre);
    }

    // POST api/<GenreController>
    [HttpPost]
    public async Task<ActionResult<Genre>> CreateGenre(GenreDtoRequest genre)
    {
        if (genre == null || string.IsNullOrWhiteSpace(genre.Name))
        {
            var error = new ApiError
            {
                Message = "Validation Error",
                Detail = "Name cannot be empty"
            };
            return BadRequest(error);
        }
        var request = new Genre
        {
            Name = genre.Name,
            IsForStoryGeneration = genre.IsForStoryGeneration
        };
        var createdGenre = await _genreService.Create(request);
        return CreatedAtAction(nameof(GetGenres), new { id = createdGenre.Id }, createdGenre);
    }

    // PUT api/<GenreController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
        throw new NotImplementedException();
    }

    // DELETE api/<GenreController>/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteGenre(int id)
    {
        var genreToDelete = await _genreService.GetGenreByIdAsync(id);
        if (genreToDelete == null)
        {
            return NotFound($"No genre with id: '{id}' found!");
        }
        await _genreService.Delete(genreToDelete);
        return NoContent();
    }
}
