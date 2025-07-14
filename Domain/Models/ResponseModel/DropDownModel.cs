using System.Text.Json.Serialization;

namespace Domain.Models.ResponseModel
{
    public class DropDownModel
    {
        [JsonPropertyName("value")]
        public int Key { get; set; }

        [JsonPropertyName("label")]
        public string Value { get; set; } = string.Empty;
    }
}
