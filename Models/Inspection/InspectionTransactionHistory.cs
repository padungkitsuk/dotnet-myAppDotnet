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
        public DateTime? CreateDate { get; set; }

        [JsonPropertyName("createBy")]
        public string? CreateBy { get; set; }

        [JsonPropertyName("jobStatus")]
        public string? JobStatus { get; set; }

        [JsonPropertyName("jobDesc")]
        public string? JobDesc { get; set; }

    }
}