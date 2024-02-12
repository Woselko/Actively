using ActivelyApp.Models.Common;
using Resources;

namespace ActivelyApp.Middleware
{
	public class ErrorHandlingMiddleware : IMiddleware
	{
		private readonly ILogger<ErrorHandlingMiddleware> _logger;
		public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
		{
			_logger = logger;
		}
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			try
			{
				await next.Invoke(context);
			}
			catch (Exception e)
			{
				_logger.LogError(e, e.Message); //here need to be changed after implementation of nlog 
				context.Response.StatusCode = 500;
				context.Response.ContentType = "application/json";
				var errorDetails = new
				{
					Type = ResponseType.Error,
					Status = Common.Error,
					Message = "Something went wrong",
					IsSuccess = false
				};
				await context.Response.WriteAsJsonAsync(errorDetails);
			};			
		}
	}
}
