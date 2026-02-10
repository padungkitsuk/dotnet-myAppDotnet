
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyBackend.Models.Vehicle
{
    public class VehicleInfo
    {
        [StringLength(1, ErrorMessage = "CarType must be 1 character.")]
        [JsonPropertyName("carType")]
        public string? CarType { get; set; }

        [StringLength(1, ErrorMessage = "CarRedLicense must be 1 character (Y/N).")]
        [JsonPropertyName("carRedLicense")]
        public string? CarRedLicense { get; set; }

        [Required(ErrorMessage = "CarPlateNo is required.")]
        [StringLength(20, ErrorMessage = "CarPlateNo cannot exceed 20 characters.")]
        [JsonPropertyName("carPlateNo")]
        public string? CarPlateNo { get; set; }

        [Required(ErrorMessage = "CarProvince is required.")]
        [StringLength(20, ErrorMessage = "CarProvince cannot exceed 20 characters.")]
        [JsonPropertyName("carProvince")]
        public string? CarProvince { get; set; }

        [JsonPropertyName("carProvinceDesc")]
        public string? CarProvinceDesc { get; set; }

        [StringLength(20, ErrorMessage = "CarBrand cannot exceed 20 characters.")]
        [JsonPropertyName("carBrand")]
        public string? CarBrand { get; set; }

        [StringLength(20, ErrorMessage = "CarModel cannot exceed 20 characters.")]
        [JsonPropertyName("carModel")]
        public string? CarModel { get; set; }

        [StringLength(20, ErrorMessage = "CarSubModel cannot exceed 20 characters.")]
        [JsonPropertyName("carSubModel")]
        public string? CarSubModel { get; set; }

        [Required(ErrorMessage = "ChassisNumber is required.")]
        [StringLength(20, ErrorMessage = "ChassisNumber cannot exceed 20 characters.")]
        [JsonPropertyName("chassisNumber")]
        public string? ChassisNumber { get; set; }

        // Field สำหรับรับค่ากลับจาก Database (ถ้ามี)
        [JsonPropertyName("jobId")]
        public string? JobId { get; set; }

    }
}