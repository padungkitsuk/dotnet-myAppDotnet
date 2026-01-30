using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyBackend.Models.Sequence
{
    [Table("running_job")]
    public class RunningJob
    {
        [Key]
        [JsonPropertyName("year")]
        public string? Year { get; set; } 

        [JsonPropertyName("jobCount")]
        public int? JobCount { get; set; }
    }
}