using System.Net;

namespace TaskManagement.Core.Commons.Responses
{
	public class ServiceResponse<T>
	{
		public T Data { get; set; }

		public bool Success { get; set; }

		public HttpStatusCode StatusCode { get; set; }

		public string Message { get; set; }
	}
}