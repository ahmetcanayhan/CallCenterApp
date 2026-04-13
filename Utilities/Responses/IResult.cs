namespace Utilities.Responses
{
    public interface IResult
    {
        bool IsSuccess { get; }
        IEnumerable<string> Messages { get; }
    }

    public class Result : IResult
    {
        public bool IsSuccess { get; }
        public IEnumerable<string> Messages { get; } = [];

        private Result(bool success)
        {
            IsSuccess = success;
        }

        private Result(bool success, IEnumerable<string> messages) : this(success)
        {
            Messages = messages;
        }

        public static IResult Success(string message) =>  new Result (true, [message]);
        public static IResult Success() =>  new Result (true);
        public static IResult Fail(IEnumerable<string> messages) => new Result (false, messages);
    }
}
