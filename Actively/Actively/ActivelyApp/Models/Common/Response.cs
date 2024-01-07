namespace ActivelyApp.Models.Common
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public object? Content { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
        public ResponseType? Type { get; set; }
    }

    public enum ResponseType
    {
        Normal = 0,
        Error = 1,
        Success = 2,
    }
}
