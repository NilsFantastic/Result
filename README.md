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

## Performance

This library is designed with performance in mind, favoring value types and branch-predictable logic.  
The benchmark below compares common usage scenarios against exception handling and various result modeling approaches.

The results clearly show that:

- Using exceptions is extremely expensive (~5300 ns and 232 B allocated)
- Struct-based result types (`Result<T, TError>`) are virtually allocation-free and extremely fast
- Class-based variants are slightly more expensive, but still efficient
- Structs with error enums or small error structs are ideal in high-performance paths

### Benchmark Results (run via BenchmarkDotNet)

|                              Method |          Mean |       Error |      StdDev |        Median |   Gen0 | Allocated |
|------------------------------------|--------------:|------------:|------------:|--------------:|-------:|----------:|
|              ThrowTryCatchException | 5,315.9152 ns | 103.7833 ns | 115.3549 ns | 5,279.3991 ns | 0.1068 |     232 B |
|     GetStructSuccessWithErrorStruct |     0.1711 ns |   0.0191 ns |   0.0149 ns |     0.1703 ns |      - |         - |
|      GetStructSuccessWithErrorClass |     0.0128 ns |   0.0149 ns |   0.0139 ns |     0.0114 ns |      - |         - |
|      GetClassSuccessWithErrorStruct |     2.7295 ns |   0.0851 ns |   0.2040 ns |     2.6800 ns | 0.0115 |      24 B |
|       GetClassSuccessWithErrorClass |     2.7441 ns |   0.0848 ns |   0.2033 ns |     2.6830 ns | 0.0115 |      24 B |
| GetStructResultErrorWithErrorStruct |     5.4272 ns |   0.0325 ns |   0.0271 ns |     5.4187 ns |      - |         - |
|  GetStructResultErrorWithErrorClass |     4.7959 ns |   0.1298 ns |   0.1861 ns |     4.7810 ns | 0.0153 |      32 B |
|  GetErrorClassResultWithErrorStruct |     6.5227 ns |   0.1497 ns |   0.1327 ns |     6.5259 ns | 0.0153 |      32 B |
|   GetErrorClassResultWithErrorClass |     9.6314 ns |   0.2536 ns |   0.3795 ns |     9.5908 ns | 0.0268 |      56 B |
|                  GetStructErrorEnum |     0.0067 ns |   0.0064 ns |   0.0053 ns |     0.0075 ns |      - |         - |
|                   GetClassErrorEnum |     2.3523 ns |   0.0752 ns |   0.0666 ns |     2.3704 ns | 0.0115 |      24 B |
|             GetStructErrorException |     9.6623 ns |   0.2558 ns |   0.6079 ns |     9.4435 ns | 0.0650 |     136 B |
|                   TryGetClassResult |     4.5490 ns |   0.1538 ns |   0.3831 ns |     4.3843 ns | 0.0115 |      24 B |
|              GetStructResultWithInt |     0.0068 ns |   0.0100 ns |   0.0094 ns |     0.0016 ns |      - |         - |
|               GetClassResultWithInt |     3.1350 ns |   0.0931 ns |   0.1034 ns |     3.1271 ns | 0.0115 |      24 B |
|                     GetStructResult |     0.0012 ns |   0.0041 ns |   0.0038 ns |     0.0000 ns |      - |         - |
|                      GetClassResult |     4.4032 ns |   0.1506 ns |   0.1735 ns |     4.3936 ns | 0.0115 |      24 B |


### Interpreting the above Method Names

Each method name in the benchmark corresponds to a specific combination of:

- ✅ **Result representation** — `Result<T, TError>`, `SuccessResult<>`, `ErrorResult<>`, or `try-catch`
- ✅ **Error type** — `ErrorStruct<T>`, `ErrorClass<T>`, enum values (`ErrorType`), or exceptions
- ✅ **Success scenario** — success case (returns a value) or error case (returns an error)

Here's how to read them:

| Prefix / Pattern                            | Meaning                                                                 |
|---------------------------------------------|-------------------------------------------------------------------------|
| `ThrowTryCatchException`                    | Baseline using traditional exception handling                          |
| `GetStruct...`                              | Uses a struct-based `Result<T, TError>`                                 |
| `GetClass...`                               | Uses a class-based `SuccessResult<T, TError>` or `ErrorResult<T, TError>` |
| `...WithErrorStruct` / `...WithErrorClass`  | The error type is either a struct or class wrapper around `ErrorType`  |
| `GetStructErrorEnum` / `GetClassErrorEnum`  | Error value is a plain `enum` (`ErrorType`)                            |
| `GetStructErrorException`                  | Uses `ArgumentException` as the error type (struct-based result)       |
| `GetClassErrorException`                   | Same as above, but class-based result                                  |
| `Get...WithInt`                             | Success case with a simple integer as value                            |
| `Get...Result`                              | Success case with a reference type (`Foo`) as value                    |

This naming convention helps compare the cost of:
- Struct vs class-based models  
- Exception throwing vs result modeling  
- Lightweight errors (enums) vs heavier ones (classes, exceptions)  
- Value vs reference success payloads

In general:  
📈 **Shorter names = faster**  
📉 **More "Class", "Exception", or "Throw" = slower**

---

## License

MIT License © [Almost Fantastic AB](https://almostfantastic.com)
