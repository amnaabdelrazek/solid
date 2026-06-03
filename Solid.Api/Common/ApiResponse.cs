namespace Solid.Api.Common;

public static class ApiResponse
{
    public static IResult Ok(object? body = null, string? message = null)
    {
        return Results.Json(new
        {
            custom_code = 2000,
            status = true,
            message = message ?? "Data retrieved successfully.",
            body = body ?? new { },
            info = "from response action"
        }, statusCode: StatusCodes.Status200OK);
    }

    public static IResult Fail(string message, int code)
    {
        return Results.Json(new
        {
            custom_code = 2000,
            status = false,
            message,
            body = new { },
            info = "from response action"
        }, statusCode: code);
    }
}
