using System;
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.ClientModel;

class Program
{
    static async Task Main(string[] args)
    {
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "https://dummy-endpoint.openai.azure.com";
        var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? "dummy_key";

        var chatClient = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey)).GetChatClient("gpt-4o-mini");

        // This is the extension method in OpenAI.Chat.OpenAIChatClientExtensions
        var agent = chatClient.CreateAIAgent(
            name: "Assistant",
            instructions: "You are a helpful AI assistant."
        );

        var thread = agent.GetNewThread();
        Console.WriteLine("Agent ready! Type 'exit' to quit.\n");

        while (true)
        {
            Console.Write("You > ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

            Console.Write("Agent > ");
            try
            {
                await foreach (var update in agent.RunStreamingAsync(input, thread))
                {
                    Console.Write(update.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Error: {ex.Message}]");
            }
            Console.WriteLine();
        }
    }
}
