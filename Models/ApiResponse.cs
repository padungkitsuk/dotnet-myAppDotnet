using System.Text.Json.Serialization;

namespace MyBackend.Models;

public class ApiResponse<T>
{
    public string Message { get; set; } = "Transaction success.";
    public string Status { get; set; } = "00";
    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }
}