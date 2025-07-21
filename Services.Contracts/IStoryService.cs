using LibraryBackend.Core.Dtos.Stories;

namespace Services.Contracts;

public interface IStoryService
{
    Task<string> GenerateAIStoryAsync(StoryDtoRequest prompt);
}
