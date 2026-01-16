namespace Shared.Result;

public sealed record Error(string Code, string Message)
{
    public static Error None => new(ErrorTypeConstant.None, string.Empty);
}