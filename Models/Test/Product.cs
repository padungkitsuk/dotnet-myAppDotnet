using System.ComponentModel.DataAnnotations;

namespace MyBackend.Models.Test
{
    public class Product 
    {
        [Key] 
        public int id { get; set; }
        
        [Required] 
        public string name { get; set; } = string.Empty;
        
        public decimal price { get; set; }
    }
}