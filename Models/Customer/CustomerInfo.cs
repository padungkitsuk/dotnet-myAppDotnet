
using System.Text.Json.Serialization;

namespace MyBackend.Models.Customer
{
    public class CustomerInfo
    {
        [JsonPropertyName("customerType")]
        public string? CustomerType { get; set; }

        [JsonPropertyName("customerFirstName")]
        public string? CustomerFirstName { get; set; }

        [JsonPropertyName("customerLastName")]
        public string? CustomerLastName { get; set; }

        [JsonPropertyName("customerPhone")]
        public string? CustomerPhone { get; set; }

        [JsonPropertyName("paymentInfo")]
        public string? PaymentInfo { get; set; }
    }
}