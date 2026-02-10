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

        [JsonPropertyName("buCode")]
        public string? BuCode { get; set; }

        [JsonPropertyName("appointmentDatetime")]
        public string? AppointmentDatetime { get; set; }

        [JsonPropertyName("taskDetail")]
        public string? TaskDetail { get; set; }


        // 2
        [JsonPropertyName("surveyDate")]
        public string? SurveyDate { get; set; }

        [JsonPropertyName("surveyCompanyCode")]
        public string? SurveyCompanyCode { get; set; }

        [JsonPropertyName("surveyCompanyType")]
        public string? SurveyCompanyType { get; set; }

        [JsonPropertyName("surveyLocationRegion")]
        public string? SurveyLocationRegion { get; set; }

        [JsonPropertyName("surveyLocationProvince")]
        public string? SurveyLocationProvince { get; set; }

        [JsonPropertyName("surveyLocationDistrict")]
        public string? SurveyLocationDistrict { get; set; }

        [JsonPropertyName("surveyPrice1")]
        public decimal? SurveyPrice1 { get; set; }

        [JsonPropertyName("surveyPrice2")]
        public decimal? SurveyPrice2 { get; set; }

        [JsonPropertyName("remarkCode")]
        public string? RemarkCode { get; set; }


    }
}