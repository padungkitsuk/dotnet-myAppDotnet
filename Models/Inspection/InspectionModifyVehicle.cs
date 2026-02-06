using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyBackend.Models.Inspection
{
    public class InspectionModifyVehicle
    {
        [JsonPropertyName("jobId")]
        public string? JobId { get; set; }

        [JsonPropertyName("accessoryNo")]
        public int? AccessoryNo { get; set; }

        [JsonPropertyName("accessoryCode")]
        public string? AccessoryCode { get; set; }

        [JsonPropertyName("accessoryDesc")]
        public string? AccessoryDesc { get; set; }

        [JsonPropertyName("accessoryBrand")]
        public string? AccessoryBrand { get; set; }

        [JsonPropertyName("accessoryPrice")]
        public decimal? AccessoryPrice { get; set; }


        /// clone
        public InspectionModifyVehicle Clone()
        {
            return (InspectionModifyVehicle)this.MemberwiseClone();
        }

    }
}