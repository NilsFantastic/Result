namespace AlmostFantastic.Result
{
    public class Error
    {
        public readonly Guid Id;
        public readonly int Code;
        public readonly string Message;
        public readonly Exception? CaughtException;
        public readonly ErrorType ErrorType;

        public Error(ErrorType errorType, int code, string message, Exception caughtException = null)
        {
            ErrorType = errorType;
            Id = Guid.NewGuid();
            Code = code;
            Message = message;
            CaughtException = caughtException;
        }
        public static Task<Result<TValue, Error>> AsResultTask<TValue>(ErrorType errorType, int code, string message,
            Exception caughtException = null)
        {
            return Task.FromResult(AsResult<TValue>(errorType, code, message, caughtException));
        }
        public static Result<TValue, Error> AsResult<TValue>(ErrorType errorType, int code, string message,
            Exception caughtException = null)
        {
            return new Result<TValue, Error>(new Error(errorType, code, message, caughtException));
        }

        public static VoidResult<Error> AsResult(ErrorType errorType, int code, string message,
            Exception caughtException = null)
        {
            return new VoidResult<Error>(new Error(errorType, code, message, caughtException));
        }
        public static VoidResult<Error> AsResult()
        {
            return new VoidResult<Error>();
        }
    }
    public enum ErrorType
    {
        NotFound = default, // Resource or item not found
        Input,             // Malformed input or validation error
        Unexpected,        // Specific unexpected errors
        Unhandled,         // Caught by middleware, not handled elsewhere
        Conflict,          // Conflicts, often in concurrent operations
        RateLimit,         // Rate limit exceeded, too many requests
        Server,            // Server-side errors (internal server error)
        Authentication,    // Authentication or authorization issues
        Timeout,           // Timeout errors
        Dependency,        // Issues related to external dependencies/services
        BusinessLogic,     // Business logic or domain-specific errors
        NetworkFailure,
        // Additional categories can be added as needed
    }
}
