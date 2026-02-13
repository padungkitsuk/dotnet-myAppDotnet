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

        [JsonPropertyName("carModification")]
        public string? CarModification { get; set; }

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
        public List<InspectionModifyVehicle?>? ModifyVehicleList { get; set; }



        /// remark code
        [JsonPropertyName("remarkCode")]
        public string? RemarkCode { get; set; }


    }
}