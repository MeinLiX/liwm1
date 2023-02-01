using Application.Exceptions;
using System.Net;
using Domain.Responses;

namespace webAPI.Middleware;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            string errorId = Guid.NewGuid().ToString();
            var responseModel = RestResponseResult<object>.Error("Error", errorId);

            var response = context.Response;
            response.ContentType = "application/json";

            _logger.LogWarning($"ErrorId {responseModel.error_id}. StackTrace:{Environment.NewLine}{exception?.StackTrace}");

            switch (exception)
            {
                case AppException e:
                    response.StatusCode = responseModel.status_code = (int)e.StatusCode;
                    break;
                case KeyNotFoundException e:
                    response.StatusCode = responseModel.status_code = (int)HttpStatusCode.NotFound;
                    responseModel.message = "Page not found";
                    break;
                case FluentValidation.ValidationException e:
                    response.StatusCode = responseModel.status_code = (int)HttpStatusCode.BadRequest;
                    responseModel.message = "Validation error";

                    Dictionary<string, List<string>> errors = new();
                    foreach (var modelState in e.Errors) //Possible rewrite using linq to object
                    {
                        if (errors.TryGetValue(modelState.PropertyName, out List<string>? value))
                            value.Add(modelState.ErrorMessage);
                        else
                            errors.Add(modelState.PropertyName, new List<string>() { modelState.ErrorMessage });
                    }
                    
                    responseModel.data = errors;
                    break;
                default:
                    response.StatusCode = responseModel.status_code = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            await response.WriteAsJsonAsync<ErrorRestResponseResult<object>>((ErrorRestResponseResult<object>)responseModel);
        }
    }
}
