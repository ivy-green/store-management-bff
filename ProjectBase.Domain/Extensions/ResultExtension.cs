using ProjectBase.Domain.Abstractions;

namespace ProjectBase.Domain.Extensions
{
    public static class ResultExtension
    {
        public static Result<T> Ensure<T>(
            this Result<T> result,
            Func<T, bool> predicate,
            Error error)
        {
            if (result.IsFailure)
            {
                return result;
            }

            return predicate(result.Value)
                ? result
                : Result.Failure<T>(error);
        }
    }
}
