namespace OrderProcessingSystem.Application.Common.Models
{
    public class Result
    {
        protected Result(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
        }

        public bool Succeeded { get; }
        public string[] Errors { get; }

        public static Result Success() => new(true, Array.Empty<string>());
        public static Result Failure(IEnumerable<string> errors) => new(false, errors);
        public static Result Failure(string error) => new(false, new[] { error });
    }
}
