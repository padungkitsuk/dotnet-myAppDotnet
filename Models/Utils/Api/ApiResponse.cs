using System.Text.Json.Serialization;
using MyBackend.Utils.Constants;

namespace MyBackend.Models.Utils.Api;

public class ApiResponse<T>
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = StatusConstant.SuccessMessage;

    [JsonPropertyName("status")]
    public string Status { get; set; } = StatusConstant.SuccessCode;

    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}