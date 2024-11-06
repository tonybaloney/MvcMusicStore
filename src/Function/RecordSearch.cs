using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace FunctionTrigger
{
    public class RecordSearch(AzureOpenAIClient openAIClient, ILogger<RecordSearch> logger)
    {
        private readonly string _openAIChatDeploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_CHAT_DEPLOYMENT_NAME") ?? "gpt-4o-mini";

        [Function("RecordSearch")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            // Get the query parameter from the request
            string? query = req.Query["query"];

            if (string.IsNullOrEmpty(query))
            {
                return new BadRequestObjectResult("Please pass a query on the query string");
            }

            // Use GPT-4o-mini to keyword prep the search query for FTS
            var chatClient = openAIClient.GetChatClient(_openAIChatDeploymentName);

            var response = chatClient.CompleteChat(
                [
                new SystemChatMessage("You rewrite user queries to help users find a record in an online music store. If you know the name of the record based on the user input, return it in the query. If you don't, return the keywords from the query. If the user enters the query in any language other than English, return the result in English. Only return the name of the record, not the name of the band."),
                new UserChatMessage("The led zeppelin album with the blimp"),
                new AssistantChatMessage("Led Zeppelin IV"),
                new UserChatMessage("Nirvana baby"),
                new AssistantChatMessage("Nevermind"),
                new UserChatMessage("Oasis album wonderwall"),
                new AssistantChatMessage("(What's the Story) Morning Glory?"),
                new UserChatMessage(query),
            ]);

            string keywordQuery = response.Value.Content[0].Text;

            // Log the response
            logger.LogInformation($"Response: {keywordQuery}");

            return new OkObjectResult(keywordQuery);
        }
    }
}
