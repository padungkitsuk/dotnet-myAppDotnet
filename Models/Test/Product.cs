using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyBackend.Models.Test
{
    public class Product 
    {
        [Key] 
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [Required] 
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}