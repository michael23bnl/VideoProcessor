using VideoMicroservice.VideoMicroservice.Core.Models.Errors;

namespace VideoMicroservice.VideoMicroservice.Core.Models.Results;

public class Result<TValue> : Result
{
    private readonly TValue _value;

    public Result(TValue value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }
    
    public TValue Value => 
        IsSuccess ? _value : 
            throw new InvalidOperationException("У отрицательного результата не может быть значения");
}