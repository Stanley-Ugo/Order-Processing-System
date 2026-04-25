using OrderProcessingSystem.Application.Common.Models;

namespace OrderProcessingSystem.Application.Common
{
    public class Result<T> : Result
    {
        protected Result(bool succeeded, T data, IEnumerable<string> errors)
            : base(succeeded, errors) => Data = data;

        public T Data { get; }

        public static Result<T> Success(T data) => new(true, data, Array.Empty<string>());
        public static new Result<T> Failure(IEnumerable<string> errors) => new(false, default!, errors);
        public static new Result<T> Failure(string error) => new(false, default!, new[] { error });
    }
}
