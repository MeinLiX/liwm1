using System;
namespace Domain.Requests;

public interface IRestResponseResult
{
    public bool is_success { get; set; }
    public DateTime date { get; set; }
    public int status_code { get; set; }
    public string message { get; set; }
}

public interface IRestResponseResult<T> : IRestResponseResult
    where T : class
{
    public T? data { get; set; }
}

public interface IErrorRestResponseResult
{
    public string error_id { get; set; }
}

public interface IErrorRestResponseResult<T> : IErrorRestResponseResult, IRestResponseResult<T>
    where T : class
{
}

