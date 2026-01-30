
using System.Text.Json.Serialization;

namespace MyBackend.Models.Paged
{
    public class PagedResult<T>
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = "Transaction success.";

        [JsonPropertyName("status")]
        public string Status { get; set; } = "00";

        [JsonPropertyName("pageNo")]
        public int PageNo { get; set; }   

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalRow")]
        public int TotalRow { get; set; }

        [JsonPropertyName("totalPage")]
        public int TotalPage => PageSize > 0 ? (int)Math.Ceiling((double)TotalRow / PageSize) : 0;

        [JsonPropertyName("data")]
        public IEnumerable<T> Data { get; set; } = new List<T>();
    }
}