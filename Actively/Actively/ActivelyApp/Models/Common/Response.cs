namespace ActivelyApp.Models.Common
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public object? Content { get; set; }
    }
}
