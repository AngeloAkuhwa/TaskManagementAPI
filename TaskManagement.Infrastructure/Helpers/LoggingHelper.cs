using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TaskManagement.Infrastructure.Helpers;

public class LoggingHelper
{
	public static void LogInformation<T>(ILogger<T> logger, string nameOfMethod, string information)
	{
		logger.LogInformation("Information: {Method} - {Information}", nameOfMethod, information);
	}

	public static void LogInformation<T>(ILogger<T> logger, HttpRequest request, string information)
	{
		logger.LogInformation("Information: {Method} {Path} - {Information}", request.Method, request.Path.Value, information);
	}

	public static void LogError<T>(ILogger<T> logger, string nameOfMethod, string error)
	{
		logger.LogError("Error: {Method} - {Error}", nameOfMethod, error);
	}

	public static void LogError<T>(ILogger<T> logger, HttpRequest request, string error)
	{
		logger.LogError("Error: {Method} {Path} - {Error}", request.Method, request.Path.Value, error);
	}

	public static void LogErrorAndThrowException<T>(ILogger<T> logger, string nameOfMethod, string error)
	{
		logger.LogError("Error: {Method} - {Error}", nameOfMethod, error);
		throw new Exception(error);
	}

	public static void LogErrorAndThrowException<T>(ILogger<T> logger, HttpRequest request, string error)
	{
		logger.LogError("Error: {Method} {Path} - {Error}", request.Method, request.Path.Value, error);
		throw new Exception(error);
	}
}