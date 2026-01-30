using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MyBackend.Models.Customer;
using MyBackend.Models.Vehicle;

namespace MyBackend.Models.Inspection
{
    public class InspectionRequest
    {
        [Key]
        [JsonPropertyName("jobId")]
        public string? JobId { get; set; } = string.Empty;

        [JsonPropertyName("refNo")]
        public string? RefNo { get; set; }

        [JsonPropertyName("jobCreateBy")]
        public string? JobCreateBy { get; set; }

        [JsonPropertyName("jobCreateDate")]
        public DateTime? jobCreateDate { get; set; }

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
        public DateTime? PolicyEffectiveDate { get; set; }

        [JsonPropertyName("customerInfo")]
        public CustomerInfo? CustomerInfo { get; set; }

        [JsonPropertyName("fleetStatus")]
        public string? FleetStatus { get; set; }

        [JsonPropertyName("fleetId")]
        public string? FleetId { get; set; }

        [JsonPropertyName("vehicleInfo")]
        public List<VehicleInfo> VehicleInfo { get; set; } = [];

        [JsonPropertyName("appointmentStatus")]
        public string? AppointmentStatus { get; set; }

        [JsonPropertyName("noSurveyStatus")]
        public string? NoSurveyStatus { get; set; }

        [JsonPropertyName("noSurveyCode")]
        public string? NoSurveyCode { get; set; }

        [JsonPropertyName("noSurveyDesc")]
        public string? NoSurveyDesc { get; set; }

        [JsonPropertyName("jobDesc")]
        public string? JobDesc { get; set; }
    }
}