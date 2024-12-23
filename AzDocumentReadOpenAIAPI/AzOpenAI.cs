using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace AzDocumentReadOpenAIAPI
{
    public class AzOpenAI
    {
        private readonly AzureOpenAIClient _azureClient;
        private readonly string _deploymentName;
        private readonly ILogger<AzOpenAI> _logger;

        // Constructor to initialize the Azure OpenAI Client
        public AzOpenAI(string openAIEndPoint, string openAIKey, string openAIDeploymentName, ILogger<AzOpenAI> logger)
        {
            if (string.IsNullOrEmpty(openAIEndPoint) || string.IsNullOrEmpty(openAIKey) || string.IsNullOrEmpty(openAIDeploymentName))
            {
                throw new ArgumentException("Azure OpenAI environment variables are missing or invalid.");
            }

            var credentials = new AzureKeyCredential(openAIKey);
            _azureClient = new AzureOpenAIClient(new Uri(openAIEndPoint), credentials);
            _deploymentName = openAIDeploymentName;
            _logger = logger;
        }

        /// <summary>
        /// Summarizes the content of a document based on a user's query using Azure OpenAI API.
        /// </summary>
        public async Task<string> SummarizeDocument(string userQuery, string documentContent)
        {
            var prompt = @$"You are a highly intelligent assistant designed to process document content and answer user queries. Your tasks include:
                             1. Summarize the content strictly based on the user's query.
                             2. If no relevant result is found, respond with <div> No relevant information found.</div>
                             3. Extracting relevant content from the provided document based on the user query or question.
                             4. If a set of questions is provided, answer the questions based on the document content.
                             5. Format the response in beautiful HTML. Use only <div> and other elements with proper inline styles. Do not include the <html> or <body> tags.";

            List<ChatMessage> messages = new List<ChatMessage>
            {
                new SystemChatMessage("Document Content : " + documentContent),
                new SystemChatMessage("Prompt : " + prompt),
                new UserChatMessage("User Query : " + userQuery)
            };

            var options = new ChatCompletionOptions
            {
                Temperature = 0,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
                MaxOutputTokenCount = 450
            };

            try
            {
                ChatClient chatClient = _azureClient.GetChatClient(_deploymentName);
                ChatCompletion chatCompletion = await chatClient.CompleteChatAsync(messages, options);
                return chatCompletion.Content[0].Text;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error reading document");
                return $"Error: {ex.Message}";
            }
        }
    }
}
