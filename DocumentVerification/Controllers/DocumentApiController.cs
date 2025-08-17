using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class DocumentApiController : ControllerBase {
    private readonly HttpClient _httpClient;

    public DocumentApiController(IHttpClientFactory httpClientFactory) {
        _httpClient = httpClientFactory.CreateClient("LMStudio");
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze(IFormFile file) {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        string text;
        using (var reader = new StreamReader(file.OpenReadStream())) {
            text = await reader.ReadToEndAsync();
        }

        var chunks = ChunkText(text);
        var results = new List<string>();

        foreach (var chunk in chunks) {
            var requestBody = new {
                model = "qwen2.5-7b-instruct",
                temperature = 0,
                messages = new[]
                {
                    new { role = "system", content = "You are an AI that detects confidential information in documents. If you find any, reply very briefly with the type of confidential data. If none, reply only 'SAFE'." },
                    new { role = "user", content = chunk }
                }
            };


            var response = await _httpClient.PostAsJsonAsync("v1/chat/completions", requestBody);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // Deserialize the response
            using var doc = JsonDocument.Parse(json);
            var reply = doc.RootElement
                           .GetProperty("choices")[0]
                           .GetProperty("message")
                           .GetProperty("content")
                           .GetString();

            results.Add(reply);

            // Stop further processing if confidential data is detected
            if (!reply.Equals("SAFE", StringComparison.OrdinalIgnoreCase)) {
                results.Add("Confidential data detected: " + reply);
                break;
            }

        }

        var finalSummary = string.Join("\n", results);
        return Ok(finalSummary);
    }


    private static List<string> ChunkText(string text, int maxChunkSize = 2000) {
        var chunks = new List<string>();
        for (int i = 0; i < text.Length; i += maxChunkSize) {
            chunks.Add(text.Substring(i, Math.Min(maxChunkSize, text.Length - i)));
        }
        return chunks;
    }

}
