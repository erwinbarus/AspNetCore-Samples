using System.Text.Json.Serialization;

namespace ApiCall;

public class Repository
{
    public int Id { get; set; }
    [JsonPropertyName("full_name")]
    public string? FullName { get; set; }
    public bool Private { get; set; }
}