namespace LibraryBackend.Services;

public interface IStoryService
{
    Task<string> GenerateAIStoryAsync(string prompt);
}
