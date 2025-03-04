using Microsoft.AspNetCore.Mvc;
using LibraryBackend.Services;
using LibraryBackend.Models;

namespace LibraryBackend.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class StoryController : ControllerBase
{
    private readonly IStoryService _storyService;

    public StoryController(IStoryService storyService)
    {
        _storyService = storyService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> GenerateAIStory([FromBody] StoryDtoRequest prompt)
    {
        try
        {
            var story = await _storyService.GenerateAIStoryAsync(prompt);
            return Ok(new { story });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
