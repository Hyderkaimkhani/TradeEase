namespace Domain.Models
{
    public class ResponseModel<TModel>
    {
        public bool IsError { get; set; }

        public string Message { get; set; } = string.Empty;

        public TModel Model { get; set; }
    }
}
