using System;
using System.Text.Json.Serialization;

namespace MyBackend.Models.Inspection
{
    public class InspectionDetail
    {
        [JsonPropertyName("jobId")]
        public string? jobId { get; set; }

        [JsonPropertyName("createDate")]
        public string? createDate { get; set; }

        [JsonPropertyName("jobStatus")]
        public string? jobStatus { get; set; }

        [JsonPropertyName("jobInformation")]
        public InspectionInformation? jobInformation { get; set; } //= new();
    }

    public class InspectionInformation
    {
        [JsonPropertyName("source")]
        public string? source { get; set; }

        [JsonPropertyName("agent")]
        public string? agent { get; set; }

        [JsonPropertyName("customerType")]
        public string? customerType { get; set; }

        [JsonPropertyName("firstName")]
        public string? firstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? lastName { get; set; }

        [JsonPropertyName("mobileNumber")]
        public string? mobileNumber { get; set; }

        [JsonPropertyName("companyName")]
        public string? companyName { get; set; }

        [JsonPropertyName("extensionNumber")]
        public string? extensionNumber { get; set; }

        [JsonPropertyName("notificationEmails")]
        public List<string?>? notificationEmails { get; set; } = new();

        [JsonPropertyName("licensePlate")]
        public string? licensePlate { get; set; }

        [JsonPropertyName("province")]
        public string? province { get; set; }

        [JsonPropertyName("carBrand")]
        public string? carBrand { get; set; }

        [JsonPropertyName("carModel")]
        public string? carModel { get; set; }

        [JsonPropertyName("otherModel")]
        public string? otherModel { get; set; }

        [JsonPropertyName("vinNumber")]
        public string? vinNumber { get; set; }

        [JsonPropertyName("coverageStartDate")]
        public string? coverageStartDate { get; set; }

        public string? remark { get; set; }
    }
}