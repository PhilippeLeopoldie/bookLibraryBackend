using Microsoft.AspNetCore.Mvc;
using LibraryBackend.Application;

namespace LibraryBackend.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StoryController : ControllerBase
{
    private readonly IStoryService _storyService;

    public StoryController(IStoryService storyService)
    {
        _storyService = storyService;
    }

    [HttpPost("AI")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> GenerateAIStory([FromBody] StoryDtoRequest prompt)
    {
        try
        {
            var story = await _storyService.GenerateAIStoryAsync(prompt);
            return CreatedAtAction("GenerateAIStory" ,new { story });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
