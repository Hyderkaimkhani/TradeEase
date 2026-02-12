namespace Domain.Models
{
    public class PaginatedResponseModel<TModel>
    {
        public List<TModel> Model { get; set; } = new List<TModel>();

        public int TotalCount { get; set; }

        public bool IsError { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
