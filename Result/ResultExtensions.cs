namespace AlmostFantastic.Result
{
    public static class ResultExtensions
    {
        /// <summary>
        /// Converts a value into a successful <see cref="Result{TValue, TError}"/>.
        /// </summary>
        /// <param name="value">The value to wrap in a <see cref="Result{TValue, TError}"/>.</param>
        public static Result<TValue, TError> ToResult<TValue, TError>(this TValue value) where TError : class
        {
            return new Result<TValue, TError>(value);
        }

        /// <summary>
        /// Converts an error into a failed <see cref="Result{TValue, TError}"/>.
        /// </summary>
        /// <param name="error">The error to wrap in a <see cref="Result{TValue, TError}"/>.</param>
        public static Result<TValue, TError> ToResult<TValue, TError>(this TError error) where TError : class
        {
            return new Result<TValue, TError>(error);
        }

        /// <summary>
        /// Converts a <see cref="Result{TValue, TError}"/> to a <see cref="VoidResult{TError}"/>, discarding the value while preserving the error.
        /// </summary>
        /// <param name="result">The result to convert.</param>
        public static VoidResult<TError> ToVoid<TValue, TError>(this Result<TValue, TError> result) where TError : class
        {
            return result.IsError ? new VoidResult<TError>(result.Error) : new VoidResult<TError>();
        }

        /// <summary>
        /// Pipes the result through a transformation function, allowing for method chaining.
        /// </summary>
        /// <param name="resultToMap">The result to transform.</param>
        /// <param name="pipe">The function to apply.</param>
        public static Result<TSourceValue, TError> Pipe<TError, TSourceValue>(
            this Result<TSourceValue, TError> resultToMap,
            Func<Result<TSourceValue, TError>, Result<TSourceValue, TError>> pipe
        ) where TError : class
        {
            return pipe(resultToMap);
        }

        /// <summary>
        /// Transforms the success value of a result using a mapping function.
        /// </summary>
        /// <param name="resultToMap">The result to transform.</param>
        /// <param name="mapper">The function to apply to the success value.</param>
        public static Result<TDestinationValue, TError> Map<TDestinationValue, TError, TSourceValue>(
            this Result<TSourceValue, TError> resultToMap, Func<TSourceValue, TDestinationValue> mapper) where TError : class
        {
            return resultToMap.IsError ? new Result<TDestinationValue, TError>(resultToMap.Error!) : new Result<TDestinationValue, TError>(mapper(resultToMap.Value!));
        }

        /// <summary>
        /// Asynchronously transforms the success value of a result using a mapping function.
        /// </summary>
        /// <param name="resultToMap">The result to transform.</param>
        /// <param name="mapper">The asynchronous function to apply to the success value.</param>
        public static async Task<Result<TDestinationValue, TError>> MapAsync<TDestinationValue, TError, TSourceValue>(
            this Result<TSourceValue, TError> resultToMap, Func<TSourceValue, Task<TDestinationValue>> mapper) where TError : class
        {
            return resultToMap.IsError ? new Result<TDestinationValue, TError>(resultToMap.Error!) : new Result<TDestinationValue, TError>(await mapper(resultToMap.Value!));
        }

        /// <summary>
        /// Conditionally modifies a result's value if the error matches a specified type.
        /// </summary>
        /// <param name="resultToCatch">The result to check.</param>
        /// <param name="errorType">The error type to catch.</param>
        /// <param name="catchBlock">The function to apply if the error matches.</param>
        public static Result<TSourceValue, TError> Catch<TError, TSourceValue>(
            this Result<TSourceValue, TError> resultToCatch,
            ErrorType errorType,
            Func<TSourceValue, TSourceValue> catchBlock
        ) where TError : Error
        {
            return errorType == resultToCatch.Error?.ErrorType
                ? new Result<TSourceValue, TError>(catchBlock(resultToCatch.Value!))
                : resultToCatch;
        }

        /// <summary>
        /// Converts a result of one value type into a result of another value type, preserving errors.
        /// </summary>
        /// <param name="resultToConvert">The result to convert.</param>
        /// <param name="newValue">The new value to use if the original result was successful.</param>
        public static Result<TValue, TError> As<TValue, TError, TDiscardedValue>(
            this Result<TDiscardedValue, TError> resultToConvert,
            TValue newValue
        ) where TError : class
        {
            return resultToConvert.IsError
                ? new Result<TValue, TError>(resultToConvert.Error!)
                : new Result<TValue, TError>(newValue);
        }

        /// <summary>
        /// Converts an existing failed result into a failed result of a different success type.
        /// </summary>
        /// <typeparam name="TValue">The new success value type.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TDiscardedValue">The original success value type.</typeparam>
        /// <param name="resultToConvert">The result to convert.</param>
        /// <returns>A new <see cref="Result{TValue, TError}"/> with the same error.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the result is not an error.</exception>
        public static Result<TValue, TError> AsError<TValue, TError, TDiscardedValue>(
            this Result<TDiscardedValue, TError> resultToConvert) where TError : class
        {
            if (resultToConvert.IsError)
            {
                return new Result<TValue, TError>(resultToConvert.Error!);
            }

            throw new InvalidOperationException("The result is not an error.");
        }

    }
}
