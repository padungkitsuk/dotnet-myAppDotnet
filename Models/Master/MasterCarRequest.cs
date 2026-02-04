using System.Text.Json.Serialization;

namespace MyBackend.Models.Master
{
    public class MasterCarRequest
    {
        [JsonPropertyName("carBrand")]
        public string CarBrand { get; set; } = string.Empty;

    }
}