using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        [JsonPropertyName("taskCompleteBy")]
        public string? TaskCompleteBy { get; set; }

        [JsonPropertyName("taskStatus")]
        public string? TaskStatus { get; set; }

        [JsonPropertyName("appointmentDatetime")]
        public string? AppointmentDatetime { get; set; }

        [JsonPropertyName("taskDetail")]
        public string? TaskDetail { get; set; }

        [JsonPropertyName("taskCreateBy")]
        public string? TaskCreateBy { get; set; }



        /// add request for => task002
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


        /// add request for => task004
        [JsonPropertyName("resultReport")]
        public string? ResultReport { get; set; }

        [JsonPropertyName("verifyResultDatetime")]
        public string? VerifyResultDatetime { get; set; }

        [JsonPropertyName("mileNumber")]
        public string? MileNumber { get; set; }

        [JsonPropertyName("inspectionDatetime")]
        public string? InspectionDatetime { get; set; }

        [JsonPropertyName("carInspectionResult")]
        public string? CarInspectionResult { get; set; }

        [JsonPropertyName("carType")]
        public string? CarType { get; set; }

        [JsonPropertyName("spare")]
        public string? Spare { get; set; }

        [JsonPropertyName("gas")]
        public string? Gas { get; set; }

        [JsonPropertyName("gasNumber")]
        public int? GasNumber { get; set; }

        [JsonPropertyName("gasType")]
        public string? GasType { get; set; }

        [JsonPropertyName("gasPrice")]
        public decimal? GasPrice { get; set; }

        [JsonPropertyName("modifyVehicle")]
        public string? ModifyVehicle { get; set; }

        /// if modifyVehicle => Y
        [JsonPropertyName("modifyVehicleList")]
        public List<InspectionModifyVehicle>? ModifyVehicleList { get; set; } = [];



        /// remark code
        [JsonPropertyName("remarkCode")]
        public string? RemarkCode { get; set; }



        /// action
        [JsonPropertyName("action")]
        public string? Action { get; set; }


        /// clone
        public InspectionTaskRequest Clone()
        {
            return (InspectionTaskRequest)this.MemberwiseClone();
        }

    }
}