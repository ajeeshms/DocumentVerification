using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

// For DOCX
using DocumentFormat.OpenXml.Packaging;

// For PDF (using PdfPig)
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

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

        string text = string.Empty;
        var ext = Path.GetExtension(file.FileName).ToLower();

        if (ext == ".txt") {
            using var reader = new StreamReader(file.OpenReadStream());
            text = await reader.ReadToEndAsync();
        }

        else if (ext == ".pdf") {
            using var pdf = PdfDocument.Open(file.OpenReadStream());
            var sb = new StringBuilder();
            foreach (var page in pdf.GetPages()) {
                sb.AppendLine(page.Text);
            }
            text = sb.ToString();
        }
        else if (ext == ".docx") {
            using var doc = WordprocessingDocument.Open(file.OpenReadStream(), false);
            var body = doc.MainDocumentPart.Document.Body;
            text = body.InnerText;
        }
        else {
            return BadRequest("Unsupported file type. Only TXT, PDF, DOCX allowed.");
        }

        var chunks = ChunkText(text);
        var results = new List<string>();

        foreach (var chunk in chunks) {
            var requestBody = new {
                //model = "qwen2.5-7b-instruct",
                model = "qwen/qwen3-4b-2507",
                temperature = 0,
                messages = new[] {
        new {
            role = "system",
            content =
@"You are an AI that detects sensitive and confidential information in documents.

Confidential data includes only:
- Personally identifiable information (PII): names, addresses, phone numbers, government IDs
- Financial information: credit card numbers, bank details, tax IDs
- Login credentials: usernames, passwords, API keys, tokens
- Proprietary or classified company data

Ignore and mark as SAFE if:
- Text is Lorem Ipsum, filler, or placeholder text
- Strings only look like emails but do not contain real domains (e.g. 'dolor.sit@amet..')
- Random Latin text, fake names, or nonsense words

If you find confidential data, reply briefly with the type (e.g., 'Email address', 'Credit card number').  
If no confidential data is found, reply only with 'SAFE'."
        },
        new { role = "user", content = chunk }
    }
            };



            var response = await _httpClient.PostAsJsonAsync("v1/chat/completions", requestBody);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var docJson = JsonDocument.Parse(json);
            var reply = docJson.RootElement
                               .GetProperty("choices")[0]
                               .GetProperty("message")
                               .GetProperty("content")
                               .GetString();

            results.Add(reply);

            if (!reply.Equals("SAFE", StringComparison.OrdinalIgnoreCase)) {
                results.Add("Confidential data detected: " + reply);
                break;
            }
        }

        return Ok(string.Join("\n", results));
    }



    private static List<string> ChunkText(string text, int maxChunkSize = 2000) {
        var chunks = new List<string>();
        for (int i = 0; i < text.Length; i += maxChunkSize) {
            chunks.Add(text.Substring(i, Math.Min(maxChunkSize, text.Length - i)));
        }
        return chunks;
    }

}
