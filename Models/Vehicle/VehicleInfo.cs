
using System.Text.Json.Serialization;

namespace MyBackend.Models.Vehicle
{
    public class VehicleInfo
    {
        [JsonPropertyName("carType")]
        public string? CarType { get; set; }

        [JsonPropertyName("carRedLicense")]
        public string? CarRedLicense { get; set; }

        [JsonPropertyName("carPlateNo")]
        public string? CarPlateNo { get; set; }

        [JsonPropertyName("carProvince")]
        public string? CarProvince { get; set; }

        [JsonPropertyName("carBrand")]
        public string? CarBrand { get; set; }

        [JsonPropertyName("carModel")]
        public string? CarModel { get; set; }

        [JsonPropertyName("carSubModel")]
        public string? CarSubModel { get; set; }

        [JsonPropertyName("chassisNumber")]
        public string? ChassisNumber { get; set; }

        [JsonPropertyName("jobId")]
        public string? JobId { get; set; }

    }
}