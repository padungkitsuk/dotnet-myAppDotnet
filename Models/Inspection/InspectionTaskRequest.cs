using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MyBackend.Models.Customer;
using MyBackend.Models.Vehicle;

namespace MyBackend.Models.Inspection
{
    public class InspectionTaskRequest
    {
        [JsonPropertyName("task")]
        public string? Task { get; set; }

        [MinLength(1, ErrorMessage = "At least one jobList is required.")]
        [JsonPropertyName("jobList")]
        public List<string> JobList { get; set; } = [];

        [JsonPropertyName("jobId")]
        public string? JobId { get; set; }

        [JsonPropertyName("round")]
        public string? Round { get; set; }

        [JsonPropertyName("taskDesc")]
        public string? TaskDesc { get; set; }

        [JsonPropertyName("taskCompleteStatus")]
        public string? TaskCompleteStatus { get; set; }

        [JsonPropertyName("taskCompleteDate")]
        public string? TaskCompleteDate { get; set; }

        [JsonPropertyName("taskStatus")]
        public string? TaskStatus { get; set; }

        [JsonPropertyName("appointmentDatetime")]
        public string? AppointmentDatetime { get; set; }

        [JsonPropertyName("taskDetail")]
        public string? TaskDetail { get; set; }

        [JsonPropertyName("taskCreateBy")]
        public string? TaskCreateBy { get; set; }

        [JsonPropertyName("action")]
        public string? Action { get; set; }

        //clone
        public InspectionTaskRequest Clone()
        {
            return (InspectionTaskRequest)this.MemberwiseClone();
        }

    }
}