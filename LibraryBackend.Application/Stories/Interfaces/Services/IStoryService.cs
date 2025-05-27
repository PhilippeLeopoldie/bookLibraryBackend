
namespace LibraryBackend.Application;

public interface IStoryService
{
    Task<string> GenerateAIStoryAsync(StoryDtoRequest prompt);
}
