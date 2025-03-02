using OpenAI.Chat;

namespace LibraryBackend.Services;

public class StoryService : IStoryService
{
    private readonly string _apiKey;
    private readonly string _modelNames;

    public StoryService(IConfiguration configuration)
    {
        _apiKey = configuration["OPENAI_API_KEY"] ??
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new ArgumentNullException(nameof(configuration));
        _modelNames = "gpt-3.5-turbo";
    }

    public async Task<string> GenerateAIStoryAsync(string prompt)
    {
        var client = new ChatClient(_modelNames, _apiKey);
        var response = await client.CompleteChatAsync(prompt);
        return response.Value.Content[0].Text;
    }
}
