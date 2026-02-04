using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MyBackend.Models.Customer;
using MyBackend.Models.Vehicle;

namespace MyBackend.Models.Inspection
{
    public class InspectionTaskDetail
    {
        [JsonPropertyName("task")]
        public string? Task { get; set; }

        [JsonPropertyName("taskDesc")]
        public string? TaskDesc { get; set; }

        [JsonPropertyName("taskCompleteStatus")]
        public string? TaskCompleteStatus { get; set; }

        [JsonPropertyName("taskCompleteStatusDesc")]
        public string? TaskCompleteStatusDesc { get; set; }

        [JsonPropertyName("taskCompleteDate")]
        public string? TaskCompleteDate { get; set; }

        [JsonPropertyName("taskStatus")]
        public string? TaskStatus { get; set; }

        [JsonPropertyName("taskStatusDesc")]
        public string? TaskStatusDesc { get; set; }

        [JsonPropertyName("appointmentDatetime")]
        public string? AppointmentDatetime { get; set; }

        [JsonPropertyName("taskDetail")]
        public string? TaskDetail { get; set; }


    }
}