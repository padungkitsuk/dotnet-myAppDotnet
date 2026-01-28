using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyBackend.Models.Inspection
{
    [Table("inspection_transaction")]
    public class InspectionTransaction
    {
        [Key]
        [JsonPropertyName("jobId")]
        public string jobId { get; set; } = string.Empty;

        [JsonPropertyName("refNo")]
        public string? refNo { get; set; }

        [JsonPropertyName("jobCreateBy")]
        public string? jobCreateBy { get; set; }

        [JsonPropertyName("jobCreateDate")]
        public DateTime? jobCreateDate { get; set; }

        [JsonPropertyName("jobOwner")]
        public string? jobOwner { get; set; }

        [JsonPropertyName("source")]
        public string? source { get; set; }

        [JsonPropertyName("agentCode")]
        public string? agentCode { get; set; }

        [JsonPropertyName("buCode")]
        public string? buCode { get; set; }

        [JsonPropertyName("policyNo")]
        public string? policyNo { get; set; }

        [JsonPropertyName("policyEffectiveDate")]
        public DateTime? policyEffectiveDate { get; set; }

        [JsonPropertyName("customerType")]
        public string? customerType { get; set; }

        [JsonPropertyName("customerFirstName")]
        public string? customerFirstName { get; set; }

        [JsonPropertyName("customerLastName")]
        public string? customerLastName { get; set; }

        [JsonPropertyName("customerPhone")]
        public string? customerPhone { get; set; }

        [JsonPropertyName("paymentInfo")]
        public string? paymentInfo { get; set; }

        [JsonPropertyName("fleetStatus")]
        public string? fleetStatus { get; set; }

        [JsonPropertyName("fleetId")]
        public string? fleetId { get; set; }

        [JsonPropertyName("carType")]
        public string? carType { get; set; }

        [JsonPropertyName("carRedLicense")]
        public string? carRedLicense { get; set; }

        [JsonPropertyName("carPlateNo")]
        public string? carPlateNo { get; set; }

        [JsonPropertyName("carProvince")]
        public string? carProvince { get; set; }

        [JsonPropertyName("carBrand")]
        public string? carBrand { get; set; }

        [JsonPropertyName("carModel")]
        public string? carModel { get; set; }

        [JsonPropertyName("carSubModel")]
        public string? carSubModel { get; set; }

        [JsonPropertyName("chassisNumber")]
        public string? chassisNumber { get; set; }

        [JsonPropertyName("appointmentStatus")]
        public string? appointmentStatus { get; set; }

        [JsonPropertyName("noSurveyStatus")]
        public string? noSurveyStatus { get; set; }

        [JsonPropertyName("noSurveyCode")]
        public string? noSurveyCode { get; set; }

        [JsonPropertyName("noSurveyDesc")]
        public string? noSurveyDesc { get; set; }

        [JsonPropertyName("jobDesc")]
        public string? jobDesc { get; set; }
    }
}