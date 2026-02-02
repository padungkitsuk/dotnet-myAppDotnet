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

        [StringLength(20, ErrorMessage = "RefNo must not exceed 20 characters.")]
        [JsonPropertyName("refNo")]
        public string? RefNo { get; set; }

        [StringLength(100, ErrorMessage = "JobCreateBy cannot exceed 100 characters.")]
        [JsonPropertyName("jobCreateBy")]
        public string? JobCreateBy { get; set; }

        [JsonPropertyName("jobCreateDate")]
        public DateTime? jobCreateDate { get; set; }

        [StringLength(100, ErrorMessage = "JobOwner cannot exceed 100 characters.")]
        [JsonPropertyName("jobOwner")]
        public string? JobOwner { get; set; }

        [StringLength(50, ErrorMessage = "Source cannot exceed 50 characters.")]
        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [StringLength(50, ErrorMessage = "AgentCode cannot exceed 50 characters.")]
        [JsonPropertyName("agentCode")]
        public string? AgentCode { get; set; }

        [StringLength(50, ErrorMessage = "BuCode cannot exceed 50 characters.")]
        [JsonPropertyName("buCode")]
        public string? BuCode { get; set; }

        [StringLength(50, ErrorMessage = "PolicyNo cannot exceed 50 characters.")]
        [JsonPropertyName("policyNo")]
        public string? PolicyNo { get; set; }

        [JsonPropertyName("policyEffectiveDate")]
        public DateTime? PolicyEffectiveDate { get; set; }

        [Required(ErrorMessage = "CustomerInfo is required.")]
        [JsonPropertyName("customerInfo")]
        public CustomerInfo? CustomerInfo { get; set; }

        [StringLength(1, ErrorMessage = "FleetStatus must be 1 character (Y/N).")]
        [JsonPropertyName("fleetStatus")]
        public string? FleetStatus { get; set; }

        [StringLength(50, ErrorMessage = "FleetId cannot exceed 50 characters.")]
        [JsonPropertyName("fleetId")]
        public string? FleetId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one vehicleInfo is required.")]
        [JsonPropertyName("vehicleInfo")]
        public List<VehicleInfo> VehicleInfo { get; set; } = [];

        [StringLength(1, ErrorMessage = "AppointmentStatus must be 1 character.")]
        [JsonPropertyName("appointmentStatus")]
        public string? AppointmentStatus { get; set; }

        [StringLength(1, ErrorMessage = "NoSurveyStatus must be 1 character.")]
        [JsonPropertyName("noSurveyStatus")]
        public string? NoSurveyStatus { get; set; }

        [StringLength(3, ErrorMessage = "NoSurveyCode cannot exceed 3 characters.")]
        [JsonPropertyName("noSurveyCode")]
        public string? NoSurveyCode { get; set; }

        [JsonPropertyName("noSurveyDesc")]
        public string? NoSurveyDesc { get; set; }

        [StringLength(20, ErrorMessage = "JobStatus cannot exceed 20 characters.")]
        [JsonPropertyName("jobStatus")]
        public string? JobStatus { get; set; }

        [JsonPropertyName("jobDesc")]
        public string? JobDesc { get; set; }
    }
}