using System.Net;

namespace ProjectBase.Domain.Abstractions
{
    public sealed record Error(string Code, HttpStatusCode statusCode = HttpStatusCode.BadRequest, string? Description = "")
    {
        public static readonly Error None = new(string.Empty);

        public static readonly Error InvalidType = new("INVALID_TYPE", Description: "Invalid type");

        public static readonly Error NullVal = new("NULL_EXCEPTION", Description: "Data is missing!");

        public static readonly Error AWSNotFound = new("AWS_NULL", Description: "AWS Setting is missing!");

        public static readonly Error ConvertedError = new("CONVERTED_EXCEPTION", statusCode: HttpStatusCode.InternalServerError, Description: "Data cannot converted!");

        public static implicit operator Result(Error error) => Result.Failure(error);
    }

    public class Result
    {
        private readonly HttpStatusCode _statusCode;
        private readonly Error? _error;
        public Result(bool isSuccess, Error error, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            if ((isSuccess && error != Error.None) ||
                (!isSuccess && error == Error.None))
            {
                throw new ArgumentException("Invalid error", nameof(error));
            }

            _error = error;
            _statusCode = statusCode;
            IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error Error => !IsSuccess
            ? _error!
            : Error.None;

        public HttpStatusCode StatusCode => IsSuccess
            ? HttpStatusCode.OK
            : _statusCode;

        public static Result Success() => new(true, Error.None);

        public static Result<TValue> Success<TValue>(TValue value)
            => new(value, true, Error.None);

        public static Result Failure(Error error) => new(false, error);

        public static Result<TValue> Failure<TValue>(Error error)
            => new(default, false, error);

        public static implicit operator Result(bool isSuccess) => isSuccess ? Success() : Error.None;
    }
    public class Result<TValue> : Result
    {
        private readonly TValue? _value;
        public Result(
            TValue? value,
            bool isSuccess,
            Error error,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(isSuccess, error)
        {
            _value = value;
        }

        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException();

        public static implicit operator Result<TValue>(TValue? value) =>
            value is not null ? Success(value) : Failure<TValue>(Error.NullVal);

        public static implicit operator Result<TValue>(Error? value) =>
            value is not null ? Failure<TValue>(value) : Failure<TValue>(Error.NullVal);
    }
}
