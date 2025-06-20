using Azure.AI.OpenAI;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenAI.Chat;
using System.Text.Json;

public class IndexModel : PageModel
{
    private readonly IConfiguration _configuration;
    public IndexModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [BindProperty]
    public string AirtimeProvider { get; set; } = "MTN"; // Default to MTN

    [BindProperty]
    public decimal AirtimeAmount { get; set; }

    [BindProperty]
    public int InvestmentPercentage { get; set; }

    [BindProperty]
    public string SavingFor { get; set; } = string.Empty;

    [BindProperty]
    public decimal AmountNeeded { get; set; }

    [BindProperty]
    public DateTime DateNeededBy { get; set; }

    [BindProperty]
    public string AiFoundryResult { get; set; }


    // Read-only property to calculate InvestmentAmount
    public decimal InvestmentAmount => AirtimeAmount * (InvestmentPercentage / 100M);

    public void OnGet()
    {
        DateNeededBy = DateTime.Today;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Retrieve the OpenAI endpoint from environment variables
        var endpoint = "https://airtime-saver.openai.azure.com/openai/deployments/gpt-4.1-mini/chat/completions?api-version=2025-01-01-preview";
        if (string.IsNullOrEmpty(endpoint))
        {
            Console.WriteLine("Please set the AZURE_OPENAI_ENDPOINT environment variable.");
        }

        var key = "fIBWb5MUsXH078gT52P37CwwKpdPbhsjyLzDHmIvxXAzSeBbwlR9";
        if (string.IsNullOrEmpty(key))
        {
            Console.WriteLine("Please set the AZURE_OPENAI_KEY environment variable.");
        }

        AzureKeyCredential credential = new AzureKeyCredential(key);

        // Initialize the AzureOpenAIClient
        AzureOpenAIClient azureClient = new(new Uri(endpoint), credential);

        // Initialize the ChatClient with the specified deployment name
        ChatClient chatClient = azureClient.GetChatClient("gpt-4.1");

        // Create a list of chat messages
        var messages = new List<ChatMessage>
    {
        new SystemChatMessage(@"You are an AI assistant that helps people find information."),
    };


        // Create chat completion options

        var options = new ChatCompletionOptions
        {
            Temperature = (float)1,
            MaxOutputTokenCount = 800,

            TopP = (float)1,
            FrequencyPenalty = (float)0,
            PresencePenalty = (float)0
        };

        try
        {
            // Create the chat completion request
            ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);

            // Print the response
            if (completion != null)
            {
                Console.WriteLine(JsonSerializer.Serialize(completion, new JsonSerializerOptions() { WriteIndented = true }));
            }
            else
            {
                Console.WriteLine("No response received.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return Page();
    }
}
