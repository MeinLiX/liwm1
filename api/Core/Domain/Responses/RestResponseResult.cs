namespace Domain.Responses;

public class RestResponseResult : IRestResponseResult
{
    public required bool is_success { get; set; }
    public DateTime date            { get; set; } = DateTime.Now;
    public required int status_code { get; set; }
    public string message           { get; set; } = string.Empty;

    public static IRestResponseResult Success(int statusCode = 200)
        => new RestResponseResult() { is_success = true, status_code = statusCode };

    public static IRestResponseResult Success(string message, int statusCode = 200)
        => new RestResponseResult() { message = message, is_success = true, status_code = statusCode };

    public static IErrorRestResponseResult<object> Error(string message, string errorId, int statusCode = 400)
        => new ErrorRestResponseResult<object>() { message = message, error_id = errorId, is_success = false, status_code = statusCode };

    public static IRestResponseResult Fail(int statusCode = 400)
        => new RestResponseResult() { is_success = false, status_code = statusCode };

    public static IRestResponseResult Fail(string message, int statusCode = 400)
        => new RestResponseResult() { message = message, is_success = false, status_code = statusCode };
}

public class RestResponseResult<T> : RestResponseResult, IRestResponseResult<T>
       where T : class
{
    public T? data { get; set; }

    public static IRestResponseResult<T> Success(T data, int statusCode = 200)
        => new RestResponseResult<T> { status_code = statusCode, is_success = true, data = data };

    public static IRestResponseResult<T> Success(T data, string message, int statusCode = 200)
        => new RestResponseResult<T> { data = data, message = message, is_success = true, status_code = statusCode };

    public static new IRestResponseResult<T> Success(string message, int statusCode = 200)
        => new RestResponseResult<T> { message = message, is_success = true, status_code = statusCode };

    public static new IErrorRestResponseResult<T> Error(string message, string errorId, int statusCode = 400)
        => new ErrorRestResponseResult<T>() { message = message, error_id = errorId, is_success = false, status_code = statusCode };

    public static new IRestResponseResult<T> Fail(int statusCode = 400)
        => new RestResponseResult<T>() { is_success = false, status_code = statusCode };

    public static new IRestResponseResult<T> Fail(string message, int statusCode = 400)
        => new RestResponseResult<T>() { message = message, is_success = false, status_code = statusCode };
}


public class ErrorRestResponseResult<T> : RestResponseResult<T>, IErrorRestResponseResult<T>
    where T : class
{
    public required string error_id { get; set; }
}

