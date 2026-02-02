using System;
using System.Text.Json.Serialization;

namespace MyBackend.Models.Inspection
{
   public class InspectionTransactionDetail
    {
        [JsonPropertyName("jobList")]
        public List<JobList>? JobList { get; set; }

        [JsonPropertyName("detail")]
        public InspectionTransaction? Detail { get; set; }

    }

    public class JobList
    {
        [JsonPropertyName("jobId")]
        public string? JobId { get; set; }

        [JsonPropertyName("carPlateNo")]
        public string? CarPlateNo { get; set; }

        [JsonPropertyName("job")]
        public string Job => !string.IsNullOrEmpty(JobId) 
            ? $"{JobId} -{CarPlateNo ?? "N/A"}" 
            : "";
    }
}