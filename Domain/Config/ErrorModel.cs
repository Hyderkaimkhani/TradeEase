using System.Text.Json;

namespace Domain.Config
{
    public class ErrorModel
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public ErrorModel()
        {
            ErrorMessage = string.Empty;
        }
    }
}
