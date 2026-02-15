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
        public string? JobId { get; set; } = string.Empty;

        [JsonPropertyName("refNo")]
        public string? RefNo { get; set; }

        [JsonPropertyName("jobCreateBy")]
        public string? JobCreateBy { get; set; }

        [JsonPropertyName("jobCreateDate")]
        public string? JobCreateDate { get; set; }

        [JsonPropertyName("jobOwner")]
        public string? JobOwner { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("agentCode")]
        public string? AgentCode { get; set; }

        [JsonPropertyName("buCode")]
        public string? BuCode { get; set; }

        [JsonPropertyName("policyNo")]
        public string? PolicyNo { get; set; }

        [JsonPropertyName("policyEffectiveDate")]
        public string? PolicyEffectiveDate { get; set; }

        [JsonPropertyName("customerType")]
        public string? CustomerType { get; set; }

        [JsonPropertyName("customerFirstName")]
        public string? CustomerFirstName { get; set; }

        [JsonPropertyName("customerLastName")]
        public string? CustomerLastName { get; set; }

        [JsonPropertyName("customerPhone")]
        public string? CustomerPhone { get; set; }

        [JsonPropertyName("paymentInfo")]
        public string? PaymentInfo { get; set; }

        [JsonPropertyName("fleetStatus")]
        public string? FleetStatus { get; set; }

        [JsonPropertyName("fleetId")]
        public string? FleetId { get; set; }

        [JsonPropertyName("carType")]
        public string? CarType { get; set; }

        [JsonPropertyName("carRedLicense")]
        public string? CarRedLicense { get; set; }

        [JsonPropertyName("carPlateNo")]
        public string? CarPlateNo { get; set; }

        [JsonPropertyName("carProvince")]
        public string? CarProvince { get; set; }

        [JsonPropertyName("carProvinceDesc")]
        public string? CarProvinceDesc { get; set; }

        [JsonPropertyName("carBrand")]
        public string? CarBrand { get; set; }

        [JsonPropertyName("carModel")]
        public string? CarModel { get; set; }

        [JsonPropertyName("carSubModel")]
        public string? CarSubModel { get; set; }

        [JsonPropertyName("chassisNumber")]
        public string? ChassisNumber { get; set; }

        [JsonPropertyName("appointmentStatus")]
        public string? AppointmentStatus { get; set; }

        [JsonPropertyName("noSurveyStatus")]
        public string? NoSurveyStatus { get; set; }

        [JsonPropertyName("noSurveyCode")]
        public string? NoSurveyCode { get; set; }

        [JsonPropertyName("noSurveyDesc")]
        public string? NoSurveyDesc { get; set; }

        [JsonPropertyName("jobStatus")]
        public string? JobStatus { get; set; }

        [JsonPropertyName("jobDesc")]
        public string? JobDesc { get; set; }

        [JsonPropertyName("informerFirstName")]
        public string? InformerFirstName { get; set; }

        [JsonPropertyName("informerLastName")]
        public string? InformerLastName { get; set; }

        [JsonPropertyName("informerPhone")]
        public string? InformerPhone { get; set; }

        [JsonPropertyName("informerEmails")]
        public string? InformerEmails { get; set; }



        [JsonPropertyName("taskCode")]
        public string? TaskCode { get; set; }

        [JsonPropertyName("taskDesc")]
        public string? TaskDesc { get; set; }


        
    }
}