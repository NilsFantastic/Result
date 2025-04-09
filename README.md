# AlmostFantastic.Result

A highly-performant, zero-allocation result pattern implementation for C#.  
Encapsulates operation outcomes with optional success values and error information — all without relying on heap allocations or exception-based logic flows.

## Features

- ✅ Struct-based `Result<TValue, TError>` and `VoidResult<TError>` types  
- ✅ No heap allocations, ideal for high-performance applications  
- ✅ Avoids branch mispredictions  
- ✅ Error abstraction with rich metadata via `Error` and `ErrorType`  
- ✅ Extension methods for mapping, chaining, and handling  
- ✅ Async-compatible mapping  
- ✅ Optional catch block logic based on error type  

## Use Cases

- Functional-style error handling  
- High-performance APIs or libraries  
- Microservices that need clear result modeling  
- Replacing exception-based flows in performance-critical code  

---

## Getting Started

### Success Example

```csharp
Result<int, Error> result = 42.ToResult<int, Error>();

if (result.IsSuccess)
{
    Console.WriteLine($"Success: {result.Value}");
}
```

### Error Example

```csharp
var error = new Error(ErrorType.NotFound, 404, "Item not found");
Result<int, Error> result = error.ToResult<int, Error>();

if (result.IsError)
{
    Console.WriteLine($"Failed with: {result.Error.Message}");
}
```

---

## Types

### `Result<TValue, TError>`

Encapsulates an operation that returns a value on success, or an error on failure.

### `VoidResult<TError>`

Optimized for operations that don't return a value but still want to model success or failure explicitly.

### `Error`

A rich error class with metadata:

- `Id`: unique identifier  
- `Code`: numeric code  
- `Message`: developer or user-friendly message  
- `CaughtException`: optional attached exception  
- `ErrorType`: categorization (see below)  

### `ErrorType`

Enum-based error categories for structured diagnostics:

- `NotFound`  
- `Input`  
- `Unexpected`  
- `Unhandled`  
- `Conflict`  
- `RateLimit`  
- `Server`  
- `Authentication`  
- `Timeout`  
- `Dependency`  
- `BusinessLogic`  
- `NetworkFailure`  

---

## Extension Methods

Located in `ResultExtensions`:

- `ToResult()` – wraps value or error into result  
- `ToVoid()` – converts `Result<T>` to `VoidResult`  
- `Map()` – transforms success values  
- `MapAsync()` – async transformation of success values  
- `Catch()` – allows handling specific error types inline  
- `As()` – converts to new result type  
- `AsError()` – extracts error to different result type  
- `Pipe()` – fluent chaining of result transformations  

---

## Design Philosophy

This library is designed for systems where:

- Performance matters  
- You want to avoid exceptions as control flow  
- Clarity in handling success vs error is key  

It draws inspiration from functional programming models like Rust’s `Result<T, E>` or F#’s discriminated unions, but tailored for C# with performance in mind.

---

## License

MIT License © [Almost Fantastic AB](https://almostfantastic.com)
