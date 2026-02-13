using System.Text.Json.Serialization;

namespace MyBackend.Models.Master
{
    public class MasterServiceRequest
    {
        [JsonPropertyName("regionCode")]
        public string? RegionCode { get; set; }

        [JsonPropertyName("provinceCode")]
        public string? ProvinceCode { get; set; }

        [JsonPropertyName("districtCode")]
        public string? DistrictCode { get; set; }

    }
}