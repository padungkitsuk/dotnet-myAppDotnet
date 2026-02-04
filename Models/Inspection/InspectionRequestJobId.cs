using System.Text.Json.Serialization;

namespace MyBackend.Models.Inspection
{
    public class InspectionRequestJobId
    {
        [JsonPropertyName("jobId")]
        public string JobId { get; set; } = string.Empty;

    }
}