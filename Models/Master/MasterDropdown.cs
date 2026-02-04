using System.Text.Json.Serialization;

namespace MyBackend.Models.Master
{
    public class MasterDropdown
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("desc")]
        public string? Desc { get; set; }

    }
}