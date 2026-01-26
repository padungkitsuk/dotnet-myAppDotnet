using System;
using System.Text.Json.Serialization;

namespace MyBackend.Models.Inspection
{
    public class InspectionTransaction
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("recordedDate")]
        public string recordedDate { get; set; } = string.Empty;

        [JsonPropertyName("jobId")]
        public string jobId { get; set; } = string.Empty;

        [JsonPropertyName("jobOwner")]
        public string? jobOwner { get; set; }

        [JsonPropertyName("licensePlate")]
        public string licensePlate { get; set; } = string.Empty;

        [JsonPropertyName("coverageDate")]
        public string coverageDate { get; set; } = string.Empty;

        [JsonPropertyName("firstName")]
        public string firstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string lastName { get; set; } = string.Empty;

        [JsonPropertyName("phoneNumber")]
        public string phoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("source")]
        public string source { get; set; } = string.Empty;

        [JsonPropertyName("agent")]
        public string agent { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string status { get; set; } = string.Empty;

        [JsonPropertyName("remark")]
        public string remark { get; set; } = string.Empty;
    }
}