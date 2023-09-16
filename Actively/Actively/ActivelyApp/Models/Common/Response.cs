namespace ActivelyApp.Models.Common
{
    public class Response
    {
        public object? ReturnObject { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
        public ResponseType? Type { get; set; }
    }

    public enum ResponseType
    {
        Normal = 0,
        Error = 1,
        Succes = 2,
    }
}
