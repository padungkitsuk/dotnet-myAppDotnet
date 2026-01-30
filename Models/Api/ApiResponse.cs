using System.Text.Json.Serialization;

namespace MyBackend.Models.Api;

public class ApiResponse<T>
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = "Transaction success.";

    [JsonPropertyName("status")]
    public string Status { get; set; } = "00";

    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}