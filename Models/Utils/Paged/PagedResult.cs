
using System.Text.Json.Serialization;

namespace MyBackend.Models.Utils.Paged
{
    public class PagedResult<T>
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = "Transaction success.";

        [JsonPropertyName("status")]
        public string Status { get; set; } = "00";

        [JsonPropertyName("pagination")]
        public Pagination Pagination { get; set; } = new ();

        [JsonPropertyName("data")]
        public T? Data { get; set; } 
        //public IEnumerable<T> Data { get; set; } = [];
    }

    public class Pagination
    {
        [JsonPropertyName("pageNo")]
        public int PageNo { get; set; }   

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalRow")]
        public int TotalRow { get; set; }

        [JsonPropertyName("totalPage")]
        public int TotalPage => PageSize > 0 ? (int)Math.Ceiling((double)TotalRow / PageSize) : 0;
    }
}