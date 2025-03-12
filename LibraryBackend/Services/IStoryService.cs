using LibraryBackend.Models;
namespace LibraryBackend.Services;

public interface IStoryService
{
    Task<string> GenerateAIStoryAsync(StoryDtoRequest prompt);
}
