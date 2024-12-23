using Microsoft.AspNetCore.Mvc;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AzDocumentReadOpenAIAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzServicesController : ControllerBase
    {
        private readonly AzAIForm _azAIForm;
        private readonly AzOpenAI _azOpenAI;

        public AzServicesController(AzAIForm azAIForm, AzOpenAI azOpenAI)
        {
            _azAIForm = azAIForm;
            _azOpenAI = azOpenAI;
        }

        // GET: api/<AzServicesController>
        /// <summary>
        /// Handles HTTP GET requests to retrieve a collection of string values.
        /// </summary>
        /// <returns>An IEnumerable of strings containing "value1" and "value2".</returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "API is Working" };
        }

        // POST api/<AzServicesController>/ReadDocument
        /// <summary>
        /// Handles HTTP POST requests to read a document from the request body.
        /// </summary>
        /// <returns>An IActionResult containing the content of the read document.</returns>
        [HttpPost("ReadDocument")]
        public async Task<IActionResult> PostRead()
        {
            try
            {
                string content;

                using (var memoryStream = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    content = await _azAIForm.ReadDocument(memoryStream);
                }
                return new OkObjectResult(content);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // POST api/<AzServicesController>/SummarizeDocument
        /// <summary>
        /// Handles HTTP POST requests to summarize a document.
        /// </summary>
        /// <param name="value">A Summary object containing the query and content to be summarized.</param>
        /// <returns>A string containing the summarized content.</returns>
        [HttpPost("SummarizeDocument")]
        public async Task<IActionResult> PostSummarize([FromBody] Summary value)
        {
            try
            {
                var summary = await _azOpenAI.SummarizeDocument(value.Query, value.Content);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
