using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyBackend.Models.Inspection
{
    public class InspectionTaskResponse
    {
        [JsonPropertyName("jobId")]
        public string? JobId { get; set; }

        [JsonPropertyName("nextTask")]
        public string? NextTask { get; set; }

        [JsonPropertyName("nextTaskDesc")]
        public string? NextTaskDesc { get; set; }

    
    }
}