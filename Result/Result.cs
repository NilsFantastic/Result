using System.Diagnostics.CodeAnalysis;

namespace AlmostFantastic.Result
{
    /// <summary>
    /// Represents a highly performant result object that avoids branch predictions and heap allocations.
    /// Used to encapsulate the outcome of an operation that can either succeed with a value or fail with an error.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error (must be a reference type).</typeparam>
    public readonly struct Result<TValue, TError> where TError : class
    {
        /// <summary>
        /// The success value of the operation, if it was successful.
        /// </summary>
        public readonly TValue? Value;

        /// <summary>
        /// The error associated with the operation, if it failed.
        /// </summary>
        public readonly TError? Error;

        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Value))]
        public bool IsSuccess { get; }

        /// <summary>
        /// Indicates whether the operation resulted in an error.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Error))]
        public bool IsError { get; }

        /// <summary>
        /// Initializes a new successful result containing the specified value.
        /// </summary>
        /// <param name="value">The value of the successful result.</param>
        public Result(TValue? value)
        {
            Value = value;
            IsSuccess = true;
            Error = null;
            IsError = false;
        }

        /// <summary>
        /// Initializes a new failed result containing the specified error.
        /// </summary>
        /// <param name="error">The error of the failed result.</param>
        /// <exception cref="ArgumentNullException">Thrown if the error is null.</exception>
        public Result(TError error)
        {
            Error = error ?? throw new ArgumentNullException($"Result is created with a null error of type: {nameof(TError)}");
            Value = default;
            IsSuccess = false;
            IsError = true;
        }

        /// <summary>
        /// Creates a new successful result with the specified value.
        /// </summary>
        /// <param name="value">The value of the result.</param>
        /// <returns>A successful <see cref="Result{TValue, TError}"/>.</returns>
        public static Result<TValue, TError> From(TValue value) => new(value);

        /// <summary>
        /// Creates a new failed result with the specified error.
        /// </summary>
        /// <param name="error">The error of the result.</param>
        /// <returns>A failed <see cref="Result{TValue, TError}"/>.</returns>
        public static Result<TValue, TError> From(TError error) => new(error);
    }

    /// <summary>
    /// Represents a highly performant result object without a value, designed for void-returning operations.
    /// Avoids branch predictions and heap allocations.
    /// </summary>
    /// <typeparam name="TError">The type of the error (must be a reference type).</typeparam>
    public readonly struct VoidResult<TError> where TError : class
    {
        /// <summary>
        /// The error associated with the operation, if it failed.
        /// </summary>
        public readonly TError? Error;

        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public readonly bool IsSuccess;

        /// <summary>
        /// Indicates whether the operation resulted in an error.
        /// </summary>
        public readonly bool IsError;

        /// <summary>
        /// Initializes a new failed void result containing the specified error.
        /// </summary>
        /// <param name="error">The error of the failed result.</param>
        /// <exception cref="ArgumentNullException">Thrown if the error is null.</exception>
        public VoidResult(TError error)
        {
            Error = error ?? throw new ArgumentNullException($"VoidResult is created with a null error of type: {nameof(TError)}");
            IsSuccess = false;
            IsError = true;
        }

        /// <summary>
        /// Initializes a new successful void result.
        /// </summary>
        public VoidResult()
        {
            IsSuccess = true;
            Error = null;
            IsError = false;
        }

        /// <summary>
        /// Creates a new failed void result from the specified error.
        /// </summary>
        /// <param name="error">The error of the result.</param>
        /// <returns>A failed <see cref="VoidResult{TError}"/>.</returns>
        public static VoidResult<TError> From(TError error) => new(error);

        /// <summary>
        /// Creates a new failed void result from a failed <see cref="Result{TValue, TError}"/>.
        /// </summary>
        /// <typeparam name="TValue">The success value type of the original result.</typeparam>
        /// <param name="error">The failed result to convert.</param>
        /// <returns>A failed <see cref="VoidResult{TError}"/>.</returns>
        public static VoidResult<TError> FromErrorResult<TValue>(Result<TValue, TError> error) => new(error.Error!);

        /// <summary>
        /// Creates a new void result from a <see cref="Result{TValue, TError}"/>, preserving the error if present.
        /// </summary>
        /// <typeparam name="TValue">The success value type of the original result.</typeparam>
        /// <param name="result">The result to convert.</param>
        /// <returns>A successful <see cref="VoidResult{TError}"/> if the original result was successful, otherwise a failed <see cref="VoidResult{TError}"/>.</returns>
        public static VoidResult<TError> FromResult<TValue>(Result<TValue, TError> result) => result.IsError ? new VoidResult<TError>(result.Error!) : new VoidResult<TError>();
    }
}
