using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Extensions.Logging;

namespace AzDocumentReadOpenAIAPI
{
    public class AzAIForm
    {

        private readonly DocumentAnalysisClient _client;
        private readonly ILogger<AzAIForm> _logger;

        public AzAIForm(DocumentAnalysisClient client, ILogger<AzAIForm> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Reads the content of a document from the provided stream using Azure Cognitive Services.
        /// </summary>
        /// <param name="stream">The stream containing the document to be read.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the content of the document as a string.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during document analysis.</exception>
        public async Task<string> ReadDocument(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            try
            {
                AnalyzeDocumentOperation operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-read", stream);
                return operation.Value.Content;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error reading document");
                return "An error occurred while reading the document. " + ex.Message;
            }
        }
    }
}
