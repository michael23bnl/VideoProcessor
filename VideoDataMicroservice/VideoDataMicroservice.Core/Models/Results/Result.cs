using VideoDataMicroservice.VideoDataMicroservice.Core.Models.Errors;

namespace VideoDataMicroservice.VideoDataMicroservice.Core.Models.Results;

public class Result<T>
{

    private readonly T? _value;
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public T? Value
    {
        get
        {
            if (IsFailure)
                throw new InvalidOperationException("Отрицательный результат не может иметь значения");

            return _value!;
        }
        private init => _value = value;
    }

    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
        Error = Error.None;
    }

    private Result(Error error)
    {
        if (error == Error.None)
            throw new ArgumentException("Некорректная ошибка");
        
        IsSuccess = false;
        Error = error;
    }
    
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
}