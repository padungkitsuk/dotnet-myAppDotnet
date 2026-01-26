using System;
using System.Text.Json.Serialization;

namespace MyBackend.Models.Inspection
{
    public class InspectionDetail
    {
        [JsonPropertyName("jobId")]
        public string jobId { get; set; } = string.Empty;

        [JsonPropertyName("createDate")]
        public string createDate { get; set; } = string.Empty;

        [JsonPropertyName("jobStatus")]
        public string jobStatus { get; set; } = string.Empty;

        [JsonPropertyName("jobInformation")]
        public InspectionInformation jobInformation { get; set; } = new();
    }

    public class InspectionInformation
    {
        [JsonPropertyName("source")]
        public string source { get; set; } = string.Empty;

        [JsonPropertyName("agent")]
        public string agent { get; set; } = string.Empty;

        [JsonPropertyName("customerType")]
        public string customerType { get; set; } = string.Empty;

        [JsonPropertyName("firstName")]
        public string firstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string lastName { get; set; } = string.Empty;

        [JsonPropertyName("mobileNumber")]
        public string mobileNumber { get; set; } = string.Empty;

        [JsonPropertyName("companyName")]
        public string companyName { get; set; } = string.Empty;

        [JsonPropertyName("extensionNumber")]
        public string extensionNumber { get; set; } = string.Empty;

        [JsonPropertyName("notificationEmails")]
        public List<string> notificationEmails { get; set; } = new();

        [JsonPropertyName("licensePlate")]
        public string licensePlate { get; set; } = string.Empty;

        [JsonPropertyName("province")]
        public string province { get; set; } = string.Empty;

        [JsonPropertyName("carBrand")]
        public string carBrand { get; set; } = string.Empty;

        [JsonPropertyName("carModel")]
        public string carModel { get; set; } = string.Empty;

        [JsonPropertyName("otherModel")]
        public string otherModel { get; set; } = string.Empty;

        [JsonPropertyName("vinNumber")]
        public string vinNumber { get; set; } = string.Empty;

        [JsonPropertyName("coverageStartDate")]
        public string coverageStartDate { get; set; } = string.Empty;
    }
}