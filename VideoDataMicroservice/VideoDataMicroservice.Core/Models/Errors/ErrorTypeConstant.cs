namespace VideoDataMicroservice.VideoDataMicroservice.Core.Models.Errors;

public static class ErrorTypeConstant
{
    public const string None = "None";
    public const string ValidationError = "ValidationError";
    public const string UnAuthorized = "UnauthorizedError";
    public const string NotFound = "NotFoundError";
    public const string InternalServerError = "InternalServerError";
    public const string Forbidden = "ForbiddenError";
}