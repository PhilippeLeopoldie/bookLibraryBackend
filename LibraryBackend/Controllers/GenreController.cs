﻿using LibraryBackend.Application;
using LibraryBackend.Common;
using LibraryBackend.Domain.Entities;
using Microsoft.AspNetCore.Mvc;



namespace LibraryBackend.Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class GenreController : ControllerBase
{
    private readonly IRepository<Genre> _genreRepository;
    private readonly IGenreService _genreService;
    private readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

    public GenreController(IRepository<Genre> genreRepository, IGenreService genreService)
    {
        _genreRepository = genreRepository;
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
    public string Get(int id)
    {
        throw new NotImplementedException();
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
        var createdGenre = await _genreRepository.Create(request);
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
    public void Delete(int id)
    {
        throw new NotImplementedException();
    }
}
