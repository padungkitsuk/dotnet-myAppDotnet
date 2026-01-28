
namespace MyBackend.Models.Paged
{
    public class PagedResult<T>
    {
        public string Message { get; set; } = "Transaction success.";
        public string Status { get; set; } = "00";
        public int? TotalItems { get; set; }
        public int? PageNo { get; set; }
        public int? PageSize { get; set; }
        public IEnumerable<T> Data { get; set; } = new List<T>();
    }
}