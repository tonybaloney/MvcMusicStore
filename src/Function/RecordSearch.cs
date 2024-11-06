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
        private readonly string _openAIChatDeploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_CHAT_DEPLOYMENT_NAME") ?? "chat";

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
                new SystemChatMessage("You rewrite user queries into SQL Server FTS syntax by extracting key terms. Do not return a SQL statement, just return the contents of the phrases or terms. If the user writes a query in any language other than English, rewrite it in English."),
                new UserChatMessage("The led zeppelin album with the blimp"),
                new AssistantChatMessage("Led Zeppelin IV"),
                new UserChatMessage(query),
            ]);

            string keywordQuery = response.Value.Content[0].Text;

            // Log the response
            logger.LogInformation($"Response: {keywordQuery}");

            return new OkObjectResult(keywordQuery);
        }
    }
}
