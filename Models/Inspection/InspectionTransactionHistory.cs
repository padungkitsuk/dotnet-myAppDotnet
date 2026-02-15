using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyBackend.Models.Inspection
{
    [Table("inspection_transaction_history")]
    public class InspectionTransactionHistory
    {
        [JsonPropertyName("jobId")]
        public required string JobId { get; set; }

        [JsonPropertyName("seq")]
        public int? Seq { get; set; }

        [JsonPropertyName("createDate")]
        public string? CreateDate { get; set; }

        [JsonPropertyName("createBy")]
        public string? CreateBy { get; set; }

        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("jobCode")]
        public string? JobCode { get; set; }

        [JsonPropertyName("jobStatus")]
        public string? JobStatus { get; set; }

        [JsonPropertyName("jobDesc")]
        public string? JobDesc { get; set; }

    }
}