using System.Text.Json.Serialization;

namespace MyBackend.Models.Inspection
{
    public class RequestDataInspection
    {
        [JsonPropertyName("startDate")]
        public string? StartDate { get; set; } 

        [JsonPropertyName("endDate")]
        public string? EndDate { get; set; } 

        [JsonPropertyName("jobId")]
        public string? JobId { get; set; }

        [JsonPropertyName("customerFirstName")]
        public string? CustomerFirstName { get; set; }

        [JsonPropertyName("customerLastName")]
        public string? CustomerLastName { get; set; }

        [JsonPropertyName("carPlateNo")]
        public string? CarPlateNo { get; set; }

        [JsonPropertyName("jobStatus")]
        public string? JobStatus { get; set; }

        [JsonPropertyName("chassisNumber")]
        public string? ChassisNumber { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("agentCode")]
        public string? AgentCode { get; set; }


        //
        [JsonPropertyName("pageNo")]
        public int PageNo { get; set; }  = 0; 

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 0;

    }
}