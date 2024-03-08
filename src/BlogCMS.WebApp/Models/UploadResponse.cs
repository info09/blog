using System.Text.Json.Serialization;

namespace BlogCMS.WebApp.Models
{
    public class UploadResponse
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }
    }
}
