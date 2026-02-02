
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyBackend.Models.Customer
{
    public class CustomerInfo
    {
        [StringLength(1, ErrorMessage = "CustomerType must be 1 character.")]
        [JsonPropertyName("customerType")]
        public string? CustomerType { get; set; }

        [StringLength(200, ErrorMessage = "CustomerFirstName cannot exceed 200 characters.")]
        [JsonPropertyName("customerFirstName")]
        public string? CustomerFirstName { get; set; }

        [StringLength(200, ErrorMessage = "CustomerLastName cannot exceed 200 characters.")]
        [JsonPropertyName("customerLastName")]
        public string? CustomerLastName { get; set; }

        [StringLength(200, ErrorMessage = "CustomerPhone cannot exceed 200 characters.")]
        [JsonPropertyName("customerPhone")]
        public string? CustomerPhone { get; set; }

        [JsonPropertyName("paymentInfo")]
        public string? PaymentInfo { get; set; }
    }
}